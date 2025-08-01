using NatechWeather.ViewModels;

namespace NatechWeather.Views
{
    public partial class MainPage : ContentPage
    {
        public MainPage(MainPageViewModel vm)
        {
            BindingContext = vm;
            InitializeComponent();
        }
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            var statusRead = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
            LocationInputView.IsEnabledLocation = statusRead == PermissionStatus.Granted;
        }
    }
}
