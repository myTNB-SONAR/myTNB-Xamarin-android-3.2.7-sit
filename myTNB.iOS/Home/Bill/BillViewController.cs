using System;
using myTNB.Home.Bill;
using UIKit;

namespace myTNB
{
    public partial class BillViewController : CustomBillUIViewController
    {

        public BillViewController(IntPtr handle) : base(handle)
        {

        }

        public override void ViewDidLoad()
        {
            foreach (UIView v in View.Subviews)
            {
                v.RemoveFromSuperview();
            }
            NavigationController.NavigationBarHidden = false;
            PageName = BillConstants.Pagename_Bills;
            base.ViewDidLoad();
            View.BackgroundColor = UIColor.White;
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            NavigationController.SetNavigationBarHidden(true, true);
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
            NavigationController.SetNavigationBarHidden(false, true);
        }
    }
}