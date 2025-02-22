﻿using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Preferences;
using Android.Runtime;
using Android.Text;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.CoordinatorLayout.Widget;
using AndroidX.Core.Content;
using CheeseBind;
using Google.Android.Material.Snackbar;
using Google.Android.Material.TextField;
using myTNB.AndroidApp.Src.AddAcc_UpdateIdentification_StepOne.MVP;
using myTNB.AndroidApp.Src.AddAcc_UpdateIdentification_StepTwo.Activity;
using myTNB.AndroidApp.Src.Barcode.Activity;
using myTNB.AndroidApp.Src.Base.Activity;
using myTNB.AndroidApp.Src.Database.Model;
using myTNB.AndroidApp.Src.Feedback_Login_BillRelated.Activity;
using myTNB.AndroidApp.Src.FeedbackGeneralEnquiryStepOne.Activity;
using myTNB.AndroidApp.Src.myTNBMenu.Models;
using myTNB.AndroidApp.Src.SiteCore;
using myTNB.AndroidApp.Src.Utils;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;

namespace myTNB.AndroidApp.Src.AddAcc_UpdateIdentification_StepOne.Activity
{
    [Activity(Label = "Update Personal Details"
  , ScreenOrientation = ScreenOrientation.Portrait
          , WindowSoftInputMode = SoftInput.AdjustPan
  , Theme = "@style/Theme.OwnerTenantBaseTheme")]


    public class AddAccUpdateIdetificationDetailsActivity : BaseToolbarAppCompatActivity, AddAccUpdateIdentificationDetailsContract.IView, View.IOnTouchListener
    {
        [BindView(Resource.Id.rootView)]
        CoordinatorLayout rootView;

        [BindView(Resource.Id.txtAccountNo)]
        EditText txtAccountNo;

        [BindView(Resource.Id.txtInputLayoutAccountNo)]
        TextInputLayout txtInputLayoutAccountNo;

        [BindView(Resource.Id.txtInputLayoutNewIC)]
        TextInputLayout txtInputLayoutNewIC;

        [BindView(Resource.Id.txtNewIC)]
        EditText txtNewIC;

        [BindView(Resource.Id.infoLabeltxtWhereIsMyAcc)]
        TextView infoLabeltxtWhereIsMyAcc;

        [BindView(Resource.Id.pageStep)]
        TextView pageStep;

        [BindView(Resource.Id.infoLabelWhereIsMyAcc)]
        LinearLayout infoLabelWhereIsMyAcc;

        [BindView(Resource.Id.enterYourNewDetails)]
        TextView enterYourNewDetails;

        [BindView(Resource.Id.scanNewEnquiry)]
        ImageButton scanNewEnquiry;

        [BindView(Resource.Id.btnNext)]
        Button btnNext;

        String GeneralEnquiry1of2_app_bar = "@string/bill_related_activity_title";

        FrameLayout rootview;

        private bool isAccChoosed = false;
        string identityType;

        AddAccUpdateIdentificationDetailsContract.IUserActionsListener userActionsListener;
        AddAccUpdateIdentificationDetailsPresenter mPresenter;
        AccountData selectedAccount;

        private bool isClicked = false;
        private ISharedPreferences mSharedPref;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            try
            {
                //init shared preferences 
                mSharedPref = PreferenceManager.GetDefaultSharedPreferences(this);
                //1 set presenter
                mPresenter = new AddAccUpdateIdentificationDetailsPresenter(this, mSharedPref);

                // Intent intent = Intent;
                SetToolBarTitle(Utility.GetLocalizedLabel("SubmitEnquiry", "updatePersonalDetTitle"));
                //2 set font type , 300 normal 500 button
                TextViewUtils.SetMuseoSans300Typeface(txtInputLayoutAccountNo, txtInputLayoutNewIC);
                TextViewUtils.SetMuseoSans300Typeface(txtAccountNo, txtNewIC, pageStep);
                TextViewUtils.SetMuseoSans500Typeface(enterYourNewDetails, btnNext, infoLabeltxtWhereIsMyAcc);
                TextViewUtils.SetTextSize14(txtAccountNo, pageStep, infoLabeltxtWhereIsMyAcc);
                TextViewUtils.SetTextSize16(btnNext, enterYourNewDetails);

                txtInputLayoutNewIC.SetErrorTextAppearance(TextViewUtils.IsLargeFonts ? Resource.Style.TextInputLayoutFeedbackCountLarge : Resource.Style.TextInputLayoutFeedbackCount);
                txtInputLayoutNewIC.SetHintTextAppearance(TextViewUtils.IsLargeFonts ? Resource.Style.TextInputLayout_TextAppearance_Large : Resource.Style.TextInputLayout_TextAppearance_Small);

                //set translation of string 

                txtInputLayoutAccountNo.Hint = Utility.GetLocalizedLabel("SubmitEnquiry", "accNumberHint");
                txtInputLayoutNewIC.Hint = Utility.GetLocalizedLabel("SubmitEnquiry", "idTitle");
                infoLabeltxtWhereIsMyAcc.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "accNumberInfo");
                pageStep.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "pageStep1");
                enterYourNewDetails.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "enterYourNewDetails");
                //txtAccountNo.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "accNumberHint");
                btnNext.Text = Utility.GetLocalizedLabel("Common", "next");

                //txtICNumber.AfterTextChanged += new EventHandler<AfterTextChangedEventArgs>(AddTextChangedListener);
                //txtICNumber.AddTextChangedListener(new InputFilterFormField(txtICNumber, textInputLayoutICNo));
                //txtICNumber.AddTextChangedListener(new PhoneTextWatcher(txtICNumber, identityType));
                //txtICNumber.SetOnKeyListener(new KeyListener());
                //txtNewIC.InputType = InputTypes.ClassNumber;
                string idnumber = UserEntity.GetActive().IdentificationNo;
                txtNewIC.Text = idnumber;

                txtNewIC.FocusChange += txtICNumber_FocusChange;
                txtNewIC.TextChanged += txtICNumber_TextChange;  //adding listener on text change
                txtNewIC.AddTextChangedListener(new InputFilterFormField(txtNewIC, txtInputLayoutNewIC));  //adding listener on text change
                //txtNewIC.SetCompoundDrawablesWithIntrinsicBounds(Resource.Drawable.placeholder_ic, 0, 0, 0);

                if (!UserEntity.IsCurrentlyActive())
                {
                    scanNewEnquiry.Visibility = ViewStates.Gone;

                    // bottom drawable hide
                    Drawable scanIcon = ContextCompat.GetDrawable(this, Resource.Drawable.scan);
                    Drawable accnoIcon = ContextCompat.GetDrawable(this, Resource.Drawable.ic_field_account_no);
                    txtAccountNo.SetCompoundDrawablesWithIntrinsicBounds(Resource.Drawable.placeholder_account_no, 0, 0, 0);
                }

                txtAccountNo.SetOnTouchListener(this);  //set listener on dropdown arrow at TextLayout
                txtAccountNo.TextChanged += TextChange;  //adding listener on text change
                //txtAccountNo.FocusChange += TxtAccountNo_FocusChange;
                txtAccountNo.AddTextChangedListener(new InputFilterFormField(txtAccountNo, txtInputLayoutAccountNo));  //adding listener on text change

                infoLabeltxtWhereIsMyAcc.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "accNumberInfo");  // inject translation to text
                DisableNextButton();
                onGetTooltipImageContent();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        private void AddTextChangedListener(object sender, AfterTextChangedEventArgs e)
        {
            try
            {
                string ic = txtNewIC.Text.ToString().Trim();
                string accno = txtAccountNo.Text.ToString().Trim();
                //ClearICMinimumCharactersError();

                this.userActionsListener.CheckRequiredFields(accno, ic);
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }


        public void ClearICMinimumCharactersError()
        {
            txtInputLayoutNewIC.SetErrorTextAppearance(TextViewUtils.IsLargeFonts
                ? Resource.Style.TextInputLayoutBottomErrorHintLarge
                : Resource.Style.TextInputLayoutBottomErrorHint);

            if (!string.IsNullOrEmpty(txtInputLayoutNewIC.Error))
            {
                txtInputLayoutNewIC.Error = null;
                txtInputLayoutNewIC.ErrorEnabled = false;
            }
        }

        public void ShowFullICError()
        {
            txtInputLayoutNewIC.SetErrorTextAppearance(TextViewUtils.IsLargeFonts ? Resource.Style.TextInputLayoutBottomErrorHintLarge : Resource.Style.TextInputLayoutBottomErrorHint);

            TextViewUtils.SetMuseoSans300Typeface(txtInputLayoutNewIC.FindViewById<TextView>(Resource.Id.textinput_error));
            if (txtInputLayoutNewIC.Error != Utility.GetLocalizedErrorLabel("invalid_icNumber"))
            {
                txtInputLayoutNewIC.Error = Utility.GetLocalizedErrorLabel("invalid_icNumber");
            }
        }

        public class PhoneTextWatcher : Java.Lang.Object, ITextWatcher
        {
            public PhoneTextWatcher(EditText text, string idtype)
            {
                eText = text;
                idText = "IC / Mykad";
            }
            private bool flagDel = false;
            private EditText eText;
            string idText;

            public void AfterTextChanged(IEditable s)
            {
            }

            public void BeforeTextChanged(Java.Lang.ICharSequence s, int start, int count, int after)
            {
                int len = eText.Text.Length;
                int totallength = eText.SelectionStart;
                int totallenafter = len - totallength;
                string Idtype = idText;

                if ((Idtype.Equals("IC / MyKad") || Idtype.Equals("Kad Pengenalan / MyKad")) && len > 0 && (totallenafter != 0) && !flagDel)
                {
                    flagDel = true;
                    KeyListener.KeyDel = 0;
                    eText.Text = s.ToString();
                    eText.SetSelection(eText.Text.Length);
                    //string input = s.ToString();
                    //char a = input[totallength - 1];
                    //string b = a.ToString();
                    //if (b.Equals("-"))
                    //{
                    //    eText.Text = s.ToString();
                    //    eText.SetSelection(eText.Text.Length);
                    //}
                }
            }

            public void OnTextChanged(Java.Lang.ICharSequence s, int start, int before, int count)
            {
                string Idtype = "IC / Mykad";

                if (Idtype.Equals("IC / MyKad") || Idtype.Equals("Kad Pengenalan / MyKad"))
                {
                    eText.SetFilters(new IInputFilter[] { new InputFilterLengthFilter(14) });
                    if (!flagDel)
                    {
                        int len = eText.Text.Length;
                        if ((len == 6 || len == 9 || start == 5 || start == 8) && before == 0)
                        {
                            eText.Text = eText.Text + "-";
                            eText.SetSelection(eText.Text.Length);
                        }

                        if (len == 7 && before == 0)
                        {
                            string first6digit = eText.Text.Substring(0, 6);
                            string last1digit = eText.Text.Substring(eText.Text.Length - 1);
                            eText.Text = first6digit + "-" + last1digit;
                            eText.SetSelection(eText.Text.Length);
                        }

                        if (len == 10 && before == 0)
                        {
                            string first9digit = eText.Text.Substring(0, 9);
                            string last1digit = eText.Text.Substring(eText.Text.Length - 1);
                            eText.Text = first9digit + "-" + last1digit;
                            eText.SetSelection(eText.Text.Length);
                        }
                    }
                    flagDel = false;
                }

            }

        }
        public class KeyListener : Java.Lang.Object, Android.Views.View.IOnKeyListener
        {
            public static int KeyDel { get; set; }
            public bool OnKey(Android.Views.View v, Android.Views.Keycode keyCode, Android.Views.KeyEvent e)
            {
                if (keyCode == Android.Views.Keycode.Del || keyCode == Android.Views.Keycode.Back)
                    KeyDel = 1;
                else
                    KeyDel = 0;
                return false;
            }
        }


        public void ShowInvalidIdentificationError()
        {
            Utility.ShowIdentificationErrorDialog(this, () =>
            {
                ShowProgressDialog();

            });
        }

        public override int ResourceId()
        {   //todo change
            return Resource.Layout.AddAccountUpdateIdentificationStepOneView;
        }

        public bool IsActive()
        {
            return Window.DecorView.RootView.IsShown;
        }

        public void makeSetClick(bool setClick)
        {
            this.SetIsClicked(setClick);
        }

        public void SetPresenter(AddAccUpdateIdentificationDetailsContract.IUserActionsListener userActionListener)
        {
            this.userActionsListener = userActionListener;
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

                    if (UserEntity.IsCurrentlyActive())
                    {
                        //to ensure only works if user is login
                        if (e.Action == MotionEventActions.Up)
                        {
                            if (e.RawX >= (txtAccountNo.Right - txtAccountNo.GetCompoundDrawables()[DRAWABLE_RIGHT].Bounds.Width()))
                            {
                                //this function listen to click on the dropdown drawable right
                                //check if from prelogin of after login disable if user from prelogin

                                this.userActionsListener.OnSelectAccount();


                                return true;
                            }
                        }

                    }
                    else
                    {
                        // cater if user is from outside
                        if (e.Action == MotionEventActions.Up)
                        {
                            if (e.RawX >= (txtAccountNo.Right - txtAccountNo.GetCompoundDrawables()[DRAWABLE_RIGHT].Bounds.Width()))
                            {
                                //this function listen to click on the dropdown drawable right

                                this.userActionsListener.showScan();

                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        private void TxtAccountNo_FocusChange(object sender, View.FocusChangeEventArgs e)
        {
            try
            {
                if (!e.HasFocus)
                {
                    string accno = txtAccountNo.Text.ToString().Trim();
                    string ic = txtNewIC.Text.ToString().Trim();
                    this.userActionsListener.CheckRequiredFields(accno, ic);
                }
                //else
                //{
                //    RemoveNumberErrorMessage();
                //}
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }

        private void txtICNumber_FocusChange(object sender, View.FocusChangeEventArgs e)
        {
            try
            {
                if (!e.HasFocus)
                {
                    string accno = txtAccountNo.Text.ToString().Trim();
                    string ic = txtNewIC.Text.ToString().Trim();
                    ShowICHint();
                    this.userActionsListener.CheckRequiredFields(accno, ic);
                }
                else
                {

                    string accno = txtAccountNo.Text.ToString().Trim();
                    string ic = txtNewIC.Text.ToString().Trim();
                    this.userActionsListener.CheckRequiredFields(accno, ic);
                    ClearICHint();
                    txtNewIC.RequestFocus();
                    txtAccountNo.ClearFocus();
                }

            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }

        private void txtICNumber_TextChange(object sender, TextChangedEventArgs e)
        {
            try
            {
                ShowICHint();
                ButtonEnable();
                string ic = txtNewIC.Text.ToString().Trim();
                string accno = txtAccountNo.Text.ToString().Trim();

                if (!string.IsNullOrEmpty(ic))
                {

                    txtInputLayoutNewIC.Error = null;
                    txtInputLayoutNewIC.ErrorEnabled = false;
                }

            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }

        private void TextChange(object sender, TextChangedEventArgs e)
        {
            try
            {
                ClearICHint();
                ButtonEnable();
                string ic = txtNewIC.Text.ToString().Trim();
                string accno = txtAccountNo.Text.ToString().Trim();

                //if (!string.IsNullOrEmpty(accno))
                //{
                //    txtInputLayoutAccountNo.Error = null;

                //}

                this.userActionsListener.CheckRequiredFields(accno, ic);

            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }


        }

        public void ButtonEnable()
        {
            string ic = txtNewIC.Text.ToString().Trim();
            string accno = txtAccountNo.Text.ToString().Trim();
            bool allow = true;

            if (string.IsNullOrEmpty(ic) || string.IsNullOrEmpty(accno))
            {
                allow = false;
            }

            /*if (!TextUtils.IsEmpty(accno))
            {

                if (!Utility.AddAccountNumberValidation(accno.Length))
                {
                    allow = false;
                }
            }
            else
            {
                allow = false;
            }*/

            if (allow)
            {
                EnableNextButton();
            }
            else
            {
                DisableNextButton();
            }
        }


        public void toggleEnableClick()
        {
            isAccChoosed = true;
        }

        public void toggleDisableClick()
        {
            isAccChoosed = false;
        }

        public override Boolean ShowCustomToolbarTitle()
        {
            return true;
        }

        public void DisableNextButton()
        {
            btnNext.Enabled = false;
            btnNext.Background = ContextCompat.GetDrawable(this, Resource.Drawable.silver_chalice_button_background);
        }

        public void EnableNextButton()
        {
            btnNext.Enabled = true;
            btnNext.Background = ContextCompat.GetDrawable(this, Resource.Drawable.green_button_background);
        }

        protected override void OnResume()
        {
            base.OnResume();
            try
            {
                FirebaseAnalyticsUtils.SetScreenName(this, "Submit New Enquiry");
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



        [OnClick(Resource.Id.scanNewEnquiry)]
        void OnScanClick(object sender, EventArgs eventArgs)
        {
            if (!this.GetIsClicked())
            {


                this.SetIsClicked(true);
                Intent barcodeIntent = new Intent(this, typeof(BarcodeActivity));
                //barcodeIntent.PutExtra(Constants.PAGE_TITLE, Utility.GetLocalizedLabel("SubmitEnquiry", "submitEnquiryTitle"));
                StartActivityForResult(barcodeIntent, Constants.BARCODE_REQUEST_CODE);
            }
        }

        public void onScan()
        {
            if (!this.GetIsClicked())
            {

                this.SetIsClicked(true);
                Intent barcodeIntent = new Intent(this, typeof(BarcodeActivity));
                //barcodeIntent.PutExtra(Constants.PAGE_TITLE, Utility.GetLocalizedLabel("SubmitEnquiry", "submitEnquiryTitle"));
                StartActivityForResult(barcodeIntent, Constants.BARCODE_REQUEST_CODE);
            }
        }

        //[OnClick(Resource.Id.txtAccountNo)]
        //void OnSelectAccountLayout1(object sender, EventArgs eventArgs)
        //{
        //    if (!this.GetIsClicked())+
        //    {
        //        this.SetIsClicked(true);
        //        this.userActionsListener.OnSelectAccount();
        //    }
        //}


        public void ShowICHint()
        {
            txtInputLayoutNewIC.SetErrorTextAppearance(TextViewUtils.IsLargeFonts
              ? Resource.Style.TextInputLayoutBottomErrorHintLarge
              : Resource.Style.TextInputLayoutBottomErrorHint);


            if (txtInputLayoutNewIC.HelperText != Utility.GetLocalizedLabel("SubmitEnquiry", "idHint"))
            {
                txtInputLayoutNewIC.HelperText = Utility.GetLocalizedLabel("SubmitEnquiry", "idHint");
            }

            if (!txtInputLayoutNewIC.HelperTextEnabled)
                txtInputLayoutNewIC.HelperTextEnabled = true;
        }

        public void ClearICHint()
        {
            txtInputLayoutNewIC.HelperText = null;
            txtInputLayoutNewIC.HelperTextEnabled = false;
        }

        public void ShowInvalidAccountNumberError()
        {
            txtInputLayoutAccountNo.SetErrorTextAppearance(TextViewUtils.IsLargeFonts
                ? Resource.Style.TextInputLayoutBottomErrorHintLarge
                : Resource.Style.TextInputLayoutBottomErrorHint);

            if (txtInputLayoutAccountNo.Error != Utility.GetLocalizedLabel("SubmitEnquiry", "validElectricityAccountNoError"))
            {
                txtInputLayoutAccountNo.Error = Utility.GetLocalizedLabel("SubmitEnquiry", "validElectricityAccountNoError");
            }

        }

        public void ShowEnterOrSelectAccNumber()
        {
            txtInputLayoutAccountNo.SetErrorTextAppearance(TextViewUtils.IsLargeFonts
                ? Resource.Style.TextInputLayoutBottomErrorHintLarge
                : Resource.Style.TextInputLayoutBottomErrorHint);

            if (txtInputLayoutAccountNo.Error != Utility.GetLocalizedLabel("SubmitEnquiry", "plsEnterAcc"))
            {
                txtInputLayoutAccountNo.Error = Utility.GetLocalizedLabel("SubmitEnquiry", "plsEnterAcc");
            }

            //txtInputLayoutAccountNo.RequestFocus();
        }

        public void RemoveIDNumberErrorMessage()
        {
            txtInputLayoutNewIC.SetErrorTextAppearance(TextViewUtils.IsLargeFonts
                ? Resource.Style.TextInputLayoutBottomErrorHintLarge
                : Resource.Style.TextInputLayoutBottomErrorHint);

            txtInputLayoutNewIC.Error = "";

        }

        public void RemoveNumberErrorMessage()
        {
            txtInputLayoutAccountNo.SetErrorTextAppearance(TextViewUtils.IsLargeFonts
                ? Resource.Style.TextInputLayoutBottomErrorHintLarge
                : Resource.Style.TextInputLayoutBottomErrorHint);

            txtInputLayoutAccountNo.Error = "";

        }

        public void ShowGeneralEnquiry()
        {

            Intent generalEnquiry = new Intent(this, typeof(FeedbackGeneralEnquiryStepOneActivity));
            generalEnquiry.PutExtra(Constants.ACCOUNT_NUMBER, txtAccountNo.Text.ToString().Trim());
            StartActivity(generalEnquiry);


            //Intent generalEnquiry = new Intent(this, typeof(FeedbackGeneralEnquiryStepOneActivity));
            //generalEnquiry.PutExtra(Constants.ACCOUNT_NUMBER, txtAccountNo.Text.ToString().Trim());
            //StartActivityForResult(generalEnquiry, Constants.REQUEST_FEEDBACK_SUCCESS_VIEW);
        }

        public void ShowSelectAccount()
        {
            Intent supplyAccount = new Intent(this, typeof(FeedbackSelectAccountActivity));
            if (selectedAccount != null)
            {
                supplyAccount.PutExtra(Constants.SELECTED_ACCOUNT, JsonConvert.SerializeObject(selectedAccount));
            }
            StartActivityForResult(supplyAccount, Constants.SELECT_ACCOUNT_REQUEST_CODE);
        }

        //handle push page with waiting return data
        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {

            try
            {
                base.OnActivityResult(requestCode, resultCode, data);
                if (requestCode == Constants.BARCODE_REQUEST_CODE)
                {
                    if (resultCode == Result.Ok)
                    {

                        string barcodeResultText = data.GetStringExtra(Constants.BARCODE_RESULT);
                        txtAccountNo.Text = barcodeResultText;

                    }
                }
                else if (requestCode == Constants.SELECT_ACCOUNT_REQUEST_CODE)
                {
                    if (resultCode == Result.Ok)
                    {
                        Bundle extras = data.Extras;

                        this.selectedAccount = JsonConvert.DeserializeObject<AccountData>(extras.GetString(Constants.SELECTED_ACCOUNT));
                        //selectedCustomerBillingAccount = CustomerBillingAccount.FindByAccNum(selectedAccount.AccountNum);

                        //injecting string into the accno
                        txtAccountNo.Text = selectedAccount.AccountNum;
                    }
                }

            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        [OnClick(Resource.Id.btnNext)]
        void OnupdatePersonalInfoConstraint(object sender, EventArgs eventArgs)
        {
            if (!this.GetIsClicked())
            {
                if (DownTimeEntity.IsBCRMDown())
                {
                    OnBCRMDownTimeErrorMessage();
                    this.SetIsClicked(false);
                }
                else
                {
                    string accno = txtAccountNo.Text.ToString().Trim();
                    string ic = txtNewIC.Text.ToString().Trim();
                    bool isAllowed = this.userActionsListener.CheckRequiredFields(accno, ic);

                    if (isAllowed)
                    {
                        this.SetIsClicked(true);
                        this.userActionsListener.ValidateAccountAsync(txtAccountNo.Text.ToString().Trim(), true);
                    }
                    else
                    {
                        this.SetIsClicked(false);
                    }

                }
            }
        }

        Snackbar mErrorMessageSnackBar;
        public void OnBCRMDownTimeErrorMessage(string message = null)
        {
            if (mErrorMessageSnackBar != null && mErrorMessageSnackBar.IsShown)
            {
                mErrorMessageSnackBar.Dismiss();
            }


            if (string.IsNullOrEmpty(message))
            {
                DownTimeEntity BCRMDownTime = DownTimeEntity.GetByCode(Constants.BCRM_SYSTEM);
                if (!string.IsNullOrEmpty(BCRMDownTime.DowntimeTextMessage))
                {
                    message = BCRMDownTime.DowntimeTextMessage;
                }
                else
                {
                    message = Utility.GetLocalizedErrorLabel("defaultErrorMessage");
                }
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

        public void showNextStep()
        {
            var updatePersoanlInfo = new Intent(this, typeof(AddAccUpdateIdetificationDetailsStepTwoActivity));

            updatePersoanlInfo.PutExtra(Constants.ACCOUNT_NUMBER, txtAccountNo.Text.ToString().Trim());
            updatePersoanlInfo.PutExtra(Constants.ACCOUNT_IC_NUMBER, txtNewIC.Text.ToString().Trim());
            updatePersoanlInfo.PutExtra(Constants.PAGE_TITLE, Utility.GetLocalizedLabel("SubmitEnquiry", "updatePersonalDetTitle"));
            updatePersoanlInfo.PutExtra(Constants.PAGE_STEP_TITLE, Utility.GetLocalizedLabel("SubmitEnquiry", "stepTitle1of3"));

            StartActivity(updatePersoanlInfo);
        }

        [OnClick(Resource.Id.infoLabelWhereIsMyAcc)]
        void OninfoLabelWhereIsMyAcc(object sender, EventArgs eventArgs)
        {
            if (!this.GetIsClicked())
            {


                this.SetIsClicked(true);
                this.userActionsListener.onShowWhereIsMyAcc();
            }
        }

        public async void ShowWhereIsMyAcc()
        {
            string base64Image = TooltipImageDirectEntity.GetImageBase64(TooltipImageDirectEntity.IMAGE_CATEGORY.WHERE_MY_ACC);

            if (!string.IsNullOrWhiteSpace(base64Image))
            {
                var imageCache = Base64ToBitmap(base64Image);

                MyTNBAppToolTipBuilder whereisMyacc = MyTNBAppToolTipBuilder.Create(this, MyTNBAppToolTipBuilder.ToolTipType.IMAGE_HEADER)
                 .SetHeaderImageBitmap(imageCache)
                .SetTitle(Utility.GetLocalizedLabel("SubmitEnquiry", "accNumberInfo"))
                .SetMessage(Utility.GetLocalizedLabel("SubmitEnquiry", "accNumberDetails"))
                .SetCTALabel(Utility.GetLocalizedCommonLabel("gotIt"))
                .SetCTAaction(() => { this.SetIsClicked(false); })
                .Build();
                whereisMyacc.Show();
            }
            else
            {
                this.SetIsClicked(false);
            }
        }

        private Task onGetTooltipImageContent()
        {
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    //check if image is exist in sql lite
                    var imageWhereAccUrl = SiteCoreConfig.SITECORE_URL + Utility.GetLocalizedLabel("SubmitEnquiry", "imageWhereAcc");

                    if (TooltipImageDirectEntity.isNeedUpdate(imageWhereAccUrl, TooltipImageDirectEntity.IMAGE_CATEGORY.WHERE_MY_ACC))
                    {
                        TooltipImageDirectEntity.DeleteImage(TooltipImageDirectEntity.IMAGE_CATEGORY.WHERE_MY_ACC);

                        var image = ImageUtils.GetImageBitmapFromUrl(imageWhereAccUrl);
                        var base64 = BitmapToBase64(image);

                        TooltipImageDirectEntity newImage_WHERE_MY_ACC = new TooltipImageDirectEntity();
                        newImage_WHERE_MY_ACC.ImageBase64 = base64;
                        newImage_WHERE_MY_ACC.ImageCategory = TooltipImageDirectEntity.IMAGE_CATEGORY.WHERE_MY_ACC.ToString();
                        newImage_WHERE_MY_ACC.Url = imageWhereAccUrl;

                        TooltipImageDirectEntity.InsertItem(newImage_WHERE_MY_ACC);

                    }
                    else
                    {
                        // recheck local is the base64 exist or not is not need update
                        string base64Image = TooltipImageDirectEntity.GetImageBase64(TooltipImageDirectEntity.IMAGE_CATEGORY.WHERE_MY_ACC);
                        if (string.IsNullOrWhiteSpace(base64Image))
                        {
                            TooltipImageDirectEntity.DeleteImage(TooltipImageDirectEntity.IMAGE_CATEGORY.WHERE_MY_ACC);

                            var image = ImageUtils.GetImageBitmapFromUrl(imageWhereAccUrl);
                            var base64 = BitmapToBase64(image);

                            TooltipImageDirectEntity newImage_WHERE_MY_ACC = new TooltipImageDirectEntity();
                            newImage_WHERE_MY_ACC.ImageBase64 = base64;
                            newImage_WHERE_MY_ACC.ImageCategory = TooltipImageDirectEntity.IMAGE_CATEGORY.WHERE_MY_ACC.ToString();
                            newImage_WHERE_MY_ACC.Url = imageWhereAccUrl;

                            TooltipImageDirectEntity.InsertItem(newImage_WHERE_MY_ACC);

                        }

                    }

                    //check if image is exist in sql lite
                    var imageIC = SiteCoreConfig.SITECORE_URL + Utility.GetLocalizedLabel("SubmitEnquiry", "imageCopyIC");

                    if (TooltipImageDirectEntity.isNeedUpdate(imageIC, TooltipImageDirectEntity.IMAGE_CATEGORY.IC_SAMPLE))
                    {
                        TooltipImageDirectEntity.DeleteImage(TooltipImageDirectEntity.IMAGE_CATEGORY.IC_SAMPLE);

                        var image = ImageUtils.GetImageBitmapFromUrl(imageIC);
                        var base64 = BitmapToBase64(image);

                        TooltipImageDirectEntity newImage_WHERE_MY_ACC = new TooltipImageDirectEntity();
                        newImage_WHERE_MY_ACC.ImageBase64 = base64;
                        newImage_WHERE_MY_ACC.ImageCategory = TooltipImageDirectEntity.IMAGE_CATEGORY.IC_SAMPLE.ToString();
                        newImage_WHERE_MY_ACC.Url = imageIC;

                        TooltipImageDirectEntity.InsertItem(newImage_WHERE_MY_ACC);

                    }
                    else
                    {
                        // recheck local is the base64 exist or not is not need update
                        string base64Image = TooltipImageDirectEntity.GetImageBase64(TooltipImageDirectEntity.IMAGE_CATEGORY.IC_SAMPLE);
                        if (string.IsNullOrWhiteSpace(base64Image))
                        {

                            TooltipImageDirectEntity.DeleteImage(TooltipImageDirectEntity.IMAGE_CATEGORY.IC_SAMPLE);

                            var image = ImageUtils.GetImageBitmapFromUrl(imageIC);
                            var base64 = BitmapToBase64(image);

                            TooltipImageDirectEntity newImage_IC_SAMPLE = new TooltipImageDirectEntity();
                            newImage_IC_SAMPLE.ImageBase64 = base64;
                            newImage_IC_SAMPLE.ImageCategory = TooltipImageDirectEntity.IMAGE_CATEGORY.IC_SAMPLE.ToString();
                            newImage_IC_SAMPLE.Url = imageIC;

                            TooltipImageDirectEntity.InsertItem(newImage_IC_SAMPLE);

                        }
                    }
                    //check if image is exist in sql lite   imagePermises
                    var imageConsent = SiteCoreConfig.SITECORE_URL + Utility.GetLocalizedLabel("SubmitEnquiry", "imageConsent");

                    if (TooltipImageDirectEntity.isNeedUpdate(imageConsent, TooltipImageDirectEntity.IMAGE_CATEGORY.PROOF_OF_CONSENT))
                    {
                        TooltipImageDirectEntity.DeleteImage(TooltipImageDirectEntity.IMAGE_CATEGORY.PROOF_OF_CONSENT);

                        var image_consent = ImageUtils.GetImageBitmapFromUrl(imageConsent);
                        var base64 = BitmapToBase64(image_consent);

                        TooltipImageDirectEntity newImage_PROOF_OF_CONSENT = new TooltipImageDirectEntity();
                        newImage_PROOF_OF_CONSENT.ImageBase64 = base64;
                        newImage_PROOF_OF_CONSENT.ImageCategory = TooltipImageDirectEntity.IMAGE_CATEGORY.PROOF_OF_CONSENT.ToString();
                        newImage_PROOF_OF_CONSENT.Url = imageConsent;

                        TooltipImageDirectEntity.InsertItem(newImage_PROOF_OF_CONSENT);

                    }
                    else
                    {
                        // recheck local is the base64 exist or not is not need update
                        string base64Image = TooltipImageDirectEntity.GetImageBase64(TooltipImageDirectEntity.IMAGE_CATEGORY.PROOF_OF_CONSENT);
                        if (string.IsNullOrWhiteSpace(base64Image))
                        {
                            TooltipImageDirectEntity.DeleteImage(TooltipImageDirectEntity.IMAGE_CATEGORY.PROOF_OF_CONSENT);

                            var image_consent = ImageUtils.GetImageBitmapFromUrl(imageConsent);
                            var base64 = BitmapToBase64(image_consent);

                            TooltipImageDirectEntity newImage_PROOF_OF_CONSENT = new TooltipImageDirectEntity();
                            newImage_PROOF_OF_CONSENT.ImageBase64 = base64;
                            newImage_PROOF_OF_CONSENT.ImageCategory = TooltipImageDirectEntity.IMAGE_CATEGORY.PROOF_OF_CONSENT.ToString();
                            newImage_PROOF_OF_CONSENT.Url = imageConsent;

                            TooltipImageDirectEntity.InsertItem(newImage_PROOF_OF_CONSENT);
                        }

                    }
                    //check if image is exist in sql lite   
                    var imagePermises = SiteCoreConfig.SITECORE_URL + Utility.GetLocalizedLabel("SubmitEnquiry", "imagePermises");

                    if (TooltipImageDirectEntity.isNeedUpdate(imagePermises, TooltipImageDirectEntity.IMAGE_CATEGORY.PERMISE_IMAGE))
                    {
                        TooltipImageDirectEntity.DeleteImage(TooltipImageDirectEntity.IMAGE_CATEGORY.PERMISE_IMAGE);

                        var image_Permises = ImageUtils.GetImageBitmapFromUrl(imagePermises);
                        var base64 = BitmapToBase64(image_Permises);

                        TooltipImageDirectEntity newImage_PERMISE_IMAGE = new TooltipImageDirectEntity();
                        newImage_PERMISE_IMAGE.ImageBase64 = base64;
                        newImage_PERMISE_IMAGE.ImageCategory = TooltipImageDirectEntity.IMAGE_CATEGORY.PERMISE_IMAGE.ToString();
                        newImage_PERMISE_IMAGE.Url = imagePermises;

                        TooltipImageDirectEntity.InsertItem(newImage_PERMISE_IMAGE);

                    }
                    else
                    {
                        // recheck local is the base64 exist or not is not need update
                        string base64Image = TooltipImageDirectEntity.GetImageBase64(TooltipImageDirectEntity.IMAGE_CATEGORY.PERMISE_IMAGE);
                        if (string.IsNullOrWhiteSpace(base64Image))
                        {
                            TooltipImageDirectEntity.DeleteImage(TooltipImageDirectEntity.IMAGE_CATEGORY.PERMISE_IMAGE);

                            var image_Permises = ImageUtils.GetImageBitmapFromUrl(imagePermises);
                            var base64 = BitmapToBase64(image_Permises);

                            TooltipImageDirectEntity newImage_PERMISE_IMAGE = new TooltipImageDirectEntity();
                            newImage_PERMISE_IMAGE.ImageBase64 = base64;
                            newImage_PERMISE_IMAGE.ImageCategory = TooltipImageDirectEntity.IMAGE_CATEGORY.PERMISE_IMAGE.ToString();
                            newImage_PERMISE_IMAGE.Url = imagePermises;

                            TooltipImageDirectEntity.InsertItem(newImage_PERMISE_IMAGE);
                        }

                    }
                }
                catch (Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                }
            });
        }

        public string BitmapToBase64(Bitmap bitmap)
        {
            string B64Output = "";
            try
            {
                MemoryStream byteArrayOutputStream = new MemoryStream();
                bitmap.Compress(Bitmap.CompressFormat.Png, 100, byteArrayOutputStream);
                byte[] byteArray = byteArrayOutputStream.ToArray();
                B64Output = Android.Util.Base64.EncodeToString(byteArray, Base64Flags.Default);
            }
            catch (Exception e)
            {
                B64Output = "";
                Utility.LoggingNonFatalError(e);
            }

            return B64Output;
        }

        public static Bitmap Base64ToBitmap(string base64String)
        {
            Bitmap convertedBitmap = null;
            try
            {
                byte[] imageAsBytes = Android.Util.Base64.Decode(base64String, Base64Flags.Default);
                convertedBitmap = BitmapFactory.DecodeByteArray(imageAsBytes, 0, imageAsBytes.Length);
            }
            catch (Exception e)
            {
                convertedBitmap = null;
                Utility.LoggingNonFatalError(e);
            }

            return convertedBitmap;
        }

        Snackbar newErrorMessageSnackBar;
        public void OnSubmitError(string message = null)
        {
            if (newErrorMessageSnackBar != null && newErrorMessageSnackBar.IsShown)
            {
                newErrorMessageSnackBar.Dismiss();
            }


            if (string.IsNullOrEmpty(message))
            {
                message = Utility.GetLocalizedErrorLabel("defaultErrorMessage");
            }

            newErrorMessageSnackBar = Snackbar.Make(rootView, message, Snackbar.LengthIndefinite)
            .SetAction(Utility.GetLocalizedCommonLabel("close"), delegate { newErrorMessageSnackBar.Dismiss(); }
            );
            View v = newErrorMessageSnackBar.View;
            TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
            tv.SetMaxLines(5);

            newErrorMessageSnackBar.Show();
            this.SetIsClicked(false);
        }
    }
}