using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using myTNB.SitecoreCMS.Model;
using myTNB.SitecoreCMS.Services;
using myTNB.AndroidApp.Src.AppLaunch.Activity;
using myTNB.AndroidApp.Src.Base;
using myTNB.AndroidApp.Src.Base.Models;
using myTNB.AndroidApp.Src.Database.Model;
using myTNB.AndroidApp.Src.myTNBMenu.Activity;
using myTNB.AndroidApp.Src.myTNBMenu.Async;
using myTNB.AndroidApp.Src.myTNBMenu.Fragments;
using myTNB.AndroidApp.Src.myTNBMenu.Fragments.HomeMenu.MVP;
using myTNB.AndroidApp.Src.myTNBMenu.Fragments.RewardMenu.Api;
using myTNB.AndroidApp.Src.myTNBMenu.Fragments.RewardMenu.Model;
using myTNB.AndroidApp.Src.myTNBMenu.Fragments.RewardMenu.Request;
using myTNB.AndroidApp.Src.myTNBMenu.Fragments.RewardMenu.Response;
using myTNB.AndroidApp.Src.myTNBMenu.Models;
using myTNB.AndroidApp.Src.myTNBMenu.Requests;
using myTNB.AndroidApp.Src.NewAppTutorial.MVP;
using myTNB.AndroidApp.Src.MyTNBService.Response;
using myTNB.AndroidApp.Src.MyTNBService.ServiceImpl;
using myTNB.AndroidApp.Src.SiteCore;
using myTNB.AndroidApp.Src.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using myTNB.Mobile;
using System.Linq;
using myTNB.AndroidApp.Src.MyHome;
using myTNB.AndroidApp.Src.Base.Activity;
using Android.Graphics;
using Android.Util;
using myTNB.AndroidApp.Src.SitecoreCMS.Model;
using System.Diagnostics;
using System.IO;

namespace myTNB.AndroidApp.Src.myTNBMenu.MVP
{
    public class DashboardHomePresenter : DashboardHomeContract.IUserActionsListener
    {
        internal readonly string TAG = typeof(DashboardHomePresenter).Name;

        CancellationTokenSource cts;

        public DashboardHomeContract.IView mView;
        private BaseAppCompatActivity mActivity;
        private ISharedPreferences mSharedPref;

        internal int currentBottomNavigationMenu = Resource.Id.menu_dashboard;

        private bool smDataError = false;
        private string smErrorCode = "204";
        private string smErrorMessage = Utility.GetLocalizedErrorLabel("defaultErrorMessage");

        AccountData selectedAccount;

        private string preSelectedAccount;
		private UsageHistoryResponse usageHistoryResponse;
        private SMUsageHistoryResponse smUsageHistoryResponse;

        private bool isBillAvailable = true;

        private RewardServiceImpl mApi;

        private List<AddUpdateRewardModel> userList = new List<AddUpdateRewardModel>();

        private RewardsCategoryEntity mRewardsCategoryEntity;

        private RewardsEntity mRewardsEntity;

        private WhatsNewCategoryEntity mWhatsNewCategoryEntity;

        private WhatsNewEntity mWhatsNewEntity;

        private FloatingButtonMarketingModel content;

        private static SSMRMeterReadingScreensParentEntity SSMRMeterReadingScreensParentManager;
        private static SSMRMeterReadingScreensEntity SSMRMeterReadingScreensManager;
        private static SSMRMeterReadingThreePhaseScreensParentEntity SSMRMeterReadingThreePhaseScreensParentManager;
        private static SSMRMeterReadingThreePhaseScreensEntity SSMRMeterReadingThreePhaseScreensManager;
        private static SSMRMeterReadingScreensOCROffParentEntity SSMRMeterReadingScreensOCROffParentManager;
        private static SSMRMeterReadingScreensOCROffEntity SSMRMeterReadingScreensOCROffManager;
        private static SSMRMeterReadingThreePhaseScreensOCROffParentEntity SSMRMeterReadingThreePhaseScreensOCROffParentManager;
        private static SSMRMeterReadingThreePhaseScreensOCROffEntity SSMRMeterReadingThreePhaseScreensOCROffManager;

        private static int FloatingButtonDefaultTimeOutMillisecond = 4000;
        private int FloatingButtonTimeOutMillisecond = FloatingButtonDefaultTimeOutMillisecond;
        private bool IsOnGetPhotoRunning = false;

        private static bool isWhatNewClicked = false;

        private static bool isRewardClicked = false;

        internal int trackBottomNavigationMenu = Resource.Id.menu_dashboard;

        private static bool isWhatsNewDialogShowNeed = false;

        internal static readonly int SELECT_SM_ACCOUNT_REQUEST_CODE = 8809;
        internal static readonly int SELECT_SM_POPUP_REQUEST_CODE = 8810;


        public DashboardHomePresenter(DashboardHomeContract.IView mView, BaseAppCompatActivity activity, ISharedPreferences preferences)
        {
            this.mView = mView;
            this.mActivity = activity;
            this.mSharedPref = preferences;
            this.mView?.SetPresenter(this);

            this.mApi = new RewardServiceImpl();
        }

		public void Logout()
		{
            UserEntity.RemoveActive();
			UserRegister.RemoveActive();
			CustomerBillingAccount.RemoveActive();
            UserManageAccessAccount.RemoveActive();
			UserSessions.RemovePersistPassword(mSharedPref);
			NotificationFilterEntity.RemoveAll();
			SMUsageHistoryEntity.RemoveAll();
			UsageHistoryEntity.RemoveAll();
			BillHistoryEntity.RemoveAll();
			PaymentHistoryEntity.RemoveAll();
			REPaymentHistoryEntity.RemoveAll();
			AccountDataEntity.RemoveAll();
			SummaryDashBoardAccountEntity.RemoveAll();
			SelectBillsEntity.RemoveAll();
			mView.ShowPreLogin();
            
        }

		public void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
		{
            try
            {
                if (requestCode == Constants.SELECT_ACCOUNT_REQUEST_CODE)
                {
                    if (resultCode == Result.Ok)
                    {
                        Bundle extras = data.Extras;

                        CustomerBillingAccount selectedAccount = JsonConvert.DeserializeObject<CustomerBillingAccount>(extras.GetString(Constants.SELECTED_ACCOUNT));

                        if (currentBottomNavigationMenu == Resource.Id.menu_dashboard)
                        {
                            if (selectedAccount != null && selectedAccount.SmartMeterCode != null && selectedAccount.SmartMeterCode.Equals("0"))
                            {
                                if (!string.IsNullOrEmpty(selectedAccount.AccNum) && !UsageHistoryEntity.IsSMDataUpdated(selectedAccount.AccNum))
                                {
                                    UsageHistoryEntity storedEntity = new UsageHistoryEntity();
                                    storedEntity = UsageHistoryEntity.GetItemByAccountNo(selectedAccount.AccNum);
                                    CustomerBillingAccount.RemoveSelected();
                                    if (!string.IsNullOrEmpty(selectedAccount.AccNum))
                                    {
                                        CustomerBillingAccount.SetSelected(selectedAccount.AccNum);
                                    }
                                    if (storedEntity != null)
                                    {
                                        usageHistoryResponse = JsonConvert.DeserializeObject<UsageHistoryResponse>(storedEntity.JsonResponse);
                                        if (usageHistoryResponse != null && usageHistoryResponse.Data != null && usageHistoryResponse.Data.ErrorCode != "7200")
                                        {
                                            usageHistoryResponse = null;
                                        }
                                        else if ((usageHistoryResponse != null && usageHistoryResponse.Data == null) || (usageHistoryResponse == null))
                                        {
                                            usageHistoryResponse = null;
                                        }
                                        else if (!IsCheckHaveByMonthData(usageHistoryResponse.Data.UsageHistoryData))
                                        {
                                            usageHistoryResponse = null;
                                        }
                                        LoadUsageHistory(selectedAccount);
                                    }
                                    else
                                    {
                                        usageHistoryResponse = null;
                                        LoadUsageHistory(selectedAccount);
                                    }
                                }
                                else
                                {
                                    usageHistoryResponse = null;
                                    LoadUsageHistory(selectedAccount);
                                }
                            }
                            else
                            {
                                if (!SMUsageHistoryEntity.IsSMDataUpdated(selectedAccount.AccNum))
                                {
                                    //Get stored data
                                    SMUsageHistoryEntity storedEntity = new SMUsageHistoryEntity();
                                    if (!string.IsNullOrEmpty(selectedAccount.AccNum))
                                    {
                                        storedEntity = SMUsageHistoryEntity.GetItemByAccountNo(selectedAccount.AccNum);
                                    }
                                    SMUsageHistoryResponse storedSMData = new SMUsageHistoryResponse();

                                    CustomerBillingAccount.RemoveSelected();
                                    if (!string.IsNullOrEmpty(selectedAccount.AccNum))
                                    {
                                        CustomerBillingAccount.SetSelected(selectedAccount.AccNum);
                                    }

                                    if (storedEntity != null)
                                    {
                                        storedSMData = JsonConvert.DeserializeObject<SMUsageHistoryResponse>(storedEntity.JsonResponse);
                                        if (storedSMData != null && storedSMData.Data != null && storedSMData.Data.ErrorCode != "7200")
                                        {
                                            smUsageHistoryResponse = null;
                                        }
                                        else if ((storedSMData != null && storedSMData.Data == null) || (storedSMData == null))
                                        {
                                            smUsageHistoryResponse = null;
                                        }
                                        else if (storedSMData.Data.IsMDMSCurrentlyUnavailable || !IsCheckHaveByMonthData(storedSMData.Data.SMUsageHistoryData))
                                        {
                                            smUsageHistoryResponse = null;
                                        }
                                        else
                                        {
                                            smUsageHistoryResponse = storedSMData;
                                        }

                                        if (MyTNBAccountManagement.GetInstance().IsEBUserVerify())
                                        {
                                            smUsageHistoryResponse = null;
                                        }

                                        LoadSMUsageHistory(selectedAccount);
                                    }
                                    else
                                    {
                                        smUsageHistoryResponse = null;
                                        LoadSMUsageHistory(selectedAccount);
                                    }
                                }
                                else
                                {
                                    smUsageHistoryResponse = null;
                                    LoadSMUsageHistory(selectedAccount);
                                }
                            }
                        }
                        else if (currentBottomNavigationMenu == Resource.Id.menu_bill)
                        {
                            this.mView.SetAccountName(selectedAccount.AccDesc);
                            AccountData accountData = new AccountData();
                            CustomerBillingAccount customerBillingAccount = CustomerBillingAccount.FindByAccNum(selectedAccount.AccNum);
                            accountData.AccountNickName = selectedAccount.AccDesc;
                            accountData.AccountName = selectedAccount.OwnerName;
                            accountData.AccountNum = selectedAccount.AccNum;
                            accountData.AddStreet = selectedAccount.AccountStAddress;
                            accountData.IsOwner = customerBillingAccount.isOwned;
                            accountData.AccountCategoryId = customerBillingAccount.AccountCategoryId;
                            accountData.IsHaveAccess = customerBillingAccount.IsHaveAccess;
                            this.mView.ShowBillMenu(accountData);
                        }
                    }
                }
                else if (requestCode == Constants.MANAGE_SUPPLY_ACCOUNT_REQUEST)
                {
                    if (resultCode == Result.Ok)
                    {
                        Bundle extras = data.Extras;

                        CustomerBillingAccount selectedAccount = CustomerBillingAccount.GetFirst();


                        if (currentBottomNavigationMenu == Resource.Id.menu_dashboard)
                        {
                            if (selectedAccount != null && selectedAccount.SmartMeterCode != null && selectedAccount.SmartMeterCode.Equals("0"))
                            {
                                if (!string.IsNullOrEmpty(selectedAccount.AccNum) && !UsageHistoryEntity.IsSMDataUpdated(selectedAccount.AccNum))
                                {
                                    UsageHistoryEntity storedEntity = new UsageHistoryEntity();
                                    storedEntity = UsageHistoryEntity.GetItemByAccountNo(selectedAccount.AccNum);
                                    CustomerBillingAccount.RemoveSelected();
                                    if (!string.IsNullOrEmpty(selectedAccount.AccNum))
                                    {
                                        CustomerBillingAccount.SetSelected(selectedAccount.AccNum);
                                    }
                                    if (storedEntity != null)
                                    {
                                        usageHistoryResponse = JsonConvert.DeserializeObject<UsageHistoryResponse>(storedEntity.JsonResponse);
                                        if (usageHistoryResponse != null && usageHistoryResponse.Data != null && usageHistoryResponse.Data.ErrorCode != "7200")
                                        {
                                            usageHistoryResponse = null;
                                        }
                                        else if ((usageHistoryResponse != null && usageHistoryResponse.Data == null) || (usageHistoryResponse == null))
                                        {
                                            usageHistoryResponse = null;
                                        }
                                        else if (!IsCheckHaveByMonthData(usageHistoryResponse.Data.UsageHistoryData))
                                        {
                                            usageHistoryResponse = null;
                                        }
                                        LoadUsageHistory(selectedAccount);
                                    }
                                    else
                                    {
                                        usageHistoryResponse = null;
                                        LoadUsageHistory(selectedAccount);
                                    }
                                }
                                else
                                {
                                    usageHistoryResponse = null;
                                    LoadUsageHistory(selectedAccount);
                                }
                            }
                            else
                            {
                                if (!SMUsageHistoryEntity.IsSMDataUpdated(selectedAccount.AccNum))
                                {
                                    //Get stored data
                                    SMUsageHistoryEntity storedEntity = new SMUsageHistoryEntity();
                                    if (!string.IsNullOrEmpty(selectedAccount.AccNum))
                                    {
                                        storedEntity = SMUsageHistoryEntity.GetItemByAccountNo(selectedAccount.AccNum);
                                    }
                                    SMUsageHistoryResponse storedSMData = new SMUsageHistoryResponse();

                                    CustomerBillingAccount.RemoveSelected();
                                    if (!string.IsNullOrEmpty(selectedAccount.AccNum))
                                    {
                                        CustomerBillingAccount.SetSelected(selectedAccount.AccNum);
                                    }

                                    if (storedEntity != null)
                                    {
                                        storedSMData = JsonConvert.DeserializeObject<SMUsageHistoryResponse>(storedEntity.JsonResponse);
                                        if (storedSMData != null && storedSMData.Data != null && storedSMData.Data.ErrorCode != "7200")
                                        {
                                            smUsageHistoryResponse = null;
                                        }
                                        else if ((storedSMData != null && storedSMData.Data == null) || (storedSMData == null))
                                        {
                                            smUsageHistoryResponse = null;
                                        }
                                        else if (storedSMData.Data.IsMDMSCurrentlyUnavailable || !IsCheckHaveByMonthData(storedSMData.Data.SMUsageHistoryData))
                                        {
                                            smUsageHistoryResponse = null;
                                        }
                                        else
                                        {
                                            smUsageHistoryResponse = storedSMData;
                                        }

                                        if (MyTNBAccountManagement.GetInstance().IsEBUserVerify())
                                        {
                                            smUsageHistoryResponse = null;
                                        }

                                        LoadSMUsageHistory(selectedAccount);
                                    }
                                    else
                                    {
                                        smUsageHistoryResponse = null;
                                        LoadSMUsageHistory(selectedAccount);
                                    }
                                }
                                else
                                {
                                    smUsageHistoryResponse = null;
                                    LoadSMUsageHistory(selectedAccount);
                                }
                            }
                        }
                        else if (currentBottomNavigationMenu == Resource.Id.menu_bill)
                        {
                            this.mView.SetAccountName(selectedAccount.AccDesc);
                            AccountData accountData = new AccountData();
                            CustomerBillingAccount customerBillingAccount = CustomerBillingAccount.FindByAccNum(selectedAccount.AccNum);
                            accountData.AccountNickName = selectedAccount.AccDesc;
                            accountData.AccountName = selectedAccount.OwnerName;
                            accountData.AccountNum = selectedAccount.AccNum;
                            accountData.AddStreet = selectedAccount.AccountStAddress;
                            accountData.IsOwner = customerBillingAccount.isOwned;
                            accountData.AccountCategoryId = customerBillingAccount.AccountCategoryId;
                            accountData.IsHaveAccess = customerBillingAccount.IsHaveAccess;
                            this.mView.ShowBillMenu(accountData);
                        }
                    }
                }
                else if (requestCode == Constants.UPDATE_IC_REQUEST)
                {
                    this.mView.SetMenuMoreCheck();
                    OnLoadMoreMenu();

                    //if (resultCode == Result.Ok)
                    //{
                    //    this.mView.SetMenuMoreCheck();
                    //    //this.mView.ShowProfile();
                    //    //OnMenuSelect(Resource.Id.menu_more);
                    //    OnLoadMoreMenu();
                    //    //DoLoadHomeDashBoardFragment();
                    //}
                }
                else if (requestCode == Constants.NEW_BILL_REDESIGN_REQUEST_CODE)
                {
                    if (resultCode == Result.Ok)
                    {
                        OnMenuSelect(Resource.Id.menu_bill);
                    }
                }
                else if (requestCode == Constants.MYHOME_MICROSITE_REQUEST_CODE
                    || requestCode == Constants.APPLICATION_STATUS_LANDING_FROM_DASHBOARD_REQUEST_CODE
                    || requestCode == Constants.NOTIFICATION_LISTING_REQUEST_CODE
                    || requestCode == Constants.NOTIFICATION_DETAILS_REQUEST_CODE
                    || requestCode == Constants.APPLICATION_STATUS_DETAIL_FROM_DASHBOARD_REQUEST_CODE)
                {
                    if (resultCode == Result.Ok)
                    {
                        if (data != null && data.Extras is Bundle extras && extras != null)
                        {
                            if (extras.ContainsKey(MyHomeConstants.ACTION_BACK_TO_APPLICATION_STATUS_LANDING))
                            {
                                bool backToApplicationStatusLanding = extras.GetBoolean(MyHomeConstants.ACTION_BACK_TO_APPLICATION_STATUS_LANDING);
                                if (backToApplicationStatusLanding)
                                {
                                    string toastMsg = string.Empty;
                                    if (extras.ContainsKey(MyHomeConstants.CANCEL_TOAST_MESSAGE))
                                    {
                                        toastMsg = extras.GetString(MyHomeConstants.CANCEL_TOAST_MESSAGE);
                                    }
                                    this.mView.RouteToApplicationLanding(toastMsg);
                                }
                            }
                            else if (extras.ContainsKey(MyHomeConstants.ACTION_BACK_TO_HOME))
                            {
                                bool backToHome = extras.GetBoolean(MyHomeConstants.ACTION_BACK_TO_HOME);
                                if (backToHome)
                                {
                                    string toastMessage = string.Empty;
                                    if (extras.ContainsKey(MyHomeConstants.CANCEL_TOAST_MESSAGE))
                                    {
                                        toastMessage = extras.GetString(MyHomeConstants.CANCEL_TOAST_MESSAGE);
                                    }
                                    OnMenuSelect(Resource.Id.menu_dashboard, false, toastMessage);
                                }
                            }
                        }
                    }
                }
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void OnMenuSelect(int resourceId, bool isIneligiblePopUpActive = false, string toastMessage = "")
        {
            if (resourceId == Resource.Id.menu_dashboard)
            {
                if (toastMessage.IsValid())
                {
                    ToastUtils.OnDisplayToast(this.mActivity, toastMessage);
                    toastMessage = string.Empty;
                }
            }

            if (!this.mView.GetAlreadyStarted())
            {
                this.mView.SetAlreadyStarted(true);
            }
            else
            {
                if (trackBottomNavigationMenu == resourceId)
                {
                    return;
                }
            }

            ServicePointManager.ServerCertificateValidationCallback += SSLFactoryHelper.CertificateValidationCallBack;

            List<CustomerBillingAccount> accountList = CustomerBillingAccount.List();
            

            if (cts != null && cts.Token.CanBeCanceled)
            {
                this.cts.Cancel();
            }

            switch (resourceId)
            {
                case Resource.Id.menu_dashboard:
                    trackBottomNavigationMenu = Resource.Id.menu_dashboard;
                    OnUpdateWhatsNewUnRead();
                    if (DashboardHomeActivity.currentFragment != null && (DashboardHomeActivity.currentFragment.GetType() == typeof(HomeMenuFragment) ||
                        DashboardHomeActivity.currentFragment.GetType() == typeof(DashboardChartFragment)))
                    {
                        DoLoadHomeDashBoardFragment();
                    }
                    else
                    {
                        if (DashboardHomeActivity.GO_TO_INNER_DASHBOARD)
                        {
                            OnAccountSelectDashBoard();
                        }
                        else
                        {
                            DoLoadHomeDashBoardFragment();
                        }

                    }
                    UserSessions.UpdateFromBRCard(mSharedPref);
                    OnUpdateRewardUnRead();
                    break;
                case Resource.Id.menu_bill:
                    OnUpdateWhatsNewUnRead();
                    if (accountList.Count > 0)
                    {
                        trackBottomNavigationMenu = Resource.Id.menu_bill;
                        CustomerBillingAccount selected;
                        CustomerBillingAccount dbrAccount = GetEligibleDBRAccount();
                        CustomerBillingAccount brAccount = GetBRcountList();
                        bool BRflag = UserSessions.GetFromBRCard(mSharedPref);
                        selected = CustomerBillingAccount.GetSelected();
                        if (CustomerBillingAccount.HasSelected())
                        {
                            if (BRflag) //check BR
                            {
                                PreNavigateBllMenu(brAccount);
                                this.mView.SetAccountName(brAccount.AccDesc);
                                //UserSessions.UpdateFromBRCard(mSharedPref);
                            }
                            else
                            {
                                
                                PreNavigateBllMenu(selected);
                                this.mView.SetAccountName(selected.AccDesc);
                            }
                        }
                        else
                        {
                            if (BRflag && BillRedesignUtility.Instance.IsCAEligible(selected.AccNum))
                            {
                                CustomerBillingAccount.SetSelected(brAccount.AccDesc);
                                //selected = BRCas[0];
                                PreNavigateBllMenu(brAccount);
                                this.mView.SetAccountName(brAccount.AccDesc);
                            }
                            else
                            {
                                CustomerBillingAccount.SetSelected(accountList[0].AccNum);
                                selected = accountList[0];
                                PreNavigateBllMenu(selected);
                                this.mView.SetAccountName(accountList[0].AccDesc);
                            }
                        }
                        if (selected != null)
                        {
                            _ = CustomerBillingAccount.List();
                            bool enableDropDown = accountList.Count > 0 ? true : false;

                            if (selected.AccountCategoryId.Equals("2"))
                            {
                                this.mView.ShowREAccount(enableDropDown);
                            }
                            else
                            {
                                this.mView.EnableDropDown(enableDropDown);
                            }
                        }

                        if (BRflag) //check BR
                        {
                            PreNavigateBllMenu(brAccount);
                            this.mView.SetAccountName(brAccount.AccDesc);
                            AccountData accountDataBR = new AccountData();
                            CustomerBillingAccount customerBillingAccountBR = CustomerBillingAccount.FindByAccNum(brAccount.AccNum);
                            accountDataBR.AccountNickName = brAccount.AccDesc;
                            accountDataBR.AccountName = brAccount.OwnerName;
                            accountDataBR.AddStreet = brAccount.AccountStAddress;
                            accountDataBR.IsOwner = customerBillingAccountBR.isOwned;
                            accountDataBR.AccountNum = brAccount.AccNum;
                            accountDataBR.AccountCategoryId = customerBillingAccountBR.AccountCategoryId;
                            accountDataBR.IsHaveAccess = brAccount.IsHaveAccess;
                            this.mView.ShowBillMenu(accountDataBR, isIneligiblePopUpActive);
                        }
                        else
                        {
                            AccountData accountData = new AccountData();
                            CustomerBillingAccount customerBillingAccount = CustomerBillingAccount.FindByAccNum(selected.AccNum);
                            accountData.AccountNickName = selected.AccDesc;
                            accountData.AccountName = selected.OwnerName;
                            accountData.AddStreet = selected.AccountStAddress;
                            accountData.IsOwner = customerBillingAccount.isOwned;
                            accountData.AccountNum = selected.AccNum;
                            accountData.AccountCategoryId = customerBillingAccount.AccountCategoryId;
                            accountData.IsHaveAccess = customerBillingAccount.IsHaveAccess;
                            this.mView.ShowBillMenu(accountData, isIneligiblePopUpActive);
                        }
                        //UserSessions.UpdateFromBRCard(mSharedPref);
                    }
                    else
                    {
                        this.mView.DisableBillMenu();
                    }

                    this.mView.OnCheckProfileTab(false, true);
                    OnUpdateRewardUnRead();
                    break;
                case Resource.Id.menu_promotion:
                    currentBottomNavigationMenu = Resource.Id.menu_promotion;
                    trackBottomNavigationMenu = Resource.Id.menu_promotion;
                    UserSessions.UpdateFromBRCard(mSharedPref);
                    this.mView.ShowWhatsNewMenu();
                    this.mView.OnCheckProfileTab(false, true);

                    isWhatNewClicked = true;

                    if (this.mView.IsActive())
                    {
                        if (WhatsNewEntity.HasUnread())
                        {
                            this.mView.ShowUnreadWhatsNew(true);

                        }
                        else
                        {
                            this.mView.HideUnreadWhatsNew(true);

                        }
                    }
                    OnUpdateRewardUnRead();
                    break;
                case Resource.Id.menu_reward:
                    OnUpdateWhatsNewUnRead();
                    currentBottomNavigationMenu = Resource.Id.menu_reward;
                    trackBottomNavigationMenu = Resource.Id.menu_reward;
                    UserSessions.UpdateFromBRCard(mSharedPref);
                    this.mView.ShowRewardsMenu();
                    this.mView.OnCheckProfileTab(false, true);

                    isRewardClicked = true;

                    if (this.mView.IsActive())
                    {
                        if (RewardsEntity.HasUnread())
                        {
                            this.mView.ShowUnreadRewards(true);

                        }
                        else
                        {
                            this.mView.HideUnreadRewards(true);

                        }
                    }
                    break;
                case Resource.Id.menu_more:
                    UserSessions.UpdateFromBRCard(mSharedPref);
                    OnLoadMoreMenu();
                    break;
            }
        }

        public void ShowBillMenuWithAccount(CustomerBillingAccount account)
        {
            CustomerBillingAccount.SetSelected(account.AccNum);
            OnMenuSelect(Resource.Id.menu_bill);
        }

        public void OnLoadMoreMenu()
        {
            OnUpdateWhatsNewUnRead();
            currentBottomNavigationMenu = Resource.Id.menu_more;
            trackBottomNavigationMenu = Resource.Id.menu_more;
            OnUpdateRewardUnRead();
            this.mView.OnCheckProfileTab(true, false);
            this.mView.ShowMoreMenu();
        }

        public void DoLoadHomeDashBoardFragment()
        {
            this.mView.ShowHomeDashBoard();
            currentBottomNavigationMenu = Resource.Id.menu_dashboard;
            trackBottomNavigationMenu = Resource.Id.menu_dashboard;
        }

        public void SelectSupplyAccount()
        {

            if (cts != null && cts.Token.CanBeCanceled)
            {
                this.cts.Cancel();
            }

            List<CustomerBillingAccount> accountList = CustomerBillingAccount.List();
            if (accountList.Count >= 1)
            {
                this.mView.ShowSelectSupplyAccount();
            }
        }

        public void OnStartRewardThread()
        {
            bool IsRewardsDisabled = MyTNBAccountManagement.GetInstance().IsRewardsDisabled();

            if (!IsRewardsDisabled)
            {
                if (!RewardsMenuUtils.GetRewardLoading())
                {
                    this.mView.ShowProgressDialog();
                    RewardsMenuUtils.OnSetRewardLoading(true);
                    new SitecoreRewardAPI(mView).ExecuteOnExecutor(AsyncTask.ThreadPoolExecutor, "");
                }
                else
                {
                    this.mView.ShowProgressDialog();
                }
            }
        }

        public void OnStartWhatsNewThread()
        {
            if (!WhatsNewMenuUtils.GetWhatsNewLoading())
            {
                this.mView.ShowProgressDialog();
                WhatsNewMenuUtils.OnSetWhatsNewLoading(true);
                new SiteCoreWhatsNewAPI(mView).ExecuteOnExecutor(AsyncTask.ThreadPoolExecutor, "");
            }
            else
            {
                this.mView.ShowProgressDialog();
            }
        }

        public void OnResetRewardPromotionThread()
        {
            try
            {
                WhatsNewMenuUtils.OnSetWhatsNewLoading(true);
                new SiteCoreWhatsNewAPI(mView).ExecuteOnExecutor(AsyncTask.ThreadPoolExecutor, "");
                bool IsRewardsDisabled = MyTNBAccountManagement.GetInstance().IsRewardsDisabled();
                if (!IsRewardsDisabled)
                {
                    RewardsMenuUtils.OnSetRewardLoading(true);
                    new SitecoreRewardAPI(mView).ExecuteOnExecutor(AsyncTask.ThreadPoolExecutor, "");
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public bool GetIsWhatsNewDialogShowNeed()
        {
            return isWhatsNewDialogShowNeed;
        }

        public void SetIsWhatsNewDialogShowNeed(bool flag)
        {
            isWhatsNewDialogShowNeed = flag;
        }

        public void Start()
        {

            if (LaunchViewActivity.MAKE_INITIAL_CALL)
            {
                new SiteCoreFloatingButtonContentAPI(mView).ExecuteOnExecutor(AsyncTask.ThreadPoolExecutor, "");
                WhatsNewMenuUtils.OnSetWhatsNewLoading(true);
                new SiteCoreWhatsNewAPI(mView).ExecuteOnExecutor(AsyncTask.ThreadPoolExecutor, "");
                bool IsRewardsDisabled = MyTNBAccountManagement.GetInstance().IsRewardsDisabled();
                if (!IsRewardsDisabled)
                {
                    RewardsMenuUtils.OnSetRewardLoading(true);
                    new SitecoreRewardAPI(mView).ExecuteOnExecutor(AsyncTask.ThreadPoolExecutor, "");
                }
                isWhatsNewDialogShowNeed = true;
                LaunchViewActivity.MAKE_INITIAL_CALL = false;
            }

            if (currentBottomNavigationMenu == Resource.Id.menu_promotion || currentBottomNavigationMenu == Resource.Id.menu_reward || currentBottomNavigationMenu == Resource.Id.menu_more)
            {
                return;
            }

            if (currentBottomNavigationMenu == Resource.Id.menu_dashboard)
            {
                DoLoadHomeDashBoardFragment();
            }
            else if (currentBottomNavigationMenu == Resource.Id.menu_bill)
            {
                OnMenuSelect(Resource.Id.menu_bill);
            }

            this.mView.OnCheckDeeplink();
            this.mView.OnCheckNotification();
        }

        private void LoadUsageHistory(CustomerBillingAccount accountSelected)
        {
            try
            {
                if (smDataError)
                {
                    smDataError = false;
                    this.mView.ShowNMREChart(usageHistoryResponse, AccountData.Copy(accountSelected, true), smErrorCode, smErrorMessage);
                }
                else
                {
                    this.mView.ShowNMREChart(usageHistoryResponse, AccountData.Copy(accountSelected, true), null, null);
                }
                usageHistoryResponse = null;
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

        }


        private async void LoadSMUsageHistory(CustomerBillingAccount accountSelected)
        {
            try
            {
                this.mView.ShowSMChart(smUsageHistoryResponse, AccountData.Copy(accountSelected, true));
                smUsageHistoryResponse = null;
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

        }

        private void PreNavigateBllMenu(CustomerBillingAccount selectedAccount)
        {
            try
            {
                AccountData accountData = AccountData.Copy(selectedAccount, true);
                this.mView.SetAccountName(selectedAccount.AccDesc);
                currentBottomNavigationMenu = Resource.Id.menu_bill;
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        private void OnUpdateWhatsNewUnRead()
        {
            if (isWhatNewClicked && !UserSessions.HasWhatNewShown(mSharedPref))
            {
                UserSessions.DoWhatNewShown(mSharedPref);
            }

            if (WhatsNewEntity.HasUnread())
            {
                this.mView.ShowUnreadWhatsNew(false);

            }
            else
            {
                this.mView.HideUnreadWhatsNew(false);

            }

            isWhatNewClicked = false;
        }

        public void OnResumeUpdateWhatsNewUnRead()
        {
            if (WhatsNewEntity.HasUnread())
            {
                this.mView.ShowUnreadWhatsNew();

            }
            else
            {
                this.mView.HideUnreadWhatsNew();

            }
        }

        public void OnResumeUpdateProfileUnRead(bool key, bool isfromHome)
        {
            UserEntity user = UserEntity.GetActive();
            //var sharedpref_data = UserSessions.GetCheckEmailVerified(this.mSharedPref);
            //bool isUpdatePersonalDetail = bool.Parse(sharedpref_data);  //get from shared pref

            if (string.IsNullOrEmpty(user.IdentificationNo))
            {
                this.mView.ShowUnverifiedProfile(key, isfromHome);

            }
            else
            {
                this.mView.HideUnverifiedProfile(key, isfromHome);
            }
        }

        private void OnResumeWhatsNewUnRead()
        {
            if (isWhatNewClicked && !UserSessions.HasWhatNewShown(mSharedPref))
            {
                UserSessions.DoWhatNewShown(mSharedPref);
            }

            if (WhatsNewEntity.HasUnread())
            {
                this.mView.ShowUnreadWhatsNew();

            }
            else
            {
                this.mView.HideUnreadWhatsNew();

            }

            isWhatNewClicked = false;
        }

        private void OnUpdateRewardUnRead()
        {
            if (isRewardClicked && !UserSessions.HasRewardShown(mSharedPref))
            {
                UserSessions.DoRewardShown(mSharedPref);
            }

            if (RewardsEntity.HasUnread())
            {
                this.mView.ShowUnreadRewards(false);

            }
            else
            {
                this.mView.HideUnreadRewards(false);

            }

            isRewardClicked = false;
        }

        public void OnResumeUpdateRewardUnRead()
        {
            if (RewardsEntity.HasUnread())
            {
                this.mView.ShowUnreadRewards();

            }
            else
            {
                this.mView.HideUnreadRewards();
            }
        }

        private void OnResumeRewardUnRead()
        {
            if (isRewardClicked && !UserSessions.HasRewardShown(mSharedPref))
            {
                UserSessions.DoRewardShown(mSharedPref);
            }

            if (RewardsEntity.HasUnread())
            {
                this.mView.ShowUnreadRewards();

            }
            else
            {
                this.mView.HideUnreadRewards();
            }

            isRewardClicked = false;
        }

        public void OnValidateData()
        {
            OnResumeWhatsNewUnRead();
            OnResumeRewardUnRead();

            List<CustomerBillingAccount> accountList = CustomerBillingAccount.List();
            if (accountList.Count == 0)
            {
                this.mView.DisableBillMenu();
            }
        }

        public void OnAccountSelectDashBoard()
        {
            try
            {
                List<CustomerBillingAccount> accountList = new List<CustomerBillingAccount>();
                accountList = CustomerBillingAccount.List();
                this.mView.SetDashboardHomeCheck();
                if (accountList != null && accountList.Count > 0)
                {
                    DashboardHomeActivity.GO_TO_INNER_DASHBOARD = true;
                    currentBottomNavigationMenu = Resource.Id.menu_dashboard;
                    trackBottomNavigationMenu = Resource.Id.menu_dashboard;
                    if (CustomerBillingAccount.HasSelected())
                    {
                        CustomerBillingAccount selected = new CustomerBillingAccount();
                        selected = CustomerBillingAccount.GetSelected();

                        if (selected != null && !string.IsNullOrEmpty(selected.AccDesc))
                        {
                            /** Smart meter account check **/
                            if (!selected.SmartMeterCode.Equals("0"))
                            {
                                if (!SMUsageHistoryEntity.IsSMDataUpdated(selected.AccNum))
                                {
                                    //Get stored data
                                    SMUsageHistoryEntity storedEntity = new SMUsageHistoryEntity();
                                    if (!string.IsNullOrEmpty(selected.AccNum))
                                    {
                                        storedEntity = SMUsageHistoryEntity.GetItemByAccountNo(selected.AccNum);
                                    }
                                    SMUsageHistoryResponse storedSMData = new SMUsageHistoryResponse();

                                    if (storedEntity != null)
                                    {
                                        storedSMData = JsonConvert.DeserializeObject<SMUsageHistoryResponse>(storedEntity.JsonResponse);
                                    }

                                    CustomerBillingAccount.RemoveSelected();
                                    if (!string.IsNullOrEmpty(selected.AccNum))
                                    {
                                        CustomerBillingAccount.SetSelected(selected.AccNum);
                                    }

                                    if (storedSMData != null && storedSMData.Data != null && storedSMData.Data.ErrorCode != "7200")
                                    {
                                        smUsageHistoryResponse = null;
                                    }
                                    else if ((storedSMData != null && storedSMData.Data == null) || (storedSMData == null))
                                    {
                                        smUsageHistoryResponse = null;
                                    }
                                    else if (storedSMData.Data.IsMDMSCurrentlyUnavailable || !IsCheckHaveByMonthData(storedSMData.Data.SMUsageHistoryData))
                                    {
                                        smUsageHistoryResponse = null;
                                    }
                                    else
                                    {
                                        smUsageHistoryResponse = storedSMData;
                                    }

                                    if (MyTNBAccountManagement.GetInstance().IsEBUserVerify())
                                    {
                                        smUsageHistoryResponse = null;
                                    }

                                    LoadSMUsageHistory(selected);
                                }
                                else
                                {
                                    smUsageHistoryResponse = null;
                                    LoadSMUsageHistory(selected);
                                }
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(selected.AccNum) && !UsageHistoryEntity.IsSMDataUpdated(selected.AccNum))
                                {
                                    UsageHistoryEntity storedEntity = new UsageHistoryEntity();
                                    storedEntity = UsageHistoryEntity.GetItemByAccountNo(selected.AccNum);
                                    if (storedEntity != null)
                                    {
                                        CustomerBillingAccount.RemoveSelected();
                                        if (!string.IsNullOrEmpty(selected.AccNum))
                                        {
                                            CustomerBillingAccount.SetSelected(selected.AccNum);
                                        }
                                        usageHistoryResponse = JsonConvert.DeserializeObject<UsageHistoryResponse>(storedEntity.JsonResponse);
                                        if (usageHistoryResponse != null && usageHistoryResponse.Data != null && usageHistoryResponse.Data.ErrorCode != "7200")
                                        {
                                            usageHistoryResponse = null;
                                        }
                                        else if ((usageHistoryResponse != null && usageHistoryResponse.Data == null) || (usageHistoryResponse == null))
                                        {
                                            usageHistoryResponse = null;
                                        }
                                        else if (!IsCheckHaveByMonthData(usageHistoryResponse.Data.UsageHistoryData))
                                        {
                                            usageHistoryResponse = null;
                                        }
                                        LoadUsageHistory(selected);
                                    }
                                    else
                                    {
                                        usageHistoryResponse = null;
                                        LoadUsageHistory(selected);
                                    }
                                }
                                else
                                {
                                    usageHistoryResponse = null;
                                    LoadUsageHistory(selected);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(accountList[0].AccNum))
                        {
                            CustomerBillingAccount.SetSelected(accountList[0].AccNum);
                            CustomerBillingAccount selected = new CustomerBillingAccount();
                            selected = CustomerBillingAccount.GetSelected();
                            if (selected != null && !string.IsNullOrEmpty(selected.AccNum))
                            {
                                if (!selected.SmartMeterCode.Equals("0"))
                                {
                                    LoadSMUsageHistory(selected);
                                }
                                else
                                {
                                    LoadUsageHistory(selected);
                                }
                            }
                        }
                    }

                }
                else
                {
                    DashboardHomeActivity.GO_TO_INNER_DASHBOARD = false;
                    DoLoadHomeDashBoardFragment();
                    this.mView.DisableBillMenu();
                }
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void OnTapToRefresh()
        {
            OnAccountSelectDashBoard();
        }

        public int CheckCurrentDashboardMenu()
        {
            return currentBottomNavigationMenu;
        }

        public void UpdateTrackDashboardMenu(int resId)
        {
            currentBottomNavigationMenu = resId;
            trackBottomNavigationMenu = resId;
        }

        private bool IsCheckHaveByMonthData(UsageHistoryData data)
        {
            bool isHaveData = true;

            if (data == null || (data != null && data.ByMonth == null) || (data != null && data.ByMonth != null && data.ByMonth.Months == null) || (data != null && data.ByMonth != null && data.ByMonth.Months != null && data.ByMonth.Months.Count == 0))
            {
                isHaveData = false;
            }
            else
            {
                for (int i = 0; i < data.ByMonth.Months.Count; i++)
                {
                    if ((string.IsNullOrEmpty(data.ByMonth.Months[i].UsageTotal.ToString()) && string.IsNullOrEmpty(data.ByMonth.Months[i].AmountTotal.ToString())) || (Math.Abs(data.ByMonth.Months[i].UsageTotal) < 0.001 && Math.Abs(data.ByMonth.Months[i].AmountTotal) < 0.001))
                    {
                        isHaveData = false;
                    }
                    else
                    {
                        isHaveData = true;
                        break;
                    }
                }
            }

            return isHaveData;
        }

        private bool IsCheckHaveByMonthData(SMUsageHistoryData data)
        {
            bool isHaveData = true;

            if (data == null || (data != null && data.ByMonth == null) || (data != null && data.ByMonth != null && data.ByMonth.Months == null) || (data != null && data.ByMonth != null && data.ByMonth.Months != null && data.ByMonth.Months.Count == 0))
            {
                isHaveData = false;
            }
            else
            {
                for (int i = 0; i < data.ByMonth.Months.Count; i++)
                {
                    if ((string.IsNullOrEmpty(data.ByMonth.Months[i].UsageTotal.ToString()) && string.IsNullOrEmpty(data.ByMonth.Months[i].AmountTotal.ToString())) || (Math.Abs(data.ByMonth.Months[i].UsageTotal) < 0.001 && Math.Abs(data.ByMonth.Months[i].AmountTotal) < 0.001))
                    {
                        isHaveData = false;
                    }
                    else
                    {
                        isHaveData = true;
                        break;
                    }
                }
            }

            return isHaveData;
        }

        public async Task OnGetUserRewardList()
        {
            try
            {
                UserInterface currentUsrInf = new UserInterface()
                {
                    eid = UserEntity.GetActive().Email,
                    sspuid = UserEntity.GetActive().UserID,
                    did = UserEntity.GetActive().DeviceId,
                    ft = FirebaseTokenEntity.GetLatest().FBToken,
                    lang = LanguageUtil.GetAppLanguage().ToUpper(),
                    sec_auth_k1 = Constants.APP_CONFIG.API_KEY_ID,
                    sec_auth_k2 = "",
                    ses_param1 = "",
                    ses_param2 = ""
                };

                GetUserRewardsRequest request = new GetUserRewardsRequest()
                {
                    usrInf = currentUsrInf
                };

                GetUserRewardsResponse response = await this.mApi.GetUserRewards(request, new System.Threading.CancellationTokenSource().Token);

                if (response != null && response.Data != null && response.Data.ErrorCode == "7200")
                {
                    if (response.Data.Data != null && response.Data.Data.CurrentList != null && response.Data.Data.CurrentList.Count > 0)
                    {
                        userList = response.Data.Data.CurrentList;
                    }
                    else
                    {
                        userList = new List<AddUpdateRewardModel>();
                    }
                    CheckRewardsCache();
                }
                else
                {
                    this.mView.OnCheckUserRewardApiFailed();
                }

            }
            catch (Exception e)
            {
                userList = new List<AddUpdateRewardModel>();
                this.mView.OnCheckUserRewardApiFailed();
                Utility.LoggingNonFatalError(e);
            }
        }

        public void CheckRewardsCache()
        {
            if (mRewardsCategoryEntity == null)
            {
                mRewardsCategoryEntity = new RewardsCategoryEntity();
            }

            if (mRewardsEntity == null)
            {
                mRewardsEntity = new RewardsEntity();
            }

            List<RewardsCategoryModel> mDisplayCategoryList = new List<RewardsCategoryModel>();

            List<RewardsCategoryEntity> mCategoryList = mRewardsCategoryEntity.GetAllItems();

            if (mCategoryList != null && mCategoryList.Count > 0)
            {
                for (int i = 0; i < mCategoryList.Count; i++)
                {
                    List<RewardsEntity> checkList = mRewardsEntity.GetActiveItemsByCategory(mCategoryList[i].ID);
                    if (checkList != null && checkList.Count > 0)
                    {
                        for (int j = 0; j < checkList.Count; j++)
                        {
                            if (userList != null && userList.Count > 0)
                            {
                                string checkID = checkList[j].ID;
                                checkID = checkID.Replace("{", "");
                                checkID = checkID.Replace("}", "");

                                AddUpdateRewardModel found = userList.Find(x => x.RewardId.Contains(checkID));
                                if (found != null)
                                {
                                    if (found.Read)
                                    {
                                        string readDate = !string.IsNullOrEmpty(found.ReadDate) ? found.ReadDate : "";
                                        if (readDate.Contains("Date("))
                                        {
                                            int startIndex = readDate.LastIndexOf("(") + 1;
                                            int lastIndex = readDate.LastIndexOf(")");
                                            int lengthOfId = (lastIndex - startIndex);
                                            if (lengthOfId < readDate.Length)
                                            {
                                                string timeStamp = readDate.Substring(startIndex, lengthOfId);
                                                DateTime dateTimeParse = DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(timeStamp)).DateTime;
                                                CultureInfo currCult = CultureInfo.CreateSpecificCulture("en-US");
                                                readDate = dateTimeParse.ToString(@"M/d/yyyy h:m:s tt", currCult);
                                            }
                                        }
                                        mRewardsEntity.UpdateReadItem(checkList[j].ID, found.Read, readDate);
                                    }

                                    if (found.Favourite)
                                    {
                                        string favDate = !string.IsNullOrEmpty(found.FavUpdatedDate) ? found.FavUpdatedDate : "";
                                        if (favDate.Contains("Date("))
                                        {
                                            int startIndex = favDate.LastIndexOf("(") + 1;
                                            int lastIndex = favDate.LastIndexOf(")");
                                            int lengthOfId = (lastIndex - startIndex);
                                            if (lengthOfId < favDate.Length)
                                            {
                                                string timeStamp = favDate.Substring(startIndex, lengthOfId);
                                                DateTime dateTimeParse = DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(timeStamp)).DateTime;
                                                CultureInfo currCult = CultureInfo.CreateSpecificCulture("en-US");
                                                favDate = dateTimeParse.ToString(@"M/d/yyyy h:m:s tt", currCult);
                                            }
                                        }
                                        mRewardsEntity.UpdateIsSavedItem(checkList[j].ID, found.Favourite, favDate);
                                    }
                                    else
                                    {
                                        mRewardsEntity.UpdateIsSavedItem(checkList[j].ID, found.Favourite, "");
                                    }

                                    if (found.Redeemed)
                                    {
                                        string redeemDate = !string.IsNullOrEmpty(found.RedeemedDate) ? found.RedeemedDate : "";
                                        if (redeemDate.Contains("Date("))
                                        {
                                            int startIndex = redeemDate.LastIndexOf("(") + 1;
                                            int lastIndex = redeemDate.LastIndexOf(")");
                                            int lengthOfId = (lastIndex - startIndex);
                                            if (lengthOfId < redeemDate.Length)
                                            {
                                                string timeStamp = redeemDate.Substring(startIndex, lengthOfId);
                                                DateTime dateTimeParse = DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(timeStamp)).DateTime;
                                                CultureInfo currCult = CultureInfo.CreateSpecificCulture("en-US");
                                                redeemDate = dateTimeParse.ToString(@"M/d/yyyy h:m:s tt", currCult);
                                            }
                                        }
                                        mRewardsEntity.UpdateIsUsedItem(checkList[j].ID, found.Redeemed, redeemDate);
                                    }
                                }
                            }
                        }

                        List<RewardsEntity> reCheckList = mRewardsEntity.GetActiveItemsByCategory(mCategoryList[i].ID);

                        if (reCheckList != null && reCheckList.Count > 0)
                        {
                            mDisplayCategoryList.Add(new RewardsCategoryModel()
                            {
                                ID = mCategoryList[i].ID,
                                CategoryName = mCategoryList[i].CategoryName
                            });
                        }
                        else
                        {
                            mRewardsEntity.RemoveItemByCategoryId(mCategoryList[i].ID);
                            mRewardsCategoryEntity.RemoveItem(mCategoryList[i].ID);
                        }
                    }
                    else
                    {
                        mRewardsEntity.RemoveItemByCategoryId(mCategoryList[i].ID);
                        mRewardsCategoryEntity.RemoveItem(mCategoryList[i].ID);
                    }
                }
            }

            this.mView.OnCheckRewardTab();
        }

        public void CheckWhatsNewCache()
        {
            if (mWhatsNewCategoryEntity == null)
            {
                mWhatsNewCategoryEntity = new WhatsNewCategoryEntity();
            }

            if (mWhatsNewEntity == null)
            {
                mWhatsNewEntity = new WhatsNewEntity();
            }

            List<WhatsNewCategoryModel> mDisplayCategoryList = new List<WhatsNewCategoryModel>();

            List<WhatsNewCategoryEntity> mCategoryList = mWhatsNewCategoryEntity.GetAllItems();

            if (mCategoryList != null && mCategoryList.Count > 0)
            {
                for (int i = 0; i < mCategoryList.Count; i++)
                {
                    List<WhatsNewEntity> checkList = mWhatsNewEntity.GetActiveItemsByCategory(mCategoryList[i].ID);
                    if (checkList != null && checkList.Count > 0)
                    {
                        mDisplayCategoryList.Add(new WhatsNewCategoryModel()
                        {
                            ID = mCategoryList[i].ID,
                            CategoryName = mCategoryList[i].CategoryName
                        });
                    }
                    else
                    {
                        //mWhatsNewEntity.RemoveItemByCategoryId(mCategoryList[i].ID);
                        //mWhatsNewCategoryEntity.RemoveItem(mCategoryList[i].ID);
                    }
                }
            }

            this.mView.OnCheckWhatsNewTab();
        }

        public void UpdateRewardRead(string itemID, bool flag)
        {
            DateTime currentDate = DateTime.UtcNow;
            RewardsEntity wtManager = new RewardsEntity();
            CultureInfo currCult = CultureInfo.CreateSpecificCulture("en-US");
            string formattedDate = currentDate.ToString(@"M/d/yyyy h:m:s tt", currCult);
            if (!flag)
            {
                formattedDate = "";

            }
            wtManager.UpdateReadItem(itemID, flag, formattedDate);

            _ = OnUpdateReward(itemID);
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
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    string density = DPUtils.GetDeviceDensity(Application.Context);
                    GetItemsService getItemsService = new GetItemsService(SiteCoreConfig.OS, density, SiteCoreConfig.SITECORE_URL, LanguageUtil.GetAppLanguage());
                    SSMRMeterReadingTimeStampResponseModel responseModel = getItemsService.GetSSMRMeterReadingOnePhaseWalkthroughTimestampItem();
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
            }, new CancellationTokenSource().Token);
        }

        public Task OnGetSSMRMeterReadingScreens()
        {
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    string density = DPUtils.GetDeviceDensity(Application.Context);
                    GetItemsService getItemsService = new GetItemsService(SiteCoreConfig.OS, density, SiteCoreConfig.SITECORE_URL, LanguageUtil.GetAppLanguage());
                    SSMRMeterReadingResponseModel responseModel = getItemsService.GetSSMRMeterReadingOnePhaseWalkthroughItems();
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
            }, new CancellationTokenSource().Token);
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
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    string density = DPUtils.GetDeviceDensity(Application.Context);
                    GetItemsService getItemsService = new GetItemsService(SiteCoreConfig.OS, density, SiteCoreConfig.SITECORE_URL, LanguageUtil.GetAppLanguage());
                    SSMRMeterReadingTimeStampResponseModel responseModel = getItemsService.GetSSMRMeterReadingOnePhaseOCROffWalkthroughTimestampItem();
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
            }, new CancellationTokenSource().Token);
        }

        public Task OnGetSSMRMeterReadingScreensNoOCR()
        {
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    string density = DPUtils.GetDeviceDensity(Application.Context);
                    GetItemsService getItemsService = new GetItemsService(SiteCoreConfig.OS, density, SiteCoreConfig.SITECORE_URL, LanguageUtil.GetAppLanguage());
                    SSMRMeterReadingResponseModel responseModel = getItemsService.GetSSMRMeterReadingOnePhaseOCROffWalkthroughItems();
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
            }, new CancellationTokenSource().Token);
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
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    string density = DPUtils.GetDeviceDensity(Application.Context);
                    GetItemsService getItemsService = new GetItemsService(SiteCoreConfig.OS, density, SiteCoreConfig.SITECORE_URL, LanguageUtil.GetAppLanguage());
                    SSMRMeterReadingTimeStampResponseModel responseModel = getItemsService.GetSSMRMeterReadingThreePhaseWalkthroughTimestampItem();
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
            }, new CancellationTokenSource().Token);
        }

        public Task OnGetSSMRMeterReadingThreePhaseScreens()
        {
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    string density = DPUtils.GetDeviceDensity(Application.Context);
                    GetItemsService getItemsService = new GetItemsService(SiteCoreConfig.OS, density, SiteCoreConfig.SITECORE_URL, LanguageUtil.GetAppLanguage());
                    SSMRMeterReadingResponseModel responseModel = getItemsService.GetSSMRMeterReadingThreePhaseWalkthroughItems();
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
            }, new CancellationTokenSource().Token);
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
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    string density = DPUtils.GetDeviceDensity(Application.Context);
                    GetItemsService getItemsService = new GetItemsService(SiteCoreConfig.OS, density, SiteCoreConfig.SITECORE_URL, LanguageUtil.GetAppLanguage());
                    SSMRMeterReadingTimeStampResponseModel responseModel = getItemsService.GetSSMRMeterReadingThreePhaseOCROffWalkthroughTimestampItem();
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
            }, new CancellationTokenSource().Token);
        }

        public Task OnGetSSMRMeterReadingThreePhaseScreensNoOCR()
        {
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    string density = DPUtils.GetDeviceDensity(Application.Context);
                    GetItemsService getItemsService = new GetItemsService(SiteCoreConfig.OS, density, SiteCoreConfig.SITECORE_URL, LanguageUtil.GetAppLanguage());
                    SSMRMeterReadingResponseModel responseModel = getItemsService.GetSSMRMeterReadingThreePhaseOCROffWalkthroughItems();
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
            }, new CancellationTokenSource().Token);
        }

        public Task OnGetEPPTooltipContentDetail()
        {
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    string density = DPUtils.GetDeviceDensity(Application.Context);
                    GetItemsService getItemsService = new GetItemsService(SiteCoreConfig.OS, density, SiteCoreConfig.SITECORE_URL, LanguageUtil.GetAppLanguage());

                    EppToolTipTimeStampResponseModel timestampModel = getItemsService.GetEppToolTipTimeStampItem();
                    if (timestampModel.Status.Equals("Success") && timestampModel.Data != null && timestampModel.Data.Count > 0)
                    {
                        if (SitecoreCmsEntity.IsNeedUpdates(SitecoreCmsEntity.SITE_CORE_ID.EPP_TOOLTIP, timestampModel.Data[0].Timestamp))
                        {
                            EppToolTipResponseModel responseModel = getItemsService.GetEppToolTipItem();

                            if (responseModel.Status.Equals("Success"))
                            {
                                SitecoreCmsEntity.InsertSiteCoreItem(SitecoreCmsEntity.SITE_CORE_ID.EPP_TOOLTIP, JsonConvert.SerializeObject(responseModel.Data), timestampModel.Data[0].Timestamp);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                }
            });
        }

        public Task OnWhereIsMyAccNumberContentDetail()
        {
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    string density = DPUtils.GetDeviceDensity(Application.Context);
                    GetItemsService getItemsService = new GetItemsService(SiteCoreConfig.OS, density, SiteCoreConfig.SITECORE_URL, LanguageUtil.GetAppLanguage());

                    WhereIsMyAccNumberTimeStampResponseModel timestampModel = getItemsService.GetWhereIsMyAccToolTipTimeStampItem();
                    if (timestampModel.Status.Equals("Success") && timestampModel.Data != null && timestampModel.Data.Count > 0)
                    {
                        if (SitecoreCmsEntity.IsNeedUpdates(SitecoreCmsEntity.SITE_CORE_ID.WHERE_IS_MY_ACC, timestampModel.Data[0].Timestamp))
                        {
                            WhereIsMyAccNumberResponseModel responseModel = getItemsService.GetWhereIsMyAccToolTipItem();

                            if (responseModel.Status.Equals("Success"))
                            {
                                SitecoreCmsEntity.InsertSiteCoreItem(SitecoreCmsEntity.SITE_CORE_ID.WHERE_IS_MY_ACC, JsonConvert.SerializeObject(responseModel.Data), timestampModel.Data[0].Timestamp);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                }
            });
        }

        public Task OnGetBillTooltipContent()
        {
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    GetBillTooltip(BillsTooltipVersionEnum.V1, SitecoreCmsEntity.SITE_CORE_ID.BILL_TOOLTIP);
                    GetBillTooltip(BillsTooltipVersionEnum.V2, SitecoreCmsEntity.SITE_CORE_ID.BILL_TOOLTIPV2);
                }
                catch (Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                }
            });
        }

        private void GetBillTooltip(BillsTooltipVersionEnum billsTooltipVersionEnum, SitecoreCmsEntity.SITE_CORE_ID siteCoreId)
        {
            string density = DPUtils.GetDeviceDensity(Application.Context);
            GetItemsService getItemsService = new GetItemsService(SiteCoreConfig.OS, density, SiteCoreConfig.SITECORE_URL, LanguageUtil.GetAppLanguage());
            BillDetailsTooltipTimeStampResponseModel timestampModel = getItemsService.GetBillDetailsTooltipTimestampItem(billsTooltipVersionEnum);
            if (timestampModel.Status.Equals("Success") && timestampModel.Data != null && timestampModel.Data.Count > 0)
            {
                if (SitecoreCmsEntity.IsNeedUpdates(siteCoreId, timestampModel.Data[0].Timestamp))
                {
                    BillDetailsTooltipResponseModel responseModel = getItemsService.GetBillDetailsTooltipItem(billsTooltipVersionEnum);
                    SitecoreCmsEntity.InsertSiteCoreItem(siteCoreId, JsonConvert.SerializeObject(responseModel.Data), timestampModel.Data[0].Timestamp);
                }
            }
        }

        private async Task OnUpdateReward(string itemID)
        {
            try
            {
                // Update api calling
                RewardsEntity wtManager = new RewardsEntity();
                RewardsEntity currentItem = wtManager.GetItem(itemID);

                UserInterface currentUsrInf = new UserInterface()
                {
                    eid = UserEntity.GetActive().Email,
                    sspuid = UserEntity.GetActive().UserID,
                    did = UserEntity.GetActive().DeviceId,
                    ft = FirebaseTokenEntity.GetLatest().FBToken,
                    lang = LanguageUtil.GetAppLanguage().ToUpper(),
                    sec_auth_k1 = Constants.APP_CONFIG.API_KEY_ID,
                    sec_auth_k2 = "",
                    ses_param1 = "",
                    ses_param2 = ""
                };

                string rewardId = currentItem.ID;
                rewardId = rewardId.Replace("{", "");
                rewardId = rewardId.Replace("}", "");

                AddUpdateRewardModel currentReward = new AddUpdateRewardModel()
                {
                    Email = UserEntity.GetActive().Email,
                    RewardId = rewardId,
                    Read = currentItem.Read,
                    ReadDate = !string.IsNullOrEmpty(currentItem.ReadDateTime) ? currentItem.ReadDateTime + " +00:00" : "",
                    Favourite = currentItem.IsSaved,
                    FavUpdatedDate = !string.IsNullOrEmpty(currentItem.IsSavedDateTime) ? currentItem.IsSavedDateTime + " +00:00" : "",
                    Redeemed = currentItem.IsUsed,
                    RedeemedDate = !string.IsNullOrEmpty(currentItem.IsUsedDateTime) ? currentItem.IsUsedDateTime + " +00:00" : ""
                };

                AddUpdateRewardRequest request = new AddUpdateRewardRequest()
                {
                    usrInf = currentUsrInf,
                    reward = currentReward
                };

                AddUpdateRewardResponse response = await this.mApi.AddUpdateReward(request, new System.Threading.CancellationTokenSource().Token);

            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void UpdateWhatsNewRead(string itemID, bool flag)
        {
            DateTime currentDate = DateTime.UtcNow;
            WhatsNewEntity wtManager = new WhatsNewEntity();
            CultureInfo currCult = CultureInfo.CreateSpecificCulture("en-US");
            string formattedDate = currentDate.ToString(@"M/d/yyyy h:m:s tt", currCult);
            if (!flag)
            {
                formattedDate = "";

            }
            wtManager.UpdateReadItem(itemID, flag, formattedDate);
        }

        public List<NewAppModel> OnGeneraNewAppTutorialList(bool isOwner, string accountTypeId)
        {
            List<NewAppModel> newList = new List<NewAppModel>();
            bool isNeedHelpHide = true;
            
            if (isOwner)
            {
                newList.Add(new NewAppModel()
                {
                    ContentShowPosition = ContentType.BottomRight,
                    ContentTitle = Utility.GetLocalizedLabel("DashboardHome", "tutorialUsageTitle"),
                    ContentMessage = Utility.GetLocalizedLabel("DashboardHome", "tutorialUsageDesc"),
                    ItemCount = CustomerBillingAccount.GetSortedCustomerBillingAccounts().Count,
                    NeedHelpHide = isNeedHelpHide,
                    IsButtonShow = false
                });
            }
            else
            {
                if (!isOwner || (accountTypeId.Equals("2") || accountTypeId.Equals("3")))
                {
                    newList.Add(new NewAppModel()
                    {
                        ContentShowPosition = ContentType.BottomRight,
                        ContentTitle = Utility.GetLocalizedLabel("DashboardHome", "tutorialUsageTitleNonOwner"),
                        ContentMessage = Utility.GetLocalizedLabel("DashboardHome", "tutorialUsageDescNonOwner"),
                        ItemCount = CustomerBillingAccount.GetSortedCustomerBillingAccounts().Count,
                        NeedHelpHide = isNeedHelpHide,
                        IsButtonShow = false
                    });
                }
                
            }            
            return newList;
        }
        
        public void OnGetBillEligibilityCheck(string accountNumber)
        {
            this.GetBillEligibilityCheck(accountNumber);
        }

        public CustomerBillingAccount GetEligibleDBRAccount()
        {
            CustomerBillingAccount customerAccount = CustomerBillingAccount.GetSelected();
            List<string> dBRCAs = DBRUtility.Instance.GetCAList();
            List<CustomerBillingAccount> allAccountList = CustomerBillingAccount.List();
            CustomerBillingAccount account = new CustomerBillingAccount();
            if (dBRCAs.Count > 0)
            {
                var dbrSelected = dBRCAs.Where(x => x == customerAccount.AccNum).FirstOrDefault();
                if (dbrSelected != string.Empty)
                {
                    account = allAccountList.Where(x => x.AccNum == dbrSelected).FirstOrDefault();
                }
                if (account == null)
                {
                    foreach (var dbrca in dBRCAs)
                    {
                        account = allAccountList.Where(x => x.AccNum == dbrca).FirstOrDefault();
                        if (account != null)
                        {
                            break;
                        }
                    }
                }
            }
            return account;
        }

        public CustomerBillingAccount GetBRcountList()
        {
            CustomerBillingAccount customerAccount = CustomerBillingAccount.GetSelected();
            List<string> BRCas = BillRedesignUtility.Instance.GetCAList();
            List<CustomerBillingAccount> allAccountList = CustomerBillingAccount.List();
            CustomerBillingAccount account = new CustomerBillingAccount();
            if (BRCas.Count > 0)
            {
                var dbrSelected = BRCas.Where(x => x == customerAccount.AccNum).FirstOrDefault();
                if (dbrSelected != string.Empty)
                {
                    account = allAccountList.Where(x => x.AccNum == dbrSelected).FirstOrDefault();
                }
                if (account == null)
                {
                    foreach (var dbrca in BRCas)
                    {
                        account = allAccountList.Where(x => x.AccNum == dbrca).FirstOrDefault();
                        if (account != null)
                        {
                            break;
                        }
                    }
                }
            }
            return account;
        }

        public void OnCheckDraftForResume(ISharedPreferences prefs)
        {
            if (MyHomeUtility.Instance.IsAccountEligible)
            {
                this.CheckNCDraftForResume(prefs);
            }
        }

        public void GetSavedFloatingButtonTimeStamp()
        {
            try
            {
                FloatingButtonTimeOutMillisecond = FloatingButtonDefaultTimeOutMillisecond;
                FloatingButtonParentEntity wtManager = new FloatingButtonParentEntity();
                List<FloatingButtonParentEntity> items = wtManager.GetAllItems();
                if (items != null && items.Count != 0)
                {
                    foreach (FloatingButtonParentEntity obj in items)
                    {
                        this.mView.OnSavedFloatingButtonTimeStampRecieved(obj.Timestamp);
                    }
                }
                else
                {
                    this.mView.OnSavedFloatingButtonTimeStampRecieved(null);
                }

            }
            catch (Exception e)
            {
                this.mView.OnSavedFloatingButtonTimeStampRecieved(null);
                Utility.LoggingNonFatalError(e);
            }

        }

        public void OnGetFloatingButtonTimeStamp()
        {
            CancellationTokenSource token = new CancellationTokenSource();
            Stopwatch sw = Stopwatch.StartNew();
            _ = Task.Run(() =>
            {
                try
                {
                    string density = DPUtils.GetDeviceDensity(Application.Context);
                    GetItemsService getItemsService = new GetItemsService(SiteCoreConfig.OS, density, SiteCoreConfig.SITECORE_URL, LanguageUtil.GetAppLanguage());
                    FloatingButtonTimeStampResponseModel responseModel = getItemsService.GetFloatingButtonTimestampItem();
                    sw.Stop();
                    try
                    {
                        if (FloatingButtonTimeOutMillisecond > 0)
                        {
                            FloatingButtonTimeOutMillisecond = FloatingButtonTimeOutMillisecond - (int)sw.ElapsedMilliseconds;
                            if (FloatingButtonTimeOutMillisecond <= 0)
                            {
                                FloatingButtonTimeOutMillisecond = 0;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Utility.LoggingNonFatalError(e);
                    }

                    if (responseModel.Status.Equals("Success"))
                    {
                        FloatingButtonParentEntity wtManager = new FloatingButtonParentEntity();
                        wtManager.DeleteTable();
                        wtManager.CreateTable();
                        wtManager.InsertListOfItems(responseModel.Data);
                        mView.OnFloatingButtonTimeStampRecieved(responseModel.Data[0].Timestamp);
                    }
                    else
                    {
                        mView.OnFloatingButtonTimeStampRecieved(null);
                    }
                }
                catch (Exception e)
                {
                    mView.OnFloatingButtonTimeStampRecieved(null);
                    Utility.LoggingNonFatalError(e);
                }
            }, token.Token);

            if (FloatingButtonTimeOutMillisecond > 0)
            {
                _ = Task.Delay(FloatingButtonTimeOutMillisecond).ContinueWith(_ =>
                {
                    if (FloatingButtonTimeOutMillisecond > 0)
                    {
                        FloatingButtonTimeOutMillisecond = 0;
                        OnGetFloatingButtonCache();
                    }
                });
            }
        }

        public void OnGetFloatingButtonItem()
        {
            CancellationTokenSource token = new CancellationTokenSource();
            Stopwatch sw = Stopwatch.StartNew();
            _ = Task.Run(() =>
            {
                try
                {
                    string density = DPUtils.GetDeviceDensity(Application.Context);
                    GetItemsService getItemsService = new GetItemsService(SiteCoreConfig.OS, density, SiteCoreConfig.SITECORE_URL, LanguageUtil.GetAppLanguage());
                    FloatingButtonResponseModel responseModel = getItemsService.GetFloatingButtonItem();
                    sw.Stop();
                    try
                    {
                        if (FloatingButtonTimeOutMillisecond > 0)
                        {
                            FloatingButtonTimeOutMillisecond = FloatingButtonTimeOutMillisecond - (int)sw.ElapsedMilliseconds;
                            if (FloatingButtonTimeOutMillisecond <= 0)
                            {
                                FloatingButtonTimeOutMillisecond = 0;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Utility.LoggingNonFatalError(e);
                    }

                    if (responseModel.Status.Equals("Success"))
                    {
                        IsOnGetPhotoRunning = false;
                        FloatingButtonEntity wtManager = new FloatingButtonEntity();
                        wtManager.DeleteTable();
                        wtManager.CreateTable();
                        wtManager.InsertListOfItems(responseModel.Data);
                        OnGetFloatingButtonCache();
                    }
                    else
                    {
                        OnGetFloatingButtonCache();
                    }
                }
                catch (Exception e)
                {
                    OnGetFloatingButtonCache();
                    Utility.LoggingNonFatalError(e);
                }
            }, token.Token);

            if (FloatingButtonTimeOutMillisecond > 0)
            {
                _ = Task.Delay(FloatingButtonTimeOutMillisecond).ContinueWith(_ =>
                {
                    if (FloatingButtonTimeOutMillisecond > 0)
                    {
                        FloatingButtonTimeOutMillisecond = 0;
                        OnGetFloatingButtonCache();
                    }
                });
            }
        }

        public Task OnGetFloatingButtonCache()
        {
            CancellationTokenSource token = new CancellationTokenSource();
            return Task.Run(() =>
            {
                try
                {
                    FloatingButtonEntity wtManager = new FloatingButtonEntity();
                    List<FloatingButtonEntity> floatingButtonList = wtManager.GetAllItems();
                    if (floatingButtonList.Count > 0)
                    {
                        FloatingButtonModel item = new FloatingButtonModel()
                        {
                            ID = floatingButtonList[0].ID,
                            Image = floatingButtonList[0].Image,
                            ImageB64 = floatingButtonList[0].ImageB64,
                            Title = floatingButtonList[0].Title,
                            Description = floatingButtonList[0].Description,
                            StartDateTime = floatingButtonList[0].StartDateTime,
                            EndDateTime = floatingButtonList[0].EndDateTime,
                            ShowForSeconds = floatingButtonList[0].ShowForSeconds,
                            ImageBitmap = null
                        };
                        OnProcessFloatingButtonItem(item);
                    }
                    else
                    {
                        FloatingButtonTimeOutMillisecond = 0;
                        //if (!this.mView.GetFloatingButtonSiteCoreDoneFlag())
                        //{
                        //    this.mView.SetDefaultAppLaunchImage();
                        //}
                    }
                }
                catch (Exception e)
                {
                    FloatingButtonTimeOutMillisecond = 0;
                    //if (!this.mView.GetAppLaunchSiteCoreDoneFlag())
                    //{
                    //    this.mView.SetDefaultAppLaunchImage();
                    //}
                    Utility.LoggingNonFatalError(e);
                }
            }, token.Token);
        }

        private void OnProcessFloatingButtonItem(FloatingButtonModel item)
        {
            try
            {
                if (!string.IsNullOrEmpty(item.ImageB64))
                {
                    Bitmap convertedImageCache = Base64ToBitmap(item.ImageB64);
                    if (convertedImageCache != null)
                    {
                        FloatingButtonTimeOutMillisecond = 0;
                        item.ImageBitmap = convertedImageCache;
                        FloatingButtonUtils.SetFloatingButtonBitmap(item);
                        if (!this.mView.GetFloatingButtonSiteCoreDoneFlag())
                        {
                            this.mView.SetCustomFloatingButtonImage(item);
                            this.mView.PopulateFloatingButton(item);
                        }
                    }
                    else
                    {
                        OnGetPhoto(item);
                    }
                }
                else
                {
                    OnGetPhoto(item);
                }
            }
            catch (Exception e)
            {
                FloatingButtonTimeOutMillisecond = 0;
                //if (!this.mView.GetAppLaunchSiteCoreDoneFlag())
                //{
                //    this.mView.SetDefaultAppLaunchImage();
                //}
                Utility.LoggingNonFatalError(e);
            }
        }

        public void OnGetPhoto(FloatingButtonModel item)
        {
            if (!IsOnGetPhotoRunning)
            {
                IsOnGetPhotoRunning = true;
                CancellationTokenSource token = new CancellationTokenSource();
                Bitmap imageCache = null;
                Stopwatch sw = Stopwatch.StartNew();
                _ = Task.Run(() =>
                {
                    try
                    {
                        //imageCache = ImageUtils.GetImageBitmapFromUrl(item.Image);  
                        imageCache = ImageUtils.GetImageBitmapFromUrlWithTimeOut(item.Image);
                        sw.Stop();
                        FloatingButtonTimeOutMillisecond = 0;

                        if (imageCache != null)
                        {
                            item.ImageBitmap = imageCache;
                            item.ImageB64 = BitmapToBase64(imageCache);
                            FloatingButtonEntity wtManager = new FloatingButtonEntity();
                            wtManager.DeleteTable();
                            wtManager.CreateTable();
                            FloatingButtonEntity newItem = new FloatingButtonEntity()
                            {
                                ID = item.ID,
                                Image = item.Image,
                                ImageB64 = item.ImageB64,
                                Title = item.Title,
                                Description = item.Description,
                                StartDateTime = item.StartDateTime,
                                EndDateTime = item.EndDateTime,
                                ShowForSeconds = item.ShowForSeconds
                            };
                            wtManager.InsertItem(newItem);
                            FloatingButtonUtils.SetFloatingButtonBitmap(item);
                            if (!this.mView.GetFloatingButtonSiteCoreDoneFlag())
                            {
                                this.mView.SetCustomFloatingButtonImage(item);
                                this.mView.PopulateFloatingButton(item);
                            }
                        }
                        else
                        {
                            //if (!this.mView.GetAppLaunchSiteCoreDoneFlag())
                            //{
                            //    this.mView.SetDefaultAppLaunchImage();
                            //}
                        }
                    }
                    catch (Exception e)
                    {
                        FloatingButtonTimeOutMillisecond = 0;
                        //if (!this.mView.GetAppLaunchSiteCoreDoneFlag())
                        //{
                        //    this.mView.SetDefaultAppLaunchImage();
                        //}
                        Utility.LoggingNonFatalError(e);
                    }
                }, token.Token);

                if (FloatingButtonTimeOutMillisecond > 0)
                {
                    _ = Task.Delay(FloatingButtonTimeOutMillisecond).ContinueWith(_ =>
                    {
                        if (FloatingButtonTimeOutMillisecond > 0)
                        {
                            FloatingButtonTimeOutMillisecond = 0;
                            //if (!this.mView.GetAppLaunchSiteCoreDoneFlag())
                            //{
                            //    this.mView.SetDefaultAppLaunchImage();
                            //}
                        }
                    });
                }
            }
        }

        public string BitmapToBase64(Bitmap bitmap)
        {
            string B64Output = "";
            try
            {
                MemoryStream byteArrayOutputStream = new MemoryStream();
                bitmap.Compress(Bitmap.CompressFormat.Png, 100, byteArrayOutputStream);
                byte[] byteArray = byteArrayOutputStream.ToArray();
                B64Output = Base64.EncodeToString(byteArray, Base64Flags.Default);
            }
            catch (Exception e)
            {
                B64Output = "";
                Utility.LoggingNonFatalError(e);
            }

            return B64Output;
        }

        public Bitmap Base64ToBitmap(string base64String)
        {
            Bitmap convertedBitmap = null;
            try
            {
                byte[] imageAsBytes = Base64.Decode(base64String, Base64Flags.Default);
                convertedBitmap = BitmapFactory.DecodeByteArray(imageAsBytes, 0, imageAsBytes.Length);
            }
            catch (Exception e)
            {
                convertedBitmap = null;
                Utility.LoggingNonFatalError(e);
            }

            return convertedBitmap;
        }
    }

}
