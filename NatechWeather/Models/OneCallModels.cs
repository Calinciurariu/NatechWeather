using NatechWeather.Interfaces;
using System.Text.Json.Serialization;

namespace NatechWeather.Models
{
    public class GeoResult
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("lat")]
        public double Lat { get; set; }

        [JsonPropertyName("lon")]
        public double Lon { get; set; }

        [JsonPropertyName("country")]
        public string Country { get; set; }
    }


    public class OneCallResult
    {
        [JsonPropertyName("lat")]
        public double Lat { get; set; }

        [JsonPropertyName("lon")]
        public double Lon { get; set; }

        [JsonPropertyName("timezone")]
        public string Timezone { get; set; }

        [JsonPropertyName("current")]
        public CurrentWeather Current { get; set; }

        [JsonPropertyName("hourly")]
        public List<HourlyForecast> Hourly { get; set; }

        [JsonPropertyName("daily")]
        public List<DailyForecast> Daily { get; set; }
    }

    public class WeatherInfo
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("main")]
        public string Main { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("icon")]
        public string Icon { get; set; }
    }

    public class CurrentWeather
    {
        [JsonPropertyName("dt")]
        public long Dt { get; set; }

        [JsonPropertyName("temp")]
        public double Temp { get; set; }

        [JsonPropertyName("feels_like")]
        public double FeelsLike { get; set; }

        [JsonPropertyName("humidity")]
        public int Humidity { get; set; }

        [JsonPropertyName("weather")]
        public List<WeatherInfo> Weather { get; set; }
    }

    public class HourlyForecast : IForecast
    {
        [JsonPropertyName("dt")]
        public long Dt { get; set; }

        [JsonPropertyName("temp")]
        public double Temp { get; set; }

        [JsonPropertyName("weather")]
        public List<WeatherInfo> Weather { get; set; }
    }

    public class DailyForecast : IForecast
    {
        [JsonPropertyName("dt")]
        public long Dt { get; set; }

        [JsonPropertyName("temp")]
        public DailyTemp Temp { get; set; }

        [JsonPropertyName("weather")]
        public List<WeatherInfo> Weather { get; set; }
    }

    public class DailyTemp
    {
        [JsonPropertyName("day")]
        public double Day { get; set; }

        [JsonPropertyName("min")]
        public double Min { get; set; }

        [JsonPropertyName("max")]
        public double Max { get; set; }
    }
}
