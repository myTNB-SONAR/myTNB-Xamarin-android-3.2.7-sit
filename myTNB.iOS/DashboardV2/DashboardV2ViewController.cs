using Foundation;
using myTNB.DashboardV2;
using System;
using System.Diagnostics;
using UIKit;

namespace myTNB
{
    public partial class DashboardV2ViewController : CustomUIViewController
    {
        public DashboardV2ViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            ConfigureNavigationBar();
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
            NavigationController.SetNavigationBarHidden(true, animated);
        }

        public override void ConfigureNavigationBar()
        {
            NavigationController.SetNavigationBarHidden(true, true);

            NavigationItem.LeftBarButtonItem = new UIBarButtonItem(UIImage.FromBundle(DashboardConstants.IMG_Back)
                , UIBarButtonItemStyle.Done, (sender, e) =>
                {
                    NavigationController.PopViewController(true);
                });
            Title = "Usage";
        }
    }
}