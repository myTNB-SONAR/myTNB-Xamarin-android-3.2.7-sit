
using System;
using System.Collections.Generic;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;


using Android.Text;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.Core.Content;
using CheeseBind;
using Google.Android.Material.TextField;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.TermsAndConditions.Activity;
using myTNB_Android.Src.Utils;

namespace myTNB_Android.Src.SSMR.SMRApplication.MVP
{
    [Activity(Label = "Self Meter Reading", Theme = "@style/Theme.RegisterForm")]
    public class ApplicationFormSMRActivity : BaseToolbarAppCompatActivity, ApplicationFormSMRContract.IView
    {
        [BindView(Resource.Id.applySMRForLabel)]
        TextView applySMRForLabel;

        [BindView(Resource.Id.applySMRAddress)]
        TextView applySMRAddress;

        [BindView(Resource.Id.applySMRContactLabel)]
        TextView applySMRContactLabel;

        [BindView(Resource.Id.txtTermsAndCondition)]
        TextView txtTermsAndCondition;

        [BindView(Resource.Id.txtEmail)]
        EditText txtEmail;

        [BindView(Resource.Id.txtMobileNumber)]
        EditText txtMobileNumber;

        [BindView(Resource.Id.textInputEmail)]
        TextInputLayout textInputEmail;

        [BindView(Resource.Id.textInputMobile)]
        TextInputLayout textInputMobile;

        [BindView(Resource.Id.txtEditingNote)]
        TextView txtEditingNote;

        [BindView(Resource.Id.selector_smr_account)]
        TextView selectAccountContainer;

        [BindView(Resource.Id.txtAccountType)]
        TextView txtAccountType;


        [BindView(Resource.Id.btnSubmitRegistration)]
        Button btnSubmitRegistration;

        ApplicationFormSMRPresenter mPresenter;

        bool checkForEditingInfo = false;

        //String email, mobileNumber;
        public override int ResourceId()
        {
            return Resource.Layout.ApplicationFormSMRLayout;
        }

        public override bool ShowCustomToolbarTitle()
        {
            return true;
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            if (requestCode == 1)
            {
                if (resultCode == Result.Canceled)
                {
                    GetCARegisteredContactInfo();
                    checkForEditingInfo = false;
                    txtEditingNote.Visibility = ViewStates.Gone;
                }
            }
            base.OnActivityResult(requestCode, resultCode, data);
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            mPresenter = new ApplicationFormSMRPresenter(this);

            TextViewUtils.SetMuseoSans300Typeface(selectAccountContainer, applySMRAddress, txtTermsAndCondition, txtEmail, txtMobileNumber, txtEditingNote);
            TextViewUtils.SetMuseoSans500Typeface(applySMRForLabel, applySMRContactLabel);
            TextViewUtils.SetTextSize16(btnSubmitRegistration);

            txtEmail.TextChanged += TextChange;
            txtMobileNumber.TextChanged += TextChange;

            if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.N)
            {
                txtTermsAndCondition.TextFormatted = Html.FromHtml(GetString(Resource.String.ssmr_subscribe_terms_conditions), FromHtmlOptions.ModeLegacy);
            }
            else
            {
                txtTermsAndCondition.TextFormatted = Html.FromHtml(GetString(Resource.String.ssmr_subscribe_terms_conditions));
            }

            txtMobileNumber.AddTextChangedListener(new InputFilterFormField(txtMobileNumber, textInputMobile));
            txtEmail.AddTextChangedListener(new InputFilterFormField(txtEmail, textInputEmail));
            txtMobileNumber.SetFilters(new Android.Text.IInputFilter[] { new InputFilterPhoneNumber() });
            OnInitiateSMREligibilityAccount();
            selectAccountContainer.Click += delegate
            {
                if (!this.GetIsClicked())
                {
                    this.SetIsClicked(true);
                    List<SMRAccount> list = UserSessions.GetRealSMREligibilityAccountList();
                    if (list == null)
                    {
                        list = UserSessions.GetSMREligibilityAccountList();
                    }
                    if (list != null && list.Count > 0)
                    {
                        Intent intent = new Intent(this, typeof(SelectSMRAccountActivity));
                        StartActivityForResult(intent, 1);
                    }
                    else
                    {
                        Intent intent = new Intent(this, typeof(SelectSMRAccountEmptyActivity));
                        StartActivity(intent);
                    }
                }
            };
            TextViewUtils.SetTextSize12(txtAccountType, txtEditingNote, txtTermsAndCondition);
            TextViewUtils.SetTextSize14(applySMRAddress);
            TextViewUtils.SetTextSize16(applySMRForLabel, selectAccountContainer, applySMRContactLabel);
        }

        public void GetCARegisteredContactInfo()
        {
            List<SMRAccount> list = UserSessions.GetRealSMREligibilityAccountList();
            if (list == null)
            {
                list = UserSessions.GetSMREligibilityAccountList();
            }
            SMRAccount sMRAccount = list.Find(smrAccount =>
            {
                return smrAccount.accountSelected;
            });
            if (sMRAccount != null)
            {
                selectAccountContainer.Text = sMRAccount.accountName;
                applySMRAddress.Visibility = ViewStates.Visible;
                applySMRAddress.Text = sMRAccount.accountAddress;
                if (sMRAccount.email != txtEmail.Text || sMRAccount.mobileNumber != txtMobileNumber.Text)
                {
                    ShowProgressDialog();
                    mPresenter.GetCARegisteredContactInfoAsync(sMRAccount);
                }
            }
        }

        private void OnInitiateSMREligibilityAccount()
        {
            selectAccountContainer.Text = "Select an account";
            applySMRAddress.Visibility = ViewStates.Gone;
            txtEmail.Enabled = false;
            txtMobileNumber.Enabled = false;
            DisableRegisterButton();
            this.mPresenter.CheckSMRAccountEligibility();
        }

        public override View OnCreateView(string name, Context context, IAttributeSet attrs)
        {
            return base.OnCreateView(name, context, attrs);
        }

        public void ShowSelectAccount()
        {
            //throw new NotImplementedException();
        }

        public void UpdateSMRInfo(SMRAccount account)
        {
            checkForEditingInfo = false;
            selectAccountContainer.Text = account.accountName;
            checkForEditingInfo = false;
            txtEmail.Text = account.email;
            checkForEditingInfo = false;
            if (!account.mobileNumber.Contains("+60"))
            {
                account.mobileNumber = "+60" + account.mobileNumber;
            }
            txtMobileNumber.Text = account.mobileNumber;
            List<SMRAccount> updatedSMRAccountList = new List<SMRAccount>();
            List<SMRAccount> list = UserSessions.GetRealSMREligibilityAccountList();
            if (list == null)
            {
                list = UserSessions.GetSMREligibilityAccountList();
            }
            foreach (SMRAccount smrAccount in list)
            {
                if (smrAccount.accountNumber == account.accountNumber)
                {
                    smrAccount.email = account.email;
                    smrAccount.mobileNumber = account.mobileNumber;
                    smrAccount.accountSelected = account.accountSelected;
                    updatedSMRAccountList.Add(smrAccount);
                }
                else
                {

                    updatedSMRAccountList.Add(smrAccount);
                }
            }
            UserSessions.SetRealSMREligibilityAccountList(updatedSMRAccountList);
            txtEmail.Enabled = true;
            txtMobileNumber.Enabled = true;
            HideProgressDialog();
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

        [OnClick(Resource.Id.btnSubmitRegistration)]
        void SubmitRegistration(object sender, EventArgs eventArgs)
        {
            if (!this.GetIsClicked())
            {
                this.SetIsClicked(true);
                List<SMRAccount> list = UserSessions.GetRealSMREligibilityAccountList();
                if (list == null)
                {
                    list = UserSessions.GetSMREligibilityAccountList();
                }
                SMRAccount sMRAccount = list.Find(smrAccount =>
                {
                    return smrAccount.accountSelected;
                });

                ShowProgressDialog();
                mPresenter.SubmitSMRRegistration(sMRAccount, txtMobileNumber.Text, txtEmail.Text, "");
            }
        }

        public void ShowSubmitSuccessResult(string jsonResponse)
        {
            Intent intent = new Intent(this, typeof(SubmiSMRAccountCompleteActivity));
            intent.PutExtra("SUBMIT_RESULT", jsonResponse);
            StartActivity(intent);
            HideProgressDialog();
        }

        public void ShowSubmitFailedResult(string jsonResponse)
        {
            Intent intent = new Intent(this, typeof(SubmiSMRAccountFailedActivity));
            intent.PutExtra("SUBMIT_RESULT", jsonResponse);
            StartActivity(intent);
            HideProgressDialog();
        }

        private void TextChange(object sender, TextChangedEventArgs e)
        {
            try
            {
                string mobile_no = txtMobileNumber.Text.ToString().Trim();
                string email = txtEmail.Text.ToString().Trim();
                this.mPresenter.CheckRequiredFields(mobile_no, email);
                if (checkForEditingInfo)
                {
                    txtEditingNote.Visibility = ViewStates.Visible;
                }
                checkForEditingInfo = true;
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }

        public void ShowInvalidMobileNoError()
        {
            textInputMobile.Error = Utility.GetLocalizedErrorLabel("invalid_mobileNumber");
        }

        public void DisableRegisterButton()
        {
            btnSubmitRegistration.Enabled = false;
            btnSubmitRegistration.Background = ContextCompat.GetDrawable(this, Resource.Drawable.silver_chalice_button_background);
        }

        public void ClearInvalidMobileError()
        {
            textInputMobile.Error = null;
        }

        public void EnableRegisterButton()
        {
            btnSubmitRegistration.Enabled = true;
            btnSubmitRegistration.Background = ContextCompat.GetDrawable(this, Resource.Drawable.green_button_background);
        }

        public string GetDeviceId()
        {
            return this.DeviceId();
        }

        public void EnableButton()
        {
            this.SetIsClicked(false);
        }

        [OnClick(Resource.Id.txtTermsAndCondition)]
        void OnTermsConditions(object sender, EventArgs eventArgs)
        {
            if (!this.GetIsClicked())
            {
                this.SetIsClicked(true);
                StartActivity(typeof(TermsAndConditionActivity));
            }
        }

        public void ShowInvalidEmailError()
        {
            try
            {
                this.textInputEmail.Error = GetString(Resource.String.login_validation_email_invalid_error);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void ClearEmailError()
        {
            try
            {
                this.textInputEmail.Error = null;
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void ClearErrors()
        {
            try
            {
                ClearEmailError();
                ClearInvalidMobileError();
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
                FirebaseAnalyticsUtils.SetScreenName(this, "Apply SMR");
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }
    }
}
