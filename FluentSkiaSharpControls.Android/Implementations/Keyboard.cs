using Android.App;
using Android.Content;
using Android.Views.InputMethods;
using FluentSkiaSharpControls.Services.Interfaces;

namespace FluentSkiaSharpControls.Android.Implementations
{
    public class Keyboard : IKeyboard
    {
        public void HideKeyboard()
        {
            var context = Application.Context;
            var inputMethodManager = context.GetSystemService(Context.InputMethodService) as InputMethodManager;
            if (inputMethodManager != null && context is Activity)
            {
                var activity = context as Activity;
                var token = activity.CurrentFocus?.WindowToken;
                inputMethodManager.HideSoftInputFromWindow(token, HideSoftInputFlags.None);

                activity.Window.DecorView.ClearFocus();
            }
        }
    }
}