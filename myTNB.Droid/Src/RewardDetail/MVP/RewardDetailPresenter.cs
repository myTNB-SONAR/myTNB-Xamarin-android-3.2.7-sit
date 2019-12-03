using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Android.Content;
using Android.Graphics;
using Android.Util;
using myTNB.SitecoreCMS.Model;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.Utils;

namespace myTNB_Android.Src.RewardDetail.MVP
{
    public class RewardDetailPresenter : RewardDetailContract.IRewardDetailPresenter
    {
        RewardDetailContract.IRewardDetailView mView;

        private ISharedPreferences mPref;

        private RewardsEntity mRewardsEntity;

        public RewardDetailPresenter(RewardDetailContract.IRewardDetailView view, ISharedPreferences pref)
        {
            this.mView = view;
            this.mPref = pref;
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
                    fetchItem.IsUsed = item.IsUsed;
                    fetchItem.TitleOnListing = item.TitleOnListing;
                    fetchItem.PeriodLabel = item.PeriodLabel;
                    fetchItem.LocationLabel = item.LocationLabel;
                    fetchItem.TandCLabel = item.TandCLabel;
                    fetchItem.StartDate = item.StartDate;
                    fetchItem.EndDate = item.EndDate;
                    fetchItem.IsSaved = item.IsSaved;

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
                    this.mView.SetRewardDetail(new RewardsModel());
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        private Bitmap ToGrayscale(Bitmap srcImage)
        {

            Bitmap bmpGrayscale = Bitmap.CreateBitmap(srcImage.Width, srcImage.Height, Bitmap.Config.Argb8888);

            Canvas canvas = new Canvas(bmpGrayscale);
            Paint paint = new Paint();

            ColorMatrix cm = new ColorMatrix();
            cm.SetSaturation(0);
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

        public void UpdateRewardSave(string itemID , bool flag)
        {
            RewardsEntity wtManager = new RewardsEntity();
            wtManager.UpdateIsSavedItem(itemID, flag);
            // Update api calling
        }
    }
}
