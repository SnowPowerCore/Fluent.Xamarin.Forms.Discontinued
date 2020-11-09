using System;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;
using FluentSkiaSharpControls.Services.Utils.Analytics;
using FluentSkiaSharpControls.Services.Utils.Language;
using FluentSkiaSharpControls.Services.Utils.Navigation;
using Application = Xamarin.Forms.Application;

namespace FluentSkiaSharpControls
{
    public partial class App : Application
    {
        #region Fields
        private ILanguageService _language;
        private INavigationService _navigation;
        private IAnalyticsService _analytics;
        #endregion

        #region Properties
        public static IServiceProvider Services { get; set; }
        #endregion

        #region Constructor
        public App()
        {
            InitializeComponent();
            Current
                .On<Android>()
                .UseWindowSoftInputModeAdjust(WindowSoftInputModeAdjust.Resize);

            InitApp();
        }
        #endregion

        #region Methods
        private void InitApp()
        {
            _language = (ILanguageService)Services.GetService(typeof(ILanguageService));
            _navigation = (INavigationService)Services.GetService(typeof(INavigationService));
            _analytics = (IAnalyticsService)Services.GetService(typeof(IAnalyticsService));
        }

        protected override void OnStart()
        {
            base.OnStart();

            _language.DetermineAndSetLanguage();
            _navigation.DetermineAndSetMainPage("mainPage");
            _analytics.TrackEvent("App started.");
        }
        #endregion
    }
}
