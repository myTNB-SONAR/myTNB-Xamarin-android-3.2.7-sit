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

        bool hasNoInternet;

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

        TariffBlockLegendAdapter tariffBlockLegendAdapter;

        private DashboardChartContract.IUserActionsListener userActionsListener;
        private DashboardChartPresenter mPresenter;

        private string txtRefreshMsg = "";
        private string txtBtnRefreshTitle = "";

        private bool hasAmtDue = false;

        private bool amountDueFailed = false;

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

        bool isBillingAvailable = true;

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


            if (extras.ContainsKey(Constants.NO_INTERNET_CONNECTION))
            {
                hasNoInternet = extras.GetBoolean(Constants.NO_INTERNET_CONNECTION);
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

            if (extras.ContainsKey(Constants.SELECTED_ACCOUNT))
            {
                selectedAccount = JsonConvert.DeserializeObject<AccountData>(extras.GetString(Constants.SELECTED_ACCOUNT));
            }

            if (extras.ContainsKey(Constants.AMOUNT_DUE_FAILED_KEY))
            {
                amountDueFailed = extras.GetBoolean(Constants.AMOUNT_DUE_FAILED_KEY);
            }

            if (extras.ContainsKey(Constants.AMOUNT_DUE_RESPONSE_KEY))
            {
                amountDueResponse = JsonConvert.DeserializeObject<AccountDueAmountResponse>(extras.GetString(Constants.AMOUNT_DUE_RESPONSE_KEY));
            }
            else
            {
                amountDueResponse = null;
            }

            if (extras.ContainsKey(Constants.IS_BILLING_AVAILABLE_KEY))
            {
                isBillingAvailable = extras.GetBoolean(Constants.IS_BILLING_AVAILABLE_KEY);
            }

            errorMSG = "";

            if (!hasNoInternet)
            {
                if (extras.ContainsKey(Constants.SELECTED_ACCOUNT_USAGE))
                {
                    selectedHistoryData = JsonConvert.DeserializeObject<UsageHistoryData>(extras.GetString(Constants.SELECTED_ACCOUNT_USAGE));
                }

                if (extras.ContainsKey(Constants.SELECTED_ACCOUNT_USAGE_RESPONSE))
                {
                    var usageHistoryDataResponse = JsonConvert.DeserializeObject<UsageHistoryResponse>(extras.GetString(Constants.SELECTED_ACCOUNT_USAGE_RESPONSE));
                    selectedHistoryData = usageHistoryDataResponse.Data.UsageHistoryData;
                }

                if (extras.ContainsKey(Constants.SELECTED_ERROR_MSG))
                {
                    errorMSG = extras.GetString(Constants.SELECTED_ERROR_MSG);
                }
            }
            else
            {
                if (extras.ContainsKey(Constants.SELECTED_ACCOUNT_USAGE_RESPONSE))
                {
                    var usageHistoryDataResponse = JsonConvert.DeserializeObject<UsageHistoryResponse>(extras.GetString(Constants.SELECTED_ACCOUNT_USAGE_RESPONSE));
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
                    txtRefreshMsg = "Uh oh, looks like this page is unplugged. Reload to stay plugged in!";
                    txtBtnRefreshTitle = "Reload Now";
                }
            }


            SetHasOptionsMenu(true);
            this.mPresenter = new DashboardChartPresenter(this);
        }

        internal static DashboardChartFragment NewInstance(UsageHistoryData usageHistoryData, AccountData accountData)
        {
            DashboardChartFragment chartFragment = new DashboardChartFragment();
            Bundle bundle = new Bundle();
            string data = JsonConvert.SerializeObject(usageHistoryData);
            bundle.PutString(Constants.SELECTED_ACCOUNT_USAGE, data);
            bundle.PutString(Constants.SELECTED_ACCOUNT, JsonConvert.SerializeObject(accountData));
            chartFragment.Arguments = bundle;
            return chartFragment;
        }

        internal static DashboardChartFragment NewInstance(bool isAmountDueDown, bool isGraphDown, bool isBillingAvailable, UsageHistoryResponse response, AccountData selectedAccount, AccountDueAmountResponse amountDueResponse)
        {
            DashboardChartFragment chartFragment = new DashboardChartFragment();
            Bundle bundle = new Bundle();
            string data = JsonConvert.SerializeObject(response);
            bundle.PutString(Constants.SELECTED_ACCOUNT_USAGE_RESPONSE, data);
            bundle.PutString(Constants.SELECTED_ACCOUNT, JsonConvert.SerializeObject(selectedAccount));
            bundle.PutString(Constants.AMOUNT_DUE_RESPONSE_KEY, JsonConvert.SerializeObject(amountDueResponse));
            bundle.PutBoolean(Constants.AMOUNT_DUE_FAILED_KEY, isAmountDueDown);
            bundle.PutBoolean(Constants.IS_BILLING_AVAILABLE_KEY, isBillingAvailable);
            bundle.PutBoolean(Constants.NO_INTERNET_CONNECTION, isGraphDown);
            chartFragment.Arguments = bundle;
            return chartFragment;
        }

        internal static DashboardChartFragment NewInstance(bool isAmountDueDown, bool isGraphDown, bool isBillingAvailable, UsageHistoryResponse response, AccountData selectedAccount, AccountDueAmountResponse amountDueResponse, string error, string errorMessage)
        {
            DashboardChartFragment chartFragment = new DashboardChartFragment();
            Bundle bundle = new Bundle();
            string data = JsonConvert.SerializeObject(response);
            bundle.PutString(Constants.SELECTED_ACCOUNT_USAGE_RESPONSE, data);
            bundle.PutString(Constants.SELECTED_ACCOUNT, JsonConvert.SerializeObject(selectedAccount));
            bundle.PutString(Constants.AMOUNT_DUE_RESPONSE_KEY, JsonConvert.SerializeObject(amountDueResponse));
            bundle.PutBoolean(Constants.AMOUNT_DUE_FAILED_KEY, isAmountDueDown);
            bundle.PutBoolean(Constants.NO_INTERNET_CONNECTION, isGraphDown);
            bundle.PutBoolean(Constants.IS_BILLING_AVAILABLE_KEY, isBillingAvailable);
            bundle.PutString(Constants.SELECTED_ERROR, error);
            bundle.PutString(Constants.SELECTED_ERROR_MSG, errorMessage);
            chartFragment.Arguments = bundle;
            return chartFragment;
        }

        internal static DashboardChartFragment NewInstance(UsageHistoryData usageHistoryData, AccountData accountData, string error, string errorMessage)
        {
            DashboardChartFragment chartFragment = new DashboardChartFragment();
            Bundle bundle = new Bundle();
            string data = JsonConvert.SerializeObject(usageHistoryData);
            bundle.PutString(Constants.SELECTED_ACCOUNT_USAGE, data);
            bundle.PutString(Constants.SELECTED_ACCOUNT, JsonConvert.SerializeObject(accountData));
            bundle.PutString(Constants.SELECTED_ERROR, error);
            bundle.PutString(Constants.SELECTED_ERROR_MSG, errorMessage);
            chartFragment.Arguments = bundle;
            return chartFragment;
        }

        internal static DashboardChartFragment NewInstance(UsageHistoryData usageHistoryData, AccountData accountData, string error, string errorMessage, AccountDueAmountResponse amountDueResponse)
        {
            DashboardChartFragment chartFragment = new DashboardChartFragment();
            Bundle bundle = new Bundle();
            string data = JsonConvert.SerializeObject(usageHistoryData);
            bundle.PutString(Constants.SELECTED_ACCOUNT_USAGE, data);
            bundle.PutString(Constants.SELECTED_ACCOUNT, JsonConvert.SerializeObject(accountData));
            bundle.PutString(Constants.SELECTED_ERROR, error);
            bundle.PutString(Constants.SELECTED_ERROR_MSG, errorMessage);
            if (amountDueResponse != null)
            {
                bundle.PutString(Constants.AMOUNT_DUE_RESPONSE_KEY, JsonConvert.SerializeObject(amountDueResponse));
            }
            chartFragment.Arguments = bundle;
            return chartFragment;
        }

        internal static DashboardChartFragment NewInstance(bool hasNoInternet, UsageHistoryResponse response, AccountData accountData)
        {
            DashboardChartFragment chartFragment = new DashboardChartFragment();
            Bundle bundle = new Bundle();

            bundle.PutBoolean(Constants.NO_INTERNET_CONNECTION, hasNoInternet);
            if (response != null && response.Data != null)
            {
                if (string.IsNullOrEmpty(response.Data.RefreshMessage))
                {
                    bundle.PutString(Constants.REFRESH_MSG, "The graph must be tired. Tap the button below to help it out.");
                }
                else
                {
                    bundle.PutString(Constants.REFRESH_MSG, response.Data.RefreshMessage);
                }

                if (!string.IsNullOrEmpty(response.Data.RefreshBtnText))
                {
                    bundle.PutString(Constants.REFRESH_BTN_MSG, response.Data.RefreshBtnText);
                }
            }
            else
            {
                bundle.PutString(Constants.REFRESH_MSG, "The graph must be tired. Tap the button below to help it out.");
                bundle.PutString(Constants.REFRESH_BTN_MSG, "Refresh Now");
            }
            if (accountData != null)
            {
                bundle.PutString(Constants.SELECTED_ACCOUNT, JsonConvert.SerializeObject(accountData));
            }
            chartFragment.Arguments = bundle;
            return chartFragment;
        }

        internal static DashboardChartFragment NewInstance(UsageHistoryData usageHistoryData, AccountData accountData, AccountDueAmountResponse amountDueResponse)
        {
            DashboardChartFragment chartFragment = new DashboardChartFragment();
            Bundle bundle = new Bundle();
            string data = JsonConvert.SerializeObject(usageHistoryData);
            bundle.PutString(Constants.SELECTED_ACCOUNT_USAGE, data);
            bundle.PutString(Constants.SELECTED_ACCOUNT, JsonConvert.SerializeObject(accountData));
            if (amountDueResponse != null)
            {
                bundle.PutString(Constants.AMOUNT_DUE_RESPONSE_KEY, JsonConvert.SerializeObject(amountDueResponse));
            }
            chartFragment.Arguments = bundle;
            return chartFragment;
        }

        internal static DashboardChartFragment NewInstance(bool hasNoInternet, UsageHistoryResponse response, AccountData accountData, AccountDueAmountResponse amountDueResponse)
        {
            DashboardChartFragment chartFragment = new DashboardChartFragment();
            Bundle bundle = new Bundle();

            bundle.PutBoolean(Constants.NO_INTERNET_CONNECTION, hasNoInternet);
            if (response != null && response.Data != null)
            {
                if (string.IsNullOrEmpty(response.Data.RefreshMessage))
                {
                    bundle.PutString(Constants.REFRESH_MSG, "The graph must be tired. Tap the button below to help it out.");
                }
                else
                {
                    bundle.PutString(Constants.REFRESH_MSG, response.Data.RefreshMessage);
                }

                if (!string.IsNullOrEmpty(response.Data.RefreshBtnText))
                {
                    bundle.PutString(Constants.REFRESH_BTN_MSG, response.Data.RefreshBtnText);
                }
            }
            else
            {
                bundle.PutString(Constants.REFRESH_MSG, "The graph must be tired. Tap the button below to help it out.");
                bundle.PutString(Constants.REFRESH_BTN_MSG, "Refresh Now");
            }
            if (accountData != null)
            {
                bundle.PutString(Constants.SELECTED_ACCOUNT, JsonConvert.SerializeObject(accountData));
            }
            if (amountDueResponse != null)
            {
                bundle.PutString(Constants.AMOUNT_DUE_RESPONSE_KEY, JsonConvert.SerializeObject(amountDueResponse));
            }
            chartFragment.Arguments = bundle;
            return chartFragment;
        }

        internal static DashboardChartFragment NewInstance(bool hasNoInternet, bool amountDueFailed, string contentTxt, string btnTxt, AccountData accountData)
        {
            DashboardChartFragment chartFragment = new DashboardChartFragment();
            Bundle bundle = new Bundle();

            bundle.PutBoolean(Constants.AMOUNT_DUE_FAILED_KEY, amountDueFailed);
            bundle.PutBoolean(Constants.NO_INTERNET_CONNECTION, hasNoInternet);
            if (string.IsNullOrEmpty(contentTxt))
            {
                bundle.PutString(Constants.REFRESH_MSG, "This page must be tired. Tap the button below to help it out.");
            }
            else
            {
                bundle.PutString(Constants.REFRESH_MSG, contentTxt);
            }

            if (!string.IsNullOrEmpty(btnTxt))
            {
                bundle.PutString(Constants.REFRESH_BTN_MSG, btnTxt);
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
                if (!hasNoInternet)
                {
                    txtTotalPayable.Text = decimalFormat.Format(selectedAccount.AmtCustBal);
                }

                TextViewUtils.SetMuseoSans300Typeface(txtAddress, txtTotalPayable, txtDueDate);
                TextViewUtils.SetMuseoSans300Typeface(txtNewRefreshMessage, ssmrAccountStatusText);
                TextViewUtils.SetMuseoSans500Typeface(txtRange, txtTotalPayableTitle, txtTotalPayableCurrency, btnViewBill, btnPay, btnNewRefresh, rmKwhLabel, kwhLabel, rmLabel, dashboardAccountName, btnTxtSsmrViewHistory, btnReadingHistory, txtEnergyDisconnection);
                TextViewUtils.SetMuseoSans300Typeface(reTotalPayable, reTotalPayableCurrency, reDueDate, txtNoPayable);
                TextViewUtils.SetMuseoSans500Typeface(reTotalPayableTitle, btnReView, txtTarifToggle, txtNoPayableTitle, txtNoPayableCurrency);


                bottomSheetBehavior = BottomSheetBehavior.From(bottomSheet);
                bottomSheetBehavior.State = BottomSheetBehavior.StateExpanded;
                bottomSheetBehavior.SetBottomSheetCallback(new DashboardBottomSheetCallBack());

                ((DashboardHomeActivity)Activity).HideAccountName();
                if (!hasNoInternet)
                {
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
                }
                else
                {
                    dashboardAccountName.Visibility = ViewStates.Gone;
                }

                tariffBlockLegendRecyclerView.Visibility = ViewStates.Gone;
                LinearLayoutManager linearTariffBlockLayoutManager = new LinearLayoutManager(this.Activity, LinearLayoutManager.Vertical, false);
                tariffBlockLegendRecyclerView.SetLayoutManager(linearTariffBlockLayoutManager);

                if (!hasNoInternet)
                {
                    energyTipsView.Visibility = ViewStates.Visible;
                    LinearLayoutManager linearEnergyTipLayoutManager = new LinearLayoutManager(this.Activity, LinearLayoutManager.Horizontal, false);
                    energyTipsList.SetLayoutManager(linearEnergyTipLayoutManager);
                    energyTipsList.NestedScrollingEnabled = true;

                    LinearSnapHelper snapHelper = new LinearSnapHelper();
                    snapHelper.AttachToRecyclerView(energyTipsList);

                    OnGetEnergyTipsItems();


                }
                else
                {
                    energyTipsView.Visibility = ViewStates.Gone;
                }

                if (!isBillingAvailable)
                {
                    btnViewBill.Enabled = false;
                    btnReView.Enabled = false;
                    btnViewBill.Background = ContextCompat.GetDrawable(this.Activity, Resource.Drawable.silver_chalice_button_outline);
                    btnReView.Background = ContextCompat.GetDrawable(this.Activity, Resource.Drawable.silver_chalice_button_outline);
                    btnViewBill.SetTextColor(ContextCompat.GetColorStateList(this.Activity, Resource.Color.silverChalice));
                    btnReView.SetTextColor(ContextCompat.GetColorStateList(this.Activity, Resource.Color.silverChalice));
                }

                if (amountDueFailed)
                {
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

                scrollView = view.FindViewById<NMREDashboardScrollView>(Resource.Id.scroll_view);
                ViewTreeObserver observer = scrollView.ViewTreeObserver;
                observer.AddOnGlobalLayoutListener(this);

                requireScroll = false;

                this.userActionsListener?.Start();

                DownTimeEntity bcrmEntity = DownTimeEntity.GetByCode(Constants.BCRM_SYSTEM);
                DownTimeEntity pgCCEntity = DownTimeEntity.GetByCode(Constants.PG_CC_SYSTEM);
                DownTimeEntity pgFPXEntity = DownTimeEntity.GetByCode(Constants.PG_FPX_SYSTEM);

                smrResponse = InnerDashboardAPICahceUtil.OnGetSMRActivityInfoResponse();
                if (smrResponse != null)
                {
                    ShowSSMRDashboardView(smrResponse);
                }
                else
                {
                    HideSSMRDashboardView();
                }

                accountStatusResponse = InnerDashboardAPICahceUtil.OnGetAccountStatusResponse();
                if (accountStatusResponse != null)
                {
                    ShowAccountStatus(accountStatusResponse.Data.Data);
                }
                else
                {
                    energyDisconnectionButton.Visibility = ViewStates.Gone;
                }

                smrResponse = InnerDashboardAPICahceUtil.OnGetSMRActivityInfoResponse();
                if (smrResponse != null)
                {
                    ShowSSMRDashboardView(smrResponse);
                }
                else
                {
                    HideSSMRDashboardView();
                }

                if (selectedAccount != null)
                {
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
                    }


                    if (bcrmEntity != null && bcrmEntity.IsDown)
                    {
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

                        HideSSMRDashboardView();
                        energyTipsView.Visibility = ViewStates.Gone;
                        if (bcrmEntity.IsDown)
                        {
                            if (Android.OS.Build.VERSION.SdkInt >= Android.OS.Build.VERSION_CODES.N)
                            {
                                txtNewRefreshMessage.TextFormatted = Html.FromHtml(bcrmEntity.DowntimeMessage, FromHtmlOptions.ModeLegacy);
                            }
                            else
                            {
                                txtNewRefreshMessage.TextFormatted = Html.FromHtml(bcrmEntity.DowntimeMessage);
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
                        if (!amountDueFailed)
                        {
                            if (amountDueResponse != null)
                            {
                                ShowAmountDue(amountDueResponse.Data.Data);
                            }
                        }

                    }

                    txtNewRefreshMessage.Click += delegate
                    {
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

                if (!string.IsNullOrEmpty(errorMSG))
                {
                    ShowUnableToFecthSmartMeterData(errorMSG);
                }
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
            if (hasNoInternet)
            {
                return;
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

            if (!hasNoInternet)
            {
                txtAddress.Text = selectedAccount.AddStreet;
            }


            if (ChartType == ChartType.RM)
            {

                if (!hasNoInternet)
                {
                    txtRange.Text = selectedHistoryData.ByMonth.Range;
                }
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

                if (!hasNoInternet)
                {
                    txtRange.Text = selectedHistoryData.ByMonth.Range;
                }
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

                    int[] colorSet = new int[selectedHistoryData.TariffBlocksLegend.Count];
                    for (int k = 0; k < selectedHistoryData.TariffBlocksLegend.Count; k++)
                    {
                        colorSet[k] = Color.Argb(50, selectedHistoryData.TariffBlocksLegend[k].Color.RedColor, selectedHistoryData.TariffBlocksLegend[k].Color.GreenColor, selectedHistoryData.TariffBlocksLegend[k].Color.BlueData);
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

                    int[] colorSet = new int[selectedHistoryData.TariffBlocksLegend.Count];
                    for (int k = 0; k < selectedHistoryData.TariffBlocksLegend.Count; k++)
                    {
                        colorSet[k] = Color.Argb(50, selectedHistoryData.TariffBlocksLegend[k].Color.RedColor, selectedHistoryData.TariffBlocksLegend[k].Color.GreenColor, selectedHistoryData.TariffBlocksLegend[k].Color.BlueData);
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
                Highlight rightMostBar = new Highlight(barLength - 1, 0, 0);
                mChart.HighlightValues(new Highlight[] { rightMostBar });
            }
        }
        #endregion

        public void ShowByKwh()
        {
            ChartType = ChartType.kWh;

            mChart.Visibility = ViewStates.Visible;

            mChart.Clear();
            SetUp();
        }

        public void ShowByRM()
        {
            ChartType = ChartType.RM;

            mChart.Visibility = ViewStates.Visible;

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
        }

        [OnClick(Resource.Id.btnReView)]
        internal void OnREViewBill(object sender, EventArgs e)
        {
            this.userActionsListener.OnViewBill(selectedAccount);
        }

        [OnClick(Resource.Id.btnRefresh)]
        internal void OnRefresh(object sender, EventArgs e)
        {
            if (hasNoInternet)
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
        }

        [OnClick(Resource.Id.tarifToggle)]
        internal void OnTariffToggled(object sender, EventArgs e)
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

        public void ShowNoInternet()
        {
            try
            {
                DownTimeEntity bcrmEnrity = DownTimeEntity.GetByCode(Constants.BCRM_SYSTEM);
                refreshLayout.Visibility = ViewStates.Visible;
                allGraphLayout.Visibility = ViewStates.Gone;
                if (bcrmEnrity != null && bcrmEnrity.IsDown)
                {
                    HideSSMRDashboardView();
                    energyTipsView.Visibility = ViewStates.Gone;
                    if (Android.OS.Build.VERSION.SdkInt >= Android.OS.Build.VERSION_CODES.N)
                    {
                        txtNewRefreshMessage.TextFormatted = Html.FromHtml(bcrmEnrity.DowntimeMessage, FromHtmlOptions.ModeLegacy);
                    }
                    else
                    {
                        txtNewRefreshMessage.TextFormatted = Html.FromHtml(bcrmEnrity.DowntimeMessage);
                    }
                    btnNewRefresh.Visibility = ViewStates.Gone;
                }
                else
                {
                    btnNewRefresh.Visibility = ViewStates.Visible;
                    btnNewRefresh.Text = txtBtnRefreshTitle;
                    if (Build.VERSION.SdkInt >= BuildVersionCodes.N)
                    {
                        txtNewRefreshMessage.TextFormatted = Html.FromHtml(txtRefreshMsg, FromHtmlOptions.ModeLegacy);
                    }
                    else
                    {
                        txtNewRefreshMessage.TextFormatted = Html.FromHtml(txtRefreshMsg);
                    }

                    if (amountDueFailed)
                    {
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
                btnNewRefresh.Text = string.IsNullOrEmpty(buttonTxt) ? txtBtnRefreshTitle : buttonTxt;
                if (Build.VERSION.SdkInt >= BuildVersionCodes.N)
                {
                    txtNewRefreshMessage.TextFormatted = string.IsNullOrEmpty(contentTxt) ? Html.FromHtml(GetString(Resource.String.text_new_refresh_content), FromHtmlOptions.ModeLegacy) : Html.FromHtml(contentTxt, FromHtmlOptions.ModeLegacy);
                }
                else
                {
                    txtNewRefreshMessage.TextFormatted = string.IsNullOrEmpty(contentTxt) ? Html.FromHtml(GetString(Resource.String.text_new_refresh_content)) : Html.FromHtml(contentTxt);
                }
                // mNoDataLayout.Visibility = ViewStates.Gone;
                mChart.Visibility = ViewStates.Gone;
                // mDownTimeLayout.Visibility = ViewStates.Gone;
                refreshLayout.Visibility = ViewStates.Visible;
                allGraphLayout.Visibility = ViewStates.Gone;

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

        public void ShowAmountProgress()
        {
            try
            {
                // progressBar.Visibility = ViewStates.Visible;
                // totalPayableLayout.Visibility = ViewStates.Gone;

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
                // progressBar.Visibility = ViewStates.Gone;
                // totalPayableLayout.Visibility = ViewStates.Visible;
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

                if (d != null)
                {
                    if (selectedAccount != null)
                    {
                        amountDueFailed = false;
                        DownTimeEntity pgCCEntity = DownTimeEntity.GetByCode(Constants.PG_CC_SYSTEM);
                        DownTimeEntity pgFPXEntity = DownTimeEntity.GetByCode(Constants.PG_FPX_SYSTEM);
                        if (!pgCCEntity.IsDown || !pgFPXEntity.IsDown)
                        {
                            EnablePayButton();
                        }
                        btnViewBill.Enabled = true;
                        txtTotalPayableCurrency.Visibility = ViewStates.Visible;
                        btnViewBill.SetTextColor(ContextCompat.GetColorStateList(this.Activity, Resource.Color.freshGreen));
                        btnViewBill.Background = ContextCompat.GetDrawable(this.Activity, Resource.Drawable.light_green_outline_button_background);
                        if (!hasNoInternet)
                        {
                            allGraphLayout.Visibility = ViewStates.Visible;
                            refreshLayout.Visibility = ViewStates.Gone;
                            // mNoDataLayout.Visibility = ViewStates.Gone;
                            mChart.Visibility = ViewStates.Visible;
                            // mDownTimeLayout.Visibility = ViewStates.Gone;
                        }
                        if (selectedAccount.AccountCategoryId.Equals("2"))
                        {
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
                                txtDueDate.Text = "by "+ GetString(Resource.String.dashboard_chartview_due_date_wildcard, dateFormatter.Format(d));
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

                mCancelledExceptionSnackBar = Snackbar.Make(rootView, GetString(Resource.String.dashboard_chart_cancelled_exception_error), Snackbar.LengthIndefinite)
                .SetAction(GetString(Resource.String.dashboard_chart_cancelled_exception_btn_retry), delegate
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

                mApiExcecptionSnackBar = Snackbar.Make(rootView, GetString(Resource.String.dashboard_chart_api_exception_error), Snackbar.LengthIndefinite)
                .SetAction(GetString(Resource.String.dashboard_chart_api_exception_btn_retry), delegate
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

                mUknownExceptionSnackBar = Snackbar.Make(rootView, GetString(Resource.String.dashboard_chart_unknown_exception_error), Snackbar.LengthIndefinite)
                .SetAction(GetString(Resource.String.dashboard_chart_unknown_exception_btn_retry), delegate
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

        public void ShowLearnMore(Weblink weblink)
        {
            //try {
            //if (weblink.OpenWith.Equals("APP"))
            //{
            //        Intent smartMeterINtent = GetIntentObject(typeof(SmartMeterLearnMoreActivity));
            //    //Intent smartMeterINtent = new Intent(this.Activity , typeof(SmartMeterLearnMoreActivity));
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
            //catch (System.Exception e)
            //{
            //    Utility.LoggingNonFatalError(e);
            //}
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

        public void ShowSSMRDashboardView(SMRActivityInfoResponse response)
        {
            try
            {
                if (response != null && response.Response != null && response.Response.Data != null)
                {
                    SMRPopUpUtils.OnSetSMRActivityInfoResponse(response);
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

                    if (IsActive())
                    {
                        HideAmountProgress();
                    }
                }
                else
                {
                    if (IsActive())
                    {
                        HideAmountProgress();
                    }
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
            List<EnergySavingTipsModel> energyList = new List<EnergySavingTipsModel>();
            List<EnergySavingTipsModel> localList = EnergyTipsUtils.GetAllItems();
            if (localList.Count > 0)
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
            }
            catch (System.Exception err)
            {
                Utility.LoggingNonFatalError(err);
            }
        }
    }
}
