using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using CheeseBind;
using myTNB_Android.Src.Utils;
using Syncfusion.SfPdfViewer.Android;
using System;
using System.IO;
using System.Net;
using System.Runtime;
using System.Threading;
using System.Threading.Tasks;

namespace myTNB_Android.Src.Base.Activity
{
    [Activity(Label = "Base PDF Viewer"
          , ScreenOrientation = ScreenOrientation.Portrait
          , Theme = "@style/Theme.AddAccount")]
    public class BasePDFViewerActivity : BaseToolbarAppCompatActivity
    {

        [BindView(Resource.Id.rootView)]
        public static FrameLayout baseView;

        private bool isLoadedDocument = false;

        private string HeaderTitle = "";

        CancellationTokenSource cts;

        SfPdfViewer pdfViewer;

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

                baseView = FindViewById<FrameLayout>(Resource.Id.rootView);
                pdfViewer = FindViewById<SfPdfViewer>(Resource.Id.pdf_viewer_control_view);

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

                MemoryStream PdfStream = new MemoryStream();

                await Task.Run(() =>
                {
                    PdfStream = OnDownloadPDF();
                }, cts.Token);

                if (PdfStream != null)
                {
                    try
                    {
                        using (StreamReader sr = new StreamReader(PdfStream))
                        {
                            pdfViewer.LoadDocument(sr.BaseStream);
                            isLoadedDocument = true;
                        }

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

        public MemoryStream OnDownloadPDF()
        {
            MemoryStream stream = new MemoryStream();

            try
            {
                if (!string.IsNullOrEmpty(pdfLink))
                {
                    using (WebClient client = new WebClient())
                    {
                        var pdfByte = client.DownloadData(pdfLink);
                        stream.Write(pdfByte, 0, pdfByte.Length);
                    }
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
            return stream;
        }

        protected override void OnDestroy()
        {
            if (cts != null && cts.Token.CanBeCanceled)
            {
                cts.Cancel();
            }
            if (isLoadedDocument)
            {
                pdfViewer.Unload();
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