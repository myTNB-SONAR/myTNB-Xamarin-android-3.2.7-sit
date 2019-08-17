using System;
using System.Collections.Generic;
using AFollestad.MaterialDialogs;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using CheeseBind;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;

namespace myTNB_Android.Src.SSMR.SubmitMeterReading.MVP
{
    [Activity(Label = "Take Photo", ScreenOrientation = ScreenOrientation.Portrait, Theme = "@style/Theme.Dashboard")]
    public class SubmitMeterTakePhotoActivity : BaseToolbarAppCompatActivity, SubmitMeterTakePhotoContract.IView
    {
        public SubmitMeterTakePhotoContract.IPresenter mPresenter;
        string contractNumber = "";
        private IMenu ssmrMenu;
        public static readonly int PickImageId = 1000;
        private static bool isGalleryFirstPress = true;
        private static bool isTakePhotFirstEnter = true;
        private bool isSinglePhase = false;
        List<MeterValidation> validatedMeterList;
        List<MeterCapturedData> meteredCapturedDataList;
        List<PhotoContainerBox> photoContainerBoxes;
        PhotoContainerBox selectedPhotoBox;
        SubmitMeterTakePhotoFragment takePhotoFragment;


        [BindView(Resource.Id.meter_capture_container)]
        LinearLayout meterCapturedContainer;

        [BindView(Resource.Id.bottomLayout)]
        LinearLayout bottomLayout;

        [BindView(Resource.Id.btnSubmitPhotoToOCR)]
        Button btnSubmitPhotoToOCR;

        [BindView(Resource.Id.btnDeletePhoto)]
        Button btnDeletePhoto;



        public override int ResourceId()
        {
            return Resource.Layout.SubmitMeterTakePhotoLayout;
        }

        public override bool ShowCustomToolbarTitle()
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
                validatedMeterList = validationStateList.FindAll(validatedMeter => { return validatedMeter.validated == false; });
                MeterCapturedData meteredCapturedData;
                meteredCapturedDataList = new List<MeterCapturedData>();
                if (validatedMeterList.Count == 0) //means all validated but should still able to take picture
                {
                    validatedMeterList = validationStateList;
                }
                foreach (MeterValidation nonValidatedMeter in validatedMeterList)
                {
                    meteredCapturedData = new MeterCapturedData();
                    meteredCapturedData.meterId = nonValidatedMeter.meterId;
                    meteredCapturedDataList.Add(meteredCapturedData);
                }
                meteredCapturedDataList[0].isSelected = true; //For initial selection;
                mPresenter.InitializeModelList(meteredCapturedDataList.Count);
                if (!isSinglePhase)
                {
                    CreatePhotoBoxContainer();
                }
            }

            if (isSinglePhase)
            {
                meterCapturedContainer.Visibility = ViewStates.Gone;
            }
            else
            {
                meterCapturedContainer.Visibility = ViewStates.Visible;
            }

            if (savedInstanceState == null)
            {
                takePhotoFragment = SubmitMeterTakePhotoFragment.NewInstance();
                FragmentManager.BeginTransaction().Replace(Resource.Id.photoContainer, takePhotoFragment).Commit();
            }
            btnSubmitPhotoToOCR.Click += delegate
            {
                mPresenter.GetMeterReadingOCRValue(contractNumber);
            };

            btnDeletePhoto.Click += delegate
            {
                DeleteCapturedImage();
            };

            isTakePhotFirstEnter = true;

            if (isTakePhotFirstEnter)
            {
                ShowTakePhotoTooltip();
            }
            EnableSubmitButton();
            TextViewUtils.SetMuseoSans500Typeface(btnDeletePhoto, btnSubmitPhotoToOCR);
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

        public override void OnBackPressed()
        {
            base.OnBackPressed();
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (requestCode == PickImageId && data != null)
            {
                Bitmap bitmap = MediaStore.Images.Media.GetBitmap(this.ContentResolver, data.Data);
                AddCapturedImage(bitmap);
            }
        }

        protected override void OnStart()
        {
            base.OnStart();
            UpdateTakePhotoNote();
        }

        private void ShowOCRLoadingScreen()
        {
            FrameLayout cameraContainer = FindViewById<FrameLayout>(Resource.Id.photoContainer);
            cameraContainer.Visibility = ViewStates.Gone;

            RelativeLayout previewContainer = FindViewById<RelativeLayout>(Resource.Id.photoPreview);
            previewContainer.Visibility = ViewStates.Gone;

            LinearLayout ocrLoadingScreen = FindViewById<LinearLayout>(Resource.Id.ocrLoadingScreen);
            ocrLoadingScreen.Visibility = ViewStates.Visible;
        }

        private void ShowImagePreView(bool isShown)
        {
            FrameLayout cameraContainer = FindViewById<FrameLayout>(Resource.Id.photoContainer);
            cameraContainer.Visibility = isShown ? ViewStates.Gone : ViewStates.Visible;

            RelativeLayout previewContainer = FindViewById<RelativeLayout>(Resource.Id.photoPreview);
            previewContainer.Visibility = isShown ? ViewStates.Visible : ViewStates.Gone;

            if (isShown)
            {
                LinearLayout cropContainer = FindViewById<LinearLayout>(Resource.Id.cropAreaContainerPreview);
                cropContainer.Alpha = 0.8f;
                cropContainer.AddView(new CropAreaPreView(this));

                TextView adjustPhotoNote = FindViewById<TextView>(Resource.Id.adjust_photo_note);
                adjustPhotoNote.Text = GetString(Resource.String.ssmr_single_adjust_photo_note);
                TextViewUtils.SetMuseoSans300Typeface(adjustPhotoNote);
                adjustPhotoNote.BringToFront();

                btnDeletePhoto.Visibility = ViewStates.Visible;
            }
            else
            {
                SetToolBarTitle("Take Photo");
                btnDeletePhoto.Visibility = ViewStates.Gone;
            }
        }

        public void CreatePhotoBoxContainer()
        {
            LinearLayout container = meterCapturedContainer;
            photoContainerBoxes = new List<PhotoContainerBox>();
            container.RemoveAllViews();
            int meterCardLength = meteredCapturedDataList.Count;

            for (int i=1; i <= meterCardLength; i++)
            {
                PhotoContainerBox photoContainerBox = new PhotoContainerBox(this, i);
                photoContainerBox.UpdateBackground();
                container.AddView(photoContainerBox);
                photoContainerBoxes.Add(photoContainerBox);
            }

            UpdateAllPhotoBoxes();
            SetPhotoBoxClickable();
        }

        public void AddCapturedImage(Bitmap capturedImage)
        {
            PhotoContainerBox photoContainerBox = photoContainerBoxes.Find(box => { return box.mIsActive; });
            photoContainerBox.SetPhotoImage(capturedImage);
            UpdateAllPhotoBoxes();
            SetPhotoBoxClickable();
            UpdateTakePhotoNote();
            EnableSubmitButton();
        }

        public void SetPhotoBoxClickable()
        {
            for (int i = 0; i < photoContainerBoxes.Count; i++)
            {
                PhotoContainerBox photoBox = photoContainerBoxes[i];
                photoBox.UpdateBackground();
                photoBox.Clickable = true;
                if (photoBox.mIsActive)
                {
                    photoBox.Click += delegate {
                        ShowImagePreView(false);
                    };
                }
                if (photoBox.mHasPhoto)
                {
                    photoBox.Click += delegate {
                        ShowImagePreView(true);
                        ImageView previewImage = FindViewById<ImageView>(Resource.Id.adjust_photo_preview);
                        previewImage.SetImageBitmap(photoBox.photoBitmap);
                        selectedPhotoBox = photoBox;
                    };
                }
                if (!photoBox.mHasPhoto && !photoBox.mIsActive)
                {
                    photoBox.Clickable = false;
                }
            }
        }

        public void UpdateAllPhotoBoxes()
        {
            PhotoContainerBox photoContainerBox = photoContainerBoxes.Find(box => { return (!box.mIsActive && !box.mHasPhoto); });
            if (photoContainerBox != null)
            {
                photoContainerBox.SetActive(true);
            }
            else
            {
                ShowImagePreView(true);
                ImageView previewImage = FindViewById<ImageView>(Resource.Id.adjust_photo_preview);
                previewImage.SetImageBitmap(photoContainerBoxes[0].photoBitmap);
                selectedPhotoBox = photoContainerBoxes[0];
            }
        }

        public void DeleteCapturedImage()
        {
            selectedPhotoBox.DeletePhotoImage();
            ShowImagePreView(false);

            for (int i = 0; i < photoContainerBoxes.Count; i++)
            {
                PhotoContainerBox photoBox = photoContainerBoxes[i];
                if (!photoBox.mHasPhoto)
                {
                    photoBox.SetActive(false);
                }
            }

            SetPhotoBoxClickable();

            PhotoContainerBox photoContainerBox = photoContainerBoxes.Find(box => { return (!box.mIsActive && !box.mHasPhoto); });
            if (photoContainerBox != null)
            {
                photoContainerBox.SetActive(true);
            }

            SetPhotoBoxClickable();
            UpdateTakePhotoNote();
            EnableSubmitButton();
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
            bottomLayout.Visibility = ViewStates.Gone;
            ShowOCRLoadingScreen();
        }

        public void ShowMeterReadingPage(string resultOCRResponseList)
        {
            Intent intent = new Intent();
            intent.PutExtra("OCR_RESULTS", resultOCRResponseList);
            SetResult(Result.Ok, intent);
            Finish();
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

        public void EnableSubmitButton()
        {
            int hasImage = photoContainerBoxes.FindIndex(box => { return box.mHasPhoto; });
            if (hasImage != -1)
            {
                btnSubmitPhotoToOCR.Enabled = true;
                btnSubmitPhotoToOCR.Background = GetDrawable(Resource.Drawable.green_button_background);
            }
            else
            {
                btnSubmitPhotoToOCR.Enabled = false;
                btnSubmitPhotoToOCR.Background = GetDrawable(Resource.Drawable.silver_chalice_button_background);
            }
        }

        public void UpdateTakePhotoNote()
        {

            List<PhotoContainerBox> photoBoxes = photoContainerBoxes.FindAll(box => { return !box.mHasPhoto; });

            if (photoBoxes.Count == photoContainerBoxes.Count)
            {
                takePhotoFragment.UpdateTakePhotoNote("There will be 3 different units. Take a photo of the 1st unit you see.");
            }
            else
            {
                takePhotoFragment.UpdateTakePhotoNote("Great! Now take a photo of the next unit you see.");
            }
            
        }

        public class CropAreaPreView : View
        {
            public Rect cropAreaRect;
            public CropAreaPreView(Context context) : base(context)
            {
            }

            public CropAreaPreView(Context context, IAttributeSet attrs) : base(context, attrs)
            {
            }

            public CropAreaPreView(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
            {
            }

            protected override void OnDraw(Canvas canvas)
            {
                base.OnDraw(canvas);

                Paint rectPaint = new Paint(PaintFlags.AntiAlias);
                rectPaint.Color = Color.ParseColor("#49494a");
                rectPaint.SetStyle(Paint.Style.Fill);
                canvas.DrawPaint(rectPaint);

                rectPaint.SetXfermode(new PorterDuffXfermode(PorterDuff.Mode.Clear));
                int height = canvas.Height;
                int width = canvas.Width;
                int left = (int)(width - (width * .809));
                int top = (int)(height - (height * .70));
                int right = (int)(width - (width * .191));
                int bottom = (int)(height - (height * .25));
                cropAreaRect = new Rect(0, top, width, bottom);
                canvas.DrawRect(cropAreaRect, rectPaint);

                rectPaint.SetXfermode(null);
                rectPaint.Color = Color.White;
                rectPaint.SetStyle(Paint.Style.Stroke);
                rectPaint.StrokeWidth = 10;
                canvas.DrawRoundRect(-10,top,width+10,bottom,0,0,rectPaint);
            }

            public Rect GetCropAreaRect()
            {
                return cropAreaRect;
            }
        }
    }
}
