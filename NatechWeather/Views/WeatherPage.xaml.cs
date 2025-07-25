using NatechWeather.ViewModels;

namespace NatechWeather.Views;

public partial class WeatherPage : ContentPage
{
	public WeatherPage(WeatherPageViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;
    }
}