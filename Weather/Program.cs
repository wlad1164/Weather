using Microsoft.EntityFrameworkCore;
using Weather.Models;

namespace Weather
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            // Add services to the container.
            //builder.Services.AddRazorPages();
            //
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            var serverVersion = new MySqlServerVersion(new Version(5, 7, 37, 0));
            builder.Services.AddDbContext<ApplicationContext>(
                options => options
                    .UseMySql(connectionString, serverVersion)
                    //.LogTo(Console.WriteLine, Microsoft.Extensions.Logging.LogLevel.Information)
                    .EnableSensitiveDataLogging()
                    .EnableDetailedErrors());
            //
            builder.Services.AddHostedService<Service.UpdateWeatherService>();
            //
            var app = builder.Build();
            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }
            //
            app.MapGet("/", () => "Hello World!");
            //
            app.MapGet("/api/add/{city}", (string city, ApplicationContext db) =>
            {
                Models.City? dbCity = db.Cities.FirstOrDefault(x => x.Name == city);
                if (dbCity != null) return Results.Problem("Данный город уже в списке парсера!");

                var parser = new ParserHTML();

                var rez = parser.GetCityWeatherAsync(city).Result;
                if (rez == null)
                    return Results.Problem("Не валидное название города");
                else
                {
                    db.Cities.Add((City)rez.City);
                    db.SaveChanges();
                    db.CityWeathers.Add(rez);
                    db.SaveChanges();
                    return Results.Ok($"Город {city} добавлен!");
                }
            });
            app.MapGet("/api/list", async (ApplicationContext db) => await db.Cities.ToListAsync());
            app.MapGet("/api/weather/{city}", async (string city, ApplicationContext db) =>
            {
                Models.City? dbCity = db.Cities.FirstOrDefault(x => x.Name == city);
                if (dbCity == null) return Results.Problem("По указанному городу сбор данных не ведется");
                int cityId = dbCity?.Id ?? 0;
                //
                Models.CityWeather? weather = db.CityWeathers.OrderBy(x => x.Date).LastOrDefault(x => x.CityId == cityId);
                if (weather == null) return Results.Problem("Нет данных о погоде, повторите запрос позже");

                return Results.Json(weather);
            });
            app.Run();
        }
    }
}