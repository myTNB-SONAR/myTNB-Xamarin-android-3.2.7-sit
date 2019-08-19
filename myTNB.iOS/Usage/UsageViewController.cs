using CoreGraphics;
using myTNB.Model;
using myTNB.Model.Usage;
using myTNB.SitecoreCMS.Model;
using myTNB.SQLite.SQLiteDataManager;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using UIKit;

namespace myTNB
{
    public partial class UsageViewController : UsageBaseViewController
    {
        public UsageViewController(IntPtr handle) : base(handle) { }

        public override void ViewDidLoad()
        {
            PageName = "UsageView";
            base.ViewDidLoad();
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            CallGetAccountStatusAPI();
            CallGetAccountUsageAPI();
        }
        #region API Calls
        private void CallGetAccountUsageAPI()
        {
            NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
            {
                InvokeOnMainThread(async () =>
                {
                    if (NetworkUtility.isReachable)
                    {
                        ActivityIndicator.Show();
                        AccountUsageCache.ClearTariffLegendList();

                        AccountUsageResponseModel accountUsageResponse = await GetAccountUsage(DataManager.DataManager.SharedInstance.SelectedAccount);
                        AccountUsageCache.AddTariffLegendList(accountUsageResponse);
                        AccountUsageCache.SetData(accountUsageResponse);
                        SetTariffLegendComponent();
                        AddSubviews();
                    }
                    else
                    {
                        DisplayNoDataAlert();
                    }
                    ActivityIndicator.Hide();
                });
            });
        }

        private void CallGetAccountStatusAPI()
        {
            NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
            {
                InvokeOnMainThread(async () =>
                {
                    if (NetworkUtility.isReachable)
                    {
                        ActivityIndicator.Show();
                        AccountStatusCache.ClearAccountStatusData();

                        AccountStatusResponseModel accountStatusResponse = await GetAccountStatus(DataManager.DataManager.SharedInstance.SelectedAccount);
                        AccountStatusCache.AddAccountStatusData(accountStatusResponse);

                        SetDisconnectionComponent();
                    }
                    else
                    {
                        DisplayNoDataAlert();
                    }
                    ActivityIndicator.Hide();
                });
            });
        }

        private async Task<AccountUsageResponseModel> GetAccountUsage(CustomerAccountRecordModel account)
        {
            AccountUsageResponseModel accountUsageResponse = null;
            ServiceManager serviceManager = new ServiceManager();
            object requestParameter = new
            {
                contractAccount = account.accNum,
                isOwner = account.isOwned,
                serviceManager.usrInf
            };

            accountUsageResponse = await Task.Run(() =>
            {
                return serviceManager.OnExecuteAPIV6<AccountUsageResponseModel>("GetAccountUsage", requestParameter);
            });

            return accountUsageResponse;
        }

        private async Task<AccountStatusResponseModel> GetAccountStatus(CustomerAccountRecordModel account)
        {
            AccountStatusResponseModel accountStatusResponse = null;
            ServiceManager serviceManager = new ServiceManager();
            object requestParameter = new
            {
                contractAccount = account.accNum,
                isOwner = account.isOwned,
                serviceManager.usrInf
            };

            accountStatusResponse = await Task.Run(() =>
            {
                return serviceManager.OnExecuteAPIV6<AccountStatusResponseModel>("GetAccountStatus", requestParameter);
            });

            return accountStatusResponse;
        }
        #endregion
    }
}
