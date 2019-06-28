using AFollestad.MaterialDialogs;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.Content;
using Android.Support.V7.Widget;
using Android.Text;
using Android.Text.Method;
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
using myTNB_Android.Src.myTNBMenu.Adapter.BillsMenu;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.myTNBMenu.MVP.Fragment;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

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

        [BindView(Resource.Id.txtMandatoryPaymentsTitle)]
        TextView txtMandatoryPaymentsTitle;

        [BindView(Resource.Id.txtMandatoryPaymentsContent)]
        TextView txtMandatoryPaymentsContent;

        [BindView(Resource.Id.txtMandatoryPaymentsRM)]
        TextView txtMandatoryPaymentsRM;

        [BindView(Resource.Id.txtOutstandingChargesTitle)]
        TextView txtOutstandingChargesTitle;

        [BindView(Resource.Id.txtOutstandingChargesContent)]
        TextView txtOutstandingChargesContent;

        [BindView(Resource.Id.txtOutstandingChargesRM)]
        TextView txtOutstandingChargesRM;

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

        [BindView(Resource.Id.mandatoryPaymentsLayout)]
        RelativeLayout mandatoryPaymentsLayout;

        [BindView(Resource.Id.mandatoryPaymentsToolTipsLayout)]
        RelativeLayout mandatoryPaymentsToolTipsLayout;

        [BindView(Resource.Id.txtMandatoryPaymentsToolTips)]
        TextView txtBtnMandatoryPaymentsToolTips;

        [BindView(Resource.Id.outstandingChangeLayout)]
        RelativeLayout outstandingChangeLayout;

        [BindView(Resource.Id.totalPayableLayout)]
        RelativeLayout totalPayableLayout;

        [BindView(Resource.Id.mandatory_list_recycler_view)]
        public RecyclerView mMandatoryPaymentDetailRecyclerView;

        RecyclerView.LayoutManager layoutManager;

        ItemizedBillingDetailsAdapter adapter;

        [BindView(Resource.Id.divider)]
        View divider;

        //[BindView(Resource.Id.swipeRefreshLayout)]
        //SwipeRefreshLayout swipeRefreshLayout;

        bool noInternet = false;

        private MaterialDialog mMandatoryPaymentCardDialog;

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


        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
            Bundle args = Arguments;
            if (args.ContainsKey(Constants.SELECTED_ACCOUNT))
            {
                selectedAccount = JsonConvert.DeserializeObject<AccountData>(args.GetString(Constants.SELECTED_ACCOUNT));
            }

            if (args.ContainsKey(Constants.NO_INTERNET_CONNECTION))
            {
                noInternet = args.GetBoolean(Constants.NO_INTERNET_CONNECTION);
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
                    // SETS THE WINDOW BACKGROUND TO HORIZONTAL GRADIENT AS PER UI ALIGNMENT
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

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnActivityCreated(savedInstanceState);


            try
            {

                TextViewUtils.SetMuseoSans500Typeface(txtAccountName,
                  txtCurrentBill,
                  txtCurrentChargesTitle,
                  txtCurrentChargesContent,
                  txtMandatoryPaymentsTitle,
                  txtMandatoryPaymentsContent,
                  txtOutstandingChargesTitle,
                  txtOutstandingChargesContent,
                  txtBtnMandatoryPaymentsToolTips,
                  txtDueDate,
                  txtCurrency,
                  btnPay,
                  txtBillPaymentHistoryTitle,
                  txtTotalDueContent,
                  txtTotalDueTitle);
                TextViewUtils.SetMuseoSans300Typeface(txtAccountNum,
                    txtAddress,
                    txtFooter, txtFooter1, txtFooter2,
                    txtCurrentChargesRM, txtOutstandingChargesRM, 
                    txtMandatoryPaymentsRM
                    );

                layoutManager = new LinearLayoutManager(Activity, LinearLayoutManager.Vertical, false);
                mMandatoryPaymentDetailRecyclerView.SetLayoutManager(layoutManager);
                mMandatoryPaymentDetailRecyclerView.SetAdapter(adapter);

                SetBillDetails(selectedAccount);

                //swipeRefreshLayout.Refresh += RefreshLayout_Refresh;

                this.userActionsListener.Start();
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }


        //private void RefreshLayout_Refresh(object sender, EventArgs e)
        //{
        //    if (ConnectionUtils.HasInternetConnection(this.Activity))
        //    {
        //        swipeRefreshLayout.Refreshing = true;
        //        this.userActionsListener.RefreshData();
        //    }
        //}

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

        [OnClick(Resource.Id.txtMandatoryPaymentsToolTips)]
        void OnMandatoryPaymentClick(object sender, EventArgs eventArgs)
        {
            try
            {
                mMandatoryPaymentCardDialog = new MaterialDialog.Builder(Activity)
                    .CustomView(Resource.Layout.CustomDialogOneButtonLayout, false)
                    .Cancelable(false)
                    .CanceledOnTouchOutside(false)
                    .Build();

                View dialogView = mMandatoryPaymentCardDialog.Window.DecorView;
                dialogView.SetBackgroundResource(Android.Resource.Color.Transparent);

                TextView txtItemizedTitle = mMandatoryPaymentCardDialog.FindViewById<TextView>(Resource.Id.txtTitle);
                TextView txtItemizedMessage = mMandatoryPaymentCardDialog.FindViewById<TextView>(Resource.Id.txtMessage);
                TextView btnGotIt = mMandatoryPaymentCardDialog.FindViewById<TextView>(Resource.Id.txtBtnFirst);
                txtItemizedMessage.MovementMethod = new ScrollingMovementMethod();
                if (Build.VERSION.SdkInt >= BuildVersionCodes.N)
                {
                    txtItemizedMessage.TextFormatted = string.IsNullOrEmpty(selectedAccount.WhatIsThisMessage) ? Html.FromHtml(Activity.GetString(Resource.String.itemized_bill_second_message), FromHtmlOptions.ModeLegacy) : Html.FromHtml(selectedAccount.WhatIsThisMessage, FromHtmlOptions.ModeLegacy);
                }
                else
                {
                    txtItemizedMessage.TextFormatted = string.IsNullOrEmpty(selectedAccount.WhatIsThisMessage) ? Html.FromHtml(Activity.GetString(Resource.String.itemized_bill_second_message)) : Html.FromHtml(selectedAccount.WhatIsThisMessage);
                }
                txtItemizedTitle.Text = string.IsNullOrEmpty(selectedAccount.WhatIsThisTitle) ? Activity.GetString(Resource.String.itemized_bill_second_title) : selectedAccount.WhatIsThisTitle;
                btnGotIt.Text = string.IsNullOrEmpty(selectedAccount.WhatIsThisButtonText) ? Activity.GetString(Resource.String.itemized_bill_got_it)  : selectedAccount.WhatIsThisButtonText;
                TextViewUtils.SetMuseoSans500Typeface(txtItemizedTitle, btnGotIt);
                TextViewUtils.SetMuseoSans300Typeface(txtItemizedMessage);
                btnGotIt.Click += delegate
                {
                    mMandatoryPaymentCardDialog.Dismiss();
                };

                if (IsActive())
                {
                    mMandatoryPaymentCardDialog.Show();
                }
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
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


        //[OnClick(Resource.Id.btnBills)]
        //void OnBillsClick(object sender, EventArgs args)
        //{
        //    this.userActionsListener.OnShowBills();
        //}

        //[OnClick(Resource.Id.btnPayment)]
        //void OnPaymentClick(object sender, EventArgs args)
        //{
        //    this.userActionsListener.OnShowPayment();
        //}

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
            //Intent payment_activity = new Intent(Activity, typeof(MakePaymentActivity));
            Intent payment_activity = new Intent(this.Activity, typeof(SelectAccountsActivity));
            payment_activity.PutExtra(Constants.SELECTED_ACCOUNT, JsonConvert.SerializeObject(selectedAccount));
            //StartActivity(payment_activity);
            StartActivityForResult(payment_activity, DashboardActivity.PAYMENT_RESULT_CODE);

        }

        public override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            //base.OnActivityResult(requestCode, resultCode, data);
            if (requestCode == DashboardActivity.PAYMENT_RESULT_CODE)
            {
                if (resultCode == Result.Ok)
                {
                    if (Activity is DashboardActivity)
                    {
                        ((DashboardActivity)Activity).OnTapRefresh();
                    }
                }
                else if (resultCode == Result.FirstUser)
                {
                    Bundle extras = data.Extras;
                    if (extras.ContainsKey(Constants.ITEMZIED_BILLING_VIEW_KEY))
                    {
                        AccountData selectedAccount = JsonConvert.DeserializeObject<AccountData>(extras.GetString(Constants.SELECTED_ACCOUNT));
                        UsageHistoryData selectedHistoryData = JsonConvert.DeserializeObject<UsageHistoryData>(extras.GetString(Constants.SELECTED_ACCOUNT_USAGE));

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
                            ((DashboardActivity)Activity).BillMenuAccessOther(selectedAccount);
                        }
                        catch (System.Exception e)
                        {
                            Utility.LoggingNonFatalError(e);
                        }
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

        public void SetBillDetails(AccountData selectedAccount)
        {
            if (selectedAccount != null)
            {
                txtAccountName.Text = (!string.IsNullOrEmpty(selectedAccount?.AccountName)) ? selectedAccount?.AccountName : "";
                txtAccountNum.Text = (!string.IsNullOrEmpty(selectedAccount?.AccountNum)) ? selectedAccount?.AccountNum : "";

                txtAddress.Text = selectedAccount?.AddStreet;

                txtCurrentChargesContent.Text = decimalFormatter.Format(selectedAccount?.AmtCurrentChg);
                txtOutstandingChargesContent.Text = decimalFormatter.Format(selectedAccount?.AmtOutstandingChg);

                if (selectedAccount?.OpenChargesTotal == 0)
                {
                    mandatoryPaymentsLayout.Visibility = ViewStates.Gone;
                    mMandatoryPaymentDetailRecyclerView.Visibility = ViewStates.Gone;
                    mandatoryPaymentsToolTipsLayout.Visibility = ViewStates.Gone;
                }
                else
                {
                    if (selectedAccount?.ItemizedBilling != null && selectedAccount?.ItemizedBilling.Count() > 0)
                    {
                        mandatoryPaymentsLayout.Visibility = ViewStates.Visible;
                        mMandatoryPaymentDetailRecyclerView.Visibility = ViewStates.Visible;
                        mandatoryPaymentsToolTipsLayout.Visibility = ViewStates.Visible;
                        txtMandatoryPaymentsContent.Text = decimalFormatter.Format(selectedAccount?.OpenChargesTotal);
                        adapter = new ItemizedBillingDetailsAdapter(Activity, selectedAccount?.ItemizedBilling);
                        txtBtnMandatoryPaymentsToolTips.Text = string.IsNullOrEmpty(selectedAccount.WhatIsThisLink) ? Activity.GetString(Resource.String.what_is_this) : selectedAccount.WhatIsThisLink;
                        mMandatoryPaymentDetailRecyclerView.SetAdapter(adapter);
                        adapter.NotifyDataSetChanged();
                    }
                    else
                    {
                        mandatoryPaymentsLayout.Visibility = ViewStates.Gone;
                        mMandatoryPaymentDetailRecyclerView.Visibility = ViewStates.Gone;
                        mandatoryPaymentsToolTipsLayout.Visibility = ViewStates.Gone;
                    }
                }

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
                //if (selectedAccount.AmtCustBal <= 0)
                //{
                //    btnPay.Enabled = false;
                //    btnPay.Background = ContextCompat.GetDrawable(this.Activity, Resource.Drawable.silver_chalice_button_background);
                //}

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

                //if (Android.OS.Build.VERSION.SdkInt >= Android.OS.Build.VERSION_CODES.N)
                //{
                //    txtFooter.TextFormatted = Html.FromHtml(GetString(Resource.String.billing_list_fragment_footer_text), FromHtmlOptions.ModeLegacy);
                //    txtFooter1.TextFormatted = Html.FromHtml(GetString(Resource.String.billing_list_fragment_footer_text1), FromHtmlOptions.ModeLegacy);

                //}
                //else
                //{
                //    txtFooter.TextFormatted = Html.FromHtml(GetString(Resource.String.billing_list_fragment_footer_text));
                //    txtFooter1.TextFormatted = Html.FromHtml(GetString(Resource.String.billing_list_fragment_footer_text1));

                //}
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
                    mandatoryPaymentsLayout.Visibility = ViewStates.Gone;
                    mMandatoryPaymentDetailRecyclerView.Visibility = ViewStates.Gone;
                    mandatoryPaymentsToolTipsLayout.Visibility = ViewStates.Gone;
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
            else
            {
                mandatoryPaymentsLayout.Visibility = ViewStates.Gone;
                mMandatoryPaymentDetailRecyclerView.Visibility = ViewStates.Gone;
                mandatoryPaymentsToolTipsLayout.Visibility = ViewStates.Gone;
            }
        }
    }
}