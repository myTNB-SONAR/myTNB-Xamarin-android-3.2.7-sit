using CoreGraphics;
using System;
using UIKit;

namespace myTNB
{
    public partial class AccountsContentViewController : UIViewController
    {
        public int pageIndex = 0;

        public AccountsContentViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            UIView viewContainer = new UIView(new CGRect(0, 0, View.Frame.Width, 395f));
            viewContainer.BackgroundColor = UIColor.Clear;

            for (int i = 0; i < 5; i++)
            {
                DashboardHomeAccountCard _homeAccountCard = new DashboardHomeAccountCard(View, 68f * i);
                viewContainer.AddSubview(_homeAccountCard.GetUI());
            }
            View.AddSubview(viewContainer);
        }

        public override void ViewDidLayoutSubviews()
        {
            base.ViewDidLayoutSubviews();
            View.BackgroundColor = UIColor.Clear;
        }

    }
}