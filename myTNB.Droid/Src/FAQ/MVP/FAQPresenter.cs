using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using myTNB_Android.Src.Utils;
using myTNB.SitecoreCMS.Services;
using myTNB_Android.Src.SiteCore;
using myTNB.SitecoreCMS.Model;
using Newtonsoft.Json;
using myTNB.SQLite.SQLiteDataManager;
using Android.Util;
using System.Threading;

namespace myTNB_Android.Src.FAQ.MVP
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
                List<FAQsParentEntity> items = wtManager.GetAllItems();
                if (items != null)
                {
                    FAQsParentEntity entity = items[0];
                    if (entity != null)
                    {
                        mView.OnSavedTimeStamp(entity.Timestamp);
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
                    GetItemsService getItemsService = new GetItemsService(SiteCoreConfig.OS, density, SiteCoreConfig.SITECORE_URL, SiteCoreConfig.DEFAULT_LANGUAGE);
                    string json = getItemsService.GetFAQsItem();
                    FAQsResponseModel responseModel = JsonConvert.DeserializeObject<FAQsResponseModel>(json);
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
                }
            }).ContinueWith((Task previous) => {
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
                    GetItemsService getItemsService = new GetItemsService(SiteCoreConfig.OS, density, SiteCoreConfig.SITECORE_URL, SiteCoreConfig.DEFAULT_LANGUAGE);
                    string json = getItemsService.GetFAQsTimestampItem();
                    FAQsParentResponseModel responseModel = JsonConvert.DeserializeObject<FAQsParentResponseModel>(json);
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
                }
            }).ContinueWith((Task previous) => {
            }, cts.Token);
        }

        public void Start()
        {
            
        }
    }
}