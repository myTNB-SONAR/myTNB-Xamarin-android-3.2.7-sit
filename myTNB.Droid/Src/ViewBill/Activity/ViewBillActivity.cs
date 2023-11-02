using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Preferences;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.Core.Content;
using CheeseBind;
using Google.Android.Material.Snackbar;
using Java.Text;
using Java.Util;
using myTNB.Mobile.API.Managers.Payment;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.MyTNBService.Response;
using myTNB_Android.Src.Utils;
using myTNB_Android.Src.Utils.PDFView;
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
          , Theme = "@style/Theme.OwnerTenantBaseTheme")]
    public class ViewBillActivity : BaseToolbarAppCompatActivity, ViewBillContract.IView
    {
        AccountData selectedAccount;
        GetBillHistoryResponse.ResponseData selectedBill;

        [BindView(Resource.Id.progressBar)]
        public ProgressBar mProgressBar;

        [BindView(Resource.Id.rootView)]
        public static LinearLayout baseView;

        private static Snackbar mErrorNoInternet;
        private string getPDFUrl = string.Empty;
        private string srNumber = string.Empty;
        private bool isLoadedDocument = false;
        private bool isFromQuickAction = false;
        private bool isTaxInvoice = false;

        CancellationTokenSource cts;

        //17/07/2017
        SimpleDateFormat simpleDateParser = new SimpleDateFormat("dd/MM/yyyy", LocaleUtils.GetDefaultLocale());
        SimpleDateFormat simpleDateFormat = new SimpleDateFormat("MMM yyyy", LocaleUtils.GetCurrentLocale());

        //[BindView(Resource.Id.pdfviewercontrol)]
        PDFView pdfViewer;

        ViewBillContract.IUserActionsListener userActionsListener;
        ViewBillPresenter mPresenter;
        ISharedPreferences mPref;

        string savedPDFPath = string.Empty;

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
            string title = Utility.GetLocalizedLabel("ViewBill", "titleBill");
            if (selectedAccount != null)
            {
                if (selectedAccount.AccountCategoryId != null && selectedAccount.AccountCategoryId.Equals("2"))
                {
                    title = Utility.GetLocalizedLabel("ViewBill", "titleAdvice");
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
                title = simpleDateFormat.Format(d) + " " + title;
            }

            if (isTaxInvoice)
            {
                title = Utility.GetLocalizedLabel("ApplicationStatusDetails", "taxInvoice");
            }
            return title;
        }

        public void SetPresenter(ViewBillContract.IUserActionsListener userActionListener)
        {
            this.userActionsListener = userActionListener;
        }

        public bool IsActive()
        {
            return Window.DecorView.RootView.IsShown;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            Bundle extras = Intent.Extras;

            isFromQuickAction = false;

            if (extras != null)
            {
                if (extras.ContainsKey(Constants.SELECTED_ACCOUNT) && extras.GetString(Constants.SELECTED_ACCOUNT) != null)
                {
                    selectedAccount = DeSerialze<AccountData>(extras.GetString(Constants.SELECTED_ACCOUNT));
                }

                if (extras.ContainsKey(Constants.SELECTED_BILL) && extras.GetString(Constants.SELECTED_BILL) != null)
                {
                    selectedBill = DeSerialze<GetBillHistoryResponse.ResponseData>(extras.GetString(Constants.SELECTED_BILL));
                }

                if (extras.ContainsKey(Constants.CODE_KEY) && extras.GetInt(Constants.CODE_KEY) == Constants.SELECT_ACCOUNT_PDF_REQUEST_CODE)
                {
                    isFromQuickAction = true;
                }
                if (extras.ContainsKey("IsTaxInvoice") && extras.GetBoolean("IsTaxInvoice"))
                {
                    isTaxInvoice = true;
                    if (extras.ContainsKey("SRNumber"))
                    {
                        srNumber = extras.GetString("SRNumber");
                    }
                }
            }
            base.OnCreate(savedInstanceState);
            try
            {
                this.mPresenter = new ViewBillPresenter(this);
                baseView = FindViewById<LinearLayout>(Resource.Id.rootView);
                mProgressBar = FindViewById<ProgressBar>(Resource.Id.progressBar);
                mProgressBar.Visibility = ViewStates.Gone;
                cts = new CancellationTokenSource();
                pdfViewer = FindViewById<PDFView>(Resource.Id.pdf_viewer_control_view);
                try
                {
                    if (isFromQuickAction)
                    {
                        RunOnUiThread(() =>
                        {
                            this.userActionsListener.LoadingBillsHistory(selectedAccount);
                        });
                    }
                    else
                    {
                        if (isTaxInvoice)
                        {
                            getPDFUrl = PaymentManager.Instance.GetTaxInvoiceURL(srNumber);
                            //getPDFUrl = Constants.SERVER_URL.END_POINT + "/v7/mytnbws.asmx/GetBillPDFByBillNo?apiKeyID=" + Constants.APP_CONFIG.API_KEY_ID + "&accNum=" + selectedAccount.AccountNum + "&billingNo=" + selectedBill.NrBill + "&lang=" + LanguageUtil.GetAppLanguage().ToUpper();
                            //pdfURL += URLEncoder.Encode(Constants.SERVER_URL.END_POINT + "/v7/mytnbws.asmx/GetBillPDFByBillNo?apiKeyID=" + Constants.APP_CONFIG.API_KEY_ID + "&accNum=" + selectedAccount.AccountNum + "&billingNo=" + selectedBill.NrBill + "&lang=" + LanguageUtil.GetAppLanguage().ToUpper(), "utf-8");
                            //getPDFUrl = Constants.SERVER_URL.END_POINT + "/v7/mytnbws.asmx/GetBillMaskingPDF?apiKeyID=" + Constants.APP_CONFIG.API_KEY_ID + "&accNum=" + selectedAccount.AccountNum + "&billingNo=" + selectedBill.NrBill + "&lang=" + LanguageUtil.GetAppLanguage().ToUpper() + "&isOwnerBill=" + selectedAccount.IsOwner.ToString();
                            //pdfURL += URLEncoder.Encode(Constants.SERVER_URL.END_POINT + "/v7/mytnbws.asmx/GetBillMaskingPDF?apiKeyID=" + Constants.APP_CONFIG.API_KEY_ID + "&accNum=" + selectedAccount.AccountNum + "&billingNo=" + selectedBill.NrBill + "&lang=" + LanguageUtil.GetAppLanguage().ToUpper() + "&isOwnerBill=" + selectedAccount.IsOwner.ToString(), "utf-8");
                            //webView.LoadUrl("http://drive.google.com/viewerng/viewer?embedded=true&url=" + URLEncoder.Encode(Constants.SERVER_URL.END_POINT + "/GetBillPDF?apiKeyID="+Constants.APP_CONFIG.API_KEY_ID+"&accNum=" + selectedAccount.AccountNum+"&billingNo="+selectedBill.NrBill, "utf-8"));
                        }
                        else
                        {
                            bool isWhiteList = UserSessions.GetWhiteList(PreferenceManager.GetDefaultSharedPreferences(this));
                            if (selectedBill != null && !string.IsNullOrEmpty(selectedBill.NrBill))
                            {
                                if (selectedAccount.IsHaveAccess == true || isWhiteList)
                                {
                                    selectedAccount.IsOwner = true;
                                }
                                //getPDFUrl = Constants.SERVER_URL.END_POINT + "/v7/mytnbws.asmx/GetBillPDFByBillNo?apiKeyID=" + Constants.APP_CONFIG.API_KEY_ID + "&accNum=" + selectedAccount.AccountNum + "&billingNo=" + selectedBill.NrBill + "&lang=" + LanguageUtil.GetAppLanguage().ToUpper();
                                //pdfURL += URLEncoder.Encode(Constants.SERVER_URL.END_POINT + "/v7/mytnbws.asmx/GetBillPDFByBillNo?apiKeyID=" + Constants.APP_CONFIG.API_KEY_ID + "&accNum=" + selectedAccount.AccountNum + "&billingNo=" + selectedBill.NrBill + "&lang=" + LanguageUtil.GetAppLanguage().ToUpper(), "utf-8");
                                getPDFUrl = Constants.SERVER_URL.END_POINT + "/v7/mytnbws.asmx/GetBillMaskingPDF?apiKeyID=" + Constants.APP_CONFIG.API_KEY_ID + "&contractAccount=" + selectedAccount.AccountNum + "&billingNo=" + selectedBill.NrBill + "&lang=" + LanguageUtil.GetAppLanguage().ToUpper() + "&isOwnerBill=" + selectedAccount.IsOwner.ToString();
                                //pdfURL += URLEncoder.Encode(Constants.SERVER_URL.END_POINT + "/v7/mytnbws.asmx/GetBillMaskingPDF?apiKeyID=" + Constants.APP_CONFIG.API_KEY_ID + "&accNum=" + selectedAccount.AccountNum + "&billingNo=" + selectedBill.NrBill + "&lang=" + LanguageUtil.GetAppLanguage().ToUpper() + "&isOwnerBill=" + selectedAccount.IsOwner.ToString(), "utf-8");
                            }
                            else
                            {
                                if (selectedAccount.IsHaveAccess == true || isWhiteList)
                                {
                                    selectedAccount.IsOwner = true;
                                }
                                //getPDFUrl = Constants.SERVER_URL.END_POINT + "/v7/mytnbws.asmx/GetBillPDF?apiKeyID=" + Constants.APP_CONFIG.API_KEY_ID + "&accNum=" + selectedAccount.AccountNum + "&lang=" + LanguageUtil.GetAppLanguage().ToUpper();
                                //pdfURL += URLEncoder.Encode(Constants.SERVER_URL.END_POINT + "/v7/mytnbws.asmx/GetBillPDF?apiKeyID=" + Constants.APP_CONFIG.API_KEY_ID + "&accNum=" + selectedAccount.AccountNum + "&lang=" + LanguageUtil.GetAppLanguage().ToUpper(), "utf-8");
                                getPDFUrl = Constants.SERVER_URL.END_POINT + "/v7/mytnbws.asmx/GetBillMaskingPDF?apiKeyID=" + Constants.APP_CONFIG.API_KEY_ID + "&accNum=" + selectedAccount.AccountNum + "&lang=" + LanguageUtil.GetAppLanguage().ToUpper() + "&isOwnerBill" + selectedAccount.IsOwner.ToString();
                                //pdfURL += URLEncoder.Encode(Constants.SERVER_URL.END_POINT + "/v7/mytnbws.asmx/GetBillMaskingPDF?apiKeyID=" + Constants.APP_CONFIG.API_KEY_ID + "&contractAccount=" + selectedAccount.AccountNum + "&lang=" + LanguageUtil.GetAppLanguage().ToUpper() + "&isOwnerBill=" + selectedAccount.IsOwner.ToString(), "utf-8");
                            }
                        }
                        RunOnUiThread(() =>
                        {
                            GetPDF();
                        });
                    }
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

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.ViewBillStatementMenu, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            try
            {
                switch (item.ItemId)
                {
                    case Resource.Id.action_share:
                        if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.Tiramisu)                                          //Starting android 13 asking notification permission
                        {
                            OnSharePDF();
                        }
                        else
                        {
                            if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.ReadExternalStorage) != (int)Permission.Granted && ContextCompat.CheckSelfPermission(this, Manifest.Permission.WriteExternalStorage) != (int)Permission.Granted)
                            {
                                RequestPermissions(new string[] { Manifest.Permission.WriteExternalStorage, Manifest.Permission.ReadExternalStorage }, Constants.RUNTIME_PERMISSION_STORAGE_REQUEST_CODE);
                            }
                            else
                            {
                                OnSharePDF();
                            }
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
                try
                {
                    LoadingOverlayUtils.OnRunLoadingAnimation(this);
                }
                catch (Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                }

                savedPDFPath = string.Empty;

                await Task.Run(() =>
                {
                    savedPDFPath = OnDownloadPDF();
                }, cts.Token);

                if (!string.IsNullOrEmpty(savedPDFPath))
                {
                    try
                    {
                        Java.IO.File file = new Java.IO.File(savedPDFPath);
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
            string path = string.Empty;

            try
            {
                if (!string.IsNullOrEmpty(getPDFUrl))
                {
                    string filename = string.Empty;
                    if (isTaxInvoice)
                    {
                        filename = srNumber + ".pdf";
                    }
                    else if (!string.IsNullOrEmpty(selectedAccount?.AccountNum))
                    {
                        filename = selectedAccount?.AccountNum + ".pdf";
                        if (!string.IsNullOrEmpty(selectedBill?.NrBill))
                        {
                            filename = selectedAccount?.AccountNum + "_" + selectedBill?.NrBill + ".pdf";
                        }
                    }
                    if (!string.IsNullOrEmpty(filename))
                    {
                        string rootPath = this.FilesDir.AbsolutePath;
                        if (Utils.FileUtils.IsExternalStorageReadable() && Utils.FileUtils.IsExternalStorageWritable())
                        {
                            rootPath = this.GetExternalFilesDir(null).AbsolutePath;
                        }

                        var directory = Path.Combine(rootPath, "pdf");
                        if (!Directory.Exists(directory))
                        {
                            Directory.CreateDirectory(directory);
                        }

                        path = Path.Combine(directory, filename);

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
                                    client.DownloadFile(getPDFUrl, path);
                                }
                            }
                            catch (Exception e)
                            {
                                Log.Debug("ViewBillActivity", e.StackTrace);
                                path = string.Empty;
                                Utility.LoggingNonFatalError(e);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                path = string.Empty;
                Utility.LoggingNonFatalError(e);
            }
            return path;
        }

        private void OnSharePDF()
        {
            try
            {
                if (!string.IsNullOrEmpty(savedPDFPath))
                {
                    Java.IO.File file = new Java.IO.File(savedPDFPath);
                    Android.Net.Uri fileUri = FileProvider.GetUriForFile(this,
                                                ApplicationContext.PackageName + ".fileprovider", file);

                    Intent shareIntent = new Intent(Intent.ActionSend);
                    shareIntent.SetType("application/pdf");
                    shareIntent.PutExtra(Intent.ExtraStream, fileUri);
                    StartActivity(Intent.CreateChooser(shareIntent, Utility.GetLocalizedLabel("Profile", "share")));
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public static void ShowErrorMessageNoInternet(string failingUrl)
        {
            try
            {
                if (mErrorNoInternet != null && mErrorNoInternet.IsShown)
                {
                    mErrorNoInternet.Dismiss();
                }

                mErrorNoInternet = Snackbar.Make(baseView, Utility.GetLocalizedErrorLabel("noDataConnectionMessage"), Snackbar.LengthIndefinite)
                .SetAction(Utility.GetLocalizedLabel("Common", "tryAgain"), delegate
                {
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
                                OnSharePDF();
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

        protected override void OnResume()
        {
            try
            {
                base.OnResume();
                FirebaseAnalyticsUtils.SetScreenName(this, "View Bill PDF");
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void ShowBillPDF(GetBillHistoryResponse.ResponseData selectedBill = null)
        {
            try
            {
                bool isWhiteList = UserSessions.GetWhiteList(PreferenceManager.GetDefaultSharedPreferences(this));
                if (selectedBill != null && !string.IsNullOrEmpty(selectedBill.NrBill))
                {
                   
                    if (selectedAccount.IsHaveAccess == true || isWhiteList)
                    {
                        selectedAccount.IsOwner = true;
                    }
                    
                    //getPDFUrl = Constants.SERVER_URL.END_POINT + "/v7/mytnbws.asmx/GetBillPDFByBillNo?apiKeyID=" + Constants.APP_CONFIG.API_KEY_ID + "&accNum=" + selectedAccount.AccountNum + "&billingNo=" + selectedBill.NrBill + "&lang=" + LanguageUtil.GetAppLanguage().ToUpper();
                    //pdfURL += URLEncoder.Encode(Constants.SERVER_URL.END_POINT + "/v7/mytnbws.asmx/GetBillPDFByBillNo?apiKeyID=" + Constants.APP_CONFIG.API_KEY_ID + "&accNum=" + selectedAccount.AccountNum + "&billingNo=" + selectedBill.NrBill + "&lang=" + LanguageUtil.GetAppLanguage().ToUpper(), "utf-8");
                    getPDFUrl = Constants.SERVER_URL.END_POINT + "/v7/mytnbws.asmx/GetBillMaskingPDF?apiKeyID=" + Constants.APP_CONFIG.API_KEY_ID + "&contractAccount=" + selectedAccount.AccountNum + "&billingNo=" + selectedBill.NrBill + "&lang=" + LanguageUtil.GetAppLanguage().ToUpper() + "&isOwnerBill=" + selectedAccount.IsOwner.ToString();
                    //pdfURL += URLEncoder.Encode(Constants.SERVER_URL.END_POINT + "/v7/mytnbws.asmx/GetBillMaskingPDF?apiKeyID=" + Constants.APP_CONFIG.API_KEY_ID + "&accNum=" + selectedAccount.AccountNum + "&billingNo=" + selectedBill.NrBill + "&lang=" + LanguageUtil.GetAppLanguage().ToUpper() + "&isOwnerBill=" + selectedAccount.IsOwner.ToString(), "utf-8");
                }
                else
                {
                    if (selectedAccount.IsHaveAccess == true || isWhiteList)
                    {
                        selectedAccount.IsOwner = true;
                    }
                    //getPDFUrl = Constants.SERVER_URL.END_POINT + "/v7/mytnbws.asmx/GetBillPDF?apiKeyID=" + Constants.APP_CONFIG.API_KEY_ID + "&accNum=" + selectedAccount.AccountNum + "&lang=" + LanguageUtil.GetAppLanguage().ToUpper();
                    //pdfURL += URLEncoder.Encode(Constants.SERVER_URL.END_POINT + "/v7/mytnbws.asmx/GetBillPDF?apiKeyID=" + Constants.APP_CONFIG.API_KEY_ID + "&accNum=" + selectedAccount.AccountNum + "&lang=" + LanguageUtil.GetAppLanguage().ToUpper(), "utf-8");
                    getPDFUrl = Constants.SERVER_URL.END_POINT + "/v7/mytnbws.asmx/GetBillMaskingPDF?apiKeyID=" + Constants.APP_CONFIG.API_KEY_ID + "&accNum=" + selectedAccount.AccountNum + "&lang=" + LanguageUtil.GetAppLanguage().ToUpper() + "&isOwnerBill" + selectedAccount.IsOwner.ToString();
                    //pdfURL += URLEncoder.Encode(Constants.SERVER_URL.END_POINT + "/v7/mytnbws.asmx/GetBillMaskingPDF?apiKeyID=" + Constants.APP_CONFIG.API_KEY_ID + "&contractAccount=" + selectedAccount.AccountNum + "&lang=" + LanguageUtil.GetAppLanguage().ToUpper() + "&isOwnerBill=" + selectedAccount.IsOwner.ToString(), "utf-8");
                }

                Date d = null;
                string title = Utility.GetLocalizedLabel("ViewBill", "titleBill");

                if (selectedAccount != null)
                {
                    if (selectedAccount.AccountCategoryId != null && selectedAccount.AccountCategoryId.Equals("2"))
                    {
                        title = Utility.GetLocalizedLabel("ViewBill", "titleAdvice");
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
                    title = simpleDateFormat.Format(d) + " " + title;
                }

                RunOnUiThread(() =>
                {
                    this.SetToolBarTitle(title);
                });

                RunOnUiThread(() =>
                {
                    _ = GetPDF();
                });
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

        public void ShowViewBillError(string title, string message)
        {
            try
            {
                this.RunOnUiThread(() =>
                {
                    try
                    {
                        this.RunOnUiThread(() =>
                        {
                            string errorTitle = string.IsNullOrEmpty(title) ? Utility.GetLocalizedErrorLabel("defaultErrorTitle") : title;
                            string errorMessage = string.IsNullOrEmpty(message) ? Utility.GetLocalizedErrorLabel("defaultErrorMessage") : message;
                            MyTNBAppToolTipBuilder.Create(this, MyTNBAppToolTipBuilder.ToolTipType.NORMAL_WITH_HEADER)
                                .SetTitle(errorTitle)
                                .SetMessage(errorMessage)
                                .SetCTALabel(Utility.GetLocalizedCommonLabel("ok"))
                                .Build().Show();
                            this.SetIsClicked(false);
                        });
                    }
                    catch (System.Exception e)
                    {
                        this.SetIsClicked(false);
                        Utility.LoggingNonFatalError(e);
                    }
                });
            }
            catch (System.Exception e)
            {
                this.SetIsClicked(false);
                Utility.LoggingNonFatalError(e);
            }
        }

        private Snackbar mLoadBillSnackBar;
        public void ShowBillErrorSnackBar()
        {
            try
            {
                if (mLoadBillSnackBar != null && mLoadBillSnackBar.IsShown)
                {
                    mLoadBillSnackBar.Dismiss();
                }

                mLoadBillSnackBar = Snackbar.Make(baseView, Utility.GetLocalizedErrorLabel("defaultErrorMessage"), Snackbar.LengthIndefinite)
                .SetAction(Utility.GetLocalizedCommonLabel("close"), delegate
                {
                    mLoadBillSnackBar.Dismiss();
                }
                );
                View v = mLoadBillSnackBar.View;
                TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
                tv.SetMaxLines(5);
                mLoadBillSnackBar.Show();
                this.SetIsClicked(false);
            }
            catch (System.Exception e)
            {
                this.SetIsClicked(false);
                Utility.LoggingNonFatalError(e);
            }
        }
    }
}