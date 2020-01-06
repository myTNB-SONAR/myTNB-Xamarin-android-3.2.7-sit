using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using myTNB.SitecoreCMS.Model;
using myTNB.SitecoreCMS.Services;
using myTNB.SQLite.SQLiteDataManager;
using myTNB_Android.Src.AddAccount.Models;
using myTNB_Android.Src.AppLaunch.Activity;
using myTNB_Android.Src.AppLaunch.Models;
using myTNB_Android.Src.Base;
using myTNB_Android.Src.Base.Models;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.myTNBMenu.Activity;
using myTNB_Android.Src.myTNBMenu.Api;
using myTNB_Android.Src.myTNBMenu.Async;
using myTNB_Android.Src.myTNBMenu.Fragments;
using myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.MVP;
using myTNB_Android.Src.myTNBMenu.Fragments.RewardMenu.Api;
using myTNB_Android.Src.myTNBMenu.Fragments.RewardMenu.Model;
using myTNB_Android.Src.myTNBMenu.Fragments.RewardMenu.Request;
using myTNB_Android.Src.myTNBMenu.Fragments.RewardMenu.Response;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.myTNBMenu.Requests;
using myTNB_Android.Src.MyTNBService.Notification;
using myTNB_Android.Src.SiteCore;
using myTNB_Android.Src.SSMR.SMRApplication.MVP;
using myTNB_Android.Src.SummaryDashBoard;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;
using Refit;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace myTNB_Android.Src.myTNBMenu.MVP
{
	public class DashboardHomePresenter : DashboardHomeContract.IUserActionsListener
	{
		internal readonly string TAG = typeof(DashboardHomePresenter).Name;

		CancellationTokenSource cts;



		private DashboardHomeContract.IView mView;
		private ISharedPreferences mSharedPref;

		internal int currentBottomNavigationMenu = Resource.Id.menu_dashboard;

		private bool smDataError = false;
		private string smErrorCode = "204";
		private string smErrorMessage = "Sorry, Something went wrong. Please try again later";

		private string preSelectedAccount;
		private UsageHistoryResponse usageHistoryResponse;
        private SMUsageHistoryResponse smUsageHistoryResponse;

        private bool isBillAvailable = true;

        private static bool isPromoClicked = false;

        private RewardServiceImpl mApi;

        private List<AddUpdateRewardModel> userList = new List<AddUpdateRewardModel>();

        private RewardsCategoryEntity mRewardsCategoryEntity;

        private RewardsEntity mRewardsEntity;

        private static SSMRMeterReadingScreensParentEntity SSMRMeterReadingScreensParentManager;
        private static SSMRMeterReadingScreensEntity SSMRMeterReadingScreensManager;
        private static SSMRMeterReadingThreePhaseScreensParentEntity SSMRMeterReadingThreePhaseScreensParentManager;
        private static SSMRMeterReadingThreePhaseScreensEntity SSMRMeterReadingThreePhaseScreensManager;
        private static SSMRMeterReadingScreensOCROffParentEntity SSMRMeterReadingScreensOCROffParentManager;
        private static SSMRMeterReadingScreensOCROffEntity SSMRMeterReadingScreensOCROffManager;
        private static SSMRMeterReadingThreePhaseScreensOCROffParentEntity SSMRMeterReadingThreePhaseScreensOCROffParentManager;
        private static SSMRMeterReadingThreePhaseScreensOCROffEntity SSMRMeterReadingThreePhaseScreensOCROffManager;

        public DashboardHomePresenter(DashboardHomeContract.IView mView, ISharedPreferences preferences)
		{
			this.mView = mView;
			this.mSharedPref = preferences;
			this.mView?.SetPresenter(this);

            this.mApi = new RewardServiceImpl();
        }

		public void Logout()
		{
			UserEntity.RemoveActive();
			UserRegister.RemoveActive();
			CustomerBillingAccount.RemoveActive();
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
                            if (selectedAccount.AccountCategoryId.Equals("2"))
                            {
                                this.mView.SetToolbarTitle(Resource.String.dashboard_chartview_re_activity_title);
                            }
                            else
                            {
                                this.mView.SetToolbarTitle(Resource.String.dashboard_chartview_activity_title);
                            }
                            this.mView.SetAccountToolbarTitle(selectedAccount.AccDesc);
                            this.mView.HideAccountName();
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
                            if (selectedAccount != null)
                            {
                                List<CustomerBillingAccount> accountList = CustomerBillingAccount.List();
                                bool enableDropDown = accountList.Count > 0 ? true : false;

                                if (selectedAccount.AccountCategoryId.Equals("2"))
                                {
                                    this.mView.ShowREAccount(enableDropDown);
                                }
                                else
                                {
                                    this.mView.EnableDropDown(enableDropDown);
                                }
                                this.mView.SetAccountName(selectedAccount.AccDesc);
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
                            this.mView.ShowBillMenu(accountData);
                        }
					}
					// NO INTERNET RESPONSE
					else if (resultCode == Result.FirstUser)
					{
						Bundle extras = data.Extras;
						if (extras.ContainsKey(Constants.ITEMZIED_BILLING_VIEW_KEY) && extras.GetBoolean(Constants.ITEMZIED_BILLING_VIEW_KEY))
						{
							AccountData selectedAccount = JsonConvert.DeserializeObject<AccountData>(extras.GetString(Constants.SELECTED_ACCOUNT));
							bool isOwned = true;
							CustomerBillingAccount customerBillingAccount = CustomerBillingAccount.FindByAccNum(selectedAccount.AccountNum);
							if (customerBillingAccount != null)
							{
								isOwned = customerBillingAccount.isOwned;
								selectedAccount.IsOwner = isOwned;
								selectedAccount.AccountCategoryId = customerBillingAccount.AccountCategoryId;

							}
							try
							{
                                this.mView.ShowAccountName();
                                this.mView.BillsMenuAccess(selectedAccount);
							}
							catch (System.Exception e)
							{
								Utility.LoggingNonFatalError(e);
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

		public void OnMenuSelect(int resourceId)
		{
			ServicePointManager.ServerCertificateValidationCallback += SSLFactoryHelper.CertificateValidationCallBack;

			List<CustomerBillingAccount> accountList = CustomerBillingAccount.List();

			if (cts != null && cts.Token.CanBeCanceled)
			{
				this.cts.Cancel();
			}

			switch (resourceId)
			{
				case Resource.Id.menu_dashboard:
                    OnUpdatePromoUnRead();
                    if (DashboardHomeActivity.currentFragment != null && (DashboardHomeActivity.currentFragment.GetType() == typeof(HomeMenuFragment) ||
						DashboardHomeActivity.currentFragment.GetType() == typeof(DashboardChartFragment)))
					{
						mView.ShowBackButton(false);
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
							mView.ShowBackButton(false);
							DoLoadHomeDashBoardFragment();
						}

					}

                    OnUpdateRewardUnRead();
                    break;
				case Resource.Id.menu_bill:
                    OnUpdatePromoUnRead();
                    this.mView.RemoveHeaderDropDown();
                    if (accountList.Count > 0)
					{
                        CustomerBillingAccount selected;
						if (CustomerBillingAccount.HasSelected())
						{
							selected = CustomerBillingAccount.GetSelected();
                            PreNavigateBllMenu(selected);
                            this.mView.SetAccountName(selected.AccDesc);
                        }
						else
						{
							CustomerBillingAccount.SetSelected(accountList[0].AccNum);
							selected = accountList[0];
                            PreNavigateBllMenu(selected);
                            this.mView.SetAccountName(accountList[0].AccDesc);
                        }
                        if (selected != null)
                        {
                            List<CustomerBillingAccount> list = CustomerBillingAccount.List();
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
                        this.mView.ShowHideActionBar(true);
                        //this.mView.SetToolbarTitle(Resource.String.bill_menu_activity_title);

                        AccountData accountData = new AccountData();
                        CustomerBillingAccount customerBillingAccount = CustomerBillingAccount.FindByAccNum(selected.AccNum);
                        accountData.AccountNickName = selected.AccDesc;
                        accountData.AccountName = selected.OwnerName;
                        accountData.AddStreet = selected.AccountStAddress;
                        accountData.IsOwner = customerBillingAccount.isOwned;
                        accountData.AccountNum = selected.AccNum;
                        accountData.AccountCategoryId = customerBillingAccount.AccountCategoryId;
                        this.mView.ShowBillMenu(accountData);
                    }
					else
					{
                        this.mView.DisableBillMenu();
                    }

                    OnUpdateRewardUnRead();
                    break;
				case Resource.Id.menu_promotion:
                    this.mView.RemoveHeaderDropDown();
                    WeblinkEntity weblinkEntity = WeblinkEntity.GetByCode("PROMO");
					if (weblinkEntity != null)
					{
                        currentBottomNavigationMenu = Resource.Id.menu_promotion;
						this.mView.HideAccountName();
						//this.mView.SetToolbarTitle(Utility.GetLocalizedLabel("Tabbar", "promotion"));
						this.mView.ShowPromotionsMenu(Weblink.Copy(weblinkEntity));
                    }

                    isPromoClicked = true;

                    if (this.mView.IsActive())
					{
						if (PromotionsEntityV2.HasUnread())
						{
							this.mView.ShowUnreadPromotions(true);

						}
						else
						{
							this.mView.HideUnreadPromotions(true);

						}
					}
                    OnUpdateRewardUnRead();
                    break;
				case Resource.Id.menu_reward:
                    OnUpdatePromoUnRead();
                    currentBottomNavigationMenu = Resource.Id.menu_reward;
                    this.mView.HideAccountName();
                    this.mView.SetToolbarTitle(Resource.String.reward_menu_activity_title);
                    this.mView.ShowRewardsMenu();

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
                    this.mView.RemoveHeaderDropDown();
                    OnUpdatePromoUnRead();
                    currentBottomNavigationMenu = Resource.Id.menu_more;
					this.mView.HideAccountName();
                    OnUpdateRewardUnRead();
                    this.mView.ShowMoreMenu();
					break;
			}
		}

		public void DoLoadHomeDashBoardFragment()
		{
			this.mView.ShowHomeDashBoard();
			currentBottomNavigationMenu = Resource.Id.menu_dashboard;
			this.mView.SetToolbarTitle(Resource.String.dashboard_activity_title);
			this.mView.EnableDropDown(false);
			this.mView.HideAccountName();
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

        public void OnResetRewardPromotionThread()
        {
            try
            {
                WhatNewMenuUtils.OnSetWhatNewLoading(true);
                new SiteCorePromotioAPI(mView).ExecuteOnExecutor(AsyncTask.ThreadPoolExecutor, "");
                RewardsMenuUtils.OnSetRewardLoading(true);
                new SitecoreRewardAPI(mView).ExecuteOnExecutor(AsyncTask.ThreadPoolExecutor, "");
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

		public void Start()
		{

			if (LaunchViewActivity.MAKE_INITIAL_CALL)
			{
                WhatNewMenuUtils.OnSetWhatNewLoading(true);
                new SiteCorePromotioAPI(mView).ExecuteOnExecutor(AsyncTask.ThreadPoolExecutor, "");
                RewardsMenuUtils.OnSetRewardLoading(true);
                new SitecoreRewardAPI(mView).ExecuteOnExecutor(AsyncTask.ThreadPoolExecutor, "");
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

            this.mView.OnDataSchemeShow();


		}

        private void LoadUsageHistory(CustomerBillingAccount accountSelected)
		{
            try
            {
                this.mView.HideAccountName();
                if (accountSelected.AccountCategoryId.Equals("2"))
                {
                    this.mView.SetToolbarTitle(Resource.String.dashboard_chartview_re_activity_title);
                }
                else
                {
                    this.mView.SetToolbarTitle(Resource.String.dashboard_chartview_activity_title);
                }

                this.mView.SetAccountToolbarTitle(accountSelected.AccDesc);

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
                this.mView.HideAccountName();
                if (accountSelected.AccountCategoryId.Equals("2"))
                {
                    this.mView.SetToolbarTitle(Resource.String.dashboard_chartview_re_activity_title);
                }
                else
                {
                    this.mView.SetToolbarTitle(Resource.String.dashboard_chartview_activity_title);
                }

                this.mView.SetAccountToolbarTitle(accountSelected.AccDesc);

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
                Utility.LoggingNonFatalError(e);
            }

        }


		private async void LoadSMUsageHistory(CustomerBillingAccount accountSelected)
		{
            try
            {
                this.mView.HideAccountName();
                this.mView.SetToolbarTitle(Resource.String.dashboard_chartview_activity_title);

                this.mView.ShowSMChart(smUsageHistoryResponse, AccountData.Copy(accountSelected, true));
                smUsageHistoryResponse = null;

				this.mView.SetAccountToolbarTitle(accountSelected.AccDesc);
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
                //this.mView.SetToolbarTitle(Resource.String.bill_menu_activity_title);
                currentBottomNavigationMenu = Resource.Id.menu_bill;
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        private void OnUpdatePromoUnRead()
        {
            if (isPromoClicked && !UserSessions.HasWhatNewShown(mSharedPref))
            {
                UserSessions.DoWhatNewShown(mSharedPref);
            }

            if (PromotionsEntityV2.HasUnread())
            {
                this.mView.ShowUnreadPromotions(false);

            }
            else
            {
                this.mView.HideUnreadPromotions(false);

            }

            isPromoClicked = false;
        }

        public void OnResumeUpdatePromotionUnReadCounter()
        {
            if (PromotionsEntityV2.HasUnread())
            {
                this.mView.ShowUnreadPromotions();

            }
            else
            {
                this.mView.HideUnreadPromotions();

            }
        }

        private void OnResumeUpdatePromotionUnRead()
        {
            if (isPromoClicked && !UserSessions.HasWhatNewShown(mSharedPref))
            {
                UserSessions.DoWhatNewShown(mSharedPref);
            }

            if (PromotionsEntityV2.HasUnread())
            {
                this.mView.ShowUnreadPromotions();

            }
            else
            {
                this.mView.HideUnreadPromotions();

            }

            isPromoClicked = false;
        }

        private void OnUpdateRewardUnRead()
        {
            if (RewardsEntity.HasUnread())
            {
                this.mView.ShowUnreadRewards(false);

            }
            else
            {
                this.mView.HideUnreadRewards(false);

            }
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

        public void OnValidateData()
		{
            OnResumeUpdatePromotionUnRead();
            OnResumeUpdateRewardUnRead();

            List<CustomerBillingAccount> accountList = CustomerBillingAccount.List();
            if(accountList.Count == 0)
            {
                this.mView.DisableBillMenu();
            }
        }

		public Task OnGetPromotionsTimeStamp()
		{
			cts = new CancellationTokenSource();
			PromotionsEntityV2 wtManager1 = new PromotionsEntityV2();
			PromotionsParentEntityV2 wtManger12 = new PromotionsParentEntityV2();
			return Task.Factory.StartNew(() =>
			{
				try
				{
					string density = DPUtils.GetDeviceDensity(Application.Context);
					GetItemsService getItemsService = new GetItemsService(SiteCoreConfig.OS, density, SiteCoreConfig.SITECORE_URL, LanguageUtil.GetAppLanguage());
					string json = getItemsService.GetPromotionsV2TimestampItem();
					PromotionsParentV2ResponseModel responseModel = JsonConvert.DeserializeObject<PromotionsParentV2ResponseModel>(json);
					if (responseModel.Status.Equals("Success"))
					{
						PromotionsParentEntityV2 wtManager = new PromotionsParentEntityV2();
						wtManager.DeleteTable();
						wtManager.CreateTable();
						wtManager.InsertListOfItems(responseModel.Data);
						mView.ShowPromotionTimestamp(true);
					}
					else
					{
						mView.ShowPromotionTimestamp(false);
					}
				}
				catch (System.Exception e)
				{
					mView.ShowPromotionTimestamp(false);
					Utility.LoggingNonFatalError(e);
				}
			}).ContinueWith((Task previous) =>
			{
			}, cts.Token);
		}

		public Task OnGetPromotions()
		{
			cts = new CancellationTokenSource();
			return Task.Factory.StartNew(() =>
			{
				try
				{
					string density = DPUtils.GetDeviceDensity(Application.Context);
					GetItemsService getItemsService = new GetItemsService(SiteCoreConfig.OS, density, SiteCoreConfig.SITECORE_URL, LanguageUtil.GetAppLanguage());
					string json = getItemsService.GetPromotionsV2Item();
					PromotionsV2ResponseModel responseModel = JsonConvert.DeserializeObject<PromotionsV2ResponseModel>(json);
					if (responseModel.Status.Equals("Success"))
					{
						PromotionsEntityV2 wtManager = new PromotionsEntityV2();
						wtManager.DeleteTable();
						wtManager.CreateTable();
						wtManager.InsertListOfItems(responseModel.Data);
						mView.ShowPromotion(true);
					}
					else
					{
						mView.ShowPromotion(false);
					}
				}
				catch (System.Exception e)
				{
					mView.ShowPromotion(true);
					Utility.LoggingNonFatalError(e);
				}
			}).ContinueWith((Task previous) =>
			{
			}, cts.Token);
		}

		public void GetSavedPromotionTimeStamp()
		{
			try
			{
				PromotionsParentEntityV2 wtManager = new PromotionsParentEntityV2();
				List<PromotionsParentEntityV2> items = wtManager.GetAllItems();
				if (items != null)
				{
					PromotionsParentEntityV2 entity = items[0];
					if (entity != null)
					{
						mView.OnSavedTimeStamp(entity.Timestamp);
					}
				}
				else
				{
					mView.OnSavedTimeStamp(null);
				}
			}
			catch (System.Exception e)
			{
				mView.OnSavedTimeStamp(null);
				Utility.LoggingNonFatalError(e);
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

							if (selected.AccountCategoryId.Equals("2"))
							{
								this.mView.ShowREAccount(true);
							}
							else
							{
								this.mView.EnableDropDown(true);
							}
							if (!string.IsNullOrEmpty(selected.AccDesc))
							{
								this.mView.SetAccountName(selected.AccDesc);
							}
						}
                        //this.mView.ShowAccountName();
                        //this.mView.SetToolbarTitle(Resource.String.dashboard_menu_activity_title);
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

								if (selected.AccountCategoryId.Equals("2"))
								{
									this.mView.ShowREAccount(true);
								}
								else
								{
									this.mView.EnableDropDown(true);
								}
								if (!string.IsNullOrEmpty(selected.AccDesc))
								{
									this.mView.SetAccountName(selected.AccDesc);
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

        public void BillMenuStartRefresh()
        {
            ServicePointManager.ServerCertificateValidationCallback += SSLFactoryHelper.CertificateValidationCallBack;

            List<CustomerBillingAccount> accountList = CustomerBillingAccount.List();
            if (accountList.Count > 0)
            {
                CustomerBillingAccount selected;
                if (CustomerBillingAccount.HasSelected())
                {
                    selected = CustomerBillingAccount.GetSelected();
                    PreNavigateBllMenu(selected);
                    this.mView.SetAccountName(selected.AccDesc);
                }
                else
                {
                    CustomerBillingAccount.SetSelected(accountList[0].AccNum);
                    selected = accountList[0];
                    PreNavigateBllMenu(selected);
                    this.mView.SetAccountName(accountList[0].AccDesc);
                }
                if (selected != null)
                {
                    List<CustomerBillingAccount> list = CustomerBillingAccount.List();
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
                this.mView.ShowHideActionBar(true);
                //this.mView.SetToolbarTitle(Resource.String.bill_menu_activity_title);

                AccountData accountData = new AccountData();
                CustomerBillingAccount customerBillingAccount = CustomerBillingAccount.FindByAccNum(selected.AccNum);
                accountData.AccountNickName = selected.AccDesc;
                accountData.AccountName = selected.OwnerName;
                accountData.AccountNum = selected.AccNum;
                accountData.AddStreet = selected.AccountStAddress;
                accountData.IsOwner = customerBillingAccount.isOwned;
                accountData.AccountCategoryId = customerBillingAccount.AccountCategoryId;
                this.mView.ShowBillMenu(accountData);

            }
            else
            {
                this.mView.DisableBillMenu();
            }
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
                for(int i = 0; i < data.ByMonth.Months.Count; i++)
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
            }

            this.mView.OnCheckRewardTab();
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

    }

}
