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

		public DashboardHomePresenter(DashboardHomeContract.IView mView, ISharedPreferences preferences)
		{
			this.mView = mView;
			this.mSharedPref = preferences;
			this.mView?.SetPresenter(this);
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
                        this.mView.SetToolbarTitle(Resource.String.bill_menu_activity_title);

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
                    WeblinkEntity weblinkEntity = WeblinkEntity.GetByCode("PROMO");
					if (weblinkEntity != null)
					{
                        currentBottomNavigationMenu = Resource.Id.menu_promotion;
						this.mView.HideAccountName();
						this.mView.SetToolbarTitle(Resource.String.promotion_menu_activity_title);
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
                    OnUpdatePromoUnRead();
                    currentBottomNavigationMenu = Resource.Id.menu_more;
					this.mView.HideAccountName();
                    this.mView.SetToolbarTitle(Resource.String.more_menu_activity_title);
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

		public void Start()
		{

			if (LaunchViewActivity.MAKE_INITIAL_CALL)
			{
				new UserNotificationAPI(this).ExecuteOnExecutor(AsyncTask.ThreadPoolExecutor, "");
				new SiteCorePromotioAPI(mView).ExecuteOnExecutor(AsyncTask.ThreadPoolExecutor, "");
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

		public void OnNotificationCount()
		{
			this.mView.ShowNotificationCount(UserNotificationEntity.Count());
		}

        private void PreNavigateBllMenu(CustomerBillingAccount selectedAccount)
        {
            try
            {
                AccountData accountData = AccountData.Copy(selectedAccount, true);
                this.mView.SetAccountName(selectedAccount.AccDesc);
                this.mView.SetToolbarTitle(Resource.String.bill_menu_activity_title);
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

        private void OnResumeUpdateRewardUnRead()
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
					GetItemsService getItemsService = new GetItemsService(SiteCoreConfig.OS, density, SiteCoreConfig.SITECORE_URL, SiteCoreConfig.DEFAULT_LANGUAGE);
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
					GetItemsService getItemsService = new GetItemsService(SiteCoreConfig.OS, density, SiteCoreConfig.SITECORE_URL, SiteCoreConfig.DEFAULT_LANGUAGE);
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
                this.mView.SetToolbarTitle(Resource.String.bill_menu_activity_title);

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

    }

}
