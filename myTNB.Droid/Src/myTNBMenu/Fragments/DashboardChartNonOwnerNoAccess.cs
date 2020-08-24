using AFollestad.MaterialDialogs;
using Android.App;
using Android.Content;
using Android.OS;



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
using myTNB_Android.Src.FAQ.Activity;
using myTNB_Android.Src.GetAccess.Activity;
using myTNB_Android.Src.MultipleAccountPayment.Activity;
using myTNB_Android.Src.myTNBMenu.Activity;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.myTNBMenu.MVP.Fragment;
using myTNB_Android.Src.Notifications.Activity;
using myTNB_Android.Src.Utils;
using myTNB_Android.Src.ViewBill.Activity;
using Newtonsoft.Json;
using Refit;
using System;

namespace myTNB_Android.Src.myTNBMenu.Fragments
{
    public class DashboardChartNonOwnerNoAccess : BaseFragment, DashboardNonOwnerContract.IView
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

        [BindView(Resource.Id.txtWhyThisAmt)]
        TextView txtWhyThisAmt;

        [BindView(Resource.Id.txtTotalPayableCurrency)]
        TextView txtTotalPayableCurrency;

        [BindView(Resource.Id.txtTotalPayable)]
        TextView txtTotalPayable;

        AccountData selectedAccount;

        AccountDueAmount accountDueAmountData;

        DecimalFormat decimalFormat = new DecimalFormat("#,###,###,###,##0.00");
        SimpleDateFormat dateParser = new SimpleDateFormat("dd/MM/yyyy");
        SimpleDateFormat dateFormatter = new SimpleDateFormat("dd MMM yyyy");

        private DashboardNonOwnerContract.IUserActionsListener userActionsListener;
        private DashboardNonOwnerPresenter mPresenter;

        private MaterialDialog mWhyThisAmtCardDialog;

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

            if (extras != null && extras.Size() > 0 && extras.ContainsKey(Constants.SELECTED_ACCOUNT))
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

            try
            {
                TextViewUtils.SetMuseoSans300Typeface(txtContent, txtTotalPayable, txtDueDate);
                TextViewUtils.SetMuseoSans500Typeface(txtTitle, btnGetAccess, btnPay, txtTotalPayableTitle, txtTotalPayableCurrency, txtWhyThisAmt);

                txtTotalPayable.Text = decimalFormat.Format(selectedAccount.AmtCustBal);

                txtWhyThisAmt.Visibility = ViewStates.Gone;

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
                            if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.N)
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

                    txtContent.Click += delegate
                    {
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
                                        Intent faqIntent = GetIntentObject(typeof(FAQListActivity));
                                        if (faqIntent != null && IsAdded)
                                        {
                                            faqIntent.PutExtra(Constants.FAQ_ID_PARAM, faqid);
                                            Activity.StartActivity(faqIntent);
                                        }
                                    }
                                }
                            }
                        }

                    };

                }

            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
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
            actionBar.Show();
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
            Intent access_form_activity = GetIntentObject(typeof(GetAccessFormActivity));
            if (access_form_activity != null && IsAdded)
            {
                access_form_activity.PutExtra(Constants.SELECTED_ACCOUNT, JsonConvert.SerializeObject(selectedAccount));
                StartActivity(access_form_activity);
            }
        }

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
            Intent payment_activity = GetIntentObject(typeof(SelectAccountsActivity));
            if (payment_activity != null && IsAdded)
            {
                payment_activity.PutExtra(Constants.SELECTED_ACCOUNT, JsonConvert.SerializeObject(selectedAccount));
                StartActivityForResult(payment_activity, DashboardHomeActivity.PAYMENT_RESULT_CODE);
            }
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

        [OnClick(Resource.Id.txtWhyThisAmt)]
        void OnWhyThisAmtClick(object sender, EventArgs eventArgs)
        {
            try
            {
                mWhyThisAmtCardDialog = new MaterialDialog.Builder(Activity)
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
                    txtItemizedMessage.TextFormatted = string.IsNullOrEmpty(accountDueAmountData.WhyThisAmountMessage) ? Html.FromHtml(Activity.GetString(Resource.String.itemized_bill_message), FromHtmlOptions.ModeLegacy) : Html.FromHtml(accountDueAmountData.WhyThisAmountMessage, FromHtmlOptions.ModeLegacy);
                }
                else
                {
                    txtItemizedMessage.TextFormatted = string.IsNullOrEmpty(accountDueAmountData.WhyThisAmountMessage) ? Html.FromHtml(Activity.GetString(Resource.String.itemized_bill_message)) : Html.FromHtml(accountDueAmountData.WhyThisAmountMessage);
                }
                txtItemizedTitle.Text = string.IsNullOrEmpty(accountDueAmountData.WhyThisAmountTitle) ? Activity.GetString(Resource.String.itemized_bill_title) : accountDueAmountData.WhyThisAmountTitle;
                btnGotIt.Text = string.IsNullOrEmpty(accountDueAmountData.WhyThisAmountSecButtonText) ? Activity.GetString(Resource.String.itemized_bill_got_it) : accountDueAmountData.WhyThisAmountSecButtonText;
                btnBringMeThere.Text = string.IsNullOrEmpty(accountDueAmountData.WhyThisAmountPriButtonText) ? Activity.GetString(Resource.String.itemized_bill_bring_me_there) : accountDueAmountData.WhyThisAmountPriButtonText;
                TextViewUtils.SetMuseoSans500Typeface(txtItemizedTitle, btnGotIt, btnBringMeThere);
                TextViewUtils.SetMuseoSans300Typeface(txtItemizedMessage);
                btnGotIt.Click += delegate
                {
                    mWhyThisAmtCardDialog.Dismiss();
                };
                btnBringMeThere.Click += delegate
                {
                    ((DashboardHomeActivity)Activity).BillsMenuAccess();
                    mWhyThisAmtCardDialog.Dismiss();
                };

                if (IsActive())
                {
                    mWhyThisAmtCardDialog.Show();
                }
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            if (requestCode == DashboardHomeActivity.PAYMENT_RESULT_CODE)
            {
                if (resultCode == Result.Ok)
                {
                    ((DashboardHomeActivity)Activity).OnTapRefresh();
                }
                else if (resultCode == Result.FirstUser)
                {
                    Bundle extras = data.Extras;
                    if(extras.ContainsKey(Constants.ITEMZIED_BILLING_VIEW_KEY) && extras.GetBoolean(Constants.ITEMZIED_BILLING_VIEW_KEY))
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
                            ((DashboardHomeActivity)Activity).BillsMenuAccess(selectedAccount);
                        }
                        catch (System.Exception e)
                        {
                            Utility.LoggingNonFatalError(e);
                        }
                    }
                }
            }
        }

        public void ShowNotification()
        {
            Intent intent = GetIntentObject(typeof(NotificationActivity));
            if (intent != null && IsAdded)
            {
                StartActivity(intent);
            }

        }

        public bool HasInternet()
        {
            return ConnectionUtils.HasInternetConnection(this.Activity);
        }

        private Snackbar mNoInternetSnackbar;
        public void ShowNoInternetSnackbar()
        {
            try
            {
                if (mNoInternetSnackbar != null && mNoInternetSnackbar.IsShown)
                {
                    mNoInternetSnackbar.Dismiss();
                }

                mNoInternetSnackbar = Snackbar.Make(rootView, GetString(Resource.String.dashboard_chartview_data_not_available_no_internet), Snackbar.LengthIndefinite)
                .SetAction(GetString(Resource.String.dashboard_chartview_data_not_available_no_internet_btn_close), delegate
                {

                    mNoInternetSnackbar.Dismiss();
                }
                );
                mNoInternetSnackbar.Show();
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void ShowAmountProgress()
        {
            try
            {
                progressBar.Visibility = ViewStates.Visible;
                totalPayableLayout.Visibility = ViewStates.Gone;

            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void HideAmountProgress()
        {
            try
            {
                progressBar.Visibility = ViewStates.Gone;
                totalPayableLayout.Visibility = ViewStates.Visible;

            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void ShowAmountDue(AccountDueAmount accountDueAmount)
        {
            try
            {
                Date d = null;
                try
                {
                    accountDueAmountData = accountDueAmount;
                    txtWhyThisAmt.Text = string.IsNullOrEmpty(accountDueAmount.WhyThisAmountLink) ? Activity.GetString(Resource.String.why_this_amount) : accountDueAmount.WhyThisAmountLink;
                    d = dateParser.Parse(accountDueAmount.BillDueDate);
                }
                catch (ParseException e)
                {
                    Utility.LoggingNonFatalError(e);
                }

                if (d != null)
                {
                    if (selectedAccount != null)
                    {
                        EnablePayButton();
                        btnViewBill.Enabled = true;
                        btnViewBill.SetTextColor(ContextCompat.GetColorStateList(this.Activity, Resource.Color.freshGreen));
                        btnViewBill.Background = ContextCompat.GetDrawable(this.Activity, Resource.Drawable.light_green_outline_button_background);
                        if (selectedAccount.AccountCategoryId.Equals("2"))
                        {
                            txtWhyThisAmt.Visibility = ViewStates.Gone;
                            selectedAccount.AmtCustBal = accountDueAmount.AmountDue;
                            double calAmt = selectedAccount.AmtCustBal * -1;
                            if (calAmt <= 0)
                            {
                                calAmt = 0.00;
                            }
                            else
                            {
                                calAmt = System.Math.Abs(selectedAccount.AmtCustBal);
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
#if STUB
                            if(accountDueAmount.OpenChargesTotal == 0)
                            {
                                txtWhyThisAmt.Visibility = ViewStates.Gone;
                            }
                            else
                            {
                                txtWhyThisAmt.Visibility = ViewStates.Visible;
                            }
#endif
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
                    else
                    {
                        txtWhyThisAmt.Visibility = ViewStates.Gone;
                    }
                }
                else
                {
                    txtTotalPayable.Text = GetString(Resource.String.dashboard_chartview_due_date_not_available);
                    txtDueDate.Text = GetString(Resource.String.dashboard_chartview_due_date_not_available);
                    txtWhyThisAmt.Visibility = ViewStates.Gone;
                }
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        private Snackbar mCancelledExceptionSnackBar;
        public void ShowRetryOptionsCancelledException(System.OperationCanceledException operationCanceledException)
        {
            try
            {
                if (mCancelledExceptionSnackBar != null && mCancelledExceptionSnackBar.IsShown)
                {
                    mCancelledExceptionSnackBar.Dismiss();
                }

                mCancelledExceptionSnackBar = Snackbar.Make(rootView, Utility.GetLocalizedErrorLabel("defaultErrorMessage"), Snackbar.LengthIndefinite)
                .SetAction(Utility.GetLocalizedCommonLabel("retry"), delegate
                {

                    mCancelledExceptionSnackBar.Dismiss();
                    this.userActionsListener.OnLoadAmount(selectedAccount.AccountNum);
                }
                );
                mCancelledExceptionSnackBar.Show();
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        private Snackbar mApiExcecptionSnackBar;
        public void ShowRetryOptionsApiException(ApiException apiException)
        {
            try
            {
                if (mApiExcecptionSnackBar != null && mApiExcecptionSnackBar.IsShown)
                {
                    mApiExcecptionSnackBar.Dismiss();
                }

                mApiExcecptionSnackBar = Snackbar.Make(rootView, Utility.GetLocalizedErrorLabel("defaultErrorMessage"), Snackbar.LengthIndefinite)
                .SetAction(Utility.GetLocalizedCommonLabel("retry"), delegate
                {

                    mApiExcecptionSnackBar.Dismiss();
                    this.userActionsListener.OnLoadAmount(selectedAccount.AccountNum);
                }
                );
                mApiExcecptionSnackBar.Show();
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }
        private Snackbar mUknownExceptionSnackBar;
        public void ShowRetryOptionsUnknownException(System.Exception exception)
        {
            try
            {
                if (mUknownExceptionSnackBar != null && mUknownExceptionSnackBar.IsShown)
                {
                    mUknownExceptionSnackBar.Dismiss();

                }

                mUknownExceptionSnackBar = Snackbar.Make(rootView, Utility.GetLocalizedErrorLabel("defaultErrorMessage"), Snackbar.LengthIndefinite)
                .SetAction(Utility.GetLocalizedCommonLabel("retry"), delegate
                {

                    mUknownExceptionSnackBar.Dismiss();
                    this.userActionsListener.OnLoadAmount(selectedAccount.AccountNum);

                }
                );
                mUknownExceptionSnackBar.Show();
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
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


        DashboardHomeActivity activity = null;
        public override void OnAttach(Context context)
        {
            base.OnAttach(context);
            try
            {
                if (context is DashboardHomeActivity)
                {
                    activity = context as DashboardHomeActivity;
                    // SETS THE WINDOW BACKGROUND TO HORIZONTAL GRADIENT AS PER UI ALIGNMENT
                    activity.Window.SetBackgroundDrawable(Activity.GetDrawable(Resource.Drawable.HorizontalGradientBackground));
                    activity.UnsetToolbarBackground();
                }

                FirebaseAnalyticsUtils.SetFragmentScreenName(this, "Non Owner Inner Dashboard");
            }
            catch (ClassCastException e)
            {
                Utility.LoggingNonFatalError(e);
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public override void OnAttach(Android.App.Activity activity)
        {
            base.OnAttach(activity);
            try
            {
                activity = activity as DashboardHomeActivity;
            }
            catch (ClassCastException e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        protected override Android.App.Activity GetActivityObject()
        {
            return activity;
        }

        public void ShowViewBill(BillHistoryV5 selectedBill)
        {
            try
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

                Intent viewBill = GetIntentObject(typeof(ViewBillActivity));
                if (viewBill != null && IsAdded)
                {
                    viewBill.PutExtra(Constants.SELECTED_ACCOUNT, JsonConvert.SerializeObject(selectedAccount));
                    viewBill.PutExtra(Constants.SELECTED_BILL, JsonConvert.SerializeObject(selectedBill));
                    StartActivity(viewBill);
                }
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }
    }
}