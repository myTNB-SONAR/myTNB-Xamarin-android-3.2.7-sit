using AFollestad.MaterialDialogs;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.Preferences;
using Android.Provider;
using Android.Runtime;
using Android.Text;
using Android.Views;
using Android.Widget;
using AndroidX.CoordinatorLayout.Widget;
using AndroidX.Core.Content;
using AndroidX.RecyclerView.Widget;
using CheeseBind;
using Google.Android.Material.Snackbar;
using Google.Android.Material.TextField;
using Java.Text;
using Java.Util;
using myTNB_Android.Src.AppLaunch.Models;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Base.Models;
using myTNB_Android.Src.Base.Request;
using myTNB_Android.Src.Feedback_Login_Others.Adapter;
using myTNB_Android.Src.Feedback_Login_Others.MVP;
using myTNB_Android.Src.FeedbackFail.Activity;
using myTNB_Android.Src.FeedbackSuccess.Activity;
using myTNB_Android.Src.SelectFeedbackType.Activity;
using myTNB_Android.Src.Utils;
using System;
using System.Collections.Generic;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace myTNB_Android.Src.Feedback_Login_Others.Activity
{
    [Activity(Label = "@string/feedback_others_activity_title"
      , ScreenOrientation = ScreenOrientation.Portrait
              , WindowSoftInputMode = SoftInput.AdjustPan
      , Theme = "@style/Theme.Others")]
    public class FeedbackLoginOthersActivity : BaseToolbarAppCompatActivity, FeedbackLoginOthersContract.IView, View.IOnTouchListener
    {


        [BindView(Resource.Id.txtInputLayoutFeedbackType)]
        TextInputLayout txtInputLayoutFeedbackType;

        [BindView(Resource.Id.txtInputLayoutFeedback)]
        TextInputLayout txtInputLayoutFeedback;

        [BindView(Resource.Id.txtInputLayoutMobileNo)]
        TextInputLayout txtInputLayoutMobileNo;


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

        [BindView(Resource.Id.txtMobileNo)]
        EditText txtMobileNo;

        [BindView(Resource.Id.recyclerView)]
        RecyclerView recyclerView;

        [BindView(Resource.Id.btnSubmit)]
        Button btnSubmit;

        FeedbackLoginOthersImageRecyclerAdapter adapter;

        GridLayoutManager layoutManager;

        FeedbackLoginOthersContract.IUserActionsListener userActionsListener;
        FeedbackLoginOthersPresenter mPresenter;

        MaterialDialog submitDialog;

        FeedbackType currentFeedbackType;

        [BindView(Resource.Id.rootView)]
        CoordinatorLayout rootView;

        public override Boolean ShowCustomToolbarTitle()
        {
            return true;
        }
        protected override void OnCreate(Android.OS.Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Intent intent = Intent;
            SetToolBarTitle(Intent.GetStringExtra("TITLE"));
            submitDialog = new MaterialDialog.Builder(this)
                .Title(Resource.String.feedback_submit_dialog_title)
                .Content(Resource.String.feedback_submit_dialog_message)
                .Progress(true, 0)
                .Cancelable(false)
                .Build();

            TextViewUtils.SetMuseoSans300Typeface(txtInputLayoutFeedbackType, txtInputLayoutFeedback, txtInputLayoutMobileNo);
            TextViewUtils.SetMuseoSans300Typeface(txtMaxImageContent, txtFeedbackType, txtFeedback, txtRelatedScreenshotTitle, txtMaxCharacters, txtMobileNo);
            TextViewUtils.SetMuseoSans500Typeface(btnSubmit);


            txtMaxCharacters.TextSize = TextViewUtils.GetFontSize(9f);
            txtRelatedScreenshotTitle.TextSize = TextViewUtils.GetFontSize(9f);
            txtMaxImageContent.TextSize = TextViewUtils.GetFontSize(9f);


            adapter = new FeedbackLoginOthersImageRecyclerAdapter(true);
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
            //txtFeedbackType.AddTextChangedListener(new InputFilterFormField(txtFeedbackType, txtInputLayoutFeedbackType));
            txtMobileNo.AddTextChangedListener(new InputFilterFormField(txtMobileNo, txtInputLayoutMobileNo));

            mPresenter = new FeedbackLoginOthersPresenter(this);

            //txtFeedbackType.SetOnTouchListener(this);

            this.userActionsListener.Start();

            if (string.IsNullOrEmpty(txtMobileNo.Text))
            {
                txtMobileNo.Append("+60");
            }
            txtMobileNo.SetFilters(new Android.Text.IInputFilter[] { new InputFilterPhoneNumber() });

            txtMobileNo.TextChanged += TextChanged;
            txtFeedback.TextChanged += TextChanged;
            txtFeedback.SetOnTouchListener(this);
            txtInputLayoutFeedback.Error = GetString(Resource.String.feedback_total_character_left);
        }

        private void TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            FeedBackCharacCount();

            string feedback = txtFeedback.Text;
            if (txtInputLayoutMobileNo.Visibility == ViewStates.Visible)
            {
                string mobile_no = txtMobileNo.Text.Trim();
                this.userActionsListener.CheckRequiredFields(mobile_no, feedback);
            }
            else
            {
                this.userActionsListener.CheckRequiredFields(feedback);
            }


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
            return Resource.Layout.FeedbackLoginOthersView;
        }

        [OnClick(Resource.Id.btnSubmit)]
        void OnSubmit(object sender, EventArgs e)
        {
            try
            {
                btnSubmit.Enabled = false;
                Android.OS.Handler h = new Android.OS.Handler();
                Action myAction = () =>
                {
                    btnSubmit.Enabled = true;
                };
                h.PostDelayed(myAction, 3000);

                string feedback = txtFeedback.Text.Trim();
                List<AttachedImage> attachedImages = adapter.GetAllImages();
                if (txtInputLayoutMobileNo.Visibility == ViewStates.Visible)
                {
                    string mobile_no = txtMobileNo.Text.Trim();
                    this.userActionsListener.OnSubmit(this.DeviceId(), mobile_no, currentFeedbackType, feedback, attachedImages);
                }
                else
                {
                    this.userActionsListener.OnSubmit(this.DeviceId(), currentFeedbackType, feedback, attachedImages);
                }
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }

        [OnClick(Resource.Id.feedbackTypeLayout)]
        void OnSelectStateLayout(object sender, EventArgs eventArgs)
        {
            this.userActionsListener.OnSelectFeedbackType();
        }

        [OnClick(Resource.Id.txtInputLayoutFeedbackType)]
        void OnSelectStateLayout1(object sender, EventArgs eventArgs)
        {
            this.userActionsListener.OnSelectFeedbackType();
        }

        protected override void OnResume()
        {
            base.OnResume();
            try
            {
                FirebaseAnalyticsUtils.SetScreenName(this, "Submit Feedback");
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
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
            string[] items = { Utility.GetLocalizedLabel("FeedbackForm", "takePhoto")  ,
                            Utility.GetLocalizedLabel("FeedbackForm", "chooseFromLibrary") ,
                            Utility.GetLocalizedCommonLabel("cancel")};

            AlertDialog.Builder builder = new AlertDialog.Builder(this)
                .SetTitle(Utility.GetLocalizedLabel("FeedbackForm", "selectOptions"));
            builder.SetItems(items, (lsender, args) =>
            {



                if (items[args.Which].Equals(Utility.GetLocalizedLabel("FeedbackForm", "takePhoto")))
                {
                    this.userActionsListener.OnAttachPhotoCamera();
                }
                else if (items[args.Which].Equals(Utility.GetLocalizedLabel("FeedbackForm", "chooseFromLibrary")))
                {
                    this.userActionsListener.OnAttachPhotoGallery();
                }
                else if (items[args.Which].Equals(Utility.GetLocalizedCommonLabel("cancel")))
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
                                            ApplicationContext.PackageName + ".fileprovider", file);
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

        public void SetPresenter(FeedbackLoginOthersContract.IUserActionsListener userActionListener)
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
            return Task.Run<string>(() =>
            {
                return FileUtils.ProcessGalleryImage(this, selectedImage, pTempImagePath, pFileName);
            });

        }

        public void ShowLoadingImage()
        {
            try
            {
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
            try
            {
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

        public void UpdateAdapter(string pFilePath, string pFileName)
        {
            try
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
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }


        private void FeedBackCharacCount()
        {
            try
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
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
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
            txtInputLayoutFeedback.SetErrorTextAppearance(TextViewUtils.SelectedFontSize() == "L" ? Resource.Style.TextInputLayoutFeedbackCountLarge : Resource.Style.TextInputLayoutFeedbackCount);
            txtInputLayoutFeedback.Error = null;
            TextViewUtils.SetMuseoSans300Typeface(txtInputLayoutFeedback.FindViewById<TextView>(Resource.Id.textinput_error));
            TextViewUtils.SetMuseoSans300Typeface(txtInputLayoutFeedback);
            FeedBackCharacCount();
        }

        public void ShowProgressDialog()
        {
            try
            {
                LoadingOverlayUtils.OnRunLoadingAnimation(this);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void HideProgressDialog()
        {
            try
            {
                LoadingOverlayUtils.OnStopLoadingAnimation(this);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
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
            string feedback = txtFeedback.Text.Trim();

            if (TextUtils.IsEmpty(feedback))
            {
                DisableSubmitButton();
                ShowEmptyFeedbackError();
                return;
            }
            else
            {
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
            txtFeedback.Text = "";
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

        public void ShowMobileNo()
        {
            this.txtInputLayoutMobileNo.Visibility = ViewStates.Visible;
        }

        public void HideMobileNo()
        {
            this.txtInputLayoutMobileNo.Visibility = ViewStates.Gone;
        }

        public void ShowEmptyMobileNoError()
        {
            txtInputLayoutMobileNo.Error = Utility.GetLocalizedErrorLabel("invalid_mobileNumber");
        }

        public void ShowInvalidMobileNoError()
        {
            txtInputLayoutMobileNo.Error = Utility.GetLocalizedErrorLabel("invalid_mobileNumber");
        }

        public void ClearMobileNoError()
        {
            txtInputLayoutMobileNo.Error = null;
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
                message = Utility.GetLocalizedErrorLabel("defaultErrorMessage");
            }

            mErrorMessageSnackBar = Snackbar.Make(rootView, message, Snackbar.LengthIndefinite)
            .SetAction(Utility.GetLocalizedCommonLabel("close"), delegate { mErrorMessageSnackBar.Dismiss(); }
            );
            View v = mErrorMessageSnackBar.View;
            TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
            tv.SetMaxLines(5);

            mErrorMessageSnackBar.Show();
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
