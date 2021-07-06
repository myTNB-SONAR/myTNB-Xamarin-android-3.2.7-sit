using Android.App;
using Android.Util;
using myTNB.SitecoreCMS.Model;
using myTNB.SitecoreCMS.Services;
using myTNB.SQLite.SQLiteDataManager;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.SiteCore;
using myTNB_Android.Src.SitecoreCMS.Model;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace myTNB_Android.Src.DigitalBill.MVP
{
    public class DigitalBillPresenter : DigitalBillContract.IUserActionsListener
    {
        private DigitalBillContract.IView mView;
        private CancellationTokenSource cts;

        public DigitalBillPresenter(DigitalBillContract.IView view)
        {
            this.mView = view;
            this.mView.SetPresenter(this);
        }

        public void GetSavedDigitalBillTimeStamp()
        {
            try
            {
                TimeStampEntity wtManager = new TimeStampEntity();
                List<TimeStampEntity> items = new List<TimeStampEntity>();
                items = wtManager.GetAllItems();
                if (items != null && items.Count > 0)
                {
                    TimeStampEntity entity = items[0];
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

        public Task OnGetDigitalBillTimeStamp()
        {
            cts = new CancellationTokenSource();
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    string density = DPUtils.GetDeviceDensity(Application.Context);
                    GetItemsService getItemsService = new GetItemsService(SiteCoreConfig.OS, density, SiteCoreConfig.SITECORE_URL, LanguageUtil.GetAppLanguage());
                    TimestampResponseModel responseModel = getItemsService.GetTimestampItem();
                    if (responseModel.Status.Equals("Success"))
                    {
                        TimeStampEntity wtManager = new TimeStampEntity();
                        wtManager.DeleteTable();
                        wtManager.CreateTable();
                        wtManager.InsertListOfItems(responseModel.Data);
                        mView.ShowDigitalBillTimestamp(true);
                    }
                    else
                    {
                        mView.ShowDigitalBillTimestamp(false);
                    }
                }
                catch (Exception e)
                {
                    mView.ShowDigitalBillTimestamp(false);
                    Utility.LoggingNonFatalError(e);
                }
            }).ContinueWith((Task previous) =>
            {
            }, cts.Token);
        }

        public Task GetDigitalBillData()
        {
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    string density = DPUtils.GetDeviceDensity(Application.Context);
                    GetItemsService getItemsService = new GetItemsService(SiteCoreConfig.OS, density, SiteCoreConfig.SITECORE_URL, LanguageUtil.GetAppLanguage());
                    FullRTEPagesResponseModel responseModel = getItemsService.GetFullRTEPagesItems();
                    if (responseModel.Status.Equals("Success"))
                    {
                        FullRTEPagesEntity wtManager = new FullRTEPagesEntity();
                        wtManager.DeleteTable();
                        wtManager.CreateTable();
                        wtManager.InsertListOfItems(responseModel.Data);
                        mView.ShowDigitalBill(true);
                        Log.Debug("FullRTEResponse", responseModel.Data.ToString());
                    }
                    else
                    {
                        mView.ShowDigitalBill(false);
                    }
                }
                catch (Exception e)
                {
                    Log.Error("API Exception", e.StackTrace);
                    mView.HideProgressBar();
                    Utility.LoggingNonFatalError(e);
                }
            });
        }

        public void Start()
        {
            //No Impl
            //GetDigitalBillData();
        }
    }
}