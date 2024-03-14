using Android.Content;
using Android.OS;
using Android.Views;
using myTNB.Android.Src.Utils;
using Android.Widget;

using static myTNB.Android.Src.Utils.Constants;
using myTNB.SitecoreCMS.Model;
using System.Collections.Generic;
using Android.Preferences;
using myTNB.Android.Src.myTNBMenu.Fragments.RewardMenu.Adapter;
using myTNB.Android.Src.RewardDetail.MVP;
using Newtonsoft.Json;
using myTNB.Android.Src.Database.Model;
using System;
using AndroidX.RecyclerView.Widget;
using AndroidX.Fragment.App;

namespace myTNB.Android.Src.myTNBMenu.Fragments.RewardMenu.MVP
{
    public class RewardItemFragment : Fragment, RewardItemContract.IRewardItemView
    {
        RecyclerView mRewardsRecyclerView;
        RewardsRecyclerAdapter mRewardsRecyclerAdapter;
        List<RewardsModel> mRewardList = new List<RewardsModel>();
        REWARDSITEMLISTMODE mListMode;
        RewardItemContract.IRewardItemPresenter presenter;
        string mRewardSearchKey = "";
        bool initializeComplete = false;

        private bool isClicked = false;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            presenter = new RewardItemPresenter(this, PreferenceManager.GetDefaultSharedPreferences(this.Activity));
            if (Arguments != null && Arguments.ContainsKey(Constants.REWARDS_ITEM_LIST_MODE))
            {
                mListMode = (REWARDSITEMLISTMODE) Arguments.GetInt(Constants.REWARDS_ITEM_LIST_MODE);
            }
            else
            {
                mListMode = REWARDSITEMLISTMODE.INITIATE;
            }

            mRewardList = new List<RewardsModel>();

            if (mListMode == REWARDSITEMLISTMODE.INITIATE)
            {
                mRewardList = this.presenter.InitializeRewardList();
            }
            else
            {
                if (Arguments != null && Arguments.ContainsKey(Constants.REWARDS_ITEM_LIST_SEARCH_STRING_KEY))
                {
                    mRewardSearchKey = Arguments.GetString(Constants.REWARDS_ITEM_LIST_SEARCH_STRING_KEY);
                }

                if (mRewardSearchKey != "")
                {
                    mRewardList = this.presenter.GetActiveRewardList(mRewardSearchKey);
                }
                else
                {
                    mRewardList = this.presenter.GetActiveRewardList();
                }
            }
        }

        public override void OnAttach(Context context)
        {
            base.OnAttach(context);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            try
            {

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
            this.isClicked = true;
        }

        public override void OnResume()
        {
            base.OnResume();
            this.isClicked = false;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View rootView = inflater.Inflate(Resource.Layout.RewardIListtemLayout, container, false);

            mRewardsRecyclerView = rootView.FindViewById<RecyclerView>(Resource.Id.rewardRecyclerView);
            LinearLayoutManager linearLayoutManager = new LinearLayoutManager(this.Activity, LinearLayoutManager.Vertical, false);
            mRewardsRecyclerView.SetLayoutManager(linearLayoutManager);
            ((SimpleItemAnimator)mRewardsRecyclerView.GetItemAnimator()).SupportsChangeAnimations = false;

            mRewardsRecyclerAdapter = new RewardsRecyclerAdapter(mRewardList, this.Activity, mListMode);
            mRewardsRecyclerView.SetAdapter(mRewardsRecyclerAdapter);
            mRewardsRecyclerView.OverScrollMode = OverScrollMode.Never;
            mRewardsRecyclerAdapter.SavedClickChanged += MRewardsRecyclerAdapter_SavedClickChanged;
            mRewardsRecyclerAdapter.ClickChanged += MRewardsRecyclerAdapter_ClickChanged;
            initializeComplete = true;
            return rootView;
        }

        private void MRewardsRecyclerAdapter_ClickChanged(object sender, int e)
        {
            if (!isClicked && e != -1)
            {
                isClicked = true;

                if (!mRewardList[e].Read)
                {
                    mRewardList[e].Read = true;
                    mRewardsRecyclerAdapter.NotifyItemChanged(e);
                    this.presenter.UpdateRewardRead(mRewardList[e].ID, mRewardList[e].Read);
                }

                RewardsMenuUtils.OnSetRefreshAll(true);

                Intent activity = new Intent(this.Activity, typeof(RewardDetailActivity));
                activity.PutExtra(Constants.REWARD_DETAIL_ITEM_KEY, mRewardList[e].ID);
                activity.PutExtra(Constants.REWARD_DETAIL_TITLE_KEY, Utility.GetLocalizedLabel("Tabbar", "rewards"));
                StartActivity(activity);
            }
        }

        private void MRewardsRecyclerAdapter_SavedClickChanged(object sender, int e)
        {
            if (mListMode == REWARDSITEMLISTMODE.LOADED)
            {
                if (e != -1)
                {
                    RewardsMenuUtils.OnSetUpdateList(mRewardSearchKey);
                    if (mRewardList[e].IsSaved)
                    {
                        mRewardList[e].IsSaved = false;
                    }
                    else
                    {
                        mRewardList[e].IsSaved = true;
                    }

                    this.presenter.UpdateRewardSave(mRewardList[e].ID, mRewardList[e].IsSaved);

                    mRewardsRecyclerAdapter.NotifyItemChanged(e);
                }
            }
        }

        public void Refresh()
        {
            if (initializeComplete)
            {
                if (mListMode == REWARDSITEMLISTMODE.LOADED)
                {
                    if (mRewardSearchKey != "")
                    {
                        mRewardList = this.presenter.GetActiveRewardList(mRewardSearchKey);
                    }
                    else
                    {
                        mRewardList = this.presenter.GetActiveRewardList();
                    }

                    mRewardsRecyclerAdapter.RefreshList(mRewardList);
                }
            }
        }

        public int GetFirstItemRelativePosition()
        {
            int i = 0;

            try
            {
                View firstView = mRewardsRecyclerView.GetChildAt(0);
                int[] location = new int[2];
                firstView.GetLocationOnScreen(location);
                i = location[1];
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

            return i;
        }

        public int GetFirstItemHeight()
        {
            int i = 0;

            try
            {
                View firstView = mRewardsRecyclerView.GetChildAt(0);
                i = firstView.Height;
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

            return i;
        }

        public void StopScrolling()
        {
            try
            {
                mRewardsRecyclerView.SmoothScrollBy(0, 0);
                mRewardsRecyclerView.ScrollTo(0, 0);
                mRewardsRecyclerView.RequestLayout();
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void RewardCustomScrolling(int yPosition)
        {
            try
            {
                Activity.RunOnUiThread(() =>
                {
                    try
                    {
                        mRewardsRecyclerView.ScrollTo(0, yPosition);
                        mRewardsRecyclerView.RequestLayout();
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
    }
}
