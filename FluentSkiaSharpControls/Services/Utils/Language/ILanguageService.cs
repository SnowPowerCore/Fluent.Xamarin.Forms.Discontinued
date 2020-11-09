using System.Globalization;

namespace FluentSkiaSharpControls.Services.Utils.Language
{
    public interface ILanguageService
    {
        string Current { get; }

        string Culture { get; }

        void DetermineAndSetLanguage();

        void SetLanguage(CultureInfo lang);
    }
}