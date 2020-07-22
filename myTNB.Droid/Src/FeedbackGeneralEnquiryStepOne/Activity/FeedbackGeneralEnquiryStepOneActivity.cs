using AFollestad.MaterialDialogs;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Preferences;
using Android.Provider;
using Android.Runtime;
using Android.Support.Constraints;
using Android.Support.Design.Widget;
using Android.Support.V4.Content;
using Android.Support.V7.Widget;
using Android.Text;
using Android.Views;
using Android.Widget;
using CheeseBind;
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
using myTNB_Android.Src.Feedback_Prelogin_NewIC.MVP;
using myTNB_Android.Src.FeedbackFail.Activity;
using myTNB_Android.Src.FeedbackGeneralEnquiryStepOne.Adapter;
using myTNB_Android.Src.FeedbackGeneralEnquiryStepOne.MVP;
using myTNB_Android.Src.FeedbackGeneralEnquiryStepTwo.Activity;
using myTNB_Android.Src.FeedbackSuccess.Activity;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;
using System;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using System.Web;


namespace myTNB_Android.Src.FeedbackGeneralEnquiryStepOne.Activity
{
 
    [Activity(Label = "General Enquiry"
      , ScreenOrientation = ScreenOrientation.Portrait
              , WindowSoftInputMode = SoftInput.AdjustPan
      , Theme = "@style/Theme.FaultyStreetLamps")]


    public class FeedbackGeneralEnquiryStepOneActivity : BaseToolbarAppCompatActivity  , FeedbackGeneralEnquiryStepOneContract.IView, View.IOnTouchListener
    {
        //needed when add contract
        FeedbackGeneralEnquiryStepOneContract.IUserActionsListener userActionsListener;

        internal static readonly string TAG = typeof(FeedbackGeneralEnquiryStepOneActivity).Name;

        FeedbackGeneralEnquiryStepOnePresenter mPresenter;


        [BindView(Resource.Id.recyclerView)]
         RecyclerView recyclerView;

        [BindView(Resource.Id.txtGeneralEnquiry1)]
        EditText txtGeneralEnquiry1;

        [BindView(Resource.Id.txtInputLayoutGeneralEnquiry1)]
        TextInputLayout txtInputLayoutGeneralEnquiry1;

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
        

        private AlertDialog _ChooseDialog;

        LinearLayoutManager layoutManager;
        FeedbackGeneralEnquiryStepOneImageRecyclerAdapter adapter;



        private string accNo = null;


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            try
            {

                Bundle extras = Intent.Extras;

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


                //add listener 
                txtGeneralEnquiry1.SetOnTouchListener(this);
                txtGeneralEnquiry1.TextChanged += TextChanged;
                txtGeneralEnquiry1.AddTextChangedListener(new InputFilterFormField(txtGeneralEnquiry1, txtInputLayoutGeneralEnquiry1));

                DisableSubmitButton();

                // set font
                TextViewUtils.SetMuseoSans300Typeface(txtInputLayoutGeneralEnquiry1);
                TextViewUtils.SetMuseoSans300Typeface(txtRelatedScreenshotTitle, txtMaxImageContent);
        
                TextViewUtils.SetMuseoSans500Typeface(txtstep1of2, IwantToEnquire, uploadSupportingDoc);


                //set translation 
                txtstep1of2.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "stepTitle1of2");
                IwantToEnquire.Text= Utility.GetLocalizedLabel("SubmitEnquiry", "enquiryAboutTitle");
                txtInputLayoutGeneralEnquiry1.Hint = Utility.GetLocalizedLabel("SubmitEnquiry", "messageHint").ToUpper();
                uploadSupportingDoc.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "uploadDocTitle");
                txtRelatedScreenshotTitle.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "attachTitle");
                txtMaxImageContent.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "attachDescription");
                btnNext.Text= Utility.GetLocalizedLabel("Common", "next");
           




            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

            // Create your application hereGetImageName
        }

        public override void Ready()
        {
            FileUtils.CreateDirectory(this, FileUtils.TEMP_IMAGE_FOLDER);
           


        }

        public string GetImageName(int itemCount)
        {
            SimpleDateFormat dateFormatter = new SimpleDateFormat("yyyyMMdd");
            Calendar calendar = Calendar.GetInstance(Locale.Default);
            return GetString(Resource.String.feedback_image_name_convention, dateFormatter.Format(calendar.TimeInMillis), UserSessions.GetCurrentImageCount(PreferenceManager.GetDefaultSharedPreferences(this)) + itemCount);
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
                                                    ApplicationContext.PackageName + ".provider", file);
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









        private void FeedBackCharacCount()
        {
            try
            {
                string feedback = txtGeneralEnquiry1.Text;
                int char_count = 0;

                if (!string.IsNullOrEmpty(feedback))
                {
                    char_count = feedback.Length;
                }

                if (char_count > 0)
                {
                    int char_left = Constants.FEEDBACK_CHAR_LIMIT - char_count;
                    txtInputLayoutGeneralEnquiry1.Error = string.Format(Utility.GetLocalizedCommonLabel("charactersLeft"), char_left);
                }
                else
                {
                    txtInputLayoutGeneralEnquiry1.Error = string.Format(Utility.GetLocalizedCommonLabel("charactersLeft"), Constants.FEEDBACK_CHAR_LIMIT);
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
                    ShowEmptyFeedbackError();
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
    
            txtInputLayoutGeneralEnquiry1.SetErrorTextAppearance(Resource.Style.TextInputLayoutBottomErrorHint);
            TextViewUtils.SetMuseoSans300Typeface(txtInputLayoutGeneralEnquiry1.FindViewById<TextView>(Resource.Id.textinput_error));
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
                txtInputLayoutGeneralEnquiry1.SetErrorTextAppearance(Resource.Style.TextInputLayoutFeedbackCount);
                txtInputLayoutGeneralEnquiry1.Error = null;

                TextViewUtils.SetMuseoSans300Typeface(txtInputLayoutGeneralEnquiry1.FindViewById<TextView>(Resource.Id.textinput_error));
                TextViewUtils.SetMuseoSans300Typeface(txtInputLayoutGeneralEnquiry1);
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