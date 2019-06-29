﻿using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.Content;
using Android.Text;
using Android.Views;
using Android.Widget;
using CheeseBind;
using Java.Lang;
using Java.Text;
using Java.Util;
using myTNB_Android.Src.Base.Fragments;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.MultipleAccountPayment.Activity;
using myTNB_Android.Src.myTNBMenu.Activity;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.myTNBMenu.MVP.Fragment;
using myTNB_Android.Src.Utils;
using myTNB_Android.Src.Utils.Custom.ProgressDialog;
using Newtonsoft.Json;
using System;

namespace myTNB_Android.Src.myTNBMenu.Fragments.BillsMenu
{
    public class BillsMenuFragment : BaseFragment, BillsPaymentFragmentContract.IView
    {

        private AccountData selectedAccount;

        [BindView(Resource.Id.rootView)]
        CoordinatorLayout rootView;

        [BindView(Resource.Id.txtAccountName)]
        TextView txtAccountName;

        [BindView(Resource.Id.txtAccountNum)]
        TextView txtAccountNum;

        [BindView(Resource.Id.txtAddress)]
        TextView txtAddress;

        [BindView(Resource.Id.txtCurrentBill)]
        TextView txtCurrentBill;

        [BindView(Resource.Id.txtCurrentChargesTitle)]
        TextView txtCurrentChargesTitle;

        [BindView(Resource.Id.txtCurrentChargesContent)]
        TextView txtCurrentChargesContent;

        [BindView(Resource.Id.txtCurrentChargesRM)]
        TextView txtCurrentChargesRM;

        [BindView(Resource.Id.txtOutstandingChargesTitle)]
        TextView txtOutstandingChargesTitle;

        [BindView(Resource.Id.txtOutstandingChargesContent)]
        TextView txtOutstandingChargesContent;

        [BindView(Resource.Id.txtOutstandingChargesRM)]
        TextView txtOutstandingChargesRM;

        [BindView(Resource.Id.txtTotalPayableTitle)]
        TextView txtTotalPayableTitle;

        [BindView(Resource.Id.txtTotalPayableContent)]
        TextView txtTotalPayableContent;

        [BindView(Resource.Id.txtTotalPayableRM)]
        TextView txtTotalPayableRM;

        [BindView(Resource.Id.txtTotalDueTitle)]
        TextView txtTotalDueTitle;

        [BindView(Resource.Id.txtDueDate)]
        TextView txtDueDate;

        [BindView(Resource.Id.txtCurrency)]
        TextView txtCurrency;

        [BindView(Resource.Id.txtTotalDueContent)]
        TextView txtTotalDueContent;

        [BindView(Resource.Id.btnPay)]
        Button btnPay;

        [BindView(Resource.Id.btnBills)]
        RadioButton btnBills;

        [BindView(Resource.Id.btnPayment)]
        RadioButton btnPayment;

        [BindView(Resource.Id.tabButtonLayout)]
        RadioGroup tabButtonLayout;

        [BindView(Resource.Id.imgLeaf)]
        ImageView imgLeaf;

        [BindView(Resource.Id.txtBillPaymentHistoryTitle)]
        TextView txtBillPaymentHistoryTitle;

        BillsPaymentFragmentPresenter mPresenter;
        BillsPaymentFragmentContract.IUserActionsListener userActionsListener;

        [BindView(Resource.Id.txtFooter)]
        TextView txtFooter;

        [BindView(Resource.Id.txtFooter1)]
        TextView txtFooter1;

        [BindView(Resource.Id.txtFooter2)]
        TextView txtFooter2;

        [BindView(Resource.Id.weblinkLayout)]
        LinearLayout webLinkLayout;

        [BindView(Resource.Id.onlyAccountOwnersLayout)]
        LinearLayout onlyAccountOwnersLayout;

        [BindView(Resource.Id.currentChangeLayout)]
        RelativeLayout currentChangeLayout;

        [BindView(Resource.Id.outstandingChangeLayout)]
        RelativeLayout outstandingChangeLayout;

        [BindView(Resource.Id.totalPayableLayout)]
        RelativeLayout totalPayableLayout;

        [BindView(Resource.Id.divider)]
        View divider;

        [BindView(Resource.Id.layout_bill_total)]
        LinearLayout allBillLayout;

        [BindView(Resource.Id.layout_api_refresh)]
        LinearLayout refreshLayout;

        [BindView(Resource.Id.btnRefresh)]
        Button btnNewRefresh;

        [BindView(Resource.Id.refresh_content)]
        TextView txtNewRefreshMessage;

        private LoadingOverlay loadingOverlay;


        bool noInternet = false;

        bool failedFetch = false;

        private string txtRefreshContent = "";
        private string txtBtnContent = "";

        DecimalFormat decimalFormatter = new DecimalFormat("###,###,###,###,##0.00");
        SimpleDateFormat dateParser = new SimpleDateFormat("dd/MM/yyyy");
        SimpleDateFormat dateFormatter = new SimpleDateFormat("dd MMM yyyy");

        internal static BillsMenuFragment NewInstance(AccountData selectedAccount)
        {
            BillsMenuFragment billsMenuFragment = new BillsMenuFragment();
            Bundle args = new Bundle();
            args.PutString(Constants.SELECTED_ACCOUNT, JsonConvert.SerializeObject(selectedAccount));
            billsMenuFragment.Arguments = args;
            return billsMenuFragment;
        }


        internal static BillsMenuFragment NewInstance(AccountData selectedAccount, bool noInternet)
        {
            BillsMenuFragment billsMenuFragment = new BillsMenuFragment();
            Bundle args = new Bundle();
            args.PutString(Constants.SELECTED_ACCOUNT, JsonConvert.SerializeObject(selectedAccount));
            args.PutBoolean(Constants.NO_INTERNET_CONNECTION, noInternet);
            billsMenuFragment.Arguments = args;
            return billsMenuFragment;
        }

        internal static BillsMenuFragment NewInstance(string contextTxt, string btnTxt, AccountData selectedAccount)
        {
            BillsMenuFragment billsMenuFragment = new BillsMenuFragment();
            Bundle args = new Bundle();
            args.PutString(Constants.SELECTED_ACCOUNT, JsonConvert.SerializeObject(selectedAccount));
            args.PutBoolean(Constants.REFRESH_MODE, true);
            args.PutString(Constants.REFRESH_MSG, contextTxt);
            args.PutString(Constants.REFRESH_BTN_MSG, btnTxt);
            billsMenuFragment.Arguments = args;
            return billsMenuFragment;
        }


        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Bundle args = Arguments;
            if (args.ContainsKey(Constants.SELECTED_ACCOUNT))
            {
                selectedAccount = JsonConvert.DeserializeObject<AccountData>(args.GetString(Constants.SELECTED_ACCOUNT));
            }

            if (args.ContainsKey(Constants.NO_INTERNET_CONNECTION))
            {
                noInternet = args.GetBoolean(Constants.NO_INTERNET_CONNECTION);
            }

            failedFetch = false;

            if (args.ContainsKey(Constants.REFRESH_MODE))
            {
                failedFetch = true;
                if (args.ContainsKey(Constants.REFRESH_MSG))
                {
                    txtRefreshContent = args.GetString(Constants.REFRESH_MSG);
                }

                if (args.ContainsKey(Constants.REFRESH_BTN_MSG))
                {
                    txtBtnContent = args.GetString(Constants.REFRESH_BTN_MSG);
                }
            }

            mPresenter = new BillsPaymentFragmentPresenter(this, selectedAccount);

        }

        public override void OnAttach(Context context)
        {

            try
            {
                if (context is DashboardActivity)
                {
                    var activity = context as DashboardActivity;
                    activity.Window.SetBackgroundDrawable(Activity.GetDrawable(Resource.Drawable.HorizontalGradientBackground));
                }
            }
            catch (ClassCastException e)
            {

            }
            base.OnAttach(context);
        }

        public override int ResourceId()
        {
            return Resource.Layout.BillsView;
        }

        [OnClick(Resource.Id.btnRefresh)]
        internal void OnRefresh(object sender, EventArgs e)
        {
            CustomerBillingAccount customerAccount = CustomerBillingAccount.FindByAccNum(selectedAccount.AccountNum);
            this.userActionsListener.OnSelectAccount(customerAccount);
        }

        public void ShowDashboardChart(UsageHistoryResponse response, AccountData accountData)
        {
            try
            {
                ((DashboardActivity)Activity).BillsMenuRefresh(accountData);
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void ShowView()
        {
            allBillLayout.Visibility = ViewStates.Visible;
            refreshLayout.Visibility = ViewStates.Gone;
        }

        public void ShowRefreshView(string contentTxt, string btnTxt)
        {
            try
            {
                allBillLayout.Visibility = ViewStates.Gone;
                refreshLayout.Visibility = ViewStates.Visible;
                btnNewRefresh.Text = string.IsNullOrEmpty(btnTxt) ? GetString(Resource.String.text_new_refresh) : btnTxt;

                if (Build.VERSION.SdkInt >= BuildVersionCodes.N)
                {
                    txtNewRefreshMessage.TextFormatted = string.IsNullOrEmpty(contentTxt) ? Html.FromHtml(GetString(Resource.String.text_new_refresh_content), FromHtmlOptions.ModeLegacy) : Html.FromHtml(contentTxt, FromHtmlOptions.ModeLegacy);
                }
                else
                {
                    txtNewRefreshMessage.TextFormatted = string.IsNullOrEmpty(contentTxt) ? Html.FromHtml(GetString(Resource.String.text_new_refresh_content)) : Html.FromHtml(contentTxt);
                }
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnActivityCreated(savedInstanceState);


            try
            {

                TextViewUtils.SetMuseoSans500Typeface(txtAccountName,
                  txtCurrentBill,
                  txtCurrentChargesTitle,
                  txtCurrentChargesContent,
                  txtOutstandingChargesTitle,
                  txtOutstandingChargesContent,
                  txtTotalPayableTitle,
                  txtDueDate,
                  txtCurrency,
                  btnPay,
                  txtBillPaymentHistoryTitle,
                  txtTotalDueContent,
                  txtTotalDueTitle,
                  btnNewRefresh);
                TextViewUtils.SetMuseoSans300Typeface(txtAccountNum,
                    txtAddress,
                    txtTotalPayableContent,
                    txtFooter, txtFooter1, txtFooter2,
                    txtCurrentChargesRM, txtOutstandingChargesRM, txtTotalPayableRM,
                    txtNewRefreshMessage
                    );

                allBillLayout.Visibility = ViewStates.Visible;
                refreshLayout.Visibility = ViewStates.Gone;

                btnNewRefresh.Text = string.IsNullOrEmpty(txtBtnContent) ? GetString(Resource.String.text_new_refresh) : txtBtnContent;

                if (Build.VERSION.SdkInt >= BuildVersionCodes.N)
                {
                    txtNewRefreshMessage.TextFormatted = string.IsNullOrEmpty(txtRefreshContent) ? Html.FromHtml(GetString(Resource.String.text_new_refresh_content), FromHtmlOptions.ModeLegacy) : Html.FromHtml(txtRefreshContent, FromHtmlOptions.ModeLegacy);
                }
                else
                {
                    txtNewRefreshMessage.TextFormatted = string.IsNullOrEmpty(txtRefreshContent) ? Html.FromHtml(GetString(Resource.String.text_new_refresh_content)) : Html.FromHtml(txtRefreshContent);
                }

                if(failedFetch)
                {
                    ShowRefreshView(null, null);
                }

                SetBillDetails(selectedAccount);

                this.userActionsListener.Start();
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }


        [OnClick(Resource.Id.btnBills)]
        void OnBills(object sender, EventArgs eventArgs)
        {
            this.userActionsListener.OnBillTab();
        }

        [OnClick(Resource.Id.btnPayment)]
        void OnPayment(object sender, EventArgs eventArgs)
        {
            this.userActionsListener.OnPaymentTab();
        }

        public void ToggleFetch(bool yesno)
        {
            failedFetch = yesno;
        }

        public void ShowBillsList(BillHistoryResponseV5 billsHistoryResponse)
        {

            ChildFragmentManager.BeginTransaction()
                .Replace(Resource.Id.layoutReplacement, BillingListFragment.NewInstance(billsHistoryResponse, selectedAccount))
                .CommitAllowingStateLoss();
        }

        public void ShowPaymentList(PaymentHistoryResponseV5 paymentHistoryResponse)
        {
            ChildFragmentManager.BeginTransaction()
                .Replace(Resource.Id.layoutReplacement, PaymentListFragment.NewInstance(paymentHistoryResponse, selectedAccount))
                .CommitAllowingStateLoss();
        }

        public void SetPresenter(BillsPaymentFragmentContract.IUserActionsListener userActionListener)
        {
            this.userActionsListener = userActionListener;
        }

        public bool IsActive()
        {
            return IsAdded && IsVisible && !IsDetached && !IsRemoving;
        }

        [OnClick(Resource.Id.btnPay)]
        void OnPay(object sender, EventArgs args)
        {
            this.userActionsListener.OnPay();
        }

        public void EnableTabs()
        {
            btnBills.Enabled = true;
            btnPayment.Enabled = true;
        }

        public void DisableTabs()
        {
            btnBills.Enabled = false;
            btnPayment.Enabled = false;
        }

        public void ShowPayment()
        {
            Intent payment_activity = new Intent(this.Activity, typeof(SelectAccountsActivity));
            payment_activity.PutExtra(Constants.SELECTED_ACCOUNT, JsonConvert.SerializeObject(selectedAccount));
            StartActivityForResult(payment_activity, DashboardActivity.PAYMENT_RESULT_CODE);
        }

        public override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            if (requestCode == DashboardActivity.PAYMENT_RESULT_CODE)
            {
                if (resultCode == Result.Ok)
                {
                    if (Activity is DashboardActivity)
                    {
                        ((DashboardActivity)Activity).OnTapRefresh();
                    }
                }
            }
        }

        public bool HasNoInternet()
        {
            return noInternet && !ConnectionUtils.HasInternetConnection(this.Activity);
        }

        public void ShowNoInternet()
        {
            ChildFragmentManager.BeginTransaction()
                .Replace(Resource.Id.layoutReplacement, new BillingPaymentNoInternetConnection())
                .CommitAllowingStateLoss();
        }

        public void ShowEmptyBillList()
        {
            ChildFragmentManager.BeginTransaction()
                .Replace(Resource.Id.layoutReplacement, new BillingListFragment())
                .CommitAllowingStateLoss();
        }

        public void ShowEmptyPaymentList()
        {
            ChildFragmentManager.BeginTransaction()
                .Replace(Resource.Id.layoutReplacement, new PaymentListFragment())
                .CommitAllowingStateLoss();
        }

        public void ShowAccountRE()
        {
            btnPay.Visibility = ViewStates.Gone;
            tabButtonLayout.Visibility = ViewStates.Visible;
            btnBills.Text = GetString(Resource.String.bill_menu_payment_advice);
            txtBillPaymentHistoryTitle.Text = GetString(Resource.String.bill_menu_payment_advice_history);
            imgLeaf.Visibility = ViewStates.Visible;

        }

        public void ShowNormalAccount()
        {
            btnPay.Visibility = ViewStates.Visible;
            tabButtonLayout.Visibility = ViewStates.Visible;
            txtBillPaymentHistoryTitle.Text = GetString(Resource.String.bill_menu_payment_history);
            imgLeaf.Visibility = ViewStates.Gone;
        }

        public void ShowREPaymentList(PaymentHistoryREResponse paymentHistoryREResponse)
        {
            ChildFragmentManager.BeginTransaction()
               .Replace(Resource.Id.layoutReplacement, PaymentListFragment.NewInstance(paymentHistoryREResponse, selectedAccount))
               .CommitAllowingStateLoss();
        }

        public void ShowProgressDialog()
        {
            try
            {
                if (loadingOverlay != null && loadingOverlay.IsShowing)
                {
                    loadingOverlay.Dismiss();
                }

                loadingOverlay = new LoadingOverlay(Activity.ApplicationContext, Resource.Style.LoadingOverlyDialogStyle);
                loadingOverlay.Show();
            }
            catch (System.Exception e)
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
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void SetBillDetails(AccountData selectedAccount)
        {
            if (selectedAccount != null)
            {
                txtAccountName.Text = (!string.IsNullOrEmpty(selectedAccount?.AccountName)) ? selectedAccount?.AccountName : "";
                txtAccountNum.Text = (!string.IsNullOrEmpty(selectedAccount?.AccountNum)) ? selectedAccount?.AccountNum : "";

                txtAddress.Text = selectedAccount?.AddStreet;
                txtCurrentChargesContent.Text = decimalFormatter.Format(selectedAccount?.AmtCurrentChg);
                txtOutstandingChargesContent.Text = decimalFormatter.Format(selectedAccount?.AmtOutstandingChg);
                txtTotalPayableContent.Text = decimalFormatter.Format(selectedAccount?.AmtPayableChg);

                Date d = null;
                try
                {
                    if (!string.IsNullOrEmpty(selectedAccount?.DatePaymentDue) && !selectedAccount.DatePaymentDue.Equals("N/A"))
                    {
                        d = dateParser.Parse(selectedAccount?.DatePaymentDue);
                    }

                }
                catch (ParseException e)
                {
                    Utility.LoggingNonFatalError(e);
                }
                if (selectedAccount != null && selectedAccount.AccountCategoryId.Equals("2"))
                {

                    double calAmt = selectedAccount.AmtCustBal * -1;
                    if (calAmt <= 0)
                    {
                        calAmt = 0.00;
                    }
                    else
                    {
                        calAmt = System.Math.Abs(selectedAccount.AmtCustBal);
                    }
                    txtTotalDueContent.Text = decimalFormatter.Format(calAmt);

                    if (d != null)
                    {
                        Calendar c = Calendar.Instance;
                        c.Time = d;
                        c.Add(CalendarField.Date, Constants.RE_ACCOUNT_DATE_INCREMENT_DAYS);
                        Date newDate = c.Time;
                        if (calAmt == 0.00)
                        {
                            txtDueDate.Text = "--";
                        }
                        else
                        {
                            txtDueDate.Text = GetString(Resource.String.dashboard_chartview_by_date_wildcard, dateFormatter.Format(newDate));
                        }

                    }
                }
                else
                {

                    txtTotalDueContent.Text = decimalFormatter.Format(selectedAccount?.AmtCustBal);
                    double calAmt = selectedAccount.AmtCustBal;
                    if (calAmt <= 0.00)
                    {
                        txtDueDate.Text = "--";
                    }
                    else
                    {
                        if (d != null)
                            txtDueDate.Text = GetString(Resource.String.dashboard_chartview_due_date_wildcard, dateFormatter.Format(d));
                    }

                }

                if (!selectedAccount.IsOwner)
                {
                    webLinkLayout.Visibility = ViewStates.Gone;
                    onlyAccountOwnersLayout.Visibility = ViewStates.Visible;
                }
                else
                {
                    webLinkLayout.Visibility = ViewStates.Visible;
                    onlyAccountOwnersLayout.Visibility = ViewStates.Gone;
                }

                txtFooter.Visibility = ViewStates.Gone;
                txtFooter1.Visibility = ViewStates.Gone;
                txtFooter2.Visibility = ViewStates.Gone;

                if (selectedAccount.AccountCategoryId.Equals("2"))
                {
                    txtCurrentBill.Text = GetString(Resource.String.title_current_payment_advice);
                    txtTotalDueTitle.Text = GetString(Resource.String.title_payment_total_receivable);

                    currentChangeLayout.Visibility = ViewStates.Gone;
                    totalPayableLayout.Visibility = ViewStates.Gone;
                    outstandingChangeLayout.Visibility = ViewStates.Gone;
                    divider.Visibility = ViewStates.Gone;
                }

                DownTimeEntity bcrmEntity = DownTimeEntity.GetByCode(Constants.BCRM_SYSTEM);
                DownTimeEntity pgCCEntity = DownTimeEntity.GetByCode(Constants.PG_CC_SYSTEM);
                DownTimeEntity pgFPXEntity = DownTimeEntity.GetByCode(Constants.PG_FPX_SYSTEM);
                if (bcrmEntity.IsDown || pgCCEntity.IsDown && pgFPXEntity.IsDown)
                {
                    btnPay.Enabled = false;
                    btnPay.Background = ContextCompat.GetDrawable(this.Activity, Resource.Drawable.silver_chalice_button_background);
                }
                else
                {
                    btnPay.Enabled = true;
                    btnPay.Background = ContextCompat.GetDrawable(this.Activity, Resource.Drawable.green_button_background);
                }

                if (pgCCEntity != null && pgFPXEntity != null)
                {
                    if (pgCCEntity.IsDown && pgFPXEntity.IsDown)
                    {
                        Snackbar downtimeSnackBar = Snackbar.Make(rootView,
                                bcrmEntity.DowntimeTextMessage,
                                Snackbar.LengthLong);
                        View v = downtimeSnackBar.View;
                        TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
                        tv.SetMaxLines(4);
                        if (!selectedAccount.AccountCategoryId.Equals("2"))
                        {
                            downtimeSnackBar.Show();
                        }
                    }
                }
            }
        }
    }
}