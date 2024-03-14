using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences;



using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.ViewPager.Widget;
using CheeseBind;
using Google.Android.Material.Tabs;
using myTNB.Android.Src.Base.Fragments;
using myTNB.Android.Src.Database.Model;
using myTNB.Android.Src.myTNBMenu.Activity;
using myTNB.Android.Src.myTNBMenu.Fragments.RewardMenu.Adapter;
using myTNB.Android.Src.myTNBMenu.Fragments.RewardMenu.Model;
using myTNB.Android.Src.SavedRewards.MVP;
using myTNB.Android.Src.Utils;

namespace myTNB.Android.Src.myTNBMenu.Fragments.RewardMenu.MVP
{
    public class RewardMenuFragment : BaseFragment, RewardMenuContract.IRewardMenuView, ViewPager.IOnPageChangeListener
    {

        private bool isSiteCoreComplete = false;

        [BindView(Resource.Id.rewardMainLayout)]
        LinearLayout rewardMainLayout;

        [BindView(Resource.Id.rewardsSlidingTabs)]
        TabLayout rewardsSlidingTabs;

        [BindView(Resource.Id.rewardViewPager)]
        ViewPager rewardViewPager;

        [BindView(Resource.Id.rewardEmptyLayout)]
        LinearLayout rewardEmptyLayout;

        [BindView(Resource.Id.rewardEmptyImg)]
        ImageView rewardEmptyImg;

        [BindView(Resource.Id.txtEmptyReward)]
        TextView txtEmptyReward;

        [BindView(Resource.Id.rewardRefreshLayout)]
        LinearLayout rewardRefreshLayout;

        [BindView(Resource.Id.txtRefresh)]
        TextView txtRefresh;

        [BindView(Resource.Id.btnRefresh)]
        Button btnRefresh;

        private RewardsTabAdapter mAdapter;

        RewardMenuContract.IRewardMenuPresenter presenter;

        private List<RewardMenuModel> mTabList = new List<RewardMenuModel>();

        private string savedTimeStamp = "0000000";

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            this.HasOptionsMenu = true;
            presenter = new RewardMenuPresenter(this, PreferenceManager.GetDefaultSharedPreferences(this.Activity));

            RewardsMenuUtils.OnSetTouchDisable(false);
        }

        public override void OnAttach(Context context)
        {
            base.OnAttach(context);
            try
            {
                FirebaseAnalyticsUtils.SetFragmentScreenName(this, "Rewards");
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

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            try
            {
                RewardsMenuUtils.OnSetTouchDisable(true);

                InitializeView();

                if (mTabList != null && mTabList.Count > 0)
                {
                    SetupTabIndicator((int)DPUtils.ConvertDPToPx(16f), (int)DPUtils.ConvertDPToPx(10f));
                    MeasureTabScroll();
                    HighLightCurrentTab(0);
                    rewardViewPager.AddOnPageChangeListener(this);
                }
                rewardViewPager.OverScrollMode = OverScrollMode.Never;

                TextViewUtils.SetMuseoSans300Typeface(txtEmptyReward, txtRefresh);
                TextViewUtils.SetMuseoSans500Typeface(btnRefresh);
                TextViewUtils.SetTextSize14(txtEmptyReward);
                TextViewUtils.SetTextSize16(txtRefresh, btnRefresh);

                rewardMainLayout.Visibility = ViewStates.Visible;
                rewardEmptyLayout.Visibility = ViewStates.Gone;
                rewardRefreshLayout.Visibility = ViewStates.Gone;

                OnGetRewardTimestamp();
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

            try
            {
                ((DashboardHomeActivity)Activity).SetToolbarBackground(Resource.Drawable.CustomGradientToolBar);
                ((DashboardHomeActivity)Activity).SetStatusBarBackground(Resource.Drawable.UsageGradientBackground);
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

            try
            {
                RewardsMenuUtils.OnResetUpdateList();
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void OnGetRewardTimestamp()
        {
            try
            {
                if (RewardsMenuUtils.GetRewardLoading())
                {
                    _ = this.presenter.OnRecheckRewardsStatus();
                }
                else
                {
                    this.presenter.GetRewardsTimeStamp();
                }
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

        public override void OnPause()
        {
            base.OnPause();
            NewAppTutorialUtils.ForceCloseNewAppTutorial();
        }

        public override void OnResume()
        {
            base.OnResume();

            try
            {
                var act = this.Activity as AppCompatActivity;

                var actionBar = act.SupportActionBar;
                actionBar.Show();
                ShowBackButton(false);

                ((DashboardHomeActivity)this.Activity).RemoveHeaderDropDown();
                ((DashboardHomeActivity)this.Activity).HideAccountName();
                ((DashboardHomeActivity)this.Activity).SetToolBarTitle(Utility.GetLocalizedLabel("Tabbar", "rewards"));
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

            try
            {
                if (RewardsMenuUtils.GetRefreshAll())
                {
                    RewardsMenuUtils.OnResetUpdateList();
                    if (mTabList != null && mTabList.Count > 0)
                    {
                        for (int i = 0; i < mTabList.Count; i++)
                        {
                            if (mTabList[i].Fragment.IsActive())
                            {
                                mTabList[i].Fragment.Refresh();
                            }
                        }
                    }
                }
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

            try
            {
                if (mTabList != null && mTabList.Count > 0 && mTabList[0].FragmentListMode == Constants.REWARDSITEMLISTMODE.LOADED)
                {
                    if (!UserSessions.HasRewardsShown(PreferenceManager.GetDefaultSharedPreferences(this.Activity)))
                    {
                        Handler h = new Handler();
                        Action myAction = () =>
                        {
                            NewAppTutorialUtils.ForceCloseNewAppTutorial();
                            OnShowRewardMenuTutorial();
                        };
                        h.PostDelayed(myAction, 50);
                    }
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
            return Resource.Layout.RewardListView;
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

        private IMenu menu;
        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            inflater.Inflate(Resource.Menu.RewardsToolBarMenu, menu);
            this.menu = menu;
            base.OnCreateOptionsMenu(menu, inflater);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.action_menu_reward:
                    if (!this.GetIsClicked())
                    {
                        this.SetIsClicked(true);
                        Intent activityIntent = new Intent(this.Activity, typeof(SavedRewardsActivity));
                        this.StartActivity(activityIntent);
                    }
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }

        private void HighLightCurrentTab(int position)
        {
            try
            {
                for (int i = 0; i < rewardsSlidingTabs.TabCount; i++)
                {
                    if (i != position)
                    {
                        TabLayout.Tab currentTab = rewardsSlidingTabs.GetTabAt(i);
                        currentTab.SetCustomView(null);
                        currentTab.SetCustomView(mAdapter.GetTabView(i));
                    }
                }
                TabLayout.Tab currentHightlightTab = rewardsSlidingTabs.GetTabAt(position);
                currentHightlightTab.SetCustomView(null);
                currentHightlightTab.SetCustomView(mAdapter.GetSelectedTabView(position));
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void SetupTabIndicator(int externalMargin, int internalMargin)
        {
            try
            {
                View TabStrip = rewardsSlidingTabs.GetChildAt(0);

                ViewGroup TabStripGroup = (ViewGroup)TabStrip;

                int childCount = ((ViewGroup)TabStrip).ChildCount;

                for (int i = 0; i < childCount; i++)
                {
                    View tabView = TabStripGroup.GetChildAt(i);
                    tabView.SetMinimumWidth(0);
                    tabView.SetPadding(0, tabView.PaddingTop, 0, tabView.PaddingBottom);
                    ViewGroup.MarginLayoutParams layoutParams = (ViewGroup.MarginLayoutParams)tabView.LayoutParameters;
                    if (i == 0)
                    {
                        // left
                        SettingMargin(layoutParams, externalMargin, internalMargin);
                    }
                    else if (i == childCount - 1)
                    {
                        // right
                        SettingMargin(layoutParams, internalMargin, externalMargin);
                    }
                    else
                    {
                        // internal
                        SettingMargin(layoutParams, internalMargin, internalMargin);
                    }
                }

                rewardsSlidingTabs.RequestLayout();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        private void SettingMargin(ViewGroup.MarginLayoutParams layoutParams, int start, int end)
        {
            if (Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.JellyBeanMr1)
            {
                layoutParams.MarginStart = start;
                layoutParams.MarginEnd = end;
                layoutParams.LeftMargin = start;
                layoutParams.RightMargin = end;
            }
            else
            {
                layoutParams.LeftMargin = start;
                layoutParams.RightMargin = end;
            }
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            this.presenter.OnCancelTask();
            mAdapter.ClearAll();
        }

        void ViewPager.IOnPageChangeListener.OnPageScrollStateChanged(int state)
        {

        }

        void ViewPager.IOnPageChangeListener.OnPageScrolled(int position, float positionOffset, int positionOffsetPixels)
        {

        }

        void ViewPager.IOnPageChangeListener.OnPageSelected(int position)
        {
            if (mTabList != null && mTabList.Count > 0)
            {
                HighLightCurrentTab(position);
                try
                {
                    RewardMenuModel currentModel = mTabList[position];
                    if (currentModel.FragmentListMode == Constants.REWARDSITEMLISTMODE.LOADED
                        && RewardsMenuUtils.OnCheckIsUpdateNeed(currentModel.FragmentSearchString))
                    {
                        currentModel.Fragment.Refresh();
                    }
                }
                catch (Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                }
            }
        }

        private void InitializeView()
        {
            mAdapter = new RewardsTabAdapter(this.FragmentManager, this.Activity);

            mTabList = this.presenter.InitializeRewardView();

            for (int i = 0; i < mTabList.Count; i++)
            {
                Bundle bundle = new Bundle();
                bundle.PutInt(Constants.REWARDS_ITEM_LIST_MODE, (int)mTabList[i].FragmentListMode);
                mTabList[i].Fragment.Arguments = bundle;
                mAdapter.AddFragment(mTabList[i].Fragment, mTabList[i].TabTitle);
            }

            rewardViewPager.Adapter = mAdapter;
            rewardsSlidingTabs.SetupWithViewPager(rewardViewPager);
        }

        public void OnSavedRewardsTimeStamp(string mSavedTimeStamp)
        {
            if (mSavedTimeStamp != null)
            {
                this.savedTimeStamp = mSavedTimeStamp;
            }
            this.presenter.OnGetRewardsTimeStamp();
        }

        public void OnSetResultTabView(List<RewardMenuModel> list)
        {
            try
            {
                Activity.RunOnUiThread(() =>
                {
                    try
                    {
                        mAdapter.ClearAll();

                        mTabList = list;

                        for (int i = 0; i < mTabList.Count; i++)
                        {
                            Bundle bundle = new Bundle();
                            bundle.PutInt(Constants.REWARDS_ITEM_LIST_MODE, (int)mTabList[i].FragmentListMode);
                            bundle.PutString(Constants.REWARDS_ITEM_LIST_SEARCH_STRING_KEY, mTabList[i].FragmentSearchString);

                            mTabList[i].Fragment.Arguments = bundle;
                            mAdapter.AddFragment(mTabList[i].Fragment, mTabList[i].TabTitle);
                        }

                        rewardViewPager.Adapter = mAdapter;
                        rewardsSlidingTabs.SetupWithViewPager(rewardViewPager);

                        if (mTabList != null && mTabList.Count > 1)
                        {
                            SetupTabIndicator((int)DPUtils.ConvertDPToPx(16f), (int)DPUtils.ConvertDPToPx(10f));
                            MeasureTabScroll();
                            HighLightCurrentTab(0);
                        }

                        if (mTabList == null || (mTabList != null && mTabList.Count <= 1))
                        {
                            rewardsSlidingTabs.Visibility = ViewStates.Gone;
                        }
                        else
                        {
                            rewardsSlidingTabs.Visibility = ViewStates.Visible;
                        }

                        try
                        {
                            if (RewardsEntity.HasUnread())
                            {
                                ((DashboardHomeActivity)this.Activity).ShowUnreadRewards();
                            }
                            else
                            {
                                ((DashboardHomeActivity)this.Activity).HideUnreadRewards();
                            }
                        }
                        catch (System.Exception ex)
                        {
                            Utility.LoggingNonFatalError(ex);
                        }

                        RewardsMenuUtils.OnSetTouchDisable(false);

                        try
                        {
                            if (!UserSessions.HasRewardsShown(PreferenceManager.GetDefaultSharedPreferences(this.Activity)))
                            {
                                Handler h = new Handler();
                                Action myAction = () =>
                                {
                                    NewAppTutorialUtils.ForceCloseNewAppTutorial();
                                    OnShowRewardMenuTutorial();
                                };
                                h.PostDelayed(myAction, 50);
                            }
                        }
                        catch (System.Exception exp)
                        {
                            Utility.LoggingNonFatalError(exp);
                        }
                    }
                    catch (System.Exception err)
                    {
                        Utility.LoggingNonFatalError(err);
                    }
                });
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        private void MeasureTabScroll()
        {
            try
            {
                Activity.RunOnUiThread(() =>
                {
                    try
                    {
                        Action myAction = () =>
                        {
                            try
                            {
                                Activity.RunOnUiThread(() =>
                                {
                                    try
                                    {
                                        int widthS = DPUtils.GetWidth();
                                        rewardsSlidingTabs.Measure((int)MeasureSpecMode.Unspecified, (int)MeasureSpecMode.Unspecified);
                                        int widthT = rewardsSlidingTabs.MeasuredWidth;

                                        if (widthS > widthT)
                                        {
                                            int diff = widthS - widthT;
                                            int diffOnEachItem = diff / mTabList.Count;
                                            SetupTabIndicator((int)DPUtils.ConvertDPToPx(16f) + diffOnEachItem / 2, (int)DPUtils.ConvertDPToPx(10f) + diffOnEachItem / 2);
                                        }
                                    }
                                    catch (Exception er)
                                    {
                                        Utility.LoggingNonFatalError(er);
                                    }
                                });
                            }
                            catch (Exception ex)
                            {
                                Utility.LoggingNonFatalError(ex);
                            }
                        };
                        rewardsSlidingTabs.Post(myAction);
                    }
                    catch (Exception e)
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

        public void CheckRewardsTimeStamp(string mTimeStemp)
        {
            try
            {
                if (mTimeStemp != null)
                {
                    if (!mTimeStemp.Equals(savedTimeStamp))
                    {
                        this.presenter.OnGetRewards();
                    }
                    else
                    {
                        RewardsEntity wtItemManager = new RewardsEntity();
                        List<RewardsEntity> subItems = wtItemManager.GetAllItems();
                        if (subItems != null && subItems.Count > 0)
                        {
                            _ = this.presenter.OnGetUserRewardList();
                        }
                        else
                        {
                            this.presenter.OnGetRewards();
                        }
                    }
                }
                else
                {
                    this.presenter.OnGetRewards();
                }
            }
            catch (System.Exception e)
            {
                this.presenter.OnGetRewards();
                Utility.LoggingNonFatalError(e);
            }
        }

        public void SetEmptyView()
        {
            try
            {
                Activity.RunOnUiThread(() =>
                {
                    try
                    {
                        rewardMainLayout.Visibility = ViewStates.Gone;

                        rewardEmptyLayout.Visibility = ViewStates.Visible;

                        rewardRefreshLayout.Visibility = ViewStates.Gone;

                        txtEmptyReward.Text = Utility.GetLocalizedLabel("Rewards", "noRewards");

                        LinearLayout.LayoutParams rewardEmptyImgParams = rewardEmptyImg.LayoutParameters as LinearLayout.LayoutParams;
                        rewardEmptyImgParams.TopMargin = GetDeviceVerticalScaleInPixel(0.155f);
                        rewardEmptyImgParams.Width = GetDeviceHorizontalScaleInPixel(0.319f);
                        rewardEmptyImgParams.Height = GetDeviceVerticalScaleInPixel(0.165f);
                        rewardEmptyImg.RequestLayout();

                        IMenuItem item = this.menu.FindItem(Resource.Id.action_menu_reward);
                        if (item != null)
                        {
                            item.SetVisible(false);
                        }

                        RewardsMenuUtils.OnSetTouchDisable(false);

                        try
                        {
                            if (RewardsEntity.HasUnread())
                            {
                                ((DashboardHomeActivity)this.Activity).ShowUnreadRewards();
                            }
                            else
                            {
                                ((DashboardHomeActivity)this.Activity).HideUnreadRewards();
                            }
                        }
                        catch (System.Exception ex)
                        {
                            Utility.LoggingNonFatalError(ex);
                        }
                    }
                    catch (System.Exception e)
                    {
                        Utility.LoggingNonFatalError(e);
                    }
                });
            }
            catch (Exception er)
            {
                Utility.LoggingNonFatalError(er);
            }
        }

        public void SetRefreshView(string buttonText, string messageText)
        {
            try
            {
                Activity.RunOnUiThread(() =>
                {
                    try
                    {
                        rewardMainLayout.Visibility = ViewStates.Gone;

                        rewardEmptyLayout.Visibility = ViewStates.Gone;

                        rewardRefreshLayout.Visibility = ViewStates.Visible;

                        if (!string.IsNullOrEmpty(buttonText))
                        {
                            btnRefresh.Text = buttonText;
                        }
                        else
                        {
                            btnRefresh.Text = Utility.GetLocalizedCommonLabel("refreshNow");
                        }

                        if (!string.IsNullOrEmpty(messageText))
                        {
                            txtRefresh.Text = messageText;
                        }
                        else
                        {
                            txtRefresh.Text = Utility.GetLocalizedCommonLabel("refreshDescription");
                        }

                        IMenuItem item = this.menu.FindItem(Resource.Id.action_menu_reward);
                        if (item != null)
                        {
                            item.SetVisible(false);
                        }

                        RewardsMenuUtils.OnSetTouchDisable(false);

                        try
                        {
                            if (RewardsEntity.HasUnread())
                            {
                                ((DashboardHomeActivity)this.Activity).ShowUnreadRewards();
                            }
                            else
                            {
                                ((DashboardHomeActivity)this.Activity).HideUnreadRewards();
                            }
                        }
                        catch (System.Exception ex)
                        {
                            Utility.LoggingNonFatalError(ex);
                        }
                    }
                    catch (System.Exception e)
                    {
                        Utility.LoggingNonFatalError(e);
                    }
                });
            }
            catch (System.Exception err)
            {
                Utility.LoggingNonFatalError(err);
            }
        }

        public void OnShowRewardMenuTutorial()
        {
            if (!UserSessions.HasRewardsShown(PreferenceManager.GetDefaultSharedPreferences(this.Activity)))
            {
                Handler h = new Handler();
                Action myAction = () =>
                {
                    NewAppTutorialUtils.OnShowNewAppTutorial(this.Activity, this, PreferenceManager.GetDefaultSharedPreferences(this.Activity), this.presenter.OnGeneraNewAppTutorialList());
                };
                h.PostDelayed(myAction, 100);
            }
        }

        public void StopScrolling()
        {
            try
            {
                if (mTabList != null && mTabList.Count > 0)
                {
                    mTabList[0].Fragment.StopScrolling();
                }
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public List<RewardMenuModel> GetTabList()
        {
            return mTabList;
        }

        public bool CheckTabVisibility()
        {
            return rewardsSlidingTabs.Visibility == ViewStates.Visible;
        }

        public int GetTabRelativePosition()
        {
            int i = 0;

            try
            {
                int[] location = new int[2];
                rewardsSlidingTabs.GetLocationOnScreen(location);
                i = location[1];
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

            return i;
        }

        public int GetTabHeight()
        {
            int i = 0;

            try
            {
                i = rewardsSlidingTabs.Height;
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

            return i;
        }

        [OnClick(Resource.Id.btnRefresh)]
        internal void OnRefresh(object sender, EventArgs e)
        {
            try
            {
                ((DashboardHomeActivity)Activity).ShowRewardsMenu();
            }
            catch (System.Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }
    }
}
