﻿using System;
using System.Collections.Generic;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using CheeseBind;
using Facebook.Shimmer;
using Java.Lang;
using myTNB.AndroidApp.Src.AddAccount.Activity;
using myTNB.AndroidApp.Src.Base.Fragments;
using myTNB.AndroidApp.Src.Database.Model;
using myTNB.AndroidApp.Src.FAQ.Activity;
using myTNB.AndroidApp.Src.myTNBMenu.Activity;
using myTNB.AndroidApp.Src.myTNBMenu.Fragments.FeedbackMenu;
using myTNB.AndroidApp.Src.myTNBMenu.Fragments.HomeMenu.Adapter;
using myTNB.AndroidApp.Src.myTNBMenu.Fragments.HomeMenu.Listener;
using myTNB.AndroidApp.Src.Notifications.Activity;
using myTNB.AndroidApp.Src.SummaryDashBoard.Models;
using myTNB.AndroidApp.Src.SummaryDashBoard.SummaryListener;
using myTNB.AndroidApp.Src.Utils;
using static myTNB.AndroidApp.Src.myTNBMenu.Fragments.HomeMenu.Adapter.MyServiceAdapter;
using static myTNB.AndroidApp.Src.myTNBMenu.Fragments.HomeMenu.Adapter.MyServiceShimmerAdapter;
using Android.App;
using myTNB.AndroidApp.Src.SSMR.SMRApplication.MVP;
using myTNB.AndroidApp.Src.Base;
using Android.Text;
using myTNB.AndroidApp.Src.SSMRMeterHistory.MVP;
using Android.Runtime;
using Android.Views.Animations;
using myTNB.AndroidApp.Src.MultipleAccountPayment.Activity;
using myTNB.AndroidApp.Src.SelectSupplyAccount.Activity;
using myTNB.AndroidApp.Src.myTNBMenu.Models;
using Newtonsoft.Json;
using myTNB.AndroidApp.Src.ViewBill.Activity;
using Android.Preferences;
using myTNB.AndroidApp.Src.RearrangeAccount.MVP;
using myTNB.AndroidApp.Src.AppLaunch.Activity;
using myTNB.AndroidApp.Src.MyTNBService.Response;
using AndroidX.RecyclerView.Widget;
using AndroidX.Core.Widget;
using AndroidX.Core.Content;
using AndroidX.AppCompat.App;
using Google.Android.Material.Snackbar;
using AndroidX.CoordinatorLayout.Widget;
using myTNB.AndroidApp.Src.ApplicationStatus.ApplicationStatusListing.MVP;
using myTNB.Mobile.SessionCache;
using myTNB;
using myTNB.Mobile;
using myTNB.AndroidApp.Src.EnergyBudget.Activity;
using AndroidX.ConstraintLayout.Widget;
using myTNB.AndroidApp.Src.MyAccount.Activity;
using myTNB.AndroidApp.Src.ManageBillDelivery.MVP;
using System.Linq;
using myTNB.AndroidApp.Src.DeviceCache;
using myTNB.Mobile.AWS.Models;
using myTNB.AndroidApp.Src.EBPopupScreen.Activity;
using Dynatrace.Xamarin;
using myTNB.AndroidApp.Src.ServiceDistruption.Activity;
using System.Threading.Tasks;
using AndroidX.Fragment.App;
using myTNB.AndroidApp.Src.MyHome;
using myTNB.AndroidApp.Src.MyHome.Model;
using myTNB.AndroidApp.Src.MyDrawer;
using myTNB.Mobile.AWS.Models.DBR;
using Android.Graphics;
using Dynatrace.Xamarin.Binding.Android;
using myTNB.AndroidApp.Src.myTNBMenu.Async;
using myTNB.AndroidApp.Src.Common.Model;
using static Android.Icu.Text.CaseMap;
using static myTNB.Mobile.Constants.Notifications.PushNotificationDetails;
using myTNB.AndroidApp.Src.QuickActionArrange.Activity;
using myTNB.Mobile.API.Models.Home.PostServices;
using myTNB.Mobile.AWS.Models.MoreIcon;
using myTNB.Mobile.AWS.Managers.MoreIcon;
using myTNB.AndroidApp.Src.QuickActionArrange.Model;

namespace myTNB.AndroidApp.Src.myTNBMenu.Fragments.HomeMenu.MVP
{
    public class HomeMenuFragment : BaseFragmentCustom
        , HomeMenuContract.IHomeMenuView
        , ViewTreeObserver.IOnGlobalLayoutListener
        , View.IOnFocusChangeListener
    {
        GetBillRenderingResponse billRenderingResponse;

        [BindView(Resource.Id.newFAQShimmerView)]
        LinearLayout newFAQShimmerView;

        [BindView(Resource.Id.newFAQList)]
        RecyclerView newFAQListRecycleView;

        [BindView(Resource.Id.newFAQShimmerList)]
        RecyclerView newFAQShimmerList;

        [BindView(Resource.Id.newFAQView)]
        LinearLayout newFAQView;

        [BindView(Resource.Id.newFAQTitle)]
        TextView newFAQTitle;

        [BindView(Resource.Id.newFAQContainer)]
        LinearLayout newFAQContainer;

        [BindView(Resource.Id.myServiceShimmerView)]
        LinearLayout myServiceShimmerView;

        [BindView(Resource.Id.discoverMoreEBImgLayout)]
        LinearLayout discoverMoreEBImgLayout;

        [BindView(Resource.Id.discoverMoreSDImgLayout)]
        LinearLayout discoverMoreSDImgLayout;

        [BindView(Resource.Id.discoverMoreContainer)]
        LinearLayout discoverMoreContainer;

        [BindView(Resource.Id.discoverMoreSDContainer)]
        LinearLayout discoverMoreSDContainer;

        [BindView(Resource.Id.myServiceList)]
        RecyclerView myServiceListRecycleView;

        [BindView(Resource.Id.myServiceShimmerList)]
        RecyclerView myServiceShimmerList;

        [BindView(Resource.Id.myServiceView)]
        LinearLayout myServiceView;

        [BindView(Resource.Id.accountsHeaderTitle)]
        TextView accountHeaderTitle;

        [BindView(Resource.Id.accountGreeting)]
        TextView accountGreeting;

        [BindView(Resource.Id.accountGreetingName)]
        TextView accountGreetingName;

        [BindView(Resource.Id.searchAction)]
        ImageView searchActionIcon;

        [BindView(Resource.Id.searchEdit)]
        Android.Widget.SearchView searchEditText;

        [BindView(Resource.Id.accountRecyclerViewContainer)]
        RecyclerView accountsRecyclerView;

        [BindView(Resource.Id.summaryNestScrollView)]
        NestedScrollView summaryNestScrollView;

        [BindView(Resource.Id.rootView)]
        CoordinatorLayout rootView;

        [BindView(Resource.Id.shimmerFAQView)]
        ShimmerFrameLayout shimmerFAQView;

        [BindView(Resource.Id.notificationHeaderIcon)]
        ImageView notificationHeaderIcon;

        [BindView(Resource.Id.addAction)]
        ImageView addActionImage;

        [BindView(Resource.Id.accountListViewContainer)]
        LinearLayout accountListViewContainer;

        [BindView(Resource.Id.topRootView)]
        LinearLayout topRootView;

        [BindView(Resource.Id.accountListRefreshContainer)]
        LinearLayout accountListRefreshContainer;

        [BindView(Resource.Id.refreshMsg)]
        TextView txtRefreshMsg;

        [BindView(Resource.Id.btnRefresh)]
        Button btnRefresh;

        [BindView(Resource.Id.accountListContainer)]
        LinearLayout accountListContainer;

        [BindView(Resource.Id.txtAdd)]
        TextView txtAdd;

        [BindView(Resource.Id.accountCard)]
        LinearLayout accountCard;

        [BindView(Resource.Id.addActionLabel)]
        TextView addActionLabel;

        [BindView(Resource.Id.searchActionLabel)]
        TextView searchActionLabel;

        [BindView(Resource.Id.addActionContainer)]
        LinearLayout addActionContainer;

        [BindView(Resource.Id.searchActionContainer)]
        LinearLayout searchActionContainer;

        [BindView(Resource.Id.accountsActionsContainer)]
        LinearLayout accountsActionsContainer;

        [BindView(Resource.Id.refreshImg)]
        ImageView refreshImg;

        [BindView(Resource.Id.refreshMsg)]
        TextView refreshMsg;

        [BindView(Resource.Id.accountActionDivider)]
        View accountActionDivider;

        [BindView(Resource.Id.loadMoreContainer)]
        LinearLayout loadMoreContainer;

        [BindView(Resource.Id.loadMoreLabel)]
        TextView loadMoreLabel;

        [BindView(Resource.Id.loadMoreImg)]
        ImageView loadMoreImg;

        [BindView(Resource.Id.rearrangeContainer)]
        LinearLayout rearrangeContainer;

        [BindView(Resource.Id.rearrangeLabel)]
        TextView rearrangeLabel;

        [BindView(Resource.Id.rearrangeImg)]
        ImageView rearrangeImg;

        [BindView(Resource.Id.rearrangeLine)]
        View rearrangeLine;

        [BindView(Resource.Id.bottomContainer)]
        LinearLayout bottomContainer;

        ImageView closeImageView;

        [BindView(Resource.Id.myServiceLoadMoreContainer)]
        LinearLayout myServiceLoadMoreContainer;

        [BindView(Resource.Id.myServiceLoadMoreLabel)]
        TextView myServiceLoadMoreLabel;

        [BindView(Resource.Id.myServiceLoadMoreImg)]
        ImageView myServiceLoadMoreImg;

        [BindView(Resource.Id.newLabel)]
        LinearLayout newLabel;

        [BindView(Resource.Id.txtNewLabel)]
        TextView txtNewLabel;

        [BindView(Resource.Id.indicatorContainer)]
        LinearLayout indicatorContainer;

        [BindView(Resource.Id.myServiceRefreshContainer)]
        LinearLayout myServiceRefreshContainer;

        [BindView(Resource.Id.myServiceRefreshImage)]
        ImageView myServiceRefreshImage;

        [BindView(Resource.Id.txtMyServiceRefreshMessage)]
        TextView txtMyServiceRefreshMessage;

        [BindView(Resource.Id.btnMyServiceRefresh)]
        Button btnMyServiceRefresh;

        [BindView(Resource.Id.myServiceContainer)]
        LinearLayout myServiceContainer;

        [BindView(Resource.Id.myServiceHideView)]
        LinearLayout myServiceHideView;

        [BindView(Resource.Id.accountContainer)]
        ConstraintLayout accountContainer;

        [BindView(Resource.Id.discoverView)]
        LinearLayout discoverView;

        [BindView(Resource.Id.img_discover_digital_bill)]
        ImageView img_discover_digital_bill;

        [BindView(Resource.Id.img_discover_energy_budget)]
        ImageView img_discover_energy_budget;

        [BindView(Resource.Id.img_discover_service_disruption)]
        ImageView img_discover_service_disruption;

        [BindView(Resource.Id.discoverMoreSectionTitle)]
        TextView discoverMoreSectionTitle;

        [BindView(Resource.Id.whatsNewUnreadImg)]
        ImageView whatsNewUnreadImg;

        [BindView(Resource.Id.discoverMoreNBRContainer)]
        LinearLayout discoverMoreNBRContainer;

        [BindView(Resource.Id.newBillRedesignBanner)]
        ImageView newBillRedesignBanner;

        [BindView(Resource.Id.discoverMoreMyHomeContainer)]
        LinearLayout discoverMoreMyHomeContainer;

        [BindView(Resource.Id.myHomeBanner)]
        ImageView myHomeBanner;

        [BindView(Resource.Id.discovercontainer)]
        LinearLayout discovercontainer;

        [BindView(Resource.Id.ContainerQuickAction)]
        LinearLayout containerQuickAction;

        [BindView(Resource.Id.quickActionTitle)]
        TextView quickActionTitle;

        [BindView(Resource.Id.quickActionIcon)]
        TextView quickActionIcon;

        bool IsAccountDBREligible;
        AccountsRecyclerViewAdapter accountsAdapter;

        private NewFAQScrollListener mListener;

        private string mSavedTimeStamp = "0000000";

        private string savedEnergySavingTipsTimeStamp = "0000000";

        public readonly static int SSMR_METER_HISTORY_ACTIVITY_CODE = 8796;

        public readonly static int REARRANGE_ACTIVITY_CODE = 8806;

        public readonly static int REARRANGE_QUICK_ACTION_ACTIVITY_CODE = 8811;

        internal static readonly int SELECT_SM_ACCOUNT_REQUEST_CODE = 8809;

        internal static readonly int SELECT_SM_POPUP_REQUEST_CODE = 8810;

        internal static readonly int SELECT_SD_POPUP_REQUEST_CODE = 8820;

        private static List<MyServiceModel> myServicesList = new List<MyServiceModel>();

        private static List<NewFAQ> currentNewFAQList = new List<NewFAQ>();

        private bool isBCRMDown = false;

        private static bool isFirstInitiate = true;

        private bool isSearchClose = false;

        private bool isAlreadyRotated = false;

        private bool isMyServiceAlreadyRotated = false;

        private bool isSMRReady = false;

        private bool isPayBillDisabled = false;

        private bool isViewBillDisabled = false;

        private bool isRefreshShown = false;

        private bool isInitiate = false;

        private bool isClickFromQuickActionSMR = false;

        private bool isFromPageRearrangeIcon = false;

        HomeMenuContract.IHomeMenuPresenter presenter;
        ISummaryFragmentToDashBoardActivtyListener mCallBack;

        MyServiceShimmerAdapter myServiceShimmerAdapter;

        MyServiceAdapter myServiceAdapter;

        NewFAQShimmerAdapter newFAQShimmerAdapter;

        NewFAQAdapter newFAQAdapter;

        CustomerBillingAccount selectedAccount;
        //PostBREligibilityIndicatorsResponse billRenderingTenantResponse;

        const string PAGE_ID = "DashboardHome";

        const string FAQ_TAGS_SEPARATOR = "|";
        const string GSL_TAG = "GSL";

        SearchApplicationTypeResponse _searchApplicationTypeResponse;

        List<MyServiceModel> listCurrentQuickAction = new List<MyServiceModel>();

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            DynatraceHelper.OnTrack(DynatraceConstants.MyHome.Screens.Home.Dashboard);
            presenter = new HomeMenuPresenter(this, PreferenceManager.GetDefaultSharedPreferences(this.Activity), this.Context);
            this.presenter.GetDownTime();
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return base.OnCreateView(inflater, container, savedInstanceState);
        }


        public override int ResourceId()
        {
            return Resource.Layout.HomeMenuFragmentView;
        }

        public override void OnAttach(Context context)
        {
            base.OnAttach(context);
            try
            {
                FirebaseAnalyticsUtils.SetFragmentScreenName(this, "Home");
                mCallBack = context as ISummaryFragmentToDashBoardActivtyListener;
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

        private void UpdateGreetingsHeader(Constants.GREETING greeting)
        {
            switch (greeting)
            {
                case Constants.GREETING.MORNING:
                    accountGreeting.Text = GetLabelByLanguage("greeting_morning");
                    break;
                case Constants.GREETING.AFTERNOON:
                    accountGreeting.Text = GetLabelByLanguage("greeting_afternoon");
                    break;
                default:
                    accountGreeting.Text = GetLabelByLanguage("greeting_evening");
                    break;
            }
        }

        public override void OnActivityResult(int requestCode, int resultCode, Intent data)
        {
            if (requestCode == SSMR_METER_HISTORY_ACTIVITY_CODE)
            {
                if (resultCode == (int)Result.Ok)
                {
                    try
                    {
                        ((DashboardHomeActivity)Activity).OnTapRefresh();
                    }
                    catch (System.Exception e)
                    {
                        Utility.LoggingNonFatalError(e);
                    }
                }
            }
            else if (requestCode == Constants.SELECT_ACCOUNT_PDF_REQUEST_CODE)
            {
                if (resultCode == (int)Result.Ok)
                {
                    Bundle extras = data.Extras;

                    CustomerBillingAccount selectedAccount = JsonConvert.DeserializeObject<CustomerBillingAccount>(extras.GetString(Constants.SELECTED_ACCOUNT));

                    AccountData selectedAccountData = AccountData.Copy(selectedAccount, true);

                    Intent viewBill = new Intent(this.Activity, typeof(ViewBillActivity));
                    viewBill.PutExtra(Constants.SELECTED_ACCOUNT, JsonConvert.SerializeObject(selectedAccountData));
                    viewBill.PutExtra(Constants.CODE_KEY, Constants.SELECT_ACCOUNT_PDF_REQUEST_CODE);
                    StartActivity(viewBill);
                }
            }
            else if (requestCode == REARRANGE_ACTIVITY_CODE)
            {
                if (resultCode == (int)Result.Ok)
                {
                    HomeMenuUtils.SetIsShowRearrangeAccountSuccessfulNeed(true);
                    RestartHomeMenu();
                }
            }
            else if (requestCode == SELECT_SM_ACCOUNT_REQUEST_CODE)
            {
                if (resultCode == (int)Result.Ok)
                {
                    MyTNBAccountManagement.GetInstance().OnHoldWhatNew(true);
                    Bundle extras = data.Extras;

                    SMRAccount selectedAccount = JsonConvert.DeserializeObject<SMRAccount>(extras.GetString(Constants.SELECTED_ACCOUNT));
                    this.SetIsClicked(false);
                    ShowAccountDetails(selectedAccount.accountNumber);
                }
            }
            else if (requestCode == SELECT_SM_POPUP_REQUEST_CODE)
            {
                if (resultCode == (int)Result.Ok)
                {
                    this.SetIsClicked(false);
                    Bundle extras = data.Extras;
                    if (extras.ContainsKey("EBList"))
                    {
                        if (UserSessions.GetEnergyBudgetList().Count == 1)
                        {
                            MyTNBAccountManagement.GetInstance().OnHoldWhatNew(true);
                            this.SetIsClicked(false);
                            List<SMRAccount> smaccEB = new List<SMRAccount>();
                            smaccEB = UserSessions.GetEnergyBudgetList();
                            ShowAccountDetails(smaccEB[0].accountNumber);
                        }
                        else if (UserSessions.GetEnergyBudgetList().Count > 1)
                        {
                            Intent energy_budget_activity = new Intent(this.Activity, typeof(EnergyBudgetActivity));
                            StartActivityForResult(energy_budget_activity, SELECT_SM_ACCOUNT_REQUEST_CODE);
                        }
                    }
                    else
                    {
                        MyTNBAccountManagement.GetInstance().OnHoldWhatNew(true);
                    }
                }
            }
            else if (requestCode == REARRANGE_QUICK_ACTION_ACTIVITY_CODE)
            {
                if (resultCode == (int)Result.Ok)
                {
                    Bundle extras = data.Extras;
                    this.SetIsClicked(false);
                    if (extras != null)
                    {
                        if (extras.ContainsKey("IconList"))
                        {
                            isFromPageRearrangeIcon = true;
                            this.presenter.ListAfterRearrangeIcon(isFromPageRearrangeIcon);
                        }
                    }
                }
            }
        }

        public void SetNotificationIndicator()
        {
            try
            {
                OnSetNotificationNewLabel(UserNotificationEntity.HasNotifications(), UserNotificationEntity.Count());
                //this.presenter.UserNotificationsCount();
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            try
            {
                //IsAccountDBREligible = DBRUtility.Instance.IsAccountEligible && CustomerBillingAccount.HasOwnerCA();
                IsAccountDBREligible = DBRUtility.Instance.IsAccountEligible;
                MyTNBAccountManagement.GetInstance().SetIsEBUser(EBUtility.Instance.IsAccountEligible);
                MyTNBAccountManagement.GetInstance().SetIsSDUser(SDUtility.Instance.IsAccountEligible);
                summaryNestScrollView.SmoothScrollingEnabled = true;
                isSearchClose = true;
                isFirstInitiate = true;
                accountGreetingName.Text = this.presenter.GetAccountDisplay() + "!";
                SetNotificationIndicator();
                SetAccountsRecyclerView();
                SetAccountActionHeader();
                SetupMyServiceView();
                //GetIndicatorTenantDBR();
                //SetDBRDiscoverView();
                SetupDiscoverView();
                SetupNewFAQView();

                TextViewUtils.SetMuseoSans300Typeface(txtRefreshMsg, txtMyServiceRefreshMessage);
                TextViewUtils.SetMuseoSans500Typeface(newFAQTitle, btnRefresh, txtAdd
                    , addActionLabel, searchActionLabel, loadMoreLabel, rearrangeLabel
                    , myServiceLoadMoreLabel, txtNewLabel, btnMyServiceRefresh, quickActionTitle);
                TextViewUtils.SetTextSize8(txtNewLabel);
                TextViewUtils.SetTextSize12(addActionLabel, searchActionLabel, rearrangeLabel
                    , loadMoreLabel, myServiceLoadMoreLabel, txtMyServiceRefreshMessage, quickActionTitle);
                TextViewUtils.SetTextSize14(refreshMsg, txtAdd, newFAQTitle, accountHeaderTitle);
                TextViewUtils.SetTextSize16(accountGreeting, accountGreetingName, btnMyServiceRefresh, btnRefresh);
                SearchView searchView = new SearchView(this.Context);
                LinearLayout linearLayout1 = (LinearLayout)searchView.GetChildAt(0);
                LinearLayout linearLayout2 = (LinearLayout)linearLayout1.GetChildAt(2);
                LinearLayout linearLayout3 = (LinearLayout)linearLayout2.GetChildAt(1);
                AutoCompleteTextView autoComplete = (AutoCompleteTextView)linearLayout3.GetChildAt(0);
                TextViewUtils.SetTextSize(12, autoComplete, true);

                addActionLabel.Text = GetLabelByLanguage("add");
                searchActionLabel.Text = GetLabelByLanguage("search");
                txtAdd.Text = GetLabelByLanguage("addElectricityAcct");
                newFAQTitle.Text = GetLabelByLanguage("needHelp");
                rearrangeLabel.Text = GetLabelByLanguage("rearrangeAccts");
                loadMoreLabel.Text = GetLabelByLanguage("moreAccts");
                myServiceLoadMoreLabel.Text = GetLabelByLanguage("showMore");
                quickActionTitle.Text = GetLabelByLanguage("quickActionTitle");

                addActionContainer.SetOnClickListener(null);
                notificationHeaderIcon.SetOnClickListener(null);

                addActionContainer.Click += delegate
                {
                    if (!this.GetIsClicked())
                    {
                        this.SetIsClicked(true);
                        try
                        {
                            FirebaseAnalyticsUtils.LogFragmentClickEvent(this, "Home Screen -> Add Account");
                        }
                        catch (System.Exception err)
                        {
                            Utility.LoggingNonFatalError(err);
                        }
                        Intent linkAccount = new Intent(this.Activity, typeof(LinkAccountActivity));
                        //Intent linkAccount = new Intent(this.Activity, typeof(MyAccountActivity));
                        linkAccount.PutExtra("fromDashboard", true);
                        StartActivity(linkAccount);
                    }
                };
                notificationHeaderIcon.Click += delegate
                {
                    if (!this.GetIsClicked())
                    {
                        this.SetIsClicked(true);
                        try
                        {
                            FirebaseAnalyticsUtils.LogFragmentClickEvent(this, "Home Screen -> Notification");
                        }
                        catch (System.Exception err)
                        {
                            Utility.LoggingNonFatalError(err);
                        }
                        this.Activity.StartActivityForResult(new Intent(this.Activity, typeof(NotificationActivity)), Constants.NOTIFICATION_LISTING_REQUEST_CODE);
                    }
                };

                try
                {
                    // Load the drawable
                    Android.Graphics.Drawables.Drawable d = ContextCompat.GetDrawable(this.Context, Resource.Drawable.ic_pen_blue);

                    // Set the bounds to define the size (adjust width and height as needed)
                    int iconSize = 48; // Specify your desired size in pixels
                    d.SetBounds(0, 0, iconSize, iconSize);

                    // Set the compound drawables with specified bounds and optional padding
                    quickActionIcon.SetCompoundDrawables(d, null, null, null);
                    //quickActionIcon.CompoundDrawablePadding = 4;
                }
                catch (System.Exception err)
                {
                    Utility.LoggingNonFatalError(err);
                }

                quickActionIcon.Click += delegate
                {
                    if (!this.GetIsClicked())
                    {
                        try
                        {
                            FirebaseAnalyticsUtils.LogFragmentClickEvent(this, "Home Screen -> Arrange Icon");
                        }
                        catch (System.Exception err)
                        {
                            Utility.LoggingNonFatalError(err);
                        }
                        this.SetIsClicked(true);
                        listCurrentQuickAction = this.presenter.GetCurrentQuickActionList();
                        UserSessions.SaveUserEmailQuickAction(PreferenceManager.GetDefaultSharedPreferences(this.Activity), UserEntity.GetActive().Email);
                        Intent rearrangeQuickAction = new Intent(this.Activity, typeof(QuickActionArrangeActivity));
                        rearrangeQuickAction.PutExtra("RearrangeQuickAction", JsonConvert.SerializeObject(listCurrentQuickAction));
                        StartActivityForResult(rearrangeQuickAction, REARRANGE_QUICK_ACTION_ACTIVITY_CODE);
                    }
                };

                if (closeImageView != null)
                {
                    closeImageView.Clickable = true;

                    closeImageView.Click += delegate
                    {
                        OnSearchOutFocus(true);
                    };
                }

                bool isGetEnergyTipsDisabled = false;
                if (MyTNBAccountManagement.GetInstance().IsEnergyTipsDisabled())
                {
                    isGetEnergyTipsDisabled = true;
                }

                if (!isGetEnergyTipsDisabled)
                {
                    this.presenter.GetEnergySavingTipsTimeStamp();
                }

                SetRefreshLayoutParams();

                try
                {
                    newFAQListRecycleView.Focusable = false;
                    newFAQShimmerList.Focusable = false;
                    myServiceListRecycleView.Focusable = false;
                    myServiceShimmerList.Focusable = false;
                    accountsRecyclerView.Focusable = false;
                    topRootView.Focusable = true;
                    topRootView.RequestFocus();
                    ViewTreeObserver observer = summaryNestScrollView.ViewTreeObserver;
                    observer.AddOnGlobalLayoutListener(this);
                }
                catch (System.Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                }

                try
                {
                    ((DashboardHomeActivity)Activity).SetStatusBarBackground(Resource.Drawable.AppLanchGradientBackground);
                }
                catch (System.Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                }

                ShowSearchAction(false);
                SMRPopUpUtils.SetFromUsageFlag(false);
                SMRPopUpUtils.SetFromUsageSubmitSuccessfulFlag(false);
                this.presenter.SetDynaUserTAG();  //call dyna set username
                OnStartLoadAccount();
                SetUpNBRView();
                ShowDiscoverMoreLayout();
                ShowDiscoverMoreSDLayout();
            }
            catch (System.Exception e)
            {
                Intent LaunchViewIntent = new Intent(this.Activity, typeof(LaunchViewActivity));
                LaunchViewActivity.MAKE_INITIAL_CALL = true;
                LaunchViewIntent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.ClearTask | ActivityFlags.NewTask);
                StartActivity(LaunchViewIntent);
                Utility.LoggingNonFatalError(e);
            }
        }

        [OnClick(Resource.Id.img_discover_digital_bill)]
        void OnManageBillDelivery(object sender, EventArgs eventArgs)
        {
            //SetDiscoverResult(IsAccountDBREligible);
            if (IsAccountDBREligible)
            {
                DynatraceHelper.OnTrack(DynatraceConstants.DBR.CTAs.Home.Home_Banner);
                GetBillRendering();

            }
            else
            {
                this.Activity.RunOnUiThread(() =>
                {
                    MyTNBAppToolTipBuilder.Create(this.Activity, MyTNBAppToolTipBuilder.ToolTipType.IMAGE_HEADER)
                        .SetTitle(Utility.GetLocalizedLabel("ManageDigitalBillLanding", "notEligibleFeatureToogleTitle"))
                        .SetMessage(Utility.GetLocalizedLabel("ManageDigitalBillLanding", "notEligibleFeatureToogleText"))
                        .SetCTALabel(Utility.GetLocalizedCommonLabel("gotIt"))
                        .SetCTAaction(() => { this.SetIsClicked(false); })
                        .Build()
                        .Show();
                });

            }
        }


        private void GetBillRendering()
        {
            ShowProgressDialog();
            Task.Run(() =>
            {
                _ = GetBillRenderingAsync();
            });
        }

        private async Task GetBillRenderingAsync()
        {
            try
            {
                string caNumber = string.Empty;
                bool isOwner = false;
                if (DBRUtility.Instance.IsAccountEligible)
                {
                    List<string> caList = DBRUtility.Instance.GetCAList();
                    caNumber = caList != null && caList.Count > 0
                        ? caList[0]
                        : string.Empty;
                }
                else
                {
                    CustomerBillingAccount dbrAccount = GetEligibleDBRAccount();
                    if (dbrAccount == null)
                    {
                        this.Activity.RunOnUiThread(() =>
                        {
                            HideProgressDialog();
                            MyTNBAppToolTipBuilder errorPopup = MyTNBAppToolTipBuilder.Create(this.Activity, MyTNBAppToolTipBuilder.ToolTipType.NORMAL_WITH_HEADER)
                                .SetTitle(Utility.GetLocalizedLabel(LanguageConstants.ERROR, LanguageConstants.Error.DEFAULT_ERROR_TITLE))
                                .SetMessage(Utility.GetLocalizedLabel(LanguageConstants.ERROR, LanguageConstants.Error.DEFAULT_ERROR_MSG))
                                .SetCTALabel(Utility.GetLocalizedLabel(LanguageConstants.COMMON, LanguageConstants.Common.GOT_IT))
                                .Build();
                            errorPopup.Show();
                        });

                        return;
                    }
                    caNumber = dbrAccount.AccNum;
                    isOwner = dbrAccount.isOwned;
                }

                if (!AccessTokenCache.Instance.HasTokenSaved(this.Activity))
                {
                    string accessToken = await AccessTokenManager.Instance.GenerateAccessToken(UserEntity.GetActive().UserID ?? string.Empty);
                    AccessTokenCache.Instance.SaveAccessToken(this.Activity, accessToken);
                }
                billRenderingResponse = await DBRManager.Instance.GetBillRendering(caNumber
                    , AccessTokenCache.Instance.GetAccessToken(this.Activity), isOwner);

                HideProgressDialog();
                //Nullity Check
                if (billRenderingResponse != null
                   && billRenderingResponse.StatusDetail != null
                   && billRenderingResponse.StatusDetail.IsSuccess
                   && billRenderingResponse.Content != null)
                {
                    Intent intent = new Intent(Activity, typeof(ManageBillDeliveryActivity));
                    intent.PutExtra("billRenderingResponse", JsonConvert.SerializeObject(billRenderingResponse));
                    //intent.PutExtra("billRenderingTenantResponse", JsonConvert.SerializeObject(billRenderingTenantResponse));
                    intent.PutExtra("accountNumber", caNumber);
                    StartActivity(intent);

                }
                else
                {
                    string title = billRenderingResponse != null && billRenderingResponse.StatusDetail != null && billRenderingResponse.StatusDetail.Title.IsValid()
                        ? billRenderingResponse?.StatusDetail?.Title
                        : Utility.GetLocalizedLabel(LanguageConstants.ERROR, LanguageConstants.Error.DEFAULT_ERROR_TITLE);

                    string message = billRenderingResponse != null && billRenderingResponse.StatusDetail != null && billRenderingResponse.StatusDetail.Message.IsValid()
                       ? billRenderingResponse?.StatusDetail?.Message
                       : Utility.GetLocalizedLabel(LanguageConstants.ERROR, LanguageConstants.Error.DEFAULT_ERROR_MSG);

                    string cta = billRenderingResponse != null && billRenderingResponse.StatusDetail != null && billRenderingResponse.StatusDetail.PrimaryCTATitle.IsValid()
                       ? billRenderingResponse?.StatusDetail?.PrimaryCTATitle
                       : Utility.GetLocalizedLabel(LanguageConstants.COMMON, LanguageConstants.Common.OK);

                    this.Activity.RunOnUiThread(() =>
                    {
                        MyTNBAppToolTipBuilder errorPopup = MyTNBAppToolTipBuilder.Create(this.Activity, MyTNBAppToolTipBuilder.ToolTipType.NORMAL_WITH_HEADER)
                            .SetTitle(title ?? Utility.GetLocalizedLabel(LanguageConstants.ERROR, LanguageConstants.Error.DEFAULT_ERROR_TITLE))
                            .SetMessage(message ?? Utility.GetLocalizedLabel(LanguageConstants.ERROR, LanguageConstants.Error.DEFAULT_ERROR_MSG))
                            .SetCTALabel(cta ?? Utility.GetLocalizedLabel(LanguageConstants.COMMON, LanguageConstants.Common.OK))
                            .Build();
                        errorPopup.Show();
                    });
                }
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }
        public CustomerBillingAccount GetEligibleDBRAccount()
        {
            CustomerBillingAccount customerAccount = CustomerBillingAccount.GetSelected();
            List<string> dBRCAs = DBRUtility.Instance.GetCAList();
            List<CustomerBillingAccount> allAccountList = CustomerBillingAccount.List();
            CustomerBillingAccount account = new CustomerBillingAccount();
            if (dBRCAs.Count > 0)
            {
                var dbrSelected = dBRCAs.Where(x => x == customerAccount.AccNum).FirstOrDefault();
                if (dbrSelected != string.Empty)
                {
                    account = allAccountList.Where(x => x.AccNum == dbrSelected).FirstOrDefault();
                }
                if (account == null)
                {
                    foreach (var dbrca in dBRCAs)
                    {
                        account = allAccountList.Where(x => x.AccNum == dbrca).FirstOrDefault();
                        if (account != null)
                        {
                            break;
                        }
                    }
                }
            }
            return account;
        }
        public void SetRefreshLayoutParams()
        {
            LinearLayout.LayoutParams refreshImgParams = refreshImg.LayoutParameters as LinearLayout.LayoutParams;
            LinearLayout.LayoutParams refreshMsgParams = refreshMsg.LayoutParameters as LinearLayout.LayoutParams;
            LinearLayout.LayoutParams btnRefreshParams = btnRefresh.LayoutParameters as LinearLayout.LayoutParams;

            refreshImgParams.Width = GetDeviceHorizontalScaleInPixel(0.266f);
            refreshImgParams.Height = GetDeviceHorizontalScaleInPixel(0.266f);
            refreshImgParams.TopMargin = (int)DPUtils.ConvertDPToPx(28f);
            refreshImg.RequestLayout();

            refreshMsgParams.Width = GetDeviceHorizontalScaleInPixel(0.80f);
            refreshMsgParams.RightMargin = GetDeviceHorizontalScaleInPixel(0.10f);
            refreshMsgParams.LeftMargin = GetDeviceHorizontalScaleInPixel(0.10f);
            refreshMsg.RequestLayout();

            btnRefreshParams.Width = GetDeviceHorizontalScaleInPixel(0.90f);
            btnRefreshParams.RightMargin = GetDeviceHorizontalScaleInPixel(0.05f);
            btnRefreshParams.LeftMargin = GetDeviceHorizontalScaleInPixel(0.05f);
            btnRefresh.RequestLayout();

            SetBottmLayoutParams(25f);
        }

        public void SetRefreshLayoutParamsWithMyServiceHide()
        {
            LinearLayout.LayoutParams refreshImgParams = refreshImg.LayoutParameters as LinearLayout.LayoutParams;
            LinearLayout.LayoutParams refreshMsgParams = refreshMsg.LayoutParameters as LinearLayout.LayoutParams;
            LinearLayout.LayoutParams btnRefreshParams = btnRefresh.LayoutParameters as LinearLayout.LayoutParams;

            refreshImgParams.Width = GetDeviceHorizontalScaleInPixel(0.266f);
            refreshImgParams.Height = GetDeviceHorizontalScaleInPixel(0.266f);
            refreshImgParams.TopMargin = (int)DPUtils.ConvertDPToPx(36f);
            refreshImg.RequestLayout();

            refreshMsgParams.Width = GetDeviceHorizontalScaleInPixel(0.80f);
            refreshMsgParams.RightMargin = GetDeviceHorizontalScaleInPixel(0.10f);
            refreshMsgParams.LeftMargin = GetDeviceHorizontalScaleInPixel(0.10f);
            refreshMsg.RequestLayout();

            btnRefreshParams.Width = GetDeviceHorizontalScaleInPixel(0.90f);
            btnRefreshParams.RightMargin = GetDeviceHorizontalScaleInPixel(0.05f);
            btnRefreshParams.LeftMargin = GetDeviceHorizontalScaleInPixel(0.05f);
            btnRefresh.RequestLayout();

            SetBottmLayoutParams(0);
        }

        public void SetRefreshLayoutParamsWithAllDown()
        {
            LinearLayout.LayoutParams refreshImgParams = refreshImg.LayoutParameters as LinearLayout.LayoutParams;
            LinearLayout.LayoutParams refreshMsgParams = refreshMsg.LayoutParameters as LinearLayout.LayoutParams;
            LinearLayout.LayoutParams btnRefreshParams = btnRefresh.LayoutParameters as LinearLayout.LayoutParams;

            refreshImgParams.Width = GetDeviceHorizontalScaleInPixel(0.266f);
            refreshImgParams.Height = GetDeviceHorizontalScaleInPixel(0.266f);
            refreshImgParams.TopMargin = (int)DPUtils.ConvertDPToPx(60f);
            refreshImg.RequestLayout();

            refreshMsgParams.Width = GetDeviceHorizontalScaleInPixel(0.80f);
            refreshMsgParams.RightMargin = GetDeviceHorizontalScaleInPixel(0.10f);
            refreshMsgParams.LeftMargin = GetDeviceHorizontalScaleInPixel(0.10f);
            refreshMsg.RequestLayout();

            btnRefreshParams.Width = GetDeviceHorizontalScaleInPixel(0.90f);
            btnRefreshParams.RightMargin = GetDeviceHorizontalScaleInPixel(0.05f);
            btnRefreshParams.LeftMargin = GetDeviceHorizontalScaleInPixel(0.05f);
            btnRefresh.RequestLayout();

            SetBottmLayoutParams(0);
        }

        public void SetMaintenanceLayoutParams()
        {
            LinearLayout.LayoutParams refreshImgParams = refreshImg.LayoutParameters as LinearLayout.LayoutParams;
            LinearLayout.LayoutParams refreshMsgParams = refreshMsg.LayoutParameters as LinearLayout.LayoutParams;
            LinearLayout.LayoutParams btnRefreshParams = btnRefresh.LayoutParameters as LinearLayout.LayoutParams;

            refreshImgParams.Width = GetDeviceHorizontalScaleInPixel(0.25f);
            refreshImgParams.Height = GetDeviceHorizontalScaleInPixel(0.25f);
            refreshImgParams.TopMargin = (int)DPUtils.ConvertDPToPx(60f);
            refreshImg.RequestLayout();
            refreshImg.SetImageResource(Resource.Drawable.maintenance_white);

            refreshMsgParams.Width = GetDeviceHorizontalScaleInPixel(0.80f);
            refreshMsgParams.RightMargin = GetDeviceHorizontalScaleInPixel(0.10f);
            refreshMsgParams.LeftMargin = GetDeviceHorizontalScaleInPixel(0.10f);
            refreshMsgParams.TopMargin = (int)DPUtils.ConvertDPToPx(16f);
            refreshMsg.RequestLayout();

            btnRefreshParams.Width = GetDeviceHorizontalScaleInPixel(0.90f);
            btnRefreshParams.RightMargin = GetDeviceHorizontalScaleInPixel(0.05f);
            btnRefreshParams.LeftMargin = GetDeviceHorizontalScaleInPixel(0.05f);
            btnRefresh.RequestLayout();

            SetBottmLayoutParams(57f);
        }

        public void SetMaintenanceLayoutParamsWithMyServiceHide()
        {
            LinearLayout.LayoutParams refreshImgParams = refreshImg.LayoutParameters as LinearLayout.LayoutParams;
            LinearLayout.LayoutParams refreshMsgParams = refreshMsg.LayoutParameters as LinearLayout.LayoutParams;
            LinearLayout.LayoutParams btnRefreshParams = btnRefresh.LayoutParameters as LinearLayout.LayoutParams;

            refreshImgParams.Width = GetDeviceHorizontalScaleInPixel(0.25f);
            refreshImgParams.Height = GetDeviceHorizontalScaleInPixel(0.25f);
            refreshImgParams.TopMargin = (int)DPUtils.ConvertDPToPx(68f);
            refreshImg.RequestLayout();
            refreshImg.SetImageResource(Resource.Drawable.maintenance_white);

            refreshMsgParams.Width = GetDeviceHorizontalScaleInPixel(0.80f);
            refreshMsgParams.RightMargin = GetDeviceHorizontalScaleInPixel(0.10f);
            refreshMsgParams.LeftMargin = GetDeviceHorizontalScaleInPixel(0.10f);
            refreshMsgParams.TopMargin = (int)DPUtils.ConvertDPToPx(16f);
            refreshMsg.RequestLayout();

            btnRefreshParams.Width = GetDeviceHorizontalScaleInPixel(0.90f);
            btnRefreshParams.RightMargin = GetDeviceHorizontalScaleInPixel(0.05f);
            btnRefreshParams.LeftMargin = GetDeviceHorizontalScaleInPixel(0.05f);
            btnRefresh.RequestLayout();

            SetBottmLayoutParams(0);
        }

        public void SetMaintenanceLayoutParamsWithAllDown()
        {
            LinearLayout.LayoutParams refreshImgParams = refreshImg.LayoutParameters as LinearLayout.LayoutParams;
            LinearLayout.LayoutParams refreshMsgParams = refreshMsg.LayoutParameters as LinearLayout.LayoutParams;
            LinearLayout.LayoutParams btnRefreshParams = btnRefresh.LayoutParameters as LinearLayout.LayoutParams;

            refreshImgParams.Width = GetDeviceHorizontalScaleInPixel(0.25f);
            refreshImgParams.Height = GetDeviceHorizontalScaleInPixel(0.25f);
            refreshImgParams.TopMargin = (int)DPUtils.ConvertDPToPx(76f);
            refreshImg.RequestLayout();
            refreshImg.SetImageResource(Resource.Drawable.maintenance_white);

            refreshMsgParams.Width = GetDeviceHorizontalScaleInPixel(0.80f);
            refreshMsgParams.RightMargin = GetDeviceHorizontalScaleInPixel(0.10f);
            refreshMsgParams.LeftMargin = GetDeviceHorizontalScaleInPixel(0.10f);
            refreshMsgParams.TopMargin = (int)DPUtils.ConvertDPToPx(16f);
            refreshMsg.RequestLayout();

            btnRefreshParams.Width = GetDeviceHorizontalScaleInPixel(0.90f);
            btnRefreshParams.RightMargin = GetDeviceHorizontalScaleInPixel(0.05f);
            btnRefreshParams.LeftMargin = GetDeviceHorizontalScaleInPixel(0.05f);
            btnRefresh.RequestLayout();

            SetBottmLayoutParams(0);
        }

        public void SetBottmLayoutParams(float dp)
        {
            RelativeLayout.LayoutParams bottomContainerParams = bottomContainer.LayoutParameters as RelativeLayout.LayoutParams;

            bottomContainerParams.TopMargin = (int)DPUtils.ConvertDPToPx(dp);
            bottomContainer.RequestLayout();
        }

        public bool IsActive()
        {
            return IsAdded && IsVisible && !IsDetached && !IsRemoving;
        }

        public void OnLoadAccount()
        {
            this.presenter.LoadAccounts();
        }

        private void SetupMyServiceView()
        {
            topRootView.Visibility = ViewStates.Visible;
            myServiceContainer.Visibility = ViewStates.Visible;
            myServiceHideView.Visibility = ViewStates.Gone;
            myServiceRefreshContainer.Visibility = ViewStates.Gone;
            containerQuickAction.Visibility = ViewStates.Gone;

            GridLayoutManager layoutManager = new GridLayoutManager(this.Activity, 3);
            layoutManager.Orientation = RecyclerView.Vertical;
            myServiceListRecycleView.SetLayoutManager(layoutManager);
            myServiceListRecycleView.AddItemDecoration(new MyServiceItemDecoration(3, 3, false, this.Activity));

            GridLayoutManager layoutShimmerManager = new GridLayoutManager(this.Activity, 3);
            layoutShimmerManager.Orientation = RecyclerView.Vertical;
            myServiceShimmerList.SetLayoutManager(layoutShimmerManager);
            myServiceShimmerList.AddItemDecoration(new MyServiceShimmerItemDecoration(3, 3, false, this.Activity));
        }

        private void SetupNewFAQView()
        {
            LinearLayoutManager linearLayoutManager = new LinearLayoutManager(this.Activity, LinearLayoutManager.Horizontal, false);
            newFAQListRecycleView.SetLayoutManager(linearLayoutManager);
            //LinearSnapHelper snapHelper = new LinearSnapHelper();
            //snapHelper.AttachToRecyclerView(newFAQListRecycleView);


            LinearLayoutManager linearShimmerLayoutManager = new LinearLayoutManager(this.Activity, LinearLayoutManager.Horizontal, false);
            newFAQShimmerList.SetLayoutManager(linearShimmerLayoutManager);
            LinearSnapHelper shimmerSnapHelper = new LinearSnapHelper();
            shimmerSnapHelper.AttachToRecyclerView(newFAQShimmerList);
        }

        private void SetupDiscoverView()
        {
            List<CustomerBillingAccount> allAccountList = CustomerBillingAccount.List();

            if (allAccountList.Count > 0)
            {
                discoverView.Visibility = ViewStates.Visible;
                img_discover_digital_bill.Visibility = ViewStates.Visible;
                img_discover_digital_bill.SetImageResource(LanguageUtil.GetAppLanguage() == "MS"
                    ? Resource.Drawable.Banner_Home_DBR_MS_New
                    : Resource.Drawable.Banner_Home_DBR_EN_New);
            }
            else
            {
                HideDiscoverViewView();
            }
           
        }

        private void SetupEBDiscoverView()
        {
            discoverMoreEBImgLayout.Visibility = ViewStates.Visible;
            img_discover_energy_budget.SetImageResource(LanguageUtil.GetAppLanguage() == "MS"
                ? Resource.Drawable.eb_discover_more_bm
                : Resource.Drawable.eb_discover_more);
        }

        private void SetupSDDiscoverView()
        {
            discoverMoreSDImgLayout.Visibility = ViewStates.Visible;
            img_discover_service_disruption.SetImageResource(LanguageUtil.GetAppLanguage() == "MS"
                ? Resource.Drawable.sd_discover_more_bm
                : Resource.Drawable.sd_discover_more);
        }


        public void SetMyServiceRecycleView()
        {
            this.Activity.RunOnUiThread(() =>
            {
                myServiceShimmerAdapter = new MyServiceShimmerAdapter(this.presenter.LoadShimmerServiceList(3), this.Activity);
                myServiceShimmerList.SetAdapter(myServiceShimmerAdapter);

                myServiceShimmerView.Visibility = ViewStates.Visible;
                myServiceView.Visibility = ViewStates.Gone;
                containerQuickAction.Visibility = ViewStates.Gone;
            });

            Task.Run(() =>
            {
                this.presenter.InitiateMyService();
            });
        }

        public void SetMyServicesResult(List<MyServiceModel> list)
        {
            try
            {
                Activity.RunOnUiThread(() =>
                {
                    try
                    {
                        myServiceAdapter = new MyServiceAdapter(list, this.Activity, isRefreshShown);
                        myServiceListRecycleView.SetAdapter(myServiceAdapter);
                        myServiceAdapter.NotifyDataSetChanged();
                        myServicesList.Clear();
                        myServicesList.AddRange(list);
                        myServiceAdapter.ClickChanged += OnClickChanged;
                        this.SetIsClicked(false);
                        try
                        {
                            myServiceShimmerAdapter = new MyServiceShimmerAdapter(null, this.Activity);
                            myServiceShimmerList.SetAdapter(myServiceShimmerAdapter);
                        }
                        catch (System.Exception e)
                        {
                            Utility.LoggingNonFatalError(e);
                        }
                        myServiceShimmerView.Visibility = ViewStates.Gone;

                        myServiceView.Visibility = ViewStates.Visible;
                        containerQuickAction.Visibility = ViewStates.Visible;

                        SetupMyHomeBanner();
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

        public void SetNewFAQRecycleView()
        {
            NewFAQParentEntity wtManager = new NewFAQParentEntity();
            List<NewFAQParentEntity> items = wtManager.GetAllItems();
            if (items != null && items.Count > 0)
            {
                NewFAQParentEntity entity = items[0];
                if (entity != null && !entity.ShowNeedHelp)
                {
                    HideNewFAQ();
                }
                else
                {
                    SetupNewFAQShimmerEffect();
                }
            }
            else
            {
                SetupNewFAQShimmerEffect();
            }
            this.presenter.GetSavedNewFAQTimeStamp();
        }

        //private void StartShimmerDiscoverMore()
        //{
        //    try
        //    {
        //        DiscoverMoreShimmerImgLayout.Visibility = ViewStates.Visible;
        //        DiscoverMoreShimmerTxtLayout.Visibility = ViewStates.Visible;
        //        NewDiscoverMoreShimmerImgLayout.Visibility = ViewStates.Gone;
        //        newDiscoverMoreShimmerTxtLayout.Visibility = ViewStates.Gone;
        //        var shimmerBuilder = ShimmerUtils.ShimmerBuilderConfig();
        //        if (shimmerBuilder != null)
        //        {
        //            shimmerDiscoverMoreImageLayout.SetShimmer(shimmerBuilder?.Build());
        //            shimmerDiscoverMoreTxtLayout.SetShimmer(shimmerBuilder?.Build());
        //        }
        //        shimmerDiscoverMoreImageLayout.StartShimmer();
        //        shimmerDiscoverMoreTxtLayout.StartShimmer();
        //    }
        //    catch (System.Exception ex)
        //    {
        //        Utility.LoggingNonFatalError(ex);
        //    }
        //}

        //public void StopShimmerDiscoverMore()
        //{
        //    try
        //    {
        //        Activity.RunOnUiThread(() =>
        //        {
        //            try
        //            {

        //                shimmerDiscoverMoreImageLayout.StopShimmer();
        //                shimmerDiscoverMoreTxtLayout.StopShimmer();
        //            }
        //            catch (System.Exception ex)
        //            {
        //                Utility.LoggingNonFatalError(ex);
        //            }
        //            NewDiscoverMoreShimmerImgLayout.Visibility = ViewStates.Visible;
        //            newDiscoverMoreShimmerTxtLayout.Visibility = ViewStates.Visible;
        //            DiscoverMoreShimmerImgLayout.Visibility = ViewStates.Gone;
        //            DiscoverMoreShimmerTxtLayout.Visibility = ViewStates.Gone;
        //        });
        //    }
        //    catch (System.Exception ex)
        //    {
        //        Utility.LoggingNonFatalError(ex);
        //    }
        //}

        private void SetupNewFAQShimmerEffect()
        {
            try
            {
                Activity.RunOnUiThread(() =>
                {
                    try
                    {
                        newFAQTitle.Visibility = ViewStates.Visible;
                        newFAQShimmerAdapter = new NewFAQShimmerAdapter(this.presenter.LoadShimmerFAQList(4), this.Activity);
                        newFAQShimmerList.SetAdapter(newFAQShimmerAdapter);

                        newFAQShimmerView.Visibility = ViewStates.Visible;
                        newFAQView.Visibility = ViewStates.Gone;
                        var shimmerBuilder = ShimmerUtils.ShimmerBuilderConfig();
                        if (shimmerBuilder != null)
                        {
                            shimmerFAQView.SetShimmer(shimmerBuilder?.Build());
                        }
                        shimmerFAQView.StartShimmer();
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

        public void SetDBRDiscoverView()
        {
            SetDiscoverResult(IsAccountDBREligible);

            //this.presenter.GetSavedNewFAQTimeStamp();
        }

        public void HideNewFAQ()
        {
            try
            {
                Activity.RunOnUiThread(() =>
                {
                    try
                    {
                        newFAQShimmerView.Visibility = ViewStates.Gone;
                        newFAQTitle.Visibility = ViewStates.Gone;
                        newFAQView.Visibility = ViewStates.Gone;

                        OnHideBottomView();
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
        public void HideDiscoverViewView()
        {
            try
            {
                Activity.RunOnUiThread(() =>
                {
                    try
                    {
                        discoverView.Visibility = ViewStates.Gone;
                        img_discover_digital_bill.Visibility = ViewStates.Gone;
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

        public bool CheckNeedHelpHide()
        {
            return newFAQTitle.Visibility == ViewStates.Gone;
        }
        public bool CheckDBRDiscoverHide()
        {
            return discovercontainer.Visibility == ViewStates.Gone;
        }

        public void SetDiscoverResult(bool IsAccountDBREligible)
        {
            try
            {
                Activity.RunOnUiThread(() =>
                {
                    try
                    {
                        this.IsAccountDBREligible = IsAccountDBREligible;
                        //TenantDBR
                        List<PostBREligibilityIndicatorsModel> tenantList = TenantDBRCache.Instance.IsTenantDBREligible();

                        if (IsAccountDBREligible)
                        {

                            if (CustomerBillingAccount.HasOwnerCA())
                            {
                                SetupDiscoverView();
                                discovercontainer.Visibility = ViewStates.Visible;
                                discoverView.Visibility = ViewStates.Visible;
                                img_discover_digital_bill.Visibility = ViewStates.Visible;
                                discoverMoreSectionTitle.Visibility = ViewStates.Visible;

                            }
                            else
                            {
                                if (tenantList != null)
                                {
                                    int countCA = 0;
                                    bool flagOwner = false;
                                    List<string> dBRCAs = DBRUtility.Instance.GetCAList();
                                    List<CustomerBillingAccount> accounts = CustomerBillingAccount.List();
                                    CustomerBillingAccount tenantOwnerInfo = new CustomerBillingAccount();



                                    foreach (CustomerBillingAccount item in accounts)
                                    {
                                        if (item.AccountHasOwner == true)
                                        {
                                            flagOwner = true;
                                        }
                                    }

                                    for (int j = 0; j < accounts.Count; j++)
                                    {
                                        for (int i = 0; i < tenantList.Count; i++)
                                        {
                                            if (flagOwner
                                                && tenantList[i].CaNo == accounts[j].AccNum
                                                && !tenantList[i].IsOwnerOverRule
                                                && !tenantList[i].IsOwnerAlreadyOptIn
                                                && !tenantList[i].IsTenantAlreadyOptIn)
                                            {
                                                countCA++;
                                            }
                                        }

                                    }

                                    if (countCA > 0)
                                    {
                                        SetupDiscoverView();
                                        discovercontainer.Visibility = ViewStates.Visible;
                                        discoverView.Visibility = ViewStates.Visible;
                                        img_discover_digital_bill.Visibility = ViewStates.Visible;
                                        discoverMoreSectionTitle.Visibility = ViewStates.Visible;
                                    }
                                    else
                                    {
                                        discovercontainer.Visibility = ViewStates.Gone;
                                    }

                                }
                                else
                                {
                                    discovercontainer.Visibility = ViewStates.Gone;

                                }

                            }
                        }
                        //}
                        //else
                        //{
                        //    discovercontainer.Visibility = ViewStates.Gone;

                        //}

                    }
                    catch (System.Exception ex)
                    {
                        // TODO: To Hide the FAQ
                        // HideNewFAQ();
                        Utility.LoggingNonFatalError(ex);
                    }
                });
            }
            catch (System.Exception e)
            {
                // TODO: To Hide the FAQ
                // HideNewFAQ();
                Utility.LoggingNonFatalError(e);
            }
        }

        public void ShowManageBill()
        {
            try
            {
                DynatraceHelper.OnTrack(DynatraceConstants.DBR.CTAs.Home.Reminder_Popup_Viewmore);
                GetBillRenderingAsync();
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }
        public void SetNewFAQResult(List<NewFAQ> list)
        {
            try
            {
                Activity.RunOnUiThread(() =>
                {
                    try
                    {
                        if (list != null && list.Count > 0)
                        {
                            newFAQAdapter = new NewFAQAdapter(list, this.Activity);
                            newFAQListRecycleView.SetAdapter(newFAQAdapter);
                            currentNewFAQList.Clear();
                            currentNewFAQList.AddRange(list);
                            int visibleCards = TextViewUtils.IsLargeFonts ? 2 : 3;
                            if (indicatorContainer != null && indicatorContainer.ChildCount > 0)
                            {
                                indicatorContainer.RemoveAllViews();
                            }

                            if (list != null && list.Count > visibleCards)
                            {
                                indicatorContainer.Visibility = ViewStates.Visible;
                                if (mListener == null)
                                {
                                    mListener = new NewFAQScrollListener(list, indicatorContainer);
                                    newFAQListRecycleView.AddOnScrollListener(mListener);
                                }
                                else
                                {
                                    newFAQListRecycleView.RemoveOnScrollListener(mListener);
                                    mListener = new NewFAQScrollListener(list, indicatorContainer);
                                    newFAQListRecycleView.AddOnScrollListener(mListener);
                                }

                                int count = 0;
                                for (int i = 0; i < list.Count; i += visibleCards)
                                {
                                    ImageView image = new ImageView(this.Activity);
                                    image.Id = i;
                                    LinearLayout.LayoutParams layoutParams = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent
                                        , ViewGroup.LayoutParams.WrapContent);
                                    layoutParams.RightMargin = 9;
                                    layoutParams.LeftMargin = 9;
                                    image.LayoutParameters = layoutParams;
                                    if (i == 0)
                                    {
                                        image.SetImageResource(Resource.Drawable.onboarding_circle_active);
                                    }
                                    else
                                    {
                                        image.SetImageResource(Resource.Drawable.faq_indication_inactive);
                                    }
                                    indicatorContainer.AddView(image, count);
                                    count++;
                                }
                            }
                            else
                            {
                                if (mListener == null)
                                {
                                    mListener = new NewFAQScrollListener(list, indicatorContainer);
                                    newFAQListRecycleView.AddOnScrollListener(mListener);
                                }
                                else
                                {
                                    newFAQListRecycleView.RemoveOnScrollListener(mListener);
                                    mListener = new NewFAQScrollListener(list, indicatorContainer);
                                    newFAQListRecycleView.AddOnScrollListener(mListener);
                                }
                                indicatorContainer.Visibility = ViewStates.Gone;
                            }

                            newFAQAdapter.ClickChanged += OnFAQClickChanged;
                            try
                            {
                                shimmerFAQView.StopShimmer();
                                newFAQShimmerAdapter = new NewFAQShimmerAdapter(null, this.Activity);
                                newFAQShimmerList.SetAdapter(newFAQShimmerAdapter);
                            }
                            catch (System.Exception e)
                            {
                                Utility.LoggingNonFatalError(e);
                            }
                            newFAQShimmerView.Visibility = ViewStates.Gone;
                            newFAQView.Visibility = ViewStates.Visible;
                            newFAQTitle.Visibility = ViewStates.Visible;

                        }
                    }
                    catch (System.Exception ex)
                    {
                        // TODO: To Hide the FAQ
                        // HideNewFAQ();
                        Utility.LoggingNonFatalError(ex);
                    }
                });
            }
            catch (System.Exception e)
            {
                // TODO: To Hide the FAQ
                // HideNewFAQ();
                Utility.LoggingNonFatalError(e);
            }
        }

        public override void OnPause()
        {
            base.OnPause();
            this.presenter.OnCancelToken();
            NewAppTutorialUtils.ForceCloseNewAppTutorial();
        }

        public void ShowSearchAction(bool isShow)
        {
            if (isShow)
            {
                accountHeaderTitle.Visibility = ViewStates.Gone;
                searchEditText.Visibility = ViewStates.Visible;
                searchEditText.SetMaxWidth(Integer.MaxValue);
                searchActionContainer.Visibility = ViewStates.Gone;
                accountHeaderTitle.Visibility = ViewStates.Gone;
                addActionContainer.Visibility = ViewStates.Gone;
                accountActionDivider.Visibility = ViewStates.Gone;
                searchEditText.OnActionViewExpanded();
                searchEditText.RequestFocus();
                try
                {
                    if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
                    {
                        searchEditText.SetBackgroundResource(Resource.Drawable.search_edit_bg);
                    }
                    else
                    {
                        searchEditText.SetBackgroundDrawable(null);
                    }
                }
                catch (System.Exception cex)
                {
                    Utility.LoggingNonFatalError(cex);
                }
                if (closeImageView != null)
                {
                    closeImageView.Visibility = ViewStates.Visible;
                }
            }
            else
            {
                accountHeaderTitle.Visibility = ViewStates.Visible;
                accountHeaderTitle.Visibility = ViewStates.Visible;
                searchEditText.Visibility = ViewStates.Gone;
                searchActionContainer.Visibility = ViewStates.Visible;
                addActionContainer.Visibility = ViewStates.Visible;
                accountActionDivider.Visibility = ViewStates.Visible;
            }
        }

        private void SetAccountActionHeader()
        {
            LinearLayout.LayoutParams param = (LinearLayout.LayoutParams)accountsActionsContainer.LayoutParameters;
            param.LeftMargin = (int)DPUtils.ConvertDPToPx(16f);
            param.RightMargin = (int)DPUtils.ConvertDPToPx(16f);

            TextViewUtils.SetMuseoSans500Typeface(accountHeaderTitle, accountGreeting, accountGreetingName);
            accountHeaderTitle.Text = GetLabelByLanguage("myAccounts");
            searchEditText.SetIconifiedByDefault(false);
            var hintText = TextViewUtils.IsLargeFonts ? (Html.FromHtml("<big>" + GetLabelByLanguage("searchPlaceholder") + "</big>")) : Html.FromHtml(GetLabelByLanguage("searchPlaceholder"));
            searchEditText.SetQueryHint(hintText);
            searchEditText.SetOnQueryTextListener(new AccountsSearchOnQueryTextListener(this, accountsAdapter));
            searchEditText.SetOnQueryTextFocusChangeListener(this);

            try
            {
                if (Build.VERSION.SdkInt < BuildVersionCodes.N)
                {
                    LinearLayout.LayoutParams searchParam = (LinearLayout.LayoutParams)searchEditText.LayoutParameters;
                    searchParam.LeftMargin = -(int)DPUtils.ConvertDPToPx(32f);
                }
            }
            catch (System.Exception cex)
            {
                Utility.LoggingNonFatalError(cex);
            }

            try
            {
                EditText searchText = searchEditText.FindViewById<EditText>(searchEditText.Context.Resources.GetIdentifier("android:id/search_src_text", null, null));
                searchText.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(this.Activity, Resource.Color.white)));
                searchText.SetHintTextColor(new Android.Graphics.Color(ContextCompat.GetColor(this.Activity, Resource.Color.sixty_opacity_white)));
                TextViewUtils.SetTextSize12(searchText);
                TextViewUtils.SetTextSize14(discoverMoreSectionTitle);
                TextViewUtils.SetMuseoSans500Typeface(searchText, discoverMoreSectionTitle);
                discoverMoreSectionTitle.Text = Utility.GetLocalizedLabel("DashboardHome", "DiscoverMore");
                if (Build.VERSION.SdkInt >= BuildVersionCodes.N)
                {
                    searchText.SetPadding((int)DPUtils.ConvertDPToPx(34f), 0, 0, 0);
                }
                else
                {
                    searchText.SetPadding(0, (int)DPUtils.ConvertDPToPx(16f), (int)DPUtils.ConvertDPToPx(16f), 0);
                }
            }
            catch (System.Exception cex)
            {
                Utility.LoggingNonFatalError(cex);
            }

            try
            {
                int searchMagId = searchEditText.Context.Resources.GetIdentifier("android:id/search_mag_icon", null, null);
                ImageView mSearchHintIcon = searchEditText.FindViewById<ImageView>(searchMagId);
                mSearchHintIcon.Visibility = ViewStates.Gone;
                mSearchHintIcon.SetImageDrawable(null);
            }
            catch (System.Exception cex)
            {
                Utility.LoggingNonFatalError(cex);
            }

            try
            {
                if (Build.VERSION.SdkInt >= BuildVersionCodes.N)
                {
                    searchEditText.SetBackgroundResource(Resource.Drawable.search_edit_bg);
                }
                else
                {
                    searchEditText.SetBackgroundDrawable(null);
                }
            }
            catch (System.Exception cex)
            {
                Utility.LoggingNonFatalError(cex);
            }

            try
            {
                int closeViewId = searchEditText.Context.Resources.GetIdentifier("android:id/search_close_btn", null, null);
                closeImageView = searchEditText.FindViewById<ImageView>(closeViewId);
                closeImageView.Visibility = ViewStates.Visible;
                closeImageView.SetImageResource(Resource.Drawable.search_close_bg);
                closeImageView.SetPadding(0, 0, 0, 0);
            }
            catch (System.Exception cex)
            {
                Utility.LoggingNonFatalError(cex);
            }

            searchActionContainer.Click += (s, e) =>
            {
                if (isSearchClose)
                {
                    ShowSearchAction(true);
                    try
                    {
                        FirebaseAnalyticsUtils.LogFragmentClickEvent(this, "Home Screen Search Button Clicked");
                    }
                    catch (System.Exception err)
                    {
                        Utility.LoggingNonFatalError(err);
                    }
                    isSearchClose = false;
                }
                else
                {
                    isSearchClose = true;
                }
            };
        }

        private void SetAccountsRecyclerView()
        {
            LinearLayoutManager linearLayoutManager = new LinearLayoutManager(Activity, LinearLayoutManager.Vertical, false);
            accountsRecyclerView.SetLayoutManager(linearLayoutManager);

            accountsAdapter = new AccountsRecyclerViewAdapter(this);
        }

        public override void OnResume()
        {
            base.OnResume();

            try
            {
                if (this.presenter != null)
                {
                    if (this.presenter.GetIsLoadedHomeDone())
                    {
                        this.presenter.OnCheckMyServiceNewFAQState();
                    }
                    else if (isFromPageRearrangeIcon)
                    {
                        this.presenter.OnCheckMyServiceNewFAQState();
                    }
                }
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

            try
            {
                var act = this.Activity as AppCompatActivity;

                var actionBar = act.SupportActionBar;
                actionBar.Hide();
                ShowBackButton(false);

                if (newLabel != null)
                {
                    if (!MyTNBAccountManagement.GetInstance().IsOnHoldWhatNew())
                    {
                        SetNotificationIndicator();
                    }
                    //OnSetupNotificationNewLabel(true, 0);
                }

                if (this.presenter != null)
                {
                    if (!MyTNBAccountManagement.GetInstance().IsOnHoldWhatNew())
                    {
                        this.presenter.GetUserNotifications();
                    }
                    UpdateGreetingsHeader(this.presenter.GetGreeting());
                }

                if (summaryNestScrollView != null)
                {
                    HomeMenuCustomScrolling(0);
                }

                if (HomeMenuUtils.GetIsShowRearrangeAccountSuccessfulNeed())
                {
                    HomeMenuUtils.SetIsShowRearrangeAccountSuccessfulNeed(false);
                    ShowRearrangeAccountSuccessful();
                }

                ((DashboardHomeActivity)Activity).EnableDropDown(false);
                ((DashboardHomeActivity)Activity).HideAccountName();
                ((DashboardHomeActivity)Activity).RemoveHeaderDropDown();
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

            try
            {
                if (MyTNBAccountManagement.GetInstance().IsOnHoldWhatNew())
                {
                    WhatNewCheckAgain();
                    //SetDBRDiscoverView();
                    SetupDiscoverView();
                }
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

            if (MyTNBAccountManagement.GetInstance().GetPostGetDraftResponse() != null)
            {
                ((DashboardHomeActivity)Activity).OnCheckDraftResumePopUp();
            }
        }

        public void CallOnCheckShowHomeTutorial()
        {
            try
            {
                this.presenter.OnCheckToCallHomeMenuTutorial();
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void OnUpdateAccountListChanged(bool isSearchSubmit)
        {
            if (isSearchSubmit)
            {
                ShowSearchAction(false);
            }
        }

        private void OnClickChanged(object sender, int position)
        {
            try
            {
                if (position != -1)
                {
                    if (!this.GetIsClicked())
                    {
                        this.SetIsClicked(true);

                        MyServiceModel selectedService = myServicesList[position];
                        if (selectedService.ServiceType == MobileEnums.ServiceEnum.SUBMITFEEDBACK)
                        {
                            ShowFeedbackMenu();
                        }
                        else if (selectedService.ServiceType == MobileEnums.ServiceEnum.SELFMETERREADING)
                        {
                            try
                            {
                                var myAction = Agent.Instance.EnterAction(Constants.SMR_icon_click);  // DYNA
                                myAction.ReportValue("session_id", LaunchViewActivity.DynatraceSessionUUID);
                                myAction.LeaveAction();
                            }
                            catch (System.Exception ex)
                            {
                                Utility.LoggingNonFatalError(ex);
                            }

                            if (!UserSessions.HasSMROnboardingShown(PreferenceManager.GetDefaultSharedPreferences(this.Activity)))
                            {
                                UserSessions.DoSMROnboardingShown(PreferenceManager.GetDefaultSharedPreferences(this.Activity));
                            }

                            Intent applySMRIntent = new Intent(this.Activity, typeof(SSMRMeterHistoryActivity));
                            StartActivityForResult(applySMRIntent, SSMR_METER_HISTORY_ACTIVITY_CODE);
                        }
                        else if (selectedService.ServiceType == MobileEnums.ServiceEnum.PAYBILL)
                        {
                            //if (myServicesList.Count > 0)
                            //{
                            //    SetMyServicesResult(myServicesList);
                            //}

                            //SetMyServiceRecycleView();

                            //this.presenter.InitiateService(); //refresh icon

                            if (Utility.IsEnablePayment()
                            && !isRefreshShown && MyTNBAccountManagement.GetInstance().IsPayBillEnabledNeeded())
                            {
                                if (!UserSessions.HasPayBillShown(PreferenceManager.GetDefaultSharedPreferences(this.Activity)))
                                {
                                    UserSessions.DoPayBillShown(PreferenceManager.GetDefaultSharedPreferences(this.Activity));
                                }
                                Intent payment_activity = new Intent(this.Activity, typeof(SelectAccountsActivity));
                                StartActivity(payment_activity);
                            }

                            if (!Utility.IsEnablePayment())
                            {
                                DownTimeEntity pgXEntity = DownTimeEntity.GetByCode(Constants.PG_SYSTEM);
                                if (pgXEntity != null)
                                {
                                    OnBCRMDownTimeErrorMessageV2(pgXEntity);
                                    this.SetIsClicked(false);
                                    //Utility.ShowBCRMDOWNTooltip(this.Activity, pgXEntity, () =>
                                    //{
                                    //    this.SetIsClicked(false);
                                    //});
                                }
                            }
                        }
                        else if (selectedService.ServiceType == MobileEnums.ServiceEnum.VIEWBILL && (!isRefreshShown
                            && MyTNBAccountManagement.GetInstance().IsViewBillEnabledNeeded()))
                        {
                            if (!UserSessions.HasViewBillShown(PreferenceManager.GetDefaultSharedPreferences(this.Activity)))
                            {
                                UserSessions.DoViewBillShown(PreferenceManager.GetDefaultSharedPreferences(this.Activity));
                            }
                            if (CustomerBillingAccount.HasOneItemOnly())
                            {
                                CustomerBillingAccount.RemoveSelected();
                                CustomerBillingAccount.MakeFirstAsSelected();
                                CustomerBillingAccount customerAccount = CustomerBillingAccount.GetSelected();

                                AccountData selectedAccountData = AccountData.Copy(customerAccount, true);

                                Intent viewBill = new Intent(this.Activity, typeof(ViewBillActivity));
                                viewBill.PutExtra(Constants.SELECTED_ACCOUNT, JsonConvert.SerializeObject(selectedAccountData));
                                viewBill.PutExtra(Constants.CODE_KEY, Constants.SELECT_ACCOUNT_PDF_REQUEST_CODE);
                                StartActivity(viewBill);
                            }
                            else
                            {
                                CustomerBillingAccount.RemoveSelected();
                                Intent supplyAccount = new Intent(this.Activity, typeof(SelectSupplyAccountActivity));
                                supplyAccount.PutExtra(Constants.CODE_KEY, Constants.SELECT_ACCOUNT_PDF_REQUEST_CODE);
                                StartActivity(supplyAccount);
                            }
                        }
                        else if (selectedService.ServiceType == MobileEnums.ServiceEnum.APPLICATIONSTATUS)
                        {
                            _searchApplicationTypeResponse = SearchApplicationTypeCache.Instance.GetData();
                            if (_searchApplicationTypeResponse == null)
                            {
                                ShowProgressDialog();

                                Task.Run(() =>
                                {
                                    _ = OnSearchApplicationTypeAsync();
                                });
                            }
                            else
                            {
                                CheckApplicationResponse();
                            }

                            this.SetIsClicked(false);
                        }
                        else if (selectedService.ServiceType == MobileEnums.ServiceEnum.ENERGYBUDGET)
                        {
                            if (Utility.IsMDMSDownEnergyBudget() && !isRefreshShown)
                            {
                                if (!UserSessions.HasSmartMeterShown(PreferenceManager.GetDefaultSharedPreferences(this.Activity)))
                                {
                                    UserSessions.DoSmartMeterShown(PreferenceManager.GetDefaultSharedPreferences(this.Activity));
                                }

                                DownTimeEntity TRILEntity = DownTimeEntity.GetByCode(Constants.TRIL_SYSTEM);
                                DownTimeEntity SAGEEntity = DownTimeEntity.GetByCode(Constants.SAGE_SYSTEM);
                                DownTimeEntity CatchupCell = DownTimeEntity.GetByCode(Constants.CatchupCell);
                                DownTimeEntity CatchupPLC = DownTimeEntity.GetByCode(Constants.CatchupPLC);
                                DownTimeEntity CatchupRF = DownTimeEntity.GetByCode(Constants.CatchupRF);
                                if (SAGEEntity != null && TRILEntity != null && CatchupCell != null && CatchupPLC != null && CatchupRF != null)
                                {
                                    if (TRILEntity.IsDown || SAGEEntity.IsDown || CatchupCell.IsDown || CatchupPLC.IsDown || CatchupRF.IsDown)
                                    {
                                        List<CustomerBillingAccount> smartmeterAccounts = CustomerBillingAccount.SMeterBudgetAccountList();        //smart meter ca
                                        string tril = "TRIL";
                                        string sage = "SAGE";
                                        string cell = "CEL";
                                        string plc = "PLC";
                                        string rf = "RF";
                                        bool resultTril = smartmeterAccounts.Exists(s => s.SmartMeterCode == tril);
                                        bool resultSage = smartmeterAccounts.Exists(s => s.SmartMeterCode == sage);
                                        bool resultCell = smartmeterAccounts.Exists(s => s.AMSIDCategory == cell);
                                        bool resultPlc = smartmeterAccounts.Exists(s => s.AMSIDCategory == plc);
                                        bool resultRf = smartmeterAccounts.Exists(s => s.AMSIDCategory == rf);
                                        if (TRILEntity.IsDown && resultTril)
                                        {
                                            Utility.ShowBCRMDOWNTooltip(this.Activity, TRILEntity, () =>
                                            {
                                                this.SetIsClicked(false);
                                                EnergyBudgetPage();
                                            });
                                        }
                                        else if (SAGEEntity.IsDown && resultSage)
                                        {
                                            Utility.ShowBCRMDOWNTooltip(this.Activity, SAGEEntity, () =>
                                            {
                                                this.SetIsClicked(false);
                                                EnergyBudgetPage();
                                            });
                                        }
                                        else if (CatchupCell.IsDown && resultCell)
                                        {
                                            Utility.ShowBCRMDOWNTooltip(this.Activity, CatchupCell, () =>
                                            {
                                                this.SetIsClicked(false);
                                                EnergyBudgetPage();
                                            });
                                        }
                                        else if (CatchupPLC.IsDown && resultPlc)
                                        {
                                            Utility.ShowBCRMDOWNTooltip(this.Activity, CatchupPLC, () =>
                                            {
                                                this.SetIsClicked(false);
                                                EnergyBudgetPage();
                                            });
                                        }
                                        else if (CatchupRF.IsDown && resultRf)
                                        {
                                            Utility.ShowBCRMDOWNTooltip(this.Activity, CatchupRF, () =>
                                            {
                                                this.SetIsClicked(false);
                                                EnergyBudgetPage();
                                            });
                                        }
                                        else
                                        {
                                            EnergyBudgetPage();
                                        }
                                    }
                                    else
                                    {
                                        EnergyBudgetPage();
                                    }
                                }
                                else
                                {
                                    EnergyBudgetPage();
                                }
                            }
                            else if (!Utility.IsMDMSDownEnergyBudget())
                            {
                                DownTimeEntity SMEntity = DownTimeEntity.GetByCode(Constants.SMART_METER_SYSTEM);
                                DownTimeEntity EBEntity = DownTimeEntity.GetByCode(Constants.EB_SYSTEM);
                                //if (SMEntity != null && EBEntity != null && SMEntity.IsDown && !MyTNBAccountManagement.GetInstance().IsMaintenanceDialogShown())
                                if (SMEntity != null && EBEntity != null && !MyTNBAccountManagement.GetInstance().IsMaintenanceDialogShown())
                                {
                                    Utility.ShowBCRMDOWNTooltip(this.Activity, EBEntity, () =>
                                    {
                                        this.SetIsClicked(false);

                                    });
                                }
                            }
                        }
                        else if (selectedService.ServiceType == MobileEnums.ServiceEnum.MYHOME)
                        {
                            if (!UserSessions.MyHomeQuickLinkHasShown(PreferenceManager.GetDefaultSharedPreferences(this.Activity)))
                            {
                                UserSessions.SetShownMyHomeQuickLink(PreferenceManager.GetDefaultSharedPreferences(this.Activity));
                            }

                            List<MyDrawerModel> drawerList = new List<MyDrawerModel>();

                            if (selectedService.Children != null
                                && selectedService.Children.Count > 0)
                            {
                                foreach (MyServiceModel child in selectedService.Children)
                                {
                                    drawerList.Add(new MyDrawerModel()
                                    {
                                        ParentServiceId = child.ParentServiceId,
                                        ServiceId = child.ServiceId,
                                        ServiceName = child.ServiceName,
                                        ServiceIconUrl = child.ServiceIconUrl,
                                        DisabledServiceIconUrl = child.DisabledServiceIconUrl,
                                        SSODomain = child.SSODomain,
                                        OriginURL = child.OriginURL,
                                        RedirectURL = child.RedirectURL,
                                        ServiceType = child.ServiceType
                                    });
                                }

                                MyHomeDrawerFragment myHomeBottomSheetDialog = new MyHomeDrawerFragment(this.Activity, drawerList, selectedService.ServiceName);

                                myHomeBottomSheetDialog.Cancelable = true;
                                myHomeBottomSheetDialog.Show(this.Activity.SupportFragmentManager, "My Home Dialog");
                            }

                            this.SetIsClicked(false);
                        }
                        else if (selectedService.ServiceType == MobileEnums.ServiceEnum.VIEWMORE)
                        {
                            this.presenter.DoMySerivceLoadMoreAccount();
                            this.SetIsClicked(false);
                        }
                        else if (selectedService.ServiceType == MobileEnums.ServiceEnum.VIEWLESS)
                        {
                            this.presenter.DoMyServiceLoadLessAccount();
                            this.SetIsClicked(false);
                        }
                        else
                        {
                            this.SetIsClicked(false);
                        }

                        try
                        {
                            FirebaseAnalyticsUtils.LogFragmentClickEvent(this, "My Services Card Clicked");
                        }
                        catch (System.Exception err)
                        {
                            Utility.LoggingNonFatalError(err);
                        }
                    }
                }
            }
            catch (System.Exception e)
            {
                this.SetIsClicked(false);
                Utility.LoggingNonFatalError(e);
            }
        }

        public void NavigateToSSMRPage()
        {
            isClickFromQuickActionSMR = false;
            Intent applySMRIntent = new Intent(this.Activity, typeof(SSMRMeterHistoryActivity));
            StartActivityForResult(applySMRIntent, SSMR_METER_HISTORY_ACTIVITY_CODE);
        }

        private void EnergyBudgetPage()
        {
            if (UserSessions.GetEnergyBudgetList().Count == 1)
            {
                this.SetIsClicked(false);
                List<SMRAccount> smacc = new List<SMRAccount>();
                smacc = UserSessions.GetEnergyBudgetList();
                ShowAccountDetails(smacc[0].accountNumber);
            }
            else if (UserSessions.GetEnergyBudgetList().Count > 1)
            {
                Intent energy_budget_activity = new Intent(this.Activity, typeof(EnergyBudgetActivity));
                StartActivityForResult(energy_budget_activity, SELECT_SM_ACCOUNT_REQUEST_CODE);
            }
        }

        private async Task OnSearchApplicationTypeAsync()
        {
            _searchApplicationTypeResponse = await ApplicationStatusManager.Instance.SearchApplicationType("16", UserEntity.GetActive() != null);

            SearchApplicationTypeCache.Instance.SetData(_searchApplicationTypeResponse);

            HideProgressDialog();
            CheckApplicationResponse();
        }

        private void CheckApplicationResponse()
        {
            if (_searchApplicationTypeResponse != null
                && _searchApplicationTypeResponse.StatusDetail != null)
            {
                if (_searchApplicationTypeResponse.StatusDetail.IsSuccess)
                {
                    AllApplicationsCache.Instance.Clear();
                    AllApplicationsCache.Instance.Reset();
                    Intent applicationLandingIntent = new Intent(this.Activity, typeof(ApplicationStatusLandingActivity));
                    this.Activity.StartActivityForResult(applicationLandingIntent, Constants.APPLICATION_STATUS_LANDING_FROM_DASHBOARD_REQUEST_CODE);
                }
                else
                {
                    this.Activity.RunOnUiThread(() =>
                    {
                        MyTNBAppToolTipBuilder errorPopup = MyTNBAppToolTipBuilder.Create(this.Activity, MyTNBAppToolTipBuilder.ToolTipType.NORMAL_WITH_HEADER)
                            .SetTitle(_searchApplicationTypeResponse?.StatusDetail?.Title ?? Utility.GetLocalizedLabel(LanguageConstants.ERROR, LanguageConstants.Error.DEFAULT_ERROR_TITLE))
                            .SetMessage(_searchApplicationTypeResponse?.StatusDetail?.Message ?? Utility.GetLocalizedLabel(LanguageConstants.ERROR, LanguageConstants.Error.DEFAULT_ERROR_MSG))
                            .SetCTALabel(_searchApplicationTypeResponse?.StatusDetail?.PrimaryCTATitle ?? Utility.GetLocalizedLabel(LanguageConstants.COMMON, LanguageConstants.Common.OK))
                            .Build();
                        errorPopup.Show();
                    });
                }
            }
        }

        void OnFAQClickChanged(object sender, int position)
        {
            try
            {
                if (position != -1)
                {
                    if (!this.GetIsClicked())
                    {
                        this.SetIsClicked(true);

                        NewFAQ selectedNewFAQ = currentNewFAQList[position];

                        if (FAQHasGSLTag(selectedNewFAQ.Tags))
                        {
                            ((DashboardHomeActivity)Activity).NavigateToGSL();
                        }
                        else
                        {
                            Intent faqIntent = new Intent(this.Activity, typeof(FAQListActivity));
                            faqIntent.PutExtra(Constants.FAQ_ID_PARAM, selectedNewFAQ.TargetItem);
                            Activity.StartActivity(faqIntent);
                        }

                        try
                        {
                            FirebaseAnalyticsUtils.LogFragmentClickEvent(this, "Need Help Card Clicked");
                        }
                        catch (System.Exception err)
                        {
                            Utility.LoggingNonFatalError(err);
                        }
                    }
                }
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        private bool FAQHasGSLTag(string tags)
        {
            var hasGSLTag = false;
            if (tags.IsValid())
            {
                var tagsList = tags.Split(FAQ_TAGS_SEPARATOR).ToList();
                if (tagsList != null && tagsList.Count > 0)
                {
                    int findIndex = tagsList.FindIndex(tag => tag == GSL_TAG);
                    hasGSLTag = findIndex > -1;
                }
            }
            return hasGSLTag;
        }

        public void ShowFeedbackMenu()
        {
            //Dynatrace

            var myAction = Agent.Instance.EnterAction(Constants.TOUCH_ON_SUBMIT_AND_TRACK_ENQUIRY);  // DYNA
            myAction.ReportValue("session_id", LaunchViewActivity.DynatraceSessionUUID);
            myAction.LeaveAction();

            ShowBackButton(true);
            FeedbackMenuFragment fragment = new FeedbackMenuFragment();

            if (((DashboardHomeActivity)Activity) != null)
            {
                ((DashboardHomeActivity)Activity).SetCurrentFragment(fragment);
                ((DashboardHomeActivity)Activity).HideAccountName();
                ((DashboardHomeActivity)Activity).SetToolbarTitle(Resource.String.feedback_menu_activity_title);
            }
            FragmentManager.BeginTransaction()
                           .Replace(Resource.Id.content_layout, fragment)
                     .CommitAllowingStateLoss();
        }

        public void ShowBackButton(bool flag)
        {
            var act = this.Activity as AppCompatActivity;

            var actionBar = act.SupportActionBar;
            actionBar.SetDisplayHomeAsUpEnabled(flag);
            actionBar.SetDisplayShowHomeEnabled(flag);
        }

        public string GetDeviceId()
        {
            return this.DeviceId();
        }

        private Snackbar mMyServiceRetrySnakebar;
        public void ShowMyServiceRetryOptions(string msg)
        {
            if (mMyServiceRetrySnakebar != null && mMyServiceRetrySnakebar.IsShown)
            {
                mMyServiceRetrySnakebar.Dismiss();
            }

            if (string.IsNullOrEmpty(msg))
            {
                msg = Utility.GetLocalizedErrorLabel("defaultErrorMessage");
            }

            mMyServiceRetrySnakebar = Snackbar.Make(rootView, msg, Snackbar.LengthIndefinite)
            .SetAction(Utility.GetLocalizedCommonLabel("retry"), delegate
            {

                mMyServiceRetrySnakebar.Dismiss();
                RetryMyService();
            }
            );
            View v = mMyServiceRetrySnakebar.View;
            TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
            tv.SetMaxLines(5);
            mMyServiceRetrySnakebar.Show();
        }

        private Snackbar mRearrangeSnackbar;
        public void ShowRearrangeAccountSuccessful()
        {
            if (mRearrangeSnackbar != null && mRearrangeSnackbar.IsShown)
            {
                mRearrangeSnackbar.Dismiss();
            }

            mRearrangeSnackbar = Snackbar.Make(rootView,
                Utility.GetLocalizedLabel("RearrangeAccount", "rearrangeToastSuccessMsg"),
                Snackbar.LengthLong);
            View v = mRearrangeSnackbar.View;
            TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
            tv.SetMaxLines(5);
            mRearrangeSnackbar.Show();
        }

        private void RetryMyService()
        {
            MyServiceShimmerAdapter adapter = new MyServiceShimmerAdapter(this.presenter.LoadShimmerServiceList(3), this.Activity);
            myServiceShimmerList.SetAdapter(adapter);

            myServiceShimmerView.Visibility = ViewStates.Visible;
            myServiceView.Visibility = ViewStates.Gone;
            containerQuickAction.Visibility = ViewStates.Gone;

            this.presenter.RetryMyService();
        }

        public void UpdateAccountListCards(List<SummaryDashBoardDetails> accountList)
        {
            accountsAdapter.UpdateAccountCards(accountList);
        }

        public void SetHeaderActionVisiblity(List<SummaryDashBoardDetails> accountList)
        {
            if (accountList.Count <= 3)
            {
                searchActionContainer.Visibility = ViewStates.Gone;
                accountActionDivider.Visibility = ViewStates.Gone;
                searchEditText.Visibility = ViewStates.Gone;
            }
            else
            {
                searchActionContainer.Visibility = ViewStates.Visible;
                accountActionDivider.Visibility = ViewStates.Visible;
            }
        }

        public void SetAccountListCards(List<SummaryDashBoardDetails> accountList)
        {
            accountsAdapter.SetAccountCards(accountList);
            accountsRecyclerView.SetAdapter(accountsAdapter);
        }

        public void SetAccountListCardsFromLocal(List<SummaryDashBoardDetails> accountList)
        {
            accountsAdapter.SetAccountCardsFromLocal(accountList);
            accountsRecyclerView.SetAdapter(accountsAdapter);
        }

        public void OnSavedTimeStamp(string savedTimeStamp)
        {
            if (savedTimeStamp != null)
            {
                this.mSavedTimeStamp = savedTimeStamp;
            }
            this.presenter.OnGetFAQTimeStamp();
        }

        public void ShowFAQTimestamp(bool success)
        {
            try
            {
                NewFAQParentEntity wtManager = new NewFAQParentEntity();
                List<NewFAQParentEntity> items = wtManager.GetAllItems();

                if (items != null && items.Count > 0)
                {
                    NewFAQParentEntity entity = items[0];
                    if (entity != null && !entity.ShowNeedHelp)
                    {
                        HideNewFAQ();
                        this.presenter.UpdateNewFAQCompleteState();
                    }
                    else
                    {
                        try
                        {
                            Activity.RunOnUiThread(() =>
                            {
                                try
                                {
                                    if (newFAQTitle.Visibility == ViewStates.Gone)
                                    {
                                        SetupNewFAQShimmerEffect();
                                    }
                                }
                                catch (System.Exception exp)
                                {
                                    Utility.LoggingNonFatalError(exp);
                                }
                            });
                        }
                        catch (System.Exception ex)
                        {
                            Utility.LoggingNonFatalError(ex);
                        }

                        if (success)
                        {
                            if (entity != null)
                            {
                                if (!entity.Timestamp.Equals(mSavedTimeStamp))
                                {
                                    this.presenter.OnGetFAQs();
                                }
                                else
                                {
                                    this.presenter.ReadNewFAQFromCache();
                                }
                            }
                            else
                            {
                                this.presenter.OnGetFAQs();
                            }
                        }
                        else
                        {
                            this.presenter.OnGetFAQs();
                        }
                    }
                }
                else
                {
                    try
                    {
                        Activity.RunOnUiThread(() =>
                        {
                            try
                            {
                                if (newFAQTitle.Visibility == ViewStates.Gone)
                                {
                                    SetupNewFAQShimmerEffect();
                                }
                            }
                            catch (System.Exception exp)
                            {
                                Utility.LoggingNonFatalError(exp);
                            }
                        });
                    }
                    catch (System.Exception ex)
                    {
                        Utility.LoggingNonFatalError(ex);
                    }

                    this.presenter.OnGetFAQs();
                }
            }
            catch (System.Exception e)
            {
                try
                {
                    Activity.RunOnUiThread(() =>
                    {
                        try
                        {
                            if (newFAQTitle.Visibility == ViewStates.Gone)
                            {
                                SetupNewFAQShimmerEffect();
                            }
                        }
                        catch (System.Exception exp)
                        {
                            Utility.LoggingNonFatalError(exp);
                        }
                    });
                }
                catch (System.Exception ex)
                {
                    Utility.LoggingNonFatalError(ex);
                }

                this.presenter.OnGetFAQs();
                Utility.LoggingNonFatalError(e);
            }
        }

        public void ShowFAQFromHide()
        {
            try
            {
                Activity.RunOnUiThread(() =>
                {
                    try
                    {
                        if (newFAQTitle.Visibility == ViewStates.Gone)
                        {
                            if (CheckNewFaqList() > 0)
                            {
                                if (mListener != null)
                                {
                                    newFAQListRecycleView.RemoveOnScrollListener(mListener);
                                    mListener = null;
                                }

                                if (indicatorContainer != null && indicatorContainer.ChildCount > 0)
                                {
                                    indicatorContainer.RemoveAllViews();
                                }

                                newFAQListRecycleView.SetAdapter(null);

                                newFAQAdapter = null;
                                currentNewFAQList.Clear();

                            }

                            SetupNewFAQShimmerEffect();
                            this.presenter.OnGetFAQs();
                        }
                        else
                        {
                            this.presenter.UpdateNewFAQCompleteState();
                        }
                    }
                    catch (System.Exception exp)
                    {
                        Utility.LoggingNonFatalError(exp);
                    }
                });
            }
            catch (System.Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }

        public void ShowAccountDetails(string accountNumber)
        {
            if (accountNumber != null)
            {
                if (!this.GetIsClicked())
                {
                    this.SetIsClicked(true);

                    CustomerBillingAccount.RemoveSelected();
                    CustomerBillingAccount.SetSelected(accountNumber);

                    if (mCallBack != null)
                    {
                        mCallBack.NavigateToDashBoardFragment();
                    }
                    else
                    {
                        try
                        {
                            ((DashboardHomeActivity)Activity).NavigateToDashBoardFragment();
                        }
                        catch (System.Exception e)
                        {
                            Utility.LoggingNonFatalError(e);
                        }
                    }
                }
            }
        }

        public void ShowProgressDialog()
        {
            try
            {
                LoadingOverlayUtils.OnRunLoadingAnimation(this.Activity);
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
                LoadingOverlayUtils.OnStopLoadingAnimation(this.Activity);
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void OnSearchOutFocus(bool isSearchLayoutInRange)
        {
            try
            {
                if (!isSearchLayoutInRange)
                {
                    isSearchClose = true;
                }
                if (searchEditText != null)
                {
                    if (searchEditText.Visibility == ViewStates.Visible)
                    {
                        if (isSearchLayoutInRange)
                        {
                            HomeMenuUtils.SetIsQuery(false);
                            HomeMenuUtils.SetQueryWord("");

                            try
                            {
                                searchEditText.SetQuery("", false);
                            }
                            catch (System.Exception e)
                            {
                                Utility.LoggingNonFatalError(e);
                            }
                            this.presenter.SetQueryClose();
                        }
                        searchEditText.ClearFocus();
                        OnUpdateAccountListChanged(true);
                    }

                    if (string.IsNullOrEmpty(searchEditText.Query))
                    {
                        if (closeImageView != null)
                        {
                            closeImageView.Visibility = ViewStates.Visible;
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }

        public void OnSearchClearFocus()
        {
            try
            {
                if (searchEditText != null)
                {
                    if (searchEditText.Visibility == ViewStates.Visible)
                    {
                        searchEditText.ClearFocus();
                    }


                    if (string.IsNullOrEmpty(searchEditText.Query))
                    {
                        closeImageView.Visibility = ViewStates.Visible;
                    }
                }
            }
            catch (System.Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }

        public AccountsRecyclerViewAdapter GetAccountAdapter()
        {
            return accountsAdapter;
        }

        public void ShowRefreshScreen(bool isRefresh, string contentMsg, string buttonMsg)
        {
            accountListRefreshContainer.Visibility = ViewStates.Visible;
            accountListViewContainer.Visibility = ViewStates.Gone;

            isRefreshShown = true;

            if (!isRefresh)
            {
                isBCRMDown = true;
                btnRefresh.Visibility = ViewStates.Gone;
                if (this.presenter.GetIsMyServiceRefreshNeeded())
                {
                    SetMaintenanceLayoutParamsWithMyServiceHide();
                }
                else
                {
                    SetMaintenanceLayoutParams();
                }

                string refreshMaintenanceMsg = string.IsNullOrEmpty(contentMsg) ? Utility.GetLocalizedLabel("Error", "plannedDownTimeMessage") : contentMsg;
                if (Build.VERSION.SdkInt >= BuildVersionCodes.N)
                {
                    txtRefreshMsg.TextFormatted = Html.FromHtml(refreshMaintenanceMsg, FromHtmlOptions.ModeLegacy);
                }
                else
                {
                    txtRefreshMsg.TextFormatted = Html.FromHtml(refreshMaintenanceMsg);
                }
                discoverMoreContainer.Visibility = ViewStates.Gone;
            }
            else
            {
                isBCRMDown = false;
                btnRefresh.Visibility = ViewStates.Visible;
                if (this.presenter.GetIsMyServiceRefreshNeeded())
                {
                    SetRefreshLayoutParamsWithMyServiceHide();
                }
                else
                {
                    SetRefreshLayoutParams();
                }

                string refreshMsg = string.IsNullOrEmpty(contentMsg) ? GetLabelByLanguage("refreshMessage") : contentMsg;
                string refreshBtnTxt = string.IsNullOrEmpty(buttonMsg) ? GetLabelByLanguage("refreshBtnText") : buttonMsg;
                discoverMoreContainer.Visibility = ViewStates.Gone;
                btnRefresh.Text = refreshBtnTxt;
                if (Build.VERSION.SdkInt >= BuildVersionCodes.N)
                {
                    txtRefreshMsg.TextFormatted = Html.FromHtml(refreshMsg, FromHtmlOptions.ModeLegacy);
                }
                else
                {
                    txtRefreshMsg.TextFormatted = Html.FromHtml(refreshMsg);
                }
            }

            if (myServicesList.Count > 0)
            {
                myServiceAdapter = new MyServiceAdapter(myServicesList, this.Activity, isRefreshShown);
                myServiceListRecycleView.SetAdapter(myServiceAdapter);

                myServiceAdapter.ClickChanged += OnClickChanged;
                this.SetIsClicked(false);
            }
        }

        public void OnStartLoadAccount()
        {
            IsLoadMoreButtonVisible(false, false);

            IsMyServiceLoadMoreButtonVisible(false, false);

            IsRearrangeButtonVisible(false);

            if (!HomeMenuUtils.GetIsLoadedHomeMenu())
            {

                List<CustomerBillingAccount> eligibleSMRBillingAccounts = CustomerBillingAccount.EligibleSMRAccountList();
                List<CustomerBillingAccount> currentSMRBillingAccounts = CustomerBillingAccount.CurrentSMRAccountList();
                List<CustomerBillingAccount> smartmeterAccounts = CustomerBillingAccount.SMeterBudgetAccountList();        //smart meter ca
                List<CustomerBillingAccount> list = CustomerBillingAccount.GetSortedCustomerBillingAccounts();
                List<SMRAccount> eligibleSmrAccountList = new List<SMRAccount>();
                List<SMRAccount> currentSmrAccountList = new List<SMRAccount>();
                List<SMRAccount> SMeterAccountList = new List<SMRAccount>();

                if (eligibleSMRBillingAccounts.Count > 0)
                {
                    foreach (CustomerBillingAccount billingAccount in eligibleSMRBillingAccounts)
                    {
                        SMRAccount smrAccount = new SMRAccount();
                        smrAccount.accountNumber = billingAccount.AccNum;
                        smrAccount.accountName = billingAccount.AccDesc;
                        smrAccount.accountAddress = billingAccount.AccountStAddress;
                        smrAccount.accountSelected = false;
                        eligibleSmrAccountList.Add(smrAccount);
                    }
                }

                if (currentSMRBillingAccounts.Count > 0)
                {
                    foreach (CustomerBillingAccount billingAccount in currentSMRBillingAccounts)
                    {
                        SMRAccount smrAccount = new SMRAccount();
                        smrAccount.accountNumber = billingAccount.AccNum;
                        smrAccount.accountName = billingAccount.AccDesc;
                        smrAccount.accountAddress = billingAccount.AccountStAddress;
                        smrAccount.accountSelected = false;
                        currentSmrAccountList.Add(smrAccount);
                    }
                }

                if (smartmeterAccounts.Count > 0)
                {
                    foreach (CustomerBillingAccount billingAccount in smartmeterAccounts)
                    {
                        List<string> ebCAs = EBUtility.Instance.GetCAList();
                        if (ebCAs != null)
                        {
                            foreach (var ca in ebCAs)
                            {
                                if (ca.Equals(billingAccount.AccNum))
                                {
                                    SMRAccount smrAccount = new SMRAccount();
                                    smrAccount.accountNumber = billingAccount.AccNum;
                                    smrAccount.accountName = billingAccount.AccDesc;
                                    smrAccount.accountAddress = billingAccount.AccountStAddress;
                                    smrAccount.accountSelected = false;
                                    smrAccount.BudgetAmount = billingAccount.BudgetAmount;
                                    smrAccount.InstallationType = billingAccount.InstallationType;
                                    smrAccount.AMSIDCategory = billingAccount.AMSIDCategory;
                                    SMeterAccountList.Add(smrAccount);
                                }
                            }
                        }
                    }
                }

                if (list.Count > 0)
                {
                    accountListContainer.Visibility = ViewStates.Visible;
                    accountCard.Visibility = ViewStates.Gone;
                    addActionContainer.Visibility = ViewStates.Visible;
                    accountActionDivider.Visibility = ViewStates.Gone;
                    if (list.Count == 1)
                    {
                        SetBottmLayoutParams(21f);
                    }
                    else if (list.Count > 1 && list.Count <= 3)
                    {
                        SetBottmLayoutParams(13f);
                    }
                    else
                    {
                        SetBottmLayoutParams(5f);
                    }
                }
                else
                {
                    addActionContainer.Visibility = ViewStates.Gone;
                    accountListContainer.Visibility = ViewStates.Gone;
                    accountActionDivider.Visibility = ViewStates.Gone;
                    accountCard.Visibility = ViewStates.Visible;
                    SetBottmLayoutParams(21f);
                }

                if (MyTNBAccountManagement.GetInstance().IsSMROpenToTenantV2())
                {
                    List<CustomerBillingAccount> eligibleSMRBillingAccountsWithTenant = CustomerBillingAccount.EligibleSMRAccountListWithTenant();
                    List<CustomerBillingAccount> currentSMRBillingAccountsWithTenant = CustomerBillingAccount.CurrentSMRAccountListWithTenant();

                    if (eligibleSMRBillingAccountsWithTenant.Count > 0)
                    {
                        foreach (CustomerBillingAccount billingAccount in eligibleSMRBillingAccountsWithTenant)
                        {
                            SMRAccount smrAccount = new SMRAccount();
                            smrAccount.accountNumber = billingAccount.AccNum;
                            smrAccount.accountName = billingAccount.AccDesc;
                            smrAccount.accountAddress = billingAccount.AccountStAddress;
                            smrAccount.accountSelected = false;
                            eligibleSmrAccountList.Add(smrAccount);
                        }
                    }

                    if (currentSMRBillingAccountsWithTenant.Count > 0)
                    {
                        foreach (CustomerBillingAccount billingAccount in currentSMRBillingAccountsWithTenant)
                        {
                            SMRAccount smrAccount = new SMRAccount();
                            smrAccount.accountNumber = billingAccount.AccNum;
                            smrAccount.accountName = billingAccount.AccDesc;
                            smrAccount.accountAddress = billingAccount.AccountStAddress;
                            smrAccount.accountSelected = false;
                            currentSmrAccountList.Add(smrAccount);
                        }
                    }
                }

                List<SMRAccount> allSMRBlillingAccounts = new List<SMRAccount>();       //energy budget
                allSMRBlillingAccounts.AddRange(SMeterAccountList);

                UserSessions.SetSMRAccountList(currentSmrAccountList);
                UserSessions.SetSMREligibilityAccountList(eligibleSmrAccountList);
                UserSessions.EnergyBudget(allSMRBlillingAccounts);

                accountListRefreshContainer.Visibility = ViewStates.Gone;
                accountListViewContainer.Visibility = ViewStates.Visible;
                if (MyTNBAccountManagement.GetInstance().IsNeedUpdatedBillingDetails())
                {
                    MyTNBAccountManagement.GetInstance().SetSMRStatusCheckOwnerCanApply(false);
                    UserSessions.SetRealSMREligibilityAccountList(eligibleSmrAccountList);
                }
                searchEditText.SetQuery("", false);
                OnLoadAccount();

                FilterCOMCLandNEM();

                SetBottomLayoutBackground(false);
                this.presenter.InitiateService();
            }
            else
            {
                List<CustomerBillingAccount> eligibleSMRBillingAccounts = CustomerBillingAccount.EligibleSMRAccountList();
                List<CustomerBillingAccount> currentSMRBillingAccounts = CustomerBillingAccount.CurrentSMRAccountList();
                List<CustomerBillingAccount> smartmeterAccounts = CustomerBillingAccount.SMeterBudgetAccountList();        //smart meter ca
                List<CustomerBillingAccount> list = CustomerBillingAccount.GetSortedCustomerBillingAccounts();
                List<SMRAccount> eligibleSmrAccountList = new List<SMRAccount>();
                List<SMRAccount> currentSmrAccountList = new List<SMRAccount>();
                List<SMRAccount> SMeterAccountList = new List<SMRAccount>();
                if (eligibleSMRBillingAccounts.Count > 0)
                {
                    foreach (CustomerBillingAccount billingAccount in eligibleSMRBillingAccounts)
                    {
                        SMRAccount smrAccount = new SMRAccount();
                        smrAccount.accountNumber = billingAccount.AccNum;
                        smrAccount.accountName = billingAccount.AccDesc;
                        smrAccount.accountAddress = billingAccount.AccountStAddress;
                        smrAccount.accountSelected = false;
                        eligibleSmrAccountList.Add(smrAccount);
                    }
                }

                if (currentSMRBillingAccounts.Count > 0)
                {
                    foreach (CustomerBillingAccount billingAccount in currentSMRBillingAccounts)
                    {
                        SMRAccount smrAccount = new SMRAccount();
                        smrAccount.accountNumber = billingAccount.AccNum;
                        smrAccount.accountName = billingAccount.AccDesc;
                        smrAccount.accountAddress = billingAccount.AccountStAddress;
                        smrAccount.accountSelected = false;
                        currentSmrAccountList.Add(smrAccount);
                    }
                }

                if (smartmeterAccounts.Count > 0)
                {
                    foreach (CustomerBillingAccount billingAccount in smartmeterAccounts)
                    {
                        List<string> ebCAs = EBUtility.Instance.GetCAList();
                        if (ebCAs != null)
                        {
                            foreach (var ca in ebCAs)
                            {
                                if (ca.Equals(billingAccount.AccNum))
                                {
                                    SMRAccount smrAccount = new SMRAccount();
                                    smrAccount.accountNumber = billingAccount.AccNum;
                                    smrAccount.accountName = billingAccount.AccDesc;
                                    smrAccount.accountAddress = billingAccount.AccountStAddress;
                                    smrAccount.accountSelected = false;
                                    smrAccount.BudgetAmount = billingAccount.BudgetAmount;
                                    smrAccount.InstallationType = billingAccount.InstallationType;
                                    smrAccount.AMSIDCategory = billingAccount.AMSIDCategory;
                                    SMeterAccountList.Add(smrAccount);
                                }
                            }
                        }
                    }
                }

                if (list.Count > 0)
                {
                    accountListContainer.Visibility = ViewStates.Visible;
                    accountCard.Visibility = ViewStates.Gone;
                    addActionContainer.Visibility = ViewStates.Visible;
                    accountActionDivider.Visibility = ViewStates.Gone;
                    if (list.Count == 1)
                    {
                        SetBottmLayoutParams(21f);
                    }
                    else if (list.Count > 1 && list.Count <= 3)
                    {
                        SetBottmLayoutParams(13f);
                    }
                    else
                    {
                        SetBottmLayoutParams(5f);
                    }
                }
                else
                {
                    addActionContainer.Visibility = ViewStates.Gone;
                    accountListContainer.Visibility = ViewStates.Gone;
                    accountActionDivider.Visibility = ViewStates.Gone;
                    accountCard.Visibility = ViewStates.Visible;
                    SetBottmLayoutParams(21f);
                }

                if (MyTNBAccountManagement.GetInstance().IsSMROpenToTenantV2())
                {
                    List<CustomerBillingAccount> eligibleSMRBillingAccountsWithTenant = CustomerBillingAccount.EligibleSMRAccountListWithTenant();
                    List<CustomerBillingAccount> currentSMRBillingAccountsWithTenant = CustomerBillingAccount.CurrentSMRAccountListWithTenant();

                    if (eligibleSMRBillingAccountsWithTenant.Count > 0)
                    {
                        foreach (CustomerBillingAccount billingAccount in eligibleSMRBillingAccountsWithTenant)
                        {
                            SMRAccount smrAccount = new SMRAccount();
                            smrAccount.accountNumber = billingAccount.AccNum;
                            smrAccount.accountName = billingAccount.AccDesc;
                            smrAccount.accountAddress = billingAccount.AccountStAddress;
                            smrAccount.accountSelected = false;
                            eligibleSmrAccountList.Add(smrAccount);
                        }
                    }

                    if (currentSMRBillingAccountsWithTenant.Count > 0)
                    {
                        foreach (CustomerBillingAccount billingAccount in currentSMRBillingAccountsWithTenant)
                        {
                            SMRAccount smrAccount = new SMRAccount();
                            smrAccount.accountNumber = billingAccount.AccNum;
                            smrAccount.accountName = billingAccount.AccDesc;
                            smrAccount.accountAddress = billingAccount.AccountStAddress;
                            smrAccount.accountSelected = false;
                            currentSmrAccountList.Add(smrAccount);
                        }
                    }
                }

                List<SMRAccount> allSMRBlillingAccounts = new List<SMRAccount>();                   //energy budget
                allSMRBlillingAccounts.AddRange(SMeterAccountList);

                UserSessions.SetSMRAccountList(currentSmrAccountList);
                UserSessions.SetSMREligibilityAccountList(eligibleSmrAccountList);
                UserSessions.EnergyBudget(allSMRBlillingAccounts);                                        //energy budget

                accountListRefreshContainer.Visibility = ViewStates.Gone;
                accountListViewContainer.Visibility = ViewStates.Visible;
                if (MyTNBAccountManagement.GetInstance().IsNeedUpdatedBillingDetails())
                {
                    MyTNBAccountManagement.GetInstance().SetSMRStatusCheckOwnerCanApply(false);
                    UserSessions.SetRealSMREligibilityAccountList(eligibleSmrAccountList);
                }

                FilterCOMCLandNEM();

                if (HomeMenuUtils.GetIsQuery())
                {
                    accountHeaderTitle.Visibility = ViewStates.Gone;
                    searchEditText.Visibility = ViewStates.Visible;
                    searchEditText.SetMaxWidth(Integer.MaxValue);
                    searchActionContainer.Visibility = ViewStates.Gone;
                    accountHeaderTitle.Visibility = ViewStates.Gone;
                    addActionContainer.Visibility = ViewStates.Gone;
                    accountActionDivider.Visibility = ViewStates.Gone;
                    searchEditText.OnActionViewExpanded();
                    isInitiate = false;
                    searchEditText.SetQuery(HomeMenuUtils.GetQueryWord(), false);
                    try
                    {
                        if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
                        {
                            searchEditText.SetBackgroundResource(Resource.Drawable.search_edit_bg);
                        }
                        else
                        {
                            searchEditText.SetBackgroundDrawable(null);
                        }
                    }
                    catch (System.Exception cex)
                    {
                        Utility.LoggingNonFatalError(cex);
                    }
                    if (closeImageView != null)
                    {
                        closeImageView.Visibility = ViewStates.Visible;
                    }
                }
                else
                {
                    this.presenter.RestoreCurrentAccountState();
                }

                this.presenter.ReadNewFAQFromCache();
            }
        }

        public void UpdateEligibilitySMRAccountList()
        {
            List<CustomerBillingAccount> eligibleSMRBillingAccounts = CustomerBillingAccount.EligibleSMRAccountList();

            if (MyTNBAccountManagement.GetInstance().IsSMROpenToTenantV2())
            {
                List<CustomerBillingAccount> eligibleSMRBillingAccountsWithTenant = CustomerBillingAccount.EligibleSMRAccountListWithTenant();
                if (eligibleSMRBillingAccountsWithTenant != null && eligibleSMRBillingAccountsWithTenant.Count > 0)
                {
                    eligibleSMRBillingAccounts = eligibleSMRBillingAccounts
                                            .Concat(eligibleSMRBillingAccountsWithTenant)
                                            .GroupBy(account => account.AccNum)
                                            .Select(group => group.First())
                                            .ToList();
                }
            }

            List<SMRAccount> eligibleSmrAccountList = new List<SMRAccount>();
            if (eligibleSMRBillingAccounts.Count > 0)
            {
                foreach (CustomerBillingAccount billingAccount in eligibleSMRBillingAccounts)
                {
                    SMRAccount smrAccount = new SMRAccount();
                    smrAccount.accountNumber = billingAccount.AccNum;
                    smrAccount.accountName = billingAccount.AccDesc;
                    smrAccount.accountAddress = billingAccount.AccountStAddress;
                    smrAccount.accountSelected = false;
                    eligibleSmrAccountList.Add(smrAccount);
                }
            }
            UserSessions.SetSMREligibilityAccountList(eligibleSmrAccountList);
            UserSessions.SetRealSMREligibilityAccountList(eligibleSmrAccountList);
        }

        public void UpdateCurrentSMRAccountList()
        {
            List<CustomerBillingAccount> currentSMRBillingAccounts = CustomerBillingAccount.CurrentSMRAccountList();

            if (MyTNBAccountManagement.GetInstance().IsSMROpenToTenantV2())
            {
                List<CustomerBillingAccount> currentSMRBillingAccountsWithTenant = CustomerBillingAccount.CurrentSMRAccountListWithTenant();
                if (currentSMRBillingAccountsWithTenant != null && currentSMRBillingAccountsWithTenant.Count > 0)
                {
                    currentSMRBillingAccounts = currentSMRBillingAccounts
                                            .Concat(currentSMRBillingAccountsWithTenant)
                                            .GroupBy(account => account.AccNum)
                                            .Select(group => group.First())
                                            .ToList();
                }
            }

            List<SMRAccount> currentSmrAccountList = new List<SMRAccount>();
            if (currentSMRBillingAccounts.Count > 0)
            {
                foreach (CustomerBillingAccount billingAccount in currentSMRBillingAccounts)
                {
                    SMRAccount smrAccount = new SMRAccount();
                    smrAccount.accountNumber = billingAccount.AccNum;
                    smrAccount.accountName = billingAccount.AccDesc;
                    smrAccount.accountAddress = billingAccount.AccountStAddress;
                    smrAccount.accountSelected = false;
                    currentSmrAccountList.Add(smrAccount);
                }
            }
            UserSessions.SetSMRAccountList(currentSmrAccountList);
        }

        // On Press Refresh button action
        [OnClick(Resource.Id.btnRefresh)]
        internal void OnRefresh(object sender, EventArgs e)
        {
            HomeMenuUtils.ResetAll();

            bottomContainer.Visibility = ViewStates.Visible;
            myServiceContainer.Visibility = ViewStates.Visible;
            myServiceHideView.Visibility = ViewStates.Gone;
            myServiceRefreshContainer.Visibility = ViewStates.Gone;
            containerQuickAction.Visibility = ViewStates.Gone;

            isRefreshShown = false;

            IsLoadMoreButtonVisible(false, false);

            IsMyServiceLoadMoreButtonVisible(false, false);

            IsRearrangeButtonVisible(false);

            SetBottomLayoutBackground(false);

            ShowSearchAction(false);

            List<CustomerBillingAccount> eligibleSMRBillingAccounts = CustomerBillingAccount.EligibleSMRAccountList();
            List<CustomerBillingAccount> currentSMRBillingAccounts = CustomerBillingAccount.CurrentSMRAccountList();
            List<CustomerBillingAccount> list = CustomerBillingAccount.GetSortedCustomerBillingAccounts();
            List<CustomerBillingAccount> smartmeterAccounts = CustomerBillingAccount.SMeterBudgetAccountList();        //smart meter ca
            List<SMRAccount> eligibleSmrAccountList = new List<SMRAccount>();
            List<SMRAccount> currentSmrAccountList = new List<SMRAccount>();
            List<SMRAccount> SMeterAccountList = new List<SMRAccount>();
            if (eligibleSMRBillingAccounts.Count > 0)
            {
                foreach (CustomerBillingAccount billingAccount in eligibleSMRBillingAccounts)
                {
                    SMRAccount smrAccount = new SMRAccount();
                    smrAccount.accountNumber = billingAccount.AccNum;
                    smrAccount.accountName = billingAccount.AccDesc;
                    smrAccount.accountAddress = billingAccount.AccountStAddress;
                    smrAccount.accountSelected = false;
                    eligibleSmrAccountList.Add(smrAccount);
                }
            }

            if (currentSMRBillingAccounts.Count > 0)
            {
                foreach (CustomerBillingAccount billingAccount in currentSMRBillingAccounts)
                {
                    SMRAccount smrAccount = new SMRAccount();
                    smrAccount.accountNumber = billingAccount.AccNum;
                    smrAccount.accountName = billingAccount.AccDesc;
                    smrAccount.accountAddress = billingAccount.AccountStAddress;
                    smrAccount.accountSelected = false;
                    currentSmrAccountList.Add(smrAccount);
                }
            }

            if (smartmeterAccounts.Count > 0)
            {
                foreach (CustomerBillingAccount billingAccount in smartmeterAccounts)
                {
                    List<string> ebCAs = EBUtility.Instance.GetCAList();
                    if (ebCAs != null)
                    {
                        foreach (var ca in ebCAs)
                        {
                            if (ca.Equals(billingAccount.AccNum))
                            {
                                SMRAccount smrAccount = new SMRAccount();
                                smrAccount.accountNumber = billingAccount.AccNum;
                                smrAccount.accountName = billingAccount.AccDesc;
                                smrAccount.accountAddress = billingAccount.AccountStAddress;
                                smrAccount.accountSelected = false;
                                smrAccount.BudgetAmount = billingAccount.BudgetAmount;
                                smrAccount.InstallationType = billingAccount.InstallationType;
                                smrAccount.AMSIDCategory = billingAccount.AMSIDCategory;
                                SMeterAccountList.Add(smrAccount);
                            }
                        }
                    }
                }
            }

            if (MyTNBAccountManagement.GetInstance().IsSMROpenToTenantV2())
            {
                List<CustomerBillingAccount> eligibleSMRBillingAccountsWithTenant = CustomerBillingAccount.EligibleSMRAccountListWithTenant();
                List<CustomerBillingAccount> currentSMRBillingAccountsWithTenant = CustomerBillingAccount.CurrentSMRAccountListWithTenant();

                if (eligibleSMRBillingAccountsWithTenant.Count > 0)
                {
                    foreach (CustomerBillingAccount billingAccount in eligibleSMRBillingAccountsWithTenant)
                    {
                        SMRAccount smrAccount = new SMRAccount();
                        smrAccount.accountNumber = billingAccount.AccNum;
                        smrAccount.accountName = billingAccount.AccDesc;
                        smrAccount.accountAddress = billingAccount.AccountStAddress;
                        smrAccount.accountSelected = false;
                        eligibleSmrAccountList.Add(smrAccount);
                    }
                }

                if (currentSMRBillingAccountsWithTenant.Count > 0)
                {
                    foreach (CustomerBillingAccount billingAccount in currentSMRBillingAccountsWithTenant)
                    {
                        SMRAccount smrAccount = new SMRAccount();
                        smrAccount.accountNumber = billingAccount.AccNum;
                        smrAccount.accountName = billingAccount.AccDesc;
                        smrAccount.accountAddress = billingAccount.AccountStAddress;
                        smrAccount.accountSelected = false;
                        currentSmrAccountList.Add(smrAccount);
                    }
                }
            }

            if (list.Count > 0)
            {
                accountListContainer.Visibility = ViewStates.Visible;
                accountCard.Visibility = ViewStates.Gone;
                addActionContainer.Visibility = ViewStates.Visible;
                accountActionDivider.Visibility = ViewStates.Gone;
                if (list.Count == 1)
                {
                    SetBottmLayoutParams(21f);
                }
                else if (list.Count > 1 && list.Count <= 3)
                {
                    SetBottmLayoutParams(13f);
                }
                else
                {
                    SetBottmLayoutParams(5f);
                }
            }
            else
            {
                addActionContainer.Visibility = ViewStates.Gone;
                accountListContainer.Visibility = ViewStates.Gone;
                accountActionDivider.Visibility = ViewStates.Gone;
                accountCard.Visibility = ViewStates.Visible;
                SetBottmLayoutParams(21f);
            }
            List<SMRAccount> allSMRBlillingAccounts = new List<SMRAccount>();       //energy budget
            allSMRBlillingAccounts.AddRange(SMeterAccountList);

            UserSessions.SetSMRAccountList(currentSmrAccountList);
            UserSessions.SetSMREligibilityAccountList(eligibleSmrAccountList);
            UserSessions.EnergyBudget(allSMRBlillingAccounts);                  //energy budget

            accountListRefreshContainer.Visibility = ViewStates.Gone;
            accountListViewContainer.Visibility = ViewStates.Visible;
            if (MyTNBAccountManagement.GetInstance().IsNeedUpdatedBillingDetails())
            {
                MyTNBAccountManagement.GetInstance().SetSMRStatusCheckOwnerCanApply(false);
                UserSessions.SetRealSMREligibilityAccountList(eligibleSmrAccountList);
            }

            FilterCOMCLandNEM();

            searchEditText.SetQuery("", false);

            this.presenter.RefreshAccountSummary();

            myServicesList = new List<MyServiceModel>();

            this.presenter.InitiateMyServiceRefresh();

            if (newFAQTitle.Visibility == ViewStates.Gone)
            {
                this.presenter.OnCheckNewFAQState();
            }
        }

        [OnClick(Resource.Id.btnMyServiceRefresh)]
        internal void OnMyServiceRefresh(object sender, EventArgs e)
        {
            if (this.presenter.GetIsAccountRefreshNeeded())
            {
                if (isBCRMDown)
                {
                    SetMaintenanceLayoutParams();
                }
                else
                {
                    SetRefreshLayoutParams();
                }
            }

            HomeMenuUtils.ResetMyService();

            myServiceContainer.Visibility = ViewStates.Visible;
            myServiceHideView.Visibility = ViewStates.Gone;
            myServiceRefreshContainer.Visibility = ViewStates.Gone;
            containerQuickAction.Visibility = ViewStates.Gone;

            IsMyServiceLoadMoreButtonVisible(false, false);

            SetBottomLayoutBackground(false);

            myServicesList = new List<MyServiceModel>();

            this.presenter.InitiateMyServiceRefresh();
        }

        [OnClick(Resource.Id.refreshMsg)]
        internal void OnRefreshMsgClick(object sender, EventArgs e)
        {
            if (isBCRMDown)
            {
                string textMessage = txtRefreshMsg.Text;
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
        }

        [OnClick(Resource.Id.accountCard)]
        internal void OnAddAccountCardClick(object sender, EventArgs e)
        {
            if (!this.GetIsClicked())
            {
                this.SetIsClicked(true);

                Intent linkAccount = new Intent(this.Activity, typeof(MyAccountActivity));
                linkAccount.PutExtra("fromDashboard", true);
                StartActivity(linkAccount);
            }
        }

        [OnClick(Resource.Id.discoverMoreContainer)]
        internal void OndiscoverMoreCardView(object sender, EventArgs e)
        {
            if (!this.GetIsClicked())
            {
                this.SetIsClicked(true);
                try
                {
                    FirebaseAnalyticsUtils.LogFragmentClickEvent(this, "Home Screen -> Energy Budget Screen Popup");
                    Intent EBPopupPage = new Intent(this.Activity, typeof(EBPopupScreenActivity));
                    EBPopupPage.PutExtra("fromDashboard", true);
                    StartActivityForResult(EBPopupPage, SELECT_SM_POPUP_REQUEST_CODE);
                }
                catch (System.Exception err)
                {
                    Utility.LoggingNonFatalError(err);
                }
            }
        }

        [OnClick(Resource.Id.discoverMoreSDContainer)]
        internal void OndiscoverMoreSDCardView(object sender, EventArgs e)
        {
            if (!this.GetIsClicked())
            {
                this.SetIsClicked(true);
                try
                {
                    //FirebaseAnalyticsUtils.LogFragmentClickEvent(this, "Home Screen -> SD Screen Popup");
                    Intent SDDiscoverCom = new Intent(this.Activity, typeof(ServiceDisruptionActivity));
                    SDDiscoverCom.PutExtra("fromDashboard", true);
                    StartActivityForResult(SDDiscoverCom, SELECT_SD_POPUP_REQUEST_CODE);
                }
                catch (System.Exception err)
                {
                    Utility.LoggingNonFatalError(err);
                }
            }
        }

        public void OnSavedEnergySavingTipsTimeStamp(string mSavedTimeStamp)
        {
            if (mSavedTimeStamp != null)
            {
                this.savedEnergySavingTipsTimeStamp = mSavedTimeStamp;
            }
            this.presenter.OnGetEnergySavingTipsTimeStamp();
        }

        public void CheckEnergySavingTipsTimeStamp()
        {
            try
            {
                EnergySavingTipsParentEntity wtManager = new EnergySavingTipsParentEntity();
                List<EnergySavingTipsParentEntity> items = wtManager.GetAllItems();
                if (items != null)
                {
                    EnergySavingTipsParentEntity entity = items[0];
                    if (entity != null)
                    {
                        if (!entity.Timestamp.Equals(savedEnergySavingTipsTimeStamp))
                        {
                            this.presenter.OnGetEnergySavingTips();
                        }
                        else
                        {
                            this.presenter.OnSetEnergySavingTipsToCache();
                        }

                    }
                    else
                    {
                        this.presenter.OnSetEnergySavingTipsToCache();
                    }
                }
                else
                {
                    this.presenter.OnSetEnergySavingTipsToCache();
                }
            }
            catch (System.Exception e)
            {
                // Read from cache
                Utility.LoggingNonFatalError(e);
            }
        }

        public void UpdateQueryListing(string searchText)
        {
            if (string.IsNullOrEmpty(searchText))
            {
                if (closeImageView != null)
                {
                    closeImageView.Visibility = ViewStates.Visible;
                }
            }

            if (!isInitiate && HomeMenuUtils.GetIsQuery())
            {
                isInitiate = true;
                this.presenter.RestoreQueryAccounts();
            }
            else
            {
                this.presenter.LoadQueryAccounts(searchText);
            }
        }

        void ViewTreeObserver.IOnGlobalLayoutListener.OnGlobalLayout()
        {
            try
            {
                if (isFirstInitiate)
                {
                    isFirstInitiate = false;
                    StopScrolling();
                }

                if (searchEditText != null)
                {
                    if (string.IsNullOrEmpty(searchEditText.Query))
                    {
                        if (closeImageView != null)
                        {
                            closeImageView.Visibility = ViewStates.Visible;
                        }
                    }
                }
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public LinearLayout GetSearchLayout()
        {
            return searchActionContainer;
        }

        public void IsLoadMoreButtonVisible(bool isVisible, bool isRotate)
        {
            if (isVisible)
            {
                loadMoreContainer.Visibility = ViewStates.Visible;
                if (isRotate)
                {
                    if (!isAlreadyRotated)
                    {
                        isAlreadyRotated = true;
                        AnimationSet animSet = new AnimationSet(true);
                        animSet.Interpolator = new DecelerateInterpolator();
                        animSet.FillAfter = true;
                        animSet.FillEnabled = true;

                        RotateAnimation animRotate = new RotateAnimation(0.0f, -180.0f,
                            Dimension.RelativeToSelf, 0.5f,
                            Dimension.RelativeToSelf, 0.5f);

                        animRotate.Duration = 500;
                        animRotate.FillAfter = true;
                        animSet.AddAnimation(animRotate);

                        loadMoreImg.StartAnimation(animSet);

                        loadMoreLabel.Text = GetLabelByLanguage("showLess");
                    }

                    IsRearrangeButtonVisible(true);
                }
                else
                {
                    if (isAlreadyRotated)
                    {
                        isAlreadyRotated = false;
                        AnimationSet animSet = new AnimationSet(true);
                        animSet.Interpolator = new DecelerateInterpolator();
                        animSet.FillAfter = true;
                        animSet.FillEnabled = true;

                        RotateAnimation animRotate = new RotateAnimation(-180.0f, 0,
                            Dimension.RelativeToSelf, 0.5f,
                            Dimension.RelativeToSelf, 0.5f);

                        animRotate.Duration = 500;
                        animRotate.FillAfter = true;
                        animSet.AddAnimation(animRotate);

                        loadMoreImg.StartAnimation(animSet);

                        loadMoreLabel.Text = GetLabelByLanguage("moreAccts");

                    }

                    IsRearrangeButtonVisible(false);
                }
            }
            else
            {
                loadMoreContainer.Visibility = ViewStates.Gone;

                IsRearrangeButtonVisible(false);
            }
        }

        public void IsMyServiceLoadMoreButtonVisible(bool isVisible, bool isRotate)
        {
            try
            {
                Activity.RunOnUiThread(() =>
                {
                    try
                    {
                        myServiceLoadMoreContainer.Visibility = ViewStates.Gone;
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

        public void IsRearrangeButtonVisible(bool isVisible)
        {
            if (isVisible)
            {
                rearrangeContainer.Visibility = ViewStates.Visible;
                rearrangeLine.Visibility = ViewStates.Visible;
            }
            else
            {
                rearrangeContainer.Visibility = ViewStates.Gone;
                rearrangeLine.Visibility = ViewStates.Gone;
            }
        }

        [OnClick(Resource.Id.loadMoreContainer)]
        internal void OnLoadMorelick(object sender, EventArgs e)
        {
            this.presenter.DoLoadMoreAccount();
        }

        [OnClick(Resource.Id.rearrangeContainer)]
        internal void OnRearrangeClick(object sender, EventArgs e)
        {
            if (!this.GetIsClicked())
            {
                this.SetIsClicked(true);
                Intent rearrangeAccount = new Intent(this.Activity, typeof(RearrangeAccountActivity));
                StartActivityForResult(rearrangeAccount, REARRANGE_ACTIVITY_CODE);
            }
        }

        void View.IOnFocusChangeListener.OnFocusChange(View v, bool hasFocus)
        {
            if (string.IsNullOrEmpty(searchEditText.Query))
            {
                if (closeImageView != null)
                {
                    closeImageView.Visibility = ViewStates.Visible;
                }
            }
        }

        public void SetBottomLayoutBackground(bool isMyServiceExpand)
        {
            try
            {
                Activity.RunOnUiThread(() =>
                {
                    try
                    {
                        bottomContainer.SetBackgroundResource(Resource.Drawable.dashboard_botton_sheet_bg_expanded);
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

        private Snackbar mLoadBillSnackBar;

        public void ShowBillErrorSnackBar()
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
                this.SetIsClicked(false);
                Utility.LoggingNonFatalError(e);
            }
        }

        public void ShowBillPDF(AccountData selectedAccountData, GetBillHistoryResponse.ResponseData selectedBill = null)
        {
            if (selectedBill != null && selectedBill.NrBill != null)
            {
                selectedBill.NrBill = null;
            }

            Intent viewBill = new Intent(this.Activity, typeof(ViewBillActivity));
            viewBill.PutExtra(Constants.SELECTED_ACCOUNT, JsonConvert.SerializeObject(selectedAccountData));
            viewBill.PutExtra(Constants.SELECTED_BILL, JsonConvert.SerializeObject(selectedBill));
            StartActivity(viewBill);
        }

        public void ShowNotificationCount(int count)
        {
            try
            {
                SetNotificationIndicator();

                if (count <= 0)
                {
                    ME.Leolin.Shortcutbadger.ShortcutBadger.RemoveCount(this.Activity);
                }
                else
                {
                    ME.Leolin.Shortcutbadger.ShortcutBadger.ApplyCount(this.Activity, count);
                }
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public override string GetPageId()
        {
            return PAGE_ID;
        }

        public void OnShowHomeMenuFragmentTutorialDialog()
        {
            try
            {

                Activity.RunOnUiThread(() =>
                {
                    try
                    {
                        Handler h = new Handler();
                        Action myAction = () =>
                        {
                            try
                            {
                                Activity.RunOnUiThread(() =>
                                {
                                    try
                                    {
                                        StopScrolling();
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
                            NewAppTutorialUtils.ForceCloseNewAppTutorial();


                            NewAppTutorialUtils.OnShowNewAppTutorial(this.Activity, this, PreferenceManager.GetDefaultSharedPreferences(this.Activity), this.presenter.OnGeneraNewAppTutorialList());
                        };
                        h.PostDelayed(myAction, 50);
                    }
                    catch (System.Exception exep)
                    {
                        Utility.LoggingNonFatalError(exep);
                    }
                });
            }
            catch (System.Exception exe)
            {
                Utility.LoggingNonFatalError(exe);
            }
        }

        public void HomeMenuCustomScrolling(int yPosition)
        {
            try
            {
                Activity.RunOnUiThread(() =>
                {
                    try
                    {
                        summaryNestScrollView.ScrollTo(0, yPosition);
                        summaryNestScrollView.RequestLayout();
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

        public bool IsMyServiceLoadMoreVisible()
        {
            return myServiceLoadMoreContainer.Visibility == ViewStates.Visible;
        }

        public bool CheckIsScrollable()
        {
            View child = (View)summaryNestScrollView.GetChildAt(0);
            return summaryNestScrollView.Height < child.Height + summaryNestScrollView.PaddingTop + summaryNestScrollView.PaddingBottom;
        }

        public int GetMyServiceContainerHeight()
        {
            return myServiceContainer.Height;
        }

        public int GetMyServiceItemHeight()
        {
            var servicesList = myServicesList;

            int a = (int)System.Math.Ceiling((double)servicesList.Count / 3);
            var itemHeight = myServiceListRecycleView.Height / a;

            return itemHeight;
        }

        public int GetMyServiceItemWidth()
        {
            var itemWidth = myServiceListRecycleView.Width / 3;
            return itemWidth;
        }

        public int GetMyServiceItemTopPosition(string id)
        {
            var topPosition = 0;
            var servicesList = myServicesList;
            int itemPosition = servicesList.FindIndex(x => x.ServiceId == id) + 1;

            int row = (int)System.Math.Ceiling((double)itemPosition / 3);
            int rowIndex = row - 1;
            topPosition = rowIndex * GetMyServiceItemHeight();

            return topPosition;
        }

        public int GetMyServiceItemLeftPosition(string id)
        {
            var leftPosition = 0;

            int column = 0;

            var servicesList = myServicesList;
            int itemIndex = servicesList.FindIndex(x => x.ServiceId == id);

            column = (itemIndex % 3);
            leftPosition = column * GetMyServiceItemWidth();

            return leftPosition;
        }

        public int GetMyServiceRecycleViewYLocation()
        {
            int i = 0;

            try
            {
                int[] location = new int[2];
                myServiceListRecycleView.GetLocationOnScreen(location);
                i = location[1];
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

            return i;
        }

        public int GetAccountContainerHeight()
        {
            return accountContainer.Height;
        }
        public int GettopRootViewHeight()
        {
            return topRootView.Height;
        }
        public int GetnewFAQContainerHeight()
        {
            return newFAQContainer.Height;
        }
        public int GetnewFAQTitleHeight()
        {
            return newFAQTitle.Height;
        }

        public int GetloadMoreContainerHeight()
        {
            return loadMoreContainer.Height;
        }
        public int GetaccountCardHeight()
        {
            return accountCard.Height;
        }
        public int GetDiscovercontainerHeight()
        {
            return discovercontainer.Height;
        }

        public void ResetNewFAQScroll()
        {
            try
            {
                Activity.RunOnUiThread(() =>
                {
                    try
                    {
                        LinearLayoutManager layoutManager = newFAQListRecycleView.GetLayoutManager() as LinearLayoutManager;
                        layoutManager.ScrollToPositionWithOffset(0, 0);
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

        public int CheckNewFaqList()
        {
            int count = 0;

            try
            {
                count = newFAQAdapter.ItemCount;
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

            return count;
        }

        public int CheckMyServiceList()
        {
            int count = 0;

            try
            {
                count = myServiceAdapter.ItemCount;
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

            return count;
        }

        public int OnGetEndOfScrollView()
        {
            View child = (View)summaryNestScrollView.GetChildAt(0);

            return child.Height + summaryNestScrollView.PaddingTop + summaryNestScrollView.PaddingBottom;
        }

        public void StopScrolling()
        {
            try
            {
                summaryNestScrollView.SmoothScrollBy(0, 0);
                summaryNestScrollView.ScrollTo(0, 0);
                summaryNestScrollView.RequestLayout();
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void OnSetNotificationNewLabel(bool flag, int count)
        {
            try
            {
                this.Activity.RunOnUiThread(() =>
                {
                    try
                    {
                        OnSetupNotificationNewLabel(flag, count);
                    }
                    catch (System.Exception ex)
                    {
                        Utility.LoggingNonFatalError(ex);
                    }
                });
            }
            catch (System.Exception e)
            {
                OnSetupNotificationNewLabel(flag, count);
                Utility.LoggingNonFatalError(e);
            }
        }

        private void OnSetupNotificationNewLabel(bool flag, int count)
        {
            try
            {
                if (flag && count > 0)
                {
                    newLabel.Visibility = ViewStates.Visible;
                    txtNewLabel.Text = count.ToString();
                    notificationHeaderIcon.SetImageResource(Resource.Drawable.ic_header_notification_unread);
                    if (count > 0 && count <= 9)
                    {
                        RelativeLayout.LayoutParams notificationHeaderIconParam = notificationHeaderIcon.LayoutParameters as RelativeLayout.LayoutParams;
                        notificationHeaderIconParam.LeftMargin = (int)DPUtils.ConvertDPToPx(-16f);
                        RelativeLayout.LayoutParams newLabelParam = newLabel.LayoutParameters as RelativeLayout.LayoutParams;
                        newLabelParam.Width = (int)DPUtils.ConvertDPToPx(14f);
                        newLabelParam.TopMargin = (int)DPUtils.ConvertDPToPx(2f);
                    }
                    else
                    {
                        if (count > 99)
                        {
                            txtNewLabel.Text = "99+";
                        }
                        RelativeLayout.LayoutParams notificationHeaderIconParam = notificationHeaderIcon.LayoutParameters as RelativeLayout.LayoutParams;
                        notificationHeaderIconParam.LeftMargin = (int)DPUtils.ConvertDPToPx(-10f);
                        RelativeLayout.LayoutParams newLabelParam = newLabel.LayoutParameters as RelativeLayout.LayoutParams;
                        newLabelParam.Width = (int)DPUtils.ConvertDPToPx(18f);
                        newLabelParam.TopMargin = (int)DPUtils.ConvertDPToPx(2f);
                    }
                }
                else
                {
                    newLabel.Visibility = ViewStates.Gone;
                    notificationHeaderIcon.SetImageResource(Resource.Drawable.ic_header_notification);
                    RelativeLayout.LayoutParams notificationHeaderIconParam = notificationHeaderIcon.LayoutParameters as RelativeLayout.LayoutParams;
                    notificationHeaderIconParam.LeftMargin = 0;
                }
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void RestartHomeMenu()
        {
            try
            {
                ((DashboardHomeActivity)Activity).ShowHomeDashBoard();
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void CheckSearchEditAction()
        {
            if (searchEditText.Visibility == ViewStates.Visible)
            {
                ShowSearchAction(false);
            }
        }

        private Snackbar mSomethingWrongExceptionSnackBar;
        public void ShowSomethingWrongException()
        {
            if (mSomethingWrongExceptionSnackBar != null && mSomethingWrongExceptionSnackBar.IsShown)
            {
                mSomethingWrongExceptionSnackBar.Dismiss();

            }

            string msg = Utility.GetLocalizedErrorLabel("defaultErrorMessage");

            mSomethingWrongExceptionSnackBar = Snackbar.Make(rootView, msg, Snackbar.LengthIndefinite)
            .SetAction(Utility.GetLocalizedCommonLabel("ok"), delegate
            {
                mSomethingWrongExceptionSnackBar.Dismiss();
            }
            );
            View v = mSomethingWrongExceptionSnackBar.View;
            TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
            tv.SetMaxLines(5);
            mSomethingWrongExceptionSnackBar.Show();
        }

        public void SetMyServiceHideView()
        {
            try
            {
                Activity.RunOnUiThread(() =>
                {
                    try
                    {
                        int mHeight = 0;
                        mHeight = myServiceContainer.Height;
                        LinearLayout.LayoutParams myServiceHideLayout = myServiceHideView.LayoutParameters as LinearLayout.LayoutParams;
                        myServiceHideLayout.Height = (mHeight / 4) * 3;
                        myServiceHideView.Visibility = ViewStates.Visible;
                        myServiceContainer.Visibility = ViewStates.Gone;
                        myServiceRefreshContainer.Visibility = ViewStates.Gone;
                        containerQuickAction.Visibility = ViewStates.Gone;

                        if (isBCRMDown)
                        {
                            SetMaintenanceLayoutParamsWithMyServiceHide();
                        }
                        else
                        {
                            SetRefreshLayoutParamsWithMyServiceHide();
                        }

                        OnHideBottomView();
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

        public void SetMyServiceRefreshView(string contentTxt, string buttonTxt)
        {
            try
            {
                Activity.RunOnUiThread(() =>
                {
                    try
                    {
                        myServiceHideView.Visibility = ViewStates.Gone;
                        myServiceContainer.Visibility = ViewStates.Gone;
                        myServiceRefreshContainer.Visibility = ViewStates.Visible;
                        containerQuickAction.Visibility = ViewStates.Gone;

                        if (string.IsNullOrEmpty(buttonTxt))
                        {
                            btnMyServiceRefresh.Text = GetLabelByLanguage("refreshBtnText");
                        }
                        else
                        {
                            btnMyServiceRefresh.Text = buttonTxt;
                        }

                        if (Build.VERSION.SdkInt >= BuildVersionCodes.N)
                        {
                            if (string.IsNullOrEmpty(contentTxt))
                            {
                                txtMyServiceRefreshMessage.TextFormatted = Html.FromHtml(GetLabelByLanguage("serviceRefreshMessage"), FromHtmlOptions.ModeLegacy);
                            }
                            else
                            {
                                txtMyServiceRefreshMessage.TextFormatted = Html.FromHtml(contentTxt, FromHtmlOptions.ModeLegacy);
                            }
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(contentTxt))
                            {
                                txtMyServiceRefreshMessage.TextFormatted = Html.FromHtml(GetLabelByLanguage("serviceRefreshMessage"));
                            }
                            else
                            {
                                txtMyServiceRefreshMessage.TextFormatted = Html.FromHtml(contentTxt);
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

        public bool GetHomeTutorialCallState()
        {
            try
            {
                return UserSessions.HasHomeTutorialShown(PreferenceManager.GetDefaultSharedPreferences(this.Activity));
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
            return false;
        }

        public bool OnGetIsRootTooltipShown()
        {
            try
            {
                return ((DashboardHomeActivity)this.Activity).GetIsRootTutorialShown();
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
            return false;
        }

        public void OnHideBottomView()
        {
            try
            {
                Activity.RunOnUiThread(() =>
                {
                    try
                    {
                        if (myServiceHideView.Visibility == ViewStates.Visible && newFAQTitle.Visibility == ViewStates.Gone)
                        {
                            bottomContainer.Visibility = ViewStates.Gone;
                            if (isBCRMDown)
                            {
                                SetMaintenanceLayoutParamsWithAllDown();
                            }
                            else
                            {
                                SetRefreshLayoutParamsWithAllDown();
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


        public class NewFAQScrollListener : RecyclerView.OnScrollListener
        {
            private List<NewFAQ> mList = new List<NewFAQ>();
            private LinearLayout mIndicatorContainer;

            public NewFAQScrollListener(List<NewFAQ> list, LinearLayout indicatorContainer)
            {
                if (list != null && list.Count > 0)
                {
                    mList = list;
                }

                mIndicatorContainer = indicatorContainer;
            }

            protected NewFAQScrollListener(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
            {

            }

            public override void OnScrollStateChanged(RecyclerView recyclerView, int newState)
            {
                base.OnScrollStateChanged(recyclerView, newState);
                int visibleCards = TextViewUtils.IsLargeFonts ? 2 : 3;
                if (newState == (int)ScrollState.Idle && mList != null && mList.Count > visibleCards)
                {
                    LinearLayoutManager layoutManager = recyclerView.GetLayoutManager() as LinearLayoutManager;
                    int firstCompleteItemShow = layoutManager.FindFirstCompletelyVisibleItemPosition();
                    int lastCompleteItemShow = layoutManager.FindLastCompletelyVisibleItemPosition();

                    bool isLastItemReach = lastCompleteItemShow == (mList.Count - 1);

                    int count = 0;


                    for (int i = 0; i < mList.Count; i += visibleCards)
                    {
                        ImageView selectedDot = (ImageView)mIndicatorContainer.GetChildAt(count);
                        int nextLastItem = i + visibleCards;

                        if (isLastItemReach)
                        {
                            if (nextLastItem > (mList.Count - 1))
                            {
                                selectedDot.SetImageResource(Resource.Drawable.onboarding_circle_active);
                            }
                            else
                            {
                                selectedDot.SetImageResource(Resource.Drawable.faq_indication_inactive);
                            }
                        }
                        else
                        {
                            if (firstCompleteItemShow >= i && firstCompleteItemShow < nextLastItem)
                            {
                                selectedDot.SetImageResource(Resource.Drawable.onboarding_circle_active);
                            }
                            else
                            {
                                selectedDot.SetImageResource(Resource.Drawable.faq_indication_inactive);
                            }
                        }
                        count++;
                    }
                }
            }
        }

        public void ShowDiscoverMoreLayout()
        {
            if (UserSessions.GetEnergyBudgetList().Count > 0 && MyTNBAccountManagement.GetInstance().IsEBUserVerify()
                && !MyTNBAccountManagement.GetInstance().COMCLandNEM() && Utility.IsMDMSDownEnergyBudget())
            {
                SetupEBDiscoverView();
                discoverMoreContainer.Visibility = ViewStates.Visible;
                discoverMoreSectionTitle.Visibility = ViewStates.Visible;
            }
            else
            {
                discoverMoreContainer.Visibility = ViewStates.Gone;
            }
        }

        public void ShowDiscoverMoreSDLayout()
        {
            if (MyTNBAccountManagement.GetInstance().IsSDUserVerify())
            {
                SetupSDDiscoverView();
                discoverMoreSDContainer.Visibility = ViewStates.Visible;
                discoverMoreSectionTitle.Visibility = ViewStates.Visible;
            }
            else
            {
                discoverMoreSDContainer.Visibility = ViewStates.Gone;
            }
        }

        public void EBPopupActivity()
        {
            try
            {
                Intent eb_popup_activity = new Intent(this.Activity, typeof(EBPopupScreenActivity));
                eb_popup_activity.PutExtra("fromLogin", "fromLogin");
                StartActivityForResult(eb_popup_activity, SELECT_SM_POPUP_REQUEST_CODE);
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }


        public void WhatNewCheckAgain()
        {
            try
            {
                if (UserSessions.GetEnergyBudgetList().Count > 0 && MyTNBAccountManagement.GetInstance().IsEBUserVerify() && MyTNBAccountManagement.GetInstance().IsOnHoldWhatNew())
                {
                    ((DashboardHomeActivity)Activity).OnCheckWhatsNewTab();
                }
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void FilterCOMCLandNEM()
        {
            try
            {
                ((DashboardHomeActivity)Activity).FilterComAndNEM();
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        private void SetUpNBRView()
        {
            try
            {
                Activity.RunOnUiThread(() =>
                {
                    if (BillRedesignUtility.Instance.ShouldShowHomeCard && BillRedesignUtility.Instance.IsAccountEligible)
                    {
                        discoverMoreSectionTitle.Visibility = ViewStates.Visible;
                        discoverMoreNBRContainer.Visibility = ViewStates.Visible;
                        newBillRedesignBanner.Visibility = ViewStates.Visible;
                        newBillRedesignBanner.SetImageResource(LanguageUtil.GetAppLanguage() == "MS" ? Resource.Drawable.Banner_Home_NBR_MS
                            : Resource.Drawable.Banner_Home_NBR_EN);
                    }
                });
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        [OnClick(Resource.Id.newBillRedesignBanner)]
        void NewBillRedesignBannerOnClick(object sender, EventArgs eventArgs)
        {
            DynatraceHelper.OnTrack(DynatraceConstants.BR.CTAs.Home.Home_Banner);
            ((DashboardHomeActivity)Activity).NavigateToNBR();
        }

        public void SetupMyHomeBanner()
        {
            Activity.RunOnUiThread(() =>
            {
                if (!MyHomeUtility.Instance.IsBannerHidden && MyHomeUtility.Instance.IsAccountEligible)
                {
                    discoverMoreSectionTitle.Visibility = ViewStates.Visible;
                    discoverMoreMyHomeContainer.Visibility = ViewStates.Visible;
                    myHomeBanner.Visibility = ViewStates.Visible;
                    myHomeBanner.SetImageResource(LanguageUtil.GetAppLanguage() == "MS" ? Resource.Drawable.Banner_Home_MyHome_MS
                        : Resource.Drawable.Banner_Home_MyHome_EN);
                }
            });
        }

        [OnClick(Resource.Id.myHomeBanner)]
        void MyHomeBannerOnClick(object sender, EventArgs eventArgs)
        {
            List<MyServiceModel> serviceList = myServicesList;
            if (serviceList != null && serviceList.Count > 0)
            {
                int index = serviceList.FindIndex(x => x.ServiceType == MobileEnums.ServiceEnum.MYHOME);
                if (index > -1)
                {
                    MyServiceModel myService = serviceList[index];
                    if (myService != null)
                    {
                        List<MyDrawerModel> drawerList = new List<MyDrawerModel>();

                        if (myService.Children != null
                            && myService.Children.Count > 0)
                        {
                            foreach (MyServiceModel child in myService.Children)
                            {
                                drawerList.Add(new MyDrawerModel()
                                {
                                    ParentServiceId = child.ParentServiceId,
                                    ServiceId = child.ServiceId,
                                    ServiceName = child.ServiceName,
                                    ServiceIconUrl = child.ServiceIconUrl,
                                    DisabledServiceIconUrl = child.DisabledServiceIconUrl,
                                    SSODomain = child.SSODomain,
                                    OriginURL = child.OriginURL,
                                    RedirectURL = child.RedirectURL,
                                    ServiceType = child.ServiceType
                                });
                            }

                            MyHomeDrawerFragment myHomeBottomSheetDialog = new MyHomeDrawerFragment(this.Activity, drawerList, myService.ServiceName);

                            myHomeBottomSheetDialog.Cancelable = true;
                            myHomeBottomSheetDialog.Show(this.Activity.SupportFragmentManager, "My Home Dialog");
                        }
                    }
                }
            }
        }
        
        public void OnBCRMDownTimeErrorMessageV2(DownTimeEntity bcrmEntity)
        {
            MyTNBAppToolTipBuilder.Create(this.Activity, MyTNBAppToolTipBuilder.ToolTipType.MYTNB_DIALOG_WITH_FLOATING_IMAGE_ONE_BUTTON)
            .SetHeaderImage(Resource.Drawable.maintenance_bcrm_new)
            .SetTitle(bcrmEntity.DowntimeTextMessage)
            .SetMessage(bcrmEntity.DowntimeMessage)
            .SetCTALabel(Utility.GetLocalizedCommonLabel(LanguageConstants.Common.GOT_IT))
            //.SetCTAaction(() => { isBCRMDown = false; })
            .Build()
            .Show();
        }
    }
}
