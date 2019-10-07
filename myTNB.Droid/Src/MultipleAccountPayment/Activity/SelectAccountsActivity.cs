using AFollestad.MaterialDialogs;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Net;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.Content;
using Android.Support.V7.Widget;
using Android.Text;
using Android.Text.Method;
using Android.Util;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using CheeseBind;
using Java.Text;
using myTNB_Android.Src.Base;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Base.Models;
using myTNB_Android.Src.Billing.MVP;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.MultipleAccountPayment.Adapter;
using myTNB_Android.Src.MultipleAccountPayment.Model;
using myTNB_Android.Src.MultipleAccountPayment.MVP;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.MyTNBService.Model;
using myTNB_Android.Src.SSMR.Util;
using myTNB_Android.Src.Utils;
using myTNB_Android.Src.Utils.Custom.ProgressDialog;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
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

        private MaterialDialog mWhyThisAmtCardDialog;

        private int TOTAL_ACCOUNTS = 0;
        private int TOTAL_NUMBER_OF_ITEMS_TO_GET = 4;
        private int REMAINING_ITEM_COUNT = 0;
        private int INDEX_COUNTER = 0;
        private int NO_OF_ITARATION = 0;
        private bool firstTime = false;
        private string preSelectedAccount = null;
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
                }

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
                if (TOTAL_ACCOUNTS < 8 && TOTAL_ACCOUNTS > 4)
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
                    //this.userActionsListener.GetMultiAccountDueAmount(Constants.APP_CONFIG.API_KEY_ID, custAccounts, preSelectedAccount);
                    this.userActionsListener.GetAccountsCharges(custAccounts, preSelectedAccount);
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
                    if (IsNetworkAvailable())
                    {
                        INDEX_COUNTER = INDEX_COUNTER + TOTAL_NUMBER_OF_ITEMS_TO_GET;
                        List<string> custAccounts = new List<string>();
                        List<CustomerBillingAccount> list = GetMoreCustomerAccounts(INDEX_COUNTER);
                        foreach (CustomerBillingAccount item in list)
                        {
                            custAccounts.Add(item.AccNum);
                        }
                        //this.userActionsListener.GetMultiAccountDueAmount(Constants.APP_CONFIG.API_KEY_ID, custAccounts, null);
                        this.userActionsListener.GetAccountsCharges(custAccounts, null);
                        NO_OF_ITARATION = NO_OF_ITARATION - 1;
                    }
                    else
                    {
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
            try
            {
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
                    ShowError(this.GetString(Resource.String.error_select_5_accounts));
                }
#if STUB
                else if (position != -2)
                {
                    List<MPAccount> list = adapter.GetSelectedAccounts();
                    Log.Debug("Selected Accounts", " List " + list);
                    MPAccount selectedAccount = list[position];
                    if(selectedAccount.tooltipPopUp)
                    {
                        if(selectedAccount.isSelected && !selectedAccount.isTooltipShow && selectedAccount.OpenChargeTotal != 0)
                        {
                            ShowTooltip(selectedAccount);
                            list[position].isTooltipShow = true;
                        }
                    }
                    UpdateTotal(list);
                }
#endif
                else
                {
                    List<MPAccount> list = adapter.GetSelectedAccounts();
                    if (position >= 0)
                    {
                        MPAccount account = adapter.GetSelectedAccounts()[position];
                        AccountChargeModel model = mPresenter.GetAccountChargeModel(account);
                        if (account.tooltipPopUp)
                        {
                            ShowHasMinimumAmoutToPayTooltip(model);
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

        public void ShowTooltip(MPAccount item)
        {
            try
            {
                mWhyThisAmtCardDialog = new MaterialDialog.Builder(this)
                    .CustomView(Resource.Layout.CustomDialogDoubleButtonLayout, false)
                    .Cancelable(false)
                    .CanceledOnTouchOutside(false)
                    .Build();

                View dialogView = mWhyThisAmtCardDialog.Window.DecorView;
                dialogView.SetBackgroundResource(Android.Resource.Color.Transparent);

                TextView txtItemizedTitle = mWhyThisAmtCardDialog.FindViewById<TextView>(Resource.Id.txtTitle);
                TextView txtItemizedMessage = mWhyThisAmtCardDialog.FindViewById<TextView>(Resource.Id.txtMessage);
                TextView btnGotIt = mWhyThisAmtCardDialog.FindViewById<TextView>(Resource.Id.txtBtnSecond);
                TextView btnBringMeThere = mWhyThisAmtCardDialog.FindViewById<TextView>(Resource.Id.txtBtnFirst);
                txtItemizedMessage.MovementMethod = new ScrollingMovementMethod();
                if (Build.VERSION.SdkInt >= BuildVersionCodes.N)
                {
                    txtItemizedMessage.TextFormatted = string.IsNullOrEmpty(item.MandatoryChargesMessage) ? Html.FromHtml(this.GetString(Resource.String.itemized_bill_third_message), FromHtmlOptions.ModeLegacy) : Html.FromHtml(item.MandatoryChargesMessage, FromHtmlOptions.ModeLegacy);
                }
                else
                {
                    txtItemizedMessage.TextFormatted = string.IsNullOrEmpty(item.MandatoryChargesMessage) ? Html.FromHtml(this.GetString(Resource.String.itemized_bill_third_message)) : Html.FromHtml(item.MandatoryChargesMessage);
                }
                txtItemizedTitle.Text = string.IsNullOrEmpty(item.MandatoryChargesTitle) ? this.GetString(Resource.String.itemized_bill_third_title) : item.MandatoryChargesTitle;
                btnGotIt.Text = string.IsNullOrEmpty(item.MandatoryChargesSecButtonText) ? this.GetString(Resource.String.itemized_bill_got_it) : item.MandatoryChargesSecButtonText;
                btnBringMeThere.Text = string.IsNullOrEmpty(item.MandatoryChargesPriButtonText) ? this.GetString(Resource.String.itemized_bill_bring_me_there) : item.MandatoryChargesPriButtonText;
                TextViewUtils.SetMuseoSans500Typeface(txtItemizedTitle, btnGotIt, btnBringMeThere);
                TextViewUtils.SetMuseoSans300Typeface(txtItemizedMessage);
                btnGotIt.Click += delegate
                {
                    mWhyThisAmtCardDialog.Dismiss();
                };
                btnBringMeThere.Click += delegate
                {
                    mWhyThisAmtCardDialog.Dismiss();
                    try
                    {
                        NavigateBillScreen(item);
                    }
                    catch (System.Exception e)
                    {
                        Utility.LoggingNonFatalError(e);
                    }
                };

                mWhyThisAmtCardDialog.Show();
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void NavigateBillScreen(MPAccount item)
        {
            try
            {
                ShowProgressDialog();
                CustomerBillingAccount customerAccount = CustomerBillingAccount.FindByAccNum(item.accountNumber);
                this.userActionsListener.OnSelectAccount(customerAccount);
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }

        public void ShowDashboardChart(AccountData accountData)
        {
            if (!this.GetIsClicked())
            {
                this.SetIsClicked(true);
                Intent result = new Intent();
                result.PutExtra(Constants.SELECTED_ACCOUNT, JsonConvert.SerializeObject(accountData));
                result.PutExtra(Constants.ITEMZIED_BILLING_VIEW_KEY, true);
                SetResult(Result.FirstUser, result);
                Finish();
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
#if STUB
                    if (account.OpenChargeTotal != 0)
                    {
                        total += account.OpenChargeTotal;
                    }
#endif
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
                .SetAction("Close", delegate { mErrorMessageSnackBar.Dismiss(); }
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
            try
            {
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

        public void SetAccountsDueAmountResult(List<MPAccount> updatedAccountList)
        {
            accountList.Clear();
            accountList.AddRange(updatedAccountList);
            ValidateAccountListAdapter();
            if (NO_OF_ITARATION <= 0 && REMAINING_ITEM_COUNT == 0)
            {
                textLoadMore.Visibility = ViewStates.Gone;
            }
            else
            {
                textLoadMore.Visibility = ViewStates.Visible;
            }
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
                            foreach (myTNB_Android.Src.MultipleAccountPayment.Model.MPAccountDueResponse.Account account in response.accountDueAmountResponse.accounts)
                            {
                                CustomerBillingAccount customerBillingAccount = CustomerBillingAccount.FindByAccNum(account.accNum);
                                double dueAmount = account.amountDue;

                                MPAccount mpAccount = new MPAccount()
                                {
                                    accountLabel = customerBillingAccount.AccDesc,
                                    accountNumber = customerBillingAccount.AccNum,
                                    accountAddress = customerBillingAccount.AccountStAddress,
                                    isSelected = selectedAccount.AccountNum.Equals(customerBillingAccount.AccNum) ? true && dueAmount > 0 : false,
                                    isTooltipShow = false,
#if STUB
                                    OpenChargeTotal = account.OpenChargesTotal == 0.00 ? 0.00 : account.OpenChargesTotal,
#else
                                    OpenChargeTotal = 0.00,
#endif
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

        public void ShowHasMinimumAmoutToPayTooltip(AccountChargeModel accountChargeModel)
        {
            if (accountChargeModel.MandatoryCharges.TotalAmount > 0f)
            {
                BillMandatoryChargesTooltipModel mandatoryTooltipModel = MyTNBAppToolTipData.GetInstance().GetMandatoryPaymentTooltipData();
                List<string> ctaList = mandatoryTooltipModel.CTA.Split(',').ToList();
                MyTNBAppToolTipBuilder.Create(this, MyTNBAppToolTipBuilder.ToolTipType.NORMAL_WITH_HEADER_TWO_BUTTON)
                    .SetTitle(mandatoryTooltipModel.Title)
                    .SetMessage(string.Format(mandatoryTooltipModel.Description, "RM"+ accountChargeModel.MandatoryCharges.TotalAmount.ToString("#,##0.00")))
                    .SetCTALabel(ctaList[0])
                    .SetCTAaction(()=> { ShowBillingDetails(accountChargeModel); })
                    .SetSecondaryCTALabel(ctaList[1])
                    .Build().Show();
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
                StartActivity(intent);
            }
        }
    }
}
