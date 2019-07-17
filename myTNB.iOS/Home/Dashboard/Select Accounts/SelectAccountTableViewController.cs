using System;
using UIKit;
using myTNB.Dashboard.SelectAccounts;
using myTNB.Model;
using System.Threading.Tasks;
using CoreGraphics;
using myTNB.Registration.CustomerAccounts;

using System.Diagnostics;

namespace myTNB
{
    public partial class SelectAccountTableViewController : UIViewController
    {
        public SelectAccountTableViewController(IntPtr handle) : base(handle)
        {
        }
        BillingAccountDetailsResponseModel _billingAccountDetailsList = new BillingAccountDetailsResponseModel();
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            AddBackButton();
            accountRecordsTableView.Frame = new CGRect(0, 0, View.Frame.Width
                , View.Frame.Height - (DeviceHelper.IsIphoneXUpResolution() ? 64 + 72 + 24 : 64 + 72));
            accountRecordsTableView.Source = new SelectAccountsDataSource(this);
            accountRecordsTableView.ReloadData();
            accountRecordsTableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
            AddCTAButton();
        }

        internal void AddBackButton()
        {
            Title = "SelectAccount_Title".Translate();
            NavigationItem.HidesBackButton = true;
            UIImage backImg = UIImage.FromBundle("Back-White");
            UIBarButtonItem btnBack = new UIBarButtonItem(backImg, UIBarButtonItemStyle.Done, (sender, e) =>
            {
                //DataManager.DataManager.SharedInstance.IsSameAccount = true;
                this.DismissViewController(true, null);
            });
            this.NavigationItem.LeftBarButtonItem = btnBack;
        }

        void AddCTAButton()
        {
            UIButton btnAddAccount = new UIButton(UIButtonType.Custom)
            {
                Frame = new CGRect(18, View.Frame.Height - (DeviceHelper.IsIphoneXUpResolution() ? 152 : 128), View.Frame.Width - 36, 48)
            };
            btnAddAccount.SetTitle("Common_AddAnotherAccount".Translate(), UIControlState.Normal);
            btnAddAccount.Font = MyTNBFont.MuseoSans16;
            btnAddAccount.Layer.CornerRadius = 5.0f;
            btnAddAccount.BackgroundColor = MyTNBColor.FreshGreen;
            View.AddSubview(btnAddAccount);
            btnAddAccount.TouchUpInside += (sender, e) =>
            {
                ActivityIndicator.Show();
                NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
                {
                    InvokeOnMainThread(() =>
                    {
                        if (NetworkUtility.isReachable)
                        {
                            Debug.WriteLine("Add account button tapped");
                            UIStoryboard storyBoard = UIStoryboard.FromName("AccountRecords", null);
                            AccountsViewController viewController = storyBoard.InstantiateViewController("AccountsViewController") as AccountsViewController;
                            viewController.isDashboardFlow = true;
                            viewController._needsUpdate = true;
                            var navController = new UINavigationController(viewController);
                            PresentViewController(navController, true, null);
                        }
                        else
                        {
                            Debug.WriteLine("No Network");
                            AlertHandler.DisplayNoDataAlert(this);
                        }
                        ActivityIndicator.Hide();
                    });
                });
            };
        }
    }
}