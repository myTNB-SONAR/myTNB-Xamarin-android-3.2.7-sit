using System;
using UIKit;
using myTNB.Dashboard.SelectAccounts;
using CoreGraphics;
using myTNB.Home.Dashboard.SelectAccounts;

namespace myTNB
{
    public partial class SelectAccountTableViewController : CustomUIViewController
    {
        public SelectAccountTableViewController(IntPtr handle) : base(handle) { }

        public bool IsFromSSMR;
        public bool IsRoot;

        public override void ViewDidLoad()
        {
            PageName = SelectAccountConstants.PageName;
            base.ViewDidLoad();
            AddBackButton();
            accountRecordsTableView.Frame = new CGRect(0, 0, View.Frame.Width
                , View.Frame.Height - (DeviceHelper.IsIphoneXUpResolution() ? 64 + 72 + 24 : 64 + 72));
            accountRecordsTableView.Source = new SelectAccountsDataSource(this);
            accountRecordsTableView.ReloadData();
            accountRecordsTableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
        }

        private void AddBackButton()
        {
            Title = GetI18NValue(SelectAccountConstants.I18N_NavTitle);
            NavigationItem.HidesBackButton = true;
            UIImage backImg = UIImage.FromBundle("Back-White");
            UIBarButtonItem btnBack = new UIBarButtonItem(backImg, UIBarButtonItemStyle.Done, (sender, e) =>
            {
                if (IsRoot)
                { NavigationController.PopViewController(true); }
                else
                { DismissViewController(true, null); }
            });
            NavigationItem.LeftBarButtonItem = btnBack;
        }
    }
}