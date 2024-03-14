using System;
using myTNB.Android.Src.Database.Model;
using myTNB.Android.Src.myTNBMenu.MVP;
using myTNB.SitecoreCMS.Model;
using System.Collections.Generic;
using System.Globalization;
using Android.OS;
using System.Threading;
using myTNB.Android.Src.SitecoreCMS.Model;
using myTNB.Android.Src.Utils;
using System.Threading.Tasks;
using Android.App;
using myTNB.SitecoreCMS.Services;
using myTNB.Android.Src.SiteCore;
using Refit;
using System.Diagnostics;

namespace myTNB.Android.Src.myTNBMenu.Async
{
	public class SiteCoreFloatingButtonContentAPI : AsyncTask
    {
        private static int FloatingButtonDefaultTimeOutMillisecond = 4000;
        private int FloatingButtonTimeOutMillisecond = FloatingButtonDefaultTimeOutMillisecond;
        private string savedContentTimeStamp = "0000000";
        CancellationTokenSource cts = null;

        private DashboardHomeContract.IView mHomeView = null;

        private bool isSitecoreApiFailed = false;

        private FloatingButtonMarketingTimeStampResponseModel responseMasterModel = new FloatingButtonMarketingTimeStampResponseModel();

        public SiteCoreFloatingButtonContentAPI(DashboardHomeContract.IView mView)
        {
            this.mHomeView = mView;
        }

        protected override void OnPreExecute()
        {

        }

        protected override Java.Lang.Object DoInBackground(params Java.Lang.Object[] @params)
        {
            try
            {
                isSitecoreApiFailed = false;
                Console.WriteLine("000 SiteCoreFloatingButtonContentAPI started");
                FloatingButtonMarketingParentEntity wtManager = new FloatingButtonMarketingParentEntity();

                try
                {
                    List<FloatingButtonMarketingParentEntity> saveditems = wtManager.GetAllItems();
                    if (saveditems != null && saveditems.Count > 0)
                    {
                        FloatingButtonMarketingParentEntity entity = saveditems[0];
                        if (entity != null)
                        {
                            savedContentTimeStamp = entity.Timestamp;
                        }
                    }
                }
                catch (Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                }

                //Get Sitecore floatingbutton content timestamp
                bool getSiteCoreContent = false;
                cts = new CancellationTokenSource();
                Stopwatch sw = Stopwatch.StartNew();
                Task.Factory.StartNew(() =>
                {
                    try
                    {
                        string density = DPUtils.GetDeviceDensity(Application.Context);
                        GetItemsService getItemsService = new GetItemsService(SiteCoreConfig.OS, density, SiteCoreConfig.SITECORE_URL, LanguageUtil.GetAppLanguage());
                        responseMasterModel = getItemsService.GetFloatingButtonMarketingTimestampItem();
                        if (responseMasterModel.Status.Equals("Success"))
                        {

                            if (responseMasterModel.Data != null && responseMasterModel.Data.Count > 0)
                            {
                                if (!responseMasterModel.Data[0].Timestamp.Equals(savedContentTimeStamp))
                                {
                                    getSiteCoreContent = true;
                                }
                                else
                                {
                                    getSiteCoreContent = false;
                                }
                            }
                            else
                            {
                                List<FloatingButtonMarketingParentEntity> items = wtManager.GetAllItems();
                                if (items != null && items.Count > 0)
                                {
                                    FloatingButtonMarketingParentEntity entity = items[0];
                                    if (entity != null)
                                    {
                                        if (!entity.Timestamp.Equals(savedContentTimeStamp))
                                        {
                                            getSiteCoreContent = true;
                                        }
                                        else
                                        {
                                            FloatingButtonMarketingParentEntity mEntityCheck = new FloatingButtonMarketingParentEntity();
                                            List<FloatingButtonMarketingParentEntity> mCheckList = mEntityCheck.GetAllItems();
                                            if (mCheckList == null || mCheckList.Count == 0)
                                            {
                                                getSiteCoreContent = true;
                                            }
                                            else
                                            {
                                                getSiteCoreContent = false;

                                            }
                                        }
                                    }
                                    else
                                    {
                                        getSiteCoreContent = true;
                                    }
                                }
                                else
                                {
                                    getSiteCoreContent = true;
                                }
                            }
                        }
                        else
                        {
                            getSiteCoreContent = true;
                        }
                        //try
                        //{
                        //    if (FloatingButtonTimeOutMillisecond > 0)
                        //    {
                        //        FloatingButtonTimeOutMillisecond = FloatingButtonTimeOutMillisecond - (int)sw.ElapsedMilliseconds;
                        //        if (FloatingButtonTimeOutMillisecond <= 0)
                        //        {
                        //            FloatingButtonTimeOutMillisecond = 0;
                        //        }
                        //    }
                        //}
                        //catch (Exception e)
                        //{
                        //    Utility.LoggingNonFatalError(e);
                        //}

                        //if (responseMasterModel.Status.Equals("Success"))
                        //{
                        //    FloatingButtonMarketingParentEntity wtManager = new FloatingButtonMarketingParentEntity();
                        //    wtManager.DeleteTable();
                        //    wtManager.CreateTable();
                        //    wtManager.InsertListOfItems(responseMasterModel.Data);
                        //    OnFBContentTimeStampRecieved(responseMasterModel.Data[0].Timestamp);
                        //}
                        //else
                        //{
                        //    OnFBContentTimeStampRecieved(null);
                        //}

                        if (getSiteCoreContent)
                        {
                            cts = new CancellationTokenSource();
                            Task.Factory.StartNew(() =>
                            {
                                try
                                {
                                    string newDensity = DPUtils.GetDeviceDensity(Application.Context);
                                    GetItemsService getFBItemsService = new GetItemsService(SiteCoreConfig.OS, newDensity, SiteCoreConfig.SITECORE_URL, LanguageUtil.GetAppLanguage());
                                    FloatingButtonMarketingResponseModel responseModel = getFBItemsService.GetFloatingButtonMarketingItem();
                                    if (responseModel != null && !string.IsNullOrEmpty(responseModel.Status))
                                    {
                                        if (responseModel.Status.Equals("Success"))
                                        {
                                            if (responseMasterModel != null && responseMasterModel.Status != null && responseMasterModel.Status.Equals("Success") && responseMasterModel.Data != null && responseMasterModel.Data.Count > 0)
                                            {
                                                FloatingButtonMarketingParentEntity mWhatsNewParentEntity = new FloatingButtonMarketingParentEntity();
                                                mWhatsNewParentEntity.DeleteTable();
                                                mWhatsNewParentEntity.CreateTable();
                                                mWhatsNewParentEntity.InsertListOfItems(responseMasterModel.Data);
                                            }


                                            try
                                            {
                                                if (FloatingButtonTimeOutMillisecond > 0)
                                                {
                                                    FloatingButtonTimeOutMillisecond = FloatingButtonTimeOutMillisecond - (int)sw.ElapsedMilliseconds;
                                                    if (FloatingButtonTimeOutMillisecond <= 0)
                                                    {
                                                        FloatingButtonTimeOutMillisecond = 0;
                                                    }
                                                }
                                            }
                                            catch (Exception e)
                                            {
                                                Utility.LoggingNonFatalError(e);
                                            }

                                            FloatingButtonMarketingEntity wtManager = new FloatingButtonMarketingEntity();
                                            wtManager.DeleteTable();
                                            wtManager.CreateTable();
                                            wtManager.InsertListOfItems(responseModel.Data);
                                            OnGetFBContentCache();
                                           
                                        }
                                        else
                                        {
                                           
                                            OnGetFBContentCache();
                                           
                                        }
                                    }
                                    else
                                    {
                                        FloatingButtonMarketingParentEntity wtManager2 = new FloatingButtonMarketingParentEntity();
                                        wtManager2.DeleteTable();
                                        wtManager2.CreateTable();
                                        isSitecoreApiFailed = true;
                                        //OnGetFBContentCache();
                                    }
                                }
                                catch (System.Exception e)
                                {
                                    FloatingButtonMarketingParentEntity wtManager2 = new FloatingButtonMarketingParentEntity();
                                    wtManager2.DeleteTable();
                                    wtManager2.CreateTable();
                                    isSitecoreApiFailed = true;
                                    Utility.LoggingNonFatalError(e);
                                }
                            }).ContinueWith((Task previous) =>
                            {
                            }, cts.Token);
                        }
                        else
                        {
                            OnGetFBContentCache();
                        }
                    }
                    catch (System.Exception e)
                    {
                        FloatingButtonMarketingParentEntity wtManager2 = new FloatingButtonMarketingParentEntity();
                        wtManager2.DeleteTable();
                        wtManager2.CreateTable();
                        isSitecoreApiFailed = true;
                        Utility.LoggingNonFatalError(e);
                    }
                }).ContinueWith((Task previous) =>
                {
                }, cts.Token);

                //if (FloatingButtonTimeOutMillisecond > 0)
                //{
                //    _ = Task.Delay(FloatingButtonTimeOutMillisecond).ContinueWith(_ =>
                //    {
                //        if (FloatingButtonTimeOutMillisecond > 0)
                //        {
                //            FloatingButtonTimeOutMillisecond = 0;
                //            OnGetFBContentCache();
                //        }
                //    });
                //}

            }
            catch (ApiException apiException)
            {
                FloatingButtonMarketingParentEntity wtManager2 = new FloatingButtonMarketingParentEntity();
                wtManager2.DeleteTable();
                wtManager2.CreateTable();
                isSitecoreApiFailed = true;
                Utility.LoggingNonFatalError(apiException);
            }
            catch (Newtonsoft.Json.JsonReaderException e)
            {
                FloatingButtonMarketingParentEntity wtManager2 = new FloatingButtonMarketingParentEntity();
                wtManager2.DeleteTable();
                wtManager2.CreateTable();
                isSitecoreApiFailed = true;
                Utility.LoggingNonFatalError(e);
            }
            catch (System.Exception e)
            {
                FloatingButtonMarketingParentEntity wtManager2 = new FloatingButtonMarketingParentEntity();
                wtManager2.DeleteTable();
                wtManager2.CreateTable();
                isSitecoreApiFailed = true;
                Utility.LoggingNonFatalError(e);
            }
            Console.WriteLine("000 SiteCoreWhatsNewAPI ended");
            return null;
        }

        public void OnFBContentTimeStampRecieved(string timestamp)
        {
            try
            {
                if (timestamp != null)
                {
                    if (timestamp.Equals(savedContentTimeStamp))
                    {
                        OnGetFBContentCache();
                    }
                    else
                    {
                        OnGetFBContentItem();
                    }
                }
                else
                {
                    OnGetFBContentCache();
                }
            }
            catch (Exception e)
            {
                OnGetFBContentCache();
                Utility.LoggingNonFatalError(e);
            }
        }

        public void OnGetFBContentItem()
        {
            CancellationTokenSource token = new CancellationTokenSource();
            Stopwatch sw = Stopwatch.StartNew();
            _ = Task.Run(() =>
            {
                try
                {
                    string density = DPUtils.GetDeviceDensity(Application.Context);
                    GetItemsService getItemsService = new GetItemsService(SiteCoreConfig.OS, density, SiteCoreConfig.SITECORE_URL, LanguageUtil.GetAppLanguage());
                    FloatingButtonMarketingResponseModel responseModel = getItemsService.GetFloatingButtonMarketingItem();
                    sw.Stop();
                    try
                    {
                        if (FloatingButtonTimeOutMillisecond > 0)
                        {
                            FloatingButtonTimeOutMillisecond = FloatingButtonTimeOutMillisecond - (int)sw.ElapsedMilliseconds;
                            if (FloatingButtonTimeOutMillisecond <= 0)
                            {
                                FloatingButtonTimeOutMillisecond = 0;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Utility.LoggingNonFatalError(e);
                    }

                    if (responseModel.Status.Equals("Success"))
                    {
                        //IsOnGetPhotoRunning = false;
                        FloatingButtonMarketingEntity wtManager = new FloatingButtonMarketingEntity();
                        wtManager.DeleteTable();
                        wtManager.CreateTable();
                        wtManager.InsertListOfItems(responseModel.Data);
                        OnGetFBContentCache();
                    }
                    else
                    {
                        OnGetFBContentCache();
                    }
                }
                catch (Exception e)
                {
                    OnGetFBContentCache();
                    Utility.LoggingNonFatalError(e);
                }
            }, token.Token);

            if (FloatingButtonTimeOutMillisecond > 0)
            {
                _ = Task.Delay(FloatingButtonTimeOutMillisecond).ContinueWith(_ =>
                {
                    if (FloatingButtonTimeOutMillisecond > 0)
                    {
                        FloatingButtonTimeOutMillisecond = 0;
                        OnGetFBContentCache();
                    }
                });
            }
        }


        public Task OnGetFBContentCache()
        {
            CancellationTokenSource token = new CancellationTokenSource();
            return Task.Run(() =>
            {
                try
                {
                    FloatingButtonMarketingEntity wtManager = new FloatingButtonMarketingEntity();
                    List<FloatingButtonMarketingEntity> contentList = wtManager.GetAllItems();
                    if (contentList.Count > 0)
                    {
                        FloatingButtonMarketingModel item = new FloatingButtonMarketingModel()
                        {
                            ID = contentList[0].ID,
                            Title = contentList[0].Title,
                            ButtonTitle = contentList[0].ButtonTitle,
                            Description = contentList[0].Description,
                            Description_Images = contentList[0].Description_Images,
                            Infographic_FullView_URL = contentList[0].Infographic_FullView_URL,
                            Infographic_FullView_URL_ImageB64 = contentList[0].Infographic_FullView_URL_ImageB64,
                        };
                        FloatingButtonMarketingUtils.SetFloatingButtonMarketingBitmap(item);
                    }
                    else
                    {
                        FloatingButtonTimeOutMillisecond = 0;
                        //if (!this.mView.GetFloatingButtonSiteCoreDoneFlag())
                        //{
                        //    this.mView.SetDefaultAppLaunchImage();
                        //}
                    }
                }
                catch (Exception e)
                {
                    FloatingButtonTimeOutMillisecond = 0;
                    //if (!this.mView.GetAppLaunchSiteCoreDoneFlag())
                    //{
                    //    this.mView.SetDefaultAppLaunchImage();
                    //}
                    Utility.LoggingNonFatalError(e);
                }
            }, token.Token);
        }

        private string GetCurrentDate()
        {
            DateTime currentDate = DateTime.Now;
            CultureInfo currCult = CultureInfo.CreateSpecificCulture("en-US");
            return currentDate.ToString(@"yyyyMMddTHHmmss", currCult);
        }

        protected override void OnPostExecute(Java.Lang.Object result)
        {
            base.OnPostExecute(result);
        }
    }
}

