using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Android.Content;
using Android.Graphics;
using Android.Icu.Text;
using Android.Util;
using Java.Util;
using Java.Util.Regex;
using myTNB.SitecoreCMS.Model;
using myTNB.AndroidApp.Src.Base.Models;
using myTNB.AndroidApp.Src.Database.Model;
using myTNB.AndroidApp.Src.myTNBMenu.Fragments.RewardMenu.Api;
using myTNB.AndroidApp.Src.myTNBMenu.Fragments.RewardMenu.Model;
using myTNB.AndroidApp.Src.myTNBMenu.Fragments.RewardMenu.Request;
using myTNB.AndroidApp.Src.myTNBMenu.Fragments.RewardMenu.Response;
using myTNB.AndroidApp.Src.NewAppTutorial.MVP;
using myTNB.AndroidApp.Src.Utils;
using Refit;

namespace myTNB.AndroidApp.Src.RewardDetail.MVP
{
    public class RewardDetailPresenter : RewardDetailContract.IRewardDetailPresenter
    {
        RewardDetailContract.IRewardDetailView mView;

        private ISharedPreferences mPref;

        private RewardsEntity mRewardsEntity;

        private RewardServiceImpl mApi;

        public RewardDetailPresenter(RewardDetailContract.IRewardDetailView view, ISharedPreferences pref)
        {
            this.mView = view;
            this.mPref = pref;
            this.mApi = new RewardServiceImpl();
        }

        public void FetchRewardImage(RewardsModel item)
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

        public void GetActiveReward(string itemID)
        {
            try
            {
                RewardsEntity wtManager = new RewardsEntity();
                RewardsEntity item = wtManager.GetItem(itemID);
                if (item != null)
                {
                    RewardsModel fetchItem = new RewardsModel();
                    fetchItem.ID = item.ID;
                    fetchItem.Title = item.Title;
                    fetchItem.Image = item.Image;
                    fetchItem.ImageB64 = item.ImageB64;
                    fetchItem.CategoryID = item.CategoryID;
                    fetchItem.Description = item.Description;
                    fetchItem.Read = item.Read;
                    fetchItem.ReadDateTime = item.ReadDateTime;
                    fetchItem.IsUsed = item.IsUsed;
                    fetchItem.IsUsedDateTime = item.IsUsedDateTime;
                    fetchItem.TitleOnListing = item.TitleOnListing;
                    fetchItem.PeriodLabel = item.PeriodLabel;
                    fetchItem.LocationLabel = item.LocationLabel;
                    fetchItem.TandCLabel = item.TandCLabel;
                    fetchItem.StartDate = item.StartDate;
                    fetchItem.EndDate = item.EndDate;
                    fetchItem.IsSaved = item.IsSaved;
                    fetchItem.IsSavedDateTime = item.IsSavedDateTime;
                    fetchItem.RewardUseWithinTime = item.RewardUseWithinTime;
                    fetchItem.RewardUseTitle = item.RewardUseTitle;
                    fetchItem.RewardUseDescription = item.RewardUseDescription;

                    this.mView.SetRewardDetail(fetchItem);
                    if (fetchItem.ImageBitmap != null)
                    {
                        if (fetchItem.IsUsed)
                        {
                            fetchItem.ImageBitmap = ToGrayscale(fetchItem.ImageBitmap);
                        }
                        this.mView.SetRewardImage(fetchItem.ImageBitmap);
                    }
                    else if (!string.IsNullOrEmpty(fetchItem.ImageB64))
                    {
                        Bitmap localBitmap = Base64ToBitmap(fetchItem.ImageB64);
                        if (localBitmap != null)
                        {
                            if (fetchItem.IsUsed)
                            {
                                localBitmap = ToGrayscale(localBitmap);
                            }
                            fetchItem.ImageBitmap = localBitmap;
                            this.mView.SetRewardImage(fetchItem.ImageBitmap);
                        }
                        else
                        {
                            this.mView.SetRewardImage(null);
                        }
                    }
                    else if (!string.IsNullOrEmpty(fetchItem.Image))
                    {
                        _ = GetImageAsync(fetchItem);
                    }
                    else
                    {
                        this.mView.SetRewardImage(null);
                    }
                }
                else
                {
                    this.mView.SetRewardDetail(null);
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public Bitmap ToGrayscale(Bitmap srcImage)
        {
            Bitmap bmpGrayscale = Bitmap.CreateBitmap(srcImage.Width, srcImage.Height, Bitmap.Config.Argb8888);

            Canvas canvas = new Canvas(bmpGrayscale);
            Paint paint = new Paint();

            float[] colorMatrixElements = { 0.33f, 0.33f, 0.33f, 0, 0, 0.33f, 0.33f, 0.33f, 0, 0, 0.33f, 0.33f, 0.33f, 0, 0, 0, 0, 0, 1, 0 };
            ColorMatrix cm = new ColorMatrix(colorMatrixElements);
            paint.SetColorFilter(new ColorMatrixColorFilter(cm));
            canvas.DrawBitmap(srcImage, 0, 0, paint);

            return bmpGrayscale;
        }

        private async Task GetImageAsync(RewardsModel item)
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            Bitmap imageBitmap = null;
            await Task.Run(() =>
            {
                imageBitmap = GetImageBitmapFromUrl(item.Image);
            }, cts.Token);

            if (imageBitmap != null)
            {
                item.ImageBitmap = imageBitmap;
                item.ImageB64 = BitmapToBase64(imageBitmap);
                RewardsEntity wtManager = new RewardsEntity();
                wtManager.UpdateCacheImage(item.ID, item.ImageB64);
                if (item.IsUsed)
                {
                    item.ImageBitmap = ToGrayscale(item.ImageBitmap);
                }
                this.mView.SetRewardImage(item.ImageBitmap);
            }
            else
            {
                // Set Default Image
                this.mView.SetRewardImage(null);
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

        public string BitmapToBase64(Bitmap bitmap)
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

        public Bitmap Base64ToBitmap(string base64String)
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

        public void UpdateRewardSave(string itemID , bool flag)
        {
            DateTime currentDate = DateTime.UtcNow;
            RewardsEntity wtManager = new RewardsEntity();
            CultureInfo currCult = CultureInfo.CreateSpecificCulture("en-US");
            string formattedDate = currentDate.ToString(@"M/d/yyyy h:m:s tt", currCult);
            if (!flag)
            {
                formattedDate = "";

            }
            wtManager.UpdateIsSavedItem(itemID, flag, formattedDate);

            _ = OnUpdateReward(itemID);

        }

        public async Task UpdateRewardUsed(string itemID)
        {
            try
            {
                this.mView.ShowProgressDialog();

                DateTime currentDate = DateTime.UtcNow;
                RewardsEntity wtManager = new RewardsEntity();
                CultureInfo currCult = CultureInfo.CreateSpecificCulture("en-US");
                string formattedDate = currentDate.ToString(@"M/d/yyyy h:m:s tt", currCult);

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
                    Redeemed = true,
                    RedeemedDate = !string.IsNullOrEmpty(formattedDate) ? formattedDate + " +00:00" : ""
                };

                AddUpdateRewardRequest request = new AddUpdateRewardRequest()
                {
                    usrInf = currentUsrInf,
                    reward = currentReward
                };

                AddUpdateRewardResponse response = await this.mApi.AddUpdateReward(request, new System.Threading.CancellationTokenSource().Token);

                this.mView.HideProgressDialog();

                if (response != null && response.Data != null && response.Data.ErrorCode == "7200")
                {
                    wtManager.UpdateIsUsedItem(itemID, true, formattedDate);
                    this.mView.OnCountDownReward();
                }
                else
                {
                    // Show Error Message
                    this.mView.ShowRetryPopup();
                }
            }
            catch (System.OperationCanceledException e)
            {
                this.mView.HideProgressDialog();
                // Show Error Message
                this.mView.ShowRetryPopup();
                Utility.LoggingNonFatalError(e);
            }
            catch (ApiException apiException)
            {
                this.mView.HideProgressDialog();
                // Show Error Message
                this.mView.ShowRetryPopup();
                Utility.LoggingNonFatalError(apiException);
            }
            catch (Exception e)
            {
                this.mView.HideProgressDialog();
                // Show Error Message
                this.mView.ShowRetryPopup();
                Utility.LoggingNonFatalError(e);
            }
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

        public List<NewAppModel> OnGeneraNewAppTutorialList()
        {
            List<NewAppModel> newList = new List<NewAppModel>();

            newList.Add(new NewAppModel()
            {
                ContentShowPosition = ContentType.TopLeft,
                ContentTitle = Utility.GetLocalizedLabel("RewardDetails", "tutorialSaveTitle"), // "Never miss out on a reward.",
                ContentMessage = Utility.GetLocalizedLabel("RewardDetails", "tutorialSaveDesc"), // "Save a reward to your favourite<br/>so that you can use it later.",
                ItemCount = 0,
                DisplayMode = "",
                IsButtonShow = false
            });

            newList.Add(new NewAppModel()
            {
                ContentShowPosition = ContentType.TopRight,
                ContentTitle = Utility.GetLocalizedLabel("RewardDetails", "tutorialUseNowTitle"), // "Redeem your rewards.",
                ContentMessage = Utility.GetLocalizedLabel("RewardDetails", "tutorialUseNowDesc"), // "At the merchant’s place and ready<br/>to redeem your reward? Tap here.",
                ItemCount = 0,
                DisplayMode = "",
                IsButtonShow = false
            });

            return newList;
        }
    }
}
