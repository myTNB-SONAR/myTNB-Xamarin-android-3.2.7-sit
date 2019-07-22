
using System;
using System.Collections.Generic;
using System.Linq;
using Android.Graphics;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using CheeseBind;
using Facebook.Shimmer;
using myTNB_Android.Src.Base.Fragments;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.myTNBMenu.Activity;
using myTNB_Android.Src.myTNBMenu.Fragments.FeedbackMenu;
using myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.Adapter;
using myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.Listener;
using myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.MVP.Models;
using myTNB_Android.Src.SummaryDashBoard.Models;
using myTNB_Android.Src.Utils;
using Refit;

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

        [BindView(Resource.Id.addAction)]
        ImageView addAccountActionIcon;

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

        [BindView(Resource.Id.shimmerMyServiceView)]
        ShimmerFrameLayout shimmerMyServiceView;

        [BindView(Resource.Id.shimmerFAQView)]
        ShimmerFrameLayout shimmerFAQView;


        AccountsRecyclerViewAdapter accountsAdapter;



        private static List<MyService> currentMyServiceList = new List<MyService>();

        HomeMenuContract.IHomeMenuPresenter presenter;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            presenter = new HomeMenuPresenter(this);
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

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            try
            {
                UpdateGreetingsHeader(this.presenter.GetGreeting());
                accountGreetingName.Text = this.presenter.GetAccountDisplay();
                SetAccountsRecyclerView();
                SetAccountActionHeader();
                SetMyServiceRecycleView();
                SetNewFAQRecycleView();
                TextViewUtils.SetMuseoSans500Typeface(myServiceTitle, newFAQTitle);

                this.presenter.LoadAccounts();
                this.presenter.InitiateService();
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

        public void SetMyServiceRecycleView()
        {
            GridLayoutManager layoutManager = new GridLayoutManager(this.Activity, 3);
            layoutManager.Orientation = RecyclerView.Vertical;
            myServiceListRecycleView.SetLayoutManager(layoutManager);

            GridLayoutManager layoutShimmerManager = new GridLayoutManager(this.Activity, 3);
            layoutShimmerManager.Orientation = RecyclerView.Vertical;
            myServiceShimmerList.SetLayoutManager(layoutShimmerManager);

            MyServiceShimmerAdapter adapter = new MyServiceShimmerAdapter(this.presenter.LoadShimmerServiceList(6));
            myServiceShimmerList.SetAdapter(adapter);

            myServiceShimmerView.Visibility = ViewStates.Visible;
            myServiceView.Visibility = ViewStates.Gone;
            var shimmerBuilder = ShimmerUtils.ShimmerBuilderConfig();
            if (shimmerBuilder != null)
            {
                shimmerMyServiceView.SetShimmer(shimmerBuilder?.Build());
            }
            shimmerMyServiceView.StartShimmer();
            this.presenter.InitiateMyService();
            
        }

        public void SetMyServiceResult(List<MyService> list)
        {
            shimmerMyServiceView.StopShimmer();
            MyServiceShimmerAdapter shimmerAdapter = new MyServiceShimmerAdapter(null);
            myServiceShimmerList.SetAdapter(shimmerAdapter);
            myServiceShimmerView.Visibility = ViewStates.Gone;
            myServiceView.Visibility = ViewStates.Visible;
            MyServiceAdapter adapter = new MyServiceAdapter(list);
            myServiceListRecycleView.SetAdapter(adapter);
            currentMyServiceList.AddRange(list);
            adapter.ClickChanged += OnClickChanged;
        }

        public void SetNewFAQRecycleView()
        {
            LinearLayoutManager linearLayoutManager = new LinearLayoutManager(this.Activity, LinearLayoutManager.Horizontal, false);
            newFAQListRecycleView.SetLayoutManager(linearLayoutManager);

            LinearLayoutManager linearShimmerLayoutManager = new LinearLayoutManager(this.Activity, LinearLayoutManager.Horizontal, false);
            newFAQShimmerList.SetLayoutManager(linearShimmerLayoutManager);

            NewFAQShimmerAdapter adapter = new NewFAQShimmerAdapter(this.presenter.LoadShimmerFAQList(3));
            newFAQShimmerList.SetAdapter(adapter);

            newFAQShimmerView.Visibility = ViewStates.Visible;
            newFAQView.Visibility = ViewStates.Gone;
            var shimmerBuilder = ShimmerUtils.ShimmerBuilderConfig();
            if (shimmerBuilder != null)
            {
                shimmerFAQView.SetShimmer(shimmerBuilder?.Build());
            }
            shimmerFAQView.StartShimmer();
            this.presenter.InitiateNewFAQ();
        }

        public void SetNewFAQResult(List<NewFAQ> list)
        {
            shimmerFAQView.StopShimmer();
            NewFAQShimmerAdapter shimmerAdapter = new NewFAQShimmerAdapter(null);
            newFAQShimmerList.SetAdapter(shimmerAdapter);
            newFAQShimmerView.Visibility = ViewStates.Gone;
            newFAQView.Visibility = ViewStates.Visible;
            NewFAQAdapter adapter = new NewFAQAdapter(list);
            newFAQListRecycleView.SetAdapter(adapter);
            adapter.ClickChanged += OnFAQClickChanged;
        }


        public void ShowSearchAction(bool isShow)
        {
            if (isShow)
            {
                accountHeaderTitle.Visibility = ViewStates.Gone;
                searchEditText.Visibility = ViewStates.Visible;
                searchActionIcon.Visibility = ViewStates.Gone;
                searchEditText.ClearFocus();
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

            addAccountActionIcon.Click += (s, e) =>
            {
                ShowSearchAction(false);
            };
        }

        private void SetAccountsRecyclerView()
        {
            LinearLayoutManager linearLayoutManager = new LinearLayoutManager(Activity, LinearLayoutManager.Horizontal, false);
            accountsRecyclerView.SetLayoutManager(linearLayoutManager);

            accountsAdapter = new AccountsRecyclerViewAdapter(this);
            accountsRecyclerView.AddOnScrollListener(new AccountsRecyclerViewOnScrollListener(linearLayoutManager, indicatorContainer));

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
                    layoutParams.RightMargin = 5;
                    layoutParams.LeftMargin = 5;
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
            ChangeMyServiceTextColor();
        }

        public void OnUpdateAccountListChanged(bool isSearchSubmit)
		{
            if (isSearchSubmit)
            {
                ShowSearchAction(false);
            }
            UpdateAccountListIndicator();
		}


        void OnClickChanged(object sender, int position)
        {
            try
            {
                if (position == -1)
                {

                }
                else
                {
                    MyService selectedService = currentMyServiceList[position];
                    if (selectedService.ServiceCategoryId == "1003")
                    {
                        ShowFeedbackMenu();
                    }
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        void OnFAQClickChanged(object sender, int position)
        {
            try
            {
                if (position == -1)
                {
                    // Toast.MakeText(this.Activity, "FAQ Position Unknown", ToastLength.Long).Show();
                }
                else
                {
                    // Toast.MakeText(this.Activity, "FAQ Position: " + position.ToString(), ToastLength.Long).Show();
                }
            }
            catch (Exception e)
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

        private void ChangeMyServiceTextColor()
        {
            try
            {
                int count = accountsAdapter.accountCardModelList.Count;
                if (count <= 2)
                {
                    myServiceTitle.SetTextColor(Color.White);
                }
                else
                {
                    myServiceTitle.SetTextColor(Resources.GetColor(Resource.Color.powerBlue));
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
            MyServiceShimmerAdapter adapter = new MyServiceShimmerAdapter(this.presenter.LoadShimmerServiceList(6));
            myServiceShimmerList.SetAdapter(adapter);

            myServiceShimmerView.Visibility = ViewStates.Visible;
            myServiceView.Visibility = ViewStates.Gone;

            this.presenter.RetryMyService();
        }

        public void UpdateAccountListCards(List<SummaryDashBoardDetails> accountList)
        {
            accountsAdapter.UpdateAccountCards(accountList);
            accountsAdapter.NotifyDataSetChanged();
        }

        public void SetAccountListCards(List<SummaryDashBoardDetails> accountList)
        {
            accountsAdapter.SetAccountCards(accountList);
            accountsRecyclerView.SetAdapter(accountsAdapter);
        }
    }
}
