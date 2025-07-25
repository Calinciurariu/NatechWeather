using NatechWeather.Interfaces;
using NatechWeather.Models;

namespace NatechWeather.Services
{
    public partial class WeatherService : IWeatherService
    {
        private readonly HttpClient _httpClient;

        public WeatherService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<WeatherResult> GetWeatherForCityAsync(string city)
        {
            var url = $"https://api.openweathermap.org/data/2.5/weather?q={city}&appid={ApiKeyConstant}&units=metric";
            var response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return System.Text.Json.JsonSerializer.Deserialize<WeatherResult>(json);
            }
            return null;
        }

    }
}
