using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Content.Res;
using Android.Graphics;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using CheeseBind;
using Com.Davemorrissey.Labs.Subscaleview;
using myTNB_Android.Src.Utils;
using myTNB_Android.Src.Utils.ZoomImageView;
using System;
using System.IO;
using System.Net;
using System.Runtime;
using System.Threading;
using System.Threading.Tasks;

namespace myTNB_Android.Src.Base.Activity
{
    [Activity(Label = "Base Image Viewer"
          , ScreenOrientation = ScreenOrientation.Portrait
          , Theme = "@style/Theme.AddAccount")]
    public class BaseFullScreenImageViewActivity : BaseToolbarAppCompatActivity
    {

        [BindView(Resource.Id.imgFullView)]
        ZoomImageView imgFullView;

        private bool isLoadedDocument = false;

        private string HeaderTitle = "";

        CancellationTokenSource cts;

        private string imageLink = "";

        public override int ResourceId()
        {
            return Resource.Layout.BaseFullScreenImageViewLayout;
        }

        public override bool ShowCustomToolbarTitle()
        {
            return true;
        }

        public bool IsActive()
        {
            return Window.DecorView.RootView.IsShown;
        }

        protected override void OnStart()
        {
            base.OnStart();

            try
            {
                SetToolbarBackground(Resource.Drawable.CustomGradientToolBar);
                SetStatusBarBackground(Resource.Drawable.UsageGradientBackground);
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Bundle extra = Intent.Extras;

            imgFullView = FindViewById<ZoomImageView>(Resource.Id.imgFullView);

            if (extra != null)
            {
                if (extra.ContainsKey(Constants.IN_APP_LINK))
                {
                    imageLink = extra.GetString(Constants.IN_APP_LINK);
                }
                else
                {
                    this.Finish();
                    return;
                }

                if (extra.ContainsKey(Constants.IN_APP_TITLE))
                {
                    HeaderTitle = extra.GetString(Constants.IN_APP_TITLE);
                    SetToolBarTitle(extra.GetString(Constants.IN_APP_TITLE));
                }
            }
            else
            {
                this.Finish();
                return;
            }

            try
            {
                cts = new CancellationTokenSource();

                try
                {
                    RunOnUiThread(() =>
                    {
                        GetImage();
                    });
                }
                catch (Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

            Android.Content.Res.Configuration configuration = Resources.Configuration;
            configuration.FontScale = (float)1; //0.85 small size, 1 normal size, 1,15 big etc
            var metrics = this.ApplicationContext.Resources.DisplayMetrics;
            metrics.ScaledDensity = configuration.FontScale * metrics.Density;
            this.Resources.UpdateConfiguration(configuration, metrics);

            SetTheme(TextViewUtils.IsLargeFonts ? Resource.Style.Theme_AddAccountLarge : Resource.Style.Theme_AddAccount);
        }
        protected override void AttachBaseContext(Context baseContext)
        {

            Context newContext;

            if (Build.VERSION.SdkInt >= BuildVersionCodes.N)
            {
                DisplayMetrics displayMetrics = baseContext.Resources.DisplayMetrics;
                Configuration configuration = baseContext.Resources.Configuration;
                configuration.DensityDpi = DisplayMetrics.DensityDeviceStable;
                newContext = baseContext.CreateConfigurationContext(configuration);
            }
            else
            {
                // Old API. Screen zoom not supported
                newContext = baseContext;
            }
            base.AttachBaseContext(newContext);
        }
        public async Task GetImage()
        {
            try
            {
                try
                {
                    LoadingOverlayUtils.OnRunLoadingAnimation(this);
                }
                catch (Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                }

                Bitmap cacheImg = null;

                await Task.Run(() =>
                {
                    cacheImg = OnDownloadImage();
                }, cts.Token);

                if (cacheImg != null)
                {
                    try
                    {
                        imgFullView.Visibility = ViewStates.Visible;

                        imgFullView
                            .FromBitmap(cacheImg)
                            .Show();

                        isLoadedDocument = true;

                    }
                    catch (Exception e)
                    {
                        Log.Debug("BaseFullScreenImageViewActivity", e.Message);
                    }
                }

                try
                {
                    LoadingOverlayUtils.OnStopLoadingAnimation(this);
                }
                catch (Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }


        }

        public Bitmap OnDownloadImage()
        {
            Bitmap cache = null;

            try
            {
                if (!string.IsNullOrEmpty(imageLink))
                {
                    cache = ImageUtils.GetImageBitmapFromUrl(imageLink);
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
            return cache;
        }

        protected override void OnDestroy()
        {
            if (cts != null && cts.Token.CanBeCanceled)
            {
                cts.Cancel();
            }
            if (isLoadedDocument)
            {
                isLoadedDocument = false;
            }

            base.OnDestroy();
        }

        public override void OnTrimMemory(TrimMemory level)
        {
            base.OnTrimMemory(level);
            try
            {
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
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        protected override void OnResume()
        {
            try
            {
                base.OnResume();
                FirebaseAnalyticsUtils.SetScreenName(this, "Base Image Viewer");
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void ShowProgressDialog()
        {
            try
            {
                RunOnUiThread(() =>
                {
                    try
                    {
                        LoadingOverlayUtils.OnRunLoadingAnimation(this);
                    }
                    catch (Exception e)
                    {
                        Utility.LoggingNonFatalError(e);
                    }
                });
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public override void OnBackPressed()
        {
            Finish();
        }

        public void HideProgressDialog()
        {
            try
            {
                RunOnUiThread(() =>
                {
                    try
                    {
                        LoadingOverlayUtils.OnStopLoadingAnimation(this);
                    }
                    catch (Exception e)
                    {
                        Utility.LoggingNonFatalError(e);
                    }
                });
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }
    }
}