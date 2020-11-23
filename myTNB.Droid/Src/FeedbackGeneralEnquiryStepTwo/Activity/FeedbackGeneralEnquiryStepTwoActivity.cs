using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.Preferences;
using Android.Runtime;
using Android.Text;
using Android.Text.Style;
using Android.Views;
using Android.Widget;
using AndroidX.CoordinatorLayout.Widget;
using AndroidX.Core.Content;
using Castle.Core.Internal;
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
using myTNB_Android.Src.FeedbackGeneralEnquiryStepTwo.Model;
using myTNB_Android.Src.FeedbackGeneralEnquiryStepTwo.MVP;
using myTNB_Android.Src.SubmitEnquirySuccess.Activity;
using myTNB_Android.Src.UpdatePersonalDetailTnC.Activity;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;

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

        //[BindView(Resource.Id.txtPhoneNumber)]
        //EditText txtPhoneNumber;

        [BindView(Resource.Id.mobileNumberFieldContainer)]
        LinearLayout mobileNumberFieldContainer;

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

        //[BindView(Resource.Id.txtInputLayoutPhoneNumber)]
        //TextInputLayout txtInputLayoutPhoneNumber;

        public List<AttachedImage> attachedImages ;

        private  List<AttachedImage> attachList = new List<AttachedImage>();

        public List<FeedbackUpdateDetailsModel> feedbackUpdateDetailsModelList = new List<FeedbackUpdateDetailsModel>();

        public List<FeedbackUpdateDetailsModel> updateFeedbackList;

        private MobileNumberInputComponent mobileNumberInputComponent;

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
        private bool isMobileValidated = false;
        private bool isSelectCountry = false;

        const int COUNTRY_CODE_SELECT_REQUEST = 1;


        protected override void OnCreate(Android.OS.Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            try
            {   
                //1 set presenter
                mPresenter = new FeedbackGeneralEnquiryStepTwoPresenter(this);




                Android.OS.Bundle extras = Intent.Extras;
                

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

                TextViewUtils.SetMuseoSans300Typeface(txtInputLayoutName,txtInputLayoutEmail);

                txtName.AddTextChangedListener(new InputFilterFormField(txtName, txtInputLayoutName));
                txtEmail.AddTextChangedListener(new InputFilterFormField(txtName, txtInputLayoutEmail));


                TextViewUtils.SetMuseoSans500Typeface(btnSubmit , WhoShouldWeContact);


               //SET TRANSLATION
                
                txtInputLayoutName.Hint= Utility.GetLocalizedLabel("SubmitEnquiry", "nameHint");    
                txtInputLayoutEmail.Hint = Utility.GetLocalizedLabel("SubmitEnquiry", "emailHint");
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


                //bind listener with id and layout
                txtName.AddTextChangedListener(new InputFilterFormField(txtName, txtInputLayoutName));
                txtEmail.AddTextChangedListener(new InputFilterFormField(txtEmail, txtInputLayoutEmail));
  


                txtName.FocusChange += (sender, e) =>
                {
                 

                    if (txtName.HasFocus)
                    {   
                        txtInputLayoutName.SetErrorTextAppearance(TextViewUtils.SelectedFontSize() == "L" ? Resource.Style.TextInputLayoutFeedbackCountLarge : Resource.Style.TextInputLayoutFeedbackCount);
                        TextViewUtils.SetMuseoSans300Typeface(txtInputLayoutName.FindViewById<TextView>(Resource.Id.textinput_error));
                        txtInputLayoutName.Error = Utility.GetLocalizedLabel("SubmitEnquiry", "nameHintBottom");

                    }
                    else
                    {
                        
                        txtInputLayoutName.Error = null;
                    }
                
                };


                //mobile number section
                mobileNumberFieldContainer.RemoveAllViews();
                mobileNumberInputComponent = new MobileNumberInputComponent(this);
                mobileNumberInputComponent.SetOnTapCountryCodeAction(OnTapCountryCode);
                mobileNumberInputComponent.SetMobileNumberLabel(Utility.GetLocalizedCommonLabel("mobileNo"));
                mobileNumberInputComponent.SetSelectedCountry(CountryUtil.Instance.GetDefaultCountry());
                mobileNumberInputComponent.SetValidationAction(OnValidateMobileNumber);
                mobileNumberFieldContainer.AddView(mobileNumberInputComponent);

                


                //auto populate if login 
                if (UserEntity.IsCurrentlyActive())
                {
                    txtName.Text = UserEntity.GetActive().DisplayName;
                    txtEmail.Text = UserEntity.GetActive().Email;
                    string tempPhone = UserEntity.GetActive().MobileNo;



                    if (!tempPhone.IsNullOrEmpty())
                    {  
                        if (tempPhone.Contains("+"))
                        {
                            var CountryFromPhoneNumber = CountryUtil.Instance.GetCountryFromPhoneNumber(tempPhone);

                            if (!CountryFromPhoneNumber.ToString().IsNullOrEmpty())
                            {

                                mobileNumberInputComponent.SetSelectedCountry(CountryFromPhoneNumber);   // set flag and country
                                mobileNumberInputComponent.SetMobileNumber(int.Parse(tempPhone.Trim().Substring(CountryFromPhoneNumber.isd.Length)));  // set phone number
                            }
                        }
                        else
                        {
                            

                            if (tempPhone.Trim().Substring(0, 1) == "6")
                            {
                                mobileNumberInputComponent.SetMobileNumber(int.Parse(tempPhone.Trim().Substring(2)));
                            }
                            else
                            {
                                mobileNumberInputComponent.SetMobileNumber(int.Parse(tempPhone.Trim()));
                            }
                        }


                    }


                }
                else
                {
                    txtName.Text = "";
                }

                txtstep2of2.TextSize = TextViewUtils.GetFontSize(12f);
                WhoShouldWeContact.TextSize = TextViewUtils.GetFontSize(16f);
                txtName.TextSize = TextViewUtils.GetFontSize(16f);
                txtEmail.TextSize = TextViewUtils.GetFontSize(16f);
                txtTermsConditionsGeneralEnquiry.TextSize = TextViewUtils.GetFontSize(12f);

            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            try
            {
                base.OnActivityResult(requestCode, resultCode, data);
                if (resultCode == Result.Ok)
                {
                 
                   if (requestCode == COUNTRY_CODE_SELECT_REQUEST)
                    {
                        string dataString = data.GetStringExtra(Constants.SELECT_COUNTRY_CODE);
                        Country selectedCountry = JsonConvert.DeserializeObject<Country>(dataString);
                        mobileNumberInputComponent.SetSelectedCountry(selectedCountry);
                        isSelectCountry = true;
    }
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

        private void OnTapCountryCode()
        {
            Intent intent = new Intent(this, typeof(SelectCountryActivity));
            StartActivityForResult(intent, COUNTRY_CODE_SELECT_REQUEST);
        }

        private void OnValidateMobileNumber(bool isValidated)
        {
            if (isValidated)
            {
                isMobileValidated = true;
                mobileNumberInputComponent.ClearError();
            }
            else
            {
                isMobileValidated = false;

                if (mobileNumberInputComponent.GetIsFromCountryCode())
                {
                    // no implementation
                }
                else
                {
                    mobileNumberInputComponent.RaiseError(Utility.GetLocalizedLabel("SubmitEnquiry", "mobileReq"));
                }
                

            }

            passCheck();
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


        //todo review this logic
        public void UpdateMobileNumber(string mobile_no)
        {
            try
            {
                //if (txtPhoneNumber.Text != mobile_no)
                //{
                //  //  txtPhoneNumber.Text = mobile_no;  
                //}
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
              //  txtInputLayoutPhoneNumber.Error = null;
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
                // no implementation
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
        {   
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

                if (!txtEmail.Text.Trim().IsNullOrEmpty() && !txtName.Text.Trim().IsNullOrEmpty())
                {
                    this.SetIsClicked(true);
                    this.userActionsListener.NavigateToTermsAndConditions();
                }
                else if(txtEmail.Text.Trim().IsNullOrEmpty())
                {
                    ShowInvalidEmailError();
                }else if (txtName.Text.Trim().IsNullOrEmpty())
                {
                    ShowFullNameError();
                }


              
            }
        }

        public void ShowNavigateToTermsAndConditions()
        { 

            var tnc = new Intent(this, typeof(UpdatePersonalDetailTnCActivity));
            tnc.PutExtra(Constants.REQ_EMAIL, txtEmail.Text.Trim());
            tnc.PutExtra(Constants.SELECT_REGISTERED_OWNER, isOwner.ToString());
            tnc.PutExtra(Constants.ENTERED_NAME, txtName.Text.Trim());
           
            StartActivity(tnc);
        }

        public void ShowFullNameError()
        {
            txtInputLayoutName.SetErrorTextAppearance(TextViewUtils.SelectedFontSize() == "L" ? Resource.Style.TextInputLayoutBottomErrorHintLarge : Resource.Style.TextInputLayoutBottomErrorHint);

           
            TextViewUtils.SetMuseoSans300Typeface(txtInputLayoutName.FindViewById<TextView>(Resource.Id.textinput_error));
            txtInputLayoutName.Error = Utility.GetLocalizedErrorLabel("invalid_fullname");
            var handleBounceError = txtInputLayoutName.FindViewById<TextView>(Resource.Id.textinput_error);
            handleBounceError.SetPadding(top: 4, left: 0, right: 0, bottom: 0);


        }
        public void DisableRegisterButton()
        {
            btnSubmit.Enabled = false;
            btnSubmit.Background = ContextCompat.GetDrawable(this, Resource.Drawable.silver_chalice_button_background);
        }

        public void ClearFullNameError()
        {

            if (txtName.HasFocus)
            {
                txtInputLayoutName.SetErrorTextAppearance(TextViewUtils.SelectedFontSize() == "L" ? Resource.Style.TextInputLayoutFeedbackCountLarge : Resource.Style.TextInputLayoutFeedbackCount);
                TextViewUtils.SetMuseoSans300Typeface(txtInputLayoutName.FindViewById<TextView>(Resource.Id.textinput_error));
                txtInputLayoutName.Error = Utility.GetLocalizedLabel("SubmitEnquiry", "nameHintBottom");
                var handleBounceError = txtInputLayoutName.FindViewById<TextView>(Resource.Id.textinput_error);
                handleBounceError.SetPadding(top: 4, left: 0, right: 0, bottom: 0);
            }
            else
            {
                txtInputLayoutName.Error = null;
            }
           
        }

        public void ShowInvalidEmailError()
        {
            txtInputLayoutEmail.SetErrorTextAppearance(TextViewUtils.SelectedFontSize() == "L" ? Resource.Style.TextInputLayoutBottomErrorHintLarge : Resource.Style.TextInputLayoutBottomErrorHint);
           
            TextViewUtils.SetMuseoSans300Typeface(txtInputLayoutEmail.FindViewById<TextView>(Resource.Id.textinput_error));
            TextViewUtils.SetMuseoSans300Typeface(txtInputLayoutEmail);
            txtInputLayoutEmail.Error = Utility.GetLocalizedErrorLabel("invalid_email");
            var handleBounceError = txtInputLayoutEmail.FindViewById<TextView>(Resource.Id.textinput_error);
            handleBounceError.SetPadding(top: 4, left: 0, right: 0, bottom: 0);

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
                    Android.OS.Handler h = new Android.OS.Handler();
                    Action myAction = () =>
                    {
                        btnSubmit.Enabled = true;
                    };
                    h.PostDelayed(myAction, 3000);

                    int ownerRelationshipID =0;
                    
                    // ensure not from feedback and owner must be false to pass this parameter
                    if(feedback.IsNullOrEmpty() && isOwner == false)
                    {
                        if (ownerRelationship == Utility.GetLocalizedLabel("SubmitEnquiry", "childTitle"))
                        {
                            ownerRelationshipID = 1;
                        }
                        else if (ownerRelationship == Utility.GetLocalizedLabel("SubmitEnquiry", "tenantTitle"))
                        {
                            ownerRelationshipID = 2;
                        }
                        else if (ownerRelationship == Utility.GetLocalizedLabel("SubmitEnquiry", "guardianTitle"))
                        {
                            ownerRelationshipID = 3;
                        }
                        else if (ownerRelationship == Utility.GetLocalizedLabel("SubmitEnquiry", "parentTitle"))
                        {
                            ownerRelationshipID = 4;
                        }
                        else if (ownerRelationship == Utility.GetLocalizedLabel("SubmitEnquiry", "spouseTitle"))
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


                   string txtPhoneNumber  = mobileNumberInputComponent.GetMobileNumberValueWithISDCode();
                  
                   this.userActionsListener.OnSubmit(acc, feedback, txtName.Text.ToString().Trim(), txtPhoneNumber.ToString().Trim(),txtEmail.Text.Trim(), attachedImages, updateFeedbackList, isOwner, ownerRelationshipID,  ownerRelationship);


                  

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

            if (toggleTncData)
            {
                txtTermsConditionsGeneralEnquiry.TextFormatted = GetFormattedText(Utility.GetLocalizedLabel("SubmitEnquiry", "enquiryTncRead"));
                StripUnderlinesFromLinks(txtTermsConditionsGeneralEnquiry);
            }
            else
            {
                txtTermsConditionsGeneralEnquiry.TextFormatted = GetFormattedText(Utility.GetLocalizedLabel("SubmitEnquiry", "enquiryTnc"));
                StripUnderlinesFromLinks(txtTermsConditionsGeneralEnquiry);
            }

            passCheck();
        }

        public void passCheck()
        {

            string fullname = txtName.Text.ToString().Trim();
            bool mobile_no = isMobileValidated;
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

                if (attachedImage.Name.ToLower().Contains("pdf"))
                {
                    try
                    {
                        byte[] pdfByteData = FileUtils.GetPDFByte(this, attachedImage.Path);

                        int size = pdfByteData.Length;
                        string hexString = HttpUtility.UrlEncode(FileUtils.ByteArrayToString(pdfByteData), Encoding.UTF8);
                        System.Console.WriteLine(string.Format("Hex string {0}", hexString));
                        return new AttachedImageRequest()
                        {
                            ImageHex = hexString,
                            FileSize = size,
                            FileName = attachedImage.Name
                        };
                    }
                    catch (Exception e)
                    {
                        Utility.LoggingNonFatalError(e);

                        Bitmap bitmap = BitmapFactory.DecodeFile(attachedImage.Path, bmOptions);
                        byte[] imageBytes = FileUtils.GetCompress(this, bitmap);

                        int size = imageBytes.Length;
                        string hexString = HttpUtility.UrlEncode(FileUtils.ByteArrayToString(imageBytes), Encoding.UTF8);
                        if (bitmap != null && !bitmap.IsRecycled)
                        {
                            bitmap.Recycle();
                        }
                        System.Console.WriteLine(string.Format("Hex string {0}", hexString));
                        return new AttachedImageRequest()
                        {
                            ImageHex = hexString,
                            FileSize = size,
                            FileName = attachedImage.Name
                        };
                    }

                }
                else {
                    Bitmap bitmap = BitmapFactory.DecodeFile(attachedImage.Path, bmOptions);
                    byte[] imageBytes = FileUtils.GetCompress(this, bitmap);
                
                    int size = imageBytes.Length;
                    string hexString = HttpUtility.UrlEncode(FileUtils.ByteArrayToString(imageBytes), Encoding.UTF8);
                    if (bitmap != null && !bitmap.IsRecycled)
                    {
                        bitmap.Recycle();
                    }
                    System.Console.WriteLine(string.Format("Hex string {0}", hexString));
                    return new AttachedImageRequest()
                    {
                        ImageHex = hexString,
                        FileSize = size,
                        FileName = attachedImage.Name
                    };
                }

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
            
            //StartActivityForResult(successIntent, Constants.REQUEST_FEEDBACK_SUCCESS_VIEW);
        }




    }
}