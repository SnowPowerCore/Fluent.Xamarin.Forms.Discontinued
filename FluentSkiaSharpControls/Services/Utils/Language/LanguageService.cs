using System.Collections.Generic;
using System.Globalization;
using FluentSkiaSharpControls.Resources;
using FluentSkiaSharpControls.Services.Interfaces;
using FluentSkiaSharpControls.Services.Utils.Settings;

namespace FluentSkiaSharpControls.Services.Utils.Language
{
    public class LanguageService : ILanguageService
    {
        private static readonly string _default = "en";
        private readonly ISettingsService _settings;
        private readonly ILocalizeService _nativeLanguage;

        private static Dictionary<string, string> LanguageCulturePairs => new Dictionary<string, string>()
        {
            { "en", "en-US" },
            { "ru", "ru-RU" }
        };

        public string Current { get; private set; } = _default;

        public string Culture => GetCultureByLanguage(Current);

        public LanguageService(ISettingsService settings,
                               ILocalizeService nativeLanguage)
        {
            _settings = settings;
            _nativeLanguage = nativeLanguage;
        }

        public void DetermineAndSetLanguage()
        {
            var ci = _settings.ContainsKey("LanguageSet")
                ? new CultureInfo(_settings.GetValueOrDefault<string>("Language"))
                : CultureInfo.InstalledUICulture;
            SetLanguage(ci);
        }

        public void SetLanguage(CultureInfo lang)
        {
            Current = lang.TwoLetterISOLanguageName;
            AppResources.Culture = lang;
            _settings.AddOrUpdateValueAsync("Language", lang.TwoLetterISOLanguageName);
            _settings.AddOrUpdateValueAsync("LanguageSet", true);
            _nativeLanguage.SetLocale(lang);
        }

        private string GetCultureByLanguage(string lang)
        {
            if (LanguageCulturePairs.ContainsKey(lang)) return LanguageCulturePairs[lang];
            else return LanguageCulturePairs[_default];
        }
    }
}