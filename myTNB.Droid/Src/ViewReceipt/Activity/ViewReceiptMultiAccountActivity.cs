using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;


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
using System.Collections.Generic;
using System.IO;

namespace myTNB_Android.Src.ViewReceipt.Activity
{
    [Activity(Label = "View Receipt"
        , ScreenOrientation = ScreenOrientation.Portrait
        , Theme = "@style/Theme.Dashboard")]
    public class ViewReceiptMultiAccountActivity : BaseToolbarAppCompatActivity, ViewReceiptMultiAccountContract.IView
    {
        private string TAG = "View Receipt Activity";

        private string receipt_html = "";
        private string receipt_pdf = "";
        private readonly string HTML_START = "<html><body>";
        private readonly string HTML_END = "</body></html>";
        private string RECEPT_NO = "101010010";
        private ViewReceiptMultiAccountPresenter mPresenter;
        private ViewReceiptMultiAccountContract.IUserActionsListener userActionsListener;

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
        ProgressBar mProgressBar;


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



        public void OnShowReceiptDetails(GetMultiReceiptByTransIdResponse response)
        {
            if (response != null)
            {
                if (response.receipt.Status.Equals("success"))
                {
                    Log.Debug(TAG, "Receipt :" + response.receipt.receiptDetails);
                    RECEPT_NO = response.receipt.receiptDetails.referenceNum;

                    List<AccMultiPay> accounts = response.receipt.receiptDetails.accMultiPay;
                    string accountList = "";
                    int index = 0;
                    foreach (AccMultiPay acct in accounts)
                    {
                        string accountInfoTable = "<tr><table>";

                        accountInfoTable += "<tr style='font-family:times new roman;'>" + "<td style='color: black'>" + this.GetString(Resource.String.receipt_account_number) + "</td><td>:</td><td>" + acct.accountNum + "</td><tr>"
                                            + "<tr style='font-family:times new roman;'><td style='color: black'>" + this.GetString(Resource.String.receipt_account_holder_name) + "</td><td>:</td><td>" + acct.accountOwnerName + "</td><tr>"
                                            + "<tr style='font-family:times new roman;'><td style='color: black'>" + this.GetString(Resource.String.receipt_account_amount) + "</td><td>:</td><td>" + acct.itmAmt + "</td><tr>";
                        accountInfoTable += "</table></br></tr>";
                        accountList += accountInfoTable;
                    }

                    string succesful_message = response.receipt.receiptDetails.payMethod.Equals("FPX") ? this.GetString(Resource.String.receipt_payment_succesful_fpx) : this.GetString(Resource.String.receipt_payment_succesful);

                    receipt_html = HTML_START + "<p style='font-family:times new roman;'><font color='black'>"
                        + this.GetString(Resource.String.receipt_dear_customer) + "</br></br>"
                        + this.GetString(Resource.String.receipt_payment_thank_you) + "</br>"
                        + succesful_message + "</p>"
                        + "<p style='font-family:times new roman;'><font color='black'>"
                        + "<table width='100 %'><tr><td>" + this.GetString(Resource.String.receipt_reference_number) + "</td><td>:</td>" + "<td>" + response.receipt.receiptDetails.referenceNum + "</td></tr>"
                        + "<tr><td>" + this.GetString(Resource.String.receipt_date) + "</td><td>:</td><td>" + response.receipt.receiptDetails.payTransDate + "</td></tr>"
                        + "<tr><td>" + this.GetString(Resource.String.receipt_amount) + "</td><td>:</td><td>" + response.receipt.receiptDetails.payAmt + "</td></tr></table><br></br>"
                        + "<table width='100%'>" + accountList + "</table></p><br></br>"
                        + "<p style='font-family:times new roman;'>" + this.GetString(Resource.String.receipt_transaction_method) + response.receipt.receiptDetails.payMethod + "<br></br>"
                        + this.GetString(Resource.String.receipt_transcation_id) + response.receipt.receiptDetails.payTransID + "<br></br><br></br>"
                        + this.GetString(Resource.String.receipt_thank_you)
                        + "</p>"
                        + HTML_END;

                    string pdfAccountList = "";

                    foreach (AccMultiPay acct in accounts)
                    {
                        string pdfAccountInfo = this.GetString(Resource.String.receipt_account_number) + " : " + acct.accountNum + "\n"
                                            + this.GetString(Resource.String.receipt_account_holder_name) + " : " + acct.accountOwnerName + "\n"
                                            + this.GetString(Resource.String.receipt_account_amount) + " : " + acct.itmAmt + "\n\n";

                        pdfAccountList += pdfAccountInfo;
                    }

                    receipt_pdf = this.GetString(Resource.String.receipt_dear_customer) + "\n\n"
                        + succesful_message + "\n\n"
                        + this.GetString(Resource.String.receipt_reference_number) + " : " + response.receipt.receiptDetails.referenceNum + "\n"
                        + this.GetString(Resource.String.receipt_date) + " : " + response.receipt.receiptDetails.payTransDate + "\n"
                        + this.GetString(Resource.String.receipt_amount) + " : " + response.receipt.receiptDetails.payAmt + "\n\n"
                        + pdfAccountList + "\n\n"
                        + this.GetString(Resource.String.receipt_transaction_method) + response.receipt.receiptDetails.payMethod + "\n"
                        + this.GetString(Resource.String.receipt_transcation_id) + response.receipt.receiptDetails.payTransID + "\n\n"
                        + this.GetString(Resource.String.receipt_thank_you) + "\n";
                    webView.LoadData(receipt_html, "text/html", "UTF-8");
                }
                else
                {
                    ShowErrorMessage(response.receipt.Message);
                }
            }
        }

        public override int ResourceId()
        {
            return Resource.Layout.ViewBillReceiptNewDesign;
        }

        public void SetPresenter(ViewReceiptMultiAccountContract.IUserActionsListener userActionListener)
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

            mPresenter = new ViewReceiptMultiAccountPresenter(this);
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
                    if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.ReadExternalStorage) == (int)Permission.Granted && ContextCompat.CheckSelfPermission(this, Manifest.Permission.WriteExternalStorage) == (int)Permission.Granted)
                    {
                        downloadClicked = true;
                        OnDownloadPDF();


                    }
                    else
                    {
                        RequestPermissions(new string[] { Manifest.Permission.WriteExternalStorage }, Constants.RUNTIME_PERMISSION_STORAGE_REQUEST_CODE);
                    }
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
                        iTextSharp.text.Font font = new iTextSharp.text.Font(bf, 28, iTextSharp.text.Font.NORMAL);
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
                            Intent intent = new Intent(Intent.ActionView);
                            intent.SetDataAndType(Android.Net.Uri.FromFile(file), "application/pdf");
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

        public override bool StoragePermissionRequired()
        {
            return true;
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            if (requestCode == Constants.RUNTIME_PERMISSION_STORAGE_REQUEST_CODE)
            {
                if (Utility.IsPermissionHasCount(grantResults))
                {
                    if (grantResults[0] == Permission.Granted)
                    {
                        RunOnUiThread(() =>
                        {
                            OnDownloadPDF();
                        });

                    }
                }
            }
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
