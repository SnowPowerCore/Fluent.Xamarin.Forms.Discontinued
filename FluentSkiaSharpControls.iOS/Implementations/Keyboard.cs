using UIKit;
using FluentSkiaSharpControls.Services.Interfaces;

namespace FluentSkiaSharpControls.iOS.Implementations
{
    public class Keyboard : IKeyboard
    {
        public void HideKeyboard() =>
            UIApplication.SharedApplication.KeyWindow.EndEditing(true);
    }
}