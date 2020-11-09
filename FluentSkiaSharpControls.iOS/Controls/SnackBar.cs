using Foundation;
using System;
using UIKit;

namespace FluentSkiaSharpControls.iOS.Controls
{
    public enum SnackbarAnimationType
    {
        FadeInFadeOut,
        SlideFromBottomToTop,
        SlideFromBottomBackToBottom,
        SlideFromLeftToRight,
        SlideFromRightToLeft,
        Flip,
    }

    public class SnackBar : UIView
    {
        /// Snackbar action button max width.
        private const float snackbarActionButtonMaxWidth = 64;

        // Snackbar action button min width.
        private const float snackbarActionButtonMinWidth = 44;

        // Snackbar icon imageView default width
        private const float snackbarIconImageViewWidth = 32;

        private UIEdgeInsets safeAreaInsets;

        public Action<SnackBar> ActionBlock { get; set; }
        public Action<SnackBar> SecondActionBlock { get; set; }

        /// <summary>
        /// Snackbar display duration. Default is 3 seconds.
        /// </summary>
	    public TimeSpan Duration { get; set; } = TimeSpan.FromSeconds(3);
        public SnackbarAnimationType AnimationType { get; set; } = SnackbarAnimationType.SlideFromBottomBackToBottom;

        public float AnimationDuration { get; set; } = 0.3f;

        public bool ShowOnTop { get; set; } = false;

        public nfloat CornerRadius
        {
            get => Layer.CornerRadius;
            set
            {
                var _cornerRadius = value;
                if (_cornerRadius > Height)
                {
                    _cornerRadius = Height / 2;
                }
                if (_cornerRadius < 0)
                    _cornerRadius = 0;

                Layer.CornerRadius = _cornerRadius;
                Layer.MasksToBounds = true;
            }
        }

        nfloat topMargin = 8;
        public nfloat TopMargin
        {
            get => topMargin + safeAreaInsets.Top;
            set => topMargin = value;
        }

        nfloat leftMargin = 4;
        public nfloat LeftMargin
        {
            get => leftMargin + safeAreaInsets.Left;
            set => leftMargin = value;
        }

        nfloat rightMargin = 4;
        public nfloat RightMargin
        {
            get => rightMargin + safeAreaInsets.Right;
            set => rightMargin = value;
        }

        /// Bottom margin. Default is 4
        nfloat bottomMargin = 4;
        public nfloat BottomMargin
        {
            get => bottomMargin + safeAreaInsets.Bottom;
            set => bottomMargin = value;
        }
        public nfloat Height { get; set; } = 44;

        public string Message
        {
            get => MessageLabel.Text;
            set => MessageLabel.Text = value;
        }

        string actionText;
        public string ActionText
        {
            get => actionText;
            set
            {
                actionText = value;
                ActionButton.SetTitle(value, UIControlState.Normal);
            }
        }

        string secondActionText;
        public string SecondActionText
        {
            get => secondActionText;
            set
            {
                secondActionText = value;
                SecondActionButton.SetTitle(value, UIControlState.Normal);
            }
        }

        private UIImage _icon;
        public UIImage Icon
        {
            get => _icon;
            set
            {
                _icon = value;
                IconImageView.Image = _icon;
            }
        }

        private UIViewContentMode _iconContentMode = UIViewContentMode.Center;
        public UIViewContentMode IconContentMode
        {
            get => _iconContentMode;
            set
            {
                _iconContentMode = value;
                IconImageView.ContentMode = _iconContentMode;
            }
        }

        public UILabel MessageLabel { get; }
        public UIButton ActionButton { get; set; }
        public UIButton SecondActionButton { get; set; }

        public UIImageView IconImageView { get; set; }
        private readonly UIView seperateView;

        // Timer to dismiss the snackbar.
        private NSTimer dismissTimer;

        // Constraints.
        private NSLayoutConstraint heightConstraint;
        private NSLayoutConstraint leftMarginConstraint;
        private NSLayoutConstraint rightMarginConstraint;
        private NSLayoutConstraint topMarginConstraint;
        private NSLayoutConstraint bottomMarginConstraint;
        private NSLayoutConstraint actionButtonWidthConstraint;
        private NSLayoutConstraint secondActionButtonWidthConstraint;
        private NSLayoutConstraint iconImageViewWidthConstraint;

        public SnackBar() : base(CoreGraphics.CGRect.FromLTRB(0, 0, 320, 44))
        {
            TranslatesAutoresizingMaskIntoConstraints = false;
            BackgroundColor = UIColor.DarkGray;
            Layer.CornerRadius = 4;
            Layer.MasksToBounds = true;

            SetupSafeAreaInsets();

            MessageLabel = new UILabel
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                TextColor = UIColor.White,
                Font = UIFont.SystemFontOfSize(14),
                BackgroundColor = UIColor.Clear,
                LineBreakMode = UILineBreakMode.WordWrap,
                TextAlignment = UITextAlignment.Left,
                Lines = 0
            };

            AddSubview(MessageLabel);

            IconImageView = new UIImageView()
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                BackgroundColor = UIColor.Clear,
                ContentMode = IconContentMode
            };

            AddSubview(IconImageView);

            ActionButton = new UIButton
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                BackgroundColor = UIColor.Clear
            };
            ActionButton.TitleLabel.Font = UIFont.SystemFontOfSize(14);
            ActionButton.TitleLabel.AdjustsFontSizeToFitWidth = true;
            ActionButton.SetTitleColor(UIColor.White, UIControlState.Normal);
            ActionButton.TouchUpInside += (s, e) =>
            {
                // there is a chance that user doesn't want to do anything here, he simply wants to dismiss
                ActionBlock?.Invoke(this);
                Dismiss();
            };

            AddSubview(ActionButton);

            SecondActionButton = new UIButton
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                BackgroundColor = UIColor.Clear
            };
            SecondActionButton.TitleLabel.Font = UIFont.BoldSystemFontOfSize(14);
            SecondActionButton.TitleLabel.AdjustsFontSizeToFitWidth = true;
            SecondActionButton.SetTitleColor(UIColor.White, UIControlState.Normal);
            SecondActionButton.TouchUpInside += (s, e) =>
            {
                SecondActionBlock?.Invoke(this);
                Dismiss();
            };

            AddSubview(SecondActionButton);

            seperateView = new UIView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                BackgroundColor = UIColor.Gray
            };

            AddSubview(seperateView);

            // Add constraints
            var hConstraints = NSLayoutConstraint.FromVisualFormat(
                "H:|-10-[iconImageView]-2-[messageLabel]-2-[seperateView(0.5)]-2-[actionButton(>=44@999)]-0-[secondActionButton(>=44@999)]-0-|",
                0,
                new NSDictionary(),
                NSDictionary.FromObjectsAndKeys(
                    new NSObject[] {
                        IconImageView,
                        MessageLabel,
                        seperateView,
                        ActionButton,
                        SecondActionButton
                    },
                    new NSObject[] {
                        new NSString("iconImageView"),
                        new NSString("messageLabel"),
                        new NSString("seperateView"),
                        new NSString("actionButton"),
                        new NSString("secondActionButton")
                    }
                )
            );

            var vConstraintsForIconImageView = NSLayoutConstraint.FromVisualFormat(
             "V:|-2-[iconImageView]-2-|", 0, new NSDictionary(), NSDictionary.FromObjectsAndKeys(new NSObject[] { IconImageView }, new NSObject[] { new NSString("iconImageView") })
            );

            var vConstraintsForMessageLabel = NSLayoutConstraint.FromVisualFormat(
                "V:|-0-[messageLabel]-0-|", 0, new NSDictionary(), NSDictionary.FromObjectsAndKeys(new NSObject[] { MessageLabel }, new NSObject[] { new NSString("messageLabel") })
            );

            var vConstraintsForSeperateView = NSLayoutConstraint.FromVisualFormat(
                "V:|-4-[seperateView]-4-|", 0, new NSDictionary(), NSDictionary.FromObjectsAndKeys(new NSObject[] { seperateView }, new NSObject[] { new NSString("seperateView") })
            );

            var vConstraintsForActionButton = NSLayoutConstraint.FromVisualFormat(
                "V:|-0-[actionButton]-0-|", 0, new NSDictionary(), NSDictionary.FromObjectsAndKeys(new NSObject[] { ActionButton }, new NSObject[] { new NSString("actionButton") })
            );

            var vConstraintsForSecondActionButton = NSLayoutConstraint.FromVisualFormat(
                "V:|-0-[secondActionButton]-0-|", 0, new NSDictionary(), NSDictionary.FromObjectsAndKeys(new NSObject[] { SecondActionButton }, new NSObject[] { new NSString("secondActionButton") })
            );

            iconImageViewWidthConstraint = NSLayoutConstraint.Create(IconImageView, NSLayoutAttribute.Width, NSLayoutRelation.Equal, null, NSLayoutAttribute.NoAttribute, 1, snackbarIconImageViewWidth);

            actionButtonWidthConstraint = NSLayoutConstraint.Create(ActionButton, NSLayoutAttribute.Width, NSLayoutRelation.Equal, null, NSLayoutAttribute.NoAttribute, 1, snackbarActionButtonMinWidth);

            secondActionButtonWidthConstraint = NSLayoutConstraint.Create(SecondActionButton, NSLayoutAttribute.Width, NSLayoutRelation.Equal, null, NSLayoutAttribute.NoAttribute, 1, snackbarActionButtonMinWidth);

            IconImageView.AddConstraint(iconImageViewWidthConstraint);
            ActionButton.AddConstraint(actionButtonWidthConstraint);
            SecondActionButton.AddConstraint(secondActionButtonWidthConstraint);

            AddConstraints(hConstraints);
            AddConstraints(vConstraintsForIconImageView);
            AddConstraints(vConstraintsForMessageLabel);
            AddConstraints(vConstraintsForSeperateView);
            AddConstraints(vConstraintsForActionButton);
            AddConstraints(vConstraintsForSecondActionButton);
        }

        private void SetupSafeAreaInsets()
        {
            if (UIDevice.CurrentDevice.CheckSystemVersion(11, 0))
            {
                safeAreaInsets = UIApplication.SharedApplication.KeyWindow.SafeAreaInsets;
            }
            else
            {
                safeAreaInsets = new UIEdgeInsets();
            }
        }

        /// <summary>
        /// Show the snackbar
        /// </summary>
        public void Show()
        {
            if (Superview != null)
                return;

            dismissTimer = NSTimer.CreateScheduledTimer(Duration, x => Dismiss());

            IconImageView.Hidden = Icon == null;
            ActionButton.Hidden = ActionBlock == null;
            SecondActionButton.Hidden = SecondActionBlock == null;
            seperateView.Hidden = ActionButton.Hidden;

            iconImageViewWidthConstraint.Constant = IconImageView.Hidden ? 0 : snackbarIconImageViewWidth;
            actionButtonWidthConstraint.Constant = ActionButton.Hidden ? 0 : SecondActionButton.Hidden ? snackbarActionButtonMaxWidth : snackbarActionButtonMinWidth;
            secondActionButtonWidthConstraint.Constant = SecondActionButton.Hidden ? 0 : ActionButton.Hidden ? snackbarActionButtonMaxWidth : snackbarActionButtonMinWidth;

            LayoutIfNeeded();

            var localSuperView = UIApplication.SharedApplication.KeyWindow;
            if (localSuperView != null)
            {
                localSuperView.AddSubview(this);

                heightConstraint = NSLayoutConstraint.Create(
                    this,
                    NSLayoutAttribute.Height,
                    NSLayoutRelation.GreaterThanOrEqual,
                    null,
                    NSLayoutAttribute.NoAttribute,
                    1,
                    Height);

                leftMarginConstraint = NSLayoutConstraint.Create(
                    this,
                    NSLayoutAttribute.Left,
                    NSLayoutRelation.Equal,
                    localSuperView,
                    NSLayoutAttribute.Left,
                    1,
                    LeftMargin);

                rightMarginConstraint = NSLayoutConstraint.Create(
                    this,
                    NSLayoutAttribute.Right,
                    NSLayoutRelation.Equal,
                    localSuperView,
                    NSLayoutAttribute.Right,
                    1,
                    -RightMargin);

                topMarginConstraint = NSLayoutConstraint.Create(
                    this,
                    NSLayoutAttribute.Top,
                    NSLayoutRelation.Equal,
                    localSuperView,
                    NSLayoutAttribute.Top,
                    1,
                    TopMargin);

                bottomMarginConstraint = NSLayoutConstraint.Create(
                    this,
                    NSLayoutAttribute.Bottom,
                    NSLayoutRelation.Equal,
                    localSuperView,
                    NSLayoutAttribute.Bottom,
                    1,
                    -BottomMargin);

                leftMarginConstraint.Priority = 999;
                rightMarginConstraint.Priority = 999;

                AddConstraint(heightConstraint);
                localSuperView.AddConstraint(leftMarginConstraint);
                localSuperView.AddConstraint(rightMarginConstraint);

                var positionConstraint = ShowOnTop
                    ? topMarginConstraint
                    : bottomMarginConstraint;

                localSuperView.AddConstraint(positionConstraint);

                // Show
                ShowWithAnimation();
            }
            else
            {
                Console.WriteLine("Snackbar needs a keyWindows to display.");
            }
        }

        /// <summary>
        /// Dismiss.
        /// - parameter animated: If dismiss with animation.
        /// </summary>
        public void Dismiss(bool animated = true)
        {
            dismissTimer?.Invalidate();
            dismissTimer = null;

            nfloat superViewWidth = 0;

            if (Superview != null)
                superViewWidth = Superview.Frame.Width;

            if (!animated)
            {
                RemoveFromSuperview();
                return;
            }

            Action animationBlock = () => { };

            switch (AnimationType)
            {
                case SnackbarAnimationType.FadeInFadeOut:
                    animationBlock = () => Alpha = 0;
                    break;
                case SnackbarAnimationType.SlideFromBottomBackToBottom:
                    animationBlock = () => bottomMarginConstraint.Constant = Height;
                    break;
                case SnackbarAnimationType.SlideFromBottomToTop:
                    animationBlock = () => { Alpha = 0; bottomMarginConstraint.Constant = -Height - BottomMargin; };
                    break;
                case SnackbarAnimationType.SlideFromLeftToRight:
                    animationBlock = () => { leftMarginConstraint.Constant = LeftMargin + superViewWidth; rightMarginConstraint.Constant = -RightMargin + superViewWidth; };
                    break;
                case SnackbarAnimationType.SlideFromRightToLeft:
                    animationBlock = () =>
                    {
                        leftMarginConstraint.Constant = LeftMargin - superViewWidth;
                        rightMarginConstraint.Constant = -RightMargin - superViewWidth;
                    };
                    break;
                case SnackbarAnimationType.Flip:
                    //todo animationBlock = () => { this.Layer.Transform = CAT(CGFloat(M_PI_2), 1, 0, 0);}
                    break;
            };

            SetNeedsLayout();

            Animate(
                AnimationDuration,
                0,
                UIViewAnimationOptions.CurveEaseIn,
                animationBlock,
                RemoveFromSuperview
            );
        }

        private void ShowWithAnimation()
        {
            Action animationBlock = () => LayoutIfNeeded();
            var superViewWidth = Superview.Frame.Width;

            switch (AnimationType)
            {
                case SnackbarAnimationType.FadeInFadeOut:
                    Alpha = 0;
                    SetNeedsLayout();

                    animationBlock = () => Alpha = 1;
                    break;
                case SnackbarAnimationType.SlideFromBottomBackToBottom:
                case SnackbarAnimationType.SlideFromBottomToTop:
                    bottomMarginConstraint.Constant = -BottomMargin;
                    LayoutIfNeeded();
                    break;
                case SnackbarAnimationType.SlideFromLeftToRight:
                    leftMarginConstraint.Constant = LeftMargin - superViewWidth;
                    rightMarginConstraint.Constant = -RightMargin - superViewWidth;
                    bottomMarginConstraint.Constant = -BottomMargin;
                    LayoutIfNeeded();
                    break;
                case SnackbarAnimationType.SlideFromRightToLeft:
                    leftMarginConstraint.Constant = LeftMargin + superViewWidth;
                    rightMarginConstraint.Constant = -RightMargin + superViewWidth;
                    bottomMarginConstraint.Constant = -BottomMargin;
                    LayoutIfNeeded();
                    break;
                case SnackbarAnimationType.Flip:
                    //todo animationBlock = () => { this.Layer.Transform = CAT(CGFloat(M_PI_2), 1, 0, 0);}
                    break;
            };

            // Final state
            topMarginConstraint.Constant = TopMargin;
            bottomMarginConstraint.Constant = -BottomMargin;
            leftMarginConstraint.Constant = LeftMargin;
            rightMarginConstraint.Constant = -RightMargin;

            AnimateNotify(
                    AnimationDuration,
                    0,
                    0.7f,
                    5f,
                    UIViewAnimationOptions.CurveEaseInOut,
                    animationBlock,
                    null
                );
        }
    }
}