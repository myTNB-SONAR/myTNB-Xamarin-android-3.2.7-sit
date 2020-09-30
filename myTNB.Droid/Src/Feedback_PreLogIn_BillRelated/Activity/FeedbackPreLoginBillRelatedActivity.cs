using AFollestad.MaterialDialogs;
using Android;
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
using AndroidX.Core.Content;
using AndroidX.RecyclerView.Widget;
using CheeseBind;
using Google.Android.Material.Snackbar;
using Google.Android.Material.TextField;
using Java.Text;
using Java.Util;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Base.Models;
using myTNB_Android.Src.Base.Request;
using myTNB_Android.Src.Common.Activity;
using myTNB_Android.Src.Common.Model;
using myTNB_Android.Src.CompoundView;
using myTNB_Android.Src.Feedback_PreLogIn_BillRelated.Adapter;
using myTNB_Android.Src.Feedback_PreLogIn_BillRelated.MVP;
using myTNB_Android.Src.FeedbackFail.Activity;
using myTNB_Android.Src.FeedbackSuccess.Activity;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace myTNB_Android.Src.Feedback_PreLogin_BillRelated.Activity
{
    [Activity(Label = "@string/bill_related_activity_title"
      , ScreenOrientation = ScreenOrientation.Portrait
              , WindowSoftInputMode = SoftInput.AdjustPan
      , Theme = "@style/Theme.FaultyStreetLamps")]
    public class FeedbackPreLoginBillRelatedActivity : BaseToolbarAppCompatActivity, FeedbackPreLoginBillRelatedContract.IView, EditText.IOnTouchListener
    {
        internal static readonly string TAG = typeof(FeedbackPreLoginBillRelatedActivity).Name;
        [BindView(Resource.Id.txtInputLayoutFullName)]
        TextInputLayout txtInputLayoutFullName;

        [BindView(Resource.Id.txtInputLayoutEmail)]
        TextInputLayout txtInputLayoutEmail;

        [BindView(Resource.Id.txtInputLayoutAccountNo)]
        TextInputLayout txtInputLayoutAccountNo;

        [BindView(Resource.Id.txtInputLayoutFeedback)]
        TextInputLayout txtInputLayoutFeedback;

        [BindView(Resource.Id.txtFullName)]
        EditText txtFullName;

        [BindView(Resource.Id.txtEmail)]
        EditText txtEmail;

        [BindView(Resource.Id.txtAccountNo)]
        EditText txtAccountNo;

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

        [BindView(Resource.Id.mobileNumberFieldContainer)]
        LinearLayout mobileNumberFieldContainer;

        FeedbackPreLoginBillRelatedImageRecyclerAdapter adapter;

        GridLayoutManager layoutManager;

        FeedbackPreLoginBillRelatedContract.IUserActionsListener userActionsListener;
        FeedbackPreLoginBillRelatedPresenter mPresenter;

        MaterialDialog submitDialog;

        [BindView(Resource.Id.rootView)]
        FrameLayout rootView;

        private MobileNumberInputComponent mobileNumberInputComponent;
        private const int COUNTRY_CODE_SELECT_REQUEST = 1;

        public override int ResourceId()
        {
            return Resource.Layout.FeedbackPreLogInBillRelatedView;
        }

        public override Boolean ShowCustomToolbarTitle()
        {
            return true;
        }

        protected override void OnPause()
        {
            base.OnPause();
        }

        protected override void OnCreate(Android.OS.Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            try
            {
                Intent intent = Intent;
                SetToolBarTitle(Intent.GetStringExtra("TITLE"));

                // Create your application here
                submitDialog = new MaterialDialog.Builder(this)
                    .Title(Resource.String.feedback_submit_dialog_title)
                    .Content(Resource.String.feedback_submit_dialog_message)
                    .Progress(true, 0)
                    .Cancelable(false)
                    .Build();

                TextViewUtils.SetMuseoSans300Typeface(txtInputLayoutFullName, txtInputLayoutEmail, txtInputLayoutAccountNo, txtInputLayoutFeedback);
                TextViewUtils.SetMuseoSans300Typeface(txtMaxImageContent, txtFullName, txtEmail, txtAccountNo, txtFeedback, txtRelatedScreenshotTitle, txtMaxCharacters);
                TextViewUtils.SetMuseoSans500Typeface(btnSubmit);

                txtInputLayoutFullName.Hint = Utility.GetLocalizedCommonLabel("fullname");
                txtInputLayoutEmail.Hint = Utility.GetLocalizedCommonLabel("email");
                txtInputLayoutAccountNo.Hint = Utility.GetLocalizedCommonLabel("accountNo");
                txtInputLayoutFeedback.Hint = Utility.GetLocalizedLabel("FeedbackForm", "feedback");

                txtRelatedScreenshotTitle.Text = Utility.GetLocalizedLabel("FeedbackForm", "attachPhotoTitle");
                txtMaxCharacters.Text = Utility.GetLocalizedLabel("FeedbackForm", "attachPhotoTitle");
                txtMaxImageContent.Text = Utility.GetLocalizedLabel("FeedbackForm", "maxFile");
                btnSubmit.Text = Utility.GetLocalizedCommonLabel("submit");

                adapter = new FeedbackPreLoginBillRelatedImageRecyclerAdapter(true);
                adapter.Insert(new Base.Models.AttachedImage()
                {
                    ViewType = Constants.VIEW_TYPE_DUMMY_RECORD
                });
                layoutManager = new GridLayoutManager(this, Constants.GRID_IMAGE_COUNT);
                recyclerView.SetLayoutManager(layoutManager);
                recyclerView.SetAdapter(adapter);

                adapter.AddClickEvent += Adapter_AddClickEvent;
                adapter.RemoveClickEvent += Adapter_RemoveClickEvent;

#if DEBUG
                //txtFullName.Text = "David Montecillo";
                //txtMobileNo.Text = "09498899648";
                //txtEmail.Text = "montecillodavid.acn@gmail.com";
                //txtAccountNo.Text = "210000193209";
                //txtFeedback.Text = GetString(Resource.String.bill_related_feedback_text);
#endif




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
                txtAccountNo.AddTextChangedListener(new InputFilterFormField(txtAccountNo, txtInputLayoutAccountNo));
                txtFeedback.AddTextChangedListener(new InputFilterFormField(txtFeedback, txtInputLayoutFeedback));

                mPresenter = new FeedbackPreLoginBillRelatedPresenter(this);
                mobileNumberFieldContainer.RemoveAllViews();
                mobileNumberInputComponent = new MobileNumberInputComponent(this);
                mobileNumberInputComponent.SetOnTapCountryCodeAction(OnTapCountryCode);
                mobileNumberInputComponent.SetMobileNumberLabel(Utility.GetLocalizedCommonLabel("mobileNo"));
                mobileNumberInputComponent.SetSelectedCountry(CountryUtil.Instance.GetDefaultCountry());
                mobileNumberInputComponent.SetValidationAction(OnValidateMobileNumber);
                mobileNumberFieldContainer.AddView(mobileNumberInputComponent);

                this.userActionsListener.Start();

                txtFullName.TextChanged += TxtFullName_TextChanged;
                txtEmail.TextChanged += TxtEmail_TextChanged;
                txtAccountNo.TextChanged += TxtAccountNo_TextChanged;
                txtFeedback.TextChanged += TxtFeedback_TextChanged;
                txtFeedback.SetOnTouchListener(this);
                txtInputLayoutFeedback.Error = string.Format(Utility.GetLocalizedCommonLabel("charactersLeft"), Constants.FEEDBACK_CHAR_LIMIT);

                //txtInputLayoutFullName.Error = null;
                txtFullName.ClearFocus();
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

        }
        [Preserve]
        private void TxtFeedback_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                string fullname = txtFullName.Text.Trim();
                string mobile_no = mobileNumberInputComponent.GetMobileNumberValue();
                string email = txtEmail.Text.Trim();
                string account_no = txtAccountNo.Text.Trim();
                string feedback = txtFeedback.Text;

                FeedBackCharacCount();

                this.userActionsListener.CheckRequiredFields(fullname, mobile_no, email, account_no, feedback);
            }
            catch (System.Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
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
                    txtInputLayoutFeedback.Error = string.Format(Utility.GetLocalizedCommonLabel("charactersLeft"), char_left);
                }
                else
                {
                    txtInputLayoutFeedback.Error = string.Format(Utility.GetLocalizedCommonLabel("charactersLeft"), Constants.FEEDBACK_CHAR_LIMIT);
                }
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        [Preserve]
        private void TxtAccountNo_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                string fullname = txtFullName.Text.Trim();
                string mobile_no = mobileNumberInputComponent.GetMobileNumberValue();
                string email = txtEmail.Text.Trim();
                string account_no = txtAccountNo.Text.Trim();
                string feedback = txtFeedback.Text;
                this.userActionsListener.CheckRequiredFields(fullname, mobile_no, email, account_no, feedback);
            }
            catch (System.Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }

        [Preserve]
        private void TxtEmail_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                string fullname = txtFullName.Text.Trim();
                string mobile_no = mobileNumberInputComponent.GetMobileNumberValue();
                string email = txtEmail.Text.Trim();
                string account_no = txtAccountNo.Text.Trim();
                string feedback = txtFeedback.Text;
                this.userActionsListener.CheckRequiredFields(fullname, mobile_no, email, account_no, feedback);
            }
            catch (System.Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }

        [Preserve]
        private void TxtMobileNo_TextChanged(object sender, TextChangedEventArgs e)
        {
            //try
            //{
            //    string checkedPhoneNumber = this.userActionsListener.OnVerfiyCellularCode(e.Text.ToString());
            //    if (checkedPhoneNumber != txtMobileNo.Text)
            //    {
            //        txtMobileNo.Text = checkedPhoneNumber;
            //    }

            //    string fullname = txtFullName.Text.Trim();
            //    string mobile_no = txtMobileNo.Text.Trim();
            //    string email = txtEmail.Text.Trim();
            //    string account_no = txtAccountNo.Text.Trim();
            //    string feedback = txtFeedback.Text;
            //    this.userActionsListener.CheckRequiredFields(fullname, mobile_no, email, account_no, feedback);
            //}
            //catch (System.Exception ex)
            //{
            //    Utility.LoggingNonFatalError(ex);
            //}
        }

        private void OnValidateMobileNumber(bool isValidated)
        {
            string fullname = txtFullName.Text.Trim();
            string mobile_no = mobileNumberInputComponent.GetMobileNumberValue();
            string email = txtEmail.Text.Trim();
            string account_no = txtAccountNo.Text.Trim();
            string feedback = txtFeedback.Text;
            this.userActionsListener.CheckRequiredFields(fullname, mobile_no, email, account_no, feedback);
        }

        [Preserve]
        private void TxtFullName_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                string fullname = txtFullName.Text.Trim();
                string mobile_no = mobileNumberInputComponent.GetMobileNumberValue();
                string email = txtEmail.Text.Trim();
                string account_no = txtAccountNo.Text.Trim();
                string feedback = txtFeedback.Text;
                this.userActionsListener.CheckRequiredFields(fullname, mobile_no, email, account_no, feedback);
            }
            catch (System.Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }

        [OnClick(Resource.Id.btnSubmit)]
        void OnSubmit(object sender, EventArgs e)
        {
            try
            {
                if (!this.GetIsClicked())
                {
                    this.SetIsClicked(true);
                    btnSubmit.Enabled = false;
                    Android.OS.Handler h = new Android.OS.Handler();
                    Action myAction = () =>
                    {
                        btnSubmit.Enabled = true;
                    };
                    h.PostDelayed(myAction, 3000);

                    string fullname = txtFullName.Text.Trim();
                    string mobile_no = mobileNumberInputComponent.GetMobileNumberValueWithISDCode();
                    string email = txtEmail.Text.Trim();
                    string account_no = txtAccountNo.Text.Trim();
                    string feedback = txtFeedback.Text.Trim();
                    List<AttachedImage> attachedImages = adapter.GetAllImages();


                    this.userActionsListener.OnSubmit(this.DeviceId(), fullname, mobile_no, email, account_no, feedback, attachedImages);
                }
            }
            catch (System.Exception ex)
            {
                this.SetIsClicked(false);
                Utility.LoggingNonFatalError(ex);
            }
        }

        private void Adapter_RemoveClickEvent(object sender, int e)
        {
            try
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
            catch (System.Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }

        private AlertDialog _ChooseDialog;
        private void Adapter_AddClickEvent(object sender, int e)
        {
            try
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
            catch (System.Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (requestCode == COUNTRY_CODE_SELECT_REQUEST)
            {
                if (resultCode == Result.Ok)
                {
                    string dataString = data.GetStringExtra(Constants.SELECT_COUNTRY_CODE);
                    Country selectedCountry = JsonConvert.DeserializeObject<Country>(dataString);
                    mobileNumberInputComponent.SetSelectedCountry(selectedCountry);
                }
            }
            else
            {
                this.userActionsListener.OnActivityResult(requestCode, resultCode, data);
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

        protected override void OnResume()
        {
            base.OnResume();
            try
            {
                FirebaseAnalyticsUtils.SetScreenName(this, "Pre Login Submit Feedback");
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void ShowCamera()
        {
            if (!this.GetIsClicked())
            {
                Permission cameraPermission = ContextCompat.CheckSelfPermission(this, Manifest.Permission.Camera);
                if (cameraPermission == (int)Permission.Granted)
                {
                    this.SetIsClicked(true);
                    var intent = new Intent(MediaStore.ActionImageCapture);
                    Java.IO.File file = new Java.IO.File(FileUtils.GetTemporaryImageFilePath(this, FileUtils.TEMP_IMAGE_FOLDER, string.Format("{0}.jpeg", "temporaryImage")));
                    Android.Net.Uri fileUri = FileProvider.GetUriForFile(this,
                                                    ApplicationContext.PackageName + ".fileprovider", file);
                    intent.PutExtra(Android.Provider.MediaStore.ExtraOutput, fileUri);
                    intent.AddFlags(ActivityFlags.GrantReadUriPermission);
                    StartActivityForResult(intent, Constants.REQUEST_ATTACHED_CAMERA_IMAGE);
                }
            }
        }

        public void ShowGallery()
        {
            if (!this.GetIsClicked())
            {
                this.SetIsClicked(true);
                Intent galleryIntent = new Intent(Intent.ActionPick, MediaStore.Images.Media.ExternalContentUri);
                galleryIntent.SetType("image/*");
                StartActivityForResult(Intent.CreateChooser(galleryIntent, GetString(Resource.String.bill_related_feedback_select_images)), Constants.RUNTIME_PERMISSION_GALLERY_REQUEST_CODE);
            }
        }

        public void SetPresenter(FeedbackPreLoginBillRelatedContract.IUserActionsListener userActionListener)
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
            catch (System.Exception e)
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
            catch (System.Exception e)
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
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void ShowEmptyFullnameError()
        {
            txtInputLayoutFullName.Error = Utility.GetLocalizedErrorLabel("invalid_fullname");
        }

        public void ShowEmptyMobileNoError()
        {
            //txtInputLayoutMobileNo.Error = Utility.GetLocalizedErrorLabel("invalid_mobileNumber");
        }

        public void ShowInvalidMobileNoError()
        {
            //txtInputLayoutMobileNo.Error = Utility.GetLocalizedErrorLabel("invalid_mobileNumber");
        }

        public void ShowEmptyEmaiError()
        {
            txtInputLayoutEmail.Error = Utility.GetLocalizedErrorLabel("invalid_email");
        }

        public void ShowInvalidEmailError()
        {
            txtInputLayoutEmail.Error = Utility.GetLocalizedErrorLabel("invalid_email");
        }

        public void ShowEmptyAccountNoError()
        {
            txtInputLayoutAccountNo.Error = Utility.GetLocalizedErrorLabel("accountLength");
        }

        public void ShowInvalidAccountNoError()
        {
            txtInputLayoutAccountNo.Error = Utility.GetLocalizedErrorLabel("accountLength");
        }

        public void ShowInValidAccountNumber()
        {
            txtInputLayoutAccountNo.Error = Utility.GetLocalizedErrorLabel("accountLength");

        }

        public void ShowEmptyFeedbackError()
        {
            txtInputLayoutFeedback.SetErrorTextAppearance(Resource.Style.TextErrorAppearance);
            TextViewUtils.SetMuseoSans300Typeface(txtInputLayoutFeedback.FindViewById<TextView>(Resource.Id.textinput_error));
            TextViewUtils.SetMuseoSans300Typeface(txtInputLayoutFeedback);
            txtInputLayoutFeedback.Error = Utility.GetLocalizedLabel("FeedbackForm", "invalidFeedback");
        }

        public void ClearErrors()
        {
            txtInputLayoutFeedback.SetErrorTextAppearance(Resource.Style.TextInputLayoutFeedbackCount);
            txtInputLayoutFullName.Error = null;
            //txtInputLayoutMobileNo.Error = null;
            txtInputLayoutEmail.Error = null;
            txtInputLayoutAccountNo.Error = null;
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
            string fullname = txtFullName.Text.Trim();
            string mobile_no = mobileNumberInputComponent.GetMobileNumberValueWithISDCode();
            string email = txtEmail.Text.Trim();
            string account_no = txtAccountNo.Text.Trim();
            string feedback = txtFeedback.Text.Trim();
            try
            {
                if (TextUtils.IsEmpty(fullname) || TextUtils.IsEmpty(mobile_no) || TextUtils.IsEmpty(email) || TextUtils.IsEmpty(account_no) || TextUtils.IsEmpty(feedback))
                {
                    if (TextUtils.IsEmpty(feedback))
                    {
                        ShowEmptyFeedbackError();
                    }

                    DisableSubmitButton();
                    return;
                }

                else
                {
                    ClearErrors();
                }

                btnSubmit.Enabled = true;
                btnSubmit.Background = ContextCompat.GetDrawable(this, Resource.Drawable.green_button_background);
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
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
            txtAccountNo.Text = "";
            txtEmail.Text = "";
            txtFeedback.Text = "";
            txtFullName.Text = "";
            //txtMobileNo.Text = "";
            adapter.ClearAll();
            adapter.Add(new AttachedImage()
            {
                ViewType = Constants.VIEW_TYPE_DUMMY_RECORD
            });
        }

        public string GetImageName(int itemCount)
        {
            SimpleDateFormat dateFormatter = new SimpleDateFormat("yyyyMMdd");
            Calendar calendar = Calendar.GetInstance(Locale.Default);
            return GetString(Resource.String.feedback_image_name_convention, dateFormatter.Format(calendar.TimeInMillis), UserSessions.GetCurrentImageCount(PreferenceManager.GetDefaultSharedPreferences(this)) + itemCount);
        }

        public bool OnTouch(View v, MotionEvent e)
        {
            if (v.Id == Resource.Id.txtFeedback)
            {
                v.Parent.RequestDisallowInterceptTouchEvent(true);
                switch (e.Action & MotionEventActions.Mask)
                {
                    case MotionEventActions.Up:
                        v.Parent.RequestDisallowInterceptTouchEvent(false);
                        break;
                }
            }
            return false;
        }


        public void ShowNameError()
        {
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
                message = Utility.GetLocalizedErrorLabel("defaultErrorMessage");
            }

            mErrorMessageSnackBar = Snackbar.Make(rootView, message, Snackbar.LengthIndefinite)
            .SetAction(Utility.GetLocalizedCommonLabel("close"), delegate { mErrorMessageSnackBar.Dismiss(); }
            );
            View v = mErrorMessageSnackBar.View;
            TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
            tv.SetMaxLines(5);

            mErrorMessageSnackBar.Show();
            this.SetIsClicked(false);
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

        public void ReplaceAccountNum(string account_no)
        {
            try
            {
                txtAccountNo.Text = account_no;
                txtAccountNo.SetSelection(account_no.Length);
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        private void OnTapCountryCode()
        {
            Intent intent = new Intent(this, typeof(SelectCountryActivity));
            StartActivityForResult(intent, COUNTRY_CODE_SELECT_REQUEST);
        }
    }
}
