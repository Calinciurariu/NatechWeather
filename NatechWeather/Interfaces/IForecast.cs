using NatechWeather.Models;

namespace NatechWeather.Interfaces
{
    public interface IForecast
    {
        public long Dt { get; set; }

        public List<WeatherInfo> Weather { get; set; }
    }
}
