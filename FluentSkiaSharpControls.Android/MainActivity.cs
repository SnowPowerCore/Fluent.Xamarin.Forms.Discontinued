using Android.App;
using Android.Content.PM;
using Android.OS;
using Microsoft.Extensions.DependencyInjection;
using FluentSkiaSharpControls.Android.Implementations;
using FluentSkiaSharpControls.Services.Interfaces;
using AGlide = Android.Glide;

namespace FluentSkiaSharpControls.Android
{
    [Activity(Label = "App", Icon = "@mipmap/icon", Theme = "@style/MainTheme",
        MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            Xamarin.Forms.Forms.Init(this, savedInstanceState);
            AGlide.Forms.Init(this);
            XamEffects.Droid.Effects.Init();
            Rg.Plugins.Popup.Popup.Init(this, savedInstanceState);

            LoadApplication(Startup.Init(ConfigureServices));
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(typeof(ILocalizeService), typeof(LocalizeService));
            services.AddSingleton(typeof(IAppQuit), typeof(AppQuit));
            services.AddSingleton(typeof(IToast), typeof(Toast));
            services.AddSingleton(typeof(IKeyboard), typeof(Keyboard));
        }

        public override void OnBackPressed() =>
            Rg.Plugins.Popup.Popup.SendBackPressed(base.OnBackPressed);
    }
}