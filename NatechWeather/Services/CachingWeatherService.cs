using Microsoft.Extensions.Logging;
using NatechWeather.Interfaces;
using NatechWeather.Models;

namespace NatechWeather.Services
{
    public class CachingWeatherService : IWeatherService
    {
        private readonly IWeatherService _decorated;
        private readonly ICacheService _cacheService;
        private readonly ILogger<CachingWeatherService> _logger;
        public CachingWeatherService(
          WeatherService decorated,
          ICacheService cacheService,
          ILogger<CachingWeatherService> logger)
        {
            _decorated = decorated;
            _cacheService = cacheService;
            _logger = logger;
        }

        public async Task<WeatherResult> GetWeatherForCityAsync(string city, CancellationToken cancellationToken = default)
        {
            string cacheKey = $"weather_city_{city.ToLowerInvariant()}";
            var cachedResult = await _cacheService.GetAsync<WeatherResult>(cacheKey);

            if (cachedResult != null)
            {
                _logger.LogInformation("Cache HIT for city: {City}", city);
                return cachedResult;
            }

            _logger.LogInformation("Cache MISS for city: {City}. Fetching from API.", city);
            var result = await _decorated.GetWeatherForCityAsync(city, cancellationToken);
            await _cacheService.SetAsync(cacheKey, result);

            return result;
        }

        public async Task<OneCallResult> GetWeatherForLocationAsync(double latitude, double longitude, CancellationToken cancellationToken = default)
        {
            string cacheKey = $"weather_onecall_loc_{latitude:F2}_{longitude:F2}";
            var cachedResult = await _cacheService.GetAsync<OneCallResult>(cacheKey);

            if (cachedResult != null)
            {
                _logger.LogInformation("Cache HIT for location: Lat={Lat}, Lon={Lon}", latitude, longitude);
                return cachedResult;
            }

            _logger.LogInformation("Cache MISS for location: Lat={Lat}, Lon={Lon}. Fetching from API.", latitude, longitude);
            var result = await _decorated.GetWeatherForLocationAsync(latitude, longitude, cancellationToken);
            await _cacheService.SetAsync(cacheKey, result);

            return result;
        }
    }
}
