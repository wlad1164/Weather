namespace Weather.Service
{
    public class UpdateWeatherService : IHostedService, IDisposable
    {
        ParserHTML parser = new ParserHTML();
        private readonly ILogger<UpdateWeatherService> _logger;
        private Timer? _timer = null;
        private TimeSpan _updateTimeSpan = TimeSpan.FromHours(12);
        private ApplicationContext Context { get; }

        public UpdateWeatherService(ILogger<UpdateWeatherService> logger, IServiceScopeFactory factory)
        {
            this.Context = factory.CreateScope().ServiceProvider.GetRequiredService<ApplicationContext>();
            this._updateTimeSpan = factory.CreateScope().ServiceProvider.GetRequiredService<IConfiguration>().GetValue<TimeSpan>("UpdateDuration");
            _logger = logger;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Parse Weather Service running.");
            _timer = new Timer(DoWork, null, TimeSpan.Zero, _updateTimeSpan);
            return Task.CompletedTask;
        }

        private async void DoWork(object? state)
        {
            var CitesUpdate = Context.Cities.Where(c =>
                c.Weathers.Count == 0 || c.Weathers.OrderByDescending(d=> d.Date).First().Date <= DateTime.Now.AddMinutes(_updateTimeSpan.TotalMinutes * -1)) // timespan.Negate()
                .ToList(); 
            //
            if (CitesUpdate.Count > 0)
            {
                foreach (var city in CitesUpdate)
                {
                    if (parser.GetCityWeatherAsync(city.Name).Result is Models.CityWeather w && w != null)
                    {
                        city.Weathers.Add(w);
                        _logger.LogInformation($"{city.Name} update weather complit ({w.Temp})");
                    }
                    await Task.Delay(new Random().Next(1, 10) * 1000);
                }
                await Context.SaveChangesAsync();
            }
            _logger.LogInformation($"Парсер погоды: обновлено {CitesUpdate.Count} городов");
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Parse Weather Service is stopping.");
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
