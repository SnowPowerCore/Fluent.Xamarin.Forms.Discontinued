using FluentSkiaSharpControls.Effects;
using FluentSkiaSharpControls.Utils;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using Xamarin.Forms;

namespace FluentSkiaSharpControls.Controls.Skia.Fluent
{
    /// <summary>
    /// Fluent-design button, inspired by UWP platform
    /// </summary>
    public class Button : ContentView
    {
        private TouchEffect _touchEffect;
        private readonly ButtonControl _button;

        public static readonly BindableProperty TextProperty =
            BindableProperty.Create(nameof(Text), typeof(string),
                typeof(Button), defaultValue: string.Empty,
                defaultBindingMode: BindingMode.OneWay,
                propertyChanged: OnTextChanged);

        public static readonly BindableProperty ButtonBackgroundColorProperty =
            BindableProperty.Create(nameof(ButtonBackgroundColor), typeof(Color),
                typeof(Button), defaultValue: Color.FromHex("#333333"),
                defaultBindingMode: BindingMode.OneWay,
                propertyChanged: OnButtonBackgroundChanged);

        public static readonly BindableProperty ButtonForegroundColorProperty =
            BindableProperty.Create(nameof(ButtonForegroundColor), typeof(Color),
                typeof(Button), defaultValue: Color.White,
                defaultBindingMode: BindingMode.OneWay,
                propertyChanged: OnButtonForegroundChanged);

        public static readonly BindableProperty CommandProperty =
            BindableProperty.Create(nameof(Command), typeof(ICommand),
                typeof(Button), defaultValue: null,
                defaultBindingMode: BindingMode.OneWay);

        public static readonly BindableProperty CommandParameterProperty =
            BindableProperty.Create(nameof(CommandParameter), typeof(object),
                typeof(Button), defaultValue: null,
                defaultBindingMode: BindingMode.OneWay);

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public Color ButtonBackgroundColor
        {
            get => (Color)GetValue(ButtonBackgroundColorProperty);
            set => SetValue(ButtonBackgroundColorProperty, value);
        }

        public Color ButtonForegroundColor
        {
            get => (Color)GetValue(ButtonForegroundColorProperty);
            set => SetValue(ButtonForegroundColorProperty, value);
        }

        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        public object CommandParameter
        {
            get => GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }

        public Button()
        {
            _touchEffect = new TouchEffect { Capture = true };
            _touchEffect.TouchAction += HandleTouch;
            Effects.Add(_touchEffect);
            Content = _button = new ButtonControl
            {
                CommandFunc = () => Command?.Execute(CommandParameter)
            };
        }

        ~Button()
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
            if (string.IsNullOrEmpty(Text))
                return base.OnMeasure(widthConstraint, heightConstraint);

            var bounds = _button.MeasureText(Text);
            if (bounds.Item1 < widthConstraint)
                return base.OnMeasure(widthConstraint, heightConstraint);

            return new SizeRequest(new Size(bounds.Item1, bounds.Item2));
        }

        private void HandleTouch(object sender, TouchActionEventArgs args) =>
            _button.HandleTouch(sender, args);

        private static void OnTextChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (oldValue != newValue && null != newValue)
            {
                var control = (Button)bindable;
                control._button.UpdateText((string)newValue);
                control.InvalidateMeasure();
            }
        }

        private static void OnButtonBackgroundChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (oldValue != newValue && null != newValue)
            {
                var control = (Button)bindable;
                control._button.UpdateButtonBackground((Color)newValue);
            }
        }

        private static void OnButtonForegroundChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (oldValue != newValue && null != newValue)
            {
                var control = (Button)bindable;
                control._button.UpdateButtonForeground((Color)newValue);
            }
        }

        private class ButtonControl : SKCanvasView
        {
            private bool _isInit = true;
            private bool _isPressed = false;

            private TouchZone _pressedZone;

            private List<TouchZone> _zones = new List<TouchZone>();
            private ButtonBox _buttonBox;

            private SKPaint _zonePaint = new SKPaint
            {
                Color = SKColors.Transparent,
            };

            private SKPaint _bgNormalPaint = new SKPaint
            {
                Style = SKPaintStyle.StrokeAndFill,
                Color = SKColor.Parse("#333333"),
                StrokeWidth = 3,
                FilterQuality = SKFilterQuality.High,
                IsAntialias = true
            };

            private SKPaint _bgPressedPaint = new SKPaint
            {
                Style = SKPaintStyle.StrokeAndFill,
                Color = Color.FromHex("#333333").Lighten(0.5f).ToSKColor(),
                StrokeWidth = 3,
                FilterQuality = SKFilterQuality.High,
                IsAntialias = true
            };

            private SKPaint BgPaint =>
                _isPressed ? _bgPressedPaint : _bgNormalPaint;

            public Action CommandFunc { get; set; }

            public ButtonControl()
            {
                _buttonBox = new ButtonBox();
            }

            protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
            {
                base.OnPaintSurface(e);

                var canvas = e.Surface.Canvas;
                canvas.Clear();

                var width = e.Info.Width;
                var height = e.Info.Height;

                if (_isInit)
                {
                    _buttonBox.SetBounds(width, height);

                    var zonewidth = width / 3f;
                    var zoneheight = height / 3f;

                    float currentLeft = 0, currentTop = 0;
                    float currentRight = zonewidth;
                    float currentBottom = zoneheight;

                    var index = 0;
                    for (var i = 0; i < 3; i++)
                    {
                        for (var j = 0; j < 3; j++)
                        {
                            _zones.Add(new TouchZone
                            {
                                Bounds = new SKRect(currentLeft, currentTop, currentRight, currentBottom),
                                TouchLocation = (TouchLocation)index
                            });
                            canvas.DrawRect(_zones[index].Bounds, _zonePaint);
                            currentLeft += zonewidth;
                            currentRight += zonewidth;
                            index++;
                        }
                        currentLeft = 0;
                        currentTop += zoneheight;
                        currentRight = zonewidth;
                        currentBottom += zoneheight;
                    }

                    _isInit = false;
                }

                _buttonBox.Paint(canvas, 12, BgPaint);

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
                        for (var i = 0; i < _zones.Count; i++)
                        {
                            var item = _zones[i];
                            if (item.HitTest(point))
                            {
                                _pressedZone = item;
                                _buttonBox.HandlePressed(item, InvalidateSurface);
                                break;
                            }
                        }
                        InvalidateSurface();
                        break;
                    case TouchActionType.Released:
                    case TouchActionType.Cancelled:
                        CommandFunc.Invoke();
                        _buttonBox.HandleReleased(_pressedZone, InvalidateSurface);
                        _pressedZone = new TouchZone();
                        _isPressed = false;
                        InvalidateSurface();
                        break;
                }
            }

            public void UpdateText(string text)
            {
                var measures = MeasureText(text);
                _buttonBox.SetText(text);
                _buttonBox.SetTextBounds(measures.Item1, measures.Item2);
                InvalidateSurface();
            }

            public void UpdateButtonBackground(Color color)
            {
                _bgNormalPaint = new SKPaint
                {
                    Style = _bgNormalPaint.Style,
                    Color = color.ToSKColor(),
                    StrokeWidth = _bgNormalPaint.StrokeWidth,
                    FilterQuality = _bgNormalPaint.FilterQuality,
                    IsAntialias = _bgNormalPaint.IsAntialias
                };
                _bgPressedPaint = new SKPaint
                {
                    Style = _bgPressedPaint.Style,
                    Color = color.Lighten(0.5f).ToSKColor(),
                    StrokeWidth = _bgPressedPaint.StrokeWidth,
                    FilterQuality = _bgPressedPaint.FilterQuality,
                    IsAntialias = _bgPressedPaint.IsAntialias
                };
                InvalidateSurface();
            }

            public void UpdateButtonForeground(Color color)
            {
                _buttonBox.SetTextColor(color.ToSKColor());
                InvalidateSurface();
            }

            public (float, float) MeasureText(string text)
            {
                var bounds = new SKRect();
                var paint = new SKPaint
                {
                    Color = SKColors.White,
                    TextSize = 32f,
                    IsAntialias = true,
                };
                paint.MeasureText(text, ref bounds);
                return (bounds.Width, bounds.Height);
            }

            private enum TouchLocation
            {
                Tl,
                Tc,
                Tr,
                Ml,
                Mc,
                Mr,
                Bl,
                Bc,
                Br
            }

            private class TouchZone
            {
                public TouchLocation TouchLocation { get; set; }

                public SKRect Bounds { get; set; }

                public bool HitTest(SKPoint location)
                {
                    if (SKMatrix.Identity.TryInvert(out var inverseMatrix))
                    {
                        // Transform the point using the inverted matrix
                        var transformedPoint = inverseMatrix.MapPoint(location);

                        // Check if it's in the untransformed bitmap rectangle
                        var rect = new SKRect(0, 0, Bounds.Right, Bounds.Bottom);
                        return rect.Contains(transformedPoint);
                    }
                    return false;
                }
            }

            private class ButtonBox
            {
                private string _text = string.Empty;
                private SKPaint _textPaint = new SKPaint
                {
                    Color = SKColors.White,
                    TextSize = 32f,
                    IsAntialias = true,
                };

                private readonly double _deflectionValue = 6d;

                private Dictionary<TouchLocation, Func<float, SKMatrix>> _touchByLocation;

                private Dictionary<TouchLocation, Func<float, SKMatrix>> TouchByLocation =>
                    _touchByLocation ?? (_touchByLocation = new Dictionary<TouchLocation, Func<float, SKMatrix>>
                    {
                        [TouchLocation.Tl] = value =>
                        {
                            return MatrixUtils.ComputeMatrix(
                                new SKSize(Width, Height),
                                new SKPoint(value, 0),
                                new SKPoint(Width, 0),
                                new SKPoint(0, Height),
                                new SKPoint(Width, Height));
                        },
                        [TouchLocation.Tc] = value =>
                        {
                            return SKMatrix.Concat(
                                TouchByLocation[TouchLocation.Tl].Invoke(value),
                                TouchByLocation[TouchLocation.Tr].Invoke(value));
                        },
                        [TouchLocation.Tr] = value =>
                        {
                            return MatrixUtils.ComputeMatrix(
                                new SKSize(Width, Height),
                                new SKPoint(0, 0),
                                new SKPoint(Width - value, 0),
                                new SKPoint(0, Height),
                                new SKPoint(Width, Height));
                        },
                        [TouchLocation.Ml] = value =>
                        {
                            return SKMatrix.Concat(
                                TouchByLocation[TouchLocation.Tl].Invoke(value),
                                TouchByLocation[TouchLocation.Bl].Invoke(value));
                        },
                        [TouchLocation.Mc] = value =>
                        {
                            return SKMatrix.Concat(
                                SKMatrix.Concat(
                                    TouchByLocation[TouchLocation.Tl].Invoke(value),
                                    TouchByLocation[TouchLocation.Tr].Invoke(value)),
                                SKMatrix.Concat(
                                    TouchByLocation[TouchLocation.Bl].Invoke(value),
                                    TouchByLocation[TouchLocation.Br].Invoke(value)));
                        },
                        [TouchLocation.Mr] = value =>
                        {
                            return SKMatrix.Concat(
                                TouchByLocation[TouchLocation.Tr].Invoke(value),
                                TouchByLocation[TouchLocation.Br].Invoke(value));
                        },
                        [TouchLocation.Bl] = value =>
                        {
                            return MatrixUtils.ComputeMatrix(
                                new SKSize(Width, Height),
                                new SKPoint(0, 0),
                                new SKPoint(Width, 0),
                                new SKPoint(value, Height),
                                new SKPoint(Width, Height));
                        },
                        [TouchLocation.Bc] = value =>
                        {
                            return SKMatrix.Concat(
                                TouchByLocation[TouchLocation.Bl].Invoke(value),
                                TouchByLocation[TouchLocation.Br].Invoke(value));
                        },
                        [TouchLocation.Br] = value =>
                        {
                            return MatrixUtils.ComputeMatrix(
                                new SKSize(Width, Height),
                                new SKPoint(0, 0),
                                new SKPoint(Width, 0),
                                new SKPoint(0, Height),
                                new SKPoint(Width - value, Height));
                        }
                    });

                public float Width { get; private set; }

                public float TextWidth { get; private set; }

                public float Height { get; private set; }

                public float TextHeight { get; private set; }

                public SKMatrix Matrix { get; set; } = SKMatrix.CreateIdentity();

                public void UpdateMatrix(SKMatrix matrix) =>
                    Matrix = matrix;

                public void ResetMatrix() =>
                    Matrix = SKMatrix.CreateIdentity();

                public void SetText(string text)
                {
                    _text = text;
                }

                public void SetTextColor(SKColor color)
                {
                    _textPaint = new SKPaint
                    {
                        Color = color,
                        TextSize = _textPaint.TextSize,
                        IsAntialias = _textPaint.IsAntialias,
                    };
                }

                public void SetBounds(float width, float height)
                {
                    Width = width;
                    Height = height;
                }

                public void SetTextBounds(float width, float height)
                {
                    TextWidth = width;
                    TextHeight = height;
                }

                public void Paint(SKCanvas canvas, float radius, SKPaint paint)
                {
                    canvas.Save();
                    var matrix = Matrix;
                    canvas.SetMatrix(matrix);
                    canvas.DrawRoundRect(new SKRoundRect(new SKRect(0, 0, Width, Height), radius), paint);
                    canvas.DrawText(_text, new SKPoint(
                        Width / 2 - TextWidth / 2,
                        Height / 2 + TextHeight / 2), _textPaint);
                    canvas.ResetMatrix();
                    canvas.Restore();
                }

                public void HandlePressed(TouchZone zone, Action invalidateSurface) =>
                    StartAnimation(zone, 0d, _deflectionValue, invalidateSurface, Easing.SinIn, 1, 0.975f);

                public void HandleReleased(TouchZone latest, Action invalidateSurface) =>
                    StartAnimation(latest, _deflectionValue, 0d, invalidateSurface, Easing.CubicInOut, 450);

                private void StartAnimation(TouchZone zone, double originalPoint,
                    double destPoint, Action invalidateSurface, Easing easing, uint length = 250,
                    float scaleFactor = 1f) =>
                    new Animation(value =>
                    {
                        UpdateMatrix(SKMatrix.Concat(
                            TouchByLocation[zone.TouchLocation].Invoke((float)value),
                            SKMatrix.CreateScale(scaleFactor, scaleFactor, Width / 2, Height / 2)));
                        invalidateSurface();
                    }, originalPoint, destPoint, easing)
                        .Commit(Application.Current.MainPage, "ButtomDeflection", length: length);
            }
        }
    }
}