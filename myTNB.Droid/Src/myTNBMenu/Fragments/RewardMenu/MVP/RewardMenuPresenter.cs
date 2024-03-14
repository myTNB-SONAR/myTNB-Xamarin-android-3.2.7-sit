using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using myTNB.SitecoreCMS.Model;
using myTNB.SitecoreCMS.Services;
using myTNB.Android.Src.Base.Models;
using myTNB.Android.Src.Database.Model;
using myTNB.Android.Src.myTNBMenu.Fragments.RewardMenu.Api;
using myTNB.Android.Src.myTNBMenu.Fragments.RewardMenu.Model;
using myTNB.Android.Src.myTNBMenu.Fragments.RewardMenu.Request;
using myTNB.Android.Src.myTNBMenu.Fragments.RewardMenu.Response;
using myTNB.Android.Src.NewAppTutorial.MVP;
using myTNB.Android.Src.SiteCore;
using myTNB.Android.Src.Utils;
using static myTNB.Android.Src.Utils.Constants;

namespace myTNB.Android.Src.myTNBMenu.Fragments.RewardMenu.MVP
{
	public class RewardMenuPresenter : RewardMenuContract.IRewardMenuPresenter
	{
        RewardMenuContract.IRewardMenuView mView;

		private ISharedPreferences mPref;

        private RewardsParentEntity mRewardsParentEntity;

        private RewardsCategoryEntity mRewardsCategoryEntity;

        private RewardsEntity mRewardsEntity;

        private RewardServiceImpl mApi;

        private List<AddUpdateRewardModel> userList = new List<AddUpdateRewardModel>();

        private RewardsTimeStampResponseModel responseMasterModel = new RewardsTimeStampResponseModel();

        public RewardMenuPresenter(RewardMenuContract.IRewardMenuView view, ISharedPreferences pref)
		{
			this.mView = view;
			this.mPref = pref;
            this.mApi = new RewardServiceImpl();
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
                    else
                    {
                        this.mView.OnSavedRewardsTimeStamp(null);
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
            // rewardsTokenSource.Cancel();
        }

        public Task OnGetRewardsTimeStamp()
        {
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    string density = DPUtils.GetDeviceDensity(Application.Context);
                    GetItemsService getItemsService = new GetItemsService(SiteCoreConfig.OS, density, SiteCoreConfig.SITECORE_URL, LanguageUtil.GetAppLanguage());
                    responseMasterModel = getItemsService.GetRewardsTimestampItem();
                    if (responseMasterModel.Status.Equals("Success") && responseMasterModel.Data != null && responseMasterModel.Data.Count > 0)
                    {
                        this.mView.CheckRewardsTimeStamp(responseMasterModel.Data[0].Timestamp);
                    }
                    else
                    {
                        this.mView.CheckRewardsTimeStamp(null);
                    }
                }
                catch (Exception e)
                {
                    this.mView.CheckRewardsTimeStamp(null);
                    Utility.LoggingNonFatalError(e);
                }
            }).ContinueWith((Task previous) =>
            {
            }, new CancellationTokenSource().Token);
        }

        public Task OnGetRewards()
        {
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    string density = DPUtils.GetDeviceDensity(Application.Context);
                    GetItemsService getItemsService = new GetItemsService(SiteCoreConfig.OS, density, SiteCoreConfig.SITECORE_URL, LanguageUtil.GetAppLanguage());
                    RewardsResponseModel responseModel = getItemsService.GetRewardsItems();
                    if (responseModel != null && !string.IsNullOrEmpty(responseModel.Status))
                    {
                        if (responseModel.Status.Equals("Success"))
                        {
                            if (responseMasterModel != null && responseMasterModel.Status != null && responseMasterModel.Status.Equals("Success") && responseMasterModel.Data != null && responseMasterModel.Data.Count > 0)
                            {
                                if (mRewardsParentEntity == null)
                                {
                                    mRewardsParentEntity = new RewardsParentEntity();
                                }
                                mRewardsParentEntity.DeleteTable();
                                mRewardsParentEntity.CreateTable();
                                mRewardsParentEntity.InsertListOfItems(responseMasterModel.Data);
                            }
                            ProcessRewardResponse(responseModel);
                        }
                        else
                        {
                            if (mRewardsParentEntity == null)
                            {
                                mRewardsParentEntity = new RewardsParentEntity();
                            }
                            mRewardsParentEntity.DeleteTable();
                            mRewardsParentEntity.CreateTable();
                            if (mRewardsEntity == null)
                            {
                                mRewardsEntity = new RewardsEntity();
                            }
                            mRewardsEntity.DeleteTable();
                            mRewardsEntity.CreateTable();
                            if (mRewardsCategoryEntity == null)
                            {
                                mRewardsCategoryEntity = new RewardsCategoryEntity();
                            }
                            mRewardsCategoryEntity.DeleteTable();
                            mRewardsCategoryEntity.CreateTable();
                            CheckRewardsCache();
                        }
                    }
                    else
                    {
                        if (mRewardsParentEntity == null)
                        {
                            mRewardsParentEntity = new RewardsParentEntity();
                        }
                        mRewardsParentEntity.DeleteTable();
                        mRewardsParentEntity.CreateTable();
                        this.mView.SetRefreshView(null, null);
                    }
                }
                catch (Exception e)
                {
                    if (mRewardsParentEntity == null)
                    {
                        mRewardsParentEntity = new RewardsParentEntity();
                    }
                    mRewardsParentEntity.DeleteTable();
                    mRewardsParentEntity.CreateTable();
                    this.mView.SetRefreshView(null, null);
                    Utility.LoggingNonFatalError(e);
                }
            }).ContinueWith((Task previous) =>
            {
            }, new CancellationTokenSource().Token);
        }

        public async Task OnGetUserRewardList()
        {
            try
            {
                UserInterface currentUsrInf = new UserInterface()
                {
                    eid = UserEntity.GetActive().Email,
                    sspuid = UserEntity.GetActive().UserID,
                    did = UserEntity.GetActive().DeviceId,
                    ft = FirebaseTokenEntity.GetLatest().FBToken,
                    lang = LanguageUtil.GetAppLanguage().ToUpper(),
                    sec_auth_k1 = Constants.APP_CONFIG.API_KEY_ID,
                    sec_auth_k2 = "",
                    ses_param1 = "",
                    ses_param2 = ""
                };

                GetUserRewardsRequest request = new GetUserRewardsRequest()
                {
                    usrInf = currentUsrInf
                };

                GetUserRewardsResponse response = await this.mApi.GetUserRewards(request, new System.Threading.CancellationTokenSource().Token);

                if (response != null && response.Data != null && response.Data.ErrorCode == "7200")
                {
                    if (response.Data.Data != null && response.Data.Data.CurrentList != null && response.Data.Data.CurrentList.Count > 0)
                    {
                        userList = response.Data.Data.CurrentList;
                    }
                    else
                    {
                        userList = new List<AddUpdateRewardModel>();
                    }
                    CheckRewardsCache();
                }
                else
                {
                    userList = new List<AddUpdateRewardModel>();

                    string messageText = "";
                    string buttonText = "";

                    if (response != null && response.Data != null)
                    {
                        if (!string.IsNullOrEmpty(response.Data.RefreshMessage))
                        {
                            messageText = response.Data.RefreshMessage;
                        }

                        if (!string.IsNullOrEmpty(response.Data.RefreshBtnText))
                        {
                            buttonText = response.Data.RefreshBtnText;
                        }
                    }

                    this.mView.SetRefreshView(buttonText, messageText);
                }
            }
            catch (Exception e)
            {
                userList = new List<AddUpdateRewardModel>();
                this.mView.SetRefreshView(null, null);
                Utility.LoggingNonFatalError(e);
            }
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
                        for (int j = 0; j < checkList.Count; j++)
                        {
                            if (userList != null && userList.Count > 0)
                            {
                                string checkID = checkList[j].ID;
                                checkID = checkID.Replace("{", "");
                                checkID = checkID.Replace("}", "");

                                AddUpdateRewardModel found = userList.Find(x => x.RewardId.Contains(checkID));
                                if (found != null)
                                {
                                    if (found.Read)
                                    {
                                        string readDate = !string.IsNullOrEmpty(found.ReadDate) ? found.ReadDate : "";
                                        if (readDate.Contains("Date("))
                                        {
                                            int startIndex = readDate.LastIndexOf("(") + 1;
                                            int lastIndex = readDate.LastIndexOf(")");
                                            int lengthOfId = (lastIndex - startIndex);
                                            if (lengthOfId < readDate.Length)
                                            {
                                                string timeStamp = readDate.Substring(startIndex, lengthOfId);
                                                DateTime dateTimeParse = DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(timeStamp)).DateTime;
                                                CultureInfo currCult = CultureInfo.CreateSpecificCulture("en-US");
                                                readDate = dateTimeParse.ToString(@"M/d/yyyy h:m:s tt", currCult);
                                            }
                                        }
                                        mRewardsEntity.UpdateReadItem(checkList[j].ID, found.Read, readDate);
                                    }

                                    if (found.Favourite)
                                    {
                                        string favDate = !string.IsNullOrEmpty(found.FavUpdatedDate) ? found.FavUpdatedDate : "";
                                        if (favDate.Contains("Date("))
                                        {
                                            int startIndex = favDate.LastIndexOf("(") + 1;
                                            int lastIndex = favDate.LastIndexOf(")");
                                            int lengthOfId = (lastIndex - startIndex);
                                            if (lengthOfId < favDate.Length)
                                            {
                                                string timeStamp = favDate.Substring(startIndex, lengthOfId);
                                                DateTime dateTimeParse = DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(timeStamp)).DateTime;
                                                CultureInfo currCult = CultureInfo.CreateSpecificCulture("en-US");
                                                favDate = dateTimeParse.ToString(@"M/d/yyyy h:m:s tt", currCult);
                                            }
                                        }
                                        mRewardsEntity.UpdateIsSavedItem(checkList[j].ID, found.Favourite, favDate);
                                    }
                                    else
                                    {
                                        mRewardsEntity.UpdateIsSavedItem(checkList[j].ID, found.Favourite, "");
                                    }

                                    if (found.Redeemed)
                                    {
                                        string redeemDate = !string.IsNullOrEmpty(found.RedeemedDate) ? found.RedeemedDate : "";
                                        if (redeemDate.Contains("Date("))
                                        {
                                            int startIndex = redeemDate.LastIndexOf("(") + 1;
                                            int lastIndex = redeemDate.LastIndexOf(")");
                                            int lengthOfId = (lastIndex - startIndex);
                                            if (lengthOfId < redeemDate.Length)
                                            {
                                                string timeStamp = redeemDate.Substring(startIndex, lengthOfId);
                                                DateTime dateTimeParse = DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(timeStamp)).DateTime;
                                                CultureInfo currCult = CultureInfo.CreateSpecificCulture("en-US");
                                                redeemDate = dateTimeParse.ToString(@"M/d/yyyy h:m:s tt", currCult);
                                            }
                                        }
                                        mRewardsEntity.UpdateIsUsedItem(checkList[j].ID, found.Redeemed, redeemDate);
                                    }
                                }
                            }
                        }

                        List<RewardsEntity> reCheckList = mRewardsEntity.GetActiveItemsByCategory(mCategoryList[i].ID);

                        if (reCheckList != null && reCheckList.Count > 0)
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

                if (mDisplayCategoryList.Count > 1)
                {
                    list.Add(new RewardMenuModel()
                    {
                        TabTitle = Utility.GetLocalizedLabel("Rewards","viewAll"),
                        Fragment = new RewardItemFragment(),
                        FragmentListMode = REWARDSITEMLISTMODE.LOADED,
                        FragmentSearchString = ""
                    });
                }

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
                if (RewardsMenuUtils.GetRewardLoading())
                {
                    CheckRewardsCache();
                }
                else
                {
                    this.mView.SetEmptyView();
                }
            }
        }

        public Task OnRecheckRewardsStatus()
        {
            return Task.Delay(Constants.REWARDS_DATA_CHECK_TIMEOUT).ContinueWith(_ =>
            {
                this.mView.OnGetRewardTimestamp();
            });
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

                _ = OnGetUserRewardList();
            }
            catch (Exception e)
            {
                this.mView.SetRefreshView(null, null);
                Utility.LoggingNonFatalError(e);
            }
        }

        public List<NewAppModel> OnGeneraNewAppTutorialList()
        {
            List<NewAppModel> newList = new List<NewAppModel>();

            newList.Add(new NewAppModel()
            {
                ContentShowPosition = ContentType.BottomLeft,
                ContentTitle = Utility.GetLocalizedLabel("Rewards", "tutorialRewardTitle"), //"Pick a reward, check it out!",
                ContentMessage = Utility.GetLocalizedLabel("Rewards", "tutorialRewardDesc"), //"Tap on a reward to find out more, or<br/>tap on the heart to save it for later.",
                ItemCount = 0,
                DisplayMode = "",
                IsButtonShow = false
            });

            if (this.mView.CheckTabVisibility())
            {
                newList.Add(new NewAppModel()
                {
                    ContentShowPosition = ContentType.BottomLeft,
                    ContentTitle = Utility.GetLocalizedLabel("Rewards", "tutorialCategoryTitle"), // "Rewards for you.",
                    ContentMessage = Utility.GetLocalizedLabel("Rewards", "tutorialCategoryDesc"), // "Switch between the categories to<br/>explore the different rewards.",
                    ItemCount = 0,
                    DisplayMode = "",
                    IsButtonShow = false
                });
            }

            newList.Add(new NewAppModel()
            {
                ContentShowPosition = ContentType.BottomRight,
                ContentTitle = Utility.GetLocalizedLabel("Rewards", "tutorialSaveTitle"), // "Your favourites all in one place.",
                ContentMessage = Utility.GetLocalizedLabel("Rewards", "tutorialSaveDesc"), // "Here you’ll find all the rewards<br/>you’ve saved for later use.",
                ItemCount = 0,
                DisplayMode = "",
                IsButtonShow = false
            });

            return newList;
        }
    }
}
