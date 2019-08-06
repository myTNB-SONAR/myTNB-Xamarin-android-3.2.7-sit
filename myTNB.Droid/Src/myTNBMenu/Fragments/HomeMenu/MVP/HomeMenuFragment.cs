
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

namespace myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.MVP
{
    public class HomeMenuFragment : BaseFragment, HomeMenuContract.IHomeMenuView
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


        AccountsRecyclerViewAdapter accountsAdapter;

        private string mSavedTimeStamp = "0000000";

        private string savedSSMRMeterReadingTimeStamp = "0000000";

        private string savedSSMRMeterReadingThreePhaseTimeStamp = "0000000";

        private static List<MyService> currentMyServiceList = new List<MyService>();

        private static List<NewFAQ> currentNewFAQList = new List<NewFAQ>();

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
            //MyTNBAccountManagement.GetInstance().SetMasterCustomerBillingAccountList();
        }

        public override void OnAttach(Context context)
        {
            base.OnAttach(context);
            try
            {
                mCallBack = context as ISummaryFragmentToDashBoardActivtyListener;
            }
            catch (Java.Lang.ClassCastException e)
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
                UpdateGreetingsHeader(this.presenter.GetGreeting());
                accountGreetingName.Text = this.presenter.GetAccountDisplay() + "!";
                SetNotificationIndicator();
                SetAccountsRecyclerView();
                SetAccountActionHeader();
                SetupMyServiceView();
                SetupNewFAQView();
                SetupNewPromotionView();
                TextViewUtils.SetMuseoSans500Typeface(myServiceTitle, newFAQTitle, newPromotionTitle);
                List<CustomerBillingAccount> customerBillingAccounts = CustomerBillingAccount.EligibleSMRAccountList();
                List<CustomerBillingAccount> list = CustomerBillingAccount.GetSortedCustomerBillingAccounts();
                List<SMRAccount> smrAccountList = new List<SMRAccount>();
                if (customerBillingAccounts.Count > 0)
                {
                    foreach (CustomerBillingAccount billingAccount in customerBillingAccounts)
                    {
                        SMRAccount smrAccount = new SMRAccount();
                        smrAccount.accountNumber = billingAccount.AccNum;
                        smrAccount.accountName = billingAccount.AccDesc;
                        smrAccount.accountAddress = billingAccount.AccountStAddress;
                        smrAccount.accountSelected = false;
                        smrAccountList.Add(smrAccount);
                    }
                    smrAccountList[0].accountSelected = true; //Default Selection
                }

                UserSessions.SetSMRAccountList(smrAccountList);
                if (MyTNBAccountManagement.GetInstance().IsNeedUpdatedBillingDetails())
                {
                    this.presenter.LoadAccounts();
                }
                else
                {
                    this.presenter.LoadLocalAccounts();
                }
                addActionImage.SetOnClickListener(null);
                notificationHeaderIcon.SetOnClickListener(null);
                addActionImage.Click += delegate
                {
                    Intent linkAccount = new Intent(this.Activity, typeof(LinkAccountActivity));
                    linkAccount.PutExtra("fromDashboard", true);
                    StartActivity(linkAccount);
                };
                notificationHeaderIcon.Click += delegate
                {
                    StartActivity(new Intent(this.Activity, typeof(NotificationActivity)));
                };
                ((DashboardHomeActivity)Activity).SetStatusBarBackground();

                this.presenter.GetSmartMeterReadingWalkthroughtTimeStamp();

                this.presenter.GetSmartMeterReadingThreePhaseWalkthroughtTimeStamp();
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public bool IsActive()
        {
            return IsAdded && IsVisible && !IsDetached && !IsRemoving;
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
                    newFAQShimmerAdapter = new NewFAQShimmerAdapter(this.presenter.LoadShimmerFAQList(3), this.Activity);
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
                searchActionIcon.Visibility = ViewStates.Gone;
                searchEditText.RequestFocus();
            }
            else
            {
                accountHeaderTitle.Visibility = ViewStates.Visible;
                searchEditText.Visibility = ViewStates.Gone;
                searchActionIcon.Visibility = ViewStates.Visible;
            }
        }

        private void SetAccountActionHeader()
        {
            TextViewUtils.SetMuseoSans500Typeface(accountHeaderTitle, accountGreeting, accountGreetingName);
            searchEditText.SetOnQueryTextListener(new AccountsSearchOnQueryTextListener(this,accountsAdapter));
            searchActionIcon.Click += (s, e) =>
            {
                ShowSearchAction(true);
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

            var act = this.Activity as AppCompatActivity;

            var actionBar = act.SupportActionBar;
            actionBar.Hide();
            ShowBackButton(false);

            this.presenter.InitiateService();
            SetNotificationIndicator();
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
                            applySMRIntent = new Intent(this.Activity, typeof(ApplicationFormSMRActivity));
                        }
                        else
                        {
                            applySMRIntent = new Intent(this.Activity, typeof(OnBoardingActivity));
                        }
                        StartActivity(applySMRIntent);
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
                searchActionIcon.Visibility = ViewStates.Gone;
                searchEditText.Visibility = ViewStates.Gone;
            }
            else
            {
                searchActionIcon.Visibility = ViewStates.Visible;
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
                CustomerBillingAccount.Update(accountNumber,true);

                if (mCallBack != null)
                {
                    mCallBack.NavigateToDashBoardFragment();
                    //ShowBackArrowIndicator()
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

        public void OnSearchOutFocus()
        {
            if (searchEditText != null)
            {
                if(searchEditText.Visibility == ViewStates.Visible)
                {
                    searchEditText.ClearFocus();
                    ShowSearchAction(false);
                    OnUpdateAccountListChanged(true);
                }
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
    }
}
