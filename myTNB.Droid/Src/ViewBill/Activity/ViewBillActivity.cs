using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.Content;
using Android.Util;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using CheeseBind;
using Java.Net;
using Java.Text;
using Java.Util;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.Utils;
using myTNB_Android.Src.Utils.Custom.ProgressDialog;
using Syncfusion.SfPdfViewer.Android;
using System;
using System.IO;
using System.Net;
using System.Runtime;
using System.Threading;
using System.Threading.Tasks;

namespace myTNB_Android.Src.ViewBill.Activity
{
    [Activity(Label = "@string/viewbill_activity_title"
          , ScreenOrientation = ScreenOrientation.Portrait
          , Theme = "@style/Theme.AddAccount")]
    public class ViewBillActivity : BaseToolbarAppCompatActivity
    {

        AccountData selectedAccount;
        BillHistoryV5 selectedBill;

        BillHistoryResponseV5 billsHistoryResponseV5;

        //[BindView(Resource.Id.webView)]
        //private static WebView webView;

        [BindView(Resource.Id.progressBar)]
        public ProgressBar mProgressBar;

        [BindView(Resource.Id.rootView)]
        public static FrameLayout baseView;

        private static Snackbar mErrorNoInternet;

        Snackbar mErrorMessageSnackBar;
        private IMenuItem downloadOption;
        private string pdfURL = "http://drive.google.com/viewerng/viewer?embedded=true&url=";
        private string getPDFUrl = "";
        private string filePath = null;
        private bool downloadClicked = false;
        private bool isLoadedDocument = false;


        CancellationTokenSource cts;

        //17/07/2017
        SimpleDateFormat simpleDateParser = new SimpleDateFormat("dd/MM/yyyy");
        SimpleDateFormat simpleDateFormat = new SimpleDateFormat("MMM yyyy");

        //[BindView(Resource.Id.pdfviewercontrol)]
        SfPdfViewer pdfViewer;

        private LoadingOverlay loadingOverlay;

        public override int ResourceId()
        {
            return Resource.Layout.ViewBillPDFView;
        }

        public override bool ShowCustomToolbarTitle()
        {
            return true;
        }

        public override string ToolbarTitle()
        {

            Date d = null;
            string title = " Bill";
            if (selectedAccount != null)
            {
                if (selectedAccount.AccountCategoryId.Equals("2"))
                {
                    title = " Advice";
                }
            }
            try
            {
                if (selectedBill != null && !string.IsNullOrEmpty(selectedBill.DtBill))
                {
                    d = simpleDateParser.Parse(selectedBill.DtBill);
                }
                else
                {
                    if (!string.IsNullOrEmpty(selectedAccount?.DateBill))
                    {
                        d = simpleDateParser.Parse(selectedAccount?.DateBill);
                    }
                }

            }
            catch (Java.Text.ParseException e)
            {
                Utility.LoggingNonFatalError(e);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

            if (d != null)
            {
                title = simpleDateFormat.Format(d) + title;
            }

            return title;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {


            Bundle extras = Intent.Extras;

            if (extras != null)
            {
                if (extras.ContainsKey(Constants.SELECTED_ACCOUNT))
                {
                    //selectedAccount = JsonConvert.DeserializeObject<AccountData>(Intent.Extras.GetString(Constants.SELECTED_ACCOUNT));
                    selectedAccount = DeSerialze<AccountData>(extras.GetString(Constants.SELECTED_ACCOUNT));
                }

                if (extras.ContainsKey(Constants.SELECTED_BILL))
                {
                    //selectedBill = JsonConvert.DeserializeObject<BillHistoryV5>(Intent.Extras.GetString(Constants.SELECTED_BILL));
                    selectedBill = DeSerialze<BillHistoryV5>(extras.GetString(Constants.SELECTED_BILL));
                }

            }




            base.OnCreate(savedInstanceState);
            try
            {
                //webView = FindViewById<WebView>(Resource.Id.webView);
                baseView = FindViewById<FrameLayout>(Resource.Id.rootView);
                mProgressBar = FindViewById<ProgressBar>(Resource.Id.progressBar);
                mProgressBar.Visibility = ViewStates.Gone;
                cts = new CancellationTokenSource();

                //webView.Settings.JavaScriptEnabled = (true);
                ////webView.SetWebChromeClient(new WebChromeClient());
                //webView.SetWebViewClient(new MyTNBWebViewClient(this, mProgressBar, downloadOption));
                pdfViewer = FindViewById<SfPdfViewer>(Resource.Id.pdf_viewer_control_view);
                //InputMethodManager inputMethodManager = (InputMethodManager)baseView.Context.GetSystemService(Context.InputMethodService);
                //inputMethodManager.HideSoftInputFromWindow(baseView.WindowToken, HideSoftInputFlags.None);

#if STUB
            if (selectedBill != null && !string.IsNullOrEmpty(selectedBill.NrBill))
            {
                getPDFUrl = Constants.SERVER_URL.END_POINT + "/v5/my_billingssp.asmx/GetBillPDFByBillNo?apiKeyID=" + Constants.APP_CONFIG.API_KEY_ID + "&accNum=" + selectedAccount.AccountNum + "&billingNo=" + selectedBill.NrBill;
                pdfURL += URLEncoder.Encode(Constants.SERVER_URL.END_POINT + "/v5/my_billingssp.asmx/GetBillPDFByBillNo?apiKeyID=" + Constants.APP_CONFIG.API_KEY_ID + "&accNum=" + selectedAccount.AccountNum + "&billingNo=" + selectedBill.NrBill, "utf-8");
                //webView.LoadUrl("http://drive.google.com/viewerng/viewer?embedded=true&url=" + URLEncoder.Encode(Constants.SERVER_URL.END_POINT + "/GetBillPDF?apiKeyID="+Constants.APP_CONFIG.API_KEY_ID+"&accNum=" + selectedAccount.AccountNum+"&billingNo="+selectedBill.NrBill, "utf-8"));
            }
            else
            {
                getPDFUrl = Constants.SERVER_URL.END_POINT + "/v5/my_billingssp.asmx/GetBillPDF?apiKeyID=" + Constants.APP_CONFIG.API_KEY_ID + "&accNum=" + selectedAccount.AccountNum;
                pdfURL += URLEncoder.Encode(Constants.SERVER_URL.END_POINT + "/v5/my_billingssp.asmx/GetBillPDF?apiKeyID=" + Constants.APP_CONFIG.API_KEY_ID + "&accNum=" + selectedAccount.AccountNum, "utf-8");
                //webView.LoadUrl("http://drive.google.com/viewerng/viewer?embedded=true&url=" + URLEncoder.Encode(Constants.SERVER_URL.END_POINT + "/GetBillPDF?apiKeyID="+Constants.APP_CONFIG.API_KEY_ID+"&accNum=" + selectedAccount.AccountNum , "utf-8"));
            }
#else
            if (selectedBill != null && !string.IsNullOrEmpty(selectedBill.NrBill))
            {
                getPDFUrl = Constants.SERVER_URL.END_POINT + "/v5/my_billingssp.asmx/GetBillPDFByBillNo?apiKeyID=" + Constants.APP_CONFIG.API_KEY_ID + "&accNum=" + selectedAccount.AccountNum + "&billingNo=" + selectedBill.NrBill;
                pdfURL += URLEncoder.Encode(Constants.SERVER_URL.END_POINT + "/v5/my_billingssp.asmx/GetBillPDFByBillNo?apiKeyID=" + Constants.APP_CONFIG.API_KEY_ID + "&accNum=" + selectedAccount.AccountNum + "&billingNo=" + selectedBill.NrBill, "utf-8");
                //webView.LoadUrl("http://drive.google.com/viewerng/viewer?embedded=true&url=" + URLEncoder.Encode(Constants.SERVER_URL.END_POINT + "/GetBillPDF?apiKeyID="+Constants.APP_CONFIG.API_KEY_ID+"&accNum=" + selectedAccount.AccountNum+"&billingNo="+selectedBill.NrBill, "utf-8"));
            }
            else
            {
                getPDFUrl = Constants.SERVER_URL.END_POINT + "/v5/my_billingssp.asmx/GetBillPDF?apiKeyID=" + Constants.APP_CONFIG.API_KEY_ID + "&accNum=" + selectedAccount.AccountNum;
                pdfURL += URLEncoder.Encode(Constants.SERVER_URL.END_POINT + "/v5/my_billingssp.asmx/GetBillPDF?apiKeyID=" + Constants.APP_CONFIG.API_KEY_ID + "&accNum=" + selectedAccount.AccountNum, "utf-8");
                //webView.LoadUrl("http://drive.google.com/viewerng/viewer?embedded=true&url=" + URLEncoder.Encode(Constants.SERVER_URL.END_POINT + "/GetBillPDF?apiKeyID="+Constants.APP_CONFIG.API_KEY_ID+"&accNum=" + selectedAccount.AccountNum , "utf-8"));
            }
#endif
                //selectedAccount = JsonConvert.DeserializeObject<AccountData>(Intent.Extras.GetString(Constants.SELECTED_ACCOUNT));
                //webView.LoadUrl("http://drive.google.com/viewerng/viewer?embedded=true&url=https://mobiletestingws.tnb.com.my/v4/my_billingssp.asmx/GetBillPDF?apiKeyID=9515F2FA-C267-42C9-8087-FABA77CB84DF&accNum=" + selectedAccount.AccountNum);

                //webView.LoadUrl(pdfURL);
                downloadClicked = true;
                RunOnUiThread(() =>
                {
                    if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.ReadExternalStorage) == (int)Permission.Granted && ContextCompat.CheckSelfPermission(this, Manifest.Permission.WriteExternalStorage) == (int)Permission.Granted)
                    {
                        GetPDF();
                    }
                    else
                    {
                        RequestPermissions(new string[] { Manifest.Permission.WriteExternalStorage }, Constants.RUNTIME_PERMISSION_STORAGE_REQUEST_CODE);
                    }
                });
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

        }







        public class MyTNBWebViewClient : WebViewClient
        {

            public Android.App.Activity mActivity;
            public ProgressBar progressBar;
            private bool isRedirected = false;
            private IMenuItem download;

            public MyTNBWebViewClient(Android.App.Activity mActivity, ProgressBar progress, IMenuItem menuItem)
            {
                this.mActivity = mActivity;
                this.progressBar = progress;
                this.download = menuItem;
            }

            public override bool ShouldOverrideUrlLoading(WebView view, string url)
            {
                try
                {
                    if (ConnectionUtils.HasInternetConnection(mActivity))
                    {
                        view.LoadUrl(url);
                    }
                    else
                    {
                        ShowErrorMessageNoInternet(url);
                    }
                }
                catch (Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                }
                return true;
            }

            public override void OnPageStarted(WebView view, string url, Android.Graphics.Bitmap favicon)
            {
                try
                {
                    if (ConnectionUtils.HasInternetConnection(mActivity))
                    {
                        base.OnPageStarted(view, url, favicon);
                        progressBar.Visibility = ViewStates.Visible;
                        if (this.download != null)
                        {
                            this.download.SetVisible(false);
                        }
                    }
                    else
                    {
                        ShowErrorMessageNoInternet(url);
                    }
                }
                catch (Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                }
            }

            public override void OnPageFinished(WebView view, string url)
            {
                try
                {
                    progressBar.Visibility = ViewStates.Gone;
                    if (this.download != null)
                    {
                        this.download.SetVisible(true);
                    }
                }
                catch (Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                }
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.ViewBillReceiptMenu, menu);
            //downloadOption = menu.GetItem(Resource.Id.action_download);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            try
            {
                switch (item.ItemId)
                {
                    case Resource.Id.action_download:
                        if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.ReadExternalStorage) == (int)Permission.Granted && ContextCompat.CheckSelfPermission(this, Manifest.Permission.WriteExternalStorage) == (int)Permission.Granted)
                        {
                            downloadClicked = true;
                            if (!string.IsNullOrEmpty(filePath))
                            {
                                OpenPDF(filePath);
                            }

                        }
                        else
                        {
                            RequestPermissions(new string[] { Manifest.Permission.WriteExternalStorage }, Constants.RUNTIME_PERMISSION_STORAGE_REQUEST_CODE);
                        }
                        return true;
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
            return base.OnOptionsItemSelected(item);
        }

        public async Task GetPDF()
        {
            try
            {
                //mProgressBar.Visibility = ViewStates.Visible;
                if (loadingOverlay != null && loadingOverlay.IsShowing)
                {
                    loadingOverlay.Dismiss();
                }

                loadingOverlay = new LoadingOverlay(this, Resource.Style.LoadingOverlyDialogStyle);
                loadingOverlay.Show();

                await Task.Run(() =>
                {
                    filePath = OnDownloadPDF();
                }, cts.Token);

                //mProgressBar.Visibility = ViewStates.Gone;
                if (!string.IsNullOrEmpty(filePath))
                {
                    try
                    {
                        Java.IO.File file = new Java.IO.File(filePath);
                        //Patch done by Jeeva on 26-12-2018.As per the App Code Scanning Report...
                        using (Stream PdfStream = File.Open(file.AbsolutePath, FileMode.Open))
                        {
                            pdfViewer.LoadDocument(PdfStream);
                            isLoadedDocument = true;
                        }
                        //Stream PdfStream = File.Open(file.AbsolutePath, FileMode.Open);//Assets.Open(path);
                        //pdfViewer.LoadDocument(PdfStream);
                        //Patch done by Jeeva on 26-12-2018.As per the App Code Scanning Report...
                    }
                    catch (Exception e)
                    {
                        Log.Debug("ViewBillActivity", e.Message);
                    }
                }

                if (loadingOverlay != null && loadingOverlay.IsShowing)
                {
                    loadingOverlay.Dismiss();
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }


        }

        public string OnDownloadPDF()
        {
            string path = null;
            //if (downloadClicked)
            //{

            try
            {
                if (!String.IsNullOrEmpty(getPDFUrl) && !String.IsNullOrEmpty(selectedAccount?.AccountNum))
                {
                    using (WebClient client = new WebClient())
                    {
                        var directory = new Java.IO.File(Android.OS.Environment.ExternalStorageDirectory, "pdf").ToString();
                        if (!Directory.Exists(directory))
                        {
                            Directory.CreateDirectory(directory);
                        }

                        string filename = selectedAccount?.AccountNum + ".pdf";
                        if (!string.IsNullOrEmpty(selectedBill?.NrBill))
                        {
                            filename = selectedAccount?.AccountNum + "_" + selectedBill?.NrBill + ".pdf";
                        }
                        path = System.IO.Path.Combine(directory, filename);

                        if (!string.IsNullOrEmpty(path))
                        {
                            if (File.Exists(path))
                            {
                                File.Delete(path);
                            }
                            client.DownloadFile(getPDFUrl, path);
                        }

                    }
                }
            }
            catch (Exception e)
            {
                Log.Debug("ViewBillActivity", e.StackTrace);
                downloadClicked = false;
                mProgressBar.Visibility = ViewStates.Gone;
                Utility.LoggingNonFatalError(e);
            }
            // }
            return path;
        }

        public void OpenPDF(string path)
        {
            try
            {
                Java.IO.File file = new Java.IO.File(path);
                Android.Net.Uri fileUri = FileProvider.GetUriForFile(this,
                                            ApplicationContext.PackageName + ".provider", file);

                Intent intent = new Intent(Intent.ActionView);
                intent.SetDataAndType(fileUri, "application/pdf");
                intent.AddFlags(ActivityFlags.GrantReadUriPermission);
                StartActivity(intent);
            }
            catch (Exception e)
            {
                Log.Debug("ViewBillActivity", e.StackTrace);
                Utility.LoggingNonFatalError(e);
            }

        }

        //public override bool StoragePermissionRequired()
        //{
        //    return true;
        //}

        public static void ShowErrorMessageNoInternet(string failingUrl)
        {
            try
            {
                if (mErrorNoInternet != null && mErrorNoInternet.IsShown)
                {
                    mErrorNoInternet.Dismiss();
                }

                mErrorNoInternet = Snackbar.Make(baseView, "Please check your internet connection.", Snackbar.LengthIndefinite)
                .SetAction("Try Again", delegate
                {
                //webView.LoadUrl(failingUrl);
                mErrorNoInternet.Dismiss();
                });
                View v = mErrorNoInternet.View;
                TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
                tv.SetMaxLines(5);

                mErrorNoInternet.Show();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
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

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            try
            {
                if (requestCode == Constants.RUNTIME_PERMISSION_STORAGE_REQUEST_CODE)
                {
                    if (Utility.IsPermissionHasCount(grantResults))
                    {
                        if (grantResults[0] == Permission.Granted)
                        {
                            RunOnUiThread(() =>
                            {
                                GetPDF();
                            });

                        }
                    }
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
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
    }
}