using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NatechWeather.Interfaces;
using NatechWeather.Models;

namespace NatechWeather.Services
{
    public partial class WeatherService : IWeatherService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ICacheService _cacheService;

        private readonly ILogger<WeatherService> _logger;
        private readonly string _apiKey;
        private readonly string _baseUrl;
        private readonly string _oneCallBaseUrl;


        public WeatherService(HttpClient httpClient, IConfiguration configuration, ILogger<WeatherService> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _apiKey = WeatherService.ApiKeyConstant;
            _baseUrl = _configuration["OpenWeatherMap:BaseUrl"] ?? "https://api.openweathermap.org/data/2.5/weather";
            _oneCallBaseUrl = configuration["OpenWeatherMap:OneCallBaseUrl"] ?? "https://api.openweathermap.org/data/3.0/onecall";
        }

        public async Task<WeatherResult> GetWeatherForCityAsync(string city, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(city))
            {
                _logger.LogWarning("City parameter is null or empty.");
                throw new ArgumentException("City name cannot be null or empty.", nameof(city));
            }

            try
            {
                var url = $"{_baseUrl}?q={Uri.EscapeDataString(city)}&appid={_apiKey}&units=metric";
                _logger.LogInformation("Requesting weather data for city: {City}", city);

                var response = await _httpClient.GetAsync(url, cancellationToken).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
                var weatherResult = JsonSerializer.Deserialize<WeatherResult>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (weatherResult == null)
                {
                    _logger.LogError("Failed to deserialize weather data for city: {City}", city);
                    throw new InvalidOperationException("Failed to deserialize weather data.");
                }

                return weatherResult;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Failed to retrieve weather data for city: {City}", city);
                throw new WeatherServiceException("Network error occurred while fetching weather data.", ex);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to parse weather data for city: {City}", city);
                throw new WeatherServiceException("Invalid weather data format received.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while fetching weather data for city: {City}", city);
                throw new WeatherServiceException("An unexpected error occurred.", ex);
            }
        }

        public async Task<OneCallResult> GetWeatherForLocationAsync(double latitude, double longitude, CancellationToken cancellationToken = default)
        {
            try
            {
                var url = $"{_oneCallBaseUrl}?lat={latitude}&lon={longitude}&exclude=minutely,alerts&appid={_apiKey}&units=metric";
                _logger.LogInformation("Requesting One Call weather data for lat:{Lat}, lon:{Lon}", latitude, longitude);

                var response = await _httpClient.GetAsync(url, cancellationToken).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
                var weatherResult = JsonSerializer.Deserialize<OneCallResult>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (weatherResult == null)
                {
                    _logger.LogError("Failed to deserialize One Call weather data for lat:{Lat}, lon:{Lon}", latitude, longitude);
                    throw new WeatherServiceException("Failed to deserialize weather data.");
                }

                return weatherResult;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Failed to retrieve One Call weather data for lat:{Lat}, lon:{Lon}", latitude, longitude);
                throw new WeatherServiceException("Network error occurred while fetching weather data.", ex);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to parse One Call weather data for lat:{Lat}, lon:{Lon}", latitude, longitude);
                throw new WeatherServiceException("Invalid weather data format received.", ex);
            }
        }
    }

    public class WeatherServiceException : Exception
    {
        public WeatherServiceException(string message) : base(message) { }
        public WeatherServiceException(string message, Exception innerException) : base(message, innerException) { }
    }
}