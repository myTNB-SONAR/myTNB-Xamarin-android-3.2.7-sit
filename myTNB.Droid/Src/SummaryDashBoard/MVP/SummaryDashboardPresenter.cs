using Android.Util;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.SummaryDashBoard.API;
using myTNB_Android.Src.SummaryDashBoard.Models;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
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

        bool refreshContent = false;

        public SummaryDashboardPresenter(SummaryDashboardContract.IView mView)
        {
            this.mView = mView;
            this.mView.SetPresenter(this);


        }


        public void FetchAccountSummary(bool makeSummaryApiCall = false)
        {
            try
            {
                if (SummaryDashBoardAccountEntity.GetAllItems().Count() == 0 || makeSummaryApiCall)
                {
                    this.mView.ShowProgressDialog();

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
            try
            {
                int forLoopCount = 0;

                int previousCount = 0;

                int i = 0;

                if (summaryDetailList != null && summaryDetailList.Count() > 0)
                {
                    curentLoadMoreCount = (summaryDetailList.Count() / Constants.SUMMARY_DASHBOARD_PAGE_COUNT);
                }

                if (billingAccoutCount > Constants.SUMMARY_DASHBOARD_PAGE_COUNT)
                {
                    previousCount = curentLoadMoreCount;
                    curentLoadMoreCount = curentLoadMoreCount + 1;
                    forLoopCount = curentLoadMoreCount * Constants.SUMMARY_DASHBOARD_PAGE_COUNT;
                    i = previousCount * Constants.SUMMARY_DASHBOARD_PAGE_COUNT;
                    if (billingAccoutCount < forLoopCount)
                    {
                        int diff = forLoopCount - billingAccoutCount;
                        diff = Constants.SUMMARY_DASHBOARD_PAGE_COUNT - diff;
                        forLoopCount = i + diff;
                    }
                }
                else
                {
                    forLoopCount = billingAccoutCount;
                }


                List<String> accounts = new List<string>();
                for (; i < forLoopCount; i++)
                {
                    if (!string.IsNullOrEmpty(customerBillingAccounts[i].AccNum))
                    {
                        accounts.Add(customerBillingAccounts[i].AccNum);
                    }
                }

                summaryDashboardRequest = new SummaryDashBordRequest();
                if (accounts != null && accounts.Count() > 0)
                {
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


        private async void SummaryDashBoardApiCall()
        {
            cts = new CancellationTokenSource();
#if STUB
            var api = RestService.For<ISummaryDashBoard>(Constants.SERVER_URL.END_POINT);
#elif DEBUG
            var httpClient = new HttpClient(new HttpLoggingHandler(/*new NativeMessageHandler()*/)) { BaseAddress = new Uri(Constants.SERVER_URL.END_POINT) };
            var api = RestService.For<ISummaryDashBoard>(httpClient);
#elif DEVELOP
            var api = RestService.For<ISummaryDashBoard>(Constants.SERVER_URL.END_POINT);
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
                    if (response.Data != null && response.Data.Status.ToUpper() == Constants.REFRESH_MODE)
                    {
                        mView.ShowRefreshSummaryDashboard(true, response.Data.RefreshMessage, response.Data.RefreshBtnText);
                        LoadEmptySummaryDetails();
                    }
                    else if (response.Data != null && !response.Data.isError && response.Data.data != null && response.Data.data.Count > 0)
                    {
                        var summaryDetails = response.Data.data;
                        for (int i = 0; i < summaryDetails.Count; i++)
                        {
                            CustomerBillingAccount cbAccount = CustomerBillingAccount.FindByAccNum(summaryDetails[i].AccNumber);
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
                        mView.ShowRefreshSummaryDashboard(false, null, null);
                    }
                    else
                    {
                        mView.ShowRefreshSummaryDashboard(true, null, null);
                        LoadEmptySummaryDetails();
                    }
                }
                else
                {
                    mView.ShowRefreshSummaryDashboard(true, null, null);
                    LoadEmptySummaryDetails();
                }
            }
            catch (System.OperationCanceledException e)
            {
                // ADD OPERATION CANCELLED HERE
                if (mView.IsActive())
                {
                    this.mView.HideProgressDialog();
                }
                Utility.LoggingNonFatalError(e);

                this.mView.ShowRefreshSummaryDashboard(true, null, null); //Show retry option for summary dashboard
                LoadEmptySummaryDetails();
            }
            catch (ApiException apiException)
            {
                // ADD HTTP CONNECTION EXCEPTION HERE
                if (this.mView.IsActive())
                {
                    this.mView.HideProgressDialog();
                }

                Utility.LoggingNonFatalError(apiException);
                this.mView.ShowRefreshSummaryDashboard(true, null, null); //Show retry option for summary dashboard
                LoadEmptySummaryDetails();
            }
            catch (Exception e)
            {
                // ADD UNKNOWN EXCEPTION HERE
                if (this.mView.IsActive())
                {
                    this.mView.HideProgressDialog();
                }
                Utility.LoggingNonFatalError(e);
                this.mView.ShowRefreshSummaryDashboard(true, null, null); //Show retry option for summary dashboard
                LoadEmptySummaryDetails();
            }
        }


        public void EnableLoadMore()
        {
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
            try
            {
                summaryDetailList = new List<SummaryDashBoardDetails>();
                customerBillingAccounts = new List<CustomerBillingAccount>();

                userEntity = UserEntity.GetActive();

                var reAccount = CustomerBillingAccount.REAccountList();

                var nonReAccount = CustomerBillingAccount.NonREAccountList();

                if (reAccount != null && reAccount.Count() > 0)
                {
                    customerBillingAccounts.AddRange(reAccount);
                }


                if (nonReAccount != null && nonReAccount.Count() > 0)
                {
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


        private void SummaryData(List<SummaryDashBoardDetails> summaryDetails = null)
        {
            try
            {
                if (summaryDetails != null && summaryDetails.Count > 0)
                {
                    if (refreshContent)
                    {
                        refreshContent = false;
                        foreach(SummaryDashBoardDetails detail in summaryDetails)
                        {
                            int selectedIndex = summaryDetailList.FindIndex(x => x.AccNumber == detail.AccNumber);
                            if (selectedIndex >= 0)
                            {
                                summaryDetailList.RemoveAt(selectedIndex);
                            }

                            summaryDetailList.Add(detail);
                        }
                    }
                    else
                    {
                        summaryDetailList.AddRange(summaryDetails);
                    }
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
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

        }

        public void DoLoadMoreAccount()
        {
            try
            {
                if (billingAccoutCount > summaryDetailList.Count())
                {
                    //FetchUserData();
                    FetchAccountSummary(true);
                    if (billingAccoutCount == summaryDetailList.Count())
                    {
                        mView.IsLoadMoreButtonVisible(false);
                    }

                }
                else
                {
                    mView.IsLoadMoreButtonVisible(false);
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void OnNotification()
        {
            this.mView.ShowNotification();
        }

        private List<CustomerBillingAccount> FindSelectedAccountAndMoveToTop(List<CustomerBillingAccount> customerBillingAccount)
        {
            try
            {
                int i = customerBillingAccount.FindIndex(x => x.IsSelected);


                if (i != -1)
                {
                    if (i != 0)
                    {
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
            try
            {
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
            try
            {
                int forLoopCount = 0;

                int i = 0;

                List<String> accounts = new List<string>();

                if (summaryDetailList != null && summaryDetailList.Count() > 0)
                {
                    List<SummaryDashBoardAccountEntity> list = SummaryDashBoardAccountEntity.GetAllItems();
                    foreach(SummaryDashBoardDetails account in summaryDetailList)
                    {
                        int searchedIndex = list.FindIndex(x => x.AccountNo == account.AccNumber);
                        if (searchedIndex < 0)
                        {
                            accounts.Add(account.AccNumber);
                        }
                    }
                }
                else
                {
                    forLoopCount = 5;
                    for (; i < forLoopCount; i++)
                    {
                        if (!string.IsNullOrEmpty(customerBillingAccounts[i].AccNum))
                        {
                            accounts.Add(customerBillingAccounts[i].AccNum);
                        }
                    }
                }

                summaryDashboardRequest = new SummaryDashBordRequest();
                if (accounts != null && accounts.Count() > 0)
                {
                    summaryDashboardRequest.AccNum = accounts;
                    summaryDashboardRequest.SspUserId = userEntity.UserID;
                    summaryDashboardRequest.ApiKeyId = Constants.APP_CONFIG.API_KEY_ID;
                }
                refreshContent = true;
                SummaryDashBoardApiCall();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void LoadEmptySummaryDetails()
        {

            if (summaryDashboardRequest == null)
            {
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
                    smDetails.AmountDue = "--";
                    smDetails.BillDueDate = "--";
                    summaryDetails.Add(smDetails);
                }

                SummaryData(summaryDetails);
            }
        }
    }
}
