using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics.Drawables;
using Android.Opengl;
using Android.OS;
using Android.Runtime;
using Android.Text;
using Android.Views;
using Android.Widget;
using AndroidX.Core.Content;
using Castle.Core.Internal;
using CheeseBind;
using Google.Android.Material.TextField;
using myTNB_Android.Src.Base;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Common.Activity;
using myTNB_Android.Src.Common.Model;
using myTNB_Android.Src.CompoundView;
using myTNB_Android.Src.UpdatePersonalDetailStepOne.Fragment;
using myTNB_Android.Src.UpdatePersonalDetailStepOne.Model;
using myTNB_Android.Src.UpdatePersonalDetailStepOne.MVP;
using myTNB_Android.Src.UpdatePersonalDetailStepTwo.Activity;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;
using NSubstitute.Core;
using static myTNB_Android.Src.UpdatePersonalDetailStepOne.Fragment.UpdatePersonalDetailStepOneSelectRelationshipFragment;

namespace myTNB_Android.Src.UpdatePersonalDetailStepOne.Activity


{

    [Activity(Label = "Update Personal Details"
      , ScreenOrientation = ScreenOrientation.Portrait
              , WindowSoftInputMode = SoftInput.AdjustPan
      , Theme = "@style/Theme.FaultyStreetLamps")]
    public class UpdatePersonalDetailStepOneActivity : BaseToolbarAppCompatActivity, UpdatePersonalDetailStepOneContract.IView
    {

        UpdatePersonalDetailStepOnePresenter mPresenter;
        UpdatePersonalDetailStepOneContract.IUserActionsListener userActionsListener;
        const string PAGE_ID = "UpdatePersonalDetailsFirstStep";


        [BindView(Resource.Id.TextView_whatIsYourRelationship)]
        TextView TextView_whatIsYourRelationship;

        [BindView(Resource.Id.txtAccountType)]
        TextView txtAccountType;

        [BindView(Resource.Id.selector_account_type)]
        TextView selector_account_type;

        [BindView(Resource.Id.txtInputLayoutRelationshipOther)]
        TextInputLayout txtInputLayoutRelationshipOther;

        [BindView(Resource.Id.EditText_relationshipOther)]
        EditText EditText_relationshipOther;

        [BindView(Resource.Id.infotxtTitle_Which_information)]
        TextView infotxtTitle_Which_information;

        [BindView(Resource.Id.txtIC)]
        TextView txtIC;

        [BindView(Resource.Id.txtInputLayoutNewIC)]
        TextInputLayout txtInputLayoutNewIC;

        [BindView(Resource.Id.txtNewIC)]
        EditText txtNewIC;


        [BindView(Resource.Id.txtOwnerName)]
        TextView txtOwnerName;

        [BindView(Resource.Id.pageStep)]
        TextView pageStep;

        [BindView(Resource.Id.txtInputLayoutNewOwnerName)]
        TextInputLayout txtInputLayoutNewOwnerName;

        [BindView(Resource.Id.txtNewOwnerName)]
        EditText txtNewOwnerName;

        [BindView(Resource.Id.txtMobileNumber)]
        TextView txtMobileNumber;

        [BindView(Resource.Id.txtEmailAddress)]
        TextView txtEmailAddress;

        [BindView(Resource.Id.txtInputLayoutCurrentEmailAddress)]
        TextInputLayout txtInputLayoutCurrentEmailAddress;

        [BindView(Resource.Id.txtGeneralCurrentEmailAddress)]
        EditText txtGeneralCurrentEmailAddress;

        [BindView(Resource.Id.txtInputLayoutNewEmailAddress)]
        TextInputLayout txtInputLayoutNewEmailAddress;

        [BindView(Resource.Id.txtNewEmailAddress)]
        EditText txtNewEmailAddress;

        [BindView(Resource.Id.txtMailingAddress)]
        TextView txtMailingAddress;

        [BindView(Resource.Id.txtInputLayoutCurrentMailingAddress)]
        TextInputLayout txtInputLayoutCurrentMailingAddress;

        [BindView(Resource.Id.txtGeneralCurrentMailingAddress)]
        EditText txtGeneralCurrentMailingAddress;

        [BindView(Resource.Id.txtInputLayoutNewMailingAddress)]
        TextInputLayout txtInputLayoutNewMailingAddress;

        [BindView(Resource.Id.txtNewMailingAddress)]
        EditText txtNewMailingAddress;

        [BindView(Resource.Id.txtGeneralCurrentICNumber)]
        EditText txtGeneralCurrentICNumber;

        [BindView(Resource.Id.txtGeneralCurrentOwnerName)]
        EditText txtGeneralCurrentOwnerName;

        [BindView(Resource.Id.txtPremiseAddress)]
        TextView txtPremiseAddress;


        [BindView(Resource.Id.txtInputLayoutCurrentPremiseAddress)]
        TextInputLayout txtInputLayoutCurrentPremiseAddress;

        [BindView(Resource.Id.txtGeneralCurrentPremiseAddress)]
        EditText txtGeneralCurrentPremiseAddress;

        [BindView(Resource.Id.txtInputLayoutNewPremiseAddress)]
        TextInputLayout txtInputLayoutNewPremiseAddress;

        [BindView(Resource.Id.txtNewPremiseAddress)]
        EditText txtNewPremiseAddress;

        [BindView(Resource.Id.btnNext)]
        Button btnNext;

        [BindView(Resource.Id.LinearLayout_relationShipOwner)]
        LinearLayout LinearLayout_relationShipOwner;

        [BindView(Resource.Id.LinearLayout_IC)]
        LinearLayout LinearLayout_IC;

        [BindView(Resource.Id.LinearLayout_OwnerName)]
        LinearLayout LinearLayout_OwnerName;

        [BindView(Resource.Id.LinearLayout_MobileNumber)]
        LinearLayout LinearLayout_MobileNumber;

        [BindView(Resource.Id.LinearLayout_EmailAddress)]
        LinearLayout LinearLayout_EmailAddress;

        [BindView(Resource.Id.LinearLayout_MailingAddress)]
        LinearLayout LinearLayout_MailingAddress;

        [BindView(Resource.Id.LinearLayout_PremiseAddress)]
        LinearLayout LinearLayout_PremiseAddress;

        [BindView(Resource.Id.TextView_updateOnOwnerBehalf)]
        TextView TextView_updateOnOwnerBehalf;

        [BindView(Resource.Id.infoLabelWhoIsRegistered)]
        LinearLayout infoLabelWhoIsRegistered;

        [BindView(Resource.Id.infoLabelDoIneedOwnerConsent)]
        LinearLayout infoLabelDoIneedOwnerConsent;


        [BindView(Resource.Id.btnNo)]
        Button btnNo;

        [BindView(Resource.Id.btnYes)]
        Button btnYes;


        [BindView(Resource.Id.spacingifSelectTypeIsNotother)]
        View spacingifSelectTypeIsNotother;


        [BindView(Resource.Id.infotxtIsThisAcc)]
        TextView infotxtIsThisAcc;


        [BindView(Resource.Id.selectICChkBox)]
        CheckBox selectICChkBox;

        [BindView(Resource.Id.selectOwnerNameChkBox)]
        CheckBox selectOwnerNameChkBox;

        [BindView(Resource.Id.mobile_number_chk_box)]
        CheckBox mobile_number_chk_box;

        [BindView(Resource.Id.select_email_address_chk_box)]
        CheckBox select_email_address_chk_box;

        [BindView(Resource.Id.select_mailing_address_chk_box)]
        CheckBox select_mailing_address_chk_box;

        [BindView(Resource.Id.premiseAddress_chk_box)]
        CheckBox premiseAddress_chk_box;

        [BindView(Resource.Id.infoLabeltxtWhoIsRegistered)]
        TextView infoLabeltxtWhoIsRegistered;

        [BindView(Resource.Id.TextViewDoINeedOwnerConsent)]
        TextView TextViewDoINeedOwnerConsent;

        [BindView(Resource.Id.mobileNumberFieldContainer)]
        LinearLayout mobileNumberFieldContainer;

        private MobileNumberInputComponent mobileNumberInputComponent;

        private bool toggleChkBoxIC = false;

        private bool toggleChkOwnerName = false;

        private bool toggleChkMobileNumber = false;

        private bool toggleChkEmailAddress = false;

        private bool toggleChkMailingAddress = false;

        private bool toggleChkPremiseAddress = false;

        private bool isMobileNumberValidated = false;

        private bool isRegisteredOwner;

        private string ownerRelationship;



        private int toggleCounter = 0;

        private SelectRelationshipModel selectedAccountRelationship;

        private bool isOtherChoosed = false;

        private readonly int SELECT_ACCOUNT_TYPE_REQ_CODE = 2012;

        private string  caNumber;

        private string pageTitle;

        private string pageStepTitle;

        const int COUNTRY_CODE_SELECT_REQUEST = 1;

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
                        caNumber = extras.GetString(Constants.ACCOUNT_NUMBER);
                    }

                    if (extras.ContainsKey(Constants.PAGE_TITLE))
                    {
                        pageTitle = extras.GetString(Constants.PAGE_TITLE);
                    }
                    if (extras.ContainsKey(Constants.PAGE_STEP_TITLE))
                    {
                        pageStepTitle = extras.GetString(Constants.PAGE_STEP_TITLE);
                        pageStep.Text = pageStepTitle;
                    }


                }


                SetToolBarTitle(pageTitle);

                this.mPresenter = new UpdatePersonalDetailStepOnePresenter(this);

                TextViewUtils.SetMuseoSans300Typeface(txtAccountType, selector_account_type,  txtOwnerName, txtIC,txtMobileNumber, txtEmailAddress, txtMailingAddress, txtPremiseAddress); //txtView
                TextViewUtils.SetMuseoSans300Typeface(txtInputLayoutRelationshipOther,  txtInputLayoutNewIC,   txtInputLayoutNewOwnerName, txtInputLayoutNewPremiseAddress); //inputLay
                TextViewUtils.SetMuseoSans500Typeface(EditText_relationshipOther,  txtNewIC,  txtNewOwnerName); //edit text
                TextViewUtils.SetMuseoSans500Typeface(infotxtIsThisAcc, TextView_whatIsYourRelationship, infotxtTitle_Which_information, infoLabeltxtWhoIsRegistered, TextViewDoINeedOwnerConsent, TextView_updateOnOwnerBehalf);  //txtView
                txtInputLayoutRelationshipOther.SetErrorTextAppearance(TextViewUtils.IsLargeFonts ? Resource.Style.TextInputLayoutFeedbackCountLarge : Resource.Style.TextInputLayoutFeedbackCount);
                
                txtInputLayoutNewIC.SetErrorTextAppearance(TextViewUtils.IsLargeFonts ? Resource.Style.TextInputLayoutFeedbackCountLarge : Resource.Style.TextInputLayoutFeedbackCount);
                
                txtInputLayoutNewOwnerName.SetErrorTextAppearance(TextViewUtils.IsLargeFonts ? Resource.Style.TextInputLayoutFeedbackCountLarge : Resource.Style.TextInputLayoutFeedbackCount);
                txtInputLayoutCurrentEmailAddress.SetErrorTextAppearance(TextViewUtils.IsLargeFonts ? Resource.Style.TextInputLayoutFeedbackCountLarge : Resource.Style.TextInputLayoutFeedbackCount);
                txtInputLayoutNewEmailAddress.SetErrorTextAppearance(TextViewUtils.IsLargeFonts ? Resource.Style.TextInputLayoutFeedbackCountLarge : Resource.Style.TextInputLayoutFeedbackCount);
                txtInputLayoutCurrentMailingAddress.SetErrorTextAppearance(TextViewUtils.IsLargeFonts ? Resource.Style.TextInputLayoutFeedbackCountLarge : Resource.Style.TextInputLayoutFeedbackCount);
                txtInputLayoutNewMailingAddress.SetErrorTextAppearance(TextViewUtils.IsLargeFonts ? Resource.Style.TextInputLayoutFeedbackCountLarge : Resource.Style.TextInputLayoutFeedbackCount);
                txtInputLayoutCurrentPremiseAddress.SetErrorTextAppearance(TextViewUtils.IsLargeFonts ? Resource.Style.TextInputLayoutFeedbackCountLarge : Resource.Style.TextInputLayoutFeedbackCount);
                txtInputLayoutNewPremiseAddress.SetErrorTextAppearance(TextViewUtils.IsLargeFonts ? Resource.Style.TextInputLayoutFeedbackCountLarge : Resource.Style.TextInputLayoutFeedbackCount);
                pageStep.TextSize = TextViewUtils.GetFontSize(12f);
                infotxtIsThisAcc.TextSize = TextViewUtils.GetFontSize(16f);
                btnNo.TextSize = TextViewUtils.GetFontSize(16f);
                btnYes.TextSize = TextViewUtils.GetFontSize(16f);
                infoLabeltxtWhoIsRegistered.TextSize = TextViewUtils.GetFontSize(11f);
                infotxtTitle_Which_information.TextSize = TextViewUtils.GetFontSize(16f);
                TextView_whatIsYourRelationship.TextSize = TextViewUtils.GetFontSize(16f);
                txtAccountType.TextSize = TextViewUtils.GetFontSize(10f);
                selector_account_type.TextSize = TextViewUtils.GetFontSize(16f);
                EditText_relationshipOther.TextSize = TextViewUtils.GetFontSize(16f);
                btnNext.TextSize = TextViewUtils.GetFontSize(16f);
                TextView_updateOnOwnerBehalf.TextSize = TextViewUtils.GetFontSize(16f);
                txtIC.TextSize = TextViewUtils.GetFontSize(14f);
                txtGeneralCurrentICNumber.TextSize = TextViewUtils.GetFontSize(16f);
                txtNewIC.TextSize = TextViewUtils.GetFontSize(16f);
                txtOwnerName.TextSize = TextViewUtils.GetFontSize(14f);
                txtGeneralCurrentOwnerName.TextSize = TextViewUtils.GetFontSize(16f);
                txtNewOwnerName.TextSize = TextViewUtils.GetFontSize(16f);
                txtMobileNumber.TextSize = TextViewUtils.GetFontSize(14f);
                txtEmailAddress.TextSize = TextViewUtils.GetFontSize(14f);
                txtGeneralCurrentEmailAddress.TextSize = TextViewUtils.GetFontSize(16f);

                txtNewEmailAddress.TextSize = TextViewUtils.GetFontSize(16f);
                txtMailingAddress.TextSize = TextViewUtils.GetFontSize(14f);
                txtGeneralCurrentMailingAddress.TextSize = TextViewUtils.GetFontSize(16f);
                txtNewMailingAddress.TextSize = TextViewUtils.GetFontSize(16f);
                txtPremiseAddress.TextSize = TextViewUtils.GetFontSize(14f);
                txtGeneralCurrentPremiseAddress.TextSize = TextViewUtils.GetFontSize(16f);
                txtNewPremiseAddress.TextSize = TextViewUtils.GetFontSize(16f);
                TextViewDoINeedOwnerConsent.TextSize = TextViewUtils.GetFontSize(11f);
            

                //TRANSLATION 
                infotxtIsThisAcc.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "registeredTitle");
                btnNo.Text = Utility.GetLocalizedLabel("Common", "no");
                btnYes.Text = Utility.GetLocalizedLabel("Common", "yes");
                btnNext.Text = Utility.GetLocalizedLabel("Common", "next");
                infoLabeltxtWhoIsRegistered.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "registeredInfo");
                infotxtTitle_Which_information.Text= Utility.GetLocalizedLabel("SubmitEnquiry", "whichInfoUpdate");
                TextView_whatIsYourRelationship.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "ownerTitle");
                txtAccountType.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "relationshipTitle").ToUpper();
                TextView_updateOnOwnerBehalf.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "updateOwnerTitle");

                txtIC.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "icTitle");
                txtOwnerName.Text= Utility.GetLocalizedLabel("SubmitEnquiry", "accNametitle");
                txtMobileNumber.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "mobileNumberTitle");
                txtEmailAddress.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "emailAddressTitle");
                txtMailingAddress.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "mailingAddressTitle");
                txtPremiseAddress.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "premiseAddressTitle");
                TextViewDoINeedOwnerConsent.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "ownerConsentInfo");


                txtInputLayoutNewIC.Hint = Utility.GetLocalizedLabel("SubmitEnquiry", "icHint");
                txtInputLayoutNewOwnerName.Hint = Utility.GetLocalizedLabel("SubmitEnquiry", "accNameHint");
                txtInputLayoutNewEmailAddress.Hint= Utility.GetLocalizedLabel("SubmitEnquiry", "emailAddressHint");
                txtInputLayoutNewMailingAddress.Hint = Utility.GetLocalizedLabel("SubmitEnquiry", "mailingAddressHint");
                txtInputLayoutNewPremiseAddress.Hint = Utility.GetLocalizedLabel("SubmitEnquiry", "premiseAddressHint");
                txtInputLayoutRelationshipOther.Hint = Utility.GetLocalizedLabel("SubmitEnquiry", "otherRelationshipHint");

                txtInputLayoutNewIC.SetHintTextAppearance(TextViewUtils.IsLargeFonts ? Resource.Style.TextInputLayout_TextAppearance_Large : Resource.Style.TextInputLayout_TextAppearance_Small);
                txtInputLayoutNewOwnerName.SetHintTextAppearance(TextViewUtils.IsLargeFonts ? Resource.Style.TextInputLayout_TextAppearance_Large : Resource.Style.TextInputLayout_TextAppearance_Small);
                txtInputLayoutNewEmailAddress.SetHintTextAppearance(TextViewUtils.IsLargeFonts ? Resource.Style.TextInputLayout_TextAppearance_Large : Resource.Style.TextInputLayout_TextAppearance_Small);
                txtInputLayoutNewMailingAddress.SetHintTextAppearance(TextViewUtils.IsLargeFonts ? Resource.Style.TextInputLayout_TextAppearance_Large : Resource.Style.TextInputLayout_TextAppearance_Small);
                txtInputLayoutNewPremiseAddress.SetHintTextAppearance(TextViewUtils.IsLargeFonts ? Resource.Style.TextInputLayout_TextAppearance_Large : Resource.Style.TextInputLayout_TextAppearance_Small);
                txtInputLayoutRelationshipOther.SetHintTextAppearance(TextViewUtils.IsLargeFonts ? Resource.Style.TextInputLayout_TextAppearance_Large : Resource.Style.TextInputLayout_TextAppearance_Small);

                //disablling the button and other layout
                this.userActionsListener.OnDisableSubmitButton();
                this.userActionsListener.OnHideOnCreate();

                SelectRelationshipModel Child = new SelectRelationshipModel();
                Child.Id = "1";
                Child.Type = Utility.GetLocalizedLabel("SubmitEnquiry", "childTitle"); 
                Child.IsSelected = true;
                selectedAccountRelationship = Child;
                selector_account_type.Text = selectedAccountRelationship.Type;
                selector_account_type.TextSize = TextViewUtils.GetFontSize(16f);
                ownerRelationship = Utility.GetLocalizedLabel("SubmitEnquiry", "childTitle");

                txtNewIC.TextChanged += TextChanged;
                txtNewIC.AddTextChangedListener(new InputFilterFormField(txtNewIC, txtInputLayoutNewIC));

                txtNewOwnerName.TextChanged += TextChanged;
                txtNewOwnerName.AddTextChangedListener(new InputFilterFormField(txtNewOwnerName, txtInputLayoutNewOwnerName));

                txtNewEmailAddress.TextChanged += TextChanged;
                txtNewEmailAddress.AddTextChangedListener(new InputFilterFormField(txtNewEmailAddress, txtInputLayoutNewEmailAddress));

                txtNewMailingAddress.TextChanged += TextChanged;
                txtNewMailingAddress.AddTextChangedListener(new InputFilterFormField(txtNewMailingAddress, txtInputLayoutNewMailingAddress));

                txtNewPremiseAddress.TextChanged += TextChanged;
                txtNewPremiseAddress.AddTextChangedListener(new InputFilterFormField(txtNewPremiseAddress, txtInputLayoutNewPremiseAddress));

                EditText_relationshipOther.TextChanged += TextChanged;
                EditText_relationshipOther.AddTextChangedListener(new InputFilterFormField(EditText_relationshipOther, txtInputLayoutRelationshipOther));

                //mobile number section
                mobileNumberFieldContainer.RemoveAllViews();
                mobileNumberInputComponent = new MobileNumberInputComponent(this);
                mobileNumberInputComponent.SetOnTapCountryCodeAction(OnTapCountryCode);
                mobileNumberInputComponent.SetMobileNumberLabel(Utility.GetLocalizedCommonLabel("mobileNo"));
                mobileNumberInputComponent.SetSelectedCountry(CountryUtil.Instance.GetDefaultCountry());
                mobileNumberInputComponent.SetValidationAction(OnValidateMobileNumber);
                mobileNumberFieldContainer.AddView(mobileNumberInputComponent);

                mobileNumberFieldContainer.Visibility = ViewStates.Gone;  //hide mobile number when initialize






            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

            
        }

        public void MobileNumberComponent()
        {
           
        }

    

        private void OnValidateMobileNumber(bool isValidated)
        {
            if (isValidated)
            {
                isMobileNumberValidated = true;
            }
            else
            {
                isMobileNumberValidated = false;
            }
            parseCheckRequiredField();
        }

        private void OnTapCountryCode()
        {
            Intent intent = new Intent(this, typeof(SelectCountryActivity));
            StartActivityForResult(intent, COUNTRY_CODE_SELECT_REQUEST);
        }


        [Preserve]
        private void TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            try
            {
                parseCheckRequiredField();
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }

        }

        public void parseCheckRequiredField()
        {
            string iC = txtNewIC.Text;

            string ownerName = txtNewOwnerName.Text;

            bool mobileNumber = isMobileNumberValidated;

            string emailAddress = txtNewEmailAddress.Text;

            string mailingAddress = txtNewMailingAddress.Text;

            string premiseAddress = txtNewPremiseAddress.Text;

            string otherRelationstip = EditText_relationshipOther.Text;

            this.userActionsListener.CheckRequiredFields(iC, toggleChkBoxIC, ownerName, toggleChkOwnerName, mobileNumber, toggleChkMobileNumber, emailAddress, toggleChkEmailAddress, mailingAddress, toggleChkMailingAddress, premiseAddress, toggleChkPremiseAddress , otherRelationstip , isOtherChoosed);
        }

        public void showRelationshipWithOwner()
        {
            Intent accountType = new Intent(this, typeof(UpdatePersonalDetailStepOneSelectRelationshipFragment));
            accountType.PutExtra("selectedAccountType", JsonConvert.SerializeObject(selectedAccountRelationship));
            StartActivityForResult(accountType, SELECT_ACCOUNT_TYPE_REQ_CODE);
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            try
            {
                base.OnActivityResult(requestCode, resultCode, data);


                if (requestCode == SELECT_ACCOUNT_TYPE_REQ_CODE)
                {

                    if (resultCode == Result.Ok)
                    {
                        selectedAccountRelationship = JsonConvert.DeserializeObject<SelectRelationshipModel>(data.GetStringExtra("selectedAccountType"));
                        if (selectedAccountRelationship != null)
                        {
                            selector_account_type.Text = selectedAccountRelationship.Type;
                            if (selectedAccountRelationship.Id.Equals("6"))
                            {  
                                isOtherRelationShip(true);
                                isOtherChoosed = true;
                                parseCheckRequiredField();

                            }
                            else
                            {
                                //hide the other  section
                                ownerRelationship = selectedAccountRelationship.Type;
                                isOtherRelationShip(false);
                                isOtherChoosed = false;
                                parseCheckRequiredField();

                            }
                        }
                    }
                }
                else if (requestCode == COUNTRY_CODE_SELECT_REQUEST)
                {
                    string dataString = data.GetStringExtra(Constants.SELECT_COUNTRY_CODE);
                    Country selectedCountry = JsonConvert.DeserializeObject<Country>(dataString);
                    mobileNumberInputComponent.SetSelectedCountry(selectedCountry);
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }



        public void isOtherRelationShip(Boolean isRelation)
        {
            if (isRelation)
            {
                //if relation is choose other

                txtInputLayoutRelationshipOther.Visibility = ViewStates.Visible; // if other then layout visible
                spacingifSelectTypeIsNotother.Visibility = ViewStates.Gone;

            }
            else
            {
                txtInputLayoutRelationshipOther.Visibility = ViewStates.Gone;
                spacingifSelectTypeIsNotother.Visibility = ViewStates.Visible;
            }
        }


        public void SetPresenter(UpdatePersonalDetailStepOneContract.IUserActionsListener userActionListener)
        {
            this.userActionsListener = userActionListener;
        }

        public override int ResourceId()
        {
            return Resource.Layout.UpdatePersonalDetailStepOneView;
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


        public void AddtoggleCounter()
        {
            toggleCounter = toggleCounter +1;
        }

        public void MinustoggleCounter()
        {
            toggleCounter = toggleCounter -1 ;
            if (toggleCounter == 0)
            {
                DisableSubmitButton();
            }
      
        }

        
                
      


        public void EnableSubmitButton()
        {
            btnNext.Enabled = true;
            btnNext.Background = ContextCompat.GetDrawable(this, Resource.Drawable.green_button_background);
        }
        public void DisableSubmitButton()
        {
            btnNext.Enabled = false;
            btnNext.Background = ContextCompat.GetDrawable(this, Resource.Drawable.silver_chalice_button_background);
        }

        public void HideOnCreate()
        {
            //tootltip title hide
            TextView_whatIsYourRelationship.Visibility = ViewStates.Gone;
            infotxtTitle_Which_information.Visibility = ViewStates.Gone;
            TextView_updateOnOwnerBehalf.Visibility = ViewStates.Gone;
            infoLabelDoIneedOwnerConsent.Visibility = ViewStates.Gone;

            //layoutHide
            LinearLayout_relationShipOwner.Visibility = ViewStates.Gone;
            LinearLayout_IC.Visibility = ViewStates.Gone;
            LinearLayout_OwnerName.Visibility = ViewStates.Gone;
            LinearLayout_MobileNumber.Visibility = ViewStates.Gone;
            LinearLayout_EmailAddress.Visibility = ViewStates.Gone;
            LinearLayout_MailingAddress.Visibility = ViewStates.Gone;
            LinearLayout_PremiseAddress.Visibility = ViewStates.Gone;



        }


        [OnClick(Resource.Id.infoLabelWhoIsRegistered)]
        public void OnToolTip(object sender, EventArgs eventArgs)
        {
            if (!this.GetIsClicked())
            {

                this.userActionsListener.OninfoLabelWhoIsRegistered();

            }
        }

        [OnClick(Resource.Id.btnNext)]
        public void OnbtnNext(object sender, EventArgs eventArgs)
        {
            if (!this.GetIsClicked())
            {


                if (toggleChkBoxIC)
                {
                    if (TextUtils.IsEmpty(txtNewIC.Text.Trim()))
                    {
                        ShowEmptyError(typeOfLayout.ic);
                        DisableSubmitButton();
                        return;
                    }
                }

                // check checkBox 
                if (toggleChkOwnerName)
                {
                    if (TextUtils.IsEmpty(txtNewOwnerName.Text.Trim()))
                    {
                        ShowEmptyError(typeOfLayout.ownerName);
                        DisableSubmitButton();
                        return;
                    }
                }

                // check checkBox 
                if (toggleChkMobileNumber)
                {   //todo checking for mobile number 

                    string mobileNumber = mobileNumberInputComponent.GetMobileNumberValueWithISDCode();
                    if (TextUtils.IsEmpty(mobileNumber) || mobileNumber == "+60")
                    {
                        mobileNumberInputComponent.RaiseError(Utility.GetLocalizedLabel("SubmitEnquiry", "mobileReq"));
                        DisableSubmitButton();
                        return;
                    }

                }

                // check checkBox 
                if (toggleChkEmailAddress)
                {
                    if (TextUtils.IsEmpty(txtNewEmailAddress.Text.Trim()))
                    {
                        ShowEmptyError(typeOfLayout.emailAddress);
                        DisableSubmitButton();
                        return;
                    }
                }

                // check checkBox 
                if (toggleChkMailingAddress)
                {
                    if (TextUtils.IsEmpty(txtNewMailingAddress.Text.Trim()))
                    {

                        ShowEmptyError(typeOfLayout.mailingAddress);
                        DisableSubmitButton();
                        return;
                    }
                 
                }

                // check checkBox 
                if (toggleChkPremiseAddress)
                {
                    if (TextUtils.IsEmpty(txtNewPremiseAddress.Text.Trim()))
                    {
                        ShowEmptyError(typeOfLayout.premiseAddress);
                        DisableSubmitButton();
                        return;
                    }
               
                }

                if (isOtherChoosed)
                {
                    if (TextUtils.IsEmpty(EditText_relationshipOther.Text.Trim()))
                    {
                        DisableSubmitButton();
                        return;

                    }
                   
                }


                this.SetIsClicked(true);
                this.userActionsListener.OnShowUpdatePersonalDetailStepTwoActivity();

            }
        }

        public void ShowUpdatePersonalDetailStepTwoActivity()
        {


            var Intent = new Intent(this, typeof(UpdatePersonalDetailStepTwoActivity));

            Intent.PutExtra(Constants.SELECT_REGISTERED_OWNER,isRegisteredOwner.ToString());
            Intent.PutExtra(Constants.ACCOUNT_NUMBER, caNumber.Trim());
            

            if (!isRegisteredOwner)
            { // send info is not regestered owner
                if (isOtherChoosed)
                {
                    Intent.PutExtra(Constants.OWNER_RELATIONSHIP, EditText_relationshipOther.Text.Trim());
                }
                else
                {
                    Intent.PutExtra(Constants.OWNER_RELATIONSHIP, ownerRelationship);
                }
            }

         
            if (toggleChkBoxIC)
            {
                Intent.PutExtra(Constants.ACCOUNT_IC_NUMBER, txtNewIC.Text.Trim());
            }
            if (toggleChkOwnerName)
            {
                Intent.PutExtra(Constants.ACCOUNT_OWNER_NAME, txtNewOwnerName.Text.Trim());
            }
            if (toggleChkEmailAddress)
            {
                Intent.PutExtra(Constants.ACCOUNT_EMAIL_ADDRESS, txtNewEmailAddress.Text.Trim());
            }
            if (toggleChkMobileNumber)
            {   
                Intent.PutExtra(Constants.ACCOUNT_MOBILE_NUMBER, mobileNumberInputComponent.GetMobileNumberValueWithISDCode());
            }
            if (toggleChkMailingAddress)
            {
                Intent.PutExtra(Constants.ACCOUNT_MAILING_ADDRESS, txtNewMailingAddress.Text.Trim());
            }
            if( toggleChkPremiseAddress)
            {
                Intent.PutExtra(Constants.ACCOUNT_PREMISE_ADDRESS, txtNewPremiseAddress.Text.Trim());
            }
            // Intent.PutExtra("ACCOUNT", accNo);
            StartActivity(Intent);

            

        }





        [OnClick(Resource.Id.infoLabelDoIneedOwnerConsent)]
        public void ClickinfoLabelDoIneedOwnerConsent(object sender, EventArgs eventArgs)
        {
            if (!this.GetIsClicked())
            {

                this.userActionsListener.OninfoLabelDoIneedOwnerConsent();

            }
        }




        [OnClick(Resource.Id.selector_account_type)]
        public void Onselector_account_type(object sender, EventArgs eventArgs)
        {
            if (!this.GetIsClicked())
            {

                this.userActionsListener.OnRelationshipWithOwner();

            }
        }

        public void ShowInvalidMobileNoError()
        {
            try
            {
                //todo error hint mobile number

                //no  implementation

                //txtInputLayoutNewMobileNumber.SetErrorTextAppearance(Resource.Style.TextInputLayoutBottomErrorHint);
                //TextViewUtils.SetMuseoSans300Typeface(txtInputLayoutNewMobileNumber);
                //txtInputLayoutNewMobileNumber.Error = Utility.GetLocalizedErrorLabel("invalid_mobileNumber");
                

            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }


        public void cleanAllField()
        {
            txtNewIC.Text = "";

            txtNewOwnerName.Text = "";

            mobileNumberInputComponent.ClearMobileNumber(); 

            mobileNumberInputComponent.SetSelectedCountry(CountryUtil.Instance.GetDefaultCountry()); // set default country

            txtNewEmailAddress.Text = "";

            txtNewMailingAddress.Text = "";

            txtNewPremiseAddress.Text = "";

            EditText_relationshipOther.Text = "";

            selectICChkBox.Checked = false;
            selectOwnerNameChkBox.Checked = false;
            mobile_number_chk_box.Checked = false;
            select_email_address_chk_box.Checked = false;
            select_mailing_address_chk_box.Checked = false;
            premiseAddress_chk_box.Checked = false;
            isOtherChoosed = false;

            isOtherRelationShip(false);  //  handle ui

            toggleChkBoxIC = false;

            toggleChkOwnerName = false;

            toggleChkMobileNumber = false;

            toggleChkEmailAddress = false;

            toggleChkMailingAddress = false;

            toggleChkPremiseAddress = false;

            txtInputLayoutNewIC.Visibility = ViewStates.Gone;
            txtInputLayoutNewOwnerName.Visibility = ViewStates.Gone;
            mobileNumberFieldContainer.Visibility = ViewStates.Gone;  
            txtInputLayoutNewEmailAddress.Visibility = ViewStates.Gone;
            txtInputLayoutNewMailingAddress.Visibility = ViewStates.Gone;
            txtInputLayoutNewPremiseAddress.Visibility = ViewStates.Gone;
            txtInputLayoutRelationshipOther.Visibility = ViewStates.Gone;

            txtInputLayoutNewIC.Error = "";
            txtInputLayoutNewOwnerName.Error = "";
            mobileNumberInputComponent.ClearError();
            txtInputLayoutNewEmailAddress.Error = "";
            txtInputLayoutNewMailingAddress.Error = "";
            txtInputLayoutNewPremiseAddress.Error = "";
            txtInputLayoutRelationshipOther.Error = "";


  

            toggleCounter = 0;


            SelectRelationshipModel Child = new SelectRelationshipModel();
            Child.Id = "1";
            Child.Type = Utility.GetLocalizedLabel("SubmitEnquiry", "childTitle"); // translate for child  Utility.GetLocalizedLabel("AddAccount", "residential")
            Child.IsSelected = true;
            selectedAccountRelationship = Child;
            selector_account_type.Text = selectedAccountRelationship.Type;
            ownerRelationship = Utility.GetLocalizedLabel("SubmitEnquiry", "childTitle");


        }


        [OnClick(Resource.Id.btnYes)]
        public void clickbtnYes(object sender, EventArgs eventArgs)
        {
            if (!this.GetIsClicked())
            {
                this.isRegisteredOwner = true;
                this.userActionsListener.OnShowbtnYes();  // show only that owner ac
                btnYes.Background = ContextCompat.GetDrawable(this, Resource.Drawable.green_button_background);  //change to green
                btnYes.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(this, Resource.Color.white)));  //change text to white
                btnNo.Background = ContextCompat.GetDrawable(this, Resource.Drawable.light_green_outline_button_background);  //change no to green ouline
                btnNo.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(this, Resource.Color.freshGreen)));  //change text to white
                cleanAllField();



            }
        }

        [OnClick(Resource.Id.btnNo)]
        public void clickbtnNo(object sender, EventArgs eventArgs)
        {
            if (!this.GetIsClicked())
            {
                this.isRegisteredOwner = false;
                // show only that non owner
                this.userActionsListener.OnShowbtnNo();
                btnNo.Background = ContextCompat.GetDrawable(this, Resource.Drawable.green_button_background);  //change to green
                btnNo.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(this, Resource.Color.white)));  //change text to white
                btnYes.Background = ContextCompat.GetDrawable(this, Resource.Drawable.light_green_outline_button_background);  //change no to green ouline
                btnYes.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(this, Resource.Color.freshGreen)));  //change text to white
                cleanAllField();
                //  btnNo.Background = ContextCompat.GetDrawable(this, Resource.Drawable.green_button_background);  //change to white on no


            }
        }

        [OnClick(Resource.Id.selectICChkBox)]
        public void ClickselectICChkBox(object sender, EventArgs eventArgs)
        {
            if (!this.GetIsClicked())
            {

                this.userActionsListener.OntoggleIc();
                
            }
        }

        public void toggleIc()
        {
            toggleChkBoxIC = !toggleChkBoxIC;  //boolean change

            if (toggleChkBoxIC)
            {
                txtInputLayoutNewIC.Visibility = ViewStates.Visible;
                Drawable icon = ContextCompat.GetDrawable(this, Resource.Drawable.placeholder_ic);
                txtNewIC.SetCompoundDrawablesWithIntrinsicBounds(icon, null, null, null);

                if (txtNewIC.Text.IsNullOrEmpty())
                {
                    this.userActionsListener.OnDisableSubmitButton();
                }
               // parseCheckRequiredField();
                AddtoggleCounter(); 
            }
            else
            {
                txtInputLayoutNewIC.Visibility = ViewStates.Gone;
                txtNewIC.Text = "";   // remove if untick
                ClearErrors(typeOfLayout.ic);
                parseCheckRequiredField();
                MinustoggleCounter();
            }
        }

        [OnClick(Resource.Id.selectOwnerNameChkBox)]
        public void ClickselectOwnerNameChkBox(object sender, EventArgs eventArgs)
        {
            if (!this.GetIsClicked())
            {
               
                this.userActionsListener.OntoggleAccountOwnerName();
            }
        }

        public void toggleAccountOwnerName()
        {
            toggleChkOwnerName = !toggleChkOwnerName;  //boolean change


            if (toggleChkOwnerName)
            {
                txtInputLayoutNewOwnerName.Visibility = ViewStates.Visible;
                // parseCheckRequiredField();
                Drawable icon = ContextCompat.GetDrawable(this, Resource.Drawable.placeholder_name);
                txtNewOwnerName.SetCompoundDrawablesWithIntrinsicBounds(icon, null, null, null);

                if (txtNewOwnerName.Text.IsNullOrEmpty())
                {
                    this.userActionsListener.OnDisableSubmitButton();
                }
                AddtoggleCounter();
            }
            else
            {
                txtInputLayoutNewOwnerName.Visibility = ViewStates.Gone;
                txtNewOwnerName.Text = "";
                ClearErrors(typeOfLayout.ownerName);
                parseCheckRequiredField();
                MinustoggleCounter();
            }

        }

        [OnClick(Resource.Id.mobile_number_chk_box)]
        public void Clickmobile_number_chk_box(object sender, EventArgs eventArgs)
        {
            if (!this.GetIsClicked())
            {
                
                this.userActionsListener.OntoggleMobileNumber();
            }
        }

        public void toggleMobileNumber()
        {
            toggleChkMobileNumber = !toggleChkMobileNumber;  //boolean change


            if (toggleChkMobileNumber)
            {
                mobileNumberFieldContainer.Visibility = ViewStates.Visible;
                if (mobileNumberInputComponent.GetMobileNumberValue().IsNullOrEmpty())
                {
                    this.userActionsListener.OnDisableSubmitButton();
                }
                AddtoggleCounter();
            }
            else
            {  
                 
                mobileNumberFieldContainer.Visibility = ViewStates.Gone;
                mobileNumberInputComponent.ClearMobileNumber();
                mobileNumberInputComponent.SetSelectedCountry(CountryUtil.Instance.GetDefaultCountry());
                parseCheckRequiredField();
                MinustoggleCounter();
            }

        }


        [OnClick(Resource.Id.select_email_address_chk_box)]
        public void Clickselect_email_address_chk_box(object sender, EventArgs eventArgs)
        {
            if (!this.GetIsClicked())
            {
                
                this.userActionsListener.OntoggleEmailAddress();
            }
        }

        public void toggleEmailAddress()
        {
            toggleChkEmailAddress = !toggleChkEmailAddress; //boolean change


            if (toggleChkEmailAddress)
            {
                txtInputLayoutNewEmailAddress.Visibility = ViewStates.Visible;
                //  parseCheckRequiredField();
                Drawable icon = ContextCompat.GetDrawable(this, Resource.Drawable.placeholder_email);
                txtNewEmailAddress.SetCompoundDrawablesWithIntrinsicBounds(icon, null, null, null);
                if (txtNewEmailAddress.Text.IsNullOrEmpty())
                {
                    this.userActionsListener.OnDisableSubmitButton();
                }
                AddtoggleCounter();
            }
            else
            {
                txtInputLayoutNewEmailAddress.Visibility = ViewStates.Gone;
                txtNewEmailAddress.Text = "";
                ClearErrors(typeOfLayout.emailAddress);
                parseCheckRequiredField();
                MinustoggleCounter();
            }

        }

        [OnClick(Resource.Id.select_mailing_address_chk_box)]
        public void clickselect_mailing_address_chk_box(object sender, EventArgs eventArgs)
        {
            if (!this.GetIsClicked())
            {
               
                this.userActionsListener.OntoggleMailingAddress();
            }
        }

        public void toggleMailingAddress()
        {
            toggleChkMailingAddress = !toggleChkMailingAddress;  //boolean change


            if (toggleChkMailingAddress)
            {
                txtInputLayoutNewMailingAddress.Visibility = ViewStates.Visible;
                Drawable icon = ContextCompat.GetDrawable(this, Resource.Drawable.placeholder_email);
                txtNewMailingAddress.SetCompoundDrawablesWithIntrinsicBounds(icon, null, null, null);

                //parseCheckRequiredField();
                if (txtNewMailingAddress.Text.IsNullOrEmpty())
                {
                    this.userActionsListener.OnDisableSubmitButton();
                }
                AddtoggleCounter();
            }
            else
            {
                txtInputLayoutNewMailingAddress.Visibility = ViewStates.Gone;
                txtNewMailingAddress.Text = "";
                ClearErrors(typeOfLayout.mailingAddress);
                parseCheckRequiredField();
                MinustoggleCounter();
            }

        }

        [OnClick(Resource.Id.premiseAddress_chk_box)]
        public void clickpremiseAddress_chk_box(object sender, EventArgs eventArgs)
        {
            if (!this.GetIsClicked())
            {
               
                this.userActionsListener.OntogglePremiseAddress();
            }
        }

        public void togglePremiseAddress()
        {
            toggleChkPremiseAddress = !toggleChkPremiseAddress;  //boolean change


            if (toggleChkPremiseAddress)
            {
                txtInputLayoutNewPremiseAddress.Visibility = ViewStates.Visible;
                Drawable icon = ContextCompat.GetDrawable(this, Resource.Drawable.ic_field_address);
                txtNewPremiseAddress.SetCompoundDrawablesWithIntrinsicBounds(icon, null, null, null);
                //parseCheckRequiredField();
                if (txtNewPremiseAddress.Text.IsNullOrEmpty())
                {
                    this.userActionsListener.OnDisableSubmitButton();
                }
                AddtoggleCounter();
            }
            else
            {
                txtInputLayoutNewPremiseAddress.Visibility = ViewStates.Gone;
                txtNewPremiseAddress.Text = "";
                ClearErrors(typeOfLayout.premiseAddress);
                parseCheckRequiredField();
                MinustoggleCounter();
            }

        }



        public void ShowbtnNo()
        {
            infotxtTitle_Which_information.Visibility = ViewStates.Gone;
            LinearLayout_relationShipOwner.Visibility = ViewStates.Visible;
            // txtInputLayoutRelationshipOther.Visibility = ViewStates.Visible;
            TextView_updateOnOwnerBehalf.Visibility = ViewStates.Visible;
            TextView_whatIsYourRelationship.Visibility = ViewStates.Visible;

            if (isOtherChoosed)
            {
                isOtherRelationShip(true);
            }
            else
            {
                isOtherRelationShip(false);
            }

            LinearLayout_IC.Visibility = ViewStates.Visible;
            LinearLayout_OwnerName.Visibility = ViewStates.Visible;
            LinearLayout_MobileNumber.Visibility = ViewStates.Visible;
            LinearLayout_EmailAddress.Visibility = ViewStates.Visible;
            LinearLayout_MailingAddress.Visibility = ViewStates.Visible;
            LinearLayout_PremiseAddress.Visibility = ViewStates.Visible;
            infoLabelDoIneedOwnerConsent.Visibility = ViewStates.Visible;
        }

        public void ShowbtnYes()
        {
            infotxtTitle_Which_information.Visibility = ViewStates.Visible;
            LinearLayout_relationShipOwner.Visibility = ViewStates.Gone;
            txtInputLayoutRelationshipOther.Visibility = ViewStates.Gone;
            TextView_updateOnOwnerBehalf.Visibility = ViewStates.Gone;
            TextView_whatIsYourRelationship.Visibility = ViewStates.Gone;


            LinearLayout_IC.Visibility = ViewStates.Visible;
            LinearLayout_OwnerName.Visibility = ViewStates.Visible;
            LinearLayout_MobileNumber.Visibility = ViewStates.Visible;
            LinearLayout_EmailAddress.Visibility = ViewStates.Visible;
            LinearLayout_MailingAddress.Visibility = ViewStates.Visible;
            LinearLayout_PremiseAddress.Visibility = ViewStates.Visible;
            infoLabelDoIneedOwnerConsent.Visibility = ViewStates.Gone;

        }

        public void BoolICLayout(Boolean isShown)

        {

            if (isShown)
            {

              //  txtInputLayoutCurrentICNumber.Visibility = ViewStates.Visible;
                txtInputLayoutNewIC.Visibility = ViewStates.Visible;
            }
            else
            {

               // txtInputLayoutCurrentICNumber.Visibility = ViewStates.Gone;
                txtInputLayoutNewIC.Visibility = ViewStates.Gone;
            }


        }

        public void BoolLayoutCurrentOwnerName(Boolean isShown)
        {

            if (isShown)
            {
              //  txtInputLayoutCurrentOwnerName.Visibility = ViewStates.Visible;
                txtInputLayoutNewOwnerName.Visibility = ViewStates.Visible;
            }
            else
            {
               // txtInputLayoutCurrentOwnerName.Visibility = ViewStates.Gone;
                txtInputLayoutNewOwnerName.Visibility = ViewStates.Gone;
            }


        }

        public void BoolInputLayoutCurrentEmailAddress(Boolean isShown)

        {
            if (isShown)
            {
             //   txtInputLayoutCurrentEmailAddress.Visibility = ViewStates.Visible;
                txtInputLayoutNewEmailAddress.Visibility = ViewStates.Visible;
            }
            else
            {
               // txtInputLayoutCurrentEmailAddress.Visibility = ViewStates.Gone;
                txtInputLayoutNewEmailAddress.Visibility = ViewStates.Gone;
            }
        }

        public void BoolInputLayoutCurrentMailingAddress(Boolean isShown)
        {
            if (isShown)
            {
               // txtInputLayoutCurrentMailingAddress.Visibility = ViewStates.Visible;
                txtInputLayoutNewMailingAddress.Visibility = ViewStates.Visible;
            }
            else
            {
               // txtInputLayoutCurrentMailingAddress.Visibility = ViewStates.Gone;
                txtInputLayoutNewMailingAddress.Visibility = ViewStates.Gone;
            }
        }

        public void BoolInputLayoutCurrentPremiseAddress(Boolean isShown)
        {

            if (isShown)
            {
                //txtInputLayoutCurrentPremiseAddress.Visibility = ViewStates.Visible;
                txtInputLayoutNewPremiseAddress.Visibility = ViewStates.Visible;
            }
            else
            {
               // txtInputLayoutCurrentPremiseAddress.Visibility = ViewStates.Gone;
                txtInputLayoutNewPremiseAddress.Visibility = ViewStates.Gone;
            }

        }



        public void ShowinfoLabelDoIneedOwnerConsent()
        {
            //List<DoINeedOwnerConsentResponseModel> modelList = MyTNBAppToolTipData.GetDoINeedOwnerConsentTipData();
            //if (modelList != null && modelList.Count > 0)
            //{
            //    MyTNBAppToolTipBuilder Tooltip = MyTNBAppToolTipBuilder.Create(this, MyTNBAppToolTipBuilder.ToolTipType.NORMAL_WITH_HEADER)
            //        .SetTitle(modelList[0].PopUpTitle)
            //        .SetMessage(modelList[0].PopUpBody)
            //        .SetCTALabel(Utility.GetLocalizedCommonLabel("gotIt"))
            //        .SetCTAaction(() => { this.SetIsClicked(false); })
            //        .Build();
            //        Tooltip.Show();
            //}
            //else
            //{
            //    //backup if sitecoreCMSEntity not retun any
            //    MyTNBAppToolTipBuilder infoLabelWhoIsRegistered = MyTNBAppToolTipBuilder.Create(this, MyTNBAppToolTipBuilder.ToolTipType.NORMAL_WITH_HEADER)
            //      .SetTitle(Utility.GetLocalizedLabel("SubmitEnquiry", "ownerConsentTitle"))
            //      .SetMessage(Utility.GetLocalizedLabel("SubmitEnquiry", "ownerConsentDescription"))
            //      .SetCTALabel(Utility.GetLocalizedCommonLabel("gotIt"))
            //      .SetCTAaction(() => { this.SetIsClicked(false); })
            //      .Build();
            //    infoLabelWhoIsRegistered.Show();
            //}


                MyTNBAppToolTipBuilder infoLabelWhoIsRegistered = MyTNBAppToolTipBuilder.Create(this, MyTNBAppToolTipBuilder.ToolTipType.NORMAL_WITH_HEADER)
                  .SetTitle(Utility.GetLocalizedLabel("SubmitEnquiry", "ownerConsentTitle"))
                  .SetMessage(Utility.GetLocalizedLabel("SubmitEnquiry", "ownerConsentDescription"))
                  .SetCTALabel(Utility.GetLocalizedCommonLabel("gotIt"))
                  .SetCTAaction(() => { this.SetIsClicked(false); })
                  .Build();
                infoLabelWhoIsRegistered.Show();
        }




        public void ShowinfoLabelWhoIsRegistered()
        {
            //List<whoIsRegisteredOwnerResponseModel> modelList = MyTNBAppToolTipData.GetWhoIsRegisteredOwnerTipData();



            //if (modelList != null && modelList.Count > 0)
            //{
            //    MyTNBAppToolTipBuilder Tooltip = MyTNBAppToolTipBuilder.Create(this, MyTNBAppToolTipBuilder.ToolTipType.NORMAL_WITH_HEADER)
            //        .SetTitle(modelList[0].PopUpTitle)
            //        .SetMessage(modelList[0].PopUpBody)
            //        .SetCTALabel(Utility.GetLocalizedCommonLabel("gotIt"))
            //        .SetCTAaction(() => { this.SetIsClicked(false); })
            //        .Build();
            //         Tooltip.Show();
            //}
            //else
            //{ 
            //    //backup if sitecoreCMSEntity not retun any
            //    MyTNBAppToolTipBuilder infoLabelWhoIsRegistered = MyTNBAppToolTipBuilder.Create(this, MyTNBAppToolTipBuilder.ToolTipType.NORMAL_WITH_HEADER)

            //      .SetTitle(Utility.GetLocalizedLabel("SubmitEnquiry", "registeredInfo"))
            //      .SetMessage(Utility.GetLocalizedLabel("SubmitEnquiry", "registeredInfoDetail"))
            //      .SetCTALabel(Utility.GetLocalizedCommonLabel("gotIt"))
            //      .SetCTAaction(() => { this.SetIsClicked(false); })
            //      .Build();
            //    infoLabelWhoIsRegistered.Show();
            //}


            MyTNBAppToolTipBuilder infoLabelWhoIsRegistered = MyTNBAppToolTipBuilder.Create(this, MyTNBAppToolTipBuilder.ToolTipType.NORMAL_WITH_HEADER)

                 .SetTitle(Utility.GetLocalizedLabel("SubmitEnquiry", "registeredInfo"))
                 .SetMessage(Utility.GetLocalizedLabel("SubmitEnquiry", "registeredInfoDetail"))
                 .SetCTALabel(Utility.GetLocalizedCommonLabel("gotIt"))
                 .SetCTAaction(() => { this.SetIsClicked(false); })
                 .Build();
            infoLabelWhoIsRegistered.Show();

        }


        public void ShowEmptyError(typeOfLayout lay)
        {
            if (lay.Equals(typeOfLayout.ic))
            {
               
                txtInputLayoutNewIC.SetErrorTextAppearance(TextViewUtils.IsLargeFonts ? Resource.Style.TextInputLayoutBottomErrorHintLarge : Resource.Style.TextInputLayoutBottomErrorHint);
                TextViewUtils.SetMuseoSans300Typeface(txtInputLayoutNewIC.FindViewById<TextView>(Resource.Id.textinput_error));
                if (txtInputLayoutNewIC.Error!= Utility.GetLocalizedLabel("SubmitEnquiry", "icReq"))
                {
                    txtInputLayoutNewIC.Error = Utility.GetLocalizedLabel("SubmitEnquiry", "icReq");
                }
                
            }

            if (lay.Equals(typeOfLayout.emailAddress))
            {
                txtInputLayoutNewEmailAddress.SetErrorTextAppearance(TextViewUtils.IsLargeFonts ? Resource.Style.TextInputLayoutBottomErrorHintLarge : Resource.Style.TextInputLayoutBottomErrorHint);
               
                TextViewUtils.SetMuseoSans300Typeface(txtInputLayoutNewEmailAddress.FindViewById<TextView>(Resource.Id.textinput_error));
                if (txtInputLayoutNewEmailAddress.Error!= Utility.GetLocalizedLabel("SubmitEnquiry", "emailReq"))
                {
                    txtInputLayoutNewEmailAddress.Error = Utility.GetLocalizedLabel("SubmitEnquiry", "emailReq");
                }
                
    


            }

            if (lay.Equals(typeOfLayout.mailingAddress))
            {
                txtInputLayoutNewMailingAddress.SetErrorTextAppearance(TextViewUtils.IsLargeFonts ? Resource.Style.TextInputLayoutBottomErrorHintLarge : Resource.Style.TextInputLayoutBottomErrorHint);
                
                TextViewUtils.SetMuseoSans300Typeface(txtInputLayoutNewMailingAddress.FindViewById<TextView>(Resource.Id.textinput_error));
                if (txtInputLayoutNewMailingAddress.Error!= Utility.GetLocalizedLabel("SubmitEnquiry", "mailingReq"))
                {
                    txtInputLayoutNewMailingAddress.Error = Utility.GetLocalizedLabel("SubmitEnquiry", "mailingReq");
                }
                
            }

            if (lay.Equals(typeOfLayout.mobileNumber))
            {
                mobileNumberInputComponent.RaiseError(Utility.GetLocalizedLabel("SubmitEnquiry", "mobileReq"));
            }

            if (lay.Equals(typeOfLayout.ownerName))
            {
                txtInputLayoutNewOwnerName.SetErrorTextAppearance(TextViewUtils.IsLargeFonts ? Resource.Style.TextInputLayoutBottomErrorHintLarge : Resource.Style.TextInputLayoutBottomErrorHint);
               
                TextViewUtils.SetMuseoSans300Typeface(txtInputLayoutNewOwnerName.FindViewById<TextView>(Resource.Id.textinput_error));
                if (txtInputLayoutNewOwnerName.Error!= Utility.GetLocalizedLabel("SubmitEnquiry", "ownerReq"))
                {
                    txtInputLayoutNewOwnerName.Error = Utility.GetLocalizedLabel("SubmitEnquiry", "ownerReq");
                }
                
            }

            if (lay.Equals(typeOfLayout.premiseAddress))
            {
                txtInputLayoutNewPremiseAddress.SetErrorTextAppearance(TextViewUtils.IsLargeFonts ? Resource.Style.TextInputLayoutBottomErrorHintLarge : Resource.Style.TextInputLayoutBottomErrorHint);
               
                TextViewUtils.SetMuseoSans300Typeface(txtInputLayoutNewPremiseAddress.FindViewById<TextView>(Resource.Id.textinput_error));
                if(txtInputLayoutNewPremiseAddress.Error!= Utility.GetLocalizedLabel("SubmitEnquiry", "permisesReq"))
                {
                    txtInputLayoutNewPremiseAddress.Error = Utility.GetLocalizedLabel("SubmitEnquiry", "permisesReq");
                }
                   
            }
        }

         public void ClearInvalidError(typeOfLayout lay)
        {
            // txtInputLayoutEmail.Error = null;

            if (lay.Equals(typeOfLayout.ic)){
                txtInputLayoutNewIC.Error = "";
            }
            if (lay.Equals(typeOfLayout.emailAddress))
            {
                txtInputLayoutNewEmailAddress.Error = "";

            }
            if (lay.Equals(typeOfLayout.mailingAddress))
            {
                txtInputLayoutNewMailingAddress.Error = "";
     
            }
            if (lay.Equals(typeOfLayout.mobileNumber))
            {
                mobileNumberInputComponent.ClearError();  
            }
            if (lay.Equals(typeOfLayout.ownerName))
            {
                txtInputLayoutNewOwnerName.Error = "";
              
            }
            if (lay.Equals(typeOfLayout.premiseAddress))
            {
                txtInputLayoutNewPremiseAddress.Error = "";
            
            }

        }

        public void ClearErrors(typeOfLayout lay) 
        {
            if (lay.Equals(typeOfLayout.ic))
                txtInputLayoutNewIC.Error = "";

            if (lay.Equals(typeOfLayout.emailAddress))
                txtInputLayoutNewEmailAddress.Error = "";

            if (lay.Equals(typeOfLayout.mailingAddress))
                txtInputLayoutNewMailingAddress.Error = "";

            if (lay.Equals(typeOfLayout.mobileNumber))
                mobileNumberInputComponent.ClearError();

            if (lay.Equals(typeOfLayout.ownerName))
                txtInputLayoutNewOwnerName.Error = "";

            if (lay.Equals(typeOfLayout.premiseAddress))
                txtInputLayoutNewPremiseAddress.Error = "";
        }

        public void ShowInvalidError(typeOfLayout lay)
        {


            if (lay.Equals(typeOfLayout.ic))
            {
                txtInputLayoutNewIC.SetErrorTextAppearance(TextViewUtils.IsLargeFonts ? Resource.Style.TextInputLayoutBottomErrorHintLarge : Resource.Style.TextInputLayoutBottomErrorHint);
               
                TextViewUtils.SetMuseoSans300Typeface(txtInputLayoutNewIC.FindViewById<TextView>(Resource.Id.textinput_error));
                if(txtInputLayoutNewIC.Error != Utility.GetLocalizedErrorLabel("invalid_icNumber"))
                {
                    txtInputLayoutNewIC.Error = Utility.GetLocalizedErrorLabel("invalid_icNumber");
                }
               

            }
            if (lay.Equals(typeOfLayout.emailAddress))
            {
                txtInputLayoutNewEmailAddress.SetErrorTextAppearance(TextViewUtils.IsLargeFonts ? Resource.Style.TextInputLayoutBottomErrorHintLarge : Resource.Style.TextInputLayoutBottomErrorHint);
               
                TextViewUtils.SetMuseoSans300Typeface(txtInputLayoutNewEmailAddress.FindViewById<TextView>(Resource.Id.textinput_error));
                if (txtInputLayoutNewEmailAddress.Error!= Utility.GetLocalizedErrorLabel("invalid_email"))
                {
                    txtInputLayoutNewEmailAddress.Error = Utility.GetLocalizedErrorLabel("invalid_email");
                }
               

            }
            if (lay.Equals(typeOfLayout.mailingAddress))
            {
               // txtInputLayoutNewMailingAddress
               // no implementation
            }
            if (lay.Equals(typeOfLayout.mobileNumber))
            {   //no implementation txtInputLayoutNewMobileNumber
          
            }
            if (lay.Equals(typeOfLayout.ownerName))
            {
                txtInputLayoutNewOwnerName.SetErrorTextAppearance(TextViewUtils.IsLargeFonts ? Resource.Style.TextInputLayoutBottomErrorHintLarge : Resource.Style.TextInputLayoutBottomErrorHint);
                
                TextViewUtils.SetMuseoSans300Typeface(txtInputLayoutNewOwnerName.FindViewById<TextView>(Resource.Id.textinput_error));
                if(txtInputLayoutNewOwnerName.Error!= Utility.GetLocalizedErrorLabel("invalid_fullname"))
                {   //this func is to cater bouncing effect on error
                    txtInputLayoutNewOwnerName.Error = Utility.GetLocalizedErrorLabel("invalid_fullname");
                }
               

            }
            if (lay.Equals(typeOfLayout.premiseAddress))
            {
                // no implementation txtInputLayoutNewPremiseAddress
  
            }
        }
    }
}