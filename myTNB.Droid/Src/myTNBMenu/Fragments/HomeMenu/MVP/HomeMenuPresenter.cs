﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Util;
using myTNB.SitecoreCMS.Model;
using myTNB.SitecoreCMS.Services;
using myTNB_Android.Src.Base;
using myTNB_Android.Src.AppLaunch.Models;
using myTNB_Android.Src.AppLaunch.Requests;
using myTNB_Android.Src.Base.Models;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.MVP.Models;
using myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.Requests;
using myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.Service;
using myTNB_Android.Src.SiteCore;
using myTNB_Android.Src.SitecoreCMS.Model;
using myTNB_Android.Src.SummaryDashBoard.Models;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;
using Refit;
using Android.Graphics;
using System.IO;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.myTNBMenu.Requests;
using System.Net.Http;
using myTNB_Android.Src.myTNBMenu.Api;
using static myTNB_Android.Src.AppLaunch.Models.MasterDataResponse;
using myTNB_Android.Src.MyTNBService.Notification;

namespace myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.MVP
{
    public class HomeMenuPresenter : HomeMenuContract.IHomeMenuPresenter
    {
        HomeMenuContract.IHomeMenuView mView;
        HomeMenuContract.IHomeMenuService serviceApi;
        private Constants.GREETING greeting;
        private List<SummaryDashBoardDetails> summaryDashboardInfoList;
        private List<SummaryDashBoardDetails> updateDashboardInfoList;
        private static bool FirstTimeMyServiceInitiate = true;
        private static bool FirstTimeNewFAQInitiate = true;
        private static List<MyService> currentMyServiceList = new List<MyService>();
        private static List<NewFAQ> currentNewFAQList = new List<NewFAQ>();
        private static NewFAQParentEntity NewFAQParentManager;
        private static NewFAQEntity NewFAQManager;
        private static SSMRMeterReadingScreensParentEntity SSMRMeterReadingScreensParentManager;
        private static SSMRMeterReadingScreensEntity SSMRMeterReadingScreensManager;
        private static SSMRMeterReadingThreePhaseScreensParentEntity SSMRMeterReadingThreePhaseScreensParentManager;
        private static SSMRMeterReadingThreePhaseScreensEntity SSMRMeterReadingThreePhaseScreensManager;
        private static SSMRMeterReadingScreensOCROffParentEntity SSMRMeterReadingScreensOCROffParentManager;
        private static SSMRMeterReadingScreensOCROffEntity SSMRMeterReadingScreensOCROffManager;
        private static SSMRMeterReadingThreePhaseScreensOCROffParentEntity SSMRMeterReadingThreePhaseScreensOCROffParentManager;
        private static SSMRMeterReadingThreePhaseScreensOCROffEntity SSMRMeterReadingThreePhaseScreensOCROffManager;
        private static EnergySavingTipsParentEntity EnergySavingTipsParentManager;
        private static EnergySavingTipsEntity EnergySavingTipsManager;
        // private static List<string> loadedSummaryList;
        private static bool isSMRApplyAllowFlag = true;
        int billingAccoutCount = 0;
        int curentLoadMoreCount = 0;
        static int trackCurrentLoadMoreCount = 0;
        bool isQuery = false;

        private bool isMyServiceExpanded = false;

        private CancellationTokenSource tokenSource = new CancellationTokenSource();

        private CancellationTokenSource FAQTokenSource = new CancellationTokenSource();

        private CancellationTokenSource walkthroughTokenSource = new CancellationTokenSource();

        private CancellationTokenSource walkthroughNoOCRTokenSource = new CancellationTokenSource();

        private CancellationTokenSource threePhaseWalkthroughTokenSource = new CancellationTokenSource();

        private CancellationTokenSource threePhaseWalkthroughNoOCRTokenSource = new CancellationTokenSource();

        private CancellationTokenSource energyTipsTokenSource = new CancellationTokenSource();

        private CancellationTokenSource queryTokenSource = new CancellationTokenSource();


        public HomeMenuPresenter(HomeMenuContract.IHomeMenuView view)
        {
            this.mView = view;
            this.serviceApi = new HomeMenuServiceImpl();
        }

        public string GetAccountDisplay()
        {
            return UserEntity.GetActive().DisplayName;
        }

        public Constants.GREETING GetGreeting()
        {
            DateTime dt = DateTime.Now.ToLocalTime();
            int hour_only = dt.Hour;

            if (hour_only >= 6 && hour_only < 12)
            {
                greeting = Constants.GREETING.MORNING;
            }
            else if (hour_only >= 12 && hour_only < 18)
            {
                greeting = Constants.GREETING.AFTERNOON;
            }
            else if (hour_only >= 0 && hour_only < 6)
            {
                greeting = Constants.GREETING.EVENING;
            }
            else
            {
                greeting = Constants.GREETING.EVENING;
            }

            return greeting;
        }

        private async Task GetAccountSummaryInfo(SummaryDashBordRequest request)
        {
            try
            {
                SummaryDashBoardResponse response = await this.serviceApi.GetLinkedSummaryInfo(request);
                if (response != null)
                {
                    if (response.Data != null && response.Data.ErrorCode != "7200")
                    {
                        this.mView.ShowRefreshScreen(response.Data.RefreshMessage, response.Data.RefreshBtnText);
                    }
                    else if (response.Data != null && response.Data.ErrorCode == "7200" && response.Data.data != null && response.Data.data.Count > 0)
                    {
                        
                        List<SummaryDashBoardDetails> summaryDetails = response.Data.data;
                        List<SummaryDashBoardAccountEntity> billingDetails = new List<SummaryDashBoardAccountEntity>();
                        for (int i = 0; i < summaryDetails.Count; i++)
                        {
                            AccountSMRStatus selectedUpdateAccount = new AccountSMRStatus();
                            CustomerBillingAccount cbAccount = CustomerBillingAccount.FindByAccNum(summaryDetails[i].AccNumber);
                            summaryDetails[i].AccName = cbAccount.AccDesc;
                            summaryDetails[i].AccType = cbAccount.AccountCategoryId;
                            summaryDetails[i].IsAccSelected = cbAccount.IsSelected;
                            summaryDetails[i].SmartMeterCode = cbAccount.SmartMeterCode;
                            summaryDetails[i].IsTaggedSMR = cbAccount.IsTaggedSMR;
                            /*** Save account data For the Day***/
                            SummaryDashBoardAccountEntity accountModel = new SummaryDashBoardAccountEntity();
                            accountModel.Timestamp = DateTime.Now.ToLocalTime();
                            accountModel.JsonResponse = JsonConvert.SerializeObject(summaryDetails[i]);
                            accountModel.AccountNo = summaryDetails[i].AccNumber;
                            billingDetails.Add(accountModel);
                            SummaryDashBoardAccountEntity.InsertItem(accountModel);

                            int findIndex = updateDashboardInfoList.FindIndex(x => x.AccNumber == summaryDetails[i].AccNumber);
                            if (findIndex != -1)
                            {
                                updateDashboardInfoList[findIndex] = summaryDetails[i];
                            }
                            // loadedSummaryList.Add(summaryDetails[i].AccNumber);
                            /*****/
                        }
                        MyTNBAccountManagement.GetInstance().UpdateCustomerBillingDetails(billingDetails);
                        this.mView.UpdateAccountListCards(updateDashboardInfoList);

                        if (billingAccoutCount > 3)
                        {
                            if (billingAccoutCount == updateDashboardInfoList.Count())
                            {
                                this.mView.IsLoadMoreButtonVisible(true, true);
                            }
                            else if (billingAccoutCount > updateDashboardInfoList.Count())
                            {
                                this.mView.IsLoadMoreButtonVisible(true, false);
                            }
                        }
                        else
                        {
                            this.mView.IsLoadMoreButtonVisible(false, false);
                        }
                    }
                    else
                    {
                        this.mView.ShowRefreshScreen(null, null);
                    }
                }
                else
                {
                    this.mView.ShowRefreshScreen(null, null);
                }
            }
            catch (System.OperationCanceledException cancelledException)
            {
                this.mView.ShowRefreshScreen(null, null);
                Utility.LoggingNonFatalError(cancelledException);
            }
            catch (ApiException apiException)
            {
                this.mView.ShowRefreshScreen(null, null);
                Utility.LoggingNonFatalError(apiException);
            }
            catch (Exception unknownException)
            {
                this.mView.ShowRefreshScreen(null, null);
                Utility.LoggingNonFatalError(unknownException);
            }
        }

        private async Task GetAccountSummaryInfoQuery(SummaryDashBordRequest request)
        {
            try
            {
                SummaryDashBoardResponse response = await this.serviceApi.GetLinkedSummaryInfoQuery(request, queryTokenSource.Token);
                if (response != null)
                {
                    if (response.Data != null && response.Data.ErrorCode != "7200")
                    {
                        this.mView.ShowRefreshScreen(response.Data.RefreshMessage, response.Data.RefreshBtnText);
                    }
                    else if (response.Data != null && response.Data.ErrorCode == "7200" && response.Data.data != null && response.Data.data.Count > 0)
                    {

                        List<SummaryDashBoardDetails> summaryDetails = response.Data.data;
                        List<SummaryDashBoardAccountEntity> billingDetails = new List<SummaryDashBoardAccountEntity>();
                        for (int i = 0; i < summaryDetails.Count; i++)
                        {
                            AccountSMRStatus selectedUpdateAccount = new AccountSMRStatus();
                            CustomerBillingAccount cbAccount = CustomerBillingAccount.FindByAccNum(summaryDetails[i].AccNumber);
                            summaryDetails[i].AccName = cbAccount.AccDesc;
                            summaryDetails[i].AccType = cbAccount.AccountCategoryId;
                            summaryDetails[i].IsAccSelected = cbAccount.IsSelected;
                            summaryDetails[i].SmartMeterCode = cbAccount.SmartMeterCode;
                            summaryDetails[i].IsTaggedSMR = cbAccount.IsTaggedSMR;
                            /*** Save account data For the Day***/
                            SummaryDashBoardAccountEntity accountModel = new SummaryDashBoardAccountEntity();
                            accountModel.Timestamp = DateTime.Now.ToLocalTime();
                            accountModel.JsonResponse = JsonConvert.SerializeObject(summaryDetails[i]);
                            accountModel.AccountNo = summaryDetails[i].AccNumber;
                            billingDetails.Add(accountModel);
                            SummaryDashBoardAccountEntity.InsertItem(accountModel);

                            int findIndex = updateDashboardInfoList.FindIndex(x => x.AccNumber == summaryDetails[i].AccNumber);
                            if (findIndex != -1)
                            {
                                updateDashboardInfoList[findIndex] = summaryDetails[i];
                            }
                            // loadedSummaryList.Add(summaryDetails[i].AccNumber);
                            /*****/
                        }
                        MyTNBAccountManagement.GetInstance().UpdateCustomerBillingDetails(billingDetails);
                        this.mView.UpdateAccountListCards(updateDashboardInfoList);

                        if (billingAccoutCount > 3)
                        {
                            if (billingAccoutCount == updateDashboardInfoList.Count())
                            {
                                this.mView.IsLoadMoreButtonVisible(true, true);
                            }
                            else if (billingAccoutCount > updateDashboardInfoList.Count())
                            {
                                this.mView.IsLoadMoreButtonVisible(true, false);
                            }
                        }
                        else
                        {
                            this.mView.IsLoadMoreButtonVisible(false, false);
                        }

                    }
                    else
                    {
                        this.mView.ShowRefreshScreen(null, null);
                    }
                }
                else
                {
                    this.mView.ShowRefreshScreen(null, null);
                }
            }
            catch (System.OperationCanceledException cancelledException)
            {
                Utility.LoggingNonFatalError(cancelledException);
            }
            catch (ApiException apiException)
            {
                this.mView.ShowRefreshScreen(null, null);
                Utility.LoggingNonFatalError(apiException);
            }
            catch (Exception unknownException)
            {
                this.mView.ShowRefreshScreen(null, null);
                Utility.LoggingNonFatalError(unknownException);
            }
        }

        private async Task GetAccountSummaryInfoBackgroundQuery(SummaryDashBordRequest request)
        {
            try
            {
                SummaryDashBoardResponse response = await this.serviceApi.GetLinkedSummaryInfoQuery(request, queryTokenSource.Token);
                if (response != null && response.Data != null && response.Data.ErrorCode == "7200" && response.Data.data != null && response.Data.data.Count > 0)
                {

                    List<SummaryDashBoardDetails> summaryDetails = response.Data.data;
                    List<SummaryDashBoardAccountEntity> billingDetails = new List<SummaryDashBoardAccountEntity>();
                    for (int i = 0; i < summaryDetails.Count; i++)
                    {
                        AccountSMRStatus selectedUpdateAccount = new AccountSMRStatus();
                        CustomerBillingAccount cbAccount = CustomerBillingAccount.FindByAccNum(summaryDetails[i].AccNumber);
                        summaryDetails[i].AccName = cbAccount.AccDesc;
                        summaryDetails[i].AccType = cbAccount.AccountCategoryId;
                        summaryDetails[i].IsAccSelected = cbAccount.IsSelected;
                        summaryDetails[i].SmartMeterCode = cbAccount.SmartMeterCode;
                        summaryDetails[i].IsTaggedSMR = cbAccount.IsTaggedSMR;
                        /*** Save account data For the Day***/
                        SummaryDashBoardAccountEntity accountModel = new SummaryDashBoardAccountEntity();
                        accountModel.Timestamp = DateTime.Now.ToLocalTime();
                        accountModel.JsonResponse = JsonConvert.SerializeObject(summaryDetails[i]);
                        accountModel.AccountNo = summaryDetails[i].AccNumber;
                        billingDetails.Add(accountModel);
                        SummaryDashBoardAccountEntity.InsertItem(accountModel);

                        int findIndex = updateDashboardInfoList.FindIndex(x => x.AccNumber == summaryDetails[i].AccNumber);
                        if (findIndex != -1)
                        {
                            updateDashboardInfoList[findIndex] = summaryDetails[i];
                        }
                        // loadedSummaryList.Add(summaryDetails[i].AccNumber);
                        /*****/
                    }
                    MyTNBAccountManagement.GetInstance().UpdateCustomerBillingDetails(billingDetails);
                    this.mView.UpdateAccountListCards(updateDashboardInfoList);

                }
            }
            catch (System.OperationCanceledException cancelledException)
            {
                Utility.LoggingNonFatalError(cancelledException);
            }
            catch (ApiException apiException)
            {
                Utility.LoggingNonFatalError(apiException);
            }
            catch (Exception unknownException)
            {
                Utility.LoggingNonFatalError(unknownException);
            }
        }

        private async Task GetAccountSummaryInfoBackground(SummaryDashBordRequest request)
        {
            try
            {
                SummaryDashBoardResponse response = await this.serviceApi.GetLinkedSummaryInfo(request);
                if (response != null && response.Data != null && response.Data.ErrorCode == "7200" && response.Data.data != null && response.Data.data.Count > 0)
                {

                    List<SummaryDashBoardDetails> summaryDetails = response.Data.data;
                    List<SummaryDashBoardAccountEntity> billingDetails = new List<SummaryDashBoardAccountEntity>();
                    for (int i = 0; i < summaryDetails.Count; i++)
                    {
                        AccountSMRStatus selectedUpdateAccount = new AccountSMRStatus();
                        CustomerBillingAccount cbAccount = CustomerBillingAccount.FindByAccNum(summaryDetails[i].AccNumber);
                        summaryDetails[i].AccName = cbAccount.AccDesc;
                        summaryDetails[i].AccType = cbAccount.AccountCategoryId;
                        summaryDetails[i].IsAccSelected = cbAccount.IsSelected;
                        summaryDetails[i].SmartMeterCode = cbAccount.SmartMeterCode;
                        summaryDetails[i].IsTaggedSMR = cbAccount.IsTaggedSMR;
                        /*** Save account data For the Day***/
                        SummaryDashBoardAccountEntity accountModel = new SummaryDashBoardAccountEntity();
                        accountModel.Timestamp = DateTime.Now.ToLocalTime();
                        accountModel.JsonResponse = JsonConvert.SerializeObject(summaryDetails[i]);
                        accountModel.AccountNo = summaryDetails[i].AccNumber;
                        billingDetails.Add(accountModel);
                        SummaryDashBoardAccountEntity.InsertItem(accountModel);

                        int findIndex = updateDashboardInfoList.FindIndex(x => x.AccNumber == summaryDetails[i].AccNumber);
                        if (findIndex != -1)
                        {
                            updateDashboardInfoList[findIndex] = summaryDetails[i];
                        }
                        // loadedSummaryList.Add(summaryDetails[i].AccNumber);
                        /*****/
                    }
                    MyTNBAccountManagement.GetInstance().UpdateCustomerBillingDetails(billingDetails);
                    this.mView.UpdateAccountListCards(updateDashboardInfoList);

                }
            }
            catch (System.OperationCanceledException cancelledException)
            {
                Utility.LoggingNonFatalError(cancelledException);
            }
            catch (ApiException apiException)
            {
                Utility.LoggingNonFatalError(apiException);
            }
            catch (Exception unknownException)
            {
                Utility.LoggingNonFatalError(unknownException);
            }
        }

        public void LoadSummaryDetailsInBatch(List<string> accountList)
        {
            try
            {
                UserInterface currentUsrInf = new UserInterface()
                {
                    eid = UserEntity.GetActive().Email,
                    sspuid = UserEntity.GetActive().UserID,
                    did = this.mView.GetDeviceId(),
                    ft = FirebaseTokenEntity.GetLatest().FBToken,
                    lang = Constants.DEFAULT_LANG.ToUpper(),
                    sec_auth_k1 = Constants.APP_CONFIG.API_KEY_ID,
                    sec_auth_k2 = "",
                    ses_param1 = "",
                    ses_param2 = ""
                };

                if (accountList.Count > 0)
                {
                    SummaryDashBordRequest request = new SummaryDashBordRequest();
                    request.AccNum = accountList;
                    request.usrInf = currentUsrInf;
                    _ = GetAccountSummaryInfo(request);
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        private void LoadSummaryDetails(List<string> accountList)
        {
            try
            {
                UserInterface currentUsrInf = new UserInterface()
                {
                    eid = UserEntity.GetActive().Email,
                    sspuid = UserEntity.GetActive().UserID,
                    did = this.mView.GetDeviceId(),
                    ft = FirebaseTokenEntity.GetLatest().FBToken,
                    lang = Constants.DEFAULT_LANG.ToUpper(),
                    sec_auth_k1 = Constants.APP_CONFIG.API_KEY_ID,
                    sec_auth_k2 = "",
                    ses_param1 = "",
                    ses_param2 = ""
                };

                if (accountList.Count > 0)
                {
                    SummaryDashBordRequest request = new SummaryDashBordRequest();
                    request.AccNum = accountList;
                    request.usrInf = currentUsrInf;
                    _ = GetAccountSummaryInfo(request);
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        private void LoadSummaryDetailsQuery(List<string> accountList)
        {
            try
            {
                UserInterface currentUsrInf = new UserInterface()
                {
                    eid = UserEntity.GetActive().Email,
                    sspuid = UserEntity.GetActive().UserID,
                    did = this.mView.GetDeviceId(),
                    ft = FirebaseTokenEntity.GetLatest().FBToken,
                    lang = Constants.DEFAULT_LANG.ToUpper(),
                    sec_auth_k1 = Constants.APP_CONFIG.API_KEY_ID,
                    sec_auth_k2 = "",
                    ses_param1 = "",
                    ses_param2 = ""
                };

                if (accountList.Count > 0)
                {
                    SummaryDashBordRequest request = new SummaryDashBordRequest();
                    request.AccNum = accountList;
                    request.usrInf = currentUsrInf;
                    _ = GetAccountSummaryInfoQuery(request);
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        private void LoadSummaryDetailsQueryBackground(List<string> accountList)
        {
            try
            {
                UserInterface currentUsrInf = new UserInterface()
                {
                    eid = UserEntity.GetActive().Email,
                    sspuid = UserEntity.GetActive().UserID,
                    did = this.mView.GetDeviceId(),
                    ft = FirebaseTokenEntity.GetLatest().FBToken,
                    lang = Constants.DEFAULT_LANG.ToUpper(),
                    sec_auth_k1 = Constants.APP_CONFIG.API_KEY_ID,
                    sec_auth_k2 = "",
                    ses_param1 = "",
                    ses_param2 = ""
                };

                if (accountList.Count > 0)
                {
                    SummaryDashBordRequest request = new SummaryDashBordRequest();
                    request.AccNum = accountList;
                    request.usrInf = currentUsrInf;
                    _ = GetAccountSummaryInfoBackgroundQuery(request);
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        private void LoadSummaryDetailsBackground(List<string> accountList)
        {
            try
            {
                UserInterface currentUsrInf = new UserInterface()
                {
                    eid = UserEntity.GetActive().Email,
                    sspuid = UserEntity.GetActive().UserID,
                    did = this.mView.GetDeviceId(),
                    ft = FirebaseTokenEntity.GetLatest().FBToken,
                    lang = Constants.DEFAULT_LANG.ToUpper(),
                    sec_auth_k1 = Constants.APP_CONFIG.API_KEY_ID,
                    sec_auth_k2 = "",
                    ses_param1 = "",
                    ses_param2 = ""
                };

                if (accountList.Count > 0)
                {
                    SummaryDashBordRequest request = new SummaryDashBordRequest();
                    request.AccNum = accountList;
                    request.usrInf = currentUsrInf;
                    _ = GetAccountSummaryInfoBackground(request);
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void LoadLocalAccounts()
        {
            summaryDashboardInfoList = new List<SummaryDashBoardDetails>();
            updateDashboardInfoList = new List<SummaryDashBoardDetails>();
            List<CustomerBillingAccount> customerBillingAccountList = CustomerBillingAccount.GetSortedCustomerBillingAccounts();
            SummaryDashBoardDetails summaryDashBoardDetails;
            foreach (CustomerBillingAccount customerBillingAccount in customerBillingAccountList)
            {

                if (customerBillingAccount.billingDetails != null)
                {
                    summaryDashBoardDetails = JsonConvert.DeserializeObject<SummaryDashBoardDetails>(customerBillingAccount.billingDetails);
                    summaryDashBoardDetails.IsTaggedSMR = customerBillingAccount.IsTaggedSMR;
                    summaryDashboardInfoList.Add(summaryDashBoardDetails);
                    if (updateDashboardInfoList.Count < 3)
                    {
                        updateDashboardInfoList.Add(summaryDashBoardDetails);
                    }
                }
                else
                {
                    summaryDashBoardDetails = new SummaryDashBoardDetails();
                    summaryDashBoardDetails.AccName = customerBillingAccount.AccDesc;
                    summaryDashBoardDetails.AccNumber = customerBillingAccount.AccNum;
                    summaryDashBoardDetails.AccType = customerBillingAccount.AccountCategoryId;
                    summaryDashBoardDetails.IsTaggedSMR = customerBillingAccount.IsTaggedSMR;
                    summaryDashboardInfoList.Add(summaryDashBoardDetails);
                }
            }

            billingAccoutCount = summaryDashboardInfoList.Count;

            this.mView.SetHeaderActionVisiblity(summaryDashboardInfoList);

            if (billingAccoutCount > 0)
            {
                if (updateDashboardInfoList.Count == 0 || updateDashboardInfoList.Count < 3)
                {
                    updateDashboardInfoList = new List<SummaryDashBoardDetails>();
                    MyTNBAccountManagement.GetInstance().RemoveCustomerBillingDetails();
                    FetchAccountSummary(true, true);
                }
                else
                {

                    this.mView.SetAccountListCardsFromLocal(updateDashboardInfoList);

                    FetchAccountSummary();
                }
            }

            List<string> smrAccountList = new List<string>();
            for (int i = 0; i < customerBillingAccountList.Count; i++)
            {
                if (!string.IsNullOrEmpty(customerBillingAccountList[i].AccNum) && customerBillingAccountList[i].isOwned && customerBillingAccountList[i].AccountCategoryId != "2" && customerBillingAccountList[i].SmartMeterCode == "0")
                {
                    smrAccountList.Add(customerBillingAccountList[i].AccNum);
                }
            }

            if (smrAccountList.Count > 0)
            {
                _ = OnStartCheckSMRAccountStatus(smrAccountList);
            }
        }

        public void LoadAccounts()
        {
            updateDashboardInfoList = new List<SummaryDashBoardDetails>();
            List<CustomerBillingAccount> customerBillingAccountList = CustomerBillingAccount.GetSortedCustomerBillingAccounts();
            summaryDashboardInfoList = new List<SummaryDashBoardDetails>();
            // loadedSummaryList = new List<string>();
            foreach (CustomerBillingAccount customerBillintAccount in customerBillingAccountList)
            {
                SummaryDashBoardDetails summaryDashBoardDetails = new SummaryDashBoardDetails();
                summaryDashBoardDetails.AccName = customerBillintAccount.AccDesc;
                summaryDashBoardDetails.AccNumber = customerBillintAccount.AccNum;
                summaryDashBoardDetails.AccType = customerBillintAccount.AccountCategoryId;
                summaryDashBoardDetails.SmartMeterCode = customerBillintAccount.SmartMeterCode;
                summaryDashBoardDetails.IsTaggedSMR = customerBillintAccount.IsTaggedSMR;
                summaryDashboardInfoList.Add(summaryDashBoardDetails);
            }

            List<string> accountList = new List<string>();
            List<string> smrAccountList = new List<string>();
            for (int i = 0; i < customerBillingAccountList.Count; i++)
            {
                if (!string.IsNullOrEmpty(customerBillingAccountList[i].AccNum))
                {
                    accountList.Add(customerBillingAccountList[i].AccNum);
                }
                if (!string.IsNullOrEmpty(customerBillingAccountList[i].AccNum) && customerBillingAccountList[i].isOwned && customerBillingAccountList[i].AccountCategoryId != "2" && customerBillingAccountList[i].SmartMeterCode == "0")
                {
                    smrAccountList.Add(customerBillingAccountList[i].AccNum);
                }
            }

            billingAccoutCount = summaryDashboardInfoList.Count;

            this.mView.SetHeaderActionVisiblity(summaryDashboardInfoList);

            if (isQuery)
            {
                isQuery = false;
                trackCurrentLoadMoreCount = 0;
            }

            if (billingAccoutCount > 0)
            {
                if (trackCurrentLoadMoreCount > 0 && !MyTNBAccountManagement.GetInstance().IsNeedUpdatedBillingDetails())
                {
                    RestoreCachedAccountList();
                }
                else
                {
                    curentLoadMoreCount = 0;
                    FetchAccountSummary(true, true);
                }
            }

            if (smrAccountList.Count > 0)
            {
                _ = OnStartCheckSMRAccountStatus(smrAccountList);
            }
        }

        private void RestoreCachedAccountList()
        {
            try
            {
                isQuery = false;
                updateDashboardInfoList = new List<SummaryDashBoardDetails>();
                this.mView.SetAccountListCardsFromLocal(updateDashboardInfoList);
                curentLoadMoreCount = trackCurrentLoadMoreCount;

                int forLoopCount = 0;

                int i = 0;

                if (billingAccoutCount > 3)
                {
                    forLoopCount = (curentLoadMoreCount == 1) ? 3 : (curentLoadMoreCount * Constants.SUMMARY_DASHBOARD_PAGE_COUNT) - 2;
                    if (billingAccoutCount < forLoopCount)
                    {
                        forLoopCount = billingAccoutCount;
                    }
                }
                else
                {
                    forLoopCount = billingAccoutCount;
                }

                bool isLoadNeed = false;

                List<string> accounts = new List<string>();
                for (; i < forLoopCount; i++)
                {
                    if (!string.IsNullOrEmpty(summaryDashboardInfoList[i].AccNumber))
                    {
                        accounts.Add(summaryDashboardInfoList[i].AccNumber);
                        CustomerBillingAccount selected = CustomerBillingAccount.FindByAccNum(summaryDashboardInfoList[i].AccNumber);
                        if (selected.billingDetails != null)
                        {
                            SummaryDashBoardDetails cached = JsonConvert.DeserializeObject<SummaryDashBoardDetails>(selected.billingDetails);
                            cached.IsTaggedSMR = selected.IsTaggedSMR;
                            summaryDashboardInfoList[i] = cached;
                        }
                        else
                        {
                            isLoadNeed = true;
                        }
                        updateDashboardInfoList.Add(summaryDashboardInfoList[i]);
                    }
                }

                if (isLoadNeed)
                {
                    this.mView.SetAccountListCards(updateDashboardInfoList);
                    this.mView.UpdateAccountListCards(updateDashboardInfoList);
                    LoadSummaryDetails(accounts);
                }
                else
                {
                    this.mView.SetAccountListCardsFromLocal(updateDashboardInfoList);
                    LoadSummaryDetailsBackground(accounts);

                    if (billingAccoutCount > 3)
                    {
                        if (billingAccoutCount == updateDashboardInfoList.Count())
                        {
                            this.mView.IsLoadMoreButtonVisible(true, true);
                        }
                        else if (billingAccoutCount > updateDashboardInfoList.Count())
                        {
                            this.mView.IsLoadMoreButtonVisible(true, false);
                        }
                    }
                    else
                    {
                        this.mView.IsLoadMoreButtonVisible(false, false);
                    }
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

        }

        public void SetQueryClose()
        {
            isQuery = false;
            trackCurrentLoadMoreCount = 0;
        }

        public void LoadQueryAccounts(string searchText)
        {
            this.mView.IsLoadMoreButtonVisible(false, false);

            queryTokenSource.Cancel();
            queryTokenSource = new CancellationTokenSource();

            if (string.IsNullOrEmpty(searchText))
            {
                searchText = "";
            }
            updateDashboardInfoList = new List<SummaryDashBoardDetails>();
            List<CustomerBillingAccount> customerBillingAccountList = CustomerBillingAccount.GetSortedCustomerBillingAccounts();
            if (!string.IsNullOrEmpty(searchText))
            {
                customerBillingAccountList = customerBillingAccountList.FindAll(cardModel => cardModel.AccNum.ToLower().Contains(searchText.ToLower()) ||
                            (cardModel.AccDesc != null &&
                            cardModel.AccDesc.ToLower().Contains(searchText.ToLower())));
            }

            summaryDashboardInfoList = new List<SummaryDashBoardDetails>();
            
            foreach (CustomerBillingAccount customerBillintAccount in customerBillingAccountList)
            {
                SummaryDashBoardDetails summaryDashBoardDetails = new SummaryDashBoardDetails();
                summaryDashBoardDetails.AccName = customerBillintAccount.AccDesc;
                summaryDashBoardDetails.AccNumber = customerBillintAccount.AccNum;
                summaryDashBoardDetails.AccType = customerBillintAccount.AccountCategoryId;
                summaryDashBoardDetails.SmartMeterCode = customerBillintAccount.SmartMeterCode;
                summaryDashBoardDetails.IsTaggedSMR = customerBillintAccount.IsTaggedSMR;
                summaryDashboardInfoList.Add(summaryDashBoardDetails);
            }

            List<string> accountList = new List<string>();
            for (int i = 0; i < customerBillingAccountList.Count; i++)
            {
                if (!string.IsNullOrEmpty(customerBillingAccountList[i].AccNum))
                {
                    accountList.Add(customerBillingAccountList[i].AccNum);
                }
            }

            billingAccoutCount = summaryDashboardInfoList.Count;

            curentLoadMoreCount = 0;

            if (string.IsNullOrEmpty(searchText))
            {
                isQuery = false;
                trackCurrentLoadMoreCount = 0;
            }
            else
            {
                isQuery = true;
            }

            try
            {
                int forLoopCount = 0;

                int previousCount = 0;

                int i = 0;

                if (updateDashboardInfoList != null && updateDashboardInfoList.Count() > 0)
                {
                    curentLoadMoreCount = ((updateDashboardInfoList.Count - 3) / Constants.SUMMARY_DASHBOARD_PAGE_COUNT);
                }

                if (billingAccoutCount > 3)
                {
                    previousCount = curentLoadMoreCount;
                    curentLoadMoreCount = curentLoadMoreCount + 1;
                    forLoopCount = (curentLoadMoreCount == 1) ? 3 : (curentLoadMoreCount * Constants.SUMMARY_DASHBOARD_PAGE_COUNT) - 2;
                    i = previousCount * Constants.SUMMARY_DASHBOARD_PAGE_COUNT;
                    if (i > 0)
                    {
                        i -= 2;
                    }
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

                bool isLoadNeed = false;

                List<string> accounts = new List<string>();
                for (; i < forLoopCount; i++)
                {
                    if (!string.IsNullOrEmpty(summaryDashboardInfoList[i].AccNumber))
                    {
                        accounts.Add(summaryDashboardInfoList[i].AccNumber);
                        CustomerBillingAccount selected = CustomerBillingAccount.FindByAccNum(summaryDashboardInfoList[i].AccNumber);
                        if (selected.billingDetails != null)
                        {
                            SummaryDashBoardDetails cached = JsonConvert.DeserializeObject<SummaryDashBoardDetails>(selected.billingDetails);
                            cached.IsTaggedSMR = selected.IsTaggedSMR;
                            summaryDashboardInfoList[i] = cached;
                        }
                        else
                        {
                            isLoadNeed = true;
                        }
                        updateDashboardInfoList.Add(summaryDashboardInfoList[i]);
                    }
                }

                if (isLoadNeed)
                {
                    this.mView.SetAccountListCards(updateDashboardInfoList);
                    this.mView.UpdateAccountListCards(updateDashboardInfoList);
                    LoadSummaryDetailsQuery(accounts);
                }
                else
                {
                    this.mView.SetAccountListCardsFromLocal(updateDashboardInfoList);
                    LoadSummaryDetailsQueryBackground(accounts);

                    if (billingAccoutCount > 3)
                    {
                        if (billingAccoutCount == updateDashboardInfoList.Count())
                        {
                            this.mView.IsLoadMoreButtonVisible(true, true);
                        }
                        else if (billingAccoutCount > updateDashboardInfoList.Count())
                        {
                            this.mView.IsLoadMoreButtonVisible(true, false);
                        }
                    }
                    else
                    {
                        this.mView.IsLoadMoreButtonVisible(false, false);
                    }
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void FetchAccountSummary(bool makeSummaryApiCall = false, bool isFirstInitiate = false, bool isReset = false)
        {
            try
            {
                if (isReset)
                {
                    curentLoadMoreCount = 0;
                    trackCurrentLoadMoreCount = 0;
                    updateDashboardInfoList = new List<SummaryDashBoardDetails>();
                    this.mView.SetAccountListCardsFromLocal(updateDashboardInfoList);
                }

                if (makeSummaryApiCall)
                {
                    FetchAccountInfo(isFirstInitiate);
                }
                else
                {
                    if (updateDashboardInfoList != null && updateDashboardInfoList.Count() > 0)
                    {
                        curentLoadMoreCount = ((updateDashboardInfoList.Count - 3) / Constants.SUMMARY_DASHBOARD_PAGE_COUNT);
                    }

                    if (billingAccoutCount > 3)
                    {
                        curentLoadMoreCount = curentLoadMoreCount + 1;
                    }

                    if (!isQuery)
                    {
                        trackCurrentLoadMoreCount = curentLoadMoreCount;
                    }

                    if (billingAccoutCount > 3)
                    {
                        if (billingAccoutCount == updateDashboardInfoList.Count())
                        {
                            this.mView.IsLoadMoreButtonVisible(true, true);
                        }
                        else if (billingAccoutCount > updateDashboardInfoList.Count())
                        {
                            this.mView.IsLoadMoreButtonVisible(true, false);
                        }
                    }
                    else
                    {
                        this.mView.IsLoadMoreButtonVisible(false, false);
                    }
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void RefreshAccountSummary()
        {
            updateDashboardInfoList = new List<SummaryDashBoardDetails>();
            List<CustomerBillingAccount> customerBillingAccountList = CustomerBillingAccount.GetSortedCustomerBillingAccounts();
            summaryDashboardInfoList = new List<SummaryDashBoardDetails>();
            // loadedSummaryList = new List<string>();
            foreach (CustomerBillingAccount customerBillintAccount in customerBillingAccountList)
            {
                SummaryDashBoardDetails summaryDashBoardDetails = new SummaryDashBoardDetails();
                summaryDashBoardDetails.AccName = customerBillintAccount.AccDesc;
                summaryDashBoardDetails.AccNumber = customerBillintAccount.AccNum;
                summaryDashBoardDetails.AccType = customerBillintAccount.AccountCategoryId;
                summaryDashBoardDetails.SmartMeterCode = customerBillintAccount.SmartMeterCode;
                summaryDashBoardDetails.IsTaggedSMR = customerBillintAccount.IsTaggedSMR;
                summaryDashboardInfoList.Add(summaryDashBoardDetails);
            }

            List<string> accountList = new List<string>();
            List<string> smrAccountList = new List<string>();
            for (int i = 0; i < customerBillingAccountList.Count; i++)
            {
                if (!string.IsNullOrEmpty(customerBillingAccountList[i].AccNum))
                {
                    accountList.Add(customerBillingAccountList[i].AccNum);
                }
                if (!string.IsNullOrEmpty(customerBillingAccountList[i].AccNum) && customerBillingAccountList[i].isOwned && customerBillingAccountList[i].AccountCategoryId != "2" && customerBillingAccountList[i].SmartMeterCode == "0")
                {
                    smrAccountList.Add(customerBillingAccountList[i].AccNum);
                }
            }

            billingAccoutCount = summaryDashboardInfoList.Count;

            this.mView.SetHeaderActionVisiblity(summaryDashboardInfoList);

            if (billingAccoutCount > 0)
            {
                FetchAccountSummary(true, true, true);
            }

            if (smrAccountList.Count > 0)
            {
                _ = OnStartCheckSMRAccountStatus(smrAccountList);
            }
        }

        public void DoLoadMoreAccount()
        {
            try
            {
                this.mView.IsLoadMoreButtonVisible(false, false);

                if (billingAccoutCount > updateDashboardInfoList.Count())
                {
                    FetchAccountSummary(true);
                }
                else if (billingAccoutCount == updateDashboardInfoList.Count())
                {
                    FetchAccountSummary(true, true, true);
                }
                else
                {
                    mView.IsLoadMoreButtonVisible(false, false);
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void FetchAccountInfo(bool isFirstInitiate)
        {
            try
            {
                int forLoopCount = 0;

                int previousCount = 0;

                int i = 0;

                if (isFirstInitiate)
                {
                    if (updateDashboardInfoList != null && updateDashboardInfoList.Count() > 0)
                    {
                        curentLoadMoreCount = ((updateDashboardInfoList.Count - 3) / Constants.SUMMARY_DASHBOARD_PAGE_COUNT);
                    }
                }

                if (billingAccoutCount > 3)
                {
                    previousCount = curentLoadMoreCount;
                    curentLoadMoreCount = curentLoadMoreCount + 1;
                    forLoopCount = (curentLoadMoreCount == 1)? 3 : (curentLoadMoreCount * Constants.SUMMARY_DASHBOARD_PAGE_COUNT) - 2;
                    i = previousCount * Constants.SUMMARY_DASHBOARD_PAGE_COUNT;
                    if (i > 0)
                    {
                        i -= 2;
                    }
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

                if (!isQuery)
                {
                    trackCurrentLoadMoreCount = curentLoadMoreCount;
                }

                bool isLoadNeed = false;

                List<string> accounts = new List<string>();
                for (; i < forLoopCount; i++)
                {
                    if (!string.IsNullOrEmpty(summaryDashboardInfoList[i].AccNumber))
                    {
                        accounts.Add(summaryDashboardInfoList[i].AccNumber);
                        CustomerBillingAccount selected = CustomerBillingAccount.FindByAccNum(summaryDashboardInfoList[i].AccNumber);
                        if (selected.billingDetails != null)
                        {
                            SummaryDashBoardDetails cached = JsonConvert.DeserializeObject<SummaryDashBoardDetails>(selected.billingDetails);
                            cached.IsTaggedSMR = selected.IsTaggedSMR;
                            summaryDashboardInfoList[i] = cached;
                        }
                        else
                        {
                            isLoadNeed = true;
                        }
                        updateDashboardInfoList.Add(summaryDashboardInfoList[i]);
                    }
                }

                if (isLoadNeed)
                {
                    this.mView.SetAccountListCards(updateDashboardInfoList);
                    this.mView.UpdateAccountListCards(updateDashboardInfoList);
                    LoadSummaryDetails(accounts);
                }
                else
                {
                    this.mView.SetAccountListCardsFromLocal(updateDashboardInfoList);
                    LoadSummaryDetailsBackground(accounts);

                    if (billingAccoutCount > 3)
                    {
                        if (billingAccoutCount == updateDashboardInfoList.Count())
                        {
                            this.mView.IsLoadMoreButtonVisible(true, true);
                        }
                        else if (billingAccoutCount > updateDashboardInfoList.Count())
                        {
                            this.mView.IsLoadMoreButtonVisible(true, false);
                        }
                    }
                    else
                    {
                        this.mView.IsLoadMoreButtonVisible(false, false);
                    }
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        private async Task OnStartCheckSMRAccountStatus(List<string> accountList)
        {
            await OnCheckSMRAccountStatus(accountList);
            /*List<SummaryDashBoardDetails> updateSummaryDashboardInfoList = new List<SummaryDashBoardDetails>();
            if (loadedSummaryList != null && loadedSummaryList.Count > 0)
            {
                List<CustomerBillingAccount> customerBillingAccountList = CustomerBillingAccount.GetSortedCustomerBillingAccounts();
                foreach (CustomerBillingAccount customerBillintAccount in customerBillingAccountList)
                {
                    string findAcc = loadedSummaryList.Find(x => x == customerBillintAccount.AccNum);

                    if (!string.IsNullOrEmpty(findAcc) && customerBillintAccount.billingDetails != null)
                    {
                        SummaryDashBoardDetails summaryDashBoardDetails = new SummaryDashBoardDetails();
                        summaryDashBoardDetails = JsonConvert.DeserializeObject<SummaryDashBoardDetails>(customerBillintAccount.billingDetails);
                        summaryDashBoardDetails.IsTaggedSMR = customerBillintAccount.IsTaggedSMR;
                        updateSummaryDashboardInfoList.Add(summaryDashBoardDetails);
                    }
                }
                if (updateSummaryDashboardInfoList.Count > 0)
                {
                    this.mView.UpdateAccountListCards(updateSummaryDashboardInfoList);
                }
            }*/
        }

        private async Task OnCheckSMRAccountStatus(List<string> accountList)
        {
            List<AccountSMRStatus> updateSMRStatus = new List<AccountSMRStatus>();
            try
            {
                UserInterface currentUsrInf = new UserInterface()
                {
                    eid = UserEntity.GetActive().Email,
                    sspuid = UserEntity.GetActive().UserID,
                    did = this.mView.GetDeviceId(),
                    ft = FirebaseTokenEntity.GetLatest().FBToken,
                    lang = Constants.DEFAULT_LANG.ToUpper(),
                    sec_auth_k1 = Constants.APP_CONFIG.API_KEY_ID,
                    sec_auth_k2 = "",
                    ses_param1 = "",
                    ses_param2 = ""
                };

                AccountSMRStatusResponse accountSMRResponse = await this.serviceApi.GetSMRAccountStatus(new AccountsSMRStatusRequest()
                {
                    ContractAccounts = accountList,
                    UserInterface = currentUsrInf
                });

                if (accountSMRResponse.Response.ErrorCode == "7200" && accountSMRResponse.Response.Data.Count > 0)
                {
                    updateSMRStatus = accountSMRResponse.Response.Data;
                    foreach (AccountSMRStatus status in updateSMRStatus)
                    {
                        CustomerBillingAccount cbAccount = CustomerBillingAccount.FindByAccNum(status.ContractAccount);
                        bool selectedUpdateIsTaggedSMR = false;
                        if (status.IsTaggedSMR == "true")
                        {
                            selectedUpdateIsTaggedSMR = true;
                        }

                        if (selectedUpdateIsTaggedSMR != cbAccount.IsTaggedSMR)
                        {
                            CustomerBillingAccount.UpdateIsSMRTagged(status.ContractAccount, selectedUpdateIsTaggedSMR);
                            cbAccount.IsTaggedSMR = selectedUpdateIsTaggedSMR;
                            for (int i = 0; i < summaryDashboardInfoList.Count; i++)
                            {
                                if (summaryDashboardInfoList[i].AccNumber == status.ContractAccount)
                                {
                                    summaryDashboardInfoList[i].IsTaggedSMR = cbAccount.IsTaggedSMR;
                                    break;
                                }
                            }
                        }
                    }
                    this.mView.UpdateCurrentSMRAccountList();
                    this.mView.UpdateEligibilitySMRAccountList();
                }
            }
            catch (System.OperationCanceledException cancelledException)
            {
                Utility.LoggingNonFatalError(cancelledException);
            }
            catch (ApiException apiException)
            {
                Utility.LoggingNonFatalError(apiException);
            }
            catch (Exception unknownException)
            {
                Utility.LoggingNonFatalError(unknownException);
            }
        }

        public void InitiateService()
        {
            tokenSource = new CancellationTokenSource();
            FAQTokenSource = new CancellationTokenSource();
            walkthroughTokenSource = new CancellationTokenSource();
            walkthroughNoOCRTokenSource = new CancellationTokenSource();
            threePhaseWalkthroughTokenSource = new CancellationTokenSource();
            threePhaseWalkthroughNoOCRTokenSource = new CancellationTokenSource();
            energyTipsTokenSource = new CancellationTokenSource();
            queryTokenSource = new CancellationTokenSource();
            isMyServiceExpanded = false;
            this.mView.SetMyServiceRecycleView();
            this.mView.SetNewFAQRecycleView();
        }

        public void InitiateMyServiceRefresh()
        {
            tokenSource = new CancellationTokenSource();
            FAQTokenSource = new CancellationTokenSource();
            walkthroughTokenSource = new CancellationTokenSource();
            walkthroughNoOCRTokenSource = new CancellationTokenSource();
            threePhaseWalkthroughTokenSource = new CancellationTokenSource();
            threePhaseWalkthroughNoOCRTokenSource = new CancellationTokenSource();
            energyTipsTokenSource = new CancellationTokenSource();
            queryTokenSource = new CancellationTokenSource();
            isMyServiceExpanded = false;
            this.mView.SetMyServiceRecycleView();
        }

        public void OnCancelToken()
        {
            tokenSource.Cancel();
            FAQTokenSource.Cancel();
            walkthroughTokenSource.Cancel();
            walkthroughNoOCRTokenSource.Cancel();
            threePhaseWalkthroughTokenSource.Cancel();
            threePhaseWalkthroughNoOCRTokenSource.Cancel();
            energyTipsTokenSource.Cancel();
        }

        public async Task InitiateGetApplySMR()
        {
            await GetIsSmrApplyAllowedService();
        }

        public async Task InitiateMyService()
        {
            await GetMyServiceService();
        }

        public async Task RetryMyService()
        {
            await GetMyServiceService();
        }

        private void ReadMyServiceFromCache()
        {
            // List<MyServiceEntity> cachedDBList = new List<MyServiceEntity>();
            List<MyService> cachedList = new List<MyService>();
            // cachedDBList = MyServiceEntity.GetAll();
            for (int i = 0; i < currentMyServiceList.Count; i++)
            {
                cachedList.Add(new MyService()
                {
                    ServiceCategoryId = currentMyServiceList[i].ServiceCategoryId,
                    serviceCategoryName = currentMyServiceList[i].serviceCategoryName
                });
            }
            this.mView.SetMyServiceResult(cachedList);
        }

        public void ReadNewFAQFromCache()
        {
            try
            {
                if (NewFAQManager == null)
                {
                    NewFAQManager = new NewFAQEntity();
                }
                currentNewFAQList.Clear();
                List<NewFAQEntity> cachedDBList = new List<NewFAQEntity>();
                cachedDBList = NewFAQManager.GetAll();
                for (int i = 0; i < cachedDBList.Count; i++)
                {
                    if (cachedDBList[i].Tags == "SM")
                    {
                        if (MyTNBAccountManagement.GetInstance().IsHasSMAccountCount() > 0)
                        {
                            currentNewFAQList.Add(new NewFAQ()
                            {
                                ID = cachedDBList[i].ID,
                                Image = cachedDBList[i].Image,
                                BGStartColor = cachedDBList[i].BGStartColor,
                                BGEndColor = cachedDBList[i].BGEndColor,
                                BGDirection = cachedDBList[i].BGDirection,
                                Title = cachedDBList[i].Title,
                                Description = cachedDBList[i].Description,
                                TopicBodyTitle = cachedDBList[i].TopicBodyTitle,
                                TopicBodyContent = cachedDBList[i].TopicBodyContent,
                                CTA = cachedDBList[i].CTA,
                                Tags = cachedDBList[i].Tags,
                                TargetItem = cachedDBList[i].TargetItem
                            });
                        }
                    }
                    else
                    {
                        currentNewFAQList.Add(new NewFAQ()
                        {
                            ID = cachedDBList[i].ID,
                            Image = cachedDBList[i].Image,
                            BGStartColor = cachedDBList[i].BGStartColor,
                            BGEndColor = cachedDBList[i].BGEndColor,
                            BGDirection = cachedDBList[i].BGDirection,
                            Title = cachedDBList[i].Title,
                            Description = cachedDBList[i].Description,
                            TopicBodyTitle = cachedDBList[i].TopicBodyTitle,
                            TopicBodyContent = cachedDBList[i].TopicBodyContent,
                            CTA = cachedDBList[i].CTA,
                            Tags = cachedDBList[i].Tags,
                            TargetItem = cachedDBList[i].TargetItem
                        });
                    }
                }
                this.mView.SetNewFAQResult(currentNewFAQList);
            }
            catch (Exception e)
            {
                if (currentNewFAQList.Count > 0)
                {
                    this.mView.SetNewFAQResult(currentNewFAQList);
                }
                Utility.LoggingNonFatalError(e);
            }
        }

        private async Task GetMyServiceService()
        {
            try
            {
                ServicePointManager.ServerCertificateValidationCallback += SSLFactoryHelper.CertificateValidationCallBack;
                UserInterface currentUsrInf = new UserInterface()
                {
                    eid = UserEntity.GetActive().Email,
                    sspuid = UserEntity.GetActive().UserID,
                    did = this.mView.GetDeviceId(),
                    ft = FirebaseTokenEntity.GetLatest().FBToken,
                    lang = Constants.DEFAULT_LANG.ToUpper(),
                    sec_auth_k1 = Constants.APP_CONFIG.API_KEY_ID,
                    sec_auth_k2 = "",
                    ses_param1 = "",
                    ses_param2 = ""
                };

                await GetIsSmrApplyAllowedService();

                GetServicesResponse getServicesResponse = await this.serviceApi.GetServices(new GetServiceRequests()
                {
                    usrInf = currentUsrInf
                });

                if (getServicesResponse.Data.ErrorCode == "7200" && getServicesResponse.Data.Data.CurrentServices.Count > 0)
                {
                    // MyServiceEntity.RemoveAll();
                    currentMyServiceList.Clear();
                    List<MyService> fetchList = new List<MyService>();
                    foreach (MyService service in getServicesResponse.Data.Data.CurrentServices)
                    {
                        fetchList.Add(service);
                        currentMyServiceList.Add(service);
                        // MyServiceEntity.InsertOrReplace(service);
                    }
                    // this.mView.SetMyServiceResult(fetchList);
                    OnProcessMyServiceCards();
                    FirstTimeMyServiceInitiate = false;
                }
                else
                {
                    // ReadMyServiceFromCache();
                    // this.mView.ShowMyServiceRetryOptions(getServicesResponse.Data.DisplayMessage);
                }

            }
            catch (System.OperationCanceledException cancelledException)
            {
                // ReadMyServiceFromCache();
                // this.mView.ShowMyServiceRetryOptions(null);
                Utility.LoggingNonFatalError(cancelledException);
            }
            catch (ApiException apiException)
            {
                // ReadMyServiceFromCache();
                // this.mView.ShowMyServiceRetryOptions(null);
                Utility.LoggingNonFatalError(apiException);
            }
            catch (Exception unknownException)
            {
                // ReadMyServiceFromCache();
                // this.mView.ShowMyServiceRetryOptions(null);
                Utility.LoggingNonFatalError(unknownException);
            }
        }

        private void OnProcessMyServiceCards()
        {
            List<MyService> fetchList = new List<MyService>();
            List<MyService> filterList = new List<MyService>();
            for (int i = 0; i < currentMyServiceList.Count; i++)
            {
                if (currentMyServiceList[i].ServiceCategoryId == "1001")
                {
                    if (isSMRApplyAllowFlag)
                    {
                        filterList.Add(currentMyServiceList[i]);
                    }
                }
                else
                {
                    filterList.Add(currentMyServiceList[i]);
                }
            }
            currentMyServiceList = filterList;
            fetchList = currentMyServiceList;
            if (fetchList.Count > 3)
            {
                fetchList = new List<MyService>();
                for (int i = 0; i < 3; i++)
                {
                    fetchList.Add(currentMyServiceList[i]);
                }
                this.mView.IsMyServiceLoadMoreButtonVisible(true, false);
                this.mView.SetMyServiceResult(fetchList);
            }
            else
            {
                this.mView.IsMyServiceLoadMoreButtonVisible(false, false);
                this.mView.SetMyServiceResult(fetchList);
            }
        }

        public void DoMySerivceLoadMoreAccount()
        {
            try
            {
                List<MyService> fetchList = new List<MyService>();

                if (!isMyServiceExpanded)
                {
                    isMyServiceExpanded = true;
                    fetchList = currentMyServiceList;
                    this.mView.IsMyServiceLoadMoreButtonVisible(true, true);
                    this.mView.SetBottomLayoutBackground(isMyServiceExpanded);
                    this.mView.SetMyServiceResult(fetchList);
                }
                else
                {
                    isMyServiceExpanded = false;
                    for (int i = 0; i < 3; i++)
                    {
                        fetchList.Add(currentMyServiceList[i]);
                    }
                    this.mView.IsMyServiceLoadMoreButtonVisible(true, false);
                    this.mView.SetBottomLayoutBackground(isMyServiceExpanded);
                    this.mView.SetMyServiceResult(fetchList);
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }


        private async Task GetIsSmrApplyAllowedService()
        {
            try
            {
                isSMRApplyAllowFlag = false;

                ServicePointManager.ServerCertificateValidationCallback += SSLFactoryHelper.CertificateValidationCallBack;
                UserInterface currentUsrInf = new UserInterface()
                {
                    eid = UserEntity.GetActive().Email,
                    sspuid = UserEntity.GetActive().UserID,
                    did = this.mView.GetDeviceId(),
                    ft = FirebaseTokenEntity.GetLatest().FBToken,
                    lang = Constants.DEFAULT_LANG.ToUpper(),
                    sec_auth_k1 = Constants.APP_CONFIG.API_KEY_ID,
                    sec_auth_k2 = "",
                    ses_param1 = "",
                    ses_param2 = ""
                };

                List<CustomerBillingAccount> eligibleSMRAccountList = CustomerBillingAccount.GetEligibleAndSMRAccountList();
                List<string> smrEligibleAccountList = new List<string>();
                eligibleSMRAccountList.ForEach(account =>
                {
                    smrEligibleAccountList.Add(account.AccNum);
                });

                GetIsSmrApplyAllowedResponse isSMRApplyResponse = await this.serviceApi.GetIsSmrApplyAllowed(new GetIsSmrApplyAllowedRequest()
                {
                    usrInf = currentUsrInf,
                    contractAccounts = smrEligibleAccountList
                });

                if (isSMRApplyResponse.Data.ErrorCode == "7200" && isSMRApplyResponse.Data.Data.Count > 0)
                {
                    for (int i = 0; i < isSMRApplyResponse.Data.Data.Count; i++)
                    {
                        if (isSMRApplyResponse.Data.Data[i].AllowApply)
                        {
                            isSMRApplyAllowFlag = true;
                            break;
                        }
                    }
                }

            }
            catch (System.OperationCanceledException cancelledException)
            {
                isSMRApplyAllowFlag = false;
                Utility.LoggingNonFatalError(cancelledException);
            }
            catch (ApiException apiException)
            {
                isSMRApplyAllowFlag = false;
                Utility.LoggingNonFatalError(apiException);
            }
            catch (Exception unknownException)
            {
                isSMRApplyAllowFlag = false;
                Utility.LoggingNonFatalError(unknownException);
            }
        }

        public List<MyService> LoadShimmerServiceList(int count)
        {
            if (count <= 0)
            {
                count = 1;
            }

            List<MyService> list = new List<MyService>();
            for (int i = 0; i < count; i++)
            {
                list.Add(new MyService()
                {
                    serviceCategoryName = ""
                });
            }

            return list;
        }

        public List<NewFAQ> LoadShimmerFAQList(int count)
        {
            if (count <= 0)
            {
                count = 1;
            }

            List<NewFAQ> list = new List<NewFAQ>();
            for (int i = 0; i < count; i++)
            {
                list.Add(new NewFAQ()
                {
                    Title = ""
                });
            }

            return list;
        }

        public void GetSavedNewFAQTimeStamp()
        {
            try
            {
                if (NewFAQParentManager == null)
                {
                    NewFAQParentManager = new NewFAQParentEntity();
                }
                List<NewFAQParentEntity> items = new List<NewFAQParentEntity>();
                items = NewFAQParentManager.GetAllItems();
                if (items != null && items.Count() > 0)
                {
                    NewFAQParentEntity entity = items[0];
                    if (entity != null && entity.Timestamp != null)
                    {
                        mView.OnSavedTimeStamp(entity?.Timestamp);
                    }
                }
                else
                {
                    mView.OnSavedTimeStamp(null);
                }
            }
            catch (Exception e)
            {
                ReadNewFAQFromCache();
                Utility.LoggingNonFatalError(e);
            }
        }

        public Task OnGetFAQTimeStamp()
        {
            FAQTokenSource = new CancellationTokenSource();
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    FAQTokenSource.Token.ThrowIfCancellationRequested();
                    string density = DPUtils.GetDeviceDensity(Application.Context);
                    GetItemsService getItemsService = new GetItemsService(SiteCoreConfig.OS, density, SiteCoreConfig.SITECORE_URL, SiteCoreConfig.DEFAULT_LANGUAGE);
                    HelpTimeStampResponseModel responseModel = getItemsService.GetHelpTimestampItem();
                    FAQTokenSource.Token.ThrowIfCancellationRequested();
                    if (responseModel.Status.Equals("Success"))
                    {
                        if (NewFAQParentManager == null)
                        {
                            NewFAQParentManager = new NewFAQParentEntity();
                        }
                        NewFAQParentManager.DeleteTable();
                        NewFAQParentManager.CreateTable();
                        NewFAQParentManager.InsertListOfItems(responseModel.Data);
                        mView.ShowFAQTimestamp(true);
                    }
                    else
                    {
                        mView.ShowFAQTimestamp(false);
                    }
                }
                catch (Exception e)
                {
                    ReadNewFAQFromCache();
                    Utility.LoggingNonFatalError(e);
                }
            }).ContinueWith((Task previous) =>
            {
            }, FAQTokenSource.Token);
        }

        public Task OnGetFAQs()
        {
            FAQTokenSource = new CancellationTokenSource();
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    FAQTokenSource.Token.ThrowIfCancellationRequested();
                    string density = DPUtils.GetDeviceDensity(Application.Context);
                    GetItemsService getItemsService = new GetItemsService(SiteCoreConfig.OS, density, SiteCoreConfig.SITECORE_URL, SiteCoreConfig.DEFAULT_LANGUAGE);
                    HelpResponseModel responseModel = getItemsService.GetHelpItems();
                    FAQTokenSource.Token.ThrowIfCancellationRequested();
                    if (responseModel.Status.Equals("Success"))
                    {
                        if (NewFAQManager == null)
                        {
                            NewFAQManager = new NewFAQEntity();
                        }
                        NewFAQManager.DeleteTable();
                        NewFAQManager.CreateTable();
                        NewFAQManager.InsertListOfItems(responseModel.Data);
                        ReadNewFAQFromCache();
                    }
                    else
                    {
                        ReadNewFAQFromCache();
                    }
                }
                catch (Exception e)
                {
                    ReadNewFAQFromCache();
                    Utility.LoggingNonFatalError(e);
                }
            }).ContinueWith((Task previous) =>
            {
            }, FAQTokenSource.Token);
        }

        public void GetSmartMeterReadingWalkthroughtTimeStamp()
        {
            try
            {
                if (SSMRMeterReadingScreensParentManager == null)
                {
                    SSMRMeterReadingScreensParentManager = new SSMRMeterReadingScreensParentEntity();
                }
                List<SSMRMeterReadingScreensParentEntity> items = new List<SSMRMeterReadingScreensParentEntity>();
                items = SSMRMeterReadingScreensParentManager.GetAllItems();
                if (items != null && items.Count > 0)
                {
                    SSMRMeterReadingScreensParentEntity entity = items[0];
                    if (entity != null && entity.Timestamp != null)
                    {
                        this.mView.OnSavedSSMRMeterReadingTimeStamp(entity?.Timestamp);
                    }
                }
                else
                {
                    this.mView.OnSavedSSMRMeterReadingTimeStamp(null);
                }
            }
            catch (Exception e)
            {
                this.mView.OnSavedSSMRMeterReadingTimeStamp(null);
                Utility.LoggingNonFatalError(e);
            }
        }

        public Task OnGetSmartMeterReadingWalkthroughtTimeStamp()
        {
            walkthroughTokenSource = new CancellationTokenSource();
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    walkthroughTokenSource.Token.ThrowIfCancellationRequested();
                    string density = DPUtils.GetDeviceDensity(Application.Context);
                    GetItemsService getItemsService = new GetItemsService(SiteCoreConfig.OS, density, SiteCoreConfig.SITECORE_URL, SiteCoreConfig.DEFAULT_LANGUAGE);
                    SSMRMeterReadingTimeStampResponseModel responseModel = getItemsService.GetSSMRMeterReadingOnePhaseWalkthroughTimestampItem();
                    walkthroughTokenSource.Token.ThrowIfCancellationRequested();
                    if (responseModel.Status.Equals("Success"))
                    {
                        if (SSMRMeterReadingScreensParentManager == null)
                        {
                            SSMRMeterReadingScreensParentManager = new SSMRMeterReadingScreensParentEntity();
                        }
                        SSMRMeterReadingScreensParentManager.DeleteTable();
                        SSMRMeterReadingScreensParentManager.CreateTable();
                        SSMRMeterReadingScreensParentManager.InsertListOfItems(responseModel.Data);
                        this.mView.CheckSSMRMeterReadingTimeStamp();
                    }
                }
                catch (Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                }
            }).ContinueWith((Task previous) =>
            {
            }, walkthroughTokenSource.Token);
        }

        public Task OnGetSSMRMeterReadingScreens()
        {
            walkthroughTokenSource = new CancellationTokenSource();
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    walkthroughTokenSource.Token.ThrowIfCancellationRequested();
                    string density = DPUtils.GetDeviceDensity(Application.Context);
                    GetItemsService getItemsService = new GetItemsService(SiteCoreConfig.OS, density, SiteCoreConfig.SITECORE_URL, SiteCoreConfig.DEFAULT_LANGUAGE);
                    SSMRMeterReadingResponseModel responseModel = getItemsService.GetSSMRMeterReadingOnePhaseWalkthroughItems();
                    walkthroughTokenSource.Token.ThrowIfCancellationRequested();
                    if (responseModel.Status.Equals("Success"))
                    {
                        if (SSMRMeterReadingScreensManager == null)
                        {
                            SSMRMeterReadingScreensManager = new SSMRMeterReadingScreensEntity();
                        }
                        SSMRMeterReadingScreensManager.DeleteTable();
                        SSMRMeterReadingScreensManager.CreateTable();
                        SSMRMeterReadingScreensManager.InsertListOfItems(responseModel.Data);
                    }
                }
                catch (Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                }
            }).ContinueWith((Task previous) =>
            {
            }, walkthroughTokenSource.Token);
        }

        public void GetSmartMeterReadingWalkthroughtNoOCRTimeStamp()
        {
            try
            {
                if (SSMRMeterReadingScreensOCROffParentManager == null)
                {
                    SSMRMeterReadingScreensOCROffParentManager = new SSMRMeterReadingScreensOCROffParentEntity();
                }
                List<SSMRMeterReadingScreensOCROffParentEntity> items = new List<SSMRMeterReadingScreensOCROffParentEntity>();
                items = SSMRMeterReadingScreensOCROffParentManager.GetAllItems();
                if (items != null && items.Count > 0)
                {
                    SSMRMeterReadingScreensOCROffParentEntity entity = items[0];
                    if (entity != null && entity.Timestamp != null)
                    {
                        this.mView.OnSavedSSMRMeterReadingNoOCRTimeStamp(entity?.Timestamp);
                    }
                }
                else
                {
                    this.mView.OnSavedSSMRMeterReadingNoOCRTimeStamp(null);
                }
            }
            catch (Exception e)
            {
                this.mView.OnSavedSSMRMeterReadingNoOCRTimeStamp(null);
                Utility.LoggingNonFatalError(e);
            }
        }

        public Task OnGetSmartMeterReadingWalkthroughtNoOCRTimeStamp()
        {
            walkthroughNoOCRTokenSource = new CancellationTokenSource();
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    walkthroughNoOCRTokenSource.Token.ThrowIfCancellationRequested();
                    string density = DPUtils.GetDeviceDensity(Application.Context);
                    GetItemsService getItemsService = new GetItemsService(SiteCoreConfig.OS, density, SiteCoreConfig.SITECORE_URL, SiteCoreConfig.DEFAULT_LANGUAGE);
                    SSMRMeterReadingTimeStampResponseModel responseModel = getItemsService.GetSSMRMeterReadingOnePhaseOCROffWalkthroughTimestampItem();
                    walkthroughNoOCRTokenSource.Token.ThrowIfCancellationRequested();
                    if (responseModel.Status.Equals("Success"))
                    {
                        if (SSMRMeterReadingScreensOCROffParentManager == null)
                        {
                            SSMRMeterReadingScreensOCROffParentManager = new SSMRMeterReadingScreensOCROffParentEntity();
                        }
                        SSMRMeterReadingScreensOCROffParentManager.DeleteTable();
                        SSMRMeterReadingScreensOCROffParentManager.CreateTable();
                        SSMRMeterReadingScreensOCROffParentManager.InsertListOfItems(responseModel.Data);
                        this.mView.CheckSSMRMeterReadingNoOCRTimeStamp();
                    }
                }
                catch (Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                }
            }).ContinueWith((Task previous) =>
            {
            }, walkthroughNoOCRTokenSource.Token);
        }

        public Task OnGetSSMRMeterReadingScreensNoOCR()
        {
            walkthroughNoOCRTokenSource = new CancellationTokenSource();
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    walkthroughNoOCRTokenSource.Token.ThrowIfCancellationRequested();
                    string density = DPUtils.GetDeviceDensity(Application.Context);
                    GetItemsService getItemsService = new GetItemsService(SiteCoreConfig.OS, density, SiteCoreConfig.SITECORE_URL, SiteCoreConfig.DEFAULT_LANGUAGE);
                    SSMRMeterReadingResponseModel responseModel = getItemsService.GetSSMRMeterReadingOnePhaseOCROffWalkthroughItems();
                    walkthroughNoOCRTokenSource.Token.ThrowIfCancellationRequested();
                    if (responseModel.Status.Equals("Success"))
                    {
                        if (SSMRMeterReadingScreensOCROffManager == null)
                        {
                            SSMRMeterReadingScreensOCROffManager = new SSMRMeterReadingScreensOCROffEntity();
                        }
                        SSMRMeterReadingScreensOCROffManager.DeleteTable();
                        SSMRMeterReadingScreensOCROffManager.CreateTable();
                        SSMRMeterReadingScreensOCROffManager.InsertListOfItems(responseModel.Data);
                    }
                }
                catch (Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                }
            }).ContinueWith((Task previous) =>
            {
            }, walkthroughNoOCRTokenSource.Token);
        }

        public void GetSmartMeterReadingThreePhaseWalkthroughtTimeStamp()
        {
            try
            {
                if (SSMRMeterReadingThreePhaseScreensParentManager == null)
                {
                    SSMRMeterReadingThreePhaseScreensParentManager = new SSMRMeterReadingThreePhaseScreensParentEntity();
                }
                List<SSMRMeterReadingThreePhaseScreensParentEntity> items = new List<SSMRMeterReadingThreePhaseScreensParentEntity>();
                items = SSMRMeterReadingThreePhaseScreensParentManager.GetAllItems();
                if (items != null && items.Count > 0)
                {
                    SSMRMeterReadingThreePhaseScreensParentEntity entity = items[0];
                    if (entity != null && entity.Timestamp != null)
                    {
                        this.mView.OnSavedSSMRMeterReadingThreePhaseTimeStamp(entity?.Timestamp);
                    }
                }
                else
                {
                    this.mView.OnSavedSSMRMeterReadingThreePhaseTimeStamp(null);
                }
            }
            catch (Exception e)
            {
                this.mView.OnSavedSSMRMeterReadingThreePhaseTimeStamp(null);
                Utility.LoggingNonFatalError(e);
            }
        }

        public Task OnGetSmartMeterReadingThreePhaseWalkthroughtTimeStamp()
        {
            threePhaseWalkthroughTokenSource = new CancellationTokenSource();
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    threePhaseWalkthroughTokenSource.Token.ThrowIfCancellationRequested();
                    string density = DPUtils.GetDeviceDensity(Application.Context);
                    GetItemsService getItemsService = new GetItemsService(SiteCoreConfig.OS, density, SiteCoreConfig.SITECORE_URL, SiteCoreConfig.DEFAULT_LANGUAGE);
                    SSMRMeterReadingTimeStampResponseModel responseModel = getItemsService.GetSSMRMeterReadingThreePhaseWalkthroughTimestampItem();
                    threePhaseWalkthroughTokenSource.Token.ThrowIfCancellationRequested();
                    if (responseModel.Status.Equals("Success"))
                    {
                        if (SSMRMeterReadingThreePhaseScreensParentManager == null)
                        {
                            SSMRMeterReadingThreePhaseScreensParentManager = new SSMRMeterReadingThreePhaseScreensParentEntity();
                        }
                        SSMRMeterReadingThreePhaseScreensParentManager.DeleteTable();
                        SSMRMeterReadingThreePhaseScreensParentManager.CreateTable();
                        SSMRMeterReadingThreePhaseScreensParentManager.InsertListOfItems(responseModel.Data);
                        this.mView.CheckSSMRMeterReadingThreePhaseTimeStamp();
                    }
                }
                catch (Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                }
            }).ContinueWith((Task previous) =>
            {
            }, threePhaseWalkthroughTokenSource.Token);
        }

        public Task OnGetSSMRMeterReadingThreePhaseScreens()
        {
            threePhaseWalkthroughTokenSource = new CancellationTokenSource();
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    threePhaseWalkthroughTokenSource.Token.ThrowIfCancellationRequested();
                    string density = DPUtils.GetDeviceDensity(Application.Context);
                    GetItemsService getItemsService = new GetItemsService(SiteCoreConfig.OS, density, SiteCoreConfig.SITECORE_URL, SiteCoreConfig.DEFAULT_LANGUAGE);
                    SSMRMeterReadingResponseModel responseModel = getItemsService.GetSSMRMeterReadingThreePhaseWalkthroughItems();
                    threePhaseWalkthroughTokenSource.Token.ThrowIfCancellationRequested();
                    if (responseModel.Status.Equals("Success"))
                    {
                        if (SSMRMeterReadingThreePhaseScreensManager == null)
                        {
                            SSMRMeterReadingThreePhaseScreensManager = new SSMRMeterReadingThreePhaseScreensEntity();
                        }
                        SSMRMeterReadingThreePhaseScreensManager.DeleteTable();
                        SSMRMeterReadingThreePhaseScreensManager.CreateTable();

                        SSMRMeterReadingThreePhaseScreensManager.InsertListOfItems(responseModel.Data);
                    }
                }
                catch (Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                }
            }).ContinueWith((Task previous) =>
            {
            }, threePhaseWalkthroughTokenSource.Token);
        }

        public void GetSmartMeterReadingThreePhaseWalkthroughtNoOCRTimeStamp()
        {
            try
            {
                if (SSMRMeterReadingThreePhaseScreensOCROffParentManager == null)
                {
                    SSMRMeterReadingThreePhaseScreensOCROffParentManager = new SSMRMeterReadingThreePhaseScreensOCROffParentEntity();
                }
                List<SSMRMeterReadingThreePhaseScreensOCROffParentEntity> items = new List<SSMRMeterReadingThreePhaseScreensOCROffParentEntity>();
                items = SSMRMeterReadingThreePhaseScreensOCROffParentManager.GetAllItems();
                if (items != null && items.Count > 0)
                {
                    SSMRMeterReadingThreePhaseScreensOCROffParentEntity entity = items[0];
                    if (entity != null && entity.Timestamp != null)
                    {
                        this.mView.OnSavedSSMRMeterReadingThreePhaseNoOCRTimeStamp(entity?.Timestamp);
                    }
                }
                else
                {
                    this.mView.OnSavedSSMRMeterReadingThreePhaseNoOCRTimeStamp(null);
                }
            }
            catch (Exception e)
            {
                this.mView.OnSavedSSMRMeterReadingThreePhaseNoOCRTimeStamp(null);
                Utility.LoggingNonFatalError(e);
            }
        }

        public Task OnGetSmartMeterReadingThreePhaseWalkthroughtNoOCRTimeStamp()
        {
            threePhaseWalkthroughNoOCRTokenSource = new CancellationTokenSource();
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    threePhaseWalkthroughNoOCRTokenSource.Token.ThrowIfCancellationRequested();
                    string density = DPUtils.GetDeviceDensity(Application.Context);
                    GetItemsService getItemsService = new GetItemsService(SiteCoreConfig.OS, density, SiteCoreConfig.SITECORE_URL, SiteCoreConfig.DEFAULT_LANGUAGE);
                    SSMRMeterReadingTimeStampResponseModel responseModel = getItemsService.GetSSMRMeterReadingThreePhaseOCROffWalkthroughTimestampItem();
                    threePhaseWalkthroughNoOCRTokenSource.Token.ThrowIfCancellationRequested();
                    if (responseModel.Status.Equals("Success"))
                    {
                        if (SSMRMeterReadingThreePhaseScreensOCROffParentManager == null)
                        {
                            SSMRMeterReadingThreePhaseScreensOCROffParentManager = new SSMRMeterReadingThreePhaseScreensOCROffParentEntity();
                        }
                        SSMRMeterReadingThreePhaseScreensOCROffParentManager.DeleteTable();
                        SSMRMeterReadingThreePhaseScreensOCROffParentManager.CreateTable();
                        SSMRMeterReadingThreePhaseScreensOCROffParentManager.InsertListOfItems(responseModel.Data);
                        this.mView.CheckSSMRMeterReadingThreePhaseNoOCRTimeStamp();
                    }
                }
                catch (Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                }
            }).ContinueWith((Task previous) =>
            {
            }, threePhaseWalkthroughNoOCRTokenSource.Token);
        }

        public Task OnGetSSMRMeterReadingThreePhaseScreensNoOCR()
        {
            threePhaseWalkthroughNoOCRTokenSource = new CancellationTokenSource();
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    threePhaseWalkthroughNoOCRTokenSource.Token.ThrowIfCancellationRequested();
                    string density = DPUtils.GetDeviceDensity(Application.Context);
                    GetItemsService getItemsService = new GetItemsService(SiteCoreConfig.OS, density, SiteCoreConfig.SITECORE_URL, SiteCoreConfig.DEFAULT_LANGUAGE);
                    SSMRMeterReadingResponseModel responseModel = getItemsService.GetSSMRMeterReadingThreePhaseOCROffWalkthroughItems();
                    threePhaseWalkthroughNoOCRTokenSource.Token.ThrowIfCancellationRequested();
                    if (responseModel.Status.Equals("Success"))
                    {
                        if (SSMRMeterReadingThreePhaseScreensOCROffManager == null)
                        {
                            SSMRMeterReadingThreePhaseScreensOCROffManager = new SSMRMeterReadingThreePhaseScreensOCROffEntity();
                        }
                        SSMRMeterReadingThreePhaseScreensOCROffManager.DeleteTable();
                        SSMRMeterReadingThreePhaseScreensOCROffManager.CreateTable();

                        SSMRMeterReadingThreePhaseScreensOCROffManager.InsertListOfItems(responseModel.Data);
                    }
                }
                catch (Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                }
            }).ContinueWith((Task previous) =>
            {
            }, threePhaseWalkthroughNoOCRTokenSource.Token);
        }

        public void GetEnergySavingTipsTimeStamp()
        {
            try
            {
                if (EnergySavingTipsParentManager == null)
                {
                    EnergySavingTipsParentManager = new EnergySavingTipsParentEntity();
                }
                List<EnergySavingTipsParentEntity> items = new List<EnergySavingTipsParentEntity>();
                items = EnergySavingTipsParentManager.GetAllItems();
                if (items != null && items.Count > 0)
                {
                    EnergySavingTipsParentEntity entity = items[0];
                    if (entity != null && entity.Timestamp != null)
                    {
                        this.mView.OnSavedEnergySavingTipsTimeStamp(entity?.Timestamp);
                    }
                }
                else
                {
                    this.mView.OnSavedEnergySavingTipsTimeStamp(null);
                }
            }
            catch (Exception e)
            {
                this.mView.OnSavedEnergySavingTipsTimeStamp(null);
                Utility.LoggingNonFatalError(e);
            }
        }

        public Task OnGetEnergySavingTipsTimeStamp()
        {
            energyTipsTokenSource = new CancellationTokenSource();
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    energyTipsTokenSource.Token.ThrowIfCancellationRequested();
                    string density = DPUtils.GetDeviceDensity(Application.Context);
                    GetItemsService getItemsService = new GetItemsService(SiteCoreConfig.OS, density, SiteCoreConfig.SITECORE_URL, SiteCoreConfig.DEFAULT_LANGUAGE);
                    EnergySavingTipsTimeStampResponseModel responseModel = getItemsService.GetEnergySavingTipsTimestampItem();
                    energyTipsTokenSource.Token.ThrowIfCancellationRequested();
                    if (responseModel.Status.Equals("Success"))
                    {
                        if (EnergySavingTipsParentManager == null)
                        {
                            EnergySavingTipsParentManager = new EnergySavingTipsParentEntity();
                        }
                        EnergySavingTipsParentManager.DeleteTable();
                        EnergySavingTipsParentManager.CreateTable();
                        EnergySavingTipsParentManager.InsertListOfItems(responseModel.Data);
                        this.mView.CheckEnergySavingTipsTimeStamp();
                    }
                }
                catch (Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                }
            }).ContinueWith((Task previous) =>
            {
            }, energyTipsTokenSource.Token);
        }

        public Task OnGetEnergySavingTips()
        {
            energyTipsTokenSource = new CancellationTokenSource();
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    energyTipsTokenSource.Token.ThrowIfCancellationRequested();
                    string density = DPUtils.GetDeviceDensity(Application.Context);
                    GetItemsService getItemsService = new GetItemsService(SiteCoreConfig.OS, density, SiteCoreConfig.SITECORE_URL, SiteCoreConfig.DEFAULT_LANGUAGE);
                    EnergySavingTipsResponseModel responseModel = getItemsService.GetEnergySavingTipsItem();
                    energyTipsTokenSource.Token.ThrowIfCancellationRequested();
                    if (responseModel.Status.Equals("Success"))
                    {
                        if (EnergySavingTipsManager == null)
                        {
                            EnergySavingTipsManager = new EnergySavingTipsEntity();
                        }
                        EnergySavingTipsManager.DeleteTable();
                        EnergySavingTipsManager.CreateTable();
                        EnergySavingTipsManager.InsertListOfItems(responseModel.Data);
                        OnSetEnergySavingTipsToCache();
                    }
                    else
                    {
                        OnSetEnergySavingTipsToCache();
                    }
                }
                catch (Exception e)
                {
                    OnSetEnergySavingTipsToCache();
                    Utility.LoggingNonFatalError(e);
                }
            }).ContinueWith((Task previous) =>
            {
            }, energyTipsTokenSource.Token);
        }

        public Task OnSetEnergySavingTipsToCache()
        {
            energyTipsTokenSource = new CancellationTokenSource();
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    energyTipsTokenSource.Token.ThrowIfCancellationRequested();
                    if (EnergySavingTipsManager == null)
                    {
                        EnergySavingTipsManager = new EnergySavingTipsEntity();
                    }
                    List<EnergySavingTipsEntity> energyList = EnergySavingTipsManager.GetAllItems();
                    if (energyList.Count > 0)
                    {
                        List<EnergySavingTipsModel> savedList = new List<EnergySavingTipsModel>();
                        foreach (EnergySavingTipsEntity item in energyList)
                        {

                            savedList.Add(new EnergySavingTipsModel()
                            {
                                Title = item.Title,
                                Description = item.Description,
                                Image = item.Image,
                                isUpdateNeeded = true,
                                ImageBitmap = null,
                                ID = item.ID
                            }); 
                        }
                        EnergyTipsUtils.OnSetEnergyTipsList(savedList);
                    }
                }
                catch (Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                }
            }).ContinueWith((Task previous) =>
            {
            }, energyTipsTokenSource.Token);
        }

        public async void LoadingBillsHistory(CustomerBillingAccount selectedAccount)
        {
            this.mView.ShowProgressDialog();
            var cts = new CancellationTokenSource();
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
                    AccountNum = selectedAccount.AccNum,
                    IsOwner = selectedAccount.isOwned,
                    Email = UserEntity.GetActive().Email
                }, cts.Token);

                var billsHistoryResponseV5 = billsHistoryResponseApi;

                this.mView.HideProgressDialog();

                AccountData select = AccountData.Copy(selectedAccount, true);

                if (billsHistoryResponseV5 != null && billsHistoryResponseV5.Data != null)
                {
                    if (!billsHistoryResponseV5.Data.IsError && !string.IsNullOrEmpty(billsHistoryResponseV5.Data.Status)
                        && billsHistoryResponseV5.Data.Status.Equals("success"))
                    {
                        if (billsHistoryResponseV5.Data.BillHistory != null && billsHistoryResponseV5.Data.BillHistory.Count() > 0)
                        {
                            this.mView.ShowBillPDF(select, billsHistoryResponseV5.Data.BillHistory[0]);
                            return;
                        }
                        else
                        {
                            this.mView.ShowBillPDF(select);
                        }
                    }
                    else
                    {
                        this.mView.ShowBillPDF(select);
                    }
                }
                else
                {
                    this.mView.ShowBillPDF(select);
                }

            }
            catch (System.OperationCanceledException e)
            {
                this.mView.HideProgressDialog();
                this.mView.ShowBillErrorSnackBar();
                Utility.LoggingNonFatalError(e);
            }
            catch (ApiException apiException)
            {
                this.mView.HideProgressDialog();
                this.mView.ShowBillErrorSnackBar();
                Utility.LoggingNonFatalError(apiException);
            }
            catch (Exception e)
            {
                this.mView.HideProgressDialog();
                this.mView.ShowBillErrorSnackBar();
                Utility.LoggingNonFatalError(e);
            }
        }

        public void GetUserNotifications()
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    tokenSource.Token.ThrowIfCancellationRequested();
                    _ = InvokeGetUserNotifications();
                }
                catch (System.Exception ne)
                {
                    Utility.LoggingNonFatalError(ne);
                }
            }).ContinueWith((Task previous) =>
            {
            }, tokenSource.Token);
        }


        private async Task InvokeGetUserNotifications()
        {
            try
            {
                NotificationApiImpl notificationAPI = new NotificationApiImpl();
                MyTNBService.Response.UserNotificationResponse response = await notificationAPI.GetUserNotifications<MyTNBService.Response.UserNotificationResponse>(new Base.Request.APIBaseRequest());
                if (response.Data != null && response.Data.ErrorCode == "7200")
                {
                    if (response.Data.ResponseData != null && response.Data.ResponseData.UserNotificationList != null &&
                        response.Data.ResponseData.UserNotificationList.Count > 0)
                    {
                        UserNotificationEntity.RemoveAll();
                        foreach (UserNotification userNotification in response.Data.ResponseData.UserNotificationList)
                        {
                            int newRecord = UserNotificationEntity.InsertOrReplace(userNotification);
                        }
                    }
                    else
                    {
                        UserNotificationEntity.RemoveAll();
                    }
                }
                this.mView.ShowNotificationCount(UserNotificationEntity.Count());
            }
            catch (System.Exception ne)
            {
                Utility.LoggingNonFatalError(ne);
            }
        }
    }
}
