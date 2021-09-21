using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
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
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.NewAppTutorial.MVP;
using Android.Content;
using myTNB_Android.Src.MyTNBService.ServiceImpl;
using myTNB_Android.Src.MyTNBService.Response;
using myTNB_Android.Src.MyTNBService.Request;
using Android.Text;
using Android.OS;
using System.Globalization;

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
        private static EnergySavingTipsParentEntity EnergySavingTipsParentManager;
        private static EnergySavingTipsEntity EnergySavingTipsManager;
        private static bool isSMRApplyAllowFlag = true;
        int billingAccoutCount = 0;
        int curentLoadMoreCount = 0;
        static int trackCurrentLoadMoreCount = 0;
        bool isQuery = false;

        private bool isMyServiceExpanded = false;

        private bool isMyServiceRefreshNeeded = false;

        private bool isAccountRefreshNeeded = false;

        private CancellationTokenSource tokenSource = new CancellationTokenSource();

        private CancellationTokenSource FAQTokenSource = new CancellationTokenSource();

        private CancellationTokenSource energyTipsTokenSource = new CancellationTokenSource();

        private CancellationTokenSource queryTokenSource = new CancellationTokenSource();

        private bool isSummaryDone = false;
        private bool isMyServiceDone = false;
        private bool isNeedHelpDone = false;
        private bool isHomeMenuTutorialShown = false;

        private ISharedPreferences mPref;

        private CancellationTokenSource normalTokenSource = new CancellationTokenSource();

        public HomeMenuPresenter(HomeMenuContract.IHomeMenuView view, ISharedPreferences pref)
        {
            this.mView = view;
            this.mPref = pref;
            this.serviceApi = new HomeMenuServiceImpl();
        }

        public string GetAccountDisplay()
        {
            return UserEntity.GetActive().DisplayName;
        }

        public void SetDynaUserTAG()
        {
            UserEntity loggedUser = UserEntity.GetActive();
            string userEmail = loggedUser.Email;
            if (!String.IsNullOrEmpty(userEmail))
            {   //dynatrace infomation for logged user
                try
                {
                    DynatraceAndroid.Dynatrace.IdentifyUser(userEmail);
                }
                catch (System.Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                }
            }
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
                normalTokenSource = new CancellationTokenSource();

                SummaryDashBoardResponse response = await this.serviceApi.GetLinkedSummaryInfo(request, normalTokenSource.Token);
                if (response != null)
                {
                    if (response.Data != null && response.Data.ErrorCode == "7200" && response.Data.data != null && response.Data.data.Count > 0)
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

                        isSummaryDone = true;
                        OnCheckToCallHomeMenuTutorial();
                        this.mView.ShowDiscoverMoreLayout();
                        OnCleanUpNotifications(summaryDetails);
                    }
                    else if (response.Data != null && response.Data.ErrorCode == "8400")
                    {
                        string contentTxt = string.Empty;

                        if (!string.IsNullOrEmpty(response.Data.DisplayMessage))
                        {
                            contentTxt = response.Data.DisplayMessage;
                        }

                        this.mView.ShowRefreshScreen(false, contentTxt, string.Empty);
                        isAccountRefreshNeeded = true;
                    }
                    else
                    {
                        string contentTxt = string.Empty;
                        string buttonTxt = string.Empty;

                        if (!string.IsNullOrEmpty(response.Data.RefreshMessage))
                        {
                            contentTxt = response.Data.RefreshMessage;
                        }

                        if (!string.IsNullOrEmpty(response.Data.RefreshBtnText))
                        {
                            buttonTxt = response.Data.RefreshBtnText;
                        }

                        this.mView.ShowRefreshScreen(true, contentTxt, buttonTxt);
                        isAccountRefreshNeeded = true;
                    }
                }
                else
                {
                    this.mView.ShowRefreshScreen(true, null, null);
                    isAccountRefreshNeeded = true;
                }
            }
            catch (System.OperationCanceledException cancelledException)
            {
                if (isAllDone() && !isHomeMenuTutorialShown || UserSessions.HasHomeTutorialShown(mPref))
                {
                    this.mView.ShowRefreshScreen(true, null, null);
                    isAccountRefreshNeeded = true;
                }
                Utility.LoggingNonFatalError(cancelledException);
            }
            catch (ApiException apiException)
            {
                this.mView.ShowRefreshScreen(true, null, null);
                isAccountRefreshNeeded = true;
                Utility.LoggingNonFatalError(apiException);
            }
            catch (Exception unknownException)
            {
                this.mView.ShowRefreshScreen(true, null, null);
                isAccountRefreshNeeded = true;
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
                    if (response.Data != null && response.Data.ErrorCode == "7200" && response.Data.data != null && response.Data.data.Count > 0)
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
                        if (true)
                        {
                            this.mView.UpdateAccountListCards(updateDashboardInfoList);
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
                        this.mView.ShowDiscoverMoreLayout();
                        OnCleanUpNotifications(summaryDetails);
                    }
                    else if (response.Data != null && response.Data.ErrorCode == "8400")
                    {
                        string contentTxt = string.Empty;

                        if (!string.IsNullOrEmpty(response.Data.DisplayMessage))
                        {
                            contentTxt = response.Data.DisplayMessage;
                        }

                        this.mView.ShowRefreshScreen(false, contentTxt, string.Empty);
                        isAccountRefreshNeeded = true;
                    }
                    else
                    {
                        string contentTxt = string.Empty;
                        string buttonTxt = string.Empty;

                        if (!string.IsNullOrEmpty(response.Data.RefreshMessage))
                        {
                            contentTxt = response.Data.RefreshMessage;
                        }

                        if (!string.IsNullOrEmpty(response.Data.RefreshBtnText))
                        {
                            buttonTxt = response.Data.RefreshBtnText;
                        }

                        this.mView.ShowRefreshScreen(true, contentTxt, buttonTxt);
                        isAccountRefreshNeeded = true;
                    }
                }
                else
                {
                    this.mView.ShowRefreshScreen(true, null, null);
                    isAccountRefreshNeeded = true;
                }
            }
            catch (System.OperationCanceledException cancelledException)
            {
                Utility.LoggingNonFatalError(cancelledException);
            }
            catch (ApiException apiException)
            {
                this.mView.ShowRefreshScreen(true, null, null);
                isAccountRefreshNeeded = true;
                Utility.LoggingNonFatalError(apiException);
            }
            catch (Exception unknownException)
            {
                this.mView.ShowRefreshScreen(true, null, null);
                isAccountRefreshNeeded = true;
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
                    lang = LanguageUtil.GetAppLanguage().ToUpper(),
                    sec_auth_k1 = Constants.APP_CONFIG.API_KEY_ID,
                    sec_auth_k2 = string.Empty,
                    ses_param1 = string.Empty,
                    ses_param2 = string.Empty
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
                    lang = LanguageUtil.GetAppLanguage().ToUpper(),
                    sec_auth_k1 = Constants.APP_CONFIG.API_KEY_ID,
                    sec_auth_k2 = string.Empty,
                    ses_param1 = string.Empty,
                    ses_param2 = string.Empty
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
                    lang = LanguageUtil.GetAppLanguage().ToUpper(),
                    sec_auth_k1 = Constants.APP_CONFIG.API_KEY_ID,
                    sec_auth_k2 = string.Empty,
                    ses_param1 = string.Empty,
                    ses_param2 = string.Empty
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
            for (int i = 0; i < customerBillingAccountList.Count; i++)
            {
                if (!string.IsNullOrEmpty(customerBillingAccountList[i].AccNum))
                {
                    accountList.Add(customerBillingAccountList[i].AccNum);
                }
            }

            billingAccoutCount = summaryDashboardInfoList.Count;

            this.mView.SetHeaderActionVisiblity(summaryDashboardInfoList);

            if (isQuery)
            {
                isQuery = false;
                trackCurrentLoadMoreCount = 0;
                HomeMenuUtils.SetTrackCurrentLoadMoreCount(trackCurrentLoadMoreCount);
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
            else
            {
                isSummaryDone = true;
                OnCheckToCallHomeMenuTutorial();
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
                HomeMenuUtils.SetTrackCurrentLoadMoreCount(trackCurrentLoadMoreCount);
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

                    isSummaryDone = true;
                    OnCheckToCallHomeMenuTutorial();
                }
                this.mView.ShowDiscoverMoreLayout();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

        }

        public void SetQueryClose()
        {
            isQuery = false;
            HomeMenuUtils.SetIsQuery(false);
            HomeMenuUtils.SetQueryWord(string.Empty);
            trackCurrentLoadMoreCount = 0;
            HomeMenuUtils.SetTrackCurrentLoadMoreCount(trackCurrentLoadMoreCount);

            LoadAccounts();
        }

        public void LoadQueryAccounts(string searchText)
        {
            this.mView.IsLoadMoreButtonVisible(false, false);

            queryTokenSource.Cancel();
            queryTokenSource = new CancellationTokenSource();

            if (string.IsNullOrEmpty(searchText))
            {
                searchText = string.Empty;
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
                HomeMenuUtils.SetTrackCurrentLoadMoreCount(trackCurrentLoadMoreCount);
            }
            else
            {
                isQuery = true;
            }

            HomeMenuUtils.SetIsQuery(isQuery);
            HomeMenuUtils.SetQueryWord(searchText);

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
                this.mView.ShowDiscoverMoreLayout();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void RestoreQueryAccounts()
        {
            this.mView.IsLoadMoreButtonVisible(false, false);

            queryTokenSource.Cancel();
            queryTokenSource = new CancellationTokenSource();

            string searchText = HomeMenuUtils.GetQueryWord();

            if (string.IsNullOrEmpty(searchText))
            {
                searchText = string.Empty;
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

            if (string.IsNullOrEmpty(searchText))
            {
                isQuery = false;
                trackCurrentLoadMoreCount = 0;
                curentLoadMoreCount = 0;
                HomeMenuUtils.SetTrackCurrentLoadMoreCount(trackCurrentLoadMoreCount);
            }
            else
            {
                isQuery = true;
                trackCurrentLoadMoreCount = HomeMenuUtils.GetTrackCurrentLoadMoreCount();
                curentLoadMoreCount = trackCurrentLoadMoreCount;
            }

            HomeMenuUtils.SetIsQuery(isQuery);
            HomeMenuUtils.SetQueryWord(searchText);


            if (trackCurrentLoadMoreCount > 0)
            {
                this.mView.SetAccountListCardsFromLocal(updateDashboardInfoList);
                curentLoadMoreCount = trackCurrentLoadMoreCount;
                HomeMenuUtils.SetTrackCurrentLoadMoreCount(trackCurrentLoadMoreCount);
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

                    isSummaryDone = true;
                    OnCheckToCallHomeMenuTutorial();
                }
                this.mView.ShowDiscoverMoreLayout();
            }
            else
            {
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

                        isSummaryDone = true;
                        OnCheckToCallHomeMenuTutorial();
                    }
                }
                catch (Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                }
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
                    HomeMenuUtils.SetTrackCurrentLoadMoreCount(trackCurrentLoadMoreCount);
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
                        HomeMenuUtils.SetTrackCurrentLoadMoreCount(trackCurrentLoadMoreCount);
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
            isSummaryDone = false;
            isMyServiceDone = false;
            isHomeMenuTutorialShown = false;
            isAccountRefreshNeeded = false;
            if (isMyServiceRefreshNeeded)
            {
                this.mView.SetBottomLayoutBackground(false);
            }
            isMyServiceRefreshNeeded = false;

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
            for (int i = 0; i < customerBillingAccountList.Count; i++)
            {
                if (!string.IsNullOrEmpty(customerBillingAccountList[i].AccNum))
                {
                    accountList.Add(customerBillingAccountList[i].AccNum);
                }
            }

            billingAccoutCount = summaryDashboardInfoList.Count;

            this.mView.SetHeaderActionVisiblity(summaryDashboardInfoList);

            if (billingAccoutCount > 0)
            {
                FetchAccountSummary(true, true, true);
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

                trackCurrentLoadMoreCount = curentLoadMoreCount;
                HomeMenuUtils.SetTrackCurrentLoadMoreCount(trackCurrentLoadMoreCount);

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

                    isSummaryDone = true;
                    OnCheckToCallHomeMenuTutorial();
                }
                this.mView.ShowDiscoverMoreLayout();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        private async Task OnStartCheckSMRAccountStatus(List<List<string>> accountList)
        {
            for (int j = 0; j < accountList.Count; j++)
            {
                await OnCheckSMRAccountStatusBatch(accountList[j]);
            }
        }

        private async Task OnCheckSMRAccountStatusBatch(List<string> accountList)
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
                    lang = LanguageUtil.GetAppLanguage().ToUpper(),
                    sec_auth_k1 = Constants.APP_CONFIG.API_KEY_ID,
                    sec_auth_k2 = string.Empty,
                    ses_param1 = string.Empty,
                    ses_param2 = string.Empty
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

        private async Task OnCheckSMRAccountStatus(List<string> accountList)
        {
            isSMRApplyAllowFlag = false;

            List<AccountSMRStatus> updateSMRStatus = new List<AccountSMRStatus>();
            try
            {
                UserInterface currentUsrInf = new UserInterface()
                {
                    eid = UserEntity.GetActive().Email,
                    sspuid = UserEntity.GetActive().UserID,
                    did = this.mView.GetDeviceId(),
                    ft = FirebaseTokenEntity.GetLatest().FBToken,
                    lang = LanguageUtil.GetAppLanguage().ToUpper(),
                    sec_auth_k1 = Constants.APP_CONFIG.API_KEY_ID,
                    sec_auth_k2 = string.Empty,
                    ses_param1 = string.Empty,
                    ses_param2 = string.Empty
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
                            isSMRApplyAllowFlag = true;
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

        public void InitiateService()
        {
            tokenSource = new CancellationTokenSource();
            FAQTokenSource = new CancellationTokenSource();
            energyTipsTokenSource = new CancellationTokenSource();
            queryTokenSource = new CancellationTokenSource();
            isMyServiceExpanded = false;
            HomeMenuUtils.SetIsMyServiceExpanded(isMyServiceExpanded);

            this.mView.SetMyServiceRecycleView();
            this.mView.SetNewFAQRecycleView();
        }

        public void InitiateMyServiceRefresh()
        {
            tokenSource = new CancellationTokenSource();
            FAQTokenSource = new CancellationTokenSource();
            energyTipsTokenSource = new CancellationTokenSource();
            queryTokenSource = new CancellationTokenSource();
            isMyServiceExpanded = false;
            HomeMenuUtils.SetIsMyServiceExpanded(isMyServiceExpanded);
            this.mView.SetMyServiceRecycleView();
        }

        public void OnCancelToken()
        {
            tokenSource.Cancel();
            FAQTokenSource.Cancel();
            energyTipsTokenSource.Cancel();
            isHomeMenuTutorialShown = false;

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
                if (NewFAQParentManager == null)
                {
                    NewFAQParentManager = new NewFAQParentEntity();
                }

                if (NewFAQManager == null)
                {
                    NewFAQManager = new NewFAQEntity();
                }

                currentNewFAQList.Clear();

                List<NewFAQParentEntity> items = NewFAQParentManager.GetAllItems();

                bool loadNeedHelpFromCache = false;

                if (items != null && items.Count > 0)
                {
                    NewFAQParentEntity entity = items[0];
                    if (entity != null && !entity.ShowNeedHelp)
                    {
                        this.mView.HideNewFAQ();
                        UpdateNewFAQCompleteState();
                    }
                    else
                    {
                        loadNeedHelpFromCache = true;
                    }
                }
                else
                {
                    loadNeedHelpFromCache = true;
                }

                if (loadNeedHelpFromCache)
                {
                    List<NewFAQEntity> cachedDBList = new List<NewFAQEntity>();
                    cachedDBList = NewFAQManager.GetAll();
                    if (cachedDBList != null && cachedDBList.Count > 0)
                    {
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

                        if (currentNewFAQList != null && currentNewFAQList.Count > 0)
                        {
                            this.mView.SetNewFAQResult(currentNewFAQList);
                        }
                        else
                        {
                            this.mView.HideNewFAQ();
                        }

                        isNeedHelpDone = true;
                        OnCheckToCallHomeMenuTutorial();
                    }
                    else
                    {
                        this.mView.HideNewFAQ();
                        UpdateNewFAQCompleteState();
                    }
                }
            }
            catch (Exception e)
            {
                this.mView.HideNewFAQ();
                UpdateNewFAQCompleteState();
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
                    lang = LanguageUtil.GetAppLanguage().ToUpper(),
                    sec_auth_k1 = Constants.APP_CONFIG.API_KEY_ID,
                    sec_auth_k2 = string.Empty,
                    ses_param1 = string.Empty,
                    ses_param2 = string.Empty
                };

                isSMRApplyAllowFlag = false;

                bool IsSMRFeatureDisabled = false;
                if (MyTNBAccountManagement.GetInstance().IsSMRFeatureDisabled())
                {
                    IsSMRFeatureDisabled = true;
                }

                if (IsSMRFeatureDisabled)
                {
                    List<CustomerBillingAccount> customerBillingAccountList = CustomerBillingAccount.CurrentSMRAccountList();
                    if (customerBillingAccountList != null && customerBillingAccountList.Count > 0)
                    {
                        for (int i = 0; i < customerBillingAccountList.Count; i++)
                        {
                            CustomerBillingAccount.UpdateIsSMRTagged(customerBillingAccountList[i].AccNum, false);
                        }

                        this.mView.UpdateCurrentSMRAccountList();
                        this.mView.UpdateEligibilitySMRAccountList();
                    }
                }
                else
                {
                    List<CustomerBillingAccount> customerBillingAccountList = CustomerBillingAccount.GetEligibleAndSMRAccountList();

                    List<string> smrAccountList = new List<string>();
                    for (int i = 0; i < customerBillingAccountList.Count; i++)
                    {
                        if (!string.IsNullOrEmpty(customerBillingAccountList[i].AccNum))
                        {
                            smrAccountList.Add(customerBillingAccountList[i].AccNum);
                        }
                    }

                    List<List<string>> splitList = new List<List<string>>();

                    if (smrAccountList.Count > 0)
                    {
                        for (int i = 0; i < smrAccountList.Count; i += 5)
                        {
                            List<string> tempList = new List<string>();
                            tempList.AddRange(smrAccountList.GetRange(i, Math.Min(5, smrAccountList.Count - i)));
                            splitList.Add(tempList);
                        }

                        for (int j = 0; j < splitList.Count; j++)
                        {
                            await OnCheckSMRAccountStatus(splitList[j]);
                            if (isSMRApplyAllowFlag)
                            {
                                List<List<string>> remainingList = new List<List<string>>();
                                for (int k = j + 1; k < splitList.Count; k++)
                                {
                                    remainingList.Add(splitList[k]);
                                }
                                if (remainingList.Count > 0)
                                {
                                    _ = OnStartCheckSMRAccountStatus(remainingList);
                                }
                                break;
                            }
                        }
                    }

                    if (!isSMRApplyAllowFlag && smrAccountList.Count > 0)
                    {
                        for (int j = 0; j < splitList.Count; j++)
                        {
                            await GetIsSmrApplyAllowedService(splitList[j]);
                            if (isSMRApplyAllowFlag)
                            {
                                break;
                            }
                        }
                    }
                }

                GetServicesResponse getServicesResponse = await this.serviceApi.GetServices(new GetServiceRequests()
                {
                    usrInf = currentUsrInf
                });

                if (getServicesResponse != null && getServicesResponse.Data != null && getServicesResponse.Data.ErrorCode == "7200")
                {
                    MyServiceEntity.RemoveAll();
                    currentMyServiceList.Clear();
                    if (getServicesResponse.Data.Data.CurrentServices.Count > 0)
                    {
                        List<MyService> fetchList = new List<MyService>();
                        foreach (MyService service in getServicesResponse.Data.Data.CurrentServices)
                        {
                            fetchList.Add(service);
                            currentMyServiceList.Add(service);
                        }
                        OnProcessMyServiceCards();
                        FirstTimeMyServiceInitiate = false;
                    }
                    else
                    {
                        SetMyServiceHideScreen();
                    }
                }
                else
                {
                    string contentTxt = string.Empty;
                    string buttonTxt = string.Empty;

                    if (getServicesResponse != null && getServicesResponse.Data != null && !string.IsNullOrEmpty(getServicesResponse.Data.RefreshMessage))
                    {
                        contentTxt = getServicesResponse.Data.RefreshMessage;
                    }

                    if (getServicesResponse != null && getServicesResponse.Data != null && !string.IsNullOrEmpty(getServicesResponse.Data.RefreshBtnText))
                    {
                        buttonTxt = getServicesResponse.Data.RefreshBtnText;
                    }

                    isMyServiceRefreshNeeded = true;
                    SetMyServiceRefreshScreen(contentTxt, buttonTxt);
                }

            }
            catch (System.OperationCanceledException cancelledException)
            {
                Utility.LoggingNonFatalError(cancelledException);
            }
            catch (ApiException apiException)
            {
                isMyServiceRefreshNeeded = true;
                SetMyServiceRefreshScreen(string.Empty, string.Empty);
                Utility.LoggingNonFatalError(apiException);
            }
            catch (Exception unknownException)
            {
                isMyServiceRefreshNeeded = true;
                SetMyServiceRefreshScreen(string.Empty, string.Empty);
                Utility.LoggingNonFatalError(unknownException);
            }
        }

        private void OnProcessMyServiceCards()
        {
            List<MyService> fetchList = new List<MyService>();
            List<MyService> filterList = new List<MyService>();
            var energyBudget = new MyService();
            for (int i = 0; i < currentMyServiceList.Count; i++)
            {
                if (currentMyServiceList[i].ServiceCategoryId == "1001")
                {
                    if (isSMRApplyAllowFlag)
                    {
                        filterList.Add(currentMyServiceList[i]);
                        MyServiceEntity.InsertOrReplace(currentMyServiceList[i]);
                    }
                }
                else if (currentMyServiceList[i].ServiceCategoryId == "1007")
                {
                    energyBudget = new MyService()
                    {
                        ServiceCategoryId = currentMyServiceList[i].ServiceCategoryId,
                        serviceCategoryName = currentMyServiceList[i].serviceCategoryName,
                        serviceCategoryIcon = currentMyServiceList[i].serviceCategoryIcon,
                        serviceCategoryIconUrl = currentMyServiceList[i].serviceCategoryIconUrl,
                        serviceCategoryDesc = currentMyServiceList[i].serviceCategoryDesc,
                    };
                }
                else
                {
                    filterList.Add(currentMyServiceList[i]);
                    MyServiceEntity.InsertOrReplace(currentMyServiceList[i]);
                }

                //this.mView.StopShimmerDiscoverMore();
            }

            //testing adding icon
            /*var testicon = new MyService()
            {
                ServiceCategoryId = "1007",
                serviceCategoryName = "My Energy Budget",
                serviceCategoryIcon = "test",
                serviceCategoryIconUrl = "test",
                serviceCategoryDesc = "test",
            };*/
            //filterList.Add(testicon);
            if (UserSessions.GetEnergyBudgetList().Count > 0 && MyTNBAccountManagement.GetInstance().IsEBUserVerify())
            {
                filterList.Insert(2, energyBudget);
            }

            MyServiceEntity.InsertOrReplace(energyBudget);

            currentMyServiceList = filterList;
            fetchList = currentMyServiceList;

            this.mView.IsMyServiceLoadMoreButtonVisible(false, false);
            this.mView.SetMyServiceResult(fetchList);

            isMyServiceDone = true;
            OnCheckToCallHomeMenuTutorial();
        }

        public void RestoreCurrentAccountState()
        {
            trackCurrentLoadMoreCount = HomeMenuUtils.GetTrackCurrentLoadMoreCount();
            LoadAccounts();
        }

        public void RestoreCurrentMyServiceState()
        {
            List<MyServiceEntity> cachedDBList = new List<MyServiceEntity>();
            var energyBudget = new MyService();
            List<MyService> cachedList = new List<MyService>();
            cachedDBList = MyServiceEntity.GetAll();
            for (int i = 0; i < cachedDBList.Count; i++)
            {
                if (cachedDBList[i].ServiceCategoryId.Contains("1007"))
                {
                    energyBudget = new MyService()
                    {
                        ServiceCategoryId = cachedDBList[i].ServiceCategoryId,
                        serviceCategoryName = cachedDBList[i].serviceCategoryName
                    };
                }
                else
                {
                    cachedList.Add(new MyService()
                    {
                        ServiceCategoryId = cachedDBList[i].ServiceCategoryId,
                        serviceCategoryName = cachedDBList[i].serviceCategoryName
                    });
                }
            }

            if (UserSessions.GetEnergyBudgetList().Count > 0 && MyTNBAccountManagement.GetInstance().IsEBUserVerify())
            {
                cachedList.Insert(2, energyBudget);
            }

            currentMyServiceList = cachedList;
            isMyServiceExpanded = true;// HomeMenuUtils.GetIsMyServiceExpanded();
            List<MyService> fetchList = new List<MyService>();
            if (isMyServiceExpanded)
            {
                fetchList = currentMyServiceList;
                this.mView.IsMyServiceLoadMoreButtonVisible(true, true);
                this.mView.SetBottomLayoutBackground(isMyServiceExpanded);
                this.mView.SetMyServiceResult(fetchList);
            }
            else
            {
                for (int i = 0; i < 3; i++)
                {
                    fetchList.Add(currentMyServiceList[i]);
                }
                if (currentMyServiceList.Count > 3)
                {
                    this.mView.IsMyServiceLoadMoreButtonVisible(true, false);
                }
                else
                {
                    this.mView.IsMyServiceLoadMoreButtonVisible(false, false);
                }
                this.mView.SetBottomLayoutBackground(isMyServiceExpanded);
                this.mView.SetMyServiceResult(fetchList);
            }
            isMyServiceDone = true;
            OnCheckToCallHomeMenuTutorial();
        }

        public void OnCheckMyServiceNewFAQState()
        {
            isMyServiceDone = false;
            isHomeMenuTutorialShown = false;
            if (UserSessions.HasHomeTutorialShown(this.mPref))
            {
                isNeedHelpDone = false;
                Handler h = new Handler();
                Action myAction = () =>
                {
                    CheckSavedNewFAQTimeStamp();
                };
                h.PostDelayed(myAction, 50);
            }
            RestoreCurrentMyServiceState();
        }

        public void OnCheckNewFAQState()
        {
            if (isNeedHelpDone)
            {
                isNeedHelpDone = false;
                GetSavedNewFAQTimeStamp();
            }
            else
            {
                isNeedHelpDone = false;
                this.mView.SetNewFAQRecycleView();
            }
        }

        public bool GetIsMyServiceRefreshNeeded()
        {
            return isMyServiceRefreshNeeded;
        }

        public bool GetIsAccountRefreshNeeded()
        {
            return isAccountRefreshNeeded;
        }

        public void SetMyServiceRefreshScreen(string contentTxt, string buttonTxt)
        {
            try
            {
                if (isAccountRefreshNeeded)
                {
                    this.mView.SetMyServiceHideView();
                }
                else
                {
                    this.mView.SetBottomLayoutBackground(true);
                    this.mView.SetMyServiceRefreshView(contentTxt, buttonTxt);
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void SetMyServiceHideScreen()
        {
            try
            {
                this.mView.SetMyServiceHideView();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void DoMySerivceLoadMoreAccount()
        {
            try
            {
                List<MyService> fetchList = new List<MyService>();
                isMyServiceExpanded = true;
                HomeMenuUtils.SetIsMyServiceExpanded(isMyServiceExpanded);
                fetchList = currentMyServiceList;
                this.mView.IsMyServiceLoadMoreButtonVisible(false, false);
                this.mView.SetBottomLayoutBackground(isMyServiceExpanded);
                this.mView.SetMyServiceResult(fetchList);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }


        private async Task GetIsSmrApplyAllowedService(List<string> accountList)
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
                    lang = LanguageUtil.GetAppLanguage().ToUpper(),
                    sec_auth_k1 = Constants.APP_CONFIG.API_KEY_ID,
                    sec_auth_k2 = string.Empty,
                    ses_param1 = string.Empty,
                    ses_param2 = string.Empty
                };

                GetIsSmrApplyAllowedResponse isSMRApplyResponse = await this.serviceApi.GetIsSmrApplyAllowed(new GetIsSmrApplyAllowedRequest()
                {
                    usrInf = currentUsrInf,
                    contractAccounts = accountList
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
                    serviceCategoryName = string.Empty
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
                    Title = string.Empty
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

        public void CheckSavedNewFAQTimeStamp()
        {
            try
            {
                FAQTokenSource = new CancellationTokenSource();
                Task.Factory.StartNew(() =>
                {
                    try
                    {
                        FAQTokenSource.Token.ThrowIfCancellationRequested();
                        string density = DPUtils.GetDeviceDensity(Application.Context);
                        GetItemsService getItemsService = new GetItemsService(SiteCoreConfig.OS, density, SiteCoreConfig.SITECORE_URL, LanguageUtil.GetAppLanguage());
                        HelpTimeStampResponseModel responseModel = getItemsService.GetHelpTimestampItem();
                        FAQTokenSource.Token.ThrowIfCancellationRequested();
                        if (responseModel != null && responseModel.Status.Equals("Success"))
                        {
                            if (responseModel.Data != null && responseModel.Data.Count > 0)
                            {
                                if (NewFAQParentManager == null)
                                {
                                    NewFAQParentManager = new NewFAQParentEntity();
                                }
                                NewFAQParentManager.DeleteTable();
                                NewFAQParentManager.CreateTable();
                                NewFAQParentManager.InsertListOfItems(responseModel.Data);

                                HelpTimeStamp checkItem = responseModel.Data[0];
                                if (checkItem != null)
                                {
                                    if (!checkItem.ShowNeedHelp)
                                    {
                                        this.mView.HideNewFAQ();
                                        UpdateNewFAQCompleteState();
                                    }
                                    else
                                    {
                                        this.mView.ShowFAQFromHide();
                                    }
                                }
                                else
                                {
                                    UpdateNewFAQCompleteState();
                                }
                            }
                            else
                            {
                                UpdateNewFAQCompleteState();
                            }

                        }
                        else
                        {
                            UpdateNewFAQCompleteState();
                        }
                    }
                    catch (Exception e)
                    {
                        UpdateNewFAQCompleteState();
                        Utility.LoggingNonFatalError(e);
                    }
                }).ContinueWith((Task previous) =>
                {
                }, FAQTokenSource.Token);
            }
            catch (Exception e)
            {
                UpdateNewFAQCompleteState();
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
                    GetItemsService getItemsService = new GetItemsService(SiteCoreConfig.OS, density, SiteCoreConfig.SITECORE_URL, LanguageUtil.GetAppLanguage());
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
                    mView.ShowFAQTimestamp(false);
                    Utility.LoggingNonFatalError(e);
                }
            }).ContinueWith((Task previous) =>
            {
            }, FAQTokenSource.Token);
        }

        public Task OnGetFAQs()
        {
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    string density = DPUtils.GetDeviceDensity(Application.Context);
                    GetItemsService getItemsService = new GetItemsService(SiteCoreConfig.OS, density, SiteCoreConfig.SITECORE_URL, LanguageUtil.GetAppLanguage());
                    HelpResponseModel responseModel = getItemsService.GetHelpItems();
                    if (responseModel != null && !string.IsNullOrEmpty(responseModel.Status))
                    {
                        if (responseModel.Status.Equals("Success"))
                        {
                            if (NewFAQManager == null)
                            {
                                NewFAQManager = new NewFAQEntity();
                            }
                            NewFAQManager.DeleteTable();
                            NewFAQManager.CreateTable();
                            NewFAQManager.InsertListOfItems(responseModel.Data);
                            if (responseModel.Data.Count > 0)
                            {
                                ReadNewFAQFromCache();
                            }
                            else
                            {
                                this.mView.HideNewFAQ();
                                isNeedHelpDone = true;
                                OnCheckToCallHomeMenuTutorial();
                            }
                        }
                        else
                        {
                            if (NewFAQParentManager == null)
                            {
                                NewFAQParentManager = new NewFAQParentEntity();
                            }
                            NewFAQParentManager.DeleteTable();
                            NewFAQParentManager.CreateTable();
                            if (NewFAQManager == null)
                            {
                                NewFAQManager = new NewFAQEntity();
                            }
                            NewFAQManager.DeleteTable();
                            NewFAQManager.CreateTable();

                            this.mView.HideNewFAQ();
                            isNeedHelpDone = true;
                            OnCheckToCallHomeMenuTutorial();
                        }
                    }
                    else
                    {
                        if (NewFAQParentManager == null)
                        {
                            NewFAQParentManager = new NewFAQParentEntity();
                        }
                        NewFAQParentManager.DeleteTable();
                        NewFAQParentManager.CreateTable();
                        this.mView.HideNewFAQ();
                    }
                }
                catch (Exception e)
                {
                    try
                    {
                        if (NewFAQParentManager == null)
                        {
                            NewFAQParentManager = new NewFAQParentEntity();
                        }
                        NewFAQParentManager.DeleteTable();
                        NewFAQParentManager.CreateTable();
                        this.mView.HideNewFAQ();
                    }
                    catch (Exception ex)
                    {
                        Utility.LoggingNonFatalError(ex);
                    }
                    Utility.LoggingNonFatalError(e);
                }
            }).ContinueWith((Task previous) =>
            {
            }, new CancellationTokenSource().Token);
        }

        public void UpdateNewFAQCompleteState()
        {
            try
            {
                isNeedHelpDone = true;
                OnCheckToCallHomeMenuTutorial();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
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
                    GetItemsService getItemsService = new GetItemsService(SiteCoreConfig.OS, density, SiteCoreConfig.SITECORE_URL, LanguageUtil.GetAppLanguage());
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
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    string density = DPUtils.GetDeviceDensity(Application.Context);
                    GetItemsService getItemsService = new GetItemsService(SiteCoreConfig.OS, density, SiteCoreConfig.SITECORE_URL, LanguageUtil.GetAppLanguage());
                    EnergySavingTipsResponseModel responseModel = getItemsService.GetEnergySavingTipsItem();
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
            }, new CancellationTokenSource().Token);
        }

        public Task OnSetEnergySavingTipsToCache()
        {
            return Task.Factory.StartNew(() =>
            {
                try
                {
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
            }, new CancellationTokenSource().Token);
        }

        public async void LoadingBillsHistory(CustomerBillingAccount selectedAccount)
        {
            this.mView.ShowProgressDialog();
            try
            {
                var billsHistoryResponse = await ServiceApiImpl.Instance.GetBillHistory(new MyTNBService.Request.GetBillHistoryRequest(selectedAccount.AccNum, selectedAccount.isOwned));

                this.mView.HideProgressDialog();

                AccountData select = AccountData.Copy(selectedAccount, true);

                if (billsHistoryResponse.IsSuccessResponse())
                {
                    if (billsHistoryResponse.GetData() != null && billsHistoryResponse.GetData().Count > 0)
                    {
                        this.mView.ShowBillPDF(select, billsHistoryResponse.GetData()[0]);
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
                    this.mView.ShowNotificationCount(UserNotificationEntity.Count());
                    _ = InvokeGetUserNotifications();
                }
                catch (System.Exception ne)
                {
                    Utility.LoggingNonFatalError(ne);
                }
            }).ContinueWith((Task previous) =>
            {
            }, new CancellationTokenSource().Token);
        }


        private async Task InvokeGetUserNotifications()
        {
            try
            {
                MyTNBAccountManagement.GetInstance().SetIsNotificationServiceCompleted(false);
                MyTNBAccountManagement.GetInstance().SetIsNotificationServiceMaintenance(false);
                MyTNBAccountManagement.GetInstance().SetIsNotificationServiceFailed(false);

                List<Notifications.Models.UserNotificationData> ToBeDeleteList = new List<Notifications.Models.UserNotificationData>();
                UserNotificationResponse response = await ServiceApiImpl.Instance.GetUserNotificationsV2(new MyTNBService.Request.BaseRequest());
                if (response.IsSuccessResponse())
                {
                    if (response.GetData() != null && response.GetData().UserNotificationList != null)
                    {
                        try
                        {
                            UserNotificationEntity.RemoveAll();
                        }
                        catch (System.Exception ne)
                        {
                            Utility.LoggingNonFatalError(ne);
                        }

                        foreach (UserNotification userNotification in response.GetData().UserNotificationList)
                        {
                            try
                            {
                                if ((userNotification.BCRMNotificationTypeId.Equals(Constants.BCRM_NOTIFICATION_BILL_DUE_ID) || userNotification.BCRMNotificationTypeId.Equals(Constants.BCRM_NOTIFICATION_DISCONNECT_NOTICE_ID)) && !userNotification.IsDeleted && !TextUtils.IsEmpty(userNotification.NotificationTypeId))
                                {
                                    CustomerBillingAccount selected = CustomerBillingAccount.FindByAccNum(userNotification.AccountNum);
                                    if (selected.billingDetails != null)
                                    {
                                        CultureInfo currCult = CultureInfo.CreateSpecificCulture("en-US");
                                        SummaryDashBoardDetails cached = JsonConvert.DeserializeObject<SummaryDashBoardDetails>(selected.billingDetails);
                                        double amtDue = 0.00;
                                        if (cached.AccType == "2")
                                        {
                                            amtDue = double.Parse(cached.AmountDue, currCult) * -1;
                                        }
                                        else
                                        {
                                            amtDue = double.Parse(cached.AmountDue, currCult);
                                        }

                                        if (amtDue <= 0.00)
                                        {
                                            userNotification.IsDeleted = true;
                                            Notifications.Models.UserNotificationData temp = new Notifications.Models.UserNotificationData();
                                            temp.Id = userNotification.Id;
                                            temp.NotificationType = userNotification.NotificationType;
                                            ToBeDeleteList.Add(temp);
                                        }
                                    }
                                }
                            }
                            catch (System.Exception ene)
                            {
                                Utility.LoggingNonFatalError(ene);
                            }

                            int newRecord = UserNotificationEntity.InsertOrReplace(userNotification);
                        }
                        MyTNBAccountManagement.GetInstance().SetIsNotificationServiceCompleted(true);
                    }
                    else
                    {
                        MyTNBAccountManagement.GetInstance().SetIsNotificationServiceFailed(true);
                    }
                }
                else if (response != null && response.Response != null && response.Response.ErrorCode == "8400")
                {
                    MyTNBAccountManagement.GetInstance().SetIsNotificationServiceMaintenance(true);
                }
                else
                {
                    MyTNBAccountManagement.GetInstance().SetIsNotificationServiceFailed(true);
                }
                this.mView.ShowNotificationCount(UserNotificationEntity.Count());

                if (ToBeDeleteList != null && ToBeDeleteList.Count > 0)
                {
                    _ = OnBatchDeleteNotifications(ToBeDeleteList);
                }
            }
            catch (System.Exception ne)
            {
                MyTNBAccountManagement.GetInstance().SetIsNotificationServiceFailed(true);
                Utility.LoggingNonFatalError(ne);
            }
        }

        private bool isAllDone()
        {
            return isSummaryDone && isMyServiceDone && isNeedHelpDone;
        }

        public bool GetIsLoadedHomeDone()
        {
            return HomeMenuUtils.GetIsLoadedHomeMenu();
        }

        public void OnCheckToCallHomeMenuTutorial()
        {
            bool EBUser = false;

            if (UserSessions.GetEnergyBudgetList().Count > 0 && MyTNBAccountManagement.GetInstance().IsEBUserVerify())
            {
                EBUser = true;
                UserSessions.DoHomeTutorialShown(this.mPref);

                if (isAllDone())
                {
                    HomeMenuUtils.SetIsLoadedHomeMenu(true);
                }
            }

            if (isAllDone() && !isHomeMenuTutorialShown && !this.mView.OnGetIsRootTooltipShown())
            {
                isHomeMenuTutorialShown = true;
                HomeMenuUtils.SetIsLoadedHomeMenu(true);

                if (!UserSessions.HasHomeTutorialShown(this.mPref))
                {
                    if (HomeMenuUtils.GetIsRestartHomeMenu())
                    {
                        this.mView.ResetNewFAQScroll();
                        this.mView.OnShowHomeMenuFragmentTutorialDialog();
                    }
                    else
                    {
                        normalTokenSource.Cancel();
                        trackCurrentLoadMoreCount = 0;
                        HomeMenuUtils.SetTrackCurrentLoadMoreCount(0);
                        isMyServiceExpanded = false;
                        HomeMenuUtils.SetIsMyServiceExpanded(false);
                        isQuery = false;
                        HomeMenuUtils.SetIsQuery(false);
                        HomeMenuUtils.SetQueryWord(string.Empty);
                        HomeMenuUtils.SetIsRestartHomeMenu(true);
                        this.mView.RestartHomeMenu();
                    }
                }
            }
        }

        public List<NewAppModel> OnGeneraNewAppTutorialList()
        {
            List<NewAppModel> newList = new List<NewAppModel>();

            bool isNeedHelpHide = false;

            if (this.mView.CheckNeedHelpHide())
            {
                isNeedHelpHide = true;
            }

            if (CustomerBillingAccount.GetSortedCustomerBillingAccounts().Count > 3)
            {
                newList.Add(new NewAppModel()
                {
                    ContentShowPosition = ContentType.BottomLeft,
                    ContentTitle = Utility.GetLocalizedLabel("DashboardHome", "tutorialAccountTitleNew"),//"Your Accounts at a glance.",
                    ContentMessage = Utility.GetLocalizedLabel("DashboardHome", "tutorialMoreAcctsDescNew"),//"View a summary of all your<br/>linked electricity accounts here.",
                    ItemCount = CustomerBillingAccount.GetSortedCustomerBillingAccounts().Count,
                    NeedHelpHide = isNeedHelpHide,
                    IsButtonShow = false,
                    IsButtonUpdateShow = true
                });

                newList.Add(new NewAppModel()
                {
                    ContentShowPosition = ContentType.BottomRight,
                    ContentTitle = Utility.GetLocalizedLabel("DashboardHome", "tutorialQuickAccessTitle"),
                    ContentMessage = Utility.GetLocalizedLabel("DashboardHome", "tutorialQuickAccessDesc"),//"Tap <strong>“Add”</strong> to link an account to<br/>myTNB. Use <strong>“Search”</strong> to look for a<br/>specific one! Just type in the<br/>nickname or account number.",
                    ItemCount = CustomerBillingAccount.GetSortedCustomerBillingAccounts().Count,
                    NeedHelpHide = isNeedHelpHide,
                    IsButtonShow = false,
                });
            }
            else if (CustomerBillingAccount.GetSortedCustomerBillingAccounts().Count <= 3 && CustomerBillingAccount.GetSortedCustomerBillingAccounts().Count > 1)
            {
                newList.Add(new NewAppModel()
                {
                    ContentShowPosition = ContentType.BottomLeft,
                    ContentTitle = Utility.GetLocalizedLabel("DashboardHome", "tutorialAccountTitleNewSec"),//"Your Accounts at a glance.",
                    ContentMessage = Utility.GetLocalizedLabel("DashboardHome", "tutorialMoreAcctsDescNew"),//"View a summary of all your linked<br/>electricity accounts here. Tap “Add”<br/>to link an account to myTNB.",
                    ItemCount = CustomerBillingAccount.GetSortedCustomerBillingAccounts().Count,
                    NeedHelpHide = isNeedHelpHide,
                    IsButtonShow = false,
                    IsButtonUpdateShow = true
                });
            }
            else if (CustomerBillingAccount.GetSortedCustomerBillingAccounts().Count == 1)
            {
                newList.Add(new NewAppModel()
                {
                    ContentShowPosition = ContentType.BottomLeft,
                    ContentTitle = Utility.GetLocalizedLabel("DashboardHome", "tutorialAccountTitleNewSec"),//"Your Accounts at a glance.",
                    ContentMessage = Utility.GetLocalizedLabel("DashboardHome", "tutorialMoreAcctsDescNew"),//"View a summary of all your linked<br/>electricity accounts here.",
                    ItemCount = CustomerBillingAccount.GetSortedCustomerBillingAccounts().Count,
                    NeedHelpHide = isNeedHelpHide,
                    IsButtonShow = false,
                    IsButtonUpdateShow = true
                });
            }
            else
            {
                newList.Add(new NewAppModel()
                {
                    ContentShowPosition = ContentType.BottomLeft,
                    ContentTitle = Utility.GetLocalizedLabel("DashboardHome", "tutorialSingleAcctTitle"),//"Your Accounts at a glance.",
                    ContentMessage = Utility.GetLocalizedLabel("DashboardHome", "tutorialNoAcctDescNew"),//"Add an electricity account to myTNB<br/>and you’ll have access to your usage<br/>and all services offered.",
                    ItemCount = CustomerBillingAccount.GetSortedCustomerBillingAccounts().Count,
                    NeedHelpHide = isNeedHelpHide,
                    IsButtonShow = false,
                    IsButtonUpdateShow = false
                });
            }

            if (isNeedHelpHide)
            {
                newList.Add(new NewAppModel()
                {
                    ContentShowPosition = ContentType.TopLeft,
                    ContentTitle = Utility.GetLocalizedLabel("DashboardHome", "tutorialQuickActionTitle"),//"Quick actions.",
                    ContentMessage = Utility.GetLocalizedLabel("DashboardHome", "tutorialQuickActionDesc"),//"Get all of the services myTNB has<br/>to offer. New features are<br/>highlighted so you don’t miss out<br/>on anything!",
                    ItemCount = CustomerBillingAccount.GetSortedCustomerBillingAccounts().Count,
                    NeedHelpHide = isNeedHelpHide,
                    IsButtonShow = false
                });
            }
            else
            {
                newList.Add(new NewAppModel()
                {
                    ContentShowPosition = ContentType.TopLeft,
                    ContentTitle = Utility.GetLocalizedLabel("DashboardHome", "tutorialQuickActionTitle"),//"Quick actions.",
                    ContentMessage = Utility.GetLocalizedLabel("DashboardHome", "tutorialQuickActionDesc"),//"Get all of the services myTNB has<br/>to offer. New features are<br/>highlighted so you don’t miss out<br/>on anything!",
                    ItemCount = CustomerBillingAccount.GetSortedCustomerBillingAccounts().Count,
                    NeedHelpHide = isNeedHelpHide,
                    IsButtonShow = false,
                });

                newList.Add(new NewAppModel()
                {
                    ContentShowPosition = ContentType.TopLeft,
                    ContentTitle = Utility.GetLocalizedLabel("DashboardHome", "tutorialNeedHelpTitle"),//"Need help?",
                    ContentMessage = Utility.GetLocalizedLabel("DashboardHome", "tutorialNeedHelpDesc"),//"We’ve highlighted some of the<br/>most commonly asked questions<br/>for you to browse through.",
                    ItemCount = CustomerBillingAccount.GetSortedCustomerBillingAccounts().Count,
                    NeedHelpHide = isNeedHelpHide,
                    IsButtonShow = false
                });
            }

            return newList;
        }

        private void OnCleanUpNotifications(List<SummaryDashBoardDetails> summaryDetails)
        {
            try
            {
                if (MyTNBAccountManagement.GetInstance().IsNotificationServiceCompleted())
                {
                    List<Notifications.Models.UserNotificationData> ToBeDeleteList = new List<Notifications.Models.UserNotificationData>();
                    for (int i = 0; i < summaryDetails.Count; i++)
                    {
                        CultureInfo currCult = CultureInfo.CreateSpecificCulture("en-US");
                        double amtDue = 0.00;
                        if (summaryDetails[i].AccType == "2")
                        {
                            amtDue = double.Parse(summaryDetails[i].AmountDue, currCult) * -1;
                        }
                        else
                        {
                            amtDue = double.Parse(summaryDetails[i].AmountDue, currCult);
                        }

                        if (amtDue <= 0.00)
                        {
                            List<UserNotificationEntity> billDueList = UserNotificationEntity.ListFilteredNotificationsByBCRMType(summaryDetails[i].AccNumber, Constants.BCRM_NOTIFICATION_BILL_DUE_ID);
                            if (billDueList != null && billDueList.Count > 0)
                            {
                                for (int j = 0; j < billDueList.Count; j++)
                                {
                                    UserNotificationEntity.UpdateIsDeleted(billDueList[j].Id, true);
                                    Notifications.Models.UserNotificationData temp = new Notifications.Models.UserNotificationData();
                                    temp.Id = billDueList[j].Id;
                                    temp.NotificationType = billDueList[j].NotificationType;
                                    ToBeDeleteList.Add(temp);
                                }
                            }

                            List<UserNotificationEntity> disconnectNoticeList = UserNotificationEntity.ListFilteredNotificationsByBCRMType(summaryDetails[i].AccNumber, Constants.BCRM_NOTIFICATION_DISCONNECT_NOTICE_ID);
                            if (disconnectNoticeList != null && disconnectNoticeList.Count > 0)
                            {
                                for (int j = 0; j < disconnectNoticeList.Count; j++)
                                {
                                    UserNotificationEntity.UpdateIsDeleted(disconnectNoticeList[j].Id, true);
                                    Notifications.Models.UserNotificationData temp = new Notifications.Models.UserNotificationData();
                                    temp.Id = disconnectNoticeList[j].Id;
                                    temp.NotificationType = disconnectNoticeList[j].NotificationType;
                                    ToBeDeleteList.Add(temp);
                                }
                            }
                        }
                    }

                    if (ToBeDeleteList != null && ToBeDeleteList.Count > 0)
                    {
                        this.mView.ShowNotificationCount(UserNotificationEntity.Count());
                        _ = OnBatchDeleteNotifications(ToBeDeleteList);
                    }
                }
            }
            catch (Exception unknownException)
            {
                Utility.LoggingNonFatalError(unknownException);
            }

        }

        private async Task OnBatchDeleteNotifications(List<Notifications.Models.UserNotificationData> accountList)
        {
            try
            {
                if (accountList != null && accountList.Count > 0)
                {
                    UserNotificationDeleteResponse notificationDeleteResponse = await ServiceApiImpl.Instance.DeleteUserNotification(new UserNotificationDeleteRequest(accountList));

                    if (notificationDeleteResponse.IsSuccessResponse())
                    {

                    }
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
    }
}