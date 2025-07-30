using NatechWeather.Helpers;
using NatechWeather.Interfaces;
using NatechWeather.Models;
using System.Windows.Input;
using Microsoft.Maui.Devices.Sensors;
using System.Collections.ObjectModel;

namespace NatechWeather.ViewModels
{
    public class MainPageViewModel : BaseViewModel
    {

        #region Services
        private readonly INavigationPageService navigationService;
        private readonly IWeatherService weatherService;
        #endregion

        #region Properties
        private string _city = string.Empty;
        public string City
        {
            get => _city;
            set => SetProperty(ref _city, value);
        }

        private WeatherResult _weatherResult = new();
        public WeatherResult WeatherResult
        {
            get => _weatherResult;
            set => SetProperty(ref _weatherResult, value);
        }
        public ObservableCollection<WeatherResult> WeatherForecastData { get; } = new ObservableCollection<WeatherResult>();

        #endregion

        #region Commands
        public ICommand GetWeatherCommand { get; }
        public ICommand GetWeatherInTenSecondsCommand { get; }
        public ICommand GetLocationCommand { get; }
        
        #endregion
        public MainPageViewModel(INavigationPageService navigation, IWeatherService weatherService) : base(navigation)
        {
            Title = "Natech Weather";
            navigationService = navigation;
            this.weatherService = weatherService;
            GetWeatherCommand = new AsyncRelayCommand(GetWeatherAsync);
            GetWeatherInTenSecondsCommand = new AsyncRelayCommand(GetWeatherAsync);
            GetLocationCommand = new AsyncRelayCommand(InputLocationAsync);
        }
        private async Task InputLocationAsync()
        {
            if (IsBusy) return;

            try
            {
                var location = await Geolocation.Default.GetLastKnownLocationAsync() ??
                               await Geolocation.Default.GetLocationAsync(new GeolocationRequest
                               {
                                   DesiredAccuracy = GeolocationAccuracy.Medium,
                                   Timeout = TimeSpan.FromSeconds(30)
                               });

                if (location == null)
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Could not retrieve your location.", "OK");
                    return;
                }

                var placemarks = await Geocoding.Default.GetPlacemarksAsync(location.Latitude, location.Longitude);
                var placemark = placemarks?.FirstOrDefault();

                if (placemark != null)
                {
                    City = placemark.Locality; 
                }
            }
            catch (FeatureNotSupportedException)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Geolocation is not supported on this device.", "OK");
            }
            catch (PermissionException)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Location permission is not granted. Please enable it in the app settings.", "OK");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"An unexpected error occurred: {ex.Message}", "OK");
            }
        }
        private async Task GetWeatherAsync()
        {
            if (IsBusy)
                return;
            IsBusy = true;
            if (string.IsNullOrWhiteSpace(City))
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Please enter a city name.", "OK");
                IsBusy = false;
                return;
            }
            try
            {
                WeatherResult = await weatherService.GetWeatherForCityAsync(City);
                WeatherForecastData.Clear();
                if (WeatherResult != null)
                {
                    WeatherForecastData.Add(WeatherResult);
                    WeatherForecastData.Add(new WeatherResult { Dt = WeatherResult.Dt + 3600, Main = new Main { Temp = WeatherResult.Main.Temp + 1 } });
                    WeatherForecastData.Add(new WeatherResult { Dt = WeatherResult.Dt + 7200, Main = new Main { Temp = WeatherResult.Main.Temp - 2 } });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching weather: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Error", "Failed to fetch weather data.", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
