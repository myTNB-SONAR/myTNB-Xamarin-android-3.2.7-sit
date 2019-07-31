using System;
using UIKit;
using myTNB.Dashboard.DashboardComponents;
using myTNB.Model;
using myTNB.Home.More.MyAccount.ManageAccounts;
using CoreGraphics;
using System.Threading.Tasks;
using myTNB.SQLite.SQLiteDataManager;

namespace myTNB
{
    public partial class ManageAccountsViewController : UIViewController
    {
        public ManageAccountsViewController(IntPtr handle) : base(handle)
        {
        }

        UIView _viewNotificationMsg;
        UILabel _lblNotificationDetails;

        public CustomerAccountRecordModel CustomerRecord = new CustomerAccountRecordModel();
        //public int AccountRecordIndex = -1;
        BaseResponseModel _removeAccount = new BaseResponseModel();

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            SetNavigationBar();
            manageAccountsTableView.Frame = new CGRect(0, DeviceHelper.IsIphoneXUpResolution()
                ? 88 : DeviceHelper.GetScaledHeight(64), View.Frame.Width, DeviceHelper.GetScaledHeight(232));
            manageAccountsTableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            if (DataManager.DataManager.SharedInstance.AccountRecordIndex > -1
               && DataManager.DataManager.SharedInstance.AccountRecordsList != null
               && DataManager.DataManager.SharedInstance.AccountRecordsList.d != null
               && DataManager.DataManager.SharedInstance.AccountRecordIndex < DataManager.DataManager.SharedInstance.AccountRecordsList?.d?.Count)
            {
                CustomerRecord = DataManager.DataManager.SharedInstance.AccountRecordsList.d[DataManager.DataManager.SharedInstance.AccountRecordIndex];
            }
            InitializeNotificationMessage();
            if (DataManager.DataManager.SharedInstance.IsNickNameUpdated)
            {
                _lblNotificationDetails.Text = "Manage_UpdateNicknameSuccess".Translate();
                ShowNotificationMessage();
                DataManager.DataManager.SharedInstance.IsNickNameUpdated = false;
            }
            manageAccountsTableView.Source = new ManageAccountsDataSource(this, CustomerRecord);
            manageAccountsTableView.ReloadData();
        }

        internal void SetNavigationBar()
        {
            NavigationController.NavigationBar.Hidden = true;
            GradientViewComponent gradientViewComponent = new GradientViewComponent(View, true, 64, true);
            UIView headerView = gradientViewComponent.GetUI();
            TitleBarComponent titleBarComponent = new TitleBarComponent(headerView);
            UIView titleBarView = titleBarComponent.GetUI();
            titleBarComponent.SetTitle("Manage_ManageElectricityAccount".Translate());
            titleBarComponent.SetPrimaryVisibility(true);
            titleBarComponent.SetBackVisibility(false);
            titleBarComponent.SetBackAction(new UITapGestureRecognizer(() =>
            {
                DismissViewController(true, null);
            }));
            headerView.AddSubview(titleBarView);
            View.AddSubview(headerView);
        }

        internal void UpdateNickName()
        {
            UIStoryboard storyBoard = UIStoryboard.FromName("UpdateNickname", null);
            UpdateNicknameViewController viewController =
                storyBoard.InstantiateViewController("UpdateNicknameViewController") as UpdateNicknameViewController;
            if (viewController != null)
            {
                viewController.CustomerRecord = CustomerRecord;
                var navController = new UINavigationController(viewController);
                PresentViewController(navController, true, null);
            }
        }

        internal void InitializeNotificationMessage()
        {
            if (_viewNotificationMsg == null)
            {
                _viewNotificationMsg = new UIView(new CGRect(18, 32, View.Frame.Width - 36, 64));
                _viewNotificationMsg.BackgroundColor = MyTNBColor.SunGlow;
                _viewNotificationMsg.Layer.CornerRadius = 2.0f;
                _viewNotificationMsg.Hidden = true;

                _lblNotificationDetails = new UILabel(new CGRect(16, 16, _viewNotificationMsg.Frame.Width - 32, 32));
                _lblNotificationDetails.TextAlignment = UITextAlignment.Left;
                _lblNotificationDetails.Font = MyTNBFont.MuseoSans12;
                _lblNotificationDetails.TextColor = MyTNBColor.TunaGrey();
                _lblNotificationDetails.Text = TNBGlobal.EMPTY_ADDRESS;
                _lblNotificationDetails.Lines = 0;
                _lblNotificationDetails.LineBreakMode = UILineBreakMode.WordWrap;

                _viewNotificationMsg.AddSubview(_lblNotificationDetails);

                UIWindow currentWindow = UIApplication.SharedApplication.KeyWindow;
                currentWindow.AddSubview(_viewNotificationMsg);
            }
        }

        internal void ShowNotificationMessage()
        {
            _viewNotificationMsg.Hidden = false;
            _viewNotificationMsg.Alpha = 1.0f;
            UIView.Animate(5, 1, UIViewAnimationOptions.CurveEaseOut, () =>
            {
                _viewNotificationMsg.Alpha = 0.0f;
            }, () =>
            {
                _viewNotificationMsg.Hidden = true;
            });
        }

        internal void OnRemoveAccount()
        {
            var alert = UIAlertController.Create("Manage_RemoveAccount".Translate()
                , string.Format("Manage_RemoveAccountMessage".Translate(), CustomerRecord.accDesc, CustomerRecord.accNum)
                , UIAlertControllerStyle.Alert);
            alert.AddAction(UIAlertAction.Create("Common_Ok".Translate(), UIAlertActionStyle.Default, (obj) =>
            {
                ActivityIndicator.Show();
                NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
                {
                    InvokeOnMainThread(() =>
                    {
                        if (NetworkUtility.isReachable)
                        {
                            ActivityIndicator.Show();
                            ExecuteRemoveTNBAccountForUserFavCall();
                        }
                        else
                        {
                            AlertHandler.DisplayNoDataAlert(this);
                            ActivityIndicator.Hide();
                        }

                    });
                });
            }));
            alert.AddAction(UIAlertAction.Create("Common_Cancel".Translate(), UIAlertActionStyle.Cancel, (obj) =>
            {

            }));
            PresentViewController(alert, animated: true, completionHandler: null);
        }

        internal void ExecuteRemoveTNBAccountForUserFavCall()
        {
            RemoveTNBAccountForUserFav().ContinueWith(task =>
            {
                InvokeOnMainThread(() =>
                {
                    if (_removeAccount != null && _removeAccount.d != null
                       && _removeAccount.d.isError.ToLower() == "false")
                    {
                        DataManager.DataManager.SharedInstance.IsAccountDeleted = true;
                        int index = DataManager.DataManager.SharedInstance.AccountRecordsList.d.FindIndex(x => x.accNum == CustomerRecord.accNum);
                        if (index > -1)
                        {
                            DataManager.DataManager.SharedInstance.DeleteAccountUsageHistory(CustomerRecord.accNum);
                            DataManager.DataManager.SharedInstance.AccountRecordsList.d.RemoveAt(index);

                            UserAccountsEntity uaManager = new UserAccountsEntity();
                            uaManager.DeleteTable();
                            uaManager.CreateTable();
                            uaManager.InsertListOfItems(DataManager.DataManager.SharedInstance.AccountRecordsList);

                            // updates the cache when user entity data is updated
                            DataManager.DataManager.SharedInstance.RefreshDataFromAccountUpdate();

                            DataManager.DataManager.SharedInstance.AccountsDeleted.Add(CustomerRecord.accNum);
                            DataManager.DataManager.SharedInstance.DeleteDue(CustomerRecord.accNum);
                            DataManager.DataManager.SharedInstance.DeleteDetailsFromBillingAccount(CustomerRecord.accNum);
                            DataManager.DataManager.SharedInstance.DeleteDetailsFromBillHistory(CustomerRecord.accNum);
                            DataManager.DataManager.SharedInstance.DeleteDetailsFromPaymentHistory(CustomerRecord.accNum);
                        }
                        DismissViewController(true, null);
                    }
                    else
                    {
                        AlertHandler.DisplayServiceError(this, _removeAccount?.d?.message);
                    }
                    ActivityIndicator.Hide();
                });
            });
        }


        internal Task RemoveTNBAccountForUserFav()
        {
            var emailAddress = string.Empty;
            if (DataManager.DataManager.SharedInstance.UserEntity?.Count > 0)
            {
                emailAddress = DataManager.DataManager.SharedInstance.UserEntity[0]?.email;
            }

            return Task.Factory.StartNew(() =>
            {
                ServiceManager serviceManager = new ServiceManager();
                object requestParameter = new
                {
                    apiKeyID = TNBGlobal.API_KEY_ID,
                    ipAddress = TNBGlobal.API_KEY_ID,
                    clientType = TNBGlobal.API_KEY_ID,
                    activeUserName = TNBGlobal.API_KEY_ID,
                    devicePlatform = TNBGlobal.API_KEY_ID,
                    deviceVersion = TNBGlobal.API_KEY_ID,
                    deviceCordova = TNBGlobal.API_KEY_ID,
                    userID = DataManager.DataManager.SharedInstance.User.UserID,
                    accNum = CustomerRecord.accNum,
                    email = emailAddress
                };
                _removeAccount = serviceManager.BaseServiceCall("RemoveTNBAccountForUserFav", requestParameter);
            });
        }
    }
}