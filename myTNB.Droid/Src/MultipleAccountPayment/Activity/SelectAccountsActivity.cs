using AFollestad.MaterialDialogs;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Net;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using AndroidX.Core.Content;
using AndroidX.RecyclerView.Widget;
using CheeseBind;
using Google.Android.Material.Snackbar;
using myTNB.Android.Src.Base;
using myTNB.Android.Src.Base.Activity;
using myTNB.Android.Src.Base.Models;
using myTNB.Android.Src.Billing.MVP;
using myTNB.Android.Src.Database.Model;
using myTNB.Android.Src.MultipleAccountPayment.Adapter;
using myTNB.Android.Src.MultipleAccountPayment.Model;
using myTNB.Android.Src.MultipleAccountPayment.MVP;
using myTNB.Android.Src.myTNBMenu.Activity;
using myTNB.Android.Src.myTNBMenu.Models;
using myTNB.Android.Src.MyTNBService.Model;
using myTNB.Android.Src.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime;

namespace myTNB.Android.Src.MultipleAccountPayment.Activity
{
    [Activity(Label = "Select Bill(s)"
       , ScreenOrientation = ScreenOrientation.Portrait, Theme = "@style/Theme.LinkAccount",
        WindowSoftInputMode = SoftInput.AdjustPan)]
    public class SelectAccountsActivity : BaseActivityCustom, MPSelectAccountsContract.IView
    {
        private readonly string TAG = "SelectAccountsActivity";
        private MPSelectAccountsPresenter mPresenter;
        private MPSelectAccountsContract.IUserActionsListener userActionsListener;
        public static SelectAccountsActivity selectAccountsActivity;
        private Snackbar mErrorMessageSnackBar;
        private MaterialDialog mGetDueAmountDialog;
        private MaterialDialog mWhyThisAmtCardDialog;
        private int TOTAL_ACCOUNTS = 0;
        private int TOTAL_NUMBER_OF_ITEMS_TO_GET = 4;
        private int REMAINING_ITEM_COUNT = 0;
        private int INDEX_COUNTER = 0;
        private int NO_OF_ITARATION = 0;
        private bool firstTime = false;
        private string preSelectedAccount = null;
        private bool isMinimumAmountTooltipShown = false;
        private string PAGE_ID = "SelectBills";
        private bool FromFloatingButtonMarketing = false;

        RecyclerView.LayoutManager layoutManager;
        SelectAccountListAdapter adapter;
        List<MPAccount> accountList = new List<MPAccount>();
        List<CustomerBillingAccount> registerdAccounts;
        AccountData selectedAccount;

        [BindView(Resource.Id.account_list_recycler_view)]
        RecyclerView accountListRecyclerView;

        [BindView(Resource.Id.baseView)]
        FrameLayout rootView;

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

            try
            {
                // Create your application here
                mPresenter = new MPSelectAccountsPresenter(this);
                selectAccountsActivity = this;

                Bundle extras = Intent.Extras;

                if (extras != null)
                {
                    if (extras.ContainsKey(Constants.SELECTED_ACCOUNT))
                    {
                        selectedAccount = DeSerialze<AccountData>(extras.GetString(Constants.SELECTED_ACCOUNT));
                        preSelectedAccount = selectedAccount.AccountNum;
                    }

                    if (extras.ContainsKey(Constants.FROM_BILL_DETAILS_PAGE))
                    {
                        mPresenter.isFromBillDetails = extras.GetBoolean(Constants.FROM_BILL_DETAILS_PAGE);
                    }

                    if (Intent.HasExtra("FromFloatingButtonMarketing"))
                    {
                        FromFloatingButtonMarketing = Intent.Extras.GetBoolean("FromFloatingButtonMarketing", false);
                    }
                }

                registerdAccounts = CustomerBillingAccount.List();
                if (selectedAccount != null)
                {
                    var found = registerdAccounts.Where(x => x.AccNum == selectedAccount.AccountNum).FirstOrDefault();
                    if (found != null)
                    {
                        registerdAccounts.Remove(found);
                    }
                }
                else if (registerdAccounts != null && registerdAccounts.Count > 0)
                {
                    if (CustomerBillingAccount.HasSelected())
                    {
                        CustomerBillingAccount preSelected = CustomerBillingAccount.GetSelected();
                        if (preSelected.AccountCategoryId == "2")
                        {
                            preSelected = registerdAccounts.Find(x => x.AccountCategoryId != "2");
                            selectedAccount = AccountData.Copy(preSelected, false);
                            preSelectedAccount = selectedAccount.AccountNum;
                            var found = registerdAccounts.Where(x => x.AccNum == selectedAccount.AccountNum).FirstOrDefault();
                            if (found != null)
                            {
                                registerdAccounts.Remove(found);
                            }
                        }
                        else
                        {
                            selectedAccount = AccountData.Copy(preSelected, true);
                            preSelectedAccount = selectedAccount.AccountNum;
                            var found = registerdAccounts.Where(x => x.AccNum == selectedAccount.AccountNum).FirstOrDefault();
                            if (found != null)
                            {
                                registerdAccounts.Remove(found);
                            }
                        }
                    }
                    else
                    {
                        CustomerBillingAccount preSelected = registerdAccounts.Find(x => x.AccountCategoryId != "2");
                        CustomerBillingAccount.SetSelected(preSelected.AccNum);
                        selectedAccount = AccountData.Copy(preSelected, true);
                        preSelectedAccount = selectedAccount.AccountNum;
                        var found = registerdAccounts.Where(x => x.AccNum == selectedAccount.AccountNum).FirstOrDefault();
                        if (found != null)
                        {
                            registerdAccounts.Remove(found);
                        }
                    }
                }

                registerdAccounts.RemoveAll(x => x.AccountCategoryId == "2");

                TOTAL_ACCOUNTS = registerdAccounts.Count;
                NO_OF_ITARATION = TOTAL_ACCOUNTS / TOTAL_NUMBER_OF_ITEMS_TO_GET;
                if (NO_OF_ITARATION == 0)
                {
                    REMAINING_ITEM_COUNT = 0;
                }
                else
                {
                    REMAINING_ITEM_COUNT = TOTAL_ACCOUNTS - (NO_OF_ITARATION * TOTAL_NUMBER_OF_ITEMS_TO_GET);
                }
                if (TOTAL_ACCOUNTS < 8 && TOTAL_ACCOUNTS > 4)
                {
                    NO_OF_ITARATION = NO_OF_ITARATION + 1;
                }


                TextViewUtils.SetMuseoSans300Typeface(textTotalPayable);
                TextViewUtils.SetMuseoSans500Typeface(textTotalPayableCurrency, textTotalPayableTitle);
                TextViewUtils.SetMuseoSans500Typeface(btnPayBill);
                TextViewUtils.SetTextSize14(textTotalPayableCurrency);
                TextViewUtils.SetTextSize16(textTotalPayableTitle, btnPayBill);
                TextViewUtils.SetTextSize24(textTotalPayable);

                SetStatusBarBackground(Resource.Drawable.UsageGradientBackground);
                SetToolbarBackground(Resource.Drawable.CustomDashboardGradientToolbar);

                btnPayBill.Text = GetLabelByLanguage("paySingle");
                textTotalPayableTitle.Text = GetLabelCommonByLanguage("totalAmount");

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
                    if (selectedAccount != null)
                    {
                        custAccounts.Add(selectedAccount.AccountNum);
                    }
                    List<CustomerBillingAccount> list = GetMoreCustomerAccounts(INDEX_COUNTER);
                    foreach (CustomerBillingAccount item in list)
                    {
                        custAccounts.Add(item.AccNum);
                    }
                    firstTime = true;
                    this.userActionsListener.GetAccountsCharges(custAccounts, preSelectedAccount);
                }

                accountListRecyclerView.SetLayoutManager(layoutManager);
                accountListRecyclerView.SetAdapter(adapter);
                adapter.CheckChanged += OnCheckChanged;

                btnPayBill.Click += delegate
                {
                    NavigateToPayment();
                };

                adapter.SetShowMoreAction(OnShowMore);

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
            try
            {
                adapter.EnableShowMoreButton(!(NO_OF_ITARATION <= 0 && REMAINING_ITEM_COUNT == 0));
                adapter.AddAccounts(accountList);
                UpdateTotal(adapter.GetSelectedAccounts());
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        void OnCheckChanged(object sender, int position)
        {
            try
            {
                if (position == -1)
                {
                    MyTNBAppToolTipBuilder.Create(this, MyTNBAppToolTipBuilder.ToolTipType.NORMAL)
                        .SetMessage(Utility.GetLocalizedCommonLabel("selectBillMaxDesc"))
                        .SetCTALabel(Utility.GetLocalizedCommonLabel("gotIt"))
                        .Build().Show();
                }
                else
                {
                    List<MPAccount> list = adapter.GetSelectedAccounts();
                    if (position >= 0)
                    {
                        MPAccount account = adapter.GetSelectedAccounts()[position];
                        AccountChargeModel model = mPresenter.GetAccountChargeModel(account);
                        if (account.tooltipPopUp)
                        {
                            ShowHasMinimumAmoutToPayTooltip(account, model);
                        }
                    }
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
            try
            {
                double total = 0;
                foreach (MPAccount account in selectedAccounts)
                {
                    total += account.amount;
                }
                CultureInfo currCult = CultureInfo.CreateSpecificCulture("en-US");
                textTotalPayable.Text = total.ToString("#,##0.00", currCult);
                if (selectedAccounts.Count > 0)
                {
                    btnPayBill.Text = string.Format(GetLabelByLanguage("payMultiple"), selectedAccounts.Count);
                    EnablePayButton();
                }
                else
                {
                    btnPayBill.Text = GetLabelByLanguage("paySingle");
                    DisablePayButton();
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void ShowError(string messge)
        {
            try
            {
                if (mErrorMessageSnackBar != null && mErrorMessageSnackBar.IsShown)
                {
                    mErrorMessageSnackBar.Dismiss();
                }

                mErrorMessageSnackBar = Snackbar.Make(rootView, messge, Snackbar.LengthIndefinite)
                .SetAction(Utility.GetLocalizedCommonLabel("close"), delegate { mErrorMessageSnackBar.Dismiss(); }
                );
                View v = mErrorMessageSnackBar.View;
                TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
                tv.SetMaxLines(5);

                mErrorMessageSnackBar.Show();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void NavigateToPayment()
        {
            try
            {
                if (adapter.IsAllAmountValid())
                {
                    if (!this.GetIsClicked())
                    {
                        this.SetIsClicked(true);
                        Intent payment_activity = new Intent(this, typeof(PaymentActivity));
                        if (selectedAccount == null)
                        {
                            selectedAccount = AccountData.Copy(CustomerBillingAccount.FindByAccNum(adapter.GetSelectedAccounts()[0].accountNumber), true);
                        }
                        payment_activity.PutExtra(Constants.SELECTED_ACCOUNT, JsonConvert.SerializeObject(selectedAccount));
                        payment_activity.PutExtra("PAYMENT_ITEMS", JsonConvert.SerializeObject(adapter.GetSelectedAccounts()));
                        List<AccountChargeModel> chargeModelList = mPresenter.GetSelectedAccountChargesModelList(adapter.GetSelectedAccounts());
                        payment_activity.PutExtra("ACCOUNT_CHARGES_LIST", JsonConvert.SerializeObject(chargeModelList));
                        payment_activity.PutExtra("TOTAL", textTotalPayable.Text);
                        StartActivityForResult(payment_activity, PaymentActivity.SELECT_PAYMENT_ACTIVITY_CODE);
                    }
                }
                else
                {
                    Log.Debug(TAG, "Enter valid amount.");
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public override void OnBackPressed()
        {
            base.OnBackPressed();
            if (FromFloatingButtonMarketing)
                ShowDashboard();
            else
                this.Finish();
        }

        public void ShowDashboard()
        {
            Intent DashboardIntent = new Intent(this, typeof(DashboardHomeActivity));
            DashboardIntent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.ClearTask | ActivityFlags.NewTask);
            StartActivity(DashboardIntent);
        }


        public void IsValidAmount(double amt)
        {
            try
            {
                if (amt > 5000)
                {
                    ShowError(this.GetString(Resource.String.error_credit_card_limit));
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void DisablePayButton()
        {
            try
            {
                btnPayBill.Enabled = false;
                btnPayBill.Background = ContextCompat.GetDrawable(this, Resource.Drawable.silver_chalice_button_background);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void EnablePayButton()
        {
            try
            {
                btnPayBill.Enabled = true;
                btnPayBill.Background = ContextCompat.GetDrawable(this, Resource.Drawable.green_button_background);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
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

        public void HideProgressDialog()
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

        public void SetAccountsDueAmountResult(List<MPAccount> updatedAccountList)
        {
            accountList.Clear();
            accountList.AddRange(updatedAccountList);
            ValidateAccountListAdapter();
        }

        public void GetAccountDueAmountResult(MPAccountDueResponse response)
        {
            try
            {
                if (response != null)
                {
                    if (response.accountDueAmountResponse.accounts != null)
                    {
                        if (response.accountDueAmountResponse.accounts.Count > 0)
                        {
                            accountList.Clear();
                            foreach (myTNB.Android.Src.MultipleAccountPayment.Model.MPAccountDueResponse.Account account in response.accountDueAmountResponse.accounts)
                            {
                                CustomerBillingAccount customerBillingAccount = CustomerBillingAccount.FindByAccNum(account.accNum);
                                double dueAmount = account.amountDue;

                                MPAccount mpAccount = new MPAccount()
                                {
                                    accountLabel = customerBillingAccount.AccDesc,
                                    accountNumber = customerBillingAccount.AccNum,
                                    accountAddress = customerBillingAccount.AccountStAddress,
                                    isSelected = (selectedAccount != null && selectedAccount.AccountNum.Equals(customerBillingAccount.AccNum)) ? true && dueAmount > 0 : false,
                                    isTooltipShow = false,
                                    OpenChargeTotal = 0.00,
                                    amount = dueAmount,
                                    MandatoryChargesTitle = response.accountDueAmountResponse.MandatoryChargesTitle,
                                    MandatoryChargesMessage = response.accountDueAmountResponse.MandatoryChargesMessage,
                                    MandatoryChargesPriButtonText = response.accountDueAmountResponse.MandatoryChargesPriButtonText,
                                    MandatoryChargesSecButtonText = response.accountDueAmountResponse.MandatoryChargesSecButtonText,
                                    orgAmount = dueAmount
                                };
                                accountList.Add(mpAccount);

                                /*** Save SM Usage History For the Day***/
                                SelectBillsEntity smUsageModel = new SelectBillsEntity();
                                smUsageModel.Timestamp = DateTime.Now.ToLocalTime();
                                smUsageModel.JsonResponse = JsonConvert.SerializeObject(mpAccount);
                                smUsageModel.AccountNo = customerBillingAccount.AccNum;
                                SelectBillsEntity.InsertItem(smUsageModel);
                                /*****/

                            }
                            ValidateAccountListAdapter();
                        }
                    }
                }
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
                FirebaseAnalyticsUtils.SetScreenName(this, "Select Bills");
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        protected override void OnPause()
        {
            base.OnPause();
        }

        public void GetAccountDueAmountResult(List<MPAccount> accounts)
        {
            try
            {
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
            try
            {
                if (registerdAccounts.Count <= 4 && index == 0)
                {
                    accountsToReturn.AddRange(registerdAccounts);
                    NO_OF_ITARATION = 0;
                }
                else
                {
                    if (INDEX_COUNTER < TOTAL_ACCOUNTS)
                    {
                        if (NO_OF_ITARATION == 0)
                        {
                            if (REMAINING_ITEM_COUNT == 0)
                            {
                                REMAINING_ITEM_COUNT = TOTAL_NUMBER_OF_ITEMS_TO_GET;
                            }
                            accountsToReturn.AddRange(registerdAccounts.GetRange(INDEX_COUNTER, REMAINING_ITEM_COUNT));
                            REMAINING_ITEM_COUNT = 0;
                        }
                        else
                        {
                            int EndIndex = (INDEX_COUNTER + TOTAL_NUMBER_OF_ITEMS_TO_GET) - 1;
                            if (EndIndex >= registerdAccounts.Count)
                            {
                                int ExpectedItemCount = TOTAL_NUMBER_OF_ITEMS_TO_GET - (EndIndex - (registerdAccounts.Count - 1));
                                accountsToReturn.AddRange(registerdAccounts.GetRange(INDEX_COUNTER, ExpectedItemCount));
                                REMAINING_ITEM_COUNT = 0;
                            }
                            else
                            {
                                accountsToReturn.AddRange(registerdAccounts.GetRange(INDEX_COUNTER, TOTAL_NUMBER_OF_ITEMS_TO_GET));
                            }
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
            try
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
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
            return false;
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            try
            {
                if (requestCode == PaymentActivity.SELECT_PAYMENT_ACTIVITY_CODE)
                {
                    if (resultCode == Result.Ok)
                    {
                        Finish();
                    }
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }


        public override void OnTrimMemory(TrimMemory level)
        {
            try
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
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void ShowHasMinimumAmoutToPayTooltip(MPAccount account, AccountChargeModel accountChargeModel)
        {
            if (!isMinimumAmountTooltipShown)
            {
                if (accountChargeModel.MandatoryCharges.TotalAmount > 0f)
                {
                    CultureInfo currCult = CultureInfo.CreateSpecificCulture("en-US");
                    BillMandatoryChargesTooltipModel mandatoryTooltipModel = MyTNBAppToolTipData.GetInstance().GetMandatoryPaymentTooltipData();
                    List<string> ctaList = mandatoryTooltipModel.CTA.Split(',').ToList();
                    string accountId = string.IsNullOrEmpty(account.accountLabel) ? account.accountNumber : account.accountLabel;
                    MyTNBAppToolTipBuilder.Create(this, MyTNBAppToolTipBuilder.ToolTipType.NORMAL_WITH_HEADER_TWO_BUTTON)
                        .SetTitle(mandatoryTooltipModel.Title)
                        .SetMessage(string.Format(mandatoryTooltipModel.Description, "RM" + accountChargeModel.MandatoryCharges.TotalAmount.ToString("#,##0.00", currCult), accountId))
                        .SetCTALabel(ctaList[0])
                        .SetCTAaction(() => { ShowBillingDetails(accountChargeModel); })
                        .SetSecondaryCTALabel(ctaList[1])
                        .Build().Show();
                }
                isMinimumAmountTooltipShown = true;
            }
        }

        private void ShowBillingDetails(AccountChargeModel accountChargeModel)
        {
            if (!this.GetIsClicked())
            {
                this.SetIsClicked(true);
                CustomerBillingAccount customerBillingAccount = CustomerBillingAccount.FindByAccNum(accountChargeModel.ContractAccount);
                AccountData selectedAccountData = new AccountData();
                selectedAccountData.AccountNum = accountChargeModel.ContractAccount;
                selectedAccountData.AccountNickName = customerBillingAccount.AccDesc;
                selectedAccountData.AddStreet = customerBillingAccount.AccountStAddress;
                selectedAccountData.AccountCategoryId = customerBillingAccount.AccountCategoryId;

                Intent intent = new Intent(this, typeof(BillingDetailsActivity));
                intent.PutExtra("SELECTED_ACCOUNT", JsonConvert.SerializeObject(selectedAccountData));
                intent.PutExtra("SELECTED_BILL_DETAILS", JsonConvert.SerializeObject(accountChargeModel));
                intent.PutExtra("PEEK_BILL_DETAILS", true);
                StartActivity(intent);
            }
        }

        public override string GetPageId()
        {
            return PAGE_ID;
        }

        public void OnShowMore()
        {
            if (IsNetworkAvailable())
            {
                INDEX_COUNTER = INDEX_COUNTER + TOTAL_NUMBER_OF_ITEMS_TO_GET;
                List<string> custAccounts = new List<string>();
                List<CustomerBillingAccount> list = GetMoreCustomerAccounts(INDEX_COUNTER);
                foreach (CustomerBillingAccount item in list)
                {
                    custAccounts.Add(item.AccNum);
                }
                this.userActionsListener.GetAccountsCharges(custAccounts, null);
                NO_OF_ITARATION = NO_OF_ITARATION - 1;
            }
            else
            {
                ShowError(this.GetString(Resource.String.dashboard_chartview_no_internet_content));
            }
        }
    }
}
