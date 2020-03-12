using System;
using UIKit;
using myTNB.Dashboard.DashboardComponents;
using myTNB.Model;
using myTNB.Home.More.MyAccount.ManageAccounts;
using CoreGraphics;
using System.Threading.Tasks;
using myTNB.SQLite.SQLiteDataManager;
using myTNB.MyAccount;
using System.Collections.Generic;

namespace myTNB
{
    public partial class ManageAccountsViewController : CustomUIViewController
    {
        public ManageAccountsViewController(IntPtr handle) : base(handle) { }

        private UIView _viewNotificationMsg;
        private UILabel _lblNotificationDetails;

        public CustomerAccountRecordModel CustomerRecord = new CustomerAccountRecordModel();
        //public int AccountRecordIndex = -1;
        private BaseResponseModelV2 _removeAccount = new BaseResponseModelV2();

        public override void ViewDidLoad()
        {
            PageName = MyAccountConstants.Pagename_ManageAccount;
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
                _lblNotificationDetails.Text = GetI18NValue(MyAccountConstants.I18N_NicknameUpdateSuccess);
                ShowNotificationMessage();
                DataManager.DataManager.SharedInstance.IsNickNameUpdated = false;
            }
            manageAccountsTableView.Source = new ManageAccountsDataSource(this, CustomerRecord);
            manageAccountsTableView.ReloadData();
        }

        private void SetNavigationBar()
        {
            NavigationController.NavigationBar.Hidden = true;
            GradientViewComponent gradientViewComponent = new GradientViewComponent(View, true, 64, true);
            UIView headerView = gradientViewComponent.GetUI();
            TitleBarComponent titleBarComponent = new TitleBarComponent(headerView);
            UIView titleBarView = titleBarComponent.GetUI();
            titleBarComponent.SetTitle(GetI18NValue(MyAccountConstants.I18N_Title));
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
                UINavigationController navController = new UINavigationController(viewController);
                navController.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
                PresentViewController(navController, true, null);
            }
        }

        private void InitializeNotificationMessage()
        {
            if (_viewNotificationMsg == null)
            {
                _viewNotificationMsg = new UIView(new CGRect(18, 32, View.Frame.Width - 36, 64))
                {
                    BackgroundColor = MyTNBColor.SunGlow,
                    Hidden = true
                };
                _viewNotificationMsg.Layer.CornerRadius = 2.0f;

                _lblNotificationDetails = new UILabel(new CGRect(16, 16, _viewNotificationMsg.Frame.Width - 32, 32))
                {
                    TextAlignment = UITextAlignment.Left,
                    Font = MyTNBFont.MuseoSans12,
                    TextColor = MyTNBColor.TunaGrey(),
                    Text = TNBGlobal.EMPTY_ADDRESS,
                    Lines = 0,
                    LineBreakMode = UILineBreakMode.WordWrap
                };

                _viewNotificationMsg.AddSubview(_lblNotificationDetails);

                UIWindow currentWindow = UIApplication.SharedApplication.KeyWindow;
                currentWindow.AddSubview(_viewNotificationMsg);
            }
        }

        private void ShowNotificationMessage()
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
            DisplayCustomAlert(GetI18NValue(MyAccountConstants.Popup_RemoveAccountTitle)
                , string.Format(GetI18NValue(MyAccountConstants.Popup_RemoveAccountMessage), CustomerRecord.accDesc, CustomerRecord.accNum)
                , new Dictionary<string, Action> { {GetCommonI18NValue(Constants.Common_Cancel), null },
                    {GetCommonI18NValue(Constants.Common_Ok), ()=>{
                        ActivityIndicator.Show();
                        NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
                        {
                            InvokeOnMainThread(() =>
                            {
                                if (NetworkUtility.isReachable)
                                {
                                    ExecuteRemoveTNBAccountForUserFavCall();
                                }
                                else
                                {
                                    DisplayNoDataAlert();
                                    ActivityIndicator.Hide();
                                }

                            });
                        });
                    } } }
            );
        }

        private void ExecuteRemoveTNBAccountForUserFavCall()
        {
            RemoveTNBAccountForUserFav().ContinueWith(task =>
            {
                InvokeOnMainThread(() =>
                {
                    if (_removeAccount != null && _removeAccount.d != null && _removeAccount.d.IsSuccess)
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

                            // remove deleted account from the rearrange list if it exists
                            DataManager.DataManager.SharedInstance.RemoveAccountFromArrangedList(CustomerRecord.accNum);
                        }
                        DismissViewController(true, null);
                    }
                    else
                    {
                        DisplayServiceError(_removeAccount?.d?.DisplayMessage ?? string.Empty);
                    }
                    ActivityIndicator.Hide();
                });
            });
        }

        private Task RemoveTNBAccountForUserFav()
        {
            string emailAddress = string.Empty;
            if (DataManager.DataManager.SharedInstance.UserEntity?.Count > 0)
            {
                emailAddress = DataManager.DataManager.SharedInstance.UserEntity[0]?.email;
            }

            return Task.Factory.StartNew(() =>
            {
                ServiceManager serviceManager = new ServiceManager();
                object requestParameter = new
                {
                    serviceManager.usrInf,
                    serviceManager.deviceInf,
                    accNum = CustomerRecord.accNum,
                };
                _removeAccount = serviceManager.BaseServiceCallV6(MyAccountConstants.Service_RemoveAccount, requestParameter);
            });
        }
    }
}