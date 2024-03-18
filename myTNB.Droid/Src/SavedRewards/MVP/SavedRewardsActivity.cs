using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Preferences;

using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using CheeseBind;
using myTNB.SitecoreCMS.Model;
using myTNB.AndroidApp.Src.Base.Activity;
using myTNB.AndroidApp.Src.RewardDetail.MVP;
using myTNB.AndroidApp.Src.SavedRewards.Adapter;
using myTNB.AndroidApp.Src.Utils;
using System;
using System.Collections.Generic;
using System.Runtime;

namespace myTNB.AndroidApp.Src.SavedRewards.MVP
{
    [Activity(Label = "My Saved Rewards"
              , Icon = "@drawable/ic_launcher"
      , ScreenOrientation = ScreenOrientation.Portrait
      , Theme = "@style/Theme.Dashboard")]
    public class SavedRewardsActivity : BaseToolbarAppCompatActivity, SavedRewardsContract.ISavedRewardsView
    {
        [BindView(Resource.Id.rewardMainLayout)]
        LinearLayout rewardMainLayout;

        [BindView(Resource.Id.rewardRecyclerView)]
        RecyclerView mRewardsRecyclerView;

        [BindView(Resource.Id.rewardEmptyLayout)]
        LinearLayout rewardEmptyLayout;

        [BindView(Resource.Id.rewardEmptyImg)]
        ImageView rewardEmptyImg;

        [BindView(Resource.Id.txtEmptyReward)]
        TextView txtEmptyReward;

        List<RewardsModel> items;

        SavedRewardsRecyclerAdapter mRewardsRecyclerAdapter;

        SavedRewardsContract.ISavedRewardsPresenter presenter;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetTheme(TextViewUtils.IsLargeFonts ? Resource.Style.Theme_DashboardLarge : Resource.Style.Theme_Dashboard);
            try
            {
                presenter = new SavedRewardsPresenter(this, PreferenceManager.GetDefaultSharedPreferences(this));

                TextViewUtils.SetMuseoSans300Typeface(txtEmptyReward);

                rewardMainLayout.Visibility = ViewStates.Visible;

                rewardEmptyLayout.Visibility = ViewStates.Gone;

                txtEmptyReward.Text = Utility.GetLocalizedLabel("SavedRewards", "emptyDesc");

                SetToolBarTitle(Utility.GetLocalizedLabel("SavedRewards", "title"));
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

                if (items != null && items.Count > 0)
                {
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
                else
                {
                    SetEmptyView();
                }
            }
            catch (Exception e)
            {
                SetEmptyView();
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
                    this.presenter.UpdateRewardRead(items[e].ID, items[e].Read);
                }

                RewardsMenuUtils.OnSetRefreshAll(true);

                Intent activity = new Intent(this, typeof(RewardDetailActivity));
                activity.PutExtra(Constants.REWARD_DETAIL_ITEM_KEY, items[e].ID);
                activity.PutExtra(Constants.REWARD_DETAIL_TITLE_KEY, Utility.GetLocalizedLabel("SavedRewards", "title"));
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
                    // GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
                    GC.Collect();
                    break;
                default:
                    // GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
                    GC.Collect();
                    break;
            }
        }

        public void SetEmptyView()
        {
            try
            {
                RunOnUiThread(() =>
                {
                    try
                    {
                        rewardMainLayout.Visibility = ViewStates.Gone;

                        rewardEmptyLayout.Visibility = ViewStates.Visible;

                        LinearLayout.LayoutParams rewardEmptyImgParams = rewardEmptyImg.LayoutParameters as LinearLayout.LayoutParams;
                        rewardEmptyImgParams.TopMargin = GetDeviceVerticalScaleInPixel(0.155f);
                        rewardEmptyImgParams.Width = GetDeviceHorizontalScaleInPixel(0.319f);
                        rewardEmptyImgParams.Height = GetDeviceVerticalScaleInPixel(0.165f);
                        rewardEmptyImg.RequestLayout();
                    }
                    catch (System.Exception e)
                    {
                        Utility.LoggingNonFatalError(e);
                    }
                });
            }
            catch (System.Exception er)
            {
                Utility.LoggingNonFatalError(er);
            }
        }

    }
}