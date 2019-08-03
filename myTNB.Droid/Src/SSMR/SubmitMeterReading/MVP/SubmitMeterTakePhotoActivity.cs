using Android.App;
using Android.Graphics;
using Android.OS;
using Android.Widget;
using myTNB_Android.Src.Base.Activity;

namespace myTNB_Android.Src.SSMR.SubmitMeterReading.MVP
{
    [Activity(Label = "Take Photo", Theme = "@style/Theme.Dashboard", MainLauncher = true)]
    public class SubmitMeterTakePhotoActivity : BaseToolbarAppCompatActivity, SubmitMeterTakePhotoContract.IView
    {
        Button btnGetMeterReadingOCR;
        SubmitMeterTakePhotoContract.IPresenter mPresenter;
        const string IMAGE_ID = "MYTNBAPP_SSMR_OCR_KWH_001";
        string contractNumber = "220098081110";
        
        public override int ResourceId()
        {
            return Resource.Layout.SubmitMeterTakePhotoLayout;
        }

        public override bool ShowCustomToolbarTitle()
        {
            return true;
        }

        public override bool CameraPermissionRequired()
        {
            return true;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            mPresenter = new SubmitMeterTakePhotoPresenter(this);
            if (savedInstanceState == null)
            {
                FragmentManager.BeginTransaction().Replace(Resource.Id.photoContainer, SubmitMeterTakePhotoFragment.NewInstance()).Commit();
            }

            btnGetMeterReadingOCR = FindViewById<Button>(Resource.Id.btnSubmitPhotoToOCR);
            btnGetMeterReadingOCR.Click += delegate
            {
                //mPresenter.GetMeterReadingOCRValue(contractNumber);
            };

        }

        protected override void OnStart()
        {
            base.OnStart();

        }

        public void ShowAdjustFragment(Bitmap myBitmap)
        {
            //string base64String = Utils.ImageUtils.GetBase64FromBitmap(myBitmap);
            //int size = myBitmap.ByteCount;
            mPresenter.AddMeterImage(contractNumber, IMAGE_ID, myBitmap);
            FragmentTransaction transaction = FragmentManager.BeginTransaction();
            SubmitMeterAdjustPhotoFragment adjustPhotoFragment = SubmitMeterAdjustPhotoFragment.NewIntance();
            adjustPhotoFragment.SetCapturedImage(myBitmap);
            transaction.Replace(Resource.Id.photoContainer, adjustPhotoFragment);
            transaction.AddToBackStack(null);
            transaction.Commit();
        }
    }
}
