using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using myTNB_Android.Src.AppLaunch.Models;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.myTNBMenu.MVP.Fragment;
using Refit;
using myTNB_Android.Src.Base.Fragments;
using CheeseBind;

using MikePhil.Charting.Charts;
using Java.Text;
using MikePhil.Charting.Formatter;
using Newtonsoft.Json;
using myTNB_Android.Src.myTNBMenu.Charts.Formatter;
using MikePhil.Charting.Components;
using static MikePhil.Charting.Components.XAxis;
using Android.Graphics;
using myTNB_Android.Src.Utils.Custom.Charts;
using static MikePhil.Charting.Components.YAxis;
using myTNB_Android.Src.myTNBMenu.Charts.SelectedMarkerView;
using MikePhil.Charting.Data;
using MikePhil.Charting.Interfaces.Datasets;
using MikePhil.Charting.Highlight;
using myTNB_Android.Src.ViewBill.Activity;
using myTNB_Android.Src.MultipleAccountPayment.Activity;
using myTNB_Android.Src.myTNBMenu.Activity;
using myTNB_Android.Src.Database.Model;

using myTNB_Android.Src.Notifications.Activity;
using myTNB_Android.Src.Utils;
using Java.Util;
using myTNB_Android.Src.myTNBMenu.Listener;
using static myTNB_Android.Src.myTNBMenu.Listener.SMDashboardScrollView;
using static myTNB_Android.Src.myTNBMenu.Models.SMUsageHistoryData;
using Android.Text;
using System.Threading.Tasks;

using myTNB_Android.Src.FAQ.Activity;
using AFollestad.MaterialDialogs;
using Android.Text.Style;
using static myTNB_Android.Src.myTNBMenu.Models.GetInstallationDetailsResponse;
using Android.Text.Method;

namespace myTNB_Android.Src.myTNBMenu.Fragments
{
    public class DashboardSmartMeterFragment : BaseFragment, DashboardSmartMeterContract.IView, SMDashboardScrollViewListener, View.IOnClickListener
    {

        [BindView(Resource.Id.progressBar)]
        ProgressBar progressBar;

        [BindView(Resource.Id.totalPayableLayout)]
        RelativeLayout totalPayableLayout;

        [BindView(Resource.Id.txtDueDate)]
        TextView txtDueDate;

        [BindView(Resource.Id.rootView)]
        CoordinatorLayout rootView;

        [BindView(Resource.Id.btnToggleDay)]
        RadioButton btnToggleDay;

        [BindView(Resource.Id.btnToggleMonth)]
        RadioButton btnToggleMonth;

        SMUsageHistoryData selectedHistoryData;

        AccountData selectedAccount;

        ChartType ChartType = ChartType.Month;
        ChartDataType ChartDataType = ChartDataType.RM;

        bool hasNoInternet;
        static bool noSMDataFOund;

        [BindView(Resource.Id.bar_chart)]
        BarChart mChart;

        [BindView(Resource.Id.no_data_layout)]
        LinearLayout mNoDataLayout;

        [BindView(Resource.Id.scroll_view_content)]
        LinearLayout scrollViewContent;

        [BindView(Resource.Id.usageMetricsDetails)]
        LinearLayout mUsageMetricsDetails;

        [BindView(Resource.Id.no_sm_data_layout)]
        LinearLayout mSMNoDataLayout;

        [BindView(Resource.Id.layoutSegmentGroup)]
        RelativeLayout mLayoutSegmentGroup;

        [BindView(Resource.Id.txtUsageHistory)]
        TextView txtUsageHistory;

        [BindView(Resource.Id.txtAddress)]
        TextView txtAddress;

        [BindView(Resource.Id.addressDivider)]
        View addressDivider;

        [BindView(Resource.Id.txtAccountStatus)]
        TextView txtAccountStatus;

        [BindView(Resource.Id.txtWhatAccountStatus)]
        TextView txtWhatAccountStatus;

        [BindView(Resource.Id.txtRange)]
        TextView txtRange;

        [BindView(Resource.Id.txtTotalPayableTitle)]
        TextView txtTotalPayableTitle;

        [BindView(Resource.Id.txtWhyThisAmt)]
        TextView txtWhyThisAmt;

        [BindView(Resource.Id.txtTotalPayableCurrency)]
        TextView txtTotalPayableCurrency;

        [BindView(Resource.Id.txtTotalPayable)]
        TextView txtTotalPayable;

        [BindView(Resource.Id.btnViewBill)]
        Button btnViewBill;

        [BindView(Resource.Id.btnPay)]
        Button btnPay;


        [BindView(Resource.Id.dashboard_chartview_no_data_title)]
        TextView txtTitleNoData;

        [BindView(Resource.Id.dashboard_chartview_no_data_content)]
        TextView txtContentNoData;

        [BindView(Resource.Id.btnLearnMore)]
        Button btnLearnMore;

        [BindView(Resource.Id.btnLeft)]
        ImageButton btnLeft;

        [BindView(Resource.Id.btnRight)]
        ImageButton btnRight;

        [BindView(Resource.Id.bottomView)]
        View bottomView;

        [BindView(Resource.Id.shadow_layout)]
        ImageView shadowLayout;


        private static BottomSheetBehavior bottomSheetBehavior;

        private SMDashboardScrollView scrollView;

        private SMChartHScrollView chartHScrollView;

        [BindView(Resource.Id.bottom_sheet)]
        LinearLayout bottomSheet;

        [BindView(Resource.Id.imgCurrentCharges)]
        ImageView imgCurrentCharges;

        [BindView(Resource.Id.txtCurrentCharges)]
        TextView txtCurrentCharges;

        [BindView(Resource.Id.txtCurrentChargesRange)]
        TextView txtCurrentChargesRange;

        [BindView(Resource.Id.txtCurrentChargesUnit1)]
        TextView txtCurrentChargesUnit1;

        [BindView(Resource.Id.txtCurrentChargesUnit2)]
        TextView txtCurrentChargesUnit2;

        [BindView(Resource.Id.txtCurretnChargesValue)]
        TextView txtCurretnChargesValue;

        [BindView(Resource.Id.imgProjectedCost)]
        ImageView imgProjectedCost;

        [BindView(Resource.Id.txtProjectedCost)]
        TextView txtProjectedCost;

        [BindView(Resource.Id.txtProjectedCostRange)]
        TextView txtProjectedCostRange;

        [BindView(Resource.Id.txtProjectedCostUnit1)]
        TextView txtProjectedCostUnit1;

        [BindView(Resource.Id.txtProjectedCostValue)]
        TextView txtProjectedCostValue;

        [BindView(Resource.Id.sm_graph_radio_group)]
        RadioGroup radioGroupSMGraph;

        [BindView(Resource.Id.rbtnRM)]
        RadioButton radioButtonRM;

        [BindView(Resource.Id.rbtnkWh)]
        RadioButton radioButtonkWh;

        [BindView(Resource.Id.rbtnCO2)]
        RadioButton radioButtonCO2;

        [BindView(Resource.Id.layoutProjectedCost)]
        LinearLayout layoutProjectedCost;

        [BindView(Resource.Id.endDivider)]
        View endDivider;

        [BindView(Resource.Id.note_text_layout)]
        LinearLayout noteTextLayout;

        [BindView(Resource.Id.noteText)]
        TextView noteTextView;

        [BindView(Resource.Id.noteDivider)]
        View noteDividerView;

        [BindView(Resource.Id.txtWhatIsThis)]
        TextView txtWhatIsThis;


        [BindView(Resource.Id.layout_graph_total)]
        LinearLayout allGraphLayout;

        [BindView(Resource.Id.layout_api_refresh)]
        LinearLayout refreshLayout;

        [BindView(Resource.Id.btnRefresh)]
        Button btnNewRefresh;

        [BindView(Resource.Id.refresh_content)]
        TextView txtNewRefreshMessage;

        private string txtRefreshMsg = "";
        private string txtBtnRefreshTitle = "";

        private bool IsCO2Disabled = true;

        private bool hasAmtDue = false;

        private DashboardSmartMeterContract.IUserActionsListener userActionsListener;
        private DashboardSmartMeterPresenter mPresenter;

        DecimalFormat decimalFormat = new DecimalFormat("#,###,###,###,##0.00");
        SimpleDateFormat dateParser = new SimpleDateFormat("dd/MM/yyyy");
        SimpleDateFormat dateFormatter = new SimpleDateFormat("dd MMM yyyy");
        IAxisValueFormatter XLabelsFormatter;
        private int currentParentIndex = 0;
        AccountDueAmount accountDueAmountData;
        private MaterialDialog mWhyThisAmtCardDialog;

        public override int ResourceId()
        {
            return Resource.Layout.DashboardSmartMeterView;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Bundle extras = Arguments;

            if (extras != null && extras.Size() > 0)
            {
                if (extras.ContainsKey(Constants.NO_INTERNET_CONNECTION))
                {
                    hasNoInternet = extras.GetBoolean(Constants.NO_INTERNET_CONNECTION);
                }
                if (extras.ContainsKey(Constants.NO_SM_DATA_FOUND))
                {
                    noSMDataFOund = extras.GetBoolean(Constants.NO_SM_DATA_FOUND);
                }
                else
                {
                    noSMDataFOund = false;
                }
            }
            else
            {
                noSMDataFOund = false;
            }

            if (extras.ContainsKey(Constants.REFRESH_MSG) && !string.IsNullOrEmpty(extras.GetString(Constants.REFRESH_MSG)))
            {
                txtRefreshMsg = extras.GetString(Constants.REFRESH_MSG);
            }
            else
            {
                txtRefreshMsg = Activity.GetString(Resource.String.text_new_refresh_content);
            }
            if (extras.ContainsKey(Constants.REFRESH_BTN_MSG) && !string.IsNullOrEmpty(extras.GetString(Constants.REFRESH_BTN_MSG)))
            {
                txtBtnRefreshTitle = extras.GetString(Constants.REFRESH_BTN_MSG);
            }
            else
            {
                txtBtnRefreshTitle = Activity.GetString(Resource.String.text_new_refresh);
            }

            if (!hasNoInternet)
            {
                if (extras.ContainsKey(Constants.SELECTED_ACCOUNT))
                {
                    selectedAccount = JsonConvert.DeserializeObject<AccountData>(extras.GetString(Constants.SELECTED_ACCOUNT));
                }

                if (extras.ContainsKey(Constants.SELECTED_ACCOUNT_USAGE))
                {
                    selectedHistoryData = JsonConvert.DeserializeObject<SMUsageHistoryData>(extras.GetString(Constants.SELECTED_ACCOUNT_USAGE));
                }
            }

            SetHasOptionsMenu(true);
            this.mPresenter = new DashboardSmartMeterPresenter(this);

        }








        internal static DashboardSmartMeterFragment NewInstance(SMUsageHistoryData usageHistoryData, AccountData accountData)
        {
            DashboardSmartMeterFragment chartFragment = new DashboardSmartMeterFragment();
            Bundle bundle = new Bundle();
            string data = JsonConvert.SerializeObject(usageHistoryData);
            bundle.PutString(Constants.SELECTED_ACCOUNT_USAGE, data);
            bundle.PutString(Constants.SELECTED_ACCOUNT, JsonConvert.SerializeObject(accountData));
            chartFragment.Arguments = bundle;
            return chartFragment;
        }

        internal static DashboardSmartMeterFragment NewInstance(SMUsageHistoryData usageHistoryData, AccountData accountData, bool noSMData)
        {
            DashboardSmartMeterFragment chartFragment = new DashboardSmartMeterFragment();
            Bundle bundle = new Bundle();

            string data = JsonConvert.SerializeObject(usageHistoryData);

            bundle.PutBoolean(Constants.NO_SM_DATA_FOUND, noSMData);
            bundle.PutString(Constants.SELECTED_ACCOUNT, JsonConvert.SerializeObject(accountData));
            chartFragment.Arguments = bundle;
            return chartFragment;
        }

        internal static DashboardSmartMeterFragment NewInstance(bool hasNoInternet)
        {
            DashboardSmartMeterFragment chartFragment = new DashboardSmartMeterFragment();
            Bundle bundle = new Bundle();

            bundle.PutBoolean(Constants.NO_INTERNET_CONNECTION, hasNoInternet);
            chartFragment.Arguments = bundle;
            return chartFragment;
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            try
            {
                if (!hasNoInternet)
                {
                    txtTotalPayable.Text = decimalFormat.Format(selectedAccount.AmtCustBal);
                }

                txtTotalPayableCurrency.Visibility = ViewStates.Gone;

                TextViewUtils.SetMuseoSans300Typeface(noteTextView);
                TextViewUtils.SetMuseoSans300Typeface(txtUsageHistory, txtAddress, txtTotalPayable, txtContentNoData, txtDueDate);
                TextViewUtils.SetMuseoSans300Typeface(btnToggleDay, btnToggleMonth, txtNewRefreshMessage);
                TextViewUtils.SetMuseoSans500Typeface(txtRange, txtTotalPayableTitle, txtTotalPayableCurrency, btnViewBill, btnPay, btnLearnMore, txtTitleNoData, txtWhyThisAmt, btnNewRefresh);

                //smart meter
                TextViewUtils.SetMuseoSans500Typeface(txtCurrentCharges, txtCurretnChargesValue, txtProjectedCost, txtProjectedCostValue);
                TextViewUtils.SetMuseoSans300Typeface(txtCurrentChargesRange, txtProjectedCostRange, txtCurrentChargesUnit1, txtCurrentChargesUnit2, txtProjectedCostUnit1);
                TextViewUtils.SetMuseoSans300Typeface(radioButtonRM, radioButtonkWh, radioButtonCO2);

                //bottom sheet
                bottomSheetBehavior = BottomSheetBehavior.From(bottomSheet);
                bottomSheetBehavior.State = BottomSheetBehavior.StateHidden;
                bottomSheetBehavior.SetBottomSheetCallback(new DashboardBottomSheetCallBack());

                DownTimeEntity bcrmEntity = DownTimeEntity.GetByCode(Constants.BCRM_SYSTEM);
                DownTimeEntity pgCCEntity = DownTimeEntity.GetByCode(Constants.PG_CC_SYSTEM);
                DownTimeEntity pgFPXEntity = DownTimeEntity.GetByCode(Constants.PG_FPX_SYSTEM);
                txtWhatIsThis.Text = selectedHistoryData?.ToolTips[0]?.SMLink ?? Activity.GetString(Resource.String.tooltip_sm_what_are_these_link);
                txtWhatIsThis.SetOnClickListener(this);

                btnNewRefresh.Text = txtBtnRefreshTitle;
                if (Build.VERSION.SdkInt >= BuildVersionCodes.N)
                {
                    txtNewRefreshMessage.TextFormatted = Html.FromHtml(txtRefreshMsg, FromHtmlOptions.ModeLegacy);
                }
                else
                {
                    txtNewRefreshMessage.TextFormatted = Html.FromHtml(txtRefreshMsg);
                }

                txtWhyThisAmt.Visibility = ViewStates.Gone;

                this.userActionsListener.Start();
                if (selectedAccount != null)
                {

                    if (selectedAccount.AccountCategoryId.Equals("2"))
                    {
                        btnPay.Visibility = ViewStates.Gone;
                        btnViewBill.Text = GetString(Resource.String.dashboard_chart_view_payment_advice);
                        txtUsageHistory.Visibility = ViewStates.Gone;
                        txtTotalPayableTitle.Text = GetString(Resource.String.title_payment_advice_amount);
                    }
                    else
                    {
                        btnPay.Visibility = ViewStates.Visible;
                        btnViewBill.Text = GetString(Resource.String.dashboard_chartview_view_bill);
                    }

                    if (bcrmEntity.IsDown)
                    {
                        DisablePayButton();
                        btnViewBill.Enabled = false;
                        btnViewBill.Background = ContextCompat.GetDrawable(this.Activity, Resource.Drawable.silver_chalice_button_outline);
                        btnViewBill.SetTextColor(ContextCompat.GetColorStateList(this.Activity, Resource.Color.silverChalice));
                        txtRange.Visibility = ViewStates.Gone;
                        Snackbar downtimeSnackBar = Snackbar.Make(rootView,
                                bcrmEntity.DowntimeTextMessage,
                                Snackbar.LengthLong);
                        View v = downtimeSnackBar.View;
                        TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
                        tv.SetMaxLines(5);
                        downtimeSnackBar.Show();
                    }
                    else
                    {
                        if (pgCCEntity.IsDown && pgFPXEntity.IsDown)
                        {
                            DisablePayButton();
                            txtDueDate.Text = GetString(Resource.String.dashboard_chartview_due_date_not_available);
                            txtTotalPayable.Text = GetString(Resource.String.dashboard_chartview_due_date_not_available);
                            btnViewBill.Enabled = false;
                            btnViewBill.Background = ContextCompat.GetDrawable(this.Activity, Resource.Drawable.silver_chalice_button_outline);
                            btnViewBill.SetTextColor(ContextCompat.GetColorStateList(this.Activity, Resource.Color.silverChalice));
                            txtRange.Visibility = ViewStates.Gone;
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
                        this.userActionsListener.GetAccountStatus(selectedAccount.AccountNum);
                        this.userActionsListener.OnLoadAmount(selectedAccount.AccountNum);
                    }

                }

                if (!noSMDataFOund)
                {
                    var metrics = Resources.DisplayMetrics;
                    int heightInDp = metrics.HeightPixels;
                    if (heightInDp > 1776)
                    {
                        bottomView.LayoutParameters.Height = 120;
                        bottomView.RequestLayout();
                    }

                    mNoDataLayout.Visibility = ViewStates.Gone;
                    mSMNoDataLayout.Visibility = ViewStates.Gone;
                    mChart.Visibility = ViewStates.Visible;

                    mLayoutSegmentGroup.Visibility = ViewStates.Visible;
                    shadowLayout.Visibility = ViewStates.Visible;
                    txtRange.Visibility = ViewStates.Visible;
                    mUsageMetricsDetails.Visibility = ViewStates.Visible;
                    // IsCO2Disabled = selectedHistoryData.IsCO2Disabled;
                    if (IsCO2Disabled)
                    {
                        radioButtonCO2.Visibility = ViewStates.Gone;
                    }
                    else
                    {
                        radioButtonCO2.Visibility = ViewStates.Visible;
                    }


                    scrollView = view.FindViewById<SMDashboardScrollView>(Resource.Id.scroll_view);
                    scrollView.SmoothScrollingEnabled = true;
                    scrollView.setOnScrollViewListener(this);

                    mNoDataLayout.Visibility = ViewStates.Gone;
                    mSMNoDataLayout.Visibility = ViewStates.Gone;
                    mChart.Visibility = ViewStates.Visible;
                    refreshLayout.Visibility = ViewStates.Gone;
                    allGraphLayout.Visibility = ViewStates.Visible;

                    radioGroupSMGraph.CheckedChange += (s, e) =>
                    {
                        if (radioButtonRM.Checked)
                        {
                            ChartDataType = ChartDataType.RM;
                            txtWhatIsThis.Visibility = ViewStates.Visible;
                            SetUp();
                        }
                        else if (radioButtonkWh.Checked)
                        {
                            ChartDataType = ChartDataType.kWh;
                            txtWhatIsThis.Visibility = ViewStates.Gone;
                            SetUp();
                        }
                        else if (radioButtonCO2.Checked)
                        {
                            ChartDataType = ChartDataType.CO2;
                            SetUp();
                        }
                    };
                }
                else
                {

                    mLayoutSegmentGroup.Visibility = ViewStates.Gone;
                    txtRange.Text = GetString(Resource.String.dashboard_smart_meter_available_soon);
                    if (bcrmEntity.IsDown)
                    {
                        if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.N)
                        {
                            txtAddress.TextFormatted = Html.FromHtml(bcrmEntity.DowntimeMessage, FromHtmlOptions.ModeLegacy);
                        }
                        else
                        {
                            txtAddress.TextFormatted = Html.FromHtml(bcrmEntity.DowntimeMessage);
                        }
                        mUsageMetricsDetails.Visibility = ViewStates.Gone;
                        radioGroupSMGraph.Visibility = ViewStates.Gone;
                        shadowLayout.Visibility = ViewStates.Gone;
                        bottomSheetBehavior.State = BottomSheetBehavior.StateExpanded;
                        bottomSheet.SetOnClickListener(null);

                        mNoDataLayout.Visibility = ViewStates.Gone;
                        mSMNoDataLayout.Visibility = ViewStates.Visible;
                        mChart.Visibility = ViewStates.Gone;
                        refreshLayout.Visibility = ViewStates.Gone;
                        allGraphLayout.Visibility = ViewStates.Visible;
                    }
                    else
                    {
                        txtAddress.Text = GetString(Resource.String.dashboard_smart_meter_no_data_error);
                    }
                    mUsageMetricsDetails.Visibility = ViewStates.Gone;
                    radioGroupSMGraph.Visibility = ViewStates.Gone;
                    shadowLayout.Visibility = ViewStates.Gone;
                    bottomSheetBehavior.State = BottomSheetBehavior.StateExpanded;
                    bottomSheet.SetOnClickListener(null);

                    mNoDataLayout.Visibility = ViewStates.Gone;
                    mSMNoDataLayout.Visibility = ViewStates.Visible;
                    mChart.Visibility = ViewStates.Gone;
                }

                txtAddress.Click += delegate {
                    if (bcrmEntity.IsDown)
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
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        [OnClick(Resource.Id.btnLeft)]
        internal void OnLeft(object sender, EventArgs eventArgs)
        {
            this.userActionsListener.OnArrowBackClick();
        }

        [OnClick(Resource.Id.btnRight)]
        internal void OnRight(object sender, EventArgs eventArgs)
        {
            this.userActionsListener.OnArrowForwardClick();
        }

        private void SetNoteVisiBility(bool isVisible)
        {
            noteTextLayout.Visibility = isVisible ? ViewStates.Visible : ViewStates.Gone;
            noteDividerView.Visibility = isVisible ? ViewStates.Visible : ViewStates.Gone;
        }

        internal void SetUp()
        {
            try
            {
                if (hasNoInternet || noSMDataFOund)
                {
                    return;
                }

                mChart.SetRoundedBarRadius(100f);
                mChart.SetDrawBarShadow(false);
                mChart.SetDrawValueAboveBar(true);
                mChart.Description.Enabled = false;
                mChart.SetMaxVisibleValueCount(7);
                mChart.SetPinchZoom(false);
                mChart.SetDrawGridBackground(false);
                mChart.SetScaleEnabled(false);
                mChart.Legend.Enabled = false;
                mChart.AnimateY(1000);

                if (!hasNoInternet)
                {
                    txtAddress.Text = selectedAccount.AddStreet;
                }
                //Since the month is coming as year and year is coming as month... we are checking like this...
                //SprintX - 1 change: Remove Note section
                SetNoteVisiBility(false);//ChartType != ChartType.Month);

                if (ChartType == ChartType.Month)
                {


                    /*if (!hasNoInternet)
                    {
                        txtRange.Text = selectedHistoryData.ByMonth[0].Range;
                    }
                    // SETUP XAXIS

                    SetUpXAxis();

                    // SETUP YAXIS

                    SetUpYAxis();

                    // SETUP MARKER VIEW

                    SetUpMarkerMonthView();

                    // ADD DATA

                    SetData(selectedHistoryData.ByMonth[0].Months.Count);*/


                }
                else if (ChartType == ChartType.Day)
                {

                    if (!hasNoInternet)
                    {
                        txtRange.Text = selectedHistoryData.ByDay[currentParentIndex].Range;
                    }
                    // SETUP XAXIS

                    SetUpXAxisDay();

                    // SETUP YAXIS

                    SetUpYAxisDay();

                    // SETUP MARKER VIEW

                    SetUpMarkerDayView();

                    // ADD DATA
                    //SetDayData(currentParentIndex, 7);
                    SetDayData(currentParentIndex, selectedHistoryData.ByDay[currentParentIndex].Days.Count);
                }
                else
                {

                    if (!hasNoInternet)
                    {
                        txtRange.Text = selectedHistoryData.ByDay[currentParentIndex].Range;
                    }
                    // SETUP XAXIS

                    SetUpXAxisHour();

                    // SETUP YAXIS

                    SetUpYAxisHour();

                    // SETUP MARKER VIEW

                    SetUpMarkerHourView();

                    // ADD DATA
                    SetDayData(currentParentIndex, selectedHistoryData.ByDay[currentParentIndex].Days.Count);
                }

                int graphTopPadding = 20;
                if (selectedAccount.AccountCategoryId.Equals("2"))
                {
                    graphTopPadding = 30;
                }
                mChart.SetExtraOffsets(0, graphTopPadding, 0, 0);


                SetupUsageMatricsData(selectedHistoryData.OtherUsageMetrics);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        #region SETUP AXIS MONTH
        internal void SetUpXAxis()
        {
            try
            {
                /*XLabelsFormatter = new SMChartsMonthFormatter(selectedHistoryData.ByMonth, mChart, 0);

                XAxis xAxis = mChart.XAxis;
                xAxis.Position = XAxisPosition.Bottom;
                xAxis.TextColor = Color.ParseColor("#ffffff");
                xAxis.AxisLineWidth = 2f;
                xAxis.AxisLineColor = Color.ParseColor("#77a3ea");

                xAxis.SetDrawGridLines(false);

                xAxis.Granularity = 1f; // only intervals of 1 day
                xAxis.LabelCount = selectedHistoryData.ByMonth[0].Months.Count;
                xAxis.ValueFormatter = XLabelsFormatter;*/
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

        }
        #endregion

        #region SETUP AXIS DAY
        internal void SetUpXAxisDay()
        {

            try
            {
                XLabelsFormatter = new SMChartsDayFormatter(selectedHistoryData.ByDay, mChart, currentParentIndex, 0);

                XAxis xAxis = mChart.XAxis;
                xAxis.Position = XAxisPosition.Bottom;
                xAxis.TextColor = Color.ParseColor("#ffffff");
                xAxis.AxisLineWidth = 2f;
                xAxis.AxisLineColor = Color.ParseColor("#77a3ea");

                xAxis.SetDrawGridLines(false);

                xAxis.Granularity = 1f; // only intervals of 1 day
                xAxis.LabelCount = selectedHistoryData.ByDay[currentParentIndex].Days.Count;
                xAxis.ValueFormatter = XLabelsFormatter;

            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

        }
        #endregion


        #region SETUP AXIS Hour
        internal void SetUpXAxisHour()
        {
            try
            {
                XLabelsFormatter = new SMChartsHourFormatter()
                {
                    DayData = selectedHistoryData.ByDay,
                    Chart = mChart,
                    ParentIndex = currentParentIndex
                };

                XAxis xAxis = mChart.XAxis;
                xAxis.Position = XAxisPosition.Bottom;
                xAxis.TextColor = Color.ParseColor("#ffffff");
                xAxis.AxisLineWidth = 2f;
                xAxis.AxisLineColor = Color.ParseColor("#77a3ea");

                xAxis.SetDrawGridLines(false);

                xAxis.Granularity = 1f; // only intervals of 1 day
                xAxis.LabelCount = 7;
                xAxis.ValueFormatter = XLabelsFormatter;

            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }
        #endregion

        #region SETUP Y AXIS BOTH MONTH
        internal void SetUpYAxis()
        {
            try
            {
                IAxisValueFormatter custom = new MyAxisValueFormatter();

                float maxVal = GetMaxMonthValues();
                float lowestPossibleSpace = (5f / 100f) * -maxVal;
                Console.WriteLine("Space {0}", lowestPossibleSpace);

                YAxis leftAxis = mChart.AxisLeft; ;
                leftAxis.Enabled = false;
                leftAxis.SetPosition(YAxisLabelPosition.OutsideChart);
                leftAxis.SetDrawGridLines(false);
                leftAxis.SpaceTop = 10f;
                leftAxis.SpaceBottom = 10f;
                leftAxis.AxisMinimum = lowestPossibleSpace;
                leftAxis.AxisMaximum = maxVal;

                YAxis rightAxis = mChart.AxisRight;
                rightAxis.Enabled = false;
                rightAxis.SetDrawGridLines(false);
                rightAxis.SpaceTop = 10f;
                rightAxis.SpaceBottom = 10f;
                rightAxis.AxisMinimum = lowestPossibleSpace;
                rightAxis.AxisMaximum = maxVal;
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }
        #endregion

        #region SETUP Y AXIS BOTH DAY
        internal void SetUpYAxisDay()
        {
            try
            {
                IAxisValueFormatter custom = new MyAxisValueFormatter();
                float maxVal = GetMaxDaysValues();
                float lowestPossibleSpace = (5f / 100f) * -maxVal;
                Console.WriteLine("Space {0}", lowestPossibleSpace);

                YAxis leftAxis = mChart.AxisLeft; ;
                leftAxis.Enabled = false;
                leftAxis.SetPosition(YAxisLabelPosition.OutsideChart);
                leftAxis.SetDrawGridLines(false);
                leftAxis.SpaceTop = 10f;
                leftAxis.SpaceBottom = 10f;
                leftAxis.AxisMinimum = lowestPossibleSpace;
                leftAxis.AxisMaximum = maxVal;

                YAxis rightAxis = mChart.AxisRight;
                rightAxis.Enabled = false;
                rightAxis.SetDrawGridLines(false);
                rightAxis.SpaceTop = 10f;
                rightAxis.SpaceBottom = 10f;
                rightAxis.AxisMinimum = lowestPossibleSpace;
                rightAxis.AxisMaximum = maxVal;
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }
        #endregion

        #region SETUP Y AXIS BOTH Hour
        internal void SetUpYAxisHour()
        {
            try
            {
                IAxisValueFormatter custom = new MyAxisValueFormatter();
                float maxVal = GetMaxHoursValues();
                float lowestPossibleSpace = (5f / 100f) * -maxVal;
                Console.WriteLine("Space {0}", lowestPossibleSpace);

                YAxis leftAxis = mChart.AxisLeft; ;
                leftAxis.Enabled = false;
                leftAxis.SetPosition(YAxisLabelPosition.OutsideChart);
                leftAxis.SetDrawGridLines(false);
                leftAxis.SpaceTop = 10f;
                leftAxis.SpaceBottom = 10f;
                leftAxis.AxisMinimum = lowestPossibleSpace;
                leftAxis.AxisMaximum = maxVal;

                YAxis rightAxis = mChart.AxisRight;
                rightAxis.Enabled = false;
                rightAxis.SetDrawGridLines(false);
                rightAxis.SpaceTop = 10f;
                rightAxis.SpaceBottom = 10f;
                rightAxis.AxisMinimum = lowestPossibleSpace;
                rightAxis.AxisMaximum = maxVal;
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }
        #endregion

        #region SETUP MARKERVIEW MONTH / HIGHLIGHT TEXT
        internal void SetUpMarkerMonthView()
        {
            try
            {
                SMSelectedMarkerView markerView = new SMSelectedMarkerView(Activity)
                {
                    UsageHistoryData = selectedHistoryData,
                    ChartType = ChartType.Month,
                    ChartDataType = ChartDataType,
                    AccountType = selectedAccount.AccountCategoryId
                };
                markerView.ChartView = mChart;
                mChart.Marker = markerView;
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }
        #endregion

        #region SETUP MARKERVIEW DAY/ HIGHLIGHT TEXT
        internal void SetUpMarkerDayView()
        {

            SMSelectedMarkerView markerView = new SMSelectedMarkerView(Activity)
            {
                UsageHistoryData = selectedHistoryData,
                ChartType = ChartType.Day,
                ChartDataType = ChartDataType,
                CurrentParentIndex = currentParentIndex

            };
            markerView.ChartView = mChart;
            mChart.Marker = markerView;
        }
        #endregion

        #region SETUP MARKERVIEW Hour/ HIGHLIGHT TEXT
        internal void SetUpMarkerHourView()
        {

            SMSelectedMarkerView markerView = new SMSelectedMarkerView(Activity)
            {
                UsageHistoryData = selectedHistoryData,
                ChartType = ChartType.Day,
                ChartDataType = ChartDataType,
                CurrentParentIndex = currentParentIndex

            };
            markerView.ChartView = mChart;
            mChart.Marker = markerView;
        }
        #endregion

        #region SETUP MONTH DATA
        internal void SetData(int barLength)
        {

            try
            {
                List<BarEntry> yVals1 = new List<BarEntry>();
                for (int i = 0; i < barLength; i++)
                {
                    /*float val = 0;
                    if (ChartDataType == ChartDataType.RM)
                    {
                        val = float.Parse(selectedHistoryData.ByMonth[0].Months[i].Amount == null ? "0.00" : selectedHistoryData.ByMonth[0].Months[i].Amount);
                    }
                    else if (ChartDataType == ChartDataType.kWh)
                    {
                        val = float.Parse(selectedHistoryData.ByMonth[0].Months[i].Consumption == null ? "0.00" : selectedHistoryData.ByMonth[0].Months[i].Consumption);
                    }
                    else
                    {
                        val = float.Parse(selectedHistoryData.ByMonth[0].Months[i].CO2 == null ? "0.00" : selectedHistoryData.ByMonth[0].Months[i].CO2);
                    }
                    if (float.IsPositiveInfinity(val))
                    {
                        val = float.PositiveInfinity;
                    }

                    yVals1.Add(new BarEntry(i, Math.Abs(val)));*/
                }

                BarDataSet set1;

                if (mChart.Data != null && mChart.Data is BarData)
                {
                    var barData = mChart.Data as BarData;

                    if (barData.DataSetCount > 0)
                    {
                        set1 = barData.GetDataSetByIndex(0) as BarDataSet;
                        set1.Values = yVals1;
                        barData.NotifyDataChanged();
                        mChart.NotifyDataSetChanged();
                    }
                }
                else
                {
                    set1 = new BarDataSet(yVals1, "");
                    set1.SetDrawIcons(false);


                    set1.HighLightColor = Color.Argb(255, 255, 255, 255);

                    int[] color = { Color.Argb(50, 255, 255, 255) };
                    set1.SetColors(color);
                    List<IBarDataSet> dataSets = new List<IBarDataSet>();
                    dataSets.Add(set1);


                    BarData data = new BarData(dataSets);

                    data.BarWidth = 0.45f;

                    data.HighlightEnabled = true;
                    data.SetValueTextSize(10f);
                    data.SetDrawValues(false);

                    mChart.Data = data;
                }

                // HIGHLIGHT RIGHT MOST ITEM
                Highlight rightMostBar = new Highlight(barLength - 1, 0, 0);
                mChart.HighlightValues(new Highlight[] { rightMostBar });
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

        }
        #endregion

        #region SETUP DAY DATA
        internal void SetDayData(int parentIndex, int barLength)
        {
            try
            {
                List<BarEntry> yVals1 = new List<BarEntry>();
                int barIndex = 0;
                for (int i = 0; i < barLength; i++)
                {
                    float val = 0;
                    /*if (ChartDataType == ChartDataType.RM)
                    {
                        val = float.Parse(selectedHistoryData.ByDay[parentIndex].Days[i].Amount == null ? "0.00" : selectedHistoryData.ByDay[parentIndex].Days[i].Amount);
                    }
                    else if (ChartDataType == ChartDataType.kWh)
                    {
                        val = float.Parse(selectedHistoryData.ByDay[parentIndex].Days[i].Consumption == null ? "0.00" : selectedHistoryData.ByDay[parentIndex].Days[i].Consumption);
                    }
                    else
                    {
                        val = float.Parse(selectedHistoryData.ByDay[parentIndex].Days[i].CO2 == null ? "0.00" : selectedHistoryData.ByDay[parentIndex].Days[i].CO2);
                    }*/
                    if (float.IsPositiveInfinity(val))
                    {
                        val = float.PositiveInfinity;
                    }

                    yVals1.Add(new BarEntry(i, Math.Abs(val)));

                }

                BarDataSet set1;

                if (mChart.Data != null && mChart.Data is BarData)
                {
                    var barData = mChart.Data as BarData;

                    if (barData.DataSetCount > 0)
                    {
                        set1 = barData.GetDataSetByIndex(0) as BarDataSet;
                        set1.Values = yVals1;
                        barData.NotifyDataChanged();
                        mChart.NotifyDataSetChanged();
                    }
                }
                else
                {
                    set1 = new BarDataSet(yVals1, "");
                    set1.SetDrawIcons(false);


                    set1.HighLightColor = Color.Argb(255, 255, 255, 255);

                    int[] color = { Color.Argb(50, 255, 255, 255) };
                    set1.SetColors(color);
                    List<IBarDataSet> dataSets = new List<IBarDataSet>();
                    dataSets.Add(set1);


                    BarData data = new BarData(dataSets);

                    data.BarWidth = 0.4f;

                    data.HighlightEnabled = true;
                    data.SetValueTextSize(10f);
                    data.SetDrawValues(false);

                    mChart.Data = data;
                }
                // HIGHLIGHT RIGHT MOST ITEM
                Highlight rightMostBar = new Highlight(barLength - 1, 0, 0);
                mChart.HighlightValues(new Highlight[] { rightMostBar });
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }
        #endregion

        #region SETUP Hour DATA
        internal void SetHourData(int parentIndex, int barLength)
        {
            try
            {
                List<BarEntry> yVals1 = new List<BarEntry>();
                /*for (int i = 0; i < barLength; i++)
                {
                    float val = float.Parse(selectedHistoryData.ByMonth[parentIndex].Months[i].Amount == null ? "0.00" : selectedHistoryData.ByMonth[parentIndex].Months[i].Amount);
                    if (float.IsPositiveInfinity(val))
                    {
                        val = float.PositiveInfinity;
                    }

                    yVals1.Add(new BarEntry(i, Math.Abs(val)));
                }*/

                BarDataSet set1;

                if (mChart.Data != null && mChart.Data is BarData)
                {
                    var barData = mChart.Data as BarData;

                    if (barData.DataSetCount > 0)
                    {
                        set1 = barData.GetDataSetByIndex(0) as BarDataSet;
                        set1.Values = yVals1;
                        barData.NotifyDataChanged();
                        mChart.NotifyDataSetChanged();
                    }
                }
                else
                {
                    set1 = new BarDataSet(yVals1, "");
                    set1.SetDrawIcons(false);


                    set1.HighLightColor = Color.Argb(255, 255, 255, 255);

                    int[] color = { Color.Argb(50, 255, 255, 255) };
                    set1.SetColors(color);
                    List<IBarDataSet> dataSets = new List<IBarDataSet>();
                    dataSets.Add(set1);


                    BarData data = new BarData(dataSets);

                    data.BarWidth = 0.45f;

                    data.HighlightEnabled = true;
                    data.SetValueTextSize(10f);
                    data.SetDrawValues(false);

                    mChart.Data = data;
                }

                // HIGHLIGHT RIGHT MOST ITEM
                Highlight rightMostBar = new Highlight(barLength - 1, 0, 0);
                mChart.HighlightValues(new Highlight[] { rightMostBar });
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }
        #endregion

        private string GetChargeRangeDate(string preLabelString, string fromDateString, string toDateString)
        {
            if (fromDateString != null && toDateString != null)
            {
                SimpleDateFormat dateParser = new SimpleDateFormat("dd/MM/yyyy");
                SimpleDateFormat dateFormatter = new SimpleDateFormat("dd MMM");
                Date formattedDate = dateParser.Parse(fromDateString);
                return preLabelString + " " + dateFormatter.Format(formattedDate) + " - " + toDateString;
            }
            return "--";
        }
        public void SetupUsageMatricsData(SMUsageHistoryData.OtherUsageMetricsData usageMetricsData)
        {
            try
            {
                if (ChartDataType == ChartDataType.RM)
                {
                    endDivider.Visibility = ViewStates.Visible;
                    layoutProjectedCost.Visibility = ViewStates.Visible;
                    imgCurrentCharges.SetImageResource(Resource.Drawable.ic_rm);
                    imgProjectedCost.SetImageResource(Resource.Drawable.ic_projected_cost);
                    txtCurrentCharges.Text = (GetString(Resource.String.current_charges));
                    txtProjectedCost.Text = GetString(Resource.String.projected_cost);
                    if (usageMetricsData != null)
                    {
                        /*txtCurretnChargesValue.Text = usageMetricsData.StatsByCost.CurrentCharges;
                        //float currChanrges = float.Parse(usageMetricsData.StatsByCost.CurrentCharges);
                        //txtCurretnChargesValue.Text = decimalFormat.Format(Math.Abs(currChanrges));
                        txtCurrentChargesUnit1.Visibility = ViewStates.Visible;
                        txtCurrentChargesUnit2.Visibility = ViewStates.Gone;
                        txtCurrentChargesRange.Text = GetChargeRangeDate(GetString(Resource.String.for_current_month), usageMetricsData.CurrentCycleStartDate, usageMetricsData.StatsByCost.AsOf);
                        txtProjectedCostRange.Text = GetChargeRangeDate(GetString(Resource.String.for_current_month), usageMetricsData.CurrentCycleStartDate, usageMetricsData.StatsByCost.AsOf);
                        txtProjectedCostValue.Text = usageMetricsData.StatsByCost.ProjectedCost;
                        //float proCost = float.Parse(usageMetricsData.StatsByCost.ProjectedCost);
                        //txtProjectedCostValue.Text = decimalFormat.Format(Math.Abs(proCost));
                        txtProjectedCostUnit1.Visibility = ViewStates.Visible;*/
                    }
                }
                else if (ChartDataType == ChartDataType.kWh)
                {
                    endDivider.Visibility = ViewStates.Visible;
                    layoutProjectedCost.Visibility = ViewStates.Visible;
                    imgCurrentCharges.SetImageResource(Resource.Drawable.ic_energy_usage);
                    imgProjectedCost.SetImageResource(Resource.Drawable.ic_avgelectric_usage);
                    txtCurrentCharges.Text = GetString(Resource.String.current_usage);
                    txtProjectedCost.Text = GetString(Resource.String.avg_elec_usage);
                    if (usageMetricsData != null)
                    {
                        /*txtCurretnChargesValue.Text = usageMetricsData.StatsByUsage.CurrentUsageKWH;
                        txtCurrentChargesUnit1.Visibility = ViewStates.Gone;
                        txtCurrentChargesUnit2.Visibility = ViewStates.Visible;
                        txtProjectedCostUnit1.Visibility = ViewStates.Gone;
                        txtCurrentChargesRange.Text = GetChargeRangeDate(GetString(Resource.String.for_current_month), usageMetricsData.CurrentCycleStartDate, usageMetricsData.StatsByUsage.AsOf);
                        string htmlAvgUsageContent = usageMetricsData.StatsByUsage.UsageComparedToPrevious.Replace("-", "").Replace("+", "") + "%";
                        double avgUsageDouble = Double.Parse(String.IsNullOrEmpty(usageMetricsData.StatsByUsage.UsageComparedToPrevious) == true ? "0.00" : usageMetricsData.StatsByUsage.UsageComparedToPrevious);
                        if (avgUsageDouble != 0.00)
                        {
                            if (usageMetricsData.StatsByUsage.UsageComparedToPrevious.Contains("-"))
                            {
                                htmlAvgUsageContent = GetString(Resource.String.avg_electric_usage_down) + htmlAvgUsageContent;
                                txtProjectedCostRange.Text = GetString(Resource.String.less_last_bill_period);
                            }
                            else
                            {
                                if (int.Parse(usageMetricsData.StatsByUsage.UsageComparedToPrevious) == 0)
                                {
                                    htmlAvgUsageContent = htmlAvgUsageContent;
                                    txtProjectedCostRange.Text = GetString(Resource.String.same_last_bill_period);
                                }
                                else
                                {
                                    htmlAvgUsageContent = GetString(Resource.String.avg_electric_usage_up) + htmlAvgUsageContent;
                                    txtProjectedCostRange.Text = GetString(Resource.String.more_last_bill_period);
                                }
                            }
                        }


                        if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.N)
                        {

                            txtProjectedCostValue.TextFormatted = Html.FromHtml(htmlAvgUsageContent, Html.FromHtmlModeLegacy);
                        }
                        else
                        {
                            txtProjectedCostValue.TextFormatted = Html.FromHtml(htmlAvgUsageContent);
                        }*/

                    }
                }
                else if (ChartDataType == ChartDataType.CO2)
                {
                    endDivider.Visibility = ViewStates.Invisible;
                    layoutProjectedCost.Visibility = ViewStates.Invisible;
                    imgCurrentCharges.SetImageResource(Resource.Drawable.ic_energy_usage_co2);
                    txtCurrentCharges.Text = GetString(Resource.String.current_emission);
                    if (usageMetricsData != null)
                    {
                        /*if (usageMetricsData.StatsByCo2 != null && usageMetricsData.StatsByCo2.Count > 0)
                        {
                            foreach (StatsByCo2 item in usageMetricsData.StatsByCo2)
                            {
                                if (item.ItemName.Equals("Emision"))
                                {
                                    txtCurretnChargesValue.Text = item.Quantity;
                                    txtCurrentChargesUnit1.Visibility = ViewStates.Gone;
                                    txtCurrentChargesUnit2.Text = "kg";
                                    txtCurrentChargesUnit2.Visibility = ViewStates.Visible;
                                    txtProjectedCostUnit1.Visibility = ViewStates.Gone;
                                    txtCurrentChargesRange.Text = GetString(Resource.String.as_of) + " " + item.AsOf;
                                }
                            }
                        }*/

                    }
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void ShowByDay()
        {


            ChartType = ChartType.Day;
            mChart.Clear();
            SetUp();
        }

        public void ShowByMonth()
        {
            try
            {
                ChartType = ChartType.Month;

                // TODO : TO FIX STATEFUL CHANGES DURING MONTH/DAY OPTION
                mNoDataLayout.Visibility = ViewStates.Gone;
                mSMNoDataLayout.Visibility = ViewStates.Gone;
                mChart.Visibility = ViewStates.Visible;

                mChart.Clear();
                SetUp();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void ShowByHour()
        {
            ChartType = ChartType.Hour;

            // TODO : TO FIX STATEFUL CHANGES DURING MONTH/DAY/Hour OPTION
            mNoDataLayout.Visibility = ViewStates.Gone;
            mSMNoDataLayout.Visibility = ViewStates.Gone;
            mChart.Visibility = ViewStates.Visible;

            mChart.Clear();
            SetUp();
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

            Intent viewBill = GetIntentObject(typeof(ViewBillActivity));
            if (viewBill != null && IsAdded)
            {
                viewBill.PutExtra(Constants.SELECTED_ACCOUNT, JsonConvert.SerializeObject(selectedAccount));
                viewBill.PutExtra(Constants.SELECTED_BILL, JsonConvert.SerializeObject(selectedBill));
                StartActivity(viewBill);
            }
        }

        public void ShowPayment()
        {
            Intent payment_activity = GetIntentObject(typeof(SelectAccountsActivity));
            if (payment_activity != null && IsAdded)
            {
                payment_activity.PutExtra(Constants.SELECTED_ACCOUNT, JsonConvert.SerializeObject(selectedAccount));
                StartActivityForResult(payment_activity, DashboardHomeActivity.PAYMENT_RESULT_CODE);
            }
        }

        [OnClick(Resource.Id.btnToggleDay)]
        internal void OnToggleDay(object sender, EventArgs e)
        {
            this.userActionsListener.OnByDay();
        }

        [OnClick(Resource.Id.btnToggleMonth)]
        internal void OnToggleMonth(object sender, EventArgs e)
        {
            this.userActionsListener.OnByMonth();
        }

        class ClickSpan : ClickableSpan
        {
            public Action<View> Click;
            public override void OnClick(View widget)
            {
                if (Click != null)
                {
                    Click(widget);
                }
            }

            public override void UpdateDrawState(TextPaint ds)
            {
                base.UpdateDrawState(ds);
                ds.UnderlineText = false;
            }
        }

        private void OnWhatIsThisTap()
        {

            /*string textMessage = selectedHistoryData?.ToolTips[0]?.Message ?? Activity.GetString(Resource.String.tooltip_sm_what_are_these_message);
            string btnLabel = selectedHistoryData?.ToolTips[0]?.SMBtnText ?? Activity.GetString(Resource.String.tooltip_btnLabel);
            MaterialDialog materialDialog = new MaterialDialog.Builder(Activity)
                    .CustomView(Resource.Layout.WhatIsThisDialogView, false)
                    .Cancelable(false)
                    .Build();

            View view = materialDialog.View;
            TextView dialogDetailsText = view.FindViewById<TextView>(Resource.Id.txtDialogMessage);
            TextView dialogBtnLabel = view.FindViewById<TextView>(Resource.Id.txtBtnLabel);
            if (btnLabel != "")
            {
                dialogBtnLabel.Text = btnLabel;
            }
            dialogBtnLabel.Click += delegate
            {
                materialDialog.Dismiss();
            };

            if (dialogDetailsText != null)
            {
                TextViewUtils.SetMuseoSans300Typeface(dialogDetailsText);
            }
            SpannableString s = new SpannableString(dialogDetailsText.TextFormatted);
            var clickableSpan = new ClickSpan();
            clickableSpan.Click += v =>
            {
                if (textMessage != null && textMessage.Contains("faq"))
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
            };
            var urlSpans = s.GetSpans(0, s.Length(), Java.Lang.Class.FromType(typeof(URLSpan)));
            int startFAQLink = s.GetSpanStart(urlSpans[0]);
            int endFAQLink = s.GetSpanEnd(urlSpans[0]);
            s.RemoveSpan(urlSpans[0]);
            s.SetSpan(clickableSpan, startFAQLink, endFAQLink, SpanTypes.ExclusiveExclusive);
            dialogDetailsText.TextFormatted = s;
            dialogDetailsText.MovementMethod = new LinkMovementMethod();
            materialDialog.Show();*/
        }

        //[OnClick(Resource.Id.btnToggleHour)]
        //internal void OnToggleHour(object sender, EventArgs e)
        //{
        //    this.userActionsListener.OnByHour();
        //}


        [OnClick(Resource.Id.btnViewBill)]
        internal void OnViewBill(object sender, EventArgs e)
        {
            this.userActionsListener.OnViewBill(selectedAccount);
        }

        [OnClick(Resource.Id.btnRefresh)]
        internal void OnRefresh(object sender, EventArgs e)
        {
            if (hasNoInternet || noSMDataFOund)
            {
                this.userActionsListener.OnTapRefresh();
            }
            else
            {
                this.userActionsListener.OnLoadAmount(selectedAccount.AccountNum);
            }
        }

        [OnClick(Resource.Id.btnPay)]
        internal void OnUserPay(object sender, EventArgs e)
        {
            this.userActionsListener.OnPay();
        }

        [OnClick(Resource.Id.btnLearnMore)]
        internal void OnLearnMore(object sender, EventArgs e)
        {
            this.userActionsListener.OnLearnMore();
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
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void SetPresenter(DashboardSmartMeterContract.IUserActionsListener userActionListener)
        {
            this.userActionsListener = userActionListener;
        }

        public bool IsActive()
        {
            return IsVisible;
        }

        public bool IsByDay()
        {
            return ChartType == ChartType.Day;
        }

        public int GetCurrentParentIndex()
        {
            return currentParentIndex;
        }

        public void SetCurrentParentIndex(int newIndex)
        {
            this.currentParentIndex = newIndex;
            mChart.Clear();
            SetUp();


        }

        public int GetMaxParentIndex()
        {
            return selectedHistoryData.ByDay.Count;
        }

        public void EnableLeftArrow(bool show)
        {
            if (show)
            {
                btnLeft.Enabled = true;
                btnLeft.Visibility = ViewStates.Visible;
            }
            else
            {
                btnLeft.Enabled = false;
                btnLeft.Visibility = ViewStates.Gone;
            }
        }

        public void EnableRightArrow(bool show)
        {
            if (show)
            {
                btnRight.Enabled = true;
                btnRight.Visibility = ViewStates.Visible;
            }
            else
            {
                btnRight.Enabled = false;
                btnRight.Visibility = ViewStates.Gone;
            }
        }

        public override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
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

        public void ShowNotAvailableDayData()
        {
            mNoDataLayout.Visibility = ViewStates.Visible;
            mSMNoDataLayout.Visibility = ViewStates.Gone;
            mChart.Visibility = ViewStates.Gone;
            refreshLayout.Visibility = ViewStates.Gone;
            allGraphLayout.Visibility = ViewStates.Visible;
        }

        public bool IsByDayEmpty()
        {
            return selectedHistoryData.ByDay.Count == 0;
        }

        public void ShowNoInternet()
        {
            try
            {
                mNoDataLayout.Visibility = ViewStates.Gone;
                mSMNoDataLayout.Visibility = ViewStates.Gone;
                mChart.Visibility = ViewStates.Gone;
                refreshLayout.Visibility = ViewStates.Visible;
                allGraphLayout.Visibility = ViewStates.Gone;
                if (!hasAmtDue)
                {
                    txtTotalPayableCurrency.Visibility = ViewStates.Gone;
                    txtDueDate.Text = GetString(Resource.String.dashboard_chartview_due_date_not_available);
                    btnViewBill.Enabled = false;
                    btnViewBill.Background = ContextCompat.GetDrawable(this.Activity, Resource.Drawable.silver_chalice_button_outline);
                    btnViewBill.SetTextColor(ContextCompat.GetColorStateList(this.Activity, Resource.Color.silverChalice));
                }
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void ShowNoInternetWithWord(string contentTxt, string buttonTxt)
        {
            try
            {
                hasAmtDue = false;
                mNoDataLayout.Visibility = ViewStates.Gone;
                mSMNoDataLayout.Visibility = ViewStates.Gone;
                mChart.Visibility = ViewStates.Gone;
                refreshLayout.Visibility = ViewStates.Visible;
                allGraphLayout.Visibility = ViewStates.Gone;
                btnNewRefresh.Text = string.IsNullOrEmpty(buttonTxt) ? txtBtnRefreshTitle : buttonTxt;
                if (Build.VERSION.SdkInt >= BuildVersionCodes.N)
                {
                    txtNewRefreshMessage.TextFormatted = string.IsNullOrEmpty(contentTxt) ? Html.FromHtml(GetString(Resource.String.text_new_refresh_content), FromHtmlOptions.ModeLegacy) : Html.FromHtml(contentTxt, FromHtmlOptions.ModeLegacy);
                }
                else
                {
                    txtNewRefreshMessage.TextFormatted = string.IsNullOrEmpty(contentTxt) ? Html.FromHtml(GetString(Resource.String.text_new_refresh_content)) : Html.FromHtml(contentTxt);
                }
                txtTotalPayableCurrency.Visibility = ViewStates.Gone;
                txtDueDate.Text = GetString(Resource.String.dashboard_chartview_due_date_not_available);
                txtTotalPayable.Text = GetString(Resource.String.dashboard_chartview_due_date_not_available);
                DisablePayButton();
                btnViewBill.Enabled = false;
                btnViewBill.Background = ContextCompat.GetDrawable(this.Activity, Resource.Drawable.silver_chalice_button_outline);
                btnViewBill.SetTextColor(ContextCompat.GetColorStateList(this.Activity, Resource.Color.silverChalice));
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public bool HasNoInternet()
        {
            return hasNoInternet;
        }

        public void ShowTapRefresh()
        {
            if (Activity is DashboardHomeActivity)
            {
                var DashboardHomeActivity = Activity as DashboardHomeActivity;
                DashboardHomeActivity.OnTapRefresh();
            }

        }

        internal float GetMaxDaysValues()
        {
            float val = 0;
            try
            {
                if (ChartDataType == ChartDataType.RM)
                {
                    foreach (SMUsageHistoryData.ByDayData ByDay in selectedHistoryData.ByDay)
                    {
                        foreach (SMUsageHistoryData.ByDayData.DayData dayData in ByDay.Days)
                        {
                            /*if (Math.Abs(float.Parse(dayData.Amount == null ? "0.00" : dayData.Amount)) > val)
                            {
                                val = Math.Abs(float.Parse(dayData.Amount == null ? "0.00" : dayData.Amount));
                            }*/
                        }
                    }
                }
                else if (ChartDataType == ChartDataType.kWh)
                {
                    foreach (SMUsageHistoryData.ByDayData ByDay in selectedHistoryData.ByDay)
                    {
                        foreach (SMUsageHistoryData.ByDayData.DayData dayData in ByDay.Days)
                        {
                            /*if (Math.Abs(float.Parse(dayData.Consumption == null ? "0.00" : dayData.Consumption)) > val)
                            {
                                val = Math.Abs(float.Parse(dayData.Consumption == null ? "0.00" : dayData.Consumption));
                            }*/
                        }
                    }
                }
                else
                {
                    foreach (SMUsageHistoryData.ByDayData ByDay in selectedHistoryData.ByDay)
                    {
                        foreach (SMUsageHistoryData.ByDayData.DayData dayData in ByDay.Days)
                        {
                            if (Math.Abs(float.Parse(dayData.CO2 == null ? "0.00" : dayData.CO2)) > val)
                            {
                                val = Math.Abs(float.Parse(dayData.CO2 == null ? "0.00" : dayData.CO2));
                            }
                        }
                    }
                }
                if (val == 0)
                {
                    val = 1;
                }

            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

            return val;
        }

        internal float GetMaxMonthValues()
        {
            float val = 0;
            try
            {
                /*if (ChartDataType == ChartDataType.RM)
                {
                    foreach (SMUsageHistoryData.ByMonthData.MonthData MonthData in selectedHistoryData.ByMonth[0].Months)
                    {
                        if (Math.Abs(float.Parse(MonthData.Amount == null ? "0.00" : MonthData.Amount)) > val)
                        {
                            val = Math.Abs(float.Parse(MonthData.Amount == null ? "0.00" : MonthData.Amount));
                        }
                    }
                }
                else if (ChartDataType == ChartDataType.kWh)
                {
                    foreach (SMUsageHistoryData.ByMonthData.MonthData MonthData in selectedHistoryData.ByMonth[0].Months)
                    {
                        if (Math.Abs(float.Parse(MonthData.Consumption == null ? "0.00" : MonthData.Consumption)) > val)
                        {
                            val = Math.Abs(float.Parse(MonthData.Consumption == null ? "0.00" : MonthData.Consumption));
                        }
                    }
                }
                else if (ChartDataType == ChartDataType.CO2)
                {
                    foreach (SMUsageHistoryData.ByMonthData.MonthData MonthData in selectedHistoryData.ByMonth[0].Months)
                    {
                        if (Math.Abs(float.Parse(MonthData.CO2 == null ? "0.00" : MonthData.CO2)) > val)
                        {
                            val = Math.Abs(float.Parse(MonthData.CO2 == null ? "0.00" : MonthData.CO2));
                        }
                    }
                }
                if (val == 0)
                {
                    val = 1;
                }*/
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
            return val;
        }

        internal float GetMaxHoursValues()
        {
            float val = 0;
            try
            {
                foreach (SMUsageHistoryData.ByDayData ByDay in selectedHistoryData.ByDay)
                {
                    foreach (SMUsageHistoryData.ByDayData.DayData dayData in ByDay.Days)
                    {
                        /*if (Math.Abs(float.Parse(dayData.Amount == null ? "0.00" : dayData.Amount)) > val)
                        {
                            val = Math.Abs(float.Parse(dayData.Amount == null ? "0.00" : dayData.Amount));
                        }*/
                    }
                }

            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
            return val;
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
                .SetAction(GetString(Resource.String.dashboard_chartview_data_not_available_no_internet_btn_close), delegate {

                    mNoInternetSnackbar.Dismiss();
                }
                );
                mNoInternetSnackbar.Show();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
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

        public void ShowAmountProgress()
        {
            progressBar.Visibility = ViewStates.Visible;
            totalPayableLayout.Visibility = ViewStates.Gone;


        }

        public void HideAmountProgress()
        {
            try
            {
                progressBar.Visibility = ViewStates.Gone;
                totalPayableLayout.Visibility = ViewStates.Visible;
                bottomSheetBehavior.State = BottomSheetBehavior.StateExpanded;
            }
            catch (Exception e)
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
                        hasAmtDue = true;
                        txtTotalPayableCurrency.Visibility = ViewStates.Visible;
                        DownTimeEntity pgCCEntity = DownTimeEntity.GetByCode(Constants.PG_CC_SYSTEM);
                        DownTimeEntity pgFPXEntity = DownTimeEntity.GetByCode(Constants.PG_FPX_SYSTEM);
                        if (!pgCCEntity.IsDown || !pgFPXEntity.IsDown)
                        {
                            EnablePayButton();
                        }
                        btnViewBill.Enabled = true;
                        btnViewBill.SetTextColor(ContextCompat.GetColorStateList(this.Activity, Resource.Color.freshGreen));
                        btnViewBill.Background = ContextCompat.GetDrawable(this.Activity, Resource.Drawable.light_green_outline_button_background);
                        if (!hasNoInternet && !noSMDataFOund)
                        {
                            refreshLayout.Visibility = ViewStates.Gone;
                            allGraphLayout.Visibility = ViewStates.Visible;
                            mNoDataLayout.Visibility = ViewStates.Gone;
                            mSMNoDataLayout.Visibility = ViewStates.Gone;
                            mChart.Visibility = ViewStates.Visible;
                        }
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
#if STUB
                            if (accountDueAmount.OpenChargesTotal == 0)
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
            catch (Exception e)
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
                .SetAction(Utility.GetLocalizedCommonLabel("retry"), delegate {

                    mCancelledExceptionSnackBar.Dismiss();
                    this.userActionsListener.OnLoadAmount(selectedAccount.AccountNum);
                }
                );
                mCancelledExceptionSnackBar.Show();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        private Snackbar mLoadBillSnackBar;
        public void ShowLoadBillRetryOptions()
        {
            try
            {
                if (mLoadBillSnackBar != null && mLoadBillSnackBar.IsShown)
                {
                    mLoadBillSnackBar.Dismiss();
                }

                mLoadBillSnackBar = Snackbar.Make(rootView, Utility.GetLocalizedErrorLabel("defaultErrorMessage"), Snackbar.LengthIndefinite)
                .SetAction(GetString(Resource.String.dashboard_chartview_data_not_available_no_internet_btn_close), delegate
                {
                    mLoadBillSnackBar.Dismiss();
                }
                );
                mLoadBillSnackBar.Show();
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
                .SetAction(Utility.GetLocalizedCommonLabel("retry"), delegate {

                    mApiExcecptionSnackBar.Dismiss();
                    this.userActionsListener.OnLoadAmount(selectedAccount.AccountNum);
                }
                );
                mApiExcecptionSnackBar.Show();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        private Snackbar mUknownExceptionSnackBar;
        public void ShowRetryOptionsUnknownException(Exception exception)
        {
            try
            {
                if (mUknownExceptionSnackBar != null && mUknownExceptionSnackBar.IsShown)
                {
                    mUknownExceptionSnackBar.Dismiss();

                }

                mUknownExceptionSnackBar = Snackbar.Make(rootView, Utility.GetLocalizedErrorLabel("defaultErrorMessage"), Snackbar.LengthIndefinite)
                .SetAction(Utility.GetLocalizedCommonLabel("retry"), delegate {

                    mUknownExceptionSnackBar.Dismiss();
                    this.userActionsListener.OnLoadAmount(selectedAccount.AccountNum);

                }
                );
                mUknownExceptionSnackBar.Show();
            }
            catch (Exception e)
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

        public void ShowLearnMore(Weblink weblink)
        {
            //try {
            //if (weblink.OpenWith.Equals("APP"))
            //{
            //        Intent smartMeterINtent = GetIntentObject(typeof(SmartMeterLearnMoreActivity));
            //    //Intent smartMeterINtent = new Intent(this.Activity, typeof(SmartMeterLearnMoreActivity));
            //    smartMeterINtent.PutExtra(Constants.SMART_METER_LINK, JsonConvert.SerializeObject(weblink));
            //    StartActivity(smartMeterINtent);
            //}
            //else
            //{
            //    var uri = Android.Net.Uri.Parse(weblink.Url);
            //    var intent = new Intent(Intent.ActionView, uri);
            //    StartActivity(intent);
            //}
            //}
            //catch (Exception e)
            //{
            //    Utility.LoggingNonFatalError(e);
            //}
        }

        void SMDashboardScrollViewListener.OnScrollChanged(SMDashboardScrollView v, int l, int t, int oldl, int oldt)
        {
            View view = (View)scrollView.GetChildAt(scrollView.ChildCount - 1);
            int scrollPosition = t - oldt;
            // if diff is zero, then the bottom has been reached
            if (scrollPosition > 0)
            {
                bottomSheetBehavior.State = BottomSheetBehavior.StateHidden;
            }
            else if (scrollPosition < 0)
            {
                bottomSheetBehavior.State = BottomSheetBehavior.StateExpanded;
            }
        }

        private class DashboardBottomSheetCallBack : BottomSheetBehavior.BottomSheetCallback
        {
            public override void OnSlide(View bottomSheet, float slideOffset)
            {

            }

            public override void OnStateChanged(View bottomSheet, int newState)
            {
                if (noSMDataFOund)
                {
                    if (newState == BottomSheetBehavior.StateHidden)
                    {
                        bottomSheetBehavior.State = BottomSheetBehavior.StateExpanded;
                    }
                }
            }
        }


        private void ScrollUpDownScrollView(int UpDown)
        {
            if (Activity != null)
            {
                Activity.RunOnUiThread(() => {
                    ScrollViewTowards(UpDown);
                });
            }
        }

        private void ScrollViewTowards(int UpDown)
        {
            if (UpDown == 0)
            {
                scrollView.FullScroll(FocusSearchDirection.Down);
            }
            else if (UpDown == 1)
            {
                scrollView.FullScroll(FocusSearchDirection.Up);
            }
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
                FirebaseAnalyticsUtils.SetFragmentScreenName(this, "Smart Meter Inner Dashboard");
            }
            catch (Java.Lang.ClassCastException e)
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
            catch (Java.Lang.ClassCastException e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        protected override Android.App.Activity GetActivityObject()
        {
            return activity;
        }

        public void OnClick(View v)
        {
            OnWhatIsThisTap();
        }

        class URLSpanNoUnderline : URLSpan
        {
            public URLSpanNoUnderline(string url) : base(url)
            {
            }

            public override void UpdateDrawState(TextPaint ds)
            {
                base.UpdateDrawState(ds);
                ds.UnderlineText = false;
            }
        }

        private void OnWhatIsThisAccountStatusTap(string dialogMessage, string btnLabelText)
        {
            TooltipGenerator tooltipGenerator = new TooltipGenerator(Activity);
            tooltipGenerator.Create(dialogMessage);
            tooltipGenerator.AddAction(btnLabelText);
            tooltipGenerator.Show();
        }

        private void SetAccountStatusVisibility(ViewStates viewStates)
        {
            addressDivider.Visibility = viewStates;
            txtAccountStatus.Visibility = viewStates;
            txtWhatAccountStatus.Visibility = viewStates;
        }

        public void ShowAccountStatus(AccountStatusData accountStatusData)
        {
            if (accountStatusData.DisconnectionStatus != "Available")
            {
                SetAccountStatusVisibility(ViewStates.Visible);
                accountStatusData = null;
                string accountStatusMessage = accountStatusData?.AccountStatusMessage ?? Activity.GetString(Resource.String.chart_electricity_status_message);
                string whatDoesThisMeanLabel = accountStatusData?.AccountStatusModalTitle ?? Activity.GetString(Resource.String.tooltip_what_does_this_link);
                string whatDoesThisToolTipMessage = accountStatusData?.AccountStatusModalMessage?? Activity.GetString(Resource.String.tooltip_what_does_this_message);
                string whatDoesThisToolTipBtnLabel = accountStatusData?.AccountStatusModalBtnText ?? Activity.GetString(Resource.String.tooltip_btnLabel);
                if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.N)
                {
                    txtAccountStatus.TextFormatted = Html.FromHtml(accountStatusMessage, FromHtmlOptions.ModeLegacy);
                }
                else
                {
                    txtAccountStatus.TextFormatted = Html.FromHtml(accountStatusMessage);
                }
                txtWhatAccountStatus.Text = whatDoesThisMeanLabel;
                txtWhatAccountStatus.Click += delegate
                {
                    OnWhatIsThisAccountStatusTap(whatDoesThisToolTipMessage, whatDoesThisToolTipBtnLabel);
                };
            }
            else
            {
                SetAccountStatusVisibility(ViewStates.Gone);
            }
        }

        public string GetDeviceId()
        {
            return this.DeviceId();
        }
    }
}
