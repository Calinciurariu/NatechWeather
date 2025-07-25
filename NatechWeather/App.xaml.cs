using NatechWeather.Views;

namespace NatechWeather
{
    public partial class App : Application
    {
        public App(MainPage page)
        {
            InitializeComponent();

            MainPage = new NavigationPage(page);
        }
    }
}
