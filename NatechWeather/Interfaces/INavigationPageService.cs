namespace NatechWeather.Interfaces
{
    public interface INavigationPageService
    {

        Task NavigateToAsync<TViewModel>(string route, IDictionary<string, object> routeParameters = null)
            where TViewModel : class, IViewModel;
        
        Task PopAsync();

        Task PopToRootAsync();

    }
}
