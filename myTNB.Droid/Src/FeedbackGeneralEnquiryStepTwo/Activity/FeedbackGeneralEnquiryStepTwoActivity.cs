using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.Icu.Text;
using Android.Opengl;
using Android.OS;
using Android.Preferences;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.Content;
using Android.Text;
using Android.Text.Style;
using Android.Views;
using Android.Widget;
using Castle.Core.Internal;
using CheeseBind;
using Java.Util;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Base.Models;
using myTNB_Android.Src.Base.Request;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.FeedbackGeneralEnquiryStepTwo.Model;
using myTNB_Android.Src.FeedbackGeneralEnquiryStepTwo.MVP;
using myTNB_Android.Src.FeedbackSuccess.Activity;
using myTNB_Android.Src.SubmitEnquirySuccess.Activity;
using myTNB_Android.Src.TermsAndConditions.Activity;
using myTNB_Android.Src.UpdatePersonalDetailTnC.Activity;
using myTNB_Android.Src.Utils;

namespace myTNB_Android.Src.FeedbackGeneralEnquiryStepTwo.Activity
{

    [Activity(Label = "@string/GeneralEnquiry2of2_app_bar"
   , ScreenOrientation = ScreenOrientation.Portrait
           , WindowSoftInputMode = SoftInput.AdjustPan
   , Theme = "@style/Theme.FaultyStreetLamps")]


    public class FeedbackGeneralEnquiryStepTwoActivity : BaseActivityCustom, FeedbackGeneralEnquiryStepTwoContract.IView
    {

        FeedbackGeneralEnquiryStepTwoContract.IUserActionsListener userActionsListener;
        FeedbackGeneralEnquiryStepTwoPresenter mPresenter;

        


        [BindView(Resource.Id.rootView)]
        CoordinatorLayout rootView;


        [BindView(Resource.Id.WhoShouldWeContact)]
        TextView WhoShouldWeContact;

        [BindView(Resource.Id.txtName)]
        EditText txtName;

        [BindView(Resource.Id.txtEmail)]
        EditText txtEmail;

        [BindView(Resource.Id.txtPhoneNumber)]
        EditText txtPhoneNumber;

        [BindView(Resource.Id.txtstep2of2)]
        TextView txtstep2of2;

        [BindView(Resource.Id.LinearLayout_TNC)]
        LinearLayout LinearLayout_TNC;

        [BindView(Resource.Id.agreementCheckbox)]
        CheckBox agreementCheckbox;

        


        [BindView(Resource.Id.txtTermsConditionsGeneralEnquiry)]
        TextView txtTermsConditionsGeneralEnquiry;

        [BindView(Resource.Id.btnSubmit)]
        Button btnSubmit;

        [BindView(Resource.Id.txtInputLayoutName)]
        TextInputLayout txtInputLayoutName;

        [BindView(Resource.Id.txtInputLayoutEmail)]
        TextInputLayout txtInputLayoutEmail;

        [BindView(Resource.Id.txtInputLayoutPhoneNumber)]
        TextInputLayout txtInputLayoutPhoneNumber;

        public List<AttachedImage> attachedImages ;

        private  List<AttachedImage> attachList = new List<AttachedImage>();

        public List<FeedbackUpdateDetailsModel> feedbackUpdateDetailsModelList = new List<FeedbackUpdateDetailsModel>();

        public List<FeedbackUpdateDetailsModel> updateFeedbackList;

        private string feedback = null;

        private string acc = null;

        const string PAGE_ID = "Register";

        private bool isOwner = false;
        private string ownerRelationship;
        private string icNumber;
        private string accOwnerName;
        private string mobileNumber;
        private string emailAddress;
        private string mailingAddress;
        private string premiseAddress;
        private bool toggleTncData = false;
        private bool isNeedTNC = false;


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            try
            {   
                //1 set presenter
                mPresenter = new FeedbackGeneralEnquiryStepTwoPresenter(this);

         
              

                Bundle extras = Intent.Extras;
                

                if (extras != null)
                {
                    ///PAGE TITLE FROM BEFORE PAGE
                    if (extras.ContainsKey(Constants.PAGE_TITLE))
                    {
                        SetToolBarTitle(extras.GetString(Constants.PAGE_TITLE));
                    }


                    if (extras.ContainsKey(Constants.PAGE_STEP_TITLE))
                    {
                        txtstep2of2.Text = extras.GetString(Constants.PAGE_STEP_TITLE);
                    }


                        /// general enquiry
                    if (extras.ContainsKey("FEEDBACK"))
                    {
                        feedback=extras.GetString("FEEDBACK");
                    }
                    if (extras.ContainsKey("IMAGE"))
                    {
                        attachedImages = DeSerialze<List<AttachedImage>>(extras.GetString("IMAGE"));
                    }
                    if (extras.ContainsKey(Constants.ACCOUNT_NUMBER))
                    {
                        acc = extras.GetString(Constants.ACCOUNT_NUMBER);
                    }

                    ///update personal info

                    if (extras.ContainsKey(Constants.SELECT_REGISTERED_OWNER))
                    {
                        isOwner = bool.Parse(extras.GetString(Constants.SELECT_REGISTERED_OWNER));

                    }

                    if (isOwner)
                    {
                        // add image 
                        attachList.Add(DeSerialze<List<AttachedImage>>(extras.GetString(Constants.IMAGE_OWNER))[0]);
                        if (extras.ContainsKey(Constants.ACCOUNT_PREMISE_ADDRESS))
                        {
                            attachList.Add(DeSerialze<List<AttachedImage>>(extras.GetString(Constants.IMAGE_PERMISES))[0]);
                        }
                        attachedImages = attachList;

                    }

                    if (!isOwner)
                    {
                        if (extras.ContainsKey(Constants.OWNER_RELATIONSHIP))
                        {
                            ownerRelationship = extras.GetString(Constants.OWNER_RELATIONSHIP);


                            attachList.Add(DeSerialze<List<AttachedImage>>(extras.GetString(Constants.IMAGE_OWNER))[0]);
                            attachList.Add(DeSerialze<List<AttachedImage>>(extras.GetString(Constants.IMAGE_OWN))[0]);
                            attachList.Add(DeSerialze<List<AttachedImage>>(extras.GetString(Constants.IMAGE_SUPPORTING_DOC))[0]);
                            if (extras.ContainsKey(Constants.ACCOUNT_PREMISE_ADDRESS))
                            {
                                attachList.Add(DeSerialze<List<AttachedImage>>(extras.GetString(Constants.IMAGE_PERMISES))[0]);
                            }
                           
                                
                                attachedImages = attachList;


                        }

                    }
                    if (extras.ContainsKey(Constants.ACCOUNT_IC_NUMBER))
                    {
                        icNumber = extras.GetString(Constants.ACCOUNT_IC_NUMBER);
                        FeedbackUpdateDetailsModel icUpdate = new FeedbackUpdateDetailsModel()
                        {
                            FeedbackUpdInfoType = 1,
                            FeedbackUpdInfoTypeDesc = Utility.GetLocalizedLabel("SubmitEnquiry", "icTitle"),
                            FeedbackUpdInfoValue = icNumber,
                        };
                        feedbackUpdateDetailsModelList.Add(icUpdate);
                        
                    }
                    if (extras.ContainsKey(Constants.ACCOUNT_OWNER_NAME))
                    {
                        accOwnerName = extras.GetString(Constants.ACCOUNT_OWNER_NAME);
                        FeedbackUpdateDetailsModel accountNameUpdate = new FeedbackUpdateDetailsModel()
                        {
                            FeedbackUpdInfoType = 2,
                            FeedbackUpdInfoTypeDesc = Utility.GetLocalizedLabel("SubmitEnquiry", "accNametitle"),
                            FeedbackUpdInfoValue = accOwnerName,
                        };
                        feedbackUpdateDetailsModelList.Add(accountNameUpdate);
                    }

                    if (extras.ContainsKey(Constants.ACCOUNT_MOBILE_NUMBER))
                    {
                        mobileNumber = extras.GetString(Constants.ACCOUNT_MOBILE_NUMBER);
                        FeedbackUpdateDetailsModel mobileUpdate = new FeedbackUpdateDetailsModel()
                        {
                            FeedbackUpdInfoType = 3,
                            FeedbackUpdInfoTypeDesc = Utility.GetLocalizedLabel("SubmitEnquiry", "mobileNumberTitle"),
                            FeedbackUpdInfoValue = mobileNumber,
                        };
                        feedbackUpdateDetailsModelList.Add(mobileUpdate);
                    }

                    if (extras.ContainsKey(Constants.ACCOUNT_EMAIL_ADDRESS))
                    {
                        emailAddress = extras.GetString(Constants.ACCOUNT_EMAIL_ADDRESS);
                        FeedbackUpdateDetailsModel emailUpdate = new FeedbackUpdateDetailsModel()
                        {
                            FeedbackUpdInfoType = 4,
                            FeedbackUpdInfoTypeDesc = Utility.GetLocalizedLabel("SubmitEnquiry", "emailAddressTitle"),
                            FeedbackUpdInfoValue = emailAddress,
                        };
                        feedbackUpdateDetailsModelList.Add(emailUpdate);
                    }

                    if (extras.ContainsKey(Constants.ACCOUNT_MAILING_ADDRESS))
                    {
                        mailingAddress = extras.GetString(Constants.ACCOUNT_MAILING_ADDRESS);

                        FeedbackUpdateDetailsModel mailingUpdate = new FeedbackUpdateDetailsModel()
                        {
                            FeedbackUpdInfoType = 5,
                            FeedbackUpdInfoTypeDesc = Utility.GetLocalizedLabel("SubmitEnquiry", "mailingAddressTitle"),
                            FeedbackUpdInfoValue = mailingAddress,
                        };
                        feedbackUpdateDetailsModelList.Add(mailingUpdate);
                    }
                    if (extras.ContainsKey(Constants.ACCOUNT_PREMISE_ADDRESS))
                    {
                        premiseAddress = extras.GetString(Constants.ACCOUNT_PREMISE_ADDRESS);


                        FeedbackUpdateDetailsModel premiseUpdate = new FeedbackUpdateDetailsModel()
                        {
                            FeedbackUpdInfoType = 6,
                            FeedbackUpdInfoTypeDesc = Utility.GetLocalizedLabel("SubmitEnquiry", "premiseAddressTitle"),
                            FeedbackUpdInfoValue = premiseAddress,
                        };
                        feedbackUpdateDetailsModelList.Add(premiseUpdate);
                    }

                    updateFeedbackList = feedbackUpdateDetailsModelList;

                }


                //2 set font type , 300 normal 500 button
                TextViewUtils.SetMuseoSans300Typeface(txtInputLayoutName, txtInputLayoutEmail, txtInputLayoutPhoneNumber);
                TextViewUtils.SetMuseoSans300Typeface(txtPhoneNumber, txtEmail, txtName);

                //, txtTermsConditionsGeneralEnquiry
                TextViewUtils.SetMuseoSans500Typeface(btnSubmit , WhoShouldWeContact);


                //SET TRANSLATION
                txtInputLayoutName.Hint= Utility.GetLocalizedLabel("SubmitEnquiry", "nameHint");
                txtInputLayoutEmail.Hint = Utility.GetLocalizedLabel("SubmitEnquiry", "emailHint");
                txtInputLayoutPhoneNumber.Hint= Utility.GetLocalizedLabel("SubmitEnquiry", "mobileHint");
                btnSubmit.Text = Utility.GetLocalizedLabel("Common", "submit");


                //set translation of string 
                 txtTermsConditionsGeneralEnquiry.TextFormatted = GetFormattedText(Utility.GetLocalizedLabel("SubmitEnquiry", "enquiryTnc"));
                 StripUnderlinesFromLinks(txtTermsConditionsGeneralEnquiry);


                WhoShouldWeContact.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "contactEnquiryTitle");

                /// cater on is need tnc or not
                if (!feedback.IsNullOrEmpty())
                {
                    LinearLayout_TNC.Visibility = ViewStates.Gone;
                    isNeedTNC = false;


                }
                else
                {  /// feedback is null so this is from update feedback page
                    isNeedTNC = true;
                }


                // bind text change 

                txtName.TextChanged += TextChange;
                txtEmail.TextChanged += TextChange;   
                txtPhoneNumber.TextChanged += TextChange;

                //bind listener with id and layout
                txtName.AddTextChangedListener(new InputFilterFormField(txtName, txtInputLayoutName));
                txtEmail.AddTextChangedListener(new InputFilterFormField(txtEmail, txtInputLayoutEmail));
                txtPhoneNumber.AddTextChangedListener(new InputFilterFormField(txtPhoneNumber, txtInputLayoutPhoneNumber));


                //enforce 60
                UpdateMobileNumber("+60");

                //auto populate if login 
                if (UserEntity.IsCurrentlyActive())
                {
                    txtName.Text = UserEntity.GetActive().DisplayName;
                    txtEmail.Text = UserEntity.GetActive().Email;
                    txtPhoneNumber.Text = UserEntity.GetActive().MobileNo;

                }


            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void StripUnderlinesFromLinks(TextView textView)
        {
            var spannable = new SpannableStringBuilder(textView.TextFormatted);
            var spans = spannable.GetSpans(0, spannable.Length(), Java.Lang.Class.FromType(typeof(URLSpan)));
            foreach (URLSpan span in spans)
            {
                var start = spannable.GetSpanStart(span);
                var end = spannable.GetSpanEnd(span);
                spannable.RemoveSpan(span);
                var newSpan = new URLSpanNoUnderline(span.URL);
                spannable.SetSpan(newSpan, start, end, 0);
            }
            textView.TextFormatted = spannable;
        }

        private void TextChange(object sender, TextChangedEventArgs e)
        {
            try
            {
                passCheck();

            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }

        class URLSpanNoUnderline : URLSpan
        {
            public URLSpanNoUnderline(string url) : base(url)
            {
            }

            public override void UpdateDrawState(TextPaint ds)
            {
                base.UpdateDrawState(ds);
                ds.UnderlineText = false;
            }
        }

        public void UpdateMobileNumber(string mobile_no)
        {
            try
            {
                if (txtPhoneNumber.Text != mobile_no)
                {
                    txtPhoneNumber.Text = mobile_no;
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }


        public void ClearInvalidMobileError()
        {
            try
            {
                txtInputLayoutPhoneNumber.Error = null;
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void ShowInvalidMobileNoError()
        {
            try
            {
                txtInputLayoutPhoneNumber.Error = Utility.GetLocalizedErrorLabel("invalid_mobileNumber");
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }


        public override string GetPageId()
        {
            return PAGE_ID;
        }

        public override int ResourceId()
        {   //todo change
            return Resource.Layout.FeedbackGeneralEnquiryStepTwoView;
        }

        public void SetPresenter(FeedbackGeneralEnquiryStepTwoContract.IUserActionsListener userActionListener)
        {
            this.userActionsListener = userActionListener;
        }

        public bool IsActive()
        {
            return Window.DecorView.RootView.IsShown;
        }

        public override Boolean ShowCustomToolbarTitle()
        {
            return true;
        }

        [OnClick(Resource.Id.txtTermsConditionsGeneralEnquiry)]
        void OnTermsConditions(object sender, EventArgs eventArgs)
        {
            if (!this.GetIsClicked())
            {

                if (!txtEmail.Text.Trim().IsNullOrEmpty())
                {
                    this.SetIsClicked(true);
                    this.userActionsListener.NavigateToTermsAndConditions();
                }
                else
                {
                    ShowInvalidEmailError();
                }


              
            }
        }

        public void ShowNavigateToTermsAndConditions()
        { 

            var tnc = new Intent(this, typeof(UpdatePersonalDetailTnCActivity));
            tnc.PutExtra(Constants.REQ_EMAIL, txtEmail.Text.Trim());
           // tnc.PutExtra(Constants.REQ_IC,  );
           // tnc.PutExtra(Constants.ACCOUNT_NUMBER, acc);
            StartActivity(tnc);
        }

        public void ShowFullNameError()
        {
            //txtInputLayoutNamee = GetString(Resource.String.name_error);
            txtInputLayoutName.Error = Utility.GetLocalizedErrorLabel("invalid_fullname");

        }



        public void DisableRegisterButton()
        {
            btnSubmit.Enabled = false;
            btnSubmit.Background = ContextCompat.GetDrawable(this, Resource.Drawable.silver_chalice_button_background);
        }

        public void ClearFullNameError()
        {
            txtInputLayoutName.Error = null;
        }

        public void ShowInvalidEmailError()
        {
            txtInputLayoutEmail.Error = Utility.GetLocalizedErrorLabel("invalid_email");
        }

        public void ClearInvalidEmailError()
        {
            txtInputLayoutEmail.Error = null;
        }



        public void EnableRegisterButton()
        {
            btnSubmit.Enabled = true;
            btnSubmit.Background = ContextCompat.GetDrawable(this, Resource.Drawable.green_button_background);
        }


        [OnClick(Resource.Id.agreementCheckbox)]
        void OnTnc(object sender, EventArgs eventArgs)
        {
            toggleTNC();
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
                    Handler h = new Handler();
                    Action myAction = () =>
                    {
                        btnSubmit.Enabled = true;
                    };
                    h.PostDelayed(myAction, 3000);

                    int ownerRelationshipID =0;
                    
                    // ensure not from feedback and owner must be false to pass this parameter
                    if(feedback.IsNullOrEmpty() && isOwner == false)
                    {
                        if (ownerRelationship == Utility.GetLocalizedLabel("Common", "childTitle"))
                        {
                            ownerRelationshipID = 1;
                        }
                        else if (ownerRelationship == Utility.GetLocalizedLabel("Common", "tenantTitle"))
                        {
                            ownerRelationshipID = 2;
                        }
                        else if (ownerRelationship == Utility.GetLocalizedLabel("Common", "guardianTitle"))
                        {
                            ownerRelationshipID = 3;
                        }
                        else if (ownerRelationship == Utility.GetLocalizedLabel("Common", "parentTitle"))
                        {
                            ownerRelationshipID = 4;
                        }
                        else if (ownerRelationship == Utility.GetLocalizedLabel("Common", "spouseTitle"))
                        {
                            ownerRelationshipID = 5;
                        }
                        else
                        {
                            ownerRelationshipID = 6;
                        }

                    }

                    // to ensure feedback is emty if null
                    if (feedback.IsNullOrEmpty())
                    {
                        feedback = "";
                    }
                   




                    this.userActionsListener.OnSubmit(acc, feedback, txtName.Text.ToString().Trim(), txtPhoneNumber.Text.ToString().Trim(),txtEmail.Text.Trim(), attachedImages, updateFeedbackList, isOwner, ownerRelationshipID,  ownerRelationship);


                  

                }
            }
            catch (Exception e)
            {
                this.SetIsClicked(false);
                Utility.LoggingNonFatalError(e);
            }
        }


        public void toggleTNC()
        {
            toggleTncData = !toggleTncData;  //boolean change

            passCheck();
        }

        public void passCheck()
        {

            string fullname = txtName.Text.ToString().Trim();
            string mobile_no = txtPhoneNumber.Text.ToString().Trim();
            string email = txtEmail.Text.ToString().Trim();
            bool tnc = toggleTncData;
            bool varNeedTNC = isNeedTNC;

            this.userActionsListener.CheckRequiredFields(fullname, mobile_no, email, tnc , varNeedTNC);

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

        public string GetImageName(int itemCount)
        {
            SimpleDateFormat dateFormatter = new SimpleDateFormat("yyyyMMdd");
            Calendar calendar = Calendar.GetInstance(Locale.Default);
            return GetString(Resource.String.feedback_image_name_convention, dateFormatter.Format(calendar.TimeInMillis), UserSessions.GetCurrentImageCount(PreferenceManager.GetDefaultSharedPreferences(this)) + itemCount);
        }

        public   Task<AttachedImageRequest> SaveImage(AttachedImage attachedImage)
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

        //need to change to other screen
        public void ShowSuccess(string date, string feedbackId, int imageCount)
        {
            ISharedPreferences sharedPref = PreferenceManager.GetDefaultSharedPreferences(this);
            int currentCount = UserSessions.GetCurrentImageCount(sharedPref);
            UserSessions.SetCurrentImageCount(sharedPref, currentCount + imageCount);
            Finish();

            //public void showSuccess(string feedbackId)
            //{
            //    var successIntent = new Intent(this, typeof(SubmitEnquirySuccessActivity));
            //    successIntent.PutExtra(Constants.RESPONSE_FEEDBACK_ID, feedbackId);
            //    StartActivityForResult(successIntent, Constants.REQUEST_FEEDBACK_SUCCESS_VIEW);

            //}

            var successIntent = new Intent(this, typeof(SubmitEnquirySuccessActivity));
            successIntent.PutExtra(Constants.RESPONSE_FEEDBACK_DATE, date);
            successIntent.PutExtra(Constants.RESPONSE_FEEDBACK_ID, feedbackId);
            StartActivity(successIntent);
        }




    }
}