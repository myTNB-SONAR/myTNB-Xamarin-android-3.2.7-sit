using Android.Util;
using myTNB_Android.Src.AppLaunch.Models;
using myTNB_Android.Src.Base.Models;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.myTNBMenu.Api;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.myTNBMenu.Requests;
using myTNB_Android.Src.Utils;
using Refit;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace myTNB_Android.Src.myTNBMenu.MVP.Fragment
{
    public class DashboardChartPresenter : DashboardChartContract.IUserActionsListener
    {
        private DashboardChartContract.IView mView;
        CancellationTokenSource cts;

        public DashboardChartPresenter(DashboardChartContract.IView mView)
        {
            this.mView = mView;
            this.mView.SetPresenter(this);
        }

        public void OnByKwh()
        {
            try
            {
                if (!this.mView.HasNoInternet())
                {
                    this.mView.ShowByKwh();
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

        }

        public void OnByRM()
        {
            try
            {
                if (!this.mView.HasNoInternet())
                {
                    this.mView.ShowByRM();
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void OnLearnMore()
        {
            WeblinkEntity weblinkEntity = WeblinkEntity.GetByCode("SMTR");
            if (weblinkEntity != null)
            {
                this.mView.ShowLearnMore(Weblink.Copy(weblinkEntity));
            }
        }

        public async void OnLoadAmount(string accountNum)
        {
            try
            {
                cts = new CancellationTokenSource();
                if (mView.IsActive())
                {
                    this.mView.ShowAmountProgress();
                }
                ServicePointManager.ServerCertificateValidationCallback += SSLFactoryHelper.CertificateValidationCallBack;
    #if DEBUG
                var httpClient = new HttpClient(new HttpLoggingHandler(/*new NativeMessageHandler()*/)) { BaseAddress = new Uri(Constants.SERVER_URL.END_POINT) };
                var amountDueApi = RestService.For<IAmountDueApi>(httpClient);

    #else
                var amountDueApi = RestService.For<IAmountDueApi>(Constants.SERVER_URL.END_POINT);

    #endif 

                var amountDueResponse = await amountDueApi.GetAccountDueAmount(new Requests.AccountDueAmountRequest()
                {
                    ApiKeyId = Constants.APP_CONFIG.API_KEY_ID,
                    AccNum = accountNum

                }, cts.Token);


                if (this.mView.IsActive())
                {
                    this.mView.HideAmountProgress();
                }
                if (amountDueResponse != null && amountDueResponse.Data != null && amountDueResponse.Data.Status.ToUpper() == Constants.REFRESH_MODE)
                {
                    this.mView.ShowNoInternetWithWord(amountDueResponse.Data.RefreshMessage, amountDueResponse.Data.RefreshBtnText);
                }
                else if (!amountDueResponse.Data.IsError)
                {
                    this.mView.ShowAmountDue(amountDueResponse.Data.Data);
                }
                else
                {
                    this.mView.ShowNoInternetWithWord(null, null);
                }
            }
            catch (System.OperationCanceledException e)
            {
                if (this.mView.IsActive())
                {
                    this.mView.HideAmountProgress();
                }
                this.mView.ShowNoInternetWithWord(null, null);
                Utility.LoggingNonFatalError(e);
            }
            catch (ApiException apiException)
            {
                // ADD HTTP CONNECTION EXCEPTION HERE
                if (this.mView.IsActive())
                {
                    this.mView.HideAmountProgress();
                }
                this.mView.ShowNoInternetWithWord(null, null);
                Utility.LoggingNonFatalError(apiException);
            }
            catch (Exception e)
            {
                // ADD UNKNOWN EXCEPTION HERE
                if (this.mView.IsActive())
                {
                    this.mView.HideAmountProgress();
                }
                this.mView.ShowNoInternetWithWord(null, null);
                Utility.LoggingNonFatalError(e);
            }
        }

        public void OnNotification()
        {
            this.mView.ShowNotification();
        }

        public void OnPay()
        {
            this.mView.ShowPayment();
        }

        public void OnTapRefresh()
        {
            this.mView.ShowTapRefresh();
        }

        public void OnViewBill(AccountData selectedAccount)
        {
            LoadingBillsHistory(selectedAccount);
        }


        private async void LoadingBillsHistory(AccountData selectedAccount)
        {
            this.mView.ShowProgress();
            cts = new CancellationTokenSource();
#if DEBUG || STUB
            var httpClient = new HttpClient(new HttpLoggingHandler(/*new NativeMessageHandler()*/)) { BaseAddress = new System.Uri(Constants.SERVER_URL.END_POINT) };
            var api = RestService.For<IBillsPaymentHistoryApi>(httpClient);
#else
            var api = RestService.For<IBillsPaymentHistoryApi>(Constants.SERVER_URL.END_POINT);
#endif

            try
            {
                var billsHistoryResponseApi = await api.GetBillHistoryV5(new BillHistoryRequest(Constants.APP_CONFIG.API_KEY_ID)
                {
                    AccountNum = selectedAccount.AccountNum,
                    IsOwner = selectedAccount.IsOwner,
                    Email = UserEntity.GetActive().Email
                }, cts.Token);

                var billsHistoryResponseV5 = billsHistoryResponseApi;

                this.mView.HideProgress();

                if (billsHistoryResponseV5 != null && billsHistoryResponseV5.Data != null)
                {
                    if (!billsHistoryResponseV5.Data.IsError && !string.IsNullOrEmpty(billsHistoryResponseV5.Data.Status)
                        && billsHistoryResponseV5.Data.Status.Equals("success"))
                    {
                        if (billsHistoryResponseV5.Data.BillHistory != null && billsHistoryResponseV5.Data.BillHistory.Count() > 0)
                        {
                            this.mView.ShowViewBill(billsHistoryResponseV5.Data.BillHistory[0]);
                            return;
                        }
                        else
                        {
                            this.mView.ShowViewBill();
                        }
                    }
                    else
                    {
                        this.mView.ShowViewBill();
                    }
                }
                else
                {
                    this.mView.ShowViewBill();
                }

            }
            catch (System.OperationCanceledException e)
            {
                this.mView.HideProgress();
                this.mView.ShowLoadBillRetryOptions();
                Utility.LoggingNonFatalError(e);
            }
            catch (ApiException apiException)
            {
                this.mView.HideProgress();
                this.mView.ShowLoadBillRetryOptions();
                Utility.LoggingNonFatalError(apiException);
            }
            catch (Exception e)
            {
                this.mView.HideProgress();
                this.mView.ShowLoadBillRetryOptions();
                Utility.LoggingNonFatalError(e);
            }
        }


        public void Start()
        {
            try
            {
                // THIS SHOULD BE THE CHART DISPLAY ANIMATION
                DownTimeEntity bcrmEntity = new DownTimeEntity();
                bcrmEntity = DownTimeEntity.GetByCode(Constants.BCRM_SYSTEM);
                if (this.mView.HasNoInternet() || bcrmEntity.IsDown)
                {
                    this.mView.ShowNoInternet();
                }
                else
                {
                    OnByRM();
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

    }
}