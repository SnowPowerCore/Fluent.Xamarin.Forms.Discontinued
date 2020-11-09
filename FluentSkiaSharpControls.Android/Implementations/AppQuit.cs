using Android.App;
using FluentSkiaSharpControls.Services.Interfaces;

namespace FluentSkiaSharpControls.Android.Implementations
{
    public class AppQuit : IAppQuit
    {
        public void Quit()
        {
            ((Activity)Xamarin.Forms.Forms.Context).FinishAffinity();
        }
    }
}