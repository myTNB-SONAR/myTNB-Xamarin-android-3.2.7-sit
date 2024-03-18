using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Preferences;
using Android.Provider;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Text;
using Android.Views;
using Android.Widget;
using AndroidX.Core.Content;
using AndroidX.RecyclerView.Widget;
using CheeseBind;
using Java.Text;
using Java.Util;
using myTNB.AndroidApp.Src.Base.Activity;
using myTNB.AndroidApp.Src.Base.Models;
using myTNB.AndroidApp.Src.FeedbackGeneralEnquiryStepOne.Adapter;
using myTNB.AndroidApp.Src.FeedbackGeneralEnquiryStepOne.MVP;
using myTNB.AndroidApp.Src.FeedbackGeneralEnquiryStepTwo.Activity;
using myTNB.AndroidApp.Src.Utils;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace myTNB.AndroidApp.Src.FeedbackGeneralEnquiryStepOne.Activity
{

    [Activity(Label = "General Enquiry"
      , ScreenOrientation = ScreenOrientation.Portrait
              , WindowSoftInputMode = SoftInput.AdjustPan
      , Theme = "@style/Theme.FaultyStreetLamps")]


    public class FeedbackGeneralEnquiryStepOneActivity : BaseToolbarAppCompatActivity, FeedbackGeneralEnquiryStepOneContract.IView, View.IOnTouchListener
    {
        //needed when add contract
        FeedbackGeneralEnquiryStepOneContract.IUserActionsListener userActionsListener;

        internal static readonly string TAG = typeof(FeedbackGeneralEnquiryStepOneActivity).Name;

        FeedbackGeneralEnquiryStepOnePresenter mPresenter;


        [BindView(Resource.Id.rootView)]
        CoordinatorLayout rootView;

        [BindView(Resource.Id.recyclerView)]
        RecyclerView recyclerView;

        [BindView(Resource.Id.txtGeneralEnquiry1)]
        EditText txtGeneralEnquiry1;

        [BindView(Resource.Id.txtInputLayoutGeneralEnquiry1)]
        Google.Android.Material.TextField.TextInputLayout txtInputLayoutGeneralEnquiry1;

        [BindView(Resource.Id.txtMaxCharacters)]
        TextView txtMaxCharacters;

        [BindView(Resource.Id.IwantToEnquire)]
        TextView IwantToEnquire;

        [BindView(Resource.Id.uploadSupportingDoc)]
        TextView uploadSupportingDoc;

        [BindView(Resource.Id.txtRelatedScreenshotTitle)]
        TextView txtRelatedScreenshotTitle;

        [BindView(Resource.Id.txtMaxImageContent)]
        TextView txtMaxImageContent;

        [BindView(Resource.Id.btnNext)]
        Button btnNext;

        [BindView(Resource.Id.txtstep1of2)]
        TextView txtstep1of2;

        [BindView(Resource.Id.TextView_CharLeft)]
        TextView TextView_CharLeft;




        private AlertDialog _ChooseDialog;

        LinearLayoutManager layoutManager;
        FeedbackGeneralEnquiryStepOneImageRecyclerAdapter adapter;



        private string accNo = null;


        protected override void OnCreate(Android.OS.Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            try
            {


                Android.OS.Bundle extras = Intent.Extras;

                if (extras != null)
                {
                    if (extras.ContainsKey(Constants.ACCOUNT_NUMBER))
                    {
                        accNo = extras.GetString(Constants.ACCOUNT_NUMBER);
                    }

                }


                // Intent intent = Intent;
                SetToolBarTitle(Utility.GetLocalizedLabel("SubmitEnquiry", "generalEnquiryTitle"));
                this.mPresenter = new FeedbackGeneralEnquiryStepOnePresenter(this);

                adapter = new FeedbackGeneralEnquiryStepOneImageRecyclerAdapter(true);
                adapter.Insert(new Base.Models.AttachedImage()
                {
                    ViewType = Constants.VIEW_TYPE_DUMMY_RECORD
                });
                layoutManager = new LinearLayoutManager(this, LinearLayoutManager.Vertical, false);
                recyclerView.SetLayoutManager(layoutManager);
                recyclerView.SetAdapter(adapter);

                // adapter listener
                adapter.AddClickEvent += Adapter_AddClickEvent;
                adapter.RemoveClickEvent += Adapter_RemoveClickEvent;

                //injecting data       
                txtInputLayoutGeneralEnquiry1.Hint = Utility.GetLocalizedLabel("SubmitEnquiry", "messageHint");
                txtInputLayoutGeneralEnquiry1.SetHintTextAppearance(TextViewUtils.IsLargeFonts
                    ? Resource.Style.TextInputLayout_TextAppearance_Large
                    : Resource.Style.TextInputLayout_TextAppearance_Small);
                txtGeneralEnquiry1.Text = "";



                //add listener 
                txtGeneralEnquiry1.TextChanged += TextChanged;
                txtGeneralEnquiry1.AddTextChangedListener(new InputFilterFormField(txtGeneralEnquiry1, txtInputLayoutGeneralEnquiry1));

                DisableSubmitButton();

                // set font
                TextViewUtils.SetMuseoSans300Typeface(txtInputLayoutGeneralEnquiry1);
                TextViewUtils.SetMuseoSans300Typeface(txtRelatedScreenshotTitle, txtMaxImageContent, TextView_CharLeft);
                TextViewUtils.SetMuseoSans500Typeface(txtstep1of2, IwantToEnquire, uploadSupportingDoc, btnNext);

                //set translation 
                txtstep1of2.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "stepTitle1of2");
                IwantToEnquire.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "enquiryAboutTitle");
                uploadSupportingDoc.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "uploadDocTitle");
                txtRelatedScreenshotTitle.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "attachTitle");
                txtMaxImageContent.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "attachDescription");
                btnNext.Text = Utility.GetLocalizedLabel("Common", "next");

                TextViewUtils.SetTextSize9(txtMaxImageContent);
                TextViewUtils.SetTextSize10(TextView_CharLeft);
                TextViewUtils.SetTextSize12(txtstep1of2);
                TextViewUtils.SetTextSize14(txtRelatedScreenshotTitle);
                TextViewUtils.SetTextSize16(IwantToEnquire, txtGeneralEnquiry1, uploadSupportingDoc, btnNext);

                //set feedback setting
                TextView_CharLeft.Text = string.Format(Utility.GetLocalizedCommonLabel("charactersLeft"), Constants.FEEDBACK_CHAR_LIMIT);
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

        }

        public override void Ready()
        {
            FileUtils.CreateDirectory(this, FileUtils.TEMP_IMAGE_FOLDER);
            FileUtils.CreateDirectory(this, FileUtils.PDF_FOLDER);

        }

        public string GetImageName(int itemCount)
        {
            SimpleDateFormat dateFormatter = new SimpleDateFormat("yyyyMMdd");
            Calendar calendar = Calendar.GetInstance(Locale.Default);
            return GetString(Resource.String.feedback_image_name_convention, dateFormatter.Format(calendar.TimeInMillis), UserSessions.GetCurrentImageCount(PreferenceManager.GetDefaultSharedPreferences(this)) + itemCount);
        }

        public string copyPDFGetFilePath(Android.Net.Uri realFilePath, string filename)
        {
            string filePath = FileUtils.CopyPDF(this, realFilePath, FileUtils.PDF_FOLDER, filename);
            return filePath;
        }

        public override void OnTrimMemory(TrimMemory level)
        {
            base.OnTrimMemory(level);

            switch (level)
            {
                case TrimMemory.RunningLow:
                    // GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
                    GC.Collect();
                    break;
                default:
                    // GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
                    GC.Collect();
                    break;
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
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }

        private void Adapter_AddClickEvent(object sender, int e)
        {
            try
            {
                string[] items = { Utility.GetLocalizedLabel("FeedbackForm", "takePhoto")  ,
                               Utility.GetLocalizedLabel("FeedbackForm", "chooseFromLibrary") ,
                               Utility.GetLocalizedLabel("SubmitEnquiry", "documentPdf") ,
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
                    else if (items[args.Which].Equals(Utility.GetLocalizedLabel("SubmitEnquiry", "documentPdf")))
                    {
                        this.userActionsListener.OnAttachPDF();
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

        public void SetPresenter(FeedbackGeneralEnquiryStepOneContract.IUserActionsListener userActionListener)
        {
            this.userActionsListener = userActionListener;
        }

        public override int ResourceId()
        {
            return Resource.Layout.FeedbackGeneralEnquiryStepOneActivityView;
        }

        public override Boolean ShowCustomToolbarTitle()
        {
            return true;
        }

        public bool IsActive()
        {
            // needed when include contract
            return Window.DecorView.RootView.IsShown;
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

        public void ShowPDF()
        {

            if (!this.GetIsClicked())
            {
                this.SetIsClicked(true);


                Intent galleryIntent = new Intent(Intent.ActionGetContent);
                galleryIntent.SetType("application/pdf");
                galleryIntent.AddFlags(ActivityFlags.GrantReadUriPermission);
                galleryIntent.AddFlags(ActivityFlags.GrantWriteUriPermission);
                galleryIntent.AddFlags(ActivityFlags.GrantPersistableUriPermission);
                galleryIntent.PutExtra(Intent.ExtraLocalOnly, true);
                StartActivityForResult(Intent.CreateChooser(galleryIntent, GetString(Resource.String.bill_related_feedback_select_images)), Constants.RUNTIME_PERMISSION_GALLERY_PDF_REQUEST_CODE);
            }

        }

        public String getFilename(Android.Net.Uri uri)
        {
            try
            {

                string filename = FileUtils.getFilenameUsingContentResolver(this, uri);
                return filename;

            }
            catch

            {
                return null;
            }

        }


        public string getActualPath(Android.Net.Uri uri)
        {
            string path = FileUtils.GetActualPathForFile(uri, this);
            return path;
        }


        Snackbar mErrorMessageSnackBar;
        public void ShowError(string message = null)
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




        private void FeedBackCharacCount()
        {
            try
            {
                TextView_CharLeft.Visibility = ViewStates.Gone;

                string feedback = txtGeneralEnquiry1.Text;
                int char_count = 0;

                if (!string.IsNullOrEmpty(feedback))
                {
                    char_count = feedback.Length;
                }

                if (char_count > 0)
                {
                    int char_left = Constants.FEEDBACK_CHAR_LIMIT - char_count;

                    txtInputLayoutGeneralEnquiry1.SetErrorTextAppearance(TextViewUtils.IsLargeFonts
                        ? Resource.Style.TextInputLayoutFeedbackCountLarge
                        : Resource.Style.TextInputLayoutFeedbackCount);

                    //TextViewUtils.SetMuseoSans300Typeface(txtInputLayoutGeneralEnquiry1.FindViewById<TextView>(Resource.Id.textinput_error));
                    TextViewUtils.SetMuseoSans300Typeface(txtInputLayoutGeneralEnquiry1);
                    txtInputLayoutGeneralEnquiry1.Error = string.Format(Utility.GetLocalizedCommonLabel("charactersLeft"), char_left);
                    var handleBounceError = txtInputLayoutGeneralEnquiry1.FindViewById<TextView>(Resource.Id.textinput_error);
                    handleBounceError.SetPadding(top: 4, left: 0, right: 0, bottom: 0);


                }
                else
                {
                    txtInputLayoutGeneralEnquiry1.SetErrorTextAppearance(TextViewUtils.IsLargeFonts
                        ? Resource.Style.TextInputLayoutFeedbackCountLarge
                        : Resource.Style.TextInputLayoutFeedbackCount);
                    // TextViewUtils.SetMuseoSans300Typeface(txtInputLayoutGeneralEnquiry1.FindViewById<TextView>(Resource.Id.textinput_error));
                    TextViewUtils.SetMuseoSans300Typeface(txtInputLayoutGeneralEnquiry1);




                    txtInputLayoutGeneralEnquiry1.Error = string.Format(Utility.GetLocalizedCommonLabel("charactersLeft"), Constants.FEEDBACK_CHAR_LIMIT);
                    var handleBounceError = txtInputLayoutGeneralEnquiry1.FindViewById<TextView>(Resource.Id.textinput_error);
                    handleBounceError.SetPadding(top: 4, left: 0, right: 0, bottom: 0);


                }
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
                string feedback = txtGeneralEnquiry1.Text.Trim();
                if (TextUtils.IsEmpty(feedback))
                {
                    //ShowEmptyFeedbackError();
                    DisableSubmitButton();
                    return;
                }
                else
                {
                    ClearErrors();
                }
                btnNext.Enabled = true;
                btnNext.Background = ContextCompat.GetDrawable(this, Resource.Drawable.green_button_background);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void ShowEmptyFeedbackError()
        {
            TextView_CharLeft.Visibility = ViewStates.Gone;
            txtInputLayoutGeneralEnquiry1.SetErrorTextAppearance(TextViewUtils.IsLargeFonts
                ? Resource.Style.TextInputLayoutBottomErrorHintLarge
                : Resource.Style.TextInputLayoutBottomErrorHint);

            //TextViewUtils.SetMuseoSans300Typeface(txtInputLayoutGeneralEnquiry1.FindViewById<TextView>(Resource.Id.textinput_error));
            TextViewUtils.SetMuseoSans300Typeface(txtInputLayoutGeneralEnquiry1);
            txtInputLayoutGeneralEnquiry1.Error = Utility.GetLocalizedLabel("FeedbackForm", "invalidFeedback");
        }

        public void DisableSubmitButton()
        {
            btnNext.Enabled = false;
            btnNext.Background = ContextCompat.GetDrawable(this, Resource.Drawable.silver_chalice_button_background);
        }


        [Preserve]
        private void TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            try
            {
                FeedBackCharacCount();

                string feedback = txtGeneralEnquiry1.Text;
                this.userActionsListener.CheckRequiredFields(feedback);
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }

        }

        public void ClearErrors()
        {
            try
            {   //remove any error
                txtInputLayoutGeneralEnquiry1.SetErrorTextAppearance(TextViewUtils.IsLargeFonts
                    ? Resource.Style.TextInputLayoutFeedbackCountLarge
                    : Resource.Style.TextInputLayoutFeedbackCount);
                TextViewUtils.SetMuseoSans300Typeface(txtInputLayoutGeneralEnquiry1);
                txtInputLayoutGeneralEnquiry1.Error = " ";
                var handleBounceError = txtInputLayoutGeneralEnquiry1.FindViewById<TextView>(Resource.Id.textinput_error);
                handleBounceError.SetPadding(top: 4, left: 0, right: 0, bottom: 0);

                //TextViewUtils.SetMuseoSans300Typeface(txtInputLayoutGeneralEnquiry1.FindViewById<TextView>(Resource.Id.textinput_error));


                FeedBackCharacCount();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }





        public void ShowGeneralEnquiry()
        {

            var feedbackGeneralEnquiry = new Intent(this, typeof(FeedbackGeneralEnquiryStepTwoActivity));
            feedbackGeneralEnquiry.PutExtra("FEEDBACK", txtGeneralEnquiry1.Text.Trim());
            feedbackGeneralEnquiry.PutExtra("IMAGE", JsonConvert.SerializeObject(adapter?.GetAllImages()));
            feedbackGeneralEnquiry.PutExtra(Constants.ACCOUNT_NUMBER, accNo);
            feedbackGeneralEnquiry.PutExtra(Constants.PAGE_TITLE, Utility.GetLocalizedLabel("SubmitEnquiry", "generalEnquiryTitle"));
            feedbackGeneralEnquiry.PutExtra(Constants.PAGE_STEP_TITLE, Utility.GetLocalizedLabel("SubmitEnquiry", "stepTitle2of2"));
            StartActivity(feedbackGeneralEnquiry);
            //StartActivityForResult(feedbackGeneralEnquiry, Constants.REQUEST_FEEDBACK_SUCCESS_VIEW);

        }

        [OnClick(Resource.Id.btnNext)]
        void OnNextButton(object sender, EventArgs eventArgs)
        {
            if (!this.GetIsClicked())
            {

                this.userActionsListener.OnGeneralEnquiry();

            }
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            this.userActionsListener.OnActivityResult(requestCode, resultCode, data);

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


        public Task<string> SaveGalleryImage(Android.Net.Uri selectedImage, string pTempImagePath, string pFileName)
        {
            return Task.Run<string>(() =>
            {
                return FileUtils.ProcessGalleryImage(this, selectedImage, pTempImagePath, pFileName);
            });

        }



        public Task<string> SaveCameraImage(string tempImagePath, string fileName)
        {
            return Task.Run<string>(() =>
            {
                return FileUtils.ProcessCameraImage(this, tempImagePath, fileName);
            });


        }

        public void UpdateAdapter(string pFilePath, string pFileName, string tfileName = "")
        {
            adapter.Update(adapter.ItemCount - 1, new AttachedImage()
            {
                ViewType = Constants.VIEW_TYPE_REAL_RECORD,
                Name = pFileName,
                Path = pFilePath,
                FileName = tfileName
            });
            if (adapter.ItemCount < 2)
            {
                adapter.Add(new AttachedImage()
                {
                    ViewType = Constants.VIEW_TYPE_DUMMY_RECORD
                });
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

                    //if (e.Action == MotionEventActions.Up)
                    //{
                    //    if (e.RawX >= (txtAccountNo.Right - txtAccountNo.GetCompoundDrawables()[DRAWABLE_RIGHT].Bounds.Width()))
                    //    {
                    //        this.userActionsListener.OnSelectAccount();

                    //        return true;
                    //    }
                    //}
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
    }
}