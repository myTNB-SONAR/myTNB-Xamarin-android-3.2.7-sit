using Foundation;
using System;
using UIKit;
using CoreGraphics;
using myTNB.Home.Feedback;

namespace myTNB
{
    public partial class SelectAccountNoViewController : UIViewController
    {
        public SelectAccountNoViewController (IntPtr handle) : base (handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            this.Title = "Select Account No";

            AccountsTableView.Source = new SelectAccountNoDataSource(this);
            AccountsTableView.RowHeight = 56f;
            AccountsTableView.TableFooterView = new UIView(new CGRect(0, 0, 0, 0));

            //AccountsTableView.ScrollEnabled = AccountsTableView.ContentSize.Height < AccountsTableView.Frame.Size.Height ? false : true;
            AccountsTableView.ScrollEnabled = true;
            this.NavigationItem.HidesBackButton = true;
            AddBackButton();
        }

        internal void AddBackButton()
        {
            UIImage backImg = UIImage.FromBundle("Back-White");
            UIBarButtonItem btnBack = new UIBarButtonItem(backImg, UIBarButtonItemStyle.Done, (sender, e) => {
                this.NavigationController.PopViewController(true);
            });
            this.NavigationItem.LeftBarButtonItem = btnBack;
        }

 
    }
}