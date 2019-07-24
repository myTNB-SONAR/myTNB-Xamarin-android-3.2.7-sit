using System;
using System.Collections.Generic;
using System.Diagnostics;
using CoreAnimation;
using CoreGraphics;
using Foundation;
using UIKit;

namespace myTNB
{
    public class CustomUIViewController : UIViewController
    {
        internal Dictionary<string, string> I18NDictionary;
        internal string PageName;
        internal bool IsGradientRequired;
        internal bool IsGradientImageRequired;
        internal UIImageView ImageViewGradientImage;
        internal UIView _statusBarView;
        internal nfloat ViewWidth, ViewHeight;
        private UIView _viewToast, _viewToastOverlay;
        private UILabel _lblToastDetails;
        private bool _isAnimating;

        public CustomUIViewController(IntPtr handle) : base(handle)
        {
        }
        #region LifeCycle
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            I18NDictionary = LanguageManager.Instance.GetValuesByPage(PageName);
            if (IsGradientRequired)
            {
                CreateBackgroundGradient();
            }
            if (IsGradientImageRequired)
            {
                CreateImageGradient();
            }
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
        }
        #endregion
        #region Widget Utilities
        public UILabel GetUILabelField(CGRect lblFrame, string key, UITextAlignment txtAlignment = UITextAlignment.Left)
        {
            return CustomUILabel.GetUILabelField(lblFrame, key, txtAlignment);
        }

        public UILabel GetUILabelField(CGRect lblFrame, string key, UIFont font
            , UIColor textColor, UITextAlignment txtAlignment = UITextAlignment.Left)
        {
            return CustomUILabel.GetUILabelField(lblFrame, key, font, textColor, txtAlignment);
        }

        public CGSize GetLabelSize(UILabel label, nfloat width, nfloat height)
        {
            return CustomUILabel.GetLabelSize(label, width, height);
        }

        public UIButton GetUIButton(CGRect frame, string key)
        {
            return CustomUIButton.GetUIButton(frame, key);
        }

        public void MakeTopCornerRadius(UIButton button)
        {
            CustomUIButton.MakeTopCornerRadius(button);
        }

        public void SetFrames()
        {
            ViewWidth = View.Frame.Width;
            ViewHeight = View.Frame.Height;
            if (NavigationController != null && NavigationController.NavigationBar != null)
            {
                ViewHeight -= NavigationController.NavigationBar.Frame.Height;
            }
            if (DeviceHelper.IsIphoneXUpResolution())
            {
                ViewHeight -= DeviceHelper.GetStatusBarHeight();
            }
        }
        #endregion
        #region Alerts
        public void DisplayNoDataAlert()
        {
            AlertHandler.DisplayNoDataAlert(this);
        }

        public void DisplayServiceError(string message, Action<UIAlertAction> handler = null)
        {
            AlertHandler.DisplayServiceError(this, message, handler);
        }

        public void DisplayGenericAlert(string title, string message, Action<UIAlertAction> handler = null)
        {
            AlertHandler.DisplayGenericAlert(this, title, message, handler);
        }

        public void DisplayCustomAlert(string title, string message, Dictionary<string, Action> ctaButtons)
        {
            AlertHandler.DisplayCustomAlert(title, message, ctaButtons);
        }

        public void DisplayCustomAlert(string title, string message, string btnTitle, Action btnAction = null)
        {
            AlertHandler.DisplayCustomAlert(title, message, new Dictionary<string, Action>() {
                { string.IsNullOrEmpty(btnTitle) ? "Common_Ok".Translate() : btnTitle, btnAction } }
            , UITextAlignment.Left, UITextAlignment.Left, true, 0.056F, false);
        }
        #endregion
        #region Toast
        public void DisplayToast(string message)
        {
            if (_viewToast == null)
            {
                _viewToast = new UIView(new CGRect(18, 32, View.Frame.Width - 36, 48))
                {
                    BackgroundColor = MyTNBColor.SunGlow,
                    Hidden = true,
                    ClipsToBounds = true
                };
                _viewToast.Layer.CornerRadius = 2.0f;

                _viewToastOverlay = new UIView(_viewToast.Frame)
                {
                    UserInteractionEnabled = true,
                    MultipleTouchEnabled = true,
                    Hidden = true
                };

                _lblToastDetails = new UILabel(new CGRect(16, 16, _viewToast.Frame.Width - 32
                    , View.Frame.Height - ((_viewToast.Frame.X * 2) + 32)))
                {
                    TextAlignment = UITextAlignment.Left,
                    Font = MyTNBFont.MuseoSans12,
                    TextColor = MyTNBColor.TunaGrey(),
                    Lines = 0,
                    LineBreakMode = UILineBreakMode.WordWrap
                };

                _viewToast.AddSubview(_lblToastDetails);
                AddSwipeGestureForToast();
                View.AddSubviews(new UIView[] { _viewToast, _viewToastOverlay });
            }
            _lblToastDetails.Text = message ?? string.Empty;

            CGSize size = LabelHelper.GetLabelSize(_lblToastDetails
                , _lblToastDetails.Frame.Width, _lblToastDetails.Frame.Height);
            _lblToastDetails.Frame = new CGRect(_lblToastDetails.Frame.X
                , _lblToastDetails.Frame.Y, _lblToastDetails.Frame.Width, size.Height);
            _viewToast.Frame = new CGRect(_viewToast.Frame.X
                , 0, _viewToast.Frame.Width, 0);
            _viewToastOverlay.Frame = new CGRect(_viewToast.Frame.X
                , 32, _viewToast.Frame.Width, size.Height + 32);

            _viewToast.Hidden = false;
            View.BringSubviewToFront(_viewToast);
            _viewToastOverlay.Hidden = false;
            if (!_isAnimating)
            {
#pragma warning disable XI0001 // Notifies you with advices on how to use Apple APIs
                UIView.Animate(0.3, 0.3, UIViewAnimationOptions.CurveEaseOut, () =>
                 {
                     _isAnimating = true;
                     _viewToast.Frame = new CGRect(_viewToast.Frame.X
                    , 32, _viewToast.Frame.Width, size.Height + 32);
                 }, () =>
                 {
                     DismissToast(2.0F);
                 });
#pragma warning restore XI0001 // Notifies you with advices on how to use Apple APIs
            }
        }
        #endregion
        #region Private Methods
        private void AddSwipeGestureForToast()
        {
            if (_viewToastOverlay != null)
            {
                _viewToastOverlay.AddGestureRecognizer(new UISwipeGestureRecognizer((obj) =>
                {
                    Debug.WriteLine("SWIPED Up");
                    DismissToast(0.1F);
                })
                {
                    Direction = UISwipeGestureRecognizerDirection.Up
                });
                _viewToastOverlay.AddGestureRecognizer(new UITapGestureRecognizer((obj) =>
                {
                    Debug.WriteLine("Tap");
                    DismissToast(0.1F);
                }));
            }
        }

        private void DismissToast(float delay)
        {
#pragma warning disable XI0001 // Notifies you with advices on how to use Apple APIs
            UIView.Animate(0.3, delay, UIViewAnimationOptions.CurveEaseOut, () =>
            {
                _viewToast.Frame = new CGRect(_viewToast.Frame.X
               , 0, _viewToast.Frame.Width, 0);
            }, () =>
            {
                _isAnimating = false;
                _viewToast.Hidden = true;
                _viewToastOverlay.Hidden = true;
                _viewToast.Layer.RemoveAllAnimations();
            });
#pragma warning restore XI0001 // Notifies you with advices on how to use Apple APIs
        }

        private void CreateBackgroundGradient()
        {
            UIView gradientView = new UIView(new CGRect(0, 0, View.Frame.Width, View.Frame.Height * 0.50F));
            CAGradientLayer gradientLayer = new CAGradientLayer
            {
                Colors = new[] { MyTNBColor.GradientPurpleLightElement.CGColor, MyTNBColor.GradientPurpleDarkElement.CGColor }
            };
            gradientLayer.Locations = new NSNumber[] { 0, 1 };
            gradientLayer.Frame = gradientView.Bounds;
            gradientView.Layer.InsertSublayer(gradientLayer, 0);
            View.AddSubview(gradientView);
        }

        private void CreateImageGradient()
        {
            ImageViewGradientImage = new UIImageView(new CGRect(0, 0
                , View.Frame.Width, UIApplication.SharedApplication.KeyWindow.Frame.Height * 0.61F))
            {
                Image = UIImage.FromBundle("Background-Home")
            };
            View.AddSubview(ImageViewGradientImage);
        }

        #endregion
        #region Customize View
        public virtual void SetStatusBarNoOverlap()
        {
            _statusBarView = new UIView(new CGRect(0, 0, View.Frame.Width, DeviceHelper.GetStatusBarHeight()));
            View.AddSubview(_statusBarView);
        }
        #endregion

        #region Utilities
        public string GetI18NValue(string key)
        {
            return I18NDictionary.ContainsKey(key) ? I18NDictionary[key] : string.Empty;
        }
        public string GetCommonI18NValue(string key)
        {
            return DataManager.DataManager.SharedInstance.CommonI18NDictionary.ContainsKey(key)
                ? DataManager.DataManager.SharedInstance.CommonI18NDictionary[key] : string.Empty;
        }
        public string GetHintI18NValue(string key)
        {
            return DataManager.DataManager.SharedInstance.HintI18NDictionary.ContainsKey(key)
                ? DataManager.DataManager.SharedInstance.HintI18NDictionary[key] : string.Empty;
        }
        public string GetErrorI18NValue(string key)
        {
            return DataManager.DataManager.SharedInstance.ErrorI18NDictionary.ContainsKey(key)
                ? DataManager.DataManager.SharedInstance.ErrorI18NDictionary[key] : string.Empty;
        }
        #endregion
    }
}