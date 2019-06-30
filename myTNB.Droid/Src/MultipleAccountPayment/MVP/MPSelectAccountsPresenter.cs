using Android.Util;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.MultipleAccountPayment.Api;
using myTNB_Android.Src.MultipleAccountPayment.Model;
using myTNB_Android.Src.MultipleAccountPayment.Requests;
using myTNB_Android.Src.myTNBMenu.Api;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;
using Refit;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;

namespace myTNB_Android.Src.MultipleAccountPayment.MVP
{
    public class MPSelectAccountsPresenter : MPSelectAccountsContract.IUserActionsListener
    {
        private static readonly string TAG = "MPSelectAccountsPresenter";
        private MPSelectAccountsContract.IView mView;

        public MPSelectAccountsPresenter(MPSelectAccountsContract.IView mView)
        {
            this.mView = mView;
            this.mView.SetPresenter(this);
        }

        public void Start()
        {

        }

        public void GetMultiAccountDueAmount(string apiKeyID, List<string> accounts, string preSelectedAccount)
        {
            ServicePointManager.ServerCertificateValidationCallback += SSLFactoryHelper.CertificateValidationCallBack;
            GetMultiAccountDueAmountAsync(apiKeyID, accounts, preSelectedAccount);
        }

        public void OnSelectAccount(CustomerBillingAccount selectedCustomerBilling)
        {
            try
            {
                ServicePointManager.ServerCertificateValidationCallback += SSLFactoryHelper.CertificateValidationCallBack;
                LoadDataUsage(selectedCustomerBilling);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }


        private async void LoadDataUsage(CustomerBillingAccount customerBillingAccount)
        {
            var cts = new CancellationTokenSource();
            if (mView.IsActive())
            {
                this.mView.ShowProgressDialog();
            }
#if STUB
            var api = RestService.For<IUsageHistoryApi>(Constants.SERVER_URL.END_POINT);
            var detailedAccountApi = RestService.For<IDetailedCustomerAccount>(Constants.SERVER_URL.END_POINT);
#elif DEBUG
            var httpClient = new HttpClient(new HttpLoggingHandler(/*new NativeMessageHandler()*/)) { BaseAddress = new Uri(Constants.SERVER_URL.END_POINT) };
            var api = RestService.For<IUsageHistoryApi>(httpClient);
            var detailedAccountApi = RestService.For<IDetailedCustomerAccount>(httpClient);
#elif DEVELOP
            var api = RestService.For<IUsageHistoryApi>(Constants.SERVER_URL.END_POINT);
            var detailedAccountApi = RestService.For<IDetailedCustomerAccount>(Constants.SERVER_URL.END_POINT);
#else
            var api = RestService.For<IUsageHistoryApi>(Constants.SERVER_URL.END_POINT);
            var detailedAccountApi = RestService.For<IDetailedCustomerAccount>(Constants.SERVER_URL.END_POINT);
#endif

            try
            {


                var response = await api.DoQuery(new myTNBMenu.Requests.UsageHistoryRequest(Constants.APP_CONFIG.API_KEY_ID)
                {
                    AccountNum = customerBillingAccount.AccNum
                }, cts.Token);

                /*** Save Usage History For the Day***/
                UsageHistoryEntity smUsageModel = new UsageHistoryEntity();
                smUsageModel.Timestamp = DateTime.Now.ToLocalTime();
                smUsageModel.JsonResponse = JsonConvert.SerializeObject(response);
                smUsageModel.AccountNo = customerBillingAccount.AccNum;
                UsageHistoryEntity.InsertItem(smUsageModel);
                /*****/

                if (response != null && response.Data.Status.Equals("success") && !response.Data.IsError)
                {

                    var customerBillingDetails = await detailedAccountApi.GetDetailedAccount(new AddAccount.Requests.AccountDetailsRequest()
                    {
                        apiKeyID = Constants.APP_CONFIG.API_KEY_ID,
                        CANum = customerBillingAccount.AccNum
                    }, cts.Token);

                    if (mView.IsActive())
                    {
                        this.mView.HideProgressDialog();
                    }

                    if (!customerBillingDetails.Data.IsError)
                    {
                        /*** Save account data For the Day***/
                        AccountDataEntity accountModel = new AccountDataEntity();
                        accountModel.Timestamp = DateTime.Now.ToLocalTime();
                        accountModel.JsonResponse = JsonConvert.SerializeObject(customerBillingDetails);
                        accountModel.AccountNo = customerBillingAccount.AccNum;
                        AccountDataEntity.InsertItem(accountModel);
                        /*****/

                        CustomerBillingAccount.RemoveSelected();
                        CustomerBillingAccount.Update(customerBillingAccount.AccNum, true);
                        AccountData accountData = AccountData.Copy(customerBillingDetails.Data.AccountData, true);
                        accountData.AccountNickName = customerBillingAccount.AccDesc;
                        accountData.AccountName = customerBillingAccount.OwnerName;
                        accountData.AddStreet = customerBillingAccount.AccountStAddress;
                        accountData.AccountCategoryId = customerBillingAccount.AccountCategoryId;
                        this.mView.ShowDashboardChart(response, accountData);
                    }
                }
                else
                {
                    if (mView.IsActive())
                    {
                        this.mView.HideProgressDialog();
                    }
                }


            }
            catch (System.OperationCanceledException e)
            {
                if (mView.IsActive())
                {
                    this.mView.HideProgressDialog();
                }
                Log.Debug("SelectAccountPresenter", e.Message + " " + e.StackTrace);
                Utility.LoggingNonFatalError(e);
            }
            catch (ApiException apiException)
            {
                if (mView.IsActive())
                {
                    this.mView.HideProgressDialog();
                }
                Log.Debug("SelectAccountPresenter", apiException.Message + " " + apiException.StackTrace);
                Utility.LoggingNonFatalError(apiException);
            }
            catch (Exception e)
            {
                if (mView.IsActive())
                {
                    this.mView.HideProgressDialog();
                }
                Log.Debug("SelectAccountPresenter", e.Message + " " + e.StackTrace);
                Utility.LoggingNonFatalError(e);
            }


        }

        public async void GetMultiAccountDueAmountAsync(string apiKeyId, List<string> accounts, string preSelectedAccount)
        {
            try
            {
                this.mView.ShowProgressDialog();

#if DEBUG || STUB
                var httpClient = new HttpClient(new HttpLoggingHandler(/*new NativeMessageHandler()*/)) { BaseAddress = new Uri(Constants.SERVER_URL.END_POINT) };
                var api = RestService.For<MPGetAccountsDueAmountApi>(httpClient);
#else
            var api = RestService.For<MPGetAccountsDueAmountApi>(Constants.SERVER_URL.END_POINT);
#endif

                List<MPAccount> storeAccounts = new List<MPAccount>();
                bool getDetailsFromApi = true;

                if (getDetailsFromApi)
                {
                    MPAccountDueResponse result = await api.GetMultiAccountDueAmount(new MPGetAccountDueAmountRequest(apiKeyId, accounts));
                    this.mView.HideProgressDialog();
                    if (result.accountDueAmountResponse != null && !result.accountDueAmountResponse.IsError)
                    {
                        this.mView.GetAccountDueAmountResult(result);
                    }
                    else
                    {
                        this.mView.ShowError(result.accountDueAmountResponse.Message);
                        this.mView.DisablePayButton();
                    }
                }
                else
                {
                    this.mView.HideProgressDialog();
                    this.mView.GetAccountDueAmountResult(storeAccounts);
                }
            }
            catch (Exception e)
            {
                Log.Debug(TAG, e.StackTrace);
                if (mView.IsActive())
                {
                    this.mView.HideProgressDialog();
                }
                Utility.LoggingNonFatalError(e);
                this.mView.ShowError("Something went wrong, Please try again.");
            }

        }
    }
}