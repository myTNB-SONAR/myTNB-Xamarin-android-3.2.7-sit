
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

namespace myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.MVP
{
    public class HomeMenuFragment : BaseFragment, HomeMenuContract.IHomeMenuView, ViewTreeObserver.IOnGlobalLayoutListener
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

        [BindView(Resource.Id.myServiceTitle)]
        TextView myServiceTitle;
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

        [BindView(Resource.Id.indicatorContainer)]
        LinearLayout indicatorContainer;

        [BindView(Resource.Id.summaryNestScrollView)]
        NestedScrollView summaryNestScrollView;

        [BindView(Resource.Id.summaryRootView)]
        CoordinatorLayout summaryRootView;

        [BindView(Resource.Id.shimmerFAQView)]
        ShimmerFrameLayout shimmerFAQView;

        [BindView(Resource.Id.notificationHeaderIcon)]
        ImageView notificationHeaderIcon;

        [BindView(Resource.Id.addAction)]
        ImageView addActionImage;


        [BindView(Resource.Id.newPromotionTitle)]
        TextView newPromotionTitle;

        [BindView(Resource.Id.newPromotionShimmerView)]
        LinearLayout newPromotionShimmerView;

        [BindView(Resource.Id.newPromotionShimmerList)]
        RecyclerView newPromotionShimmerList;

        [BindView(Resource.Id.newPromotionView)]
        LinearLayout newPromotionView;

        [BindView(Resource.Id.newPromotionList)]
        RecyclerView newPromotionList;

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
        CardView accountCard;

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


        AccountsRecyclerViewAdapter accountsAdapter;

        private string mSavedTimeStamp = "0000000";

        private string savedSSMRMeterReadingTimeStamp = "0000000";

        private string savedSSMRMeterReadingThreePhaseTimeStamp = "0000000";

        private string savedEnergySavingTipsTimeStamp = "0000000";

        public readonly static int SSMR_METER_HISTORY_ACTIVITY_CODE = 8796;

        private static List<MyService> currentMyServiceList = new List<MyService>();

        private static List<NewFAQ> currentNewFAQList = new List<NewFAQ>();

        private static bool isBCRMDown = false;

        private static bool isFirstInitiate = true;

        private bool isSearchClose = false;

        HomeMenuContract.IHomeMenuPresenter presenter;
        ISummaryFragmentToDashBoardActivtyListener mCallBack;

        MyServiceShimmerAdapter myServiceShimmerAdapter;

        MyServiceAdapter myServiceAdapter;

        NewFAQShimmerAdapter newFAQShimmerAdapter;

        NewFAQAdapter newFAQAdapter;

        NewPromotionShimmerAdapter newPromotionShimmerAdapter;

        NewPromotionAdapter newPromotionAdapter;

        private LoadingOverlay loadingOverlay;



        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            presenter = new HomeMenuPresenter(this);
        }

        public override void OnAttach(Context context)
        {
            base.OnAttach(context);
            try
            {
                mCallBack = context as ISummaryFragmentToDashBoardActivtyListener;
                FirebaseAnalyticsUtils.SetFragmentScreenName(this, "Home Screen");
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
                isSearchClose = true;
                isFirstInitiate = true;
                UpdateGreetingsHeader(this.presenter.GetGreeting());
                accountGreetingName.Text = this.presenter.GetAccountDisplay() + "!";
                SetNotificationIndicator();
                SetAccountsRecyclerView();
                SetAccountActionHeader();
                SetupMyServiceView();
                SetupNewFAQView();
                SetupNewPromotionView();
                TextViewUtils.SetMuseoSans300Typeface(txtRefreshMsg);
                TextViewUtils.SetMuseoSans500Typeface(myServiceTitle, newFAQTitle, newPromotionTitle, btnRefresh, txtAdd, addActionLabel, searchActionLabel);

                addActionContainer.SetOnClickListener(null);
                notificationHeaderIcon.SetOnClickListener(null);
                addActionContainer.Click += delegate
                {
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
                };
                notificationHeaderIcon.Click += delegate
                {
                    try
                    {
                        FirebaseAnalyticsUtils.LogFragmentClickEvent(this, "Home Screen -> Notification");
                    }
                    catch (System.Exception err)
                    {
                        Utility.LoggingNonFatalError(err);
                    }
                    StartActivity(new Intent(this.Activity, typeof(NotificationActivity)));
                };
                ((DashboardHomeActivity)Activity).SetStatusBarBackground();

                this.presenter.GetSmartMeterReadingWalkthroughtTimeStamp();

                this.presenter.GetSmartMeterReadingThreePhaseWalkthroughtTimeStamp();

                // Lin Siong TODO: Check Energy Saving Tips Enable Disable
                this.presenter.GetEnergySavingTipsTimeStamp();
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
                    newPromotionShimmerList.Focusable = false;
                    newPromotionList.Focusable = false;
                    topRootView.Focusable = true;
                    topRootView.RequestFocus();
                    ViewTreeObserver observer = summaryNestScrollView.ViewTreeObserver;
                    observer.AddOnGlobalLayoutListener(this);
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

        public void SetRefreshLayoutParams()
        {
            LinearLayout.LayoutParams refreshImgParams = refreshImg.LayoutParameters as LinearLayout.LayoutParams;
            LinearLayout.LayoutParams refreshMsgParams = refreshMsg.LayoutParameters as LinearLayout.LayoutParams;
            LinearLayout.LayoutParams btnRefreshParams = btnRefresh.LayoutParameters as LinearLayout.LayoutParams;

            refreshImgParams.Width = GetDeviceHorizontalScaleInPixel(0.431f);
            refreshImgParams.Height = GetDeviceHorizontalScaleInPixel(0.431f);
            refreshImg.RequestLayout();

            refreshMsgParams.Width = GetDeviceHorizontalScaleInPixel(0.80f);
            refreshMsgParams.RightMargin = GetDeviceHorizontalScaleInPixel(0.10f);
            refreshMsgParams.LeftMargin = GetDeviceHorizontalScaleInPixel(0.10f);
            refreshMsg.RequestLayout();

            btnRefreshParams.Width = GetDeviceHorizontalScaleInPixel(0.90f);
            btnRefreshParams.RightMargin = GetDeviceHorizontalScaleInPixel(0.05f);
            btnRefreshParams.LeftMargin = GetDeviceHorizontalScaleInPixel(0.05f);
            btnRefresh.RequestLayout();
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

        private void SetupNewPromotionView()
        {
            LinearLayoutManager linearLayoutManager = new LinearLayoutManager(this.Activity, LinearLayoutManager.Horizontal, false);
            newPromotionShimmerList.SetLayoutManager(linearLayoutManager);

            LinearLayoutManager linearShimmerLayoutManager = new LinearLayoutManager(this.Activity, LinearLayoutManager.Horizontal, false);
            newPromotionList.SetLayoutManager(linearShimmerLayoutManager);
        }

        public void SetMyServiceRecycleView()
        {
            myServiceShimmerAdapter = new MyServiceShimmerAdapter(this.presenter.LoadShimmerServiceList(6), this.Activity);
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
                    myServiceShimmerAdapter = new MyServiceShimmerAdapter(null, this.Activity);
                    myServiceShimmerList.SetAdapter(myServiceShimmerAdapter);
                    myServiceShimmerView.Visibility = ViewStates.Gone;
                    myServiceView.Visibility = ViewStates.Visible;
                    myServiceAdapter = new MyServiceAdapter(list, this.Activity);
                    myServiceListRecycleView.SetAdapter(myServiceAdapter);
                    currentMyServiceList.Clear();
                    currentMyServiceList.AddRange(list);
                    myServiceAdapter.ClickChanged += OnClickChanged;
                    try
                    {
                        if (accountsAdapter != null && accountsAdapter.accountCardModelList != null && myServiceAdapter != null)
                        {
                            int count = accountsAdapter.accountCardModelList.Count;
                            if (count < 1 && myServiceAdapter.ItemCount == 0)
                            {
                                newFAQTitle.SetTextColor(Color.White);
                            }
                            else
                            {
                                newFAQTitle.SetTextColor(Resources.GetColor(Resource.Color.powerBlue));
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
                    shimmerFAQView.StopShimmer();
                    newFAQShimmerAdapter = new NewFAQShimmerAdapter(null, this.Activity);
                    newFAQShimmerList.SetAdapter(newFAQShimmerAdapter);
                    newFAQShimmerView.Visibility = ViewStates.Gone;
                    newFAQView.Visibility = ViewStates.Visible;
                    newFAQAdapter = new NewFAQAdapter(list, this.Activity);
                    newFAQListRecycleView.SetAdapter(newFAQAdapter);
                    currentNewFAQList.Clear();
                    currentNewFAQList.AddRange(list);
                    newFAQAdapter.ClickChanged += OnFAQClickChanged;
                });
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void SetNewPromotionRecycleView()
        {
            SetupNewPromotionShimmerEffect();
            this.presenter.InitiateNewPromotion();
        }

        public void SetNewPromotionResult(List<NewPromotion> list)
        {
            try
            {
                Activity.RunOnUiThread(() =>
                {
                    newPromotionShimmerAdapter = new NewPromotionShimmerAdapter(null, this.Activity);
                    newPromotionShimmerList.SetAdapter(newPromotionShimmerAdapter);
                    newPromotionShimmerView.Visibility = ViewStates.Gone;
                    newPromotionView.Visibility = ViewStates.Visible;
                    newPromotionAdapter = new NewPromotionAdapter(list, this.Activity);
                    newPromotionList.SetAdapter(newPromotionAdapter);
                });
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        private void SetupNewPromotionShimmerEffect()
        {
            try
            {
                Activity.RunOnUiThread(() =>
                {
                    newPromotionShimmerAdapter = new NewPromotionShimmerAdapter(this.presenter.LoadShimmerPromotionList(2), this.Activity);
                    newPromotionShimmerList.SetAdapter(newPromotionShimmerAdapter);

                    newPromotionShimmerView.Visibility = ViewStates.Visible;
                    newPromotionView.Visibility = ViewStates.Gone;
                });
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }


        public void ShowSearchAction(bool isShow)
        {
            if (isShow)
            {
                accountHeaderTitle.Visibility = ViewStates.Gone;
                searchEditText.Visibility = ViewStates.Visible;
                searchEditText.SetMaxWidth(Integer.MaxValue);
                searchActionContainer.Visibility = ViewStates.Gone;
                searchEditText.OnActionViewExpanded();
                searchEditText.RequestFocus();
                if (searchEditText.Query != "")
                {
                    searchEditText.SetBackgroundResource(Resource.Drawable.rectangle_rounded_corner_bg);
                }
                else
                {
                    searchEditText.SetBackgroundResource(Resource.Drawable.search_edit_bg);
                }
            }
            else
            {
                accountHeaderTitle.Visibility = ViewStates.Visible;
                searchEditText.Visibility = ViewStates.Gone;
                searchActionContainer.Visibility = ViewStates.Visible;
            }
        }

        private void SetAccountActionHeader()
        {
            LinearLayout.LayoutParams param = (LinearLayout.LayoutParams)accountsActionsContainer.LayoutParameters;
            param.LeftMargin = GetDeviceHorizontalScaleInPixel(0.05f);
            param.RightMargin = GetDeviceHorizontalScaleInPixel(0.05f);

            TextViewUtils.SetMuseoSans500Typeface(accountHeaderTitle, accountGreeting, accountGreetingName);
            searchEditText.SetOnQueryTextListener(new AccountsSearchOnQueryTextListener(this,accountsAdapter));
            int closeViewId = searchEditText.Context.Resources.GetIdentifier("android:id/search_close_btn", null, null);
            int canceledViewId = searchEditText.Context.Resources.GetIdentifier("android:id/search_close_btn", null, null);
            ImageView closeImageView = searchEditText.FindViewById<ImageView>(closeViewId);
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
            LinearLayoutManager linearLayoutManager = new LinearLayoutManager(Activity, LinearLayoutManager.Horizontal, false);
            accountsRecyclerView.SetLayoutManager(linearLayoutManager);

            accountsAdapter = new AccountsRecyclerViewAdapter(this);
            accountsRecyclerView.AddOnScrollListener(new AccountsRecyclerViewOnScrollListener(this.presenter, linearLayoutManager, indicatorContainer));

            SnapHelper snapHelper = new LinearSnapHelper();
            snapHelper.AttachToRecyclerView(accountsRecyclerView);
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
                ShowSearchAction(false);
                DownTimeEntity bcrmDownTime = DownTimeEntity.GetByCode(Constants.BCRM_SYSTEM);
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
                    ShowRefreshScreen(bcrmDownTime.DowntimeMessage, null);
                }

                this.presenter.InitiateService();
                SetNotificationIndicator();
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
        private void UpdateAccountListIndicator()
        {
            indicatorContainer.RemoveAllViews();
            int accountsCount = accountsAdapter.ItemCount;
            if (accountsCount > 1)
            {
                indicatorContainer.Visibility = ViewStates.Visible;
                for (int i = 0; i < accountsCount; i++)
                {
                    ImageView image = new ImageView(Activity);
                    image.Id = i;
                    LinearLayout.LayoutParams layoutParams = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
                    layoutParams.RightMargin = 8;
                    layoutParams.LeftMargin = 8;
                    image.LayoutParameters = layoutParams;
                    if (i == 0)
                    {
                        image.SetImageResource(Resource.Drawable.circle_active);
                    }
                    else
                    {
                        image.SetImageResource(Resource.Drawable.circle);
                    }
                    indicatorContainer.AddView(image, i);
                }
            }
            else
            {
                indicatorContainer.Visibility = ViewStates.Gone;
            }
            ChangeMyServiceFAQTextColor();
        }

        public void OnUpdateAccountListChanged(bool isSearchSubmit)
		{
            if (isSearchSubmit)
            {
                ShowSearchAction(false);
                LinearLayoutManager manager = accountsRecyclerView.GetLayoutManager() as LinearLayoutManager;
                int position = manager.FindFirstCompletelyVisibleItemPosition();

                List<string> accountNumberList = accountsAdapter.GetAccountCardNumberListByPosition(position);
                this.presenter.LoadSummaryDetailsInBatch(accountNumberList);
			}
            UpdateAccountListIndicator();
		}


        void OnClickChanged(object sender, int position)
        {
            try
            {
                if (position != -1)
                {
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
                        StartActivityForResult(applySMRIntent, SSMR_METER_HISTORY_ACTIVITY_CODE);
                    }

                    try
                    {
                        FirebaseAnalyticsUtils.LogFragmentClickEvent(this, "My Service Tile Clicked");
                    }
                    catch (System.Exception err)
                    {
                        Utility.LoggingNonFatalError(err);
                    }
                }
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        void OnFAQClickChanged(object sender, int position)
        {
            try
            {
                if (position != -1)
                {
                    NewFAQ selectedNewFAQ = currentNewFAQList[position];
                    Intent faqIntent = new Intent(this.Activity, typeof(FAQListActivity));
                    faqIntent.PutExtra(Constants.FAQ_ID_PARAM, selectedNewFAQ.TargetItem);
                    Activity.StartActivity(faqIntent);

                    try
                    {
                        FirebaseAnalyticsUtils.LogFragmentClickEvent(this, "Need Help Tile Clicked");
                    }
                    catch (System.Exception err)
                    {
                        Utility.LoggingNonFatalError(err);
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

        private void ChangeMyServiceFAQTextColor()
        {
            try
            {
                if (accountsAdapter != null && accountsAdapter.accountCardModelList != null && myServiceAdapter != null)
                {
                    int count = accountsAdapter.accountCardModelList.Count;
                    if (count < 2)
                    {
                        myServiceTitle.SetTextColor(Color.White);
                    }
                    else
                    {
                        myServiceTitle.SetTextColor(Resources.GetColor(Resource.Color.powerBlue));
                    }
                    if (count < 1 && myServiceAdapter.ItemCount == 0)
                    {
                        newFAQTitle.SetTextColor(Color.White);
                    }
                    else
                    {
                        newFAQTitle.SetTextColor(Resources.GetColor(Resource.Color.powerBlue));
                    }
                }
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        private void ChangeMyServiceTextColor()
        {
            try
            {
                if (accountsAdapter != null && accountsAdapter.accountCardModelList != null)
                {
                    int count = accountsAdapter.accountCardModelList.Count;
                    if (count < 2)
                    {
                        myServiceTitle.SetTextColor(Color.White);
                    }
                    else
                    {
                        myServiceTitle.SetTextColor(Resources.GetColor(Resource.Color.powerBlue));
                    }
                }
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
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

            mMyServiceRetrySnakebar = Snackbar.Make(summaryRootView, msg, Snackbar.LengthIndefinite)
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
            MyServiceShimmerAdapter adapter = new MyServiceShimmerAdapter(this.presenter.LoadShimmerServiceList(6), this.Activity);
            myServiceShimmerList.SetAdapter(adapter);

            myServiceShimmerView.Visibility = ViewStates.Visible;
            myServiceView.Visibility = ViewStates.Gone;

            this.presenter.RetryMyService();
        }

        public void UpdateAccountListCards(List<SummaryDashBoardDetails> accountList)
        {
            accountsAdapter.UpdateAccountCards(accountList);
        }

        private void SetHeaderActionVisiblity(List<SummaryDashBoardDetails> accountList)
        {
            if (accountList.Count <= 5)
            {
                searchActionContainer.Visibility = ViewStates.Gone;
                searchEditText.Visibility = ViewStates.Gone;
            }
            else
            {
                searchActionContainer.Visibility = ViewStates.Visible;
            }
        }

        public void SetAccountListCards(List<SummaryDashBoardDetails> accountList)
        {
            SetHeaderActionVisiblity(accountList);
            accountsAdapter.SetAccountCards(accountList);
            accountsRecyclerView.SetAdapter(accountsAdapter);
            ChangeMyServiceTextColor();
        }

        public void SetAccountListCardsFromLocal(List<SummaryDashBoardDetails> accountList)
        {
            SetHeaderActionVisiblity(accountList);
            accountsAdapter.SetAccountCardsFromLocal(accountList);
            accountsRecyclerView.SetAdapter(accountsAdapter);
            ChangeMyServiceTextColor();
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
                CustomerBillingAccount.RemoveSelected();
                CustomerBillingAccount.SetSelected(accountNumber);

                if (mCallBack != null)
                {
                    mCallBack.NavigateToDashBoardFragment();
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
                        }
                        searchEditText.ClearFocus();
                        OnUpdateAccountListChanged(true);
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

        public void ShowRefreshScreen(string contentMsg, string buttonMsg)
        {
            myServiceTitle.SetTextColor(Resources.GetColor(Resource.Color.powerBlue));
            topRootView.SetBackgroundResource(Resource.Drawable.dashboard_home_refresh_bg);
            accountListRefreshContainer.Visibility = ViewStates.Visible;
            accountListViewContainer.Visibility = ViewStates.Gone;
            string refreshMsg = string.IsNullOrEmpty(contentMsg) ? "Uh oh, looks like this page is unplugged. Reload to stay plugged in!" : contentMsg;
            string refreshBtnTxt = string.IsNullOrEmpty(buttonMsg) ? "Reload Now" : buttonMsg;
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
            }
            else
            {
                btnRefresh.Visibility = ViewStates.Visible;
            }
        }

        private void OnStartLoadAccount()
        {
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
            }
            else
            {
                myServiceTitle.SetTextColor(Color.White);
                addActionContainer.Visibility = ViewStates.Gone;
                accountListContainer.Visibility = ViewStates.Gone;
                accountCard.Visibility = ViewStates.Visible;
            }

            UserSessions.SetSMRAccountList(currentSmrAccountList);
            UserSessions.SetSMREligibilityAccountList(eligibleSmrAccountList);

            topRootView.SetBackgroundResource(Resource.Drawable.dashboard_home_bg);
            accountListRefreshContainer.Visibility = ViewStates.Gone;
            accountListViewContainer.Visibility = ViewStates.Visible;
            if (MyTNBAccountManagement.GetInstance().IsNeedUpdatedBillingDetails())
            {
                UserSessions.SetRealSMREligibilityAccountList(eligibleSmrAccountList);
                OnLoadAccount();
            }
            else
            {
                this.presenter.LoadLocalAccounts();
            }
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
            OnStartLoadAccount();
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
            Intent linkAccount = new Intent(this.Activity, typeof(LinkAccountActivity));
            linkAccount.PutExtra("fromDashboard", true);
            StartActivity(linkAccount);
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
        public void UpdateSearchViewBackground(string searchText)
        {
            if (searchText != "")
            {
                searchEditText.SetBackgroundResource(Resource.Drawable.rectangle_rounded_corner_bg);
            }
            else
            {
                searchEditText.SetBackgroundResource(Resource.Drawable.search_edit_bg);
            }

        }

        void ViewTreeObserver.IOnGlobalLayoutListener.OnGlobalLayout()
        {
            try
            {
                if (isFirstInitiate)
                {
                    isFirstInitiate = false;
                    summaryNestScrollView.SmoothScrollTo(0, 0);
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

    }
}
