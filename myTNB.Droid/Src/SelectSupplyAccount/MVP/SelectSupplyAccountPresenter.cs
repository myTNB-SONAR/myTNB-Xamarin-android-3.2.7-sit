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
using System.Net;
using System.Net.Http;
using System.Threading;

namespace myTNB_Android.Src.SelectSupplyAccount.MVP
{
    public class SelectSupplyAccountPresenter : SelectSupplyAccountContract.IUserActionsListener
    {
        CancellationTokenSource cts;
        private SelectSupplyAccountContract.IView mView;

        public SelectSupplyAccountPresenter(SelectSupplyAccountContract.IView mView)
        {
            this.mView = mView;
            this.mView.SetPresenter(this);
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
            cts = new CancellationTokenSource();
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

//            api.DoQuery(new myTNBMenu.Requests.UsageHistoryRequest(Constants.APP_CONFIG.API_KEY_ID)
//            {
//                AccountNum = customerBillingAccount.AccNum
//            }, cts.Token)
//            .ReturnsForAnyArgs(
//                Task.Run<UsageHistoryResponse>(
//                    () => JsonConvert.DeserializeObject<UsageHistoryResponse>(this.mView.GetUsageHistoryStub())
//                ));
//
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
                        this.mView.HideShowProgressDialog();
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
                    else
                    {
                        // TODO : SHOW ERROR WHEN NO BILLING IS RETURNED
                        this.mView.ShowQueryError(customerBillingDetails.Data.Message);
                    }


                }
                else
                {
                    if (mView.IsActive())
                    {
                        this.mView.HideShowProgressDialog();
                    }
                }


            }
            catch (System.OperationCanceledException e)
            {
                if (mView.IsActive())
                {
                    this.mView.HideShowProgressDialog();
                }
                // ADD OPERATION CANCELLED HERE
                Log.Debug("SelectSupplyAccountPresenter", e.Message + " " + e.StackTrace);
                this.mView.ShowRetryOptionsCancelledException(e);
                Utility.LoggingNonFatalError(e);
            }
            catch (ApiException apiException)
            {
                if (mView.IsActive())
                {
                    this.mView.HideShowProgressDialog();
                }
                // ADD HTTP CONNECTION EXCEPTION HERE
                Log.Debug("SelectSupplyAccountPresenter", apiException.Message + " " + apiException.StackTrace);
                this.mView.ShowRetryOptionsApiException(apiException);
                Utility.LoggingNonFatalError(apiException);
            }
            catch (Exception e)
            {
                if (mView.IsActive())
                {
                    this.mView.HideShowProgressDialog();
                }
                // ADD UNKNOWN EXCEPTION HERE
                if (!this.mView.HasInternetConnection())
                {
                    CustomerBillingAccount.RemoveSelected();
                    CustomerBillingAccount.Update(customerBillingAccount.AccNum, true);
                    this.mView.ShowNoInternetConnection();
                }
                else
                {
                    Log.Debug("SelectSupplyAccountPresenter", e.Message + " " + e.StackTrace);
                    this.mView.ShowRetryOptionsUnknownException(e);
                }
                Utility.LoggingNonFatalError(e);
            }


        }

        public void Start()
        {
            List<CustomerBillingAccount> custBAList = CustomerBillingAccount.List();

            this.mView.ShowList(custBAList);
        }
    }
}
