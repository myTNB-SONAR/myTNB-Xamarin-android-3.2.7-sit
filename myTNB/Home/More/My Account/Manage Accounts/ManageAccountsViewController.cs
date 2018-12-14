using System;
using UIKit;
using myTNB.Dashboard.DashboardComponents;
using myTNB.Model;
using myTNB.Home.More.MyAccount.ManageAccounts;
using CoreGraphics;
using System.Threading.Tasks;
using myTNB.SQLite.SQLiteDataManager;
using myTNB.DataManager;

namespace myTNB
{
    public partial class ManageAccountsViewController : UIViewController
    {
        public ManageAccountsViewController (IntPtr handle) : base (handle)
        {
        }

        UIView _viewNotificationMsg;
        UILabel _lblNotificationDetails;

        public CustomerAccountRecordModel CustomerRecord = new CustomerAccountRecordModel();
        public int AccountRecordIndex = -1;
        BaseResponseModel _removeAccount = new BaseResponseModel();

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            SetNavigationBar();
            manageAccountsTableView.Frame = new CGRect(0, DeviceHelper.IsIphoneX() ? 88 : 64, View.Frame.Width, 232);
            manageAccountsTableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            if(AccountRecordIndex > -1 
               && DataManager.DataManager.SharedInstance.AccountRecordsList != null
               && DataManager.DataManager.SharedInstance.AccountRecordsList.d != null){
                CustomerRecord = DataManager.DataManager.SharedInstance.AccountRecordsList.d[AccountRecordIndex];
            }
            InitializeNotificationMessage();
            if (DataManager.DataManager.SharedInstance.IsNickNameUpdated)
            {
                _lblNotificationDetails.Text = "Account nickname has been updated successfully.";
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
            titleBarComponent.SetTitle("Manage Supply Account(s)");
            titleBarComponent.SetNotificationVisibility(true);
            titleBarComponent.SetBackVisibility(false);
            titleBarComponent.SetBackAction(new UITapGestureRecognizer(() => {
                DismissViewController(true, null);
            }));
            headerView.AddSubview(titleBarView);
            View.AddSubview(headerView);
        }

        internal void UpdateNickName(){
            UIStoryboard storyBoard = UIStoryboard.FromName("UpdateNickname", null);
            UpdateNicknameViewController viewController =
                storyBoard.InstantiateViewController("UpdateNicknameViewController") as UpdateNicknameViewController;
            viewController.CustomerRecord = CustomerRecord;
            var navController = new UINavigationController(viewController);
            PresentViewController(navController, true, null);
        }

        internal void InitializeNotificationMessage()
        {
            if (_viewNotificationMsg == null)
            {
                _viewNotificationMsg = new UIView(new CGRect(18, 32, View.Frame.Width - 36, 64));
                _viewNotificationMsg.BackgroundColor = myTNBColor.SunGlow();
                _viewNotificationMsg.Layer.CornerRadius = 2.0f;
                _viewNotificationMsg.Hidden = true;

                _lblNotificationDetails = new UILabel(new CGRect(16, 16, _viewNotificationMsg.Frame.Width - 32, 32));
                _lblNotificationDetails.TextAlignment = UITextAlignment.Left;
                _lblNotificationDetails.Font = myTNBFont.MuseoSans12();
                _lblNotificationDetails.TextColor = myTNBColor.TunaGrey();
                _lblNotificationDetails.Text = "- - -";
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
            UIView.Animate(5, 1, UIViewAnimationOptions.CurveEaseOut, () => {
                _viewNotificationMsg.Alpha = 0.0f;
            }, () => {
                _viewNotificationMsg.Hidden = true;
            });
        }

        internal void OnRemoveAccount(){
            string message = string.Empty; 
            if(CustomerRecord.isOwned.ToLower() == "true"){
                message = string.Format("You are about to remove {0}, Account No. {1}.\r\n\r\nAccess of all users associating with this accout will also be removed.\r\n\r\nVisit the Self - Service Portal or Kedai Tenaga to close your Supply Account.", CustomerRecord.accDesc, CustomerRecord.accNum);
            }else{
                message = string.Format("You are about to remove {0}, Account No. {1}.\r\n\r\nVisit the Self - Service Portal or Kedai Tenaga to close your Supply Account.", CustomerRecord.accDesc, CustomerRecord.accNum);
            }

            var alert = UIAlertController.Create("Remove Account", message, UIAlertControllerStyle.Alert);
            alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Default, (obj) => {
                ActivityIndicator.Show();
                NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
                {
                    InvokeOnMainThread(() => {
                        if (NetworkUtility.isReachable)
                        {
                            ExecuteRemoveTNBAccountForUserFavCall();
                        }
                        else
                        {
                            Console.WriteLine("No Network");
                            DisplayAlertMessage("No Data Connection", "Please check your data connection and try again.");
                            ActivityIndicator.Hide();
                        }

                    });
                });
            }));
            alert.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, (obj) => {

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
                            if(!ServiceCall.HasAccountList()){
                                DataManager.DataManager.SharedInstance.SelectedAccount = new CustomerAccountRecordModel();
                            }
                            UserAccountsEntity uaManager = new UserAccountsEntity();
                            uaManager.DeleteTable();
                            uaManager.CreateTable();
                            uaManager.InsertListOfItems(DataManager.DataManager.SharedInstance.AccountRecordsList);
                            DataManager.DataManager.SharedInstance.CurrentSelectedAccountIndex = 0;
                            DataManager.DataManager.SharedInstance.PreviousSelectedAccountIndex = 0;
                            DataManager.DataManager.SharedInstance.SelectedAccount = 
                                DataManager.DataManager.SharedInstance.AccountRecordsList.d[0];
                            DataManager.DataManager.SharedInstance.IsSameAccount = false;

                        }
                        DismissViewController(true, null);
                    }
                    ActivityIndicator.Hide();
                });
            });
        }

        internal Task RemoveTNBAccountForUserFav()
        {
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
                    email = DataManager.DataManager.SharedInstance.UserEntity[0].email
                };
                _removeAccount = serviceManager.BaseServiceCall("RemoveTNBAccountForUserFav", requestParameter);
            });
        }

        internal void DisplayAlertMessage(string title, string message)
        {
            var alert = UIAlertController.Create(title, message, UIAlertControllerStyle.Alert);
            alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Cancel, null));
            PresentViewController(alert, animated: true, completionHandler: null);
        }
    }
}