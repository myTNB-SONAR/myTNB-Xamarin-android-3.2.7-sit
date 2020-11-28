using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Android.Content;
using Android.Graphics;
using Android.Util;
using Java.Util.Regex;
using myTNB.SitecoreCMS.Model;
using myTNB_Android.Src.Base.Models;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.myTNBMenu.Fragments.RewardMenu.Api;
using myTNB_Android.Src.myTNBMenu.Fragments.RewardMenu.Model;
using myTNB_Android.Src.myTNBMenu.Fragments.RewardMenu.Request;
using myTNB_Android.Src.myTNBMenu.Fragments.RewardMenu.Response;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;

namespace myTNB_Android.Src.WhatsNewDetail.MVP
{
    public class WhatsNewDetailPresenter : WhatsNewDetailContract.IWhatsNewDetailPresenter
    {
        WhatsNewDetailContract.IWhatsNewDetaillView mView;

        private RewardServiceImpl mApi;

        private ISharedPreferences mPref;

        public WhatsNewDetailPresenter(WhatsNewDetailContract.IWhatsNewDetaillView view, ISharedPreferences pref)
        {
            this.mView = view;
            this.mPref = pref;
            this.mApi = new RewardServiceImpl();
        }

        public void FetchWhatsNewImage(WhatsNewModel item)
        {
            try
            {
                _ = GetImageAsync(item);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void GetActiveWhatsNew(string itemID)
        {
            try
            {
                WhatsNewEntity wtManager = new WhatsNewEntity();
                WhatsNewEntity item = wtManager.GetItem(itemID);
                if (item != null)
                {
                    WhatsNewModel fetchItem = new WhatsNewModel();
                    fetchItem.ID = item.ID;
                    fetchItem.Title = item.Title;
                    fetchItem.CategoryID = item.CategoryID;
                    fetchItem.Description = item.Description;
                    fetchItem.Read = item.Read;
                    fetchItem.ReadDateTime = item.ReadDateTime;
                    fetchItem.TitleOnListing = item.TitleOnListing;
                    fetchItem.StartDate = item.StartDate;
                    fetchItem.EndDate = item.EndDate;
                    fetchItem.PublishDate = item.PublishDate;
                    fetchItem.CTA = item.CTA;
                    fetchItem.Image_DetailsView = item.Image_DetailsView;
                    fetchItem.Image_DetailsViewB64 = item.Image_DetailsViewB64;
                    fetchItem.Styles_DetailsView = item.Styles_DetailsView;
                    fetchItem.Description_Images = item.Description_Images;
                    fetchItem.Infographic_FullView_URL = item.Infographic_FullView_URL;
                    fetchItem.Infographic_FullView_URL_ImageB64 = item.Infographic_FullView_URL_ImageB64;

                    //send dynatrace name
                    this.mView.dynaAction(item.Title);

                    if (!string.IsNullOrEmpty(fetchItem.Infographic_FullView_URL))
                    {
                        this.mView.SetupFullScreenShimmer();
                        this.mView.UpdateWhatsNewDetail(fetchItem);
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
                        this.mView.SetWhatsNewDetail(fetchItem);
                        if (fetchItem.Image_DetailsViewBitmap != null)
                        {
                            this.mView.SetWhatsNewImage(fetchItem.Image_DetailsViewBitmap);
                        }
                        else if (!string.IsNullOrEmpty(fetchItem.Image_DetailsViewB64))
                        {
                            Bitmap localBitmap = Base64ToBitmap(fetchItem.Image_DetailsViewB64);
                            if (localBitmap != null)
                            {
                                fetchItem.Image_DetailsViewBitmap = localBitmap;
                                this.mView.SetWhatsNewImage(fetchItem.Image_DetailsViewBitmap);
                            }
                            else
                            {
                                this.mView.SetWhatsNewImage(null);
                            }
                        }
                        else if (!string.IsNullOrEmpty(fetchItem.Image_DetailsView))
                        {
                            _ = GetImageAsync(fetchItem);
                        }
                        else
                        {
                            this.mView.HideWhatsNewDetailImage();
                        }
                    }
                }
                else
                {
                    this.mView.HideWhatsNewDetailImage();
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        private async Task GetImageAsync(WhatsNewModel item)
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            Bitmap imageBitmap = null;
            await Task.Run(() =>
            {
                imageBitmap = GetImageBitmapFromUrl(item.Image_DetailsView);
            }, cts.Token);

            if (imageBitmap != null)
            {
                item.Image_DetailsViewBitmap = imageBitmap;
                item.Image_DetailsViewB64 = BitmapToBase64(imageBitmap);
                WhatsNewEntity wtManager = new WhatsNewEntity();
                wtManager.UpdateCacheDetailImage(item.ID, item.Image_DetailsViewB64);
                this.mView.SetWhatsNewImage(item.Image_DetailsViewBitmap);
            }
            else
            {
                // Set Default Image
                this.mView.SetWhatsNewImage(null);
            }
        }

        private async Task OnGetFullScreenItem(WhatsNewModel item)
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
                                        WhatsNewEntity wtManager = new WhatsNewEntity();
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

        public List<WhatsNewDetailImageModel> ExtractImage(string text)
        {
            List<WhatsNewDetailImageModel> containedImage = new List<WhatsNewDetailImageModel>();
            string urlRegex = @"<img[^>]*?src\s*=\s*[""']?([^'"" >]+?)[ '""][^>]*?>";
            System.Text.RegularExpressions.MatchCollection matchesImgSrc = System.Text.RegularExpressions.Regex.Matches(text, urlRegex, System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.Singleline);
            foreach (System.Text.RegularExpressions.Match m in matchesImgSrc)
            {
                string url = m.Groups[1].Value;
                string imgTag = m.Groups[0].Value;
                WhatsNewDetailImageModel item = new WhatsNewDetailImageModel()
                {
                    ExtractedImageTag = imgTag,
                    ExtractedImageUrl = url
                };
                containedImage.Add(item);
            }
            return containedImage;
        }

        public async Task FetchWhatsNewDetailImage(List<WhatsNewDetailImageModel> containedImage)
        {
            try
            {
                List<WhatsNewDetailImageDBModel> listToDB = new List<WhatsNewDetailImageDBModel>();

                for (int i = 0; i < containedImage.Count; i++)
                {
                    containedImage[i] = await GetDetailImageAsync(containedImage[i]);

                    WhatsNewDetailImageDBModel itemToDB = new WhatsNewDetailImageDBModel();
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
                        WhatsNewEntity wtManager = new WhatsNewEntity();
                        wtManager.UpdateCacheDescriptionImages(this.mView.GetLocalItemID(), dbCahce);
                    }
                    catch (Exception ex)
                    {
                        Utility.LoggingNonFatalError(ex);
                    }
                }

                this.mView.SetWhatsNewDetailImage(containedImage);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public async Task ProcessWhatsNewDetailImage(List<WhatsNewDetailImageDBModel> containedImageDB)
        {
            try
            {
                List<WhatsNewDetailImageModel> containedImage = new List<WhatsNewDetailImageModel>();

                bool isApiCallInvolved = false;

                for (int i = 0; i < containedImageDB.Count; i++)
                {
                    WhatsNewDetailImageModel containedItem = new WhatsNewDetailImageModel();
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
                        WhatsNewEntity wtManager = new WhatsNewEntity();
                        wtManager.UpdateCacheDescriptionImages(this.mView.GetLocalItemID(), dbCahce);
                    }
                    catch (Exception ex)
                    {
                        Utility.LoggingNonFatalError(ex);
                    }
                }

                this.mView.SetWhatsNewDetailImage(containedImage);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        private async Task<WhatsNewDetailImageModel> GetDetailImageAsync(WhatsNewDetailImageModel item)
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

        public List<string> ExtractUrls(string text)
        {
            List<string> containedUrls = new List<string>();
            string urlRegex = "\\(?\\b(https://|http://|www[.])[-A-Za-z0-9+&amp;@#/%?=~_()|!:,.;]*[-A-Za-z0-9+&amp;@#/%=~_()|]";
            Pattern pattern = Pattern.Compile(urlRegex);
            Matcher urlMatcher = pattern.Matcher(text);

            try
            {
                while (urlMatcher.Find())
                {
                    string urlStr = urlMatcher.Group();
                    if (urlStr.StartsWith("(") && urlStr.EndsWith(")"))
                    {
                        urlStr = urlStr.Substring(1, urlStr.Length - 1);
                    }

                    if (!containedUrls.Contains(urlStr))
                    {
                        containedUrls.Add(urlStr);
                    }
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

            return containedUrls;
        }

        public void UpdateWhatsNewRead(string itemID, bool flag)
        {
            DateTime currentDate = DateTime.UtcNow;
            WhatsNewEntity wtManager = new WhatsNewEntity();
            CultureInfo currCult = CultureInfo.CreateSpecificCulture("en-US");
            string formattedDate = currentDate.ToString(@"M/d/yyyy h:m:s tt", currCult);
            if (!flag)
            {
                formattedDate = "";

            }
            wtManager.UpdateReadItem(itemID, flag, formattedDate);
        }

        public void UpdateRewardRead(string itemID, bool flag)
        {
            DateTime currentDate = DateTime.UtcNow;
            RewardsEntity wtManager = new RewardsEntity();
            CultureInfo currCult = CultureInfo.CreateSpecificCulture("en-US");
            string formattedDate = currentDate.ToString(@"M/d/yyyy h:m:s tt", currCult);
            if (!flag)
            {
                formattedDate = "";

            }
            wtManager.UpdateReadItem(itemID, flag, formattedDate);

            _ = OnUpdateReward(itemID);
        }

        private async Task OnUpdateReward(string itemID)
        {
            try
            {
                // Update api calling
                RewardsEntity wtManager = new RewardsEntity();
                RewardsEntity currentItem = wtManager.GetItem(itemID);

                UserInterface currentUsrInf = new UserInterface()
                {
                    eid = UserEntity.GetActive().Email,
                    sspuid = UserEntity.GetActive().UserID,
                    did = UserEntity.GetActive().DeviceId,
                    ft = FirebaseTokenEntity.GetLatest().FBToken,
                    lang = LanguageUtil.GetAppLanguage().ToUpper(),
                    sec_auth_k1 = Constants.APP_CONFIG.API_KEY_ID,
                    sec_auth_k2 = "",
                    ses_param1 = "",
                    ses_param2 = ""
                };

                string rewardId = currentItem.ID;
                rewardId = rewardId.Replace("{", "");
                rewardId = rewardId.Replace("}", "");

                AddUpdateRewardModel currentReward = new AddUpdateRewardModel()
                {
                    Email = UserEntity.GetActive().Email,
                    RewardId = rewardId,
                    Read = currentItem.Read,
                    ReadDate = !string.IsNullOrEmpty(currentItem.ReadDateTime) ? currentItem.ReadDateTime + " +00:00" : "",
                    Favourite = currentItem.IsSaved,
                    FavUpdatedDate = !string.IsNullOrEmpty(currentItem.IsSavedDateTime) ? currentItem.IsSavedDateTime + " +00:00" : "",
                    Redeemed = currentItem.IsUsed,
                    RedeemedDate = !string.IsNullOrEmpty(currentItem.IsUsedDateTime) ? currentItem.IsUsedDateTime + " +00:00" : ""
                };

                AddUpdateRewardRequest request = new AddUpdateRewardRequest()
                {
                    usrInf = currentUsrInf,
                    reward = currentReward
                };

                AddUpdateRewardResponse response = await this.mApi.AddUpdateReward(request, new System.Threading.CancellationTokenSource().Token);

            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }
    }
}
