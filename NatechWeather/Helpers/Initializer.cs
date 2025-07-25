using NatechWeather.Interfaces;
using NatechWeather.Services;

namespace NatechWeather.Helpers
{
    public static class Initializer
    {

        internal static Dictionary<Type, Type> ViewModelsMappedToPages { get; set; }

        public static void RegisterViewModelToPage<TPage, TViewModel>() where TPage : Page where TViewModel : class, IViewModel
        {

            if (ViewModelsMappedToPages == null)
                ViewModelsMappedToPages = new Dictionary<Type, Type>();

            if (ViewModelsMappedToPages.ContainsKey(typeof(TViewModel)))
                ViewModelsMappedToPages[typeof(TViewModel)] = typeof(TPage);
            else ViewModelsMappedToPages.Add(typeof(TViewModel), typeof(TPage));
        }

        internal static Type GetPageTypeFromVM<TViewModel>()
        {
            if (ViewModelsMappedToPages == null)
                throw new Exception("you must register any page");

            if (ViewModelsMappedToPages.ContainsKey(typeof(TViewModel)))
                return ViewModelsMappedToPages[typeof(TViewModel)];
            else throw new Exception($"{typeof(TViewModel)} not found");


        }
        public static MauiAppBuilder UseCustomNavigation(this MauiAppBuilder mauiAppBuilder)
        {
            mauiAppBuilder.Services.AddSingleton<INavigationPageService, NavigationPageService>();
            return mauiAppBuilder;
        }
    }
}
