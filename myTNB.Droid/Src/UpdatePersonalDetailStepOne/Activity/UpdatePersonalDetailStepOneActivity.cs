using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Opengl;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.Content;
using Android.Views;
using Android.Widget;
using CheeseBind;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.UpdatePersonalDetailStepOne.Fragment;
using myTNB_Android.Src.UpdatePersonalDetailStepOne.Model;
using myTNB_Android.Src.UpdatePersonalDetailStepOne.MVP;
using myTNB_Android.Src.UpdatePersonalDetailStepTwo.Activity;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;
using NSubstitute.Core;
using Syncfusion.Drawing;
using static myTNB_Android.Src.UpdatePersonalDetailStepOne.Fragment.UpdatePersonalDetailStepOneSelectRelationshipFragment;

namespace myTNB_Android.Src.UpdatePersonalDetailStepOne.Activity


{

    [Activity(Label = "Update Personal Details"
      , ScreenOrientation = ScreenOrientation.Portrait
              , WindowSoftInputMode = SoftInput.AdjustPan
      , Theme = "@style/Theme.FaultyStreetLamps")]
    public class UpdatePersonalDetailStepOneActivity : BaseToolbarAppCompatActivity, UpdatePersonalDetailStepOneContract.IView
    {

        // , View.IOnTouchListener
        UpdatePersonalDetailStepOnePresenter mPresenter;
        UpdatePersonalDetailStepOneContract.IUserActionsListener userActionsListener;
        const string PAGE_ID = "UpdatePersonalDetailsFirstStep";

        //[BindView(Resource.Id.infotxtIsThisAcc)]
        //TextView infotxtIsThisAcc;

        //[BindView(Resource.Id.infoLabeltxtWhoIsRegistered)]
        //TextView infoLabeltxtWhoIsRegistered;

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

        //[BindView(Resource.Id.txtInputLayoutCurrentICNumber)]
        //TextInputLayout txtInputLayoutCurrentICNumber;


        //[BindView(Resource.Id.txtGeneralCurrentICNumber)]
        //EditText txtGeneralCurrentICNumber;

        [BindView(Resource.Id.txtInputLayoutNewIC)]
        TextInputLayout txtInputLayoutNewIC;

        [BindView(Resource.Id.txtNewIC)]
        EditText txtNewIC;


        [BindView(Resource.Id.txtOwnerName)]
        TextView txtOwnerName;




        [BindView(Resource.Id.pageStep)]
        TextView pageStep;




        //[BindView(Resource.Id.txtInputLayoutCurrentOwnerName)]
        //TextInputLayout txtInputLayoutCurrentOwnerName;

        //[BindView(Resource.Id.txtGeneralCurrentOwnerName)]
        //EditText txtGeneralCurrentOwnerName;

        [BindView(Resource.Id.txtInputLayoutNewOwnerName)]
        TextInputLayout txtInputLayoutNewOwnerName;

        [BindView(Resource.Id.txtNewOwnerName)]
        EditText txtNewOwnerName;

        [BindView(Resource.Id.txtMobileNumber)]
        TextView txtMobileNumber;


        [BindView(Resource.Id.txtInputLayoutCurrentMobileNumber)]
        TextInputLayout txtInputLayoutCurrentMobileNumber;

        [BindView(Resource.Id.txtGeneralCurrentMobileNumber)]
        EditText txtGeneralCurrentMobileNumber;


        [BindView(Resource.Id.txtInputLayoutNewMobileNumber)]
        TextInputLayout txtInputLayoutNewMobileNumber;

        [BindView(Resource.Id.txtNewMobileNumber)]
        EditText txtNewMobileNumber;


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




        private bool toggleChkBoxIC = false;

        private bool toggleChkOwnerName = false;

        private bool toggleChkMobileNumber = false;

        private bool toggleChkEmailAddress = false;

        private bool toggleChkMailingAddress = false;

        private bool toggleChkPremiseAddress = false;

        private bool isRegisteredOwner;

        private string ownerRelationship;



        private int toggleCounter = 0;

        private SelectRelationshipModel selectedAccountRelationship;

        private bool isOtherChoosed = false;

        private readonly int SELECT_ACCOUNT_TYPE_REQ_CODE = 2012;

        private string  caNumber;

        private string pageTitle;

        private string pageStepTitle;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            try
            {

                //get data form prev page
                Bundle extras = Intent.Extras;
           
                if (extras != null)
                {
                    ///PAGE TITLE FROM BEFORE PAGE
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


                // Intent intent = Intent;
                SetToolBarTitle(pageTitle);


                //Set Presenter
                this.mPresenter = new UpdatePersonalDetailStepOnePresenter(this);



                // set font
                TextViewUtils.SetMuseoSans300Typeface(txtAccountType, selector_account_type,  txtOwnerName, txtIC,txtMobileNumber, txtEmailAddress, txtMailingAddress, txtPremiseAddress); //txtView
                TextViewUtils.SetMuseoSans300Typeface(txtInputLayoutRelationshipOther,  txtInputLayoutNewIC,   txtInputLayoutNewOwnerName, txtInputLayoutCurrentMobileNumber, txtInputLayoutNewMobileNumber, txtInputLayoutNewPremiseAddress); //inputLay
                TextViewUtils.SetMuseoSans500Typeface(EditText_relationshipOther,  txtNewIC,  txtNewOwnerName, txtGeneralCurrentMobileNumber, txtNewMobileNumber); //edit text
                TextViewUtils.SetMuseoSans500Typeface(infotxtIsThisAcc, TextView_whatIsYourRelationship, infotxtTitle_Which_information);  //txtView
                // infoLabeltxtWhoIsRegistered, , infoLabelDoIneedOwnerConsent
                //  TextViewUtils.SetMuseoSans500Typeface();
                //txtGeneralCurrentICNumber txtGeneralCurrentOwnerName  txtInputLayoutCurrentICNumber   txtInputLayoutCurrentOwnerName

                //add listener 
                //txtGeneralEnquiry1.AddTextChangedListener(new InputFilterFormField(txtGeneralEnquiry1, txtInputLayoutGeneralEnquiry1));
                //txtGeneralEnquiry1.TextChanged += TextChanged;
                //txtGeneralEnquiry1.SetOnTouchListener(this);

                txtPremiseAddress.Text = "Permises Address";
                txtInputLayoutNewPremiseAddress.Hint = "ENTER NEW PREMISES ADDRESS";

                //disablling the button and other layout
                this.userActionsListener.OnDisableSubmitButton();
                this.userActionsListener.OnHideOnCreate();


                SelectRelationshipModel Child = new SelectRelationshipModel();
                Child.Id = "1";
                Child.Type = "Child";  // translate for child  Utility.GetLocalizedLabel("AddAccount", "residential")
                Child.IsSelected = true;
                selectedAccountRelationship = Child;
                selector_account_type.Text = selectedAccountRelationship.Type;
                ownerRelationship = "Child";
                // selector_account_type.SetOnTouchListener(this);  //here


                //Add listener to InputLayout

                //     txtNewIC.SetOnTouchListener(this);
                txtNewIC.TextChanged += TextChanged;
                txtNewIC.AddTextChangedListener(new InputFilterFormField(txtNewIC, txtInputLayoutNewIC));

                txtNewOwnerName.TextChanged += TextChanged;
                txtNewOwnerName.AddTextChangedListener(new InputFilterFormField(txtNewOwnerName, txtInputLayoutNewOwnerName));

                txtNewMobileNumber.TextChanged += TextChanged;
                txtNewMobileNumber.AddTextChangedListener(new InputFilterFormField(txtNewMobileNumber, txtInputLayoutNewMobileNumber));

                txtNewEmailAddress.TextChanged += TextChanged;
                txtNewEmailAddress.AddTextChangedListener(new InputFilterFormField(txtNewEmailAddress, txtInputLayoutNewEmailAddress));

                txtNewMailingAddress.TextChanged += TextChanged;
                txtNewMailingAddress.AddTextChangedListener(new InputFilterFormField(txtNewMailingAddress, txtInputLayoutNewMailingAddress));

                txtNewPremiseAddress.TextChanged += TextChanged;
                txtNewPremiseAddress.AddTextChangedListener(new InputFilterFormField(txtNewPremiseAddress, txtInputLayoutNewPremiseAddress));



            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

            // Create your application here
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

            string mobileNumber = txtNewMobileNumber.Text;

            string emailAddress = txtNewEmailAddress.Text;

            string mailingAddress = txtNewMailingAddress.Text;

            string premiseAddress = txtNewPremiseAddress.Text;
            this.userActionsListener.CheckRequiredFields(iC, toggleChkBoxIC, ownerName, toggleChkOwnerName, mobileNumber, toggleChkMobileNumber, emailAddress, toggleChkEmailAddress, mailingAddress, toggleChkMailingAddress, premiseAddress, toggleChkPremiseAddress);
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

                            }
                            else
                            {
                                //hide the other  section
                                ownerRelationship = selectedAccountRelationship.Type;
                                isOtherRelationShip(false);
                                isOtherChoosed = false;

                            }
                        }
                    }
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
                Intent.PutExtra(Constants.ACCOUNT_MOBILE_NUMBER, txtNewMobileNumber.Text.Trim());
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
                parseCheckRequiredField();
                AddtoggleCounter(); 
            }
            else
            {
                txtInputLayoutNewIC.Visibility = ViewStates.Gone;
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
                parseCheckRequiredField();
                AddtoggleCounter();
            }
            else
            {
                txtInputLayoutNewOwnerName.Visibility = ViewStates.Gone;
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
                txtInputLayoutNewMobileNumber.Visibility = ViewStates.Visible;
                parseCheckRequiredField();
                AddtoggleCounter();
            }
            else
            {
                txtInputLayoutNewMobileNumber.Visibility = ViewStates.Gone;
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
                parseCheckRequiredField();
                AddtoggleCounter();
            }
            else
            {
                txtInputLayoutNewEmailAddress.Visibility = ViewStates.Gone;
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
                parseCheckRequiredField();
                AddtoggleCounter();
            }
            else
            {
                txtInputLayoutNewMailingAddress.Visibility = ViewStates.Gone;
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
                parseCheckRequiredField();
                AddtoggleCounter();
            }
            else
            {
                txtInputLayoutNewPremiseAddress.Visibility = ViewStates.Gone;
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
            MyTNBAppToolTipBuilder infoLabelWhoIsRegistered = MyTNBAppToolTipBuilder.Create(this, MyTNBAppToolTipBuilder.ToolTipType.NORMAL_WITH_HEADER)

         .SetTitle("Do I need the owner’s consent?")
         .SetMessage("You as a non-owner or an authorised person are required to provide the owner’s proof of consent to update the personal details on their behalf as it will permanently update the owner’s TNB electricity account details. You will also consent to update your own contact information as it is still the owner’s property.")
         .SetCTALabel(Utility.GetLocalizedCommonLabel("gotIt"))
         .SetCTAaction(() => { this.SetIsClicked(false); })
         .Build();
            infoLabelWhoIsRegistered.Show();

        }




        public void ShowinfoLabelWhoIsRegistered()
        {


            MyTNBAppToolTipBuilder infoLabelWhoIsRegistered = MyTNBAppToolTipBuilder.Create(this, MyTNBAppToolTipBuilder.ToolTipType.NORMAL_WITH_HEADER)

               .SetTitle("Who is a registered owner?")
               .SetMessage("This electricity account must be registered under your name with your IC or Passport.")
               .SetCTALabel(Utility.GetLocalizedCommonLabel("gotIt"))
               .SetCTAaction(() => { this.SetIsClicked(false); })
               .Build();
            infoLabelWhoIsRegistered.Show();

        }


        public void ShowEmptyError(typeOfLayout lay)
        {
            if (lay.Equals(typeOfLayout.ic))
            {
                txtInputLayoutNewIC.SetErrorTextAppearance(Resource.Style.TextInputLayoutBottomErrorHint);
                // TextViewUtils.SetMuseoSans300Typeface(txtInputLayoutNewIC.FindViewById<TextView>(Resource.Id.textinput_error));
                TextViewUtils.SetMuseoSans300Typeface(txtInputLayoutNewIC);
                txtInputLayoutNewIC.Error = "Ic is required"; //Utility.GetLocalizedLabel("FeedbackForm", "invalidFeedback");  // change lang 
            }

            if (lay.Equals(typeOfLayout.emailAddress))
            {
                txtInputLayoutNewEmailAddress.SetErrorTextAppearance(Resource.Style.TextInputLayoutBottomErrorHint);
                //TextViewUtils.SetMuseoSans300Typeface(txtInputLayoutNewEmailAddress.FindViewById<TextView>(Resource.Id.textinput_error));
                TextViewUtils.SetMuseoSans300Typeface(txtInputLayoutNewEmailAddress);
                txtInputLayoutNewEmailAddress.Error = "Email is required"; //Utility.GetLocalizedLabel("FeedbackForm", "invalidFeedback");  // change lang 

            }

            if (lay.Equals(typeOfLayout.mailingAddress))
            {
                txtInputLayoutNewMailingAddress.SetErrorTextAppearance(Resource.Style.TextInputLayoutBottomErrorHint);
                //  TextViewUtils.SetMuseoSans300Typeface(txtInputLayoutNewMailingAddress.FindViewById<TextView>(Resource.Id.textinput_error));
                TextViewUtils.SetMuseoSans300Typeface(txtInputLayoutNewMailingAddress);
                txtInputLayoutNewMailingAddress.Error = "Mailing Address is required"; //Utility.GetLocalizedLabel("FeedbackForm", "invalidFeedback");  // change lang 

            }

            if (lay.Equals(typeOfLayout.mobileNumber))
            {
                txtInputLayoutNewMobileNumber.SetErrorTextAppearance(Resource.Style.TextInputLayoutBottomErrorHint);
                //TextViewUtils.SetMuseoSans300Typeface(txtInputLayoutNewMobileNumber.FindViewById<TextView>(Resource.Id.textinput_error));
                TextViewUtils.SetMuseoSans300Typeface(txtInputLayoutNewMobileNumber);
                txtInputLayoutNewMobileNumber.Error = "Mobile number is required"; //Utility.GetLocalizedLabel("FeedbackForm", "invalidFeedback");  // change lang 
            }

            if (lay.Equals(typeOfLayout.ownerName))
            {
                    txtInputLayoutNewOwnerName.SetErrorTextAppearance(Resource.Style.TextInputLayoutBottomErrorHint);
                // TextViewUtils.SetMuseoSans300Typeface(txtInputLayoutNewOwnerName.FindViewById<TextView>(Resource.Id.textinput_error));
                TextViewUtils.SetMuseoSans300Typeface(txtInputLayoutNewOwnerName);
                txtInputLayoutNewOwnerName.Error = "Owner name is required"; //Utility.GetLocalizedLabel("FeedbackForm", "invalidFeedback");  // change lang 
            }

            if (lay.Equals(typeOfLayout.premiseAddress))
            {
                txtInputLayoutNewPremiseAddress.SetErrorTextAppearance(Resource.Style.TextInputLayoutBottomErrorHint);
                //TextViewUtils.SetMuseoSans300Typeface(txtInputLayoutNewPremiseAddress.FindViewById<TextView>(Resource.Id.textinput_error));
                TextViewUtils.SetMuseoSans300Typeface(txtInputLayoutNewPremiseAddress);
                txtInputLayoutNewPremiseAddress.Error = "Premises address is required"; //Utility.GetLocalizedLabel("FeedbackForm", "invalidFeedback");  // change lang 
            }
        }

         public void ClearInvalidError(typeOfLayout lay)
        {
            // txtInputLayoutEmail.Error = null;

            if (lay.Equals(typeOfLayout.ic)){
                txtInputLayoutNewIC.Error = null;
            }
            if (lay.Equals(typeOfLayout.emailAddress))
            {
                txtInputLayoutNewEmailAddress.Error = null;
            }
            if (lay.Equals(typeOfLayout.mailingAddress))
            {
                txtInputLayoutNewMailingAddress.Error = null;
            }
            if (lay.Equals(typeOfLayout.mobileNumber))
            {
                txtInputLayoutNewMobileNumber.Error = null;
            }
            if (lay.Equals(typeOfLayout.ownerName))
            {
                txtInputLayoutNewOwnerName.Error = null;
            }
            if (lay.Equals(typeOfLayout.premiseAddress))
            {
                txtInputLayoutNewPremiseAddress.Error = null;
            }

        }

       public void ClearErrors()
        {
            txtInputLayoutNewIC.Error = null;
            txtInputLayoutNewEmailAddress.Error = null;
            txtInputLayoutNewMailingAddress.Error = null;
            txtInputLayoutNewMobileNumber.Error = null;
            txtInputLayoutNewOwnerName.Error = null;
            txtInputLayoutNewPremiseAddress.Error = null;
        }

       public void ShowInvalidError(typeOfLayout lay)
        {

            //Utility.GetLocalizedErrorLabel("invalid_fullname"); 


            if (lay.Equals(typeOfLayout.ic))
            {
                //txtInputLayoutNewIC.SetErrorTextAppearance(Resource.Style.TextErrorAppearance);
                TextViewUtils.SetMuseoSans300Typeface(txtInputLayoutNewIC.FindViewById<TextView>(Resource.Id.textinput_error));
                TextViewUtils.SetMuseoSans300Typeface(txtInputLayoutNewIC);
                txtInputLayoutNewIC.Error = Utility.GetLocalizedErrorLabel("invalid_icNumber");
            }
            if (lay.Equals(typeOfLayout.emailAddress))
            {
                //txtInputLayoutNewEmailAddress.SetErrorTextAppearance(Resource.Style.TextErrorAppearance);
                TextViewUtils.SetMuseoSans300Typeface(txtInputLayoutNewEmailAddress.FindViewById<TextView>(Resource.Id.textinput_error));
                TextViewUtils.SetMuseoSans300Typeface(txtInputLayoutNewEmailAddress);
                txtInputLayoutNewEmailAddress.Error = Utility.GetLocalizedErrorLabel("invalid_email"); ;
            }
            if (lay.Equals(typeOfLayout.mailingAddress))
            {
                //txtInputLayoutNewEmailAddress.SetErrorTextAppearance(Resource.Style.TextErrorAppearance);
                TextViewUtils.SetMuseoSans300Typeface(txtInputLayoutNewMailingAddress.FindViewById<TextView>(Resource.Id.textinput_error));
                TextViewUtils.SetMuseoSans300Typeface(txtInputLayoutNewMailingAddress);
                txtInputLayoutNewMailingAddress.Error = "Invalid mailing address";
            }
            if (lay.Equals(typeOfLayout.mobileNumber))
            {
                TextViewUtils.SetMuseoSans300Typeface(txtInputLayoutNewMobileNumber.FindViewById<TextView>(Resource.Id.textinput_error));
                TextViewUtils.SetMuseoSans300Typeface(txtInputLayoutNewMobileNumber);
                txtInputLayoutNewMobileNumber.Error = Utility.GetLocalizedErrorLabel("invalid_mobileNumber"); ;
            }
            if (lay.Equals(typeOfLayout.ownerName))
            {
                TextViewUtils.SetMuseoSans300Typeface(txtInputLayoutNewOwnerName.FindViewById<TextView>(Resource.Id.textinput_error));
                TextViewUtils.SetMuseoSans300Typeface(txtInputLayoutNewOwnerName);
                txtInputLayoutNewOwnerName.Error = Utility.GetLocalizedErrorLabel("invalid_fullname"); 
            }
            if (lay.Equals(typeOfLayout.premiseAddress))
            {
                TextViewUtils.SetMuseoSans300Typeface(txtInputLayoutNewPremiseAddress.FindViewById<TextView>(Resource.Id.textinput_error));
                TextViewUtils.SetMuseoSans300Typeface(txtInputLayoutNewPremiseAddress);
                txtInputLayoutNewPremiseAddress.Error = "Invalid premise address";
            }
        }
    }
}