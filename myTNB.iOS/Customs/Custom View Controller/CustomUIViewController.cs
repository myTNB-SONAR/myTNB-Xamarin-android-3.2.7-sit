using System;
using System.Collections.Generic;
using System.Diagnostics;
using CoreAnimation;
using CoreGraphics;
using Firebase.Analytics;
using Foundation;
using myTNB.Customs;
using UIKit;

namespace myTNB
{
    public class CustomUIViewController : UIViewController
    {
        internal Dictionary<string, string> I18NDictionary;
        internal string PageName;
        internal bool IsGradientRequired, IsFullGradient, IsReversedGradient, IsNewGradientRequired;
        internal UIImageView ImageViewGradientImage;
        internal UIView _statusBarView, _customNavBar;
        internal nfloat ViewWidth, ViewHeight, BaseMargin, BaseMarginedWidth;
        internal UILabel LblNavTitle;

        private UIView _viewToast, _viewToastOverlay;
        private UILabel _lblToastDetails;

        private bool _isAnimating;



        public enum PermissionMode
        {
            Camera,
            Gallery
        }
        public CustomUIViewController() { }

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
            if (IsNewGradientRequired)
            {
                CreateNewBackgroundGradient();
            }
            SetFrames();
            ConfigureNavigationBar();
            NotifCenterUtility.AddObserver((NSString)"LanguageDidChange", LanguageDidChange);
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            Analytics.SetScreenNameAndClass(PageName, string.Format("{0}ViewController", PageName));
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
        }

        protected virtual void LanguageDidChange(NSNotification notification)
        {
            Debug.WriteLine("DEBUG >>> LanguageDidChange: " + PageName);
            I18NDictionary = LanguageManager.Instance.GetValuesByPage(PageName);
        }

        #endregion
        #region Widget Utilities
        public CGSize GetLabelSize(UILabel label, nfloat width, nfloat height)
        {
            return CustomUILabel.GetLabelSize(label, width, height);
        }

        public CGSize GetLabelSize(UILabel label, nfloat height)
        {
            return CustomUILabel.GetLabelSize(label, label.Frame.Width, height);
        }

        public void SetFrames()
        {
            ViewWidth = View.Frame.Width;
            ViewHeight = View.Frame.Height;
            if (NavigationController != null && !NavigationController.NavigationBarHidden && NavigationController.NavigationBar != null)
            {
                nfloat navBarHeight = NavigationController.NavigationBar.GetFrame().Height;
                NavigationController.NavigationBar.Frame = new CGRect(NavigationController.NavigationBar.Frame.Location
                    , new CGSize(NavigationController.NavigationBar.Frame.Width, navBarHeight));// GetScaledHeight(navBarHeight)));
                ViewHeight -= NavigationController.NavigationBar.Frame.Height;
            }
            if (TabBarController != null && TabBarController.TabBar != null && !TabBarController.TabBar.Hidden)
            {
                ViewHeight -= (View.Frame.Height - TabBarController.TabBar.Frame.GetMinY());
            }
            else
            {
                ViewHeight -= GetBottomPadding;
            }
            ViewHeight -= DeviceHelper.GetStatusBarHeight();
            BaseMargin = GetScaledWidth(16);
            BaseMarginedWidth = ViewWidth - (BaseMargin * 2);
        }

        public nfloat GetBottomPadding
        {
            get
            {
                try
                {
                    return DeviceHelper.BottomSafeAreaInset;
                }
                catch (MonoTouchException m) { Debug.WriteLine("Error in Bottom Safe Area Inset: " + m.Message); }
                catch (Exception e)
                {
                    Debug.WriteLine("Error in Bottom Safe Area Inset: " + e.Message);
                    if (DeviceHelper.IsIphoneXUpResolution())
                    {
                        return 20;
                    }
                }
                return 0;
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

        public void DisplayCustomAlert(string title, string message, Dictionary<string, Action> ctaButtons
            , UITextAlignment titleAlignment, UITextAlignment messageAlignment)
        {
            AlertHandler.DisplayCustomAlert(title, message, ctaButtons, titleAlignment, messageAlignment);
        }

        public void DisplayCustomAlert(string title, string message, string btnTitle, Action btnAction = null)
        {
            AlertHandler.DisplayCustomAlert(title, message, new Dictionary<string, Action>() {
                { string.IsNullOrEmpty(btnTitle) ? GetCommonI18NValue(Constants.Common_Ok) : btnTitle, btnAction } }
            , UITextAlignment.Left, UITextAlignment.Left, true, 0.056F, false);
        }

        public void DisplayCustomAlert(string title, string message, Dictionary<string, Action> ctaButtons, UIImage image)
        {
            AlertHandler.DisplayCustomAlert(title, message, ctaButtons, UITextAlignment.Left
                , UITextAlignment.Left, true, 0.056F, false, image);
        }

        public void DisplayCustomAlert(string title, string message, Dictionary<string, Action> ctaButtons, bool isDefaultURLAction)
        {
            AlertHandler.DisplayCustomAlert(title, message, ctaButtons, UITextAlignment.Left
                , UITextAlignment.Left, true, 0.056F, isDefaultURLAction);
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
            alert.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
            PresentViewController(alert, true, null);
        }
        #endregion
        #region Toast
        public void DisplayToast(string message, bool isWindow = false)
        {
            UIWindow currentWindow = UIApplication.SharedApplication.KeyWindow;

            if (_viewToast == null)
            {
                _viewToast = new UIView(new CGRect(GetScaledWidth(18F), GetScaledHeight(32F), ViewWidth - GetScaledWidth(36F), GetScaledHeight(48F)))
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

                _lblToastDetails = new UILabel(new CGRect(BaseMarginWidth16, BaseMarginHeight16, _viewToast.Frame.Width - (BaseMarginWidth16 * 2)
                    , (BaseMarginHeight16 * 2)))
                {
                    TextAlignment = UITextAlignment.Left,
                    Font = TNBFont.MuseoSans_12_300,
                    TextColor = MyTNBColor.CharcoalGrey,
                    Lines = 0,
                    LineBreakMode = UILineBreakMode.WordWrap
                };

                _viewToast.AddSubview(_lblToastDetails);
                AddSwipeGestureForToast();
                if (isWindow)
                {
                    currentWindow.AddSubviews(new UIView[] { _viewToast, _viewToastOverlay });
                }
                else
                {
                    View.AddSubviews(new UIView[] { _viewToast, _viewToastOverlay });
                }
            }
            _lblToastDetails.Text = message ?? string.Empty;

            CGSize size = _lblToastDetails.SizeThatFits(new CGSize(_lblToastDetails.Frame.Width, _lblToastDetails.Frame.Height));
            _lblToastDetails.Frame = new CGRect(_lblToastDetails.Frame.X
                , _lblToastDetails.Frame.Y, _lblToastDetails.Frame.Width, size.Height);
            _viewToast.Frame = new CGRect(_viewToast.Frame.X
                , 0, _viewToast.Frame.Width, 0);
            _viewToastOverlay.Frame = new CGRect(_viewToast.Frame.X
                , GetScaledHeight(32F), _viewToast.Frame.Width, size.Height + GetScaledHeight(32F));

            _viewToast.Hidden = false;
            if (isWindow)
            {
                currentWindow.BringSubviewToFront(_viewToast);
            }
            else
            {
                View.BringSubviewToFront(_viewToast);
            }
            _viewToastOverlay.Hidden = false;
            if (!_isAnimating)
            {
#pragma warning disable XI0001 // Notifies you with advices on how to use Apple APIs
                UIView.Animate(0.3, 0.3, UIViewAnimationOptions.CurveEaseOut, () =>
                 {
                     _isAnimating = true;
                     _viewToast.Frame = new CGRect(_viewToast.Frame.X
                    , GetScaledHeight(32F), _viewToast.Frame.Width, size.Height + GetScaledHeight(32F));
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
         
        private void CreateNewBackgroundGradient()
        {
            UIView gradientView = new UIView(new CGRect(0, 0, View.Frame.Width, View.Frame.Height));
            CGColor startColor = MyTNBColor.LightIndigo.CGColor;
            CGColor endColor = MyTNBColor.ClearBlue.CGColor;
            CAGradientLayer gradientLayer = new CAGradientLayer
            {
                Colors = new[] { startColor, endColor }
            };
            gradientLayer.Locations = new NSNumber[] { 0, 1 };
            gradientLayer.Frame = gradientView.Bounds;
            gradientView.Layer.InsertSublayer(gradientLayer, 0);
            View.AddSubview(gradientView);
        }
        #endregion
        #region Customize View
        public virtual void SetStatusBarNoOverlap()
        {
            _statusBarView = new UIView(new CGRect(0, 0, View.Frame.Width, DeviceHelper.GetStatusBarHeight()));
            View.AddSubview(_statusBarView);
        }
        public virtual void ConfigureNavigationBar()
        {
            if (NavigationItem != null)
            {
                NavigationItem.HidesBackButton = true;
            }
        }
        public virtual void AddCustomNavBar(Action backAction = null)
        {
            if (NavigationController != null) { NavigationController.SetNavigationBarHidden(true, true); }
            UIView viewBack = new UIView(new CGRect(GetScaledWidth(16), 0, GetScaledWidth(24), GetScaledHeight(24)));
            viewBack.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                if (backAction != null) { backAction.Invoke(); }
                else { if (NavigationController != null) { NavigationController.PopViewController(true); } }
            }));

            UIImageView imgBack = new UIImageView(new CGRect(new CGPoint(0, 0), viewBack.Frame.Size))
            {
                Image = UIImage.FromBundle(CustomViewControllerConstants.IMG_Back)
            };
            viewBack.AddSubview(imgBack);

            LblNavTitle = new UILabel(new CGRect(viewBack.Frame.GetMaxX(), 0, ViewWidth - (viewBack.Frame.GetMaxX() * 2), GetScaledHeight(24)))
            {
                TextAlignment = UITextAlignment.Center,
                Font = TNBFont.MuseoSans_16_500,
                TextColor = UIColor.White,
                Text = Title
            };

            _customNavBar = new UIView(new CGRect(0, GetScaledHeight(28), ViewWidth, GetScaledHeight(24)));
            _customNavBar.AddSubviews(new UIView[] { viewBack, LblNavTitle });
            View.AddSubview(_customNavBar);
        }
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
        #region Scale Utility
        public nfloat GetScaledWidth(nfloat value)
        {
            return ScaleUtility.GetScaledWidth(value);
        }
        public nfloat GetScaledHeight(nfloat value)
        {
            return ScaleUtility.GetScaledHeight(value);
        }
        public static nfloat GetHeightByScreenSize(nfloat value)
        {
            return ScaleUtility.GetHeightByScreenSize(value);
        }
        public void GetYLocationFromFrame(CGRect frame, ref nfloat yValue)
        {
            ScaleUtility.GetYLocationFromFrame(frame, ref yValue);
        }
        public void GetYLocationFromFrame(CGRect frame, nfloat yValue, out nfloat scaledValue)
        {
            ScaleUtility.GetYLocationFromFrame(frame, ref yValue);
            scaledValue = yValue;
        }
        public nfloat GetYLocationFromFrame(CGRect frame, nfloat yValue)
        {
            ScaleUtility.GetYLocationFromFrame(frame, ref yValue);
            return yValue;
        }
        public nfloat GetXLocationFromFrame(CGRect frame, nfloat xValue)
        {
            ScaleUtility.GetXLocationFromFrame(frame, ref xValue);
            return xValue;
        }
        public nfloat GetYLocationToCenterObject(nfloat height, UIView view = null)
        {
            return ScaleUtility.GetYLocationToCenterObject(height, view);
        }
        public nfloat GetXLocationToCenterObject(nfloat width, UIView view = null)
        {
            return ScaleUtility.GetXLocationToCenterObject(width, view);
        }
        public void GetValuesFromAspectRatio(ref nfloat width, ref nfloat height)
        {
            ScaleUtility.GetValuesFromAspectRatio(ref width, ref height);
        }
        public nfloat BaseMarginWidth8
        {
            get { return ScaleUtility.BaseMarginWidth8; }
        }
        public nfloat BaseMarginWidth12
        {
            get { return ScaleUtility.BaseMarginWidth12; }
        }
        public nfloat BaseMarginWidth16
        {
            get { return ScaleUtility.BaseMarginWidth16; }
        }
        public nfloat BaseMarginHeight16
        {
            get { return ScaleUtility.BaseMarginHeight16; }
        }
        #endregion
        #endregion
    }
}