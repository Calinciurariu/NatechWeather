using NatechWeather.Helpers;
using NatechWeather.Interfaces;
using NatechWeather.Models;
using System.Windows.Input;
using Microsoft.Maui.Devices.Sensors;

namespace NatechWeather.ViewModels
{
    public partial class MainPageViewModel : BaseViewModel
    {

        #region Services
        private readonly INavigationPageService navigationService;
        private readonly IWeatherService weatherService;
        #endregion

        #region Properties
        private string _city;
        public string City
        {
            get => _city;
            set => SetProperty(ref _city, value);
        }

        private WeatherResult _weatherResult;
        public WeatherResult WeatherResult
        {
            get => _weatherResult;
            set => SetProperty(ref _weatherResult, value);
        }
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
                    await Shell.Current.DisplayAlert("Error", "Could not retrieve your location.", "OK");
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
                await Shell.Current.DisplayAlert("Error", "Geolocation is not supported on this device.", "OK");
            }
            catch (PermissionException)
            {
                await Shell.Current.DisplayAlert("Error", "Location permission is not granted. Please enable it in the app settings.", "OK");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"An unexpected error occurred: {ex.Message}", "OK");
            }
        }
        private async Task GetWeatherAsync()
        {
            if (IsBusy)
                return;
            IsBusy = true;
            if (string.IsNullOrWhiteSpace(City))
            {
                await Shell.Current.DisplayAlert("Error", "Please enter a city name.", "OK");
                IsBusy = false;
                return;
            }
            try
            {
                WeatherResult = await weatherService.GetWeatherForCityAsync(City);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching weather: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
