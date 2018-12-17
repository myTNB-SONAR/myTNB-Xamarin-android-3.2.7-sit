using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using myTNB_Android.Src.Base.Activity;
using Android.Support.V7.Widget;
using myTNB_Android.Src.MultipleAccountPayment.Adapter;
using myTNB_Android.Src.MultipleAccountPayment.Model;
using CheeseBind;
using myTNB_Android.Src.Database.Model;
using Android.Content.PM;
using Android.Util;
using myTNB_Android.Src.MultipleAccountPayment.MVP;
using myTNB_Android.Src.Utils;
using Android.Support.Design.Widget;
using Android.Support.V4.Content;
using myTNB_Android.Src.myTNBMenu.Models;
using Newtonsoft.Json;
using AFollestad.MaterialDialogs;
using Android.Text;
using Android.Net;
using Android.Views.InputMethods;
using Java.Text;
using myTNB_Android.Src.Utils.Custom.ProgressDialog;
using System.Runtime;

namespace myTNB_Android.Src.MultipleAccountPayment.Activity
{
    [Activity(Label = "Select Bill(s)"
       , ScreenOrientation = ScreenOrientation.Portrait, Theme = "@style/Theme.LinkAccount",
        WindowSoftInputMode = SoftInput.AdjustPan)]
    public class SelectAccountsActivity : BaseToolbarAppCompatActivity, MPSelectAccountsContract.IView
    {
        private readonly string TAG = "SelectAccountsActivity";
        private MPSelectAccountsPresenter mPresenter;
        private MPSelectAccountsContract.IUserActionsListener userActionsListener;
        public static SelectAccountsActivity selectAccountsActivity;

        private Snackbar mErrorMessageSnackBar;
        private MaterialDialog mGetDueAmountDialog;

        private LoadingOverlay loadingOverlay;

        RecyclerView.LayoutManager layoutManager;
        SelectAccountListAdapter adapter;
        List<MPAccount> accountList = new List<MPAccount>();

        private int TOTAL_ACCOUNTS = 0;
        private int TOTAL_NUMBER_OF_ITEMS_TO_GET = 4;
        private int REMAINING_ITEM_COUNT = 0;
        private int INDEX_COUNTER = 0;
        private int NO_OF_ITARATION = 0;
        private bool firstTime = false;
        List<CustomerBillingAccount> registerdAccounts;
        AccountData selectedAccount;

        private DecimalFormat payableFormatter = new DecimalFormat("###############0.00");

        [BindView(Resource.Id.account_list_recycler_view)]
        RecyclerView accountListRecyclerView;

        [BindView(Resource.Id.baseView)]
        FrameLayout rootView;

        [BindView(Resource.Id.text_load_more)]
        TextView textLoadMore;

        [BindView(Resource.Id.txtTotalPayableCurrency)]
        TextView textTotalPayableCurrency;

        [BindView(Resource.Id.txtTotalPayableTitle)]
        TextView textTotalPayableTitle;

        [BindView(Resource.Id.txtTotalPayable)]
        TextView textTotalPayable;

        [BindView(Resource.Id.btnPayBills)]
        Button btnPayBill;

        public bool IsActive()
        {
            return this.Window.DecorView.RootView.IsShown;
        }

        public override int ResourceId()
        {
            return Resource.Layout.SelectMultipleAccountsView;
        }

        public void SetPresenter(MPSelectAccountsContract.IUserActionsListener userActionListener)
        {
            this.userActionsListener = userActionListener;
        }

        public override bool ShowCustomToolbarTitle()
        {
            return true;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            try {
            // Create your application here
            mPresenter = new MPSelectAccountsPresenter(this);
            selectAccountsActivity = this;
            selectedAccount = JsonConvert.DeserializeObject<AccountData>(Intent.Extras.GetString(Constants.SELECTED_ACCOUNT));

            registerdAccounts = CustomerBillingAccount.List();
            var found = registerdAccounts.Where(x => x.AccNum == selectedAccount.AccountNum).FirstOrDefault();
            if (found != null)
            {
                registerdAccounts.Remove(found);
            }
            registerdAccounts.RemoveAll(x => x.AccountCategoryId == "2");

            TOTAL_ACCOUNTS = registerdAccounts.Count;
            NO_OF_ITARATION = (TOTAL_ACCOUNTS / TOTAL_NUMBER_OF_ITEMS_TO_GET);
            REMAINING_ITEM_COUNT = TOTAL_ACCOUNTS - (NO_OF_ITARATION * TOTAL_NUMBER_OF_ITEMS_TO_GET);
            if(TOTAL_ACCOUNTS < 8 && TOTAL_ACCOUNTS > 4)
            {
                NO_OF_ITARATION = NO_OF_ITARATION + 1;
            }


            TextViewUtils.SetMuseoSans300Typeface(textTotalPayable, textTotalPayableTitle);
            TextViewUtils.SetMuseoSans500Typeface(textTotalPayableCurrency);
            TextViewUtils.SetMuseoSans500Typeface(btnPayBill);

            mGetDueAmountDialog = new MaterialDialog.Builder(this)
               .Title(GetString(Resource.String.getdueamount_progress_title))
               .Content(GetString(Resource.String.getdueamount_progress_message))
               .Progress(true, 0)
               .Cancelable(false)
               .Build();

            //ValidateAccountListAdapter();
            adapter = new SelectAccountListAdapter(this, accountList);
            layoutManager = new LinearLayoutManager(this, LinearLayoutManager.Vertical, false);

            if (CustomerBillingAccount.HasItems())
            {
                NO_OF_ITARATION = NO_OF_ITARATION - 1;
                List<string> custAccounts = new List<string>();
                custAccounts.Add(selectedAccount.AccountNum);
                List<CustomerBillingAccount> list = GetMoreCustomerAccounts(INDEX_COUNTER);
                foreach (CustomerBillingAccount item in list)
                {
                    custAccounts.Add(item.AccNum);
                }
                firstTime = true;
                this.userActionsListener.GetMultiAccountDueAmount(Constants.APP_CONFIG.API_KEY_ID, custAccounts);
            }

            accountListRecyclerView.SetLayoutManager(layoutManager);
            accountListRecyclerView.SetAdapter(adapter);
            adapter.CheckChanged += OnCheckChanged;

            btnPayBill.Click += delegate
            {
                NavigateToPayment();
            };

            string html = "<html><u>" + this.GetString(Resource.String.load_more) + "</u></html>";
            if (Android.OS.Build.VERSION.SdkInt >= Android.OS.Build.VERSION_CODES.N)
            {
                textLoadMore.TextFormatted = Html.FromHtml(html, FromHtmlOptions.ModeLegacy);
            }
            else
            {
                textLoadMore.TextFormatted = Html.FromHtml(html);
            }
            TextViewUtils.SetMuseoSans300Typeface(textLoadMore);
            textLoadMore.Click += delegate
            {
                if (IsNetworkAvailable()) {
                    NO_OF_ITARATION = NO_OF_ITARATION - 1;
                    INDEX_COUNTER = INDEX_COUNTER + TOTAL_NUMBER_OF_ITEMS_TO_GET;
                    List<string> custAccounts = new List<string>();
                    List<CustomerBillingAccount> list = GetMoreCustomerAccounts(INDEX_COUNTER);
                    foreach (CustomerBillingAccount item in list)
                    {
                        custAccounts.Add(item.AccNum);
                    }
                    this.userActionsListener.GetMultiAccountDueAmount(Constants.APP_CONFIG.API_KEY_ID, custAccounts);
                } else {
                    ShowError(this.GetString(Resource.String.dashboard_chartview_no_internet_content));
                }
            };

            btnPayBill.RequestFocus();
            InputMethodManager inputMethodManager = this.GetSystemService(Context.InputMethodService) as InputMethodManager;
            inputMethodManager.HideSoftInputFromWindow(rootView.WindowToken, 0);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        private void ValidateAccountListAdapter()
        {
            try {
            //adapter = new SelectAccountListAdapter(this, accountList);
            //layoutManager = new LinearLayoutManager(this, LinearLayoutManager.Vertical, false);
            //accountListRecyclerView.SetLayoutManager(layoutManager);
            //accountListRecyclerView.SetAdapter(adapter);
            //adapter.NotifyDataSetChanged();
            adapter.AddAccounts(accountList);
            //adapter.CheckChanged += OnCheckChanged;
            UpdateTotal(adapter.GetSelectedAccounts());
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        void OnCheckChanged(object sender, int position)
        {
            try {
            if (position == -1)
            {
                ShowError(this.GetString(Resource.String.error_select_5_accounts));
            }
            else
            {
                List<MPAccount> list = adapter.GetSelectedAccounts();
                Log.Debug("Selected Accounts", " List " + list);
                UpdateTotal(list);
            }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void UpdateTotal(List<MPAccount> selectedAccounts)
        {
            try {
            double total = 0;
            foreach (MPAccount account in selectedAccounts)
            {
                total += account.amount;
            }
            textTotalPayable.Text = payableFormatter.Format(total);
            if (selectedAccounts.Count > 0)
            {
                btnPayBill.Text = this.GetString(Resource.String.text_pay_bill) + " (" + selectedAccounts.Count + ")";
                EnablePayButton();
            }
            else
            {
                btnPayBill.Text = this.GetString(Resource.String.text_pay_bill);
                DisablePayButton();
            }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
            //IsValidAmount(total);
        }

        public void ShowError(string messge)
        {
            if (mErrorMessageSnackBar != null && mErrorMessageSnackBar.IsShown)
            {
                mErrorMessageSnackBar.Dismiss();
            }

            mErrorMessageSnackBar = Snackbar.Make(rootView, messge, Snackbar.LengthIndefinite)
            .SetAction("Close", delegate { mErrorMessageSnackBar.Dismiss(); }
            );
            View v = mErrorMessageSnackBar.View;
            TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
            tv.SetMaxLines(5);

            mErrorMessageSnackBar.Show();
        }

        public void NavigateToPayment()
        {
            if (adapter.IsAllAmountValid())
            {
                ///<remarks>
                /// Proceed to next screen 
                ///</remarks>
                Intent payment_activity = new Intent(this, typeof(PaymentActivity));
                payment_activity.PutExtra(Constants.SELECTED_ACCOUNT, JsonConvert.SerializeObject(selectedAccount));
                payment_activity.PutExtra("PAYMENT_ITEMS", JsonConvert.SerializeObject(adapter.GetSelectedAccounts()));
                payment_activity.PutExtra("TOTAL", textTotalPayable.Text);
                //StartActivity(payment_activity);
                StartActivityForResult(payment_activity, PaymentActivity.SELECT_PAYMENT_ACTIVITY_CODE);
            }
            else
            {
                Log.Debug(TAG, "Enter valid amount.");
            }
        }

        public void IsValidAmount(double amt)
        {
            if (amt > 5000)
            {
                ShowError(this.GetString(Resource.String.error_credit_card_limit));
            }
        }

        public void DisablePayButton()
        {
            btnPayBill.Enabled = false;
            btnPayBill.Background = ContextCompat.GetDrawable(this, Resource.Drawable.silver_chalice_button_background);
        }

        public void EnablePayButton()
        {
            btnPayBill.Enabled = true;
            btnPayBill.Background = ContextCompat.GetDrawable(this, Resource.Drawable.green_button_background);
        }

        public void ShowProgressDialog()
        {
            //if (this.mGetDueAmountDialog != null && !this.mGetDueAmountDialog.IsShowing)
            //{
            //    this.mGetDueAmountDialog.Show();
            //}
            try {
            if (loadingOverlay != null && loadingOverlay.IsShowing)
            {
                loadingOverlay.Dismiss();
            }

            loadingOverlay = new LoadingOverlay(this, Resource.Style.LoadingOverlyDialogStyle);
            loadingOverlay.Show();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void HideProgressDialog()
        {
            //if (this.mGetDueAmountDialog != null && this.mGetDueAmountDialog.IsShowing)
            //{
            //    this.mGetDueAmountDialog.Dismiss();
            //}
            try {
            if (loadingOverlay != null && loadingOverlay.IsShowing)
            {
                loadingOverlay.Dismiss();
            }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void GetAccountDueAmountResult(MPAccountDueResponse response)
        {
            //            List<CustomerBillingAccount> registerdAccounts = CustomerBillingAccount.List();
            try {
            if (response != null)
            {
                if (response.accountDueAmountResponse.accounts != null)
                {
                    if (response.accountDueAmountResponse.accounts.Count > 0)
                    {
                        accountList.Clear();
                        foreach (myTNB_Android.Src.MultipleAccountPayment.Model.MPAccountDueResponse.Account account in response.accountDueAmountResponse.accounts)
                        {
                            CustomerBillingAccount customerBillingAccount = CustomerBillingAccount.FindByAccNum(account.accNum);
                            double dueAmount = account.amountDue;
                            //if (dueAmount > 1)
                            //{
                            MPAccount mpAccount = new MPAccount()
                            {
                                accountLabel = customerBillingAccount.AccDesc,
                                accountNumber = customerBillingAccount.AccNum,
                                accountAddress = customerBillingAccount.AccountStAddress,
                                isSelected = selectedAccount.AccountNum.Equals(customerBillingAccount.AccNum) ? true && dueAmount > 0  : false,
                                amount = dueAmount,
                                orgAmount = dueAmount
                            };
                            accountList.Add(mpAccount);
                            //}

                            /*** Save SM Usage History For the Day***/
                            SelectBillsEntity smUsageModel = new SelectBillsEntity();
                            smUsageModel.Timestamp = DateTime.Now.ToLocalTime();
                            smUsageModel.JsonResponse = JsonConvert.SerializeObject(mpAccount);
                            smUsageModel.AccountNo = customerBillingAccount.AccNum;
                            SelectBillsEntity.InsertItem(smUsageModel);
                            /*****/

                        }
                        ValidateAccountListAdapter();
                        if (NO_OF_ITARATION == 0)
                        {
                            textLoadMore.Visibility = ViewStates.Gone;
                        }
                        else
                        {
                            textLoadMore.Visibility = ViewStates.Visible;
                        }
                    }
                    else
                    {
                        textLoadMore.Visibility = ViewStates.Gone;
                    }
                }
            }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void GetAccountDueAmountResult(List<MPAccount> accounts)
        {
            try {
            //            List<CustomerBillingAccount> registerdAccounts = CustomerBillingAccount.List();
            if (accounts != null)
            {
                    if (accounts.Count > 0)
                    {
                        accountList.Clear();
                        foreach (MPAccount account in accounts)
                        {
                            accountList.Add(account);                            
                        }
                        ValidateAccountListAdapter();
                        if (NO_OF_ITARATION == 0)
                        {
                            textLoadMore.Visibility = ViewStates.Gone;
                        }
                        else
                        {
                            textLoadMore.Visibility = ViewStates.Visible;
                        }
                    }
                    else
                    {
                        textLoadMore.Visibility = ViewStates.Gone;
                    }
            }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public List<CustomerBillingAccount> GetMoreCustomerAccounts(int index)
        {
            List<CustomerBillingAccount> accountsToReturn = new List<CustomerBillingAccount>();
            try {
            if (registerdAccounts.Count <= 4 && index == 0)
            {
                accountsToReturn.AddRange(registerdAccounts);
                NO_OF_ITARATION = 0;
            }
            else
            {

                //INDEX_COUNTER = index * TOTAL_NUMBER_OF_ITEMS_TO_GET;
                if (INDEX_COUNTER < TOTAL_ACCOUNTS)
                {
                    if (NO_OF_ITARATION == 0)
                    {
                        if(REMAINING_ITEM_COUNT == 0)
                        {
                            REMAINING_ITEM_COUNT = TOTAL_NUMBER_OF_ITEMS_TO_GET;
                        }
                        accountsToReturn.AddRange(registerdAccounts.GetRange(INDEX_COUNTER, REMAINING_ITEM_COUNT));
                    }
                    else
                    {
                        accountsToReturn.AddRange(registerdAccounts.GetRange(INDEX_COUNTER, TOTAL_NUMBER_OF_ITEMS_TO_GET));
                    }
                }
            }
        }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
            return accountsToReturn;
        }

        public bool IsNetworkAvailable()
        {
            ConnectivityManager connectivity = (ConnectivityManager)(Application.Context.ApplicationContext).GetSystemService(Context.ConnectivityService);
            if (connectivity != null)
            {
                NetworkInfo[] info = connectivity.GetAllNetworkInfo();
                if (info != null)
                    for (int i = 0; i < info.Length; i++)
                        if (info[i].GetState() == NetworkInfo.State.Connected)
                        {
                            return true;
                        }

            }
            return false;
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            //base.OnActivityResult(requestCode, resultCode, data);
            if (requestCode == PaymentActivity.SELECT_PAYMENT_ACTIVITY_CODE)
            {
                if (resultCode == Result.Ok)
                {
                    Finish();
                }
            }
        }


        public override void OnTrimMemory(TrimMemory level)
        {
            base.OnTrimMemory(level);

            switch (level)
            {
                case TrimMemory.RunningLow:
                    GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
                    GC.Collect();
                    break;
                default:
                    GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
                    GC.Collect();
                    break;
            }
        }
    }
}