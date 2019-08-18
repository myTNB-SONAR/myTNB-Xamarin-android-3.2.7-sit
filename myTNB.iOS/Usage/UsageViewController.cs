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

        TariffSelectionComponent _tariffSelectionComponent;

        UIView _tariffSelectionContainer, _energyTipsContainer, _tariffLegendContainer;

        public override void ViewDidLoad()
        {
            PageName = "UsageView";
            base.ViewDidLoad();
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            //SetTariffSelectionComponent();
            //SetEnergyTipsComponent();
            CallGetAccountStatusAPI();
            //CallGetAccountUsageAPI();
        }
        #region TARIFF LEGEND Methods
        private void SetTariffLegendComponent()
        {
            nfloat yPos = _tariffSelectionContainer.Frame.GetMaxY() + GetScaledHeight(30f);
            List<LegendItemModel> tariffList = new List<LegendItemModel>(AccountUsageCache.GetTariffLegendList());
            nfloat height = 0;
            if (tariffList != null && tariffList.Count > 0)
            {
                height = tariffList.Count * GetScaledHeight(25f);
            }
            _tariffLegendContainer = new UIView(new CGRect(0, yPos, ViewWidth, height))
            {
                BackgroundColor = UIColor.Clear,
                Hidden = true
            };
            View.AddSubview(_tariffLegendContainer);

            TariffLegendComponent tariffLegendComponent = new TariffLegendComponent(View, tariffList);
            _tariffLegendContainer.AddSubview(tariffLegendComponent.GetUI());
        }
        private void ShowHideTariffLegends(bool isVisible)
        {
            List<LegendItemModel> tariffList = new List<LegendItemModel>(AccountUsageCache.GetTariffLegendList());
            if (tariffList != null && tariffList.Count > 0)
            {
                _tariffLegendContainer.Hidden = !isVisible;
            }
        }
        #endregion
        #region ENERGY TIPS Methods
        private void SetEnergyTipsComponent()
        {
            List<TipsModel> tipsList;
            EnergyTipsEntity wsManager = new EnergyTipsEntity();
            tipsList = wsManager.GetAllItems();

            if (tipsList != null &&
                tipsList.Count > 0)
            {
                nfloat footerRatio = 136.0f / 320.0f;
                nfloat footerHeight = ViewWidth * footerRatio;
                nfloat footerYPos = View.Frame.Height - footerHeight;

                _energyTipsContainer = new UIView(new CGRect(0, footerYPos - GetScaledHeight(140f), ViewWidth, GetScaledHeight(100f)))
                {
                    BackgroundColor = UIColor.Clear
                };
                View.AddSubview(_energyTipsContainer);

                EnergyTipsComponent energyTipsComponent = new EnergyTipsComponent(_energyTipsContainer, tipsList);
                _energyTipsContainer.AddSubview(energyTipsComponent.GetUI());
            }
        }
        #endregion
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

                        SetTariffLegendComponent();
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
