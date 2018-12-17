using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.Utils;
using System.Net;
using Refit;
using myTNB_Android.Src.myTNBMenu.Api;
using System.Net.Http;
using System.Threading;
using Android.Util;
using myTNB_Android.Src.Database.Model;
using Newtonsoft.Json;

namespace myTNB_Android.Src.myTNBMenu.MVP.Fragment
{
    public class BillsPaymentFragmentPresenter : BillsPaymentFragmentContract.IUserActionsListener
    {
        private BillsPaymentFragmentContract.IView mView;
        private AccountData selectedAccount;
        CancellationTokenSource cts;

        BillHistoryResponse billsHistoryResponse;
        BillHistoryResponseV5 billsHistoryResponseV5;
        PaymentHistoryResponse paymentHistoryResponse;
        PaymentHistoryResponseV5 paymentHistoryResponseV5;
        PaymentHistoryREResponse paymentHistoryREResponse;

        public BillsPaymentFragmentPresenter(BillsPaymentFragmentContract.IView mView , AccountData accountData)
        {
            this.mView = mView;
            this.selectedAccount = accountData;
            this.mView.SetPresenter(this);
        }

        public void OnBillTab()
        {
            try
            {
                if (billsHistoryResponseV5 != null && !billsHistoryResponseV5.Data.IsError && billsHistoryResponseV5.Data.Status.Equals("success"))
                {
                    this.mView.ShowBillsList(billsHistoryResponseV5);
                }
                else
                {
                    if (!BillHistoryEntity.IsSMDataUpdated(selectedAccount.AccountNum))
                    {
                        BillHistoryEntity storedEntity = BillHistoryEntity.GetItemByAccountNo(selectedAccount.AccountNum);
                        if (storedEntity != null)
                        {
                            billsHistoryResponseV5 = JsonConvert.DeserializeObject<BillHistoryResponseV5>(storedEntity.JsonResponse);
                            if (billsHistoryResponseV5.Data.BillHistory != null && billsHistoryResponseV5.Data.BillHistory.Count() > 0)
                            {
                                this.mView.ShowBillsList(billsHistoryResponseV5);
                            }
                            else
                            {
                                LoadingBillsHistory();
                            }
                            if (this.mView.IsActive())
                            {
                                this.mView.EnableTabs();
                            }
                        }
                        else
                        {
                            LoadingBillsHistory();
                        }
                    }
                    else
                    {
                        LoadingBillsHistory();
                    }
                }
            } catch(Exception e) {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void OnPaymentTab()
        {
            try {
            if (selectedAccount.AccountCategoryId.Equals("2"))
            {
                if (paymentHistoryREResponse != null && !paymentHistoryREResponse.Data.IsError && paymentHistoryREResponse.Data.Status.Equals("success"))
                {
                    this.mView.ShowREPaymentList(paymentHistoryREResponse);
                }
                else
                {
                    if (!PaymentHistoryEntity.IsSMDataUpdated(selectedAccount.AccountNum))
                    {
                        REPaymentHistoryEntity storedEntity = REPaymentHistoryEntity.GetItemByAccountNo(selectedAccount.AccountNum);
                        if (storedEntity != null)
                        {
                            paymentHistoryREResponse = JsonConvert.DeserializeObject<PaymentHistoryREResponse>(storedEntity.JsonResponse);
                            if (paymentHistoryREResponse.Data.PaymentHistoryRE != null && paymentHistoryREResponse.Data.PaymentHistoryRE.Count() > 0)
                            {
                                this.mView.ShowREPaymentList(paymentHistoryREResponse);
                            }else{
                                LoadingREPaymentHistory();
                            }
                        }
                        else
                        {
                            LoadingREPaymentHistory();
                        }
                    }
                    else
                    {
                        LoadingREPaymentHistory();
                    }
                    
                }
            }
            else
            {
                if (paymentHistoryResponseV5 != null && !paymentHistoryResponseV5.Data.IsError && paymentHistoryResponseV5.Data.Status.Equals("success"))
                {
                    this.mView.ShowPaymentList(paymentHistoryResponseV5);
                }
                else
                {
                    if (!PaymentHistoryEntity.IsSMDataUpdated(selectedAccount.AccountNum))
                    {
                        PaymentHistoryEntity storedEntity = PaymentHistoryEntity.GetItemByAccountNo(selectedAccount.AccountNum);
                        if (storedEntity != null)
                        {
                            paymentHistoryResponseV5 = JsonConvert.DeserializeObject<PaymentHistoryResponseV5>(storedEntity.JsonResponse);
                            if (paymentHistoryResponseV5.Data.PaymentHistory != null && paymentHistoryResponseV5.Data.PaymentHistory.Count() > 0)
                            {
                                this.mView.ShowPaymentList(paymentHistoryResponseV5);
                            }else{
                                LoadingPaymentHistory();
                            }
                        }
                        else
                        {
                            LoadingPaymentHistory();
                        }
                    }
                    else
                    {
                        LoadingPaymentHistory();
                    }
                    
                }
            }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void OnPay()
        {
            this.mView.ShowPayment();
        }

        public void OnShowBills()
        {
            this.mView.ShowBillsList(billsHistoryResponseV5);
        }

        public void OnShowPayment()
        {
            this.mView.ShowPaymentList(paymentHistoryResponseV5);
        }

        public void Start()
        {
            try {
            // NO IMPL
            this.mView.DisableTabs();
            ServicePointManager.ServerCertificateValidationCallback += SSLFactoryHelper.CertificateValidationCallBack;

            if (!BillHistoryEntity.IsSMDataUpdated(selectedAccount.AccountNum))
            {
                BillHistoryEntity storedEntity = BillHistoryEntity.GetItemByAccountNo(selectedAccount.AccountNum);
                if (storedEntity != null)
                {
                    billsHistoryResponseV5 = JsonConvert.DeserializeObject<BillHistoryResponseV5>(storedEntity.JsonResponse);
                    this.mView.ShowBillsList(billsHistoryResponseV5);
                    if (this.mView.IsActive())
                    {
                        this.mView.EnableTabs();
                    }
                }
                else
                {
                    LoadingBillsHistory();
                }
            }
            else
            {
                LoadingBillsHistory();
            }
            

            if (selectedAccount.AccountCategoryId.Equals("2"))
            {
                this.mView.ShowAccountRE();
            }
            else
            {
                this.mView.ShowNormalAccount();
            }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
       }

        private async void LoadingBillsHistory()
        {

            if (this.mView.HasNoInternet())
            {
                return;
            }

            cts = new CancellationTokenSource();
#if DEBUG || STUB
            var httpClient = new HttpClient(new HttpLoggingHandler(/*new NativeMessageHandler()*/)) { BaseAddress = new Uri(Constants.SERVER_URL.END_POINT) };
            var api = RestService.For<IBillsPaymentHistoryApi>(httpClient);
#else
            var api = RestService.For<IBillsPaymentHistoryApi>(Constants.SERVER_URL.END_POINT);
#endif

            try
            {
                var billsHistoryResponseApi = await api.GetBillHistoryV5(new Requests.BillHistoryRequest(Constants.APP_CONFIG.API_KEY_ID)
                {
                    AccountNum = selectedAccount.AccountNum,
                    IsOwner = selectedAccount.IsOwner,
                    Email = UserEntity.GetActive().Email
                }, cts.Token);

                this.billsHistoryResponseV5 = billsHistoryResponseApi;

                if (!billsHistoryResponseV5.Data.IsError && billsHistoryResponseV5.Data.Status.Equals("success"))
                {
                    /*** Save Bill History For the Day ***/
                    BillHistoryEntity smUsageModel = new BillHistoryEntity();
                    smUsageModel.Timestamp = DateTime.Now.ToLocalTime();
                    smUsageModel.JsonResponse = JsonConvert.SerializeObject(billsHistoryResponseV5);
                    smUsageModel.AccountNo = selectedAccount.AccountNum;
                    BillHistoryEntity.InsertItem(smUsageModel);
                    /*****/

                    if (this.mView.IsActive())
                    {
                        this.mView.ShowBillsList(billsHistoryResponseV5);
                    }
                }else
                {
                    if (this.mView.IsActive())
                    {
                        this.mView.ShowEmptyBillList();
                    }
                }

                if (this.mView.IsActive())
                {
                    this.mView.EnableTabs();
                }
                
            }
            catch (System.OperationCanceledException e)
            {
                Log.Debug("BillPayment Presenter", "Cancelled Exception");
                if (this.mView.IsActive())
                {
                    this.mView.ShowNoInternet();
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
                    this.mView.ShowNoInternet();
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
                    this.mView.ShowNoInternet();
                }
                Utility.LoggingNonFatalError(e);
            }
        }

        private async void LoadingPaymentHistory()
        {

            if (this.mView.HasNoInternet())
            {
                return;
            }

            cts = new CancellationTokenSource();
#if DEBUG || STUB
            var httpClient = new HttpClient(new HttpLoggingHandler(/*new NativeMessageHandler()*/)) { BaseAddress = new Uri(Constants.SERVER_URL.END_POINT) };
            var api = RestService.For<IBillsPaymentHistoryApi>(httpClient);
#else
            var api = RestService.For<IBillsPaymentHistoryApi>(Constants.SERVER_URL.END_POINT);
#endif

            try
            {

                var paymentHistoryResponseApi = await api.GetPaymentHistoryV5(new Requests.PaymentHistoryRequest(Constants.APP_CONFIG.API_KEY_ID)
                {
                    AccountNum = selectedAccount.AccountNum,
                    IsOwner = selectedAccount.IsOwner,
                    Email = UserEntity.GetActive().Email
                }, cts.Token);

                this.paymentHistoryResponseV5 = paymentHistoryResponseApi;

                if (!paymentHistoryResponseV5.Data.IsError && paymentHistoryResponseV5.Data.Status.Equals("success"))
                {
                    /*** Save Payment History For the Day***/
                    PaymentHistoryEntity smUsageModel = new PaymentHistoryEntity();
                    smUsageModel.Timestamp = DateTime.Now.ToLocalTime();
                    smUsageModel.JsonResponse = JsonConvert.SerializeObject(paymentHistoryResponseV5);
                    smUsageModel.AccountNo = selectedAccount.AccountNum;
                    PaymentHistoryEntity.InsertItem(smUsageModel);
                    /*****/

                    if (this.mView.IsActive())
                    {
                        this.mView.ShowPaymentList(paymentHistoryResponseV5);
                    }
                    
                }
                else
                {
                    if (this.mView.IsActive())
                    {
                        this.mView.ShowEmptyPaymentList();
                    }
                    
                }
             

            }
            catch (System.OperationCanceledException e)
            {
                Log.Debug("BillPayment Presenter", "Cancelled Exception");
                // ADD OPERATION CANCELLED HERE
                //this.mView.ShowRetryOptionsCancelledException(e);
                if (this.mView.IsActive())
                {
                    this.mView.ShowNoInternet();
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
                    this.mView.ShowNoInternet();
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
                    this.mView.ShowNoInternet();
                }

                Utility.LoggingNonFatalError(e);
            }
        }


        private async void LoadingREPaymentHistory()
        {

            if (this.mView.HasNoInternet())
            {
                return;
            }

            cts = new CancellationTokenSource();
#if DEBUG || STUB
            var httpClient = new HttpClient(new HttpLoggingHandler(/*new NativeMessageHandler()*/)) { BaseAddress = new Uri(Constants.SERVER_URL.END_POINT) };
            var api = RestService.For<IBillsPaymentHistoryApi>(httpClient);
#else
            var api = RestService.For<IBillsPaymentHistoryApi>(Constants.SERVER_URL.END_POINT);
#endif

            try
            {

                var paymentHistoryResponseApi = await api.GetREPaymentHistory(new Requests.REPaymentHistoryRequest()
                {
                    ApiKeyID = Constants.APP_CONFIG.API_KEY_ID,
                    AccountNum = selectedAccount.AccountNum,
                    IsOwner = selectedAccount.IsOwner,
                    Email = UserEntity.GetActive().Email
                }, cts.Token);

                this.paymentHistoryREResponse = paymentHistoryResponseApi;

                if (!paymentHistoryREResponse.Data.IsError && paymentHistoryREResponse.Data.Status.Equals("success"))
                {
                    /*** Save Payment History For the Day***/
                    REPaymentHistoryEntity smUsageModel = new REPaymentHistoryEntity();
                    smUsageModel.Timestamp = DateTime.Now.ToLocalTime();
                    smUsageModel.JsonResponse = JsonConvert.SerializeObject(paymentHistoryREResponse);
                    smUsageModel.AccountNo = selectedAccount.AccountNum;
                    REPaymentHistoryEntity.InsertItem(smUsageModel);
                    /*****/

                    if (this.mView.IsActive())
                    {
                        this.mView.ShowREPaymentList(paymentHistoryREResponse);
                    }

                }
                else
                {
                    if (this.mView.IsActive())
                    {
                        this.mView.ShowEmptyPaymentList();
                    }

                }


            }
            catch (System.OperationCanceledException e)
            {
                Log.Debug("BillPayment Presenter", "Cancelled Exception");
                // ADD OPERATION CANCELLED HERE
                //this.mView.ShowRetryOptionsCancelledException(e);
                if (this.mView.IsActive())
                {
                    this.mView.ShowNoInternet();
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
                    this.mView.ShowNoInternet();
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
                    this.mView.ShowNoInternet();
                }

                Utility.LoggingNonFatalError(e);
            }
        }
    }
}