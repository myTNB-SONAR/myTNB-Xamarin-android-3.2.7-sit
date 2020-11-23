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
using myTNB_Android.Src.Feedback_PreLogin_FaultyStreetLamps.Adapter;
using myTNB_Android.Src.Feedback_PreLogin_FaultyStreetLamps.MVP;
using myTNB_Android.Src.FeedbackFail.Activity;
using myTNB_Android.Src.FeedbackSuccess.Activity;
using myTNB_Android.Src.SelectFeedbackState.Activity;
using myTNB_Android.Src.Utils;
using System;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace myTNB_Android.Src.Feedback_PreLogin_FaultyStreetLamps.Activity
{
    [Activity(Label = "@string/faulty_street_lamps_activity_title"
      , ScreenOrientation = ScreenOrientation.Portrait
              , WindowSoftInputMode = SoftInput.AdjustPan
      , Theme = "@style/Theme.FaultyStreetLamps")]
    public class FeedbackPreLoginFaultyStreetLampsActivity : BaseToolbarAppCompatActivity, FeedbackPreLoginFaultyStreetLampsContract.IView, View.IOnTouchListener
    {

        [BindView(Resource.Id.rootView)]
        CoordinatorLayout rootView;

        [BindView(Resource.Id.txtFeedbackTitle)]
        TextView txtFeedbackTitle;

        [BindView(Resource.Id.txtFeedbackContent)]
        TextView txtFeedbackContent;


        [BindView(Resource.Id.txtInputLayoutFullName)]
        TextInputLayout txtInputLayoutFullName;

        [BindView(Resource.Id.txtInputLayoutMobileNo)]
        TextInputLayout txtInputLayoutMobileNo;

        [BindView(Resource.Id.txtInputLayoutEmail)]
        TextInputLayout txtInputLayoutEmail;

        [BindView(Resource.Id.txtInputLayoutState)]
        TextInputLayout txtInputLayoutState;

        [BindView(Resource.Id.txtInputLayoutLocation)]
        TextInputLayout txtInputLayoutLocation;

        [BindView(Resource.Id.txtInputLayoutPoleNo)]
        TextInputLayout txtInputLayoutPoleNo;

        [BindView(Resource.Id.txtInputLayoutFeedback)]
        TextInputLayout txtInputLayoutFeedback;


        [BindView(Resource.Id.txtFullName)]
        EditText txtFullName;


        [BindView(Resource.Id.txtMobileNo)]
        EditText txtMobileNo;

        [BindView(Resource.Id.txtEmail)]
        EditText txtEmail;

        [BindView(Resource.Id.txtState)]
        EditText txtState;

        [BindView(Resource.Id.txtLocation)]
        EditText txtLocation;

        [BindView(Resource.Id.txtPoleNo)]
        EditText txtPoleNo;

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

        MaterialDialog submitDialog;

        FeedbackPreLoginFaultyStreetLampsRecyclerAdapter adapter;
        GridLayoutManager layoutManager;

        FeedbackPreLoginFaultyStreetLampsContract.IUserActionsListener userActionsListener;
        FeedbackPreLoginFaultyStreetLampsPresenter mPresenter;

        FeedbackState currentFeedbackState;

        public override Boolean ShowCustomToolbarTitle()
        {
            return true;
        }

        protected override void OnCreate(Android.OS.Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            try
            {
                // Create your application here
                Intent intent = Intent;
                SetToolBarTitle(Intent.GetStringExtra("TITLE"));
                submitDialog = new MaterialDialog.Builder(this)
                    .Title(Resource.String.feedback_submit_dialog_title)
                    .Content(Resource.String.feedback_submit_dialog_message)
                    .Progress(true, 0)
                    .Cancelable(false)
                    .Build();

                TextViewUtils.SetMuseoSans300Typeface(txtMaxImageContent, txtRelatedScreenshotTitle, txtFeedbackContent, txtFullName, txtMobileNo, txtEmail, txtState, txtLocation, txtPoleNo, txtFeedback, txtMaxCharacters);
                TextViewUtils.SetMuseoSans300Typeface(txtInputLayoutFullName, txtInputLayoutEmail, txtInputLayoutFeedback, txtInputLayoutLocation, txtInputLayoutMobileNo, txtInputLayoutPoleNo, txtInputLayoutState);
                TextViewUtils.SetMuseoSans500Typeface(txtFeedbackTitle, btnSubmit);

                txtFeedbackTitle.TextSize = TextViewUtils.GetFontSize(16f);
                txtFeedbackContent.TextSize = TextViewUtils.GetFontSize(14f);
                txtMaxCharacters.TextSize = TextViewUtils.GetFontSize(9f);
                txtRelatedScreenshotTitle.TextSize = TextViewUtils.GetFontSize(16f);
                txtMaxImageContent.TextSize = TextViewUtils.GetFontSize(16f);
               

                if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.N)
                {
                    txtFeedbackContent.TextFormatted = Html.FromHtml(GetString(Resource.String.faulty_street_lamps_txt_content), FromHtmlOptions.ModeLegacy);
                }
                else
                {
                    txtFeedbackContent.TextFormatted = Html.FromHtml(GetString(Resource.String.faulty_street_lamps_txt_content));
                }

                adapter = new FeedbackPreLoginFaultyStreetLampsRecyclerAdapter(true);
                adapter.Insert(new Base.Models.AttachedImage()
                {
                    ViewType = Constants.VIEW_TYPE_DUMMY_RECORD
                });
                layoutManager = new GridLayoutManager(this, Constants.GRID_IMAGE_COUNT);
                recyclerView.SetLayoutManager(layoutManager);
                recyclerView.SetAdapter(adapter);

                adapter.AddClickEvent += Adapter_AddClickEvent;
                adapter.RemoveClickEvent += Adapter_RemoveClickEvent;

                txtState.EnableClick();
                txtLocation.SetOnTouchListener(this);
                //txtState.SetOnTouchListener(this);

#if DEBUG
                //txtFullName.Text = "David Montecillo";
                //txtMobileNo.Text = "09498899648";
                //txtEmail.Text = "montecillodavid.acn@gmail.com";
                //txtLocation.Text = "Jalan Timur";
                //txtPoleNo.Text = "17493 8E 1";
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
                txtMobileNo.AddTextChangedListener(new InputFilterFormField(txtMobileNo, txtInputLayoutMobileNo));
                txtEmail.AddTextChangedListener(new InputFilterFormField(txtEmail, txtInputLayoutEmail));
                txtFeedback.AddTextChangedListener(new InputFilterFormField(txtFeedback, txtInputLayoutFeedback));
                txtPoleNo.AddTextChangedListener(new InputFilterFormField(txtPoleNo, txtInputLayoutPoleNo));
                txtState.AddTextChangedListener(new InputFilterFormField(txtState, txtInputLayoutState));
                txtLocation.AddTextChangedListener(new InputFilterFormField(txtLocation, txtInputLayoutLocation));

                mPresenter = new FeedbackPreLoginFaultyStreetLampsPresenter(this);
                this.userActionsListener.Start();

                if (string.IsNullOrEmpty(txtMobileNo.Text))
                {
                    txtMobileNo.Append("+60");
                }
                txtMobileNo.SetFilters(new Android.Text.IInputFilter[] { new InputFilterPhoneNumber() });

                txtFullName.TextChanged += TxtFullName_TextChanged;
                txtMobileNo.TextChanged += TxtMobileNo_TextChanged;
                txtEmail.TextChanged += TxtEmail_TextChanged;
                txtLocation.TextChanged += TxtLocation_TextChanged;
                txtFeedback.TextChanged += TxtFeedback_TextChanged;
                txtFeedback.SetOnTouchListener(this);
                txtInputLayoutFeedback.Error = GetString(Resource.String.feedback_total_character_left);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

            //txtState.Text = Constants.SELECT_STATE;
        }
        [Preserve]
        private void TxtFeedback_TextChanged(object sender, TextChangedEventArgs e)
        {
            string fullname = txtFullName.Text.Trim();
            string mobile_no = txtMobileNo.Text.Trim();
            string email = txtEmail.Text.Trim();
            string location = txtLocation.Text.Trim();
            string state = txtState.Text.Trim();
            string feedback = txtFeedback.Text;

            //int char_count = 0;

            //if (!string.IsNullOrEmpty(feedback))
            //{
            //    char_count = feedback.Length;
            //}

            //if (char_count > 0)
            //{
            //    int char_left = Constants.FEEDBACK_CHAR_LIMIT - char_count;
            //    txtInputLayoutFeedback.Error = char_left +" " + GetString(Resource.String.feedback_character_left);
            //}
            //else
            //{
            //    txtInputLayoutFeedback.Error = GetString(Resource.String.feedback_total_character_left);
            //}
            try
            {
                FeedBackCharacCount();

                this.userActionsListener.CheckRequiredFields(fullname, mobile_no, email, location, feedback, state);
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }

        [Preserve]
        private void TxtLocation_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                string fullname = txtFullName.Text.Trim();
                string mobile_no = txtMobileNo.Text.Trim();
                string email = txtEmail.Text.Trim();
                string location = txtLocation.Text.Trim();
                string feedback = txtFeedback.Text;
                string state = txtState.Text.Trim();
                this.userActionsListener.CheckRequiredFields(fullname, mobile_no, email, location, feedback, state);
            }
            catch (Exception ex)
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
                string mobile_no = txtMobileNo.Text.Trim();
                string email = txtEmail.Text.Trim();
                string location = txtLocation.Text.Trim();
                string feedback = txtFeedback.Text;
                string state = txtState.Text.Trim();
                this.userActionsListener.CheckRequiredFields(fullname, mobile_no, email, location, feedback, state);
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }
        [Preserve]
        private void TxtMobileNo_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                string fullname = txtFullName.Text.Trim();
                string mobile_no = txtMobileNo.Text.Trim();
                string email = txtEmail.Text.Trim();
                string location = txtLocation.Text.Trim();
                string feedback = txtFeedback.Text;
                string state = txtState.Text.Trim();
                this.userActionsListener.CheckRequiredFields(fullname, mobile_no, email, location, feedback, state);
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }

        [Preserve]
        private void TxtFullName_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                string fullname = txtFullName.Text.Trim();
                string mobile_no = txtMobileNo.Text.Trim();
                string email = txtEmail.Text.Trim();
                string location = txtLocation.Text.Trim();
                string feedback = txtFeedback.Text;
                string state = txtState.Text.Trim();
                this.userActionsListener.CheckRequiredFields(fullname, mobile_no, email, location, feedback, state);
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }

        public bool IsActive()
        {
            return Window.DecorView.RootView.IsShown;
        }

        public override int ResourceId()
        {
            return Resource.Layout.FeedbackPreLoginFaultyStreetLampsView;
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
            try
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
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
            return false;
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
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }
        [OnClick(Resource.Id.btnSubmit)]
        void OnSubmit(object sender, EventArgs eventArgs)
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

                string fullName = txtFullName.Text.Trim();
                string mobileNo = txtMobileNo.Text.Trim();
                string email = txtEmail.Text.Trim();
                string state = txtState.Text.Trim();
                string locationName = txtLocation.Text.Trim();
                string poleNo = txtPoleNo.Text.Trim();
                string feedback = txtFeedback.Text.Trim();

                this.userActionsListener.OnSubmit(this.DeviceId(), fullName, mobileNo, email, currentFeedbackState, locationName, poleNo, feedback, adapter.GetAllImages());
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
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            this.userActionsListener.OnActivityResult(requestCode, resultCode, data);
        }

        public void ClearErrors()
        {
            try
            {
                //txtInputLayoutFeedback.SetErrorTextAppearance(Resource.Style.TextErrorAppearance);
                txtInputLayoutFeedback.SetErrorTextAppearance(TextViewUtils.SelectedFontSize() == "L" ? Resource.Style.TextInputLayoutFeedbackCountLarge : Resource.Style.TextInputLayoutFeedbackCount);
                txtInputLayoutLocation.SetErrorTextAppearance(Resource.Style.TextErrorAppearance);
                txtInputLayoutPoleNo.SetErrorTextAppearance(Resource.Style.TextErrorAppearance);


                TextViewUtils.SetMuseoSans300Typeface(txtInputLayoutLocation.FindViewById<TextView>(Resource.Id.textinput_error),
                txtInputLayoutFeedback.FindViewById<TextView>(Resource.Id.textinput_error),
                txtInputLayoutPoleNo.FindViewById<TextView>(Resource.Id.textinput_error));

                txtInputLayoutEmail.Error = null;
                txtInputLayoutFeedback.Error = null;
                txtInputLayoutFullName.Error = null;
                txtInputLayoutLocation.Error = null;
                txtInputLayoutMobileNo.Error = null;
                txtInputLayoutPoleNo.Error = null;
                txtInputLayoutState.Error = null;
                txtInputLayoutMobileNo.Error = null;

                TextViewUtils.SetMuseoSans300Typeface(txtInputLayoutFullName, txtInputLayoutEmail, txtInputLayoutFeedback, txtInputLayoutLocation, txtInputLayoutMobileNo, txtInputLayoutPoleNo, txtInputLayoutState);

                FeedBackCharacCount();
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

        public void ClearInputFields()
        {
            txtEmail.Text = "";
            txtFeedback.Text = "";
            txtFullName.Text = "";
            txtLocation.Text = "";
            txtMobileNo.Text = "";
            txtPoleNo.Text = "";
        }

        public void EnableSubmitButton()
        {
            string fullname = txtFullName.Text.Trim();
            string mobile_no = txtMobileNo.Text.Trim();
            string email = txtEmail.Text.Trim();
            string location = txtLocation.Text.Trim();
            string feedback = txtFeedback.Text.Trim();
            try
            {
                if (TextUtils.IsEmpty(fullname) || TextUtils.IsEmpty(mobile_no) || TextUtils.IsEmpty(email) || TextUtils.IsEmpty(location))
                {
                    DisableSubmitButton();
                    return;
                }
                else if (TextUtils.IsEmpty(feedback))
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

        public void SetPresenter(FeedbackPreLoginFaultyStreetLampsContract.IUserActionsListener userActionListener)
        {
            this.userActionsListener = userActionListener;
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

        public void ShowEmptyEmaiError()
        {
            txtInputLayoutEmail.Error = GetString(Resource.String.faulty_street_lamps_feedback_empty_email_error);
        }

        public void ShowEmptyFeedbackError()
        {
            txtInputLayoutFeedback.SetErrorTextAppearance(Resource.Style.TextErrorAppearance);
            TextViewUtils.SetMuseoSans300Typeface(txtInputLayoutFeedback.FindViewById<TextView>(Resource.Id.textinput_error));
            TextViewUtils.SetMuseoSans300Typeface(txtInputLayoutFeedback);
            txtInputLayoutFeedback.Error = GetString(Resource.String.faulty_street_lamps_feedback_empty_feedback_error);
        }

        public void ShowEmptyFullnameError()
        {
            txtInputLayoutFullName.Error = GetString(Resource.String.faulty_street_lamps_feedback_empty_fullname_error);
        }

        public void ShowEmptyMobileNoError()
        {
            txtInputLayoutMobileNo.Error = Utility.GetLocalizedErrorLabel("invalid_mobileNumber");
        }

        public void ShowInvalidEmailError()
        {
            txtInputLayoutEmail.Error = GetString(Resource.String.faulty_street_lamps_feedback_invalid_email_error);
        }

        public void ShowInvalidMobileNoError()
        {
            txtInputLayoutMobileNo.Error = Utility.GetLocalizedErrorLabel("invalid_mobileNumber");
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

        public void ShowState(FeedbackState feedbackState)
        {
            try
            {
                if (feedbackState != null)
                {
                    this.currentFeedbackState = feedbackState;
                    txtState.Text = feedbackState.StateName;
                }
                else
                {
                    if (string.IsNullOrEmpty(txtState.Text.Trim()))
                    {
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
