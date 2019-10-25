using Foundation;
using System;
using UIKit;
using CoreGraphics;
using System.Threading.Tasks;
using myTNB.Model;
using CoreAnimation;
using myTNB.Logout;

namespace myTNB
{
    public partial class LogoutViewController : CustomUIViewController
    {
        public LogoutViewController(IntPtr handle) : base(handle) { }

        public override void ViewDidLoad()
        {
            PageName = LogoutConstants.Pagename_Logout;
            base.ViewDidLoad();
            NavigationController.NavigationBar.Hidden = true;
            SetupSuperViewBackground();
            AddCTA();
            SetSubView();
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            UIApplication.SharedApplication.ApplicationIconBadgeNumber = 0;
            ExecuteLogout();
        }

        private void SetSubView()
        {
            UIView viewContent = new UIView(new CGRect(18, DeviceHelper.IsIphoneXUpResolution() ? 60 : 36, View.Frame.Width - 36, 240));
            viewContent.BackgroundColor = UIColor.White;
            viewContent.Layer.CornerRadius = 5.0f;

            UIImageView imgLogo = new UIImageView(new CGRect((viewContent.Frame.Width / 2) - 75, 16, 150, 150));
            imgLogo.Image = UIImage.FromBundle(LogoutConstants.IMG_Logout);

            UILabel lblThankYou = new UILabel(new CGRect(0, 182, viewContent.Frame.Width, 18));
            lblThankYou.TextColor = MyTNBColor.PowerBlue;
            lblThankYou.Font = MyTNBFont.MuseoSans16;
            lblThankYou.Text = GetI18NValue(LogoutConstants.I18N_Title);
            lblThankYou.TextAlignment = UITextAlignment.Center;

            UILabel lblSubTitle = new UILabel(new CGRect(24, 200, viewContent.Frame.Width - 48, 16));
            lblSubTitle.Font = MyTNBFont.MuseoSans12;
            lblSubTitle.TextColor = MyTNBColor.TunaGrey();
            lblSubTitle.TextAlignment = UITextAlignment.Center;
            lblSubTitle.Text = GetI18NValue(LogoutConstants.I18N_Message);

            viewContent.AddSubviews(new UIView[] { imgLogo, lblThankYou, lblSubTitle });
            View.AddSubview(viewContent);
        }

        private void AddCTA()
        {
            UIButton btnCTA = new UIButton(UIButtonType.Custom)
            {
                Frame = new CGRect(18, View.Frame.Height - (DeviceHelper.IsIphoneXUpResolution() ? 96 : 72), View.Frame.Width - 36, 48),
                Font = MyTNBFont.MuseoSans16,
                BackgroundColor = MyTNBColor.FreshGreen
            };
            btnCTA.Layer.BorderWidth = 1;
            btnCTA.SetTitle(GetI18NValue(LogoutConstants.I18N_LoginAgain), UIControlState.Normal);
            btnCTA.Layer.CornerRadius = 4;
            btnCTA.Layer.BorderColor = MyTNBColor.FreshGreen.CGColor;
            btnCTA.SetTitleColor(UIColor.White, UIControlState.Normal);
            btnCTA.TouchUpInside += (sender, e) =>
            {
                DataManager.DataManager.SharedInstance.ClearLoginState();
                BackToHome();
            };
            View.AddSubview(btnCTA);
        }

        private void ExecuteLogout()
        {
            ActivityIndicator.Show();
            LogoutUser().ContinueWith(task =>
            {
                InvokeOnMainThread(() =>
                {
                    DataManager.DataManager.SharedInstance.ClearLoginState();
                    ActivityIndicator.Hide();
                });
            });
        }

        private void BackToHome()
        {
            UIStoryboard loginStoryboard = UIStoryboard.FromName("Login", null);
            UIViewController preloginVC = loginStoryboard.InstantiateViewController("PreloginViewController");
            preloginVC.ModalTransitionStyle = UIModalTransitionStyle.CrossDissolve;
            preloginVC.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
            PresentViewController(preloginVC, true, null);
        }

        private Task LogoutUser()
        {
            return Task.Factory.StartNew(() =>
            {
                ServiceManager serviceManager = new ServiceManager();
                object requestParameter = new
                {
                    ApiKeyID = TNBGlobal.API_KEY_ID,
                    Email = DataManager.DataManager.SharedInstance.UserEntity[0].email,
                    DeviceId = DataManager.DataManager.SharedInstance.UDID,
                    AppVersion = AppVersionHelper.GetBuildVersion(),
                    OsType = TNBGlobal.DEVICE_PLATFORM_IOS,
                    OsVersion = DeviceHelper.GetOSVersion()
                };
                BaseResponseModel logoutResponse = serviceManager.BaseServiceCall(LogoutConstants.Service_Logout, requestParameter);
            });
        }

        private void SetupSuperViewBackground()
        {
            UIColor startColor = MyTNBColor.GradientPurpleDarkElement;
            UIColor endColor = MyTNBColor.GradientPurpleLightElement;
            CAGradientLayer gradientLayer = new CAGradientLayer
            {
                Colors = new[] { startColor.CGColor, endColor.CGColor },
                Locations = new NSNumber[] { 0, 1 },
                Frame = View.Bounds
            };
            View.Layer.InsertSublayer(gradientLayer, 0);
        }
    }
}