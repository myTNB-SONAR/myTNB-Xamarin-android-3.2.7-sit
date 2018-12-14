using System;
using UIKit;
using myTNB.PushNotification;

namespace myTNB
{
    public partial class SelectNotificationViewController : UITableViewController
    {
        public SelectNotificationViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.
            SetNavigationBar();
            selectNotificationTableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            selectNotificationTableView.Source = new SelectNotificationDataSource(this, DataManager.DataManager.SharedInstance.NotificationGeneralTypes);
            selectNotificationTableView.ReloadData();
        }

        internal void SetNavigationBar()
        {
            NavigationItem.HidesBackButton = true;
            NavigationItem.Title = "Select Notification";
            UIImage backImg = UIImage.FromBundle("Back-White");
            UIBarButtonItem btnBack = new UIBarButtonItem(backImg, UIBarButtonItemStyle.Done, (sender, e) =>
            {
                DismissVC();
            });
            NavigationItem.LeftBarButtonItem = btnBack;
        }

        internal void DismissVC()
        {
            DismissViewController(true, null);
        }
    }
}