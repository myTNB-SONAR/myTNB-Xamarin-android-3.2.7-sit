using Android.Util;
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

namespace myTNB_Android.Src.myTNBMenu.MVP.Fragment
{
    public class DashboardNonOwnerPresenter : DashboardNonOwnerContract.IUserActionsListener
    {

        private DashboardNonOwnerContract.IView mView;
        CancellationTokenSource cts;

        public DashboardNonOwnerPresenter(DashboardNonOwnerContract.IView mView)
        {
            this.mView = mView;
            this.mView.SetPresenter(this);
        }

        public void OnGetAccess()
        {
            this.mView.ShowGetAccessForm();
        }

        public async void OnLoadAmount(string accountNum)
        {
            cts = new CancellationTokenSource();

            if (mView.IsActive())
            {
                this.mView.ShowAmountProgress();
            }

            this.mView.DisablePayButton();
            ServicePointManager.ServerCertificateValidationCallback += SSLFactoryHelper.CertificateValidationCallBack;
#if DEBUG
            var httpClient = new HttpClient(new HttpLoggingHandler(/*new NativeMessageHandler()*/)) { BaseAddress = new Uri(Constants.SERVER_URL.END_POINT) };
            var amountDueApi = RestService.For<IAmountDueApi>(httpClient);

#else
            var amountDueApi = RestService.For<IAmountDueApi>(Constants.SERVER_URL.END_POINT);

#endif 

            try
            {
                var amountDueResponse = await amountDueApi.GetAccountDueAmount(new Requests.AccountDueAmountRequest()
                {
                    ApiKeyId = Constants.APP_CONFIG.API_KEY_ID,
                    AccNum = accountNum

                }, cts.Token);


                if (this.mView.IsActive())
                {
                    this.mView.HideAmountProgress();
                }
                if (!amountDueResponse.Data.IsError)
                {
                    //if (amountDueResponse.Data.Data.AmountDue > 0.00)
                    //{
                    this.mView.EnablePayButton();
                    //}
                    this.mView.ShowAmountDue(amountDueResponse.Data.Data);
                }
                else
                {
                    this.mView.ShowRetryOptionsApiException(null);

                }
            }
            catch (System.OperationCanceledException e)
            {
                if (this.mView.IsActive())
                {
                    this.mView.HideAmountProgress();
                    this.mView.ShowRetryOptionsCancelledException(e);

                }
                Utility.LoggingNonFatalError(e);
            }
            catch (ApiException apiException)
            {
                // ADD HTTP CONNECTION EXCEPTION HERE
                if (this.mView.IsActive())
                {
                    this.mView.HideAmountProgress();
                    this.mView.ShowRetryOptionsApiException(apiException);
                }
                Utility.LoggingNonFatalError(apiException);
            }
            catch (Exception e)
            {
                // ADD UNKNOWN EXCEPTION HERE
                if (this.mView.IsActive())
                {
                    this.mView.HideAmountProgress();
                    this.mView.ShowRetryOptionsUnknownException(e);
                }
                Utility.LoggingNonFatalError(e);
            }


        }

        public void OnNotification()
        {
            if (this.mView.HasInternet())
            {
                this.mView.ShowNotification();
            }
            else
            {
                this.mView.ShowNoInternetSnackbar();
            }
        }

        public void OnPay()
        {
            this.mView.ShowSelectPaymentScreen();
        }

        public void OnViewBill(AccountData selectedAccount)
        {
            LoadingBillsHistory(selectedAccount);
        }


        private async void LoadingBillsHistory(AccountData selectedAccount)
        {

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


                        /*** Save Bill History For the Day ***/
                        //BillHistoryEntity smUsageModel = new BillHistoryEntity();
                        //smUsageModel.Timestamp = DateTime.Now.ToLocalTime();
                        //smUsageModel.JsonResponse = JsonConvert.SerializeObject(billsHistoryResponseV5);
                        //smUsageModel.AccountNo = selectedAccount.AccountNum;
                        //BillHistoryEntity.InsertItem(smUsageModel);
                        /*****/

                        //if (IsActive())
                        //{
                        //    this.mView.ShowBillsList(billsHistoryResponseV5);
                        //}
                    }
                    else
                    {
                        //if (this.mView.IsActive())
                        //{
                        //    this.mView.ShowEmptyBillList();
                        //}
                    }
                }
                //if (this.mView.IsActive())
                //{
                //    this.mView.EnableTabs();
                //}

            }
            catch (System.OperationCanceledException e)
            {
                Log.Debug("BillPayment Presenter", "Cancelled Exception");
                if (this.mView.IsActive())
                {
                    this.mView.ShowNoInternetSnackbar();
                }
                Utility.LoggingNonFatalError(e);
            }
            catch (ApiException apiException)
            {
                // ADD HTTP CONNECTION EXCEPTION HERE
                //this.mView.ShowRetryOptionsApiException(apiException);
                Log.Debug("BillPayment Presenter", "Stack " + apiException.StackTrace);
                if (this.mView.IsActive())
                {
                    this.mView.ShowNoInternetSnackbar();
                }
                Utility.LoggingNonFatalError(apiException);
            }
            catch (Exception e)
            {
                // ADD UNKNOWN EXCEPTION HERE
                Log.Debug("BillPayment Presenter", "Stack " + e.StackTrace);
                //this.mView.ShowRetryOptionsUnknownException(e);
                if (this.mView.IsActive())
                {
                    this.mView.ShowNoInternetSnackbar();
                }
                Utility.LoggingNonFatalError(e);
            }
        }


        public void Start()
        {
            // NO IMPL
        }
    }
}