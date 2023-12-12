using System;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.Core.Content;
using CheeseBind;
using myTNB.Mobile;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Utils;
using myTNB_Android.Src.Utils.PDFView;

namespace myTNB_Android.Src.Bills.AccountStatement.Activity
{
    [Activity(ScreenOrientation = ScreenOrientation.Portrait, Theme = "@style/Theme.Dashboard")]
    public class AccountStatementPDFActivity : BaseToolbarAppCompatActivity
    {
        [BindView(Resource.Id.acctStmntBaseView)]
        public static LinearLayout acctStmntBaseView;

        PDFView pdfViewer;
        private string pdfFilePath;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            try
            {
                SetUpViews();
                pdfViewer = FindViewById<PDFView>(Resource.Id.acctStmntPDFView);
                Bundle extras = Intent.Extras;
                if (extras != null)
                {
                    if (extras.ContainsKey(Constants.ACCT_STMNT_PDF_FILE_PATH))
                    {
                        pdfFilePath = extras.GetString(Constants.ACCT_STMNT_PDF_FILE_PATH);
                        RenderPDF(pdfFilePath);
                    }
                }
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
            DynatraceHelper.OnTrack(DynatraceConstants.BR.Screens.AccountStatement.View_Account_Statement);
        }

        private void SetUpViews()
        {
            SetTheme(TextViewUtils.IsLargeFonts
                ? Resource.Style.Theme_DashboardLarge
                : Resource.Style.Theme_Dashboard);

            SetToolBarTitle(Utility.GetLocalizedLabel(LanguageConstants.STATEMENT_PERIOD, LanguageConstants.StatementPeriod.TITLE));
            SetStatusBarBackground(Resource.Drawable.UsageGradientBackground);
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

        protected override void OnStart()
        {
            base.OnStart();
        }

        public override int ResourceId()
        {
            return Resource.Layout.AccountStatementPDFView;
        }

        public override Boolean ShowCustomToolbarTitle()
        {
            return true;
        }

        public bool IsActive()
        {
            return this.Window.DecorView.RootView.IsShown;
        }

        private void RenderPDF(string pdfFilePath)
        {
            if (pdfFilePath.IsValid())
            {
                try
                {
                    Java.IO.File file = new Java.IO.File(pdfFilePath);
                    pdfViewer
                        .FromFile(file)
                        .Show();
                }
                catch (Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                }
            }
        }

        private void OnSharePDF()
        {
            try
            {
                if (!string.IsNullOrEmpty(pdfFilePath))
                {
                    Java.IO.File file = new Java.IO.File(pdfFilePath);
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

        public override void OnBackPressed()
        {
            SetResult(Result.Canceled);
            Finish();
        }
    }
}
