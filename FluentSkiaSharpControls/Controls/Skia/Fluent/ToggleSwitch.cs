using FluentSkiaSharpControls.Effects;
using FluentSkiaSharpControls.Utils;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using Xamarin.Forms;

namespace FluentSkiaSharpControls.Controls.Skia.Fluent
{
    /// <summary>
    /// Fluent-design toggle switch, inspired by UWP platform
    /// </summary>
    public class ToggleSwitch : ContentView
    {
        private TouchEffect _touchEffect;
        private readonly ToggleSwitchControl _toggleSwitchControl;

        public static readonly BindableProperty IsOnProperty =
            BindableProperty.Create(nameof(IsOn), typeof(bool),
                typeof(ToggleSwitch), defaultValue: false,
                defaultBindingMode: BindingMode.TwoWay,
                propertyChanged: OnToggled);

        public bool IsOn
        {
            get => (bool)GetValue(IsOnProperty);
            set => SetValue(IsOnProperty, value);
        }

        public ToggleSwitch()
        {
            _touchEffect = new TouchEffect { Capture = true };
            _touchEffect.TouchAction += HandleTouch;
            Effects.Add(_touchEffect);
            Content = _toggleSwitchControl =
                new ToggleSwitchControl { UpdateActivePropertyFromControl = UpdateIsOnProperty };
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

        protected override SizeRequest OnMeasure(double widthConstraint, double heightConstraint)
        {
            var size = new SizeRequest
            {
                Request = new Size { Height = heightConstraint, Width = widthConstraint },
                Minimum = new Size { Height = 30.0, Width = 60.0 }
            };

            if (widthConstraint > heightConstraint / 2)
                size = new SizeRequest
                {
                    Request = new Size { Height = size.Request.Height, Width = heightConstraint / 2 },
                    Minimum = new Size { Height = 30.0, Width = 60.0 }
                };

            if (heightConstraint > widthConstraint / 2)
                size = new SizeRequest
                {
                    Request = new Size { Height = widthConstraint / 2, Width = size.Request.Width },
                    Minimum = new Size { Height = 30.0, Width = 60.0 }
                };

            return size;
        }

        private void HandleTouch(object sender, TouchActionEventArgs args) =>
            _toggleSwitchControl.HandleTouch(sender, args);

        private static void OnToggled(BindableObject bindable, object oldValue, object newValue)
        {
            if (oldValue != newValue && null != newValue)
            {
                var control = (ToggleSwitch)bindable;
                control._toggleSwitchControl.UpdateActivated((bool)newValue);
            }
        }

        private void UpdateIsOnProperty(bool value) =>
            IsOn = value;

        private class ToggleSwitchControl : SKCanvasView
        {
            private bool _isInit = true;
            private bool _isPressed = false;
            private bool _isMoving = false;

            private SKPoint _pressedPoint = SKPoint.Empty;

            private Selector _selector;

            private SKPaint _borderNormalPaint = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                Color = SKColor.Parse("#999999"),
                StrokeWidth = 3,
                FilterQuality = SKFilterQuality.High,
                IsAntialias = true
            };

            private SKPaint _bgNormalPaint = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                Color = SKColors.Transparent,
                FilterQuality = SKFilterQuality.High
            };

            private SKPaint _borderPressedPaint = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                Color = SKColors.WhiteSmoke,
                StrokeWidth = 3,
                FilterQuality = SKFilterQuality.High,
                IsAntialias = true
            };

            private SKPaint _bgPressedPaint = new SKPaint
            {
                Style = SKPaintStyle.StrokeAndFill,
                Color = SKColors.White.WithAlpha((byte)(0xFF * 0.4)),
                StrokeWidth = 3,
                FilterQuality = SKFilterQuality.High,
                IsAntialias = true
            };

            private SKPaint BorderPaint
            {
                get
                {
                    if (Activated)
                        return _isPressed
                            ? new SKPaint
                            {
                                Style = SKPaintStyle.StrokeAndFill,
                                Color = ToggleBackgroundColor.ChangeColorBrightness(-0.25f).ToSKColor(),
                                StrokeWidth = 3,
                                FilterQuality = SKFilterQuality.High,
                                IsAntialias = true
                            }
                            : new SKPaint
                            {
                                Style = SKPaintStyle.StrokeAndFill,
                                Color = ToggleBackgroundColor.ToSKColor(),
                                StrokeWidth = 3,
                                FilterQuality = SKFilterQuality.High,
                                IsAntialias = true
                            };
                    else
                        return _isPressed ? _borderPressedPaint : _borderNormalPaint;
                }
            }

            private SKPaint BgPaint
            {
                get
                {
                    if (Activated)
                        return _isPressed 
                            ? new SKPaint
                        {
                            Style = SKPaintStyle.StrokeAndFill,
                            Color = ToggleBackgroundColor.ChangeColorBrightness(-0.5f).ToSKColor(),
                            StrokeWidth = 3,
                            FilterQuality = SKFilterQuality.High,
                            IsAntialias = true
                        } 
                            : new SKPaint
                        {
                            Style = SKPaintStyle.StrokeAndFill,
                            Color = ToggleBackgroundColor.ToSKColor(),
                            StrokeWidth = 3,
                            FilterQuality = SKFilterQuality.High,
                            IsAntialias = true
                        };
                    else
                        return _isPressed ? _bgPressedPaint : _bgNormalPaint;
                }
            }

            public Color ToggleBackgroundColor { get; set; } = Color.FromHex("#00adef");

            public bool Activated { get; private set; } = false;

            public Action<bool> UpdateActivePropertyFromControl { get; set; }

            public ToggleSwitchControl()
            {
                _selector = new Selector { UpdateControlState = UpdateControlState };
            }

            protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
            {
                base.OnPaintSurface(e);

                var canvas = e.Surface.Canvas;
                canvas.Clear();

                int width = e.Info.Width;
                var originalHeight = e.Info.Height;
                var actualHeight = width / 2f;

                if (_isInit)
                {
                    var selectorLeftLimit = width / 4f;
                    var selectorRightLimit = width - selectorLeftLimit;

                    _selector.UpdateControlWidth(width);
                    _selector.UpdateLimits(selectorLeftLimit, selectorRightLimit);
                    _selector.UpdatePosition(selectorLeftLimit, originalHeight / 2f);
                    _selector.UpdatePosition(Activated);

                    _isInit = false;
                }

                canvas.Translate(0, originalHeight / 2f - actualHeight / 2f);

                canvas.DrawRoundRect(new SKRoundRect(new SKRect(3f, 3f, width - 3f, actualHeight - 3f), actualHeight), BorderPaint);
                canvas.DrawRoundRect(new SKRoundRect(new SKRect(6f, 6f, width - 6f, actualHeight - 6f), actualHeight), BgPaint);

                _selector.Paint(canvas, actualHeight / 4f);

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
                    case TouchActionType.Pressed:
                        _isPressed = true;
                        _pressedPoint = point;
                        _selector.UpdateLatestPoint(point);
                        InvalidateSurface();
                        break;
                    case TouchActionType.Moved:
                        if (point.X == _pressedPoint.X && point.Y == _pressedPoint.Y)
                            return;
                        _isMoving = true;
                        _selector.HandleMovement(point, InvalidateSurface);
                        break;
                    case TouchActionType.Released:
                    case TouchActionType.Cancelled:
                        if (_isMoving)
                            _selector.HandleReleased(InvalidateSurface);
                        else
                        {
                            var newActive = !Activated;
                            UpdateControlState(newActive);
                            UpdateActivated(newActive);
                        }
                        _pressedPoint = SKPoint.Empty;
                        _isPressed = false;
                        _isMoving = false;
                        InvalidateSurface();
                        break;
                }
            }

            public void UpdateActivated(bool newValue)
            {
                Activated = newValue;
                _selector.HandleActiveChanged(newValue, InvalidateSurface);
                InvalidateSurface();
            }

            private void UpdateControlState(bool active)
            {
                Activated = active;
                UpdateActivePropertyFromControl?.Invoke(active);
            }

            private class Selector
            {
                private float _width = -1;

                private SKPaint _paint = new SKPaint
                {
                    Style = SKPaintStyle.Fill,
                    Color = SKColors.WhiteSmoke,
                    FilterQuality = SKFilterQuality.High,
                    IsAntialias = true
                };

                private SKPoint _latestPoint = SKPoint.Empty;
                private ValueTuple<float, float> _limits;

                public SKMatrix Matrix { get; set; } = SKMatrix.CreateIdentity();

                public Action<bool> UpdateControlState { get; set; }

                public void UpdateControlWidth(float width) =>
                    _width = width;

                public void UpdateLimits(float left, float right) =>
                    _limits = new ValueTuple<float, float>(left, right);

                public void UpdateLatestPoint(SKPoint point) =>
                    _latestPoint = point;

                public void UpdatePosition(float x, float y) =>
                    Matrix = SKMatrix.CreateTranslation(x, y);

                public void UpdatePosition(bool active) =>
                    UpdatePosition(active ? _limits.Item2 : _limits.Item1, Matrix.TransY);

                public void Paint(SKCanvas canvas, float radius)
                {
                    canvas.Save();
                    var matrix = Matrix;
                    canvas.SetMatrix(matrix);
                    canvas.DrawCircle(0, 0, radius, _paint);
                    canvas.ResetMatrix();
                    canvas.Restore();
                }

                public void HandleMovement(SKPoint currentPoint, Action invalidateSurface) =>
                    Device.InvokeOnMainThreadAsync(() =>
                    {
                        if (currentPoint.X > _latestPoint.X)
                        {
                            var delta = currentPoint.X - _latestPoint.X;
                            if (Matrix.TransX + delta <= _limits.Item2)
                                Matrix = Matrix.PostConcat(
                                    SKMatrix.CreateTranslation(delta, 0));
                        }
                        else if (currentPoint.X < _latestPoint.X)
                        {
                            var delta = _latestPoint.X - currentPoint.X;
                            if (Matrix.TransX - delta >= _limits.Item1)
                                Matrix = Matrix.PostConcat(
                                    SKMatrix.CreateTranslation(-delta, 0));
                        }

                        UpdateLatestPoint(currentPoint);
                        invalidateSurface();
                    });

                public void HandleReleased(Action invalidateSurface)
                {
                    if (_width <= 0) return;

                    Device.InvokeOnMainThreadAsync(() =>
                    {
                        var destination = Matrix.TransX > _width / 2 ? _limits.Item2 : _limits.Item1;

                        StartAnimation(destination, invalidateSurface);

                        UpdateControlState?.Invoke(destination == _limits.Item2);
                        UpdateLatestPoint(new SKPoint(destination, Matrix.TransY));
                    });
                }

                public void HandleActiveChanged(bool active, Action invalidateSurface)
                {
                    if (_width <= 0) return;

                    Device.InvokeOnMainThreadAsync(() =>
                    {
                        var destination = active ? _limits.Item2 : _limits.Item1;

                        StartAnimation(destination, invalidateSurface);

                        UpdateLatestPoint(new SKPoint(destination, Matrix.TransY));
                    });
                }

                private void StartAnimation(float destPoint, Action invalidateSurface) =>
                    new Animation(value =>
                    {
                        UpdatePosition((float)value, Matrix.TransY);
                        invalidateSurface();
                    }, Matrix.TransX, destPoint, Easing.SinOut)
                        .Commit(Application.Current.MainPage, "ToggleSwitchSnap");
            }
        }
    }
}