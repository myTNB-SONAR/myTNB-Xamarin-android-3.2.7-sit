
using System;
using System.Collections.Generic;
using System.Linq;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using CheeseBind;
using Facebook.Shimmer;
using Java.Lang;
using myTNB_Android.Src.AddAccount.Activity;
using myTNB_Android.Src.Base.Fragments;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.FAQ.Activity;
using myTNB_Android.Src.myTNBMenu.Activity;
using myTNB_Android.Src.myTNBMenu.Fragments.FeedbackMenu;
using myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.Adapter;
using myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.Listener;
using myTNB_Android.Src.Notifications.Activity;
using myTNB_Android.Src.SummaryDashBoard.Models;
using myTNB_Android.Src.SummaryDashBoard.SummaryListener;
using myTNB_Android.Src.Utils;
using static myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.Adapter.MyServiceAdapter;
using static myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.Adapter.MyServiceShimmerAdapter;
using myTNB_Android.Src.Utils.Custom.ProgressDialog;
using Android.App;
using myTNB_Android.Src.SSMR.SMRApplication.MVP;
using myTNB_Android.Src.Base;
using Android.Text;
using myTNB_Android.Src.SSMRMeterHistory.MVP;
using Android.Runtime;
using Android.Util;
using static myTNB_Android.Src.AppLaunch.Models.MasterDataResponse;
using Android.Views.Animations;
using myTNB_Android.Src.MultipleAccountPayment.Activity;
using myTNB_Android.Src.SelectSupplyAccount.Activity;
using myTNB_Android.Src.myTNBMenu.Models;
using Newtonsoft.Json;
using myTNB_Android.Src.ViewBill.Activity;
using Android.Preferences;
using myTNB_Android.Src.NewAppTutorial.MVP;

namespace myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.MVP
{
    public class HomeMenuFragment : BaseFragment, HomeMenuContract.IHomeMenuView, ViewTreeObserver.IOnGlobalLayoutListener, View.IOnFocusChangeListener
    {
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

        [BindView(Resource.Id.myServiceShimmerView)]
        LinearLayout myServiceShimmerView;

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


        AccountsRecyclerViewAdapter accountsAdapter;


        private string mSavedTimeStamp = "0000000";

        private string savedSSMRMeterReadingTimeStamp = "0000000";

        private string savedSSMRMeterReadingThreePhaseTimeStamp = "0000000";

        private string savedSSMRMeterReadingNoOCRTimeStamp = "0000000";

        private string savedSSMRMeterReadingThreePhaseNoOCRTimeStamp = "0000000";

        private string savedEnergySavingTipsTimeStamp = "0000000";

        public readonly static int SSMR_METER_HISTORY_ACTIVITY_CODE = 8796;

        private static List<MyService> currentMyServiceList = new List<MyService>();

        private static List<NewFAQ> currentNewFAQList = new List<NewFAQ>();

        private static bool isBCRMDown = false;

        private static bool isFirstInitiate = true;

        private bool isSearchClose = false;

        private bool isAlreadyRotated = false;

        private bool isMyServiceAlreadyRotated = false;

        private bool isSMRReady = false;

        private bool isPayBillDisabled = false;

        private bool isViewBillDisabled = false;

        private bool isRefreshShown = false;

        HomeMenuContract.IHomeMenuPresenter presenter;
        ISummaryFragmentToDashBoardActivtyListener mCallBack;

        MyServiceShimmerAdapter myServiceShimmerAdapter;

        MyServiceAdapter myServiceAdapter;

        NewFAQShimmerAdapter newFAQShimmerAdapter;

        NewFAQAdapter newFAQAdapter;

        private LoadingOverlay loadingOverlay;



        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            presenter = new HomeMenuPresenter(this, PreferenceManager.GetDefaultSharedPreferences(this.Activity));
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
                    accountGreeting.Text = GetString(Resource.String.greeting_text_morning);
                    break;
                case Constants.GREETING.AFTERNOON:
                    accountGreeting.Text = GetString(Resource.String.greeting_text_afternoon);
                    break;
                default:
                    accountGreeting.Text = GetString(Resource.String.greeting_text_evening);
                    break;
            }
        }

        public override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            if (requestCode == SSMR_METER_HISTORY_ACTIVITY_CODE)
            {
                if (resultCode == Result.Ok)
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
                if (resultCode == Result.Ok)
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
        }

        private void SetNotificationIndicator()
        {
            if (UserNotificationEntity.HasNotifications())
            {
                notificationHeaderIcon.SetImageResource(Resource.Drawable.ic_header_notification_unread);
            }
            else
            {
                notificationHeaderIcon.SetImageResource(Resource.Drawable.ic_header_notification);
            }
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            try
            {
                summaryNestScrollView.SmoothScrollingEnabled = true;
                isSearchClose = true;
                isFirstInitiate = true;
                UpdateGreetingsHeader(this.presenter.GetGreeting());
                accountGreetingName.Text = this.presenter.GetAccountDisplay() + "!";
                SetNotificationIndicator();
                SetAccountsRecyclerView();
                SetAccountActionHeader();
                SetupMyServiceView();
                SetupNewFAQView();
                TextViewUtils.SetMuseoSans300Typeface(txtRefreshMsg);
                TextViewUtils.SetMuseoSans500Typeface(newFAQTitle, btnRefresh, txtAdd, addActionLabel, searchActionLabel, loadMoreLabel, rearrangeLabel, myServiceLoadMoreLabel);

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
                        StartActivity(new Intent(this.Activity, typeof(NotificationActivity)));
                    }
                };

                closeImageView.Clickable = true;

                closeImageView.Click += delegate {
                    OnSearchOutFocus(true);
                };

                this.presenter.GetSmartMeterReadingWalkthroughtTimeStamp();

                this.presenter.GetSmartMeterReadingThreePhaseWalkthroughtTimeStamp();

                this.presenter.GetSmartMeterReadingWalkthroughtNoOCRTimeStamp();

                this.presenter.GetSmartMeterReadingThreePhaseWalkthroughtNoOCRTimeStamp();


                bool isGetEnergyTipsDisabled = false;
                MasterDataObj currentMasterData = MyTNBAccountManagement.GetInstance().GetCurrentMasterData().Data;
                if (currentMasterData.IsEnergyTipsDisabled)
                {
                    isGetEnergyTipsDisabled = true;
                }

                if (!isGetEnergyTipsDisabled)
                {
                    this.presenter.GetEnergySavingTipsTimeStamp();
                }

                SetRefreshLayoutParams();

                ((DashboardHomeActivity)Activity).EnableDropDown(false);
                ((DashboardHomeActivity)Activity).HideAccountName();
                ((DashboardHomeActivity)Activity).ShowBackButton(false);
                ((DashboardHomeActivity)Activity).SetToolbarTitle(Resource.String.dashboard_activity_title);

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
                DownTimeEntity bcrmDownTime = DownTimeEntity.GetByCode(Constants.BCRM_SYSTEM);
                SMRPopUpUtils.SetFromUsageFlag(false);
                SMRPopUpUtils.SetFromUsageSubmitSuccessfulFlag(false);
                if (bcrmDownTime != null && bcrmDownTime.IsDown)
                {
                    isBCRMDown = true;
                }
                else
                {
                    isBCRMDown = false;
                }

                if (!isBCRMDown)
                {
                    OnStartLoadAccount();
                }
                else
                {
                    IsLoadMoreButtonVisible(false, false);

                    IsMyServiceLoadMoreButtonVisible(false, false);

                    IsRearrangeButtonVisible(false);

                    ShowRefreshScreen(bcrmDownTime.DowntimeMessage, null);
                }

                SetBottomLayoutBackground(false);
                this.presenter.InitiateService();
                this.presenter.GetUserNotifications();
                SetNotificationIndicator();
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
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

        private void OnLoadAccount()
        {
            this.presenter.LoadAccounts();
        }

        private void SetupMyServiceView()
        {
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

            LinearLayoutManager linearShimmerLayoutManager = new LinearLayoutManager(this.Activity, LinearLayoutManager.Horizontal, false);
            newFAQShimmerList.SetLayoutManager(linearShimmerLayoutManager);

        }

        public void SetMyServiceRecycleView()
        {
            myServiceShimmerAdapter = new MyServiceShimmerAdapter(this.presenter.LoadShimmerServiceList(3), this.Activity);
            myServiceShimmerList.SetAdapter(myServiceShimmerAdapter);

            myServiceShimmerView.Visibility = ViewStates.Visible;
            myServiceView.Visibility = ViewStates.Gone;
            this.presenter.InitiateMyService();

        }

        public void SetMyServiceResult(List<MyService> list)
        {
            try
            {
                Activity.RunOnUiThread(() =>
                {
                    myServiceAdapter = new MyServiceAdapter(list, this.Activity, isBCRMDown, isRefreshShown);
                    myServiceListRecycleView.SetAdapter(myServiceAdapter);
                    currentMyServiceList.Clear();
                    currentMyServiceList.AddRange(list);
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
                });
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void SetNewFAQRecycleView()
        {
            SetupNewFAQShimmerEffect();
            this.presenter.GetSavedNewFAQTimeStamp();
        }

        private void SetupNewFAQShimmerEffect()
        {
            try
            {
                Activity.RunOnUiThread(() =>
                {
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
                });
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
                    if (list != null && list.Count > 0)
                    {
                        newFAQAdapter = new NewFAQAdapter(list, this.Activity);
                        newFAQListRecycleView.SetAdapter(newFAQAdapter);
                        currentNewFAQList.Clear();
                        currentNewFAQList.AddRange(list);
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
                    }
                });
            }
            catch (System.Exception e)
            {
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
                searchEditText.SetBackgroundResource(Resource.Drawable.search_edit_bg);
                closeImageView.Visibility = ViewStates.Visible;
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
            param.LeftMargin = (int) DPUtils.ConvertDPToPx(16f);
            param.RightMargin = (int)DPUtils.ConvertDPToPx(16f);

            TextViewUtils.SetMuseoSans500Typeface(accountHeaderTitle, accountGreeting, accountGreetingName);
            searchEditText.SetOnQueryTextListener(new AccountsSearchOnQueryTextListener(this,accountsAdapter));
            searchEditText.SetOnQueryTextFocusChangeListener(this);
            EditText searchText = searchEditText.FindViewById<EditText>(searchEditText.Context.Resources.GetIdentifier("android:id/search_src_text", null, null));
            searchText.SetTextColor(Resources.GetColor(Resource.Color.white));
            searchText.SetHintTextColor(Resources.GetColor(Resource.Color.sixty_opacity_white));
            searchText.SetTextSize(ComplexUnitType.Dip, 12f);
            TextViewUtils.SetMuseoSans500Typeface(searchText);
            searchText.SetPadding((int) DPUtils.ConvertDPToPx(34f), 0, 0, 0);

            int closeViewId = searchEditText.Context.Resources.GetIdentifier("android:id/search_close_btn", null, null);

            closeImageView = searchEditText.FindViewById<ImageView>(closeViewId);
            closeImageView.Visibility = ViewStates.Visible;

            closeImageView.SetPadding(0, 0, 0, 0);
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
                var act = this.Activity as AppCompatActivity;

                var actionBar = act.SupportActionBar;
                actionBar.Hide();
                ShowBackButton(false);
                if (this.presenter != null)
                {
                    this.presenter.OnCheckToCallHomeMenuTutorial();
                }
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return base.OnCreateView(inflater, container, savedInstanceState);
        }


        public override int ResourceId()
        {
            return Resource.Layout.HomeMenuFragmentView;
        }

        public void OnUpdateAccountListChanged(bool isSearchSubmit)
		{
            if (isSearchSubmit)
            {
                ShowSearchAction(false);
                /*LinearLayoutManager manager = accountsRecyclerView.GetLayoutManager() as LinearLayoutManager;
                int position = manager.FindFirstCompletelyVisibleItemPosition();

                List<string> accountNumberList = accountsAdapter.GetAccountCardNumberLists();
                this.presenter.LoadSummaryDetailsInBatch(accountNumberList);*/
			}
		}


        void OnClickChanged(object sender, int position)
        {
            try
            {
                if (position != -1)
                {
                    if (!this.GetIsClicked())
                    {
                        this.SetIsClicked(true);

                        MyService selectedService = currentMyServiceList[position];
                        if (selectedService.ServiceCategoryId == "1003")
                        {
                            ShowFeedbackMenu();
                        }
                        else if (selectedService.ServiceCategoryId == "1001")
                        {
                            Intent applySMRIntent;
                            if (MyTNBAccountManagement.GetInstance().IsSMROnboardingShown())
                            {
                                applySMRIntent = new Intent(this.Activity, typeof(SSMRMeterHistoryActivity));
                            }
                            else
                            {
                                applySMRIntent = new Intent(this.Activity, typeof(OnBoardingActivity));
                            }

                            if (!UserSessions.HasSMROnboardingShown(PreferenceManager.GetDefaultSharedPreferences(this.Activity)))
                            {
                                UserSessions.DoSMROnboardingShown(PreferenceManager.GetDefaultSharedPreferences(this.Activity));
                            }

                            StartActivityForResult(applySMRIntent, SSMR_METER_HISTORY_ACTIVITY_CODE);
                        }
                        else if (selectedService.ServiceCategoryId == "1004" && (!isBCRMDown && !isRefreshShown && MyTNBAccountManagement.GetInstance().IsPayBillEnabledNeeded()))
                        {
                            if (!UserSessions.HasPayBillShown(PreferenceManager.GetDefaultSharedPreferences(this.Activity)))
                            {
                                UserSessions.DoPayBillShown(PreferenceManager.GetDefaultSharedPreferences(this.Activity));
                            }
                            Intent payment_activity = new Intent(this.Activity, typeof(SelectAccountsActivity));
                            StartActivity(payment_activity);
                        }
                        else if (selectedService.ServiceCategoryId == "1005" && (!isBCRMDown && !isRefreshShown && MyTNBAccountManagement.GetInstance().IsViewBillEnabledNeeded()))
                        {
                            if (!UserSessions.HasViewBillShown(PreferenceManager.GetDefaultSharedPreferences(this.Activity)))
                            {
                                UserSessions.DoViewBillShown(PreferenceManager.GetDefaultSharedPreferences(this.Activity));
                            }
                            CustomerBillingAccount.RemoveSelected();
                            Intent supplyAccount = new Intent(this.Activity, typeof(SelectSupplyAccountActivity));
                            supplyAccount.PutExtra(Constants.CODE_KEY, Constants.SELECT_ACCOUNT_PDF_REQUEST_CODE);
                            StartActivity(supplyAccount);
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
                        Intent faqIntent = new Intent(this.Activity, typeof(FAQListActivity));
                        faqIntent.PutExtra(Constants.FAQ_ID_PARAM, selectedNewFAQ.TargetItem);
                        Activity.StartActivity(faqIntent);

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

        public void ShowFeedbackMenu()
        {
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
                msg = GetString(Resource.String.my_service_error);
            }

            mMyServiceRetrySnakebar = Snackbar.Make(rootView, msg, Snackbar.LengthIndefinite)
            .SetAction(GetString(Resource.String.my_service_btn_retry), delegate
            {

                mMyServiceRetrySnakebar.Dismiss();
                RetryMyService();
            }
            );
            mMyServiceRetrySnakebar.Show();
        }

        private void RetryMyService()
        {
            MyServiceShimmerAdapter adapter = new MyServiceShimmerAdapter(this.presenter.LoadShimmerServiceList(3), this.Activity);
            myServiceShimmerList.SetAdapter(adapter);

            myServiceShimmerView.Visibility = ViewStates.Visible;
            myServiceView.Visibility = ViewStates.Gone;

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
                if (success)
                {
                    NewFAQParentEntity wtManager = new NewFAQParentEntity();
                    List<NewFAQParentEntity> items = wtManager.GetAllItems();
                    if (items != null)
                    {
                        NewFAQParentEntity entity = items[0];
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
                    }

                }
                else
                {
                    this.presenter.ReadNewFAQFromCache();
                }
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
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
                if (loadingOverlay != null && loadingOverlay.IsShowing)
                {
                    loadingOverlay.Dismiss();
                }

                loadingOverlay = new LoadingOverlay(this.Activity, Resource.Style.LoadingOverlyDialogStyle);
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
                        closeImageView.Visibility = ViewStates.Visible;
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

        public void OnSavedSSMRMeterReadingTimeStamp(string mSavedTimeStamp)
        {
            if (mSavedTimeStamp != null)
            {
                this.savedSSMRMeterReadingTimeStamp = mSavedTimeStamp;
            }
            this.presenter.OnGetSmartMeterReadingWalkthroughtTimeStamp();
        }

        public void CheckSSMRMeterReadingTimeStamp()
        {
            try
            {
                SSMRMeterReadingScreensParentEntity wtManager = new SSMRMeterReadingScreensParentEntity();
                List<SSMRMeterReadingScreensParentEntity> items = wtManager.GetAllItems();
                if (items != null)
                {
                    SSMRMeterReadingScreensParentEntity entity = items[0];
                    if (entity != null)
                    {
                        if (!entity.Timestamp.Equals(savedSSMRMeterReadingTimeStamp))
                        {
                            this.presenter.OnGetSSMRMeterReadingScreens();
                        }
                    }
                }
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void OnSavedSSMRMeterReadingNoOCRTimeStamp(string mSavedTimeStamp)
        {
            if (mSavedTimeStamp != null)
            {
                this.savedSSMRMeterReadingNoOCRTimeStamp = mSavedTimeStamp;
            }
            this.presenter.OnGetSmartMeterReadingWalkthroughtNoOCRTimeStamp();
        }

        public void CheckSSMRMeterReadingNoOCRTimeStamp()
        {
            try
            {
                SSMRMeterReadingScreensOCROffParentEntity wtManager = new SSMRMeterReadingScreensOCROffParentEntity();
                List<SSMRMeterReadingScreensOCROffParentEntity> items = wtManager.GetAllItems();
                if (items != null)
                {
                    SSMRMeterReadingScreensOCROffParentEntity entity = items[0];
                    if (entity != null)
                    {
                        if (!entity.Timestamp.Equals(savedSSMRMeterReadingNoOCRTimeStamp))
                        {
                            this.presenter.OnGetSSMRMeterReadingScreensNoOCR();
                        }
                    }
                }
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void OnSavedSSMRMeterReadingThreePhaseTimeStamp(string mSavedTimeStamp)
        {
            if (mSavedTimeStamp != null)
            {
                this.savedSSMRMeterReadingThreePhaseTimeStamp = mSavedTimeStamp;
            }
            this.presenter.OnGetSmartMeterReadingThreePhaseWalkthroughtTimeStamp();
        }

        public void CheckSSMRMeterReadingThreePhaseTimeStamp()
        {
            try
            {
                SSMRMeterReadingThreePhaseScreensParentEntity wtManager = new SSMRMeterReadingThreePhaseScreensParentEntity();
                List<SSMRMeterReadingThreePhaseScreensParentEntity> items = wtManager.GetAllItems();
                if (items != null)
                {
                    SSMRMeterReadingThreePhaseScreensParentEntity entity = items[0];
                    if (entity != null)
                    {
                        if (!entity.Timestamp.Equals(savedSSMRMeterReadingThreePhaseTimeStamp))
                        {
                            this.presenter.OnGetSSMRMeterReadingThreePhaseScreens();
                        }
                    }
                }
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }


        public void OnSavedSSMRMeterReadingThreePhaseNoOCRTimeStamp(string mSavedTimeStamp)
        {
            if (mSavedTimeStamp != null)
            {
                this.savedSSMRMeterReadingThreePhaseNoOCRTimeStamp = mSavedTimeStamp;
            }
            this.presenter.OnGetSmartMeterReadingThreePhaseWalkthroughtNoOCRTimeStamp();
        }

        public void CheckSSMRMeterReadingThreePhaseNoOCRTimeStamp()
        {
            try
            {
                SSMRMeterReadingThreePhaseScreensOCROffParentEntity wtManager = new SSMRMeterReadingThreePhaseScreensOCROffParentEntity();
                List<SSMRMeterReadingThreePhaseScreensOCROffParentEntity> items = wtManager.GetAllItems();
                if (items != null)
                {
                    SSMRMeterReadingThreePhaseScreensOCROffParentEntity entity = items[0];
                    if (entity != null)
                    {
                        if (!entity.Timestamp.Equals(savedSSMRMeterReadingThreePhaseNoOCRTimeStamp))
                        {
                            this.presenter.OnGetSSMRMeterReadingThreePhaseScreensNoOCR();
                        }
                    }
                }
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void ShowRefreshScreen(string contentMsg, string buttonMsg)
        {
            accountListRefreshContainer.Visibility = ViewStates.Visible;
            accountListViewContainer.Visibility = ViewStates.Gone;
            string refreshMsg = string.IsNullOrEmpty(contentMsg) ? "Uh oh, looks like this page is unplugged. Refresh to stay plugged in!" : contentMsg;
            string refreshBtnTxt = string.IsNullOrEmpty(buttonMsg) ? "Refresh Now" : buttonMsg;
            btnRefresh.Text = refreshBtnTxt;
            if (Android.OS.Build.VERSION.SdkInt >= Android.OS.Build.VERSION_CODES.N)
            {
                txtRefreshMsg.TextFormatted = Html.FromHtml(refreshMsg, FromHtmlOptions.ModeLegacy);
            }
            else
            {
                txtRefreshMsg.TextFormatted = Html.FromHtml(refreshMsg);
            }
            if (isBCRMDown)
            {
                btnRefresh.Visibility = ViewStates.Gone;
                SetMaintenanceLayoutParams();
            }
            else
            {
                isRefreshShown = true;
                btnRefresh.Visibility = ViewStates.Visible;
                SetRefreshLayoutParams();
                if (currentMyServiceList.Count > 0)
                {
                    myServiceAdapter = new MyServiceAdapter(currentMyServiceList, this.Activity, isBCRMDown, isRefreshShown);
                    myServiceListRecycleView.SetAdapter(myServiceAdapter);

                    myServiceAdapter.ClickChanged += OnClickChanged;
                    this.SetIsClicked(false);
                }
            }
        }

        private void OnStartLoadAccount()
        {
            IsLoadMoreButtonVisible(false, false);

            IsMyServiceLoadMoreButtonVisible(false, false);

            IsRearrangeButtonVisible(false);

            List<CustomerBillingAccount> eligibleSMRBillingAccounts = CustomerBillingAccount.EligibleSMRAccountList();
            List<CustomerBillingAccount> currentSMRBillingAccounts = CustomerBillingAccount.CurrentSMRAccountList();
            List<CustomerBillingAccount> list = CustomerBillingAccount.GetSortedCustomerBillingAccounts();
            List<SMRAccount> eligibleSmrAccountList = new List<SMRAccount>();
            List<SMRAccount> currentSmrAccountList = new List<SMRAccount>();
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

            UserSessions.SetSMRAccountList(currentSmrAccountList);
            UserSessions.SetSMREligibilityAccountList(eligibleSmrAccountList);

            accountListRefreshContainer.Visibility = ViewStates.Gone;
            accountListViewContainer.Visibility = ViewStates.Visible;
            if (MyTNBAccountManagement.GetInstance().IsNeedUpdatedBillingDetails())
            {
                UserSessions.SetRealSMREligibilityAccountList(eligibleSmrAccountList);
            }

            searchEditText.SetQuery("", false);
            OnLoadAccount();
        }

        public void UpdateEligibilitySMRAccountList()
        {
            List<CustomerBillingAccount> eligibleSMRBillingAccounts = CustomerBillingAccount.EligibleSMRAccountList();
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
            isRefreshShown = false;

            IsLoadMoreButtonVisible(false, false);

            IsMyServiceLoadMoreButtonVisible(false, false);

            IsRearrangeButtonVisible(false);

            SetBottomLayoutBackground(false);

            List<CustomerBillingAccount> eligibleSMRBillingAccounts = CustomerBillingAccount.EligibleSMRAccountList();
            List<CustomerBillingAccount> currentSMRBillingAccounts = CustomerBillingAccount.CurrentSMRAccountList();
            List<CustomerBillingAccount> list = CustomerBillingAccount.GetSortedCustomerBillingAccounts();
            List<SMRAccount> eligibleSmrAccountList = new List<SMRAccount>();
            List<SMRAccount> currentSmrAccountList = new List<SMRAccount>();
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

            UserSessions.SetSMRAccountList(currentSmrAccountList);
            UserSessions.SetSMREligibilityAccountList(eligibleSmrAccountList);

            accountListRefreshContainer.Visibility = ViewStates.Gone;
            accountListViewContainer.Visibility = ViewStates.Visible;
            if (MyTNBAccountManagement.GetInstance().IsNeedUpdatedBillingDetails())
            {
                UserSessions.SetRealSMREligibilityAccountList(eligibleSmrAccountList);
            }

            searchEditText.SetQuery("", false);
            searchEditText.Visibility = ViewStates.Gone;

            this.presenter.RefreshAccountSummary();

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

                Intent linkAccount = new Intent(this.Activity, typeof(LinkAccountActivity));
                linkAccount.PutExtra("fromDashboard", true);
                StartActivity(linkAccount);
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
                closeImageView.Visibility = ViewStates.Visible;
            }

            this.presenter.LoadQueryAccounts(searchText);
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
                        closeImageView.Visibility = ViewStates.Visible;
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

                        loadMoreLabel.Text = Activity.GetString(Resource.String.show_less_accounts);
                    }

                    // Lin Siong TODO: Hide this on 15 Nov Release
                    // IsRearrangeButtonVisible(true);
                    IsRearrangeButtonVisible(false);
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

                        loadMoreLabel.Text = Activity.GetString(Resource.String.load_more_accounts);

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
            Activity.RunOnUiThread(() =>
            {
                if (isVisible)
                {
                    myServiceLoadMoreContainer.Visibility = ViewStates.Visible;
                    if (isRotate)
                    {
                        if (!isMyServiceAlreadyRotated)
                        {
                            isMyServiceAlreadyRotated = true;
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

                            myServiceLoadMoreImg.StartAnimation(animSet);

                            myServiceLoadMoreLabel.Text = Activity.GetString(Resource.String.show_less);
                        }
                    }
                    else
                    {
                        if (isMyServiceAlreadyRotated)
                        {
                            isMyServiceAlreadyRotated = false;
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

                            myServiceLoadMoreImg.StartAnimation(animSet);

                            myServiceLoadMoreLabel.Text = Activity.GetString(Resource.String.show_more);

                        }
                    }
                }
                else
                {
                    myServiceLoadMoreContainer.Visibility = ViewStates.Gone;
                }
            });
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

        [OnClick(Resource.Id.myServiceLoadMoreContainer)]
        internal void OnMyServiceLoadMorelick(object sender, EventArgs e)
        {
            this.presenter.DoMySerivceLoadMoreAccount();
        }

        [OnClick(Resource.Id.rearrangeContainer)]
        internal void OnRearrangeClick(object sender, EventArgs e)
        {

        }

        void View.IOnFocusChangeListener.OnFocusChange(View v, bool hasFocus)
        {
            if (string.IsNullOrEmpty(searchEditText.Query))
            {
                closeImageView.Visibility = ViewStates.Visible;
            }
        }

        public void SetBottomLayoutBackground(bool isMyServiceExpand)
        {
            Activity.RunOnUiThread(() =>
            {
                if (isMyServiceExpand)
                {
                    bottomContainer.SetBackgroundResource(Resource.Drawable.dashboard_botton_sheet_bg_expanded);
                }
                else
                {
                    bottomContainer.SetBackgroundResource(Resource.Drawable.dashboard_botton_sheet_bg);
                }
            });
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

                mLoadBillSnackBar = Snackbar.Make(rootView, GetString(Resource.String.dashboard_chart_cancelled_exception_error), Snackbar.LengthIndefinite)
                .SetAction(GetString(Resource.String.dashboard_chartview_data_not_available_no_internet_btn_close), delegate
                {
                    mLoadBillSnackBar.Dismiss();
                }
                );
                mLoadBillSnackBar.Show();
                this.SetIsClicked(false);
            }
            catch (System.Exception e)
            {
                this.SetIsClicked(false);
                Utility.LoggingNonFatalError(e);
            }
        }

        public void ShowBillPDF(AccountData selectedAccountData, BillHistoryV5 selectedBill = null)
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
                if (count <= 0)
                {
                    ME.Leolin.Shortcutbadger.ShortcutBadger.RemoveCount(this.Activity);
                }
                else
                {
                    ME.Leolin.Shortcutbadger.ShortcutBadger.ApplyCount(this.Activity, count);
                }

                SetNotificationIndicator();
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void OnShowHomeMenuFragmentTutorialDialog()
        {
            Activity.RunOnUiThread(() =>
            {
                StopScrolling();
            });
            NewAppTutorialUtils.ForceCloseNewAppTutorial();
            NewAppTutorialUtils.OnShowNewAppTutorial(this.Activity, this, PreferenceManager.GetDefaultSharedPreferences(this.Activity), this.presenter.OnGeneraNewAppTutorialList());
        }

        public void HomeMenuCustomScrolling(int yPosition)
        {
            try
            {
                Activity.RunOnUiThread(() =>
                {
                    summaryNestScrollView.ScrollTo(0, yPosition);
                    summaryNestScrollView.RequestLayout();
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

        public void ResetNewFAQScroll()
        {
            try
            {
                Activity.RunOnUiThread(() =>
                {
                    LinearLayoutManager layoutManager = newFAQListRecycleView.GetLayoutManager() as LinearLayoutManager;
                    layoutManager.ScrollToPositionWithOffset(0, 0);
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
    }
}
