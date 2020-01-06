using Android.App;
using Android.Util;
using myTNB.SitecoreCMS.Model;
using myTNB.SitecoreCMS.Services;
using myTNB.SQLite.SQLiteDataManager;
using myTNB_Android.Src.SiteCore;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace myTNB_Android.Src.Promotions.MVP
{
    public class PromotionPresenter : PromotionContract.IUserActionsListener
    {

        private PromotionContract.IView mView;
        private CancellationTokenSource cts;

        public PromotionPresenter(PromotionContract.IView mView)
        {
            this.mView = mView;
            this.mView.SetPresenter(this);
        }

        public Task OnGetPromotionsTimeStamp()
        {
            cts = new CancellationTokenSource();
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    string density = DPUtils.GetDeviceDensity(Application.Context);
                    GetItemsService getItemsService = new GetItemsService(SiteCoreConfig.OS, density, SiteCoreConfig.SITECORE_URL, LanguageUtil.GetAppLanguage());
                    string json = getItemsService.GetPromotionsV2TimestampItem();
                    PromotionsParentV2ResponseModel responseModel = JsonConvert.DeserializeObject<PromotionsParentV2ResponseModel>(json);
                    if (responseModel.Status.Equals("Success"))
                    {
                        PromotionsParentEntityV2 wtManager = new PromotionsParentEntityV2();
                        wtManager.DeleteTable();
                        wtManager.CreateTable();
                        wtManager.InsertListOfItems(responseModel.Data);
                        mView.ShowPromotionTimestamp(true);
                    }
                    else
                    {
                        mView.ShowPromotionTimestamp(false);
                    }
                }
                catch (Exception e)
                {
                    mView.ShowPromotionTimestamp(false);
                    Utility.LoggingNonFatalError(e);
                }
            }).ContinueWith((Task previous) =>
            {
            }, cts.Token);
        }

        public Task OnGetPromotions()
        {
            cts = new CancellationTokenSource();
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    string density = DPUtils.GetDeviceDensity(Application.Context);
                    GetItemsService getItemsService = new GetItemsService(SiteCoreConfig.OS, density, SiteCoreConfig.SITECORE_URL, LanguageUtil.GetAppLanguage());
                    string json = getItemsService.GetPromotionsV2Item();
                    PromotionsV2ResponseModel responseModel = JsonConvert.DeserializeObject<PromotionsV2ResponseModel>(json);
                    if (responseModel.Status.Equals("Success"))
                    {
                        PromotionsEntityV2 wtManager = new PromotionsEntityV2();
                        wtManager.DeleteTable();
                        wtManager.CreateTable();
                        wtManager.InsertListOfItems(responseModel.Data);
                        mView.ShowPromotion(true);
                    }
                    else
                    {
                        PromotionsParentEntityV2 wtManager = new PromotionsParentEntityV2();
                        wtManager.DeleteTable();
                        wtManager.CreateTable();
                        PromotionsEntityV2 wtManager2 = new PromotionsEntityV2();
                        wtManager.DeleteTable();
                        wtManager.CreateTable();
                        mView.ShowPromotion(false);
                    }
                }
                catch (Exception e)
                {
                    PromotionsParentEntityV2 wtManager = new PromotionsParentEntityV2();
                    wtManager.DeleteTable();
                    wtManager.CreateTable();
                    PromotionsEntityV2 wtManager2 = new PromotionsEntityV2();
                    wtManager.DeleteTable();
                    wtManager.CreateTable();
                    mView.ShowPromotion(false);
                    Utility.LoggingNonFatalError(e);
                }
            }).ContinueWith((Task previous) =>
            {
            }, cts.Token);
        }

        public void Start()
        {

        }

        public void GetSavedPromotionTimeStamp()
        {
            try
            {
                PromotionsParentEntityV2 wtManager = new PromotionsParentEntityV2();
                List<PromotionsParentEntityV2> items = new List<PromotionsParentEntityV2>();
                items = wtManager.GetAllItems();
                if (items != null && items.Count() > 0)
                {
                    PromotionsParentEntityV2 entity = items[0];
                    if (entity != null && !string.IsNullOrEmpty(entity.Timestamp))
                    {
                        mView.OnSavedTimeStamp(entity.Timestamp);
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
                mView.OnSavedTimeStamp(null);
                Utility.LoggingNonFatalError(e);
            }
        }
    }
}
