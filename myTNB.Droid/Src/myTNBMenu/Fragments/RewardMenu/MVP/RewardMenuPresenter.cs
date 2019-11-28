using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using myTNB.SitecoreCMS.Model;
using myTNB.SitecoreCMS.Services;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.myTNBMenu.Fragments.RewardMenu.Model;
using myTNB_Android.Src.SiteCore;
using myTNB_Android.Src.Utils;
using static myTNB_Android.Src.Utils.Constants;

namespace myTNB_Android.Src.myTNBMenu.Fragments.RewardMenu.MVP
{
	public class RewardMenuPresenter : RewardMenuContract.IRewardMenuPresenter
	{
        RewardMenuContract.IRewardMenuView mView;

		private ISharedPreferences mPref;

        private RewardsParentEntity mRewardsParentEntity;

        private RewardsCategoryEntity mRewardsCategoryEntity;

        private RewardsEntity mRewardsEntity;

        private CancellationTokenSource rewardsTokenSource = new CancellationTokenSource();

        public RewardMenuPresenter(RewardMenuContract.IRewardMenuView view, ISharedPreferences pref)
		{
			this.mView = view;
			this.mPref = pref;
		}


		public List<RewardMenuModel> InitializeRewardView()
        {
			List<RewardMenuModel> list = new List<RewardMenuModel>();

			list.Add(new RewardMenuModel()
			{
				TabTitle = "",
                Fragment = new RewardItemFragment(),
                FragmentListMode = REWARDSITEMLISTMODE.INITIATE,
                FragmentSearchString = ""
            });

            list.Add(new RewardMenuModel()
            {
                TabTitle = "",
                Fragment = new RewardItemFragment(),
                FragmentListMode = REWARDSITEMLISTMODE.INITIATE,
                FragmentSearchString = ""
            });

            list.Add(new RewardMenuModel()
            {
                TabTitle = "",
                Fragment = new RewardItemFragment(),
                FragmentListMode = REWARDSITEMLISTMODE.INITIATE,
                FragmentSearchString = ""
            });

            list.Add(new RewardMenuModel()
            {
                TabTitle = "",
                Fragment = new RewardItemFragment(),
                FragmentListMode = REWARDSITEMLISTMODE.INITIATE,
                FragmentSearchString = ""
            });

            list.Add(new RewardMenuModel()
            {
                TabTitle = "",
                Fragment = new RewardItemFragment(),
                FragmentListMode = REWARDSITEMLISTMODE.INITIATE,
                FragmentSearchString = ""
            });

            return list;
		}

        public void GetRewardsTimeStamp()
        {
            try
            {
                if (mRewardsParentEntity == null)
                {
                    mRewardsParentEntity = new RewardsParentEntity();
                }
                List<RewardsParentEntity> items = new List<RewardsParentEntity>();
                items = mRewardsParentEntity.GetAllItems();
                if (items != null && items.Count > 0)
                {
                    RewardsParentEntity entity = items[0];
                    if (entity != null && entity.Timestamp != null)
                    {
                        this.mView.OnSavedRewardsTimeStamp(entity?.Timestamp);
                    }
                }
                else
                {
                    this.mView.OnSavedRewardsTimeStamp(null);
                }
            }
            catch (Exception e)
            {
                this.mView.OnSavedRewardsTimeStamp(null);
                Utility.LoggingNonFatalError(e);
            }
        }

        public void OnCancelTask()
        {
            rewardsTokenSource.Cancel();
        }

        public Task OnGetRewardsTimeStamp()
        {
            rewardsTokenSource = new CancellationTokenSource();
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    rewardsTokenSource.Token.ThrowIfCancellationRequested();
                    string density = DPUtils.GetDeviceDensity(Application.Context);
                    GetItemsService getItemsService = new GetItemsService(SiteCoreConfig.OS, density, SiteCoreConfig.SITECORE_URL, SiteCoreConfig.DEFAULT_LANGUAGE);
                    RewardsTimeStampResponseModel responseModel = getItemsService.GetRewardsTimestampItem();
                    rewardsTokenSource.Token.ThrowIfCancellationRequested();
                    if (responseModel.Status.Equals("Success"))
                    {
                        if (mRewardsParentEntity == null)
                        {
                            mRewardsParentEntity = new RewardsParentEntity();
                        }
                        mRewardsParentEntity.DeleteTable();
                        mRewardsParentEntity.CreateTable();
                        mRewardsParentEntity.InsertListOfItems(responseModel.Data);
                        this.mView.CheckRewardsTimeStamp();
                    }
                }
                catch (Exception e)
                {
                    this.mView.CheckRewardsTimeStamp();
                    Utility.LoggingNonFatalError(e);
                }
            }).ContinueWith((Task previous) =>
            {
            }, rewardsTokenSource.Token);
        }

        public Task OnGetRewards()
        {
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    string density = DPUtils.GetDeviceDensity(Application.Context);
                    GetItemsService getItemsService = new GetItemsService(SiteCoreConfig.OS, density, SiteCoreConfig.SITECORE_URL, SiteCoreConfig.DEFAULT_LANGUAGE);
                    RewardsResponseModel responseModel = getItemsService.GetRewardsItems();
                    if (responseModel.Status.Equals("Success"))
                    {
                        ProcessRewardResponse(responseModel);
                    }
                    else
                    {
                        CheckRewardsCache();
                    }
                }
                catch (Exception e)
                {
                    CheckRewardsCache();
                    Utility.LoggingNonFatalError(e);
                }
            }).ContinueWith((Task previous) =>
            {
            }, new CancellationTokenSource().Token);
        }

        public void CheckRewardsCache()
        {
            if (mRewardsCategoryEntity == null)
            {
                mRewardsCategoryEntity = new RewardsCategoryEntity();
            }

            if (mRewardsEntity == null)
            {
                mRewardsEntity = new RewardsEntity();
            }

            List<RewardsCategoryModel> mDisplayCategoryList = new List<RewardsCategoryModel>();

            List <RewardsCategoryEntity> mCategoryList = mRewardsCategoryEntity.GetAllItems();

            if (mCategoryList != null && mCategoryList.Count > 0)
            {
                for (int i = 0; i < mCategoryList.Count; i++)
                {
                    List<RewardsEntity> checkList = mRewardsEntity.GetActiveItemsByCategory(mCategoryList[i].ID);
                    if (checkList != null && checkList.Count > 0)
                    {
                        mDisplayCategoryList.Add(new RewardsCategoryModel()
                        {
                            ID = mCategoryList[i].ID,
                            CategoryName = mCategoryList[i].CategoryName
                        });
                    }
                    else
                    {
                        mRewardsEntity.RemoveItemByCategoryId(mCategoryList[i].ID);
                        mRewardsCategoryEntity.RemoveItem(mCategoryList[i].ID);
                    }
                }
            }

            if (mDisplayCategoryList.Count > 0)
            {
                List<RewardMenuModel> list = new List<RewardMenuModel>();

                list.Add(new RewardMenuModel()
                {
                    TabTitle = "View All",
                    Fragment = new RewardItemFragment(),
                    FragmentListMode = REWARDSITEMLISTMODE.LOADED,
                    FragmentSearchString = ""
                });

                for (int j = 0; j < mDisplayCategoryList.Count; j++)
                {
                    list.Add(new RewardMenuModel()
                    {
                        TabTitle = mDisplayCategoryList[j].CategoryName,
                        Fragment = new RewardItemFragment(),
                        FragmentListMode = REWARDSITEMLISTMODE.LOADED,
                        FragmentSearchString = mDisplayCategoryList[j].ID
                    });
                }

                this.mView.OnSetResultTabView(list);
            }
            else
            {
                // TODO: Show Empty List
            }
        }

        private void ProcessRewardResponse(RewardsResponseModel response)
        {
            try
            {
                if (mRewardsCategoryEntity == null)
                {
                    mRewardsCategoryEntity = new RewardsCategoryEntity();
                }

                if (mRewardsEntity == null)
                {
                    mRewardsEntity = new RewardsEntity();
                }

                if (response != null && response.Data != null && response.Data.Count > 0)
                {
                    List<RewardsCategoryModel> ToStoredList = new List<RewardsCategoryModel>();
                    List<RewardsModel> ToStoredRewardList = new List<RewardsModel>();

                    for (int i = 0; i < response.Data.Count; i++)
                    {
                        if (response.Data[i].RewardList != null && response.Data[i].RewardList.Count > 0)
                        {
                            List<RewardsModel> localList = new List<RewardsModel>();
                            for (int j = 0; j < response.Data[i].RewardList.Count; j++)
                            {
                                int startResult = -1;
                                int endResult = 1;
                                try
                                {
                                    if (!string.IsNullOrEmpty(response.Data[i].RewardList[j].StartDate) && !string.IsNullOrEmpty(response.Data[i].RewardList[j].EndDate))
                                    {
                                        DateTime startDateTime = DateTime.ParseExact(response.Data[i].RewardList[j].StartDate, "yyyyMMddTHHmmss",
                                        CultureInfo.InvariantCulture, DateTimeStyles.None);
                                        DateTime stopDateTime = DateTime.ParseExact(response.Data[i].RewardList[j].EndDate, "yyyyMMddTHHmmss",
                                            CultureInfo.InvariantCulture, DateTimeStyles.None);
                                        DateTime nowDateTime = DateTime.Now;
                                        startResult = DateTime.Compare(nowDateTime, startDateTime);
                                        endResult = DateTime.Compare(nowDateTime, stopDateTime);
                                    }
                                }
                                catch (Exception ne)
                                {
                                    Utility.LoggingNonFatalError(ne);
                                }
                                if (startResult >= 0 && endResult <= 0)
                                {
                                    RewardsModel mModel = response.Data[i].RewardList[j];

                                    RewardsEntity searchItem = mRewardsEntity.GetItem(mModel.ID);
                                    if (searchItem != null)
                                    {
                                        mModel.IsSaved = searchItem.IsSaved;
                                        mModel.IsUsed = searchItem.IsUsed;
                                    }
                                    localList.Add(mModel);
                                }
                            }

                            if (localList.Count > 0)
                            {
                                ToStoredList.Add(new RewardsCategoryModel()
                                {
                                    ID = response.Data[i].ID,
                                    CategoryName = response.Data[i].CategoryName
                                });

                                ToStoredRewardList.AddRange(localList);
                            }
                        }
                    }

                    mRewardsCategoryEntity.DeleteTable();
                    mRewardsEntity.DeleteTable();
                    mRewardsCategoryEntity.CreateTable();
                    mRewardsEntity.CreateTable();

                    if (ToStoredList.Count > 0)
                    {
                        mRewardsCategoryEntity.InsertListOfItems(ToStoredList);
                        mRewardsEntity.InsertListOfItems(ToStoredRewardList);
                    }
                }
                else
                {
                    mRewardsCategoryEntity.DeleteTable();
                    mRewardsEntity.DeleteTable();
                    mRewardsCategoryEntity.CreateTable();
                    mRewardsEntity.CreateTable();
                }
                CheckRewardsCache();
            }
            catch (Exception e)
            {
                CheckRewardsCache();
                Utility.LoggingNonFatalError(e);
            }
        }
    }
}
