﻿using System;
using System.Collections.Generic;
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
using Newtonsoft.Json;

namespace myTNB_Android.Src.SSMR.SubmitMeterReading.MVP
{
    [Activity(Label = "Take Photo", Theme = "@style/Theme.Dashboard")]
    public class SubmitMeterTakePhotoActivity : BaseToolbarAppCompatActivity, SubmitMeterTakePhotoContract.IView
    {
        Button btnGetMeterReadingOCR;
        public SubmitMeterTakePhotoContract.IPresenter mPresenter;
        const string IMAGE_ID = "MYTNBAPP_SSMR_OCR_KWH_001";
        string contractNumber = "";

        int selectedCapturedImage = 0;

        private IMenu ssmrMenu;

        [BindView(Resource.Id.meterReadingTakePhotoTitle)]
        TextView meterReadingTakePhotoTitle;

        [BindView(Resource.Id.meter_capture_container)]
        LinearLayout meterCapturedContainer;

        [BindView(Resource.Id.bottomLayout)]
        LinearLayout bottomLayout;

        [BindView(Resource.Id.meter_capture_container)]
        LinearLayout meterCapturePhotoContainer;

        public static readonly int PickImageId = 1000;

        private static bool isGalleryFirstPress = true;

        private static bool isTakePhotFirstEnter = true;

        private bool isSinglePhase = false;

        SubmitMeterAdjustPhotoFragment adjustPhotoFragment;
        OCRLoadingFragment ocrLoadingFragment;
        List<MeterValidation> validatedMeterList;
        List<MeterCapturedData> meteredCapturedDataList;

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

            Bundle extras = Intent.Extras;
            if (extras != null && extras.ContainsKey("IS_SINGLE_PHASE"))
            {
                isSinglePhase = extras.GetBoolean("IS_SINGLE_PHASE");
            }

            if (extras != null && extras.ContainsKey("CONTRACT_NUMBER"))
            {
                contractNumber = extras.GetString("CONTRACT_NUMBER");
            }

            if (extras != null && extras.ContainsKey("REQUEST_PHOTOS"))
            {
                List<MeterValidation> validationStateList = JsonConvert.DeserializeObject<List<MeterValidation>>(extras.GetString("REQUEST_PHOTOS"));
                validatedMeterList = validationStateList.FindAll(validatedMeter => {return validatedMeter.validated == false;});
                MeterCapturedData meteredCapturedData;
                meteredCapturedDataList = new List<MeterCapturedData>();
                foreach (MeterValidation nonValidatedMeter in validatedMeterList)
                {
                    meteredCapturedData = new MeterCapturedData();
                    meteredCapturedData.meterId = nonValidatedMeter.meterId;
                    meteredCapturedDataList.Add(meteredCapturedData);
                }
                meteredCapturedDataList[0].isSelected = true; //For initial selection;
                CreateImageHolders();
            }

            if (isSinglePhase)
            {
                meterCapturePhotoContainer.Visibility = ViewStates.Gone;
            }
            else
            {
                meterCapturePhotoContainer.Visibility = ViewStates.Visible;
            }

            if (savedInstanceState == null)
            {
                FragmentManager.BeginTransaction().Replace(Resource.Id.photoContainer, SubmitMeterTakePhotoFragment.NewInstance()).Commit();
            }

            btnGetMeterReadingOCR = FindViewById<Button>(Resource.Id.btnSubmitPhotoToOCR);
            btnGetMeterReadingOCR.Click += delegate
            {
                mPresenter.GetMeterReadingOCRValue(contractNumber);
            };

            isTakePhotFirstEnter = true;

            if (isTakePhotFirstEnter)
            {
                ShowTakePhotoTooltip();
            }

            mPresenter.InitializeModelList();
        }

        public void UpdateCapturedBorder()
        {
            for (int i=0; i < meteredCapturedDataList.Count; i++)
            {
                //Set default border first
                meterCapturedContainer.GetChildAt(i).SetBackgroundDrawable(GetDrawable(Resource.Drawable.meter_capture_holder_inactive));

                //Set border with Image already
                if (meteredCapturedDataList[i].hasImage)
                {
                    meterCapturedContainer.GetChildAt(i).SetBackgroundDrawable(GetDrawable(Resource.Drawable.meter_capture_holder_active));
                }

                //Set next active
                int nextSelectedItem = meteredCapturedDataList.FindIndex(capturedMeter => {
                    return !capturedMeter.hasImage;
                });
                if (nextSelectedItem >= 0)
                {
                    meterCapturedContainer.GetChildAt(nextSelectedItem).SetBackgroundDrawable(GetDrawable(Resource.Drawable.meter_capture_holder_selected));
                    meteredCapturedDataList[nextSelectedItem].isSelected = true;
                }
            }
        }

        private void ShowImagePreView(bool isShown)
        {
            FrameLayout cameraContainer = FindViewById<FrameLayout>(Resource.Id.photoContainer);
            cameraContainer.Visibility = isShown ? ViewStates.Gone : ViewStates.Visible;

            RelativeLayout previewContainer = FindViewById<RelativeLayout>(Resource.Id.photoPreview);
            previewContainer.Visibility = isShown ? ViewStates.Visible : ViewStates.Gone;
        }

        public class OnContainerClickListener : Java.Lang.Object, View.IOnClickListener
        {
            int containerPosition;
            SubmitMeterTakePhotoActivity mActivity;
            SubmitMeterTakePhotoPresenter mPresenter;
            public OnContainerClickListener(SubmitMeterTakePhotoActivity activity, int position)
            {
                containerPosition = position;
                mActivity = activity;
                mPresenter = (SubmitMeterTakePhotoPresenter)activity.mPresenter;
            }
            public void OnClick(View v)
            {
                Bitmap selectedImage = mPresenter.GetMeterImages()[containerPosition - 1].ImageData;
                bool isSelected = mActivity.meteredCapturedDataList[containerPosition - 1].isSelected;
                bool hasImage = mActivity.meteredCapturedDataList[containerPosition - 1].hasImage;
                if (hasImage)
                {
                    ImageView previewImage = mActivity.FindViewById<ImageView>(Resource.Id.adjust_photo_preview);
                    previewImage.SetImageBitmap(selectedImage);
                    mActivity.ShowImagePreView(true);
                }
                else
                {
                    if (isSelected)
                    {
                        mActivity.ShowImagePreView(false);
                    }
                }
                mActivity.UpdateCapturedBorder();
            }
        }

        public void CreateImageHolders()
        {
            LinearLayout container = meterCapturedContainer;
            container.RemoveAllViews();
            int holderText = 1;
            float scale = Resources.DisplayMetrics.Density;

            foreach (MeterCapturedData meterCapturedData in meteredCapturedDataList)
            {
                int size = (int)(52 * scale + 0.5f);
                LinearLayout imageHolderContainer = new LinearLayout(this);
                LinearLayout.LayoutParams containerParams = new LinearLayout.LayoutParams(size, size);
                containerParams.SetMargins(10,0,10,0);
                imageHolderContainer.LayoutParameters = containerParams;
                imageHolderContainer.SetBackgroundDrawable(GetDrawable(Resource.Drawable.meter_capture_holder_inactive));
                imageHolderContainer.SetOnClickListener(new OnContainerClickListener(this,holderText));

                TextView imageHolderLabel = new TextView(this);
                LinearLayout.LayoutParams imageHolderParams = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, LinearLayout.LayoutParams.MatchParent);
                imageHolderLabel.LayoutParameters = imageHolderParams;
                imageHolderLabel.Gravity = GravityFlags.Center;
                imageHolderLabel.Text = holderText++.ToString();
                imageHolderContainer.AddView(imageHolderLabel);
                container.AddView(imageHolderContainer);
            }
            UpdateCapturedBorder();
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
            DisableMoreMenu();
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
        }

        public void UpdateActivesBorders()
        {
            int position = 0;
            foreach (MeterImageModel imageModel in mPresenter.GetMeterImages())
            {
                if (imageModel.ImageData != null)
                {
                    meterCapturedContainer.GetChildAt(position).SetBackgroundDrawable(GetDrawable(Resource.Drawable.meter_capture_holder_active));
                }
                position++;
            }
        }

        public void AddCapturedImage(Bitmap capturedImage)
        {
            int nextSelectedPosition = meteredCapturedDataList.FindIndex(meterCapturedData => { return !meterCapturedData.hasImage; });

            if (isSinglePhase)
            {
                mPresenter.AddMeterImageAt(nextSelectedPosition, contractNumber, IMAGE_ID, capturedImage);
                ShowAdjustFragment(nextSelectedPosition, capturedImage);
            }
            else
            {
                LinearLayout container = (LinearLayout)meterCapturedContainer.GetChildAt(nextSelectedPosition);
                container.RemoveAllViews();
                container.AddView(CreateImageView(capturedImage));
                mPresenter.AddMeterImageAt(nextSelectedPosition, contractNumber, IMAGE_ID, capturedImage);
                meteredCapturedDataList[nextSelectedPosition].hasImage = true;
                UpdateCapturedBorder();
            }
        }

        public void DeleteCapturedImage()
        {
            mPresenter.RemoveMeterImageAt(selectedCapturedImage);
            DeleteCapturedImageInContainer();
        }

        private void AddCapturedImageInContainer(Bitmap capturedImage)
        {
            int nextSelectedPosition = meteredCapturedDataList.FindIndex(meterCapturedData => { return !meterCapturedData.hasImage; });
            LinearLayout container = (LinearLayout)meterCapturedContainer.GetChildAt(nextSelectedPosition);
            container.RemoveAllViews();
            container.AddView(CreateImageView(capturedImage));
            mPresenter.AddMeterImageAt(nextSelectedPosition, contractNumber, IMAGE_ID, capturedImage);
            meteredCapturedDataList[nextSelectedPosition].hasImage = true;
            UpdateCapturedBorder();
        }

        private void DeleteCapturedImageInContainer()
        {
            LinearLayout container = (LinearLayout)meterCapturedContainer.GetChildAt(selectedCapturedImage);
            container.SetBackgroundDrawable(GetDrawable(Resource.Drawable.meter_capture_holder_inactive));
            container.RemoveViewAt(0);
            //UpdateImageSelections();
            UpdateCapturedBorder();
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
            List<MeterImageModel> modelList = mPresenter.GetMeterImages();
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
                container = (LinearLayout)meterCapturedContainer.GetChildAt(selectedCapturedImage-1);
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
            //Intent intent = new Intent(this,typeof(SubmitMeterReadingActivity));
            //intent.PutExtra("OCR_RESULTS", resultOCRResponseList);
            //StartActivityForResult(intent,7200);

            Intent intent = new Intent();
            intent.PutExtra("OCR_RESULTS", resultOCRResponseList);
            SetResult(Result.Ok, intent);
            Finish();
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
                isTakePhotFirstEnter = false;
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
