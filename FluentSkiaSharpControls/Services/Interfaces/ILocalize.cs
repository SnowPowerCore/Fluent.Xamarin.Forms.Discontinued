using System;
using System.Globalization;

namespace FluentSkiaSharpControls.Services.Interfaces
{
    public interface ILocalizeService
    {
        CultureInfo GetCurrentCultureInfo();

        void SetLocale(CultureInfo ci);
    }

    public class PlatformCulture
    {
        public PlatformCulture(string platformCultureString)
        {
            if (string.IsNullOrEmpty(platformCultureString))
            {
                throw new ArgumentException("Expected culture identifier",
                    "platformCultureString"); // in C# 6 use nameof(platformCultureString)
            }

            PlatformString = platformCultureString.Replace("_", "-"); // .NET expects dash, not underscore
            var dashIndex = PlatformString.IndexOf("-", StringComparison.Ordinal);
            if (dashIndex > 0)
            {
                var parts = PlatformString.Split('-');
                LanguageCode = parts[0];
                LocaleCode = parts[1];
            }
            else
            {
                LanguageCode = PlatformString;
                LocaleCode = "";
            }
        }

        public string PlatformString { get; }
        public string LanguageCode { get; }
        public string LocaleCode { get; }

        public override string ToString()
        {
            return PlatformString;
        }
    }
}