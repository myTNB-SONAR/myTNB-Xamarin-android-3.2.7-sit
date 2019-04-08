using System;
using UIKit;
using myTNB.Dashboard.SelectAccounts;
using myTNB.Model;
using System.Threading.Tasks;
using CoreGraphics;
using myTNB.Registration.CustomerAccounts;
using myTNB.Extensions;
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
            UIButton btnAddAccount = new UIButton(UIButtonType.Custom);
            btnAddAccount.Frame = new CGRect(18, View.Frame.Height - (DeviceHelper.IsIphoneXUpResolution() ? 152 : 128), View.Frame.Width - 36, 48);
            btnAddAccount.SetTitle("Common_AddAnotherAccount".Translate(), UIControlState.Normal);
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
                            ErrorHandler.DisplayNoDataAlert(this);
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
                                    ErrorHandler.DisplayServiceError(this, _billingAccountDetailsList?.d?.message);
                                }
                                ActivityIndicator.Hide();
                            });
                        });
                    }
                    else
                    {
                        Debug.WriteLine("No Network");
                        ErrorHandler.DisplayNoDataAlert(this);
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