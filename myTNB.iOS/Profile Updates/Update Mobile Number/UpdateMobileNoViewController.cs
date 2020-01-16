using System;
using CoreGraphics;
using myTNB.MyAccount;
using UIKit;

namespace myTNB
{
    public class UpdateMobileNoViewController : CustomUIViewController
    {
        public bool IsFromLogin { set; private get; }

        public override void ViewDidLoad()
        {
            PageName = MyAccountConstants.Pagename_UpdateMobileNumber;
            base.ViewDidLoad();
            View.BackgroundColor = MyTNBColor.LightGrayBG;
            SetNavigationBar();
            SetViews();
            SetCTA();
        }

        private void SetNavigationBar()
        {
            Title = GetI18NValue(IsFromLogin ? MyAccountConstants.I18N_VerifyDeviceTitle : MyAccountConstants.I18N_UpdateMobileTitle);
            UIBarButtonItem btnBack = new UIBarButtonItem(UIImage.FromBundle(Constants.IMG_Back), UIBarButtonItemStyle.Done, (sender, e) =>
            {
                DismissViewController(true, null);
            });
            NavigationItem.LeftBarButtonItem = btnBack;
        }

        private void SetViews()
        {
            UIView cardView = new UIView(new CGRect(0, 0, View.Frame.Width, GetScaledHeight(123))) { BackgroundColor = UIColor.White };
            cardView.Layer.BorderColor = UIColor.Red.CGColor;
            cardView.Layer.BorderWidth = 1;



            View.AddSubview(cardView);
        }

        private void SetCTA()
        {
            nfloat containerHeight = GetScaledHeight(80) + DeviceHelper.BottomSafeAreaInset;
            nfloat yLoc = View.Frame.Height - DeviceHelper.TopSafeAreaInset - NavigationController.NavigationBar.Frame.Height - containerHeight;

            UIView cardView = new UIView(new CGRect(0, yLoc, View.Frame.Width, containerHeight)) { BackgroundColor = UIColor.White };
            cardView.Layer.BorderColor = UIColor.Red.CGColor;
            cardView.Layer.BorderWidth = 1;

            CustomUIButtonV2 btnNext = new CustomUIButtonV2()
            {
                Frame = new CGRect(BaseMargin, BaseMargin, BaseMarginedWidth, GetScaledHeight(48)),
                BackgroundColor = MyTNBColor.SilverChalice
            };

            cardView.AddSubview(btnNext);
            View.AddSubview(cardView);
        }
    }
}