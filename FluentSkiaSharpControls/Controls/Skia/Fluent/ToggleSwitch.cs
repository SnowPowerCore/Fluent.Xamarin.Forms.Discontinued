using FluentSkiaSharpControls.Effects;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;

namespace FluentSkiaSharpControls.Controls.Skia.Fluent
{
    /// <summary>
    /// Fluent-design toggle switch, inspired by UWP platform
    /// </summary>
    public class ToggleSwitch : ContentView
    {
        private TouchEffect _touchEffect;
        private ToggleSwitchControl _toggleSwitchControl;

        protected override SizeRequest OnMeasure(double widthConstraint, double heightConstraint)
        {
            return new SizeRequest
            {
                Request = new Size { Height = 30.0, Width = 60.0 },
                Minimum = new Size { Height = 30.0, Width = 60.0 }
            };
        }

        public ToggleSwitch()
        {
            _touchEffect = new TouchEffect { Capture = true };
            _touchEffect.TouchAction += HandleTouch;
            Effects.Add(_touchEffect);
            Content = _toggleSwitchControl = new ToggleSwitchControl();
        }

        ~ToggleSwitch()
        {
            Effects?.Clear();
            if (_touchEffect != null)
            {
                _touchEffect.TouchAction -= HandleTouch;
                _touchEffect = null;
            }
        }

        private void HandleTouch(object sender, TouchActionEventArgs args) =>
            _toggleSwitchControl.HandleTouch(sender, args);

        private class ToggleSwitchControl : SKCanvasView
        {
            private bool _isHoveredOver = false;

            private SKPaint _borderNormalPaint = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                Color = SKColor.Parse("#999999"),
                StrokeWidth = 3,
                FilterQuality = SKFilterQuality.High
            };

            private SKPaint _borderHoverPaint = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                Color = SKColor.Parse("#CCCCCC"),
                StrokeWidth = 3,
                FilterQuality = SKFilterQuality.High
            };

            private SKPaint BorderPaint =>
                _isHoveredOver ? _borderHoverPaint : _borderNormalPaint;

            protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
            {
                base.OnPaintSurface(e);

                var canvas = e.Surface.Canvas;
                canvas.Clear();

                int width = e.Info.Width;
                int height = e.Info.Height;

                //var backPaint = new SKPaint
                //{
                //    Style = SKPaintStyle.Fill,
                //    Color = SKColors.Red,
                //};

                //canvas.DrawRect(new SKRect(0, 0, width, height), backPaint);

                canvas.DrawRoundRect(new SKRoundRect(new SKRect(3, 3, width-3, height-3), 33), BorderPaint);

                canvas.Save();
            }

            public void HandleTouch(object sender, TouchActionEventArgs args)
            {
                var pt = args.Location;
                var point =
                    new SKPoint((float)(CanvasSize.Width * pt.X / Width),
                                (float)(CanvasSize.Height * pt.Y / Height));
                switch (args.Type)
                {
                    case TouchActionType.Entered:
                        _isHoveredOver = true;
                        InvalidateSurface();
                        break;
                    case TouchActionType.Exited:
                        _isHoveredOver = false;
                        InvalidateSurface();
                        break;
                }
            }

            protected override void OnPropertyChanged(string propertyName = null)
            {
                base.OnPropertyChanged(propertyName);
            }
        }
    }
}