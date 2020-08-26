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
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.Feedback_Login_BillRelated.Adapter;
using myTNB_Android.Src.Feedback_Login_BillRelated.MVP;
using myTNB_Android.Src.FeedbackFail.Activity;
using myTNB_Android.Src.FeedbackSuccess.Activity;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;
using System;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace myTNB_Android.Src.Feedback_Login_BillRelated.Activity
{
    [Activity(Label = "@string/bill_related_activity_title"
      , ScreenOrientation = ScreenOrientation.Portrait
              , WindowSoftInputMode = SoftInput.AdjustPan
      , Theme = "@style/Theme.FaultyStreetLamps")]
    public class FeedbackLoginBillRelatedActivity : BaseToolbarAppCompatActivity, FeedbackLoginBillRelatedContract.IView, View.IOnTouchListener
    {

        [BindView(Resource.Id.txtInputLayoutAccountNo)]
        TextInputLayout txtInputLayoutAccountNo;

        [BindView(Resource.Id.txtInputLayoutFeedback)]
        TextInputLayout txtInputLayoutFeedback;

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

        [BindView(Resource.Id.rootview)]
        FrameLayout rootView;

        FeedbackLoginBillRelatedImageRecyclerAdapter adapter;

        MaterialDialog submitDialog;
        GridLayoutManager layoutManager;
        FeedbackLoginBillRelatedContract.IUserActionsListener userActionsListener;
        FeedbackLoginBillRelatedPresenter mPresenter;
        CustomerBillingAccount customerBillingAccount;
        private MobileNumberInputComponent mobileNumberInputComponent;
        private bool isMobileNumberShown = false;
        private const int COUNTRY_CODE_SELECT_REQUEST = 1;

        public override Boolean ShowCustomToolbarTitle()
        {
            return true;
        }
        protected override void OnCreate(Android.OS.Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            try
            {

                Intent intent = Intent;
                SetToolBarTitle(Intent.GetStringExtra("TITLE"));

                submitDialog = new MaterialDialog.Builder(this)
                .Title(Resource.String.feedback_submit_dialog_title)
                .Content(Resource.String.feedback_submit_dialog_message)
                .Progress(true, 0)
                .Cancelable(false)
                .Build();


                customerBillingAccount = new CustomerBillingAccount();

                // Create your application here
                TextViewUtils.SetMuseoSans300Typeface(txtInputLayoutAccountNo, txtInputLayoutFeedback);
                TextViewUtils.SetMuseoSans300Typeface(txtMaxImageContent, txtAccountNo, txtFeedback, txtRelatedScreenshotTitle, txtMaxCharacters);
                TextViewUtils.SetMuseoSans500Typeface(btnSubmit);

                txtInputLayoutAccountNo.Hint = Utility.GetLocalizedCommonLabel("accountNo");
                txtInputLayoutFeedback.Hint = Utility.GetLocalizedLabel("FeedbackForm", "feedback");
                txtRelatedScreenshotTitle.Text = Utility.GetLocalizedLabel("FeedbackForm", "attachPhotoTitle");
                txtMaxImageContent.Text = Utility.GetLocalizedLabel("FeedbackForm", "maxFile");
                btnSubmit.Text = Utility.GetLocalizedCommonLabel("submit");

                adapter = new FeedbackLoginBillRelatedImageRecyclerAdapter(true);
                adapter.Insert(new Base.Models.AttachedImage()
                {
                    ViewType = Constants.VIEW_TYPE_DUMMY_RECORD
                });
                layoutManager = new GridLayoutManager(this, Constants.GRID_IMAGE_COUNT);
                recyclerView.SetLayoutManager(layoutManager);
                recyclerView.SetAdapter(adapter);

                adapter.AddClickEvent += Adapter_AddClickEvent;
                adapter.RemoveClickEvent += Adapter_RemoveClickEvent;




                txtFeedback.AddTextChangedListener(new InputFilterFormField(txtFeedback, txtInputLayoutFeedback));
                txtAccountNo.AddTextChangedListener(new InputFilterFormField(txtAccountNo, txtInputLayoutAccountNo));

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

                mPresenter = new FeedbackLoginBillRelatedPresenter(this);
                txtAccountNo.EnableClick();
                txtAccountNo.SetOnTouchListener(this);

                mobileNumberFieldContainer.RemoveAllViews();
                mobileNumberInputComponent = new MobileNumberInputComponent(this);
                mobileNumberInputComponent.SetOnTapCountryCodeAction(OnTapCountryCode);
                mobileNumberInputComponent.SetValidationAction(OnValidateMobileNumber);
                mobileNumberInputComponent.SetMobileNumberLabel(Utility.GetLocalizedCommonLabel("mobileNo"));
                mobileNumberInputComponent.SetSelectedCountry(CountryUtil.Instance.GetDefaultCountry());
                mobileNumberFieldContainer.AddView(mobileNumberInputComponent);

                txtFeedback.TextChanged += TextChanged;
                txtFeedback.SetOnTouchListener(this);
                txtInputLayoutFeedback.Error = string.Format(Utility.GetLocalizedCommonLabel("charactersLeft"), Constants.FEEDBACK_CHAR_LIMIT);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
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

        protected override void OnPause()
        {
            base.OnPause();
        }

        [Preserve]
        private void TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            try
            {
                FeedBackCharacCount();

                string feedback = txtFeedback.Text;
                if (isMobileNumberShown)
                {
                    string mobile_no = mobileNumberInputComponent.GetMobileNumberValue();
                    this.userActionsListener.CheckRequiredFields(mobile_no, feedback);
                }
                else
                {
                    this.userActionsListener.CheckRequiredFields(feedback);
                }

            }
            catch (Exception ex)
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
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
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
                if (eTxtView.Id == Resource.Id.txtAccountNo)
                {

                    if (e.Action == MotionEventActions.Up)
                    {
                        if (e.RawX >= (txtAccountNo.Right - txtAccountNo.GetCompoundDrawables()[DRAWABLE_RIGHT].Bounds.Width()))
                        {
                            this.userActionsListener.OnSelectAccount();

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

        [Preserve]
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
        private AlertDialog _ChooseDialog;
        [Preserve]
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

        public void ClearErrors()
        {
            try
            {
                txtInputLayoutFeedback.SetErrorTextAppearance(Resource.Style.TextInputLayoutFeedbackCount);
                txtInputLayoutAccountNo.Error = null;
                txtInputLayoutFeedback.Error = null;

                TextViewUtils.SetMuseoSans300Typeface(txtInputLayoutFeedback.FindViewById<TextView>(Resource.Id.textinput_error));
                TextViewUtils.SetMuseoSans300Typeface(txtInputLayoutFeedback);
                FeedBackCharacCount();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void EnableSubmitButton()
        {
            try
            {
                string feedback = txtFeedback.Text.Trim();
                if (TextUtils.IsEmpty(feedback))
                {
                    ShowEmptyFeedbackError();
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


        public bool IsActive()
        {
            return Window.DecorView.RootView.IsShown;
        }

        public override int ResourceId()
        {
            return Resource.Layout.FeedbackLoggedInBillRelatedView;
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

        public void SetPresenter(FeedbackLoginBillRelatedContract.IUserActionsListener userActionListener)
        {
            this.userActionsListener = userActionListener;
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

        public void ShowEmptyFeedbackError()
        {
            txtInputLayoutFeedback.SetErrorTextAppearance(Resource.Style.TextErrorAppearance);
            TextViewUtils.SetMuseoSans300Typeface(txtInputLayoutFeedback.FindViewById<TextView>(Resource.Id.textinput_error));
            TextViewUtils.SetMuseoSans300Typeface(txtInputLayoutFeedback);
            txtInputLayoutFeedback.Error = Utility.GetLocalizedLabel("FeedbackForm", "invalidFeedback");
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
            var failIntent = new Intent(this, typeof(FeedbackFailActivity));
            StartActivityForResult(failIntent, Constants.REQUEST_FEEDBACK_FAIL_VIEW);
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

        [OnClick(Resource.Id.btnSubmit)]
        void OnSubmit(object sender, EventArgs eventArgs)
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

                    string feedback = txtFeedback.Text.Trim();
                    if (isMobileNumberShown)
                    {
                        string mobile_no = mobileNumberInputComponent.GetMobileNumberValueWithISDCode();
                        this.userActionsListener.OnSubmit(this.DeviceId(), mobile_no, customerBillingAccount?.AccNum, feedback, adapter?.GetAllImages());
                    }
                    else
                    {
                        this.userActionsListener.OnSubmit(this.DeviceId(), customerBillingAccount?.AccNum, feedback, adapter?.GetAllImages());
                    }
                }
            }
            catch (Exception e)
            {
                this.SetIsClicked(false);
                Utility.LoggingNonFatalError(e);
            }
        }


        [OnClick(Resource.Id.accountLayout)]
        void OnSelectAccountLayout(object sender, EventArgs eventArgs)
        {
            if (!this.GetIsClicked())
            {
                this.SetIsClicked(true);
                this.userActionsListener.OnSelectAccount();
            }
        }

        [OnClick(Resource.Id.txtInputLayoutAccountNo)]
        void OnSelectAccountLayout1(object sender, EventArgs eventArgs)
        {
            if (!this.GetIsClicked())
            {
                this.SetIsClicked(true);
                this.userActionsListener.OnSelectAccount();
            }
        }

        [OnClick(Resource.Id.txtAccountNo)]
        void OnSelectAccountLayout2(object sender, EventArgs eventArgs)
        {
            if (!this.GetIsClicked())
            {
                this.SetIsClicked(true);
                this.userActionsListener.OnSelectAccount();
            }
        }


        public override void Ready()
        {
            FileUtils.CreateDirectory(this, FileUtils.TEMP_IMAGE_FOLDER);
            this.userActionsListener.Start();
            string feedback = txtFeedback.Text;
            if (isMobileNumberShown)
            {
                string mobile_no = mobileNumberInputComponent.GetMobileNumberValue();
                this.userActionsListener.CheckRequiredFields(mobile_no, feedback);
            }
            else
            {
                this.userActionsListener.CheckRequiredFields(feedback);
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


        public void ClearInputFields()
        {
            mobileNumberInputComponent.ClearMobileNumber();
            txtFeedback.Text = "";
        }

        public void ShowSelectedAccount(CustomerBillingAccount customerBillingAccount)
        {
            this.customerBillingAccount = customerBillingAccount;
            txtAccountNo.Text = customerBillingAccount.AccNum + " - " + customerBillingAccount.AccDesc;
        }

        public void ShowSelectAccount(AccountData accountData)
        {
            Intent supplyAccount = new Intent(this, typeof(FeedbackSelectAccountActivity));
            supplyAccount.PutExtra(Constants.SELECTED_ACCOUNT, JsonConvert.SerializeObject(accountData));
            StartActivityForResult(supplyAccount, Constants.SELECT_ACCOUNT_REQUEST_CODE);
        }

        public string GetImageName(int itemCount)
        {
            SimpleDateFormat dateFormatter = new SimpleDateFormat("yyyyMMdd");
            Calendar calendar = Calendar.GetInstance(Locale.Default);
            return GetString(Resource.String.feedback_image_name_convention, dateFormatter.Format(calendar.TimeInMillis), UserSessions.GetCurrentImageCount(PreferenceManager.GetDefaultSharedPreferences(this)) + itemCount);
        }

        public void ShowMobileNo()
        {
            isMobileNumberShown = true;
            mobileNumberFieldContainer.Visibility = ViewStates.Visible;
        }

        public void HideMobileNo()
        {
            isMobileNumberShown = false;
            mobileNumberFieldContainer.Visibility = ViewStates.Gone;
        }

        public void ShowEmptyMobileNoError()
        {
            //No Impl
        }

        public void ShowInvalidMobileNoError()
        {
            //No Impl
        }

        public void ClearMobileNoError()
        {
            //No Impl
        }


        public void showInvalidAccountNumberError()
        {
            txtInputLayoutAccountNo.Error = GetString(Resource.String.account_number_validation_error);
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

        private void OnTapCountryCode()
        {
            Intent intent = new Intent(this, typeof(SelectCountryActivity));
            StartActivityForResult(intent, COUNTRY_CODE_SELECT_REQUEST);
        }

        private void OnValidateMobileNumber(bool isValidated)
        {
            string feedback = txtFeedback.Text;
            string mobile_no = mobileNumberInputComponent.GetMobileNumberValue();
            this.userActionsListener.CheckRequiredFields(mobile_no, feedback);
        }
    }
}
