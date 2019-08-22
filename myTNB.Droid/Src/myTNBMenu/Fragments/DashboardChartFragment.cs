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
using static myTNB_Android.Src.myTNBMenu.Listener.NMREDashboardScrollView;
using static myTNB_Android.Src.myTNBMenu.Models.GetInstallationDetailsResponse;

namespace myTNB_Android.Src.myTNBMenu.Fragments
{
    public class DashboardChartFragment : BaseFragment, DashboardChartContract.IView, NMREDashboardScrollViewListener, ViewTreeObserver.IOnGlobalLayoutListener, MikePhil.Charting.Listener.IOnChartValueSelectedListenerSupport
    {

        [BindView(Resource.Id.totalPayableLayout)]
        RelativeLayout totalPayableLayout;

        [BindView(Resource.Id.txtDueDate)]
        TextView txtDueDate;

        [BindView(Resource.Id.rootView)]
        CoordinatorLayout rootView;

        UsageHistoryData selectedHistoryData;

        AccountData selectedAccount;

        ChartType ChartType = ChartType.RM;

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

        private NMREDashboardScrollView scrollView;

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

        [BindView(Resource.Id.ssmrHistoryContainer)]
        LinearLayout ssmrHistoryContainer;

        [BindView(Resource.Id.ssmrAccountStatusText)]
        TextView ssmrAccountStatusText;

        [BindView(Resource.Id.btnTxtSsmrViewHistory)]
        TextView btnTxtSsmrViewHistory;

        [BindView(Resource.Id.btnReadingHistory)]
        Button btnReadingHistory;

        EnergySavingTipsAdapter energyTipsAdapter;

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

        TariffBlockLegendAdapter tariffBlockLegendAdapter;

        private DashboardChartContract.IUserActionsListener userActionsListener;
        private DashboardChartPresenter mPresenter;

        private string txtRefreshMsg = "";
        private string txtBtnRefreshTitle = "";

        private bool isSubmitMeter = false;

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

        public StackedBarChartRenderer renderer;

        static bool requireScroll;

        private int CurrentParentIndex = -1;

        public override int ResourceId()
        {
            return Resource.Layout.DashboardNewChartView;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Bundle extras = Arguments;

            if (extras.ContainsKey(Constants.SELECTED_ACCOUNT_USAGE_RESPONSE))
            {
                isUsageLoadedNeeded = false; 
                var usageHistoryDataResponse = JsonConvert.DeserializeObject<UsageHistoryResponse>(extras.GetString(Constants.SELECTED_ACCOUNT_USAGE_RESPONSE));
                selectedHistoryData = usageHistoryDataResponse.Data.UsageHistoryData;
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
                isUsageLoadedNeeded = true;
                selectedHistoryData = null;
                txtRefreshMsg = "Uh oh, looks like this page is unplugged. Reload to stay plugged in!";
                txtBtnRefreshTitle = "Reload Now";
            }

            if (extras.ContainsKey(Constants.SELECTED_ACCOUNT))
            {
                selectedAccount = JsonConvert.DeserializeObject<AccountData>(extras.GetString(Constants.SELECTED_ACCOUNT));
            }

            errorMSG = "";

            if (extras.ContainsKey(Constants.SELECTED_ERROR_MSG))
            {
                errorMSG = extras.GetString(Constants.SELECTED_ERROR_MSG);
            }


            SetHasOptionsMenu(true);
            this.mPresenter = new DashboardChartPresenter(this);

            ((DashboardHomeActivity)Activity).HideBottomNavigationBar();
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

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            try
            {
                bottomSheetBehavior = BottomSheetBehavior.From(bottomSheet);
                bottomSheetBehavior.State = BottomSheetBehavior.StateExpanded;
                bottomSheetBehavior.SetBottomSheetCallback(new DashboardBottomSheetCallBack());

                scrollView = view.FindViewById<NMREDashboardScrollView>(Resource.Id.scroll_view);
                ViewTreeObserver observer = scrollView.ViewTreeObserver;
                observer.AddOnGlobalLayoutListener(this);

                scrollViewContent.SetBackgroundResource(Resource.Drawable.dashboard_chart_bg);

                requireScroll = false;

                TextViewUtils.SetMuseoSans300Typeface(txtAddress, txtTotalPayable, txtDueDate);
                TextViewUtils.SetMuseoSans300Typeface(txtNewRefreshMessage, ssmrAccountStatusText);
                TextViewUtils.SetMuseoSans500Typeface(txtRange, txtTotalPayableTitle, txtTotalPayableCurrency, btnViewBill, btnPay, btnNewRefresh, rmKwhLabel, kwhLabel, rmLabel, dashboardAccountName, btnTxtSsmrViewHistory, btnReadingHistory, txtEnergyDisconnection);
                TextViewUtils.SetMuseoSans300Typeface(reTotalPayable, reTotalPayableCurrency, reDueDate, txtNoPayable);
                TextViewUtils.SetMuseoSans500Typeface(reTotalPayableTitle, btnReView, txtTarifToggle, txtNoPayableTitle, txtNoPayableCurrency);

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
                    }
                    else
                    {
                        isREAccount = false;
                        reContainer.Visibility = ViewStates.Gone;
                        btnPay.Visibility = ViewStates.Visible;
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

                    LinearLayoutManager linearEnergyTipLayoutManager = new LinearLayoutManager(this.Activity, LinearLayoutManager.Horizontal, false);
                    energyTipsList.SetLayoutManager(linearEnergyTipLayoutManager);
                    energyTipsList.NestedScrollingEnabled = true;

                    LinearSnapHelper snapHelper = new LinearSnapHelper();
                    snapHelper.AttachToRecyclerView(energyTipsList);

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

                    energyDisconnectionButton.Visibility = ViewStates.Gone;

                    OnGetEnergyTipsItems();


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


        /*[OnClick(Resource.Id.txtWhyThisAmt)]
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
        }*/

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

            bool isTariffAvailable = true;
            for (int i = 0; i < selectedHistoryData.ByMonth.Months.Count; i++)
            {
                if (selectedHistoryData.ByMonth.Months[i].TariffBlocksList != null && selectedHistoryData.ByMonth.Months[i].TariffBlocksList.Count > 0)
                {
                    isTariffAvailable = true;
                }
                else
                {
                    isTariffAvailable = false;
                }
            }

            if (isTariffAvailable)
            {
                tarifToggle.Enabled = true;
            }
            else
            {
                tarifToggle.Enabled = false;
                isToggleTariff = false;
            }

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

            mChart.SetDrawBarShadow(false);
            mChart.SetDrawValueAboveBar(true);
            mChart.Description.Enabled = false;
            mChart.SetMaxVisibleValueCount(7);
            mChart.SetPinchZoom(false);
            mChart.SetDrawGridBackground(false);
            mChart.SetScaleEnabled(false);
            mChart.Legend.Enabled = false;
            mChart.AnimateY(1000);

            txtAddress.Text = selectedAccount.AddStreet;

            if (ChartType == ChartType.RM)
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

            XLabelsFormatter = new ChartsMonthFormatter(selectedHistoryData.ByMonth, mChart);

            XAxis xAxis = mChart.XAxis;
            xAxis.Position = XAxisPosition.Bottom;
            xAxis.TextColor = Color.ParseColor("#ffffff");
            xAxis.AxisLineWidth = 2f;
            xAxis.AxisLineColor = Color.ParseColor("#4cffffff");

            xAxis.SetDrawGridLines(false);

            xAxis.Granularity = 1f; // only intervals of 1 day
            xAxis.LabelCount = selectedHistoryData.ByMonth.Months.Count;
            xAxis.ValueFormatter = XLabelsFormatter;


        }
        #endregion

        #region SETUP AXIS KWH
        internal void SetUpXAxiskWh()
        {
            XLabelsFormatter = new ChartsKWhFormatter(selectedHistoryData.ByMonth, mChart);

            XAxis xAxis = mChart.XAxis;
            xAxis.Position = XAxisPosition.Bottom;
            xAxis.TextColor = Color.ParseColor("#ffffff");
            xAxis.AxisLineWidth = 2f;
            xAxis.AxisLineColor = Color.ParseColor("#4cffffff");

            xAxis.SetDrawGridLines(false);

            xAxis.Granularity = 1f; // only intervals of 1 day
            xAxis.LabelCount = selectedHistoryData.ByMonth.Months.Count;
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
            SelectedMarkerView markerView = new SelectedMarkerView(Activity)
            {
                UsageHistoryData = selectedHistoryData,
                ChartType = ChartType.RM,
                AccountType = selectedAccount.AccountCategoryId
            };
            markerView.ChartView = mChart;
            mChart.Marker = markerView;
        }
        #endregion

        #region SETUP MARKERVIEW KWH/ HIGHLIGHT TEXT
        internal void SetUpMarkerKWhView()
        {
            SelectedMarkerView markerView = new SelectedMarkerView(Activity)
            {
                UsageHistoryData = selectedHistoryData,
                ChartType = ChartType.kWh,
                AccountType = selectedAccount.AccountCategoryId
            };
            markerView.ChartView = mChart;
            mChart.Marker = markerView;
        }
        #endregion

        #region SETUP RM DATA
        internal void SetData(int barLength)
        {
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
                        yVals1.Add(new BarEntry(i, 0));
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
        #endregion

        #region SETUP KWH DATA
        internal void SetKWhData(int barLength)
        {
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
                        yVals1.Add(new BarEntry(i, 0));
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
        #endregion

        public void ShowByKwh()
        {
            ChartType = ChartType.kWh;

            mChart.Visibility = ViewStates.Visible;

            rmKwhSelection.Enabled = true;
            tarifToggle.Enabled = true;

            mChart.Clear();
            SetUp();
        }

        public void ShowByRM()
        {
            ChartType = ChartType.RM;

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
                    activity.SetStatusBarBackground(Resource.Drawable.dashboard_fluid_background);
                    activity.SetToolbarBackground(Resource.Drawable.CustomDashboardGradientToolbar);
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
                    scrollViewContent.SetBackgroundResource(Resource.Drawable.dashboard_chart_bg);
                }
                else
                {
                    imgTarifToggle.SetImageResource(Resource.Drawable.eye_hide);
                    txtTarifToggle.Text = "Hide Tariff";
                    isToggleTariff = true;
                    tariffBlockLegendRecyclerView.Visibility = ViewStates.Visible;
                    if (tariffBlockLegendAdapter == null)
                    {
                        tariffBlockLegendAdapter = new TariffBlockLegendAdapter(selectedHistoryData.TariffBlocksLegend, this.Activity);
                        tariffBlockLegendRecyclerView.SetAdapter(tariffBlockLegendAdapter);
                    }
                    Context context = tariffBlockLegendRecyclerView.Context;
                    LayoutAnimationController controller =
                            AnimationUtils.LoadLayoutAnimation(context, Resource.Animation.layout_animation_fall_down);

                    tariffBlockLegendRecyclerView.LayoutAnimation = controller;
                    tariffBlockLegendRecyclerView.GetAdapter().NotifyDataSetChanged();
                    tariffBlockLegendRecyclerView.ScheduleLayoutAnimation();
                    scrollViewContent.SetBackgroundResource(Resource.Drawable.dashboard_chart_extended_bg);
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
                ((DashboardHomeActivity)Activity).SetToolbarBackground(Resource.Drawable.CustomGradientToolBar);
                ((DashboardHomeActivity)Activity).SetStatusBarBackground(Resource.Drawable.bg_smr);
                scrollViewContent.SetBackgroundResource(Resource.Drawable.dasbord_chart_refresh_bg);


                refreshLayout.Visibility = ViewStates.Visible;
                allGraphLayout.Visibility = ViewStates.Gone;
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
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
            return val;
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

        void NMREDashboardScrollViewListener.OnScrollChanged(NMREDashboardScrollView v, int l, int t, int oldl, int oldt)
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

        private class DashboardBottomSheetCallBack : BottomSheetBehavior.BottomSheetCallback
        {
            public override void OnSlide(View bottomSheet, float slideOffset)
            {

            }

            public override void OnStateChanged(View bottomSheet, int newState)
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
                List<EnergySavingTipsModel> energyList = new List<EnergySavingTipsModel>();
                List<EnergySavingTipsModel> localList = EnergyTipsUtils.GetAllItems();
                if (localList != null && localList.Count > 0)
                {
                    var random = new System.Random();
                    List<int> listNumbers = new List<int>();
                    int number;
                    for (int i = 0; i < 3; i++)
                    {
                        do
                        {
                            number = random.Next(1, localList.Count);
                        } while (listNumbers.Contains(number));
                        listNumbers.Add(number);
                    }


                    for (int j = 0; j < listNumbers.Count; j++)
                    {
                        energyList.Add(localList[listNumbers[j]]);
                    }

                    energyTipsAdapter = new EnergySavingTipsAdapter(energyList, this.Activity);
                    energyTipsList.SetAdapter(energyTipsAdapter);
                }
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        void ViewTreeObserver.IOnGlobalLayoutListener.OnGlobalLayout()
        {
            try
            {
                int viewHeight = scrollView.MeasuredHeight;
                int contentHeight = scrollView.GetChildAt(0).Height;
                if (viewHeight - contentHeight < 0)
                {
                    scrollView.SetScrollingEnabled(true);
                    scrollView.SmoothScrollingEnabled = true;
                    scrollView.setOnScrollViewListener(this);
                }
                else
                {
                    scrollView.SetScrollingEnabled(false);
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

        void IOnChartValueSelectedListenerSupport.OnValueSelected(Entry e, Highlight h)
        {
            try
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

        public void SetRefreshLayoutParams()
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

        public void SetMaintenanceLayoutParams()
        {
            LinearLayout.LayoutParams refreshImgParams = refresh_image.LayoutParameters as LinearLayout.LayoutParams;

            refreshImgParams.Width = GetDeviceHorizontalScaleInPixel(0.603f);
            refreshImgParams.Height = GetDeviceHorizontalScaleInPixel(0.603f);
            refreshImgParams.TopMargin = (int) DPUtils.ConvertDPToPx(8f);
            refresh_image.RequestLayout();

            LinearLayout.LayoutParams refreshTxtParams = txtNewRefreshMessage.LayoutParameters as LinearLayout.LayoutParams;
            refreshTxtParams.TopMargin = (int)DPUtils.ConvertDPToPx(6f);
            txtNewRefreshMessage.RequestLayout();
        }
    }
}
