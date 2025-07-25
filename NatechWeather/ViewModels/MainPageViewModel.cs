using NatechWeather.Helpers;
using NatechWeather.Interfaces;
using System.Windows.Input;

namespace NatechWeather.ViewModels
{
    public partial class MainPageViewModel : BaseViewModel
    {
        #region Services
        private readonly INavigationPageService navigationService;
        #endregion
        #region Commands
        public ICommand GetWeatherCommand { get; }
        #endregion
        public MainPageViewModel(INavigationPageService navigation) : base(navigation)
        {
            navigationService = navigation;
            Title = "Natech Weather";
            GetWeatherCommand = new AsyncRelayCommand(GetWeatherAsync);
        }
        private async Task GetWeatherAsync()
        {
            if (IsBusy)
                return;
            IsBusy = true;
            try
            {
                await Task.Delay(1000);
            }
            catch (Exception ex)
            {
                // todo: log and popup
                Console.WriteLine($"Error fetching weather: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
