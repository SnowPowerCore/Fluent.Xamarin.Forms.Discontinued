using Foundation;
using Microsoft.Extensions.DependencyInjection;
using UIKit;
using FluentSkiaSharpControls.iOS.Implementations;
using FluentSkiaSharpControls.Services.Interfaces;

namespace FluentSkiaSharpControls.iOS
{
    [Register("AppDelegate")]
    public partial class AppDelegate : Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            Xamarin.Forms.Forms.Init();
            Xamarin.Forms.Nuke.FormsHandler.Init();
            Rg.Plugins.Popup.Popup.Init();
            XamEffects.iOS.Effects.Init();
            LoadApplication(Startup.Init(ConfigureServices));

            return base.FinishedLaunching(app, options);
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(typeof(ILocalizeService), typeof(LocalizeService));
            services.AddSingleton(typeof(IAppQuit), typeof(AppQuit));
            services.AddSingleton(typeof(IToast), typeof(Toast));
            services.AddSingleton(typeof(IKeyboard), typeof(Keyboard));
        }
    }
}
