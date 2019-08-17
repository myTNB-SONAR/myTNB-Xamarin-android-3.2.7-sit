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
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.myTNBMenu.Activity;
using myTNB_Android.Src.myTNBMenu.Api;
using myTNB_Android.Src.myTNBMenu.Async;
using myTNB_Android.Src.myTNBMenu.Fragments;
using myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.MVP;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.SiteCore;
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

							if (selectedAccount != null && selectedAccount.SmartMeterCode != null && selectedAccount.SmartMeterCode.Equals("0"))
							{
                                if (selectedAccount.AccountCategoryId.Equals("2"))
                                {
                                    this.mView.SetToolbarTitle(Resource.String.dashboard_chartview_re_activity_title);
                                }
                                else
                                {
                                    this.mView.SetToolbarTitle(Resource.String.dashboard_chartview_activity_title);
                                }
                                this.mView.HideAccountName();
                                if (!string.IsNullOrEmpty(selectedAccount.AccNum) && !UsageHistoryEntity.IsSMDataUpdated(selectedAccount.AccNum))
                                {
                                    UsageHistoryEntity storedEntity = new UsageHistoryEntity();
                                    storedEntity = UsageHistoryEntity.GetItemByAccountNo(selectedAccount.AccNum);
                                    if (storedEntity != null)
                                    {
                                        CustomerBillingAccount.RemoveSelected();
                                        if (!string.IsNullOrEmpty(selectedAccount.AccNum))
                                        {
                                            CustomerBillingAccount.Update(selectedAccount.AccNum, true);
                                        }
                                        usageHistoryResponse = JsonConvert.DeserializeObject<UsageHistoryResponse>(storedEntity.JsonResponse);
                                        if ((usageHistoryResponse != null && usageHistoryResponse.Data != null && usageHistoryResponse.Data.Status.ToUpper() == Constants.REFRESH_MODE) || !(usageHistoryResponse != null && usageHistoryResponse.Data.Status.Equals("success") && !usageHistoryResponse.Data.IsError))
                                        {
                                            usageHistoryResponse = null;
                                        }
                                        LoadUsageHistory(selectedAccount);
                                    }
                                    else
                                    {
                                        LoadUsageHistory(selectedAccount);
                                    }
                                }
                                else
                                {
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
                                    if (storedEntity != null)
                                    {
                                        storedSMData = JsonConvert.DeserializeObject<SMUsageHistoryResponse>(storedEntity.JsonResponse);
                                    }
                                    if (storedSMData != null && storedSMData.Data != null && storedSMData.Data.SMUsageHistoryData != null)
                                    {
                                        this.mView.ShowAccountName();
                                        this.mView.SetToolbarTitle(Resource.String.dashboard_menu_activity_title);
                                        this.mView.ShowSMChart(storedSMData.Data.SMUsageHistoryData, AccountData.Copy(selectedAccount, true));
                                    }
                                    else
                                    {
                                        LoadSMUsageHistory(selectedAccount);
                                    }
                                }
                                else
                                {
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
                            PreNavigateBllMenu(selectedAccount);
                            this.mView.SetAccountName(selectedAccount.AccDesc);
                            if (selectedAccount != null)
                            {
                                List<CustomerBillingAccount> list = CustomerBillingAccount.List();
                                bool enableDropDown = list.Count > 0 ? true : false;

                                if (selectedAccount.AccountCategoryId.Equals("2"))
                                {
                                    this.mView.ShowREAccount(enableDropDown);
                                }
                                else
                                {
                                    this.mView.EnableDropDown(enableDropDown);
                                }
                            }
                            LoadBills(selectedAccount);
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
						else if (extras.ContainsKey(Constants.REFRESH_MODE) && extras.GetBoolean(Constants.REFRESH_MODE))
						{
							CustomerBillingAccount customerBillingAccount = CustomerBillingAccount.GetSelectedOrFirst();
							if (currentBottomNavigationMenu == Resource.Id.menu_bill)
							{
								CustomerBillingAccount selectedCustomerAccount = CustomerBillingAccount.GetSelectedOrFirst();
								AccountData selectedAccount = AccountData.Copy(selectedCustomerAccount, true);
								bool isOwned = true;
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
						else if (CustomerBillingAccount.HasSelected())
						{
							CustomerBillingAccount customerBillingAccount = CustomerBillingAccount.GetSelectedOrFirst();
							if (currentBottomNavigationMenu == Resource.Id.menu_dashboard)
							{
                                if (customerBillingAccount != null && !customerBillingAccount.isOwned)
								{
									CustomerBillingAccount selected = CustomerBillingAccount.GetSelected();
                                    this.mView.HideAccountName();
                                }
								else
								{
									CustomerBillingAccount selected = CustomerBillingAccount.GetSelected();
                                    this.mView.HideAccountName();
                                    if (selected.AccountCategoryId.Equals("2"))
                                    {
                                        this.mView.SetToolbarTitle(Resource.String.dashboard_chartview_re_activity_title);
                                    }
                                    else
                                    {
                                        this.mView.SetToolbarTitle(Resource.String.dashboard_chartview_activity_title);
                                    }
                                    if (selected != null && !string.IsNullOrEmpty(selected.AccNum))
									{
										this.mView.ShowOwnerDashboardNoInternetConnection(selected.AccDesc, null, AccountData.Copy(selected, true));
									}
									else
									{
										this.mView.ShowOwnerDashboardNoInternetConnection(selected.AccDesc, null, null);
									}
								}
							}
							else if (currentBottomNavigationMenu == Resource.Id.menu_bill)
							{
                                if (customerBillingAccount != null && !customerBillingAccount.isOwned)
								{
									CustomerBillingAccount selected = CustomerBillingAccount.GetSelected();

									this.mView.ShowAccountName();
									this.mView.ShowBillMenu(AccountData.Copy(selected, true));
								}
								else
								{
									CustomerBillingAccount selected = CustomerBillingAccount.GetSelected();
									this.mView.ShowAccountName();
									this.mView.ShowOwnerBillsNoInternetConnection(AccountData.Copy(selected, true));
								}
							}


							if (customerBillingAccount != null)
							{
								List<CustomerBillingAccount> accountList = CustomerBillingAccount.List();
								bool enableDropDown = accountList.Count > 0 ? true : false;
								if (customerBillingAccount.AccountCategoryId.Equals("2"))
								{
									this.mView.ShowREAccount(enableDropDown);
								}
								else
								{
									this.mView.EnableDropDown(enableDropDown);
								}
							}


						}
						else
						{
							CustomerBillingAccount selected = CustomerBillingAccount.GetSelected();
                            this.mView.HideAccountName();
                            this.mView.SetToolbarTitle(Resource.String.dashboard_chartview_activity_title);
                            if (selected != null && !string.IsNullOrEmpty(selected.AccNum))
							{
								this.mView.ShowOwnerDashboardNoInternetConnection(selected.AccDesc, null, AccountData.Copy(selected, true));
							}
							else
							{
								this.mView.ShowOwnerDashboardNoInternetConnection(selected.AccDesc, null, null);
							}

							List<CustomerBillingAccount> accountList = CustomerBillingAccount.List();
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
					if (DashboardHomeActivity.currentFragment != null && (DashboardHomeActivity.currentFragment.GetType() == typeof(HomeMenuFragment) ||
						DashboardHomeActivity.currentFragment.GetType() == typeof(DashboardChartFragment) ||
						DashboardHomeActivity.currentFragment.GetType() == typeof(DashboardChartNoTNBAccount) ||
						DashboardHomeActivity.currentFragment.GetType() == typeof(DashboardChartNonOwnerNoAccess) ||
						DashboardHomeActivity.currentFragment.GetType() == typeof(DashboardSmartMeterFragment)))
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
					break;
				case Resource.Id.menu_bill:
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
                        LoadBills(selected);

                    }
					else
					{
                        this.mView.DisableBillMenu();
                    }


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

					if (this.mView.IsActive())
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
					break;
				case Resource.Id.menu_reward:
                    currentBottomNavigationMenu = Resource.Id.menu_reward;
                    this.mView.ShowToBeAddedToast();
					break;
				case Resource.Id.menu_feedback:
                    currentBottomNavigationMenu = Resource.Id.menu_feedback;
					this.mView.HideAccountName();
                    this.mView.SetToolbarTitle(Resource.String.feedback_menu_activity_title);
					this.mView.ShowFeedbackMenu();
					break;
				case Resource.Id.menu_more:
                    currentBottomNavigationMenu = Resource.Id.menu_more;
					this.mView.HideAccountName();
                    this.mView.SetToolbarTitle(Resource.String.more_menu_activity_title);
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
				new UserNotificationAPI(mView.GetDeviceId(), this).ExecuteOnExecutor(AsyncTask.ThreadPoolExecutor, "");
				new SiteCorePromotioAPI(mView).ExecuteOnExecutor(AsyncTask.ThreadPoolExecutor, "");
				LaunchViewActivity.MAKE_INITIAL_CALL = false;
			}

			if (currentBottomNavigationMenu == Resource.Id.menu_promotion || currentBottomNavigationMenu == Resource.Id.menu_feedback || currentBottomNavigationMenu == Resource.Id.menu_reward || currentBottomNavigationMenu == Resource.Id.menu_more)
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


		private async void LoadUsageHistory(CustomerBillingAccount accountSelected)
		{
			try
			{
				cts = new CancellationTokenSource();
				this.mView.ShowProgressDialog();
				ServicePointManager.ServerCertificateValidationCallback += SSLFactoryHelper.CertificateValidationCallBack;
#if DEBUG
                var httpClient = new HttpClient(new HttpLoggingHandler(/*new NativeMessageHandler()*/)) { BaseAddress = new Uri(Constants.SERVER_URL.END_POINT) };
                var amountDueApi = RestService.For<IAmountDueApi>(httpClient);
                var api = RestService.For<IUsageHistoryApi>(httpClient);

#else
				var amountDueApi = RestService.For<IAmountDueApi>(Constants.SERVER_URL.END_POINT);
				var api = RestService.For<IUsageHistoryApi>(Constants.SERVER_URL.END_POINT);
#endif

				var amountDueResponse = await amountDueApi.GetAccountDueAmount(new Requests.AccountDueAmountRequest()
				{
					ApiKeyId = Constants.APP_CONFIG.API_KEY_ID,
					AccNum = accountSelected.AccNum

				}, cts.Token);

				if (amountDueResponse != null && amountDueResponse.Data != null && amountDueResponse.Data.Status.ToUpper() == Constants.REFRESH_MODE)
				{
					if (this.mView.IsActive())
					{
						this.mView.HideProgressDialog();
					}
                    this.mView.HideAccountName();
                    if (accountSelected.AccountCategoryId.Equals("2"))
                    {
                        this.mView.SetToolbarTitle(Resource.String.dashboard_chartview_re_activity_title);
                    }
                    else
                    {
                        this.mView.SetToolbarTitle(Resource.String.dashboard_chartview_activity_title);
                    }
                    this.mView.ShowOwnerDashboardNoInternetConnection(accountSelected.AccDesc, true, amountDueResponse.Data.RefreshMessage, amountDueResponse.Data.RefreshBtnText, AccountData.Copy(accountSelected, true));
				}
				else if (!amountDueResponse.Data.IsError)
				{
					cts = new CancellationTokenSource();

					try
					{
						if (usageHistoryResponse == null)
						{
							usageHistoryResponse = await api.DoQuery(new Requests.UsageHistoryRequest(Constants.APP_CONFIG.API_KEY_ID)
							{
								AccountNum = accountSelected.AccNum
							}, cts.Token);
						}

						if (usageHistoryResponse != null && usageHistoryResponse.Data != null && usageHistoryResponse.Data.Status.ToUpper() == Constants.REFRESH_MODE)
						{
							if (this.mView.IsActive())
							{
								this.mView.HideProgressDialog();
							}
                            this.mView.HideAccountName();
                            if (accountSelected.AccountCategoryId.Equals("2"))
                            {
                                this.mView.SetToolbarTitle(Resource.String.dashboard_chartview_re_activity_title);
                            }
                            else
                            {
                                this.mView.SetToolbarTitle(Resource.String.dashboard_chartview_activity_title);
                            }
                            this.mView.ShowOwnerDashboardNoInternetConnection(accountSelected.AccDesc, usageHistoryResponse, AccountData.Copy(accountSelected, true), amountDueResponse);
							usageHistoryResponse = null;
						}
						else if (usageHistoryResponse != null && usageHistoryResponse.Data.Status.Equals("success") && !usageHistoryResponse.Data.IsError)
						{
							if (this.mView.IsActive())
							{
								this.mView.HideProgressDialog();
							}

							UsageHistoryEntity smUsageModel = new UsageHistoryEntity();
							smUsageModel.Timestamp = DateTime.Now.ToLocalTime();
							smUsageModel.JsonResponse = JsonConvert.SerializeObject(usageHistoryResponse);
							smUsageModel.AccountNo = accountSelected.AccNum;
							UsageHistoryEntity.InsertItem(smUsageModel);

							if (currentBottomNavigationMenu == Resource.Id.menu_dashboard)
							{
								if (smDataError)
								{
									smDataError = false;
									this.mView.ShowChartWithError(usageHistoryResponse.Data.UsageHistoryData, AccountData.Copy(accountSelected, true), smErrorCode, smErrorMessage, amountDueResponse);
								}
								else
								{
                                    this.mView.ShowChart(usageHistoryResponse.Data.UsageHistoryData, AccountData.Copy(accountSelected, true), amountDueResponse);
								}
								usageHistoryResponse = null;
                                this.mView.HideAccountName();
                                if (accountSelected.AccountCategoryId.Equals("2"))
                                {
                                    this.mView.SetToolbarTitle(Resource.String.dashboard_chartview_re_activity_title);
                                }
                                else
                                {
                                    this.mView.SetToolbarTitle(Resource.String.dashboard_chartview_activity_title);
                                }
                            }
							else if (currentBottomNavigationMenu == Resource.Id.menu_bill)
							{
								this.mView.ShowAccountName();
								this.mView.SetToolbarTitle(Resource.String.bill_menu_activity_title);
								LoadBills(accountSelected);
							}
							this.mView.SetAccountName(accountSelected.AccDesc);
						}
						else
						{
							if (this.mView.IsActive())
							{
								this.mView.HideProgressDialog();
							}
                            this.mView.HideAccountName();
                            if (accountSelected.AccountCategoryId.Equals("2"))
                            {
                                this.mView.SetToolbarTitle(Resource.String.dashboard_chartview_re_activity_title);
                            }
                            else
                            {
                                this.mView.SetToolbarTitle(Resource.String.dashboard_chartview_activity_title);
                            }
                            this.mView.ShowOwnerDashboardNoInternetConnection(accountSelected.AccDesc, null, AccountData.Copy(accountSelected, true), amountDueResponse);
							this.mView.SetAccountName(accountSelected.AccDesc);
							usageHistoryResponse = null;
						}
					}
					catch (System.OperationCanceledException e)
					{
						if (this.mView.IsActive())
						{
							this.mView.HideProgressDialog();
						}
                        this.mView.HideAccountName();
                        if (accountSelected.AccountCategoryId.Equals("2"))
                        {
                            this.mView.SetToolbarTitle(Resource.String.dashboard_chartview_re_activity_title);
                        }
                        else
                        {
                            this.mView.SetToolbarTitle(Resource.String.dashboard_chartview_activity_title);
                        }
                        this.mView.ShowOwnerDashboardNoInternetConnection(accountSelected.AccDesc, null, AccountData.Copy(accountSelected, true), amountDueResponse);
						usageHistoryResponse = null;
						Utility.LoggingNonFatalError(e);
					}
					catch (ApiException apiException)
					{
						if (this.mView.IsActive())
						{
							this.mView.HideProgressDialog();
						}
                        this.mView.HideAccountName();
                        if (accountSelected.AccountCategoryId.Equals("2"))
                        {
                            this.mView.SetToolbarTitle(Resource.String.dashboard_chartview_re_activity_title);
                        }
                        else
                        {
                            this.mView.SetToolbarTitle(Resource.String.dashboard_chartview_activity_title);
                        }
                        this.mView.ShowOwnerDashboardNoInternetConnection(accountSelected.AccDesc, null, AccountData.Copy(accountSelected, true), amountDueResponse);
						usageHistoryResponse = null;
						Utility.LoggingNonFatalError(apiException);
					}
					catch (System.Exception e)
					{
						if (this.mView.IsActive())
						{
							this.mView.HideProgressDialog();
						}
                        this.mView.HideAccountName();
                        if (accountSelected.AccountCategoryId.Equals("2"))
                        {
                            this.mView.SetToolbarTitle(Resource.String.dashboard_chartview_re_activity_title);
                        }
                        else
                        {
                            this.mView.SetToolbarTitle(Resource.String.dashboard_chartview_activity_title);
                        }
                        this.mView.ShowOwnerDashboardNoInternetConnection(accountSelected.AccDesc, null, AccountData.Copy(accountSelected, true), amountDueResponse);
						usageHistoryResponse = null;
						Utility.LoggingNonFatalError(e);
					}
				}
				else
				{
					if (this.mView.IsActive())
					{
						this.mView.HideProgressDialog();
					}
                    this.mView.HideAccountName();
                    if (accountSelected.AccountCategoryId.Equals("2"))
                    {
                        this.mView.SetToolbarTitle(Resource.String.dashboard_chartview_re_activity_title);
                    }
                    else
                    {
                        this.mView.SetToolbarTitle(Resource.String.dashboard_chartview_activity_title);
                    }
                    this.mView.ShowOwnerDashboardNoInternetConnection(accountSelected.AccDesc, true, null, null, AccountData.Copy(accountSelected, true));
				}
			}
			catch (System.OperationCanceledException e)
			{
				if (this.mView.IsActive())
				{
					this.mView.HideProgressDialog();
				}
                this.mView.HideAccountName();
                if (accountSelected.AccountCategoryId.Equals("2"))
                {
                    this.mView.SetToolbarTitle(Resource.String.dashboard_chartview_re_activity_title);
                }
                else
                {
                    this.mView.SetToolbarTitle(Resource.String.dashboard_chartview_activity_title);
                }
                this.mView.ShowOwnerDashboardNoInternetConnection(accountSelected.AccDesc, true, null, null, AccountData.Copy(accountSelected, true));
				Utility.LoggingNonFatalError(e);
			}
			catch (ApiException apiException)
			{
				// ADD HTTP CONNECTION EXCEPTION HERE
				if (this.mView.IsActive())
				{
					this.mView.HideProgressDialog();
				}
                this.mView.HideAccountName();
                if (accountSelected.AccountCategoryId.Equals("2"))
                {
                    this.mView.SetToolbarTitle(Resource.String.dashboard_chartview_re_activity_title);
                }
                else
                {
                    this.mView.SetToolbarTitle(Resource.String.dashboard_chartview_activity_title);
                }
                this.mView.ShowOwnerDashboardNoInternetConnection(accountSelected.AccDesc, true, null, null, AccountData.Copy(accountSelected, true));
				Utility.LoggingNonFatalError(apiException);
			}
			catch (Exception e)
			{
				// ADD UNKNOWN EXCEPTION HERE
				if (this.mView.IsActive())
				{
					this.mView.HideProgressDialog();
				}
                this.mView.HideAccountName();
                if (accountSelected.AccountCategoryId.Equals("2"))
                {
                    this.mView.SetToolbarTitle(Resource.String.dashboard_chartview_re_activity_title);
                }
                else
                {
                    this.mView.SetToolbarTitle(Resource.String.dashboard_chartview_activity_title);
                }
                this.mView.ShowOwnerDashboardNoInternetConnection(accountSelected.AccDesc, true, null, null, AccountData.Copy(accountSelected, true));
				Utility.LoggingNonFatalError(e);
			}

		}


		private async void LoadSMUsageHistory(CustomerBillingAccount accountSelected)
		{
			/*** Check for timestamp to call service ***/

			/****/

			cts = new CancellationTokenSource();
            this.mView.ShowProgressDialog();

#if STUB
            var api = RestService.For<IUsageHistoryApi>(Constants.SERVER_URL.END_POINT);
#elif DEBUG
            var httpClient = new HttpClient(new HttpLoggingHandler(/*new NativeMessageHandler()*/)) { BaseAddress = new Uri(Constants.SERVER_URL.END_POINT) };
            var api = RestService.For<IUsageHistoryApi>(httpClient);

#elif DEVELOP
            var api = RestService.For<IUsageHistoryApi>(Constants.SERVER_URL.END_POINT);
#else
			var api = RestService.For<IUsageHistoryApi>(Constants.SERVER_URL.END_POINT);
#endif



			try
			{
				var response = await api.DoSMQueryV2(new Requests.SMUsageHistoryRequest(Constants.APP_CONFIG.API_KEY_ID)
				{
					AccountNum = accountSelected.AccNum,
					isOwner = true,
					sspUserId = UserEntity.GetActive().UserID,
					userEmail = UserEntity.GetActive().Email,
					MeterCode = accountSelected.SmartMeterCode
				}, cts.Token);

				if (response != null && response.Data.Status.Equals("success") && !response.Data.IsError)
				{

                    if (this.mView.IsActive())
                    {
                        this.mView.HideProgressDialog();
                    }

                    if (!string.IsNullOrEmpty(response.Data.StatusCode) && response.Data.StatusCode.Equals("201"))
					{
                        ///No data condition
                        this.mView.ShowAccountName();
                        this.mView.ShowSMChartWithError(response.Data.SMUsageHistoryData, AccountData.Copy(accountSelected, true), true);
					}
					else
					{
						/*** Save SM Usage History For the Day***/
						SMUsageHistoryEntity smUsageModel = new SMUsageHistoryEntity();
						smUsageModel.Timestamp = DateTime.Now.ToLocalTime();
						smUsageModel.JsonResponse = JsonConvert.SerializeObject(response);
						smUsageModel.AccountNo = accountSelected.AccNum;
						SMUsageHistoryEntity.InsertItem(smUsageModel);
						/*****/

						if (currentBottomNavigationMenu == Resource.Id.menu_dashboard)
						{
							this.mView.ShowAccountName();
                            this.mView.SetToolbarTitle(Resource.String.dashboard_menu_activity_title);
                            this.mView.ShowSMChart(response.Data.SMUsageHistoryData, AccountData.Copy(accountSelected, true));
						}
						else if (currentBottomNavigationMenu == Resource.Id.menu_bill)
						{
							this.mView.ShowAccountName();
							this.mView.SetToolbarTitle(Resource.String.bill_menu_activity_title);
							LoadBills(accountSelected);
						}
					}


					this.mView.SetAccountName(accountSelected.AccDesc);
				}
				else
				{
                    ///On 204 (No Content) error display normal dashboard
                    if (this.mView.IsActive())
                    {
                        this.mView.HideProgressDialog();
                    }
                    smDataError = true;
					smErrorCode = response.Data.StatusCode;
					smErrorMessage = response.Data.Message;
					LoadUsageHistory(accountSelected);
				}
			}
			catch (System.OperationCanceledException e)
			{
                if (this.mView.IsActive())
                {
                    this.mView.HideProgressDialog();
                }
                // ADD OPERATION CANCELLED HERE
                smDataError = true;
				LoadUsageHistory(accountSelected);
				Utility.LoggingNonFatalError(e);
			}
			catch (ApiException apiException)
			{
                // ADD HTTP CONNECTION EXCEPTION HERE
                if (this.mView.IsActive())
                {
                    this.mView.HideProgressDialog();
                }
                smDataError = true;
				LoadUsageHistory(accountSelected);
				Utility.LoggingNonFatalError(apiException);
			}
			catch (System.Exception e)
			{
                // ADD UNKNOWN EXCEPTION HERE
                if (this.mView.IsActive())
                {
                    this.mView.HideProgressDialog();
                }
                smDataError = true;
				LoadUsageHistory(accountSelected);
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
            var detailedAccountApi = RestService.For<IDetailedCustomerAccount>(httpClient);
#elif DEVELOP
            var detailedAccountApi = RestService.For<IDetailedCustomerAccount>(Constants.SERVER_URL.END_POINT);
#else
			var detailedAccountApi = RestService.For<IDetailedCustomerAccount>(Constants.SERVER_URL.END_POINT);
#endif

			try
			{
                this.mView.ShowProgressDialog();
                AccountDetailsResponse customerBillingDetails = await detailedAccountApi.GetDetailedAccount(new AddAccount.Requests.AccountDetailsRequest()
				{
					apiKeyID = Constants.APP_CONFIG.API_KEY_ID,
					CANum = accountSelected.AccNum
				}, cts.Token);
				if (this.mView.IsActive())
				{
					this.mView.HideProgressDialog();
				}
				if (customerBillingDetails != null && customerBillingDetails.Data != null && customerBillingDetails.Data.Status.ToUpper() == Constants.REFRESH_MODE)
				{
					NavigateBllMenu(accountSelected, true, customerBillingDetails);
				}
				else if (!customerBillingDetails.Data.IsError)
				{
					NavigateBllMenu(accountSelected, false, customerBillingDetails);
				}
				else
				{
					NavigateBllMenu(accountSelected, true, null);
				}
			}
			catch (System.OperationCanceledException e)
			{
				if (this.mView.IsActive())
				{
					this.mView.HideProgressDialog();
				}
				NavigateBllMenu(accountSelected, true, null);
				Utility.LoggingNonFatalError(e);
			}
			catch (ApiException apiException)
			{
				if (this.mView.IsActive())
				{
					this.mView.HideProgressDialog();
				}
				NavigateBllMenu(accountSelected, true, null);
				Utility.LoggingNonFatalError(apiException);
			}
			catch (System.Exception e)
			{
				if (this.mView.IsActive())
				{
					this.mView.HideProgressDialog();
				}
				NavigateBllMenu(accountSelected, true, null);
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
                this.mView.PreShowBillMenu(accountData);
                this.mView.SetToolbarTitle(Resource.String.bill_menu_activity_title);
                currentBottomNavigationMenu = Resource.Id.menu_bill;
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        private void NavigateBllMenu(CustomerBillingAccount selectedAccount, bool hasError, AccountDetailsResponse response)
		{
			try
			{
				AccountData accountData = AccountData.Copy(selectedAccount, true);
				this.mView.SetAccountName(selectedAccount.AccDesc);
				if (hasError)
				{
					if (response != null && response.Data != null && !string.IsNullOrEmpty(response.Data.RefreshMessage) && !string.IsNullOrEmpty(response.Data.RefreshBtnText))
					{
						this.mView.ShowBillMenuWithError(response.Data.RefreshMessage, response.Data.RefreshBtnText, accountData);
					}
					else
					{
						this.mView.ShowBillMenuWithError(null, null, accountData);
					}
				}
				else
				{
					if (response != null && response.Data != null && response.Data.AccountData != null)
					{
						accountData = AccountData.Copy(response.Data.AccountData, true);
					}
					CustomerBillingAccount customerBillingAccount = CustomerBillingAccount.FindByAccNum(accountData.AccountNum);
					accountData.AccountNickName = selectedAccount.AccDesc;
					accountData.AccountName = selectedAccount.OwnerName;
					accountData.AddStreet = selectedAccount.AccountStAddress;
					accountData.IsOwner = customerBillingAccount.isOwned;
					accountData.AccountCategoryId = customerBillingAccount.AccountCategoryId;
					this.mView.ShowBillMenu(accountData);
				}
				this.mView.ShowAccountName();
                this.mView.ShowHideActionBar(true);
                this.mView.SetToolbarTitle(Resource.String.bill_menu_activity_title);
				currentBottomNavigationMenu = Resource.Id.menu_bill;
			}
			catch (System.Exception e)
			{
				Utility.LoggingNonFatalError(e);
			}
		}

		public void OnValidateData()
		{
			if (PromotionsEntityV2.HasUnread())
			{
				this.mView.ShowUnreadPromotions();

			}
			else
			{
				this.mView.HideUnreadPromotions();

			}

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
					string json = getItemsService.GetPromotionsTimestampItem();
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
					Log.Error("API Exception", e.StackTrace);
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
					Log.Error("API Exception", e.StackTrace);
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
				Log.Error("DB Exception", e.StackTrace);
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
									if (storedSMData != null && storedSMData.Data != null && storedSMData.Data.SMUsageHistoryData != null)
									{
										this.mView.ShowAccountName();
                                        this.mView.SetToolbarTitle(Resource.String.dashboard_menu_activity_title);
                                        this.mView.ShowSMChart(storedSMData.Data.SMUsageHistoryData, AccountData.Copy(selected, true));
									}
									else
									{
										LoadSMUsageHistory(selected);
									}
								}
								else
								{
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
											CustomerBillingAccount.Update(selected.AccNum, true);
										}
										usageHistoryResponse = JsonConvert.DeserializeObject<UsageHistoryResponse>(storedEntity.JsonResponse);
										if ((usageHistoryResponse != null && usageHistoryResponse.Data != null && usageHistoryResponse.Data.Status.ToUpper() == Constants.REFRESH_MODE) || !(usageHistoryResponse != null && usageHistoryResponse.Data.Status.Equals("success") && !usageHistoryResponse.Data.IsError))
										{
											usageHistoryResponse = null;
										}
										LoadUsageHistory(selected);
									}
									else
									{
										LoadUsageHistory(selected);
									}
								}
								else
								{
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
                if (selected.AccountCategoryId.Equals("2"))
                {
                    this.mView.ShowREAccount(true);
                }
                else
                {
                    this.mView.EnableDropDown(true);
                }
                LoadBills(selected);

            }
            else
            {
                this.mView.DisableBillMenu();
            }
        }

    }

}
