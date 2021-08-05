using System;
using System.Collections.Generic;
using AFollestad.MaterialDialogs;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Preferences;
using Android.Runtime;
using Android.Text;
using Android.Text.Method;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.CoordinatorLayout.Widget;
using AndroidX.Core.Content;
using AndroidX.RecyclerView.Widget;
using CheeseBind;
using Facebook.Shimmer;
using Google.Android.Material.BottomSheet;
using Google.Android.Material.Snackbar;
using Java.Lang;
using Java.Text;
using Java.Util;
using MikePhil.Charting.Charts;
using MikePhil.Charting.Components;
using MikePhil.Charting.Data;
using MikePhil.Charting.Formatter;
using MikePhil.Charting.Highlight;
using MikePhil.Charting.Interfaces.Datasets;
using MikePhil.Charting.Jobs;
using MikePhil.Charting.Listener;
using myTNB.SitecoreCMS.Model;
using myTNB_Android.Src.Base;
using myTNB_Android.Src.Base.Fragments;
using myTNB_Android.Src.Billing.MVP;
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
using myTNB_Android.Src.MyTNBService.Model;
using myTNB_Android.Src.MyTNBService.Response;
using myTNB_Android.Src.Notifications.Activity;
using myTNB_Android.Src.SSMR.SubmitMeterReading.MVP;
using myTNB_Android.Src.SSMRMeterHistory.MVP;
using myTNB_Android.Src.Utils;
using myTNB_Android.Src.ViewBill.Activity;
using Newtonsoft.Json;
using static MikePhil.Charting.Components.XAxis;
using static MikePhil.Charting.Components.YAxis;
using static myTNB_Android.Src.myTNBMenu.Listener.NMRESMDashboardScrollView;
using static myTNB_Android.Src.myTNBMenu.Models.GetInstallationDetailsResponse;

namespace myTNB_Android.Src.myTNBMenu.Fragments
{
    public class DashboardChartFragment : BaseFragment
        , DashboardChartContract.IView
        , NMRESMDashboardScrollViewListener
        , ViewTreeObserver.IOnGlobalLayoutListener
        , MikePhil.Charting.Listener.IOnChartValueSelectedListenerSupport
        , View.IOnTouchListener
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

        AccountDueAmountData accountDueAmountData;

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

        [BindView(Resource.Id.tariffBlockLegendDisclaimerLayout)]
        LinearLayout tariffBlockLegendDisclaimerLayout;

        [BindView(Resource.Id.txtTariffBlockLegendDisclaimer)]
        TextView txtTariffBlockLegendDisclaimer;

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

        [BindView(Resource.Id.smGraphZoomToggleLayout)]
        LinearLayout smGraphZoomToggleLayout;

        [BindView(Resource.Id.smGraphZoomToggle)]
        ImageView smGraphZoomToggle;

        [BindView(Resource.Id.mdmsDayViewDownLayout)]
        LinearLayout mdmsDayViewDownLayout;

        [BindView(Resource.Id.txtMdmsDayViewDown)]
        TextView txtMdmsDayViewDown;

        [BindView(Resource.Id.smDayViewZoomInIndicatorLayout)]
        LinearLayout smDayViewZoomInIndicatorLayout;

        [BindView(Resource.Id.txtDayViewZoomInIndicator)]
        TextView txtDayViewZoomInIndicator;

        [BindView(Resource.Id.layout_new_account)]
        LinearLayout newAccountLayout;

        [BindView(Resource.Id.layout_not_new_account)]
        LinearLayout notNewAccountLayout;

        [BindView(Resource.Id.new_account_content)]
        TextView newAccountContent;

        [BindView(Resource.Id.new_account_image)]
        ImageView newAccountImage;

        [BindView(Resource.Id.ssmr_account_message)]
        RelativeLayout ssmr_account_message;

        [BindView(Resource.Id.ssmr_shimmer_layout)]
        RelativeLayout ssmr_shimmer_layout;

        [BindView(Resource.Id.shimmerSSMRImg)]
        ShimmerFrameLayout shimmerSSMRImg;

        [BindView(Resource.Id.shimmerSSMRTitle)]
        ShimmerFrameLayout shimmerSSMRTitle;

        [BindView(Resource.Id.shimmerSSMRMessage)]
        ShimmerFrameLayout shimmerSSMRMessage;

        [BindView(Resource.Id.btnMDMSDownRefresh)]
        Button btnMDMSDownRefresh;

        [BindView(Resource.Id.imgMdmsDayViewDown)]
        ImageView imgMdmsDayViewDown;

        [BindView(Resource.Id.dashboard_dropdown)]
        ImageView imgRmKwhDropdownArrow;

        [BindView(Resource.Id.reNoPayableLayout)]
        LinearLayout reNoPayableLayout;

        [BindView(Resource.Id.txtReNoPayableTitle)]
        TextView txtReNoPayableTitle;

        [BindView(Resource.Id.txtReNoPayable)]
        TextView txtReNoPayable;

        [BindView(Resource.Id.txtReNoPayableCurrency)]
        TextView txtReNoPayableCurrency;

        [BindView(Resource.Id.dashboard_bottom_view)]
        LinearLayout dashboard_bottom_view;

        [BindView(Resource.Id.dashboard_top_view)]
        LinearLayout dashboard_top_view;

        [BindView(Resource.Id.infoLabelContainerEPP)]
        LinearLayout infoLabelEPP;

        [BindView(Resource.Id.infoLabelEPP)]
        TextView lblinfoLabelEPP;


        private static bool isZoomIn = false;

        TariffBlockLegendAdapter tariffBlockLegendAdapter;

        private DashboardChartContract.IUserActionsListener userActionsListener;
        private DashboardChartPresenter mPresenter;

        private string txtRefreshMsg = "";
        private string txtBtnRefreshTitle = "";

        private bool isSubmitMeter = false;

        private bool isBackendTariffDisabled = false;

        private static bool isREAccount = false;

        DecimalFormat decimalFormat = new DecimalFormat("#,###,###,###,##0.00", new DecimalFormatSymbols(Java.Util.Locale.Us));
        SimpleDateFormat dateParser = new SimpleDateFormat("dd/MM/yyyy", LocaleUtils.GetDefaultLocale());
        SimpleDateFormat dateFormatter = new SimpleDateFormat("dd MMM yyyy", LocaleUtils.GetCurrentLocale());
        IAxisValueFormatter XLabelsFormatter;
        private string errorMSG = null;

        public readonly static int SSMR_METER_HISTORY_ACTIVITY_CODE = 8796;

        public readonly static int SSMR_SUBMIT_METER_ACTIVITY_CODE = 8797;

        private SMRActivityInfoResponse smrResponse;

        private AccountDueAmountResponse amountDueResponse;

        private GetInstallationDetailsResponse accountStatusResponse;

        ISharedPreferences mPref;

        bool isToggleTariff = false;

        bool isDayViewToggle = false;

        static bool isBCRMDown = false;

        static bool isPaymentDown = false;

        static bool isUsageLoadedNeeded = true;

        private bool isChangeBackgroundNeeded = true;

        private bool isScrollIndicatorShowNeed = false;

        private bool isSMR = false;

        bool isMDMSDown = false;

        bool isSMAccount = false;

        public StackedBarChartRenderer renderer;

        public SMStackedBarChartRenderer smRenderer;

        static bool requireScroll;

        private int CurrentParentIndex = -1;

        private List<double> DayViewRMData = new List<double>();
        private List<double> DayViewkWhData = new List<double>();
        private List<bool> missingReadingList = new List<bool>();
        private static List<string> dayViewMonthList = new List<string>();

        private Bitmap mdmsBitmap = null;
        private Bitmap missingBitmap = null;
        private Bitmap dpcBitmap = null;

        private static bool isDayViewFirstMove = false;

        private static List<BarEntry> dayViewTariffList = new List<BarEntry>();

        private static int minCurrentDayViewIndex = 4;
        private static int maxCurrentDayViewIndex = 0;

        private static float currentLowestVisibleX = -0.5f;
        private static float trackingLowestVisibleX = -0.5f;
        private static float minLowestVisibleX = -0.5f;
        private static float maxLowestVisibleX = 0f;

        private static int currentDayViewIndex = 0;

        private static bool isShowLog = false;

        private static float lowestVisibleX = -1f;

        private bool isChangeVirtualHeightNeed = false;

        private bool isShowAnimationDisable = false;

        private static bool isTutorialShow = false;

        private static bool isClickedShowTariff = false;

        private static bool isHideBottomSheetShowTariff = false;

        private string MDMSUnavailableTitle = "";

        private string MDMSUnavailableMessage = "";

        private string MDMSUnavailableCTA = "";

        private int currentSelectedBar = -1;

        private bool isDPCBarClicked = false;

        private bool isMDMSPlannedDownTime = false;

        private bool isGoToBillingDetail = false;

        private bool mIsPendingPayment = false;

        private DecimalFormat smDecimalFormat = new DecimalFormat("#,###,##0.00", new DecimalFormatSymbols(Java.Util.Locale.Us));
        private DecimalFormat smKwhFormat = new DecimalFormat("#,###,##0", new DecimalFormatSymbols(Java.Util.Locale.Us));

        ScaleGestureDetector mScaleDetector;

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
                    if (usageHistoryDataResponse != null && usageHistoryDataResponse.Data != null && usageHistoryDataResponse.Data.UsageHistoryData != null && IsCheckDataReadyData(usageHistoryDataResponse.Data.UsageHistoryData))
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

                        try
                        {
                            if (usageHistoryDataResponse != null && usageHistoryDataResponse.Data != null && usageHistoryDataResponse.Data.RefreshMessage != null && !string.IsNullOrEmpty(usageHistoryDataResponse.Data.RefreshMessage))
                            {
                                txtRefreshMsg = usageHistoryDataResponse.Data.RefreshMessage;
                            }
                            else
                            {
                                txtRefreshMsg = Utility.GetLocalizedCommonLabel("refreshDescription");
                            }
                        }
                        catch (System.Exception e)
                        {
                            txtRefreshMsg = Utility.GetLocalizedCommonLabel("refreshDescription");
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
                                txtBtnRefreshTitle = Utility.GetLocalizedCommonLabel("refreshNow");
                            }
                        }
                        catch (System.Exception e)
                        {
                            txtBtnRefreshTitle = Utility.GetLocalizedCommonLabel("refreshNow");
                            Utility.LoggingNonFatalError(e);
                        }
                    }
                    else
                    {
                        isUsageLoadedNeeded = true;
                        selectedHistoryData = null;
                        txtRefreshMsg = Utility.GetLocalizedCommonLabel("refreshDescription");
                        txtBtnRefreshTitle = Utility.GetLocalizedCommonLabel("refreshNow");
                    }
                }
                else
                {
                    isSMAccount = true;
                    isUsageLoadedNeeded = false;
                    selectedHistoryData = null;
                    var usageHistoryDataResponse = JsonConvert.DeserializeObject<SMUsageHistoryResponse>(extras.GetString(Constants.SELECTED_SM_ACCOUNT_USAGE_RESPONSE));
                    if (usageHistoryDataResponse != null && usageHistoryDataResponse.Data != null && usageHistoryDataResponse.Data.SMUsageHistoryData != null && IsCheckDataReadyData(usageHistoryDataResponse.Data.SMUsageHistoryData))
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

                        try
                        {
                            if (usageHistoryDataResponse != null && usageHistoryDataResponse.Data != null && usageHistoryDataResponse.Data.RefreshMessage != null && !string.IsNullOrEmpty(usageHistoryDataResponse.Data.RefreshMessage))
                            {
                                txtRefreshMsg = usageHistoryDataResponse.Data.RefreshMessage;
                            }
                            else
                            {
                                txtRefreshMsg = Utility.GetLocalizedCommonLabel("refreshDescription");
                            }
                        }
                        catch (System.Exception e)
                        {
                            txtRefreshMsg = Utility.GetLocalizedCommonLabel("refreshDescription");
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
                                txtBtnRefreshTitle = Utility.GetLocalizedCommonLabel("refreshNow");
                            }
                        }
                        catch (System.Exception e)
                        {
                            txtBtnRefreshTitle = Utility.GetLocalizedCommonLabel("refreshNow");
                            Utility.LoggingNonFatalError(e);
                        }
                    }
                    else
                    {
                        isUsageLoadedNeeded = true;
                        selectedSMHistoryData = null;
                        txtRefreshMsg = Utility.GetLocalizedCommonLabel("refreshDescription");
                        txtBtnRefreshTitle = Utility.GetLocalizedCommonLabel("refreshNow");
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
                txtRefreshMsg = Utility.GetLocalizedCommonLabel("refreshDescription");
                txtBtnRefreshTitle = Utility.GetLocalizedCommonLabel("refreshNow");
            }

            errorMSG = "";

            if (extras.ContainsKey(Constants.SELECTED_ERROR_MSG))
            {
                errorMSG = extras.GetString(Constants.SELECTED_ERROR_MSG);
            }

            this.HasOptionsMenu = true;
            this.mPresenter = new DashboardChartPresenter(this, PreferenceManager.GetDefaultSharedPreferences(this.Activity));

            mIsPendingPayment = false;
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

                smDayViewZoomInIndicatorLayout.Visibility = ViewStates.Gone;

                isTutorialShow = false;
                isClickedShowTariff = false;
                isHideBottomSheetShowTariff = false;

                BitmapFactory.Options opt = new BitmapFactory.Options();
                opt.InMutable = true;
                mdmsBitmap = BitmapFactory.DecodeResource(this.Activity.Resources, Resource.Drawable.mdms_down, opt);
                missingBitmap = BitmapFactory.DecodeResource(this.Activity.Resources, Resource.Drawable.dashboard_missing_copy, opt);
                dpcBitmap = BitmapFactory.DecodeResource(this.Activity.Resources, Resource.Drawable.dpc_down, opt);


                bottomSheetBehavior = BottomSheetBehavior.From(bottomSheet);
                bottomSheetBehavior.State = BottomSheetBehavior.StateExpanded;
                bottomSheetBehavior.SetBottomSheetCallback(new DashboardBottomSheetCallBack());

                scrollView = view.FindViewById<NMRESMDashboardScrollView>(Resource.Id.scroll_view);
                ViewTreeObserver observer = scrollView.ViewTreeObserver;
                observer.AddOnGlobalLayoutListener(this);

                scrollView.setOnScrollViewListener(this);
                scrollView.OverScrollMode = OverScrollMode.Always;

                scrollViewContent.SetBackgroundResource(Resource.Drawable.dashboard_chart_bg);

                requireScroll = false;

                isDayViewFirstMove = false;

                currentDayViewIndex = 0;
                dayViewTariffList = new List<BarEntry>();

                TextViewUtils.SetMuseoSans300Typeface(txtAddress, txtTotalPayable, txtDueDate);
                TextViewUtils.SetMuseoSans300Typeface(txtNewRefreshMessage, ssmrAccountStatusText);
                TextViewUtils.SetMuseoSans500Typeface(txtRange, txtTotalPayableTitle
                    , txtTotalPayableCurrency, btnViewBill, btnPay, btnNewRefresh
                    , rmKwhLabel, kwhLabel, rmLabel, dashboardAccountName
                    , btnTxtSsmrViewHistory, btnReadingHistory, txtEnergyDisconnection);
                TextViewUtils.SetMuseoSans300Typeface(reTotalPayable, reTotalPayableCurrency
                    , reDueDate, txtNoPayable);
                TextViewUtils.SetMuseoSans500Typeface(reTotalPayableTitle, btnReView
                    , txtTarifToggle, txtNoPayableTitle, txtNoPayableCurrency);
                TextViewUtils.SetMuseoSans300Typeface(smStatisticBillSubTitle, smStatisticBill
                    , smStatisticBillCurrency, smStatisticBillKwhUnit, smStatisticBillKwh
                    , smStatisticPredictSubTitle, smStatisticPredict, smStatisticPredictCurrency
                    , smStatisticTrendSubTitle, smStatisticTrend);
                TextViewUtils.SetMuseoSans500Typeface(smStatisticBillTitle, smStatisticPredictTitle
                    , txtSmStatisticTooltip, smStatisticTrendTitle, txtDayViewZoomInIndicator);
                TextViewUtils.SetMuseoSans300Typeface(btnToggleDay, btnToggleMonth
                    , txtMdmsDayViewDown, newAccountContent, txtTariffBlockLegendDisclaimer);
                TextViewUtils.SetMuseoSans300Typeface(txtReNoPayable, txtReNoPayableCurrency);
                TextViewUtils.SetMuseoSans500Typeface(txtReNoPayableTitle);

                /*
                 DO NOT DELETE
                    Retain Fonts for:
                    1. txtTotalPayableTitle
                    2. txtTotalPayable
                    3. txtTotalPayableCurrency
                    4. txtDueDate

                 */

                TextViewUtils.SetTextSize(13, smStatisticBillTitle, false);
                TextViewUtils.SetTextSize(13, smStatisticBillSubTitle, false);
                TextViewUtils.SetTextSize(13, smStatisticPredictTitle, false);
                TextViewUtils.SetTextSize(13, smStatisticPredictCurrency, false);
                TextViewUtils.SetTextSize(13, smStatisticPredictSubTitle, false);
                TextViewUtils.SetTextSize(13, smStatisticTrendTitle, false);
                TextViewUtils.SetTextSize(13, smStatisticTrendSubTitle, false);
                TextViewUtils.SetTextSize(14, txtTotalPayableTitle, false);
                TextViewUtils.SetTextSize(14, txtTotalPayableCurrency, false);
                TextViewUtils.SetTextSize(14, txtDueDate, false);
                TextViewUtils.SetTextSize(17, smStatisticBill, false);
                TextViewUtils.SetTextSize(17, smStatisticBillKwh, false);
                TextViewUtils.SetTextSize(17, smStatisticPredict, false);
                TextViewUtils.SetTextSize(17, smStatisticTrend, false);
                TextViewUtils.SetTextSize(24, txtTotalPayable, false);

                TextViewUtils.SetTextSize11(txtAddress, txtDayViewZoomInIndicator, txtTariffBlockLegendDisclaimer
                    , reTotalPayableCurrency, smStatisticBillCurrency, smStatisticBillKwhUnit, lblinfoLabelEPP);
                TextViewUtils.SetTextSize13(btnToggleDay, btnToggleMonth, txtEnergyDisconnection, txtRange
                    , kwhLabel, rmLabel, rmKwhLabel, txtTarifToggle, reTotalPayableTitle, reDueDate, txtReNoPayableTitle
                    , txtReNoPayableCurrency, ssmrAccountStatusText, btnTxtSsmrViewHistory, txtSmStatisticTooltip, txtNoPayableCurrency);
                TextViewUtils.SetTextSize14(txtNewRefreshMessage);
                TextViewUtils.SetTextSize15(newAccountContent, txtMdmsDayViewDown, txtNoPayableTitle);
                TextViewUtils.SetTextSize16(btnPay, btnNewRefresh, btnMDMSDownRefresh
                    , btnReView, btnViewBill, btnReadingHistory);
                TextViewUtils.SetTextSize17(reTotalPayable, txtReNoPayable);
                TextViewUtils.SetTextSize18(dashboardAccountName);
                TextViewUtils.SetTextSize25(txtNoPayable);

                txtTarifToggle.Text = Utility.GetLocalizedLabel("Usage", "tariffBlock");
                btnViewBill.Text = Utility.GetLocalizedLabel("Usage", "viewDetails");
                btnPay.Text = Utility.GetLocalizedLabel("Usage", "pay");
                txtNoPayableTitle.Text = Utility.GetLocalizedLabel("Usage", "needToPay");
                txtTotalPayableTitle.Text = Utility.GetLocalizedLabel("Usage", "needToPay");
                reTotalPayableTitle.Text = Utility.GetLocalizedLabel("Usage", "myEarnings");
                txtReNoPayableTitle.Text = Utility.GetLocalizedLabel("Usage", "myEarnings");
                btnReView.Text = Utility.GetLocalizedLabel("Usage", "viewPaymentAdvice");
                btnToggleDay.Text = Utility.GetLocalizedCommonLabel("day");
                btnToggleMonth.Text = Utility.GetLocalizedCommonLabel("month");

                DownTimeEntity bcrmEntity = DownTimeEntity.GetByCode(Constants.BCRM_SYSTEM);

                isZoomIn = false;

                if (bcrmEntity != null && bcrmEntity.IsDown)
                {
                    isBCRMDown = true;
                }
                else
                {
                    isBCRMDown = false;
                }

                isPaymentDown = false;

                if (!Utility.IsEnablePayment())
                {
                    isPaymentDown = true;
                }

                try
                {
                    ((DashboardHomeActivity)Activity).SetStatusBarBackground(Resource.Drawable.UsageGradientBackground);
                    ((DashboardHomeActivity)Activity).SetToolbarBackground(Resource.Drawable.CustomDashboardGradientToolbar);
                }
                catch (System.Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                }

                dashboard_bottom_view.SetBackgroundResource(Resource.Drawable.usage_bottom_view);

                if (selectedAccount != null)
                {

                    //commercial pop up checking YANA
                    GovermentCommercial();

                    //txtAddress.Text = selectedAccount.AddStreet;

                    //if not owner mask the address IRUL
                    if (!selectedAccount.IsOwner == true)
                    {
                        if (!selectedAccount.IsHaveAccess == true)
                        {
                            txtAddress.Text = Utility.StringSpaceMasking(Utility.Masking.Address, selectedAccount.AddStreet);
                        }
                        else
                        {
                            txtAddress.Text = selectedAccount.AddStreet;
                        }
                    }
                    else
                    {
                        txtAddress.Text = selectedAccount.AddStreet;
                    }




                    if (selectedAccount.AccountCategoryId.Equals("2"))
                    {
                        HideSSMRDashboardView();
                        bottomSheetBehavior.State = BottomSheetBehavior.StateHidden;
                        isREAccount = true;
                        reContainer.Visibility = ViewStates.Visible;
                        ssmrHistoryContainer.Visibility = ViewStates.Gone;
                        btnPay.Visibility = ViewStates.Gone;
                        energyTipsView.Visibility = ViewStates.Gone;
                        btnViewBill.Text = Utility.GetLocalizedLabel("Usage", "viewPaymentAdvice");
                        // txtUsageHistory.Visibility = ViewStates.Gone;
                        txtTotalPayableTitle.Text = Utility.GetLocalizedLabel("Usage", "myEarnings");
                        graphToggleSelection.Visibility = ViewStates.Gone;
                        isChangeVirtualHeightNeed = true;
                        SetVirtualHeightParams(6f);
                        isChangeBackgroundNeeded = true;
                        layoutSMSegmentGroup.Visibility = ViewStates.Gone;
                        isSMR = false;
                    }
                    else if (isSMAccount)
                    {
                        // Smart Meter
                        HideSSMRDashboardView();
                        isChangeVirtualHeightNeed = true;
                        SetVirtualHeightParams(6f);
                        isREAccount = false;
                        reContainer.Visibility = ViewStates.Gone;
                        btnPay.Visibility = ViewStates.Visible;
                        btnViewBill.Text = Utility.GetLocalizedLabel("Usage", "viewDetails");
                        graphToggleSelection.Visibility = ViewStates.Visible;
                        energyTipsView.Visibility = ViewStates.Visible;
                        isChangeBackgroundNeeded = true;
                        layoutSMSegmentGroup.Visibility = ViewStates.Visible;
                        isSMR = false;
                        smGraphZoomToggleLayout.Visibility = ViewStates.Gone;
                    }
                    else
                    {
                        isSMR = this.mPresenter.IsOwnedSMRLocal(selectedAccount.AccountNum);
                        if (isSMR)
                        {
                            StartSSMRDashboardViewShimmer();
                            isChangeVirtualHeightNeed = true;
                            SetVirtualHeightParams(6f);
                            isChangeBackgroundNeeded = true;
                        }
                        else
                        {
                            HideSSMRDashboardView();
                            dashboard_bottom_view.SetBackgroundResource(0);
                            isChangeVirtualHeightNeed = true;
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
                        }
                        isREAccount = false;
                        reContainer.Visibility = ViewStates.Gone;
                        btnPay.Visibility = ViewStates.Visible;
                        layoutSMSegmentGroup.Visibility = ViewStates.Gone;
                        btnViewBill.Text = Utility.GetLocalizedLabel("Usage", "viewDetails");
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
                    HideSSMRDashboardView();
                    dashboard_bottom_view.SetBackgroundResource(0);
                    rootView.SetBackgroundResource(0);
                    scrollViewContent.SetBackgroundResource(0);
                    isChangeVirtualHeightNeed = true;
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

                ((DashboardHomeActivity)Activity).HideAccountName();
                dashboardAccountName.Visibility = ViewStates.Gone;
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
                tariffBlockLegendDisclaimerLayout.Visibility = ViewStates.Gone;
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

                if (isUsageLoadedNeeded)
                {
                    rmKwhSelection.Enabled = false;
                    tarifToggle.Enabled = false;
                    btnToggleDay.Enabled = false;
                    btnToggleMonth.Enabled = false;
                    txtRange.Visibility = ViewStates.Gone;
                    StartRangeShimmer();
                    mChart.Visibility = ViewStates.Gone;
                    StartGraphShimmer();
                }
                else
                {
                    rmKwhSelection.Enabled = true;
                    rmKwhLabel.SetTextColor(new Color(ContextCompat.GetColor(this.Activity, Resource.Color.powerBlue)));
                    imgRmKwhDropdownArrow.SetImageResource(Resource.Drawable.rectangle);
                    tarifToggle.Enabled = true;
                    btnToggleDay.Enabled = true;
                    btnToggleMonth.Enabled = true;
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
                        if (MyTNBAccountManagement.GetInstance().IsEnergyTipsDisabled())
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
                            FirebaseAnalyticsUtils.LogFragmentClickEvent(this, "BCRM Downtime Message Clicked");
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

        public void GovermentCommercial()
        {

            List<AccountData> selectedAccountList = new List<AccountData>();
            List<AccountData> selectedAccountList2 = new List<AccountData>();
            List<AccountData> selectedAccountList3 = UserSessions.GetCommercialList();
            if (selectedAccountList3.Count == 0)
            {
                selectedAccountList.Add(selectedAccount);
                UserSessions.SetCommercialList(selectedAccountList);
                GovermentCommercialDialog();
            }
            else
            {
                selectedAccountList2 = UserSessions.GetCommercialList();
                bool acc = false;
                foreach (AccountData accountData in selectedAccountList2)
                {
                    selectedAccountList.Add(accountData);
                    if (accountData.AccountNum == selectedAccount.AccountNum)
                    {
                        acc = true;
                    }

                }

                if ((selectedAccount.AccountTypeId.Equals("2") || selectedAccount.AccountCategoryId.Equals("3")) && !acc)
                {
                    selectedAccountList.Add(selectedAccount);
                    UserSessions.SetCommercialList(selectedAccountList);
                    GovermentCommercialDialog();
                }

            }

        }

        //popup commercial
        public void GovermentCommercialDialog()
        {
            if (selectedAccount.AccountTypeId.Equals("2") || selectedAccount.AccountCategoryId.Equals("3"))
            {
                string data = Utility.GetLocalizedLabel("Usage", "accountTypeDialogMessage");
                string temp = string.Format(data);
                MyTNBAppToolTipBuilder.Create(this.Activity, MyTNBAppToolTipBuilder.ToolTipType.NORMAL_WITH_HEADER)
                           .SetTitle((string.Format(Utility.GetLocalizedLabel("Usage", "accountTypeDialogTitle"))))
                           .SetMessage(temp)
                           .SetContentGravity(GravityFlags.Left)
                           .SetCTALabel(Utility.GetLocalizedCommonLabel("gotIt"))
                           .Build().Show();

            }

        }


        [OnClick(Resource.Id.dashboard_txt_account_name)]
        void OnSelectSupplyAccount(object sender, EventArgs eventArgs)
        {
            try
            {
                if (!this.GetIsClicked())
                {
                    this.SetIsClicked(true);
                    ((DashboardHomeActivity)Activity).OnSelectAccount();
                }
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void ShowBillPDF()
        {
            Intent viewBill = new Intent(this.activity, typeof(ViewBillActivity));
            viewBill.PutExtra(Constants.SELECTED_ACCOUNT, JsonConvert.SerializeObject(selectedAccount));
            viewBill.PutExtra(Constants.CODE_KEY, Constants.SELECT_ACCOUNT_PDF_REQUEST_CODE);
            StartActivity(viewBill);
            this.SetIsClicked(false);
        }

        private void showEPPTooltip()
        {
            List<EPPTooltipResponse> modelList = MyTNBAppToolTipData.GetEppToolTipData();

            if (modelList != null && modelList.Count > 0)
            {
                MyTNBAppToolTipBuilder eppTooltip = MyTNBAppToolTipBuilder.Create(this.Activity, MyTNBAppToolTipBuilder.ToolTipType.IMAGE_HEADER_TWO_BUTTON)
                    .SetHeaderImageBitmap(modelList[0].ImageBitmap)
                    .SetTitle(modelList[0].PopUpTitle)
                    .SetMessage(modelList[0].PopUpBody)
                    .SetCTALabel(Utility.GetLocalizedCommonLabel("gotIt"))
                    .SetCTAaction(() => { this.SetIsClicked(false); })
                    .SetSecondaryCTALabel(Utility.GetLocalizedCommonLabel("viewBill"))
                    .SetSecondaryCTAaction(() => ShowBillPDF())
                    .Build();
                eppTooltip.Show();
            }
            else
            {
                this.SetIsClicked(false);
            }
        }

        [OnClick(Resource.Id.smStatisticTooltip)]
        void OnSMStatisticTooltipClick(object sender, EventArgs eventArgs)
        {
            try
            {
                string textMessage = Utility.GetLocalizedLabel("Usage", "projectedCostMsg");
                string btnLabel = Utility.GetLocalizedCommonLabel("gotIt");

                if (selectedSMHistoryData != null
                    && selectedSMHistoryData.OtherUsageMetrics != null
                    && selectedSMHistoryData.ToolTips != null
                    && selectedSMHistoryData.ToolTips.Count > 0)
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

                            btnLabel = costValue.SMBtnText ?? Utility.GetLocalizedCommonLabel("gotIt");
                        }
                    }
                }

                if (textMessage != "" && btnLabel != "")
                {
                    MyTNBAppToolTipBuilder smartMeterStatsTooltip = MyTNBAppToolTipBuilder.Create(this.Activity, MyTNBAppToolTipBuilder.ToolTipType.NORMAL_WITH_HEADER)
                        .SetTitle(string.Empty)
                        .SetMessage(textMessage)
                        .SetCTALabel(btnLabel)
                        .Build();
                    smartMeterStatsTooltip.Show();
                }
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
                if (!this.GetIsClicked())
                {
                    this.SetIsClicked(true);
                    StartSSMRMeterHistoryPage();
                }
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        [OnClick(Resource.Id.infoLabelContainerEPP)]
        void OnEPPClick(object sender, EventArgs eventArgs)
        {
            try
            {
                if (!this.GetIsClicked())
                {
                    this.SetIsClicked(true);
                    showEPPTooltip();

                }
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
                if (!this.GetIsClicked())
                {
                    this.SetIsClicked(true);
                    if (isSubmitMeter)
                    {
                        StartSSMRSubmitMeterReadingPage();
                    }
                    else
                    {
                        StartSSMRMeterHistoryPage();
                    }
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
            smGraphZoomToggleLayout.Visibility = ViewStates.Visible;
            this.userActionsListener.OnByDay();
            if (!isDayViewToggle && !isMDMSDown)
            {
                isDayViewToggle = true;
                try
                {
                    MaterialDialog dayViewZoomTooltip = DayZoomOutPinchUtil.OnBuildZoomOutPinchTooltip(this.Activity);
                    dayViewZoomTooltip.Show();
                }
                catch (System.Exception ne)
                {
                    Utility.LoggingNonFatalError(ne);
                }
            }
        }

        [OnClick(Resource.Id.btnToggleMonth)]
        internal void OnToggleMonth(object sender, EventArgs e)
        {
            isDayViewFirstMove = false;
            currentDayViewIndex = 0;
            smGraphZoomToggleLayout.Visibility = ViewStates.Gone;
            this.userActionsListener.OnByMonth();
        }

        [OnClick(Resource.Id.smGraphZoomToggleLayout)]
        internal void OnToggleZoom(object sender, EventArgs e)
        {
            if (!isZoomIn)
            {
                isZoomIn = true;
            }
            else
            {
                isZoomIn = false;
            }

            isDayViewFirstMove = false;
            currentDayViewIndex = 0;

            this.userActionsListener.OnByZoom();
        }

        private void StartSSMRMeterHistoryPage()
        {
            Intent ssmr_history_activity = new Intent(this.Activity, typeof(SSMRMeterHistoryActivity));
            ssmr_history_activity.PutExtra(Constants.SELECTED_ACCOUNT, JsonConvert.SerializeObject(selectedAccount));
            ssmr_history_activity.PutExtra(Constants.SMR_RESPONSE_KEY, JsonConvert.SerializeObject(smrResponse));
            ssmr_history_activity.PutExtra("fromUsage", true);
            SMRPopUpUtils.SetFromUsageFlag(true);
            StartActivityForResult(ssmr_history_activity, SSMR_METER_HISTORY_ACTIVITY_CODE);
        }

        private void StartSSMRSubmitMeterReadingPage()
        {
            Intent ssmr_submit_meter_activity = new Intent(this.Activity, typeof(SubmitMeterReadingActivity));
            ssmr_submit_meter_activity.PutExtra(Constants.SELECTED_ACCOUNT, JsonConvert.SerializeObject(selectedAccount));
            ssmr_submit_meter_activity.PutExtra(Constants.SMR_RESPONSE_KEY, JsonConvert.SerializeObject(smrResponse));
            SMRPopUpUtils.SetFromUsageFlag(true);
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

            bool isTariffAvailable = true;
            if (isBackendTariffDisabled)
            {
                isTariffAvailable = false;
            }
            else
            {
                if (isSMAccount)
                {
                    if (GetIsMDMSDown() && ChartType == ChartType.Day)
                    {
                        isTariffAvailable = false;
                        rmKwhSelection.Enabled = false;
                        smGraphZoomToggleLayout.Visibility = ViewStates.Gone;
                        rmKwhLabel.SetTextColor(new Color(ContextCompat.GetColor(this.Activity, Resource.Color.silverChalice)));
                        imgRmKwhDropdownArrow.SetImageResource(Resource.Drawable.rectangle_disable);
                    }
                    else
                    {
                        if (selectedSMHistoryData != null
                            && selectedSMHistoryData.ByMonth != null
                            && selectedSMHistoryData.ByMonth.Months != null
                            && selectedSMHistoryData.ByMonth.Months.Count > 0)
                        {
                            for (int i = 0; i < selectedSMHistoryData.ByMonth.Months.Count; i++)
                            {
                                if (selectedSMHistoryData.ByMonth.Months[i].TariffBlocksList != null
                                    && selectedSMHistoryData.ByMonth.Months[i].TariffBlocksList.Count > 0
                                    && !selectedSMHistoryData.ByMonth.Months[i].DPCIndicator)
                                {
                                    bool isFound = false;

                                    if (selectedSMHistoryData.TariffBlocksLegend != null && selectedSMHistoryData.TariffBlocksLegend.Count > 0)
                                    {
                                        for (int k = 0; k < selectedSMHistoryData.TariffBlocksLegend.Count; k++)
                                        {
                                            for (int j = 0; j < selectedSMHistoryData.ByMonth.Months[i].TariffBlocksList.Count; j++)
                                            {
                                                if (selectedSMHistoryData.ByMonth.Months[i].TariffBlocksList[j].BlockId == selectedSMHistoryData.TariffBlocksLegend[k].BlockId)
                                                {
                                                    float val = (float)selectedSMHistoryData.ByMonth.Months[i].TariffBlocksList[j].Amount;
                                                    if (float.IsPositiveInfinity(val))
                                                    {
                                                        val = float.PositiveInfinity;
                                                    }
                                                    if (System.Math.Abs(val) > 0)
                                                    {
                                                        isFound = true;
                                                        isTariffAvailable = true;
                                                        break;
                                                    }
                                                }
                                            }

                                            if (isFound)
                                            {
                                                break;
                                            }
                                        }
                                    }

                                    if (!isFound)
                                    {
                                        isTariffAvailable = false;
                                    }
                                    else
                                    {
                                        break;
                                    }
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
                else
                {
                    if (selectedHistoryData != null
                        && selectedHistoryData.ByMonth != null
                        && selectedHistoryData.ByMonth.Months != null
                        && selectedHistoryData.ByMonth.Months.Count > 0)
                    {
                        for (int i = 0; i < selectedHistoryData.ByMonth.Months.Count; i++)
                        {
                            if (selectedHistoryData.ByMonth.Months[i].TariffBlocksList != null
                                && selectedHistoryData.ByMonth.Months[i].TariffBlocksList.Count > 0
                                && !selectedHistoryData.ByMonth.Months[i].DPCIndicator)
                            {
                                bool isFound = false;

                                if (selectedHistoryData.TariffBlocksLegend != null && selectedHistoryData.TariffBlocksLegend.Count > 0)
                                {
                                    for (int k = 0; k < selectedHistoryData.TariffBlocksLegend.Count; k++)
                                    {
                                        for (int j = 0; j < selectedHistoryData.ByMonth.Months[i].TariffBlocksList.Count; j++)
                                        {
                                            if (selectedHistoryData.ByMonth.Months[i].TariffBlocksList[j].BlockId == selectedHistoryData.TariffBlocksLegend[k].BlockId)
                                            {
                                                float val = (float)selectedHistoryData.ByMonth.Months[i].TariffBlocksList[j].Amount;
                                                if (float.IsPositiveInfinity(val))
                                                {
                                                    val = float.PositiveInfinity;
                                                }
                                                if (System.Math.Abs(val) > 0)
                                                {
                                                    isFound = true;
                                                    isTariffAvailable = true;
                                                    break;
                                                }
                                            }
                                        }

                                        if (isFound)
                                        {
                                            break;
                                        }
                                    }
                                }

                                if (!isFound)
                                {
                                    isTariffAvailable = false;
                                }
                                else
                                {
                                    break;
                                }
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

            if (GetIsMDMSDown())
            {
                HideSMStatisticCard();
            }
            else
            {
                ShowSMStatisticCard();
            }

            if (isTariffAvailable)
            {
                tarifToggle.Enabled = true;
                if (!isToggleTariff)
                {
                    imgTarifToggle.SetImageResource(Resource.Drawable.eye);
                    tarifToggle.SetBackgroundResource(Resource.Drawable.rectangle_white_outline_rounded_button_bg);
                    txtTarifToggle.SetTextColor(new Color(ContextCompat.GetColor(this.Activity, Resource.Color.white)));
                    txtTarifToggle.Alpha = 1f;
                    txtTarifToggle.Text = Utility.GetLocalizedLabel("Usage", "tariffBlock");
                }
                else
                {
                    imgTarifToggle.SetImageResource(Resource.Drawable.eye_hide);
                    tarifToggle.SetBackgroundResource(Resource.Drawable.rectangle_rounded_button_bg);
                    txtTarifToggle.SetTextColor(new Color(ContextCompat.GetColor(this.Activity, Resource.Color.powerBlue)));
                    txtTarifToggle.Alpha = 1f;
                    txtTarifToggle.Text = Utility.GetLocalizedLabel("Usage", "tariffBlock");
                }

                if (!isSMAccount)
                {
                    OnGenerateTariffLegendValue(CurrentParentIndex == -1 ? selectedHistoryData.ByMonth.Months.Count - 1 : CurrentParentIndex, isToggleTariff);
                }
                else
                {
                    OnGenerateTariffLegendValue(CurrentParentIndex == -1 ? selectedSMHistoryData.ByMonth.Months.Count - 1 : CurrentParentIndex, isToggleTariff);
                }
            }
            else
            {
                tarifToggle.Enabled = false;
                if (!isSMAccount)
                {
                    OnGenerateTariffLegendValue(CurrentParentIndex == -1 ? selectedHistoryData.ByMonth.Months.Count - 1 : CurrentParentIndex, isToggleTariff);
                }
                else
                {
                    OnGenerateTariffLegendValue(CurrentParentIndex == -1 ? selectedSMHistoryData.ByMonth.Months.Count - 1 : CurrentParentIndex, isToggleTariff);
                }
                imgTarifToggle.SetImageResource(Resource.Drawable.eye_disable);
                tarifToggle.SetBackgroundResource(Resource.Drawable.rectangle_white_outline_disable_rounded_button_bg);
                txtTarifToggle.SetTextColor(new Color(ContextCompat.GetColor(this.Activity, Resource.Color.white)));
                txtTarifToggle.Alpha = 0.7f;
                txtTarifToggle.Text = Utility.GetLocalizedLabel("Usage", "tariffBlock");
            }

            if (isSMAccount)
            {
                // Lin Siong Note: this is for smart meter Inner Dashboard
                // Lin Siong Note: the isStacked is to determine whether want to have spacing or not
                // Lin Siong Note: isStacked = true -> have spacing
                // Lin Siong Note: isStacked = false -> no spacing

                missingReadingList = new List<bool>();
                dayViewMonthList = new List<string>();
                if (selectedSMHistoryData != null && selectedSMHistoryData.ByDay != null && selectedSMHistoryData.ByDay.Count > 0)
                {
                    foreach (SMUsageHistoryData.ByDayData DayData in selectedSMHistoryData.ByDay)
                    {
                        foreach (SMUsageHistoryData.ByDayData.DayData IndividualDayData in DayData.Days)
                        {
                            missingReadingList.Add(IndividualDayData.IsMissingReading);
                            dayViewMonthList.Add(IndividualDayData.Month);
                        }
                    }
                }

                if (ChartType == ChartType.Day && isZoomIn)
                {
                    List<bool> newMissingReadingList = new List<bool>();
                    List<string> newDayViewMonthList = new List<string>();

                    for (int j = 0; j < 4; j++)
                    {
                        newMissingReadingList.Add(false);
                        newDayViewMonthList.Add("");
                    }

                    for (int j = 0; j < missingReadingList.Count; j++)
                    {
                        newMissingReadingList.Add(missingReadingList[j]);
                        newDayViewMonthList.Add(dayViewMonthList[j]);
                    }


                    for (int j = 0; j < 4; j++)
                    {
                        newMissingReadingList.Add(false);
                        newDayViewMonthList.Add("");
                    }

                    missingReadingList = newMissingReadingList;
                    dayViewMonthList = newDayViewMonthList;
                }

                if (!isZoomIn)
                {
                    missingBitmap = Bitmap.CreateScaledBitmap(missingBitmap, (int)DPUtils.ConvertDPToPx(5f), (int)DPUtils.ConvertDPToPx(5f), false);
                }
                else
                {
                    missingBitmap = Bitmap.CreateScaledBitmap(missingBitmap, (int)DPUtils.ConvertDPToPx(13f), (int)DPUtils.ConvertDPToPx(13f), false);
                }

                if (isToggleTariff)
                {
                    smRenderer = new SMStackedBarChartRenderer(mChart, mChart.Animator, mChart.ViewPortHandler)
                    {
                        selectedSMHistoryData = selectedSMHistoryData,
                        currentActivity = Activity,
                        isStacked = true,
                        isZoomIn = isZoomIn,
                        currentChartType = ChartType,
                        currentChartDataType = ChartDataType,
                        missingReadingList = missingReadingList,
                        isMDMSDown = isMDMSDown,
                        mdmsBitmap = mdmsBitmap,
                        dpcBitmap = dpcBitmap,
                        missingBitmap = missingBitmap
                    };
                    mChart.Renderer = smRenderer;
                }
                else
                {
                    smRenderer = new SMStackedBarChartRenderer(mChart, mChart.Animator, mChart.ViewPortHandler)
                    {
                        selectedSMHistoryData = selectedSMHistoryData,
                        currentActivity = Activity,
                        isStacked = false,
                        isZoomIn = isZoomIn,
                        currentChartType = ChartType,
                        currentChartDataType = ChartDataType,
                        missingReadingList = missingReadingList,
                        isMDMSDown = isMDMSDown,
                        mdmsBitmap = mdmsBitmap,
                        dpcBitmap = dpcBitmap,
                        missingBitmap = missingBitmap
                    };
                    mChart.Renderer = smRenderer;
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
                        currentActivity = Activity,
                        dpcBitmap = dpcBitmap,
                        currentChartType = ChartType,
                        currentChartDataType = ChartDataType
                    };
                    mChart.Renderer = renderer;
                }
                else
                {
                    renderer = new StackedBarChartRenderer(mChart, mChart.Animator, mChart.ViewPortHandler)
                    {
                        selectedHistoryData = selectedHistoryData,
                        currentActivity = Activity,
                        dpcBitmap = dpcBitmap,
                        currentChartType = ChartType,
                        currentChartDataType = ChartDataType
                    };
                    mChart.Renderer = renderer;
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
                if (!isShowAnimationDisable)
                {
                    mChart.AnimateY(1000);
                }
                else
                {
                    isShowAnimationDisable = false;
                }
            }
            else if (ChartType == ChartType.Day)
            {
                if (!isZoomIn)
                {
                    mChart.SetDrawBarShadow(false);
                    mChart.SetDrawValueAboveBar(true);
                    mChart.Description.Enabled = false;
                    mChart.SetMaxVisibleValueCount(32);
                    mChart.SetVisibleXRangeMaximum(32);
                    mChart.SetPinchZoom(false);
                    mChart.SetDrawGridBackground(false);
                    mChart.SetScaleEnabled(false);
                    mChart.Legend.Enabled = false;
                    if (!isShowAnimationDisable)
                    {
                        mChart.AnimateY(1000);
                    }
                    else
                    {
                        isShowAnimationDisable = false;
                    }
                }
                else
                {
                    mChart.SetDrawBarShadow(false);
                    mChart.SetDrawValueAboveBar(true);
                    mChart.Description.Enabled = false;
                    mChart.SetMaxVisibleValueCount(42);
                    mChart.SetVisibleXRangeMaximum(42);
                    mChart.SetPinchZoom(false);
                    mChart.SetDrawGridBackground(false);
                    mChart.SetScaleEnabled(false);
                    mChart.Legend.Enabled = false;
                    if (!isShowAnimationDisable)
                    {
                        mChart.AnimateY(1000);
                    }
                    else
                    {
                        isShowAnimationDisable = false;
                    }
                }
            }

            //txtAddress.Text = selectedAccount.AddStreet;

            //if not owner mask the address IRUL
            if (!selectedAccount.IsOwner == true)
            {
                if (!selectedAccount.IsHaveAccess == true)
                {
                    txtAddress.Text = Utility.StringSpaceMasking(Utility.Masking.Address, selectedAccount.AddStreet);
                }
                else
                {
                    txtAddress.Text = selectedAccount.AddStreet;
                }
            }
            else
            {
                txtAddress.Text = selectedAccount.AddStreet;
            }


            mdmsDayViewDownLayout.Visibility = ViewStates.Gone;
            mChart.Visibility = ViewStates.Visible;
            smDayViewZoomInIndicatorLayout.Visibility = ViewStates.Gone;

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

                        mChart.SetVisibleXRangeMinimum(selectedSMHistoryData.ByMonth.Months.Count);
                        mChart.SetVisibleXRangeMaximum(selectedSMHistoryData.ByMonth.Months.Count);
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

                        mChart.SetVisibleXRangeMinimum(selectedSMHistoryData.ByMonth.Months.Count);
                        mChart.SetVisibleXRangeMaximum(selectedSMHistoryData.ByMonth.Months.Count);
                    }
                }
                else if (ChartType == ChartType.Day)
                {
                    if (isMDMSDown)
                    {
                        txtRange.Visibility = ViewStates.Gone;
                        mdmsDayViewDownLayout.Visibility = ViewStates.Visible;
                        mChart.Visibility = ViewStates.Gone;
                        smGraphZoomToggleLayout.Enabled = false;
                    }
                    else
                    {
                        if (isZoomIn)
                        {
                            smDayViewZoomInIndicatorLayout.Visibility = ViewStates.Visible;
                        }

                        smGraphZoomToggleLayout.Enabled = true;
                        if (ChartDataType == ChartDataType.RM)
                        {
                            DayViewkWhData = new List<double>();
                            DayViewRMData = new List<double>();
                            if (selectedSMHistoryData != null && selectedSMHistoryData.ByDay != null && selectedSMHistoryData.ByDay.Count > 0)
                            {
                                foreach (SMUsageHistoryData.ByDayData DayData in selectedSMHistoryData.ByDay)
                                {
                                    foreach (SMUsageHistoryData.ByDayData.DayData IndividualDayData in DayData.Days)
                                    {
                                        DayViewRMData.Add(IndividualDayData.Amount);
                                        DayViewkWhData.Add(IndividualDayData.Consumption);
                                    }
                                }
                            }

                            txtRange.Text = selectedSMHistoryData.DateRange;

                            // SETUP XAXIS

                            SetUpXAxis();

                            // SETUP YAXIS

                            SetUpYAxis();

                            // ADD DATA
                            SetData(DayViewRMData.Count);

                            // SETUP MARKER VIEW

                            SetUpMarkerRMView();

                            if (isZoomIn)
                            {
                                mChart.SetVisibleXRangeMinimum(9);
                                mChart.SetVisibleXRangeMaximum(9);
                            }
                            else
                            {
                                mChart.SetVisibleXRangeMinimum(DayViewRMData.Count);
                                mChart.SetVisibleXRangeMaximum(DayViewRMData.Count);
                            }
                        }
                        else
                        {
                            DayViewkWhData = new List<double>();
                            DayViewRMData = new List<double>();
                            if (selectedSMHistoryData != null && selectedSMHistoryData.ByDay != null && selectedSMHistoryData.ByDay.Count > 0)
                            {
                                foreach (SMUsageHistoryData.ByDayData DayData in selectedSMHistoryData.ByDay)
                                {
                                    foreach (SMUsageHistoryData.ByDayData.DayData IndividualDayData in DayData.Days)
                                    {
                                        DayViewRMData.Add(IndividualDayData.Amount);
                                        DayViewkWhData.Add(IndividualDayData.Consumption);
                                    }
                                }
                            }

                            txtRange.Text = selectedSMHistoryData.DateRange;
                            // SETUP XAXIS

                            SetUpXAxiskWh();

                            // SETUP YAXIS

                            SetUpYAxisKwh();

                            // ADD DATA
                            SetKWhData(DayViewkWhData.Count);

                            // SETUP MARKER VIEW

                            SetUpMarkerKWhView();

                            if (isZoomIn)
                            {
                                mChart.SetVisibleXRangeMinimum(9);
                                mChart.SetVisibleXRangeMaximum(9);
                            }
                            else
                            {
                                mChart.SetVisibleXRangeMinimum(DayViewkWhData.Count);
                                mChart.SetVisibleXRangeMaximum(DayViewkWhData.Count);
                            }
                        }
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

                        mChart.SetVisibleXRangeMinimum(selectedHistoryData.ByMonth.Months.Count);
                        mChart.SetVisibleXRangeMaximum(selectedHistoryData.ByMonth.Months.Count);
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

                        mChart.SetVisibleXRangeMinimum(selectedHistoryData.ByMonth.Months.Count);
                        mChart.SetVisibleXRangeMaximum(selectedHistoryData.ByMonth.Months.Count);
                    }
                }
            }

            int graphTopPadding = 30;
            int graphLeftRightPadding = (int)DPUtils.ConvertDPToPx(4f);
            int graphBottomPadding = 10;
            if (selectedAccount.AccountCategoryId.Equals("2"))
            {
                graphTopPadding = 40;
                mChart.LayoutParameters.Height = (int)DPUtils.ConvertDPToPx(230f);
            }
            else if (ChartType == ChartType.Day && isZoomIn)
            {
                graphBottomPadding = 6;
                mChart.LayoutParameters.Height = (int)DPUtils.ConvertDPToPx(216f);
            }
            else
            {
                mChart.LayoutParameters.Height = (int)DPUtils.ConvertDPToPx(240f);
            }

            mChart.SetExtraOffsets(graphLeftRightPadding, graphTopPadding, graphLeftRightPadding, graphBottomPadding);

            mChart.SetOnChartValueSelectedListener(this);
            mScaleDetector = new ScaleGestureDetector(this.Activity, new BarGraphPinchListener(ChartType, this.userActionsListener));
            mChart.SetOnTouchListener(this);

            mChart.OnTouchListener = new OnBarChartTouchLister(mChart, mChart.Matrix, 3)
            {
                currentChartType = ChartType,
                currentChartDataType = ChartDataType,
                currentDayViewkWhList = DayViewkWhData,
                currentDayViewRMList = DayViewRMData,
                currentIsZoomIn = isZoomIn,
                currentFragment = this,
                currentActivity = this.Activity
            };

            mChart.NestedScrollingEnabled = true;

            isChangeVirtualHeightNeed = true;
            SetVirtualHeightParams(6f);
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
                    if (!isZoomIn)
                    {
                        for (int i = 0; i < DayViewRMData.Count; i++)
                        {
                            if (DayViewRMData.Count > 20 && i == 1)
                            {
                                if (!string.IsNullOrEmpty(selectedSMHistoryData.StartDate))
                                {
                                    DayViewLabel.Add(selectedSMHistoryData.StartDate);
                                }
                                else
                                {
                                    DayViewLabel.Add("");
                                }
                            }
                            else if (DayViewRMData.Count > 20 && (i == DayViewRMData.Count - 2))
                            {
                                if (!string.IsNullOrEmpty(selectedSMHistoryData.EndDate))
                                {
                                    DayViewLabel.Add(selectedSMHistoryData.EndDate);
                                }
                                else
                                {
                                    DayViewLabel.Add("");
                                }
                            }
                            else if (DayViewRMData.Count <= 20 && i == 0)
                            {
                                if (!string.IsNullOrEmpty(selectedSMHistoryData.StartDate))
                                {
                                    DayViewLabel.Add(selectedSMHistoryData.StartDate);
                                }
                                else
                                {
                                    DayViewLabel.Add("");
                                }
                            }
                            else if (DayViewRMData.Count <= 20 && (i == DayViewRMData.Count - 1))
                            {
                                if (!string.IsNullOrEmpty(selectedSMHistoryData.EndDate))
                                {
                                    DayViewLabel.Add(selectedSMHistoryData.EndDate);
                                }
                                else
                                {
                                    DayViewLabel.Add("");
                                }
                            }
                            else if (i == (int)((DayViewRMData.Count - 1) / 2))
                            {
                                if (!string.IsNullOrEmpty(selectedSMHistoryData.MidDate))
                                {
                                    DayViewLabel.Add(selectedSMHistoryData.MidDate);
                                }
                                else
                                {
                                    DayViewLabel.Add("");
                                }
                            }
                            else
                            {
                                DayViewLabel.Add("");
                            }
                        }
                    }
                    else
                    {
                        if (selectedSMHistoryData != null && selectedSMHistoryData.ByDay != null && selectedSMHistoryData.ByDay.Count > 0)
                        {
                            foreach (SMUsageHistoryData.ByDayData DayData in selectedSMHistoryData.ByDay)
                            {
                                foreach (SMUsageHistoryData.ByDayData.DayData IndividualDayData in DayData.Days)
                                {
                                    DayViewLabel.Add(IndividualDayData.Day);
                                }
                            }
                        }

                        if (isZoomIn)
                        {
                            List<string> NewDayViewLabel = new List<string>();

                            for (int i = 0; i < 4; i++)
                            {
                                NewDayViewLabel.Add("");
                            }

                            for (int i = 0; i < DayViewLabel.Count; i++)
                            {
                                NewDayViewLabel.Add(DayViewLabel[i]);
                            }


                            for (int i = 0; i < 4; i++)
                            {
                                NewDayViewLabel.Add("");
                            }

                            DayViewLabel = NewDayViewLabel;
                        }
                    }

                    XLabelsFormatter = new SMChartsZoomInDayFormatter(DayViewLabel, mChart);
                }
            }
            else
            {
                XLabelsFormatter = new ChartsMonthFormatter(selectedHistoryData.ByMonth, mChart);
            }

            CustomXAxisRenderer xAxisRenderer = new CustomXAxisRenderer(mChart.ViewPortHandler, mChart.XAxis, mChart.GetTransformer(YAxis.AxisDependency.Left))
            {
                barChart = mChart,
                currentActivity = this.Activity,
                isZoomIn = isZoomIn,
                currentChartType = ChartType,
                currentChartDataType = ChartDataType
            };

            mChart.SetXAxisRenderer(xAxisRenderer);

            XAxis xAxis = mChart.XAxis;
            xAxis.Position = XAxisPosition.Bottom;
            xAxis.TextColor = Color.ParseColor("#ffffff");
            xAxis.AxisLineWidth = 2f;
            xAxis.AxisLineColor = Color.ParseColor("#4cffffff");
            xAxis.TextSize = 11f;

            xAxis.SetDrawGridLines(false);

            try
            {
                Typeface plain = Typeface.CreateFromAsset(this.Activity.Assets, "fonts/" + TextViewUtils.MuseoSans300);
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
                    if (!isZoomIn)
                    {
                        for (int i = 0; i < DayViewkWhData.Count; i++)
                        {
                            if (DayViewkWhData.Count > 20 && i == 1)
                            {
                                if (!string.IsNullOrEmpty(selectedSMHistoryData.StartDate))
                                {
                                    DayViewLabel.Add(selectedSMHistoryData.StartDate);
                                }
                                else
                                {
                                    DayViewLabel.Add("");
                                }
                            }
                            else if (DayViewkWhData.Count > 20 && (i == DayViewkWhData.Count - 2))
                            {
                                if (!string.IsNullOrEmpty(selectedSMHistoryData.EndDate))
                                {
                                    DayViewLabel.Add(selectedSMHistoryData.EndDate);
                                }
                                else
                                {
                                    DayViewLabel.Add("");
                                }
                            }
                            else if (DayViewkWhData.Count <= 20 && i == 0)
                            {
                                if (!string.IsNullOrEmpty(selectedSMHistoryData.StartDate))
                                {
                                    DayViewLabel.Add(selectedSMHistoryData.StartDate);
                                }
                                else
                                {
                                    DayViewLabel.Add("");
                                }
                            }
                            else if (DayViewkWhData.Count <= 20 && (i == DayViewkWhData.Count - 1))
                            {
                                if (!string.IsNullOrEmpty(selectedSMHistoryData.EndDate))
                                {
                                    DayViewLabel.Add(selectedSMHistoryData.EndDate);
                                }
                                else
                                {
                                    DayViewLabel.Add("");
                                }
                            }
                            else if (i == (int)((DayViewkWhData.Count - 1) / 2))
                            {
                                if (!string.IsNullOrEmpty(selectedSMHistoryData.MidDate))
                                {
                                    DayViewLabel.Add(selectedSMHistoryData.MidDate);
                                }
                                else
                                {
                                    DayViewLabel.Add("");
                                }
                            }
                            else
                            {
                                DayViewLabel.Add("");
                            }
                        }
                    }
                    else
                    {
                        if (selectedSMHistoryData != null && selectedSMHistoryData.ByDay != null && selectedSMHistoryData.ByDay.Count > 0)
                        {
                            foreach (SMUsageHistoryData.ByDayData DayData in selectedSMHistoryData.ByDay)
                            {
                                foreach (SMUsageHistoryData.ByDayData.DayData IndividualDayData in DayData.Days)
                                {
                                    DayViewLabel.Add(IndividualDayData.Day);
                                }
                            }
                        }

                        if (isZoomIn)
                        {
                            List<string> NewDayViewLabel = new List<string>();

                            for (int i = 0; i < 4; i++)
                            {
                                NewDayViewLabel.Add("");
                            }

                            for (int i = 0; i < DayViewLabel.Count; i++)
                            {
                                NewDayViewLabel.Add(DayViewLabel[i]);
                            }


                            for (int i = 0; i < 4; i++)
                            {
                                NewDayViewLabel.Add("");
                            }

                            DayViewLabel = NewDayViewLabel;
                        }
                    }

                    XLabelsFormatter = new SMChartsZoomInDayFormatter(DayViewLabel, mChart);
                }
            }
            else
            {
                XLabelsFormatter = new ChartsKWhFormatter(selectedHistoryData.ByMonth, mChart);
            }

            CustomXAxisRenderer xAxisRenderer = new CustomXAxisRenderer(mChart.ViewPortHandler, mChart.XAxis, mChart.GetTransformer(YAxis.AxisDependency.Left))
            {
                barChart = mChart,
                currentActivity = this.Activity,
                isZoomIn = isZoomIn,
                currentChartType = ChartType,
                currentChartDataType = ChartDataType
            };

            mChart.SetXAxisRenderer(xAxisRenderer);

            XAxis xAxis = mChart.XAxis;
            xAxis.Position = XAxisPosition.Bottom;
            xAxis.TextColor = Color.ParseColor("#ffffff");
            xAxis.AxisLineWidth = 2f;
            xAxis.AxisLineColor = Color.ParseColor("#4cffffff");
            xAxis.TextSize = 11f;

            xAxis.SetDrawGridLines(false);

            try
            {
                Typeface plain = Typeface.CreateFromAsset(this.Activity.Assets, "fonts/" + TextViewUtils.MuseoSans300);
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
                List<double> newDayViewCurrencyList = new List<double>();

                if (ChartType == ChartType.Day && isZoomIn)
                {

                    for (int i = 0; i < 4; i++)
                    {
                        newDayViewCurrencyList.Add(0);
                    }

                    for (int i = 0; i < DayViewRMData.Count; i++)
                    {
                        newDayViewCurrencyList.Add(DayViewRMData[i]);
                    }


                    for (int i = 0; i < 4; i++)
                    {
                        newDayViewCurrencyList.Add(0);
                    }
                }
                else
                {
                    newDayViewCurrencyList = DayViewRMData;
                }


                SMSelectedMarkerView markerView = new SMSelectedMarkerView(Activity)
                {
                    UsageHistoryData = selectedSMHistoryData,
                    ChartType = ChartType,
                    ChartDataType = ChartDataType,
                    isMDMSDown = isMDMSDown,
                    smDayViewCurrencyList = newDayViewCurrencyList,
                    smDayCurrencyUnit = "RM",
                    isZoomIn = isZoomIn,
                    smMissingList = missingReadingList,
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
                List<double> newDayViewUsageList = new List<double>();

                if (ChartType == ChartType.Day && isZoomIn)
                {

                    for (int i = 0; i < 4; i++)
                    {
                        newDayViewUsageList.Add(0);
                    }

                    for (int i = 0; i < DayViewkWhData.Count; i++)
                    {
                        newDayViewUsageList.Add(DayViewkWhData[i]);
                    }


                    for (int i = 0; i < 4; i++)
                    {
                        newDayViewUsageList.Add(0);
                    }
                }
                else
                {
                    newDayViewUsageList = DayViewkWhData;
                }


                SMSelectedMarkerView markerView = new SMSelectedMarkerView(Activity)
                {
                    UsageHistoryData = selectedSMHistoryData,
                    ChartType = ChartType,
                    ChartDataType = ChartDataType,
                    isMDMSDown = isMDMSDown,
                    smDayViewUsageList = newDayViewUsageList,
                    smDayUsageUnit = "kWh",
                    isZoomIn = isZoomIn,
                    smMissingList = missingReadingList,
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
                            if (selectedHistoryData.ByMonth.Months[i].DPCIndicator)
                            {
                                float[] valList = new float[1];

                                float val = (float)selectedHistoryData.ByMonth.Months[i].AmountTotal;
                                if (float.IsPositiveInfinity(val))
                                {
                                    val = float.PositiveInfinity;
                                }

                                if (val < 0)
                                {
                                    val = 0;
                                }

                                valList[0] = System.Math.Abs(val);
                                if (i == (CurrentParentIndex == -1 ? barLength - 1 : CurrentParentIndex))
                                {
                                    stackIndex = valList.Length - 1;
                                }
                                yVals1.Add(new BarEntry(i, valList));
                            }
                            else
                            {
                                List<float> newValList = new List<float>();

                                float valTotal = (float)selectedHistoryData.ByMonth.Months[i].AmountTotal;
                                if (valTotal > 0)
                                {
                                    if (selectedHistoryData.ByMonth.Months[i].TariffBlocksList != null && selectedHistoryData.ByMonth.Months[i].TariffBlocksList.Count > 0)
                                    {
                                        for (int j = 0; j < selectedHistoryData.ByMonth.Months[i].TariffBlocksList.Count; j++)
                                        {
                                            float val = (float)selectedHistoryData.ByMonth.Months[i].TariffBlocksList[j].Amount;
                                            if (float.IsPositiveInfinity(val))
                                            {
                                                val = float.PositiveInfinity;
                                            }
                                            if (System.Math.Abs(val) > 0)
                                            {
                                                newValList.Add(System.Math.Abs(val));
                                            }
                                        }
                                    }

                                    if (newValList.Count == 0)
                                    {
                                        float val = (float)selectedHistoryData.ByMonth.Months[i].AmountTotal;
                                        if (float.IsPositiveInfinity(val))
                                        {
                                            val = float.PositiveInfinity;
                                        }
                                        if (System.Math.Abs(val) > 0)
                                        {
                                            newValList.Add(System.Math.Abs(val));
                                        }
                                    }
                                }

                                if (newValList.Count > 0)
                                {
                                    float[] valList = new float[newValList.Count];
                                    for (int j = 0; j < newValList.Count; j++)
                                    {
                                        valList[j] = newValList[j];
                                    }

                                    if (i == (CurrentParentIndex == -1 ? barLength - 1 : CurrentParentIndex))
                                    {
                                        stackIndex = valList.Length - 1;
                                    }

                                    yVals1.Add(new BarEntry(i, valList));
                                }
                                else
                                {
                                    float[] valList = new float[1];
                                    valList[0] = 0f;

                                    if (i == (CurrentParentIndex == -1 ? barLength - 1 : CurrentParentIndex))
                                    {
                                        stackIndex = valList.Length - 1;
                                    }

                                    yVals1.Add(new BarEntry(i, valList));
                                }
                            }
                        }
                        else
                        {
                            float[] valList = new float[1];
                            valList[0] = 0f;

                            if (i == (CurrentParentIndex == -1 ? barLength - 1 : CurrentParentIndex))
                            {
                                stackIndex = valList.Length - 1;
                            }

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
                                if (selectedHistoryData.ByMonth.Months[i].DPCIndicator || (float)selectedHistoryData.ByMonth.Months[i].AmountTotal <= 0.00)
                                {
                                    listOfColor.Add(Color.Argb(50, 255, 255, 255));
                                }
                                else
                                {
                                    bool isSetColor = false;
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

                                                    float val = (float)selectedHistoryData.ByMonth.Months[i].TariffBlocksList[j].Amount;
                                                    if (float.IsPositiveInfinity(val))
                                                    {
                                                        val = float.PositiveInfinity;
                                                    }
                                                    if (System.Math.Abs(val) > 0)
                                                    {
                                                        isSetColor = true;
                                                        listOfColor.Add(Color.Argb(50, selectedHistoryData.TariffBlocksLegend[k].Color.RedColor, selectedHistoryData.TariffBlocksLegend[k].Color.GreenColor, selectedHistoryData.TariffBlocksLegend[k].Color.BlueData));
                                                    }
                                                    else if (selectedHistoryData.ByMonth.Months[i].TariffBlocksList.Count == 1)
                                                    {
                                                        isSetColor = true;
                                                        listOfColor.Add(Color.Argb(50, 255, 255, 255));
                                                    }
                                                    break;
                                                }
                                            }

                                            if (!isFound)
                                            {
                                                isSetColor = true;
                                                listOfColor.Add(Color.Argb(50, 255, 255, 255));
                                            }

                                        }
                                        else
                                        {
                                            isSetColor = true;
                                            listOfColor.Add(Color.Argb(50, 255, 255, 255));
                                        }
                                    }

                                    if (!isSetColor)
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
                    if (CurrentParentIndex == -1)
                    {
                        CurrentParentIndex = barLength - 1;
                    }
                    Highlight rightMostBar = new Highlight(CurrentParentIndex, 0, stackIndex);
                    mChart.HighlightValues(new Highlight[] { rightMostBar });
                }
                else
                {
                    List<BarEntry> yVals1 = new List<BarEntry>();
                    for (int i = 0; i < barLength; i++)
                    {
                        float[] valList = new float[1];
                        float val = (float)selectedHistoryData.ByMonth.Months[i].AmountTotal;
                        if (!isREAccount)
                        {
                            if (!isREAccount && selectedHistoryData.ByMonth.Months[i].DPCIndicator)
                            {
                                val = (float)selectedHistoryData.ByMonth.Months[i].AmountTotal;
                            }

                            if (float.IsPositiveInfinity(val))
                            {
                                val = float.PositiveInfinity;
                            }

                            if (!isREAccount && selectedHistoryData.ByMonth.Months[i].DPCIndicator && val < 0)
                            {
                                val = 0;
                            }
                            else if (!isREAccount && (float)selectedHistoryData.ByMonth.Months[i].AmountTotal <= 0.00)
                            {
                                val = 0;
                            }
                        }
                        else
                        {
                            val = (float)selectedHistoryData.ByMonth.Months[i].UsageTotal;
                            if (float.IsPositiveInfinity(val))
                            {
                                val = float.PositiveInfinity;
                            }
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
                    if (CurrentParentIndex == -1)
                    {
                        CurrentParentIndex = barLength - 1;
                    }
                    Highlight rightMostBar = new Highlight(CurrentParentIndex, 0, 0);
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
                    if (ChartType == ChartType.Month)
                    {
                        for (int i = 0; i < barLength; i++)
                        {
                            if (i == barLength - 1 && GetIsMDMSDown())
                            {
                                float[] valList = new float[1];
                                valList[0] = 0f;
                                if (i == (CurrentParentIndex == -1 ? barLength - 1 : CurrentParentIndex))
                                {
                                    stackIndex = valList.Length - 1;
                                }
                                yVals1.Add(new BarEntry(i, valList));
                            }
                            else if (selectedSMHistoryData.ByMonth.Months[i].TariffBlocksList != null && selectedSMHistoryData.ByMonth.Months[i].TariffBlocksList.Count > 0)
                            {
                                if (selectedSMHistoryData.ByMonth.Months[i].DPCIndicator)
                                {
                                    float[] valList = new float[1];

                                    float val = (float)selectedSMHistoryData.ByMonth.Months[i].AmountTotal;
                                    if (float.IsPositiveInfinity(val))
                                    {
                                        val = float.PositiveInfinity;
                                    }

                                    if (val < 0)
                                    {
                                        val = 0;
                                    }

                                    valList[0] = System.Math.Abs(val);
                                    if (i == (CurrentParentIndex == -1 ? barLength - 1 : CurrentParentIndex))
                                    {
                                        stackIndex = valList.Length - 1;
                                    }
                                    yVals1.Add(new BarEntry(i, valList));
                                }
                                else
                                {
                                    List<float> newValList = new List<float>();

                                    float valTotal = (float)selectedSMHistoryData.ByMonth.Months[i].AmountTotal;

                                    if (valTotal > 0)
                                    {
                                        if (selectedSMHistoryData.ByMonth.Months[i].TariffBlocksList != null
                                            && selectedSMHistoryData.ByMonth.Months[i].TariffBlocksList.Count > 0)
                                        {
                                            for (int j = 0; j < selectedSMHistoryData.ByMonth.Months[i].TariffBlocksList.Count; j++)
                                            {
                                                float val = (float)selectedSMHistoryData.ByMonth.Months[i].TariffBlocksList[j].Amount;
                                                if (float.IsPositiveInfinity(val))
                                                {
                                                    val = float.PositiveInfinity;
                                                }

                                                if (System.Math.Abs(val) > 0)
                                                {
                                                    newValList.Add(System.Math.Abs(val));
                                                }
                                            }
                                        }

                                        if (newValList.Count == 0)
                                        {
                                            float val = (float)selectedSMHistoryData.ByMonth.Months[i].AmountTotal;
                                            if (float.IsPositiveInfinity(val))
                                            {
                                                val = float.PositiveInfinity;
                                            }

                                            if (System.Math.Abs(val) > 0)
                                            {
                                                newValList.Add(System.Math.Abs(val));
                                            }
                                        }
                                    }

                                    if (newValList.Count > 0)
                                    {
                                        float[] valList = new float[newValList.Count];

                                        for (int j = 0; j < newValList.Count; j++)
                                        {
                                            valList[j] = newValList[j];
                                        }

                                        if (i == (CurrentParentIndex == -1 ? barLength - 1 : CurrentParentIndex))
                                        {
                                            stackIndex = valList.Length - 1;
                                        }

                                        yVals1.Add(new BarEntry(i, valList));
                                    }
                                    else
                                    {
                                        float[] valList = new float[1];
                                        valList[0] = 0f;
                                        if (i == (CurrentParentIndex == -1 ? barLength - 1 : CurrentParentIndex))
                                        {
                                            stackIndex = valList.Length - 1;
                                        }
                                        yVals1.Add(new BarEntry(i, valList));
                                    }
                                }
                            }
                            else
                            {
                                float[] valList = new float[1];
                                valList[0] = 0f;
                                if (i == (CurrentParentIndex == -1 ? barLength - 1 : CurrentParentIndex))
                                {
                                    stackIndex = valList.Length - 1;
                                }
                                yVals1.Add(new BarEntry(i, valList));
                            }
                        }
                    }
                    else if (ChartType == ChartType.Day)
                    {
                        int i = 0;
                        if (selectedSMHistoryData != null && selectedSMHistoryData.ByDay != null && selectedSMHistoryData.ByDay.Count > 0)
                        {
                            foreach (SMUsageHistoryData.ByDayData DayData in selectedSMHistoryData.ByDay)
                            {
                                if (DayData.Days != null && DayData.Days.Count > 0)
                                {
                                    foreach (SMUsageHistoryData.ByDayData.DayData IndividualDayData in DayData.Days)
                                    {
                                        if (IndividualDayData.TariffBlocksList != null && IndividualDayData.TariffBlocksList.Count > 0)
                                        {
                                            List<float> newValList = new List<float>();
                                            for (int j = 0; j < IndividualDayData.TariffBlocksList.Count; j++)
                                            {
                                                float val = (float)IndividualDayData.TariffBlocksList[j].Amount;
                                                if (float.IsPositiveInfinity(val))
                                                {
                                                    val = float.PositiveInfinity;
                                                }

                                                if (System.Math.Abs(val) > 0)
                                                {
                                                    newValList.Add(System.Math.Abs(val));
                                                }
                                            }

                                            if (newValList.Count > 0)
                                            {
                                                float[] valList = new float[newValList.Count];

                                                for (int z = 0; z < newValList.Count; z++)
                                                {
                                                    valList[z] = newValList[z];
                                                }

                                                yVals1.Add(new BarEntry(i, valList));
                                                if (i == barLength - 1)
                                                {
                                                    stackIndex = valList.Length - 1;
                                                }
                                            }
                                            else
                                            {
                                                float[] valList = new float[1];
                                                valList[0] = 0f;
                                                if (i == barLength - 1)
                                                {
                                                    stackIndex = valList.Length - 1;
                                                }
                                                yVals1.Add(new BarEntry(i, valList));
                                            }
                                        }
                                        else
                                        {
                                            float[] valList = new float[1];
                                            valList[0] = 0f;
                                            if (i == barLength - 1)
                                            {
                                                stackIndex = valList.Length - 1;
                                            }
                                            yVals1.Add(new BarEntry(i, valList));
                                        }
                                        i++;
                                    }
                                }
                                else
                                {
                                    float[] valList = new float[1];
                                    valList[0] = 0f;
                                    if (i == barLength - 1)
                                    {
                                        stackIndex = valList.Length - 1;
                                    }
                                    yVals1.Add(new BarEntry(i, valList));
                                    i++;
                                }
                            }
                        }
                        else
                        {
                            float[] valList = new float[1];
                            valList[0] = 0f;
                            if (i == barLength - 1)
                            {
                                stackIndex = valList.Length - 1;
                            }
                            yVals1.Add(new BarEntry(i, valList));
                        }
                    }

                    if (ChartType == ChartType.Day && isZoomIn)
                    {
                        List<BarEntry> yValNew = new List<BarEntry>();

                        for (int j = 0; j < 4; j++)
                        {
                            float[] valList = new float[1];
                            valList[0] = 0f;
                            yValNew.Add(new BarEntry(j, valList));
                        }

                        for (int j = 0; j < yVals1.Count; j++)
                        {
                            yValNew.Add(new BarEntry(j + 4, yVals1[j].GetYVals()));
                        }


                        for (int j = 0; j < 4; j++)
                        {
                            float[] valList = new float[1];
                            valList[0] = 0f;
                            yValNew.Add(new BarEntry(j + 4 + yVals1.Count, valList));
                        }

                        yVals1 = yValNew;

                        dayViewTariffList = yVals1;
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
                        if (ChartType == ChartType.Month)
                        {
                            for (int i = 0; i < barLength; i++)
                            {
                                if (i == barLength - 1 && GetIsMDMSDown())
                                {
                                    listOfColor.Add(Color.Argb(50, 255, 255, 255));
                                }
                                else if (selectedSMHistoryData.ByMonth.Months[i].TariffBlocksList != null
                                    && selectedSMHistoryData.ByMonth.Months[i].TariffBlocksList.Count > 0)
                                {
                                    if (selectedSMHistoryData.ByMonth.Months[i].DPCIndicator || (float)selectedSMHistoryData.ByMonth.Months[i].AmountTotal <= 0.00)
                                    {
                                        listOfColor.Add(Color.Argb(50, 255, 255, 255));
                                    }
                                    else
                                    {
                                        bool isSetColor = false;

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

                                                        float val = (float)selectedSMHistoryData.ByMonth.Months[i].TariffBlocksList[j].Amount;
                                                        if (float.IsPositiveInfinity(val))
                                                        {
                                                            val = float.PositiveInfinity;
                                                        }

                                                        if (System.Math.Abs(val) > 0)
                                                        {
                                                            isSetColor = true;
                                                            listOfColor.Add(Color.Argb(50, selectedSMHistoryData.TariffBlocksLegend[k].Color.RedColor, selectedSMHistoryData.TariffBlocksLegend[k].Color.GreenColor, selectedSMHistoryData.TariffBlocksLegend[k].Color.BlueData));
                                                        }
                                                        else if (selectedSMHistoryData.ByMonth.Months[i].TariffBlocksList.Count == 1)
                                                        {
                                                            isSetColor = true;
                                                            listOfColor.Add(Color.Argb(50, 255, 255, 255));
                                                        }
                                                        break;
                                                    }
                                                }

                                                if (!isFound)
                                                {
                                                    isSetColor = true;
                                                    listOfColor.Add(Color.Argb(50, 255, 255, 255));
                                                }

                                            }
                                            else
                                            {
                                                isSetColor = true;
                                                listOfColor.Add(Color.Argb(50, 255, 255, 255));
                                            }
                                        }

                                        if (!isSetColor)
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
                        else if (ChartType == ChartType.Day)
                        {
                            if (selectedSMHistoryData != null && selectedSMHistoryData.ByDay != null && selectedSMHistoryData.ByDay.Count > 0)
                            {
                                foreach (SMUsageHistoryData.ByDayData DayData in selectedSMHistoryData.ByDay)
                                {
                                    if (DayData.Days != null && DayData.Days.Count > 0)
                                    {
                                        foreach (SMUsageHistoryData.ByDayData.DayData IndividualDayData in DayData.Days)
                                        {
                                            if (IndividualDayData.TariffBlocksList != null && IndividualDayData.TariffBlocksList.Count > 0)
                                            {
                                                bool isSetColor = false;

                                                for (int j = 0; j < IndividualDayData.TariffBlocksList.Count; j++)
                                                {
                                                    if (selectedSMHistoryData.TariffBlocksLegend != null && selectedSMHistoryData.TariffBlocksLegend.Count > 0)
                                                    {
                                                        bool isFound = false;
                                                        for (int k = 0; k < selectedSMHistoryData.TariffBlocksLegend.Count; k++)
                                                        {
                                                            if (IndividualDayData.TariffBlocksList[j].BlockId == selectedSMHistoryData.TariffBlocksLegend[k].BlockId)
                                                            {
                                                                isFound = true;

                                                                float val = (float)IndividualDayData.TariffBlocksList[j].Amount;
                                                                if (float.IsPositiveInfinity(val))
                                                                {
                                                                    val = float.PositiveInfinity;
                                                                }

                                                                if (System.Math.Abs(val) > 0)
                                                                {
                                                                    isSetColor = true;
                                                                    listOfColor.Add(Color.Argb(50, selectedSMHistoryData.TariffBlocksLegend[k].Color.RedColor, selectedSMHistoryData.TariffBlocksLegend[k].Color.GreenColor, selectedSMHistoryData.TariffBlocksLegend[k].Color.BlueData));
                                                                }
                                                                else if (IndividualDayData.TariffBlocksList.Count == 1)
                                                                {
                                                                    isSetColor = true;
                                                                    listOfColor.Add(Color.Argb(50, 255, 255, 255));
                                                                }
                                                                break;
                                                            }
                                                        }

                                                        if (!isFound)
                                                        {
                                                            isSetColor = true;
                                                            listOfColor.Add(Color.Argb(50, 255, 255, 255));
                                                        }

                                                    }
                                                    else
                                                    {
                                                        isSetColor = true;
                                                        listOfColor.Add(Color.Argb(50, 255, 255, 255));
                                                    }
                                                }

                                                if (!isSetColor)
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

                            if (isZoomIn)
                            {
                                List<int> listOfColorNew = new List<int>();

                                for (int i = 0; i < 4; i++)
                                {
                                    listOfColorNew.Add(Color.Argb(50, 255, 255, 255));
                                }

                                for (int i = 0; i < listOfColor.Count; i++)
                                {
                                    listOfColorNew.Add(listOfColor[i]);
                                }


                                for (int i = 0; i < 4; i++)
                                {
                                    listOfColorNew.Add(Color.Argb(50, 255, 255, 255));
                                }

                                listOfColor = listOfColorNew;
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

                        if (ChartType == ChartType.Month)
                        {
                            data.BarWidth = 0.25f;
                        }
                        else if (ChartType == ChartType.Day)
                        {
                            if (!isZoomIn)
                            {
                                data.BarWidth = 0.60f;
                            }
                            else
                            {
                                data.BarWidth = 0.35f;
                            }
                        }

                        set1.HighLightAlpha = 0;

                        data.HighlightEnabled = true;
                        data.SetValueTextSize(10f);
                        data.SetDrawValues(false);

                        mChart.Data = data;
                    }

                    if (ChartType == ChartType.Month)
                    {
                        // HIGHLIGHT RIGHT MOST ITEM
                        if (CurrentParentIndex == -1)
                        {
                            CurrentParentIndex = barLength - 1;
                        }
                        Highlight rightMostBar = new Highlight(CurrentParentIndex, 0, stackIndex);
                        mChart.HighlightValues(new Highlight[] { rightMostBar });
                    }
                    else if (ChartType == ChartType.Day)
                    {

                    }
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
                            if (i == barLength - 1 && GetIsMDMSDown())
                            {
                                val = 0;
                            }
                            else
                            {
                                if (selectedSMHistoryData.ByMonth.Months[i].DPCIndicator)
                                {
                                    val = (float)selectedSMHistoryData.ByMonth.Months[i].AmountTotal;
                                }

                                if (float.IsPositiveInfinity(val))
                                {
                                    val = float.PositiveInfinity;
                                }

                                if (selectedSMHistoryData.ByMonth.Months[i].DPCIndicator && val < 0)
                                {
                                    val = 0;
                                }
                                else if ((float)selectedSMHistoryData.ByMonth.Months[i].AmountTotal <= 0.00)
                                {
                                    val = 0;
                                }
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

                    if (ChartType == ChartType.Day && isZoomIn)
                    {
                        List<BarEntry> yValNew = new List<BarEntry>();

                        for (int j = 0; j < 4; j++)
                        {
                            float[] valLis = new float[1];
                            valLis[0] = 0f;
                            yValNew.Add(new BarEntry(j, valLis));
                        }

                        for (int j = 0; j < yVals1.Count; j++)
                        {
                            yValNew.Add(new BarEntry(j + 4, yVals1[j].GetYVals()));
                        }


                        for (int j = 0; j < 4; j++)
                        {
                            float[] valLis = new float[1];
                            valLis[0] = 0f;
                            yValNew.Add(new BarEntry(j + 4 + yVals1.Count, valLis));
                        }

                        yVals1 = yValNew;

                        dayViewTariffList = yVals1;
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
                            if (ChartType == ChartType.Month)
                            {
                                listOfColor.Add(Color.Argb(50, 255, 255, 255));
                            }
                            else if (ChartType == ChartType.Day)
                            {
                                listOfColor.Add(Color.Argb(50, 255, 255, 255));
                            }
                        }

                        if (ChartType == ChartType.Day && isZoomIn)
                        {
                            List<int> listOfColorNew = new List<int>();

                            for (int i = 0; i < 4; i++)
                            {
                                listOfColorNew.Add(Color.Argb(50, 255, 255, 255));
                            }

                            for (int i = 0; i < listOfColor.Count; i++)
                            {
                                listOfColorNew.Add(listOfColor[i]);
                            }


                            for (int i = 0; i < 4; i++)
                            {
                                listOfColorNew.Add(Color.Argb(50, 255, 255, 255));
                            }

                            listOfColor = listOfColorNew;
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

                        if (ChartType == ChartType.Month)
                        {
                            data.BarWidth = 0.25f;
                        }
                        else if (ChartType == ChartType.Day)
                        {
                            if (!isZoomIn)
                            {
                                data.BarWidth = 0.60f;
                            }
                            else
                            {
                                data.BarWidth = 0.35f;
                            }
                        }

                        set1.HighLightAlpha = 0;

                        data.HighlightEnabled = true;
                        data.SetValueTextSize(10f);
                        data.SetDrawValues(false);

                        mChart.Data = data;
                    }

                    if (ChartType == ChartType.Month)
                    {
                        // HIGHLIGHT RIGHT MOST ITEM
                        if (CurrentParentIndex == -1)
                        {
                            CurrentParentIndex = barLength - 1;
                        }
                        Highlight rightMostBar = new Highlight(CurrentParentIndex, 0, 0);
                        mChart.HighlightValues(new Highlight[] { rightMostBar });
                    }
                    else if (ChartType == ChartType.Day)
                    {

                    }
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
                        if (selectedHistoryData.ByMonth.Months[i].TariffBlocksList != null
                            && selectedHistoryData.ByMonth.Months[i].TariffBlocksList.Count > 0
                            && !selectedHistoryData.ByMonth.Months[i].DPCIndicator)
                        {
                            List<float> newValList = new List<float>();

                            if (selectedHistoryData.ByMonth.Months[i].TariffBlocksList != null && selectedHistoryData.ByMonth.Months[i].TariffBlocksList.Count > 0)
                            {
                                for (int j = 0; j < selectedHistoryData.ByMonth.Months[i].TariffBlocksList.Count; j++)
                                {
                                    float val = (float)selectedHistoryData.ByMonth.Months[i].TariffBlocksList[j].Usage;
                                    if (float.IsPositiveInfinity(val))
                                    {
                                        val = float.PositiveInfinity;
                                    }

                                    if (System.Math.Abs(val) > 0)
                                    {
                                        newValList.Add(System.Math.Abs(val));
                                    }
                                }
                            }

                            if (newValList.Count > 0)
                            {
                                float[] valList = new float[newValList.Count];

                                for (int j = 0; j < newValList.Count; j++)
                                {
                                    valList[j] = newValList[j];
                                }

                                if (i == (CurrentParentIndex == -1 ? barLength - 1 : CurrentParentIndex))
                                {
                                    stackIndex = valList.Length - 1;
                                }

                                yVals1.Add(new BarEntry(i, valList));
                            }
                            else
                            {
                                float[] valList = new float[1];
                                float val = (float)selectedHistoryData.ByMonth.Months[i].UsageTotal;
                                if (float.IsPositiveInfinity(val))
                                {
                                    val = float.PositiveInfinity;
                                }

                                valList[0] = System.Math.Abs(val);

                                if (i == (CurrentParentIndex == -1 ? barLength - 1 : CurrentParentIndex))
                                {
                                    stackIndex = valList.Length - 1;
                                }

                                yVals1.Add(new BarEntry(i, valList));
                            }
                        }
                        else
                        {
                            float[] valList = new float[1];
                            valList[0] = 0f;

                            if (i == (CurrentParentIndex == -1 ? barLength - 1 : CurrentParentIndex))
                            {
                                stackIndex = valList.Length - 1;
                            }

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
                            if (selectedHistoryData.ByMonth.Months[i].TariffBlocksList != null
                                && selectedHistoryData.ByMonth.Months[i].TariffBlocksList.Count > 0
                                && !selectedHistoryData.ByMonth.Months[i].DPCIndicator)
                            {
                                bool isSetColor = false;

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

                                                float val = (float)selectedHistoryData.ByMonth.Months[i].TariffBlocksList[j].Usage;
                                                if (float.IsPositiveInfinity(val))
                                                {
                                                    val = float.PositiveInfinity;
                                                }

                                                if (System.Math.Abs(val) > 0)
                                                {
                                                    isSetColor = true;
                                                    listOfColor.Add(Color.Argb(50, selectedHistoryData.TariffBlocksLegend[k].Color.RedColor, selectedHistoryData.TariffBlocksLegend[k].Color.GreenColor, selectedHistoryData.TariffBlocksLegend[k].Color.BlueData));
                                                }
                                                else if (selectedHistoryData.ByMonth.Months[i].TariffBlocksList.Count == 1)
                                                {
                                                    isSetColor = true;
                                                    listOfColor.Add(Color.Argb(50, 255, 255, 255));
                                                }
                                                break;
                                            }
                                        }

                                        if (!isFound)
                                        {
                                            isSetColor = true;
                                            listOfColor.Add(Color.Argb(50, 255, 255, 255));
                                        }

                                    }
                                    else
                                    {
                                        isSetColor = true;
                                        listOfColor.Add(Color.Argb(50, 255, 255, 255));
                                    }
                                }

                                if (!isSetColor)
                                {
                                    listOfColor.Add(Color.Argb(50, 255, 255, 255));
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
                    if (CurrentParentIndex == -1)
                    {
                        CurrentParentIndex = barLength - 1;
                    }
                    Highlight rightMostBar = new Highlight(CurrentParentIndex, 0, stackIndex);
                    mChart.HighlightValues(new Highlight[] { rightMostBar });
                }
                else
                {

                    List<BarEntry> yVals1 = new List<BarEntry>();
                    for (int i = 0; i < barLength; i++)
                    {
                        float[] valList = new float[1];
                        float val = (float)selectedHistoryData.ByMonth.Months[i].UsageTotal;

                        if (selectedHistoryData.ByMonth.Months[i].DPCIndicator)
                        {
                            val = 0;
                        }
                        else
                        {
                            if (float.IsPositiveInfinity(val))
                            {
                                val = float.PositiveInfinity;
                            }
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
                    if (CurrentParentIndex == -1)
                    {
                        CurrentParentIndex = barLength - 1;
                    }
                    Highlight rightMostBar = new Highlight(CurrentParentIndex, 0, 0);
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
                    if (ChartType == ChartType.Month)
                    {
                        for (int i = 0; i < barLength; i++)
                        {
                            if (i == barLength - 1 && GetIsMDMSDown())
                            {
                                float[] valList = new float[1];
                                valList[0] = 0f;
                                if (i == (CurrentParentIndex == -1 ? barLength - 1 : CurrentParentIndex))
                                {
                                    stackIndex = valList.Length - 1;
                                }
                                yVals1.Add(new BarEntry(i, valList));
                            }
                            else if (selectedSMHistoryData.ByMonth.Months[i].TariffBlocksList != null && selectedSMHistoryData.ByMonth.Months[i].TariffBlocksList.Count > 0 && !selectedSMHistoryData.ByMonth.Months[i].DPCIndicator)
                            {
                                List<float> newValList = new List<float>();

                                if (selectedSMHistoryData.ByMonth.Months[i].TariffBlocksList != null && selectedSMHistoryData.ByMonth.Months[i].TariffBlocksList.Count > 0)
                                {
                                    for (int j = 0; j < selectedSMHistoryData.ByMonth.Months[i].TariffBlocksList.Count; j++)
                                    {
                                        float val = (float)selectedSMHistoryData.ByMonth.Months[i].TariffBlocksList[j].Usage;
                                        if (float.IsPositiveInfinity(val))
                                        {
                                            val = float.PositiveInfinity;
                                        }

                                        if (System.Math.Abs(val) > 0)
                                        {
                                            newValList.Add(System.Math.Abs(val));
                                        }
                                    }
                                }

                                if (newValList.Count > 0)
                                {
                                    float[] valList = new float[newValList.Count];

                                    for (int j = 0; j < newValList.Count; j++)
                                    {
                                        valList[j] = newValList[j];
                                    }

                                    if (i == (CurrentParentIndex == -1 ? barLength - 1 : CurrentParentIndex))
                                    {
                                        stackIndex = valList.Length - 1;
                                    }

                                    yVals1.Add(new BarEntry(i, valList));
                                }
                                else
                                {
                                    float[] valList = new float[1];
                                    float val = (float)selectedSMHistoryData.ByMonth.Months[i].UsageTotal;
                                    if (float.IsPositiveInfinity(val))
                                    {
                                        val = float.PositiveInfinity;
                                    }
                                    valList[0] = System.Math.Abs(val);

                                    if (i == (CurrentParentIndex == -1 ? barLength - 1 : CurrentParentIndex))
                                    {
                                        stackIndex = valList.Length - 1;
                                    }

                                    yVals1.Add(new BarEntry(i, valList));
                                }
                            }
                            else
                            {
                                float[] valList = new float[1];
                                valList[0] = 0f;

                                if (i == (CurrentParentIndex == -1 ? barLength - 1 : CurrentParentIndex))
                                {
                                    stackIndex = valList.Length - 1;
                                }

                                yVals1.Add(new BarEntry(i, valList));
                            }
                        }
                    }
                    else if (ChartType == ChartType.Day)
                    {
                        int i = 0;
                        if (selectedSMHistoryData != null && selectedSMHistoryData.ByDay != null && selectedSMHistoryData.ByDay.Count > 0)
                        {
                            foreach (SMUsageHistoryData.ByDayData DayData in selectedSMHistoryData.ByDay)
                            {
                                if (DayData.Days != null && DayData.Days.Count > 0)
                                {
                                    foreach (SMUsageHistoryData.ByDayData.DayData IndividualDayData in DayData.Days)
                                    {
                                        if (IndividualDayData.TariffBlocksList != null && IndividualDayData.TariffBlocksList.Count > 0)
                                        {
                                            List<float> newValList = new List<float>();
                                            for (int j = 0; j < IndividualDayData.TariffBlocksList.Count; j++)
                                            {
                                                float val = (float)IndividualDayData.TariffBlocksList[j].Usage;
                                                if (float.IsPositiveInfinity(val))
                                                {
                                                    val = float.PositiveInfinity;
                                                }

                                                if (System.Math.Abs(val) > 0)
                                                {
                                                    newValList.Add(System.Math.Abs(val));
                                                }
                                            }

                                            if (newValList.Count > 0)
                                            {
                                                float[] valList = new float[newValList.Count];

                                                for (int j = 0; j < newValList.Count; j++)
                                                {
                                                    valList[j] = newValList[j];
                                                }

                                                yVals1.Add(new BarEntry(i, valList));
                                                if (i == barLength - 1)
                                                {
                                                    stackIndex = valList.Length - 1;
                                                }
                                            }
                                            else
                                            {
                                                float[] valList = new float[1];
                                                valList[0] = 0f;
                                                if (i == barLength - 1)
                                                {
                                                    stackIndex = valList.Length - 1;
                                                }
                                                yVals1.Add(new BarEntry(i, valList));
                                            }
                                        }
                                        else
                                        {
                                            float[] valList = new float[1];
                                            valList[0] = 0f;
                                            if (i == barLength - 1)
                                            {
                                                stackIndex = valList.Length - 1;
                                            }
                                            yVals1.Add(new BarEntry(i, valList));
                                        }
                                        i++;
                                    }
                                }
                                else
                                {
                                    float[] valList = new float[1];
                                    valList[0] = 0f;
                                    if (i == barLength - 1)
                                    {
                                        stackIndex = valList.Length - 1;
                                    }
                                    yVals1.Add(new BarEntry(i, valList));
                                    i++;
                                }
                            }
                        }
                        else
                        {
                            float[] valList = new float[1];
                            valList[0] = 0f;
                            if (i == barLength - 1)
                            {
                                stackIndex = valList.Length - 1;
                            }
                            yVals1.Add(new BarEntry(i, valList));
                        }
                    }

                    if (ChartType == ChartType.Day && isZoomIn)
                    {
                        List<BarEntry> yValNew = new List<BarEntry>();

                        for (int j = 0; j < 4; j++)
                        {
                            float[] valList = new float[1];
                            valList[0] = 0f;
                            yValNew.Add(new BarEntry(j, valList));
                        }

                        for (int j = 0; j < yVals1.Count; j++)
                        {
                            yValNew.Add(new BarEntry(j + 4, yVals1[j].GetYVals()));
                        }


                        for (int j = 0; j < 4; j++)
                        {
                            float[] valList = new float[1];
                            valList[0] = 0f;
                            yValNew.Add(new BarEntry(j + 4 + yVals1.Count, valList));
                        }

                        yVals1 = yValNew;

                        dayViewTariffList = yVals1;
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

                        if (ChartType == ChartType.Month)
                        {
                            for (int i = 0; i < barLength; i++)
                            {
                                if (i == barLength - 1 && GetIsMDMSDown())
                                {
                                    listOfColor.Add(Color.Argb(50, 255, 255, 255));
                                }
                                else if (selectedSMHistoryData.ByMonth.Months[i].TariffBlocksList != null && selectedSMHistoryData.ByMonth.Months[i].TariffBlocksList.Count > 0 && !selectedSMHistoryData.ByMonth.Months[i].DPCIndicator)
                                {
                                    bool isSetColor = false;

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

                                                    float val = (float)selectedSMHistoryData.ByMonth.Months[i].TariffBlocksList[j].Usage;
                                                    if (float.IsPositiveInfinity(val))
                                                    {
                                                        val = float.PositiveInfinity;
                                                    }

                                                    if (System.Math.Abs(val) > 0)
                                                    {
                                                        isSetColor = true;
                                                        listOfColor.Add(Color.Argb(50, selectedSMHistoryData.TariffBlocksLegend[k].Color.RedColor, selectedSMHistoryData.TariffBlocksLegend[k].Color.GreenColor, selectedSMHistoryData.TariffBlocksLegend[k].Color.BlueData));
                                                    }
                                                    else if (selectedSMHistoryData.ByMonth.Months[i].TariffBlocksList.Count == 1)
                                                    {
                                                        isSetColor = true;
                                                        listOfColor.Add(Color.Argb(50, 255, 255, 255));
                                                    }
                                                    break;
                                                }
                                            }

                                            if (!isFound)
                                            {
                                                isSetColor = true;
                                                listOfColor.Add(Color.Argb(50, 255, 255, 255));
                                            }

                                        }
                                        else
                                        {
                                            isSetColor = true;
                                            listOfColor.Add(Color.Argb(50, 255, 255, 255));
                                        }
                                    }

                                    if (!isSetColor)
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
                        else if (ChartType == ChartType.Day)
                        {
                            if (selectedSMHistoryData != null && selectedSMHistoryData.ByDay != null && selectedSMHistoryData.ByDay.Count > 0)
                            {
                                foreach (SMUsageHistoryData.ByDayData DayData in selectedSMHistoryData.ByDay)
                                {
                                    if (DayData.Days != null && DayData.Days.Count > 0)
                                    {
                                        foreach (SMUsageHistoryData.ByDayData.DayData IndividualDayData in DayData.Days)
                                        {
                                            if (IndividualDayData.TariffBlocksList != null && IndividualDayData.TariffBlocksList.Count > 0)
                                            {
                                                bool isSetColor = false;

                                                for (int j = 0; j < IndividualDayData.TariffBlocksList.Count; j++)
                                                {
                                                    if (selectedSMHistoryData.TariffBlocksLegend != null && selectedSMHistoryData.TariffBlocksLegend.Count > 0)
                                                    {
                                                        bool isFound = false;
                                                        for (int k = 0; k < selectedSMHistoryData.TariffBlocksLegend.Count; k++)
                                                        {
                                                            if (IndividualDayData.TariffBlocksList[j].BlockId == selectedSMHistoryData.TariffBlocksLegend[k].BlockId)
                                                            {
                                                                isFound = true;
                                                                float val = (float)IndividualDayData.TariffBlocksList[j].Usage;
                                                                if (float.IsPositiveInfinity(val))
                                                                {
                                                                    val = float.PositiveInfinity;
                                                                }

                                                                if (System.Math.Abs(val) > 0)
                                                                {
                                                                    isSetColor = true;
                                                                    listOfColor.Add(Color.Argb(50, selectedSMHistoryData.TariffBlocksLegend[k].Color.RedColor, selectedSMHistoryData.TariffBlocksLegend[k].Color.GreenColor, selectedSMHistoryData.TariffBlocksLegend[k].Color.BlueData));
                                                                }
                                                                else if (IndividualDayData.TariffBlocksList.Count == 1)
                                                                {
                                                                    isSetColor = true;
                                                                    listOfColor.Add(Color.Argb(50, 255, 255, 255));
                                                                }
                                                                break;
                                                            }
                                                        }

                                                        if (!isFound)
                                                        {
                                                            isSetColor = true;
                                                            listOfColor.Add(Color.Argb(50, 255, 255, 255));
                                                        }

                                                    }
                                                    else
                                                    {
                                                        isSetColor = true;
                                                        listOfColor.Add(Color.Argb(50, 255, 255, 255));
                                                    }
                                                }

                                                if (!isSetColor)
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

                            if (isZoomIn)
                            {
                                List<int> listOfColorNew = new List<int>();

                                for (int i = 0; i < 4; i++)
                                {
                                    listOfColorNew.Add(Color.Argb(50, 255, 255, 255));
                                }

                                for (int i = 0; i < listOfColor.Count; i++)
                                {
                                    listOfColorNew.Add(listOfColor[i]);
                                }


                                for (int i = 0; i < 4; i++)
                                {
                                    listOfColorNew.Add(Color.Argb(50, 255, 255, 255));
                                }

                                listOfColor = listOfColorNew;
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

                        if (ChartType == ChartType.Month)
                        {
                            data.BarWidth = 0.25f;
                        }
                        else if (ChartType == ChartType.Day)
                        {
                            if (!isZoomIn)
                            {
                                data.BarWidth = 0.60f;
                            }
                            else
                            {
                                data.BarWidth = 0.35f;
                            }
                        }

                        set1.HighLightAlpha = 0;

                        data.HighlightEnabled = true;
                        data.SetValueTextSize(10f);
                        data.SetDrawValues(false);

                        mChart.Data = data;
                    }

                    if (ChartType == ChartType.Month)
                    {
                        // HIGHLIGHT RIGHT MOST ITEM
                        if (CurrentParentIndex == -1)
                        {
                            CurrentParentIndex = barLength - 1;
                        }
                        Highlight rightMostBar = new Highlight(CurrentParentIndex, 0, stackIndex);
                        mChart.HighlightValues(new Highlight[] { rightMostBar });
                    }
                    else if (ChartType == ChartType.Day)
                    {

                    }
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
                            float val = (float)selectedSMHistoryData.ByMonth.Months[i].UsageTotal;
                            if (i == barLength - 1 && GetIsMDMSDown())
                            {
                                val = 0;
                            }
                            else if (selectedSMHistoryData.ByMonth.Months[i].DPCIndicator)
                            {
                                val = 0;
                            }
                            else
                            {
                                if (float.IsPositiveInfinity(val))
                                {
                                    val = float.PositiveInfinity;
                                }
                            }
                            valList[0] = System.Math.Abs(val);
                            yVals1.Add(new BarEntry(i, valList));
                        }
                        else if (ChartType == ChartType.Day)
                        {
                            float[] valList = new float[1];
                            float val = (float)DayViewkWhData[i];
                            if (float.IsPositiveInfinity(val))
                            {
                                val = float.PositiveInfinity;
                            }
                            valList[0] = System.Math.Abs(val);
                            yVals1.Add(new BarEntry(i, valList));
                        }
                    }

                    if (ChartType == ChartType.Day && isZoomIn)
                    {
                        List<BarEntry> yValNew = new List<BarEntry>();

                        for (int j = 0; j < 4; j++)
                        {
                            float[] valLis = new float[1];
                            valLis[0] = 0f;
                            yValNew.Add(new BarEntry(j, valLis));
                        }

                        for (int j = 0; j < yVals1.Count; j++)
                        {
                            yValNew.Add(new BarEntry(j + 4, yVals1[j].GetYVals()));
                        }


                        for (int j = 0; j < 4; j++)
                        {
                            float[] valLis = new float[1];
                            valLis[0] = 0f;
                            yValNew.Add(new BarEntry(j + 4 + yVals1.Count, valLis));
                        }

                        yVals1 = yValNew;

                        dayViewTariffList = yVals1;
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
                            if (ChartType == ChartType.Month)
                            {
                                listOfColor.Add(Color.Argb(50, 255, 255, 255));
                            }
                            else if (ChartType == ChartType.Day)
                            {
                                listOfColor.Add(Color.Argb(50, 255, 255, 255));
                            }
                        }

                        if (ChartType == ChartType.Day && isZoomIn)
                        {
                            List<int> listOfColorNew = new List<int>();

                            for (int i = 0; i < 4; i++)
                            {
                                listOfColorNew.Add(Color.Argb(50, 255, 255, 255));
                            }

                            for (int i = 0; i < listOfColor.Count; i++)
                            {
                                listOfColorNew.Add(listOfColor[i]);
                            }


                            for (int i = 0; i < 4; i++)
                            {
                                listOfColorNew.Add(Color.Argb(50, 255, 255, 255));
                            }

                            listOfColor = listOfColorNew;
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

                        if (ChartType == ChartType.Month)
                        {
                            data.BarWidth = 0.25f;
                        }
                        else if (ChartType == ChartType.Day)
                        {
                            if (!isZoomIn)
                            {
                                data.BarWidth = 0.60f;
                            }
                            else
                            {
                                data.BarWidth = 0.35f;
                            }
                        }

                        set1.HighLightAlpha = 0;

                        data.HighlightEnabled = true;
                        data.SetValueTextSize(10f);
                        data.SetDrawValues(false);

                        mChart.Data = data;
                    }

                    if (ChartType == ChartType.Month)
                    {
                        // HIGHLIGHT RIGHT MOST ITEM
                        if (CurrentParentIndex == -1)
                        {
                            CurrentParentIndex = barLength - 1;
                        }
                        Highlight rightMostBar = new Highlight(CurrentParentIndex, 0, 0);
                        mChart.HighlightValues(new Highlight[] { rightMostBar });
                    }
                    else if (ChartType == ChartType.Day)
                    {

                    }
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
            btnToggleDay.Enabled = true;
            btnToggleMonth.Enabled = true;

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
            btnToggleDay.Enabled = true;
            btnToggleMonth.Enabled = true;

            mChart.Clear();
            SetUp();
        }

        // Lin Siong Note: Show by Day graph
        public void ShowByDay()
        {
            ChartType = ChartType.Day;

            CurrentParentIndex = -1;

            mChart.Visibility = ViewStates.Visible;

            rmKwhSelection.Enabled = true;
            rmKwhLabel.SetTextColor(new Color(ContextCompat.GetColor(this.Activity, Resource.Color.powerBlue)));
            imgRmKwhDropdownArrow.SetImageResource(Resource.Drawable.rectangle);
            tarifToggle.Enabled = true;
            btnToggleDay.Enabled = true;
            btnToggleMonth.Enabled = true;

            DayViewkWhData = new List<double>();
            DayViewRMData = new List<double>();
            mChart.Clear();
            SetUp();
        }

        // Lin Siong Note: Show by Month graph
        public void ShowByMonth()
        {
            ChartType = ChartType.Month;

            mChart.Visibility = ViewStates.Visible;

            rmKwhSelection.Enabled = true;
            rmKwhLabel.SetTextColor(new Color(ContextCompat.GetColor(this.Activity, Resource.Color.powerBlue)));
            imgRmKwhDropdownArrow.SetImageResource(Resource.Drawable.rectangle);
            tarifToggle.Enabled = true;
            btnToggleDay.Enabled = true;
            btnToggleMonth.Enabled = true;

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

        public void ShowViewBill(GetBillHistoryResponse.ResponseData selectedBill)
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
            if (!this.GetIsClicked())
            {
                this.SetIsClicked(true);
                isGoToBillingDetail = true;
                Intent intent = new Intent(Activity, typeof(BillingDetailsActivity));
                intent.PutExtra("SELECTED_ACCOUNT", JsonConvert.SerializeObject(selectedAccount));
                intent.PutExtra("PENDING_PAYMENT", mIsPendingPayment);
                StartActivity(intent);
                try
                {
                    FirebaseAnalyticsUtils.LogFragmentClickEvent(this, "View Details Buttom Clicked");
                }
                catch (System.Exception ne)
                {
                    Utility.LoggingNonFatalError(ne);
                }
            }
        }

        [OnClick(Resource.Id.btnReView)]
        internal void OnREViewBill(object sender, EventArgs e)
        {
            if (!this.GetIsClicked())
            {
                this.SetIsClicked(true);
                Intent viewBill = new Intent(this.Activity, typeof(ViewBillActivity));
                viewBill.PutExtra(Constants.SELECTED_ACCOUNT, JsonConvert.SerializeObject(selectedAccount));
                viewBill.PutExtra(Constants.CODE_KEY, Constants.SELECT_ACCOUNT_PDF_REQUEST_CODE);
                StartActivity(viewBill);
                try
                {
                    FirebaseAnalyticsUtils.LogFragmentClickEvent(this, "View Bill Buttom Clicked");
                }
                catch (System.Exception ne)
                {
                    Utility.LoggingNonFatalError(ne);
                }
            }
        }

        [OnClick(Resource.Id.btnRefresh)]
        internal void OnRefresh(object sender, EventArgs e)
        {
            this.userActionsListener.OnTapRefresh();
            try
            {
                FirebaseAnalyticsUtils.LogFragmentClickEvent(this, "Inner Dashboard Refresh Buttom Clicked");
            }
            catch (System.Exception ne)
            {
                Utility.LoggingNonFatalError(ne);
            }
        }

        [OnClick(Resource.Id.btnMDMSDownRefresh)]
        internal void OnMDMSDownRefresh(object sender, EventArgs e)
        {
            this.userActionsListener.OnTapRefresh();
            try
            {
                FirebaseAnalyticsUtils.LogFragmentClickEvent(this, "Inner Dashboard MDMS Down Refresh Buttom Clicked");
            }
            catch (System.Exception ne)
            {
                Utility.LoggingNonFatalError(ne);
            }
        }

        [OnClick(Resource.Id.btnPay)]
        internal void OnUserPay(object sender, EventArgs e)
        {
            if (!this.GetIsClicked())
            {
                this.SetIsClicked(true);
                this.userActionsListener.OnPay();
                try
                {
                    FirebaseAnalyticsUtils.LogFragmentClickEvent(this, "Inner Dashboard Payment Buttom Clicked");
                }
                catch (System.Exception ne)
                {
                    Utility.LoggingNonFatalError(ne);
                }
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
                FirebaseAnalyticsUtils.LogFragmentClickEvent(this, "RM / kWh Toggle Button Clicked");
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
                    tarifToggle.SetBackgroundResource(Resource.Drawable.rectangle_white_outline_rounded_button_bg);
                    txtTarifToggle.SetTextColor(new Color(ContextCompat.GetColor(this.Activity, Resource.Color.white)));
                    txtTarifToggle.Alpha = 1f;
                    txtTarifToggle.Text = Utility.GetLocalizedLabel("Usage", "tariffBlock");
                    isToggleTariff = false;
                    if (isChangeBackgroundNeeded)
                    {
                        scrollViewContent.SetBackgroundResource(Resource.Drawable.dashboard_chart_bg);
                    }
                    DashboardCustomScrolling(0);

                    isClickedShowTariff = false;
                    isHideBottomSheetShowTariff = false;

                    bottomSheetBehavior.State = BottomSheetBehavior.StateExpanded;
                }
                else
                {
                    imgTarifToggle.SetImageResource(Resource.Drawable.eye_hide);
                    tarifToggle.SetBackgroundResource(Resource.Drawable.rectangle_rounded_button_bg);
                    txtTarifToggle.SetTextColor(new Color(ContextCompat.GetColor(this.Activity, Resource.Color.powerBlue)));
                    txtTarifToggle.Alpha = 1f;
                    txtTarifToggle.Text = Utility.GetLocalizedLabel("Usage", "tariffBlock");
                    isToggleTariff = true;
                    if (isChangeBackgroundNeeded)
                    {
                        scrollViewContent.SetBackgroundResource(Resource.Drawable.dashboard_chart_bg);
                    }

                    isClickedShowTariff = true;
                }

                mChart.Clear();
                SetUp();

                if (isToggleTariff && isClickedShowTariff)
                {
                    if (CheckIsScrollable())
                    {
                        bottomSheetBehavior.State = BottomSheetBehavior.StateHidden;
                        isHideBottomSheetShowTariff = true;
                    }
                }

                FirebaseAnalyticsUtils.LogFragmentClickEvent(this, "Tariff Toggle Button Clicked");
            }
            catch (System.Exception ne)
            {
                Utility.LoggingNonFatalError(ne);
            }
        }

        private void OnGenerateTariffLegendValue(int index, bool isShow)
        {
            try
            {
                currentSelectedBar = index;
                bool isHighlightNeed = false;

                string message = Utility.GetLocalizedLabel("Usage", "tariffLegendNote");

                if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.N)
                {
                    txtTariffBlockLegendDisclaimer.TextFormatted = Html.FromHtml(message, FromHtmlOptions.ModeLegacy);
                }
                else
                {
                    txtTariffBlockLegendDisclaimer.TextFormatted = Html.FromHtml(message);
                }

                if (!isShow)
                {
                    tariffBlockLegendRecyclerView.Visibility = ViewStates.Gone;
                    tariffBlockLegendDisclaimerLayout.Visibility = ViewStates.Gone;

                    if (ChartType == ChartType.Month && ChartDataType == ChartDataType.kWh && index != -1)
                    {
                        if (isSMAccount)
                        {
                            if (GetIsMDMSDown())
                            {
                                if (index != selectedSMHistoryData.ByMonth.Months.Count - 1 && selectedSMHistoryData.ByMonth.Months[index].DPCIndicator
                                    && !string.IsNullOrEmpty(selectedSMHistoryData.ByMonth.Months[index].DPCIndicatorUsageMessage.Trim()))
                                {
                                    tariffBlockLegendDisclaimerLayout.Visibility = ViewStates.Visible;

                                    message = selectedSMHistoryData.ByMonth.Months[index].DPCIndicatorUsageMessage;

                                    if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.N)
                                    {
                                        txtTariffBlockLegendDisclaimer.TextFormatted = Html.FromHtml(message, FromHtmlOptions.ModeLegacy);
                                    }
                                    else
                                    {
                                        txtTariffBlockLegendDisclaimer.TextFormatted = Html.FromHtml(message);
                                    }

                                    txtTariffBlockLegendDisclaimer = LinkRedirectionUtils
                                        .Create(this.Activity, "")
                                        .SetTextView(txtTariffBlockLegendDisclaimer)
                                        .SetMessage(message, new Android.Graphics.Color(ContextCompat.GetColor(this.Activity, Resource.Color.sunGlow)))
                                        .Build()
                                        .GetProcessedTextView();

                                    isDPCBarClicked = true;
                                    isChangeVirtualHeightNeed = true;
                                    SetVirtualHeightParams(6f);
                                }
                                else
                                {
                                    if (isDPCBarClicked)
                                    {
                                        isDPCBarClicked = false;
                                        isChangeVirtualHeightNeed = true;
                                        SetVirtualHeightParams(6f);
                                    }
                                }
                            }
                            else
                            {
                                if (selectedSMHistoryData.ByMonth.Months[index].DPCIndicator
                                    && !string.IsNullOrEmpty(selectedSMHistoryData.ByMonth.Months[index].DPCIndicatorUsageMessage.Trim()))
                                {
                                    tariffBlockLegendDisclaimerLayout.Visibility = ViewStates.Visible;

                                    message = selectedSMHistoryData.ByMonth.Months[index].DPCIndicatorUsageMessage;

                                    if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.N)
                                    {
                                        txtTariffBlockLegendDisclaimer.TextFormatted = Html.FromHtml(message, FromHtmlOptions.ModeLegacy);
                                    }
                                    else
                                    {
                                        txtTariffBlockLegendDisclaimer.TextFormatted = Html.FromHtml(message);
                                    }

                                    txtTariffBlockLegendDisclaimer = LinkRedirectionUtils
                                        .Create(this.Activity, "")
                                        .SetTextView(txtTariffBlockLegendDisclaimer)
                                        .SetMessage(message, new Android.Graphics.Color(ContextCompat.GetColor(this.Activity, Resource.Color.sunGlow)))
                                        .Build()
                                        .GetProcessedTextView();

                                    isDPCBarClicked = true;
                                    isChangeVirtualHeightNeed = true;
                                    SetVirtualHeightParams(6f);
                                }
                                else
                                {
                                    if (smStatisticContainer.Visibility == ViewStates.Visible)
                                    {
                                        scrollViewContent.SetBackgroundResource(Resource.Drawable.dashboard_chart_bg);
                                    }

                                    if (isDPCBarClicked)
                                    {
                                        isDPCBarClicked = false;
                                        isChangeVirtualHeightNeed = true;
                                        SetVirtualHeightParams(6f);
                                    }
                                }
                            }
                        }
                        else if (!isREAccount)
                        {
                            if (selectedHistoryData.ByMonth.Months[index].DPCIndicator
                                && !string.IsNullOrEmpty(selectedHistoryData.ByMonth.Months[index].DPCIndicatorUsageMessage.Trim()))
                            {
                                tariffBlockLegendDisclaimerLayout.Visibility = ViewStates.Visible;

                                message = selectedHistoryData.ByMonth.Months[index].DPCIndicatorUsageMessage;

                                if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.N)
                                {
                                    txtTariffBlockLegendDisclaimer.TextFormatted = Html.FromHtml(message, FromHtmlOptions.ModeLegacy);
                                }
                                else
                                {
                                    txtTariffBlockLegendDisclaimer.TextFormatted = Html.FromHtml(message);
                                }

                                if (isSMR && ssmrHistoryContainer.Visibility == ViewStates.Visible)
                                {
                                    scrollViewContent.SetBackgroundResource(Resource.Drawable.dashboard_chart_bg);
                                }

                                txtTariffBlockLegendDisclaimer = LinkRedirectionUtils
                                    .Create(this.Activity, "")
                                    .SetTextView(txtTariffBlockLegendDisclaimer)
                                    .SetMessage(message, new Android.Graphics.Color(ContextCompat.GetColor(this.Activity, Resource.Color.sunGlow)))
                                    .Build()
                                    .GetProcessedTextView();

                                isDPCBarClicked = true;
                                isChangeVirtualHeightNeed = true;
                                SetVirtualHeightParams(6f);
                            }
                            else
                            {
                                if (isSMR && ssmrHistoryContainer.Visibility == ViewStates.Visible)
                                {
                                    scrollViewContent.SetBackgroundResource(Resource.Drawable.dashboard_chart_bg);
                                }

                                if (isDPCBarClicked)
                                {
                                    isDPCBarClicked = false;
                                    isChangeVirtualHeightNeed = true;
                                    SetVirtualHeightParams(6f);
                                }
                            }
                        }
                    }
                    else if (ChartType == ChartType.Month && ChartDataType == ChartDataType.RM && index != -1)
                    {
                        if (isSMAccount)
                        {
                            if (GetIsMDMSDown())
                            {
                                if (index != selectedSMHistoryData.ByMonth.Months.Count - 1 && selectedSMHistoryData.ByMonth.Months[index].DPCIndicator
                                    && !string.IsNullOrEmpty(selectedSMHistoryData.ByMonth.Months[index].DPCIndicatorRMMessage.Trim()))
                                {
                                    tariffBlockLegendDisclaimerLayout.Visibility = ViewStates.Visible;

                                    message = selectedSMHistoryData.ByMonth.Months[index].DPCIndicatorRMMessage;

                                    if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.N)
                                    {
                                        txtTariffBlockLegendDisclaimer.TextFormatted = Html.FromHtml(message, FromHtmlOptions.ModeLegacy);
                                    }
                                    else
                                    {
                                        txtTariffBlockLegendDisclaimer.TextFormatted = Html.FromHtml(message);
                                    }

                                    txtTariffBlockLegendDisclaimer = LinkRedirectionUtils
                                        .Create(this.Activity, "")
                                        .SetTextView(txtTariffBlockLegendDisclaimer)
                                        .SetMessage(message, new Android.Graphics.Color(ContextCompat.GetColor(this.Activity, Resource.Color.sunGlow)))
                                        .Build()
                                        .GetProcessedTextView();

                                    isDPCBarClicked = true;
                                    isChangeVirtualHeightNeed = true;
                                    SetVirtualHeightParams(6f);
                                }
                                else
                                {
                                    if (isDPCBarClicked)
                                    {
                                        isDPCBarClicked = false;
                                        isChangeVirtualHeightNeed = true;
                                        SetVirtualHeightParams(6f);
                                    }
                                }
                            }
                            else
                            {
                                if (selectedSMHistoryData.ByMonth.Months[index].DPCIndicator
                                    && !string.IsNullOrEmpty(selectedSMHistoryData.ByMonth.Months[index].DPCIndicatorRMMessage.Trim()))
                                {
                                    tariffBlockLegendDisclaimerLayout.Visibility = ViewStates.Visible;

                                    message = selectedSMHistoryData.ByMonth.Months[index].DPCIndicatorRMMessage;

                                    if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.N)
                                    {
                                        txtTariffBlockLegendDisclaimer.TextFormatted = Html.FromHtml(message, FromHtmlOptions.ModeLegacy);
                                    }
                                    else
                                    {
                                        txtTariffBlockLegendDisclaimer.TextFormatted = Html.FromHtml(message);
                                    }

                                    txtTariffBlockLegendDisclaimer = LinkRedirectionUtils
                                        .Create(this.Activity, "")
                                        .SetTextView(txtTariffBlockLegendDisclaimer)
                                        .SetMessage(message, new Android.Graphics.Color(ContextCompat.GetColor(this.Activity, Resource.Color.sunGlow)))
                                        .Build()
                                        .GetProcessedTextView();

                                    isDPCBarClicked = true;
                                    isChangeVirtualHeightNeed = true;
                                    SetVirtualHeightParams(6f);
                                }
                                else
                                {
                                    if (smStatisticContainer.Visibility == ViewStates.Visible)
                                    {
                                        scrollViewContent.SetBackgroundResource(Resource.Drawable.dashboard_chart_bg);
                                    }

                                    if (isDPCBarClicked)
                                    {
                                        isDPCBarClicked = false;
                                        isChangeVirtualHeightNeed = true;
                                        SetVirtualHeightParams(6f);
                                    }
                                }
                            }
                        }
                        else if (!isREAccount)
                        {
                            if (selectedHistoryData.ByMonth.Months[index].DPCIndicator
                                && !string.IsNullOrEmpty(selectedHistoryData.ByMonth.Months[index].DPCIndicatorRMMessage.Trim()))
                            {
                                tariffBlockLegendDisclaimerLayout.Visibility = ViewStates.Visible;

                                message = selectedHistoryData.ByMonth.Months[index].DPCIndicatorRMMessage;

                                if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.N)
                                {
                                    txtTariffBlockLegendDisclaimer.TextFormatted = Html.FromHtml(message, FromHtmlOptions.ModeLegacy);
                                }
                                else
                                {
                                    txtTariffBlockLegendDisclaimer.TextFormatted = Html.FromHtml(message);
                                }

                                txtTariffBlockLegendDisclaimer = LinkRedirectionUtils
                                    .Create(this.Activity, "")
                                    .SetTextView(txtTariffBlockLegendDisclaimer)
                                    .SetMessage(message, new Android.Graphics.Color(ContextCompat.GetColor(this.Activity, Resource.Color.sunGlow)))
                                    .Build()
                                    .GetProcessedTextView();

                                if (isSMR && ssmrHistoryContainer.Visibility == ViewStates.Visible)
                                {
                                    scrollViewContent.SetBackgroundResource(Resource.Drawable.dashboard_chart_bg);
                                }

                                isDPCBarClicked = true;
                                isChangeVirtualHeightNeed = true;
                                SetVirtualHeightParams(6f);
                            }
                            else
                            {
                                if (isSMR && ssmrHistoryContainer.Visibility == ViewStates.Visible)
                                {
                                    scrollViewContent.SetBackgroundResource(Resource.Drawable.dashboard_chart_bg);
                                }

                                if (isDPCBarClicked)
                                {
                                    isDPCBarClicked = false;
                                    isChangeVirtualHeightNeed = true;
                                    SetVirtualHeightParams(6f);
                                }
                            }
                        }
                    }
                    else if (ChartType == ChartType.Month && ChartDataType == ChartDataType.kWh && isSMR)
                    {
                        if (isSMR && smStatisticContainer.Visibility == ViewStates.Visible)
                        {
                            scrollViewContent.SetBackgroundResource(Resource.Drawable.dashboard_chart_bg);
                        }

                        if (isDPCBarClicked)
                        {
                            isDPCBarClicked = false;
                            isChangeVirtualHeightNeed = true;
                            SetVirtualHeightParams(6f);
                        }
                    }

                }
                else
                {
                    if (ChartType == ChartType.Month)
                    {
                        int startCount = -1;
                        bool isFound = false;
                        tariffBlockLegendRecyclerView.Visibility = ViewStates.Visible;
                        tariffBlockLegendDisclaimerLayout.Visibility = ViewStates.Visible;
                        List<TariffBlocksLegendData> newTariffList = new List<TariffBlocksLegendData>();
                        if (index == -1)
                        {
                            tariffBlockLegendRecyclerView.Visibility = ViewStates.Gone;
                            tariffBlockLegendDisclaimerLayout.Visibility = ViewStates.Gone;
                        }
                        else if (!isSMAccount)
                        {
                            if (selectedHistoryData.ByMonth.Months[index].TariffBlocksList != null)
                            {
                                startCount = selectedHistoryData.ByMonth.Months[index].TariffBlocksList.Count - 1;
                            }

                            if (ChartDataType == ChartDataType.RM && (float)selectedHistoryData.ByMonth.Months[index].AmountTotal <= 0.00)
                            {
                                startCount = -1;
                            }

                            if (startCount == -1)
                            {
                                tariffBlockLegendRecyclerView.Visibility = ViewStates.Gone;
                                tariffBlockLegendDisclaimerLayout.Visibility = ViewStates.Gone;
                            }
                            else
                            {
                                for (int i = 0; i < selectedHistoryData.TariffBlocksLegend.Count; i++)
                                {
                                    for (int j = 0; j < selectedHistoryData.ByMonth.Months[index].TariffBlocksList.Count; j++)
                                    {
                                        if ((selectedHistoryData.TariffBlocksLegend[i].BlockId == selectedHistoryData.ByMonth.Months[index].TariffBlocksList[j].BlockId))
                                        {
                                            if ((ChartDataType == ChartDataType.RM && System.Math.Abs(selectedHistoryData.ByMonth.Months[index].TariffBlocksList[j].Amount) > 0.00) || (ChartDataType == ChartDataType.kWh && System.Math.Abs(selectedHistoryData.ByMonth.Months[index].TariffBlocksList[j].Usage) > 0.00))
                                            {
                                                isFound = true;
                                                newTariffList.Add(selectedHistoryData.TariffBlocksLegend[i]);
                                            }
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (index == selectedSMHistoryData.ByMonth.Months.Count - 1)
                            {
                                isHighlightNeed = true;
                            }

                            if (selectedSMHistoryData.ByMonth.Months[index].TariffBlocksList != null)
                            {
                                startCount = selectedSMHistoryData.ByMonth.Months[index].TariffBlocksList.Count - 1;
                            }

                            if (ChartDataType == ChartDataType.RM && (float)selectedSMHistoryData.ByMonth.Months[index].AmountTotal <= 0.00)
                            {
                                startCount = -1;
                            }

                            if (startCount == -1)
                            {
                                tariffBlockLegendRecyclerView.Visibility = ViewStates.Gone;
                                tariffBlockLegendDisclaimerLayout.Visibility = ViewStates.Gone;
                            }
                            else
                            {
                                for (int i = 0; i < selectedSMHistoryData.TariffBlocksLegend.Count; i++)
                                {
                                    for (int j = 0; j < selectedSMHistoryData.ByMonth.Months[index].TariffBlocksList.Count; j++)
                                    {
                                        if (selectedSMHistoryData.TariffBlocksLegend[i].BlockId == selectedSMHistoryData.ByMonth.Months[index].TariffBlocksList[j].BlockId)
                                        {
                                            if ((ChartDataType == ChartDataType.RM && System.Math.Abs(selectedSMHistoryData.ByMonth.Months[index].TariffBlocksList[j].Amount) > 0.00) || (ChartDataType == ChartDataType.kWh && System.Math.Abs(selectedSMHistoryData.ByMonth.Months[index].TariffBlocksList[j].Usage) > 0.00))
                                            {
                                                isFound = true;
                                                newTariffList.Add(selectedSMHistoryData.TariffBlocksLegend[i]);
                                            }
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        if (startCount > -1 && isFound)
                        {
                            tariffBlockLegendAdapter = new TariffBlockLegendAdapter(newTariffList, this.Activity, isHighlightNeed);
                            tariffBlockLegendRecyclerView.SetAdapter(tariffBlockLegendAdapter);

                            LayoutAnimationController controller =
                                    AnimationUtils.LoadLayoutAnimation(this.Activity, Resource.Animation.layout_animation_fall_down);

                            tariffBlockLegendRecyclerView.LayoutAnimation = controller;
                            tariffBlockLegendRecyclerView.GetAdapter().NotifyDataSetChanged();
                            tariffBlockLegendRecyclerView.ScheduleLayoutAnimation();
                        }
                        else
                        {
                            tariffBlockLegendRecyclerView.Visibility = ViewStates.Gone;
                            tariffBlockLegendDisclaimerLayout.Visibility = ViewStates.Gone;
                        }
                    }
                    else if (ChartType == ChartType.Day && isSMAccount)
                    {
                        bool isFound = false;
                        bool isAlreadyExists = false;


                        List<TariffBlocksLegendData> newTariffList = new List<TariffBlocksLegendData>();
                        for (int i = 0; i < selectedSMHistoryData.TariffBlocksLegend.Count; i++)
                        {
                            isAlreadyExists = false;

                            for (int j = 0; j < selectedSMHistoryData.ByDay.Count; j++)
                            {
                                for (int k = 0; k < selectedSMHistoryData.ByDay[j].Days.Count; k++)
                                {
                                    for (int l = 0; l < selectedSMHistoryData.ByDay[j].Days[k].TariffBlocksList.Count; l++)
                                    {
                                        if ((selectedSMHistoryData.TariffBlocksLegend[i].BlockId == selectedSMHistoryData.ByDay[j].Days[k].TariffBlocksList[l].BlockId))
                                        {
                                            if ((ChartDataType == ChartDataType.RM && System.Math.Abs(selectedSMHistoryData.ByDay[j].Days[k].TariffBlocksList[l].Amount) > 0.00) || (ChartDataType == ChartDataType.kWh && System.Math.Abs(selectedSMHistoryData.ByDay[j].Days[k].TariffBlocksList[l].Usage) > 0.00))
                                            {
                                                isFound = true;
                                                isAlreadyExists = true;
                                                newTariffList.Add(selectedSMHistoryData.TariffBlocksLegend[i]);
                                            }
                                            break;
                                        }
                                    }

                                    if (isAlreadyExists)
                                    {
                                        break;
                                    }
                                }

                                if (isAlreadyExists)
                                {
                                    break;
                                }
                            }
                        }

                        if (isFound)
                        {
                            tariffBlockLegendRecyclerView.Visibility = ViewStates.Visible;
                            tariffBlockLegendDisclaimerLayout.Visibility = ViewStates.Visible;
                            tariffBlockLegendAdapter = new TariffBlockLegendAdapter(newTariffList, this.Activity, false);
                            tariffBlockLegendRecyclerView.SetAdapter(tariffBlockLegendAdapter);

                            LayoutAnimationController controller =
                                    AnimationUtils.LoadLayoutAnimation(this.Activity, Resource.Animation.layout_animation_fall_down);

                            tariffBlockLegendRecyclerView.LayoutAnimation = controller;
                            tariffBlockLegendRecyclerView.GetAdapter().NotifyDataSetChanged();
                            tariffBlockLegendRecyclerView.ScheduleLayoutAnimation();
                        }
                        else
                        {
                            tariffBlockLegendRecyclerView.Visibility = ViewStates.Gone;
                            tariffBlockLegendDisclaimerLayout.Visibility = ViewStates.Gone;
                        }
                    }
                    else
                    {
                        tariffBlockLegendRecyclerView.Visibility = ViewStates.Gone;
                        tariffBlockLegendDisclaimerLayout.Visibility = ViewStates.Gone;
                    }

                    tariffBlockLegendRecyclerView.RequestLayout();

                    if (ChartType == ChartType.Month && index != -1)
                    {
                        if (isSMAccount)
                        {
                            if (GetIsMDMSDown())
                            {
                                if (index != selectedSMHistoryData.ByMonth.Months.Count - 1 && selectedSMHistoryData.ByMonth.Months[index].DPCIndicator
                                    && !string.IsNullOrEmpty(selectedSMHistoryData.ByMonth.Months[index].DPCIndicatorTariffMessage.Trim()))
                                {
                                    tariffBlockLegendRecyclerView.Visibility = ViewStates.Gone;
                                    tariffBlockLegendDisclaimerLayout.Visibility = ViewStates.Visible;

                                    message = selectedSMHistoryData.ByMonth.Months[index].DPCIndicatorTariffMessage;

                                    if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.N)
                                    {
                                        txtTariffBlockLegendDisclaimer.TextFormatted = Html.FromHtml(message, FromHtmlOptions.ModeLegacy);
                                    }
                                    else
                                    {
                                        txtTariffBlockLegendDisclaimer.TextFormatted = Html.FromHtml(message);
                                    }

                                    txtTariffBlockLegendDisclaimer = LinkRedirectionUtils
                                        .Create(this.Activity, "")
                                        .SetTextView(txtTariffBlockLegendDisclaimer)
                                        .SetMessage(message, new Android.Graphics.Color(ContextCompat.GetColor(this.Activity, Resource.Color.sunGlow)))
                                        .Build()
                                        .GetProcessedTextView();
                                }
                            }
                            else
                            {
                                if (selectedSMHistoryData.ByMonth.Months[index].DPCIndicator
                                    && !string.IsNullOrEmpty(selectedSMHistoryData.ByMonth.Months[index].DPCIndicatorTariffMessage.Trim()))
                                {
                                    tariffBlockLegendRecyclerView.Visibility = ViewStates.Gone;
                                    tariffBlockLegendDisclaimerLayout.Visibility = ViewStates.Visible;

                                    message = selectedSMHistoryData.ByMonth.Months[index].DPCIndicatorTariffMessage;

                                    if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.N)
                                    {
                                        txtTariffBlockLegendDisclaimer.TextFormatted = Html.FromHtml(message, FromHtmlOptions.ModeLegacy);
                                    }
                                    else
                                    {
                                        txtTariffBlockLegendDisclaimer.TextFormatted = Html.FromHtml(message);
                                    }

                                    txtTariffBlockLegendDisclaimer = LinkRedirectionUtils
                                        .Create(this.Activity, "")
                                        .SetTextView(txtTariffBlockLegendDisclaimer)
                                        .SetMessage(message, new Android.Graphics.Color(ContextCompat.GetColor(this.Activity, Resource.Color.sunGlow)))
                                        .Build()
                                        .GetProcessedTextView();
                                }
                            }
                        }
                        else if (!isREAccount)
                        {
                            if (selectedHistoryData.ByMonth.Months[index].DPCIndicator
                                && !string.IsNullOrEmpty(selectedHistoryData.ByMonth.Months[index].DPCIndicatorTariffMessage.Trim()))
                            {
                                tariffBlockLegendRecyclerView.Visibility = ViewStates.Gone;
                                tariffBlockLegendDisclaimerLayout.Visibility = ViewStates.Visible;

                                message = selectedHistoryData.ByMonth.Months[index].DPCIndicatorTariffMessage;

                                if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.N)
                                {
                                    txtTariffBlockLegendDisclaimer.TextFormatted = Html.FromHtml(message, FromHtmlOptions.ModeLegacy);
                                }
                                else
                                {
                                    txtTariffBlockLegendDisclaimer.TextFormatted = Html.FromHtml(message);
                                }

                                txtTariffBlockLegendDisclaimer = LinkRedirectionUtils
                                    .Create(this.Activity, "")
                                    .SetTextView(txtTariffBlockLegendDisclaimer)
                                    .SetMessage(message, new Android.Graphics.Color(ContextCompat.GetColor(this.Activity, Resource.Color.sunGlow)))
                                    .Build()
                                    .GetProcessedTextView();
                            }
                        }
                    }

                    isChangeVirtualHeightNeed = true;
                    SetVirtualHeightParams(6f);
                }
            }
            catch (System.Exception e)
            {
                tariffBlockLegendRecyclerView.Visibility = ViewStates.Gone;
                tariffBlockLegendDisclaimerLayout.Visibility = ViewStates.Gone;
                Utility.LoggingNonFatalError(e);
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


        public override void OnActivityResult(int requestCode, int resultCode, Intent data)
        {
            if (requestCode == DashboardHomeActivity.PAYMENT_RESULT_CODE)
            {
                if (resultCode == (int)Result.Ok)
                {
                    ((DashboardHomeActivity)Activity).OnTapRefresh();
                }
            }
            else if (requestCode == SSMR_METER_HISTORY_ACTIVITY_CODE)
            {
                if (resultCode == (int)Result.Ok)
                {
                    this.userActionsListener.OnTapRefresh();
                }
            }
        }

        public void ShowNewAccountView(string contentTxt)
        {
            try
            {
                Activity.RunOnUiThread(() =>
                {
                    try
                    {
                        refreshLayout.Visibility = ViewStates.Gone;
                        newAccountLayout.Visibility = ViewStates.Visible;
                        notNewAccountLayout.Visibility = ViewStates.Gone;
                        layoutSMSegmentGroup.Visibility = ViewStates.Gone;
                        allGraphLayout.Visibility = ViewStates.Visible;
                        SetNewAccountLayoutParams();
                        StopAddressShimmer();
                        StopRangeShimmer();
                        StopGraphShimmer();
                        HideSMStatisticCard();
                        energyTipsView.Visibility = ViewStates.Gone;

                        string defaultMessage = Utility.GetLocalizedLabel("Usage", "emptyDataMsg");

                        if (isREAccount)
                        {
                            defaultMessage = "Welcome! You can track your generated electricity here after you receive your first Payment Advice.";
                        }

                        if (Build.VERSION.SdkInt >= BuildVersionCodes.N)
                        {
                            if (string.IsNullOrEmpty(contentTxt))
                            {
                                newAccountContent.TextFormatted = Html.FromHtml(defaultMessage, FromHtmlOptions.ModeLegacy);
                            }
                            else
                            {
                                newAccountContent.TextFormatted = Html.FromHtml(contentTxt, FromHtmlOptions.ModeLegacy);
                            }
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(contentTxt))
                            {
                                newAccountContent.TextFormatted = Html.FromHtml(defaultMessage);
                            }
                            else
                            {
                                newAccountContent.TextFormatted = Html.FromHtml(contentTxt);
                            }
                        }

                        isChangeVirtualHeightNeed = true;
                        SetVirtualHeightParams(6f);
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

        public void ShowNoInternet(string contentTxt, string buttonTxt)
        {
            try
            {
                Activity.RunOnUiThread(() =>
                {
                    try
                    {
                        refreshLayout.Visibility = ViewStates.Visible;
                        newAccountLayout.Visibility = ViewStates.Gone;
                        allGraphLayout.Visibility = ViewStates.Gone;
                        smStatisticContainer.Visibility = ViewStates.Gone;
                        if (isREAccount || isSMR)
                        {

                        }
                        else
                        {
                            rootView.SetBackgroundResource(0);
                            scrollViewContent.SetBackgroundResource(0);
                            dashboard_bottom_view.SetBackgroundResource(0);
                            try
                            {
                                ((DashboardHomeActivity)Activity).SetStatusBarBackground(Resource.Drawable.NewHorizontalGradientBackground);
                                ((DashboardHomeActivity)Activity).UnsetToolbarBackground();
                            }
                            catch (System.Exception e)
                            {
                                Utility.LoggingNonFatalError(e);
                            }
                        }

                        refresh_image.SetImageResource(Resource.Drawable.refresh_white);
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

                        isChangeVirtualHeightNeed = true;
                        SetVirtualHeightParams(6f);
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

        public void OnMissingReadingClick()
        {
            try
            {
                MaterialDialog mDialog = new MaterialDialog.Builder(Activity)
                    .CustomView(Resource.Layout.CustomDialogOneButtonLayout, false)
                    .Cancelable(false)
                    .CanceledOnTouchOutside(false)
                    .Build();

                View dialogView = mDialog.Window.DecorView;
                dialogView.SetBackgroundResource(Android.Resource.Color.Transparent);

                TextView txtTitle = mDialog.FindViewById<TextView>(Resource.Id.txtTitle);
                TextView txtMessage = mDialog.FindViewById<TextView>(Resource.Id.txtMessage);
                TextView btnGotIt = mDialog.FindViewById<TextView>(Resource.Id.txtBtnFirst);
                txtMessage.MovementMethod = new ScrollingMovementMethod();

                txtMessage.Text = Utility.GetLocalizedLabel("Usage", "missedReadTitle");
                txtTitle.Text = Utility.GetLocalizedLabel("Usage", "missedReadMsg");
                btnGotIt.Text = Utility.GetLocalizedCommonLabel("gotIt");

                TextViewUtils.SetTextSize14(txtTitle, txtMessage);
                TextViewUtils.SetTextSize16(btnGotIt);

                foreach (SMUsageHistoryData.SmartMeterToolTips costValue in selectedSMHistoryData.ToolTips)
                {
                    if (costValue.Type == Constants.MISSING_READING_KEY)
                    {
                        if (costValue.Message != null && costValue.Message.Count > 0)
                        {
                            string textMessage = "";
                            foreach (string stringValue in costValue.Message)
                            {
                                textMessage += stringValue;
                            }

                            if (Build.VERSION.SdkInt >= BuildVersionCodes.N)
                            {
                                txtMessage.TextFormatted = Html.FromHtml(textMessage, FromHtmlOptions.ModeLegacy);
                            }
                            else
                            {
                                txtMessage.TextFormatted = Html.FromHtml(textMessage);
                            }
                        }

                        txtTitle.Text = costValue.Title;
                        btnGotIt.Text = costValue.SMBtnText;
                    }
                }

                TextViewUtils.SetMuseoSans500Typeface(txtTitle, btnGotIt);
                TextViewUtils.SetMuseoSans300Typeface(txtMessage);
                btnGotIt.Click += delegate
                {
                    mDialog.Dismiss();
                };

                if (IsActive())
                {
                    mDialog.Show();
                }
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        internal float GetMaxRMValues()
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
                            if (MonthData.AmountTotal > 0)
                            {
                                if (MonthData.TariffBlocksList != null && MonthData.TariffBlocksList.Count > 0)
                                {
                                    for (int i = 0; i < MonthData.TariffBlocksList.Count; i++)
                                    {
                                        valTotal += System.Math.Abs((float)MonthData.TariffBlocksList[i].Amount);
                                    }
                                }

                                if (valTotal <= 0)
                                {
                                    valTotal = System.Math.Abs((float)MonthData.AmountTotal);
                                }
                            }

                            if (!isREAccount && MonthData.DPCIndicator)
                            {
                                valTotal = MonthData.AmountTotal < 0 ? 0 : System.Math.Abs((float)MonthData.AmountTotal);
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
                            if (!isREAccount && MonthData.DPCIndicator)
                            {
                                float valTotal = MonthData.AmountTotal < 0 ? 0 : System.Math.Abs((float)MonthData.AmountTotal);

                                if (System.Math.Abs(valTotal) > val)
                                {
                                    val = System.Math.Abs((float)valTotal);
                                }
                            }
                            else if (!isREAccount)
                            {
                                if (MonthData.AmountTotal > 0 && System.Math.Abs(MonthData.AmountTotal) > val)
                                {
                                    val = System.Math.Abs((float)MonthData.AmountTotal);
                                }
                            }
                            else
                            {
                                if (System.Math.Abs(MonthData.UsageTotal) > val)
                                {
                                    val = System.Math.Abs((float)MonthData.UsageTotal);
                                }
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

                                if (MonthData.AmountTotal > 0)
                                {
                                    if (MonthData.TariffBlocksList != null && MonthData.TariffBlocksList.Count > 0)
                                    {
                                        for (int i = 0; i < MonthData.TariffBlocksList.Count; i++)
                                        {
                                            valTotal += System.Math.Abs((float)MonthData.TariffBlocksList[i].Amount);
                                        }
                                    }

                                    if (valTotal <= 0)
                                    {
                                        valTotal = System.Math.Abs((float)MonthData.AmountTotal);
                                    }
                                }

                                if (MonthData.DPCIndicator)
                                {
                                    valTotal = MonthData.AmountTotal < 0 ? 0 : System.Math.Abs((float)MonthData.AmountTotal);
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
                                if (MonthData.DPCIndicator)
                                {
                                    float valTotal = MonthData.AmountTotal < 0 ? 0 : System.Math.Abs((float)MonthData.AmountTotal);

                                    if (System.Math.Abs(valTotal) > val)
                                    {
                                        val = System.Math.Abs((float)valTotal);
                                    }
                                }
                                else
                                {
                                    if (MonthData.AmountTotal > 0 && System.Math.Abs(MonthData.AmountTotal) > val)
                                    {
                                        val = System.Math.Abs((float)MonthData.AmountTotal);
                                    }
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
                            if (MonthData.TariffBlocksList != null && MonthData.TariffBlocksList.Count > 0)
                            {
                                for (int i = 0; i < MonthData.TariffBlocksList.Count; i++)
                                {
                                    valTotal += System.Math.Abs((float)MonthData.TariffBlocksList[i].Usage);
                                }
                            }

                            if (valTotal <= 0)
                            {
                                valTotal = System.Math.Abs((float)MonthData.UsageTotal);
                            }

                            if (!isREAccount && MonthData.DPCIndicator)
                            {
                                valTotal = 0;
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
                            if (!isREAccount && MonthData.DPCIndicator)
                            {
                                float valTotal = MonthData.AmountTotal < 0 ? 0 : System.Math.Abs((float)MonthData.AmountTotal);

                                if (System.Math.Abs(valTotal) > val)
                                {
                                    val = System.Math.Abs((float)valTotal);
                                }
                            }
                            else
                            {
                                if (System.Math.Abs(MonthData.UsageTotal) > val)
                                {
                                    val = System.Math.Abs((float)MonthData.UsageTotal);
                                }
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
                                if (MonthData.TariffBlocksList != null && MonthData.TariffBlocksList.Count > 0)
                                {
                                    for (int i = 0; i < MonthData.TariffBlocksList.Count; i++)
                                    {
                                        valTotal += System.Math.Abs((float)MonthData.TariffBlocksList[i].Usage);
                                    }
                                }

                                if (valTotal <= 0)
                                {
                                    valTotal = System.Math.Abs((float)MonthData.UsageTotal);
                                }

                                if (!isREAccount && MonthData.DPCIndicator)
                                {
                                    valTotal = 0;
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
                                if (!isREAccount && MonthData.DPCIndicator)
                                {
                                    float valTotal = MonthData.AmountTotal < 0 ? 0 : System.Math.Abs((float)MonthData.AmountTotal);

                                    if (System.Math.Abs(valTotal) > val)
                                    {
                                        val = System.Math.Abs((float)valTotal);
                                    }
                                }
                                else
                                {
                                    if (System.Math.Abs(MonthData.UsageTotal) > val)
                                    {
                                        val = System.Math.Abs((float)MonthData.UsageTotal);
                                    }
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

            try
            {
                ShowBackButton(true);
                ((DashboardHomeActivity)this.Activity).HideAccountName();
                ((DashboardHomeActivity)this.Activity).SetAccountToolbarTitle(selectedAccount.AccountNickName);
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }


            try
            {
                if (SMRPopUpUtils.GetFromUsageFlag())
                {
                    SMRPopUpUtils.SetFromUsageFlag(false);
                    if (SMRPopUpUtils.GetFromUsageSubmitSuccessfulFlag())
                    {
                        SMRPopUpUtils.SetFromUsageSubmitSuccessfulFlag(false);
                        this.userActionsListener.OnTapRefresh();
                    }
                }
                else
                {
                    if (SMRPopUpUtils.GetFromUsageSubmitSuccessfulFlag())
                    {
                        SMRPopUpUtils.SetFromUsageSubmitSuccessfulFlag(false);
                    }
                }
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

            Handler h = new Handler();
            Action myAction = () =>
            {
                try
                {
                    NewAppTutorialUtils.ForceCloseNewAppTutorial();
                    if (this.mPresenter != null)
                    {
                        this.mPresenter.OnCheckToCallDashboardTutorial();
                    }
                }
                catch (System.Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                }
            };
            h.PostDelayed(myAction, 50);

            try
            {
                if (isGoToBillingDetail)
                {
                    isGoToBillingDetail = false;
                    if (!Utility.IsEnablePayment())
                    {
                        isPaymentDown = true;
                    }
                    else
                    {
                        isPaymentDown = false;
                    }

                    if (isPaymentDown)
                    {
                        DisablePayButton();
                    }
                    else
                    {
                        EnablePayButton();
                    }
                }
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void ShowBackButton(bool flag)
        {
            var act = this.Activity as AppCompatActivity;

            var actionBar = act.SupportActionBar;
            actionBar.SetDisplayHomeAsUpEnabled(flag);
            actionBar.SetDisplayShowHomeEnabled(flag);
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

                mNoInternetSnackbar = Snackbar.Make(rootView, Utility.GetLocalizedErrorLabel("noDataConnectionMessage"), Snackbar.LengthIndefinite)
                .SetAction(Utility.GetLocalizedCommonLabel("close"), delegate
                {

                    mNoInternetSnackbar.Dismiss();
                }
                );
                View v = mNoInternetSnackbar.View;
                TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
                tv.SetMaxLines(5);
                mNoInternetSnackbar.Show();
                this.SetIsClicked(false);
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

                mLoadBillSnackBar = Snackbar.Make(rootView, Utility.GetLocalizedErrorLabel("defaultErrorMessage"), Snackbar.LengthIndefinite)
                .SetAction(Utility.GetLocalizedCommonLabel("close"), delegate
                {
                    mLoadBillSnackBar.Dismiss();
                }
                );
                View v = mLoadBillSnackBar.View;
                TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
                tv.SetMaxLines(5);
                mLoadBillSnackBar.Show();
                this.SetIsClicked(false);
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

        public void ShowAmountDue(AccountDueAmountData accountDueAmount, bool isPendingPayment)
        {
            try
            {
                mIsPendingPayment = isPendingPayment;

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
                                        DisablePayButton();
                                    }
                                    else
                                    {
                                        if (accountDueAmount.ShowEppToolTip == true)
                                        {
                                            ShowEpp();
                                            isChangeVirtualHeightNeed = true;
                                            SetVirtualHeightParams(6f);
                                        }
                                        else
                                        {
                                            HideEpp();
                                        }
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
                                        selectedAccount.AmtCustBal = accountDueAmount.AmountDue;
                                        double calAmt = selectedAccount.AmtCustBal * -1;

                                        txtTotalPayable.Text = decimalFormat.Format(System.Math.Abs(selectedAccount.AmtCustBal));
                                        reTotalPayable.Text = decimalFormat.Format(System.Math.Abs(selectedAccount.AmtCustBal));
                                        txtReNoPayable.Text = decimalFormat.Format(System.Math.Abs(selectedAccount.AmtCustBal));

                                        int incrementDays = int.Parse(accountDueAmount.IncrementREDueDateByDays == null ? "0" : accountDueAmount.IncrementREDueDateByDays);
                                        Constants.RE_ACCOUNT_DATE_INCREMENT_DAYS = incrementDays;
                                        Calendar c = Calendar.Instance;
                                        c.Time = d;
                                        c.Add(CalendarField.Date, incrementDays);
                                        SimpleDateFormat df = new SimpleDateFormat("dd MMM", LocaleUtils.GetCurrentLocale());
                                        Date newDate = c.Time;
                                        string dateString = df.Format(newDate);
                                        if (calAmt <= 0)
                                        {
                                            rePayableLayout.Visibility = ViewStates.Gone;
                                            reNoPayableLayout.Visibility = ViewStates.Visible;
                                            if (System.Math.Abs(calAmt) < 0.0001)
                                            {
                                                txtReNoPayableTitle.Text = Utility.GetLocalizedLabel("Usage", "myEarnings");
                                                txtReNoPayable.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(this.Activity, Resource.Color.charcoalGrey)));
                                                txtReNoPayableCurrency.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(this.Activity, Resource.Color.charcoalGrey)));
                                            }
                                            else
                                            {
                                                txtReNoPayableTitle.Text = Utility.GetLocalizedLabel("Usage", "beenPaidExtra");
                                                txtReNoPayable.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(this.Activity, Resource.Color.charcoalGrey)));
                                                txtReNoPayableCurrency.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(this.Activity, Resource.Color.charcoalGrey)));
                                            }
                                        }
                                        else if (calAmt > 0)
                                        {
                                            rePayableLayout.Visibility = ViewStates.Visible;
                                            reNoPayableLayout.Visibility = ViewStates.Gone;
                                            txtDueDate.Text = Utility.GetLocalizedLabel("Usage", "iWillGetBy") + " " + GetString(Resource.String.dashboard_chartview_due_date_wildcard, dateFormatter.Format(newDate));
                                            reDueDate.Text = Utility.GetLocalizedLabel("Usage", "iWillGetBy") + " " + dateString;
                                        }

                                        if (accountDueAmount.AmountDue.ToString().Length > 10)
                                        {
                                            txtReNoPayableTitle.SetMaxWidth((int)DPUtils.ConvertDPToPx(120f));
                                            reDueDate.SetMaxWidth((int)DPUtils.ConvertDPToPx(120f));
                                            txtDueDate.SetMaxWidth((int)DPUtils.ConvertDPToPx(120f));
                                            reTotalPayableTitle.SetMaxWidth((int)DPUtils.ConvertDPToPx(120f));
                                            txtTotalPayableTitle.SetMaxWidth((int)DPUtils.ConvertDPToPx(120f));

                                        }
                                        else if (accountDueAmount.AmountDue.ToString().Length > 5)
                                        {
                                            txtReNoPayableTitle.SetMaxWidth((int)DPUtils.ConvertDPToPx(170f));
                                            reDueDate.SetMaxWidth((int)DPUtils.ConvertDPToPx(170f));
                                            txtDueDate.SetMaxWidth((int)DPUtils.ConvertDPToPx(170f));
                                            reTotalPayableTitle.SetMaxWidth((int)DPUtils.ConvertDPToPx(170f));
                                            txtTotalPayableTitle.SetMaxWidth((int)DPUtils.ConvertDPToPx(170f));
                                        }
                                        else
                                        {
                                            txtReNoPayableTitle.SetMaxWidth((int)DPUtils.ConvertDPToPx(200f));
                                            reDueDate.SetMaxWidth((int)DPUtils.ConvertDPToPx(200f));
                                            txtDueDate.SetMaxWidth((int)DPUtils.ConvertDPToPx(200f));
                                            reTotalPayableTitle.SetMaxWidth((int)DPUtils.ConvertDPToPx(200f));
                                            txtTotalPayableTitle.SetMaxWidth((int)DPUtils.ConvertDPToPx(200f));
                                        }

                                    }
                                    else
                                    {
                                        txtTotalPayable.Text = decimalFormat.Format(accountDueAmount.AmountDue);
                                        selectedAccount.AmtCustBal = accountDueAmount.AmountDue;
                                        double calAmt = selectedAccount.AmtCustBal;
                                        if (calAmt <= 0.00)
                                        {
                                            totalPayableLayout.Visibility = ViewStates.Gone;
                                            noPayableLayout.Visibility = ViewStates.Visible;
                                            if (System.Math.Abs(calAmt) < 0.0001)
                                            {
                                                txtNoPayableTitle.Text = Utility.GetLocalizedLabel("Usage", "clearedAllBills");
                                                txtNoPayable.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(this.Activity, Resource.Color.charcoalGrey)));
                                                txtNoPayableCurrency.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(this.Activity, Resource.Color.charcoalGrey)));
                                            }
                                            else
                                            {
                                                txtNoPayableTitle.Text = Utility.GetLocalizedLabel("Usage", "paidExtra");
                                                txtNoPayable.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(this.Activity, Resource.Color.freshGreen)));
                                                txtNoPayableCurrency.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(this.Activity, Resource.Color.freshGreen)));
                                            }
                                            txtNoPayable.Text = decimalFormat.Format(System.Math.Abs(accountDueAmount.AmountDue));
                                            txtDueDate.Text = "- -";
                                        }
                                        else
                                        {
                                            totalPayableLayout.Visibility = ViewStates.Visible;
                                            noPayableLayout.Visibility = ViewStates.Gone;
                                            txtDueDate.Text = Utility.GetLocalizedLabel("Usage", "by") + " " + GetString(Resource.String.dashboard_chartview_due_date_wildcard, dateFormatter.Format(d));
                                        }

                                        if (isPendingPayment)
                                        {
                                            totalPayableLayout.Visibility = ViewStates.Gone;
                                            noPayableLayout.Visibility = ViewStates.Visible;
                                            txtNoPayableTitle.Text = Utility.GetLocalizedCommonLabel("paymentPendingMsg");
                                            txtNoPayable.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(this.Activity, Resource.Color.lightOrange)));
                                            txtNoPayableCurrency.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(this.Activity, Resource.Color.lightOrange)));
                                            txtNoPayable.Text = decimalFormat.Format(System.Math.Abs(accountDueAmount.AmountDue));
                                            txtDueDate.Text = "- -";
                                        }

                                        if (accountDueAmount.AmountDue.ToString().Length > 10)
                                        {
                                            txtNoPayableTitle.SetMaxWidth((int)DPUtils.ConvertDPToPx(150f));
                                            txtTotalPayableTitle.SetMaxWidth((int)DPUtils.ConvertDPToPx(150f));
                                            txtDueDate.SetMaxWidth((int)DPUtils.ConvertDPToPx(150f));
                                        }
                                        else if (accountDueAmount.AmountDue.ToString().Length > 5)
                                        {
                                            txtNoPayableTitle.SetMaxWidth((int)DPUtils.ConvertDPToPx(190f));
                                            txtTotalPayableTitle.SetMaxWidth((int)DPUtils.ConvertDPToPx(190f));
                                            txtDueDate.SetMaxWidth((int)DPUtils.ConvertDPToPx(190f));
                                        }
                                        else
                                        {
                                            txtNoPayableTitle.SetMaxWidth((int)DPUtils.ConvertDPToPx(220f));
                                            txtTotalPayableTitle.SetMaxWidth((int)DPUtils.ConvertDPToPx(220f));
                                            txtDueDate.SetMaxWidth((int)DPUtils.ConvertDPToPx(220f));
                                        }
                                    }
                                }
                                else
                                {
                                    ShowAmountDueFailed();
                                }
                            }
                            else
                            {
                                if (selectedAccount != null)
                                {
                                    if (isPaymentDown)
                                    {
                                        DisablePayButton();
                                    }
                                    else
                                    {
                                        if (accountDueAmount.ShowEppToolTip == true)
                                        {
                                            ShowEpp();
                                            isChangeVirtualHeightNeed = true;
                                            SetVirtualHeightParams(6f);
                                        }
                                        else
                                        {
                                            HideEpp();
                                        }
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
                                        selectedAccount.AmtCustBal = accountDueAmount.AmountDue;
                                        double calAmt = selectedAccount.AmtCustBal * -1;
                                        txtTotalPayable.Text = decimalFormat.Format(System.Math.Abs(selectedAccount.AmtCustBal));
                                        reTotalPayable.Text = decimalFormat.Format(System.Math.Abs(selectedAccount.AmtCustBal));
                                        txtReNoPayable.Text = decimalFormat.Format(System.Math.Abs(selectedAccount.AmtCustBal));
                                        if (calAmt <= 0)
                                        {
                                            rePayableLayout.Visibility = ViewStates.Gone;
                                            reNoPayableLayout.Visibility = ViewStates.Visible;
                                            if (System.Math.Abs(calAmt) < 0.0001)
                                            {
                                                txtReNoPayableTitle.Text = Utility.GetLocalizedLabel("Usage", "myEarnings");
                                                txtReNoPayable.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(this.Activity, Resource.Color.charcoalGrey)));
                                                txtReNoPayableCurrency.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(this.Activity, Resource.Color.charcoalGrey)));
                                            }
                                            else
                                            {
                                                txtReNoPayableTitle.Text = Utility.GetLocalizedLabel("Usage", "beenPaidExtra");
                                                txtReNoPayable.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(this.Activity, Resource.Color.charcoalGrey)));
                                                txtReNoPayableCurrency.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(this.Activity, Resource.Color.charcoalGrey)));
                                            }
                                        }
                                        else
                                        {
                                            ShowAmountDueFailed();
                                        }
                                        if (accountDueAmount.AmountDue.ToString().Length > 10)
                                        {
                                            txtTotalPayableTitle.SetMaxWidth((int)DPUtils.ConvertDPToPx(120f));
                                            reTotalPayableTitle.SetMaxWidth((int)DPUtils.ConvertDPToPx(120f));
                                            reDueDate.SetMaxWidth((int)DPUtils.ConvertDPToPx(120f));
                                        }
                                        else if (accountDueAmount.AmountDue.ToString().Length > 5)
                                        {
                                            txtTotalPayableTitle.SetMaxWidth((int)DPUtils.ConvertDPToPx(170f));
                                            reTotalPayableTitle.SetMaxWidth((int)DPUtils.ConvertDPToPx(170f));
                                            reDueDate.SetMaxWidth((int)DPUtils.ConvertDPToPx(170f));
                                        }
                                        else
                                        {
                                            txtTotalPayableTitle.SetMaxWidth((int)DPUtils.ConvertDPToPx(200f));
                                            reTotalPayableTitle.SetMaxWidth((int)DPUtils.ConvertDPToPx(200f));
                                            reDueDate.SetMaxWidth((int)DPUtils.ConvertDPToPx(200f));
                                        }
                                    }
                                    else
                                    {
                                        selectedAccount.AmtCustBal = accountDueAmount.AmountDue;
                                        double calAmt = selectedAccount.AmtCustBal;
                                        if (calAmt <= 0.00)
                                        {
                                            totalPayableLayout.Visibility = ViewStates.Gone;
                                            noPayableLayout.Visibility = ViewStates.Visible;
                                            txtTotalPayable.Text = decimalFormat.Format(accountDueAmount.AmountDue);
                                            if (System.Math.Abs(calAmt) < 0.0001)
                                            {
                                                txtNoPayableTitle.Text = Utility.GetLocalizedLabel("Usage", "clearedAllBills");
                                                txtNoPayable.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(this.Activity, Resource.Color.charcoalGrey)));
                                                txtNoPayableCurrency.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(this.Activity, Resource.Color.charcoalGrey)));
                                            }
                                            else
                                            {
                                                txtNoPayableTitle.Text = Utility.GetLocalizedLabel("Usage", "paidExtra");
                                                txtNoPayable.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(this.Activity, Resource.Color.freshGreen)));
                                                txtNoPayableCurrency.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(this.Activity, Resource.Color.freshGreen)));
                                            }
                                            txtNoPayable.Text = decimalFormat.Format(System.Math.Abs(accountDueAmount.AmountDue));
                                            txtDueDate.Text = "- -";

                                            if (isPendingPayment)
                                            {
                                                totalPayableLayout.Visibility = ViewStates.Gone;
                                                noPayableLayout.Visibility = ViewStates.Visible;
                                                txtNoPayableTitle.Text = Utility.GetLocalizedCommonLabel("paymentPendingMsg");
                                                txtNoPayable.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(this.Activity, Resource.Color.lightOrange)));
                                                txtNoPayableCurrency.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(this.Activity, Resource.Color.lightOrange)));
                                                txtNoPayable.Text = decimalFormat.Format(System.Math.Abs(accountDueAmount.AmountDue));
                                                txtDueDate.Text = "- -";
                                            }

                                            if (accountDueAmount.AmountDue.ToString().Length > 10)
                                            {
                                                txtNoPayableTitle.SetMaxWidth((int)DPUtils.ConvertDPToPx(150f));
                                            }
                                            else if (accountDueAmount.AmountDue.ToString().Length > 5)
                                            {
                                                txtNoPayableTitle.SetMaxWidth((int)DPUtils.ConvertDPToPx(190f));
                                            }
                                            else
                                            {
                                                txtNoPayableTitle.SetMaxWidth((int)DPUtils.ConvertDPToPx(220f));
                                            }
                                        }
                                        else
                                        {
                                            ShowAmountDueFailed();
                                        }
                                    }
                                }
                                else
                                {
                                    ShowAmountDueFailed();
                                }
                                // txtWhyThisAmt.Visibility = ViewStates.Gone;
                            }
                        }
                        catch (System.Exception ne)
                        {
                            ShowAmountDueFailed();
                            Utility.LoggingNonFatalError(ne);
                        }
                    });
                }
                catch (System.Exception e)
                {
                    ShowAmountDueFailed();
                    Utility.LoggingNonFatalError(e);
                }
            }
            catch (System.Exception e)
            {
                ShowAmountDueFailed();
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
                .SetAction(Utility.GetLocalizedCommonLabel("common"), delegate
                {
                    mSmartMeterError.Dismiss();
                }
                );
                View v = mSmartMeterError.View;
                TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
                tv.SetMaxLines(5);
                mSmartMeterError.Show();
                this.SetIsClicked(false);
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

                mDisconnectionSnackbar = Snackbar.Make(rootView, Utility.GetLocalizedErrorLabel("defaultErrorMessage"), Snackbar.LengthIndefinite)
                .SetAction(Utility.GetLocalizedCommonLabel("retry"), delegate
                {

                    mDisconnectionSnackbar.Dismiss();
                }
                );
                View v = mDisconnectionSnackbar.View;
                TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
                tv.SetMaxLines(5);
                mDisconnectionSnackbar.Show();
                this.SetIsClicked(false);
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
                View v = mSMRSnackbar.View;
                TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
                tv.SetMaxLines(5);
                mSMRSnackbar.Show();
                this.SetIsClicked(false);
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

        public void ShowEpp()
        {
            lblinfoLabelEPP.Text = Utility.GetLocalizedCommonLabel("eppToolTipTitle");
            infoLabelEPP.Visibility = ViewStates.Visible;
            //btnPay.Background = ContextCompat.GetDrawable(this.Activity, Resource.Drawable.green_button_background);
        }

        public void HideEpp()
        {
            infoLabelEPP.Visibility = ViewStates.Gone;
            //btnPay.Background = ContextCompat.GetDrawable(this.Activity, Resource.Drawable.green_button_background);
        }

        public void DisablePayButton()
        {
            btnPay.Enabled = false;
            btnPay.Background = ContextCompat.GetDrawable(this.Activity, Resource.Drawable.silver_chalice_button_background);

        }

        public void EnableViewBillButton()
        {
            try
            {
                Activity.RunOnUiThread(() =>
                {
                    try
                    {
                        btnViewBill.Enabled = true;
                        btnReView.Enabled = true;
                        btnViewBill.SetTextColor(ContextCompat.GetColorStateList(this.Activity, Resource.Color.freshGreen));
                        btnReView.SetTextColor(ContextCompat.GetColorStateList(this.Activity, Resource.Color.freshGreen));
                        btnViewBill.Background = ContextCompat.GetDrawable(this.Activity, Resource.Drawable.light_green_outline_button_background);
                        btnReView.Background = ContextCompat.GetDrawable(this.Activity, Resource.Drawable.light_green_outline_button_background);
                    }
                    catch (System.Exception e)
                    {
                        Utility.LoggingNonFatalError(e);
                    }
                });
            }
            catch (System.Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }

        public void DisableViewBillButton()
        {
            try
            {
                Activity.RunOnUiThread(() =>
                {
                    try
                    {
                        btnViewBill.Enabled = false;
                        btnReView.Enabled = false;
                        btnViewBill.Background = ContextCompat.GetDrawable(this.Activity, Resource.Drawable.silver_chalice_button_outline);
                        btnReView.Background = ContextCompat.GetDrawable(this.Activity, Resource.Drawable.silver_chalice_button_outline);
                        btnViewBill.SetTextColor(ContextCompat.GetColorStateList(this.Activity, Resource.Color.silverChalice));
                        btnReView.SetTextColor(ContextCompat.GetColorStateList(this.Activity, Resource.Color.silverChalice));
                    }
                    catch (System.Exception ex)
                    {
                        Utility.LoggingNonFatalError(ex);
                    }
                });
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void ShowAmountDueFailed()
        {
            try
            {
                Activity.RunOnUiThread(() =>
                {
                    try
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
                    }
                    catch (System.Exception e)
                    {
                        Utility.LoggingNonFatalError(e);
                    }
                });
            }
            catch (System.Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
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
                if (!string.IsNullOrEmpty(accountStatusData.DisconnectionStatus) && accountStatusData.DisconnectionStatus.ToUpper() != Constants.ENERGY_DISCONNECTION_KEY)
                {
                    energyDisconnectionButton.Visibility = ViewStates.Visible;
                    string accountStatusMessage = accountStatusData?.AccountStatusMessage ?? Utility.GetLocalizedCommonLabel("disconnectionMsg");
                    string whatDoesThisMeanLabel = accountStatusData?.AccountStatusModalTitle ?? Utility.GetLocalizedLabel("Usage", "missedReadTitle");
                    string whatDoesThisToolTipMessage = accountStatusData?.AccountStatusModalMessage ?? Utility.GetLocalizedLabel("Usage", "disconnectionMsg");
                    string whatDoesThisToolTipBtnLabel = accountStatusData?.AccountStatusModalBtnText ?? Utility.GetLocalizedCommonLabel("gotIt");
                    if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.N)
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

                    if (isSMR)
                    {
                        rootView.SetBackgroundResource(0);
                        scrollViewContent.SetBackgroundResource(0);
                        dashboard_bottom_view.SetBackgroundResource(0);
                        try
                        {
                            ((DashboardHomeActivity)Activity).SetStatusBarBackground(Resource.Drawable.NewHorizontalGradientBackground);
                            ((DashboardHomeActivity)Activity).UnsetToolbarBackground();
                        }
                        catch (System.Exception e)
                        {
                            Utility.LoggingNonFatalError(e);
                        }
                        isSMR = false;
                        isChangeBackgroundNeeded = false;
                        isChangeVirtualHeightNeed = true;
                        SetVirtualHeightParams(6f);
                    }
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
                    StopSSMRDashboardViewShimmer();
                    smrResponse = response;
                    MyTNBAccountManagement.GetInstance().SetAccountActivityInfo(new SMRAccountActivityInfo(selectedAccount.AccountNum, smrResponse));
                    SMRPopUpUtils.OnSetSMRActivityInfoResponse(response);
                    MyTNBAppToolTipData.SetSMRActivityInfo(response.Response);
                    try
                    {
                        Activity.RunOnUiThread(() =>
                        {
                            try
                            {
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
                                    if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.N)
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
                            }
                            catch (System.Exception ex)
                            {
                                Utility.LoggingNonFatalError(ex);
                            }
                        });
                    }
                    catch (System.Exception e)
                    {
                        Utility.LoggingNonFatalError(e);
                    }
                }
                else
                {
                    HideSSMRDashboardView();
                }

                isChangeVirtualHeightNeed = true;
                SetVirtualHeightParams(6f);
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void HideSSMRDashboardView()
        {
            StopSSMRDashboardViewShimmer();
            ssmrHistoryContainer.Visibility = ViewStates.Gone;
            if (isSMR)
            {
                isSMR = false;
                isChangeBackgroundNeeded = false;
                rootView.SetBackgroundResource(0);
                scrollViewContent.SetBackgroundResource(0);
                dashboard_bottom_view.SetBackgroundResource(0);
                isChangeVirtualHeightNeed = true;
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
            }
        }

        private void StartSSMRDashboardViewShimmer()
        {
            ssmrHistoryContainer.Visibility = ViewStates.Visible;
            ssmr_account_message.Visibility = ViewStates.Gone;
            btnReadingHistory.Enabled = false;
            btnReadingHistory.Background = ContextCompat.GetDrawable(this.Activity, Resource.Drawable.silver_chalice_button_outline);
            btnReadingHistory.SetTextColor(ContextCompat.GetColorStateList(this.Activity, Resource.Color.silverChalice));
            btnReadingHistory.Text = Utility.GetLocalizedLabel("Usage", "viewReadHistory");
            // Start SSMR Shimmer
            var shimmerBuilder = ShimmerUtils.ShimmerBuilderConfig();
            if (shimmerBuilder != null)
            {
                shimmerSSMRImg.SetShimmer(shimmerBuilder?.Build());
                shimmerSSMRTitle.SetShimmer(shimmerBuilder?.Build());
                shimmerSSMRMessage.SetShimmer(shimmerBuilder?.Build());
            }
            shimmerSSMRImg.StartShimmer();
            shimmerSSMRTitle.StartShimmer();
            shimmerSSMRMessage.StartShimmer();

            ssmr_shimmer_layout.Visibility = ViewStates.Visible;
        }

        private void StopSSMRDashboardViewShimmer()
        {
            ssmr_shimmer_layout.Visibility = ViewStates.Gone;
            // Stop SSMR Shimmer
            if (shimmerSSMRImg.IsShimmerStarted)
            {
                shimmerSSMRImg.StopShimmer();
            }
            if (shimmerSSMRTitle.IsShimmerStarted)
            {
                shimmerSSMRTitle.StopShimmer();
            }
            if (shimmerSSMRMessage.IsShimmerStarted)
            {
                shimmerSSMRMessage.StopShimmer();
            }

            ssmr_account_message.Visibility = ViewStates.Visible;
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
                    if (t == 0)
                    {
                        requireScroll = false;
                        if (!isTutorialShow)
                        {
                            if (!isClickedShowTariff)
                            {
                                bottomSheetBehavior.State = BottomSheetBehavior.StateExpanded;
                                shadowLayout.SetBackgroundResource(Resource.Drawable.scroll_indicator);
                            }
                            if (!isToggleTariff)
                            {
                                if (isSMAccount || isSMR)
                                {
                                    if (isSMAccount)
                                    {
                                        if (smStatisticContainer.Visibility == ViewStates.Visible)
                                        {
                                            rootView.SetBackgroundResource(0);
                                            scrollViewContent.SetBackgroundResource(0);
                                            dashboard_bottom_view.SetBackgroundResource(0);
                                            try
                                            {
                                                ((DashboardHomeActivity)Activity).SetStatusBarBackground(Resource.Drawable.UsageGradientBackground);
                                                ((DashboardHomeActivity)Activity).UnsetToolbarBackground();
                                            }
                                            catch (System.Exception e)
                                            {
                                                Utility.LoggingNonFatalError(e);
                                            }
                                            smStatisticContainer.Visibility = ViewStates.Invisible;
                                        }
                                    }
                                    else if (isSMR)
                                    {
                                        if (ssmrHistoryContainer.Visibility == ViewStates.Visible)
                                        {
                                            rootView.SetBackgroundResource(0);
                                            scrollViewContent.SetBackgroundResource(0);
                                            dashboard_bottom_view.SetBackgroundResource(0);
                                            try
                                            {
                                                ((DashboardHomeActivity)Activity).SetStatusBarBackground(Resource.Drawable.UsageGradientBackground);
                                                ((DashboardHomeActivity)Activity).UnsetToolbarBackground();
                                            }
                                            catch (System.Exception e)
                                            {
                                                Utility.LoggingNonFatalError(e);
                                            }
                                            ssmrHistoryContainer.Visibility = ViewStates.Invisible;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (isSMAccount || isSMR)
                                {
                                    if (isSMAccount)
                                    {
                                        if (smStatisticContainer.Visibility == ViewStates.Invisible)
                                        {
                                            try
                                            {
                                                ((DashboardHomeActivity)Activity).SetStatusBarBackground(Resource.Drawable.UsageGradientBackground);
                                                ((DashboardHomeActivity)Activity).SetToolbarBackground(Resource.Drawable.CustomDashboardGradientToolbar);
                                            }
                                            catch (System.Exception e)
                                            {
                                                Utility.LoggingNonFatalError(e);
                                            }
                                            rootView.SetBackgroundResource(Resource.Color.background_pale_grey);
                                            dashboard_bottom_view.SetBackgroundResource(Resource.Drawable.usage_bottom_view);
                                            smStatisticContainer.Visibility = ViewStates.Visible;
                                        }
                                    }
                                    else if (isSMR)
                                    {
                                        if (ssmrHistoryContainer.Visibility == ViewStates.Invisible)
                                        {
                                            try
                                            {
                                                ((DashboardHomeActivity)Activity).SetStatusBarBackground(Resource.Drawable.UsageGradientBackground);
                                                ((DashboardHomeActivity)Activity).SetToolbarBackground(Resource.Drawable.CustomDashboardGradientToolbar);
                                            }
                                            catch (System.Exception e)
                                            {
                                                Utility.LoggingNonFatalError(e);
                                            }
                                            rootView.SetBackgroundResource(Resource.Color.background_pale_grey);
                                            dashboard_bottom_view.SetBackgroundResource(Resource.Drawable.usage_bottom_view);
                                            ssmrHistoryContainer.Visibility = ViewStates.Visible;
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (!isToggleTariff)
                            {
                                if (isSMAccount || isSMR)
                                {
                                    if (isSMAccount)
                                    {
                                        if (smStatisticContainer.Visibility == ViewStates.Invisible)
                                        {
                                            try
                                            {
                                                ((DashboardHomeActivity)Activity).SetStatusBarBackground(Resource.Drawable.UsageGradientBackground);
                                                ((DashboardHomeActivity)Activity).SetToolbarBackground(Resource.Drawable.CustomDashboardGradientToolbar);
                                            }
                                            catch (System.Exception e)
                                            {
                                                Utility.LoggingNonFatalError(e);
                                            }
                                            rootView.SetBackgroundResource(Resource.Color.background_pale_grey);
                                            scrollViewContent.SetBackgroundResource(Resource.Drawable.dashboard_chart_bg);
                                            dashboard_bottom_view.SetBackgroundResource(Resource.Drawable.usage_bottom_view);
                                            smStatisticContainer.Visibility = ViewStates.Visible;
                                        }
                                    }
                                    else if (isSMR)
                                    {
                                        if (ssmrHistoryContainer.Visibility == ViewStates.Invisible)
                                        {
                                            try
                                            {
                                                ((DashboardHomeActivity)Activity).SetStatusBarBackground(Resource.Drawable.UsageGradientBackground);
                                                ((DashboardHomeActivity)Activity).SetToolbarBackground(Resource.Drawable.CustomDashboardGradientToolbar);
                                            }
                                            catch (System.Exception e)
                                            {
                                                Utility.LoggingNonFatalError(e);
                                            }
                                            rootView.SetBackgroundResource(Resource.Color.background_pale_grey);
                                            scrollViewContent.SetBackgroundResource(Resource.Drawable.dashboard_chart_bg);
                                            dashboard_bottom_view.SetBackgroundResource(Resource.Drawable.usage_bottom_view);
                                            ssmrHistoryContainer.Visibility = ViewStates.Visible;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else if (scrollPosition > 0 || scrollPosition < 0)
                    {
                        requireScroll = true;
                        if (isClickedShowTariff)
                        {
                            isClickedShowTariff = false;
                            isHideBottomSheetShowTariff = false;
                        }
                        bottomSheetBehavior.State = BottomSheetBehavior.StateHidden;
                        shadowLayout.SetBackgroundResource(0);

                        if (!isToggleTariff)
                        {
                            if (isSMAccount || isSMR)
                            {
                                if (isSMAccount)
                                {
                                    if (smStatisticContainer.Visibility == ViewStates.Invisible)
                                    {
                                        try
                                        {
                                            ((DashboardHomeActivity)Activity).SetStatusBarBackground(Resource.Drawable.UsageGradientBackground);
                                            ((DashboardHomeActivity)Activity).SetToolbarBackground(Resource.Drawable.CustomDashboardGradientToolbar);
                                        }
                                        catch (System.Exception e)
                                        {
                                            Utility.LoggingNonFatalError(e);
                                        }
                                        rootView.SetBackgroundResource(Resource.Color.background_pale_grey);
                                        scrollViewContent.SetBackgroundResource(Resource.Drawable.dashboard_chart_bg);
                                        dashboard_bottom_view.SetBackgroundResource(Resource.Drawable.usage_bottom_view);
                                        smStatisticContainer.Visibility = ViewStates.Visible;
                                    }
                                }
                                else if (isSMR)
                                {
                                    if (ssmrHistoryContainer.Visibility == ViewStates.Invisible)
                                    {
                                        try
                                        {
                                            ((DashboardHomeActivity)Activity).SetStatusBarBackground(Resource.Drawable.UsageGradientBackground);
                                            ((DashboardHomeActivity)Activity).SetToolbarBackground(Resource.Drawable.CustomDashboardGradientToolbar);
                                        }
                                        catch (System.Exception e)
                                        {
                                            Utility.LoggingNonFatalError(e);
                                        }
                                        rootView.SetBackgroundResource(Resource.Color.background_pale_grey);
                                        scrollViewContent.SetBackgroundResource(Resource.Drawable.dashboard_chart_bg);
                                        dashboard_bottom_view.SetBackgroundResource(Resource.Drawable.usage_bottom_view);
                                        ssmrHistoryContainer.Visibility = ViewStates.Visible;
                                    }
                                }
                            }
                        }
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
                        if (newState == BottomSheetBehavior.StateHidden || newState == BottomSheetBehavior.StateCollapsed)
                        {
                            if (!isTutorialShow && !isClickedShowTariff)
                            {
                                bottomSheetBehavior.State = BottomSheetBehavior.StateExpanded;
                            }
                        }
                    }
                    else if (requireScroll)
                    {
                        bottomSheetBehavior.State = BottomSheetBehavior.StateHidden;
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
                kwhLabel.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(this.Activity, Resource.Color.powerBlue)));
                rmLabel.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(this.Activity, Resource.Color.new_grey)));
                isShowAnimationDisable = true;
                ShowByKwh();
                FirebaseAnalyticsUtils.LogFragmentClickEvent(this, "kWh Selection Clicked");
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
                rmLabel.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(this.Activity, Resource.Color.powerBlue)));
                kwhLabel.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(this.Activity, Resource.Color.new_grey)));
                isShowAnimationDisable = true;
                ShowByRM();
                FirebaseAnalyticsUtils.LogFragmentClickEvent(this, "RM Selection Clicked");

            }
            catch (System.Exception ne)
            {
                Utility.LoggingNonFatalError(ne);
            }

        }

        private bool CheckIsDPCIndicatorAvailable()
        {
            bool isFound = false;

            if (isSMAccount)
            {
                if (selectedSMHistoryData != null && selectedSMHistoryData.ByMonth != null && selectedSMHistoryData.ByMonth.Months != null && selectedSMHistoryData.ByMonth.Months.Count > 0)
                {
                    for (int i = 0; i < selectedSMHistoryData.ByMonth.Months.Count; i++)
                    {
                        if (selectedSMHistoryData.ByMonth.Months[i].DPCIndicator)
                        {
                            isFound = true;
                            break;
                        }
                    }
                }
            }
            else if (!isREAccount)
            {
                if (selectedHistoryData != null && selectedHistoryData.ByMonth != null && selectedHistoryData.ByMonth.Months != null && selectedHistoryData.ByMonth.Months.Count > 0)
                {
                    for (int i = 0; i < selectedHistoryData.ByMonth.Months.Count; i++)
                    {
                        if (selectedHistoryData.ByMonth.Months[i].DPCIndicator)
                        {
                            isFound = true;
                            break;
                        }
                    }
                }
            }

            return isFound;
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

                isChangeVirtualHeightNeed = true;
                SetVirtualHeightParams(6f);
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void HideEnergyTipsShimmerView()
        {
            try
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
            catch (System.Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }

        private void OnSetEnergyTipsShimmerAdapter(List<EnergySavingTipsModel> list)
        {
            energyTipsShimmerAdapter = new EnergySavingTipsShimmerAdapter(list, this.Activity);
            energyTipsShimmerList.SetAdapter(energyTipsShimmerAdapter);
        }

        public void ShowEnergyTipsView(List<EnergySavingTipsModel> list)
        {
            try
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
            catch (System.Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }

        void ViewTreeObserver.IOnGlobalLayoutListener.OnGlobalLayout()
        {
            try
            {
                int childHeight = scrollViewContent.Height;
                int screenHeightWithoutBottomSheet = rootView.Height - bottomSheet.Height + shadowLayout.Height;
                int screenHeightWithoutVirtualHeight = childHeight + scrollView.PaddingTop + scrollView.PaddingBottom - virtualHeight.Height;

                float diffLayout = DPUtils.ConvertPxToDP((childHeight + scrollView.PaddingTop + scrollView.PaddingBottom) - scrollView.Height);

                float compareOffSet = 10f;

                if (ChartType == ChartType.Month)
                {
                    compareOffSet = 12f;
                }
                else if (ChartType == ChartType.Day)
                {
                    compareOffSet = 24f;
                }

                isScrollIndicatorShowNeed = ((scrollView.Height) < (childHeight + scrollView.PaddingTop + scrollView.PaddingBottom)) && (diffLayout > compareOffSet);
                if (isScrollIndicatorShowNeed && !requireScroll)
                {
                    isChangeVirtualHeightNeed = false;
                    shadowLayout.SetBackgroundResource(Resource.Drawable.scroll_indicator);
                    bottomSheet.RequestLayout();

                    if (isClickedShowTariff && !isHideBottomSheetShowTariff && CheckIsScrollable())
                    {
                        isHideBottomSheetShowTariff = true;
                        shadowLayout.SetBackgroundResource(0);
                        bottomSheet.RequestLayout();
                        bottomSheetBehavior.State = BottomSheetBehavior.StateHidden;
                    }

                    if (!isTutorialShow)
                    {
                        if (!isToggleTariff)
                        {
                            if (isSMAccount || isSMR)
                            {
                                if (isSMAccount)
                                {
                                    if (smStatisticContainer.Visibility == ViewStates.Visible)
                                    {
                                        rootView.SetBackgroundResource(0);
                                        scrollViewContent.SetBackgroundResource(0);
                                        dashboard_bottom_view.SetBackgroundResource(0);
                                        try
                                        {
                                            ((DashboardHomeActivity)Activity).SetStatusBarBackground(Resource.Drawable.UsageGradientBackground);
                                            ((DashboardHomeActivity)Activity).UnsetToolbarBackground();
                                        }
                                        catch (System.Exception e)
                                        {
                                            Utility.LoggingNonFatalError(e);
                                        }
                                        DashboardCustomScrolling(0);
                                        smStatisticContainer.Visibility = ViewStates.Invisible;
                                    }
                                }
                                else if (isSMR)
                                {
                                    if (ssmrHistoryContainer.Visibility == ViewStates.Visible)
                                    {
                                        rootView.SetBackgroundResource(0);
                                        scrollViewContent.SetBackgroundResource(0);
                                        dashboard_bottom_view.SetBackgroundResource(0);
                                        try
                                        {
                                            ((DashboardHomeActivity)Activity).SetStatusBarBackground(Resource.Drawable.UsageGradientBackground);
                                            ((DashboardHomeActivity)Activity).UnsetToolbarBackground();
                                        }
                                        catch (System.Exception e)
                                        {
                                            Utility.LoggingNonFatalError(e);
                                        }
                                        DashboardCustomScrolling(0);
                                        ssmrHistoryContainer.Visibility = ViewStates.Invisible;
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (isSMAccount || isSMR)
                            {
                                if (isSMAccount)
                                {
                                    if (smStatisticContainer.Visibility == ViewStates.Invisible)
                                    {
                                        try
                                        {
                                            ((DashboardHomeActivity)Activity).SetStatusBarBackground(Resource.Drawable.UsageGradientBackground);
                                            ((DashboardHomeActivity)Activity).SetToolbarBackground(Resource.Drawable.CustomDashboardGradientToolbar);
                                        }
                                        catch (System.Exception e)
                                        {
                                            Utility.LoggingNonFatalError(e);
                                        }
                                        rootView.SetBackgroundResource(Resource.Color.background_pale_grey);
                                        dashboard_bottom_view.SetBackgroundResource(Resource.Drawable.usage_bottom_view);
                                        smStatisticContainer.Visibility = ViewStates.Visible;
                                    }
                                }
                                else if (isSMR)
                                {
                                    if (ssmrHistoryContainer.Visibility == ViewStates.Invisible)
                                    {
                                        try
                                        {
                                            ((DashboardHomeActivity)Activity).SetStatusBarBackground(Resource.Drawable.UsageGradientBackground);
                                            ((DashboardHomeActivity)Activity).SetToolbarBackground(Resource.Drawable.CustomDashboardGradientToolbar);
                                        }
                                        catch (System.Exception e)
                                        {
                                            Utility.LoggingNonFatalError(e);
                                        }
                                        rootView.SetBackgroundResource(Resource.Color.background_pale_grey);
                                        dashboard_bottom_view.SetBackgroundResource(Resource.Drawable.usage_bottom_view);
                                        ssmrHistoryContainer.Visibility = ViewStates.Visible;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (!isToggleTariff)
                        {
                            if (isSMAccount || isSMR)
                            {
                                if (isSMAccount)
                                {
                                    if (smStatisticContainer.Visibility == ViewStates.Invisible)
                                    {
                                        try
                                        {
                                            ((DashboardHomeActivity)Activity).SetStatusBarBackground(Resource.Drawable.UsageGradientBackground);
                                            ((DashboardHomeActivity)Activity).SetToolbarBackground(Resource.Drawable.CustomDashboardGradientToolbar);
                                        }
                                        catch (System.Exception e)
                                        {
                                            Utility.LoggingNonFatalError(e);
                                        }
                                        rootView.SetBackgroundResource(Resource.Color.background_pale_grey);
                                        scrollViewContent.SetBackgroundResource(Resource.Drawable.dashboard_chart_bg);
                                        dashboard_bottom_view.SetBackgroundResource(Resource.Drawable.usage_bottom_view);
                                        smStatisticContainer.Visibility = ViewStates.Visible;
                                    }
                                }
                                else if (isSMR)
                                {
                                    if (ssmrHistoryContainer.Visibility == ViewStates.Invisible)
                                    {
                                        try
                                        {
                                            ((DashboardHomeActivity)Activity).SetStatusBarBackground(Resource.Drawable.UsageGradientBackground);
                                            ((DashboardHomeActivity)Activity).SetToolbarBackground(Resource.Drawable.CustomDashboardGradientToolbar);
                                        }
                                        catch (System.Exception e)
                                        {
                                            Utility.LoggingNonFatalError(e);
                                        }
                                        rootView.SetBackgroundResource(Resource.Color.background_pale_grey);
                                        scrollViewContent.SetBackgroundResource(Resource.Drawable.dashboard_chart_bg);
                                        dashboard_bottom_view.SetBackgroundResource(Resource.Drawable.usage_bottom_view);
                                        ssmrHistoryContainer.Visibility = ViewStates.Visible;
                                    }
                                }
                            }
                        }
                    }
                }
                else if (!isScrollIndicatorShowNeed && isChangeVirtualHeightNeed)
                {
                    if (!isREAccount)
                    {
                        if (screenHeightWithoutVirtualHeight > screenHeightWithoutBottomSheet && (DPUtils.ConvertPxToDP(screenHeightWithoutVirtualHeight - screenHeightWithoutBottomSheet)) > 8f)
                        {
                            float newVirtualHeight = DPUtils.ConvertPxToDP(virtualHeight.Height) + DPUtils.ConvertPxToDP((screenHeightWithoutVirtualHeight - screenHeightWithoutBottomSheet) / 2);
                            SetVirtualHeightParams(newVirtualHeight);
                            bottomSheet.RequestLayout();
                        }
                        else
                        {
                            isChangeVirtualHeightNeed = false;
                            SetVirtualHeightParams(6f);
                            shadowLayout.SetBackgroundResource(0);
                            bottomSheet.RequestLayout();

                            if (!isToggleTariff)
                            {
                                if (isSMAccount || isSMR)
                                {
                                    if (isSMAccount)
                                    {
                                        if (smStatisticContainer.Visibility == ViewStates.Invisible)
                                        {
                                            try
                                            {
                                                ((DashboardHomeActivity)Activity).SetStatusBarBackground(Resource.Drawable.UsageGradientBackground);
                                                ((DashboardHomeActivity)Activity).SetToolbarBackground(Resource.Drawable.CustomDashboardGradientToolbar);
                                            }
                                            catch (System.Exception e)
                                            {
                                                Utility.LoggingNonFatalError(e);
                                            }
                                            rootView.SetBackgroundResource(Resource.Color.background_pale_grey);
                                            scrollViewContent.SetBackgroundResource(Resource.Drawable.dashboard_chart_bg);
                                            dashboard_bottom_view.SetBackgroundResource(Resource.Drawable.usage_bottom_view);
                                            smStatisticContainer.Visibility = ViewStates.Visible;
                                        }
                                    }
                                    else if (isSMR)
                                    {
                                        if (ssmrHistoryContainer.Visibility == ViewStates.Invisible)
                                        {
                                            try
                                            {
                                                ((DashboardHomeActivity)Activity).SetStatusBarBackground(Resource.Drawable.UsageGradientBackground);
                                                ((DashboardHomeActivity)Activity).SetToolbarBackground(Resource.Drawable.CustomDashboardGradientToolbar);
                                            }
                                            catch (System.Exception e)
                                            {
                                                Utility.LoggingNonFatalError(e);
                                            }
                                            rootView.SetBackgroundResource(Resource.Color.background_pale_grey);
                                            scrollViewContent.SetBackgroundResource(Resource.Drawable.dashboard_chart_bg);
                                            dashboard_bottom_view.SetBackgroundResource(Resource.Drawable.usage_bottom_view);
                                            ssmrHistoryContainer.Visibility = ViewStates.Visible;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        isChangeVirtualHeightNeed = false;
                        SetVirtualHeightParams(6f);
                        shadowLayout.SetBackgroundResource(0);
                        bottomSheet.RequestLayout();
                    }
                }
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
            try
            {
                Activity.RunOnUiThread(() =>
                {
                    try
                    {
                        DoNothingSelected();
                    }
                    catch (System.Exception ex)
                    {
                        Utility.LoggingNonFatalError(ex);
                    }
                });
            }
            catch (System.Exception err)
            {
                DoNothingSelected();
                Utility.LoggingNonFatalError(err);
            }
        }

        private void DoNothingSelected()
        {
            CurrentParentIndex = -1;
            try
            {
                isDPCBarClicked = false;
                if (ChartType == ChartType.Month)
                {
                    OnGenerateTariffLegendValue(-1, isToggleTariff);
                    FirebaseAnalyticsUtils.LogFragmentClickEvent(this, "Graph Bar Tap Out");
                }
                else if (ChartType == ChartType.Day && isZoomIn)
                {
                    BarEntry dayViewTariff = dayViewTariffList[currentDayViewIndex];
                    Highlight centerBar = new Highlight(currentDayViewIndex, 0, dayViewTariff.GetYVals().Length - 1);
                    mChart.HighlightValue(centerBar, false);
                }
            }
            catch (System.Exception err)
            {
                Utility.LoggingNonFatalError(err);
            }
        }

        // Lin Siong Note: Handle the chart select event
        // Lin Siong Note: Will have vibration effect when selected
        // Lin Siong Note: if isToggleTariff = true then it will force the entry to be hightlighted to most upper one
        void IOnChartValueSelectedListenerSupport.OnValueSelected(Entry e, Highlight h)
        {
            try
            {
                Activity.RunOnUiThread(() =>
                {
                    try
                    {
                        DoValueSelected(e, h);
                    }
                    catch (System.Exception ex)
                    {
                        Utility.LoggingNonFatalError(ex);
                    }
                });
            }
            catch (System.Exception err)
            {
                DoValueSelected(e, h);
                Utility.LoggingNonFatalError(err);
            }
        }

        private void DoValueSelected(Entry e, Highlight h)
        {
            try
            {
                if (ChartType == ChartType.Month)
                {
                    if (!isSMAccount)
                    {
                        if (isToggleTariff && h != null)
                        {
                            int stackedIndex = 0;
                            if (selectedHistoryData.ByMonth.Months[(int)e.GetX()].TariffBlocksList != null && selectedHistoryData.ByMonth.Months[(int)e.GetX()].TariffBlocksList.Count > 0)
                            {
                                if (mChart.Data != null && mChart.Data is BarData)
                                {
                                    BarData barData = mChart.Data as BarData;

                                    BarDataSet currentDataSet = barData.GetDataSetByIndex(0) as BarDataSet;
                                    BarEntry listEntries = currentDataSet.Values[(int)e.GetX()] as BarEntry;

                                    stackedIndex = listEntries.GetYVals().Length - 1;
                                }
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
                                if (mChart.Data != null && mChart.Data is BarData)
                                {
                                    BarData barData = mChart.Data as BarData;

                                    BarDataSet currentDataSet = barData.GetDataSetByIndex(0) as BarDataSet;
                                    BarEntry listEntries = currentDataSet.Values[(int)e.GetX()] as BarEntry;

                                    stackedIndex = listEntries.GetYVals().Length - 1;
                                }
                            }

                            Highlight stackedHigh = new Highlight((int)e.GetX(), 0, stackedIndex);
                            mChart.HighlightValue(stackedHigh, false);
                        }

                        if (h != null && GetIsMDMSDown())
                        {
                            if ((int)e.GetX() == selectedSMHistoryData.ByMonth.Months.Count - 1)
                            {
                                OnShowMDMSDownPopup();
                            }
                        }
                    }

                    if (e != null)
                    {
                        OnGenerateTariffLegendValue((int)e.GetX(), isToggleTariff);
                    }
                }
                else if (ChartType == ChartType.Day)
                {
                    if (isZoomIn)
                    {
                        int count = dayViewTariffList.Count - 1 - (int)e.GetX();

                        if (((int)e.GetX() >= 0 && (int)e.GetX() < 4) || (count >= 0 && count < 4))
                        {
                            BarEntry dayViewTariff = dayViewTariffList[currentDayViewIndex];
                            Highlight centerBar = new Highlight(currentDayViewIndex, 0, dayViewTariff.GetYVals().Length - 1);
                            mChart.HighlightValue(centerBar, false);
                        }
                        else if (h != null)
                        {
                            int diff = currentDayViewIndex - (int)e.GetX();

                            currentDayViewIndex = (int)e.GetX();

                            if (diff > 0 || diff < 0)
                            {
                                isShowLog = true;
                                currentLowestVisibleX = currentLowestVisibleX - diff;
                                trackingLowestVisibleX = currentLowestVisibleX;
                                lowestVisibleX = currentLowestVisibleX;

                                IRunnable specificJob = MoveViewJob.GetInstance(mChart.ViewPortHandler, lowestVisibleX, 0f,
                                    mChart.GetTransformer(YAxis.AxisDependency.Left), mChart);
                                specificJob.Run();
                                mChart.Invalidate();

                                float[] pts = { lowestVisibleX, 0f };
                                mChart.GetTransformer(YAxis.AxisDependency.Left).PointValuesToPixel(pts);
                                mChart.ViewPortHandler.Translate(pts, mChart.Matrix);
                                mChart.Invalidate();

                                Vibrator vibrator = (Vibrator)this.Activity.GetSystemService(Context.VibratorService);
                                if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.O)
                                {
                                    vibrator.Vibrate(VibrationEffect.CreateOneShot(150, 10));

                                }
                                else
                                {
                                    vibrator.Vibrate(150);

                                }
                            }

                            BarEntry dayViewTariff = dayViewTariffList[currentDayViewIndex];
                            Highlight centerBar = new Highlight(currentDayViewIndex, 0, dayViewTariff.GetYVals().Length - 1);
                            mChart.HighlightValue(centerBar, false);
                            SetDayViewMonthText(dayViewMonthList[currentDayViewIndex]);

                            if (missingReadingList[currentDayViewIndex])
                            {
                                OnMissingReadingClick();
                            }
                        }
                        else
                        {
                            BarEntry dayViewTariff = dayViewTariffList[currentDayViewIndex];
                            Highlight centerBar = new Highlight(currentDayViewIndex, 0, dayViewTariff.GetYVals().Length - 1);
                            mChart.HighlightValue(centerBar, false);
                        }
                    }
                    else
                    {
                        mChart.HighlightValue(null, false);
                    }
                }
            }
            catch (System.Exception err)
            {
                Utility.LoggingNonFatalError(err);
            }

            try
            {
                if (ChartType == ChartType.Month)
                {
                    if (e != null)
                    {
                        int index = (int)e.GetX();
                        if (index != CurrentParentIndex)
                        {
                            CurrentParentIndex = index;
                            Vibrator vibrator = (Vibrator)this.Activity.GetSystemService(Context.VibratorService);
                            if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.O)
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
                }

                try
                {
                    FirebaseAnalyticsUtils.LogFragmentClickEvent(this, "Graph Bar Tap In");
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
                if (isREAccount)
                {
                    RelativeLayout.LayoutParams param = shimmrtGraphView.LayoutParameters as RelativeLayout.LayoutParams;
                    param.Height = (int)DPUtils.ConvertDPToPx(200);
                }
                else
                {
                    RelativeLayout.LayoutParams param = shimmrtGraphView.LayoutParameters as RelativeLayout.LayoutParams;
                    param.Height = (int)DPUtils.ConvertDPToPx(210);
                }
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

        public void HideSMStatisticCard()
        {
            if (isSMAccount)
            {
                StopSMStatisticShimmer();
                smStatisticContainer.Visibility = ViewStates.Gone;
                if (GetIsMDMSDown())
                {
                    rootView.SetBackgroundResource(0);
                    dashboard_bottom_view.SetBackgroundResource(0);
                    scrollViewContent.SetBackgroundResource(0);
                    try
                    {
                        ((DashboardHomeActivity)Activity).SetStatusBarBackground(Resource.Drawable.UsageGradientBackground);
                        ((DashboardHomeActivity)Activity).UnsetToolbarBackground();
                    }
                    catch (System.Exception e)
                    {
                        Utility.LoggingNonFatalError(e);
                    }
                }
            }
            else
            {
                smStatisticContainer.Visibility = ViewStates.Gone;
            }
        }

        public void ShowSMStatisticCard()
        {
            try
            {
                if (isSMAccount)
                {
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
                        txtSmStatisticTooltip.Text = Utility.GetLocalizedLabel("Usage", "projectedCostTitle");
                        if ((selectedSMHistoryData != null && selectedSMHistoryData.OtherUsageMetrics != null && selectedSMHistoryData.OtherUsageMetrics.CostData != null))
                        {
                            foreach (SMUsageHistoryData.Stats costValue in selectedSMHistoryData.OtherUsageMetrics.CostData)
                            {
                                System.Globalization.CultureInfo currCult = System.Globalization.CultureInfo.CreateSpecificCulture("en-US");
                                if (costValue.Key == Constants.CURRENT_COST_KEY)
                                {
                                    smStatisticBillTitle.Text = string.IsNullOrEmpty(costValue.Title) ? "My bill amount so far" : costValue.Title;
                                    smStatisticBillSubTitle.Text = string.IsNullOrEmpty(costValue.SubTitle) ? "- -" : costValue.SubTitle;
                                    smStatisticBill.Text = string.IsNullOrEmpty(costValue.Value) ? "- -" : smDecimalFormat.Format(double.Parse(costValue.Value, currCult));
                                    smStatisticBillCurrency.Text = string.IsNullOrEmpty(costValue.ValueUnit) ? "RM" : costValue.ValueUnit;
                                    if (isMDMSDown)
                                    {
                                        smStatisticBillSubTitle.Text = "- -";
                                        smStatisticBill.Text = "- -";
                                    }
                                }
                                else if (costValue.Key == Constants.PROJECTED_COST_KEY)
                                {
                                    smStatisticPredictTitle.Text = string.IsNullOrEmpty(costValue.Title) ? "My bill amount so far" : costValue.Title;
                                    smStatisticPredictSubTitle.Text = string.IsNullOrEmpty(costValue.SubTitle) ? "- -" : costValue.SubTitle;
                                    smStatisticPredict.Text = string.IsNullOrEmpty(costValue.Value) ? "- -" : smDecimalFormat.Format(double.Parse(costValue.Value, currCult));
                                    smStatisticPredictCurrency.Text = string.IsNullOrEmpty(costValue.ValueUnit) ? "RM" : costValue.ValueUnit;
                                    if (isMDMSDown)
                                    {
                                        smStatisticPredictSubTitle.Text = "- -";
                                        smStatisticPredict.Text = "- -";
                                    }
                                }
                            }
                        }

                        if (selectedSMHistoryData != null && selectedSMHistoryData.OtherUsageMetrics != null && selectedSMHistoryData.ToolTips != null && selectedSMHistoryData.ToolTips.Count > 0)
                        {
                            foreach (SMUsageHistoryData.SmartMeterToolTips costValue in selectedSMHistoryData.ToolTips)
                            {
                                if (costValue.Type == Constants.PROJECTED_COST_KEY)
                                {
                                    txtSmStatisticTooltip.Text = string.IsNullOrEmpty(costValue.SMLink) ? Utility.GetLocalizedLabel("Usage", "projectedCostTitle") : costValue.SMLink;
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
                        if ((selectedSMHistoryData != null && selectedSMHistoryData.OtherUsageMetrics != null && selectedSMHistoryData.OtherUsageMetrics.UsageData != null && selectedSMHistoryData.OtherUsageMetrics.UsageData.Count > 0))
                        {
                            foreach (SMUsageHistoryData.Stats costValue in selectedSMHistoryData.OtherUsageMetrics.UsageData)
                            {
                                System.Globalization.CultureInfo currCult = System.Globalization.CultureInfo.CreateSpecificCulture("en-US");
                                if (costValue.Key == Constants.CURRENT_USAGE_KEY)
                                {
                                    smStatisticBillTitle.Text = string.IsNullOrEmpty(costValue.Title) ? "My bill amount so far" : costValue.Title;
                                    smStatisticBillSubTitle.Text = string.IsNullOrEmpty(costValue.SubTitle) ? "- -" : costValue.SubTitle;
                                    smStatisticBillKwh.Text = string.IsNullOrEmpty(costValue.Value) ? "- -" : smKwhFormat.Format(double.Parse(costValue.Value, currCult));
                                    smStatisticBillKwhUnit.Text = string.IsNullOrEmpty(costValue.ValueUnit) ? "kWh" : costValue.ValueUnit;
                                    if (isMDMSDown)
                                    {
                                        smStatisticBillSubTitle.Text = "- -";
                                        smStatisticBillKwh.Text = "- -";
                                    }
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

                                        smStatisticTrend.TextFormatted = Html.FromHtml(trendString, FromHtmlOptions.ModeLegacy);
                                    }
                                    else
                                    {
                                        smStatisticTrend.TextFormatted = Html.FromHtml(trendString);
                                    }

                                    if (isMDMSDown)
                                    {
                                        smStatisticTrendSubTitle.Text = "- -";
                                        smStatisticTrend.Text = "- -%";
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

                refreshImgParams.Width = GetDeviceHorizontalScaleInPixel(0.322f);
                refreshImgParams.Height = GetDeviceHorizontalScaleInPixel(0.322f);
                refreshImgParams.TopMargin = (int)DPUtils.ConvertDPToPx(38f);
                refresh_image.RequestLayout();

                LinearLayout.LayoutParams refreshTxtParams = txtNewRefreshMessage.LayoutParameters as LinearLayout.LayoutParams;
                refreshTxtParams.TopMargin = (int)DPUtils.ConvertDPToPx(18f);
                txtNewRefreshMessage.RequestLayout();

                LinearLayout.LayoutParams refreshButtonParams = btnNewRefresh.LayoutParameters as LinearLayout.LayoutParams;
                refreshButtonParams.TopMargin = (int)DPUtils.ConvertDPToPx(16f);
                if (!isREAccount)
                {
                    refreshButtonParams.BottomMargin = (int)DPUtils.ConvertDPToPx(24f);
                }
                btnNewRefresh.RequestLayout();
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public override void OnPause()
        {
            base.OnPause();
            try
            {
                if (isSMR)
                {
                    NewAppTutorialUtils.ForceCloseNewAppTutorial();
                }
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        // Lin Siong Note: Set New Account layout param
        public void SetNewAccountLayoutParams()
        {
            try
            {
                if (isSMAccount)
                {
                    rootView.SetBackgroundResource(0);
                    scrollViewContent.SetBackgroundResource(0);
                    dashboard_bottom_view.SetBackgroundResource(0);
                    try
                    {
                        ((DashboardHomeActivity)Activity).SetStatusBarBackground(Resource.Drawable.NewHorizontalGradientBackground);
                        ((DashboardHomeActivity)Activity).UnsetToolbarBackground();
                    }
                    catch (System.Exception e)
                    {
                        Utility.LoggingNonFatalError(e);
                    }
                }

                LinearLayout.LayoutParams newAccountImageParams = newAccountImage.LayoutParameters as LinearLayout.LayoutParams;

                newAccountImageParams.Width = GetDeviceHorizontalScaleInPixel(0.30f);
                newAccountImageParams.Height = GetDeviceHorizontalScaleInPixel(0.30f);
                if (isREAccount || isSMR)
                {
                    newAccountImageParams.TopMargin = (int)DPUtils.ConvertDPToPx(32f);
                }
                else
                {
                    newAccountImageParams.TopMargin = GetDeviceHorizontalScaleInPixel(0.201f);
                }
                newAccountImage.RequestLayout();

                LinearLayout.LayoutParams newAccountContentParams = newAccountContent.LayoutParameters as LinearLayout.LayoutParams;
                newAccountContentParams.TopMargin = (int)DPUtils.ConvertDPToPx(24f);
                if (isREAccount || isSMR)
                {
                    if (isSMR)
                    {
                        newAccountContentParams.BottomMargin = (int)DPUtils.ConvertDPToPx(35f);
                    }
                    else
                    {
                        newAccountContentParams.BottomMargin = (int)DPUtils.ConvertDPToPx(3f);
                    }
                }
                newAccountContent.RequestLayout();
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

                refreshImgParams.Width = GetDeviceHorizontalScaleInPixel(0.306f);
                refreshImgParams.Height = GetDeviceHorizontalScaleInPixel(0.306f);
                refreshImgParams.TopMargin = (int)DPUtils.ConvertDPToPx(60f);
                refresh_image.RequestLayout();

                LinearLayout.LayoutParams refreshTxtParams = txtNewRefreshMessage.LayoutParameters as LinearLayout.LayoutParams;
                refreshTxtParams.TopMargin = (int)DPUtils.ConvertDPToPx(16f);
                if (isREAccount)
                {
                    refreshTxtParams.BottomMargin = (int)DPUtils.ConvertDPToPx(42f);
                }
                else if (isSMR)
                {
                    refreshTxtParams.BottomMargin = (int)DPUtils.ConvertDPToPx(58f);
                }
                txtNewRefreshMessage.RequestLayout();
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        // To get isMDMSDown flag
        public bool GetIsMDMSDown()
        {
            return isMDMSDown;
        }

        // To set isMDMSDown flag
        public void SetISMDMSDown(bool flag)
        {
            isMDMSDown = flag;
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

        bool View.IOnTouchListener.OnTouch(View v, MotionEvent e)
        {
            mScaleDetector.OnTouchEvent(e);
            return false;
        }

        public void ByZoomDayView()
        {
            mChart.Visibility = ViewStates.Visible;

            rmKwhSelection.Enabled = true;
            tarifToggle.Enabled = true;
            btnToggleDay.Enabled = true;
            btnToggleMonth.Enabled = true;

            if (isZoomIn)
            {
                smGraphZoomToggle.SetImageResource(Resource.Drawable.zoom_out);
            }
            else
            {
                smGraphZoomToggle.SetImageResource(Resource.Drawable.dashboard_zoom_tooltip);
            }

            mChart.Clear();
            SetUp();
        }

        private void SetDayViewMonthText(string str)
        {
            txtDayViewZoomInIndicator.Text = str;
        }

        public void ShowBillDetails(AccountData accountData, List<AccountChargeModel> selectedAccountChargesModelList)
        {
            Intent intent = new Intent(Activity, typeof(BillingDetailsActivity));
            intent.PutExtra("SELECTED_ACCOUNT", JsonConvert.SerializeObject(accountData));
            intent.PutExtra("SELECTED_BILL_DETAILS", JsonConvert.SerializeObject(selectedAccountChargesModelList[0]));
            StartActivity(intent);
        }

        private class OnBarChartTouchLister : BarLineChartTouchListener
        {
            public ChartType currentChartType { get; set; }
            public ChartDataType currentChartDataType { get; set; }
            public bool currentIsZoomIn { get; set; }
            private BarLineChartBase currentChart;
            public List<double> currentDayViewRMList { get; set; }
            public List<double> currentDayViewkWhList { get; set; }
            public DashboardChartFragment currentFragment { get; set; }
            public Android.App.Activity currentActivity { get; set; }

            public OnBarChartTouchLister(BarLineChartBase mChart, Matrix mMatrix, float mDistance) : base(mChart, mMatrix, mDistance)
            {
                currentChart = mChart;
                isShowLog = false;
            }

            protected OnBarChartTouchLister(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
            {

            }

            public override void StopDeceleration()
            {
                base.StopDeceleration();
                try
                {
                    currentFragment.Activity.RunOnUiThread(() =>
                    {
                        try
                        {
                            OnStopDeceleration();
                        }
                        catch (System.Exception ex)
                        {
                            Utility.LoggingNonFatalError(ex);
                        }
                    });
                }
                catch (System.Exception e)
                {
                    OnStopDeceleration();
                    Utility.LoggingNonFatalError(e);
                }
            }

            private void OnStopDeceleration()
            {
                if (currentChartType == ChartType.Day && currentIsZoomIn)
                {
                    if (!isShowLog && isDayViewFirstMove)
                    {
                        isShowLog = true;

                        int roundedLowestVisibleX = (int)lowestVisibleX;
                        float resultLowestVisibleX = roundedLowestVisibleX;

                        int checkPoint = (int)(lowestVisibleX * 100);
                        checkPoint = checkPoint % 100;

                        if (roundedLowestVisibleX == 0 && checkPoint <= -50)
                        {
                            resultLowestVisibleX = -0.5f;
                        }
                        else if (roundedLowestVisibleX == 0 && (checkPoint > -50 && checkPoint <= 0))
                        {
                            resultLowestVisibleX = roundedLowestVisibleX + 0.5f;
                        }
                        else if (roundedLowestVisibleX == 0 && (checkPoint > 0 && checkPoint <= 25))
                        {
                            resultLowestVisibleX = -0.5f;
                        }
                        else if (roundedLowestVisibleX == 0 && (checkPoint > 25 && checkPoint <= 50))
                        {
                            resultLowestVisibleX = 0.5f;
                        }
                        else if (roundedLowestVisibleX == 0 && (checkPoint > 50 && checkPoint <= 99))
                        {
                            resultLowestVisibleX = 1.5f;
                        }
                        else if (checkPoint >= 0 && checkPoint < 25)
                        {
                            resultLowestVisibleX = roundedLowestVisibleX - 0.5f;
                        }
                        else if (checkPoint >= 25 && checkPoint <= 50)
                        {
                            resultLowestVisibleX = roundedLowestVisibleX + 0.5f;
                        }
                        else
                        {
                            resultLowestVisibleX = roundedLowestVisibleX + 1.5f;
                        }

                        currentDayViewIndex = (int)(resultLowestVisibleX + 4.5f);

                        if (roundedLowestVisibleX == 0 && (checkPoint <= -50 || (checkPoint > 0 && checkPoint <= 25)))
                        {
                            currentDayViewIndex = minCurrentDayViewIndex;
                        }
                        else if (currentDayViewIndex > maxCurrentDayViewIndex)
                        {
                            currentDayViewIndex = maxCurrentDayViewIndex;
                        }

                        if (resultLowestVisibleX <= minLowestVisibleX)
                        {
                            resultLowestVisibleX = minLowestVisibleX;
                            currentDayViewIndex = minCurrentDayViewIndex;
                        }
                        else if (resultLowestVisibleX >= maxLowestVisibleX)
                        {
                            resultLowestVisibleX = maxLowestVisibleX;
                            currentDayViewIndex = maxCurrentDayViewIndex;
                        }

                        lowestVisibleX = resultLowestVisibleX;
                        currentLowestVisibleX = lowestVisibleX;
                        trackingLowestVisibleX = currentLowestVisibleX;

                        IRunnable specificJob = MoveViewJob.GetInstance(currentChart.ViewPortHandler, lowestVisibleX, 0f,
                            currentChart.GetTransformer(YAxis.AxisDependency.Left), currentChart);
                        specificJob.Run();
                        currentChart.Invalidate();

                        float[] pts = { lowestVisibleX, 0f };
                        currentChart.GetTransformer(YAxis.AxisDependency.Left).PointValuesToPixel(pts);
                        currentChart.ViewPortHandler.Translate(pts, currentChart.Matrix);
                        currentChart.Invalidate();

                        BarEntry dayViewTariff = dayViewTariffList[currentDayViewIndex];
                        Highlight centerBar = new Highlight(currentDayViewIndex, 0, dayViewTariff.GetYVals().Length - 1);
                        currentChart.HighlightValue(centerBar, false);

                        currentFragment.SetDayViewMonthText(dayViewMonthList[currentDayViewIndex]);

                    }
                }
            }

            public override void EndAction(MotionEvent p0)
            {
                base.EndAction(p0);
                try
                {
                    currentFragment.Activity.RunOnUiThread(() =>
                    {
                        try
                        {
                            OnEndAction(p0);
                        }
                        catch (System.Exception ex)
                        {
                            Utility.LoggingNonFatalError(ex);
                        }
                    });
                }
                catch (System.Exception e)
                {
                    OnEndAction(p0);
                    Utility.LoggingNonFatalError(e);
                }
            }

            private void OnEndAction(MotionEvent p0)
            {
                if (currentChartType == ChartType.Day && currentIsZoomIn)
                {
                    if (p0.Action == MotionEventActions.Up)
                    {
                        if (!isShowLog && isDayViewFirstMove)
                        {
                            isShowLog = true;

                            int roundedLowestVisibleX = (int)lowestVisibleX;
                            float resultLowestVisibleX = roundedLowestVisibleX;

                            int checkPoint = (int)(lowestVisibleX * 100);
                            checkPoint = checkPoint % 100;

                            if (roundedLowestVisibleX == 0 && checkPoint <= -50)
                            {
                                resultLowestVisibleX = -0.5f;
                            }
                            else if (roundedLowestVisibleX == 0 && (checkPoint > -50 && checkPoint <= 0))
                            {
                                resultLowestVisibleX = roundedLowestVisibleX + 0.5f;
                            }
                            else if (roundedLowestVisibleX == 0 && (checkPoint > 0 && checkPoint <= 25))
                            {
                                resultLowestVisibleX = -0.5f;
                            }
                            else if (roundedLowestVisibleX == 0 && (checkPoint > 25 && checkPoint <= 50))
                            {
                                resultLowestVisibleX = 0.5f;
                            }
                            else if (roundedLowestVisibleX == 0 && (checkPoint > 50 && checkPoint <= 99))
                            {
                                resultLowestVisibleX = 1.5f;
                            }
                            else if (checkPoint >= 0 && checkPoint < 25)
                            {
                                resultLowestVisibleX = roundedLowestVisibleX - 0.5f;
                            }
                            else if (checkPoint >= 25 && checkPoint <= 50)
                            {
                                resultLowestVisibleX = roundedLowestVisibleX + 0.5f;
                            }
                            else
                            {
                                resultLowestVisibleX = roundedLowestVisibleX + 1.5f;
                            }

                            currentDayViewIndex = (int)(resultLowestVisibleX + 4.5f);

                            if (roundedLowestVisibleX == 0 && (checkPoint <= -50 || (checkPoint > 0 && checkPoint <= 25)))
                            {
                                currentDayViewIndex = minCurrentDayViewIndex;
                            }
                            else if (currentDayViewIndex > maxCurrentDayViewIndex)
                            {
                                currentDayViewIndex = maxCurrentDayViewIndex;
                            }

                            if (resultLowestVisibleX <= minLowestVisibleX)
                            {
                                resultLowestVisibleX = minLowestVisibleX;
                                currentDayViewIndex = minCurrentDayViewIndex;
                            }
                            else if (resultLowestVisibleX >= maxLowestVisibleX)
                            {
                                resultLowestVisibleX = maxLowestVisibleX;
                                currentDayViewIndex = maxCurrentDayViewIndex;
                            }

                            lowestVisibleX = resultLowestVisibleX;
                            currentLowestVisibleX = lowestVisibleX;
                            trackingLowestVisibleX = currentLowestVisibleX;

                            IRunnable specificJob = MoveViewJob.GetInstance(currentChart.ViewPortHandler, lowestVisibleX, 0f,
                                currentChart.GetTransformer(YAxis.AxisDependency.Left), currentChart);
                            specificJob.Run();
                            currentChart.Invalidate();

                            float[] pts = { lowestVisibleX, 0f };
                            currentChart.GetTransformer(YAxis.AxisDependency.Left).PointValuesToPixel(pts);
                            currentChart.ViewPortHandler.Translate(pts, currentChart.Matrix);
                            currentChart.Invalidate();

                            BarEntry dayViewTariff = dayViewTariffList[currentDayViewIndex];
                            Highlight centerBar = new Highlight(currentDayViewIndex, 0, dayViewTariff.GetYVals().Length - 1);
                            currentChart.HighlightValue(centerBar, false);

                            currentFragment.SetDayViewMonthText(dayViewMonthList[currentDayViewIndex]);
                        }
                    }
                }
            }

            public override void ComputeScroll()
            {
                // base.ComputeScroll();
                try
                {
                    currentFragment.Activity.RunOnUiThread(() =>
                    {
                        try
                        {
                            OnComputeScroll();
                        }
                        catch (System.Exception ex)
                        {
                            Utility.LoggingNonFatalError(ex);
                        }
                    });
                }
                catch (System.Exception e)
                {
                    OnComputeScroll();
                    Utility.LoggingNonFatalError(e);
                }

            }

            private void OnComputeScroll()
            {
                if (currentChartType == ChartType.Day && currentIsZoomIn)
                {
                    if (!isDayViewFirstMove)
                    {
                        if (currentChartDataType == ChartDataType.RM)
                        {
                            lowestVisibleX = currentDayViewRMList.Count - 1 - 0.5f;
                            currentDayViewIndex = currentDayViewRMList.Count - 1 + 4;
                        }
                        else
                        {
                            lowestVisibleX = currentDayViewkWhList.Count - 1 - 0.5f;
                            currentDayViewIndex = currentDayViewkWhList.Count - 1 + 4;
                        }
                        currentLowestVisibleX = lowestVisibleX;
                        trackingLowestVisibleX = currentLowestVisibleX;
                        maxLowestVisibleX = lowestVisibleX;

                        IRunnable specificJob = MoveViewJob.GetInstance(currentChart.ViewPortHandler, lowestVisibleX, 0f,
                            currentChart.GetTransformer(YAxis.AxisDependency.Left), currentChart);
                        specificJob.Run();
                        currentChart.Invalidate();

                        float[] pts = { lowestVisibleX, 0f };
                        currentChart.GetTransformer(YAxis.AxisDependency.Left).PointValuesToPixel(pts);
                        currentChart.ViewPortHandler.Translate(pts, currentChart.Matrix);
                        currentChart.Invalidate();

                        maxCurrentDayViewIndex = currentDayViewIndex;


                        BarEntry dayViewTariff = dayViewTariffList[currentDayViewIndex];
                        Highlight centerBar = new Highlight(currentDayViewIndex, 0, dayViewTariff.GetYVals().Length - 1);
                        currentChart.HighlightValue(centerBar, true);
                        isDayViewFirstMove = true;
                        isShowLog = true;

                        currentFragment.SetDayViewMonthText(dayViewMonthList[currentDayViewIndex]);

                        currentChart.DispatchTouchEvent(MotionEvent.Obtain(SystemClock.UptimeMillis(), SystemClock.UptimeMillis(), (int)MotionEventActions.Down, (currentChart.Resources.DisplayMetrics.WidthPixels / 2) + (int)DPUtils.ConvertDPToPx(20f), 0, 0));
                        currentChart.DispatchTouchEvent(MotionEvent.Obtain(SystemClock.UptimeMillis(), SystemClock.UptimeMillis(), (int)MotionEventActions.Up, (currentChart.Resources.DisplayMetrics.WidthPixels / 2) + (int)DPUtils.ConvertDPToPx(20f), 0, 0));
                    }
                    else
                    {
                        if (!isShowLog && (System.Math.Abs(trackingLowestVisibleX - currentChart.LowestVisibleX) >= 1))
                        {
                            trackingLowestVisibleX = currentChart.LowestVisibleX;


                            int roundedLowestVisibleX = (int)trackingLowestVisibleX;

                            int checkPoint = (int)(trackingLowestVisibleX * 100);
                            checkPoint = checkPoint % 100;

                            int tempDayViewIndex = (int)trackingLowestVisibleX + 5;

                            if (roundedLowestVisibleX == 0 && (checkPoint <= -50 || (checkPoint > 0 && checkPoint <= 25)))
                            {
                                tempDayViewIndex = minCurrentDayViewIndex;
                            }
                            else if (currentDayViewIndex > maxCurrentDayViewIndex)
                            {
                                tempDayViewIndex = maxCurrentDayViewIndex;
                            }

                            BarEntry dayViewTariff = dayViewTariffList[tempDayViewIndex];

                            Highlight centerBar = new Highlight(tempDayViewIndex, 0, dayViewTariff.GetYVals().Length - 1);
                            currentChart.HighlightValue(centerBar, false);

                            currentFragment.SetDayViewMonthText(dayViewMonthList[tempDayViewIndex]);

                            Vibrator vibrator = (Vibrator)currentActivity.GetSystemService(Context.VibratorService);
                            if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.O)
                            {
                                vibrator.Vibrate(VibrationEffect.CreateOneShot(200, 12));

                            }
                            else
                            {
                                vibrator.Vibrate(200);

                            }
                        }

                        if ((System.Math.Abs(lowestVisibleX - currentChart.LowestVisibleX) > 4))
                        {
                            IRunnable specificJob = MoveViewJob.GetInstance(currentChart.ViewPortHandler, lowestVisibleX, 0f,
                                currentChart.GetTransformer(YAxis.AxisDependency.Left), currentChart);
                            specificJob.Run();
                            currentChart.Invalidate();

                            float[] pts = { lowestVisibleX, 0f };
                            currentChart.GetTransformer(YAxis.AxisDependency.Left).PointValuesToPixel(pts);
                            currentChart.ViewPortHandler.Translate(pts, currentChart.Matrix);
                            currentChart.Invalidate();

                            BarEntry dayViewTariff = dayViewTariffList[currentDayViewIndex];
                            Highlight centerBar = new Highlight(currentDayViewIndex, 0, dayViewTariff.GetYVals().Length - 1);
                            currentChart.HighlightValue(centerBar, false);

                            currentFragment.SetDayViewMonthText(dayViewMonthList[currentDayViewIndex]);

                            currentChart.DispatchTouchEvent(MotionEvent.Obtain(SystemClock.UptimeMillis(), SystemClock.UptimeMillis(), (int)MotionEventActions.Down, (currentChart.Resources.DisplayMetrics.WidthPixels / 2) + (int)DPUtils.ConvertDPToPx(20f), 0, 0));
                            currentChart.DispatchTouchEvent(MotionEvent.Obtain(SystemClock.UptimeMillis(), SystemClock.UptimeMillis(), (int)MotionEventActions.Up, (currentChart.Resources.DisplayMetrics.WidthPixels / 2) + (int)DPUtils.ConvertDPToPx(20f), 0, 0));
                        }
                        else
                        {
                            if ((System.Math.Abs(lowestVisibleX - currentChart.LowestVisibleX) > 0.0000001))
                            {
                                isShowLog = false;

                                lowestVisibleX = currentChart.LowestVisibleX;
                            }
                            else
                            {
                                if (!isShowLog && isDayViewFirstMove)
                                {
                                    isShowLog = true;

                                    int roundedLowestVisibleX = (int)lowestVisibleX;
                                    float resultLowestVisibleX = roundedLowestVisibleX;

                                    int checkPoint = (int)(lowestVisibleX * 100);
                                    checkPoint = checkPoint % 100;

                                    if (roundedLowestVisibleX == 0 && checkPoint <= -50)
                                    {
                                        resultLowestVisibleX = -0.5f;
                                    }
                                    else if (roundedLowestVisibleX == 0 && (checkPoint > -50 && checkPoint <= 0))
                                    {
                                        resultLowestVisibleX = roundedLowestVisibleX + 0.5f;
                                    }
                                    else if (roundedLowestVisibleX == 0 && (checkPoint > 0 && checkPoint <= 25))
                                    {
                                        resultLowestVisibleX = -0.5f;
                                    }
                                    else if (roundedLowestVisibleX == 0 && (checkPoint > 25 && checkPoint <= 50))
                                    {
                                        resultLowestVisibleX = 0.5f;
                                    }
                                    else if (roundedLowestVisibleX == 0 && (checkPoint > 50 && checkPoint <= 99))
                                    {
                                        resultLowestVisibleX = 1.5f;
                                    }
                                    else if (checkPoint >= 0 && checkPoint < 25)
                                    {
                                        resultLowestVisibleX = roundedLowestVisibleX - 0.5f;
                                    }
                                    else if (checkPoint >= 25 && checkPoint <= 50)
                                    {
                                        resultLowestVisibleX = roundedLowestVisibleX + 0.5f;
                                    }
                                    else
                                    {
                                        resultLowestVisibleX = roundedLowestVisibleX + 1.5f;
                                    }

                                    currentDayViewIndex = (int)(resultLowestVisibleX + 4.5f);

                                    if (roundedLowestVisibleX == 0 && (checkPoint <= -50 || (checkPoint > 0 && checkPoint <= 25)))
                                    {
                                        currentDayViewIndex = minCurrentDayViewIndex;
                                    }
                                    else if (currentDayViewIndex > maxCurrentDayViewIndex)
                                    {
                                        currentDayViewIndex = maxCurrentDayViewIndex;
                                    }

                                    if (resultLowestVisibleX <= minLowestVisibleX)
                                    {
                                        resultLowestVisibleX = minLowestVisibleX;
                                        currentDayViewIndex = minCurrentDayViewIndex;
                                    }
                                    else if (resultLowestVisibleX >= maxLowestVisibleX)
                                    {
                                        resultLowestVisibleX = maxLowestVisibleX;
                                        currentDayViewIndex = maxCurrentDayViewIndex;
                                    }

                                    lowestVisibleX = resultLowestVisibleX;
                                    currentLowestVisibleX = lowestVisibleX;
                                    trackingLowestVisibleX = currentLowestVisibleX;

                                    IRunnable specificJob = MoveViewJob.GetInstance(currentChart.ViewPortHandler, lowestVisibleX, 0f,
                                        currentChart.GetTransformer(YAxis.AxisDependency.Left), currentChart);
                                    specificJob.Run();
                                    currentChart.Invalidate();

                                    float[] pts = { lowestVisibleX, 0f };
                                    currentChart.GetTransformer(YAxis.AxisDependency.Left).PointValuesToPixel(pts);
                                    currentChart.ViewPortHandler.Translate(pts, currentChart.Matrix);
                                    currentChart.Invalidate();

                                    BarEntry dayViewTariff = dayViewTariffList[currentDayViewIndex];
                                    Highlight centerBar = new Highlight(currentDayViewIndex, 0, dayViewTariff.GetYVals().Length - 1);
                                    currentChart.HighlightValue(centerBar, false);

                                    currentFragment.SetDayViewMonthText(dayViewMonthList[currentDayViewIndex]);
                                }
                            }
                        }
                    }
                }
            }
        }

        private class BarGraphPinchListener : ScaleGestureDetector.SimpleOnScaleGestureListener
        {
            ChartType currentChartType;
            float currentScaleFactor = 1.00f;

            DashboardChartContract.IUserActionsListener userActionsListener;

            public BarGraphPinchListener(ChartType mType, DashboardChartContract.IUserActionsListener currentListener)
            {
                this.currentChartType = mType;
                currentScaleFactor = 1.00f;
                this.userActionsListener = currentListener;
            }

            public override void OnScaleEnd(ScaleGestureDetector detector)
            {
                if (currentChartType == ChartType.Day)
                {
                    if ((currentScaleFactor - detector.ScaleFactor) > 0.005)
                    {
                        currentScaleFactor = detector.ScaleFactor;
                        if (isZoomIn)
                        {
                            isZoomIn = false;
                            this.userActionsListener.OnByZoom();
                        }
                    }
                    else if ((detector.ScaleFactor - currentScaleFactor) > 0.005)
                    {
                        currentScaleFactor = detector.ScaleFactor;
                        if (!isZoomIn)
                        {
                            isZoomIn = true;
                            this.userActionsListener.OnByZoom();
                        }
                    }
                }
            }
        }

        public bool GetIsREAccount()
        {
            return isREAccount;
        }

        public void OnShowDashboardFragmentTutorialDialog()
        {
            try
            {
                Activity.RunOnUiThread(() =>
                {
                    try
                    {
                        StopScrolling();
                    }
                    catch (System.Exception e)
                    {
                        Utility.LoggingNonFatalError(e);
                    }
                });
            }
            catch (System.Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }

            if (ChartDataType != ChartDataType.RM)
            {
                rmKwhSelectDropdown.Visibility = ViewStates.Gone;
                rmKwhLabel.Text = "RM  ";
                rmLabel.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(this.Activity, Resource.Color.powerBlue)));
                kwhLabel.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(this.Activity, Resource.Color.new_grey)));
                isShowAnimationDisable = true;
                ShowByRM();
            }

            if (tarifToggle.Enabled && isToggleTariff)
            {
                try
                {
                    imgTarifToggle.SetImageResource(Resource.Drawable.eye);
                    tarifToggle.SetBackgroundResource(Resource.Drawable.rectangle_white_outline_rounded_button_bg);
                    txtTarifToggle.SetTextColor(new Color(ContextCompat.GetColor(this.Activity, Resource.Color.white)));
                    txtTarifToggle.Alpha = 1f;
                    txtTarifToggle.Text = Utility.GetLocalizedLabel("Usage", "tariffBlock");
                    isToggleTariff = false;
                    if (isChangeBackgroundNeeded)
                    {
                        scrollViewContent.SetBackgroundResource(Resource.Drawable.dashboard_chart_bg);
                    }

                    mChart.Clear();
                    SetUp();
                }
                catch (System.Exception ne)
                {
                    Utility.LoggingNonFatalError(ne);
                }
            }

            NewAppTutorialUtils.OnShowNewAppTutorial(this.Activity, this, PreferenceManager.GetDefaultSharedPreferences(this.Activity), this.mPresenter.OnGeneraNewAppTutorialList());
        }

        public void DashboardCustomScrolling(int yPosition)
        {
            try
            {
                Activity.RunOnUiThread(() =>
                {
                    try
                    {
                        scrollView.ScrollTo(0, yPosition);
                        scrollView.RequestLayout();
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



        public bool CheckIsScrollable()
        {
            View child = (View)scrollView.GetChildAt(0);

            return scrollView.Height < child.Height + scrollView.PaddingTop + scrollView.PaddingBottom;
        }

        public int GetSMRCardHeight()
        {
            return ssmrHistoryContainer.Height;
        }

        public int GetSMRCardLocation()
        {
            int i = 0;

            try
            {
                Rect offsetViewBounds = new Rect();
                //returns the visible bounds
                ssmrHistoryContainer.GetDrawingRect(offsetViewBounds);
                // calculates the relative coordinates to the parent

                rootView.OffsetDescendantRectToMyCoords(ssmrHistoryContainer, offsetViewBounds);

                i = offsetViewBounds.Top;

            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

            return i;
        }

        public int OnGetEndOfScrollView()
        {
            View child = (View)scrollView.GetChildAt(0);

            return child.Height + scrollView.PaddingTop + scrollView.PaddingBottom;
        }

        public void StopScrolling()
        {
            try
            {
                scrollView.SmoothScrollBy(0, 0);
                scrollView.ScrollTo(0, 0);
                scrollView.RequestLayout();
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void HideBottomSheet()
        {
            isTutorialShow = true;
            bottomSheetBehavior.State = BottomSheetBehavior.StateHidden;
        }

        public void ShowBottomSheet()
        {
            isTutorialShow = false;
            bottomSheetBehavior.State = BottomSheetBehavior.StateExpanded;
        }

        public void OnShowMDMSDownPopup()
        {
            if (GetIsMDMSDown() && isSMAccount)
            {
                if (!isMDMSPlannedDownTime)
                {
                    MyTNBAppToolTipBuilder mdmsDownPopup = MyTNBAppToolTipBuilder.Create(this.Activity, MyTNBAppToolTipBuilder.ToolTipType.NORMAL_WITH_HEADER_TWO_BUTTON)
                        .SetTitle(MDMSUnavailableTitle)
                        .SetMessage(MDMSUnavailableMessage)
                        .SetSecondaryCTALabel(MDMSUnavailableCTA)
                        .SetSecondaryCTAaction(userActionsListener.OnTapRefresh)
                        .SetCTALabel(Utility.GetLocalizedLabel("Common", "gotIt"))
                        .Build();
                    mdmsDownPopup.Show();
                }
                else
                {
                    MyTNBAppToolTipBuilder mdmsDownPopup = MyTNBAppToolTipBuilder.Create(this.Activity, MyTNBAppToolTipBuilder.ToolTipType.NORMAL_WITH_HEADER)
                    .SetTitle(MDMSUnavailableTitle)
                    .SetMessage(MDMSUnavailableMessage)
                    .SetCTALabel(MDMSUnavailableCTA)
                    .Build();
                    mdmsDownPopup.Show();
                }
            }
        }

        /// <summary>
        /// Set MDMS Down Message - Planned
        /// </summary>
        /// <param name="response"></param>
        public void SetMDMSDownMessage(SMUsageHistoryResponse response)
        {
            isMDMSPlannedDownTime = true;
            if (response != null && response.Data != null)
            {
                if (!string.IsNullOrEmpty(response.Data.DisplayTitle))
                {
                    MDMSUnavailableTitle = response.Data.DisplayTitle;
                }
                else
                {
                    MDMSUnavailableTitle = this.Activity.GetString(Resource.String.tooltip_what_does_this_link);
                }

                if (!string.IsNullOrEmpty(response.Data.DisplayMessage))
                {
                    MDMSUnavailableMessage = response.Data.DisplayMessage;
                }

                if (!string.IsNullOrEmpty(response.Data.CTA))
                {
                    MDMSUnavailableCTA = response.Data.CTA;
                }
                else
                {
                    MDMSUnavailableCTA = this.Activity.GetString(Resource.String.tooltip_btnLabel);
                }

                imgMdmsDayViewDown.SetImageResource(Resource.Drawable.mdms_down_dayview);
                btnMDMSDownRefresh.Visibility = ViewStates.Gone;

                if (Build.VERSION.SdkInt >= BuildVersionCodes.N)
                {
                    txtMdmsDayViewDown.TextFormatted = Html.FromHtml(response.Data.ErrorMessage, FromHtmlOptions.ModeLegacy);
                }
                else
                {
                    txtMdmsDayViewDown.TextFormatted = Html.FromHtml(response.Data.ErrorMessage);
                }
            }
        }

        /// <summary>
        /// Set MDMS Down Message - Unplanned
        /// </summary>
        /// <param name="response"></param>
        public void SetMDMSDownRefreshMessage(SMUsageHistoryResponse response)
        {
            isMDMSPlannedDownTime = false;

            if (response != null && response.Data != null)
            {
                if (!string.IsNullOrEmpty(response.Data.DisplayTitle))
                {
                    MDMSUnavailableTitle = response.Data.DisplayTitle;
                }
                else
                {
                    MDMSUnavailableTitle = this.Activity.GetString(Resource.String.tooltip_what_does_this_link);
                }

                if (!string.IsNullOrEmpty(response.Data.DisplayMessage))
                {
                    MDMSUnavailableMessage = response.Data.DisplayMessage;
                }

                if (!string.IsNullOrEmpty(response.Data.CTA))
                {
                    MDMSUnavailableCTA = response.Data.CTA;
                }
                else
                {
                    MDMSUnavailableCTA = this.Activity.GetString(Resource.String.text_new_refresh);
                }

                imgMdmsDayViewDown.SetImageResource(Resource.Drawable.refresh_white);
                btnMDMSDownRefresh.Visibility = ViewStates.Visible;

                btnMDMSDownRefresh.Text = !string.IsNullOrEmpty(response.Data.RefreshBtnText) ? response.Data.RefreshBtnText : Utility.GetLocalizedCommonLabel("refreshNow");

                if (Build.VERSION.SdkInt >= BuildVersionCodes.N)
                {
                    txtMdmsDayViewDown.TextFormatted = Html.FromHtml(response.Data.ErrorMessage, FromHtmlOptions.ModeLegacy);
                }
                else
                {
                    txtMdmsDayViewDown.TextFormatted = Html.FromHtml(response.Data.ErrorMessage);
                }
            }
        }

        private bool IsCheckDataReadyData(UsageHistoryData data)
        {
            bool isHaveData = true;

            if (data == null || (data != null && data.ByMonth == null) || (data != null && data.ByMonth != null && data.ByMonth.Months == null) || (data != null && data.ByMonth != null && data.ByMonth.Months != null && data.ByMonth.Months.Count == 0))
            {
                isHaveData = false;
            }
            else
            {
                for (int i = 0; i < data.ByMonth.Months.Count; i++)
                {
                    if ((string.IsNullOrEmpty(data.ByMonth.Months[i].UsageTotal.ToString()) && string.IsNullOrEmpty(data.ByMonth.Months[i].AmountTotal.ToString())) || (System.Math.Abs(data.ByMonth.Months[i].UsageTotal) < 0.001 && System.Math.Abs(data.ByMonth.Months[i].AmountTotal) < 0.001))
                    {
                        isHaveData = false;
                    }
                    else
                    {
                        isHaveData = true;
                        break;
                    }
                }
            }

            return isHaveData;
        }

        private bool IsCheckDataReadyData(SMUsageHistoryData data)
        {
            bool isHaveData = true;

            if (data == null || (data != null && data.ByMonth == null) || (data != null && data.ByMonth != null && data.ByMonth.Months == null) || (data != null && data.ByMonth != null && data.ByMonth.Months != null && data.ByMonth.Months.Count == 0))
            {
                isHaveData = false;
            }
            else
            {
                for (int i = 0; i < data.ByMonth.Months.Count; i++)
                {
                    if ((string.IsNullOrEmpty(data.ByMonth.Months[i].UsageTotal.ToString()) && string.IsNullOrEmpty(data.ByMonth.Months[i].AmountTotal.ToString())) || (System.Math.Abs(data.ByMonth.Months[i].UsageTotal) < 0.001 && System.Math.Abs(data.ByMonth.Months[i].AmountTotal) < 0.001))
                    {
                        isHaveData = false;
                    }
                    else
                    {
                        isHaveData = true;
                        break;
                    }
                }
            }

            return isHaveData;
        }

        public void CheckSMRAccountValidaty()
        {
            if (!isSMR)
            {
                isSMR = true;
                StartSSMRDashboardViewShimmer();
                isChangeVirtualHeightNeed = true;
                SetVirtualHeightParams(6f);
                isChangeBackgroundNeeded = true;
                try
                {
                    ((DashboardHomeActivity)Activity).SetStatusBarBackground(Resource.Drawable.UsageGradientBackground);
                    ((DashboardHomeActivity)Activity).SetToolbarBackground(Resource.Drawable.CustomDashboardGradientToolbar);
                }
                catch (System.Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                }
                rootView.SetBackgroundResource(Resource.Color.background_pale_grey);
                scrollViewContent.SetBackgroundResource(Resource.Drawable.dashboard_chart_bg);
            }
        }

        public void OnShowPlannedDowntimeScreen(string contentTxt)
        {
            try
            {
                Activity.RunOnUiThread(() =>
                {
                    try
                    {
                        refreshLayout.Visibility = ViewStates.Visible;
                        newAccountLayout.Visibility = ViewStates.Gone;
                        allGraphLayout.Visibility = ViewStates.Gone;
                        smStatisticContainer.Visibility = ViewStates.Gone;

                        if (isREAccount || isSMR)
                        {

                        }
                        else
                        {
                            rootView.SetBackgroundResource(0);
                            scrollViewContent.SetBackgroundResource(0);
                            dashboard_bottom_view.SetBackgroundResource(0);
                            try
                            {
                                ((DashboardHomeActivity)Activity).SetStatusBarBackground(Resource.Drawable.NewHorizontalGradientBackground);
                                ((DashboardHomeActivity)Activity).UnsetToolbarBackground();
                            }
                            catch (System.Exception e)
                            {
                                Utility.LoggingNonFatalError(e);
                            }
                        }

                        refresh_image.SetImageResource(Resource.Drawable.maintenance_white);
                        SetMaintenanceLayoutParams();
                        StopAddressShimmer();
                        StopRangeShimmer();
                        StopGraphShimmer();
                        StopSMStatisticShimmer();
                        energyTipsView.Visibility = ViewStates.Gone;
                        btnNewRefresh.Visibility = ViewStates.Gone;

                        if (!string.IsNullOrEmpty(contentTxt))
                        {
                            if (Build.VERSION.SdkInt >= BuildVersionCodes.N)
                            {
                                txtNewRefreshMessage.TextFormatted = Html.FromHtml(contentTxt, FromHtmlOptions.ModeLegacy);
                            }
                            else
                            {
                                txtNewRefreshMessage.TextFormatted = Html.FromHtml(contentTxt);
                            }
                        }
                        else
                        {
                            if (Build.VERSION.SdkInt >= BuildVersionCodes.N)
                            {
                                txtNewRefreshMessage.TextFormatted = Html.FromHtml(Utility.GetLocalizedLabel("Error", "plannedDownTimeMessage"), FromHtmlOptions.ModeLegacy);
                            }
                            else
                            {
                                txtNewRefreshMessage.TextFormatted = Html.FromHtml(Utility.GetLocalizedLabel("Error", "plannedDownTimeMessage"));
                            }
                        }

                        isChangeVirtualHeightNeed = true;
                        SetVirtualHeightParams(6f);
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
    }
}
