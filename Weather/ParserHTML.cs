using System.Net;
using HtmlAgilityPack;

namespace Weather
{
    public class ParserHTML
    {
        private static readonly WebClient web = new WebClient();
        //
        public async Task<Models.CityWeather> GetCityWeatherAsync(string cityName)
        {
            try
            {
                HtmlDocument doc = new HtmlDocument();
                var htmlData = web.DownloadString($"https://world-weather.ru/pogoda/russia/{cityName}/");
                doc.LoadHtml(htmlData);
                //
                int temp;
                int.TryParse(string.Join("", doc.GetElementbyId("weather-now-number").InnerText.Where(c => char.IsDigit(c) || c == '-' || c == '+')), out temp);
                //
                Models.City? city = new Models.City() { Name = cityName };
                Models.CityWeather? weather = new Models.CityWeather() { City = city, Date = DateTime.Now, Temp = temp };
                return weather;
            }
            catch (WebException wex)
            {
                Console.WriteLine($"({wex.Status}){wex.Message}");
                return null;
            }
        }
    }
}
