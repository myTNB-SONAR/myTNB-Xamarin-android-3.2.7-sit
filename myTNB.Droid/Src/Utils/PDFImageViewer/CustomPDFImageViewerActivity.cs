using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.Core.Content;
using CheeseBind;
using myTNB.Mobile;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Base.MVP;
using myTNB_Android.Src.Utils;
using myTNB_Android.Src.Utils.PDFView;
using myTNB_Android.Src.Utils.ZoomImageView;

namespace myTNB_Android.Src.Bills.AccountStatement.Activity
{
    [Activity(ScreenOrientation = ScreenOrientation.Portrait, Theme = "@style/Theme.Dashboard")]
    public class CustomPDFImageViewerActivity : BaseToolbarAppCompatActivity
    {
        [BindView(Resource.Id.pdfBaseView)]
        private LinearLayout pdfBaseView;

        [BindView(Resource.Id.imageBaseView)]
        private LinearLayout imageBaseView;

        [BindView(Resource.Id.imageZoomView)]
        private ZoomImageView imageZoomView;

        private PDFView pdfViewer;

        private string _filePath;
        private string _fileExtension;
        private string _fileTitle;
        public Bitmap fullBitmap;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            try
            {
                SetUpViews();
                pdfViewer = FindViewById<PDFView>(Resource.Id.pdfView);
                Bundle extras = Intent.Extras;
                if (extras != null)
                {
                    if (extras.ContainsKey(Constants.PDF_IMAGE_VIWER_FILE_PATH) && extras.ContainsKey(Constants.PDF_IMAGE_VIEWER_EXTENSION))
                    {
                        if (extras.ContainsKey(Constants.PDF_FILE_TITLE))
                        {
                            _fileTitle = extras.GetString(Constants.PDF_FILE_TITLE);
                            SetToolBarTitle(_fileTitle);
                        }

                        _filePath = extras.GetString(Constants.PDF_IMAGE_VIWER_FILE_PATH);
                        _fileExtension = extras.GetString(Constants.PDF_IMAGE_VIEWER_EXTENSION);

                        if (_fileExtension == Constants.PDF_FILE_EXTENSION)
                        {
                            pdfBaseView.Visibility = ViewStates.Visible;
                            imageBaseView.Visibility = ViewStates.Gone;
                            RenderPDF(_filePath);
                        }
                        else
                        {
                            pdfBaseView.Visibility = ViewStates.Gone;
                            imageBaseView.Visibility = ViewStates.Visible;
                            RenderImage(_filePath);
                        }
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

            SetStatusBarBackground(Resource.Drawable.UsageGradientBackground);
            SetToolbarBackground(Resource.Drawable.CustomDashboardGradientToolbar);
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
                        if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.ReadExternalStorage) != (int)Permission.Granted && ContextCompat.CheckSelfPermission(this, Manifest.Permission.WriteExternalStorage) != (int)Permission.Granted)
                        {
                            RequestPermissions(new string[] { Manifest.Permission.WriteExternalStorage, Manifest.Permission.ReadExternalStorage }, Constants.RUNTIME_PERMISSION_STORAGE_REQUEST_CODE);
                        }
                        else
                        {
                            OnShareFile();
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
                                OnShareFile();
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
            return Resource.Layout.CustomPDFImageView;
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

        private void RenderImage(string imageFilePath)
        {
            _ = GetImageAsync(imageFilePath);
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

                imageZoomView.Visibility = ViewStates.Visible;
                if (fullBitmap != null)
                {
                    imageZoomView
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

        private void OnShareFile()
        {
            try
            {
                if (!string.IsNullOrEmpty(_filePath))
                {
                    Java.IO.File file = new Java.IO.File(_filePath);
                    Android.Net.Uri fileUri = FileProvider.GetUriForFile(this,
                                                ApplicationContext.PackageName + ".fileprovider", file);

                    Intent shareIntent = new Intent(Intent.ActionSend);
                    shareIntent.SetType("application/" + _fileExtension.ToLower());
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
