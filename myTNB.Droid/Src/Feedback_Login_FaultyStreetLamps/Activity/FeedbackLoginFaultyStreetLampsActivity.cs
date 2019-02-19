﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using myTNB_Android.Src.Base.Activity;
using Android.Content.PM;
using CheeseBind;
using Android.Support.Design.Widget;
using Android.Support.V7.Widget;
using myTNB_Android.Src.Utils;
using myTNB_Android.Src.Feedback_Login_FaultyStreetLamps.MVP;
using Android.Net;
using myTNB_Android.Src.AppLaunch.Models;
using myTNB_Android.Src.Base.Models;
using myTNB_Android.Src.Base.Request;
using System.Threading.Tasks;
using myTNB_Android.Src.Feedback_PreLogin_FaultyStreetLamps.Adapter;
using myTNB_Android.Src.Feedback_Login_FaultyStreetLamps.Adapter;
using AFollestad.MaterialDialogs;
using Android.Text;
using Android.Support.V4.Content;
using Android.Graphics;
using System.Web;
using Android.Provider;
using myTNB_Android.Src.FeedbackSuccess.Activity;
using myTNB_Android.Src.FeedbackFail.Activity;
using myTNB_Android.Src.SelectFeedbackState.Activity;
using Java.Text;
using Java.Util;
using Android.Preferences;
using myTNB_Android.Src.Utils.Custom.ProgressDialog;
using System.Runtime;

namespace myTNB_Android.Src.Feedback_Login_FaultyStreetLamps.Activity
{
    [Activity(Label = "@string/faulty_street_lamps_activity_title"
      , ScreenOrientation = ScreenOrientation.Portrait
              , WindowSoftInputMode = SoftInput.AdjustPan
      , Theme = "@style/Theme.FaultyStreetLamps")]
    public class FeedbackLoginFaultyStreetLampsActivity : BaseToolbarAppCompatActivity , FeedbackLoginFaultyStreetLampsContract.IView, View.IOnTouchListener
    {

        [BindView(Resource.Id.rootView)]
        CoordinatorLayout rootView;

        [BindView(Resource.Id.txtFeedbackTitle)]
        TextView txtFeedbackTitle;

        [BindView(Resource.Id.txtFeedbackContent)]
        TextView txtFeedbackContent;


        [BindView(Resource.Id.txtInputLayoutState)]
        TextInputLayout txtInputLayoutState;

        [BindView(Resource.Id.txtInputLayoutLocation)]
        TextInputLayout txtInputLayoutLocation;

        [BindView(Resource.Id.txtInputLayoutPoleNo)]
        TextInputLayout txtInputLayoutPoleNo;

        [BindView(Resource.Id.txtInputLayoutFeedback)]
        TextInputLayout txtInputLayoutFeedback;

        [BindView(Resource.Id.txtInputLayoutMobileNo)]
        TextInputLayout txtInputLayoutMobileNo;

        [BindView(Resource.Id.txtState)]
        EditText txtState;

        [BindView(Resource.Id.txtLocation)]
        EditText txtLocation;

        [BindView(Resource.Id.txtPoleNo)]
        EditText txtPoleNo;

        [BindView(Resource.Id.txtFeedback)]
        EditText txtFeedback;


        [BindView(Resource.Id.txtMobileNo)]
        EditText txtMobileNo;

        [BindView(Resource.Id.txtRelatedScreenshotTitle)]
        TextView txtRelatedScreenshotTitle;

        [BindView(Resource.Id.txtMaxImageContent)]
        TextView txtMaxImageContent;


        [BindView(Resource.Id.txtMaxCharacters)]
        TextView txtMaxCharacters;

        [BindView(Resource.Id.recyclerView)]
        RecyclerView recyclerView;

        [BindView(Resource.Id.btnSubmit)]
        Button btnSubmit;


        MaterialDialog submitDialog;
        FeedbackLoginFaultyStreetLampsRecyclerAdapter adapter;
        GridLayoutManager layoutManager;
        LoadingOverlay loadingOverlay;

        FeedbackLoginFaultyStreetLampsContract.IUserActionsListener userActionsListener;
        FeedbackLoginFaultyStreetLampsPresenter mPresenter;
        FeedbackState currentFeedbackState;


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            submitDialog = new MaterialDialog.Builder(this)
                .Title(Resource.String.feedback_submit_dialog_title)
                .Content(Resource.String.feedback_submit_dialog_message)
                .Progress(true, 0)
                .Cancelable(false)
                .Build();
            // Create your application here
            TextViewUtils.SetMuseoSans300Typeface(txtMobileNo, txtMaxImageContent, txtRelatedScreenshotTitle, txtFeedbackContent, txtState, txtLocation, txtPoleNo, txtFeedback , txtMaxCharacters);
            TextViewUtils.SetMuseoSans300Typeface(txtInputLayoutMobileNo, txtInputLayoutFeedback, txtInputLayoutLocation, txtInputLayoutPoleNo, txtInputLayoutState);
            TextViewUtils.SetMuseoSans500Typeface(txtFeedbackTitle, btnSubmit);

#if DEBUG
            //txtLocation.Text = "Jalan Timur";
            //txtPoleNo.Text = "17493 8E 1";
            //txtFeedback.Text = GetString(Resource.String.bill_related_feedback_text);
#endif


            if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.N)
            {
                txtFeedbackContent.TextFormatted = Html.FromHtml(GetString(Resource.String.faulty_street_lamps_txt_content), Html.FromHtmlModeLegacy);
            }
            else
            {
                txtFeedbackContent.TextFormatted = Html.FromHtml(GetString(Resource.String.faulty_street_lamps_txt_content));
            }


            adapter = new FeedbackLoginFaultyStreetLampsRecyclerAdapter(true);
            adapter.Insert(new Base.Models.AttachedImage()
            {
                ViewType = Constants.VIEW_TYPE_DUMMY_RECORD
            });
            layoutManager = new GridLayoutManager(this, Constants.GRID_IMAGE_COUNT);
            recyclerView.SetLayoutManager(layoutManager);
            recyclerView.SetAdapter(adapter);

            adapter.AddClickEvent += Adapter_AddClickEvent;
            adapter.RemoveClickEvent += Adapter_RemoveClickEvent;



            //txtMobileNo.FocusChange += (object sender, View.FocusChangeEventArgs e) =>
            //{
            //    if (e.HasFocus)
            //    {
            //        if (string.IsNullOrEmpty(txtMobileNo.Text))
            //        {
            //            txtMobileNo.Append("+60");
            //        }
            //    }
            //};

            txtFeedback.AddTextChangedListener(new InputFilterFormField(txtFeedback, txtInputLayoutFeedback));
            txtMobileNo.AddTextChangedListener(new InputFilterFormField(txtMobileNo, txtInputLayoutMobileNo));
            txtPoleNo.AddTextChangedListener(new InputFilterFormField(txtPoleNo, txtInputLayoutPoleNo));
            txtLocation.AddTextChangedListener(new InputFilterFormField(txtLocation, txtInputLayoutLocation));
            //txtState.AddTextChangedListener(new InputFilterFormField(txtState, txtInputLayoutState));

            txtState.EnableClick();

            //txtState.Text = Constants.SELECT_STATE;

            txtLocation.SetOnTouchListener(this);
            //txtState.SetOnTouchListener(this);

            mPresenter = new FeedbackLoginFaultyStreetLampsPresenter(this);
            this.userActionsListener.Start();

            if (string.IsNullOrEmpty(txtMobileNo.Text))
            {
                txtMobileNo.Append("+60");
            }
            txtMobileNo.SetFilters(new Android.Text.IInputFilter[] { new InputFilterPhoneNumber() });

            txtMobileNo.TextChanged += TextChanged;
            txtLocation.TextChanged += TextChanged;
            txtFeedback.TextChanged += TextChanged;
            txtFeedback.SetOnTouchListener(this);
            txtInputLayoutFeedback.Error = GetString(Resource.String.feedback_total_character_left);

        }
        [Preserve]
        private void TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                string location = txtLocation.Text.Trim();
                string feedback = txtFeedback.Text;
                string state = txtState.Text.Trim();


                FeedBackCharacCount();

                if (txtInputLayoutMobileNo.Visibility == ViewStates.Visible)
                {
                    string mobile_no = txtMobileNo.Text.Trim();
                    this.userActionsListener.CheckRequiredFields(mobile_no, location, feedback, state);
                }
                else
                {
                    this.userActionsListener.CheckRequiredFields(location, feedback, state);
                }
            } catch(Exception ex) {
                Utility.LoggingNonFatalError(ex);
            }

        }


        private void FeedBackCharacCount() {
            try {
            string feedback = txtFeedback.Text;
            int char_count = 0;

            if (!string.IsNullOrEmpty(feedback))
            {
                char_count = feedback.Length;
            }

            if (char_count > 0)
            {
                int char_left = Constants.FEEDBACK_CHAR_LIMIT - char_count;
                txtInputLayoutFeedback.Error = char_left + " " + GetString(Resource.String.feedback_character_left);
            }
            else
            {
                txtInputLayoutFeedback.Error = GetString(Resource.String.feedback_total_character_left);
            }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public bool IsActive()
        {
            return Window.DecorView.RootView.IsShown;
        }

        public override int ResourceId()
        {
            return Resource.Layout.FeedbackLoginFaultyStreetLampsView;
        }

        public override bool ShowCustomToolbarTitle()
        {
            return true;
        }



        [OnAfterTextChanged(Resource.Id.txtLocation)]
        private void TxtLocation_AfterTextChanged(object sender, AfterTextChangedEventArgs e)
        {
            txtInputLayoutLocation.SetErrorTextAppearance(Resource.Style.TextInformationAppearance);
            txtInputLayoutLocation.Error = GetString(Resource.String.faulty_street_lamps_feedback_location_hint);
            TextViewUtils.SetMuseoSans300Typeface(txtInputLayoutLocation.FindViewById<TextView>(Resource.Id.textinput_error));
        }

        [OnAfterTextChanged(Resource.Id.txtPoleNo)]
        private void TxtPole_AfterTextChanged(object sender, AfterTextChangedEventArgs e)
        {
            txtInputLayoutPoleNo.SetErrorTextAppearance(Resource.Style.TextInformationAppearance);
            txtInputLayoutPoleNo.Error = GetString(Resource.String.faulty_street_lamps_feedback_pole_no_hint);
            TextViewUtils.SetMuseoSans300Typeface(txtInputLayoutPoleNo.FindViewById<TextView>(Resource.Id.textinput_error));
        }

        [OnAfterTextChanged(Resource.Id.txtFeedback)]
        private void TxtFeedback_AfterTextChanged(object sender, AfterTextChangedEventArgs e)
        {
            //txtInputLayoutFeedback.SetErrorTextAppearance(Resource.Style.TextInformationAppearance);
            //txtInputLayoutFeedback.Error = GetString(Resource.String.faulty_street_lamps_feedback_feedback_hint);
            TextViewUtils.SetMuseoSans300Typeface(txtInputLayoutFeedback.FindViewById<TextView>(Resource.Id.textinput_error));
        }

        public bool OnTouch(View v, MotionEvent e)
        {
            const int DRAWABLE_LEFT = 0;
            const int DRAWABLE_TOP = 1;
            const int DRAWABLE_RIGHT = 2;
            const int DRAWABLE_BOTTOM = 3;
            if (v is EditText)
            {
                EditText eTxtView = v as EditText;
                if (eTxtView.Id == Resource.Id.txtLocation)
                {

                    if (e.Action == MotionEventActions.Up)
                    {
                        if (e.RawX >= (txtLocation.Right - txtLocation.GetCompoundDrawables()[DRAWABLE_RIGHT].Bounds.Width()))
                        {
                            if (IsLocationGranted())
                            {
                                if (Android.Locations.Geocoder.IsPresent)
                                {
                                    this.userActionsListener.OnLocationRequest(new Android.Locations.Geocoder(this));
                                }
                                else
                                {
                                    this.userActionsListener.OnLocationRequest(GetString(Resource.String.google_maps_reverse_geocode_api_key));
                                }
                            }
                            else
                            {
                                InitiateLocationPermission();
                            }

                            return true;
                        }
                    }
                }
                else if (eTxtView.Id == Resource.Id.txtState)
                {
                    if (e.RawX >= (txtLocation.Right - txtLocation.GetCompoundDrawables()[DRAWABLE_RIGHT].Bounds.Width()))
                    {
                        this.userActionsListener.OnSelectFeedbackState();

                        return true;
                    }
                }
                else if (v.Id == Resource.Id.txtFeedback)
                {
                    v.Parent.RequestDisallowInterceptTouchEvent(true);
                    switch (e.Action & MotionEventActions.Mask)
                    {
                        case MotionEventActions.Up:
                            v.Parent.RequestDisallowInterceptTouchEvent(false);
                            break;
                    }
                }
            }
            return false;
        }

        private AlertDialog _ChooseDialog;
        private void Adapter_AddClickEvent(object sender, int e)
        {
            string[] items = { GetString(Resource.String.bill_related_feedback_selection_take_photo) ,
                               GetString(Resource.String.bill_related_feedback_selection_choose_from_library) ,
                               GetString(Resource.String.bill_related_feedback_selection_cancel)};

            AlertDialog.Builder builder = new AlertDialog.Builder(this)
                .SetTitle(GetString(Resource.String.bill_related_feedback_selection_option_title));
            builder.SetItems(items, (lsender, args) =>
            {



                if (items[args.Which].Equals(GetString(Resource.String.bill_related_feedback_selection_take_photo)))
                {
                    this.userActionsListener.OnAttachPhotoCamera();
                }
                else if (items[args.Which].Equals(GetString(Resource.String.bill_related_feedback_selection_choose_from_library)))
                {
                    this.userActionsListener.OnAttachPhotoGallery();
                }
                else if (items[args.Which].Equals(GetString(Resource.String.bill_related_feedback_selection_cancel)))
                {
                    _ChooseDialog.Dismiss();
                }
            }
            );
            _ChooseDialog = builder.Show();
        }
        [OnClick(Resource.Id.btnSubmit)]
        void OnSubmit(object sender, EventArgs eventArgs)
        {

            try {
            btnSubmit.Enabled = false;
            Handler h = new Handler();
            Action myAction = () =>
            {
                btnSubmit.Enabled = true;
            };
            h.PostDelayed(myAction, 3000);

            string state = txtState.Text.Trim();
            string locationName = txtLocation.Text.Trim();
            string poleNo = txtPoleNo.Text.Trim();
            string feedback = txtFeedback.Text.Trim();
            if (txtInputLayoutMobileNo.Visibility == ViewStates.Visible)
            {
                string mobile_no = txtMobileNo.Text.Trim();
                this.userActionsListener.OnSubmit(this.DeviceId(), mobile_no, currentFeedbackState, locationName, poleNo, feedback, adapter.GetAllImages());
            }
            else
            {
                this.userActionsListener.OnSubmit(this.DeviceId(), currentFeedbackState, locationName, poleNo, feedback, adapter.GetAllImages());
            }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
            
        }

        [OnClick(Resource.Id.stateLayout)]
        void OnSelectStateLayout(object sender, EventArgs eventArgs)
        {
            this.userActionsListener.OnSelectFeedbackState();
        }


        [OnClick(Resource.Id.txtInputLayoutState)]
        void OnSelectStateLayout1(object sender, EventArgs eventArgs)
        {
            this.userActionsListener.OnSelectFeedbackState();
        }

        [OnClick(Resource.Id.txtState)]
        void OnSelectStateLayout2(object sender, EventArgs eventArgs)
        {
            this.userActionsListener.OnSelectFeedbackState();
        }

        private void Adapter_RemoveClickEvent(object sender, int e)
        {
            adapter.Remove(e);
            if (adapter.GetAllImages().Count == 1 && adapter.ItemCount == 1)
            {
                adapter.Add(new AttachedImage()
                {
                    ViewType = Constants.VIEW_TYPE_DUMMY_RECORD
                });
            }
            else
            {

                if (adapter.ItemCount == 0)
                {
                    adapter.Add(new AttachedImage()
                    {
                        ViewType = Constants.VIEW_TYPE_DUMMY_RECORD
                    });
                }
            }
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            this.userActionsListener.OnActivityResult(requestCode, resultCode, data);


        }

        public void ClearErrors()
        {
            try {
            //txtInputLayoutFeedback.SetErrorTextAppearance(Resource.Style.TextErrorAppearance);
            txtInputLayoutFeedback.SetErrorTextAppearance(Resource.Style.TextInputLayoutFeedbackCount);

            txtInputLayoutLocation.SetErrorTextAppearance(Resource.Style.TextErrorAppearance);
            txtInputLayoutPoleNo.SetErrorTextAppearance(Resource.Style.TextErrorAppearance);

            TextViewUtils.SetMuseoSans300Typeface(txtInputLayoutLocation.FindViewById<TextView>(Resource.Id.textinput_error) ,
                txtInputLayoutFeedback.FindViewById<TextView>(Resource.Id.textinput_error) ,
                txtInputLayoutPoleNo.FindViewById<TextView>(Resource.Id.textinput_error));


            txtInputLayoutFeedback.Error = null;
            txtInputLayoutLocation.Error = null;
            txtInputLayoutPoleNo.Error = null;
            txtInputLayoutState.Error = null;
            txtInputLayoutMobileNo.Error = null;

            TextViewUtils.SetMuseoSans300Typeface(txtInputLayoutFeedback, txtInputLayoutLocation, txtInputLayoutPoleNo, txtInputLayoutState);

            FeedBackCharacCount();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void ClearInputFields()
        {
            txtFeedback.Text = "";
            txtLocation.Text = "";
            txtPoleNo.Text = "";
            txtMobileNo.Text = "";
        }

        public void EnableSubmitButton()
        {
            try {
            string location = txtLocation.Text.Trim();
            string feedback = txtFeedback.Text.Trim();

            if (TextUtils.IsEmpty(location))
            {
                DisableSubmitButton();
                return;
            } else if (TextUtils.IsEmpty(feedback)) {
                DisableSubmitButton();
                ShowEmptyFeedbackError();
                return;
            } else {
                ClearErrors();
            }

            btnSubmit.Enabled = true;
            btnSubmit.Background = ContextCompat.GetDrawable(this, Resource.Drawable.green_button_background);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void DisableSubmitButton()
        {
            btnSubmit.Enabled = false;
            btnSubmit.Background = ContextCompat.GetDrawable(this, Resource.Drawable.silver_chalice_button_background);
        }


        public string GetTemporaryImageFilePath(string pFolder, string pFileName)
        {
            return FileUtils.GetTemporaryImageFilePath(this, pFolder, pFileName);
        }

        public void ShowLoadingImage()
        {
            try {
            int position = adapter.ItemCount - 1;
            AttachedImage attachImage = adapter.GetItemObject(position);
            if (attachImage != null && attachImage.ViewType == Constants.VIEW_TYPE_DUMMY_RECORD)
            {
                attachImage.IsLoading = true;
                adapter.Update(position, attachImage);
            }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void HideLoadingImage()
        {
            try {
            int position = adapter.ItemCount - 1;
            AttachedImage attachImage = adapter.GetItemObject(position);
            if (attachImage != null && attachImage.ViewType == Constants.VIEW_TYPE_DUMMY_RECORD)
            {
                attachImage.IsLoading = false;
                adapter.Update(position, attachImage);
            }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }


        public Task<string> SaveCameraImage(string tempImagePath, string fileName)
        {
            return Task.Run<string>(() =>
            {
                return FileUtils.ProcessCameraImage(this, tempImagePath, fileName);
            });
        }

        public Task<string> SaveGalleryImage(Android.Net.Uri selectedImage, string pTempImagePath, string pFileName)
        {
            return Task.Run<string>(() => {
                return FileUtils.ProcessGalleryImage(this, selectedImage, pTempImagePath, pFileName);
            });

        }

        public Task<AttachedImageRequest> SaveImage(AttachedImage attachedImage)
        {
            
            return Task.Run<AttachedImageRequest>(() =>
            {
                BitmapFactory.Options bmOptions = new BitmapFactory.Options();

                Bitmap bitmap = BitmapFactory.DecodeFile(attachedImage.Path, bmOptions);

                byte[] imageBytes = FileUtils.Get(this, bitmap);
                int size = imageBytes.Length;
                string hexString = HttpUtility.UrlEncode(FileUtils.ByteArrayToString(imageBytes), Encoding.UTF8);
                if (bitmap != null && !bitmap.IsRecycled)
                {
                    bitmap.Recycle();
                }
                Console.WriteLine(string.Format("Hex string {0}", hexString));
                return new AttachedImageRequest()
                {
                    ImageHex = hexString,
                    FileSize = size,
                    FileName = attachedImage.Name
                };

            });
            
        }

        public void SetPresenter(FeedbackLoginFaultyStreetLampsContract.IUserActionsListener userActionListener)
        {
            this.userActionsListener = userActionListener;
        }

        public void ShowCamera()
        {
            var intent = new Intent(MediaStore.ActionImageCapture);
            Java.IO.File file = new Java.IO.File(FileUtils.GetTemporaryImageFilePath(this, FileUtils.TEMP_IMAGE_FOLDER, string.Format("{0}.jpeg", "temporaryImage")));
            Android.Net.Uri fileUri = FileProvider.GetUriForFile(this,
                                            ApplicationContext.PackageName + ".provider", file);
            intent.PutExtra(Android.Provider.MediaStore.ExtraOutput, fileUri);
            intent.AddFlags(ActivityFlags.GrantReadUriPermission);
            StartActivityForResult(intent, Constants.REQUEST_ATTACHED_CAMERA_IMAGE);
        }

        public void ShowGallery()
        {
            Intent galleryIntent = new Intent(Intent.ActionPick, MediaStore.Images.Media.ExternalContentUri);
            galleryIntent.SetType("image/*");
            StartActivityForResult(Intent.CreateChooser(galleryIntent, GetString(Resource.String.bill_related_feedback_select_images)), Constants.RUNTIME_PERMISSION_GALLERY_REQUEST_CODE);

        }



        public void ShowSuccess(string date, string feedbackId , int imageCount)
        {
            ISharedPreferences sharedPref = PreferenceManager.GetDefaultSharedPreferences(this);
            int currentCount = UserSessions.GetCurrentImageCount(sharedPref);
            UserSessions.SetCurrentImageCount(sharedPref, currentCount + imageCount);
            Finish();
            var successIntent = new Intent(this, typeof(FeedbackSuccessActivity));
            successIntent.PutExtra(Constants.RESPONSE_FEEDBACK_DATE, date);
            successIntent.PutExtra(Constants.RESPONSE_FEEDBACK_ID, feedbackId);
            StartActivityForResult(successIntent, Constants.REQUEST_FEEDBACK_SUCCESS_VIEW);
        }

        public void ShowFail()
        {
            // TODO : IMPL FAIL SCREEN
            var failIntent = new Intent(this, typeof(FeedbackFailActivity));
            StartActivityForResult(failIntent, Constants.REQUEST_FEEDBACK_FAIL_VIEW);
        }


        public void ShowEmptyFeedbackError()
        {
            txtInputLayoutFeedback.SetErrorTextAppearance(Resource.Style.TextErrorAppearance);

            TextViewUtils.SetMuseoSans300Typeface(txtInputLayoutLocation.FindViewById<TextView>(Resource.Id.textinput_error),
                txtInputLayoutFeedback.FindViewById<TextView>(Resource.Id.textinput_error),
                txtInputLayoutPoleNo.FindViewById<TextView>(Resource.Id.textinput_error));


            txtInputLayoutFeedback.Error = null;
           

            TextViewUtils.SetMuseoSans300Typeface(txtInputLayoutFeedback, txtInputLayoutLocation, txtInputLayoutPoleNo, txtInputLayoutState);
            txtInputLayoutFeedback.Error = GetString(Resource.String.faulty_street_lamps_feedback_empty_feedback_error);
        }


        public void ShowEmptyPoleNoError()
        {
            txtInputLayoutPoleNo.Error = GetString(Resource.String.faulty_street_lamps_feedback_empty_pole_no_error);
        }

        public void ShowInvalidPoleNoError()
        {
            txtInputLayoutPoleNo.Error = GetString(Resource.String.faulty_street_lamps_feedback_invalid_pole_no_error);
        }

        public void ShowEmptyLocationError()
        {
            txtInputLayoutLocation.Error = GetString(Resource.String.faulty_street_lamps_feedback_empty_location_error);
        }


        public void ShowMaximumAttachPhotosAllowed()
        {
            throw new NotImplementedException();
        }


        public void ShowProgressDialog()
        {
            //if (submitDialog != null && !submitDialog.IsShowing)
            //{
            //    submitDialog.Show();
            //}
            try
            {
                if (loadingOverlay != null && loadingOverlay.IsShowing)
                {
                    loadingOverlay.Dismiss();
                }

                loadingOverlay = new LoadingOverlay(this, Resource.Style.LoadingOverlyDialogStyle);
                loadingOverlay.Show();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void HideProgressDialog()
        {
            //if (submitDialog != null && submitDialog.IsShowing)
            //{
            //    submitDialog.Dismiss();
            //}
            try {
            if (loadingOverlay != null && loadingOverlay.IsShowing)
            {
                loadingOverlay.Dismiss();
            }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void ShowState(FeedbackState feedbackState)
        {
            try {
            ClearErrors();
            if (feedbackState != null) {
                this.currentFeedbackState = feedbackState;
                txtState.Text = feedbackState.StateName;    
            } else {
                if (string.IsNullOrEmpty(txtState.Text.Trim())) {
                    currentFeedbackState = null;
                    ShowEmptyStateError();
                }
            }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }



        public void UpdateAdapter(string pFilePath, string pFileName)
        {
            try {
            adapter.Update(adapter.ItemCount - 1, new AttachedImage()
            {
                ViewType = Constants.VIEW_TYPE_REAL_RECORD,
                Name = pFileName,
                Path = pFilePath

            });
            if (adapter.ItemCount < 2)
            {
                adapter.Add(new AttachedImage()
                {
                    ViewType = Constants.VIEW_TYPE_DUMMY_RECORD
                });
            }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public override bool StoragePermissionRequired()
        {
            return true;
        }

        public override bool CameraPermissionRequired()
        {
            return true;
        }


        public override void Ready()
        {
            FileUtils.CreateDirectory(this, FileUtils.TEMP_IMAGE_FOLDER);
        }

        public void ShowSelectFeedbackState()
        {
            // TODO : SHOW NEW ACTIVITY FOR FEEDBACK STATE SELECTION
            var selectStateIntent = new Intent(this, typeof(SelectFeedbackStateActivity));
            StartActivityForResult(selectStateIntent, Constants.SELECT_FEEDBACK_STATE);
        }

        public override int LocationTitleRationale()
        {
            return Resource.String.faulty_street_lamps_feedback_runtime_permission_dialog_location_title;
        }

        public override int LocationContentRationale()
        {
            return Resource.String.faulty_street_lamps_feedback_runtime_permission_location_rationale;
        }

        public void ShowGeocodedLocation(string geocodeString)
        {
            txtLocation.Text = geocodeString;
        }

        public string GetImageName(int itemCount)
        {
            SimpleDateFormat dateFormatter = new SimpleDateFormat("yyyyMMdd");
            Calendar calendar = Calendar.GetInstance(Locale.Default);
            return GetString(Resource.String.feedback_image_name_convention, dateFormatter.Format(calendar.TimeInMillis), UserSessions.GetCurrentImageCount(PreferenceManager.GetDefaultSharedPreferences(this)) + itemCount);
        }

        public void ShowMobileNo()
        {
            txtInputLayoutMobileNo.Visibility = ViewStates.Visible;
        }

        public void HideMobileNo()
        {
            txtInputLayoutMobileNo.Visibility = ViewStates.Gone;
        }

        protected override void OnResume()
        {
            base.OnResume();

        }

        public void ShowEmptyMobileNoError()
        {
            txtInputLayoutMobileNo.Error = GetString(Resource.String.faulty_street_lamps_feedback_empty_mobile_error);
        }

        public void ShowInvalidMobileNoError()
        {
            txtInputLayoutMobileNo.Error = GetString(Resource.String.faulty_street_lamps_feedback_invalid_mobile_error);
        }

        public void ClearMobileNoError()
        {
            txtInputLayoutMobileNo.Error = null;
            txtInputLayoutMobileNo.Error = null;
        }


        Snackbar mErrorMessageSnackBar;
        public void OnSubmitError(string message = null)
        {
            if (mErrorMessageSnackBar != null && mErrorMessageSnackBar.IsShown)
            {
                mErrorMessageSnackBar.Dismiss();
            }

            if(string.IsNullOrEmpty(message)) {
                message = GetString(Resource.String.app_launch_http_exception_error);
            }

            mErrorMessageSnackBar = Snackbar.Make(rootView, message, Snackbar.LengthIndefinite)
            .SetAction("Close", delegate { mErrorMessageSnackBar.Dismiss(); }
            );
            View v = mErrorMessageSnackBar.View;
            TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
            tv.SetMaxLines(5);

            mErrorMessageSnackBar.Show();
        }

        public void ShowEmptyStateError()
        {
            txtInputLayoutState.Error = GetString(Resource.String.invalid_state);
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