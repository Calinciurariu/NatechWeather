using NatechCharts.Models;
using NatechWeather.Helpers;
using NatechWeather.Interfaces;
using NatechWeather.Models;
using SkiaSharp;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace NatechWeather.ViewModels
{
    public partial class WeatherPageViewModel : BaseViewModel
    {
        public ICommand CleanupCommand { get; }
        public ICommand SwitchHourlyCommand { get; }
        public ICommand SwitchDailyCommand { get; }
        
        private readonly INavigationPageService navigationService;
        private bool isHourly = true;
        public bool IsHourly
        {
            get => isHourly;
            set => SetProperty(ref isHourly, value);
        }
        private string _city = string.Empty;
        public string City
        {
            get => _city;
            set => SetProperty(ref _city, value);
        }
        private SKColor _color = new SKColor(81,43,212);
        public SKColor Color
        {
            get => _color;
            set => SetProperty(ref _color, value);
        }
        private CurrentWeather _currentWeather ;
        public CurrentWeather CurrentWeather
        {
            get => _currentWeather;
            set => SetProperty(ref _currentWeather, value);
        }
        public ObservableCollection<ChartDataPoint> WeatherForecastData { get; set; } = new();
        private OneCallResult result;
        private Placemark placemark;
        public WeatherPageViewModel(INavigationPageService navigation) : base(navigation)
        {
            navigationService = navigation;
            CleanupCommand = new RelayCommand(Cleanup);
            SwitchDailyCommand = new RelayCommand(SwitchDaily, canExecute=> !IsBusy);
            SwitchHourlyCommand = new RelayCommand(SwitchHourly, canExecute => !IsBusy);
            Color = SKColors.Red;
        }

        private void SwitchDaily()
        {
            IsBusy = true;
            try
            {
                WeatherForecastData.Clear();
                foreach (var item in result.Daily)
                {
                    var dateTime = DateTimeOffset.FromUnixTimeSeconds(item.Dt).LocalDateTime;
                    WeatherForecastData.Add(new ChartDataPoint { Date = dateTime, Val = item.Temp.Day });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SwitchDaily: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void SwitchHourly()
        {
            IsBusy = true;

            try
            {
                WeatherForecastData.Clear();
            foreach (var item in result.Hourly)
            {
                var dateTime = DateTimeOffset.FromUnixTimeSeconds(item.Dt).LocalDateTime;
                WeatherForecastData.Add(new ChartDataPoint { Date = dateTime, Val = item.Temp });
            }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SwitchDaily: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void Cleanup()
        {
            CurrentWeather = null;
            WeatherForecastData?.Clear();
            WeatherForecastData = new ObservableCollection<ChartDataPoint>();

            Color = SKColors.Transparent;
        }
        public override void SetParameters(IDictionary<string, object> routeParameters)
        {
            base.SetParameters(routeParameters);

            result = routeParameters["Result"] as OneCallResult;
            placemark = routeParameters["Placemark"] as Placemark;
            City = placemark.Locality;
            Title = $"Weather in {City}";
            CurrentWeather = result.Current;
            //if (result?.Hourly != null && IsHourly)
            //{
            //    SwitchHourly();
           
            //}
            //else if (result?.Daily != null && !IsHourly)
            //{
            //    SwitchDaily();
            //}

        }

    }
}
