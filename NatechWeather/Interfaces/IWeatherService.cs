using NatechWeather.Models;

namespace NatechWeather.Interfaces
{
    public interface IWeatherService
    {
        Task<WeatherResult> GetWeatherForCityAsync(string city);
    }
}
