using Android.App;
using Android.OS;
using myTNB.SitecoreCMS.Model;
using myTNB.SitecoreCMS.Services;
using myTNB.SQLite.SQLiteDataManager;
using myTNB.AndroidApp.Src.Database.Model;
using myTNB.AndroidApp.Src.myTNBMenu.MVP;
using myTNB.AndroidApp.Src.SiteCore;
using myTNB.AndroidApp.Src.Utils;
using Newtonsoft.Json;
using Refit;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace myTNB.AndroidApp.Src.myTNBMenu.Async
{
	public class SitecoreRewardAPI : AsyncTask
	{

		private string savedRewardTimeStamp = "0000000";
		CancellationTokenSource cts = null;

        private DashboardHomeContract.IView mHomeView = null;

        private bool isSitecoreApiFailed = false;

        private RewardsTimeStampResponseModel responseMasterModel = new RewardsTimeStampResponseModel();

        public SitecoreRewardAPI(DashboardHomeContract.IView mView)
		{
            this.mHomeView = mView;
        }

		protected override void OnPreExecute()
		{

		}

		protected override Java.Lang.Object DoInBackground(params Java.Lang.Object[] @params)
		{
			try
			{
                isSitecoreApiFailed = false;
                Console.WriteLine("000 SitecoreRewardAPI started");
                RewardsParentEntity wtManager = new RewardsParentEntity();

                try
                {
                    List<RewardsParentEntity> saveditems = wtManager.GetAllItems();
                    if (saveditems != null && saveditems.Count > 0)
                    {
                        RewardsParentEntity entity = saveditems[0];
                        if (entity != null)
                        {
                            savedRewardTimeStamp = entity.Timestamp;
                        }
                    }
                }
                catch (Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                }

				//Get Sitecore reward timestamp
				bool getSiteCoreRewards = false;
				cts = new CancellationTokenSource();
				Task.Factory.StartNew(() =>
				{
					try
					{
						string density = DPUtils.GetDeviceDensity(Application.Context);
                        GetItemsService getItemsService = new GetItemsService(SiteCoreConfig.OS, density, SiteCoreConfig.SITECORE_URL, LanguageUtil.GetAppLanguage());
                        responseMasterModel = getItemsService.GetRewardsTimestampItem();
                        if (responseMasterModel.Status.Equals("Success"))
                        {
                            if (responseMasterModel.Data != null && responseMasterModel.Data.Count > 0)
                            {
                                if (!responseMasterModel.Data[0].Timestamp.Equals(savedRewardTimeStamp))
                                {
                                    getSiteCoreRewards = true;
                                }
                                else
                                {
                                    getSiteCoreRewards = false;
                                }
                            }
                            else
                            {
                                List<RewardsParentEntity> items = wtManager.GetAllItems();
                                if (items != null && items.Count > 0)
                                {
                                    RewardsParentEntity entity = items[0];
                                    if (entity != null)
                                    {
                                        if (!entity.Timestamp.Equals(savedRewardTimeStamp))
                                        {
                                            getSiteCoreRewards = true;
                                        }
                                        else
                                        {
                                            RewardsEntity mRewardsEntityCheck = new RewardsEntity();
                                            List<RewardsEntity> mCheckList = mRewardsEntityCheck.GetAllItems();
                                            if (mCheckList == null || mCheckList.Count == 0)
                                            {
                                                getSiteCoreRewards = true;
                                            }
                                            else
                                            {
                                                getSiteCoreRewards = false;

                                            }
                                        }
                                    }
                                    else
                                    {
                                        getSiteCoreRewards = true;
                                    }
                                }
                                else
                                {
                                    getSiteCoreRewards = true;
                                }
                            }
                        }
                        else
                        {
                            getSiteCoreRewards = true;
                        }

						if (getSiteCoreRewards)
						{
							cts = new CancellationTokenSource();
							Task.Factory.StartNew(() =>
							{
								try
								{
                                    string newDensity = DPUtils.GetDeviceDensity(Application.Context);
                                    GetItemsService getRewardItemsService = new GetItemsService(SiteCoreConfig.OS, newDensity, SiteCoreConfig.SITECORE_URL, LanguageUtil.GetAppLanguage());
                                    RewardsResponseModel responseModel = getRewardItemsService.GetRewardsItems();
                                    if (responseModel != null && !string.IsNullOrEmpty(responseModel.Status))
                                    {
                                        if (responseModel.Status.Equals("Success"))
                                        {
                                            if (responseMasterModel != null && responseMasterModel.Status != null && responseMasterModel.Status.Equals("Success") && responseMasterModel.Data != null && responseMasterModel.Data.Count > 0)
                                            {
                                                RewardsParentEntity mRewardsParentEntity = new RewardsParentEntity();
                                                mRewardsParentEntity.DeleteTable();
                                                mRewardsParentEntity.CreateTable();
                                                mRewardsParentEntity.InsertListOfItems(responseMasterModel.Data);
                                            }

                                            RewardsCategoryEntity mRewardsCategoryEntity = new RewardsCategoryEntity();
                                            RewardsEntity mRewardsEntity = new RewardsEntity();

                                            if (responseModel != null && responseModel.Data != null && responseModel.Data.Count > 0)
                                            {
                                                List<RewardsCategoryModel> ToStoredList = new List<RewardsCategoryModel>();
                                                List<RewardsModel> ToStoredRewardList = new List<RewardsModel>();

                                                for (int i = 0; i < responseModel.Data.Count; i++)
                                                {
                                                    if (responseModel.Data[i].RewardList != null && responseModel.Data[i].RewardList.Count > 0)
                                                    {
                                                        List<RewardsModel> localList = new List<RewardsModel>();
                                                        for (int j = 0; j < responseModel.Data[i].RewardList.Count; j++)
                                                        {
                                                            int startResult = -1;
                                                            int endResult = 1;
                                                            try
                                                            {
                                                                if (!string.IsNullOrEmpty(responseModel.Data[i].RewardList[j].StartDate) && !string.IsNullOrEmpty(responseModel.Data[i].RewardList[j].EndDate))
                                                                {
                                                                    DateTime startDateTime = DateTime.ParseExact(responseModel.Data[i].RewardList[j].StartDate, "yyyyMMddTHHmmss",
                                                                    CultureInfo.InvariantCulture, DateTimeStyles.None);
                                                                    DateTime stopDateTime = DateTime.ParseExact(responseModel.Data[i].RewardList[j].EndDate, "yyyyMMddTHHmmss",
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
                                                                RewardsModel mModel = responseModel.Data[i].RewardList[j];

                                                                RewardsEntity searchItem = mRewardsEntity.GetItem(mModel.ID);
                                                                if (searchItem != null)
                                                                {
                                                                    mModel.IsSaved = searchItem.IsSaved;
                                                                    mModel.IsSavedDateTime = searchItem.IsSavedDateTime;
                                                                    mModel.IsUsed = searchItem.IsUsed;
                                                                    mModel.IsUsedDateTime = searchItem.IsUsedDateTime;
                                                                    mModel.Read = searchItem.Read;
                                                                    mModel.ReadDateTime = searchItem.ReadDateTime;
                                                                }
                                                                localList.Add(mModel);
                                                            }
                                                        }

                                                        if (localList.Count > 0)
                                                        {
                                                            ToStoredList.Add(new RewardsCategoryModel()
                                                            {
                                                                ID = responseModel.Data[i].ID,
                                                                CategoryName = responseModel.Data[i].CategoryName
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

                                                if (mHomeView != null)
                                                {
                                                    mHomeView.OnCheckUserReward(isSitecoreApiFailed);
                                                }
                                            }
                                            else
                                            {
                                                mRewardsCategoryEntity.DeleteTable();
                                                mRewardsEntity.DeleteTable();
                                                mRewardsCategoryEntity.CreateTable();
                                                mRewardsEntity.CreateTable();

                                                if (mHomeView != null)
                                                {
                                                    mHomeView.OnCheckUserReward(isSitecoreApiFailed);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            RewardsParentEntity wtManager2 = new RewardsParentEntity();
                                            wtManager2.DeleteTable();
                                            wtManager2.CreateTable();
                                            RewardsCategoryEntity mRewardsCategoryEntity2 = new RewardsCategoryEntity();
                                            RewardsEntity mRewardsEntity2 = new RewardsEntity();
                                            mRewardsCategoryEntity2.DeleteTable();
                                            mRewardsEntity2.DeleteTable();
                                            mRewardsCategoryEntity2.CreateTable();
                                            mRewardsEntity2.CreateTable();
                                            if (mHomeView != null)
                                            {
                                                mHomeView.OnCheckUserReward(isSitecoreApiFailed);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        RewardsParentEntity wtManager2 = new RewardsParentEntity();
                                        wtManager2.DeleteTable();
                                        wtManager2.CreateTable();
                                        isSitecoreApiFailed = true;
                                        if (mHomeView != null)
                                        {
                                            mHomeView.OnCheckUserReward(isSitecoreApiFailed);
                                        }
                                    }
								}
								catch (System.Exception e)
								{
                                    RewardsParentEntity wtManager2 = new RewardsParentEntity();
                                    wtManager2.DeleteTable();
                                    wtManager2.CreateTable();
                                    isSitecoreApiFailed = true;
                                    if (mHomeView != null)
                                    {
                                        mHomeView.OnCheckUserReward(isSitecoreApiFailed);
                                    }
                                    Utility.LoggingNonFatalError(e);
								}
							}).ContinueWith((Task previous) =>
							{
							}, cts.Token);
						}
                        else
                        {
                            if (mHomeView != null)
                            {
                                mHomeView.OnCheckUserReward(isSitecoreApiFailed);
                            }
                        }
					}
					catch (System.Exception e)
					{
                        RewardsParentEntity wtManager2 = new RewardsParentEntity();
                        wtManager2.DeleteTable();
                        wtManager2.CreateTable();
                        isSitecoreApiFailed = true;
                        if (mHomeView != null)
                        {
                            mHomeView.OnCheckUserReward(isSitecoreApiFailed);
                        }
                        Utility.LoggingNonFatalError(e);
					}
				}).ContinueWith((Task previous) =>
				{
				}, cts.Token);

			}
			catch (ApiException apiException)
			{
                RewardsParentEntity wtManager2 = new RewardsParentEntity();
                wtManager2.DeleteTable();
                wtManager2.CreateTable();
                isSitecoreApiFailed = true;
                if (mHomeView != null)
                {
                    mHomeView.OnCheckUserReward(isSitecoreApiFailed);
                }
                Utility.LoggingNonFatalError(apiException);
			}
			catch (Newtonsoft.Json.JsonReaderException e)
			{
                RewardsParentEntity wtManager2 = new RewardsParentEntity();
                wtManager2.DeleteTable();
                wtManager2.CreateTable();
                isSitecoreApiFailed = true;
                if (mHomeView != null)
                {
                    mHomeView.OnCheckUserReward(isSitecoreApiFailed);
                }
                Utility.LoggingNonFatalError(e);
			}
			catch (System.Exception e)
			{
                RewardsParentEntity wtManager2 = new RewardsParentEntity();
                wtManager2.DeleteTable();
                wtManager2.CreateTable();
                isSitecoreApiFailed = true;
                if (mHomeView != null)
                {
                    mHomeView.OnCheckUserReward(isSitecoreApiFailed);
                }
                Utility.LoggingNonFatalError(e);
			}
			Console.WriteLine("000 SitecoreRewardAPI ended");
			return null;
		}

		protected override void OnPostExecute(Java.Lang.Object result)
		{
			base.OnPostExecute(result);
        }

	}
}
