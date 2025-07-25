using NatechWeather.Interfaces;

namespace NatechWeather.ViewModels
{
    public partial class WeatherPageViewModel : BaseViewModel
    {
        public WeatherPageViewModel(INavigationPageService navigation) : base(navigation)
        {
        }
    }
}
