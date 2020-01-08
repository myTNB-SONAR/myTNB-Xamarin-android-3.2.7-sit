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
            UIView viewContent = new UIView(new CGRect(18, DeviceHelper.IsIphoneXUpResolution() ? 60 : 36, View.Frame.Width - 36, 240))
            {
                BackgroundColor = UIColor.White
            };
            viewContent.Layer.CornerRadius = 5.0f;

            UIImageView imgLogo = new UIImageView(new CGRect((viewContent.Frame.Width / 2) - 75, 16, 150, 150))
            {
                Image = UIImage.FromBundle(LogoutConstants.IMG_Logout)
            };

            UILabel lblThankYou = new UILabel(new CGRect(0, 182, viewContent.Frame.Width, 18))
            {
                TextColor = MyTNBColor.PowerBlue,
                Font = MyTNBFont.MuseoSans16,
                Text = GetI18NValue(LogoutConstants.I18N_Title),
                TextAlignment = UITextAlignment.Center,
                LineBreakMode = UILineBreakMode.WordWrap,
                Lines = 0
            };

            nfloat newLblThankyouHeight = lblThankYou.GetLabelHeight(50);
            lblThankYou.Frame = new CGRect(lblThankYou.Frame.Location, new CGSize(lblThankYou.Frame.Width, newLblThankyouHeight));

            UILabel lblSubTitle = new UILabel(new CGRect(16, lblThankYou.Frame.GetMaxY(), viewContent.Frame.Width - 32, 16))
            {
                Font = MyTNBFont.MuseoSans12,
                TextColor = MyTNBColor.TunaGrey(),
                TextAlignment = UITextAlignment.Center,
                Text = GetI18NValue(LogoutConstants.I18N_Message),
                LineBreakMode = UILineBreakMode.WordWrap,
                Lines = 0
            };

            nfloat newLblSubHeight = lblSubTitle.GetLabelHeight(50);
            lblSubTitle.Frame = new CGRect(lblSubTitle.Frame.Location, new CGSize(lblSubTitle.Frame.Width, newLblSubHeight));

            viewContent.AddSubviews(new UIView[] { imgLogo, lblThankYou, lblSubTitle });
            viewContent.Frame = new CGRect(viewContent.Frame.Location, new CGSize(viewContent.Frame.Width, lblSubTitle.Frame.GetMaxY() + 24));
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
                    serviceManager.usrInf,
                    serviceManager.deviceInf
                };
                BaseResponseModelV2 logoutResponse = serviceManager.BaseServiceCallV6(LogoutConstants.Service_Logout, requestParameter);
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