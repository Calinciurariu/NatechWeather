using NatechCharts.Controls;
using NatechCharts.Interfaces;
using NatechWeather.ViewModels;
namespace NatechWeather.Views;

public partial class WeatherPage : ContentPage
{
	public WeatherPage(WeatherPageViewModel viewModel)
	{
        BindingContext = viewModel;
		InitializeComponent();
        var lineSeries = new LineSeries
        {
            Label = "Temperature",
            XValuePath = "Date",
            YValuePath = "Val",
            BindingContext = viewModel

        };
        lineSeries.SetBinding(LineSeries.ColorProperty, new Binding("Color"));

        lineSeries.SetBinding(LineSeries.ItemsSourceProperty, new Binding("WeatherForecastData"));
        lineChart.Series = new List<ISeries> { lineSeries };
    }
    
    protected override void OnNavigatedFrom(NavigatedFromEventArgs args)
    {
        base.OnNavigatedFrom(args);
    }
}