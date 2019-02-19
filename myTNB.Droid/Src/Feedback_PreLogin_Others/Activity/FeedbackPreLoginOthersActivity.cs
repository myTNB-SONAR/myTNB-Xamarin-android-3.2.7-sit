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
using Android.Content.PM;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Feedback_PreLogin_Others.MVP;
using Android.Net;
using myTNB_Android.Src.Base.Models;
using myTNB_Android.Src.Base.Request;
using System.Threading.Tasks;
using CheeseBind;
using Android.Support.Design.Widget;
using Android.Support.V7.Widget;
using myTNB_Android.Src.Feedback_PreLogIn_Others.Adapter;
using AFollestad.MaterialDialogs;
using myTNB_Android.Src.Utils;
using myTNB_Android.Src.AppLaunch.Models;
using Android.Provider;
using Android.Graphics;
using myTNB_Android.Src.FeedbackSuccess.Activity;
using myTNB_Android.Src.FeedbackFail.Activity;
using System.Web;
using Android.Support.V4.Content;
using myTNB_Android.Src.SelectFeedbackType.Activity;
using Java.Text;
using Java.Util;
using Android.Preferences;
using Android.Text;
using myTNB_Android.Src.Utils.Custom.ProgressDialog;

namespace myTNB_Android.Src.Feedback_PreLogin_Others.Activity
{
    [Activity(Label = "@string/feedback_others_activity_title"
      , ScreenOrientation = ScreenOrientation.Portrait
              , WindowSoftInputMode = SoftInput.AdjustPan
      , Theme = "@style/Theme.Others")]
    public class FeedbackPreLoginOthersActivity : BaseToolbarAppCompatActivity , FeedbackPreLoginOthersContract.IView, View.IOnTouchListener
    {

        [BindView(Resource.Id.txtInputLayoutFullName)]
        TextInputLayout txtInputLayoutFullName;

        [BindView(Resource.Id.txtInputLayoutEmail)]
        TextInputLayout txtInputLayoutEmail;

        [BindView(Resource.Id.txtInputLayoutMobileNo)]
        TextInputLayout txtInputLayoutMobileNo;

        [BindView(Resource.Id.txtInputLayoutFeedbackType)]
        TextInputLayout txtInputLayoutFeedbackType;

        [BindView(Resource.Id.txtInputLayoutFeedback)]
        TextInputLayout txtInputLayoutFeedback;

        [BindView(Resource.Id.txtFullName)]
        EditText txtFullName;

        [BindView(Resource.Id.txtEmail)]
        EditText txtEmail;

        [BindView(Resource.Id.txtMobileNo)]
        EditText txtMobileNo;

        [BindView(Resource.Id.txtFeedbackType)]
        EditText txtFeedbackType;

        [BindView(Resource.Id.txtFeedback)]
        EditText txtFeedback;

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

        [BindView(Resource.Id.rootView)]
        FrameLayout rootView;

        FeedbackPreLoginOthersImageRecyclerAdapter adapter;

        GridLayoutManager layoutManager;

        FeedbackPreLoginOthersContract.IUserActionsListener userActionsListener;
        FeedbackPreLoginOthersPresenter mPresenter;

        MaterialDialog submitDialog;
        LoadingOverlay loadingOverlay;

        FeedbackType currentFeedbackType;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            submitDialog = new MaterialDialog.Builder(this)
                .Title(Resource.String.feedback_submit_dialog_title)
                .Content(Resource.String.feedback_submit_dialog_message)
                .Progress(true, 0)
                .Cancelable(false)
                .Build();

            TextViewUtils.SetMuseoSans300Typeface(txtInputLayoutFullName, txtInputLayoutEmail, txtInputLayoutMobileNo, txtInputLayoutFeedbackType, txtInputLayoutFeedback);
            TextViewUtils.SetMuseoSans300Typeface(txtMaxImageContent,txtFullName, txtEmail, txtMobileNo, txtFeedbackType, txtFeedback, txtRelatedScreenshotTitle , txtMaxCharacters);
            TextViewUtils.SetMuseoSans500Typeface(btnSubmit);

            adapter = new FeedbackPreLoginOthersImageRecyclerAdapter(true);
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



            txtFullName.AddTextChangedListener(new InputFilterFormField(txtFullName, txtInputLayoutFullName));
            txtEmail.AddTextChangedListener(new InputFilterFormField(txtEmail, txtInputLayoutEmail));
            txtMobileNo.AddTextChangedListener(new InputFilterFormField(txtMobileNo, txtInputLayoutMobileNo));
            txtFeedback.AddTextChangedListener(new InputFilterFormField(txtFeedback, txtInputLayoutFeedback));            

            mPresenter = new FeedbackPreLoginOthersPresenter(this);
            //txtFeedbackType.SetOnTouchListener(this);
            this.userActionsListener.Start();

            if (string.IsNullOrEmpty(txtMobileNo.Text))
            {
                txtMobileNo.Append("+60");
            }
            txtMobileNo.SetFilters(new Android.Text.IInputFilter[] { new InputFilterPhoneNumber() });

            txtFullName.TextChanged += TxtFullName_TextChanged;
            txtEmail.TextChanged += TxtEmail_TextChanged;
            txtMobileNo.TextChanged += TxtMobileNo_TextChanged;
            txtFeedback.TextChanged += TxtFeedback_TextChanged;
            txtFeedback.SetOnTouchListener(this);
            txtInputLayoutFeedback.Error = GetString(Resource.String.feedback_total_character_left);

        }

        [Preserve]
        private void TxtFeedback_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            string fullname = txtFullName.Text.Trim();
            string email = txtEmail.Text.Trim();
            string mobile_no = txtMobileNo.Text.Trim();
            string feedback = txtFeedback.Text;

            FeedBackCharacCount();

            this.userActionsListener.CheckRequiredFields(fullname, mobile_no, email, feedback);
        }

        [Preserve]
        private void TxtMobileNo_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            string fullname = txtFullName.Text.Trim();
            string email = txtEmail.Text.Trim();
            string mobile_no = txtMobileNo.Text.Trim();
            string feedback = txtFeedback.Text;
            this.userActionsListener.CheckRequiredFields(fullname, mobile_no, email, feedback);
        }

        [Preserve]
        private void TxtEmail_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            string fullname = txtFullName.Text.Trim();
            string email = txtEmail.Text.Trim();
            string mobile_no = txtMobileNo.Text.Trim();
            string feedback = txtFeedback.Text;
            this.userActionsListener.CheckRequiredFields(fullname, mobile_no, email, feedback);
        }

        [Preserve]
        private void TxtFullName_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            string fullname = txtFullName.Text.Trim();
            string email = txtEmail.Text.Trim();
            string mobile_no = txtMobileNo.Text.Trim();
            string feedback = txtFeedback.Text;
            this.userActionsListener.CheckRequiredFields(fullname , mobile_no , email , feedback);
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
                if (eTxtView.Id == Resource.Id.txtFeedbackType)
                {

                    if (e.Action == MotionEventActions.Up)
                    {
                        if (e.RawX >= (txtFeedbackType.Right - txtFeedbackType.GetCompoundDrawables()[DRAWABLE_RIGHT].Bounds.Width()))
                        {
                            this.userActionsListener.OnSelectFeedbackType();
                            return true;
                        }
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

        public override int ResourceId()
        {
            return Resource.Layout.FeedbackPreLoginOthersView;
        }


        public override bool ShowCustomToolbarTitle()
        {
            return true;
        }

        [OnClick(Resource.Id.btnSubmit)]
        void OnSubmit(object sender, EventArgs e)
        {
            btnSubmit.Enabled = false;
            Handler h = new Handler();
            Action myAction = () =>
            {
                btnSubmit.Enabled = true;
            };
            h.PostDelayed(myAction, 3000);

            string fullname = txtFullName.Text.Trim();
            string mobile_no = txtMobileNo.Text.Trim();
            string email = txtEmail.Text.Trim();
            string feedback = txtFeedback.Text.Trim();
            List<AttachedImage> attachedImages = adapter.GetAllImages();


            this.userActionsListener.OnSubmit(this.DeviceId(), fullname, mobile_no, email, currentFeedbackType, feedback, attachedImages);
        }



        [OnClick(Resource.Id.stateLayout)]
        void OnSelectStateLayout(object sender, EventArgs eventArgs)
        {
            this.userActionsListener.OnSelectFeedbackType();
        }

        [OnClick(Resource.Id.txtInputLayoutFeedbackType)]
        void OnSelectStateLayout1(object sender, EventArgs eventArgs)
        {
            this.userActionsListener.OnSelectFeedbackType();
        }


        [OnClick(Resource.Id.txtFeedbackType)]
        void OnSelectStateLayout2(object sender, EventArgs eventArgs)
        {
            this.userActionsListener.OnSelectFeedbackType();
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

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            this.userActionsListener.OnActivityResult(requestCode, resultCode, data);


        }



        public override bool StoragePermissionRequired()
        {
            return true;
        }

        public override bool CameraPermissionRequired()
        {
            return true;
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

        public void SetPresenter(FeedbackPreLoginOthersContract.IUserActionsListener userActionListener)
        {
            this.userActionsListener = userActionListener;
        }

        public bool IsActive()
        {
            return Window.DecorView.RootView.IsShown;
        }

        public void ShowMaximumAttachPhotosAllowed()
        {
            throw new NotImplementedException();
        }

        public string GetTemporaryImageFilePath(string pFolder, string pFileName)
        {
            return FileUtils.GetTemporaryImageFilePath(this, pFolder, pFileName);
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

        public void ShowLoadingImage()
        {
            int position = adapter.ItemCount - 1;
            AttachedImage attachImage = adapter.GetItemObject(position);
            if (attachImage != null && attachImage.ViewType == Constants.VIEW_TYPE_DUMMY_RECORD)
            {
                attachImage.IsLoading = true;
                adapter.Update(position, attachImage);
            }
        }

        public void HideLoadingImage()
        {
            int position = adapter.ItemCount - 1;
            AttachedImage attachImage = adapter.GetItemObject(position);
            if (attachImage != null && attachImage.ViewType == Constants.VIEW_TYPE_DUMMY_RECORD)
            {
                attachImage.IsLoading = false;
                adapter.Update(position, attachImage);
            }
        }

        public void UpdateAdapter(string pFilePath, string pFileName)
        {
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

        public void ShowEmptyFullnameError()
        {
            txtInputLayoutFullName.Error = GetString(Resource.String.bill_related_feedback_empty_fullname_error);
        }

        public void ShowEmptyMobileNoError()
        {
            txtInputLayoutMobileNo.Error = GetString(Resource.String.bill_related_feedback_empty_mobile_error);
        }

        public void ShowInvalidMobileNoError()
        {
            txtInputLayoutMobileNo.Error = GetString(Resource.String.bill_related_feedback_invalid_mobile_error);
        }

        public void ShowEmptyEmaiError()
        {
            txtInputLayoutEmail.Error = GetString(Resource.String.bill_related_feedback_empty_email_error);
        }

        public void ShowInvalidEmailError()
        {
            txtInputLayoutEmail.Error = GetString(Resource.String.bill_related_feedback_invalid_email_error);
        }


        public void ShowEmptyFeedbackError()
        {
             txtInputLayoutFeedback.SetErrorTextAppearance(Resource.Style.TextErrorAppearance);
            TextViewUtils.SetMuseoSans300Typeface(txtInputLayoutFeedback.FindViewById<TextView>(Resource.Id.textinput_error));
            TextViewUtils.SetMuseoSans300Typeface(txtInputLayoutFeedback);
            txtInputLayoutFeedback.Error = GetString(Resource.String.bill_related_feedback_empty_feedback_error);
        }

        public void ClearErrors()
        {
            txtInputLayoutFeedback.SetErrorTextAppearance(Resource.Style.TextInputLayoutFeedbackCount);
            txtInputLayoutFullName.Error = null;
            txtInputLayoutMobileNo.Error = null;
            txtInputLayoutEmail.Error = null;
            txtInputLayoutFeedback.Error = null;
            TextViewUtils.SetMuseoSans300Typeface(txtInputLayoutFeedback.FindViewById<TextView>(Resource.Id.textinput_error));
            TextViewUtils.SetMuseoSans300Typeface(txtInputLayoutFeedback);
            FeedBackCharacCount();
        }

        private void FeedBackCharacCount()
        {

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

        public void ShowProgressDialog()
        {
            //if (submitDialog != null && !submitDialog.IsShowing)
            //{
            //    submitDialog.Show();
            //}
            if (loadingOverlay != null && loadingOverlay.IsShowing)
            {
                loadingOverlay.Dismiss();
            }

            loadingOverlay = new LoadingOverlay(this, Resource.Style.LoadingOverlyDialogStyle);
            loadingOverlay.Show();
        }

        public void HideProgressDialog()
        {
            //if (submitDialog != null && submitDialog.IsShowing)
            //{
            //    submitDialog.Dismiss();
            //}
            if (loadingOverlay != null && loadingOverlay.IsShowing)
            {
                loadingOverlay.Dismiss();
            }
        }

        public void ShowSuccess(string date, string feedbackId, int imageCount)
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

        public void EnableSubmitButton()
        {
            string fullname = txtFullName.Text.Trim();
            string email = txtEmail.Text.Trim();
            string mobile_no = txtMobileNo.Text.Trim();
            string feedback = txtFeedback.Text.Trim();

            if (TextUtils.IsEmpty(fullname) || TextUtils.IsEmpty(email) || TextUtils.IsEmpty(mobile_no))
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

        public void DisableSubmitButton()
        {
            btnSubmit.Enabled = false;
            btnSubmit.Background = ContextCompat.GetDrawable(this, Resource.Drawable.silver_chalice_button_background);
        }

        public override void Ready()
        {
            FileUtils.CreateDirectory(this, FileUtils.TEMP_IMAGE_FOLDER);
        }

        public void ClearInputFields()
        {
            txtEmail.Text = "";
            txtFeedback.Text = "";
            txtFullName.Text = "";
            txtMobileNo.Text = "";
            adapter.ClearAll();
            adapter.Add(new AttachedImage()
            {
                ViewType = Constants.VIEW_TYPE_DUMMY_RECORD
            });
        }


        public void ShowSelectedFeedbackType(FeedbackType feedbackType)
        {
            this.currentFeedbackType = feedbackType;
            txtFeedbackType.Text = currentFeedbackType.FeedbackTypeName;
        }

        public void ShowSelectFeedbackType()
        {
            // TODO : SHOW NEW ACTIVITY FOR FEEDBACK TYPE SELECTION
            var selectStateIntent = new Intent(this, typeof(SelectFeedbackTypeActivity));
            StartActivityForResult(selectStateIntent, Constants.SELECT_FEEDBACK_TYPE);
        }

        public string GetImageName(int itemCount)
        {
            SimpleDateFormat dateFormatter = new SimpleDateFormat("yyyyMMdd");
            Calendar calendar = Calendar.GetInstance(Locale.Default);
            return GetString(Resource.String.feedback_image_name_convention, dateFormatter.Format(calendar.TimeInMillis), UserSessions.GetCurrentImageCount(PreferenceManager.GetDefaultSharedPreferences(this)) + itemCount);
        }


        public void ShowNameError() {
            txtInputLayoutFullName.Error = GetString(Resource.String.name_error);
        }

        Snackbar mErrorMessageSnackBar;
        public void OnSubmitError(string message = null)
        {
            if (mErrorMessageSnackBar != null && mErrorMessageSnackBar.IsShown)
            {
                mErrorMessageSnackBar.Dismiss();
            }


            if (string.IsNullOrEmpty(message))
            {
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

    }
}