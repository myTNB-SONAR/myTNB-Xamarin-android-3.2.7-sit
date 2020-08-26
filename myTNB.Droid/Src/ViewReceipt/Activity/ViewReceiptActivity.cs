using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;


using Android.Util;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using CheeseBind;
using iTextSharp.text;
using iTextSharp.text.pdf;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.Utils;
using myTNB_Android.Src.ViewReceipt.Model;
using myTNB_Android.Src.ViewReceipt.MVP;
using System;
using System.IO;

namespace myTNB_Android.Src.ViewReceipt.Activity
{
    [Activity(Label = "View Receipt"
        , ScreenOrientation = ScreenOrientation.Portrait
        , Theme = "@style/Theme.Dashboard")]
    public class ViewReceiptActivity : BaseToolbarAppCompatActivity, ViewReceiptContract.IView
    {
        private string TAG = "View Receipt Activity";

        private string receipt_html = "";
        private string receipt_pdf = "";
        private readonly string HTML_START = "<html><body>";
        private readonly string HTML_END = "</body></html>";
        private string RECEPT_NO = "101010010";
        private ViewReceiptPresenter mPresenter;
        private ViewReceiptContract.IUserActionsListener userActionsListener;

        private AlertDialog mGetReceiptDialog;
        private Snackbar mErrorMessageSnackBar;
        private bool downloadClicked = false;

        private static Snackbar mErrorNoInternet;

        AccountData selectedAccount;
        BillHistory selectedBill;

        [BindView(Resource.Id.rootView)]
        private static FrameLayout mainView;

        [BindView(Resource.Id.webView)]
        private static WebView webView;

        [BindView(Resource.Id.rootView)]
        private FrameLayout baseView;

        [BindView(Resource.Id.progressBar)]
        private ProgressBar mProgressBar;

        public void HideGetReceiptDialog()
        {
            if (this.mGetReceiptDialog != null && this.mGetReceiptDialog.IsShowing)
            {
                this.mGetReceiptDialog.Dismiss();
            }
        }

        public bool IsActive()
        {
            return Window.DecorView.RootView.IsShown && !IsFinishing;
        }

        public void OnShowReceiptDetails(GetReceiptResponse response)
        {
            try
            {
                if (response != null)
                {
                    if (response.receipt.Status.Equals("success"))
                    {
                        Log.Debug(TAG, "Receipt :" + response.receipt.receiptDetails);
                        RECEPT_NO = response.receipt.receiptDetails.referenceNum;
                        receipt_html = HTML_START + "<p><font color='grey'>" + this.GetString(Resource.String.receipt_dear_customer) + "</font></p></b><br/>"
                            + "<p><font color='grey'>" + this.GetString(Resource.String.receipt_payment_succesful) + "</font></p></b><br/>"
                            + "<p><font color='grey'>" + this.GetString(Resource.String.receipt_reference_number) + response.receipt.receiptDetails.referenceNum + "<br></br>"
                            + this.GetString(Resource.String.receipt_date) + response.receipt.receiptDetails.payTransDate + "<br></br>"
                            + this.GetString(Resource.String.receipt_amount) + response.receipt.receiptDetails.payAmt + "<br></br>"
                            + this.GetString(Resource.String.receipt_from_account) + response.receipt.receiptDetails.customerName + "<br></br>"
                            + this.GetString(Resource.String.receipt_account_number) + response.receipt.receiptDetails.accountNum + "<br></br>"
                            + this.GetString(Resource.String.receipt_transaction_method) + response.receipt.receiptDetails.payMethod + "<br></br>"
                            + this.GetString(Resource.String.receipt_transcation_id) + response.receipt.receiptDetails.payTransID + "<br></br>"
                            + "</font></p>"
                            + HTML_END;

                        receipt_pdf = this.GetString(Resource.String.receipt_dear_customer) + "\n\n"
                            + this.GetString(Resource.String.receipt_payment_succesful) + "\n\n"
                            + this.GetString(Resource.String.receipt_reference_number) + response.receipt.receiptDetails.referenceNum + "\n"
                            + this.GetString(Resource.String.receipt_date) + response.receipt.receiptDetails.payTransDate + "\n"
                            + this.GetString(Resource.String.receipt_amount) + response.receipt.receiptDetails.payAmt + "\n"
                            + this.GetString(Resource.String.receipt_from_account) + response.receipt.receiptDetails.customerName + "\n"
                            + this.GetString(Resource.String.receipt_account_number) + response.receipt.receiptDetails.accountNum + "\n"
                            + this.GetString(Resource.String.receipt_transaction_method) + response.receipt.receiptDetails.payMethod + "\n"
                            + this.GetString(Resource.String.receipt_transcation_id) + response.receipt.receiptDetails.payTransID + "\n";
                        webView.LoadData(receipt_html, "text/html", "UTF-8");
                    }
                    else
                    {
                        ShowErrorMessage(response.receipt.Message);
                    }
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public override int ResourceId()
        {
            return Resource.Layout.ViewBillView;
        }

        public void SetPresenter(ViewReceiptContract.IUserActionsListener userActionListener)
        {
            this.userActionsListener = userActionListener;
        }

        public override bool ShowCustomToolbarTitle()
        {
            return true;
        }

        public void ShowErrorMessage(string msg)
        {
            if (mErrorMessageSnackBar != null && mErrorMessageSnackBar.IsShown)
            {
                mErrorMessageSnackBar.Dismiss();
            }

            mErrorMessageSnackBar = Snackbar.Make(baseView, msg, Snackbar.LengthIndefinite)
            .SetAction(Utility.GetLocalizedCommonLabel("close"), delegate { mErrorMessageSnackBar.Dismiss(); }
            );
            View v = mErrorMessageSnackBar.View;
            TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
            tv.SetMaxLines(5);

            mErrorMessageSnackBar.Show();
        }

        public void ShowGetReceiptDialog()
        {
            if (this.mGetReceiptDialog != null && !this.mGetReceiptDialog.IsShowing)
            {
                this.mGetReceiptDialog.Show();
            }
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            try
            {
                mPresenter = new ViewReceiptPresenter(this);
                webView = FindViewById<WebView>(Resource.Id.webView);
                baseView = FindViewById<FrameLayout>(Resource.Id.rootView);
                mProgressBar = FindViewById<ProgressBar>(Resource.Id.progressBar);

                mGetReceiptDialog = new AlertDialog.Builder(this)
                  .SetTitle("Get Receipt")
                  .SetMessage("Please wait while we are getting receipt details...")
                  .SetCancelable(false)
                  .Create();

                // Create your application here
                webView.Settings.JavaScriptEnabled = (true);
                webView.SetWebChromeClient(new WebChromeClient());
                webView.SetWebViewClient(new MyTNBWebViewClient(this, mProgressBar));

                string apiKeyID = Constants.APP_CONFIG.API_KEY_ID;
                string merchantTransId = Intent.Extras.GetString("merchantTransId");
                this.userActionsListener.GetReceiptDetails(apiKeyID, merchantTransId);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.ViewBillReceiptMenu, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.action_download:
                    downloadClicked = true;
                    OnDownloadPDF();
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }

        public void OnDownloadPDF()
        {

            if (downloadClicked)
            {
                mProgressBar.Visibility = ViewStates.Visible;
                RunOnUiThread(() =>
                {
                    try
                    {
                        string rootPath = this.FilesDir.AbsolutePath;

                        if (FileUtils.IsExternalStorageReadable() && FileUtils.IsExternalStorageWritable())
                        {
                            rootPath = this.GetExternalFilesDir(null).AbsolutePath;
                        }

                        var directory = System.IO.Path.Combine(rootPath, "pdf");
                        if (!Directory.Exists(directory))
                        {
                            Directory.CreateDirectory(directory);
                        }

                        var path = System.IO.Path.Combine(directory, RECEPT_NO + ".pdf");

                        if (File.Exists(path))
                        {
                            File.Delete(path);
                        }
                        var fs = new FileStream(path, FileMode.Create);


                        iTextSharp.text.Document document = new iTextSharp.text.Document(PageSize.A4, 25, 25, 30, 30);

                        PdfWriter writer = PdfWriter.GetInstance(document, fs);
                        document.Open();
                        BaseFont bf = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                        iTextSharp.text.Font font = new iTextSharp.text.Font(bf, 32, iTextSharp.text.Font.NORMAL);
                        document.Add(new Paragraph(new Chunk(receipt_pdf, font)));
                        document.Close();
                        writer.Close();
                        fs.Close();

                        if (mErrorMessageSnackBar != null && mErrorMessageSnackBar.IsShown)
                        {
                            mErrorMessageSnackBar.Dismiss();
                        }
                        
                        string downloadLinkLocation = string.Format(Utility.GetLocalizedCommonLabel("pdfDownloadMessage"), path);

                        mErrorMessageSnackBar = Snackbar.Make(baseView, downloadLinkLocation, Snackbar.LengthIndefinite)
                        .SetAction(Utility.GetLocalizedCommonLabel("open"), delegate
                        {
                            Java.IO.File file = new Java.IO.File(path);
                            Android.Net.Uri fileUri = FileProvider.GetUriForFile(this,
                                            ApplicationContext.PackageName + ".fileprovider", file);

                            Intent intent = new Intent(Intent.ActionView);
                            intent.SetDataAndType(fileUri, "application/pdf");
                            intent.AddFlags(ActivityFlags.GrantReadUriPermission);
                            StartActivity(intent);
                            mErrorMessageSnackBar.Dismiss();
                        }
                        );

                        View v = mErrorMessageSnackBar.View;
                        TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
                        tv.SetMaxLines(5);
                        Button btn = (Button)v.FindViewById<Button>(Resource.Id.snackbar_action);
                        btn.SetTextColor(Android.Graphics.Color.Yellow);
                        mErrorMessageSnackBar.Show();
                        downloadClicked = false;
                        mProgressBar.Visibility = ViewStates.Gone;

                    }
                    catch (Exception e)
                    {
                        Log.Debug("ViewReceiptActivity", e.StackTrace);
                        downloadClicked = false;
                        mProgressBar.Visibility = ViewStates.Gone;
                        Utility.LoggingNonFatalError(e);
                    }
                });
            }

        }

        public class MyTNBWebViewClient : WebViewClient
        {

            public Android.App.Activity mActivity;
            public ProgressBar progressBar;
            private bool isRedirected = false;

            public MyTNBWebViewClient(Android.App.Activity mActivity, ProgressBar progress)
            {
                this.mActivity = mActivity;
                this.progressBar = progress;
            }

            public override bool ShouldOverrideUrlLoading(WebView view, string url)
            {
                if (ConnectionUtils.HasInternetConnection(mActivity))
                {
                    view.LoadUrl(url);
                }
                else
                {
                    ShowErrorMessageNoInternet(url);
                }
                return true;
            }

            public override void OnPageStarted(WebView view, string url, Android.Graphics.Bitmap favicon)
            {
                if (ConnectionUtils.HasInternetConnection(mActivity))
                {
                    base.OnPageStarted(view, url, favicon);
                    progressBar.Visibility = ViewStates.Visible;
                }
                else
                {
                    ShowErrorMessageNoInternet(url);
                }
            }

            public override void OnPageFinished(WebView view, string url)
            {
                progressBar.Visibility = ViewStates.Gone;

            }

            public override void OnReceivedError(WebView view, ClientError errorCode, string description, string failingUrl)
            {
                String message = "Please check your internet connection.";
                if (ConnectionUtils.HasInternetConnection(mActivity))
                {
                    switch (errorCode)
                    {
                        case ClientError.FileNotFound:
                            message = "File Not Found."; break;
                        case ClientError.Authentication:
                            message = "Authetication Error."; break;
                        case ClientError.FailedSslHandshake:
                            message = "SSL Handshake Failed."; break;
                        case ClientError.Unknown:
                            message = "Unkown Error."; break;
                    }
                    ShowErrorMessageNoInternet(failingUrl);
                }
                else
                {
                    ShowErrorMessageNoInternet(failingUrl);
                }

                //Toast.makeText(PaymentWebViewActivity.this,message,Toast.LENGTH_LONG).show();
                webView.LoadUrl("");
            }
        }

        public static void ShowErrorMessageNoInternet(string failingUrl)
        {
            if (mErrorNoInternet != null && mErrorNoInternet.IsShown)
            {
                mErrorNoInternet.Dismiss();
            }

            mErrorNoInternet = Snackbar.Make(mainView, Utility.GetLocalizedErrorLabel("noDataConnectionMessage"), Snackbar.LengthIndefinite)
            .SetAction(Utility.GetLocalizedLabel("Common", "tryAgain"), delegate
            {
                webView.LoadUrl(failingUrl);
                mErrorNoInternet.Dismiss();
            });
            View v = mErrorNoInternet.View;
            TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
            tv.SetMaxLines(5);

            mErrorNoInternet.Show();
        }

        protected override void OnResume()
        {
            try
            {
                base.OnResume();
                FirebaseAnalyticsUtils.SetScreenName(this, "View Payment Receipt");
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }
    }
}
