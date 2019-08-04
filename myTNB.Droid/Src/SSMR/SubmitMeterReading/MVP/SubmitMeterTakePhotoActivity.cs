using AFollestad.MaterialDialogs;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using CheeseBind;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Utils;

namespace myTNB_Android.Src.SSMR.SubmitMeterReading.MVP
{
    [Activity(Label = "Take Photo", Theme = "@style/Theme.Dashboard")]
    public class SubmitMeterTakePhotoActivity : BaseToolbarAppCompatActivity, SubmitMeterTakePhotoContract.IView
    {
        Button btnGetMeterReadingOCR;
        SubmitMeterTakePhotoContract.IPresenter mPresenter;
        const string IMAGE_ID = "MYTNBAPP_SSMR_OCR_KWH_001";
        string contractNumber = "220098081110";

        int selectedCapturedImage = 0;

        private IMenu ssmrMenu;

        [BindView(Resource.Id.meterReadingTakePhotoTitle)]
        TextView meterReadingTakePhotoTitle;

        [BindView(Resource.Id.meter_capture_container)]
        LinearLayout meterCapturedContainer;

        [BindView(Resource.Id.bottomLayout)]
        LinearLayout bottomLayout;

        public static readonly int PickImageId = 1000;

        private static bool isGalleryFirstPress = true;

        private bool isSinglePhase = false;

        SubmitMeterAdjustPhotoFragment adjustPhotoFragment;
        OCRLoadingFragment ocrLoadingFragment;

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
            isGalleryFirstPress = true;
            if (savedInstanceState == null)
            {
                FragmentManager.BeginTransaction().Replace(Resource.Id.photoContainer, SubmitMeterTakePhotoFragment.NewInstance()).Commit();
            }

            btnGetMeterReadingOCR = FindViewById<Button>(Resource.Id.btnSubmitPhotoToOCR);
            btnGetMeterReadingOCR.Click += delegate
            {
                mPresenter.GetMeterReadingOCRValue(contractNumber);
            };
            mPresenter.InitializeModelList();

            // TODO: Logic to reflect the flag
            isSinglePhase = false;
        }

        public override void OnBackPressed()
        {
            base.OnBackPressed();
            SetToolBarTitle("Take Photo");
            meterReadingTakePhotoTitle.Text = "You're done with kW and kVARh! On to kWh reading now.";
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (requestCode == PickImageId)
            {
                Bitmap bitmap = MediaStore.Images.Media.GetBitmap(this.ContentResolver, data.Data);
                AddCapturedImageInContainer(bitmap);
                ShowAdjustFragment(selectedCapturedImage,bitmap);
            }
        }

        protected override void OnStart()
        {
            base.OnStart();
        }

        public void ShowAdjustFragment(int position,Bitmap myBitmap)
        {
            FragmentTransaction transaction = FragmentManager.BeginTransaction();
            SetToolBarTitle("Adjust Photo");
            meterReadingTakePhotoTitle.Text = "You can delete and retake the photo or adjust it before submission.";
            if (adjustPhotoFragment == null)
            {
                adjustPhotoFragment = SubmitMeterAdjustPhotoFragment.NewIntance();
                adjustPhotoFragment.SetCapturedImage(myBitmap);
                transaction.Replace(Resource.Id.photoContainer, adjustPhotoFragment);
                transaction.AddToBackStack(null);
                transaction.Commit();
            }
            else
            {
                adjustPhotoFragment.UpdateCapturedImage(myBitmap);
            }
            selectedCapturedImage = position;

            //FragmentManager.PopBackStack(null, FragmentManager.PopBackStackInclusive);
            //FragmentManager fm = this.FragmentManager;
            //int count = fm.BackStackEntryCount;
            //for (int i = 1; i < fm.BackStackEntryCount; ++i)
            //{
            //    fm.PopBackStack(;
            //}
        }

        public void AddCapturedImage(Bitmap capturedImage)
        {
            mPresenter.AddMeterImageAt(selectedCapturedImage,contractNumber, IMAGE_ID, capturedImage);
            LinearLayout container = (LinearLayout)meterCapturedContainer.GetChildAt(selectedCapturedImage);
            container.SetBackgroundDrawable(GetDrawable(Resource.Drawable.meter_capture_holder_active));
            AddCapturedImageInContainer(capturedImage);
        }

        public void DeleteCapturedImage()
        {
            mPresenter.RemoveMeterImageAt(selectedCapturedImage);
            DeleteCapturedImageInContainer();
        }

        private void AddCapturedImageInContainer(Bitmap capturedImage)
        {
            LinearLayout container = (LinearLayout)meterCapturedContainer.GetChildAt(selectedCapturedImage);
            container.AddView(CreateImageView(capturedImage));
            container.SetBackgroundDrawable(GetDrawable(Resource.Drawable.meter_capture_holder_active));
            UpdateImageSelections();
        }

        private void DeleteCapturedImageInContainer()
        {
            LinearLayout container = (LinearLayout)meterCapturedContainer.GetChildAt(selectedCapturedImage);
            container.SetBackgroundDrawable(GetDrawable(Resource.Drawable.meter_capture_holder_inactive));
            container.RemoveViewAt(0);
            UpdateImageSelections();
            adjustPhotoFragment = null;
            OnBackPressed();
        }

        private ImageView CreateImageView(Bitmap bitmap)
        {
            ImageView imageView = new ImageView(this);
            LinearLayout.LayoutParams layoutParams = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, LinearLayout.LayoutParams.MatchParent);
            imageView.LayoutParameters = layoutParams;
            imageView.SetPadding(1,1,1,1);
            imageView.SetScaleType(ImageView.ScaleType.FitXy);
            imageView.SetImageBitmap(bitmap);
            imageView.SetOnClickListener(new OnCapturedImageClick(this,bitmap,selectedCapturedImage));
            return imageView;
        }

        class OnCapturedImageClick : Java.Lang.Object, Android.Views.View.IOnClickListener
        {
            Bitmap imageBitmap;
            SubmitMeterTakePhotoActivity activity;
            int position;
            public OnCapturedImageClick(SubmitMeterTakePhotoActivity activityValue, Bitmap imageBitmapValue, int positionValue)
            {
                imageBitmap = imageBitmapValue;
                activity = activityValue;
                position = positionValue;
            }
            public void OnClick(View v)
            {
                activity.ShowAdjustFragment(position,imageBitmap);
            }
        }

        private void UpdateImageSelections()
        {
            LinearLayout container;
            selectedCapturedImage = mPresenter.GetMeterImages().FindIndex(model =>
            {
                return model.ImageData == null;
            });

            if (selectedCapturedImage == -1)
            {
                int totalSize = mPresenter.GetMeterImages().Count;
                ShowAdjustFragment(totalSize - 1, mPresenter.GetMeterImages()[totalSize - 1].ImageData);
            }
            else
            {
                container = (LinearLayout)meterCapturedContainer.GetChildAt(selectedCapturedImage);
                container.SetBackgroundDrawable(GetDrawable(Resource.Drawable.meter_capture_holder_selected));
            }
        }

        public void ShowGallery()
        {
            if (isGalleryFirstPress)
            {
                ShowUploadPhotoTooltip();
                isGalleryFirstPress = false;
            }
            else
            {
                Intent intent = new Intent();
                intent.SetType("image/*");
                intent.SetAction(Intent.ActionGetContent);
                StartActivityForResult(Intent.CreateChooser(intent, "Select Picture"), PickImageId);
            }
        }

        public void ShowOCRLoading()
        {
            meterReadingTakePhotoTitle.Visibility = ViewStates.Gone;
            bottomLayout.Visibility = ViewStates.Gone;
            FragmentTransaction transaction = FragmentManager.BeginTransaction();
            SetToolBarTitle("Take Photo");
            ocrLoadingFragment = OCRLoadingFragment.NewIntance();
            transaction.Replace(Resource.Id.photoContainer, ocrLoadingFragment);
            transaction.AddToBackStack(null);
            transaction.Commit();
        }

        public void ShowMeterReadingPage(string resultOCRResponseList)
        {
            Intent intent = new Intent(this,typeof(SubmitMeterReadingActivity));
            intent.PutExtra("OCR_RESULTS", resultOCRResponseList);
            StartActivity(intent);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.SSMRMeterSubmitMenu, menu);
            ssmrMenu = menu;
            return base.OnCreateOptionsMenu(menu);
        }


        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.action_ssmr_meter_reading_more:
                    ShowTakePhotoTooltip();
                    break;
            }
            return base.OnOptionsItemSelected(item);
        }

        private void ShowTakePhotoTooltip()
        {
            MaterialDialog myDiaLog = SMRPopUpUtils.OnBuildSMRPhotoTooltip(true, isSinglePhase, this);
            LinearLayout btnFirst = myDiaLog.FindViewById<LinearLayout>(Resource.Id.btnFirst);

            btnFirst.Click += delegate
            {
                myDiaLog.Dismiss();
            };

            myDiaLog.Show();
        }

        private void ShowUploadPhotoTooltip()
        {
            MaterialDialog myDiaLog = SMRPopUpUtils.OnBuildSMRPhotoTooltip(false, isSinglePhase, this);
            LinearLayout btnFirst = myDiaLog.FindViewById<LinearLayout>(Resource.Id.btnFirst);

            btnFirst.Click += delegate
            {
                ShowGallery();
                myDiaLog.Dismiss();
            };

            myDiaLog.Show();
        }

        public void EnableMoreMenu()
        {
            ssmrMenu.FindItem(Resource.Id.action_ssmr_meter_reading_more).SetVisible(true);
        }

        public void DisableMoreMenu()
        {
            ssmrMenu.FindItem(Resource.Id.action_ssmr_meter_reading_more).SetVisible(false);
        }
    }
}
