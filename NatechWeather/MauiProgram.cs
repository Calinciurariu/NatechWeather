using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using NatechWeather.Helpers;
using NatechWeather.Interfaces;
using NatechWeather.Services;
using NatechWeather.ViewModels;
using NatechWeather.Views;
using SkiaSharp.Views.Maui.Controls.Hosting;
#if ANDROID
using NatechWeather.Platforms.Android.Services;
#elif IOS
using NatechWeather.Platforms.iOS.Services;
#endif
namespace NatechWeather
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .UseCustomNavigation()
                .RegisterAppServices()
                .RegisterViewModels()
                .RegisterViews()
                .MapViewMoldelsToPages()
                .UseSkiaSharp()
                .UseAudioHelper()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("FontAwesome6Brands.otf", "FontAwesomeBrands");
                    fonts.AddFont("FontAwesome6Duotone.otf", "FontAwesomeDuotone");
                    fonts.AddFont("FontAwesome6Light.otf", "FontAwesomeLight");
                    fonts.AddFont("FontAwesome6Regular.otf", "FontAwesomeRegular");
                    fonts.AddFont("FontAwesome6Solid.otf", "FontAwesomeSolid");
                    fonts.AddFont("FontAwesome6Thin.otf", "FontAwesomeThin");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
        public static MauiAppBuilder MapViewMoldelsToPages(this MauiAppBuilder mauiAppBuilder)
        {
            Initializer.RegisterViewModelToPage<MainPage, MainPageViewModel>();
            Initializer.RegisterViewModelToPage<WeatherPage, WeatherPageViewModel>();
            return mauiAppBuilder;
        }
        public static MauiAppBuilder RegisterAppServices(this MauiAppBuilder mauiAppBuilder)
        {
            mauiAppBuilder.Services.AddMemoryCache();
            mauiAppBuilder.Services.AddSingleton<ICacheService, CacheService>();
            mauiAppBuilder.Services.AddSingleton<HttpClient>();
            mauiAppBuilder.Services.AddSingleton<WeatherService>();
            //decorator
            mauiAppBuilder.Services.AddSingleton<IWeatherService>(provider =>
              new CachingWeatherService(
                  provider.GetRequiredService<WeatherService>(),   
                  provider.GetRequiredService<ICacheService>(),     
                  provider.GetRequiredService<ILogger<CachingWeatherService>>() 
              ));
            return mauiAppBuilder;
        }
        public static MauiAppBuilder RegisterViewModels(this MauiAppBuilder mauiAppBuilder)
        {
            mauiAppBuilder.Services.AddTransient<MainPageViewModel>();
            mauiAppBuilder.Services.AddTransient<WeatherPageViewModel>();
            return mauiAppBuilder;
        }
        public static MauiAppBuilder UseAudioHelper(this MauiAppBuilder mauiAppBuilder)
        {
            mauiAppBuilder.Services.AddSingleton<IAudioHelper, AudioHelper>();

            return mauiAppBuilder;
        }
        public static MauiAppBuilder RegisterViews(this MauiAppBuilder mauiAppBuilder)
        {
            mauiAppBuilder.Services.AddTransient<MainPage>();
            mauiAppBuilder.Services.AddTransient<WeatherPage>();
            return mauiAppBuilder;
        }
    }
}
