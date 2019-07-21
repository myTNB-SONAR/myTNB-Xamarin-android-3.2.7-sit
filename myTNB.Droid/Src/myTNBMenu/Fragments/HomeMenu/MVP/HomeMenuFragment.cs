
using System;
using System.Collections.Generic;
using System.Linq;
using Android.Graphics;
using Android.OS;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using CheeseBind;
using Facebook.Shimmer;
using myTNB_Android.Src.Base.Fragments;
using myTNB_Android.Src.myTNBMenu.Activity;
using myTNB_Android.Src.myTNBMenu.Fragments.FeedbackMenu;
using myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.Adapter;
using myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.Listener;
using myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.Models;
using myTNB_Android.Src.Utils;

namespace myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.MVP
{
    public class HomeMenuFragment : BaseFragment, HomeMenuContract.IView, NestedScrollView.IOnScrollChangeListener
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

        //[BindView(Resource.Id.shimmer_view_container)]
        //ShimmerFrameLayout shimmerViewContainer;
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

        AccountsRecyclerViewAdapter accountsAdapter;

        HomeMenuContract.IUserActionsListener userActionsListener;

        private bool isTextViewColorUpdateNeeded = true;

        private HomeMenuPresenter mPresenter;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            try
            {
                mPresenter = new HomeMenuPresenter(this);
                TextViewUtils.SetMuseoSans500Typeface(myServiceTitle, newFAQTitle);
                myServiceTitle.SetTextColor(Color.White);
                SetAccountsRecyclerView();
                SetAccountActionHeader();
                summaryNestScrollView.SetOnScrollChangeListener(this);
                this.userActionsListener.Start();
                ChangeMyServiceTextColor(false, 0);
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void SetPresenter(HomeMenuContract.IUserActionsListener userActionListener)
        {
           this.userActionsListener = userActionListener;
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

            MyServiceShimmerAdapter adapter = new MyServiceShimmerAdapter(this.userActionsListener.LoadShimmerServiceList(6));
            myServiceShimmerList.SetAdapter(adapter);

            myServiceShimmerView.Visibility = ViewStates.Visible;
            myServiceView.Visibility = ViewStates.Gone;
            this.userActionsListener.InitiateMyService();
            
        }

        public void SetMyServiceResult(List<MyService> list)
        {
            MyServiceShimmerAdapter shimmerAdapter = new MyServiceShimmerAdapter(null);
            myServiceShimmerList.SetAdapter(shimmerAdapter);
            myServiceShimmerView.Visibility = ViewStates.Gone;
            myServiceView.Visibility = ViewStates.Visible;
            MyServiceAdapter adapter = new MyServiceAdapter(list);
            myServiceListRecycleView.SetAdapter(adapter);
            adapter.ClickChanged += OnClickChanged;
        }

        public void SetNewFAQRecycleView()
        {
            LinearLayoutManager linearLayoutManager = new LinearLayoutManager(this.Activity, LinearLayoutManager.Horizontal, false);
            newFAQListRecycleView.SetLayoutManager(linearLayoutManager);

            LinearLayoutManager linearShimmerLayoutManager = new LinearLayoutManager(this.Activity, LinearLayoutManager.Horizontal, false);
            newFAQShimmerList.SetLayoutManager(linearShimmerLayoutManager);

            NewFAQShimmerAdapter adapter = new NewFAQShimmerAdapter(this.userActionsListener.LoadShimmerFAQList(3));
            newFAQShimmerList.SetAdapter(adapter);

            newFAQShimmerView.Visibility = ViewStates.Visible;
            newFAQView.Visibility = ViewStates.Gone;

            this.userActionsListener.InitiateNewFAQ();
        }

        public void SetNewFAQResult(List<NewFAQ> list)
        {
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

            accountsAdapter = new AccountsRecyclerViewAdapter(this,16);
            accountsAdapter.SetAccountCards(13);
            accountsRecyclerView.SetAdapter(accountsAdapter);

            accountsRecyclerView.AddOnScrollListener(new AccountsRecyclerViewOnScrollListener(linearLayoutManager, indicatorContainer));
        }
        private void SetShimmer()
        {
            //var shimmerBuilder = new Shimmer.AlphaHighlightBuilder();
            //shimmerBuilder = default(Shimmer.AlphaHighlightBuilder);
            //shimmerViewContainer.SetShimmer(shimmerBuilder?.Build());
        }

        public override void OnResume()
        {
            base.OnResume();

            var act = this.Activity as AppCompatActivity;

            var actionBar = act.SupportActionBar;
            actionBar.Hide();
            ShowBackButton(false);
            //var shimmerBuilder = new Shimmer.AlphaHighlightBuilder();
            //shimmerBuilder = default(Shimmer.AlphaHighlightBuilder);
            //shimmerViewContainer.SetShimmer(shimmerBuilder?.Build());
            //shimmerViewContainer.StartShimmer();
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);

            return base.OnCreateView(inflater, container, savedInstanceState);
        }

        public override int ResourceId()
        {
            return Resource.Layout.HomeMenuFragmentView;
        }
        private void UpdateAccountListIndicator()
        {
            indicatorContainer.RemoveAllViews();
            int accountsCount = accountsAdapter.ItemCount;// accountCardModelList.Count;
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
            ChangeMyServiceTextColor(false, 0);
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
                    // Toast.MakeText(this.Activity, "My Service Position Unknown", ToastLength.Long).Show();
                }
                else
                {
                    if (position == 2)
                    {
                        ShowFeedbackMenu();
                    }
                    // Toast.MakeText(this.Activity, "My Service Position: " + position.ToString(), ToastLength.Long).Show();
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

        public void OnScrollChange(NestedScrollView v, int scrollX, int scrollY, int oldScrollX, int oldScrollY)
        {
            float currentDP = scrollY / Resources.DisplayMetrics.Density;
            ChangeMyServiceTextColor(true, currentDP);

        }

        private void ChangeMyServiceTextColor(bool onScroll, float currentDP)
        {
            try
            {
                int count = accountsAdapter.accountCardModelList.Count;
                if (!onScroll)
                {
                    if (count <= 2)
                    {
                        isTextViewColorUpdateNeeded = false;
                        myServiceTitle.SetTextColor(Color.White);
                    }
                    else
                    {
                        isTextViewColorUpdateNeeded = true;
                        myServiceTitle.SetTextColor(Resources.GetColor(Resource.Color.powerBlue));
                    }
                }
                else
                {
                    if (isTextViewColorUpdateNeeded)
                    {
                        if (count > 5)
                        {
                            if (currentDP < Constants.ACCOUNT_LIST_MORE_THAN_FIVE_DP_LIMIT)
                            {
                                myServiceTitle.SetTextColor(Resources.GetColor(Resource.Color.powerBlue));
                            }
                            else
                            {
                                myServiceTitle.SetTextColor(Color.White);
                            }
                        }
                        else if (count == 5)
                        {
                            if (currentDP < Constants.ACCOUNT_LIST_FIVE_DP_LIMIT)
                            {
                                myServiceTitle.SetTextColor(Resources.GetColor(Resource.Color.powerBlue));
                            }
                            else
                            {
                                myServiceTitle.SetTextColor(Color.White);
                            }
                        }
                        else if (count > 2 && count < 5)
                        {
                            if (count == 3)
                            {
                                if (currentDP < Constants.ACCOUNT_LIST_THREE_DP_LIMIT)
                                {
                                    myServiceTitle.SetTextColor(Resources.GetColor(Resource.Color.powerBlue));
                                }
                                else
                                {
                                    myServiceTitle.SetTextColor(Color.White);
                                }
                            }
                            else
                            {
                                if (currentDP < Constants.ACCOUNT_LIST_FOUR_DP_LIMIT)
                                {
                                    myServiceTitle.SetTextColor(Resources.GetColor(Resource.Color.powerBlue));
                                }
                                else
                                {
                                    myServiceTitle.SetTextColor(Color.White);
                                }
                            }
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
