using Android.Util;
using myTNB_Android.Src.AddAccount.Models;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.myTNBMenu.Api;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;

namespace myTNB_Android.Src.myTNBMenu.MVP.Fragment
{
    public class BillsPaymentFragmentPresenter : BillsPaymentFragmentContract.IUserActionsListener
    {

        internal readonly string TAG = typeof(BillsPaymentFragmentPresenter).Name;
        private BillsPaymentFragmentContract.IView mView;
        private AccountData selectedAccount;
        CancellationTokenSource cts;

        BillHistoryResponse billsHistoryResponse;
        BillHistoryResponseV5 billsHistoryResponseV5;
        PaymentHistoryResponse paymentHistoryResponse;
        PaymentHistoryResponseV5 paymentHistoryResponseV5;
        PaymentHistoryREResponse paymentHistoryREResponse;

        public BillsPaymentFragmentPresenter(BillsPaymentFragmentContract.IView mView, AccountData accountData)
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
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void OnPaymentTab()
        {
            try
            {
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
            try
            {
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
                }
                else
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
                if (this.mView.IsActive())
                {
                    this.mView.ShowNoInternet();
                }
                Utility.LoggingNonFatalError(e);
            }
            catch (ApiException apiException)
            {
                // ADD HTTP CONNECTION EXCEPTION HERE
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
                if (this.mView.IsActive())
                {
                    this.mView.ShowNoInternet();
                }

                Utility.LoggingNonFatalError(e);
            }
        }




        private async void LoadBills(CustomerBillingAccount accountSelected)
        {
            cts = new CancellationTokenSource();
#if STUB
            var detailedAccountApi = RestService.For<IDetailedCustomerAccount>(Constants.SERVER_URL.END_POINT);
#elif DEBUG
            var httpClient = new HttpClient(new HttpLoggingHandler(/*new NativeMessageHandler()*/)) { BaseAddress = new Uri(Constants.SERVER_URL.END_POINT) };
            var api = RestService.For<IUsageHistoryApi>(httpClient);

            var detailedAccountApi = RestService.For<IDetailedCustomerAccount>(httpClient);
#elif DEVELOP
            var detailedAccountApi = RestService.For<IDetailedCustomerAccount>(Constants.SERVER_URL.END_POINT);
#else
            var detailedAccountApi = RestService.For<IDetailedCustomerAccount>(Constants.SERVER_URL.END_POINT);
#endif

            try
            {
                AccountDetailsResponse customerBillingDetails = null;

                AccountDataEntity accountEntity = AccountDataEntity.GetItemByAccountNo(accountSelected.AccNum);
                if (accountEntity != null)
                {
                    customerBillingDetails = JsonConvert.DeserializeObject<AccountDetailsResponse>(accountEntity.JsonResponse);
                }


                if (customerBillingDetails == null)
                {
                    customerBillingDetails = await detailedAccountApi.GetDetailedAccount(new AddAccount.Requests.AccountDetailsRequest()

                    {
                        apiKeyID = Constants.APP_CONFIG.API_KEY_ID,
                        CANum = accountSelected.AccNum
                    }, cts.Token);
                }

                if (!customerBillingDetails.Data.IsError)
                {
                    AccountData accountData = AccountData.Copy(customerBillingDetails.Data.AccountData, true);
                    CustomerBillingAccount customerBillingAccount = CustomerBillingAccount.FindByAccNum(accountData.AccountNum);
                    accountData.AccountNickName = accountSelected.AccDesc;
                    accountData.AccountName = accountSelected.OwnerName;
                    accountData.AddStreet = accountSelected.AccountStAddress;
                    accountData.IsOwner = customerBillingAccount.isOwned;
                    accountData.AccountCategoryId = customerBillingAccount.AccountCategoryId;

                    selectedAccount = accountData;
                    this.mView.SetBillDetails(accountData);

                }
            }
            catch (System.OperationCanceledException e)
            {
                Log.Debug(TAG, "Cancelled Exception");
                // ADD OPERATION CANCELLED HERE
                Utility.LoggingNonFatalError(e);
            }
            catch (ApiException apiException)
            {
                // ADD HTTP CONNECTION EXCEPTION HERE
                Utility.LoggingNonFatalError(apiException);
            }
            catch (System.Exception e)
            {
                // ADD UNKNOWN EXCEPTION HERE
                Log.Debug(TAG, "Stack " + e.StackTrace);
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
                if (this.mView.IsActive())
                {
                    this.mView.ShowNoInternet();
                }

                Utility.LoggingNonFatalError(e);
            }
            catch (ApiException apiException)
            {
                // ADD HTTP CONNECTION EXCEPTION HERE
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
                if (this.mView.IsActive())
                {
                    this.mView.ShowNoInternet();
                }

                Utility.LoggingNonFatalError(e);
            }
        }

        void BillsPaymentFragmentContract.IUserActionsListener.LoadBills(CustomerBillingAccount accountSelected)
        {
            throw new NotImplementedException();
        }

        public void RefreshData()
        {
            CustomerBillingAccount selected = new CustomerBillingAccount();
            if (CustomerBillingAccount.HasSelected())
            {
                selected = CustomerBillingAccount.GetSelected();
            }
            else
            {
                List<CustomerBillingAccount> accountList = new List<CustomerBillingAccount>();
                accountList = CustomerBillingAccount.List();
                CustomerBillingAccount.SetSelected(accountList[0].AccNum);
                selected = CustomerBillingAccount.GetSelected();
            }

            LoadBills(selected);
        }

    }
}