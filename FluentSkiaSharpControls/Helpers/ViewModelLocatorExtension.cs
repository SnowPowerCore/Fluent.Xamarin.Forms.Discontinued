using System;
using Xamarin.Forms.Xaml;

namespace FluentSkiaSharpControls.Helpers
{
    public class ViewModelLocatorExtension : IMarkupExtension
    {
        public Type ViewModelType { get; set; }

        public object ProvideValue(IServiceProvider serviceProvider) =>
            null != ViewModelType
                ? App.Services.GetService(ViewModelType)
                : throw new Exception($"Couldn't locate viewmodel of type { ViewModelType }.");
    }
}