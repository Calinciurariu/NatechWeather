using NatechWeather.Helpers;
using NatechWeather.Interfaces;

namespace NatechWeather.Services
{
    public class NavigationPageService : INavigationPageService
    {
    
        public Task PopAsync()
        {
            return Application.Current.MainPage.Navigation.PopAsync(true);
        }

        public Task PopToRootAsync()
        {
            return Application.Current.MainPage.Navigation.PopToRootAsync();
        }

        Task INavigationPageService.NavigateToAsync<TViewModel>(string route, IDictionary<string, object> routeParameters)
        {
            var pageType = Initializer.GetPageTypeFromVM<TViewModel>();
            var currentPage = Application.Current.MainPage;
            var page = currentPage.Handler.MauiContext.Services.GetService(pageType) as Page;
            if (routeParameters != null && routeParameters.Count > 0)
                (page.BindingContext as IViewModel).SetParameters(routeParameters);

            return currentPage.Navigation.PushAsync(page);
        }

       
    }
}
