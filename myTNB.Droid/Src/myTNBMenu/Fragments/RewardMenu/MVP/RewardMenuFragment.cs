using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Support.V7.App;
using Android.Views;
using CheeseBind;
using myTNB_Android.Src.Base.Fragments;
using myTNB_Android.Src.myTNBMenu.Activity;
using myTNB_Android.Src.myTNBMenu.Fragments.RewardMenu.Adapter;
using myTNB_Android.Src.myTNBMenu.Fragments.RewardMenu.Model;
using myTNB_Android.Src.Utils;

namespace myTNB_Android.Src.myTNBMenu.Fragments.RewardMenu.MVP
{
    public class RewardMenuFragment : BaseFragment, RewardMenuContract.IRewardMenuView, ViewPager.IOnPageChangeListener
    {

        private bool isSiteCoreComplete = false;

        [BindView(Resource.Id.rewardsSlidingTabs)]
        TabLayout rewardsSlidingTabs;

        [BindView(Resource.Id.rewardViewPager)]
        ViewPager rewardViewPager;

        private RewardsTabAdapter mAdapter;

        RewardMenuContract.IRewardMenuPresenter presenter;

        private List<RewardMenuModel> mTabList = new List<RewardMenuModel>();

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            presenter = new RewardMenuPresenter(this, PreferenceManager.GetDefaultSharedPreferences(this.Activity));
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
                InitializeView();

                SetupTabIndicator((int) DPUtils.ConvertDPToPx(16f), (int)DPUtils.ConvertDPToPx(8f));
                HighLightCurrentTab(0);
                rewardViewPager.AddOnPageChangeListener(this);

                // TODO: Sitecore Load
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
            if (Build.VERSION.SdkInt >= Build.VERSION_CODES.JellyBeanMr1)
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
            HighLightCurrentTab(position);
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
    }
}
