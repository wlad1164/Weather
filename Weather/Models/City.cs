using System.Numerics;
using System.Text.Json.Serialization;

namespace Weather.Models
{
    public class City
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        [JsonIgnore]
        public virtual ICollection<CityWeather> Weathers { get; set; }
        public City()
        {
            Weathers = new List<CityWeather>();
        }
    }
}
