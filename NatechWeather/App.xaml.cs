using NatechWeather.ViewModels;
using NatechWeather.Views;

namespace NatechWeather
{
    public partial class App : Application
    {
        public App(MainPageViewModel vm)
        {
            InitializeComponent();
           // Application.Current.UserAppTheme = AppTheme.Dark; dark theme test
            MainPage = new NavigationPage(new MainPage(vm));
        }
    }
}
