using Android.App;
using Android.Util;
using myTNB.SitecoreCMS.Model;
using myTNB.SitecoreCMS.Services;
using myTNB.SQLite.SQLiteDataManager;
using myTNB.Android.Src.SiteCore;
using myTNB.Android.Src.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace myTNB.Android.Src.FAQ.MVP
{
    public class FAQPresenter : FAQContract.IUserActionsListener
    {
        private FAQContract.IView mView;
        private CancellationTokenSource cts;

        public FAQPresenter(FAQContract.IView view)
        {
            this.mView = view;
            this.mView.SetPresenter(this);
        }

        public void GetSavedFAQTimeStamp()
        {
            try
            {
                FAQsParentEntity wtManager = new FAQsParentEntity();
                List<FAQsParentEntity> items = new List<FAQsParentEntity>();
                items = wtManager.GetAllItems();
                if (items != null && items.Count() > 0)
                {
                    FAQsParentEntity entity = items[0];
                    if (entity != null && !string.IsNullOrEmpty(entity?.Timestamp))
                    {
                        mView.OnSavedTimeStamp(entity?.Timestamp);
                    }
                    else
                    {
                        mView.OnSavedTimeStamp(null);
                    }
                }
                else
                {
                    mView.OnSavedTimeStamp(null);
                }
            }
            catch (Exception e)
            {
                Log.Error("DB Exception", e.StackTrace);
                mView.OnSavedTimeStamp(null);
                Utility.LoggingNonFatalError(e);
            }
        }

        public Task OnGetFAQs()
        {
            cts = new CancellationTokenSource();
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    string density = DPUtils.GetDeviceDensity(Application.Context);
                    GetItemsService getItemsService = new GetItemsService(SiteCoreConfig.OS, density, SiteCoreConfig.SITECORE_URL, LanguageUtil.GetAppLanguage());
                    FAQsResponseModel responseModel = getItemsService.GetFAQsItem();
                    if (responseModel.Status.Equals("Success"))
                    {
                        FAQsEntity wtManager = new FAQsEntity();
                        wtManager.DeleteTable();
                        wtManager.CreateTable();
                        wtManager.InsertListOfItems(responseModel.Data);
                        mView.ShowFAQ(true);
                        Log.Debug("FAQsResponse", responseModel.Data.ToString());
                    }
                    else
                    {
                        mView.ShowFAQ(false);
                    }
                }
                catch (Exception e)
                {
                    Log.Error("API Exception", e.StackTrace);
                    mView.ShowFAQ(false);
                    Utility.LoggingNonFatalError(e);
                }
            }).ContinueWith((Task previous) =>
            {
            }, cts.Token);
        }

        public Task OnGetFAQTimeStamp()
        {
            cts = new CancellationTokenSource();
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    string density = DPUtils.GetDeviceDensity(Application.Context);
                    GetItemsService getItemsService = new GetItemsService(SiteCoreConfig.OS, density, SiteCoreConfig.SITECORE_URL, LanguageUtil.GetAppLanguage());
                    FAQsParentResponseModel responseModel = getItemsService.GetFAQsTimestampItem();
                    if (responseModel.Status.Equals("Success"))
                    {
                        FAQsParentEntity wtManager = new FAQsParentEntity();
                        wtManager.DeleteTable();
                        wtManager.CreateTable();
                        wtManager.InsertListOfItems(responseModel.Data);
                        mView.ShowFAQTimestamp(true);
                        Log.Debug("WalkThroughResponse", responseModel.Data.ToString());
                    }
                    else
                    {
                        mView.ShowFAQTimestamp(false);
                    }
                }
                catch (Exception e)
                {
                    Log.Error("API Exception", e.StackTrace);
                    mView.ShowFAQTimestamp(false);
                    Utility.LoggingNonFatalError(e);
                }
            }).ContinueWith((Task previous) =>
            {
            }, cts.Token);
        }

        public void Start()
        {

        }
    }
}
