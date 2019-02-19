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
using myTNB_Android.Src.Base.Fragments;
using CheeseBind;
using myTNB_Android.Src.Utils;
using myTNB_Android.Src.myTNBMenu.Models;
using Newtonsoft.Json;
using myTNB_Android.Src.myTNBMenu.MVP.Fragment;
using myTNB_Android.Src.GetAccess.Activity;
using Java.Text;
using myTNB_Android.Src.MakePayment.Activity;
using myTNB_Android.Src.Database.Model;
using Android.Support.V4.Content;
using myTNB_Android.Src.Notifications.Activity;
using myTNB_Android.Src.myTNBMenu.Activity;
using Android.Support.Design.Widget;
using Refit;
using Java.Util;
using myTNB_Android.Src.ViewBill.Activity;
using myTNB_Android.Src.AddAccount.Fragment;
using myTNB_Android.Src.MultipleAccountPayment.Activity;
using Android.Support.V7.App;
using Android.Text;
using myTNB_Android.Src.FAQ.Activity;

namespace myTNB_Android.Src.myTNBMenu.Fragments
{
    public class DashboardChartNonOwnerNoAccess : BaseFragment , DashboardNonOwnerContract.IView
    {

        [BindView(Resource.Id.progressBar)]
        ProgressBar progressBar;

        [BindView(Resource.Id.totalPayableLayout)]
        RelativeLayout totalPayableLayout;

        [BindView(Resource.Id.txtDueDate)]
        TextView txtDueDate;

        [BindView(Resource.Id.rootView)]
        CoordinatorLayout rootView;

        [BindView(Resource.Id.dashboard_chartview_no_access_title)]
        TextView txtTitle;

        [BindView(Resource.Id.dashboard_chartview_no_access_content)]
        TextView txtContent;

        [BindView(Resource.Id.btnGetAccess)]
        Button btnGetAccess;

        [BindView(Resource.Id.btnPay)]
        Button btnPay;

        [BindView(Resource.Id.btnViewBill)]
        Button btnViewBill;

        [BindView(Resource.Id.txtTotalPayableTitle)]
        TextView txtTotalPayableTitle;

        [BindView(Resource.Id.txtTotalPayableCurrency)]
        TextView txtTotalPayableCurrency;

        [BindView(Resource.Id.txtTotalPayable)]
        TextView txtTotalPayable;

        AccountData selectedAccount;

        DecimalFormat decimalFormat = new DecimalFormat("#,###,###,###,##0.00");
        SimpleDateFormat dateParser = new SimpleDateFormat("dd/MM/yyyy");
        SimpleDateFormat dateFormatter = new SimpleDateFormat("dd MMM yyyy");

        private DashboardNonOwnerContract.IUserActionsListener userActionsListener;
        private DashboardNonOwnerPresenter mPresenter;

        internal static DashboardChartNonOwnerNoAccess NewInstance(AccountData accountData)
        {
            DashboardChartNonOwnerNoAccess fragment = new DashboardChartNonOwnerNoAccess();
            Bundle bundle = new Bundle();
            bundle.PutString(Constants.SELECTED_ACCOUNT, JsonConvert.SerializeObject(accountData));
            fragment.Arguments = bundle;
            return fragment;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Bundle extras = Arguments;

            if (extras.ContainsKey(Constants.SELECTED_ACCOUNT))
            {
                selectedAccount = JsonConvert.DeserializeObject<AccountData>(extras.GetString(Constants.SELECTED_ACCOUNT));
            }
            SetHasOptionsMenu(true);
            mPresenter = new DashboardNonOwnerPresenter(this);
        }



        public override int ResourceId()
        {
            return Resource.Layout.DashboardChartNonOwnerNoAccess;
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            TextViewUtils.SetMuseoSans300Typeface(txtContent, txtTotalPayable , txtDueDate);
            TextViewUtils.SetMuseoSans500Typeface(txtTitle , btnGetAccess , btnPay , txtTotalPayableTitle , txtTotalPayableCurrency);

            txtTotalPayable.Text = decimalFormat.Format(selectedAccount.AmtCustBal);
            //if (selectedAccount.AmtCustBal <= 0)
            //{
            //    btnPay.Enabled = false;
            //    btnPay.Background = ContextCompat.GetDrawable(this.Activity, Resource.Drawable.silver_chalice_button_background);
            //}

            if (selectedAccount != null)
            {
                if (selectedAccount.AccountCategoryId.Equals("2"))
                {
                    btnPay.Visibility = ViewStates.Gone;
                    btnViewBill.Text = GetString(Resource.String.dashboard_chart_view_payment_advice);
                    txtTotalPayableTitle.Text = GetString(Resource.String.title_payment_advice_amount);
                }
                else
                {
                    btnPay.Visibility = ViewStates.Visible;
                    btnViewBill.Text = GetString(Resource.String.dashboard_chartview_view_bill);
                    ///<summary>
                    /// Revert non owner CR changes
                    ///</summary>
                    //if (!selectedAccount.IsOwner)
                    //{
                    //    btnViewBill.Visibility = ViewStates.Gone;
                    //}
                    //else
                    //{
                    //    btnViewBill.Visibility = ViewStates.Visible;
                    //}

                }

                DownTimeEntity bcrmEntity = DownTimeEntity.GetByCode(Constants.BCRM_SYSTEM);
                DownTimeEntity pgCCEntity = DownTimeEntity.GetByCode(Constants.PG_CC_SYSTEM);
                DownTimeEntity pgFPXEntity = DownTimeEntity.GetByCode(Constants.PG_FPX_SYSTEM);
                if (bcrmEntity != null && bcrmEntity.IsDown)
                {
                    DisablePayButton();
                    btnViewBill.Enabled = false;
                    btnViewBill.Background = ContextCompat.GetDrawable(this.Activity, Resource.Drawable.silver_chalice_button_outline);
                    btnViewBill.SetTextColor(ContextCompat.GetColorStateList(this.Activity, Resource.Color.silverChalice));
                    if (bcrmEntity.IsDown)
                    {
                        if (Android.OS.Build.VERSION.SdkInt >= Android.OS.Build.VERSION_CODES.N)
                        {
                            txtContent.TextFormatted = Html.FromHtml(bcrmEntity.DowntimeMessage, FromHtmlOptions.ModeLegacy);
                        }
                        else
                        {
                            txtContent.TextFormatted = Html.FromHtml(bcrmEntity.DowntimeMessage);
                        }

                        Snackbar downtimeSnackBar = Snackbar.Make(rootView,
                            bcrmEntity.DowntimeTextMessage,
                            Snackbar.LengthLong);
                        View v = downtimeSnackBar.View;
                        TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
                        tv.SetMaxLines(5);
                        downtimeSnackBar.Show();
                    }
                }
                else
                {

                    if (pgCCEntity.IsDown && pgFPXEntity.IsDown)
                    {
                        DisablePayButton();
                        Snackbar downtimeSnackBar = Snackbar.Make(rootView,
                                bcrmEntity.DowntimeTextMessage,
                                Snackbar.LengthLong);
                        View v = downtimeSnackBar.View;
                        TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
                        tv.SetMaxLines(5);
                        if (!selectedAccount.AccountCategoryId.Equals("2"))
                        {
                            downtimeSnackBar.Show();
                        }
                    }

                    this.userActionsListener.OnLoadAmount(selectedAccount.AccountNum);
                }                

                txtContent.Click += delegate {
                    if (bcrmEntity != null && bcrmEntity.IsDown)
                    {
                        string textMessage = bcrmEntity.DowntimeMessage;
                        if (textMessage != null && textMessage.Contains("http"))
                        {
                            //Launch webview
                            int startIndex = textMessage.LastIndexOf("=") + 2;
                            int lastIndex = textMessage.LastIndexOf("\"");
                            int lengthOfId = (lastIndex - startIndex);
                            if (lengthOfId < textMessage.Length)
                            {
                                string url = textMessage.Substring(startIndex, lengthOfId);
                                if (!string.IsNullOrEmpty(url))
                                {
                                    Intent intent = new Intent(Intent.ActionView);
                                    intent.SetData(Android.Net.Uri.Parse(url));
                                    StartActivity(intent);
                                }
                            }
                        }
                        else if (textMessage != null && textMessage.Contains("faq"))
                        {
                            //Lauch FAQ
                            int startIndex = textMessage.LastIndexOf("=") + 1;
                            int lastIndex = textMessage.LastIndexOf("}");
                            int lengthOfId = (lastIndex - startIndex) + 1;
                            if (lengthOfId < textMessage.Length)
                            {
                                string faqid = textMessage.Substring(startIndex, lengthOfId);
                                if (!string.IsNullOrEmpty(faqid))
                                {
                                    Intent faqIntent = new Intent(this.Activity, typeof(FAQListActivity));
                                    faqIntent.PutExtra(Constants.FAQ_ID_PARAM, faqid);
                                    Activity.StartActivity(faqIntent);
                                }
                            }
                        }
                    }

                };

            }
        }

        private IMenu menu;
        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            inflater.Inflate(Resource.Menu.DashboardToolbarMenu, menu);
            this.menu = menu;
            if (UserNotificationEntity.HasNotifications())
            {
                menu.FindItem(Resource.Id.action_notification).SetIcon(ContextCompat.GetDrawable(this.Activity, Resource.Drawable.ic_header_notification_unread));
            }
            else
            {
                menu.FindItem(Resource.Id.action_notification).SetIcon(ContextCompat.GetDrawable(this.Activity, Resource.Drawable.ic_header_notification));
            }
            base.OnCreateOptionsMenu(menu, inflater);
        }

        public override void OnResume()
        {
            base.OnResume();
            this.Activity.InvalidateOptionsMenu();

            var act = this.Activity as AppCompatActivity;

            var actionBar = act.SupportActionBar;
            //actionBar.SetDisplayHomeAsUpEnabled(true);
            //actionBar.SetDisplayShowHomeEnabled(true);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.action_notification:
                    this.userActionsListener.OnNotification();
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }

        public void ShowGetAccessForm()
        {
            Intent access_form_activity = new Intent(this.Activity , typeof(GetAccessFormActivity));
            access_form_activity.PutExtra(Constants.SELECTED_ACCOUNT , JsonConvert.SerializeObject(selectedAccount));
            StartActivity(access_form_activity);
        }

        //[OnClick(Resource.Id.btnGetAccess)]
        //void OnGetAccess(object sender , EventArgs eventArgs)
        //{
        //    this.userActionsListener.OnGetAccess();
        //}

        public void SetPresenter(DashboardNonOwnerContract.IUserActionsListener userActionListener)
        {
            this.userActionsListener = userActionListener;
        }

        public bool IsActive()
        {
            return IsVisible;
        }

        public void ShowSelectPaymentScreen()
        {
            //Intent payment_activity = new Intent(this.Activity, typeof(MakePaymentActivity));
            Intent payment_activity = new Intent(this.Activity, typeof(SelectAccountsActivity));
            payment_activity.PutExtra(Constants.SELECTED_ACCOUNT, JsonConvert.SerializeObject(selectedAccount));
            //StartActivity(payment_activity);
            StartActivityForResult(payment_activity, DashboardActivity.PAYMENT_RESULT_CODE);
        }

        [OnClick(Resource.Id.btnViewBill)]
        internal void OnViewBill(object sender, EventArgs e)
        {
            this.userActionsListener.OnViewBill(selectedAccount);
        }

        [OnClick(Resource.Id.btnPay)]
        void OnPay(object sender, EventArgs eventArgs)
        {
            this.userActionsListener.OnPay();
        }

        public override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            //base.OnActivityResult(requestCode, resultCode, data);
            if (requestCode == DashboardActivity.PAYMENT_RESULT_CODE)
            {
                if (resultCode == Result.Ok)
                {
                    ((DashboardActivity)Activity).OnTapRefresh();
                }
            }
        }

        public void ShowNotification()
        {
            StartActivity(new Intent(this.Activity, typeof(NotificationActivity)));
        }

        public bool HasInternet()
        {
            return ConnectionUtils.HasInternetConnection(this.Activity);
        }

        private Snackbar mNoInternetSnackbar;
        public void ShowNoInternetSnackbar()
        {
            if (mNoInternetSnackbar != null && mNoInternetSnackbar.IsShown)
            {
                mNoInternetSnackbar.Dismiss();
            }

            mNoInternetSnackbar = Snackbar.Make(rootView, GetString(Resource.String.dashboard_chartview_data_not_available_no_internet), Snackbar.LengthIndefinite)
            .SetAction(GetString(Resource.String.dashboard_chartview_data_not_available_no_internet_btn_close), delegate {

                mNoInternetSnackbar.Dismiss();
            }
            );
            mNoInternetSnackbar.Show();
        }

        public void ShowAmountProgress()
        {
            progressBar.Visibility = ViewStates.Visible;
            totalPayableLayout.Visibility = ViewStates.Gone;


        }

        public void HideAmountProgress()
        {
            progressBar.Visibility = ViewStates.Gone;
            totalPayableLayout.Visibility = ViewStates.Visible;


        }

        public void ShowAmountDue(AccountDueAmount accountDueAmount)
        {
            
            Date d = null;
            try
            {
                d = dateParser.Parse(accountDueAmount.BillDueDate);
            }
            catch (ParseException e)
            {

            }

            if (d != null)
            {
                if (selectedAccount != null)
                {
                    if (selectedAccount.AccountCategoryId.Equals("2"))
                    {
                        selectedAccount.AmtCustBal = accountDueAmount.AmountDue;
                        double calAmt = selectedAccount.AmtCustBal * -1;
                        if (calAmt <= 0)
                        {
                            calAmt = 0.00;
                        }
                        else
                        {
                            calAmt = Math.Abs(selectedAccount.AmtCustBal);
                        }
                        txtTotalPayable.Text = decimalFormat.Format(calAmt);
                        

                        int incrementDays = int.Parse(accountDueAmount.IncrementREDueDateByDays == null ? "0" : accountDueAmount.IncrementREDueDateByDays);
                        Constants.RE_ACCOUNT_DATE_INCREMENT_DAYS = incrementDays;
                        Calendar c = Calendar.Instance;
                        c.Time = d;
                        c.Add(CalendarField.Date, incrementDays);
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
                    else
                    {
                        txtTotalPayable.Text = decimalFormat.Format(accountDueAmount.AmountDue);
                        selectedAccount.AmtCustBal = accountDueAmount.AmountDue;
                        double calAmt = selectedAccount.AmtCustBal;
                        if (calAmt <= 0.00)
                        {
                            txtDueDate.Text = "--";
                        }
                        else
                        {
                            txtDueDate.Text = GetString(Resource.String.dashboard_chartview_due_date_wildcard, dateFormatter.Format(d));
                        }
                    }
                }
            }
            else
            {
                txtDueDate.Text = GetString(Resource.String.dashboard_chartview_due_date_not_available);
            }
        }

        private Snackbar mCancelledExceptionSnackBar;
        public void ShowRetryOptionsCancelledException(System.OperationCanceledException operationCanceledException)
        {
            if (mCancelledExceptionSnackBar != null && mCancelledExceptionSnackBar.IsShown)
            {
                mCancelledExceptionSnackBar.Dismiss();
            }

            mCancelledExceptionSnackBar = Snackbar.Make(rootView, GetString(Resource.String.dashboard_chart_cancelled_exception_error), Snackbar.LengthIndefinite)
            .SetAction(GetString(Resource.String.dashboard_chart_cancelled_exception_btn_retry), delegate {

                mCancelledExceptionSnackBar.Dismiss();
                this.userActionsListener.OnLoadAmount(selectedAccount.AccountNum);
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

            mApiExcecptionSnackBar = Snackbar.Make(rootView, GetString(Resource.String.dashboard_chart_api_exception_error), Snackbar.LengthIndefinite)
            .SetAction(GetString(Resource.String.dashboard_chart_api_exception_btn_retry), delegate {

                mApiExcecptionSnackBar.Dismiss();
                this.userActionsListener.OnLoadAmount(selectedAccount.AccountNum);
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

            mUknownExceptionSnackBar = Snackbar.Make(rootView, GetString(Resource.String.dashboard_chart_unknown_exception_error), Snackbar.LengthIndefinite)
            .SetAction(GetString(Resource.String.dashboard_chart_unknown_exception_btn_retry), delegate {

                mUknownExceptionSnackBar.Dismiss();
                this.userActionsListener.OnLoadAmount(selectedAccount.AccountNum);

            }
            );
            mUknownExceptionSnackBar.Show();

        }

        public void EnablePayButton()
        {
            btnPay.Enabled = true;
            btnPay.Background = ContextCompat.GetDrawable(this.Activity, Resource.Drawable.green_button_background);
        }

        public void DisablePayButton()
        {
            btnPay.Enabled = false;
            btnPay.Background = ContextCompat.GetDrawable(this.Activity, Resource.Drawable.silver_chalice_button_background);

        }

        public void ShowViewBill(BillHistoryV5 selectedBill)
        {
            btnViewBill.Enabled = false;
            Handler h = new Handler();
            Action myAction = () =>
            {
                btnViewBill.Enabled = true;
            };
            h.PostDelayed(myAction, 3000);

            if (selectedBill != null && selectedBill.NrBill != null)
            {
                selectedBill.NrBill = null;
            }

            Intent viewBill = new Intent(this.Activity, typeof(ViewBillActivity));
            viewBill.PutExtra(Constants.SELECTED_ACCOUNT, JsonConvert.SerializeObject(selectedAccount));
            viewBill.PutExtra(Constants.SELECTED_BILL, JsonConvert.SerializeObject(selectedBill));
            StartActivity(viewBill);
        }
    }
}