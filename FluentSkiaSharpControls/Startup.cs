using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Reflection;
using Xamarin.Forms;
using FluentSkiaSharpControls.Services.Utils.Analytics;
using FluentSkiaSharpControls.Services.Utils.Language;
using FluentSkiaSharpControls.Services.Utils.Message;
using FluentSkiaSharpControls.Services.Utils.Navigation;
using FluentSkiaSharpControls.Services.Utils.Settings;
using FluentSkiaSharpControls.ViewModels;
using FluentSkiaSharpControls.Views.Pages;
using FluentSkiaSharpControls.Views.Shell;

namespace FluentSkiaSharpControls
{
    /// <summary>
    /// We create application and all deps throughout this class
    /// </summary>
    public static class Startup
    {
        /// <summary>
        /// Use this method to initialize application
        /// </summary>
        /// <param name="nativeConfigureServices">Native services' configure callback</param>
        /// <returns>Application</returns>
        public static App Init(Action<IServiceCollection> nativeConfigureServices)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var assemblyName = assembly.GetName();
#if RELEASE
            using (var stream = assembly
                .GetManifestResourceStream($"{assemblyName.Name}.Configuration.appsettings.Release.json"))
#else
            using (var stream = assembly
                .GetManifestResourceStream($"{assemblyName.Name}.Configuration.appsettings.Debug.json"))
#endif
            {
                var host = new HostBuilder()
                    .UseContentRoot(Directory.GetCurrentDirectory())
                    .UseDefaultServiceProvider((context, options) =>
                    {
                        var isDevelopment = context.HostingEnvironment.IsDevelopment();
                        options.ValidateScopes = isDevelopment;
                        options.ValidateOnBuild = isDevelopment;
                    })
                    .ConfigureHostConfiguration(c =>
                    {
                        c.AddJsonStream(stream);
                    })
                    .ConfigureServices(x =>
                    {
                        nativeConfigureServices(x);
                        ConfigureServices(x);
                        RegisterRoutes();
                    })
#if DEBUG
                    .ConfigureLogging(logging =>
                    {
                        logging.ClearProviders();
                        logging.AddConsole();
                    })
#endif
                    .Build();

                App.Services = host.Services;

                return App.Services.GetService<App>();
            }
        }

        /// <summary>
        /// Crossplatform services configuration
        /// </summary>
        /// <param name="services">Services collection</param>
        private static void ConfigureServices(IServiceCollection services)
        {
            #region Services
            services.AddSingleton<IAnalyticsService, AnalyticsService>();
            services.AddSingleton<ILanguageService, LanguageService>();
            services.AddSingleton<IMessageService, MessageService>();
            services.AddSingleton<INavigationService, ShellNavigationService>();
            services.AddSingleton<ISettingsService, SettingsService>();
            #endregion

            #region ViewModels
            services.AddSingleton<WelcomeViewModel>();
            #endregion

            #region Application
            services.AddSingleton<App>();
            #endregion
        }

        /// <summary>
        /// Registers routes for navigation
        /// </summary>
        public static void RegisterRoutes()
        {
            //Routes
            Routing.RegisterRoute("mainPage", typeof(AppShell));
            Routing.RegisterRoute("welcomePage", typeof(WelcomePage));
        }
    }
}