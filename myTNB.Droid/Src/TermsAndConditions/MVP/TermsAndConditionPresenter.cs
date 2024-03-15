using Android.App;
using Android.Util;
using myTNB.SitecoreCMS.Model;
using myTNB.SitecoreCMS.Services;
using myTNB.SQLite.SQLiteDataManager;
using myTNB.AndroidApp.Src.Database.Model;
using myTNB.AndroidApp.Src.SiteCore;
using myTNB.AndroidApp.Src.SitecoreCMS.Model;
using myTNB.AndroidApp.Src.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace myTNB.AndroidApp.Src.TermsAndConditions.MVP
{
    public class TermsAndConditionPresenter : TermsAndConditionContract.IUserActionsListener
    {
        private TermsAndConditionContract.IView mView;
        private CancellationTokenSource cts;

        public TermsAndConditionPresenter(TermsAndConditionContract.IView view)
        {
            this.mView = view;
            this.mView.SetPresenter(this);
        }

        public void GetSavedTermsAndConditionTimeStamp()
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

        public Task OnGetTermsAndConditionTimeStamp()
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
                        mView.ShowTermsAndConditionTimestamp(true);
                    }
                    else
                    {
                        mView.ShowTermsAndConditionTimestamp(false);
                    }
                }
                catch (Exception e)
                {
                    mView.ShowTermsAndConditionTimestamp(false);
                    Utility.LoggingNonFatalError(e);
                }
            }).ContinueWith((Task previous) =>
            {
            }, cts.Token);
        }

        public Task GetTermsAndConditionData()
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
                        mView.ShowTermsAndCondition(true);
                        Log.Debug("FullRTEResponse", responseModel.Data.ToString());
                    }
                    else
                    {
                        mView.ShowTermsAndCondition(false);
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
            //GetTermsAndConditionData();
        }
    }
}