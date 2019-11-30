using Android.Content;
using Android.OS;
using Android.App;
using Android.Views;
using myTNB_Android.Src.Utils;
using Android.Widget;
using Android.Support.V7.Widget;
using static myTNB_Android.Src.Utils.Constants;
using myTNB.SitecoreCMS.Model;
using System.Collections.Generic;
using Android.Preferences;
using myTNB_Android.Src.myTNBMenu.Fragments.RewardMenu.Adapter;

namespace myTNB_Android.Src.myTNBMenu.Fragments.RewardMenu.MVP
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
        }

        public override void OnResume()
        {
            base.OnResume();
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View rootView = inflater.Inflate(Resource.Layout.RewardIListtemLayout, container, false);

            mRewardsRecyclerView = rootView.FindViewById<RecyclerView>(Resource.Id.rewardRecyclerView);
            LinearLayoutManager linearLayoutManager = new LinearLayoutManager(this.Activity, LinearLayoutManager.Vertical, false);
            mRewardsRecyclerView.SetLayoutManager(linearLayoutManager);

            mRewardsRecyclerAdapter = new RewardsRecyclerAdapter(mRewardList, this.Activity);
            mRewardsRecyclerView.SetAdapter(mRewardsRecyclerAdapter);
            initializeComplete = true;
            return rootView;
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
    }
}
