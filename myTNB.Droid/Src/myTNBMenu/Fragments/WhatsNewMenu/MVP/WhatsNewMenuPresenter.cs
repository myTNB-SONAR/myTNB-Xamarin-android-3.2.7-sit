using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using myTNB.SitecoreCMS.Model;
using myTNB.SitecoreCMS.Services;
using myTNB_Android.Src.Base.Models;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.myTNBMenu.Fragments.RewardMenu.Api;
using myTNB_Android.Src.myTNBMenu.Fragments.RewardMenu.Model;
using myTNB_Android.Src.myTNBMenu.Fragments.RewardMenu.Request;
using myTNB_Android.Src.myTNBMenu.Fragments.RewardMenu.Response;
using myTNB_Android.Src.myTNBMenu.Fragments.WhatsNewMenu.Model;
using myTNB_Android.Src.NewAppTutorial.MVP;
using myTNB_Android.Src.SiteCore;
using myTNB_Android.Src.Utils;
using static myTNB_Android.Src.Utils.Constants;

namespace myTNB_Android.Src.myTNBMenu.Fragments.WhatsNewMenu.MVP
{
    public class WhatsNewMenuPresenter : WhatsNewMenuContract.IWhatsNewMenuPresenter
    {
        WhatsNewMenuContract.IWhatsNewMenuView mView;

        private ISharedPreferences mPref;

        private WhatsNewParentEntity mWhatsNewParentEntity;

        private WhatsNewCategoryEntity mWhatsNewCategoryEntity;

        private WhatsNewEntity mWhatsNewEntity;

        private RewardServiceImpl mApi;

        private List<AddUpdateRewardModel> userList = new List<AddUpdateRewardModel>();

        public WhatsNewMenuPresenter(WhatsNewMenuContract.IWhatsNewMenuView view, ISharedPreferences pref)
        {
            this.mView = view;
            this.mPref = pref;
            this.mApi = new RewardServiceImpl();
        }


        public List<WhatsNewMenuModel> InitializeWhatsNewView()
        {
            List<WhatsNewMenuModel> list = new List<WhatsNewMenuModel>();

            list.Add(new WhatsNewMenuModel()
            {
                TabTitle = "",
                Fragment = new WhatsNewItemFragment(),
                FragmentListMode = WHATSNEWITEMLISTMODE.INITIATE,
                FragmentSearchString = ""
            });

            list.Add(new WhatsNewMenuModel()
            {
                TabTitle = "",
                Fragment = new WhatsNewItemFragment(),
                FragmentListMode = WHATSNEWITEMLISTMODE.INITIATE,
                FragmentSearchString = ""
            });

            list.Add(new WhatsNewMenuModel()
            {
                TabTitle = "",
                Fragment = new WhatsNewItemFragment(),
                FragmentListMode = WHATSNEWITEMLISTMODE.INITIATE,
                FragmentSearchString = ""
            });

            list.Add(new WhatsNewMenuModel()
            {
                TabTitle = "",
                Fragment = new WhatsNewItemFragment(),
                FragmentListMode = WHATSNEWITEMLISTMODE.INITIATE,
                FragmentSearchString = ""
            });

            list.Add(new WhatsNewMenuModel()
            {
                TabTitle = "",
                Fragment = new WhatsNewItemFragment(),
                FragmentListMode = WHATSNEWITEMLISTMODE.INITIATE,
                FragmentSearchString = ""
            });

            return list;
        }

        public void GetWhatsNewsTimeStamp()
        {
            try
            {
                if (mWhatsNewParentEntity == null)
                {
                    mWhatsNewParentEntity = new WhatsNewParentEntity();
                }
                List<WhatsNewParentEntity> items = new List<WhatsNewParentEntity>();
                items = mWhatsNewParentEntity.GetAllItems();
                if (items != null && items.Count > 0)
                {
                    WhatsNewParentEntity entity = items[0];
                    if (entity != null && entity.Timestamp != null)
                    {
                        this.mView.OnSavedWhatsNewsTimeStamp(entity?.Timestamp);
                    }
                    else
                    {
                        this.mView.OnSavedWhatsNewsTimeStamp(null);
                    }
                }
                else
                {
                    this.mView.OnSavedWhatsNewsTimeStamp(null);
                }
            }
            catch (Exception e)
            {
                this.mView.OnSavedWhatsNewsTimeStamp(null);
                Utility.LoggingNonFatalError(e);
            }
        }

        public void OnCancelTask()
        {
            // rewardsTokenSource.Cancel();
        }

        public Task OnGetWhatsNewsTimeStamp()
        {
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    string density = DPUtils.GetDeviceDensity(Application.Context);
                    GetItemsService getItemsService = new GetItemsService(SiteCoreConfig.OS, density, SiteCoreConfig.SITECORE_URL, LanguageUtil.GetAppLanguage());
                    WhatsNewTimeStampResponseModel responseModel = getItemsService.GetWhatsNewTimestampItem();
                    if (responseModel.Status.Equals("Success"))
                    {
                        if (mWhatsNewParentEntity == null)
                        {
                            mWhatsNewParentEntity = new WhatsNewParentEntity();
                        }
                        mWhatsNewParentEntity.DeleteTable();
                        mWhatsNewParentEntity.CreateTable();
                        mWhatsNewParentEntity.InsertListOfItems(responseModel.Data);
                        this.mView.CheckWhatsNewsTimeStamp();
                    }
                    else
                    {
                        this.mView.CheckWhatsNewsTimeStamp();
                    }
                }
                catch (Exception e)
                {
                    this.mView.CheckWhatsNewsTimeStamp();
                    Utility.LoggingNonFatalError(e);
                }
            }).ContinueWith((Task previous) =>
            {
            }, new CancellationTokenSource().Token);
        }

        public Task OnGetWhatsNews()
        {
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    string density = DPUtils.GetDeviceDensity(Application.Context);
                    GetItemsService getItemsService = new GetItemsService(SiteCoreConfig.OS, density, SiteCoreConfig.SITECORE_URL, LanguageUtil.GetAppLanguage());
                    WhatsNewResponseModel responseModel = getItemsService.GetWhatsNewItems();
                    if (responseModel != null && !string.IsNullOrEmpty(responseModel.Status))
                    {
                        if (responseModel.Status.Equals("Success"))
                        {
                            ProcessWhatsNewResponse(responseModel);
                        }
                        else
                        {
                            if (mWhatsNewParentEntity == null)
                            {
                                mWhatsNewParentEntity = new WhatsNewParentEntity();
                            }
                            mWhatsNewParentEntity.DeleteTable();
                            mWhatsNewParentEntity.CreateTable();
                            if (mWhatsNewEntity == null)
                            {
                                mWhatsNewEntity = new WhatsNewEntity();
                            }
                            mWhatsNewEntity.DeleteTable();
                            mWhatsNewEntity.CreateTable();
                            if (mWhatsNewCategoryEntity == null)
                            {
                                mWhatsNewCategoryEntity = new WhatsNewCategoryEntity();
                            }
                            mWhatsNewCategoryEntity.DeleteTable();
                            mWhatsNewCategoryEntity.CreateTable();
                            CheckWhatsNewsCache();
                        }
                    }
                    else
                    {
                        if (mWhatsNewParentEntity == null)
                        {
                            mWhatsNewParentEntity = new WhatsNewParentEntity();
                        }
                        mWhatsNewParentEntity.DeleteTable();
                        mWhatsNewParentEntity.CreateTable();
                        this.mView.SetRefreshView(null, null);
                    }
                }
                catch (Exception e)
                {
                    if (mWhatsNewParentEntity == null)
                    {
                        mWhatsNewParentEntity = new WhatsNewParentEntity();
                    }
                    mWhatsNewParentEntity.DeleteTable();
                    mWhatsNewParentEntity.CreateTable();
                    this.mView.SetRefreshView(null, null);
                    Utility.LoggingNonFatalError(e);
                }
            }).ContinueWith((Task previous) =>
            {
            }, new CancellationTokenSource().Token);
        }

        public async Task OnGetUserWhatsNewList()
        {
            // Whats New TODO: Check Whats New Api
            /*try
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
                    CheckWhatsNewsCache();
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
            }*/

            CheckWhatsNewsCache();
        }

        public void CheckWhatsNewsCache()
        {
            if (mWhatsNewCategoryEntity == null)
            {
                mWhatsNewCategoryEntity = new WhatsNewCategoryEntity();
            }

            if (mWhatsNewEntity == null)
            {
                mWhatsNewEntity = new WhatsNewEntity();
            }

            List<WhatsNewCategoryModel> mDisplayCategoryList = new List<WhatsNewCategoryModel>();

            List<WhatsNewCategoryEntity> mCategoryList = mWhatsNewCategoryEntity.GetAllItems();

            if (mCategoryList != null && mCategoryList.Count > 0)
            {
                for (int i = 0; i < mCategoryList.Count; i++)
                {
                    List<WhatsNewEntity> checkList = mWhatsNewEntity.GetActiveItemsByCategory(mCategoryList[i].ID);
                    if (checkList != null && checkList.Count > 0)
                    {
                        for (int j = 0; j < checkList.Count; j++)
                        {
                            // Whats New TODO: Whats New comparison
                            /*if (userList != null && userList.Count > 0)
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
                                        mWhatsNewEntity.UpdateReadItem(checkList[j].ID, found.Read, readDate);
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
                                        mWhatsNewEntity.UpdateIsSavedItem(checkList[j].ID, found.Favourite, favDate);
                                    }
                                    else
                                    {
                                        mWhatsNewEntity.UpdateIsSavedItem(checkList[j].ID, found.Favourite, "");
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
                                        mWhatsNewEntity.UpdateIsUsedItem(checkList[j].ID, found.Redeemed, redeemDate);
                                    }
                                }
                            }*/
                        }

                        mDisplayCategoryList.Add(new WhatsNewCategoryModel()
                        {
                            ID = mCategoryList[i].ID,
                            CategoryName = mCategoryList[i].CategoryName
                        });
                    }
                    else
                    {
                        mWhatsNewEntity.RemoveItemByCategoryId(mCategoryList[i].ID);
                        mWhatsNewCategoryEntity.RemoveItem(mCategoryList[i].ID);
                    }
                }
            }

            if (mDisplayCategoryList.Count > 0)
            {
                List<WhatsNewMenuModel> list = new List<WhatsNewMenuModel>();

                if (mDisplayCategoryList.Count > 1)
                {
                    list.Add(new WhatsNewMenuModel()
                    {
                        TabTitle = Utility.GetLocalizedLabel("Rewards", "viewAll"),
                        Fragment = new WhatsNewItemFragment(),
                        FragmentListMode = WHATSNEWITEMLISTMODE.LOADED,
                        FragmentSearchString = ""
                    });
                }

                for (int j = 0; j < mDisplayCategoryList.Count; j++)
                {
                    list.Add(new WhatsNewMenuModel()
                    {
                        TabTitle = mDisplayCategoryList[j].CategoryName,
                        Fragment = new WhatsNewItemFragment(),
                        FragmentListMode = WHATSNEWITEMLISTMODE.LOADED,
                        FragmentSearchString = mDisplayCategoryList[j].ID
                    });
                }

                this.mView.OnSetResultTabView(list);
            }
            else
            {
                if (WhatsNewMenuUtils.GetWhatsNewLoading())
                {
                    CheckWhatsNewsCache();
                }
                else
                {
                    this.mView.SetEmptyView();
                }
            }
        }

        public Task OnRecheckWhatsNewsStatus()
        {
            return Task.Delay(Constants.REWARDS_DATA_CHECK_TIMEOUT).ContinueWith(_ =>
            {
                this.mView.OnGetWhatsNewTimestamp();
            });
        }

        private void ProcessWhatsNewResponse(WhatsNewResponseModel response)
        {
            try
            {
                if (mWhatsNewCategoryEntity == null)
                {
                    mWhatsNewCategoryEntity = new WhatsNewCategoryEntity();
                }

                if (mWhatsNewEntity == null)
                {
                    mWhatsNewEntity = new WhatsNewEntity();
                }

                if (response != null && response.Data != null && response.Data.Count > 0)
                {
                    List<WhatsNewCategoryModel> ToStoredList = new List<WhatsNewCategoryModel>();
                    List<WhatsNewModel> ToStoredWhatsNewList = new List<WhatsNewModel>();

                    for (int i = 0; i < response.Data.Count; i++)
                    {
                        if (response.Data[i].WhatsNewList != null && response.Data[i].WhatsNewList.Count > 0)
                        {
                            List<WhatsNewModel> localList = new List<WhatsNewModel>();
                            for (int j = 0; j < response.Data[i].WhatsNewList.Count; j++)
                            {
                                int startResult = -1;
                                int endResult = 1;
                                try
                                {
                                    if (!string.IsNullOrEmpty(response.Data[i].WhatsNewList[j].StartDate) && !string.IsNullOrEmpty(response.Data[i].WhatsNewList[j].EndDate))
                                    {
                                        DateTime startDateTime = DateTime.ParseExact(response.Data[i].WhatsNewList[j].StartDate, "yyyyMMddTHHmmss",
                                        CultureInfo.InvariantCulture, DateTimeStyles.None);
                                        DateTime stopDateTime = DateTime.ParseExact(response.Data[i].WhatsNewList[j].EndDate, "yyyyMMddTHHmmss",
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
                                    WhatsNewModel mModel = response.Data[i].WhatsNewList[j];

                                    WhatsNewEntity searchItem = mWhatsNewEntity.GetItem(mModel.ID);
                                    if (searchItem != null)
                                    {
                                        mModel.Read = searchItem.Read;
                                        mModel.ReadDateTime = searchItem.ReadDateTime;
                                    }
                                    localList.Add(mModel);
                                }
                            }

                            if (localList.Count > 0)
                            {
                                ToStoredList.Add(new WhatsNewCategoryModel()
                                {
                                    ID = response.Data[i].ID,
                                    CategoryName = response.Data[i].CategoryName
                                });

                                ToStoredWhatsNewList.AddRange(localList);
                            }
                        }
                    }

                    mWhatsNewCategoryEntity.DeleteTable();
                    mWhatsNewEntity.DeleteTable();
                    mWhatsNewCategoryEntity.CreateTable();
                    mWhatsNewEntity.CreateTable();

                    if (ToStoredList.Count > 0)
                    {
                        mWhatsNewCategoryEntity.InsertListOfItems(ToStoredList);
                        mWhatsNewEntity.InsertListOfItems(ToStoredWhatsNewList);
                    }
                }
                else
                {
                    mWhatsNewCategoryEntity.DeleteTable();
                    mWhatsNewEntity.DeleteTable();
                    mWhatsNewCategoryEntity.CreateTable();
                    mWhatsNewEntity.CreateTable();
                }

                _ = OnGetUserWhatsNewList();
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
                IsButtonShow = true
            });

            return newList;
        }
    }
}
