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

        private WhatsNewTimeStampResponseModel responseMasterModel = new WhatsNewTimeStampResponseModel();

        public WhatsNewMenuPresenter(WhatsNewMenuContract.IWhatsNewMenuView view, ISharedPreferences pref)
        {
            this.mView = view;
            this.mPref = pref;
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
                    responseMasterModel = getItemsService.GetWhatsNewTimestampItem();
                    if (responseMasterModel.Status.Equals("Success") && responseMasterModel.Data != null && responseMasterModel.Data.Count > 0)
                    {
                        this.mView.CheckWhatsNewsTimeStamp(responseMasterModel.Data[0].Timestamp);
                    }
                    else
                    {
                        this.mView.CheckWhatsNewsTimeStamp(null);
                    }
                }
                catch (Exception e)
                {
                    this.mView.CheckWhatsNewsTimeStamp(null);
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
                            if (responseMasterModel != null && responseMasterModel.Status != null && responseMasterModel.Status.Equals("Success") && responseMasterModel.Data != null && responseMasterModel.Data.Count > 0)
                            {
                                if (mWhatsNewParentEntity == null)
                                {
                                    mWhatsNewParentEntity = new WhatsNewParentEntity();
                                }
                                mWhatsNewParentEntity.DeleteTable();
                                mWhatsNewParentEntity.CreateTable();
                                mWhatsNewParentEntity.InsertListOfItems(responseMasterModel.Data);
                            }
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
                        TabTitle = Utility.GetLocalizedLabel("WhatsNew", "viewAll"),
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
                                        mModel.ShowDateForDay = searchItem.ShowDateForDay;
                                        mModel.ShowCountForDay = searchItem.ShowCountForDay;
                                        mModel.SkipShowOnAppLaunch = searchItem.SkipShowOnAppLaunch;
                                    }
                                    else
                                    {
                                        mModel.ShowDateForDay = GetCurrentDate();
                                        mModel.ShowCountForDay = 0;
                                        mModel.SkipShowOnAppLaunch = false;
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

                CheckWhatsNewsCache();
            }
            catch (Exception e)
            {
                this.mView.SetRefreshView(null, null);
                Utility.LoggingNonFatalError(e);
            }
        }

        private string GetCurrentDate()
        {
            DateTime currentDate = DateTime.Now;
            CultureInfo currCult = CultureInfo.CreateSpecificCulture("en-US");
            return currentDate.ToString(@"yyyyMMddTHHmmss", currCult);
        }

        public List<NewAppModel> OnGeneraNewAppTutorialList()
        {
            List<NewAppModel> newList = new List<NewAppModel>();

            if (this.mView.CheckTabVisibility())
            {
                newList.Add(new NewAppModel()
                {
                    ContentShowPosition = ContentType.BottomLeft,
                    ContentTitle = Utility.GetLocalizedLabel("WhatsNew", "tutorialCategoryTitle"), // "Get caught up with TNB news.",
                    ContentMessage = Utility.GetLocalizedLabel("WhatsNew", "tutorialCategoryDesc"), // "Switch between the categories to view announcements and/or promotions relevant to you.",
                    ItemCount = 0,
                    DisplayMode = "",
                    IsButtonShow = false
                });
            }

            newList.Add(new NewAppModel()
            {
                ContentShowPosition = ContentType.BottomLeft,
                ContentTitle = Utility.GetLocalizedLabel("WhatsNew", "tutorialItemTitle"), // "Check out what’s new and exciting.",
                ContentMessage = Utility.GetLocalizedLabel("WhatsNew", "tutorialItemDesc"), // "Tap on the promotion or announcement for more details.",
                ItemCount = 0,
                DisplayMode = "",
                IsButtonShow = false
            });

            return newList;
        }
    }
}
