using NatechWeather.Interfaces;
using NatechWeather.Models;

namespace NatechWeather.Services
{
    public class MockWeatherService : IWeatherService
    {
        public Task<WeatherResult> GetWeatherForCityAsync(string city)
        {
            return Task.FromResult(new WeatherResult
            {
                Name = city, 
                Main = new Main
                {
                    Temp = 15.5,
                    FeelsLike = 14.9,
                    TempMin = 14.0,
                    TempMax = 17.0,
                    Humidity = 75,
                    Pressure = 1012
                },
                Weather = new List<Weather>
                {
                    new Weather
                    {
                        Main = "Clouds",
                        Description = "broken clouds",
                        Icon = "04d"
                    }
                },
                Wind = new Wind
                {
                    Speed = 5.5
                },
                Sys = new Sys
                {
                    Country = "RO"
                }
            }
);
        }

        public Task<WeatherResult> GetWeatherForCityAsync(string city, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<OneCallResult> GetWeatherForLocationAsync(double latitude, double longitude, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
