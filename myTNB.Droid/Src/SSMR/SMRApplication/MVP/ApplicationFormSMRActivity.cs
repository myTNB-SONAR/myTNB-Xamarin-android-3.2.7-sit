
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.Content;
using Android.Text;
using Android.Util;
using Android.Views;
using Android.Widget;
using CheeseBind;
using myTNB_Android.Src.Base;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.SelectSupplyAccount.Activity;
using myTNB_Android.Src.Utils;
using myTNB_Android.Src.Utils.Custom.ProgressDialog;
using Newtonsoft.Json;
using static myTNB_Android.Src.SSMR.SMRApplication.Api.CARegisteredContactInfoResponse;
using static myTNB_Android.Src.SSMR.SMRApplication.Api.SMRregistrationSubmitResponse;

namespace myTNB_Android.Src.SSMR.SMRApplication.MVP
{
    [Activity(Label = "Apply Self Meter Reading", Theme = "@style/Theme.Dashboard")]
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

        [BindView(Resource.Id.btnSubmitRegistration)]
        Button btnSubmitRegistration;

        ApplicationFormSMRPresenter mPresenter;
        LoadingOverlay loadingOverlay;

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
            selectAccountContainer.Click += delegate
            {
                Intent intent = new Intent(this, typeof(SelectSMRAccountActivity));
                StartActivityForResult(intent,1);
            };

            TextViewUtils.SetMuseoSans300Typeface(selectAccountContainer,applySMRAddress,txtTermsAndCondition,txtEmail,txtMobileNumber,txtEditingNote);
            TextViewUtils.SetMuseoSans500Typeface(applySMRForLabel, applySMRContactLabel);

            txtMobileNumber.TextChanged += TextChange;

            txtMobileNumber.AddTextChangedListener(new InputFilterFormField(txtMobileNumber, textInputMobile));
            txtEmail.AddTextChangedListener(new InputFilterFormField(txtEmail, textInputEmail));
            txtMobileNumber.SetFilters(new Android.Text.IInputFilter[] { new InputFilterPhoneNumber() });
            GetCARegisteredContactInfo();
        }

        public void GetCARegisteredContactInfo()
        {
            SMRAccount sMRAccount = UserSessions.GetSMRAccountList().Find(smrAccount =>
            {
                return smrAccount.accountSelected;
            });
            selectAccountContainer.Text = sMRAccount.accountName;
            applySMRAddress.Text = sMRAccount.accountAddress;
            if (sMRAccount.email != txtEmail.Text || sMRAccount.mobileNumber != txtMobileNumber.Text)
            {
                ShowProgressDialog();
                mPresenter.GetCARegisteredContactInfoAsync(sMRAccount);
            }
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
            selectAccountContainer.Text = account.accountName;
            txtEmail.Text = account.email;
            txtMobileNumber.Text = "+60" + account.mobileNumber;
            List<SMRAccount> updatedSMRAccountList = new List<SMRAccount>();
            foreach (SMRAccount smrAccount in UserSessions.GetSMRAccountList())
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
            UserSessions.SetSMRAccountList(updatedSMRAccountList);
            HideProgressDialog();
        }

        public void ShowProgressDialog()
        {
            try
            {
                if (loadingOverlay != null && loadingOverlay.IsShowing)
                {
                    loadingOverlay.Dismiss();
                }

                loadingOverlay = new LoadingOverlay(this, Resource.Style.LoadingOverlyDialogStyle);
                loadingOverlay.Show();
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
                if (loadingOverlay != null && loadingOverlay.IsShowing)
                {
                    loadingOverlay.Dismiss();
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        [OnClick(Resource.Id.btnSubmitRegistration)]
        void SubmitRegistration(object sender, EventArgs eventArgs)
        {
            SMRAccount sMRAccount = UserSessions.GetSMRAccountList().Find(smrAccount =>
            {
                return smrAccount.accountSelected;
            });

            ShowProgressDialog();
            mPresenter.SubmitSMRRegistration(sMRAccount,txtMobileNumber.Text,txtEmail.Text,"");
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
            textInputMobile.Error = GetString(Resource.String.registration_form_errors_invalid_mobile_no);
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
    }
}
