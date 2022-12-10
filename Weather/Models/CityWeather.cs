using System.Text.Json.Serialization;

namespace Weather.Models
{
    public class CityWeather
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public decimal Temp { get; set; }

        public int? CityId { get; set; }
        [JsonIgnore]
        public virtual City City { get; set; }
    }
}
