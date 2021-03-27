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
    public class DashboardPresenter : DashboardContract.IUserActionsListener
    {
        internal readonly string TAG = typeof(DashboardPresenter).Name;

        CancellationTokenSource cts;



        private DashboardContract.IView mView;
        private ISharedPreferences mSharedPref;

        internal int currentBottomNavigationMenu = Resource.Id.menu_dashboard;

        private bool smDataError = false;
        private string smErrorCode = "204";
        private string smErrorMessage = "Sorry, Something went wrong. Please try again later";

        private string preSelectedAccount;
        private UsageHistoryResponse usageHistoryResponse;

        public DashboardPresenter(DashboardContract.IView mView, ISharedPreferences preferences)
        {
            this.mView = mView;
            this.mSharedPref = preferences;
            this.mView?.SetPresenter(this);

            //Console.WriteLine("UserID {0}" + UserEntity.GetActive().UserID);

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

                        AccountData selectedAccount = JsonConvert.DeserializeObject<AccountData>(extras.GetString(Constants.SELECTED_ACCOUNT));
                        UsageHistoryData selectedHistoryData = JsonConvert.DeserializeObject<UsageHistoryData>(extras.GetString(Constants.SELECTED_ACCOUNT_USAGE));

                        bool isOwned = true;
                        CustomerBillingAccount customerBillingAccount = CustomerBillingAccount.FindByAccNum(selectedAccount.AccountNum);
                        preSelectedAccount = selectedAccount.AccountNum;
                        if (customerBillingAccount != null)
                        {
                            isOwned = customerBillingAccount.isOwned;
                            selectedAccount.IsOwner = isOwned;
                            selectedAccount.AccountCategoryId = customerBillingAccount.AccountCategoryId;

                        }

                        if (currentBottomNavigationMenu == Resource.Id.menu_dashboard)
                        {
                            this.mView.ShowAccountName();
                            this.mView.SetToolbarTitle(Resource.String.dashboard_menu_activity_title);

                            if (customerBillingAccount != null && customerBillingAccount.SmartMeterCode != null && customerBillingAccount.SmartMeterCode.Equals("0"))
                            {
                                this.mView.ShowChart(selectedHistoryData, selectedAccount);
                            }
                            else
                            {
                                if (!SMUsageHistoryEntity.IsSMDataUpdated(selectedAccount.AccountNum))
                                {
                                    SMUsageHistoryEntity storedEntity = SMUsageHistoryEntity.GetItemByAccountNo(selectedAccount.AccountNum);
                                    if (storedEntity != null)
                                    {
                                        SMUsageHistoryResponse storedSMData = JsonConvert.DeserializeObject<SMUsageHistoryResponse>(storedEntity.JsonResponse);
                                        this.mView.ShowSMChart(storedSMData.Data.SMUsageHistoryData, selectedAccount);
                                    }
                                    else
                                    {
                                        if (customerBillingAccount != null)
                                            LoadSMUsageHistory(customerBillingAccount);
                                    }
                                }
                                else
                                {
                                    if (customerBillingAccount != null)
                                        LoadSMUsageHistory(customerBillingAccount);
                                }
                            }
                        }
                        //}
                        else if (currentBottomNavigationMenu == Resource.Id.menu_bill)
                        {
                            this.mView.ShowAccountName();
                            this.mView.SetToolbarTitle(Resource.String.bill_menu_activity_title);
                            this.mView.ShowBillMenu(selectedAccount);

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
                            this.mView.SetAccountName(customerBillingAccount.AccDesc);
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
                                    this.mView.ShowAccountName();
                                }
                                else
                                {
                                    CustomerBillingAccount selected = CustomerBillingAccount.GetSelected();
                                    this.mView.ShowAccountName();
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
                            this.mView.ShowAccountName();
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

                    //this.mView.SetToolbarTitle(Resource.String.dashboard_menu_activity_title);
                    if (accountList != null && accountList.Count <= 1)
                    {
                        OnAccountSelectDashBoard();
                    }
                    else
                    {
                        if (DashboardActivity.currentFragment != null && (DashboardActivity.currentFragment.GetType() == typeof(SummaryDashBoardFragment) ||
                            DashboardActivity.currentFragment.GetType() == typeof(DashboardChartFragment) ||
                            DashboardActivity.currentFragment.GetType() == typeof(DashboardSmartMeterFragment)))
                        {
                            mView.ShowBackButton(false);
                            DoLoadSummaryDashBoardFragment();
                        }
                        else
                        {
                            if (DashboardActivity.GO_TO_INNER_DASHBOARD)
                            {
                                OnAccountSelectDashBoard();
                            }
                            else
                            {
                                mView.ShowBackButton(false);
                                DoLoadSummaryDashBoardFragment();
                            }

                        }
                    }

                    break;
                case Resource.Id.menu_bill:
                    //this.mView.ShowAccountName();
                    ////this.mView.EnableDropDown(true);
                    //this.mView.SetToolbarTitle(Resource.String.bill_menu_activity_title);
                    //currentBottomNavigationMenu = Resource.Id.menu_bill;
                    if (accountList.Count > 0)
                    {
                        CustomerBillingAccount selected;
                        if (CustomerBillingAccount.HasSelected())
                        {
                            selected = CustomerBillingAccount.GetSelected();
                            LoadBills(selected);
                            this.mView.SetAccountName(selected.AccDesc);
                        }
                        else
                        {
                            CustomerBillingAccount.SetSelected(accountList[0].AccNum);
                            selected = CustomerBillingAccount.GetSelected();
                            LoadBills(accountList[0]);
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

                    }
                    else
                    {
                        this.mView.HideAccountName();
                        this.mView.ShowNoAccountBillMenu();
                    }
                    //currentBottomNavigationMenu = Resource.Id.menu_bill;


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
                    //Logout();
                    break;
            }
        }

        public void DoLoadSummaryDashBoardFragment()
        {
            Console.WriteLine("000 DoLoadSummaryDashBoardFragment started");
            //currentBottomNavigationMenu = Resource.Id.menu_promotion;
            this.mView.ShowSummaryDashBoard();
            this.mView.SetToolbarTitle(Resource.String.all_accounts);
            currentBottomNavigationMenu = Resource.Id.menu_dashboard;

            this.mView.EnableDropDown(false);
            this.mView.HideAccountName();
            //this.mView.ShowSummaryDashBoard();
            Console.WriteLine("000 DoLoadSummaryDashBoardFragment ended");
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

            if (currentBottomNavigationMenu == Resource.Id.menu_promotion || currentBottomNavigationMenu == Resource.Id.menu_feedback || currentBottomNavigationMenu == Resource.Id.menu_more)
            {
                return;
            }
            // NO IMPL
            // TODO : ADD LOGIC FOR SHOWING SCREENS BELOW
            // TODO : 1. NO INTERNET CONNECTION
            // TODO : 2. DASHBOARD WITH MULTIPLE ACCOUNTS
            // TODO : 3. DASHBOARD WITH SINGLE ACCOUNT
            // TODO : 4. DASHBOARD NON OWNER
            // TODO : 5. DASHBOARD NO LINKED SUPPLY ACCOUNT
            if (currentBottomNavigationMenu == Resource.Id.menu_dashboard)
            {
                List<CustomerBillingAccount> accountList = CustomerBillingAccount.List();
                if (accountList != null && accountList.Count <= 1)
                {
                    OnAccountSelectDashBoard();
                }
                else
                {
                    DoLoadSummaryDashBoardFragment();
                }
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
                            this.mView.ShowOwnerDashboardNoInternetConnection(accountSelected.AccDesc, usageHistoryResponse, AccountData.Copy(accountSelected, true));
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
                                this.mView.ShowAccountName();
                                this.mView.SetToolbarTitle(Resource.String.dashboard_menu_activity_title);
                                if (smDataError)
                                {
                                    smDataError = false;
                                    this.mView.ShowChartWithError(usageHistoryResponse.Data.UsageHistoryData, AccountData.Copy(accountSelected, true), smErrorCode, smErrorMessage);
                                }
                                else
                                {
                                    this.mView.ShowChart(usageHistoryResponse.Data.UsageHistoryData, AccountData.Copy(accountSelected, true));
                                }
                                usageHistoryResponse = null;
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
                            this.mView.ShowOwnerDashboardNoInternetConnection(accountSelected.AccDesc, null, AccountData.Copy(accountSelected, true));
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
                        this.mView.ShowOwnerDashboardNoInternetConnection(accountSelected.AccDesc, null, AccountData.Copy(accountSelected, true));
                        usageHistoryResponse = null;
                        Utility.LoggingNonFatalError(e);
                    }
                    catch (ApiException apiException)
                    {
                        if (this.mView.IsActive())
                        {
                            this.mView.HideProgressDialog();
                        }
                        this.mView.ShowOwnerDashboardNoInternetConnection(accountSelected.AccDesc, null, AccountData.Copy(accountSelected, true));
                        usageHistoryResponse = null;
                        Utility.LoggingNonFatalError(apiException);
                    }
                    catch (System.Exception e)
                    {
                        if (this.mView.IsActive())
                        {
                            this.mView.HideProgressDialog();
                        }
                        this.mView.ShowOwnerDashboardNoInternetConnection(accountSelected.AccDesc, null, AccountData.Copy(accountSelected, true));
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
                    this.mView.ShowOwnerDashboardNoInternetConnection(accountSelected.AccDesc, true, null, null, AccountData.Copy(accountSelected, true));
                }
            }
            catch (System.OperationCanceledException e)
            {
                if (this.mView.IsActive())
                {
                    this.mView.HideProgressDialog();
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
                this.mView.ShowOwnerDashboardNoInternetConnection(accountSelected.AccDesc, true, null, null, AccountData.Copy(accountSelected, true));
                Utility.LoggingNonFatalError(e);
            }

        }


        private async void LoadSMUsageHistory(CustomerBillingAccount accountSelected)
        {
            /*** Check for timestamp to call service ***/

            /****/

            cts = new CancellationTokenSource();
            if (mView.IsActive())
            {
                this.mView.ShowProgressDialog();
            }

#if STUB
            var api = RestService.For<IUsageHistoryApi>(Constants.SERVER_URL.END_POINT);
#elif DEBUG
            var httpClient = new HttpClient(new HttpLoggingHandler(/*new NativeMessageHandler()*/)) { BaseAddress = new Uri(Constants.SERVER_URL.END_POINT) };
            var api = RestService.For<IUsageHistoryApi>(httpClient);

#elif DEVELOP
            var api = RestService.For<IUsageHistoryApi>(Constants.SERVER_URL.END_POINT);

            //api.DoQuery(new Requests.UsageHistoryRequest(Constants.APP_CONFIG.API_KEY_ID) {
            //    AccountNum = accountSelected.AccNum
            //}, cts.Token)
            //.ReturnsForAnyArgs(
            //    Task.Run<UsageHistoryResponse>(
            //        () => JsonConvert.DeserializeObject<UsageHistoryResponse>(this.mView.GetUsageHistoryStub())
            //    ));
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
                Log.Debug(TAG, "Cancelled Exception");
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
                Log.Debug(TAG, "Stack " + e.StackTrace);
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
            //this.mView.ShowProgressDialog();
#if STUB
            var detailedAccountApi = RestService.For<IDetailedCustomerAccount>(Constants.SERVER_URL.END_POINT);
#elif DEBUG
            var httpClient = new HttpClient(new HttpLoggingHandler(/*new NativeMessageHandler()*/)) { BaseAddress = new Uri(Constants.SERVER_URL.END_POINT) };
            var api = RestService.For<IUsageHistoryApi>(httpClient);
            var detailedAccountApi = RestService.For<IDetailedCustomerAccount>(httpClient);
#elif DEVELOP
            var detailedAccountApi = RestService.For<IDetailedCustomerAccount>(Constants.SERVER_URL.END_POINT);
#else
            var detailedAccountApi = RestService.For<IDetailedCustomerAccount>(Constants.SERVER_URL.END_POINT);
#endif

            try
            {
                if (this.mView.IsActive())
                {
                    this.mView.ShowProgressDialog();
                }
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
                Log.Debug(TAG, "Cancelled Exception");
                // ADD OPERATION CANCELLED HERE
                if (this.mView.IsActive())
                {
                    this.mView.HideProgressDialog();
                }
                NavigateBllMenu(accountSelected, true, null);
                Utility.LoggingNonFatalError(e);
                //this.mView.ShowRetryOptionsCancelledException(e);
            }
            catch (ApiException apiException)
            {
                // ADD HTTP CONNECTION EXCEPTION HERE
                if (this.mView.IsActive())
                {
                    this.mView.HideProgressDialog();
                }
                NavigateBllMenu(accountSelected, true, null);
                //this.mView.ShowRetryOptionsApiException(apiException);
                Utility.LoggingNonFatalError(apiException);
            }
            catch (System.Exception e)
            {
                // ADD UNKNOWN EXCEPTION HERE
                Log.Debug(TAG, "Stack " + e.StackTrace);
                if (this.mView.IsActive())
                {
                    this.mView.HideProgressDialog();
                }
                NavigateBllMenu(accountSelected, true, null);
                //this.mView.ShowRetryOptionsUnknownException(e);

                Utility.LoggingNonFatalError(e);
            }



        }

        public void OnNotificationCount()
        {
            this.mView.ShowNotificationCount(UserNotificationEntity.Count());
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
                    string json = getItemsService.GetPromotionsTimestampItem();
                    PromotionsParentV2ResponseModel responseModel = JsonConvert.DeserializeObject<PromotionsParentV2ResponseModel>(json);
                    if (responseModel.Status.Equals("Success"))
                    {
                        PromotionsParentEntityV2 wtManager = new PromotionsParentEntityV2();
                        wtManager.DeleteTable();
                        wtManager.CreateTable();
                        wtManager.InsertListOfItems(responseModel.Data);
                        mView.ShowPromotionTimestamp(true);
                        Log.Debug("WalkThroughResponse", responseModel.Data.ToString());
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
                        Log.Debug("DashboardPresenter", responseModel.Data.ToString());
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
            Console.WriteLine("000 OnAccountSelectDashBoard started");
            try
            {
                DashboardActivity.GO_TO_INNER_DASHBOARD = true;
                List<CustomerBillingAccount> accountList = new List<CustomerBillingAccount>();
                accountList = CustomerBillingAccount.List();
                this.mView.ShowAccountName();
                this.mView.SetToolbarTitle(Resource.String.dashboard_menu_activity_title);
                currentBottomNavigationMenu = Resource.Id.menu_dashboard;
                if (accountList != null && accountList.Count > 0)
                {
                    if (CustomerBillingAccount.HasSelected())
                    {
                        CustomerBillingAccount selected = new CustomerBillingAccount();
                        selected = CustomerBillingAccount.GetSelected();

                        if (selected != null && !string.IsNullOrEmpty(selected.AccDesc))
                        {
                            /** Smart meter account check **/
                            if (!selected.SmartMeterCode.Equals("0"))// && selected.isOwned)
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
                    this.mView.HideAccountName();
                    this.mView.DisableBillMenu();
                }
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
            Console.WriteLine("000 OnAccountSelectDashBoard ended");
            //currentBottomNavigationMenu = Resource.Id.menu_dashboard;
        }


        public void OnStartDashboard()
        {
            try
            {
                ServicePointManager.ServerCertificateValidationCallback += SSLFactoryHelper.CertificateValidationCallBack;

                List<CustomerBillingAccount> accountList = CustomerBillingAccount.List();

                if (accountList.Count > 0)
                {
                    if (this.mView.IsActive())
                    {
                        this.mView.EnableBillMenu();
                    }

                    CustomerBillingAccount selected = CustomerBillingAccount.GetSelected();

                    //Check BCRM Downtime
                    DownTimeEntity bcrmDownTime = DownTimeEntity.GetByCode(Constants.BCRM_SYSTEM);
                    if (bcrmDownTime != null && bcrmDownTime.IsDown)
                    {
                        if (selected.SmartMeterCode != null && !selected.SmartMeterCode.Equals("0"))
                        {
                            SMUsageHistoryData data = new SMUsageHistoryData();
                            this.mView.ShowSMChartWithError(data, AccountData.Copy(selected, true), true);
                        }
                        else
                        {
                            this.mView.ShowDownTimeView(Constants.BCRM_SYSTEM, selected.AccDesc);
                        }
                    }
                    else
                    {

                        if (CustomerBillingAccount.HasSelected())
                        {

                            /** Smart meter check **/
                            if (selected.SmartMeterCode != null && !selected.SmartMeterCode.Equals("0"))// && selected.isOwned)
                            {
                                if (!SMUsageHistoryEntity.IsSMDataUpdated(selected.AccNum))
                                {
                                    //Get stored data
                                    SMUsageHistoryEntity storedEntity = SMUsageHistoryEntity.GetItemByAccountNo(selected.AccNum);
                                    SMUsageHistoryResponse storedSMData = null;
                                    if (storedEntity != null)
                                    {
                                        storedSMData = JsonConvert.DeserializeObject<SMUsageHistoryResponse>(storedEntity.JsonResponse);
                                    }
                                    if (storedEntity != null && storedSMData.Data.SMUsageHistoryData != null)
                                    {
                                        if (currentBottomNavigationMenu == Resource.Id.menu_dashboard)
                                        {
                                            this.mView.ShowAccountName();
                                            this.mView.SetToolbarTitle(Resource.String.dashboard_menu_activity_title);
                                            this.mView.ShowSMChart(storedSMData.Data.SMUsageHistoryData, AccountData.Copy(selected, true));
                                        }
                                        else if (currentBottomNavigationMenu == Resource.Id.menu_bill)
                                        {
                                            this.mView.ShowAccountName();
                                            this.mView.SetToolbarTitle(Resource.String.bill_menu_activity_title);
                                            LoadBills(selected);
                                        }

                                        this.mView.SetAccountName(selected.AccDesc);
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

                                if (!UsageHistoryEntity.IsSMDataUpdated(selected.AccNum))
                                {
                                    UsageHistoryEntity storedEntity = UsageHistoryEntity.GetItemByAccountNo(selected.AccNum);
                                    if (storedEntity != null)
                                    {
                                        CustomerBillingAccount.RemoveSelected();
                                        CustomerBillingAccount.Update(selected.AccNum, true);
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
                            if (accountList.Count >= 1)
                            {
                                if (selected.AccountCategoryId.Equals("2"))
                                {
                                    this.mView.ShowREAccount(true);
                                }
                                else
                                {
                                    this.mView.EnableDropDown(true);
                                }

                                this.mView.SetAccountName(selected.AccDesc);
                            }
                            else
                            {
                                if (selected.AccountCategoryId.Equals("2"))
                                {
                                    this.mView.ShowREAccount(false);
                                }
                                else
                                {
                                    this.mView.EnableDropDown(false);
                                }
                            }
                        }
                        else
                        {
                            CustomerBillingAccount.SetSelected(accountList[0].AccNum);
                            LoadUsageHistory(accountList[0]);
                            if (accountList.Count >= 1)
                            {
                                if (accountList[0].AccountCategoryId.Equals("2"))
                                {
                                    this.mView.ShowREAccount(true);
                                }
                                else
                                {
                                    this.mView.EnableDropDown(true);
                                }
                                this.mView.SetAccountName(accountList[0].AccDesc);
                            }
                            else
                            {
                                if (accountList[0].AccountCategoryId.Equals("2"))
                                {
                                    this.mView.ShowREAccount(false);
                                }
                                else
                                {
                                    this.mView.EnableDropDown(false);
                                }
                            }
                        }

                    }
                }
                else
                {
                    // 5
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
    }

}