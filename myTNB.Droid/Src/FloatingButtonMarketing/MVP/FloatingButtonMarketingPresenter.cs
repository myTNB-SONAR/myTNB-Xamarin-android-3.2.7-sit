using System;
using myTNB.AndroidApp.Src.Database.Model;
using myTNB.SitecoreCMS.Model;
using Android.Content;
using myTNB.AndroidApp.Src.SitecoreCMS.Model;
using System.Collections.Generic;
using myTNB.AndroidApp.Src.Utils;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Android.Graphics;
using System.Threading;
using Android.Util;
using System.IO;
using System.Net;
using myTNB.AndroidApp.Src.myTNBMenu.Fragments.RewardMenu.Api;
using myTNB.AndroidApp.Src.WhatsNewDetail.MVP;
using myTNB.AndroidApp.Src.SiteCore;
using myTNB.SitecoreCMS.Services;
using System.Diagnostics;
using Android.App;
using myTNB.AndroidApp.Src.myTNBMenu.MVP;

namespace myTNB.AndroidApp.Src.FloatingButtonMarketing.MVP
{
	public class FloatingButtonMarketingPresenter : FloatingButtonMarketingContract.IUserActionsListener
    {
        private static int FloatingButtonDefaultTimeOutMillisecond = 4000;
        private int FloatingButtonTimeOutMillisecond = FloatingButtonDefaultTimeOutMillisecond;

       
        public FloatingButtonMarketingContract.IView mView;
        private ISharedPreferences mSharedPref;

        public FloatingButtonMarketingPresenter(FloatingButtonMarketingContract.IView mView, ISharedPreferences preferences)
        {
            this.mView = mView;
            this.mSharedPref = preferences;
            this.mView?.SetPresenter(this);
        }

        public void GetSavedFBContentTimeStamp()
        {
            try
            {
                FloatingButtonTimeOutMillisecond = FloatingButtonDefaultTimeOutMillisecond;
                FloatingButtonMarketingParentEntity wtManager = new FloatingButtonMarketingParentEntity();
                List<FloatingButtonMarketingParentEntity> items = wtManager.GetAllItems();
                if (items != null && items.Count != 0)
                {
                    foreach (FloatingButtonMarketingParentEntity obj in items)
                    {
                        this.mView.OnSavedFBContentTimeStampRecieved(obj.Timestamp);
                    }
                }
                else
                {
                    this.mView.OnSavedFBContentTimeStampRecieved(null);
                }

            }
            catch (Exception e)
            {
                this.mView.OnSavedFBContentTimeStampRecieved(null);
                Utility.LoggingNonFatalError(e);
            }

        }

        public void OnGetFBContentTimeStamp()
        {
            CancellationTokenSource token = new CancellationTokenSource();
            Stopwatch sw = Stopwatch.StartNew();
            _ = Task.Run(() =>
            {
                try
                {
                    string density = DPUtils.GetDeviceDensity(Application.Context);
                    GetItemsService getItemsService = new GetItemsService(SiteCoreConfig.OS, density, SiteCoreConfig.SITECORE_URL, LanguageUtil.GetAppLanguage());
                    FloatingButtonMarketingTimeStampResponseModel responseModel = getItemsService.GetFloatingButtonMarketingTimestampItem();
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
                        FloatingButtonMarketingParentEntity wtManager = new FloatingButtonMarketingParentEntity();
                        wtManager.DeleteTable();
                        wtManager.CreateTable();
                        wtManager.InsertListOfItems(responseModel.Data);
                        mView.OnFBContentTimeStampRecieved(responseModel.Data[0].Timestamp);
                    }
                    else
                    {
                        mView.OnFBContentTimeStampRecieved(null);
                    }
                }
                catch (Exception e)
                {
                    mView.OnFBContentTimeStampRecieved(null);
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
                        this.mView.SetFBMarketingDetail(item);
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



        public void GetFBMarketingContent(string itemID)
        {
            try
            {
                FloatingButtonMarketingEntity wtManager = new FloatingButtonMarketingEntity();
                FloatingButtonMarketingEntity item = wtManager.GetItem(itemID);

                if (item != null)
                {
                    FloatingButtonMarketingModel fetchItem = new FloatingButtonMarketingModel();
                    fetchItem.ID = item.ID;
                    fetchItem.Title = item.Title;
                    fetchItem.ButtonTitle = item.ButtonTitle;
                    fetchItem.Description = item.Description;
                    fetchItem.Description_Images = item.Description_Images;
                    fetchItem.Infographic_FullView_URL = item.Infographic_FullView_URL;
                    fetchItem.Infographic_FullView_URL_ImageB64 = item.Infographic_FullView_URL_ImageB64;

                    //send dynatrace name
                    //this.mView.dynaAction(item.Title);
                    this.mView.SetupFullScreenShimmer();
                    if (!string.IsNullOrEmpty(fetchItem.Infographic_FullView_URL))
                    {
                        //this.mView.SetupFullScreenShimmer();
                        this.mView.UpdateContentDetail(fetchItem);
                        if (!string.IsNullOrEmpty(fetchItem.Infographic_FullView_URL_ImageB64))
                        {
                            this.mView.OnUpdateFullScreenImage(Base64ToBitmap(fetchItem.Infographic_FullView_URL_ImageB64));
                        }
                        else
                        {
                            _ = OnGetFullScreenItem(fetchItem);
                        }
                    }
                    else
                    {
                        //this.mView.SetToolBarContentTitle(fetchItem);
                        this.mView.SetFBMarketingDetail(fetchItem);
                        //if (fetchItem.Image_DetailsViewBitmap != null)
                        //{
                        //    this.mView.SetWhatsNewImage(fetchItem.Image_DetailsViewBitmap);
                        //}
                        //else if (!string.IsNullOrEmpty(fetchItem.Image_DetailsViewB64))
                        //{
                        //    Bitmap localBitmap = Base64ToBitmap(fetchItem.Image_DetailsViewB64);
                        //    if (localBitmap != null)
                        //    {
                        //        fetchItem.Image_DetailsViewBitmap = localBitmap;
                        //        this.mView.SetWhatsNewImage(fetchItem.Image_DetailsViewBitmap);
                        //    }
                        //    else
                        //    {
                        //        this.mView.SetWhatsNewImage(null);
                        //    }
                        //}
                        //else if (!string.IsNullOrEmpty(fetchItem.Image_DetailsView))
                        //{
                        //    _ = GetImageAsync(fetchItem);
                        //}
                        //else
                        //{
                        //    this.mView.HideWhatsNewDetailImage();
                        //}
                    }
                }
                //else
                //{
                //    this.mView.HideWhatsNewDetailImage();
                //}
            
                //else
                //{
                //    FloatingButtonTimeOutMillisecond = 0;
                //    //if (!this.mView.GetFloatingButtonSiteCoreDoneFlag())
                //    //{
                //    //    this.mView.SetDefaultAppLaunchImage();
                //    //}
                //}
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        private async Task OnGetFullScreenItem(FloatingButtonMarketingModel item)
        {
            try
            {
                CancellationTokenSource cts = new CancellationTokenSource();
                string contentType = "";
                Bitmap imageBitmap = null;
                string fileDirectory = "";
                await Task.Run(() =>
                {
                    WebRequest webRequest = WebRequest.Create(item.Infographic_FullView_URL);
                    using (WebResponse response = webRequest.GetResponse())
                    {
                        if (response != null && !string.IsNullOrEmpty(response.ContentType))
                        {
                            contentType = response.ContentType.ToLower();
                            if (contentType.Contains("pdf"))
                            {
                                using (var stream = response.GetResponseStream())
                                {
                                    if (stream != null)
                                    {
                                        fileDirectory = this.mView.GenerateTmpFilePath();

                                        if (!string.IsNullOrEmpty(fileDirectory))
                                        {
                                            if (File.Exists(fileDirectory))
                                            {
                                                File.Delete(fileDirectory);
                                            }

                                            try
                                            {
                                                using (var memoryStream = new MemoryStream())
                                                {
                                                    stream.CopyTo(memoryStream);
                                                    Java.IO.File file = new Java.IO.File(fileDirectory);

                                                    Java.IO.FileOutputStream outs = new Java.IO.FileOutputStream(file);

                                                    outs.Write(memoryStream.GetBuffer());

                                                    outs.Flush();
                                                    outs.Close();

                                                    this.mView.OnUpdateFullScreenPdf(fileDirectory);
                                                }
                                            }
                                            catch (Exception e)
                                            {
                                                Utility.LoggingNonFatalError(e);
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                using (var stream = response.GetResponseStream())
                                {
                                    if (stream != null)
                                    {
                                        imageBitmap = BitmapFactory.DecodeStream(stream);
                                        item.Infographic_FullView_URL_ImageB64 = BitmapToBase64(imageBitmap);
                                        FloatingButtonMarketingEntity wtManager = new FloatingButtonMarketingEntity();
                                        wtManager.UpdateFullScreenImage(item.ID, item.Infographic_FullView_URL_ImageB64);
                                        this.mView.OnUpdateFullScreenImage(imageBitmap);
                                    }
                                }
                            }
                        }
                    }
                }, cts.Token);
            }
            catch (Exception exe)
            {
                Utility.LoggingNonFatalError(exe);
            }
        }

        public async Task ProcessContentImage(List<FBMarketingDetailImageDBModel> containedImageDB)
        {
            try
            {
                List<FBMarketingDetailImageModel> containedImage = new List<FBMarketingDetailImageModel>();

                bool isApiCallInvolved = false;
                
                for (int i = 0; i < containedImageDB.Count; i++)
                {
                    FBMarketingDetailImageModel containedItem = new FBMarketingDetailImageModel();
                    containedItem.ExtractedImageUrl = containedImageDB[i].ExtractedImageUrl;
                    containedItem.ExtractedImageTag = containedImageDB[i].ExtractedImageTag;

                    if (!string.IsNullOrEmpty(containedImageDB[i].ExtractedImageB64))
                    {
                        Bitmap localBitmap = Base64ToBitmap(containedImageDB[i].ExtractedImageB64);
                        if (localBitmap != null)
                        {
                            containedItem.ExtractedImageBitmap = localBitmap;
                        }
                        else
                        {
                            containedItem = await GetDetailImageAsync(containedItem);
                            if (containedItem.ExtractedImageBitmap != null)
                            {
                                containedImageDB[i].ExtractedImageB64 = BitmapToBase64(containedItem.ExtractedImageBitmap);
                                isApiCallInvolved = true;
                            }
                        }
                    }
                    else
                    {
                        containedItem = await GetDetailImageAsync(containedItem);
                        if (containedItem.ExtractedImageBitmap != null)
                        {
                            containedImageDB[i].ExtractedImageB64 = BitmapToBase64(containedItem.ExtractedImageBitmap);
                            isApiCallInvolved = true;
                        }
                    }

                    containedImage.Add(containedItem);
                }

                if (isApiCallInvolved)
                {
                    try
                    {
                        string dbCahce = JsonConvert.SerializeObject(containedImageDB);
                        FloatingButtonMarketingEntity wtManager = new FloatingButtonMarketingEntity();
                        wtManager.UpdateCacheDescriptionImages(this.mView.GetLocalItemID(), dbCahce);
                    }
                    catch (Exception ex)
                    {
                        Utility.LoggingNonFatalError(ex);
                    }
                }

                this.mView.SetContentDetailImage(containedImage);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        private async Task<FBMarketingDetailImageModel> GetDetailImageAsync(FBMarketingDetailImageModel item)
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            Bitmap imageBitmap = null;
            await Task.Run(() =>
            {
                imageBitmap = GetImageBitmapFromUrl(item.ExtractedImageUrl);
            }, cts.Token);

            if (imageBitmap != null)
            {
                item.ExtractedImageBitmap = imageBitmap;
            }

            return item;
        }

        public async Task FetchContentImage(List<FBMarketingDetailImageModel> containedImage)
        {
            try
            {
                List<FBMarketingDetailImageDBModel> listToDB = new List<FBMarketingDetailImageDBModel>();

                for (int i = 0; i < containedImage.Count; i++)
                {
                    containedImage[i] = await GetDetailImageAsync(containedImage[i]);

                    FBMarketingDetailImageDBModel itemToDB = new FBMarketingDetailImageDBModel();
                    itemToDB.ExtractedImageUrl = containedImage[i].ExtractedImageUrl;
                    itemToDB.ExtractedImageTag = containedImage[i].ExtractedImageTag;
                    itemToDB.ExtractedImageB64 = "";
                    if (containedImage[i].ExtractedImageBitmap != null)
                    {
                        itemToDB.ExtractedImageB64 = BitmapToBase64(containedImage[i].ExtractedImageBitmap);
                    }
                    listToDB.Add(itemToDB);
                }

                if (listToDB.Count > 0)
                {
                    try
                    {
                        string dbCahce = JsonConvert.SerializeObject(listToDB);
                        FloatingButtonMarketingEntity wtManager = new FloatingButtonMarketingEntity();
                        wtManager.UpdateCacheDescriptionImages(this.mView.GetLocalItemID(), dbCahce);
                    }
                    catch (Exception ex)
                    {
                        Utility.LoggingNonFatalError(ex);
                    }
                }

                this.mView.SetContentDetailImage(containedImage);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        private Bitmap GetImageBitmapFromUrl(string url)
        {
            Bitmap image = null;
            try
            {
                using (WebClient webClient = new WebClient())
                {
                    var imageBytes = webClient.DownloadData(url);
                    if (imageBytes != null && imageBytes.Length > 0)
                    {
                        image = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);
                    }
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
            return image;
        }

        private string BitmapToBase64(Bitmap bitmap)
        {
            string B64Output = "";
            try
            {
                MemoryStream byteArrayOutputStream = new MemoryStream();
                bitmap.Compress(Bitmap.CompressFormat.Png, 100, byteArrayOutputStream);
                byte[] byteArray = byteArrayOutputStream.ToArray();
                B64Output = Android.Util.Base64.EncodeToString(byteArray, Base64Flags.Default);
            }
            catch (Exception e)
            {
                B64Output = "";
                Utility.LoggingNonFatalError(e);
            }

            return B64Output;
        }

        private Bitmap Base64ToBitmap(string base64String)
        {
            Bitmap convertedBitmap = null;
            try
            {
                byte[] imageAsBytes = Android.Util.Base64.Decode(base64String, Base64Flags.Default);
                convertedBitmap = BitmapFactory.DecodeByteArray(imageAsBytes, 0, imageAsBytes.Length);
            }
            catch (Exception e)
            {
                convertedBitmap = null;
                Utility.LoggingNonFatalError(e);
            }

            return convertedBitmap;
        }

        public List<FBMarketingDetailImageModel> ExtractImage(string text)
        {
            List<FBMarketingDetailImageModel> containedImage = new List<FBMarketingDetailImageModel>();
            string urlRegex = @"<img[^>]*?src\s*=\s*[""']?([^'"" >]+?)[ '""][^>]*?>";
            System.Text.RegularExpressions.MatchCollection matchesImgSrc = System.Text.RegularExpressions.Regex.Matches(text, urlRegex, System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.Singleline);
            foreach (System.Text.RegularExpressions.Match m in matchesImgSrc)
            {
                string url = m.Groups[1].Value;
                string imgTag = m.Groups[0].Value;
                FBMarketingDetailImageModel item = new FBMarketingDetailImageModel()
                {
                    ExtractedImageTag = imgTag,
                    ExtractedImageUrl = url
                };
                containedImage.Add(item);
            }
            return containedImage;
        }

        public void Start()
        {
            throw new NotImplementedException();
        }
    }
}

