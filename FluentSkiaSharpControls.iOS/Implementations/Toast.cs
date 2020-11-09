using Xamarin.Forms;
using FluentSkiaSharpControls.Services.Interfaces;
using FluentSkiaSharpControls.iOS.Controls;
using System;
using Xamarin.Forms.Platform.iOS;

namespace FluentSkiaSharpControls.iOS.Implementations
{
    public class Toast : IToast
    {
        public void ShowToast(string message) =>
            Device.InvokeOnMainThreadAsync(() =>
            {
                var snackbar = new SnackBar
                {
                    Message = message,
                    LeftMargin = 25f,
                    RightMargin = 25f,
                    BottomMargin = 25f,
                    BackgroundColor = Color.Gray.ToUIColor(),
                    Duration = TimeSpan.FromSeconds(3),
                    AnimationType = SnackbarAnimationType.FadeInFadeOut
                };
                snackbar.Show();
            });
    }
}