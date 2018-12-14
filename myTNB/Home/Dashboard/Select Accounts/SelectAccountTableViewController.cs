using System;
using UIKit;
using myTNB.Dashboard.SelectAccounts;
using myTNB.Model;
using System.Threading.Tasks;
using CoreGraphics;
using myTNB.Registration.CustomerAccounts;
using myTNB.Extensions;

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
            accountRecordsTableView.Frame = new CGRect(0
                                                       , 0
                                                       , View.Frame.Width
                                                       , View.Frame.Height
                                                            - (DeviceHelper.IsIphoneXUpResolution()
                                                               ? 64 + 72 + 24
                                                               : 64 + 72));
            accountRecordsTableView.Source = new SelectAccountsDataSource(this);
            accountRecordsTableView.ReloadData();
            accountRecordsTableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
            AddCTAButton();
        }

        internal void AddBackButton()
        {
            Title = "Select Electricity Account";
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
            UIButton btnAddAccount = new UIButton(UIButtonType.Custom);
            btnAddAccount.Frame = new CGRect(18, View.Frame.Height - (DeviceHelper.IsIphoneXUpResolution() ? 152 : 128), View.Frame.Width - 36, 48);
            btnAddAccount.SetTitle("AddAnotherAccount".Translate(), UIControlState.Normal);
            btnAddAccount.Font = myTNBFont.MuseoSans16();
            btnAddAccount.Layer.CornerRadius = 5.0f;
            btnAddAccount.BackgroundColor = myTNBColor.FreshGreen();
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
                            Console.WriteLine("Add account button tapped");
                            UIStoryboard storyBoard = UIStoryboard.FromName("AccountRecords", null);
                            AccountsViewController viewController = storyBoard.InstantiateViewController("AccountsViewController") as AccountsViewController;
                            viewController.isDashboardFlow = true;
                            viewController._needsUpdate = true;
                            var navController = new UINavigationController(viewController);
                            PresentViewController(navController, true, null);
                        }
                        else
                        {
                            Console.WriteLine("No Network");
                            var alert = UIAlertController.Create("ErrNoNetworkTitle".Translate(), "ErrNoNetworkMsg".Translate(), UIAlertControllerStyle.Alert);
                            alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Cancel, null));
                            PresentViewController(alert, animated: true, completionHandler: null);
                        }
                        ActivityIndicator.Hide();
                    });
                });
            };
        }

        /// <summary>
        /// Loads the billing account details.
        /// </summary>
        public void LoadBillingAccountDetails()
        {
            if (DataManager.DataManager.SharedInstance.SelectedAccount.IsREAccount)
            {
                ExecuteGetBillAccountDetailsCall();
            }
            else
            {
                var cachedDetails = DataManager.DataManager.SharedInstance.GetCachedBillingAccountDetails(DataManager.DataManager.SharedInstance.SelectedAccount.accNum);

                if (cachedDetails != null)
                {
                    DataManager.DataManager.SharedInstance.BillingAccountDetails = cachedDetails;
                    DataManager.DataManager.SharedInstance.IsBillUpdateNeeded = false;
                    this.DismissViewController(true, null);
                }
                else
                {
                    ExecuteGetBillAccountDetailsCall();
                }
            }
        }

        internal void ExecuteGetBillAccountDetailsCall()
        {
            NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
            {
                InvokeOnMainThread(() =>
                {
                    if (NetworkUtility.isReachable)
                    {
                        ActivityIndicator.Show();
                        GetBillingAccountDetails().ContinueWith(task =>
                        {
                            InvokeOnMainThread(() =>
                            {
                                if (_billingAccountDetailsList != null && _billingAccountDetailsList.d != null
                                    && _billingAccountDetailsList.d.data != null)
                                {
                                    DataManager.DataManager.SharedInstance.BillingAccountDetails = _billingAccountDetailsList.d.data;
                                    DataManager.DataManager.SharedInstance.IsBillUpdateNeeded = false;
                                    if (!DataManager.DataManager.SharedInstance.SelectedAccount.IsREAccount)
                                    {
                                        DataManager.DataManager.SharedInstance.SaveToBillingAccounts(DataManager.DataManager.SharedInstance.BillingAccountDetails,
                                                                                                     DataManager.DataManager.SharedInstance.SelectedAccount.accNum);
                                    }
                                    this.DismissViewController(true, null);
                                }
                                else
                                {
                                    DataManager.DataManager.SharedInstance.IsSameAccount = true;
                                    DataManager.DataManager.SharedInstance.BillingAccountDetails = new BillingAccountDetailsDataModel();
                                    var alert = UIAlertController.Create("Error in Response", "There is an error in the server, please try again.", UIAlertControllerStyle.Alert);
                                    alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Cancel, null));
                                    PresentViewController(alert, animated: true, completionHandler: null);
                                }
                                ActivityIndicator.Hide();
                            });
                        });
                    }
                    else
                    {
                        Console.WriteLine("No Network");
                        var alert = UIAlertController.Create("ErrNoNetworkTitle".Translate(), "ErrNoNetworkMsg".Translate(), UIAlertControllerStyle.Alert);
                        alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Cancel, null));
                        PresentViewController(alert, animated: true, completionHandler: null);
                    }
                });
            });
        }

        internal Task GetBillingAccountDetails()
        {
            return Task.Factory.StartNew(() =>
            {
                ServiceManager serviceManager = new ServiceManager();
                object requestParameter = new
                {
                    apiKeyID = TNBGlobal.API_KEY_ID,
                    CANum = DataManager.DataManager.SharedInstance.SelectedAccount.accNum
                };
                _billingAccountDetailsList = serviceManager.GetBillingAccountDetails("GetBillingAccountDetails", requestParameter);
            });
        }
    }
}