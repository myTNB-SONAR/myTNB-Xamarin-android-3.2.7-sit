using Foundation;
using System;
using UIKit;
using myTNB.Registration;
using CoreGraphics;

namespace myTNB
{
    public partial class SelectAccountTypeViewController : UIViewController
    {
        public SelectAccountTypeViewController (IntPtr handle) : base (handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            SelectAccounTypeTableView.Source = new SelectAccountTypeDataSource(this);
            SelectAccounTypeTableView.TableFooterView = new UIView(new CGRect (0, 0, 0, 0));

            SelectAccounTypeTableView.ScrollEnabled = SelectAccounTypeTableView.ContentSize.Height < SelectAccounTypeTableView.Frame.Size.Height ? false : true;

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