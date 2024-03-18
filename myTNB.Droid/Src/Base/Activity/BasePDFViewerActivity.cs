using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using CheeseBind;
using myTNB.AndroidApp.Src.Utils;
using myTNB.AndroidApp.Src.Utils.PDFView;
using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace myTNB.AndroidApp.Src.Base.Activity
{
    [Activity(Label = "Base PDF Viewer"
          , ScreenOrientation = ScreenOrientation.Portrait
          , Theme = "@style/Theme.AddAccount")]
    public class BasePDFViewerActivity : BaseToolbarAppCompatActivity
    {

        [BindView(Resource.Id.rootView)]
        public static LinearLayout baseView;

        private bool isLoadedDocument = false;

        private string HeaderTitle = "";

        CancellationTokenSource cts;

        PDFView pdfViewer;

        private string pdfLink = "";

        public override int ResourceId()
        {
            return Resource.Layout.BasePDFViewerLayout;
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

            if (extra != null)
            {
                if (extra.ContainsKey(Constants.IN_APP_LINK))
                {
                    pdfLink = extra.GetString(Constants.IN_APP_LINK);
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

                baseView = FindViewById<LinearLayout>(Resource.Id.rootView);
                pdfViewer = FindViewById<PDFView>(Resource.Id.pdf_viewer_control_view);

                try
                {
                    RunOnUiThread(() =>
                    {
                        GetPDF();
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
            SetTheme(TextViewUtils.IsLargeFonts
                ? Resource.Style.Theme_AddAccountLarge
                : Resource.Style.Theme_AddAccount);
        }

        public async Task GetPDF()
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

                string downloadPath = "";

                await Task.Run(() =>
                {
                    downloadPath = OnDownloadPDF();
                }, cts.Token);

                if (!string.IsNullOrEmpty(downloadPath))
                {
                    try
                    {
                        Java.IO.File file = new Java.IO.File(downloadPath);

                        pdfViewer
                            .FromFile(file)
                            .Show();
                        isLoadedDocument = true;
                    }
                    catch (Exception e)
                    {
                        Log.Debug("BasePDFViewerActivity", e.Message);
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

        public string OnDownloadPDF()
        {
            string path = "";

            try
            {
                if (!string.IsNullOrEmpty(pdfLink))
                {
                    string rootPath = this.FilesDir.AbsolutePath;

                    if (Utils.FileUtils.IsExternalStorageReadable() && Utils.FileUtils.IsExternalStorageWritable())
                    {
                        rootPath = this.GetExternalFilesDir(null).AbsolutePath;
                    }

                    var directory = System.IO.Path.Combine(rootPath, "pdf");
                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }

                    string filename = "temp.pdf";

                    path = System.IO.Path.Combine(directory, filename);

                    if (!string.IsNullOrEmpty(path))
                    {
                        if (File.Exists(path))
                        {
                            File.Delete(path);
                        }

                        try
                        {
                            using (WebClient client = new WebClient())
                            {
                                client.DownloadFile(pdfLink, path);
                            }
                        }
                        catch (Exception e)
                        {
                            path = "";
                            Log.Debug("ViewBillActivity", e.StackTrace);
                            Utility.LoggingNonFatalError(e);
                        }

                    }
                }
            }
            catch (Exception e)
            {
                path = "";
                Utility.LoggingNonFatalError(e);
            }
            return path;
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
                        // GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
                        GC.Collect();
                        break;
                    default:
                        // GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
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
                FirebaseAnalyticsUtils.SetScreenName(this, "Base PDF Viewer");
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