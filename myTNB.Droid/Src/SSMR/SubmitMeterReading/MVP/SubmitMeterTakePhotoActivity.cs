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
using Com.Airbnb.Lottie;
using myTNB_Android.Src.Base;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.SSMR.SMRApplication.MVP;
using myTNB_Android.Src.SSMR.Util;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;

namespace myTNB_Android.Src.SSMR.SubmitMeterReading.MVP
{
    [Activity(Label = "Take Photo", ScreenOrientation = ScreenOrientation.Portrait, Theme = "@style/Theme.Dashboard")]
    public class SubmitMeterTakePhotoActivity : BaseToolbarAppCompatActivity, SubmitMeterTakePhotoContract.IView
    {
        public SubmitMeterTakePhotoContract.IPresenter mPresenter;
        string contractNumber = "";
        const string IMAGE_ID = "MYTNBAPP_SSMR_OCR_";
        private IMenu ssmrMenu;
        public static readonly int PickImageId = 1000;
        private static bool isGalleryFirstPress = true;
        private static bool isTakePhotFirstEnter = true;
        private bool isSinglePhase = false;
        private bool isFromSingleCapture = false;
        private string singlePhaseMeterId = "";
        private Bitmap singlePhaseImageData = null;
        public int mCropAreaHeight;
        //List<MeterValidation> validatedMeterList;
        //List<MeterValidation> validationStateList;

        List<MeterReadingModel> meterReadingModelList;
        List<MeterReadingModel> requiredMeterReadingModelList;

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

        [BindView(Resource.Id.loadingMessage)]
        TextView loadingMessage;

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
            SetTheme(TextViewUtils.SelectedFontSize() == "L" ? Resource.Style.Theme_DashboardLarge : Resource.Style.Theme_Dashboard);
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

            if (extras != null && extras.ContainsKey("REQUESTED_PHOTOS"))
            {
                meterReadingModelList = JsonConvert.DeserializeObject<List<MeterReadingModel>>(extras.GetString("REQUESTED_PHOTOS"));
                requiredMeterReadingModelList = meterReadingModelList.FindAll(model =>
                {
                    return !model.isValidated;
                });

                if (requiredMeterReadingModelList.Count == 0)
                {
                    requiredMeterReadingModelList = meterReadingModelList;
                    requiredMeterReadingModelList.ForEach(model=> {
                        model.isValidated = false;
                    });
                }
            }

            if (isSinglePhase)
            {
                meterCapturedContainer.Visibility = ViewStates.Gone;
            }
            else
            {
                meteredCapturedDataList = new List<MeterCapturedData>();
                MeterCapturedData meteredCapturedModel;
                requiredMeterReadingModelList.ForEach(meterReadingModel =>
                {
                    meteredCapturedModel = new MeterCapturedData();
                    meteredCapturedModel.meterId = meterReadingModel.meterReadingUnit.ToUpper();
                    meteredCapturedDataList.Add(meteredCapturedModel);
                });
                meterCapturedContainer.Visibility = ViewStates.Visible;
                CreatePhotoBoxContainer();
            }

            if (savedInstanceState == null)
            {
                takePhotoFragment = SubmitMeterTakePhotoFragment.NewInstance();
                SupportFragmentManager.BeginTransaction().Replace(Resource.Id.photoContainer, takePhotoFragment).Commit();
            }
            btnSubmitPhotoToOCR.Click += delegate
            {
                List<MeterImageModel> meterImageDataList = new List<MeterImageModel>();
                MeterImageModel meterImageModel;
                if (isSinglePhase)
                {
                    meterImageModel = new MeterImageModel();
                    meterImageModel.RequestReadingUnit = singlePhaseMeterId;
                    meterImageModel.ImageId = IMAGE_ID + singlePhaseMeterId;
                    meterImageModel.ImageData = singlePhaseImageData;
                    meterImageDataList.Add(meterImageModel);
                }
                else
                {
                    foreach (PhotoContainerBox containerBox in photoContainerBoxes)
                    {
                        meterImageModel = new MeterImageModel();
                        meterImageModel.RequestReadingUnit = containerBox.mMeterId;
                        meterImageModel.ImageId = IMAGE_ID + containerBox.mMeterId;
                        meterImageModel.ImageData = containerBox.photoBitmap;
                        meterImageDataList.Add(meterImageModel);
                    }
                }
                mPresenter.SetMeterImageList(meterImageDataList);
                mPresenter.GetMeterReadingOCRValue(contractNumber);
            };

            btnDeletePhoto.Click += delegate
            {
                DeleteCapturedImage();
            };

            if (!MyTNBAccountManagement.GetInstance().GetIsSMRTakePhotoOnBoardShown())
            {
                ShowTakePhotoTooltip();
                MyTNBAccountManagement.GetInstance().SetIsSMRTakePhotoOnBoardShown();
            }
            EnableSubmitButton();
            TextViewUtils.SetMuseoSans500Typeface(btnDeletePhoto, btnSubmitPhotoToOCR);
            TextViewUtils.SetMuseoSans300Typeface(loadingMessage);

            btnDeletePhoto.TextSize = TextViewUtils.GetFontSize(16f);
            btnSubmitPhotoToOCR.TextSize = TextViewUtils.GetFontSize(16f);
            loadingMessage.TextSize = TextViewUtils.GetFontSize(16f);
          

            btnDeletePhoto.Text = Utility.GetLocalizedLabel("SSMRCaptureMeter", "deletePhoto");
            btnSubmitPhotoToOCR.Text = Utility.GetLocalizedCommonLabel("submit");
            loadingMessage.Text = Utility.GetLocalizedLabel("SSMRCaptureMeter", "ocrReading");
            SetToolBarTitle(Utility.GetLocalizedLabel("SSMRCaptureMeter", "navTitleTakePhoto"));
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
                takePhotoFragment.UpdateImage(bitmap);
                singlePhaseImageData = bitmap;
            }
        }

        protected override void OnStart()
        {
            base.OnStart();
            if (meterReadingModelList.Count == requiredMeterReadingModelList.Count)
            {
                UpdateTakePhotoNote();
            }
            else
            {
                UpdateTakePhotoFormattedNote();
            }
        }

        private void ShowOCRLoadingScreen()
        {
            FrameLayout cameraContainer = FindViewById<FrameLayout>(Resource.Id.photoContainer);
            cameraContainer.Visibility = ViewStates.Gone;

            RelativeLayout previewContainer = FindViewById<RelativeLayout>(Resource.Id.photoPreview);
            previewContainer.Visibility = ViewStates.Gone;

            LinearLayout ocrLoadingScreen = FindViewById<LinearLayout>(Resource.Id.ocrLoadingScreen);
            ocrLoadingScreen.Visibility = ViewStates.Visible;

            LottieAnimationView loadingImage = ocrLoadingScreen.FindViewById<LottieAnimationView>(Resource.Id.loadingImage);

            try
            {
                loadingImage.Progress = 0f;
                loadingImage.PlayAnimation();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
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
                cropContainer.AddView(new CropAreaPreView(this, mCropAreaHeight));

                TextView adjustPhotoNote = FindViewById<TextView>(Resource.Id.adjust_photo_note);
                adjustPhotoNote.Text = Utility.GetLocalizedLabel("SSMRCaptureMeter", "editDescription");
                TextViewUtils.SetMuseoSans300Typeface(adjustPhotoNote);
                adjustPhotoNote.BringToFront();

                btnDeletePhoto.Visibility = ViewStates.Visible;
            }
            else
            {
                SetToolBarTitle(Utility.GetLocalizedLabel("SSMRCaptureMeter", "navTitleTakePhoto"));
                btnDeletePhoto.Visibility = ViewStates.Gone;
            }
        }

        public void CreatePhotoBoxContainer()
        {
            LinearLayout container = meterCapturedContainer;
            photoContainerBoxes = new List<PhotoContainerBox>();
            container.RemoveAllViews();
            int meterCardLength = meteredCapturedDataList.Count;

            for (int i=0; i < meterCardLength; i++)
            {
                PhotoContainerBox photoContainerBox = new PhotoContainerBox(this, i+1);
                photoContainerBox.SetMeterId(meteredCapturedDataList[i].meterId);
                photoContainerBox.UpdateBackground();
                container.AddView(photoContainerBox);
                photoContainerBoxes.Add(photoContainerBox);
            }

            UpdateAllPhotoBoxes();
            SetPhotoBoxClickable();
        }

        private int FixOrientation(Bitmap bitmap)
        {
            if (bitmap.Width > bitmap.Height)
            {
                return 90;
            }
            return 0;
        }

        private Bitmap FixRotatedBitmap(Bitmap sourceBitmap, int rotation)
        {
            Matrix matrix = new Matrix();
            matrix.PostRotate(rotation);
            return Bitmap.CreateBitmap(sourceBitmap,0,0,sourceBitmap.Width,sourceBitmap.Height,matrix,true);
        }

        public void AddCapturedImage(Bitmap capturedImage)
        {
            int rotation = FixOrientation(capturedImage);
            Bitmap checkedCapturedImage = capturedImage;
            if (rotation == 90)
            {
                checkedCapturedImage = FixRotatedBitmap(capturedImage,rotation);
            }
            if (!isSinglePhase)
            {
                PhotoContainerBox photoContainerBox = photoContainerBoxes.Find(box => { return box.mIsActive; });
                photoContainerBox.SetPhotoImage(checkedCapturedImage);
                UpdateAllPhotoBoxes();
                SetPhotoBoxClickable();
				takePhotoFragment.ResetZoom();
			}
            else
            {
                ScaledImageView previewImage = FindViewById<ScaledImageView>(Resource.Id.adjust_photo_preview);
                //previewImage.SetScaleType(ImageView.ScaleType.CenterCrop);
                previewImage.SetImageBitmap(checkedCapturedImage);
                ShowImagePreView(true);
                isFromSingleCapture = true;
                singlePhaseImageData = checkedCapturedImage;
            }

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
                        ScaledImageView previewImage = FindViewById<ScaledImageView>(Resource.Id.adjust_photo_preview);
                        //previewImage.SetScaleType(ImageView.ScaleType.CenterCrop);
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
                ShowImagePreView(false);
                photoContainerBox.SetActive(true);
            }
            else
            {
                ShowImagePreView(true);
                ScaledImageView previewImage = FindViewById<ScaledImageView>(Resource.Id.adjust_photo_preview);
                //previewImage.SetScaleType(ImageView.ScaleType.CenterCrop);
                previewImage.SetImageBitmap(photoContainerBoxes[0].photoBitmap);
                selectedPhotoBox = photoContainerBoxes[0];
            }
        }

        public void DeleteCapturedImage()
        {
            if (!isSinglePhase)
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

                List<PhotoContainerBox> photoBoxes = photoContainerBoxes.FindAll(box => { return !box.mHasPhoto; });
                if (photoBoxes.Count == photoContainerBoxes.Count)
                {
                    if (meterReadingModelList.Count == requiredMeterReadingModelList.Count)
                    {
                        UpdateTakePhotoNote();
                    }
                    else
                    {
                        UpdateTakePhotoFormattedNote();
                    }
                }
                else
                {
                    takePhotoFragment.UpdateTakePhotoNote(Utility.GetLocalizedLabel("SSMRCaptureMeter", "multiTakeNextPhotoDescription"));
                }
            }
            else
            {
                takePhotoFragment.ResetZoom();
                ShowImagePreView(false);
                isFromSingleCapture = false;
                UpdateTakePhotoNote();
            }
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
                isGalleryFirstPress = true;
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

        public void ShowMeterReadingPageWithError()
        {
            SetResult(Result.Ok);
            Finish();
        }

        private void ShowTakePhotoTooltip()
        {
            List<string> needMeterCaptureList = new List<string>();
            for (int i=0; i < requiredMeterReadingModelList.Count; i++)
            {
                needMeterCaptureList.Add(requiredMeterReadingModelList[i].meterReadingUnitDisplay);
            }
            string remainingMeter = needMeterCaptureList.Count > 1 ? String.Join(", ", needMeterCaptureList.ToArray()) : needMeterCaptureList[0];
            SMRPhotoPopUpDetailsModel tooltipData = MyTNBAppToolTipData.GetTakePhotoToolTipData(isSinglePhase,
                requiredMeterReadingModelList.Count == 1,
                requiredMeterReadingModelList.Count.ToString(),
                remainingMeter);
            MyTNBAppToolTipBuilder.Create(this, MyTNBAppToolTipBuilder.ToolTipType.IMAGE_HEADER)
                .SetHeaderImage(isSinglePhase ? Resource.Drawable.single_phase : Resource.Drawable.multiple_phase)
                .SetTitle(tooltipData.Title)
                .SetMessage(tooltipData.Description)
                .SetCTALabel(tooltipData.CTA)
                .Build()
                .Show();
        }

        private void ShowUploadPhotoTooltip()
        {
            List<string> needMeterCaptureList = new List<string>();
            for (int i = 0; i < requiredMeterReadingModelList.Count; i++)
            {
                needMeterCaptureList.Add(requiredMeterReadingModelList[i].meterReadingUnitDisplay);
            }
            string remainingMeter = needMeterCaptureList.Count > 1 ? String.Join(",", needMeterCaptureList.ToArray()) : needMeterCaptureList[0];
            SMRPhotoPopUpDetailsModel tooltipData = MyTNBAppToolTipData.GetUploadPhotoToolTipData(isSinglePhase,
                requiredMeterReadingModelList.Count == 1,
                requiredMeterReadingModelList.Count.ToString(),
                remainingMeter);
            MyTNBAppToolTipBuilder.Create(this, MyTNBAppToolTipBuilder.ToolTipType.IMAGE_HEADER)
                .SetHeaderImage(isSinglePhase ? Resource.Drawable.single_phase : Resource.Drawable.multiple_phase)
                .SetTitle(tooltipData.Title)
                .SetMessage(tooltipData.Description)
                .SetCTALabel(tooltipData.CTA)
                .SetCTAaction(() => { ShowGallery(); })
                .Build()
                .Show();
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
            if (!isSinglePhase)
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
            else
            {
                if (isFromSingleCapture)
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
        }

        public void UpdateTakePhotoNote()
        {
            if (!isSinglePhase)
            {
                List<PhotoContainerBox> photoBoxes = photoContainerBoxes.FindAll(box => { return !box.mHasPhoto; });

                if (photoBoxes.Count == photoContainerBoxes.Count)
                {
                    takePhotoFragment.UpdateTakePhotoNote(String.Format(Utility.GetLocalizedLabel("SSMRCaptureMeter", "multiTakePhotoDescription"), photoBoxes.Count.ToString()));
                }
                else
                {
                    takePhotoFragment.UpdateTakePhotoNote(Utility.GetLocalizedLabel("SSMRCaptureMeter", "multiTakeNextPhotoDescription"));
                }
            }
        }

        public void UpdateTakePhotoFormattedNote()
        {
            string singleDone = Utility.GetLocalizedLabel("SSMRCaptureMeter", "singularDone") + " ";
            string pluralDone = Utility.GetLocalizedLabel("SSMRCaptureMeter", "pluralDone") + " ";
            string singleOnto = Utility.GetLocalizedLabel("SSMRCaptureMeter", "singularOnto");
            string pluralOnto = Utility.GetLocalizedLabel("SSMRCaptureMeter", "pluralOnto");
            string finalString = "";

            List<string> doneUnitList = new List<string>();
            List<string> notDoneUnitList = new List<string>();

            for (int i=0; i < meterReadingModelList.Count; i++)
            {
                if (meterReadingModelList[i].isValidated)
                {
                    doneUnitList.Add(meterReadingModelList[i].meterReadingUnitDisplay);
                }
                else
                {
                    notDoneUnitList.Add(meterReadingModelList[i].meterReadingUnitDisplay);
                }
            }

            //Construct done meter reading message
            if (doneUnitList.Count > 1)
            {
                finalString = string.Format(pluralDone, "<font color='#20bd4c'>" + doneUnitList[0] + "</font>","<font color = '#20bd4c'>" + doneUnitList[1] + "</font>");
            }
            else
            {
                finalString = string.Format(singleDone, "<font color='#20bd4c'>" + doneUnitList[0] + "</font>");
            }

            //Construct not done meter reading message
            if (notDoneUnitList.Count > 1)
            {
                finalString += string.Format(pluralOnto, "<font color='#fecd39'>" + notDoneUnitList[0] + "</font>", "<font color = '#fecd39'>" + notDoneUnitList[1] + "</font>");
            }
            else
            {
                finalString += string.Format(singleOnto, "<font color='#fecd39'>" + notDoneUnitList[0] + "</font>");
            }

            takePhotoFragment.UpdateTakePhotoFormattedNote(GetFormattedText(finalString));
        }

        protected override void OnResume()
        {
            base.OnResume();
            try
            {
                FirebaseAnalyticsUtils.SetScreenName(this, "Capture Meter Reading (Camera)");
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public class CropAreaPreView : View
        {
            public Rect cropAreaRect;
            private int mCanvasHeight;
            public CropAreaPreView(Context context, int canvasHeight) : base(context)
            {
                mCanvasHeight = canvasHeight;
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
                int height = mCanvasHeight;// canvas.Height;
                int width = canvas.Width;
                int left = (int)(width - (width * .809));
                int top = (int)(height - (height * .70));
                int right = (int)(width - (width * .191));
                int bottom = (int)(height - (height * .35));
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
