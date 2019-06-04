﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using Android.Util;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.SummaryDashBoard.API;
using myTNB_Android.Src.SummaryDashBoard.Models;
using myTNB_Android.Src.Utils;
using Refit;
using Newtonsoft.Json;
using static myTNB_Android.Src.SummaryDashBoard.MVP.SummaryDashboardContract;

namespace myTNB_Android.Src.SummaryDashBoard.MVP
{
    public class SummaryDashboardPresenter : SummaryDashboardContract.ISummaryDashBoardListener
    {
        internal readonly string TAG = typeof(SummaryDashboardPresenter).Name;

        List<SummaryDashBoardDetails> summaryDetailList = null;
        private SummaryDashboardContract.IView mView;
        CancellationTokenSource cts;
        SummaryDashBordRequest summaryDashboardRequest = null;
        UserEntity userEntity = null;

        List<CustomerBillingAccount> customerBillingAccounts = null;

        int totalLoadMoreCount = 0;
        int curentLoadMoreCount = 0;
        int billingAccoutCount = 0;

        public SummaryDashboardPresenter(SummaryDashboardContract.IView mView)
        {
            this.mView = mView;
            this.mView.SetPresenter(this);


        }


        public void FetchAccountSummary(bool makeSummaryApiCall = false)
        {
            try
            {
                //if (CustomerBillingAccount.List().Count() != SummaryDashBoardAccountEntity.GetAllItems().Count())
                if (SummaryDashBoardAccountEntity.GetAllItems().Count() == 0 || makeSummaryApiCall)
                {
                    //if (mView.IsActive())
                    //{
                    this.mView.ShowProgressDialog();
                    //}
                    FetchUserData();
                    if (summaryDashboardRequest != null)
                    {
                        SummaryDashBoardApiCall();
                    }
                    else
                    {
                        if (mView.IsActive())
                        {
                            this.mView.HideProgressDialog();
                        }
                    }
                }
                else
                {
                    if (summaryDetailList != null && summaryDetailList.Count > 0)
                    {
                        SummaryData();

                    }
                    else
                    {
                        List<SummaryDashBoardAccountEntity> list = SummaryDashBoardAccountEntity.GetAllItems();
                        List<SummaryDashBoardDetails> summaryDetailDBList = new List<SummaryDashBoardDetails>();
                        foreach (SummaryDashBoardAccountEntity details in list)
                        {
                            SummaryDashBoardDetails accountDetails = JsonConvert.DeserializeObject<SummaryDashBoardDetails>(details.JsonResponse);
                            if (!summaryDetailDBList.Any(p => p.AccNumber.Equals(accountDetails.AccNumber)))
                            {
                                summaryDetailDBList.Add(accountDetails);
                            }
                        }
                        SummaryData(summaryDetailDBList);
                    }
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }




        public void FetchUserData()
        {
            try {
            int forLoopCount = 0;

            int previousCount = 0;

            int i = 0;

            if (summaryDetailList != null && summaryDetailList.Count() > 0) {
                curentLoadMoreCount = (summaryDetailList.Count() / Constants.SUMMARY_DASHBOARD_PAGE_COUNT);    
            }

            if (billingAccoutCount > Constants.SUMMARY_DASHBOARD_PAGE_COUNT) {
                previousCount = curentLoadMoreCount;
                curentLoadMoreCount = curentLoadMoreCount + 1;
                forLoopCount = curentLoadMoreCount * Constants.SUMMARY_DASHBOARD_PAGE_COUNT;
                i = previousCount * Constants.SUMMARY_DASHBOARD_PAGE_COUNT;
                if (billingAccoutCount < forLoopCount) {
                    int diff = forLoopCount - billingAccoutCount;
                    diff = Constants.SUMMARY_DASHBOARD_PAGE_COUNT - diff;
                    forLoopCount = i + diff;
                }
            } else {
                forLoopCount = billingAccoutCount;
            }


            List<String> accounts = new List<string>();
            for (; i < forLoopCount; i++) {

                //if (CustomerBillingAccount.List().Count() != SummaryDashBoardAccountEntity.GetAllItems().Count())
                //{
                //    SummaryDashBoardAccountEntity accountEntity = SummaryDashBoardAccountEntity.GetItemByAccountNo(customerBillingAccounts[i].AccNum);
                //    if(accountEntity != null)
                //    {
                //        SummaryDashBoardDetails accountDetails = JsonConvert.DeserializeObject<SummaryDashBoardDetails>(accountEntity.JsonResponse);
                //        summaryDetailList.Add(accountDetails);
                //    }
                //    else
                //    {
                //        accounts.Add(customerBillingAccounts[i].AccNum);
                //    }

                //}
                //else
                //{
                if (!string.IsNullOrEmpty(customerBillingAccounts[i].AccNum)) {
                    accounts.Add(customerBillingAccounts[i].AccNum);
                }
                //}
            }

            summaryDashboardRequest = new SummaryDashBordRequest();
            if (accounts != null && accounts.Count() > 0) {
                summaryDashboardRequest.AccNum = accounts;
                summaryDashboardRequest.SspUserId = userEntity.UserID;
                summaryDashboardRequest.ApiKeyId = Constants.APP_CONFIG.API_KEY_ID;
            }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }


        private async void SummaryDashBoardApiCall() {
            cts = new CancellationTokenSource();
            //if (mView.IsActive()) {
            //this.mView.ShowProgressDialog();
            //}
#if STUB
            var api = Substitute.For<IUsageHistoryApi>();

            var detailedAccountApi = Substitute.For<IDetailedCustomerAccount>();

            api.DoQuery(new Requests.UsageHistoryRequest(Constants.APP_CONFIG.API_KEY_ID) {
                AccountNum = accountSelected.AccNum
            }, cts.Token)
            .ReturnsForAnyArgs(
                Task.Run<UsageHistoryResponse>(
                    () => JsonConvert.DeserializeObject<UsageHistoryResponse>(this.mView.GetUsageHistoryStub())
                ));

            detailedAccountApi.GetDetailedAccount(new AddAccount.Requests.AccountDetailsRequest()
            {
                apiKeyID = Constants.APP_CONFIG.API_KEY_ID,
                CANum = accountSelected.AccNum
            })
            .ReturnsForAnyArgs(
                Task.Run<AccountDetailsResponse>(
                    () => JsonConvert.DeserializeObject<AccountDetailsResponse>(this.mView.GetAccountDetailsStub(accountSelected.AccNum))
                ));


            api.GetDetailedAccount(new AddAccount.Requests.AccountDetailsRequest()
            {
                apiKeyID = Constants.APP_CONFIG.API_KEY_ID,
                CANum = accountSelected.AccNum
            })
            .ReturnsForAnyArgs(
                Task.Run<AccountDetailsResponse>(
                    () => JsonConvert.DeserializeObject<AccountDetailsResponse>(this.mView.GetAccountDetailsStub(accountSelected.AccNum))
                ));
#elif DEBUG
            var httpClient = new HttpClient(new HttpLoggingHandler(/*new NativeMessageHandler()*/)) { BaseAddress = new Uri(Constants.SERVER_URL.END_POINT) };
            httpClient.Timeout = TimeSpan.FromSeconds(10);
            var api = RestService.For<ISummaryDashBoard>(httpClient);
#elif DEVELOP
            var api = RestService.For<ISummaryDashBoard>(Constants.SERVER_URL.END_POINT);

            //api.DoQuery(new Requests.UsageHistoryRequest(Constants.APP_CONFIG.API_KEY_ID) {
            //    AccountNum = accountSelected.AccNum
            //}, cts.Token)
            //.ReturnsForAnyArgs(
            //    Task.Run<UsageHistoryResponse>(
            //        () => JsonConvert.DeserializeObject<UsageHistoryResponse>(this.mView.GetUsageHistoryStub())
            //    ));
#else
            var api = RestService.For<ISummaryDashBoard>(Constants.SERVER_URL.END_POINT);
#endif



            try
            {
                var response = await api.GetLinkedAccountsSummaryInfo(summaryDashboardRequest, cts.Token);
                if (mView.IsActive())
                {
                    this.mView.HideProgressDialog();
                }
                if (response != null)
                {
                    if (response.Data != null) {
                        var summaryDetails = response.Data.data;
                        if (summaryDetails != null && summaryDetails.Count > 0) {
                            for (int i = 0; i < summaryDetails.Count; i++) {
                                CustomerBillingAccount cbAccount= CustomerBillingAccount.FindByAccNum(summaryDetails[i].AccNumber);
                                summaryDetails[i].AccName = cbAccount.AccDesc;
                                summaryDetails[i].AccType = cbAccount.AccountCategoryId;
                                summaryDetails[i].IsAccSelected = cbAccount.IsSelected;

                                /*** Save account data For the Day***/
                                SummaryDashBoardAccountEntity accountModel = new SummaryDashBoardAccountEntity();
                                accountModel.Timestamp = DateTime.Now.ToLocalTime();
                                accountModel.JsonResponse = JsonConvert.SerializeObject(summaryDetails[i]);
                                accountModel.AccountNo = summaryDetails[i].AccNumber;
                                SummaryDashBoardAccountEntity.InsertItem(accountModel);
                                /*****/
                            }
                            
                            SummaryData(summaryDetails);
                        }

                    }
                    mView.ShowRefreshSummaryDashboard(false);
                }
                
                //    if (accountSelected.isOwned)
                //    {

                //        if (currentBottomNavigationMenu == Resource.Id.menu_dashboard)
                //        {
                //            this.mView.ShowAccountName();
                //            this.mView.SetToolbarTitle(Resource.String.dashboard_menu_activity_title);
                //            if (smDataError)
                //            {
                //                smDataError = false;
                //                if (smErrorCode.Equals("204"))
                //                {
                //                    this.mView.ShowChartWithError(response.Data.UsageHistoryData, AccountData.Copy(accountSelected, true), smErrorCode);
                //                }
                //            }
                //            else
                //            {
                //                this.mView.ShowChart(response.Data.UsageHistoryData, AccountData.Copy(accountSelected, true));
                //            }
                //        }
                //        else if (currentBottomNavigationMenu == Resource.Id.menu_bill)
                //        {
                //            this.mView.ShowAccountName();
                //            this.mView.SetToolbarTitle(Resource.String.bill_menu_activity_title);
                //            LoadBills(accountSelected);
                //        }


                //    }
                //    else
                //    {
                //        this.mView.ShowNonOWner(AccountData.Copy(accountSelected, true));
                //    }
                //    this.mView.SetAccountName(accountSelected.AccDesc);

                //}
                //else
                //{
                //    this.mView.ShowOwnerDashboardNoInternetConnection(accountSelected.AccDesc);
                //    this.mView.SetAccountName(accountSelected.AccDesc);
                //}
            }
            catch (System.OperationCanceledException e)
            {
                Log.Debug(TAG, "Cancelled Exception");
                // ADD OPERATION CANCELLED HERE
                //this.mView.ShowRetryOptionsCancelledException(e);
                //this.mView.ShowOwnerNoInternetConnection(accountSelected.AccDesc);
                if (mView.IsActive())
                {
                    this.mView.HideProgressDialog();
                }
                Utility.LoggingNonFatalError(e);
                this.mView.ShowRefreshSummaryDashboard(true); //Show retry option for summary dashboard
                LoadEmptySummaryDetails();
            }
            catch (ApiException apiException)
            {
                // ADD HTTP CONNECTION EXCEPTION HERE
                //this.mView.ShowRetryOptionsApiException(apiException);
                if (this.mView.IsActive())
                {
                    this.mView.HideProgressDialog();
                    //this.mView.ShowOwnerDashboardNoInternetConnection(accountSelected.AccDesc);
                }  

                Utility.LoggingNonFatalError(apiException);
                this.mView.ShowRefreshSummaryDashboard(true); //Show retry option for summary dashboard
                LoadEmptySummaryDetails();
            }
            catch (Exception e)
            {
                // ADD UNKNOWN EXCEPTION HERE
                Log.Debug(TAG, "Stack " + e.StackTrace);
                //this.mView.ShowRetryOptionsUnknownException(e);
                if (this.mView.IsActive())
                {
                    this.mView.HideProgressDialog();
                    //this.mView.ShowOwnerDashboardNoInternetConnection(accountSelected.AccDesc);
                }
                Utility.LoggingNonFatalError(e);
                this.mView.ShowRefreshSummaryDashboard(true); //Show retry option for summary dashboard
                LoadEmptySummaryDetails();
            }

            //if (this.mView.IsActive())
            //{
            //    this.mView.HideProgressDialog();
            //}
        }


        public void EnableLoadMore() {
            if (billingAccoutCount > Constants.SUMMARY_DASHBOARD_PAGE_COUNT)
            {
                totalLoadMoreCount = billingAccoutCount / Constants.SUMMARY_DASHBOARD_PAGE_COUNT;
                this.mView.IsLoadMoreButtonVisible(true);
            }
            else
            {
                this.mView.IsLoadMoreButtonVisible(false);
            }
        }

        public void Start()
        {
            try {
            summaryDetailList = new List<SummaryDashBoardDetails>();
            customerBillingAccounts = new List<CustomerBillingAccount>();

            userEntity = UserEntity.GetActive();
            //customerBillingAccounts = CustomerBillingAccount.List();

            var reAccount = CustomerBillingAccount.REAccountList();

            var nonReAccount = CustomerBillingAccount.NonREAccountList();

            if (reAccount != null && reAccount.Count() > 0) {
                /**Since Summary dashBoard logic is changed these codes where commented on 01-11-2018**/
                //reAccount = FindSelectedAccountAndMoveToTop(reAccount);
                /**Since Summary dashBoard logic is changed these codes where commented on 01-11-2018**/
                customerBillingAccounts.AddRange(reAccount);
            }


            if (nonReAccount != null && nonReAccount.Count() > 0)
            {
                /**Since Summary dashBoard logic is changed these codes where commented on 01-11-2018**/
                //nonReAccount = FindSelectedAccountAndMoveToTop(nonReAccount);
                /**Since Summary dashBoard logic is changed these codes where commented on 01-11-2018**/
                customerBillingAccounts.AddRange(nonReAccount);
            }

            billingAccoutCount = customerBillingAccounts.Count();



            DateTime dt = DateTime.Now.ToLocalTime();
                
            int hour_only = dt.Hour;


            if (hour_only >= 6 && hour_only < 12)
            {
                //Good Morning...
                mView.SetGreetingImageAndText(eGreeting.MORNING, 
                                              MyTNBApplication.Context.GetString(Resource.String.greeting_text_morning));
            }
            else if (hour_only >= 12 && hour_only < 18)
            {
                //Good Afternoon...
                mView.SetGreetingImageAndText(eGreeting.AFTERNOON, 
                                              MyTNBApplication.Context.GetString(Resource.String.greeting_text_afternoon));
            }
            else if (hour_only >= 0 && hour_only < 6)
            {
                        //Evening illustration and morning greeting text...
                mView.SetGreetingImageAndText(eGreeting.EVENING, 
                                              MyTNBApplication.Context.GetString(Resource.String.greeting_text_morning));
            }
            else
            {
                //Good Evening...
                mView.SetGreetingImageAndText(eGreeting.EVENING, 
                                              MyTNBApplication.Context.GetString(Resource.String.greeting_text_evening));
            }


            this.mView.SetUserName(userEntity.DisplayName);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }


        private void SummaryData(List<SummaryDashBoardDetails> summaryDetails = null) {
            try
            {
                if (summaryDetails != null && summaryDetails.Count > 0)
                {
                    summaryDetailList.AddRange(summaryDetails);
                }


                List<SummaryDashBoardDetails> reAccount = (from item in summaryDetailList
                                                           where item.AccType == "2"
                                                           select item).ToList();


                List<SummaryDashBoardDetails> normalAccount = (from item in summaryDetailList
                                                               where item.AccType != "2"
                                                               select item).ToList();

                /**Since Summary dashBoard logic is changed these codes where commented on 01-11-2018**/
                //reAccount = FindSelectedAccountAndMoveToTop(reAccount);
                //normalAccount = FindSelectedAccountAndMoveToTop(normalAccount);
                /**Since Summary dashBoard logic is changed these codes where commented on 01-11-2018**/


                this.mView.LoadREAccountData(reAccount.OrderBy(x => x.AccName).ToList());
                this.mView.LoadNormalAccountData(normalAccount.OrderBy(x => x.AccName).ToList());

                if (billingAccoutCount == summaryDetailList.Count())
                {
                    mView.IsLoadMoreButtonVisible(false);
                }
                else if (billingAccoutCount > summaryDetailList.Count())
                {
                    mView.IsLoadMoreButtonVisible(true);
                }
            }catch(Exception e) {
                Utility.LoggingNonFatalError(e);
            }

        }

        public void DoLoadMoreAccount()
        {
            try {
            if (billingAccoutCount > summaryDetailList.Count()) {
                //FetchUserData();
                FetchAccountSummary(true);
                if (billingAccoutCount == summaryDetailList.Count()) {
                    mView.IsLoadMoreButtonVisible(false);
                }

            } else {
                mView.IsLoadMoreButtonVisible(false);
            }                
        }catch(Exception e) {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void OnNotification()
        {
            if (this.mView.HasNetworkConnection())
            {
                this.mView.ShowNotification();
            }
            else
            {
                this.mView.ShowNoInternetSnackbar();
            }
        }

        private List<CustomerBillingAccount> FindSelectedAccountAndMoveToTop(List<CustomerBillingAccount> customerBillingAccount) {
            //var selectedAccount = (from item in summaryDetailList 
            //where item.IsAccSelected == true select item).ToList();

            try {
            int i = customerBillingAccount.FindIndex(x => x.IsSelected);


            if (i != -1) {
                if (i != 0) {
                    CustomerBillingAccount tempCustomerBillinAccount = new CustomerBillingAccount();

                    tempCustomerBillinAccount = customerBillingAccount[i];

                    customerBillingAccount[i] = customerBillingAccount[0];

                    customerBillingAccount[0] = tempCustomerBillinAccount;
                }


            }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

            return customerBillingAccount;

        }

        private List<SummaryDashBoardDetails> FindSelectedAccountAndMoveToTop(List<SummaryDashBoardDetails> SummaryDetails)
        {
            //var selectedAccount = (from item in summaryDetailList 
            //where item.IsAccSelected == true select item).ToList();
            try {
            CustomerBillingAccount customerBillingAccount = CustomerBillingAccount.GetSelected();

            int i = SummaryDetails.FindIndex(x => x.AccNumber == customerBillingAccount.AccNum);

            if (i != -1)
            {
                if (i != 0)
                {
                    SummaryDashBoardDetails tempSummaryDashBoardDetails = new SummaryDashBoardDetails();

                    tempSummaryDashBoardDetails = SummaryDetails[i];

                    SummaryDetails[i] = SummaryDetails[0];

                    SummaryDetails[0] = tempSummaryDashBoardDetails;
                }


            }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

            return SummaryDetails;

        }

        public void RefreshAccountSummary()
        {
            summaryDetailList.Clear();
            SummaryDashBoardApiCall();
        }

        public void LoadEmptySummaryDetails()
        {

            if(summaryDashboardRequest == null){
                FetchUserData();
            }

            if (summaryDashboardRequest != null && summaryDashboardRequest.AccNum.Count > 0)
            {
                List<SummaryDashBoardDetails> summaryDetails = new List<SummaryDashBoardDetails>();
                for (int i = 0; i < summaryDashboardRequest.AccNum.Count; i++)
                {
                    CustomerBillingAccount cbAccount = CustomerBillingAccount.FindByAccNum(summaryDashboardRequest.AccNum[i]);
                    SummaryDashBoardDetails smDetails = new SummaryDashBoardDetails();
                    smDetails.AccName = cbAccount.AccDesc;
                    smDetails.AccNumber = cbAccount.AccNum;
                    smDetails.AccType = cbAccount.AccountCategoryId;
                    smDetails.IsAccSelected = cbAccount.IsSelected;
                    smDetails.AmountDue = "0.00";
                    smDetails.BillDueDate = "--";
                    summaryDetails.Add(smDetails);
                }

                SummaryData(summaryDetails);
                mView.IsLoadMoreButtonVisible(false);
            }
        }
    }
}
