using Android.App;
using Android.OS;
using myTNB.SitecoreCMS.Model;
using myTNB.SitecoreCMS.Services;
using myTNB.SQLite.SQLiteDataManager;
using myTNB_Android.Src.myTNBMenu.MVP;
using myTNB_Android.Src.SiteCore;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;
using Refit;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace myTNB_Android.Src.myTNBMenu.Async
{
    public class SiteCorePromotioAPI : AsyncTask
    {

        private string savedPromoTimeStamp = "0000000";
        CancellationTokenSource cts = null;

        private DashboardContract.IView mView = null;

        public SiteCorePromotioAPI(DashboardContract.IView mView)
        {
            this.mView = mView;
        }

        protected override void OnPreExecute()
        {

        }

        protected override Java.Lang.Object DoInBackground(params Java.Lang.Object[] @params)
        {
            try
            {
                Console.WriteLine("000 SiteCorePromotioAPI started");
                PromotionsParentEntityV2 wtManager = new PromotionsParentEntityV2();
                wtManager.CreateTable();
                List<PromotionsParentEntityV2> saveditems = wtManager.GetAllItems();
                if (saveditems != null && saveditems.Count > 0)
                {
                    PromotionsParentEntityV2 entity = saveditems[0];
                    if (entity != null)
                    {
                        savedPromoTimeStamp = entity.Timestamp;
                    }
                }

                //Get Sitecore promotion timestamp
                bool getSiteCorePromotions = false;
                cts = new CancellationTokenSource();
                Task.Factory.StartNew(() =>
                {
                    try
                    {
                        string density = DPUtils.GetDeviceDensity(Application.Context);
                        GetItemsService getItemsService = new GetItemsService(SiteCoreConfig.OS, density, SiteCoreConfig.SITECORE_URL, SiteCoreConfig.DEFAULT_LANGUAGE);
                        string json = getItemsService.GetPromotionsTimestampItem();
                        PromotionsParentV2ResponseModel responseModel = JsonConvert.DeserializeObject<PromotionsParentV2ResponseModel>(json);
                        if (responseModel.Status.Equals("Success"))
                        {
                            //PromotionsParentEntityV2 wtManager = new PromotionsParentEntityV2();
                            wtManager.DeleteTable();
                            wtManager.CreateTable();
                            wtManager.InsertListOfItems(responseModel.Data);
                            List<PromotionsParentEntityV2> items = wtManager.GetAllItems();
                            if (items != null && items.Count > 0)
                            {
                                PromotionsParentEntityV2 entity = items[0];
                                if (entity != null)
                                {
                                    if (!entity.Timestamp.Equals(savedPromoTimeStamp))
                                    {
                                        getSiteCorePromotions = true;
                                    }
                                    else
                                    {
                                        getSiteCorePromotions = false;
                                    }
                                }
                            }

                            //Log.Debug("WalkThroughResponse", responseModel.Data.ToString());
                        }
                        else
                        {
                            getSiteCorePromotions = true;
                        }
                    }
                    catch (System.Exception e)
                    {
                        //Log.Error("API Exception", e.StackTrace);
                        Utility.LoggingNonFatalError(e);
                    }
                }).ContinueWith((Task previous) =>
                {
                }, cts.Token);

                //promotionWatch.Stop();
                //Console.WriteLine($"Execution Time for promotion: {promotionWatch.ElapsedMilliseconds} ms");

                if (getSiteCorePromotions)
                {
                    //var getPromotionWatch = new System.Diagnostics.Stopwatch();
                    //getPromotionWatch.Start();

                    cts = new CancellationTokenSource();
                    Task.Factory.StartNew(() =>
                    {
                        try
                        {
                            string density = DPUtils.GetDeviceDensity(Application.Context);
                            GetItemsService getItemsService = new GetItemsService(SiteCoreConfig.OS, density, SiteCoreConfig.SITECORE_URL, SiteCoreConfig.DEFAULT_LANGUAGE);
                            string json = getItemsService.GetPromotionsV2Item();
                            PromotionsV2ResponseModel promoResponseModel = JsonConvert.DeserializeObject<PromotionsV2ResponseModel>(json);
                            if (promoResponseModel.Status.Equals("Success"))
                            {
                                PromotionsEntityV2 wtManager2 = new PromotionsEntityV2();
                                wtManager2.DeleteTable();
                                wtManager2.CreateTable();
                                wtManager2.InsertListOfItems(promoResponseModel.Data);
                                //Log.Debug("DashboardPresenter", promoResponseModel.Data.ToString());
                            }

                        }
                        catch (System.Exception e)
                        {
                            //Log.Error("API Exception", e.StackTrace);
                            Utility.LoggingNonFatalError(e);
                        }
                    }).ContinueWith((Task previous) =>
                    {
                    }, cts.Token);


                    //getPromotionWatch.Stop();
                    //Console.WriteLine($"Execution Time for get promotion: {getPromotionWatch.ElapsedMilliseconds} ms");
                }
            }
            catch (ApiException apiException)
            {
                Utility.LoggingNonFatalError(apiException);
            }
            catch (Newtonsoft.Json.JsonReaderException e)
            {
                Utility.LoggingNonFatalError(e);
            }
            catch (System.Exception e)
            {
                //Log.Error("DB Exception", e.StackTrace);
                Utility.LoggingNonFatalError(e);
            }
            Console.WriteLine("000 SiteCorePromotioAPI ended");
            return null;
        }

        protected override void OnPostExecute(Java.Lang.Object result)
        {
            base.OnPostExecute(result);


            mView.ShowPromotion(true);
        }

    }
}
