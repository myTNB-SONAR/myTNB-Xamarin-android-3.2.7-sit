using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Preferences;
using Android.Support.Design.Widget;
using Android.Support.V4.Content;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using CheeseBind;
using myTNB.SitecoreCMS.Model;
using myTNB_Android.Src.Base;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.RewardDetail.MVP;
using myTNB_Android.Src.SavedRewards.Adapter;
using myTNB_Android.Src.SSMR.Util;
using myTNB_Android.Src.Utils;
using myTNB_Android.Src.Utils.Custom.ProgressDialog;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime;

namespace myTNB_Android.Src.SavedRewards.MVP
{
    [Activity(Label = "My Saved Rewards"
              , Icon = "@drawable/ic_launcher"
      , ScreenOrientation = ScreenOrientation.Portrait
      , Theme = "@style/Theme.Dashboard")]
    public class SavedRewardsActivity : BaseToolbarAppCompatActivity, SavedRewardsContract.ISavedRewardsView
    {
        [BindView(Resource.Id.rewardRecyclerView)]
        RecyclerView mRewardsRecyclerView;

        List<RewardsModel> items;

        SavedRewardsRecyclerAdapter mRewardsRecyclerAdapter;

        SavedRewardsContract.ISavedRewardsPresenter presenter;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            try
            {
                presenter = new SavedRewardsPresenter(this, PreferenceManager.GetDefaultSharedPreferences(this));
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        protected override void OnStart()
        {
            base.OnStart();

            try
            {
                LinearLayoutManager linearLayoutManager = new LinearLayoutManager(this, LinearLayoutManager.Vertical, false);
                mRewardsRecyclerView.SetLayoutManager(linearLayoutManager);
                ((SimpleItemAnimator)mRewardsRecyclerView.GetItemAnimator()).SupportsChangeAnimations = false;
                mRewardsRecyclerView.OverScrollMode = OverScrollMode.Never;
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

            try
            {
                SetToolbarBackground(Resource.Drawable.CustomGradientToolBar);
                SetStatusBarBackground(Resource.Drawable.UsageGradientBackground);
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public override Boolean ShowCustomToolbarTitle()
        {
            return true;
        }

        public bool IsActive()
        {
            return Window.DecorView.RootView.IsShown;
        }

        public override int ResourceId()
        {
            return Resource.Layout.SavedRewardsLayout;
        }

        protected override void OnResume()
        {
            base.OnResume();
            try
            {
                FirebaseAnalyticsUtils.SetScreenName(this, "My Saved Rewards");
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

            try
            {
                items = this.presenter.GetActiveSavedRewardList();

                if (mRewardsRecyclerAdapter == null)
                {
                    mRewardsRecyclerAdapter = new SavedRewardsRecyclerAdapter(items, this);
                    mRewardsRecyclerView.SetAdapter(mRewardsRecyclerAdapter);
                    mRewardsRecyclerAdapter.ClickChanged += MRewardsRecyclerAdapter_ClickChanged;
                }
                else
                {
                    mRewardsRecyclerAdapter.RefreshList(items);
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        private void MRewardsRecyclerAdapter_ClickChanged(object sender, int e)
        {
            if (!this.GetIsClicked() && e != -1)
            {
                this.SetIsClicked(true);

                if (!items[e].Read)
                {
                    items[e].Read = true;
                    mRewardsRecyclerAdapter.NotifyItemChanged(e);
                    RewardsEntity wtManager = new RewardsEntity();
                    wtManager.UpdateReadItem(items[e].ID, items[e].Read);
                }

                RewardsMenuUtils.OnSetRefreshAll(true);

                Intent activity = new Intent(this, typeof(RewardDetailActivity));
                activity.PutExtra(Constants.REWARD_DETAIL_ITEM_KEY, items[e].ID);
                activity.PutExtra(Constants.REWARD_DETAIL_TITLE_KEY, "My Saved Rewards");
                StartActivity(activity);
            }
        }


        protected override void OnPause()
        {
            base.OnPause();
        }

        public override void OnTrimMemory(TrimMemory level)
        {
            base.OnTrimMemory(level);

            switch (level)
            {
                case TrimMemory.RunningLow:
                    GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
                    GC.Collect();
                    break;
                default:
                    GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
                    GC.Collect();
                    break;
            }
        }

    }
}