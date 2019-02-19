using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Content.PM;
using myTNB_Android.Src.Base.Activity;
using ZXing.Mobile;
using CheeseBind;
using System.Threading.Tasks;
using Android.Util;
using myTNB_Android.Src.Barcode.MVP;
using myTNB_Android.Src.Utils;
using Android.Support.Design.Widget;
using Android.Support.V4.Content;
using Android;
using System.Runtime;

namespace myTNB_Android.Src.Barcode.Activity
{
    [Activity(Label = "@string/barcode_activity_title"
        , ScreenOrientation = ScreenOrientation.Portrait
        , Theme = "@style/Theme.BarCode")]
    public class BarcodeActivity : BaseToolbarAppCompatActivity , BarcodeContract.IView
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
            base.OnCreate(savedInstanceState);
            // Create your application here
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
        }

        protected override void OnResume()
        {
            base.OnResume();

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

                        .Commit()
                        ;
                    SupportFragmentManager.ExecutePendingTransactions();
                }

                scan();

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
                CameraResolutionSelector = availableResolutions => {

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
            Log.Debug(TAG , "Starting Scan...");
            scanFragment.StartScanning(result => {
                Log.Debug(TAG, string.Format("Scanning result {0}" , result));
                // Null result means scanning was cancelled
                if (result == null || string.IsNullOrEmpty(result.Text))
                {
                    //Toast.MakeText(this, "Scanning Cancelled", ToastLength.Long).Show();
                    return;
                }

                // Otherwise, proceed with result
                string scannedText = result.Text;
                if(!string.IsNullOrEmpty(scannedText) && scannedText.Length > 12)
                {
                    scannedText = scannedText.Substring(0, 12);
                }
                this.userActionsListener.OnResult(scannedText);
                //RunOnUiThread(() => Toast.MakeText(this, "Scanned: " + result.Text, ToastLength.Short).Show());
            }, opts);
            StartAutoFocus();

        }

        private void StartAutoFocus()
        {
            if (scanFragment == null)
            {
                return;
            }

            Task.Run(async () =>
            {
                await Task.Delay(2000);
                RunOnUiThread(() => scanFragment.AutoFocus());
            });
        }
        private Snackbar mSnackbarMessage;
        public void ShowInvalidBarCodeError()
        {

            if (mSnackbarMessage != null && !mSnackbarMessage.IsShown)
            {
                mSnackbarMessage.Dismiss();
            }

            scanFragment.SetErrorMessage(GetString(Resource.String.barcode_form_invalid_barcode));
            mSnackbarMessage = Snackbar.Make(rootView, GetString(Resource.String.barcode_form_invalid_barcode), Snackbar.LengthIndefinite)
            .SetAction(GetString(Resource.String.barcode_form_btn_snackbar_close), delegate {

                mSnackbarMessage.Dismiss();
                
            }
            );
            mSnackbarMessage.Show();
        }

        public void ShowSuccessActivity(string result)
        {
            // PROCEED TO PREVIOUS ACTIVITY AND SEND THE RESULT
            scanFragment.SetSuccess();
            Intent resultIntent = new Intent();
            resultIntent.PutExtra(Constants.BARCODE_RESULT , result);
            SetResult(Android.App.Result.Ok , resultIntent);
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
    }
}