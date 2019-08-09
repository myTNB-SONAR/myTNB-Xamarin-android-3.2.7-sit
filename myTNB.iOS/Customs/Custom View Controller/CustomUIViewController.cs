using System;
using System.Collections.Generic;
using System.Diagnostics;
using CoreAnimation;
using CoreGraphics;
using Foundation;
using myTNB.Customs;
using UIKit;

namespace myTNB
{
    public class CustomUIViewController : UIViewController
    {
        internal Dictionary<string, string> I18NDictionary;
        internal string PageName;
        internal bool IsGradientRequired, IsFullGradient, IsReversedGradient;
        internal bool IsGradientImageRequired;
        internal UIImageView ImageViewGradientImage;
        internal UIView _statusBarView;
        internal nfloat ViewWidth, ViewHeight;

        private UIView _viewToast, _viewToastOverlay;
        private UILabel _lblToastDetails;
        private bool _isAnimating;

        private nfloat WidthBase = 320;
        private nfloat HeightBase = 568;

        public enum PermissionMode
        {
            Camera,
            Gallery
        }

        public CustomUIViewController(IntPtr handle) : base(handle)
        {
        }
        #region LifeCycle
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            I18NDictionary = LanguageManager.Instance.GetValuesByPage(PageName);
            ConfigureNavigationBar();
            if (IsGradientRequired)
            {
                CreateBackgroundGradient();
            }
            if (IsGradientImageRequired)
            {
                CreateImageGradient();
            }
            SetFrames();
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
            if (NavigationController != null && !NavigationController.NavigationBarHidden && NavigationController.NavigationBar != null)
            {
                ViewHeight -= NavigationController.NavigationBar.Frame.Height;
            }
            if (TabBarController != null && TabBarController.TabBar != null && !TabBarController.TabBar.Hidden)
            {
                ViewHeight -= TabBarController.TabBar.Frame.Height;
            }
            if (DeviceHelper.IsIphoneXUpResolution())
            {
                ViewHeight -= 20;
            }
            ViewHeight -= DeviceHelper.GetStatusBarHeight();
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

        public void DisplayCustomAlert(string title, string message, Dictionary<string, Action> ctaButtons, UIImage image)
        {
            AlertHandler.DisplayCustomAlert(title, message, ctaButtons, UITextAlignment.Left
                , UITextAlignment.Left, true, 0.056F, false, image);
        }

        public void DisplayPermission(PermissionMode pMode)
        {
            string title = string.Empty;
            string description = string.Empty;
            if (pMode == PermissionMode.Camera)
            {
                title = GetCommonI18NValue(CustomViewControllerConstants.I18N_CameraPermissionTitle);
                description = GetCommonI18NValue(CustomViewControllerConstants.I18N_PhotoPermissionDescription);
            }
            else if (pMode == PermissionMode.Gallery)
            {
                title = GetCommonI18NValue(CustomViewControllerConstants.I18N_GalleryPermissionTitle);
                description = GetCommonI18NValue(CustomViewControllerConstants.I18N_PhotoPermissionDescription);
            }

            UIAlertController alert = UIAlertController.Create(title, description, UIAlertControllerStyle.Alert);
            alert.AddAction(UIAlertAction.Create(GetCommonI18NValue(CustomViewControllerConstants.I18N_Cancel), UIAlertActionStyle.Cancel, null));
            alert.AddAction(UIAlertAction.Create(GetCommonI18NValue(CustomViewControllerConstants.I18N_Settings), UIAlertActionStyle.Default, (sender) =>
            {
                NSUrl url = new NSUrl(UIApplication.OpenSettingsUrlString);
                if (url != null && UIApplication.SharedApplication.CanOpenUrl(url))
                {
                    UIApplication.SharedApplication.OpenUrl(url);
                }
            }));
            PresentViewController(alert, true, null);
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
            UIView gradientView = new UIView(new CGRect(0, 0, View.Frame.Width, View.Frame.Height * (IsFullGradient ? 1.0F : 0.50F)));
            CGColor startColor = (IsReversedGradient ? MyTNBColor.GradientPurpleDarkElement : MyTNBColor.GradientPurpleLightElement).CGColor;
            CGColor endColor = (IsReversedGradient ? MyTNBColor.GradientPurpleLightElement : MyTNBColor.GradientPurpleDarkElement).CGColor;
            CAGradientLayer gradientLayer = new CAGradientLayer
            {
                Colors = new[] { startColor, endColor }
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
        public virtual void ConfigureNavigationBar() { }
        #endregion

        #region Utilities
        #region I18N
        public string GetI18NValue(string key)
        {
            return I18NDictionary != null && I18NDictionary.ContainsKey(key) ? I18NDictionary[key] : string.Empty;
        }
        public string GetCommonI18NValue(string key)
        {
            return DataManager.DataManager.SharedInstance.CommonI18NDictionary != null
                && DataManager.DataManager.SharedInstance.CommonI18NDictionary.ContainsKey(key)
                ? DataManager.DataManager.SharedInstance.CommonI18NDictionary[key] : string.Empty;
        }
        public string GetHintI18NValue(string key)
        {
            return DataManager.DataManager.SharedInstance.HintI18NDictionary != null
                && DataManager.DataManager.SharedInstance.HintI18NDictionary.ContainsKey(key)
                ? DataManager.DataManager.SharedInstance.HintI18NDictionary[key] : string.Empty;
        }
        public string GetErrorI18NValue(string key)
        {
            return DataManager.DataManager.SharedInstance.ErrorI18NDictionary != null
                && DataManager.DataManager.SharedInstance.ErrorI18NDictionary.ContainsKey(key)
                ? DataManager.DataManager.SharedInstance.ErrorI18NDictionary[key] : string.Empty;
        }
        #endregion
        public nfloat GetScaledWidth(nfloat value)
        {
            nfloat percentage = value / WidthBase;
            return UIScreen.MainScreen.Bounds.Width * percentage;
        }
        public nfloat GetScaledHeight(nfloat value)
        {
            nfloat percentage = value / HeightBase;
            return UIScreen.MainScreen.Bounds.Height * percentage;
        }
        #endregion
    }
}
