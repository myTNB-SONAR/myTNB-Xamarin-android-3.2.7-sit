using CoreGraphics;
using Foundation;
using myTNB.SitecoreCMS.Model;
using myTNB.SQLite.SQLiteDataManager;
using System;
using System.Collections.Generic;
using UIKit;

namespace myTNB
{
    public partial class UsageViewController : CustomUIViewController
    {
        public UsageViewController(IntPtr handle) : base(handle) { }

        UIView _navbarContainer, _energyTipsContainer;

        nfloat titleBarHeight = 24f;

        public override void ViewDidLoad()
        {
            PageName = "UsageView";
            IsGradientRequired = true;
            IsFullGradient = true;
            base.ViewDidLoad();
            NavigationController.NavigationBarHidden = true;
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            SetNavigation();
            SetEnergyTipsComponent();
        }

        private void SetNavigation()
        {
            _navbarContainer = new UIView(new CGRect(0, 0, ViewWidth, DeviceHelper.GetStatusBarHeight() + NavigationController.NavigationBar.Frame.Height))
            {
                BackgroundColor = UIColor.Clear
            };
            UIView viewTitleBar = new UIView(new CGRect(0, DeviceHelper.GetStatusBarHeight() + 8f, _navbarContainer.Frame.Width, titleBarHeight));

            UILabel lblTitle = new UILabel(new CGRect(58, 0, _navbarContainer.Frame.Width - 116, titleBarHeight))
            {
                Font = TNBFont.MuseoSans_16_500,
                Text = "Usage"
            };

            lblTitle.TextAlignment = UITextAlignment.Center;
            lblTitle.TextColor = UIColor.White;
            viewTitleBar.AddSubview(lblTitle);

            UIView viewBack = new UIView(new CGRect(18, 0, 24, titleBarHeight));
            UIImageView imgViewBack = new UIImageView(new CGRect(0, 0, 24, titleBarHeight))
            {
                Image = UIImage.FromBundle("Back-White")
            };
            viewBack.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                DismissViewController(true, null);
            }));
            viewBack.AddSubview(imgViewBack);
            viewTitleBar.AddSubview(viewBack);
            _navbarContainer.AddSubview(viewTitleBar);
            View.AddSubview(_navbarContainer);
        }

        private void SetEnergyTipsComponent()
        {
            List<TipsModel> tipsList;
            EnergyTipsEntity wsManager = new EnergyTipsEntity();
            tipsList = wsManager.GetAllItems();

            _energyTipsContainer = new UIView(new CGRect(0, 100f, View.Frame.Width, 150f))
            {
                BackgroundColor = UIColor.Clear
            };
            View.AddSubview(_energyTipsContainer);

            EnergyTipsComponent energyTipsComponent = new EnergyTipsComponent(_energyTipsContainer, tipsList);
            _energyTipsContainer.AddSubview(energyTipsComponent.GetUI());
        }
    }
}