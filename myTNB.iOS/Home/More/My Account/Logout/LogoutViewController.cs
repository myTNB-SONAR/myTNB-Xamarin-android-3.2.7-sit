using Foundation;
using System;
using UIKit;
using CoreGraphics;
using System.Threading.Tasks;
using myTNB.Model;
using CoreAnimation;

namespace myTNB
{
    public partial class LogoutViewController : UIViewController
    {
        public LogoutViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            NavigationController.NavigationBar.Hidden = true;
            SetupSuperViewBackground();
            AddCTA();
            SetSubView();
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            ExecuteLogout();
            UIApplication.SharedApplication.ApplicationIconBadgeNumber = 0;
        }

        internal void SetSubView()
        {
            UIView viewContent = new UIView(new CGRect(18, DeviceHelper.IsIphoneXUpResolution() ? 60 : 36, View.Frame.Width - 36, 240));
            viewContent.BackgroundColor = UIColor.White;
            viewContent.Layer.CornerRadius = 5.0f;

            UIImageView imgLogo = new UIImageView(new CGRect((viewContent.Frame.Width / 2) - 75, 16, 150, 150));
            imgLogo.Image = UIImage.FromBundle("Logout-Logo");

            UILabel lblThankYou = new UILabel(new CGRect(0, 182, viewContent.Frame.Width, 18));
            lblThankYou.TextColor = myTNBColor.PowerBlue();
            lblThankYou.Font = myTNBFont.MuseoSans16();
            lblThankYou.Text = "Logout_ThankYouMessage".Translate();
            lblThankYou.TextAlignment = UITextAlignment.Center;

            UILabel lblSubTitle = new UILabel(new CGRect(24, 200, viewContent.Frame.Width - 48, 16));
            lblSubTitle.Font = myTNBFont.MuseoSans12();
            lblSubTitle.TextColor = myTNBColor.TunaGrey();
            lblSubTitle.TextAlignment = UITextAlignment.Center;
            lblSubTitle.Text = "Logout_Message".Translate();

            viewContent.AddSubviews(new UIView[] { imgLogo, lblThankYou, lblSubTitle });
            View.AddSubview(viewContent);
        }

        internal void AddCTA()
        {
            UIButton btnCTA = new UIButton(UIButtonType.Custom);
            btnCTA.Frame = new CGRect(18, View.Frame.Height - (DeviceHelper.IsIphoneXUpResolution() ? 96 : 72), View.Frame.Width - 36, 48);
            btnCTA.Layer.CornerRadius = 4;
            btnCTA.Layer.BorderColor = myTNBColor.FreshGreen().CGColor;
            btnCTA.BackgroundColor = myTNBColor.FreshGreen();
            btnCTA.Layer.BorderWidth = 1;
            btnCTA.SetTitle("Logout_BackToHome".Translate(), UIControlState.Normal);
            btnCTA.Font = myTNBFont.MuseoSans16();
            btnCTA.SetTitleColor(UIColor.White, UIControlState.Normal);
            btnCTA.TouchUpInside += (sender, e) =>
            {
                DataManager.DataManager.SharedInstance.ClearLoginState();
                BackToHome();
            };
            View.AddSubview(btnCTA);
        }

        internal void ExecuteLogout()
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

        internal void BackToHome()
        {
            UIStoryboard loginStoryboard = UIStoryboard.FromName("Login", null);
            UIViewController preloginVC = (UIViewController)loginStoryboard.InstantiateViewController("PreloginViewController");
            preloginVC.ModalTransitionStyle = UIModalTransitionStyle.CrossDissolve;
            PresentViewController(preloginVC, true, null);
        }

        internal Task LogoutUser()
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
                BaseResponseModel logoutResponse = serviceManager.BaseServiceCall("LogoutUser_V2", requestParameter);
            });
        }

        internal void SetupSuperViewBackground()
        {
            var startColor = myTNBColor.GradientPurpleDarkElement();
            var endColor = myTNBColor.GradientPurpleLightElement();
            var gradientLayer = new CAGradientLayer();
            gradientLayer.Colors = new[] { startColor.CGColor, endColor.CGColor };
            gradientLayer.Locations = new NSNumber[] { 0, 1 };
            gradientLayer.Frame = View.Bounds;
            View.Layer.InsertSublayer(gradientLayer, 0);
        }
    }
}