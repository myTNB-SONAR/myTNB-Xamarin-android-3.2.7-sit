using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using myTNB_Android.Src.Utils;
using myTNB.SitecoreCMS.Services;
using myTNB.SitecoreCMS.Model;
using myTNB_Android.Src.SiteCore;
using Newtonsoft.Json;
using Android.Util;
using myTNB.SQLite.SQLiteDataManager;
using System.Threading.Tasks;

namespace myTNB_Android.Src.WalkThrough.MVP
{
    public class WalkThroughPresenter : WalkThroughContract.IUserActionsListener
    {
        private WalkThroughContract.IView mView;
        private ISharedPreferences mPrefs;

        public WalkThroughPresenter(WalkThroughContract.IView mView , ISharedPreferences sharedPreferences)
        {
            this.mView = mView;
            this.mPrefs = sharedPreferences;
            this.mView.SetPresenter(this);
        }

        public void NavigatePrelogin()
        {
            this.mView.ShowPreLogin();
        }


        public void Start()
        {
            //OnGetWalkThroughData();
            //GetSavedTimeStamp();
        }


        public void NavigateNextScreen()
        {
            int index = this.mView.GetCurrentItem() + 1;
            this.mView.ShowNext(index);
        }

        public void OnPageSelected(int position)
        {
            int index = position + 1;
            if (index == this.mView.GetTotalItems() )
            {
                this.mView.ShowDone();
            }
        }

        public void OnSkip()
        {
            UserSessions.DoSkipped(mPrefs);
        }

        public void GetSavedTimeStamp()
        {
            try
            {
                TimeStampEntity wtManager = new TimeStampEntity();
                List<TimeStampEntity> items = wtManager.GetAllItems();
                if(items != null && items.Count != 0)
                {
                    foreach(TimeStampEntity obj in items)
                    {
                        this.mView.OnSavedTimeStampRecievd(obj.Timestamp);
                    }
                }
                else
                {
                    this.mView.OnSavedTimeStampRecievd(null);
                }

            }catch(Exception e)
            {
                Log.Error("API Exception", e.StackTrace);
                Utility.LoggingNonFatalError(e);
            }
        }

        public Task OnGetTimeStamp()
        {
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    string density = DPUtils.GetDeviceDensity(Application.Context);
                    GetItemsService getItemsService = new GetItemsService(SiteCoreConfig.OS, density, SiteCoreConfig.SITECORE_URL, SiteCoreConfig.DEFAULT_LANGUAGE);
                    string json = getItemsService.GetTimestampItem();
                    TimestampResponseModel responseModel = JsonConvert.DeserializeObject<TimestampResponseModel>(json);
                    if(responseModel.Status.Equals("Success"))
                    {
                        TimeStampEntity wtManager = new TimeStampEntity();
                        wtManager.DeleteTable();
                        wtManager.CreateTable();
                        wtManager.InsertListOfItems(responseModel.Data);
                        mView.OnTimeStampRecieved(responseModel.Data[0].Timestamp);
                    }
                    else
                    {
                        mView.OnTimeStampRecieved(null);
                    }
                }
                catch (Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                    Log.Error("API Exception", e.StackTrace);
                }
            });
        }

        public Task OnGetWalkThroughData()
        {
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    string density = DPUtils.GetDeviceDensity(Application.Context);
                    GetItemsService getItemsService = new GetItemsService(SiteCoreConfig.OS, density, SiteCoreConfig.SITECORE_URL, SiteCoreConfig.DEFAULT_LANGUAGE);

                    string json = getItemsService.GetWalkthroughScreenItems();
                    WalkthroughScreensResponseModel responseModel = JsonConvert.DeserializeObject<WalkthroughScreensResponseModel>(json);
                    if (responseModel.Status.Equals("Success"))
                    {
                        WalkthroughScreensEntity wtManager = new WalkthroughScreensEntity();
                        wtManager.DeleteTable();
                        wtManager.CreateTable();
                        wtManager.InsertListOfItems(responseModel.Data);
                        mView.ShowWalkThroughData(true);
                        Log.Debug("WalkThroughResponse", responseModel.Data.ToString());
                    }
                    else
                    {
                        mView.ShowWalkThroughData(false);
                    }
                }
                catch (Exception e)
                {
                    Log.Error("API Exception", e.StackTrace);
                    Utility.LoggingNonFatalError(e);
                }
            });
        }
    }
}