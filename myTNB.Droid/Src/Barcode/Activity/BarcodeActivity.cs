using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using CheeseBind;
using Google.Android.Material.Snackbar;
using myTNB_Android.Src.Barcode.MVP;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Utils;
using System;
using System.Collections.Generic;
using System.Runtime;
using System.Threading.Tasks;
using ZXing.Mobile;

namespace myTNB_Android.Src.Barcode.Activity
{
    [Activity(Label = "@string/barcode_activity_title"
        , ScreenOrientation = ScreenOrientation.Portrait
        , Theme = "@style/Theme.BarCode")]
    public class BarcodeActivity : BaseActivityCustom, BarcodeContract.IView
    {
        readonly static string TAG = typeof(BarcodeActivity).Name;
        ZXingScannerFragment scanFragment;

        [BindView(Resource.Id.barCodeView)]
        FrameLayout barCodeView;


        [BindView(Resource.Id.topBarcodeView)]
        View topBarcodeView;

        [BindView(Resource.Id.txtTitle)]
        TextView txtTitle;

        [BindView(Resource.Id.rootView)]
        LinearLayout rootView;


        private BarcodeContract.IUserActionsListener userActionsListener;
        private BarcodePresenter mPresenter;
        private string PAGE_ID = "AddAccount";

        public override int ResourceId()
        {
            return Resource.Layout.BarcodeScannerView;
        }

        

        public override bool ShowCustomToolbarTitle()
        {
            return true;
        }

        protected override void OnCreate(Bundle savedInstanceState)

        {


            Bundle extras = Intent.Extras;
            if (extras != null)
            {
                ///PAGE TITLE FROM BEFORE PAGE
                if (extras.ContainsKey(Constants.PAGE_TITLE))
                {

              
                    PAGE_ID = "SubmitEnquiry";



                }
            } 

               

            base.OnCreate(savedInstanceState);
            SetTheme(TextViewUtils.IsLargeFonts ? Resource.Style.Theme_BarCodeLarge : Resource.Style.Theme_BarCode);
            mPresenter = new BarcodePresenter(this);
            barCodeView.Click += delegate
            {
                if (scanFragment != null)
                {
                    scanFragment.StopScanning();
                    scan();
                }
            };

            TextViewUtils.SetMuseoSans500Typeface(txtTitle);
            txtTitle.Text = GetLabelByLanguage("scanMessage");
            txtTitle.TextSize = TextViewUtils.GetFontSize(14);

        }

        protected override void OnResume()
        {
            base.OnResume();

            try
            {
                var needsPermissionRequest = ZXing.Net.Mobile.Android.PermissionsHandler.NeedsPermissionRequest(this);

                if (needsPermissionRequest)
                {
                    ZXing.Net.Mobile.Android.PermissionsHandler.RequestPermissionsAsync(this);
                }
                else
                {
                    if (scanFragment == null)
                    {
                        scanFragment = new ZXingScannerFragment();
                        SupportFragmentManager.BeginTransaction()
                            .Replace(Resource.Id.barCodeView, scanFragment)
                            .CommitAllowingStateLoss();
                        SupportFragmentManager.ExecutePendingTransactions();
                    }

                    scan();

                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

            try
            {
                FirebaseAnalyticsUtils.SetScreenName(this, "Add Account -> Barcode Scan");
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }


        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            ZXing.Net.Mobile.Android.PermissionsHandler.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        protected override void OnPause()
        {
            if (scanFragment != null)
            {
                scanFragment?.StopScanning();
                scanFragment = null;
            }


            base.OnPause();
        }

        void scan()
        {
            var opts = new MobileBarcodeScanningOptions
            {
                PossibleFormats = new List<ZXing.BarcodeFormat> {
                    ZXing.BarcodeFormat.QR_CODE,
                    ZXing.BarcodeFormat.CODE_128,
                    ZXing.BarcodeFormat.CODE_39,
                    ZXing.BarcodeFormat.CODE_93,
                    ZXing.BarcodeFormat.AZTEC,
                    ZXing.BarcodeFormat.All_1D,
                    ZXing.BarcodeFormat.CODABAR,
                    ZXing.BarcodeFormat.DATA_MATRIX,
                    ZXing.BarcodeFormat.EAN_13,
                    ZXing.BarcodeFormat.EAN_8,
                    ZXing.BarcodeFormat.IMB,
                    ZXing.BarcodeFormat.ITF,
                    ZXing.BarcodeFormat.MAXICODE,
                    ZXing.BarcodeFormat.MSI,
                    ZXing.BarcodeFormat.PDF_417,
                    ZXing.BarcodeFormat.PLESSEY,
                    ZXing.BarcodeFormat.RSS_14,
                    ZXing.BarcodeFormat.UPC_A,
                    ZXing.BarcodeFormat.UPC_E,
                    ZXing.BarcodeFormat.UPC_EAN_EXTENSION
                },
                CameraResolutionSelector = availableResolutions =>
                {

                    foreach (var ar in availableResolutions)
                    {
                        Console.WriteLine("Resolution: " + ar.Width + "x" + ar.Height);
                    }
                    return null;
                }
            };

            scanFragment.UseCustomOverlayView = true;
            scanFragment.CustomOverlayView = barCodeView;

            scanFragment.AutoFocus();
            Log.Debug(TAG, "Starting Scan...");
            scanFragment.StartScanning(result =>
            {
                Log.Debug(TAG, string.Format("Scanning result {0}", result));
                if (result == null || string.IsNullOrEmpty(result.Text))
                {
                    return;
                }

                string scannedText = result.Text;
                if (!string.IsNullOrEmpty(scannedText) && scannedText.Length > 12)
                {
                    scannedText = scannedText.Substring(0, 12);
                }
                this.userActionsListener.OnResult(scannedText);
            }, opts);
            StartAutoFocus();

        }

        private void StartAutoFocus()
        {
            if (scanFragment == null)
            {
                return;
            }

            try
            {
                Task.Run(async () =>
                {
                    try
                    {
                        await Task.Delay(2000);
                        RunOnUiThread(() => {
                            try
                            {
                                if (scanFragment != null)
                                {
                                    scanFragment.AutoFocus();
                                }
                            }
                            catch (System.Exception exe)
                            {
                                Utility.LoggingNonFatalError(exe);
                            }
                        });
                    }
                    catch (System.Exception ex)
                    {
                        Utility.LoggingNonFatalError(ex);
                    }
                });
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }
        private Snackbar mSnackbarMessage;
        public void ShowInvalidBarCodeError()
        {

            if (mSnackbarMessage != null && !mSnackbarMessage.IsShown)
            {
                mSnackbarMessage.Dismiss();
            }

            scanFragment.SetErrorMessage(GetLabelByLanguage("invalidBarcode"));
            mSnackbarMessage = Snackbar.Make(rootView, GetLabelByLanguage("invalidBarcode"), Snackbar.LengthIndefinite)
            .SetAction(Utility.GetLocalizedCommonLabel("close"), delegate
            {

                mSnackbarMessage.Dismiss();

            }
            );
            View v = mSnackbarMessage.View;
            TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
            tv.SetMaxLines(5);
            mSnackbarMessage.Show();
        }

        public void ShowSuccessActivity(string result)
        {
            // PROCEED TO PREVIOUS ACTIVITY AND SEND THE RESULT
            scanFragment.SetSuccess();
            Intent resultIntent = new Intent();
            resultIntent.PutExtra(Constants.BARCODE_RESULT, result);
            SetResult(Android.App.Result.Ok, resultIntent);
            Finish();
        }

        public void SetPresenter(BarcodeContract.IUserActionsListener userActionListener)
        {
            this.userActionsListener = userActionListener;
        }

        public bool IsActive()
        {
            return Window.DecorView.RootView.IsShown;
        }

        public void HideInvalidBarCodeError()
        {

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

        public override string GetPageId()
        {
            return PAGE_ID;
        }
    }
}
