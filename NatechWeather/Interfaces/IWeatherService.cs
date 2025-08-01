using NatechWeather.Models;

namespace NatechWeather.Interfaces
{
    public interface IWeatherService
    {
        Task<WeatherResult> GetWeatherForCityAsync(string city, CancellationToken cancellationToken = default);
        Task<OneCallResult> GetWeatherForLocationAsync(double latitude, double longitude, CancellationToken cancellationToken = default);

    }
}
