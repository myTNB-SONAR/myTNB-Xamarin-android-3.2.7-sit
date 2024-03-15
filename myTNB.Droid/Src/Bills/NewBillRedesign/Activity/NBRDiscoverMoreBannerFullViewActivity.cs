using System;
using System.Net;
using System.Runtime;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using CheeseBind;
using myTNB.AndroidApp.Src.Base.Activity;
using myTNB.AndroidApp.Src.Utils;
using myTNB.AndroidApp.Src.Utils.ZoomImageView;

namespace myTNB.AndroidApp.Src.Bills.NewBillRedesign.Activity
{
    [Activity(Label = "", ScreenOrientation = ScreenOrientation.Portrait, Theme = "@style/Theme.Dashboard")]
    public class NBRDiscoverMoreBannerFullViewActivity : BaseToolbarAppCompatActivity
    {
        public Bitmap fullBitmap;

        [BindView(Resource.Id.imgNBRBanner)]
        ZoomImageView imgNBRBanner;

        [BindView(Resource.Id.bannerShimmerContainer)]
        LinearLayout bannerShimmerContainer;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            try
            {
                RunOnUiThread(() =>
                {
                    try
                    {
                        Bundle extras = Intent.Extras;
                        if (extras != null)
                        {
                            if (extras.ContainsKey("IMAGE_PATH"))
                            {
                                var imagePathString = extras.GetString("IMAGE_PATH");
                                _ = GetImageAsync(imagePathString);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Utility.LoggingNonFatalError(ex);
                    }
                });
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public async Task GetImageAsync(string imageName)
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            try
            {
                await Task.Run(() =>
                {
                    fullBitmap = GetImageBitmapFromUrl(imageName);
                }, cts.Token);

                bannerShimmerContainer.Visibility = ViewStates.Gone;
                imgNBRBanner.Visibility = ViewStates.Visible;
                if (fullBitmap != null)
                {
                    imgNBRBanner
                        .FromBitmap(fullBitmap)
                        .Show();
                }
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

        public override int ResourceId()
        {
            return Resource.Layout.NBRDiscoverMoreBannerFullView;
        }

        public override bool ShowCustomToolbarTitle()
        {
            return true;
        }

        protected override void OnResume()
        {
            base.OnResume();
        }

        public override void OnTrimMemory(TrimMemory level)
        {
            base.OnTrimMemory(level);

            switch (level)
            {
                case TrimMemory.RunningLow:
                    GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
                    GC.Collect();
                    break;
                default:
                    GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
                    GC.Collect();
                    break;
            }
        }
    }
}
