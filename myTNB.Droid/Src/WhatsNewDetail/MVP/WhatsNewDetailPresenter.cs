using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Android.Content;
using Android.Graphics;
using Android.Util;
using Java.Util.Regex;
using myTNB.SitecoreCMS.Model;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.Utils;

namespace myTNB_Android.Src.WhatsNewDetail.MVP
{
    public class WhatsNewDetailPresenter : WhatsNewDetailContract.IWhatsNewDetailPresenter
    {
        WhatsNewDetailContract.IWhatsNewDetaillView mView;

        private ISharedPreferences mPref;

        public WhatsNewDetailPresenter(WhatsNewDetailContract.IWhatsNewDetaillView view, ISharedPreferences pref)
        {
            this.mView = view;
            this.mPref = pref;
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
                    fetchItem.Image = item.Image;
                    fetchItem.ImageB64 = item.ImageB64;
                    fetchItem.CategoryID = item.CategoryID;
                    fetchItem.Description = item.Description;
                    fetchItem.Read = item.Read;
                    fetchItem.ReadDateTime = item.ReadDateTime;
                    fetchItem.TitleOnListing = item.TitleOnListing;
                    fetchItem.StartDate = item.StartDate;
                    fetchItem.EndDate = item.EndDate;
                    fetchItem.CTA = item.CTA;

                    this.mView.SetWhatsNewDetail(fetchItem);
                    if (fetchItem.ImageBitmap != null)
                    {
                        this.mView.SetWhatsNewImage(fetchItem.ImageBitmap);
                    }
                    else if (!string.IsNullOrEmpty(fetchItem.ImageB64))
                    {
                        Bitmap localBitmap = Base64ToBitmap(fetchItem.ImageB64);
                        if (localBitmap != null)
                        {
                            fetchItem.ImageBitmap = localBitmap;
                            this.mView.SetWhatsNewImage(fetchItem.ImageBitmap);
                        }
                        else
                        {
                            this.mView.SetWhatsNewImage(null);
                        }
                    }
                    else if (!string.IsNullOrEmpty(fetchItem.Image))
                    {
                        _ = GetImageAsync(fetchItem);
                    }
                    else
                    {
                        this.mView.SetWhatsNewImage(null);
                    }
                }
                else
                {
                    this.mView.SetWhatsNewImage(null);
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
                imageBitmap = GetImageBitmapFromUrl(item.Image);
            }, cts.Token);

            if (imageBitmap != null)
            {
                item.ImageBitmap = imageBitmap;
                item.ImageB64 = BitmapToBase64(imageBitmap);
                WhatsNewEntity wtManager = new WhatsNewEntity();
                wtManager.UpdateCacheImage(item.ID, item.ImageB64);
                this.mView.SetWhatsNewImage(item.ImageBitmap);
            }
            else
            {
                // Set Default Image
                this.mView.SetWhatsNewImage(null);
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
    }
}
