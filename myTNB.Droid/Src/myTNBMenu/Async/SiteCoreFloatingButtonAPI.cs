using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Graphics;
using Android.OS;
using Android.Util;
using myTNB.SitecoreCMS.Model;
using myTNB.SitecoreCMS.Services;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.myTNBMenu.MVP;
using myTNB_Android.Src.SiteCore;
using myTNB_Android.Src.SitecoreCMS.Model;
using myTNB_Android.Src.Utils;
using Refit;

namespace myTNB_Android.Src.myTNBMenu.Async
{
	public class SiteCoreFloatingButtonAPI : AsyncTask
    {
        CancellationTokenSource cts = null;
        private static int FloatingButtonDefaultTimeOutMillisecond = 4000;
        private int FloatingButtonTimeOutMillisecond = FloatingButtonDefaultTimeOutMillisecond;
        private string savedFloatingButtonTimeStamp = "0000000";
        private bool IsOnGetPhotoRunning = false;

        private bool isSitecoreApiFailed = false;
        private DashboardHomeContract.IView mHomeView = null;

        private FloatingButtonTimeStampResponseModel responseMasterModel = new FloatingButtonTimeStampResponseModel();

        public SiteCoreFloatingButtonAPI(DashboardHomeContract.IView mView)
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
                Console.WriteLine("000 SiteCoreFloatingButtonAPI started");
                FloatingButtonParentEntity wtManager = new FloatingButtonParentEntity();

                try
                {
                    List<FloatingButtonParentEntity> saveditems = wtManager.GetAllItems();
                    if (saveditems != null && saveditems.Count > 0)
                    {
                        FloatingButtonParentEntity entity = saveditems[0];
                        if (entity != null)
                        {
                            savedFloatingButtonTimeStamp = entity.Timestamp;
                        }
                    }
                }
                catch (Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                }

                //Get Sitecore FloatingButton timestamp
                bool getSiteCoreFloatingButton = false;
                cts = new CancellationTokenSource();
                Task.Factory.StartNew(() =>
                {
                    try
                    {
                        string density = DPUtils.GetDeviceDensity(Application.Context);
                        GetItemsService getItemsService = new GetItemsService(SiteCoreConfig.OS, density, SiteCoreConfig.SITECORE_URL, LanguageUtil.GetAppLanguage());
                        FloatingButtonTimeStampResponseModel responseModelTimeStamp = getItemsService.GetFloatingButtonTimestampItem();
                        if (responseModelTimeStamp.Status.Equals("Success"))
                        {
                            if (responseModelTimeStamp.Data != null && responseModelTimeStamp.Data.Count > 0)
                            {
                                if (!responseModelTimeStamp.Data[0].Timestamp.Equals(savedFloatingButtonTimeStamp))
                                {
                                    getSiteCoreFloatingButton = true;
                                }
                                else
                                {
                                    getSiteCoreFloatingButton = false;
                                }
                            }
                            else
                            {
                                List<FloatingButtonParentEntity> items = wtManager.GetAllItems();
                                if (items != null && items.Count > 0)
                                {
                                    FloatingButtonParentEntity entity = items[0];
                                    if (entity != null)
                                    {
                                        if (!entity.Timestamp.Equals(savedFloatingButtonTimeStamp))
                                        {
                                            getSiteCoreFloatingButton = true;
                                        }
                                        else
                                        {
                                            FloatingButtonParentEntity mFBEntityCheck = new FloatingButtonParentEntity();
                                            List<FloatingButtonParentEntity> mCheckList = mFBEntityCheck.GetAllItems();
                                            if (mCheckList == null || mCheckList.Count == 0)
                                            {
                                                getSiteCoreFloatingButton = true;
                                            }
                                            else
                                            {
                                                getSiteCoreFloatingButton = false;

                                            }
                                        }
                                    }
                                    else
                                    {
                                        getSiteCoreFloatingButton = true;
                                    }
                                }
                                else
                                {
                                    getSiteCoreFloatingButton = true;
                                }
                            }
                        }
                        else
                        {
                            getSiteCoreFloatingButton = true;
                        }

                        if (getSiteCoreFloatingButton)
                        {
                            cts = new CancellationTokenSource();
                            Task.Factory.StartNew(() =>
                            {
                                try
                                {
                                    string newDensity = DPUtils.GetDeviceDensity(Application.Context);
                                    GetItemsService getFBItemsService = new GetItemsService(SiteCoreConfig.OS, newDensity, SiteCoreConfig.SITECORE_URL, LanguageUtil.GetAppLanguage());
                                    FloatingButtonResponseModel responseModel = getFBItemsService.GetFloatingButtonItem();
                                    if (responseModel != null && !string.IsNullOrEmpty(responseModel.Status))
                                    {
                                        if (responseModel.Status.Equals("Success"))
                                        {
                                            if (responseMasterModel != null && responseMasterModel.Status != null && responseMasterModel.Status.Equals("Success") && responseMasterModel.Data != null && responseMasterModel.Data.Count > 0)
                                            {
                                                IsOnGetPhotoRunning = false;
                                                FloatingButtonParentEntity mFBParentEntity = new FloatingButtonParentEntity();
                                                mFBParentEntity.DeleteTable();
                                                mFBParentEntity.CreateTable();
                                                mFBParentEntity.InsertListOfItems(responseMasterModel.Data);
                                            }

                                            FloatingButtonEntity wtManager = new FloatingButtonEntity();
                                            wtManager.DeleteTable();
                                            wtManager.CreateTable();
                                            wtManager.InsertListOfItems(responseModel.Data);
                                            OnGetFloatingButtonCache();
                                        }
                                        else
                                        {
                                            OnGetFloatingButtonCache();
                                        }
                                    }
                                    else
                                    {
                                        OnGetFloatingButtonCache();
                                    }
                                }
                                catch (System.Exception e)
                                {
                                    OnGetFloatingButtonCache();
                                    Utility.LoggingNonFatalError(e);
                                }
                            }).ContinueWith((Task previous) =>
                            {
                            }, cts.Token);
                        }
                        else
                        {
                            OnGetFloatingButtonCache();
                            //if (mHomeView != null)
                            //{
                            //   // mHomeView.OnCheckUserWhatsNew(isSitecoreApiFailed);
                            //}
                        }
                    }
                    catch (System.Exception e)
                    {
                        OnGetFloatingButtonCache();
                        Utility.LoggingNonFatalError(e);
                    }
                }).ContinueWith((Task previous) =>
                {
                }, cts.Token);

            }
            catch (ApiException apiException)
            {
                //FloatingButtonParentEntity wtManager2 = new FloatingButtonParentEntity();
                //wtManager2.DeleteTable();
                //wtManager2.CreateTable();
                //isSitecoreApiFailed = true;
                //if (mHomeView != null)
                //{
                //    mHomeView.OnCheckUserWhatsNew(isSitecoreApiFailed);
                //}
                OnGetFloatingButtonCache();
                Utility.LoggingNonFatalError(apiException);
            }
            catch (Newtonsoft.Json.JsonReaderException e)
            {
                //FloatingButtonParentEntity wtManager2 = new FloatingButtonParentEntity();
                //wtManager2.DeleteTable();
                //wtManager2.CreateTable();
                //isSitecoreApiFailed = true;
                //if (mHomeView != null)
                //{
                //    mHomeView.OnCheckUserWhatsNew(isSitecoreApiFailed);
                //}
                OnGetFloatingButtonCache();
                Utility.LoggingNonFatalError(e);
            }
            catch (System.Exception e)
            {
                //FloatingButtonParentEntity wtManager2 = new FloatingButtonParentEntity();
                //wtManager2.DeleteTable();
                //wtManager2.CreateTable();
                //isSitecoreApiFailed = true;
                //if (mHomeView != null)
                //{
                //    mHomeView.OnCheckUserWhatsNew(isSitecoreApiFailed);
                //}
                OnGetFloatingButtonCache();
                Utility.LoggingNonFatalError(e);
            }
            Console.WriteLine("000 SiteCoreFloatingButtonAPI ended");
            return null;
        }

        public Task OnGetFloatingButtonCache()
        {
            CancellationTokenSource token = new CancellationTokenSource();
            return Task.Run(() =>
            {
                try
                {
                    FloatingButtonEntity wtManager = new FloatingButtonEntity();
                    List<FloatingButtonEntity> floatingButtonList = wtManager.GetAllItems();
                    if (floatingButtonList.Count > 0)
                    {
                        FloatingButtonModel item = new FloatingButtonModel()
                        {
                            ID = floatingButtonList[0].ID,
                            Image = floatingButtonList[0].Image,
                            ImageB64 = floatingButtonList[0].ImageB64,
                            Title = floatingButtonList[0].Title,
                            Description = floatingButtonList[0].Description,
                            StartDateTime = floatingButtonList[0].StartDateTime,
                            EndDateTime = floatingButtonList[0].EndDateTime,
                            ShowForSeconds = floatingButtonList[0].ShowForSeconds,
                            ImageBitmap = null
                        };
                        OnProcessFloatingButtonItem(item);
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


        private void OnProcessFloatingButtonItem(FloatingButtonModel item)
        {
            try
            {
                if (!string.IsNullOrEmpty(item.ImageB64))
                {
                    Bitmap convertedImageCache = Base64ToBitmap(item.ImageB64);
                    if (convertedImageCache != null)
                    {
                        FloatingButtonTimeOutMillisecond = 0;
                        item.ImageBitmap = convertedImageCache;
                        FloatingButtonUtils.SetFloatingButtonBitmap(item);
                        if (!mHomeView.GetFloatingButtonSiteCoreDoneFlag())
                        {
                            mHomeView.SetCustomFloatingButtonImage(item);
                        }
                    }
                    else
                    {
                        OnGetPhoto(item);
                    }
                }
                else
                {
                    OnGetPhoto(item);
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
        }

        public void OnGetPhoto(FloatingButtonModel item)
        {
            if (!IsOnGetPhotoRunning)
            {
                IsOnGetPhotoRunning = true;
                CancellationTokenSource token = new CancellationTokenSource();
                Bitmap imageCache = null;
                Stopwatch sw = Stopwatch.StartNew();
                _ = Task.Run(() =>
                {
                    try
                    {
                        //imageCache = ImageUtils.GetImageBitmapFromUrl(item.Image);  
                        imageCache = ImageUtils.GetImageBitmapFromUrlWithTimeOut(item.Image);
                        sw.Stop();
                        FloatingButtonTimeOutMillisecond = 0;

                        if (imageCache != null)
                        {
                            item.ImageBitmap = imageCache;
                            item.ImageB64 = BitmapToBase64(imageCache);
                            FloatingButtonEntity wtManager = new FloatingButtonEntity();
                            wtManager.DeleteTable();
                            wtManager.CreateTable();
                            FloatingButtonEntity newItem = new FloatingButtonEntity()
                            {
                                ID = item.ID,
                                Image = item.Image,
                                ImageB64 = item.ImageB64,
                                Title = item.Title,
                                Description = item.Description,
                                StartDateTime = item.StartDateTime,
                                EndDateTime = item.EndDateTime,
                                ShowForSeconds = item.ShowForSeconds
                            };
                            wtManager.InsertItem(newItem);
                            FloatingButtonUtils.SetFloatingButtonBitmap(item);
                            if (!mHomeView.GetFloatingButtonSiteCoreDoneFlag())
                            {
                                mHomeView.SetCustomFloatingButtonImage(item);
                            }
                        }
                        else
                        {
                            //if (!this.mView.GetAppLaunchSiteCoreDoneFlag())
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

                if (FloatingButtonTimeOutMillisecond > 0)
                {
                    _ = Task.Delay(FloatingButtonTimeOutMillisecond).ContinueWith(_ =>
                    {
                        if (FloatingButtonTimeOutMillisecond > 0)
                        {
                            FloatingButtonTimeOutMillisecond = 0;
                            //if (!this.mView.GetAppLaunchSiteCoreDoneFlag())
                            //{
                            //    this.mView.SetDefaultAppLaunchImage();
                            //}
                        }
                    });
                }
            }
        }

        public string BitmapToBase64(Bitmap bitmap)
        {
            string B64Output = "";
            try
            {
                MemoryStream byteArrayOutputStream = new MemoryStream();
                bitmap.Compress(Bitmap.CompressFormat.Png, 100, byteArrayOutputStream);
                byte[] byteArray = byteArrayOutputStream.ToArray();
                B64Output = Base64.EncodeToString(byteArray, Base64Flags.Default);
            }
            catch (Exception e)
            {
                B64Output = "";
                Utility.LoggingNonFatalError(e);
            }

            return B64Output;
        }

        public Bitmap Base64ToBitmap(string base64String)
        {
            Bitmap convertedBitmap = null;
            try
            {
                byte[] imageAsBytes = Base64.Decode(base64String, Base64Flags.Default);
                convertedBitmap = BitmapFactory.DecodeByteArray(imageAsBytes, 0, imageAsBytes.Length);
            }
            catch (Exception e)
            {
                convertedBitmap = null;
                Utility.LoggingNonFatalError(e);
            }

            return convertedBitmap;
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

