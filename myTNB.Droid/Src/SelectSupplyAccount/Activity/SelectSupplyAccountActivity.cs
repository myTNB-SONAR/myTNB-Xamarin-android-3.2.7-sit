﻿using AFollestad.MaterialDialogs;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Preferences;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using CheeseBind;
using Google.Android.Material.Snackbar;
using myTNB.Mobile;
using myTNB.AndroidApp.Src.Base.Activity;
using myTNB.AndroidApp.Src.Dashboard.Adapter;
using myTNB.AndroidApp.Src.Database.Model;
using myTNB.AndroidApp.Src.DBR.DBRApplication.MVP;
using myTNB.AndroidApp.Src.myTNBMenu.Models;
using myTNB.AndroidApp.Src.SelectSupplyAccount.MVP;
using myTNB.AndroidApp.Src.Utils;
using myTNB.AndroidApp.Src.ViewBill.Activity;
using Newtonsoft.Json;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;

namespace myTNB.AndroidApp.Src.SelectSupplyAccount.Activity
{
    [Activity(Label = "Select Electricity Account"
        , Icon = "@drawable/ic_launcher"
        , ScreenOrientation = ScreenOrientation.Portrait
        , Theme = "@style/Theme.Dashboard")]
    public class SelectSupplyAccountActivity : BaseActivityCustom, SelectSupplyAccountContract.IView
    {

        private SelectSupplyAccountContract.IUserActionsListener userActionsListener;
        private SelectSupplyAccountPresenter mPresenter;
        [BindView(Resource.Id.list_view)]
        ListView listView;

        [BindView(Resource.Id.btnAddAnotherAccount)]
        Button btnAddAnotherAccount;

        SelectSupplyAccountAdapter accountListAdapter;

        MaterialDialog materialDialog;

        
        const string PAGE_ID = "SelectElectricityAccounts";

        private bool isFromQuickAction = false;

        public bool IsActive()
        {
            return Window.DecorView.RootView.IsShown;
        }

        public override int ResourceId()
        {
            return Resource.Layout.SelectSupplyAccountView;
        }

        public void SetPresenter(SelectSupplyAccountContract.IUserActionsListener userActionListener)
        {
            this.userActionsListener = userActionListener;
        }

        public override bool ShowCustomToolbarTitle()
        {
            return true;
        }

        public void ShowList(List<CustomerBillingAccount> customerBillingAccountList)
        {
            Console.WriteLine(string.Format("Size {0}", customerBillingAccountList.Count));
            List<CustomerBillingAccount> custBRList = new List<CustomerBillingAccount>();
            List<string> BRCas = BillRedesignUtility.Instance.GetCAList();
            bool BRflag = UserSessions.GetFromBRCard(PreferenceManager.GetDefaultSharedPreferences(this));
            int isSelected = 0;
            if (BRflag && BRCas.Count > 0)
            {
                int countBRlistAdded = 0;
                for (int x = 0; x < customerBillingAccountList.Count; x++)
                {
                    foreach (var brca in BRCas)
                    {
                        if (customerBillingAccountList[x].AccNum == brca)
                        {
                            CustomerBillingAccount customerBillingAccount = new CustomerBillingAccount();
                            customerBillingAccount.AccDesc = customerBillingAccountList[x].AccDesc;
                            customerBillingAccount.OwnerName = customerBillingAccountList[x].OwnerName;
                            customerBillingAccount.AccNum = customerBillingAccountList[x].AccNum;
                            customerBillingAccount.AccountStAddress = customerBillingAccountList[x].AccountStAddress;
                            customerBillingAccount.isOwned = customerBillingAccountList[x].isOwned;
                            customerBillingAccount.AccountCategoryId = customerBillingAccountList[x].AccountCategoryId;
                            customerBillingAccount.IsHaveAccess = customerBillingAccountList[x].IsHaveAccess;
                            customerBillingAccount.IsSelected = customerBillingAccountList[x].IsSelected;

                            custBRList.Add(customerBillingAccount);
                            countBRlistAdded++;

                            if (customerBillingAccountList[x].IsSelected)
                            {
                                isSelected++;
                            }
                        }
                    }
                }
                if (isSelected == 0 && custBRList != null && custBRList.Count > 0)
                {
                    custBRList[0].IsSelected = true;
                }
                if (custBRList != null && custBRList.Count > 0)
                {
                    accountListAdapter.AddAll(custBRList);
                }
                else
                {
                    accountListAdapter.AddAll(customerBillingAccountList);
                }
            }
            else
            {
                accountListAdapter.AddAll(customerBillingAccountList);
            }

        }

        private Snackbar mCancelledExceptionSnackBar;
        public void ShowRetryOptionsCancelledException(System.OperationCanceledException operationCanceledException)
        {
            if (mCancelledExceptionSnackBar != null && mCancelledExceptionSnackBar.IsShown)
            {
                mCancelledExceptionSnackBar.Dismiss();
            }

            mCancelledExceptionSnackBar = Snackbar.Make(listView, Utility.GetLocalizedErrorLabel("defaultErrorMessage"), Snackbar.LengthIndefinite)
            .SetAction(Utility.GetLocalizedCommonLabel("close"), delegate
            {

                mCancelledExceptionSnackBar.Dismiss();
            }
            );
            mCancelledExceptionSnackBar.Show();

        }

        private Snackbar mApiExcecptionSnackBar;
        public void ShowRetryOptionsApiException(ApiException apiException)
        {
            if (mApiExcecptionSnackBar != null && mApiExcecptionSnackBar.IsShown)
            {
                mApiExcecptionSnackBar.Dismiss();
            }

            mApiExcecptionSnackBar = Snackbar.Make(listView, Utility.GetLocalizedErrorLabel("defaultErrorMessage"), Snackbar.LengthIndefinite)
            .SetAction(Utility.GetLocalizedCommonLabel("close"), delegate
            {

                mApiExcecptionSnackBar.Dismiss();
            }
            );
            mApiExcecptionSnackBar.Show();

        }
        private Snackbar mUknownExceptionSnackBar;
        public void ShowRetryOptionsUnknownException(Exception exception)
        {
            if (mUknownExceptionSnackBar != null && mUknownExceptionSnackBar.IsShown)
            {
                mUknownExceptionSnackBar.Dismiss();

            }

            mUknownExceptionSnackBar = Snackbar.Make(listView, Utility.GetLocalizedErrorLabel("defaultErrorMessage"), Snackbar.LengthIndefinite)
            .SetAction(Utility.GetLocalizedCommonLabel("close"), delegate
            {

                mUknownExceptionSnackBar.Dismiss();
            }
            );
            mUknownExceptionSnackBar.Show();

        }
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetTheme(TextViewUtils.IsLargeFonts ? Resource.Style.Theme_DashboardLarge : Resource.Style.Theme_Dashboard);
            try
            {
                isFromQuickAction = false;

                Bundle extras = Intent.Extras;

                if (extras != null && extras.ContainsKey(Constants.CODE_KEY) && extras.GetInt(Constants.CODE_KEY) == Constants.SELECT_ACCOUNT_PDF_REQUEST_CODE)
                {
                    isFromQuickAction = true;
                }

                mPresenter = new SelectSupplyAccountPresenter(this);

                accountListAdapter = new SelectSupplyAccountAdapter(this, true);
                listView.Adapter = accountListAdapter;

                listView.ItemClick += OnItemClick;

                this.userActionsListener.Start();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        protected override void OnResume()
        {
            base.OnResume();
            try
            {
                FirebaseAnalyticsUtils.SetScreenName(this, "Select Electricity Account");
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        [Preserve]
        internal void OnItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            try
            {
                CustomerBillingAccount customerAccount = accountListAdapter.GetItemObject(e.Position);
                CustomerBillingAccount.RemoveSelected();
                CustomerBillingAccount.SetSelected(customerAccount.AccNum);

                if (isFromQuickAction)
                {
                    OnViewPDF(customerAccount);
                    accountListAdapter.Clear();
                    this.userActionsListener.Start();
                }
                else
                {
                    Intent result = new Intent();
                    result.PutExtra(Constants.SELECTED_ACCOUNT, JsonConvert.SerializeObject(customerAccount));
                    SetResult(Result.Ok, result);
                    Finish();
                }
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }

        private Snackbar mSnackBarError;
        public void ShowQueryError(string errorMessage)
        {
            if (mSnackBarError != null && mSnackBarError.IsShown)
            {
                mSnackBarError.Dismiss();

            }

            mSnackBarError = Snackbar.Make(listView, errorMessage, Snackbar.LengthIndefinite)
            .SetAction(Utility.GetLocalizedCommonLabel("close"), delegate
            {

                mSnackBarError.Dismiss();
            }
            );
            mSnackBarError.Show();
        }

        public void ShowDashboardChart(UsageHistoryResponse response, AccountData accountData)
        {
            Intent result = new Intent();
            result.PutExtra(Constants.SELECTED_ACCOUNT, JsonConvert.SerializeObject(accountData));
            result.PutExtra(Constants.SELECTED_ACCOUNT_USAGE, JsonConvert.SerializeObject(response.Data.UsageHistoryData));
            SetResult(Result.Ok, result);
            Finish();
        }

        public void ShowDashboardChartWithError()
        {
            Intent result = new Intent();
            result.PutExtra(Constants.REFRESH_MODE, true);
            SetResult(Result.FirstUser, result);
            Finish();
        }

        public void ShowProgressDialog()
        {
            try
            {
                LoadingOverlayUtils.OnRunLoadingAnimation(this);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void HideShowProgressDialog()
        {
            try
            {
                LoadingOverlayUtils.OnStopLoadingAnimation(this);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }


#if STUB || DEVELOP
        public string GetUsageHistoryStub()
        {
            var inputStream = Resources.OpenRawResource(Resource.Raw.GetUsageHistoryResponse);
            var stringContent = string.Empty;

            using (StreamReader sr = new StreamReader(inputStream))
            {
                stringContent = sr.ReadToEnd();
            }

            return stringContent;

        }


        public string GetAccountDetailsStub(string accNum)
        {

            var inputStream = Resources.OpenRawResource(Resource.Raw.GetAccountBillingDetailsResponse);
            if (accNum.Equals("210040320600"))
            {
                inputStream = Resources.OpenRawResource(Resource.Raw.GetAccountBillingDetailsResponse210040320600);
            }
            else if (accNum.Equals("220130881800"))
            {
                inputStream = Resources.OpenRawResource(Resource.Raw.GetAccountBillingDetailsResponse220130881800);
            }
            else if (accNum.Equals("220136555409"))
            {
                inputStream = Resources.OpenRawResource(Resource.Raw.GetAccountBillingDetailsResponse220136555409);
            }
            else if (accNum.Equals("220147054010"))
            {
                inputStream = Resources.OpenRawResource(Resource.Raw.GetAccountBillingDetailsResponse220147054010);
            }
            else if (accNum.Equals("220163099904"))
            {
                inputStream = Resources.OpenRawResource(Resource.Raw.GetAccountBillingDetailsResponse220163099904);
            }
            else if (accNum.Equals("220164535604"))
            {
                inputStream = Resources.OpenRawResource(Resource.Raw.GetAccountBillingDetailsResponse220164535604);
            }
            else if (accNum.Equals("220223313703"))
            {
                inputStream = Resources.OpenRawResource(Resource.Raw.GetAccountBillingDetailsResponse220223313703);
            }
            else if (accNum.Equals("220231662807"))
            {
                inputStream = Resources.OpenRawResource(Resource.Raw.GetAccountBillingDetailsResponse220231662807);
            }
            else if (accNum.Equals("220272777303"))
            {
                inputStream = Resources.OpenRawResource(Resource.Raw.GetAccountBillingDetailsResponse220272777303);
            }
            else if (accNum.Equals("220280837809"))
            {
                inputStream = Resources.OpenRawResource(Resource.Raw.GetAccountBillingDetailsResponse220280837809);
            }
            else if (accNum.Equals("220595158104"))
            {
                inputStream = Resources.OpenRawResource(Resource.Raw.GetAccountBillingDetailsResponse220595158104);
            }
            var stringContent = string.Empty;

            using (StreamReader sr = new StreamReader(inputStream))
            {
                stringContent = sr.ReadToEnd();
            }

            return stringContent;

        }
#endif


        public void ShowNoInternetConnection()
        {
            SetResult(Result.FirstUser);
            Finish();
        }

        public bool HasInternetConnection()
        {
            return ConnectionUtils.HasInternetConnection(this);
        }

        private void OnViewPDF(CustomerBillingAccount selectedAccount)
        {
            AccountData selectedAccountData = AccountData.Copy(selectedAccount, true);

            Intent viewBill = new Intent(this, typeof(ViewBillActivity));
            viewBill.PutExtra(Constants.SELECTED_ACCOUNT, JsonConvert.SerializeObject(selectedAccountData));
            viewBill.PutExtra(Constants.CODE_KEY, Constants.SELECT_ACCOUNT_PDF_REQUEST_CODE);
            StartActivity(viewBill);
        }

        public override void OnTrimMemory(TrimMemory level)
        {
            base.OnTrimMemory(level);

            switch (level)
            {
                case TrimMemory.RunningLow:
                    // GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
                    GC.Collect();
                    break;
                default:
                    // GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
                    GC.Collect();
                    break;
            }
        }

        public override string GetPageId()
        {
            return PAGE_ID;
        }
    }
}