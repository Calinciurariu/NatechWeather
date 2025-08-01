using NatechWeather.Helpers;
using NatechWeather.Interfaces;
using NatechWeather.Models;
using System.Windows.Input;
using NatechWeather.Services;

namespace NatechWeather.ViewModels
{

    public class MainPageViewModel : BaseViewModel
    {

        #region Services
        private readonly INavigationPageService navigationService;
        private readonly IWeatherService weatherService;
        private readonly IAudioHelper audioHelper;
        #endregion

        #region Properties
        private Placemark placemark;
        private string _city, lastPlacemarkString = string.Empty;
        public string City
        {
            get => _city;
            set => SetProperty(ref _city, value);
        }
        #endregion

        #region Commands
        public ICommand GetWeatherCommand { get; }
        public ICommand GetWeatherInTenSecondsCommand { get; }
        public ICommand GetLocationCommand { get; }
        
        #endregion
        public MainPageViewModel(INavigationPageService navigation, IWeatherService weatherService, IAudioHelper audioHelper) : base(navigation)
        {
            Title = "Natech Weather";
            navigationService = navigation;
            this.weatherService = weatherService;
            this.audioHelper = audioHelper;
            GetWeatherCommand = new AsyncRelayCommand(GetWeatherAsyncAndNavigate);
          //  GetWeatherInTenSecondsCommand = new AsyncRelayCommand(GetWeatherAsync);
            GetLocationCommand = new AsyncRelayCommand(InputLocationAsync);
        }
        private async Task InputLocationAsync()
        {
            if (IsBusy) return;
            IsBusy = true;
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
                placemark = placemarks?.FirstOrDefault();
                
                if (placemark != null)
                {
                    City = lastPlacemarkString = placemark.Locality??placemark.AdminArea??placemark.SubAdminArea??placemark.SubLocality; 
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
            finally
            {
                IsBusy = false;
            }
        }
        private async Task<OneCallResult> FetchWeather(Func<CancellationToken, Task<OneCallResult>> fetchAction)
        {
            IsBusy = true;

            try
            {
                var weatherData = await fetchAction(CancellationToken.None);

                if (string.IsNullOrWhiteSpace(City) && weatherData.Lat != 0)
                {
                    var placemarks = await Geocoding.Default.GetPlacemarksAsync(weatherData.Lat, weatherData.Lon);
                    City = placemarks?.FirstOrDefault()?.Locality ?? "Current Location";
                }

                await audioHelper.PlayAudioFileAsync("success.wav");
                return weatherData;
            }
            catch (WeatherServiceException)
            {
                await audioHelper.PlayAudioFileAsync("mistake.wav");
                throw;

            }
            catch (Exception)
            {
                await audioHelper.PlayAudioFileAsync("mistake.wav");
                throw;

            }
            finally
            {
                IsBusy = false;
            }
        }
        private async Task<Placemark> GetPlacemarkForCityAsync(string city)
        {
            try
            {
                var locations = await Geocoding.Default.GetLocationsAsync(city);
                var location = locations?.FirstOrDefault();

                if (location != null)
                {
                    var placemarks = await Geocoding.Default.GetPlacemarksAsync(location.Latitude, location.Longitude);
                    return placemarks?.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Geocoding failed: {ex.Message}");
            }
            return null;
        }
        private async Task GetWeatherAsyncAndNavigate()
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
                OneCallResult weatherData;
                if (City != lastPlacemarkString)
                {
                    var location = await GetPlacemarkForCityAsync(City);
                    placemark = location ?? throw new WeatherServiceException("Could not find the specified city.");
                }
               
                weatherData = await FetchWeather(token => weatherService.GetWeatherForLocationAsync(placemark.Location.Latitude, placemark.Location.Longitude, token));
                var paramsToPass = new Dictionary<string, object> { };
                paramsToPass.Add("Result", weatherData);
                paramsToPass.Add("Placemark", placemark);

                await Navigation.NavigateToAsync<WeatherPageViewModel>(nameof(WeatherPageViewModel), paramsToPass);

            }
            catch (WeatherServiceException ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Failed to fetch weather data: {ex.Message}", "OK");
                Console.WriteLine($"Error fetching weather: {ex.Message}");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
