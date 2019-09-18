using AFollestad.MaterialDialogs;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.Content;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Text;
using Android.Text.Method;
using Android.Text.Style;
using Android.Util;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using CheeseBind;
using Facebook.Shimmer;
using Java.Lang;
using Java.Text;
using Java.Util;
using MikePhil.Charting.Charts;
using MikePhil.Charting.Components;
using MikePhil.Charting.Data;
using MikePhil.Charting.Formatter;
using MikePhil.Charting.Highlight;
using MikePhil.Charting.Interfaces.Datasets;
using MikePhil.Charting.Listener;
using MikePhil.Charting.Util;
using myTNB.SitecoreCMS.Model;
using myTNB_Android.Src.AppLaunch.Models;
using myTNB_Android.Src.Base;
using myTNB_Android.Src.Base.Fragments;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.FAQ.Activity;
using myTNB_Android.Src.MultipleAccountPayment.Activity;
using myTNB_Android.Src.myTNBMenu.Activity;
using myTNB_Android.Src.myTNBMenu.Adapter;
using myTNB_Android.Src.myTNBMenu.ChartRenderer;
using myTNB_Android.Src.myTNBMenu.Charts.Formatter;
using myTNB_Android.Src.myTNBMenu.Charts.SelectedMarkerView;
using myTNB_Android.Src.myTNBMenu.Listener;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.myTNBMenu.MVP.Fragment;
using myTNB_Android.Src.Notifications.Activity;
using myTNB_Android.Src.SSMR.SubmitMeterReading.MVP;
using myTNB_Android.Src.SSMRMeterHistory.MVP;
using myTNB_Android.Src.Utils;
using myTNB_Android.Src.Utils.Custom.Charts;
using myTNB_Android.Src.ViewBill.Activity;
using Newtonsoft.Json;
using Refit;
using System;
using System.Collections.Generic;
using static MikePhil.Charting.Components.XAxis;
using static MikePhil.Charting.Components.YAxis;
using static myTNB_Android.Src.myTNBMenu.Listener.NMRESMDashboardScrollView;
using static myTNB_Android.Src.myTNBMenu.Models.GetInstallationDetailsResponse;

namespace myTNB_Android.Src.myTNBMenu.Fragments
{
    public class DashboardChartFragment : BaseFragment, DashboardChartContract.IView, NMRESMDashboardScrollViewListener, ViewTreeObserver.IOnGlobalLayoutListener, MikePhil.Charting.Listener.IOnChartValueSelectedListenerSupport
    {

        [BindView(Resource.Id.totalPayableLayout)]
        RelativeLayout totalPayableLayout;

        [BindView(Resource.Id.txtDueDate)]
        TextView txtDueDate;

        [BindView(Resource.Id.rootView)]
        CoordinatorLayout rootView;

        UsageHistoryData selectedHistoryData;

        SMUsageHistoryData selectedSMHistoryData;

        AccountData selectedAccount;

        ChartType ChartType = ChartType.Month;
        ChartDataType ChartDataType = ChartDataType.RM;

        [BindView(Resource.Id.bar_chart)]
        BarChart mChart;

        [BindView(Resource.Id.txtAddress)]
        TextView txtAddress;

        [BindView(Resource.Id.addressDivider)]
        View addressDivider;

        [BindView(Resource.Id.txtRange)]
        TextView txtRange;

        [BindView(Resource.Id.txtTotalPayableTitle)]
        TextView txtTotalPayableTitle;

        [BindView(Resource.Id.txtTotalPayableCurrency)]
        TextView txtTotalPayableCurrency;

        [BindView(Resource.Id.txtTotalPayable)]
        TextView txtTotalPayable;

        [BindView(Resource.Id.btnViewBill)]
        Button btnViewBill;

        [BindView(Resource.Id.btnPay)]
        Button btnPay;

        AccountDueAmount accountDueAmountData;

        [BindView(Resource.Id.layout_graph_total)]
        LinearLayout allGraphLayout;

        [BindView(Resource.Id.layout_api_refresh)]
        LinearLayout refreshLayout;

        [BindView(Resource.Id.refresh_image)]
        ImageView refresh_image;

        [BindView(Resource.Id.btnRefresh)]
        Button btnNewRefresh;

        [BindView(Resource.Id.refresh_content)]
        TextView txtNewRefreshMessage;

        private static BottomSheetBehavior bottomSheetBehavior;

        private NMRESMDashboardScrollView scrollView;

        [BindView(Resource.Id.bottom_sheet)]
        LinearLayout bottomSheet;

        [BindView(Resource.Id.rmKwhSelection)]
        LinearLayout rmKwhSelection;

        [BindView(Resource.Id.rmKwhSelectDropdown)]
        RelativeLayout rmKwhSelectDropdown;

        [BindView(Resource.Id.rmKwhLabel)]
        TextView rmKwhLabel;

        [BindView(Resource.Id.kwhLabel)]
        TextView kwhLabel;

        [BindView(Resource.Id.rmLabel)]
        TextView rmLabel;

        [BindView(Resource.Id.dashboard_txt_account_name)]
        TextView dashboardAccountName;

        [BindView(Resource.Id.graphToggleSelection)]
        LinearLayout graphToggleSelection;

        [BindView(Resource.Id.energyTipsView)]
        LinearLayout energyTipsView;

        [BindView(Resource.Id.energyTipsList)]
        RecyclerView energyTipsList;

        [BindView(Resource.Id.energyTipsShimmerView)]
        LinearLayout energyTipsShimmerView;

        [BindView(Resource.Id.energyTipsShimmerList)]
        RecyclerView energyTipsShimmerList;


        [BindView(Resource.Id.ssmrHistoryContainer)]
        LinearLayout ssmrHistoryContainer;

        [BindView(Resource.Id.ssmrAccountStatusText)]
        TextView ssmrAccountStatusText;

        [BindView(Resource.Id.btnTxtSsmrViewHistory)]
        TextView btnTxtSsmrViewHistory;

        [BindView(Resource.Id.btnReadingHistory)]
        Button btnReadingHistory;

        EnergySavingTipsAdapter energyTipsAdapter;

        EnergySavingTipsShimmerAdapter energyTipsShimmerAdapter;

        [BindView(Resource.Id.energyDisconnectionButton)]
        LinearLayout energyDisconnectionButton;

        [BindView(Resource.Id.txtEnergyDisconnection)]
        TextView txtEnergyDisconnection;

        [BindView(Resource.Id.reContainer)]
        LinearLayout reContainer;

        [BindView(Resource.Id.reTotalPayableTitle)]
        TextView reTotalPayableTitle;

        [BindView(Resource.Id.reTotalPayable)]
        TextView reTotalPayable;

        [BindView(Resource.Id.reTotalPayableCurrency)]
        TextView reTotalPayableCurrency;

        [BindView(Resource.Id.reDueDate)]
        TextView reDueDate;

        [BindView(Resource.Id.btnReView)]
        Button btnReView;

        [BindView(Resource.Id.tarifToggle)]
        LinearLayout tarifToggle;

        [BindView(Resource.Id.imgTarifToggle)]
        ImageView imgTarifToggle;

        [BindView(Resource.Id.txtTarifToggle)]
        TextView txtTarifToggle;

        [BindView(Resource.Id.tariffBlockLegendRecyclerView)]
        RecyclerView tariffBlockLegendRecyclerView;

        [BindView(Resource.Id.scroll_view_content)]
        LinearLayout scrollViewContent;

        [BindView(Resource.Id.noPayableLayout)]
        RelativeLayout noPayableLayout;

        [BindView(Resource.Id.txtNoPayableTitle)]
        TextView txtNoPayableTitle;

        [BindView(Resource.Id.txtNoPayable)]
        TextView txtNoPayable;

        [BindView(Resource.Id.txtNoPayableCurrency)]
        TextView txtNoPayableCurrency;

        [BindView(Resource.Id.shimmrtAddressView)]
        LinearLayout shimmrtAddressView;

        [BindView(Resource.Id.shimmrtTxtAddress1)]
        ShimmerFrameLayout shimmrtTxtAddress1;

        [BindView(Resource.Id.shimmrtTxtAddress2)]
        ShimmerFrameLayout shimmrtTxtAddress2;

        [BindView(Resource.Id.shimmrtRangeView)]
        LinearLayout shimmrtRangeView;

        [BindView(Resource.Id.shimmrtTxtRange)]
        ShimmerFrameLayout shimmrtTxtRange;

        [BindView(Resource.Id.shimmrtGraphView)]
        LinearLayout shimmrtGraphView;

        [BindView(Resource.Id.shimmrtGraph)]
        ShimmerFrameLayout shimmrtGraph;

        [BindView(Resource.Id.shimmerPayableLayout)]
        RelativeLayout shimmerPayableLayout;

        [BindView(Resource.Id.shimmrtTotalPayableTitle)]
        ShimmerFrameLayout shimmrtTotalPayableTitle;

        [BindView(Resource.Id.shimmrtTotalPayable)]
        ShimmerFrameLayout shimmrtTotalPayable;

        [BindView(Resource.Id.shimmrtDueDate)]
        ShimmerFrameLayout shimmrtDueDate;

        [BindView(Resource.Id.shimmerREPayableLayout)]
        RelativeLayout shimmerREPayableLayout;

        [BindView(Resource.Id.shimmrtRETotalPayableTitle)]
        ShimmerFrameLayout shimmrtRETotalPayableTitle;

        [BindView(Resource.Id.shimmrtRETotalPayable)]
        ShimmerFrameLayout shimmrtRETotalPayable;

        [BindView(Resource.Id.shimmrtREDueDate)]
        ShimmerFrameLayout shimmrtREDueDate;

        [BindView(Resource.Id.rePayableLayout)]
        RelativeLayout rePayableLayout;

        [BindView(Resource.Id.shimmerREImg)]
        ShimmerFrameLayout shimmerREImg;

        [BindView(Resource.Id.re_img)]
        ImageView re_img;

        [BindView(Resource.Id.virtualHeight)]
        LinearLayout virtualHeight;

        [BindView(Resource.Id.shadow_layout)]
        ImageView shadowLayout;

        [BindView(Resource.Id.smStatisticContainer)]
        LinearLayout smStatisticContainer;

        [BindView(Resource.Id.sm_statistic_bill)]
        LinearLayout smStatisticBillMainLayout;

        [BindView(Resource.Id.shimmerSMStatisticBillImg)]
        ShimmerFrameLayout shimmerSMStatisticBillImg;

        [BindView(Resource.Id.shimmerSMStatisticBillLayout)]
        RelativeLayout shimmerSMStatisticBillLayout;

        [BindView(Resource.Id.shimmrtSMStatisticBillTitle)]
        ShimmerFrameLayout shimmrtSMStatisticBillTitle;

        [BindView(Resource.Id.shimmrtSMStatisticBill)]
        ShimmerFrameLayout shimmrtSMStatisticBill;

        [BindView(Resource.Id.shimmrtSMStatisticBillDueDate)]
        ShimmerFrameLayout shimmrtSMStatisticBillDueDate;

        [BindView(Resource.Id.sm_statistic_bill_img)]
        ImageView smStatisticBillImg;

        [BindView(Resource.Id.smStatisticBillLayout)]
        RelativeLayout smStatisticBillLayout;

        [BindView(Resource.Id.sm_statistic_predict)]
        LinearLayout smStatisticPredictMainLayout;

        [BindView(Resource.Id.shimmerSMStatisticPredictImg)]
        ShimmerFrameLayout shimmerSMStatisticPredictImg;

        [BindView(Resource.Id.shimmerSMStatisticPredictLayout)]
        RelativeLayout shimmerSMStatisticPredictLayout;

        [BindView(Resource.Id.shimmrtSMStatisticPredictTitle)]
        ShimmerFrameLayout shimmrtSMStatisticPredictTitle;

        [BindView(Resource.Id.shimmrtSMStatisticPredict)]
        ShimmerFrameLayout shimmrtSMStatisticPredict;

        [BindView(Resource.Id.shimmrtSMStatisticPredictDueDate)]
        ShimmerFrameLayout shimmrtSMStatisticPredictDueDate;

        [BindView(Resource.Id.shimmrtSmStatisticTooltip)]
        ShimmerFrameLayout shimmrtSmStatisticTooltip;

        [BindView(Resource.Id.sm_statistic_predict_img)]
        ImageView smStatisticPredictImg;

        [BindView(Resource.Id.smStatisticPredictLayout)]
        RelativeLayout smStatisticPredictLayout;

        [BindView(Resource.Id.smStatisticTooltip)]
        LinearLayout smStatisticTooltip;

        [BindView(Resource.Id.sm_statistic_trend)]
        LinearLayout smStatisticTrendMainLayout;

        [BindView(Resource.Id.smStatisticBillTitle)]
        TextView smStatisticBillTitle;

        [BindView(Resource.Id.smStatisticBillSubTitle)]
        TextView smStatisticBillSubTitle;

        [BindView(Resource.Id.smStatisticBill)]
        TextView smStatisticBill;

        [BindView(Resource.Id.smStatisticBillCurrency)]
        TextView smStatisticBillCurrency;

        [BindView(Resource.Id.smStatisticBillKwhUnit)]
        TextView smStatisticBillKwhUnit;

        [BindView(Resource.Id.smStatisticBillKwh)]
        TextView smStatisticBillKwh;

        [BindView(Resource.Id.smStatisticPredictTitle)]
        TextView smStatisticPredictTitle;

        [BindView(Resource.Id.smStatisticPredictSubTitle)]
        TextView smStatisticPredictSubTitle;

        [BindView(Resource.Id.smStatisticPredict)]
        TextView smStatisticPredict;

        [BindView(Resource.Id.smStatisticPredictCurrency)]
        TextView smStatisticPredictCurrency;

        [BindView(Resource.Id.txtSmStatisticTooltip)]
        TextView txtSmStatisticTooltip;

        [BindView(Resource.Id.smStatisticTrendTitle)]
        TextView smStatisticTrendTitle;

        [BindView(Resource.Id.smStatisticTrendSubTitle)]
        TextView smStatisticTrendSubTitle;

        [BindView(Resource.Id.smStatisticTrend)]
        TextView smStatisticTrend;

        [BindView(Resource.Id.layoutSegmentGroup)]
        RelativeLayout layoutSMSegmentGroup;

        [BindView(Resource.Id.btnToggleDay)]
        RadioButton btnToggleDay;

        [BindView(Resource.Id.btnToggleMonth)]
        RadioButton btnToggleMonth;

        TariffBlockLegendAdapter tariffBlockLegendAdapter;

        private DashboardChartContract.IUserActionsListener userActionsListener;
        private DashboardChartPresenter mPresenter;

        private string txtRefreshMsg = "";
        private string txtBtnRefreshTitle = "";

        private bool isSubmitMeter = false;

        private bool isBackendTariffDisabled = false;

        private static bool isREAccount = false;

        DecimalFormat decimalFormat = new DecimalFormat("#,###,###,###,##0.00");
        SimpleDateFormat dateParser = new SimpleDateFormat("dd/MM/yyyy");
        SimpleDateFormat dateFormatter = new SimpleDateFormat("dd MMM yyyy");
        IAxisValueFormatter XLabelsFormatter;
        private int currentParentIndex = 0;
        private string errorMSG = null;

        private MaterialDialog mWhyThisAmtCardDialog;

        public readonly static int SSMR_METER_HISTORY_ACTIVITY_CODE = 8796;

        public readonly static int SSMR_SUBMIT_METER_ACTIVITY_CODE = 8797;

        private SMRActivityInfoResponse smrResponse;

        private AccountDueAmountResponse amountDueResponse;

        private GetInstallationDetailsResponse accountStatusResponse;

        bool isToggleTariff = false;

        static bool isBCRMDown = false;

        static bool isPaymentDown = false;

        static bool isUsageLoadedNeeded = true;

        private bool isChangeBackgroundNeeded = true;

        private bool isScrollIndicatorShowNeed = false;

        private bool isSMR = false;

        bool isSMDown = false;

        bool isSMAccount = false;

        public StackedBarChartRenderer renderer;

        public SMStackedBarChartRenderer smRenderer;

        static bool requireScroll;

        private int CurrentParentIndex = -1;

        private List<double> DayViewRMData = new List<double>();
        private List<double> DayViewkWhData = new List<double>();

        public override int ResourceId()
        {
            return Resource.Layout.DashboardNewChartView;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Bundle extras = Arguments;

            if (extras.ContainsKey(Constants.SELECTED_ACCOUNT))
            {
                selectedAccount = JsonConvert.DeserializeObject<AccountData>(extras.GetString(Constants.SELECTED_ACCOUNT));
            }

            if (extras.ContainsKey(Constants.SELECTED_ACCOUNT_USAGE_RESPONSE) || extras.ContainsKey(Constants.SELECTED_SM_ACCOUNT_USAGE_RESPONSE))
            {
                if (extras.ContainsKey(Constants.SELECTED_ACCOUNT_USAGE_RESPONSE))
                {
                    isSMAccount = false;
                    isUsageLoadedNeeded = false;
                    selectedSMHistoryData = null;
                    var usageHistoryDataResponse = JsonConvert.DeserializeObject<UsageHistoryResponse>(extras.GetString(Constants.SELECTED_ACCOUNT_USAGE_RESPONSE));
                    if (usageHistoryDataResponse != null && usageHistoryDataResponse.Data != null && usageHistoryDataResponse.Data.UsageHistoryData != null)
                    {
                        selectedHistoryData = usageHistoryDataResponse.Data.UsageHistoryData;
                        if (!usageHistoryDataResponse.Data.IsMonthlyTariffBlocksDisabled && !usageHistoryDataResponse.Data.IsMonthlyTariffBlocksUnavailable)
                        {
                            OnSetBackendTariffDisabled(false);
                        }
                        else
                        {
                            OnSetBackendTariffDisabled(true);
                        }
                    }
                    try
                    {
                        if (usageHistoryDataResponse != null && usageHistoryDataResponse.Data != null && usageHistoryDataResponse.Data.RefreshMessage != null && !string.IsNullOrEmpty(usageHistoryDataResponse.Data.RefreshMessage))
                        {
                            txtRefreshMsg = usageHistoryDataResponse.Data.RefreshMessage;
                        }
                        else
                        {
                            txtRefreshMsg = "Uh oh, looks like this page is unplugged. Reload to stay plugged in!";
                        }
                    }
                    catch (System.Exception e)
                    {
                        txtRefreshMsg = "Uh oh, looks like this page is unplugged. Reload to stay plugged in!";
                        Utility.LoggingNonFatalError(e);
                    }
                    try
                    {
                        if (usageHistoryDataResponse != null && usageHistoryDataResponse.Data != null && usageHistoryDataResponse.Data.RefreshBtnText != null && !string.IsNullOrEmpty(usageHistoryDataResponse.Data.RefreshBtnText))
                        {
                            txtBtnRefreshTitle = usageHistoryDataResponse.Data.RefreshBtnText;
                        }
                        else
                        {
                            txtBtnRefreshTitle = "Reload Now";
                        }
                    }
                    catch (System.Exception e)
                    {
                        txtBtnRefreshTitle = "Reload Now";
                        Utility.LoggingNonFatalError(e);
                    }
                }
                else
                {
                    isSMAccount = true;
                    isUsageLoadedNeeded = false;
                    selectedHistoryData = null;
                    var usageHistoryDataResponse = JsonConvert.DeserializeObject<SMUsageHistoryResponse>(extras.GetString(Constants.SELECTED_SM_ACCOUNT_USAGE_RESPONSE));
                    if (usageHistoryDataResponse != null && usageHistoryDataResponse.Data != null && usageHistoryDataResponse.Data.SMUsageHistoryData != null)
                    {
                        selectedSMHistoryData = usageHistoryDataResponse.Data.SMUsageHistoryData;
                        if (!usageHistoryDataResponse.Data.IsMonthlyTariffBlocksDisabled && !usageHistoryDataResponse.Data.IsMonthlyTariffBlocksUnavailable)
                        {
                            OnSetBackendTariffDisabled(false);
                        }
                        else
                        {
                            OnSetBackendTariffDisabled(true);
                        }
                    }
                    try
                    {
                        if (usageHistoryDataResponse != null && usageHistoryDataResponse.Data != null && usageHistoryDataResponse.Data.RefreshMessage != null && !string.IsNullOrEmpty(usageHistoryDataResponse.Data.RefreshMessage))
                        {
                            txtRefreshMsg = usageHistoryDataResponse.Data.RefreshMessage;
                        }
                        else
                        {
                            txtRefreshMsg = "Uh oh, looks like this page is unplugged. Reload to stay plugged in!";
                        }
                    }
                    catch (System.Exception e)
                    {
                        txtRefreshMsg = "Uh oh, looks like this page is unplugged. Reload to stay plugged in!";
                        Utility.LoggingNonFatalError(e);
                    }
                    try
                    {
                        if (usageHistoryDataResponse != null && usageHistoryDataResponse.Data != null && usageHistoryDataResponse.Data.RefreshBtnText != null && !string.IsNullOrEmpty(usageHistoryDataResponse.Data.RefreshBtnText))
                        {
                            txtBtnRefreshTitle = usageHistoryDataResponse.Data.RefreshBtnText;
                        }
                        else
                        {
                            txtBtnRefreshTitle = "Reload Now";
                        }
                    }
                    catch (System.Exception e)
                    {
                        txtBtnRefreshTitle = "Reload Now";
                        Utility.LoggingNonFatalError(e);
                    }
                }
            }
            else
            {
                if (selectedAccount != null && !selectedAccount.SmartMeterCode.Equals("0"))
                {
                    isSMAccount = true;
                }
                else
                {
                    isSMAccount = false;
                }
                isUsageLoadedNeeded = true;
                selectedHistoryData = null;
                selectedSMHistoryData = null;
                txtRefreshMsg = "Uh oh, looks like this page is unplugged. Reload to stay plugged in!";
                txtBtnRefreshTitle = "Reload Now";
            }

            errorMSG = "";

            if (extras.ContainsKey(Constants.SELECTED_ERROR_MSG))
            {
                errorMSG = extras.GetString(Constants.SELECTED_ERROR_MSG);
            }


            SetHasOptionsMenu(true);
            this.mPresenter = new DashboardChartPresenter(this);
        }

        internal static DashboardChartFragment NewInstance(UsageHistoryResponse usageHistoryResponse, AccountData accountData, string error, string errorMessage)
        {
            DashboardChartFragment chartFragment = new DashboardChartFragment();
            Bundle bundle = new Bundle();
            if (usageHistoryResponse != null)
            {
                bundle.PutString(Constants.SELECTED_ACCOUNT_USAGE_RESPONSE, JsonConvert.SerializeObject(usageHistoryResponse));
            }
            if (accountData != null)
            {
                bundle.PutString(Constants.SELECTED_ACCOUNT, JsonConvert.SerializeObject(accountData));
            }
            if (!string.IsNullOrEmpty(error))
            {
                bundle.PutString(Constants.SELECTED_ERROR, error);
            }

            if (!string.IsNullOrEmpty(errorMessage))
            {
                bundle.PutString(Constants.SELECTED_ERROR_MSG, errorMessage);
            }

            chartFragment.Arguments = bundle;
            return chartFragment;
        }

        internal static DashboardChartFragment NewInstance(SMUsageHistoryResponse usageHistoryResponse, AccountData accountData)
        {
            DashboardChartFragment chartFragment = new DashboardChartFragment();
            Bundle bundle = new Bundle();
            if (usageHistoryResponse != null)
            {
                bundle.PutString(Constants.SELECTED_SM_ACCOUNT_USAGE_RESPONSE, JsonConvert.SerializeObject(usageHistoryResponse));
            }
            if (accountData != null)
            {
                bundle.PutString(Constants.SELECTED_ACCOUNT, JsonConvert.SerializeObject(accountData));
            }

            chartFragment.Arguments = bundle;
            return chartFragment;
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            try
            {
                bottomSheetBehavior = BottomSheetBehavior.From(bottomSheet);
                bottomSheetBehavior.State = BottomSheetBehavior.StateExpanded;
                bottomSheetBehavior.SetBottomSheetCallback(new DashboardBottomSheetCallBack());

                scrollView = view.FindViewById<NMRESMDashboardScrollView>(Resource.Id.scroll_view);
                ViewTreeObserver observer = scrollView.ViewTreeObserver;
                observer.AddOnGlobalLayoutListener(this);

                scrollViewContent.SetBackgroundResource(Resource.Drawable.dashboard_chart_bg);

                requireScroll = false;

                TextViewUtils.SetMuseoSans300Typeface(txtAddress, txtTotalPayable, txtDueDate);
                TextViewUtils.SetMuseoSans300Typeface(txtNewRefreshMessage, ssmrAccountStatusText);
                TextViewUtils.SetMuseoSans500Typeface(txtRange, txtTotalPayableTitle, txtTotalPayableCurrency, btnViewBill, btnPay, btnNewRefresh, rmKwhLabel, kwhLabel, rmLabel, dashboardAccountName, btnTxtSsmrViewHistory, btnReadingHistory, txtEnergyDisconnection);
                TextViewUtils.SetMuseoSans300Typeface(reTotalPayable, reTotalPayableCurrency, reDueDate, txtNoPayable);
                TextViewUtils.SetMuseoSans500Typeface(reTotalPayableTitle, btnReView, txtTarifToggle, txtNoPayableTitle, txtNoPayableCurrency);
                TextViewUtils.SetMuseoSans300Typeface(smStatisticBillSubTitle, smStatisticBill, smStatisticBillCurrency, smStatisticBillKwhUnit, smStatisticBillKwh, smStatisticPredictSubTitle, smStatisticPredict, smStatisticPredictCurrency, smStatisticTrendSubTitle, smStatisticTrend);
                TextViewUtils.SetMuseoSans500Typeface(smStatisticBillTitle, smStatisticPredictTitle, txtSmStatisticTooltip, smStatisticTrendTitle);
                TextViewUtils.SetMuseoSans300Typeface(btnToggleDay, btnToggleMonth);

                DownTimeEntity bcrmEntity = DownTimeEntity.GetByCode(Constants.BCRM_SYSTEM);
                DownTimeEntity pgCCEntity = DownTimeEntity.GetByCode(Constants.PG_CC_SYSTEM);
                DownTimeEntity pgFPXEntity = DownTimeEntity.GetByCode(Constants.PG_FPX_SYSTEM);

                if (bcrmEntity != null && bcrmEntity.IsDown)
                {
                    isBCRMDown = true;
                }
                else
                {
                    isBCRMDown = false;

                    if (pgCCEntity.IsDown && pgFPXEntity.IsDown)
                    {
                        isPaymentDown = true;

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
                    else
                    {
                        isPaymentDown = false;
                    }
                }

                try
                {
                    ((DashboardHomeActivity)Activity).SetStatusBarBackground(Resource.Drawable.dashboard_fluid_background);
                    ((DashboardHomeActivity)Activity).SetToolbarBackground(Resource.Drawable.CustomDashboardGradientToolbar);
                }
                catch (System.Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                }

                if (selectedAccount != null)
                {
                    txtAddress.Text = selectedAccount.AddStreet;
                    if (selectedAccount.AccountCategoryId.Equals("2"))
                    {
                        bottomSheetBehavior.State = BottomSheetBehavior.StateHidden;
                        isREAccount = true;
                        reContainer.Visibility = ViewStates.Visible;
                        ssmrHistoryContainer.Visibility = ViewStates.Gone;
                        btnPay.Visibility = ViewStates.Gone;
                        energyTipsView.Visibility = ViewStates.Gone;
                        btnViewBill.Text = GetString(Resource.String.dashboard_chart_view_payment_advice);
                        // txtUsageHistory.Visibility = ViewStates.Gone;
                        txtTotalPayableTitle.Text = GetString(Resource.String.title_payment_advice_amount);
                        graphToggleSelection.Visibility = ViewStates.Gone;
                        SetVirtualHeightParams(6f);
                        isChangeBackgroundNeeded = true;
                        layoutSMSegmentGroup.Visibility = ViewStates.Gone;
                        isSMR = false;
                    }
                    else if (isSMAccount)
                    {
                        // Smart Meter
                        SetVirtualHeightParams(80f);
                        isREAccount = false;
                        reContainer.Visibility = ViewStates.Gone;
                        btnPay.Visibility = ViewStates.Visible;
                        btnViewBill.Text = GetString(Resource.String.dashboard_chartview_view_bill);
                        graphToggleSelection.Visibility = ViewStates.Visible;
                        energyTipsView.Visibility = ViewStates.Visible;
                        scrollViewContent.SetBackgroundResource(Resource.Drawable.dashboard_chart_sm_bg);
                        isChangeBackgroundNeeded = true;
                        layoutSMSegmentGroup.Visibility = ViewStates.Visible;
                        isSMR = false;
                        // Lin Siong TODO: Last bar tap event to day view
                        // Lin Siong TODO: Stripped bar background implementation
                        // Lin Siong TODO: Estimated Reading Handling & Display
                        // Lin Siong TODO: Graph Explanatory ToolTip
                        // Lin Siong TODO: Fallback for Error from MDMS service
                    }
                    else
                    {
                        isSMR = this.mPresenter.IsOwnedSMR(selectedAccount.AccountNum);
                        if (isSMR)
                        {
                            SetVirtualHeightParams(80f);
                            isChangeBackgroundNeeded = true;
                        }
                        else
                        {
                            rootView.SetBackgroundResource(0);
                            scrollViewContent.SetBackgroundResource(0);
                            SetVirtualHeightParams(50f);
                            try
                            {
                                ((DashboardHomeActivity)Activity).SetStatusBarBackground(Resource.Drawable.NewHorizontalGradientBackground);
                                ((DashboardHomeActivity)Activity).UnsetToolbarBackground();
                            }
                            catch (System.Exception e)
                            {
                                Utility.LoggingNonFatalError(e);
                            }
                            isChangeBackgroundNeeded = false;
                        }
                        isREAccount = false;
                        reContainer.Visibility = ViewStates.Gone;
                        btnPay.Visibility = ViewStates.Visible;
                        layoutSMSegmentGroup.Visibility = ViewStates.Gone;
                        btnViewBill.Text = GetString(Resource.String.dashboard_chartview_view_bill);
                        graphToggleSelection.Visibility = ViewStates.Visible;
                        energyTipsView.Visibility = ViewStates.Visible;
                    }

                    if (selectedAccount.AccountCategoryId.Equals("2"))
                    {
                        try
                        {
                            FirebaseAnalyticsUtils.SetFragmentScreenName(this, "RE Inner Dashboard");
                        }
                        catch (System.Exception e)
                        {
                            Utility.LoggingNonFatalError(e);
                        }
                    }
                    else if (!selectedAccount.SmartMeterCode.Equals("0"))
                    {
                        try
                        {
                            FirebaseAnalyticsUtils.SetFragmentScreenName(this, "Smart Meter Inner Dashboard");
                        }
                        catch (System.Exception e)
                        {
                            Utility.LoggingNonFatalError(e);
                        }
                    }
                    else
                    {
                        try
                        {
                            if (!string.IsNullOrEmpty(errorMSG))
                            {
                                FirebaseAnalyticsUtils.SetFragmentScreenName(this, "Smart Meter Inner Dashboard");
                            }
                            else
                            {
                                FirebaseAnalyticsUtils.SetFragmentScreenName(this, "Normal Inner Dashboard");
                            }
                        }
                        catch (System.Exception e)
                        {
                            Utility.LoggingNonFatalError(e);
                        }
                    }
                }
                else
                {
                    rootView.SetBackgroundResource(0);
                    scrollViewContent.SetBackgroundResource(0);
                    SetVirtualHeightParams(6f);
                    try
                    {
                        ((DashboardHomeActivity)Activity).SetStatusBarBackground(Resource.Drawable.NewHorizontalGradientBackground);
                        ((DashboardHomeActivity)Activity).UnsetToolbarBackground();
                    }
                    catch (System.Exception e)
                    {
                        Utility.LoggingNonFatalError(e);
                    }
                    isChangeBackgroundNeeded = false;
                    isSMR = false;
                    energyTipsView.Visibility = ViewStates.Gone;
                    try
                    {
                        FirebaseAnalyticsUtils.SetFragmentScreenName(this, "Normal / RE Inner Dashboard");
                    }
                    catch (System.Exception e)
                    {
                        Utility.LoggingNonFatalError(e);
                    }
                }

                if (isBCRMDown)
                {
                    ShowAmountDueNotAvailable();

                    HideSSMRDashboardView();
                    energyTipsView.Visibility = ViewStates.Gone;
                    dashboardAccountName.Visibility = ViewStates.Gone;

                    if (Android.OS.Build.VERSION.SdkInt >= Android.OS.Build.VERSION_CODES.N)
                    {
                        txtNewRefreshMessage.TextFormatted = Html.FromHtml(bcrmEntity.DowntimeMessage, FromHtmlOptions.ModeLegacy);
                    }
                    else
                    {
                        txtNewRefreshMessage.TextFormatted = Html.FromHtml(bcrmEntity.DowntimeMessage);
                    }

                    this.userActionsListener?.Start();

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
                    ((DashboardHomeActivity)Activity).HideAccountName();
                    // Lin Siong Note: Enable it when design confirm
                    // dashboardAccountName.Visibility = ViewStates.Gone;
                    dashboardAccountName.Visibility = ViewStates.Visible;
                    dashboardAccountName.Text = selectedAccount.AccountNickName;
                    List<CustomerBillingAccount> accountList = CustomerBillingAccount.List();
                    bool enableDropDown = accountList.Count > 0 ? true : false;
                    if (enableDropDown)
                    {
                        Drawable dropdown = ContextCompat.GetDrawable(this.Activity, Resource.Drawable.ic_spinner_dropdown);
                        Drawable transparentDropDown = ContextCompat.GetDrawable(this.Activity, Resource.Drawable.ic_action_dropdown);
                        transparentDropDown.Alpha = 0;
                        dashboardAccountName.SetCompoundDrawablesWithIntrinsicBounds(transparentDropDown, null, dropdown, null);
                    }
                    else
                    {
                        dashboardAccountName.SetCompoundDrawablesWithIntrinsicBounds(0, 0, 0, 0);
                    }

                    tariffBlockLegendRecyclerView.Visibility = ViewStates.Gone;
                    LinearLayoutManager linearTariffBlockLayoutManager = new LinearLayoutManager(this.Activity, LinearLayoutManager.Vertical, false);
                    tariffBlockLegendRecyclerView.SetLayoutManager(linearTariffBlockLayoutManager);

                    energyTipsView.Visibility = ViewStates.Gone;
                    energyTipsShimmerView.Visibility = ViewStates.Gone;

                    LinearLayoutManager linearEnergyTipLayoutManager = new LinearLayoutManager(this.Activity, LinearLayoutManager.Horizontal, false);
                    energyTipsList.SetLayoutManager(linearEnergyTipLayoutManager);
                    energyTipsList.NestedScrollingEnabled = true;

                    LinearSnapHelper snapHelper = new LinearSnapHelper();
                    snapHelper.AttachToRecyclerView(energyTipsList);

                    LinearLayoutManager linearEnergyTipShimmerLayoutManager = new LinearLayoutManager(this.Activity, LinearLayoutManager.Horizontal, false);
                    energyTipsShimmerList.SetLayoutManager(linearEnergyTipShimmerLayoutManager);
                    energyTipsShimmerList.NestedScrollingEnabled = true;

                    LinearSnapHelper snapShimmerHelper = new LinearSnapHelper();
                    snapShimmerHelper.AttachToRecyclerView(energyTipsShimmerList);

                    DisablePayButton();
                    DisableViewBillButton();

                    energyDisconnectionButton.Visibility = ViewStates.Gone;

                    HideSSMRDashboardView();

                    if (isUsageLoadedNeeded)
                    {
                        rmKwhSelection.Enabled = false;
                        tarifToggle.Enabled = false;
                        txtRange.Visibility = ViewStates.Gone;
                        StartRangeShimmer();
                        mChart.Visibility = ViewStates.Gone;
                        StartGraphShimmer();
                    }
                    else
                    {
                        rmKwhSelection.Enabled = true;
                        tarifToggle.Enabled = true;
                    }

                    re_img.Visibility = ViewStates.Gone;
                    rePayableLayout.Visibility = ViewStates.Gone;
                    totalPayableLayout.Visibility = ViewStates.Gone;
                    noPayableLayout.Visibility = ViewStates.Gone;

                    StartAmountDueShimmer();

                    StartSMStatisticShimmer();

                    energyDisconnectionButton.Visibility = ViewStates.Gone;

                    // Lin Siong Note: Energy Saving Tip On Start Shimmer and get data
                    if (selectedAccount != null)
                    {
                        if (!selectedAccount.AccountCategoryId.Equals("2"))
                        {
                            bool isGetEnergyTipsDisabled = false;
                            if (MyTNBAccountManagement.GetInstance() != null && MyTNBAccountManagement.GetInstance().GetCurrentMasterData() != null && MyTNBAccountManagement.GetInstance().GetCurrentMasterData().Data != null && MyTNBAccountManagement.GetInstance().GetCurrentMasterData().Data.IsEnergyTipsDisabled)
                            {
                                isGetEnergyTipsDisabled = true;
                            }

                            if (!isGetEnergyTipsDisabled)
                            {
                                OnGetEnergyTipsItems();
                            }
                        }
                    }                  

                    this.userActionsListener?.Start();

                    if (!string.IsNullOrEmpty(errorMSG))
                    {
                        ShowUnableToFecthSmartMeterData(errorMSG);
                    }
                }

                txtNewRefreshMessage.Click += delegate
                {
                    if (isBCRMDown)
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

                        try
                        {
                            FirebaseAnalyticsUtils.LogFragmentClickEvent(this, "BCRM Downtime Message Click");
                        }
                        catch (System.Exception e)
                        {
                            Utility.LoggingNonFatalError(e);
                        }
                    }

                };

            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        [OnClick(Resource.Id.dashboard_txt_account_name)]
        void OnSelectSupplyAccount(object sender, EventArgs eventArgs)
        {
            try
            {
                ((DashboardHomeActivity)Activity).OnSelectAccount();
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
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

        [OnClick(Resource.Id.smStatisticTooltip)]
        void OnSMStatisticTooltipClick(object sender, EventArgs eventArgs)
        {
            try
            {
                string textMessage = Activity.GetString(Resource.String.tooltip_sm_what_are_these_message);
                string btnLabel = Activity.GetString(Resource.String.tooltip_btnLabel);

                if (selectedSMHistoryData != null && selectedSMHistoryData.OtherUsageMetrics != null && selectedSMHistoryData.ToolTips != null && selectedSMHistoryData.ToolTips.Count > 0)
                {
                    textMessage = "";
                    foreach (SMUsageHistoryData.SmartMeterToolTips costValue in selectedSMHistoryData.ToolTips)
                    {
                        if (costValue.Type == Constants.PROJECTED_COST_KEY)
                        {
                            if (costValue.Message != null && costValue.Message.Count > 0)
                            {
                                foreach (string stringValue in costValue.Message)
                                {
                                    textMessage += stringValue;
                                }
                            }

                            btnLabel = costValue.SMBtnText ?? Activity.GetString(Resource.String.tooltip_btnLabel);
                        }
                    }
                }

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

                if (textMessage != "")
                {
                    if (Android.OS.Build.VERSION.SdkInt >= Android.OS.Build.VERSION_CODES.N)
                    {
                        dialogDetailsText.TextFormatted = Html.FromHtml(textMessage, FromHtmlOptions.ModeLegacy);
                    }
                    else
                    {
                        dialogDetailsText.TextFormatted = Html.FromHtml(textMessage);
                    }
                }

                if (dialogDetailsText != null)
                {
                    TextViewUtils.SetMuseoSans300Typeface(dialogDetailsText);
                }
                if (dialogBtnLabel != null)
                {
                    TextViewUtils.SetMuseoSans500Typeface(dialogBtnLabel);
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
                materialDialog.Show();
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        [OnClick(Resource.Id.btnTxtSsmrViewHistory)]
        void OnSsmrViewHistory(object sender, EventArgs eventArgs)
        {
            try
            {
                StartSSMRMeterHistoryPage();
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        [OnClick(Resource.Id.btnReadingHistory)]
        void OnBtnSsmrViewHistory(object sender, EventArgs eventArgs)
        {
            try
            {
                if (isSubmitMeter)
                {
                    StartSSMRSubmitMeterReadingPage();
                }
                else
                {
                    StartSSMRMeterHistoryPage();
                }
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
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

        private void StartSSMRMeterHistoryPage()
        {
            Intent ssmr_history_activity = new Intent(this.Activity, typeof(SSMRMeterHistoryActivity));
            ssmr_history_activity.PutExtra(Constants.SELECTED_ACCOUNT, JsonConvert.SerializeObject(selectedAccount));
            ssmr_history_activity.PutExtra(Constants.SMR_RESPONSE_KEY, JsonConvert.SerializeObject(smrResponse));
            ssmr_history_activity.PutExtra("fromUsage", true);
            StartActivityForResult(ssmr_history_activity, SSMR_METER_HISTORY_ACTIVITY_CODE);
        }

        private void StartSSMRSubmitMeterReadingPage()
        {
            Intent ssmr_submit_meter_activity = new Intent(this.Activity, typeof(SubmitMeterReadingActivity));
            ssmr_submit_meter_activity.PutExtra(Constants.SELECTED_ACCOUNT, JsonConvert.SerializeObject(selectedAccount));
            ssmr_submit_meter_activity.PutExtra(Constants.SMR_RESPONSE_KEY, JsonConvert.SerializeObject(smrResponse));
            StartActivityForResult(ssmr_submit_meter_activity, SSMR_SUBMIT_METER_ACTIVITY_CODE);
        }

        internal void SetUp()
        {
            StopAddressShimmer();
            StopRangeShimmer();
            StopGraphShimmer();

            txtAddress.Visibility = ViewStates.Visible;

            txtRange.Visibility = ViewStates.Visible;

            mChart.Visibility = ViewStates.Visible;

            // Lin Siong TODO: This part to determine if tariff is available or not, soon will replace by flag
            // Lin Siong TODO: And also UI handling for this
            bool isTariffAvailable = true;
            if (isBackendTariffDisabled)
            {
                isTariffAvailable = false;
            }
            else
            {
                if (isSMAccount)
                {
                    if (selectedSMHistoryData != null && selectedSMHistoryData.ByMonth != null && selectedSMHistoryData.ByMonth.Months != null && selectedSMHistoryData.ByMonth.Months.Count > 0)
                    {
                        for (int i = 0; i < selectedSMHistoryData.ByMonth.Months.Count; i++)
                        {
                            if (selectedSMHistoryData.ByMonth.Months[i].TariffBlocksList != null && selectedSMHistoryData.ByMonth.Months[i].TariffBlocksList.Count > 0)
                            {
                                isTariffAvailable = true;
                                break;
                            }
                            else
                            {
                                isTariffAvailable = false;
                            }
                        }
                    }
                    else
                    {
                        isTariffAvailable = false;
                    }
                }
                else
                {
                    if (selectedHistoryData != null && selectedHistoryData.ByMonth != null && selectedHistoryData.ByMonth.Months != null && selectedHistoryData.ByMonth.Months.Count > 0)
                    {
                        for (int i = 0; i < selectedHistoryData.ByMonth.Months.Count; i++)
                        {
                            if (selectedHistoryData.ByMonth.Months[i].TariffBlocksList != null && selectedHistoryData.ByMonth.Months[i].TariffBlocksList.Count > 0)
                            {
                                isTariffAvailable = true;
                                break;
                            }
                            else
                            {
                                isTariffAvailable = false;
                            }
                        }
                    }
                    else
                    {
                        isTariffAvailable = false;
                    }
                }
            }

            ShowSMStatisticCard();

            if (isTariffAvailable)
            {
                tarifToggle.Enabled = true;
                if (!isToggleTariff)
                {
                    imgTarifToggle.SetImageResource(Resource.Drawable.eye);
                    txtTarifToggle.Text = "Show Tariff";
                }
                else
                {
                    imgTarifToggle.SetImageResource(Resource.Drawable.eye_hide);
                    txtTarifToggle.Text = "Hide Tariff";
                }
            }
            else
            {
                tarifToggle.Enabled = false;
                isToggleTariff = false;
                imgTarifToggle.SetImageResource(Resource.Drawable.eye_disable);
                txtTarifToggle.Text = "Show Tariff";
                txtTarifToggle.SetTextColor(ContextCompat.GetColorStateList(this.Activity, Resource.Color.silverChalice));
            }

            if (isSMAccount)
            {
                // Lin Siong Note: this is for smart meter Inner Dashboard
                // Lin Siong Note: the isStacked is to determine whether want to have spacing or not
                // Lin Siong Note: isStacked = true -> have spacing
                // Lin Siong Note: isStacked = false -> no spacing

                // Lin Siong TODO: To Add day view chart view render on smart meter
                // 

                if (!isSMDown)
                {
                    if (isToggleTariff)
                    {
                        smRenderer = new SMStackedBarChartRenderer(mChart, mChart.Animator, mChart.ViewPortHandler)
                        {
                            selectedSMHistoryData = selectedSMHistoryData,
                            currentContext = Activity,
                            isStacked = true,
                            currentChartType = ChartType,
                            currentChartDataType = ChartDataType
                        };
                        mChart.Renderer = smRenderer;
                    }
                    else
                    {
                        smRenderer = new SMStackedBarChartRenderer(mChart, mChart.Animator, mChart.ViewPortHandler)
                        {
                            selectedSMHistoryData = selectedSMHistoryData,
                            currentContext = Activity,
                            isStacked = false,
                            currentChartType = ChartType,
                            currentChartDataType = ChartDataType
                        };
                        mChart.Renderer = smRenderer;
                    }
                }
                else
                {
                    // Lin Siong TODO: SM Downtime Handling
                    // Lin Siong TODO: To use back the SMStackedBarChartRenderer with new flag inside like isDowntime = true
                    // Lin Siong TODO: Then draw the unavialble things out
                }
            }
            else
            {
                // Lin Siong Note: this is for normal / RE Inner Dashboard
                // Lin Siong Note: Only tariff is using the renderer as it's the only need
                if (isToggleTariff)
                {
                    renderer = new StackedBarChartRenderer(mChart, mChart.Animator, mChart.ViewPortHandler)
                    {
                        selectedHistoryData = selectedHistoryData,
                        currentContext = Activity
                    };
                    mChart.Renderer = renderer;
                }
                else
                {
                    mChart.SetRoundedBarRadius(100f);
                }
            }

            if (ChartType == ChartType.Month)
            {
                mChart.SetDrawBarShadow(false);
                mChart.SetDrawValueAboveBar(true);
                mChart.Description.Enabled = false;
                mChart.SetMaxVisibleValueCount(7);
                mChart.SetPinchZoom(false);
                mChart.SetDrawGridBackground(false);
                mChart.SetScaleEnabled(false);
                mChart.Legend.Enabled = false;
                mChart.AnimateY(1000);
            }
            else
            {
                mChart.SetDrawBarShadow(false);
                mChart.SetDrawValueAboveBar(true);
                mChart.Description.Enabled = false;
                mChart.SetMaxVisibleValueCount(32);
                mChart.SetPinchZoom(false);
                mChart.SetDrawGridBackground(false);
                mChart.SetScaleEnabled(false);
                mChart.Legend.Enabled = false;
                mChart.AnimateY(1000);
            }

            txtAddress.Text = selectedAccount.AddStreet;

            // Lin Siong TODO: Estimated Reading Handling & Display on graph X Axis on Smart Meter

            if (isSMAccount)
            {
                if (ChartType == ChartType.Month)
                {
                    if (ChartDataType == ChartDataType.RM)
                    {

                        txtRange.Text = selectedSMHistoryData.ByMonth.Range;

                        // SETUP XAXIS

                        SetUpXAxis();

                        // SETUP YAXIS

                        SetUpYAxis();

                        // ADD DATA

                        SetData(selectedSMHistoryData.ByMonth.Months.Count);


                        // SETUP MARKER VIEW

                        SetUpMarkerRMView();

                    }
                    else
                    {

                        txtRange.Text = selectedSMHistoryData.ByMonth.Range;
                        // SETUP XAXIS

                        SetUpXAxiskWh();

                        // SETUP YAXIS

                        SetUpYAxisKwh();

                        // ADD DATA
                        SetKWhData(selectedSMHistoryData.ByMonth.Months.Count);

                        // SETUP MARKER VIEW

                        SetUpMarkerKWhView();
                    }
                }
                else if (ChartType == ChartType.Day)
                {
                    if (ChartDataType == ChartDataType.RM)
                    {
                        // Lin Siong TODO: txtRange date update from day view
                        txtRange.Text = selectedSMHistoryData.ByMonth.Range;

                        // SETUP XAXIS

                        SetUpXAxis();

                        // SETUP YAXIS

                        SetUpYAxis();

                        // ADD DATA
                        DayViewRMData = new List<double>();
                        if(selectedSMHistoryData != null && selectedSMHistoryData.ByDay != null && selectedSMHistoryData.ByDay.Count > 0)
                        {
                            foreach (SMUsageHistoryData.ByDayData DayData in selectedSMHistoryData.ByDay)
                            {
                                foreach (SMUsageHistoryData.ByDayData.DayData IndividualDayData in DayData.Days)
                                {
                                    DayViewRMData.Add(IndividualDayData.Amount);
                                }
                            }
                        }
                        SetData(DayViewRMData.Count);

                        // SETUP MARKER VIEW

                        SetUpMarkerRMView();

                    }
                    else
                    {

                        txtRange.Text = selectedSMHistoryData.ByMonth.Range;
                        // SETUP XAXIS

                        SetUpXAxiskWh();

                        // SETUP YAXIS

                        SetUpYAxisKwh();

                        // ADD DATA
                        DayViewkWhData = new List<double>();
                        if (selectedSMHistoryData != null && selectedSMHistoryData.ByDay != null && selectedSMHistoryData.ByDay.Count > 0)
                        {
                            foreach (SMUsageHistoryData.ByDayData DayData in selectedSMHistoryData.ByDay)
                            {
                                foreach (SMUsageHistoryData.ByDayData.DayData IndividualDayData in DayData.Days)
                                {
                                    DayViewkWhData.Add(IndividualDayData.Consumption);
                                }
                            }
                        }
                        SetKWhData(DayViewkWhData.Count);

                        // SETUP MARKER VIEW

                        SetUpMarkerKWhView();
                    }
                }
            }
            else
            {
                if (ChartType == ChartType.Month)
                {
                    if (ChartDataType == ChartDataType.RM)
                    {

                        txtRange.Text = selectedHistoryData.ByMonth.Range;

                        // SETUP XAXIS

                        SetUpXAxis();

                        // SETUP YAXIS

                        SetUpYAxis();

                        // ADD DATA

                        SetData(selectedHistoryData.ByMonth.Months.Count);


                        // SETUP MARKER VIEW

                        SetUpMarkerRMView();

                    }
                    else
                    {

                        txtRange.Text = selectedHistoryData.ByMonth.Range;
                        // SETUP XAXIS

                        SetUpXAxiskWh();

                        // SETUP YAXIS

                        SetUpYAxisKwh();

                        // ADD DATA
                        SetKWhData(selectedHistoryData.ByMonth.Months.Count);

                        // SETUP MARKER VIEW

                        SetUpMarkerKWhView();
                    }
                }
            }

            int graphTopPadding = 30;
            int graphBottomPadding = 10;
            if (selectedAccount.AccountCategoryId.Equals("2"))
            {
                graphTopPadding = 40;
                mChart.LayoutParameters.Height = (int) DPUtils.ConvertDPToPx(240f);
            }
            mChart.SetExtraOffsets(0, graphTopPadding, 0, graphBottomPadding);

            mChart.SetOnChartValueSelectedListener(this);
        }
        #region SETUP AXIS RM
        internal void SetUpXAxis()
        {
            List<string> DayViewLabel = new List<string>();

            if (isSMAccount)
            {
                if (ChartType == ChartType.Month)
                {
                    XLabelsFormatter = new SMChartsMonthFormatter(selectedSMHistoryData.ByMonth, mChart);
                }
                else if (ChartType == ChartType.Day)
                {
                    if (!string.IsNullOrEmpty(selectedSMHistoryData.StarttDate))
                    {
                        DayViewLabel.Add(selectedSMHistoryData.StarttDate);
                    }
                    if (!string.IsNullOrEmpty(selectedSMHistoryData.MidDate))
                    {
                        DayViewLabel.Add(selectedSMHistoryData.MidDate);
                    }
                    if (!string.IsNullOrEmpty(selectedSMHistoryData.EndDate))
                    {
                        DayViewLabel.Add(selectedSMHistoryData.EndDate);
                    }
                    XLabelsFormatter = new SMChartsZoomInDayFormatter(DayViewLabel, mChart);
                }
            }
            else
            {
                XLabelsFormatter = new ChartsMonthFormatter(selectedHistoryData.ByMonth, mChart);
            }

            XAxis xAxis = mChart.XAxis;
            xAxis.Position = XAxisPosition.Bottom;
            xAxis.TextColor = Color.ParseColor("#ffffff");
            xAxis.AxisLineWidth = 2f;
            xAxis.AxisLineColor = Color.ParseColor("#4cffffff");

            xAxis.SetDrawGridLines(false);

            // Adding the MuseoSans300 to X Axis
            try
            {
                Typeface plain = Typeface.CreateFromAsset(Context.Assets, "fonts/" + TextViewUtils.MuseoSans300);
                xAxis.Typeface = plain;
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

            xAxis.Granularity = 1f; // only intervals of 1 day

            if (isSMAccount)
            {
                if (ChartType == ChartType.Month)
                {
                    xAxis.LabelCount = selectedSMHistoryData.ByMonth.Months.Count;
                }
                else if (ChartType == ChartType.Day)
                {
                    xAxis.LabelCount = DayViewLabel.Count;
                }
            }
            else
            {
                xAxis.LabelCount = selectedHistoryData.ByMonth.Months.Count;
            }
            xAxis.ValueFormatter = XLabelsFormatter;


        }
        #endregion

        #region SETUP AXIS KWH
        internal void SetUpXAxiskWh()
        {
            List<string> DayViewLabel = new List<string>();

            if (isSMAccount)
            {
                if (ChartType == ChartType.Month)
                {
                    XLabelsFormatter = new SMChartsMonthFormatter(selectedSMHistoryData.ByMonth, mChart);
                }
                else if (ChartType == ChartType.Day)
                {
                    if (!string.IsNullOrEmpty(selectedSMHistoryData.StarttDate))
                    {
                        DayViewLabel.Add(selectedSMHistoryData.StarttDate);
                    }
                    if (!string.IsNullOrEmpty(selectedSMHistoryData.MidDate))
                    {
                        DayViewLabel.Add(selectedSMHistoryData.MidDate);
                    }
                    if (!string.IsNullOrEmpty(selectedSMHistoryData.EndDate))
                    {
                        DayViewLabel.Add(selectedSMHistoryData.EndDate);
                    }
                    XLabelsFormatter = new SMChartsZoomInDayFormatter(DayViewLabel, mChart);
                }
            }
            else
            {
                XLabelsFormatter = new ChartsKWhFormatter(selectedHistoryData.ByMonth, mChart);
            }

            XAxis xAxis = mChart.XAxis;
            xAxis.Position = XAxisPosition.Bottom;
            xAxis.TextColor = Color.ParseColor("#ffffff");
            xAxis.AxisLineWidth = 2f;
            xAxis.AxisLineColor = Color.ParseColor("#4cffffff");

            xAxis.SetDrawGridLines(false);

            // Lin Siong Note: Adding the MuseoSans300 to X Axis
            try
            {
                Typeface plain = Typeface.CreateFromAsset(Context.Assets, "fonts/" + TextViewUtils.MuseoSans300);
                xAxis.Typeface = plain;
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

            xAxis.Granularity = 1f; // only intervals of 1 day
            if (isSMAccount)
            {
                if (ChartType == ChartType.Month)
                {
                    xAxis.LabelCount = selectedSMHistoryData.ByMonth.Months.Count;
                }
                else if (ChartType == ChartType.Day)
                {
                    xAxis.LabelCount = DayViewLabel.Count;
                }
            }
            else
            {
                xAxis.LabelCount = selectedHistoryData.ByMonth.Months.Count;
            }
            xAxis.ValueFormatter = XLabelsFormatter;


        }
        #endregion

        #region SETUP Y AXIS RM
        internal void SetUpYAxis()
        {
            IAxisValueFormatter custom = new MyAxisValueFormatter();

            float maxVal = GetMaxRMValues();
            float lowestPossibleSpace = (5f / 100f) * -maxVal;

            YAxis leftAxis = mChart.AxisLeft;
            leftAxis.Enabled = false;
            leftAxis.SetPosition(YAxisLabelPosition.OutsideChart);
            leftAxis.SetDrawGridLines(false);
            leftAxis.SpaceTop = 10f;
            leftAxis.SpaceBottom = 10f;
            leftAxis.AxisMinimum = lowestPossibleSpace;
            // Lin Siong Note: the 1.5f is important as it give the spacing for marker to render
            leftAxis.AxisMaximum = maxVal + 1.5f;

            YAxis rightAxis = mChart.AxisRight;
            rightAxis.Enabled = false;
            rightAxis.SetDrawGridLines(false);
            rightAxis.SpaceTop = 10f;
            rightAxis.SpaceBottom = 10f;
            rightAxis.AxisMinimum = lowestPossibleSpace;
            // Lin Siong Note: the 1.5f is important as it give the spacing for marker to render
            rightAxis.AxisMaximum = maxVal + 1.5f;

        }
        #endregion

        #region SETUP Y AXIS KWH
        internal void SetUpYAxisKwh()
        {
            IAxisValueFormatter custom = new MyAxisValueFormatter();
            float maxVal = GetMaxKWhValues();
            float lowestPossibleSpace = (5f / 100f) * -maxVal;

            YAxis leftAxis = mChart.AxisLeft;
            leftAxis.Enabled = false;
            leftAxis.SetPosition(YAxisLabelPosition.OutsideChart);
            leftAxis.SetDrawGridLines(false);
            leftAxis.SpaceTop = 10f;
            leftAxis.SpaceBottom = 10f;
            leftAxis.AxisMinimum = lowestPossibleSpace;
            leftAxis.AxisMaximum = maxVal + 1.5f;

            YAxis rightAxis = mChart.AxisRight;
            rightAxis.Enabled = false;
            rightAxis.SetDrawGridLines(false);
            rightAxis.SpaceTop = 10f;
            rightAxis.SpaceBottom = 10f;
            rightAxis.AxisMinimum = lowestPossibleSpace;
            rightAxis.AxisMaximum = maxVal + 1.5f;

        }
        #endregion

        #region SETUP MARKERVIEW RM / HIGHLIGHT TEXT
        internal void SetUpMarkerRMView()
        {
            if (isSMAccount)
            {
                SMSelectedMarkerView markerView = new SMSelectedMarkerView(Activity)
                {
                    UsageHistoryData = selectedSMHistoryData,
                    ChartType = ChartType,
                    ChartDataType = ChartDataType,
                    AccountType = selectedAccount.AccountCategoryId
                };

                markerView.ChartView = mChart;
                mChart.Marker = markerView;
            }
            else
            {
                SelectedMarkerView markerView = new SelectedMarkerView(Activity)
                {
                    UsageHistoryData = selectedHistoryData,
                    ChartType = ChartType,
                    ChartDataType = ChartDataType,
                    AccountType = selectedAccount.AccountCategoryId
                };

                markerView.ChartView = mChart;
                mChart.Marker = markerView;
            }
        }
        #endregion

        #region SETUP MARKERVIEW KWH/ HIGHLIGHT TEXT
        internal void SetUpMarkerKWhView()
        {
            if (isSMAccount)
            {
                SMSelectedMarkerView markerView = new SMSelectedMarkerView(Activity)
                {
                    UsageHistoryData = selectedSMHistoryData,
                    ChartType = ChartType,
                    ChartDataType = ChartDataType,
                    AccountType = selectedAccount.AccountCategoryId
                };

                markerView.ChartView = mChart;
                mChart.Marker = markerView;
            }
            else
            {
                SelectedMarkerView markerView = new SelectedMarkerView(Activity)
                {
                    UsageHistoryData = selectedHistoryData,
                    ChartType = ChartType,
                    ChartDataType = ChartDataType,
                    AccountType = selectedAccount.AccountCategoryId
                };
                markerView.ChartView = mChart;
                mChart.Marker = markerView;
            }
        }
        #endregion

        #region SETUP RM DATA
        internal void SetData(int barLength)
        {
            if (!isSMAccount)
            {
                // Lin Siong Note: the tariff data entry handling
                // Lin Siong Note: one row will contain stacked of data
                if (isToggleTariff)
                {
                    int stackIndex = 0;
                    List<BarEntry> yVals1 = new List<BarEntry>();
                    for (int i = 0; i < barLength; i++)
                    {
                        if (selectedHistoryData.ByMonth.Months[i].TariffBlocksList != null && selectedHistoryData.ByMonth.Months[i].TariffBlocksList.Count > 0)
                        {
                            float[] valList = new float[selectedHistoryData.ByMonth.Months[i].TariffBlocksList.Count];
                            for (int j = 0; j < selectedHistoryData.ByMonth.Months[i].TariffBlocksList.Count; j++)
                            {
                                float val = (float)selectedHistoryData.ByMonth.Months[i].TariffBlocksList[j].Amount;
                                if (float.IsPositiveInfinity(val))
                                {
                                    val = float.PositiveInfinity;
                                }
                                valList[j] = System.Math.Abs(val);
                            }
                            if (i == barLength - 1)
                            {
                                stackIndex = valList.Length - 1;
                            }
                            yVals1.Add(new BarEntry(i, valList));
                        }
                        else
                        {
                            float[] valList = new float[1];
                            valList[0] = 0f;
                            yVals1.Add(new BarEntry(i, valList));
                        }
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

                        // Lin Siong Note: the tariff data entrycolor  handling
                        // Lin Siong Note: it will track on which block it will use which color
                        // Lin Siong Note: then fill the color inside the array
                        // Lin Siong Note: then set the color array to chart
                        List<int> listOfColor = new List<int>();

                        for (int i = 0; i < barLength; i++)
                        {
                            if (selectedHistoryData.ByMonth.Months[i].TariffBlocksList != null && selectedHistoryData.ByMonth.Months[i].TariffBlocksList.Count > 0)
                            {
                                for (int j = 0; j < selectedHistoryData.ByMonth.Months[i].TariffBlocksList.Count; j++)
                                {
                                    if (selectedHistoryData.TariffBlocksLegend != null && selectedHistoryData.TariffBlocksLegend.Count > 0)
                                    {
                                        bool isFound = false;
                                        for (int k = 0; k < selectedHistoryData.TariffBlocksLegend.Count; k++)
                                        {
                                            if (selectedHistoryData.ByMonth.Months[i].TariffBlocksList[j].BlockId == selectedHistoryData.TariffBlocksLegend[k].BlockId)
                                            {
                                                isFound = true;
                                                listOfColor.Add(Color.Argb(50, selectedHistoryData.TariffBlocksLegend[k].Color.RedColor, selectedHistoryData.TariffBlocksLegend[k].Color.GreenColor, selectedHistoryData.TariffBlocksLegend[k].Color.BlueData));
                                                break;
                                            }
                                        }

                                        if (!isFound)
                                        {
                                            listOfColor.Add(Color.Argb(50, 255, 255, 255));
                                        }

                                    }
                                    else
                                    {
                                        listOfColor.Add(Color.Argb(50, 255, 255, 255));
                                    }
                                }
                            }
                            else
                            {
                                listOfColor.Add(Color.Argb(50, 255, 255, 255));
                            }
                        }


                        int[] colorSet = new int[listOfColor.Count];
                        for (int z = 0; z < listOfColor.Count; z++)
                        {
                            colorSet[z] = listOfColor[z];
                        }

                        set1.SetColors(colorSet);

                        List<IBarDataSet> dataSets = new List<IBarDataSet>();
                        dataSets.Add(set1);


                        BarData data = new BarData(dataSets);

                        data.BarWidth = 0.25f;

                        set1.HighLightAlpha = 0;

                        data.HighlightEnabled = true;
                        data.SetValueTextSize(10f);
                        data.SetDrawValues(false);

                        mChart.Data = data;
                    }

                    // HIGHLIGHT RIGHT MOST ITEM
                    CurrentParentIndex = barLength - 1;
                    Highlight rightMostBar = new Highlight(barLength - 1, 0, stackIndex);
                    mChart.HighlightValues(new Highlight[] { rightMostBar });
                }
                else
                {
                    List<BarEntry> yVals1 = new List<BarEntry>();
                    for (int i = 0; i < barLength; i++)
                    {
                        float val = (float)selectedHistoryData.ByMonth.Months[i].AmountTotal;
                        if (float.IsPositiveInfinity(val))
                        {
                            val = float.PositiveInfinity;
                        }

                        yVals1.Add(new BarEntry(i, System.Math.Abs(val)));
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
                        set1.HighLightAlpha = 255;

                        int[] color = { Color.Argb(100, 255, 255, 255) };
                        set1.SetColors(color);
                        List<IBarDataSet> dataSets = new List<IBarDataSet>();
                        dataSets.Add(set1);


                        BarData data = new BarData(dataSets);

                        data.BarWidth = 0.25f;

                        data.HighlightEnabled = true;
                        data.SetValueTextSize(10f);
                        data.SetDrawValues(false);

                        mChart.Data = data;
                    }

                    // HIGHLIGHT RIGHT MOST ITEM
                    CurrentParentIndex = barLength - 1;
                    Highlight rightMostBar = new Highlight(barLength - 1, 0, 0);
                    mChart.HighlightValues(new Highlight[] { rightMostBar });
                }
            }
            else
            {
                if (isToggleTariff)
                {
                    // Lin Siong Note: the tariff data entry handling
                    // Lin Siong Note: one row will contain stacked of data
                    int stackIndex = 0;
                    List<BarEntry> yVals1 = new List<BarEntry>();
                    for (int i = 0; i < barLength; i++)
                    {
                        if (ChartType == ChartType.Month)
                        {
                            if (selectedSMHistoryData.ByMonth.Months[i].TariffBlocksList != null && selectedSMHistoryData.ByMonth.Months[i].TariffBlocksList.Count > 0)
                            {
                                float[] valList = new float[selectedSMHistoryData.ByMonth.Months[i].TariffBlocksList.Count];
                                for (int j = 0; j < selectedSMHistoryData.ByMonth.Months[i].TariffBlocksList.Count; j++)
                                {
                                    float val = (float)selectedSMHistoryData.ByMonth.Months[i].TariffBlocksList[j].Amount;
                                    if (float.IsPositiveInfinity(val))
                                    {
                                        val = float.PositiveInfinity;
                                    }
                                    valList[j] = System.Math.Abs(val);
                                }
                                if (i == barLength - 1)
                                {
                                    stackIndex = valList.Length - 1;
                                }
                                yVals1.Add(new BarEntry(i, valList));
                            }
                            else
                            {
                                float[] valList = new float[1];
                                valList[0] = 0f;
                                yVals1.Add(new BarEntry(i, valList));
                            }
                        }
                        else if (ChartType == ChartType.Day)
                        {
                            if (selectedSMHistoryData != null && selectedSMHistoryData.ByDay != null && selectedSMHistoryData.ByDay.Count > 0)
                            {
                                foreach (SMUsageHistoryData.ByDayData DayData in selectedSMHistoryData.ByDay)
                                {
                                    if (DayData.Days != null && DayData.Days.Count > 0 && DayData.Days[i].TariffBlocksList != null && DayData.Days[i].TariffBlocksList.Count > 0)
                                    {
                                        float[] valList = new float[DayData.Days[i].TariffBlocksList.Count];
                                        for (int j = 0; j < DayData.Days[i].TariffBlocksList.Count; j++)
                                        {
                                            float val = (float)DayData.Days[i].TariffBlocksList[j].Amount;
                                            if (float.IsPositiveInfinity(val))
                                            {
                                                val = float.PositiveInfinity;
                                            }
                                            valList[j] = System.Math.Abs(val);
                                        }
                                        if (i == barLength - 1)
                                        {
                                            stackIndex = valList.Length - 1;
                                        }
                                        yVals1.Add(new BarEntry(i, valList));
                                    }
                                    else
                                    {
                                        float[] valList = new float[1];
                                        valList[0] = 0f;
                                        yVals1.Add(new BarEntry(i, valList));
                                    }
                                }
                            }
                            else
                            {
                                float[] valList = new float[1];
                                valList[0] = 0f;
                                yVals1.Add(new BarEntry(i, valList));
                            }
                        }
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

                        List<int> listOfColor = new List<int>();

                        // Lin Siong Note: the tariff data entrycolor  handling
                        // Lin Siong Note: it will track on which block it will use which color
                        // Lin Siong Note: then fill the color inside the array
                        // Lin Siong Note: then set the color array to chart

                        for (int i = 0; i < barLength; i++)
                        {
                            if (ChartType == ChartType.Month)
                            {
                                if (selectedSMHistoryData.ByMonth.Months[i].TariffBlocksList != null && selectedSMHistoryData.ByMonth.Months[i].TariffBlocksList.Count > 0)
                                {
                                    for (int j = 0; j < selectedSMHistoryData.ByMonth.Months[i].TariffBlocksList.Count; j++)
                                    {
                                        if (selectedSMHistoryData.TariffBlocksLegend != null && selectedSMHistoryData.TariffBlocksLegend.Count > 0)
                                        {
                                            bool isFound = false;
                                            for (int k = 0; k < selectedSMHistoryData.TariffBlocksLegend.Count; k++)
                                            {
                                                if (selectedSMHistoryData.ByMonth.Months[i].TariffBlocksList[j].BlockId == selectedSMHistoryData.TariffBlocksLegend[k].BlockId)
                                                {
                                                    isFound = true;
                                                    listOfColor.Add(Color.Argb(50, selectedSMHistoryData.TariffBlocksLegend[k].Color.RedColor, selectedSMHistoryData.TariffBlocksLegend[k].Color.GreenColor, selectedSMHistoryData.TariffBlocksLegend[k].Color.BlueData));
                                                    break;
                                                }
                                            }

                                            if (!isFound)
                                            {
                                                listOfColor.Add(Color.Argb(50, 255, 255, 255));
                                            }

                                        }
                                        else
                                        {
                                            listOfColor.Add(Color.Argb(50, 255, 255, 255));
                                        }
                                    }
                                }
                                else
                                {
                                    listOfColor.Add(Color.Argb(50, 255, 255, 255));
                                }
                            }
                            else if (ChartType == ChartType.Day)
                            {
                                if (selectedSMHistoryData != null && selectedSMHistoryData.ByDay != null && selectedSMHistoryData.ByDay.Count > 0)
                                {
                                    foreach (SMUsageHistoryData.ByDayData DayData in selectedSMHistoryData.ByDay)
                                    {
                                        if (DayData.Days != null && DayData.Days.Count > 0 && DayData.Days[i].TariffBlocksList != null && DayData.Days[i].TariffBlocksList.Count > 0)
                                        {
                                            for (int j = 0; j < DayData.Days[i].TariffBlocksList.Count; j++)
                                            {
                                                if (selectedSMHistoryData.TariffBlocksLegend != null && selectedSMHistoryData.TariffBlocksLegend.Count > 0)
                                                {
                                                    bool isFound = false;
                                                    for (int k = 0; k < selectedSMHistoryData.TariffBlocksLegend.Count; k++)
                                                    {
                                                        if (DayData.Days[i].TariffBlocksList[j].BlockId == selectedSMHistoryData.TariffBlocksLegend[k].BlockId)
                                                        {
                                                            isFound = true;
                                                            listOfColor.Add(Color.Argb(50, selectedSMHistoryData.TariffBlocksLegend[k].Color.RedColor, selectedSMHistoryData.TariffBlocksLegend[k].Color.GreenColor, selectedSMHistoryData.TariffBlocksLegend[k].Color.BlueData));
                                                            break;
                                                        }
                                                    }

                                                    if (!isFound)
                                                    {
                                                        listOfColor.Add(Color.Argb(50, 255, 255, 255));
                                                    }

                                                }
                                                else
                                                {
                                                    listOfColor.Add(Color.Argb(50, 255, 255, 255));
                                                }
                                            }
                                        }
                                        else
                                        {
                                            listOfColor.Add(Color.Argb(50, 255, 255, 255));
                                        }
                                    }
                                }
                                else
                                {
                                    listOfColor.Add(Color.Argb(50, 255, 255, 255));
                                }
                            }
                        }


                        int[] colorSet = new int[listOfColor.Count];
                        for (int z = 0; z < listOfColor.Count; z++)
                        {
                            colorSet[z] = listOfColor[z];
                        }

                        set1.SetColors(colorSet);

                        List<IBarDataSet> dataSets = new List<IBarDataSet>();
                        dataSets.Add(set1);


                        BarData data = new BarData(dataSets);

                        data.BarWidth = 0.25f;

                        set1.HighLightAlpha = 0;

                        data.HighlightEnabled = true;
                        data.SetValueTextSize(10f);
                        data.SetDrawValues(false);

                        mChart.Data = data;
                    }

                    // HIGHLIGHT RIGHT MOST ITEM
                    CurrentParentIndex = barLength - 1;
                    Highlight rightMostBar = new Highlight(barLength - 1, 0, stackIndex);
                    mChart.HighlightValues(new Highlight[] { rightMostBar });
                }
                else
                {
                    // Lin Siong Note: the normal data entry handling for smart meter
                    // Lin Siong Note: one row will contain stack of data, but only first is filleded
                    List<BarEntry> yVals1 = new List<BarEntry>();
                    for (int i = 0; i < barLength; i++)
                    {
                        if (ChartType == ChartType.Month)
                        {
                            float[] valList = new float[1];
                            float val = (float)selectedSMHistoryData.ByMonth.Months[i].AmountTotal;
                            if (float.IsPositiveInfinity(val))
                            {
                                val = float.PositiveInfinity;
                            }
                            valList[0] = System.Math.Abs(val);
                            yVals1.Add(new BarEntry(i, valList));
                        }
                        else if (ChartType == ChartType.Day)
                        {
                            float[] valList = new float[1];
                            float val = (float)DayViewRMData[i];
                            if (float.IsPositiveInfinity(val))
                            {
                                val = float.PositiveInfinity;
                            }
                            valList[0] = System.Math.Abs(val);
                            yVals1.Add(new BarEntry(i, valList));
                        }
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

                        List<int> listOfColor = new List<int>();

                        for (int i = 0; i < barLength; i++)
                        {
                            listOfColor.Add(Color.Argb(50, 255, 255, 255));
                        }

                        int[] colorSet = new int[listOfColor.Count];
                        for (int z = 0; z < listOfColor.Count; z++)
                        {
                            colorSet[z] = listOfColor[z];
                        }

                        set1.SetColors(colorSet);

                        List<IBarDataSet> dataSets = new List<IBarDataSet>();
                        dataSets.Add(set1);


                        BarData data = new BarData(dataSets);

                        data.BarWidth = 0.25f;

                        set1.HighLightAlpha = 0;

                        data.HighlightEnabled = true;
                        data.SetValueTextSize(10f);
                        data.SetDrawValues(false);

                        mChart.Data = data;
                    }

                    // HIGHLIGHT RIGHT MOST ITEM
                    CurrentParentIndex = barLength - 1;
                    Highlight rightMostBar = new Highlight(barLength - 1, 0, 0);
                    mChart.HighlightValues(new Highlight[] { rightMostBar });
                }
            }

        }
        #endregion

        #region SETUP KWH DATA
        internal void SetKWhData(int barLength)
        {
            if (!isSMAccount)
            {
                if (isToggleTariff)
                {
                    // Lin Siong Note: the tariff data entry handling
                    // Lin Siong Note: one row will contain stacked of data
                    int stackIndex = 0;
                    List<BarEntry> yVals1 = new List<BarEntry>();
                    for (int i = 0; i < barLength; i++)
                    {
                        if (selectedHistoryData.ByMonth.Months[i].TariffBlocksList != null && selectedHistoryData.ByMonth.Months[i].TariffBlocksList.Count > 0)
                        {
                            float[] valList = new float[selectedHistoryData.ByMonth.Months[i].TariffBlocksList.Count];
                            for (int j = 0; j < selectedHistoryData.ByMonth.Months[i].TariffBlocksList.Count; j++)
                            {
                                float val = (float)selectedHistoryData.ByMonth.Months[i].TariffBlocksList[j].Usage;
                                if (float.IsPositiveInfinity(val))
                                {
                                    val = float.PositiveInfinity;
                                }
                                valList[j] = System.Math.Abs(val);
                            }
                            if (i == barLength - 1)
                            {
                                stackIndex = valList.Length - 1;
                            }
                            yVals1.Add(new BarEntry(i, valList));
                        }
                        else
                        {
                            float[] valList = new float[1];
                            valList[0] = 0f;
                            yVals1.Add(new BarEntry(i, valList));
                        }
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

                        // Lin Siong Note: the tariff data entrycolor  handling
                        // Lin Siong Note: it will track on which block it will use which color
                        // Lin Siong Note: then fill the color inside the array
                        // Lin Siong Note: then set the color array to chart

                        List<int> listOfColor = new List<int>();

                        for (int i = 0; i < barLength; i++)
                        {
                            if (selectedHistoryData.ByMonth.Months[i].TariffBlocksList != null && selectedHistoryData.ByMonth.Months[i].TariffBlocksList.Count > 0)
                            {
                                for (int j = 0; j < selectedHistoryData.ByMonth.Months[i].TariffBlocksList.Count; j++)
                                {
                                    if (selectedHistoryData.TariffBlocksLegend != null && selectedHistoryData.TariffBlocksLegend.Count > 0)
                                    {
                                        bool isFound = false;
                                        for (int k = 0; k < selectedHistoryData.TariffBlocksLegend.Count; k++)
                                        {
                                            if (selectedHistoryData.ByMonth.Months[i].TariffBlocksList[j].BlockId == selectedHistoryData.TariffBlocksLegend[k].BlockId)
                                            {
                                                isFound = true;
                                                listOfColor.Add(Color.Argb(50, selectedHistoryData.TariffBlocksLegend[k].Color.RedColor, selectedHistoryData.TariffBlocksLegend[k].Color.GreenColor, selectedHistoryData.TariffBlocksLegend[k].Color.BlueData));
                                                break;
                                            }
                                        }

                                        if (!isFound)
                                        {
                                            listOfColor.Add(Color.Argb(50, 255, 255, 255));
                                        }

                                    }
                                    else
                                    {
                                        listOfColor.Add(Color.Argb(50, 255, 255, 255));
                                    }
                                }
                            }
                            else
                            {
                                listOfColor.Add(Color.Argb(50, 255, 255, 255));
                            }
                        }


                        int[] colorSet = new int[listOfColor.Count];
                        for (int z = 0; z < listOfColor.Count; z++)
                        {
                            colorSet[z] = listOfColor[z];
                        }

                        set1.SetColors(colorSet);

                        List<IBarDataSet> dataSets = new List<IBarDataSet>();
                        dataSets.Add(set1);


                        BarData data = new BarData(dataSets);

                        data.BarWidth = 0.25f;

                        set1.HighLightAlpha = 0;

                        data.HighlightEnabled = true;
                        data.SetValueTextSize(10f);
                        data.SetDrawValues(false);

                        mChart.Data = data;
                    }

                    // HIGHLIGHT RIGHT MOST ITEM
                    CurrentParentIndex = barLength - 1;
                    Highlight rightMostBar = new Highlight(barLength - 1, 0, stackIndex);
                    mChart.HighlightValues(new Highlight[] { rightMostBar });
                }
                else
                {

                    List<BarEntry> yVals1 = new List<BarEntry>();
                    for (int i = 0; i < barLength; i++)
                    {
                        float val = (float)selectedHistoryData.ByMonth.Months[i].UsageTotal;
                        if (float.IsPositiveInfinity(val))
                        {
                            val = float.PositiveInfinity;
                        }

                        yVals1.Add(new BarEntry(i, System.Math.Abs(val)));
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
                        set1.HighLightAlpha = 255;

                        int[] color = { Color.Argb(100, 255, 255, 255) };
                        set1.SetColors(color);
                        List<IBarDataSet> dataSets = new List<IBarDataSet>();
                        dataSets.Add(set1);


                        BarData data = new BarData(dataSets);

                        data.BarWidth = 0.25f;

                        data.HighlightEnabled = true;
                        data.SetValueTextSize(10f);
                        data.SetDrawValues(false);

                        mChart.Data = data;
                    }
                    // HIGHLIGHT RIGHT MOST ITEM
                    CurrentParentIndex = barLength - 1;
                    Highlight rightMostBar = new Highlight(barLength - 1, 0, 0);
                    mChart.HighlightValues(new Highlight[] { rightMostBar });
                }
            }
            else
            {
                if (isToggleTariff)
                {
                    // Lin Siong Note: the tariff data entry handling
                    // Lin Siong Note: one row will contain stacked of data
                    int stackIndex = 0;
                    List<BarEntry> yVals1 = new List<BarEntry>();
                    for (int i = 0; i < barLength; i++)
                    {
                        if (selectedSMHistoryData.ByMonth.Months[i].TariffBlocksList != null && selectedSMHistoryData.ByMonth.Months[i].TariffBlocksList.Count > 0)
                        {
                            float[] valList = new float[selectedSMHistoryData.ByMonth.Months[i].TariffBlocksList.Count];
                            for (int j = 0; j < selectedSMHistoryData.ByMonth.Months[i].TariffBlocksList.Count; j++)
                            {
                                float val = (float)selectedSMHistoryData.ByMonth.Months[i].TariffBlocksList[j].Usage;
                                if (float.IsPositiveInfinity(val))
                                {
                                    val = float.PositiveInfinity;
                                }
                                valList[j] = System.Math.Abs(val);
                            }
                            if (i == barLength - 1)
                            {
                                stackIndex = valList.Length - 1;
                            }
                            yVals1.Add(new BarEntry(i, valList));
                        }
                        else
                        {
                            float[] valList = new float[1];
                            valList[0] = 0f;
                            yVals1.Add(new BarEntry(i, valList));
                        }
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

                        // Lin Siong Note: the tariff data entrycolor  handling
                        // Lin Siong Note: it will track on which block it will use which color
                        // Lin Siong Note: then fill the color inside the array
                        // Lin Siong Note: then set the color array to chart

                        List<int> listOfColor = new List<int>();

                        for (int i = 0; i < barLength; i++)
                        {
                            if (selectedSMHistoryData.ByMonth.Months[i].TariffBlocksList != null && selectedSMHistoryData.ByMonth.Months[i].TariffBlocksList.Count > 0)
                            {
                                for (int j = 0; j < selectedSMHistoryData.ByMonth.Months[i].TariffBlocksList.Count; j++)
                                {
                                    if (selectedSMHistoryData.TariffBlocksLegend != null && selectedSMHistoryData.TariffBlocksLegend.Count > 0)
                                    {
                                        bool isFound = false;
                                        for (int k = 0; k < selectedSMHistoryData.TariffBlocksLegend.Count; k++)
                                        {
                                            if (selectedSMHistoryData.ByMonth.Months[i].TariffBlocksList[j].BlockId == selectedSMHistoryData.TariffBlocksLegend[k].BlockId)
                                            {
                                                isFound = true;
                                                listOfColor.Add(Color.Argb(50, selectedSMHistoryData.TariffBlocksLegend[k].Color.RedColor, selectedSMHistoryData.TariffBlocksLegend[k].Color.GreenColor, selectedSMHistoryData.TariffBlocksLegend[k].Color.BlueData));
                                                break;
                                            }
                                        }

                                        if (!isFound)
                                        {
                                            listOfColor.Add(Color.Argb(50, 255, 255, 255));
                                        }

                                    }
                                    else
                                    {
                                        listOfColor.Add(Color.Argb(50, 255, 255, 255));
                                    }
                                }
                            }
                            else
                            {
                                listOfColor.Add(Color.Argb(50, 255, 255, 255));
                            }
                        }


                        int[] colorSet = new int[listOfColor.Count];
                        for (int z = 0; z < listOfColor.Count; z++)
                        {
                            colorSet[z] = listOfColor[z];
                        }

                        set1.SetColors(colorSet);

                        List<IBarDataSet> dataSets = new List<IBarDataSet>();
                        dataSets.Add(set1);


                        BarData data = new BarData(dataSets);

                        data.BarWidth = 0.25f;

                        set1.HighLightAlpha = 0;

                        data.HighlightEnabled = true;
                        data.SetValueTextSize(10f);
                        data.SetDrawValues(false);

                        mChart.Data = data;
                    }

                    // HIGHLIGHT RIGHT MOST ITEM
                    CurrentParentIndex = barLength - 1;
                    Highlight rightMostBar = new Highlight(barLength - 1, 0, stackIndex);
                    mChart.HighlightValues(new Highlight[] { rightMostBar });
                }
                else
                {
                    // Lin Siong Note: the normal data entry handling for smart meter
                    // Lin Siong Note: one row will contain stack of data, but only first is filleded
                    List<BarEntry> yVals1 = new List<BarEntry>();
                    for (int i = 0; i < barLength; i++)
                    {
                        float[] valList = new float[1];
                        float val = (float)selectedSMHistoryData.ByMonth.Months[i].UsageTotal;
                        if (float.IsPositiveInfinity(val))
                        {
                            val = float.PositiveInfinity;
                        }
                        valList[0] = System.Math.Abs(val);
                        yVals1.Add(new BarEntry(i, valList));
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

                        List<int> listOfColor = new List<int>();

                        for (int i = 0; i < barLength; i++)
                        {
                            listOfColor.Add(Color.Argb(50, 255, 255, 255));
                        }

                        int[] colorSet = new int[listOfColor.Count];
                        for (int z = 0; z < listOfColor.Count; z++)
                        {
                            colorSet[z] = listOfColor[z];
                        }

                        set1.SetColors(colorSet);

                        List<IBarDataSet> dataSets = new List<IBarDataSet>();
                        dataSets.Add(set1);


                        BarData data = new BarData(dataSets);

                        data.BarWidth = 0.25f;

                        set1.HighLightAlpha = 0;

                        data.HighlightEnabled = true;
                        data.SetValueTextSize(10f);
                        data.SetDrawValues(false);

                        mChart.Data = data;
                    }

                    // HIGHLIGHT RIGHT MOST ITEM
                    CurrentParentIndex = barLength - 1;
                    Highlight rightMostBar = new Highlight(barLength - 1, 0, 0);
                    mChart.HighlightValues(new Highlight[] { rightMostBar });
                }
            }
        }
        #endregion

        // Lin Siong Note: Show by kWh graph
        public void ShowByKwh()
        {
            ChartDataType = ChartDataType.kWh;

            mChart.Visibility = ViewStates.Visible;

            rmKwhSelection.Enabled = true;
            tarifToggle.Enabled = true;

            mChart.Clear();
            SetUp();
        }

        // Lin Siong Note: Show by RM graph
        public void ShowByRM()
        {
            ChartDataType = ChartDataType.RM;

            mChart.Visibility = ViewStates.Visible;

            rmKwhSelection.Enabled = true;
            tarifToggle.Enabled = true;

            mChart.Clear();
            SetUp();
        }

        // Lin Siong Note: Show by Day graph
        public void ShowByDay()
        {
            ChartType = ChartType.Day;

            mChart.Visibility = ViewStates.Visible;

            rmKwhSelection.Enabled = true;
            tarifToggle.Enabled = true;

            mChart.Clear();
            SetUp();
        }

        // Lin Siong Note: Show by Month graph
        public void ShowByMonth()
        {
            ChartType = ChartType.Month;

            mChart.Visibility = ViewStates.Visible;

            rmKwhSelection.Enabled = true;
            tarifToggle.Enabled = true;

            mChart.Clear();
            SetUp();
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
                }
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
                this.activity = activity as DashboardHomeActivity;
            }
            catch (ClassCastException e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void ShowViewBill(BillHistoryV5 selectedBill)
        {
            btnViewBill.Clickable = false;
            btnReView.Clickable = false;
            btnViewBill.Enabled = false;
            btnReView.Enabled = false;
            Handler h = new Handler();
            Action myAction = () =>
            {
                btnViewBill.Clickable = true;
                btnReView.Clickable = true;
                btnViewBill.Enabled = true;
                btnReView.Enabled = true;
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

        [OnClick(Resource.Id.btnViewBill)]
        internal void OnViewBill(object sender, EventArgs e)
        {
            this.userActionsListener.OnViewBill(selectedAccount);
            try
            {
                FirebaseAnalyticsUtils.LogFragmentClickEvent(this, "View Bill Buttom Click");
            }
            catch (System.Exception ne)
            {
                Utility.LoggingNonFatalError(ne);
            }
        }

        [OnClick(Resource.Id.btnReView)]
        internal void OnREViewBill(object sender, EventArgs e)
        {
            this.userActionsListener.OnViewBill(selectedAccount);
            try
            {
                FirebaseAnalyticsUtils.LogFragmentClickEvent(this, "View Bill Buttom Click");
            }
            catch (System.Exception ne)
            {
                Utility.LoggingNonFatalError(ne);
            }
        }

        [OnClick(Resource.Id.btnRefresh)]
        internal void OnRefresh(object sender, EventArgs e)
        {
            this.userActionsListener.OnTapRefresh();
            try
            {
                FirebaseAnalyticsUtils.LogFragmentClickEvent(this, "Inner Dashboard Refresh Buttom Click");
            }
            catch (System.Exception ne)
            {
                Utility.LoggingNonFatalError(ne);
            }
        }

        [OnClick(Resource.Id.btnPay)]
        internal void OnUserPay(object sender, EventArgs e)
        {
            this.userActionsListener.OnPay();
            try
            {
                FirebaseAnalyticsUtils.LogFragmentClickEvent(this, "Inner Dashboard Payment Buttom Click");
            }
            catch (System.Exception ne)
            {
                Utility.LoggingNonFatalError(ne);
            }
        }

        // Lin Siong Note: Switch between RM or kWh Graph
        [OnClick(Resource.Id.rmKwhSelection)]
        internal void OnRMKwhToogleSelection(object sender, EventArgs e)
        {
            if (rmKwhSelectDropdown.Visibility == ViewStates.Gone)
            {
                rmKwhSelectDropdown.Visibility = ViewStates.Visible;
            }
            else
            {
                rmKwhSelectDropdown.Visibility = ViewStates.Gone;
            }
            try
            {
                FirebaseAnalyticsUtils.LogFragmentClickEvent(this, "RM / kWh Toggle Button Click");
            }
            catch (System.Exception ne)
            {
                Utility.LoggingNonFatalError(ne);
            }
        }

        // Lin Siong Note: Toggle the tariff block graph
        [OnClick(Resource.Id.tarifToggle)]
        internal void OnTariffToggled(object sender, EventArgs e)
        {
            try
            {
                if (isToggleTariff)
                {
                    imgTarifToggle.SetImageResource(Resource.Drawable.eye);
                    txtTarifToggle.Text = "Show Tariff";
                    isToggleTariff = false;
                    tariffBlockLegendRecyclerView.Visibility = ViewStates.Gone;
                    if (isChangeBackgroundNeeded)
                    {
                        if (isSMAccount)
                        {
                            scrollViewContent.SetBackgroundResource(Resource.Drawable.dashboard_chart_sm_bg);
                        }
                        else
                        {
                            scrollViewContent.SetBackgroundResource(Resource.Drawable.dashboard_chart_bg);
                        }
                    }
                    else
                    {
                        SetVirtualHeightParams(70f);
                    }
                }
                else
                {
                    imgTarifToggle.SetImageResource(Resource.Drawable.eye_hide);
                    txtTarifToggle.Text = "Hide Tariff";
                    isToggleTariff = true;
                    tariffBlockLegendRecyclerView.Visibility = ViewStates.Visible;
                    if (tariffBlockLegendAdapter == null)
                    {
                        if (!isSMAccount)
                        {
                            tariffBlockLegendAdapter = new TariffBlockLegendAdapter(selectedHistoryData.TariffBlocksLegend, this.Activity);
                            tariffBlockLegendRecyclerView.SetAdapter(tariffBlockLegendAdapter);
                        }
                        else
                        {
                            tariffBlockLegendAdapter = new TariffBlockLegendAdapter(selectedSMHistoryData.TariffBlocksLegend, this.Activity);
                            tariffBlockLegendRecyclerView.SetAdapter(tariffBlockLegendAdapter);
                        }
                    }
                    Context context = tariffBlockLegendRecyclerView.Context;
                    LayoutAnimationController controller =
                            AnimationUtils.LoadLayoutAnimation(context, Resource.Animation.layout_animation_fall_down);

                    tariffBlockLegendRecyclerView.LayoutAnimation = controller;
                    tariffBlockLegendRecyclerView.GetAdapter().NotifyDataSetChanged();
                    tariffBlockLegendRecyclerView.ScheduleLayoutAnimation();
                    if (isChangeBackgroundNeeded)
                    {
                        scrollViewContent.SetBackgroundResource(Resource.Drawable.dashboard_chart_extended_bg);
                    }
                    else
                    {
                        SetVirtualHeightParams(120f);
                    }
                }

                mChart.Clear();
                SetUp();
                FirebaseAnalyticsUtils.LogFragmentClickEvent(this, "Tariff Toggle Button Click");
            }
            catch (System.Exception ne)
            {
                Utility.LoggingNonFatalError(ne);
            }
        }

        /*[OnClick(Resource.Id.btnLearnMore)]
        internal void OnLearnMore(object sender, EventArgs e)
        {
            this.userActionsListener.OnLearnMore();
        }*/

        public void SetPresenter(DashboardChartContract.IUserActionsListener userActionListener)
        {
            this.userActionsListener = userActionListener;
        }

        public bool IsActive()
        {
            return IsVisible;
        }

        public void ShowProgress()
        {
            try
            {
                ((DashboardHomeActivity)Activity).ShowProgressDialog();
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void HideProgress()
        {
            try
            {
                ((DashboardHomeActivity)Activity).HideProgressDialog();
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
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
            else if (requestCode == SSMR_METER_HISTORY_ACTIVITY_CODE)
            {
                if (resultCode == Result.Ok)
                {
                    this.userActionsListener.OnTapRefresh();
                }
            }
        }

        public void ShowNoInternet(string contentTxt, string buttonTxt)
        {
            try
            {
                Activity.RunOnUiThread(() =>
                {
                    try
                    {
                        ((DashboardHomeActivity)Activity).SetToolbarBackground(Resource.Drawable.CustomGradientToolBar);
                        ((DashboardHomeActivity)Activity).SetStatusBarBackground(Resource.Drawable.bg_smr);
                        rootView.SetBackgroundColor(Resources.GetColor(Resource.Color.greyBackground));
                        scrollViewContent.SetBackgroundResource(Resource.Drawable.dasbord_chart_refresh_bg);


                        refreshLayout.Visibility = ViewStates.Visible;
                        allGraphLayout.Visibility = ViewStates.Gone;
                        smStatisticContainer.Visibility = ViewStates.Gone;
                        if (isBCRMDown)
                        {
                            refresh_image.SetImageResource(Resource.Drawable.maintenance_new);
                            SetMaintenanceLayoutParams();
                            btnNewRefresh.Visibility = ViewStates.Gone;

                        }
                        else
                        {
                            refresh_image.SetImageResource(Resource.Drawable.refresh_1);
                            SetRefreshLayoutParams();
                            StopAddressShimmer();
                            StopRangeShimmer();
                            StopGraphShimmer();
                            StopSMStatisticShimmer();
                            btnNewRefresh.Visibility = ViewStates.Visible;
                            energyTipsView.Visibility = ViewStates.Gone;
                            if (string.IsNullOrEmpty(buttonTxt))
                            {
                                btnNewRefresh.Text = txtBtnRefreshTitle;
                            }
                            else
                            {
                                btnNewRefresh.Text = buttonTxt;
                            }

                            if (Build.VERSION.SdkInt >= BuildVersionCodes.N)
                            {
                                if (string.IsNullOrEmpty(contentTxt))
                                {
                                    txtNewRefreshMessage.TextFormatted = Html.FromHtml(txtRefreshMsg, FromHtmlOptions.ModeLegacy);
                                }
                                else
                                {
                                    txtNewRefreshMessage.TextFormatted = Html.FromHtml(contentTxt, FromHtmlOptions.ModeLegacy);
                                }
                            }
                            else
                            {
                                if (string.IsNullOrEmpty(contentTxt))
                                {
                                    txtNewRefreshMessage.TextFormatted = Html.FromHtml(txtRefreshMsg);
                                }
                                else
                                {
                                    txtNewRefreshMessage.TextFormatted = Html.FromHtml(contentTxt);
                                }
                            }
                        }
                    }
                    catch (System.Exception e)
                    {
                        Utility.LoggingNonFatalError(e);
                    }
                });
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void ShowAmountDueNotAvailable()
        {
            txtDueDate.Text = "- -";
            reDueDate.Text = "- -";
            txtTotalPayable.Text = "- -";
            reTotalPayable.Text = "- -";
            DisablePayButton();
            DisableViewBillButton();
        }

        public void ShowTapRefresh()
        {
            if (Activity is DashboardHomeActivity)
            {
                var DashboardHomeActivity = Activity as DashboardHomeActivity;
                DashboardHomeActivity.OnTapRefresh();
            }

        }

        internal float GetMaxRMValues()
        {
            float val = 0;
            try
            {
                if(!isSMAccount)
                {
                    if (isToggleTariff)
                    {
                        foreach (UsageHistoryData.ByMonthData.MonthData MonthData in selectedHistoryData.ByMonth.Months)
                        {
                            float valTotal = 0;
                            for (int i = 0; i < MonthData.TariffBlocksList.Count; i++)
                            {
                                valTotal += System.Math.Abs((float)MonthData.TariffBlocksList[i].Amount);
                            }
                            if (System.Math.Abs(valTotal) > val)
                            {
                                val = System.Math.Abs((float)valTotal);
                            }
                        }
                        if (val == 0)
                        {
                            val = 1;
                        }
                    }
                    else
                    {
                        foreach (UsageHistoryData.ByMonthData.MonthData MonthData in selectedHistoryData.ByMonth.Months)
                        {
                            if (System.Math.Abs(MonthData.AmountTotal) > val)
                            {
                                val = System.Math.Abs((float)MonthData.AmountTotal);
                            }
                        }
                        if (val == 0)
                        {
                            val = 1;
                        }
                    }
                }
                else
                {
                    if (ChartType == ChartType.Month)
                    {
                        if (isToggleTariff)
                        {
                            foreach (SMUsageHistoryData.ByMonthData.MonthData MonthData in selectedSMHistoryData.ByMonth.Months)
                            {
                                float valTotal = 0;
                                for (int i = 0; i < MonthData.TariffBlocksList.Count; i++)
                                {
                                    valTotal += System.Math.Abs((float)MonthData.TariffBlocksList[i].Amount);
                                }
                                if (System.Math.Abs(valTotal) > val)
                                {
                                    val = System.Math.Abs((float)valTotal);
                                }
                            }
                            if (val == 0)
                            {
                                val = 1;
                            }
                        }
                        else
                        {
                            foreach (SMUsageHistoryData.ByMonthData.MonthData MonthData in selectedSMHistoryData.ByMonth.Months)
                            {
                                if (System.Math.Abs(MonthData.AmountTotal) > val)
                                {
                                    val = System.Math.Abs((float)MonthData.AmountTotal);
                                }
                            }
                            if (val == 0)
                            {
                                val = 1;
                            }
                        }
                    }
                    else if (ChartType == ChartType.Day)
                    {
                        if (isToggleTariff)
                        {
                            foreach (SMUsageHistoryData.ByDayData DayData in selectedSMHistoryData.ByDay)
                            {
                                foreach (SMUsageHistoryData.ByDayData.DayData IndividualDayData in DayData.Days)
                                {
                                    float valTotal = 0;
                                    for (int i = 0; i < IndividualDayData.TariffBlocksList.Count; i++)
                                    {
                                        valTotal += System.Math.Abs((float)IndividualDayData.TariffBlocksList[i].Amount);
                                    }
                                    if (System.Math.Abs(valTotal) > val)
                                    {
                                        val = System.Math.Abs((float)valTotal);
                                    }
                                }
                            }
                            if (val == 0)
                            {
                                val = 1;
                            }
                        }
                        else
                        {
                            foreach (SMUsageHistoryData.ByDayData DayData in selectedSMHistoryData.ByDay)
                            {
                                foreach (SMUsageHistoryData.ByDayData.DayData IndividualDayData in DayData.Days)
                                {
                                    if (System.Math.Abs(IndividualDayData.Amount) > val)
                                    {
                                        val = System.Math.Abs((float)IndividualDayData.Amount);
                                    }
                                }
                            }
                            if (val == 0)
                            {
                                val = 1;
                            }
                        }
                    }
                }
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
            return val;
        }

        internal float GetMaxKWhValues()
        {
            float val = 0;
            try
            {
                if (!isSMAccount)
                {
                    if (isToggleTariff)
                    {
                        foreach (UsageHistoryData.ByMonthData.MonthData MonthData in selectedHistoryData.ByMonth.Months)
                        {
                            float valTotal = 0;
                            for (int i = 0; i < MonthData.TariffBlocksList.Count; i++)
                            {
                                valTotal += System.Math.Abs((float)MonthData.TariffBlocksList[i].Usage);
                            }
                            if (System.Math.Abs(valTotal) > val)
                            {
                                val = System.Math.Abs((float)valTotal);
                            }
                        }
                        if (val == 0)
                        {
                            val = 1;
                        }
                    }
                    else
                    {
                        foreach (UsageHistoryData.ByMonthData.MonthData MonthData in selectedHistoryData.ByMonth.Months)
                        {
                            if (System.Math.Abs(MonthData.UsageTotal) > val)
                            {
                                val = System.Math.Abs((float)MonthData.UsageTotal);
                            }
                        }
                        if (val == 0)
                        {
                            val = 1;
                        }
                    }
                }
                else
                {
                    if (ChartType == ChartType.Month)
                    {
                        if (isToggleTariff)
                        {
                            foreach (SMUsageHistoryData.ByMonthData.MonthData MonthData in selectedSMHistoryData.ByMonth.Months)
                            {
                                float valTotal = 0;
                                for (int i = 0; i < MonthData.TariffBlocksList.Count; i++)
                                {
                                    valTotal += System.Math.Abs((float)MonthData.TariffBlocksList[i].Usage);
                                }
                                if (System.Math.Abs(valTotal) > val)
                                {
                                    val = System.Math.Abs((float)valTotal);
                                }
                            }
                            if (val == 0)
                            {
                                val = 1;
                            }
                        }
                        else
                        {
                            foreach (SMUsageHistoryData.ByMonthData.MonthData MonthData in selectedSMHistoryData.ByMonth.Months)
                            {
                                if (System.Math.Abs(MonthData.UsageTotal) > val)
                                {
                                    val = System.Math.Abs((float)MonthData.UsageTotal);
                                }
                            }
                            if (val == 0)
                            {
                                val = 1;
                            }
                        }
                    }
                    else if (ChartType == ChartType.Day)
                    {
                        if (isToggleTariff)
                        {
                            foreach (SMUsageHistoryData.ByDayData DayData in selectedSMHistoryData.ByDay)
                            {
                                foreach (SMUsageHistoryData.ByDayData.DayData IndividualDayData in DayData.Days)
                                {
                                    float valTotal = 0;
                                    for (int i = 0; i < IndividualDayData.TariffBlocksList.Count; i++)
                                    {
                                        valTotal += System.Math.Abs((float)IndividualDayData.TariffBlocksList[i].Usage);
                                    }
                                    if (System.Math.Abs(valTotal) > val)
                                    {
                                        val = System.Math.Abs((float)valTotal);
                                    }
                                }
                            }
                            if (val == 0)
                            {
                                val = 1;
                            }
                        }
                        else
                        {
                            foreach (SMUsageHistoryData.ByDayData DayData in selectedSMHistoryData.ByDay)
                            {
                                foreach (SMUsageHistoryData.ByDayData.DayData IndividualDayData in DayData.Days)
                                {
                                    if (System.Math.Abs(IndividualDayData.Consumption) > val)
                                    {
                                        val = System.Math.Abs((float)IndividualDayData.Consumption);
                                    }
                                }
                            }
                            if (val == 0)
                            {
                                val = 1;
                            }
                        }
                    }
                }
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
            return val;
        }

        public override void OnResume()
        {
            base.OnResume();

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

        private Snackbar mLoadBillSnackBar;
        public void ShowLoadBillRetryOptions()
        {
            try
            {
                if (mLoadBillSnackBar != null && mLoadBillSnackBar.IsShown)
                {
                    mLoadBillSnackBar.Dismiss();
                }

                mLoadBillSnackBar = Snackbar.Make(rootView, GetString(Resource.String.dashboard_chart_cancelled_exception_error), Snackbar.LengthIndefinite)
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

        public void ShowNotification()
        {
            Intent payment_activity = GetIntentObject(typeof(NotificationActivity));

            if (payment_activity != null && IsAdded)
            {
                StartActivity(payment_activity);
            }
        }

        public void ShowAmountDue(AccountDueAmount accountDueAmount)
        {
            try
            {
                accountDueAmountData = accountDueAmount;
                // txtWhyThisAmt.Text = string.IsNullOrEmpty(accountDueAmount.WhyThisAmountLink) ? Activity.GetString(Resource.String.why_this_amount) : accountDueAmount.WhyThisAmountLink;
                Date d = null;
                try
                {
                    d = dateParser.Parse(accountDueAmount.BillDueDate);
                }
                catch (ParseException e)
                {
                    Utility.LoggingNonFatalError(e);
                }

                try
                {
                    Activity.RunOnUiThread(() =>
                    {
                        try
                        {
                            StopAmountDueShimmer();
                            if (d != null)
                            {
                                if (selectedAccount != null)
                                {
                                    if (isPaymentDown)
                                    {
                                        DownTimeEntity bcrmEntity = DownTimeEntity.GetByCode(Constants.BCRM_SYSTEM);
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
                                    else
                                    {
                                        EnablePayButton();
                                    }

                                    if (this.userActionsListener.IsBillingAvailable())
                                    {
                                        EnableViewBillButton();
                                    }
                                    else
                                    {
                                        DisableViewBillButton();
                                    }

                                    txtTotalPayableCurrency.Visibility = ViewStates.Visible;
                                    btnViewBill.SetTextColor(ContextCompat.GetColorStateList(this.Activity, Resource.Color.freshGreen));
                                    btnViewBill.Background = ContextCompat.GetDrawable(this.Activity, Resource.Drawable.light_green_outline_button_background);

                                    if (selectedAccount.AccountCategoryId.Equals("2"))
                                    {
                                        re_img.Visibility = ViewStates.Visible;
                                        rePayableLayout.Visibility = ViewStates.Visible;
                                        // txtWhyThisAmt.Visibility = ViewStates.Gone;
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
                                        reTotalPayable.Text = decimalFormat.Format(calAmt);

                                        int incrementDays = int.Parse(accountDueAmount.IncrementREDueDateByDays == null ? "0" : accountDueAmount.IncrementREDueDateByDays);
                                        Constants.RE_ACCOUNT_DATE_INCREMENT_DAYS = incrementDays;
                                        Calendar c = Calendar.Instance;
                                        c.Time = d;
                                        c.Add(CalendarField.Date, incrementDays);
                                        SimpleDateFormat df = new SimpleDateFormat("dd MMM");
                                        Date newDate = c.Time;
                                        string dateString = df.Format(newDate);
                                        if (System.Math.Abs(calAmt) < 0.0001)
                                        {
                                            txtDueDate.Text = "- -";
                                            reDueDate.Text = "- -";
                                        }
                                        else
                                        {
                                            txtDueDate.Text = "I will get by " + GetString(Resource.String.dashboard_chartview_due_date_wildcard, dateFormatter.Format(newDate));
                                            reDueDate.Text = "I will get by " + dateString;
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
                                            totalPayableLayout.Visibility = ViewStates.Gone;
                                            noPayableLayout.Visibility = ViewStates.Visible;
                                            if (System.Math.Abs(calAmt) < 0.0001)
                                            {
                                                txtNoPayableTitle.Text = "I’ve cleared all bills";
                                                txtNoPayable.SetTextColor(Resources.GetColor(Resource.Color.charcoalGrey));
                                                txtNoPayableCurrency.SetTextColor(Resources.GetColor(Resource.Color.charcoalGrey));
                                            }
                                            else
                                            {
                                                txtNoPayableTitle.Text = "I’ve paid extra";
                                                txtNoPayable.SetTextColor(Resources.GetColor(Resource.Color.freshGreen));
                                                txtNoPayableCurrency.SetTextColor(Resources.GetColor(Resource.Color.freshGreen));
                                            }
                                            txtNoPayable.Text = decimalFormat.Format(System.Math.Abs(accountDueAmount.AmountDue));
                                            txtDueDate.Text = "- -";
                                        }
                                        else
                                        {
                                            totalPayableLayout.Visibility = ViewStates.Visible;
                                            noPayableLayout.Visibility = ViewStates.Gone;
                                            txtDueDate.Text = "by " + GetString(Resource.String.dashboard_chartview_due_date_wildcard, dateFormatter.Format(d));
                                        }
                                    }
                                }
                                else
                                {
                                    // txtWhyThisAmt.Visibility = ViewStates.Gone;
                                }
                            }
                            else
                            {
                                txtDueDate.Text = "- -";
                                reDueDate.Text = "- -";
                                txtTotalPayable.Text = "- -";
                                reTotalPayable.Text = "- -";
                                // txtWhyThisAmt.Visibility = ViewStates.Gone;
                            }
                        }
                        catch (System.Exception ne)
                        {
                            Utility.LoggingNonFatalError(ne);
                        }
                    });
                }
                catch (System.Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                }
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        private Snackbar mSmartMeterError;
        public void ShowUnableToFecthSmartMeterData(string errorMsg)
        {
            try
            {
                if (mSmartMeterError != null && mSmartMeterError.IsShown)
                {
                    mSmartMeterError.Dismiss();

                }

                mSmartMeterError = Snackbar.Make(rootView, errorMsg, 10000)
                .SetAction(GetString(Resource.String.logout_rate_unknown_exception_btn_close), delegate
                {
                    mSmartMeterError.Dismiss();
                }
                );
                mSmartMeterError.Show();
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        private Snackbar mDisconnectionSnackbar;
        public void ShowDisconnectionRetrySnakebar()
        {
            try
            {
                if (mDisconnectionSnackbar != null && mDisconnectionSnackbar.IsShown)
                {
                    mDisconnectionSnackbar.Dismiss();
                }

                mDisconnectionSnackbar = Snackbar.Make(rootView, GetString(Resource.String.dashboard_chart_cancelled_exception_error), Snackbar.LengthIndefinite)
                .SetAction(GetString(Resource.String.dashboard_chart_cancelled_exception_btn_retry), delegate
                {

                    mDisconnectionSnackbar.Dismiss();
                }
                );
                mDisconnectionSnackbar.Show();
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

        }

        private Snackbar mSMRSnackbar;
        public void ShowSMRRetrySnakebar()
        {
            try
            {
                if (mSMRSnackbar != null && mSMRSnackbar.IsShown)
                {
                    mSMRSnackbar.Dismiss();
                }

                mSMRSnackbar = Snackbar.Make(rootView, GetString(Resource.String.dashboard_chart_cancelled_exception_error), Snackbar.LengthIndefinite)
                .SetAction(GetString(Resource.String.dashboard_chart_cancelled_exception_btn_retry), delegate
                {

                    mSMRSnackbar.Dismiss();
                }
                );
                mSMRSnackbar.Show();
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

        public void EnableViewBillButton()
        {
            Activity.RunOnUiThread(() =>
            {
                btnViewBill.Enabled = true;
                btnReView.Enabled = true;
                btnViewBill.SetTextColor(ContextCompat.GetColorStateList(this.Activity, Resource.Color.freshGreen));
                btnReView.SetTextColor(ContextCompat.GetColorStateList(this.Activity, Resource.Color.freshGreen));
                btnViewBill.Background = ContextCompat.GetDrawable(this.Activity, Resource.Drawable.light_green_outline_button_background);
                btnReView.Background = ContextCompat.GetDrawable(this.Activity, Resource.Drawable.light_green_outline_button_background);
            });
        }

        public void DisableViewBillButton()
        {
            Activity.RunOnUiThread(() =>
            {
                btnViewBill.Enabled = false;
                btnReView.Enabled = false;
                btnViewBill.Background = ContextCompat.GetDrawable(this.Activity, Resource.Drawable.silver_chalice_button_outline);
                btnReView.Background = ContextCompat.GetDrawable(this.Activity, Resource.Drawable.silver_chalice_button_outline);
                btnViewBill.SetTextColor(ContextCompat.GetColorStateList(this.Activity, Resource.Color.silverChalice));
                btnReView.SetTextColor(ContextCompat.GetColorStateList(this.Activity, Resource.Color.silverChalice));
            });
        }

        public void ShowAmountDueFailed()
        {
            Activity.RunOnUiThread(() =>
            {
                StopAmountDueShimmer();
                re_img.Visibility = ViewStates.Visible;
                rePayableLayout.Visibility = ViewStates.Visible;
                totalPayableLayout.Visibility = ViewStates.Visible;
                txtDueDate.Text = "- -";
                reDueDate.Text = "- -";
                txtTotalPayable.Text = "- -";
                reTotalPayable.Text = "- -";
                DisablePayButton();
                btnViewBill.Enabled = false;
                btnReView.Enabled = false;
                btnViewBill.Background = ContextCompat.GetDrawable(this.Activity, Resource.Drawable.silver_chalice_button_outline);
                btnReView.Background = ContextCompat.GetDrawable(this.Activity, Resource.Drawable.silver_chalice_button_outline);
                btnViewBill.SetTextColor(ContextCompat.GetColorStateList(this.Activity, Resource.Color.silverChalice));
                btnReView.SetTextColor(ContextCompat.GetColorStateList(this.Activity, Resource.Color.silverChalice));
            });
        }

        protected override Android.App.Activity GetActivityObject()
        {
            return activity;
        }

        private void OnWhatIsThisAccountStatusTap(string dialogMessage, string btnLabelText)
        {
            TooltipGenerator tooltipGenerator = new TooltipGenerator(Activity);
            tooltipGenerator.Create(dialogMessage);
            tooltipGenerator.AddAction(btnLabelText);
            tooltipGenerator.Show();
        }

        public void ShowAccountStatus(AccountStatusData accountStatusData)
        {
            if (accountStatusData != null)
            {
                if (accountStatusData.DisconnectionStatus != "Available")
                {
                    energyDisconnectionButton.Visibility = ViewStates.Visible;
                    string accountStatusMessage = accountStatusData?.AccountStatusMessage ?? "Your electricity is currently disconnected.";
                    string whatDoesThisMeanLabel = accountStatusData?.AccountStatusModalTitle ?? "What does this mean?";
                    string whatDoesThisToolTipMessage = accountStatusData?.AccountStatusModalMessage ?? "<strong>What does this mean?</strong><br/><br/>Your electricity has been disconnected and is unavailable. This was not caused by a power outage.<br/><br/>If you’ve made a payment, please give us some time for this to be reflected.";
                    string whatDoesThisToolTipBtnLabel = accountStatusData?.AccountStatusModalBtnText ?? "Got It!";
                    if (Android.OS.Build.VERSION.SdkInt >= Android.OS.Build.VERSION_CODES.N)
                    {
                        txtEnergyDisconnection.TextFormatted = Html.FromHtml(accountStatusMessage, FromHtmlOptions.ModeLegacy);
                    }
                    else
                    {
                        txtEnergyDisconnection.TextFormatted = Html.FromHtml(accountStatusMessage);
                    }
                    txtEnergyDisconnection.Click += delegate
                    {
                        OnWhatIsThisAccountStatusTap(whatDoesThisToolTipMessage, whatDoesThisToolTipBtnLabel);
                    };
                }
                else
                {
                    energyDisconnectionButton.Visibility = ViewStates.Gone;
                }
            }
            else
            {
                energyDisconnectionButton.Visibility = ViewStates.Gone;
            }
        }


        // Lin Siong Note: SSMR Dashboard View Setup
        public void ShowSSMRDashboardView(SMRActivityInfoResponse response)
        {
            try
            {
                if (response != null && response.Response != null && response.Response.Data != null)
                {
                    smrResponse = response;
                    MyTNBAccountManagement.GetInstance().SetAccountActivityInfo(new SMRAccountActivityInfo(selectedAccount.AccountNum, smrResponse));
                    SMRPopUpUtils.OnSetSMRActivityInfoResponse(response);
                    MyTNBAppToolTipData.SetSMRActivityInfo(response.Response);
                    Activity.RunOnUiThread(() =>
                    {
                        ssmrHistoryContainer.Visibility = ViewStates.Visible;

                        if (response.Response.Data.DashboardCTAType.ToUpper() == Constants.SMR_SUBMIT_METER_KEY)
                        {
                            isSubmitMeter = true;
                            if (response.Response.Data.showReadingHistoryLink == "true")
                            {
                                btnTxtSsmrViewHistory.Visibility = ViewStates.Visible;
                                if (!string.IsNullOrEmpty(response.Response.Data.ReadingHistoryLinkText))
                                {
                                    btnTxtSsmrViewHistory.Text = response.Response.Data.ReadingHistoryLinkText;
                                }
                            }
                            else
                            {
                                btnTxtSsmrViewHistory.Visibility = ViewStates.Gone;
                            }
                        }
                        else
                        {
                            isSubmitMeter = false;
                        }

                        if (!string.IsNullOrEmpty(response.Response.Data.DashboardMessage))
                        {
                            if (Android.OS.Build.VERSION.SdkInt >= Android.OS.Build.VERSION_CODES.N)
                            {
                                ssmrAccountStatusText.TextFormatted = Html.FromHtml(response.Response.Data.DashboardMessage, FromHtmlOptions.ModeLegacy);
                            }
                            else
                            {
                                ssmrAccountStatusText.TextFormatted = Html.FromHtml(response.Response.Data.DashboardMessage);
                            }
                        }

                        if (response.Response.Data.isDashboardCTADisabled == "true")
                        {
                            btnReadingHistory.Enabled = false;
                            btnReadingHistory.Background = ContextCompat.GetDrawable(this.Activity, Resource.Drawable.silver_chalice_button_outline);
                            btnReadingHistory.SetTextColor(ContextCompat.GetColorStateList(this.Activity, Resource.Color.silverChalice));
                        }
                        else
                        {
                            btnReadingHistory.Enabled = true;
                            btnReadingHistory.SetTextColor(ContextCompat.GetColorStateList(this.Activity, Resource.Color.freshGreen));
                            btnReadingHistory.Background = ContextCompat.GetDrawable(this.Activity, Resource.Drawable.light_green_outline_button_background);
                        }

                        if (!string.IsNullOrEmpty(response.Response.Data.DashboardCTAText))
                        {
                            btnReadingHistory.Text = response.Response.Data.DashboardCTAText;
                        }
                        else
                        {
                            if (response.Response.Data.DashboardCTAType.ToUpper() == Constants.SMR_SUBMIT_METER_KEY)
                            {
                                btnReadingHistory.Text = this.Activity.GetString(Resource.String.ssmr_submit_meter);
                            }
                            else
                            {
                                btnReadingHistory.Text = this.Activity.GetString(Resource.String.ssmr_view_meter);
                            }
                        }
                    });

                }
                else
                {
                    HideSSMRDashboardView();
                }
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void HideSSMRDashboardView()
        {
            ssmrHistoryContainer.Visibility = ViewStates.Gone;
        }

        public string GetDeviceId()
        {
            return this.DeviceId();
        }

        void NMRESMDashboardScrollViewListener.OnScrollChanged(NMRESMDashboardScrollView v, int l, int t, int oldl, int oldt)
        {
            try
            {
                View view = (View)scrollView.GetChildAt(scrollView.ChildCount - 1);
                int scrollPosition = t - oldt;
                // if diff is zero, then the bottom has been reached
                if (!isREAccount)
                {
                    if (scrollPosition > 0)
                    {
                        requireScroll = true;
                        bottomSheetBehavior.State = BottomSheetBehavior.StateHidden;
                    }
                    else if (scrollPosition < 0)
                    {
                        bottomSheetBehavior.State = BottomSheetBehavior.StateExpanded;
                    }

                    if (t == 0)
                    {
                        requireScroll = false;
                    }
                }
            }
            catch (System.Exception ne)
            {
                Utility.LoggingNonFatalError(ne);
            }
        }

        private class DashboardBottomSheetCallBack : BottomSheetBehavior.BottomSheetCallback
        {
            public override void OnSlide(View bottomSheet, float slideOffset)
            {

            }

            public override void OnStateChanged(View bottomSheet, int newState)
            {
                try
                {
                    if (isREAccount)
                    {
                        bottomSheetBehavior.State = BottomSheetBehavior.StateHidden;
                    }
                    else if (!requireScroll)
                    {
                        if (newState == BottomSheetBehavior.StateHidden)
                        {
                            bottomSheetBehavior.State = BottomSheetBehavior.StateExpanded;
                        }
                    }
                }
                catch (System.Exception ne)
                {
                    Utility.LoggingNonFatalError(ne);
                }
            }
        }

        [OnClick(Resource.Id.kwhLabel)]
        internal void OnUserClickKwh(object sender, EventArgs e)
        {
            try
            {
                rmKwhSelectDropdown.Visibility = ViewStates.Gone;
                rmKwhLabel.Text = "kWh";
                kwhLabel.SetTextColor(Resources.GetColor(Resource.Color.powerBlue));
                rmLabel.SetTextColor(Resources.GetColor(Resource.Color.new_grey));
                ShowByKwh();
                FirebaseAnalyticsUtils.LogFragmentClickEvent(this, "kWh Selection Click");
            }
            catch (System.Exception ne)
            {
                Utility.LoggingNonFatalError(ne);
            }

        }

        [OnClick(Resource.Id.rmLabel)]
        internal void OnUserClickRM(object sender, EventArgs e)
        {
            try
            {
                rmKwhSelectDropdown.Visibility = ViewStates.Gone;
                rmKwhLabel.Text = "RM  ";
                rmLabel.SetTextColor(Resources.GetColor(Resource.Color.powerBlue));
                kwhLabel.SetTextColor(Resources.GetColor(Resource.Color.new_grey));
                ShowByRM();
                FirebaseAnalyticsUtils.LogFragmentClickEvent(this, "RM Selection Click");

            }
            catch (System.Exception ne)
            {
                Utility.LoggingNonFatalError(ne);
            }

        }

        public void CheckRMKwhSelectDropDown()
        {
            if (rmKwhSelectDropdown.Visibility == ViewStates.Visible)
            {
                rmKwhSelectDropdown.Visibility = ViewStates.Gone;
            }
        }

        private void OnGetEnergyTipsItems()
        {
            try
            {
                energyTipsView.Visibility = ViewStates.Gone;
                energyTipsShimmerView.Visibility = ViewStates.Visible;
                OnSetEnergyTipsShimmerAdapter(this.mPresenter.OnLoadEnergySavingTipsShimmerList(3));
                List<EnergySavingTipsModel> localList = EnergyTipsUtils.GetAllItems();
                if (localList != null && localList.Count >= 3)
                {
                    _ = this.mPresenter.OnRandomizeEnergyTipsView(localList);
                }
                else
                {
                    this.mPresenter.OnGetEnergySavingTips();
                }
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void HideEnergyTipsShimmerView()
        {
            Activity.RunOnUiThread(() =>
            {
                try
                {
                    energyTipsShimmerView.Visibility = ViewStates.Gone;
                    OnSetEnergyTipsShimmerAdapter(null);
                }
                catch (System.Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                }
            });
        }

        private void OnSetEnergyTipsShimmerAdapter(List<EnergySavingTipsModel> list)
        {
            energyTipsShimmerAdapter = new EnergySavingTipsShimmerAdapter(list, this.Activity);
            energyTipsShimmerList.SetAdapter(energyTipsShimmerAdapter);
        }

        public void ShowEnergyTipsView(List<EnergySavingTipsModel> list)
        {
            Activity.RunOnUiThread(() =>
            {
                try
                {
                    energyTipsView.Visibility = ViewStates.Visible;
                    energyTipsAdapter = new EnergySavingTipsAdapter(list, this.Activity);
                    energyTipsList.SetAdapter(energyTipsAdapter);
                }
                catch (System.Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                }
            });
            HideEnergyTipsShimmerView();
        }

        void ViewTreeObserver.IOnGlobalLayoutListener.OnGlobalLayout()
        {
            try
            {
                scrollView.setOnScrollViewListener(this);
                scrollView.OverScrollMode = OverScrollMode.Always;

                int childHeight = scrollViewContent.Height;
                isScrollIndicatorShowNeed = scrollView.Height < (childHeight + scrollView.PaddingTop + scrollView.PaddingBottom);
                if (isScrollIndicatorShowNeed)
                {
                    shadowLayout.SetBackgroundResource(Resource.Drawable.scroll_indicator);
                }
                else
                {
                    shadowLayout.SetBackgroundResource(Resource.Drawable.scroll_shadow);
                }
                bottomSheet.RequestLayout();
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public TextView GetkwhLabel()
        {
            return kwhLabel;
        }

        public TextView GetRmLabel()
        {
            return rmLabel;
        }

        public LinearLayout GetRmKwhSelection()
        {
            return rmKwhSelection;
        }

        void IOnChartValueSelectedListenerSupport.OnNothingSelected()
        {
            CurrentParentIndex = -1;
            try
            {
                FirebaseAnalyticsUtils.LogFragmentClickEvent(this, "Graph Hightlight Deselected");
            }
            catch (System.Exception err)
            {
                Utility.LoggingNonFatalError(err);
            }
        }

        // Lin Siong Note: Handle the chart select event
        // Lin Siong Note: Will have vibration effect when selected
        // Lin Siong Note: if isToggleTariff = true then it will force the entry to be hightlighted to most upper one
        // Lin Siong TODO: Select last bar trigger to day view
        void IOnChartValueSelectedListenerSupport.OnValueSelected(Entry e, Highlight h)
        {
            try
            {
                if (!isSMAccount)
                {
                    if (isToggleTariff && h != null)
                    {
                        int stackedIndex = 0;
                        if (selectedHistoryData.ByMonth.Months[(int)e.GetX()].TariffBlocksList != null && selectedHistoryData.ByMonth.Months[(int)e.GetX()].TariffBlocksList.Count > 0)
                        {
                            stackedIndex = selectedHistoryData.ByMonth.Months[(int)e.GetX()].TariffBlocksList.Count - 1;
                        }

                        Highlight stackedHigh = new Highlight((int)e.GetX(), 0, stackedIndex);
                        mChart.HighlightValue(stackedHigh, false);
                    }
                }
                else
                {
                    if (isToggleTariff && h != null)
                    {
                        int stackedIndex = 0;
                        if (selectedSMHistoryData.ByMonth.Months[(int)e.GetX()].TariffBlocksList != null && selectedSMHistoryData.ByMonth.Months[(int)e.GetX()].TariffBlocksList.Count > 0)
                        {
                            stackedIndex = selectedSMHistoryData.ByMonth.Months[(int)e.GetX()].TariffBlocksList.Count - 1;
                        }

                        Highlight stackedHigh = new Highlight((int)e.GetX(), 0, stackedIndex);
                        mChart.HighlightValue(stackedHigh, false);
                    }
                }
            }
            catch (System.Exception err)
            {
                Utility.LoggingNonFatalError(err);
            }

            try
            {
                if (e != null)
                {
                    int index = (int)e.GetX();
                    if (index != CurrentParentIndex)
                    {
                        CurrentParentIndex = index;
                        Vibrator vibrator = (Vibrator)this.Activity.GetSystemService(Context.VibratorService);
                        if (Android.OS.Build.VERSION.SdkInt >= Android.OS.Build.VERSION_CODES.O)
                        {
                            vibrator.Vibrate(VibrationEffect.CreateOneShot(200, 12));

                        }
                        else
                        {
                            vibrator.Vibrate(200);

                        }
                    }
                }
                else
                {
                    CurrentParentIndex = -1;
                }

                try
                {
                    FirebaseAnalyticsUtils.LogFragmentClickEvent(this, "Graph Hightlight Selected");
                }
                catch (System.Exception err)
                {
                    Utility.LoggingNonFatalError(err);
                }
            }
            catch (System.Exception err)
            {
                Utility.LoggingNonFatalError(err);
            }
        }

        public bool isSMDataError()
        {
            return !string.IsNullOrEmpty(errorMSG);
        }

        public bool IsBCRMDownFlag()
        {
            return isBCRMDown;
        }

        public bool IsLoadUsageNeeded()
        {
            return isUsageLoadedNeeded;
        }

        public AccountData GetSelectedAccount()
        {
            return selectedAccount;
        }

        public void SetUsageData(UsageHistoryData data)
        {
            if (data != null)
            {
                isUsageLoadedNeeded = false;
                selectedHistoryData = data;
            }
        }

        public void SetSMUsageData(SMUsageHistoryData data)
        {
            if (data != null)
            {
                isUsageLoadedNeeded = false;
                selectedSMHistoryData = data;
            }
        }

        public void StartAddressShimmer()
        {
            try
            {
                var shimmerBuilder = ShimmerUtils.InvertedShimmerBuilderConfig();
                if (shimmerBuilder != null)
                {
                    shimmrtTxtAddress1.SetShimmer(shimmerBuilder?.Build());
                    shimmrtTxtAddress2.SetShimmer(shimmerBuilder?.Build());
                }
                shimmrtTxtAddress1.StartShimmer();
                shimmrtTxtAddress2.StartShimmer();
                shimmrtAddressView.Visibility = ViewStates.Visible;
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void StopAddressShimmer()
        {
            try
            {
                shimmrtAddressView.Visibility = ViewStates.Gone;
                if (shimmrtTxtAddress1.IsShimmerStarted)
                {
                    shimmrtTxtAddress1.StopShimmer();
                }
                if (shimmrtTxtAddress2.IsShimmerStarted)
                {
                    shimmrtTxtAddress2.StopShimmer();
                }
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void StartRangeShimmer()
        {
            try
            {
                var shimmerBuilder = ShimmerUtils.InvertedShimmerBuilderConfig();
                if (shimmerBuilder != null)
                {
                    shimmrtTxtRange.SetShimmer(shimmerBuilder?.Build());
                }
                shimmrtTxtRange.StartShimmer();
                shimmrtRangeView.Visibility = ViewStates.Visible;
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void StopRangeShimmer()
        {
            try
            {
                shimmrtRangeView.Visibility = ViewStates.Gone;
                if (shimmrtTxtRange.IsShimmerStarted)
                {
                    shimmrtTxtRange.StopShimmer();
                }
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void StartGraphShimmer()
        {
            try
            {
                var shimmerBuilder = ShimmerUtils.InvertedShimmerBuilderConfig();
                if (shimmerBuilder != null)
                {
                    shimmrtGraph.SetShimmer(shimmerBuilder?.Build());
                }
                shimmrtGraph.StartShimmer();
                shimmrtGraphView.Visibility = ViewStates.Visible;
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void StopGraphShimmer()
        {
            try
            {
                shimmrtGraphView.Visibility = ViewStates.Gone;
                if (shimmrtGraph.IsShimmerStarted)
                {
                    shimmrtGraph.StopShimmer();
                }
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public UsageHistoryData GetUsageHistoryData()
        {
            return selectedHistoryData;
        }

        public SMUsageHistoryData GetSMUsageHistoryData()
        {
            return selectedSMHistoryData;
        }

        public void StartAmountDueShimmer()
        {
            try
            {
                if (selectedAccount != null)
                {
                    if (selectedAccount.AccountCategoryId.Equals("2"))
                    {
                        var shimmerBuilder = ShimmerUtils.ShimmerBuilderConfig();
                        if (shimmerBuilder != null)
                        {
                            shimmrtRETotalPayableTitle.SetShimmer(shimmerBuilder?.Build());
                            shimmrtRETotalPayable.SetShimmer(shimmerBuilder?.Build());
                            shimmrtREDueDate.SetShimmer(shimmerBuilder?.Build());
                            shimmerREImg.SetShimmer(shimmerBuilder?.Build());
                        }
                        shimmrtTotalPayableTitle.StartShimmer();
                        shimmrtTotalPayable.StartShimmer();
                        shimmrtDueDate.StartShimmer();
                        shimmerREImg.StartShimmer();
                        shimmerREImg.Visibility = ViewStates.Visible;
                        shimmerREPayableLayout.Visibility = ViewStates.Visible;
                    }
                    else
                    {
                        var shimmerBuilder = ShimmerUtils.ShimmerBuilderConfig();
                        if (shimmerBuilder != null)
                        {
                            shimmrtTotalPayableTitle.SetShimmer(shimmerBuilder?.Build());
                            shimmrtTotalPayable.SetShimmer(shimmerBuilder?.Build());
                            shimmrtDueDate.SetShimmer(shimmerBuilder?.Build());
                        }
                        shimmrtTotalPayableTitle.StartShimmer();
                        shimmrtTotalPayable.StartShimmer();
                        shimmrtDueDate.StartShimmer();
                        shimmerPayableLayout.Visibility = ViewStates.Visible;
                    }
                }
                else
                {
                    var shimmerBuilder = ShimmerUtils.ShimmerBuilderConfig();
                    if (shimmerBuilder != null)
                    {
                        shimmrtTotalPayableTitle.SetShimmer(shimmerBuilder?.Build());
                        shimmrtTotalPayable.SetShimmer(shimmerBuilder?.Build());
                        shimmrtDueDate.SetShimmer(shimmerBuilder?.Build());
                    }
                    shimmrtTotalPayableTitle.StartShimmer();
                    shimmrtTotalPayable.StartShimmer();
                    shimmrtDueDate.StartShimmer();
                    shimmerPayableLayout.Visibility = ViewStates.Visible;
                }
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void StopAmountDueShimmer()
        {
            try
            {
                if (selectedAccount != null)
                {
                    if (selectedAccount.AccountCategoryId.Equals("2"))
                    {
                        shimmerREImg.Visibility = ViewStates.Gone;
                        shimmerREPayableLayout.Visibility = ViewStates.Gone;
                        if (shimmrtRETotalPayableTitle.IsShimmerStarted)
                        {
                            shimmrtRETotalPayableTitle.StopShimmer();
                        }
                        if (shimmrtRETotalPayable.IsShimmerStarted)
                        {
                            shimmrtRETotalPayable.StopShimmer();
                        }
                        if (shimmrtREDueDate.IsShimmerStarted)
                        {
                            shimmrtREDueDate.StopShimmer();
                        }
                        if (shimmerREImg.IsShimmerStarted)
                        {
                            shimmerREImg.StopShimmer();
                        }
                    }
                    else
                    {
                        shimmerPayableLayout.Visibility = ViewStates.Gone;
                        if (shimmrtTotalPayableTitle.IsShimmerStarted)
                        {
                            shimmrtTotalPayableTitle.StopShimmer();
                        }
                        if (shimmrtTotalPayable.IsShimmerStarted)
                        {
                            shimmrtTotalPayable.StopShimmer();
                        }
                        if (shimmrtDueDate.IsShimmerStarted)
                        {
                            shimmrtDueDate.StopShimmer();
                        }
                    }
                }
                else
                {
                    shimmerPayableLayout.Visibility = ViewStates.Gone;
                    if (shimmrtTotalPayableTitle.IsShimmerStarted)
                    {
                        shimmrtTotalPayableTitle.StopShimmer();
                    }
                    if (shimmrtTotalPayable.IsShimmerStarted)
                    {
                        shimmrtTotalPayable.StopShimmer();
                    }
                    if (shimmrtDueDate.IsShimmerStarted)
                    {
                        shimmrtDueDate.StopShimmer();
                    }
                }
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void StartSMStatisticShimmer()
        {
            try
            {
                if (isSMAccount)
                {
                    var shimmerBuilder = ShimmerUtils.ShimmerBuilderConfig();
                    if (shimmerBuilder != null)
                    {
                        shimmerSMStatisticBillImg.SetShimmer(shimmerBuilder?.Build());
                        shimmrtSMStatisticBillTitle.SetShimmer(shimmerBuilder?.Build());
                        shimmrtSMStatisticBill.SetShimmer(shimmerBuilder?.Build());
                        shimmrtSMStatisticBillDueDate.SetShimmer(shimmerBuilder?.Build());
                        shimmerSMStatisticPredictImg.SetShimmer(shimmerBuilder?.Build());
                        shimmrtSMStatisticPredictTitle.SetShimmer(shimmerBuilder?.Build());
                        shimmrtSMStatisticPredict.SetShimmer(shimmerBuilder?.Build());
                        shimmrtSMStatisticPredictDueDate.SetShimmer(shimmerBuilder?.Build());
                        shimmrtSmStatisticTooltip.SetShimmer(shimmerBuilder?.Build());
                    }

                    shimmerSMStatisticBillImg.StartShimmer();
                    shimmrtSMStatisticBillTitle.StartShimmer();
                    shimmrtSMStatisticBill.StartShimmer();
                    shimmrtSMStatisticBillDueDate.StartShimmer();
                    shimmerSMStatisticPredictImg.StartShimmer();
                    shimmrtSMStatisticPredictTitle.StartShimmer();
                    shimmrtSMStatisticPredict.StartShimmer();
                    shimmrtSMStatisticPredictDueDate.StartShimmer();
                    shimmrtSmStatisticTooltip.StartShimmer();

                    smStatisticContainer.Visibility = ViewStates.Visible;
                    smStatisticBillMainLayout.Visibility = ViewStates.Visible;
                    smStatisticPredictMainLayout.Visibility = ViewStates.Visible;
                    shimmerSMStatisticPredictImg.Visibility = ViewStates.Visible;
                    shimmerSMStatisticPredictLayout.Visibility = ViewStates.Visible;
                    shimmerSMStatisticBillImg.Visibility = ViewStates.Visible;
                    shimmerSMStatisticBillLayout.Visibility = ViewStates.Visible;
                    shimmrtSmStatisticTooltip.Visibility = ViewStates.Visible;
                    smStatisticBillLayout.Visibility = ViewStates.Gone;
                    smStatisticBillImg.Visibility = ViewStates.Gone;
                    smStatisticPredictImg.Visibility = ViewStates.Gone;
                    smStatisticPredictLayout.Visibility = ViewStates.Gone;
                    smStatisticTrendMainLayout.Visibility = ViewStates.Gone;
                    smStatisticTooltip.Visibility = ViewStates.Gone;
                }
                else
                {
                    smStatisticContainer.Visibility = ViewStates.Gone;
                }
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void StopSMStatisticShimmer()
        {
            try
            {
                if (isSMAccount)
                {
                    smStatisticBillMainLayout.Visibility = ViewStates.Visible;
                    smStatisticPredictMainLayout.Visibility = ViewStates.Visible;
                    smStatisticTrendMainLayout.Visibility = ViewStates.Gone;
                    shimmerSMStatisticPredictImg.Visibility = ViewStates.Gone;
                    shimmerSMStatisticPredictLayout.Visibility = ViewStates.Gone;
                    shimmerSMStatisticBillImg.Visibility = ViewStates.Gone;
                    shimmerSMStatisticBillLayout.Visibility = ViewStates.Gone;
                    shimmrtSmStatisticTooltip.Visibility = ViewStates.Gone;
                    smStatisticBillLayout.Visibility = ViewStates.Visible;
                    smStatisticBillImg.Visibility = ViewStates.Visible;
                    smStatisticPredictImg.Visibility = ViewStates.Visible;
                    smStatisticPredictLayout.Visibility = ViewStates.Visible;
                    smStatisticTooltip.Visibility = ViewStates.Visible;

                    if (shimmerSMStatisticBillImg.IsShimmerStarted)
                    {
                        shimmerSMStatisticBillImg.StopShimmer();
                    }
                    if (shimmrtSMStatisticBillTitle.IsShimmerStarted)
                    {
                        shimmrtSMStatisticBillTitle.StopShimmer();
                    }
                    if (shimmrtSMStatisticBill.IsShimmerStarted)
                    {
                        shimmrtSMStatisticBill.StopShimmer();
                    }
                    if (shimmrtSMStatisticBillDueDate.IsShimmerStarted)
                    {
                        shimmrtSMStatisticBillDueDate.StopShimmer();
                    }
                    if (shimmerSMStatisticPredictImg.IsShimmerStarted)
                    {
                        shimmerSMStatisticPredictImg.StopShimmer();
                    }
                    if (shimmrtSMStatisticPredictTitle.IsShimmerStarted)
                    {
                        shimmrtSMStatisticPredictTitle.StopShimmer();
                    }
                    if (shimmrtSMStatisticPredict.IsShimmerStarted)
                    {
                        shimmrtSMStatisticPredict.StopShimmer();
                    }
                    if (shimmrtSMStatisticPredictDueDate.IsShimmerStarted)
                    {
                        shimmrtSMStatisticPredictDueDate.StopShimmer();
                    }
                    if (shimmrtSmStatisticTooltip.IsShimmerStarted)
                    {
                        shimmrtSmStatisticTooltip.StopShimmer();
                    }

                }
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void ShowSMStatisticCard()
        {
            try
            {
                if (isSMAccount)
                {
                    smStatisticContainer.Visibility = ViewStates.Visible;
                    StopSMStatisticShimmer();
                    if (ChartDataType == ChartDataType.RM)
                    {
                        smStatisticTooltip.Visibility = ViewStates.Visible;
                        smStatisticPredictMainLayout.Visibility = ViewStates.Visible;
                        smStatisticTrendMainLayout.Visibility = ViewStates.Gone;
                        smStatisticBill.Visibility = ViewStates.Visible;
                        smStatisticBillCurrency.Visibility = ViewStates.Visible;
                        smStatisticBillKwhUnit.Visibility = ViewStates.Gone;
                        smStatisticBillKwh.Visibility = ViewStates.Gone;
                        smStatisticBillTitle.Text = "My bill amount so far";
                        smStatisticBillSubTitle.Text = "- -";
                        smStatisticBill.Text = "- -";
                        smStatisticPredictTitle.Text = "My bill may reach";
                        smStatisticPredictSubTitle.Text = "- -";
                        smStatisticPredict.Text = "- -";
                        txtSmStatisticTooltip.Text = "What are these?";
                        if (selectedSMHistoryData != null && selectedSMHistoryData.OtherUsageMetrics != null && selectedSMHistoryData.OtherUsageMetrics.CostData != null)
                        {
                            foreach (SMUsageHistoryData.Stats costValue in selectedSMHistoryData.OtherUsageMetrics.CostData)
                            {
                                if (costValue.Key == Constants.CURRENT_COST_KEY)
                                {
                                    smStatisticBillTitle.Text = string.IsNullOrEmpty(costValue.Title) ? "My bill amount so far" : costValue.Title;
                                    smStatisticBillSubTitle.Text = string.IsNullOrEmpty(costValue.SubTitle) ? "- -" : costValue.SubTitle;
                                    smStatisticBill.Text = string.IsNullOrEmpty(costValue.Value) ?  "- -" : costValue.Value;
                                    smStatisticBillCurrency.Text = string.IsNullOrEmpty(costValue.ValueUnit) ? "RM" : costValue.ValueUnit;
                                }
                                else if (costValue.Key == Constants.PROJECTED_COST_KEY)
                                {
                                    smStatisticPredictTitle.Text = string.IsNullOrEmpty(costValue.Title) ? "My bill amount so far" : costValue.Title;
                                    smStatisticPredictSubTitle.Text = string.IsNullOrEmpty(costValue.SubTitle) ? "- -" : costValue.SubTitle;
                                    smStatisticPredict.Text = string.IsNullOrEmpty(costValue.Value) ? "- -" : costValue.Value;
                                    smStatisticPredictCurrency.Text = string.IsNullOrEmpty(costValue.ValueUnit) ? "RM" : costValue.ValueUnit;
                                }
                            }
                        }

                        if (selectedSMHistoryData != null && selectedSMHistoryData.OtherUsageMetrics != null && selectedSMHistoryData.ToolTips != null && selectedSMHistoryData.ToolTips.Count > 0)
                        {
                            foreach (SMUsageHistoryData.SmartMeterToolTips costValue in selectedSMHistoryData.ToolTips)
                            {
                                if (costValue.Type == Constants.PROJECTED_COST_KEY)
                                {
                                    txtSmStatisticTooltip.Text = string.IsNullOrEmpty(costValue.SMLink) ? "What are these?" : costValue.SMLink;
                                }
                            }
                        }
                    }
                    else if (ChartDataType == ChartDataType.kWh)
                    {
                        smStatisticTooltip.Visibility = ViewStates.Gone;
                        smStatisticPredictMainLayout.Visibility = ViewStates.Gone;
                        smStatisticTrendMainLayout.Visibility = ViewStates.Visible;
                        smStatisticBill.Visibility = ViewStates.Gone;
                        smStatisticBillCurrency.Visibility = ViewStates.Gone;
                        smStatisticBillKwhUnit.Visibility = ViewStates.Visible;
                        smStatisticBillKwh.Visibility = ViewStates.Visible;
                        smStatisticBillTitle.Text = "My current usage";
                        smStatisticBillSubTitle.Text = "- -";
                        smStatisticBillKwh.Text = "- -";
                        smStatisticTrendTitle.Text = "My current usage trend is";
                        smStatisticTrendSubTitle.Text = "- -";
                        smStatisticTrend.Text = "- -%";
                        if (selectedSMHistoryData != null && selectedSMHistoryData.OtherUsageMetrics != null && selectedSMHistoryData.OtherUsageMetrics.UsageData != null && selectedSMHistoryData.OtherUsageMetrics.UsageData.Count > 0)
                        {
                            foreach (SMUsageHistoryData.Stats costValue in selectedSMHistoryData.OtherUsageMetrics.UsageData)
                            {
                                if (costValue.Key == Constants.CURRENT_USAGE_KEY)
                                {
                                    smStatisticBillTitle.Text = string.IsNullOrEmpty(costValue.Title) ? "My bill amount so far" : costValue.Title;
                                    smStatisticBillSubTitle.Text = string.IsNullOrEmpty(costValue.SubTitle) ? "- -" : costValue.SubTitle;
                                    smStatisticBillKwh.Text = string.IsNullOrEmpty(costValue.Value) ? "- -" : costValue.Value;
                                    smStatisticBillKwhUnit.Text = string.IsNullOrEmpty(costValue.ValueUnit) ? "kWh" : costValue.ValueUnit;
                                }
                                else if (costValue.Key == Constants.AVERAGE_USAGE_KEY)
                                {
                                    smStatisticTrendTitle.Text = string.IsNullOrEmpty(costValue.Title) ? "My current usage trend is" : costValue.Title;
                                    smStatisticTrendSubTitle.Text = string.IsNullOrEmpty(costValue.SubTitle) ? "- -" : costValue.SubTitle;
                                    string trendString = "- -%";
                                    if (!string.IsNullOrEmpty(costValue.Value))
                                    {
                                        if (!string.IsNullOrEmpty(costValue.ValueIndicator) && costValue.ValueIndicator.Equals("+"))
                                        {
                                            trendString = GetString(Resource.String.avg_electric_usage_up) + costValue.Value;
                                        }
                                        else if (!string.IsNullOrEmpty(costValue.ValueIndicator) && costValue.ValueIndicator.Equals("-"))
                                        {
                                            trendString = GetString(Resource.String.avg_electric_usage_down) + costValue.Value;
                                        }
                                        else
                                        {
                                            trendString = costValue.Value;
                                        }
                                    }

                                    if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.N)
                                    {

                                        smStatisticTrend.TextFormatted = Html.FromHtml(trendString, Html.FromHtmlModeLegacy);
                                    }
                                    else
                                    {
                                        smStatisticTrend.TextFormatted = Html.FromHtml(trendString);
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    smStatisticContainer.Visibility = ViewStates.Gone;
                }
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        // Lin Siong Note: Set Refresh Screen layout param
        public void SetRefreshLayoutParams()
        {
            try
            {
                LinearLayout.LayoutParams refreshImgParams = refresh_image.LayoutParameters as LinearLayout.LayoutParams;

                refreshImgParams.Width = GetDeviceHorizontalScaleInPixel(0.431f);
                refreshImgParams.Height = GetDeviceHorizontalScaleInPixel(0.431f);
                refreshImgParams.TopMargin = (int)DPUtils.ConvertDPToPx(38f);
                refresh_image.RequestLayout();

                LinearLayout.LayoutParams refreshTxtParams = txtNewRefreshMessage.LayoutParameters as LinearLayout.LayoutParams;
                refreshTxtParams.TopMargin = (int)DPUtils.ConvertDPToPx(24f);
                txtNewRefreshMessage.RequestLayout();

                LinearLayout.LayoutParams refreshButtonParams = btnNewRefresh.LayoutParameters as LinearLayout.LayoutParams;
                refreshButtonParams.TopMargin = (int)DPUtils.ConvertDPToPx(24f);
                refreshButtonParams.BottomMargin = (int)DPUtils.ConvertDPToPx(21f);
                btnNewRefresh.RequestLayout();
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        // Lin Siong Note: Set Refresh Screen layout param
        public void SetMaintenanceLayoutParams()
        {
            try
            {
                LinearLayout.LayoutParams refreshImgParams = refresh_image.LayoutParameters as LinearLayout.LayoutParams;

                refreshImgParams.Width = GetDeviceHorizontalScaleInPixel(0.603f);
                refreshImgParams.Height = GetDeviceHorizontalScaleInPixel(0.603f);
                refreshImgParams.TopMargin = (int)DPUtils.ConvertDPToPx(8f);
                refresh_image.RequestLayout();

                LinearLayout.LayoutParams refreshTxtParams = txtNewRefreshMessage.LayoutParameters as LinearLayout.LayoutParams;
                refreshTxtParams.TopMargin = (int)DPUtils.ConvertDPToPx(6f);
                txtNewRefreshMessage.RequestLayout();
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        // To get isSMDown flag
        public bool GetIsSMDown()
        {
            return isSMDown;
        }

        // To set isSMDown flag
        public void SetISSMDown(bool flag)
        {
            isSMDown = flag;
        }

        public bool GetIsSMAccount()
        {
            return isSMAccount;
        }

        // Lin Siong Note: Set virtual height layout param
        // Lin Siong Note: To handle UI misalignment
        public void SetVirtualHeightParams(float heightInDP)
        {
            try
            {
                LinearLayout.LayoutParams virtualHeightParams = virtualHeight.LayoutParameters as LinearLayout.LayoutParams;

                virtualHeightParams.Height = (int)DPUtils.ConvertDPToPx(heightInDP);
                virtualHeight.RequestLayout();
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void OnSetBackendTariffDisabled(bool flag)
        {
            isBackendTariffDisabled = flag;
        }
    }
}
