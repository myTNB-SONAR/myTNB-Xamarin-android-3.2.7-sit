using System;
using System.Runtime;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;
using Android.Views;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.Utils.Custom.ProgressDialog;
using Newtonsoft.Json;
using myTNB_Android.Src.Utils;
using CheeseBind;
using Android.Widget;
using Android.Support.Design.Widget;
using Android.Text;
using Android.Support.V4.Content;

namespace myTNB_Android.Src.SSMRTerminate.MVP
{
    [Activity(Label = "Discontinue Self Reading", Theme = "@style/Theme.Dashboard")]
    public class SSMRTerminateActivity : BaseToolbarAppCompatActivity, SSMRTerminateContract.IView
    {
        LoadingOverlay loadingOverlay;

        private AccountData selectedAccount;

        [BindView(Resource.Id.txtInputLayoutEmail)]
        TextInputLayout txtInputLayoutEmail;

        [BindView(Resource.Id.txtInputLayoutMobileNo)]
        TextInputLayout txtInputLayoutMobileNo;

        [BindView(Resource.Id.txtInputLayoutReason)]
        TextInputLayout txtInputLayoutReason;

        [BindView(Resource.Id.txtInputLayoutTxtReason)]
        TextInputLayout txtInputLayoutTxtReason;

        [BindView(Resource.Id.disconnectionAccountAddress)]
        TextView disconnectionAccountAddress;

        [BindView(Resource.Id.contactDetailConsent)]
        TextView contactDetailConsent;

        [BindView(Resource.Id.txtTermsConditions)]
        TextView txtTermsConditions;

        [BindView(Resource.Id.txtEmail)]
        EditText txtEmail;

        [BindView(Resource.Id.txtMobileNo)]
        EditText txtMobileNo;

        [BindView(Resource.Id.txtSelectReason)]
        EditText txtSelectReason;

        [BindView(Resource.Id.txtReason)]
        EditText txtReason;

        [BindView(Resource.Id.disconnectionTtile)]
        TextView disconnectionTtile;

        [BindView(Resource.Id.disconnectionAccountTtile)]
        TextView disconnectionAccountTtile;

        [BindView(Resource.Id.contactDetailTtile)]
        TextView contactDetailTtile;

        [BindView(Resource.Id.terminationReasonTitle)]
        TextView terminationReasonTitle;

        [BindView(Resource.Id.btnDisconnectionSubmit)]
        Button btnDisconnectionSubmit;

        SSMRTerminatePresenter mPresenter;

        bool isOtherReasonSelected = false;

        public override int ResourceId()
        {
            return Resource.Layout.SSMRDiscontinueApplicationLayout;
        }

        public override bool ShowCustomToolbarTitle()
        {
            return true;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            mPresenter = new SSMRTerminatePresenter(this);
            Bundle extras = Intent.Extras;

            if (extras.ContainsKey(Constants.SELECTED_ACCOUNT))
            {
                selectedAccount = JsonConvert.DeserializeObject<AccountData>(extras.GetString(Constants.SELECTED_ACCOUNT));
                disconnectionAccountTtile.Text = selectedAccount.AccountNickName;
                disconnectionAccountAddress.Text = selectedAccount.AddStreet;
            }

            TextViewUtils.SetMuseoSans300Typeface(txtInputLayoutReason, txtInputLayoutEmail, txtInputLayoutMobileNo, txtInputLayoutTxtReason);
            TextViewUtils.SetMuseoSans500Typeface(btnDisconnectionSubmit, disconnectionTtile, disconnectionAccountTtile, contactDetailTtile, terminationReasonTitle);
            TextViewUtils.SetMuseoSans300Typeface(disconnectionAccountAddress, contactDetailConsent, txtTermsConditions, txtEmail, txtMobileNo, txtSelectReason, txtReason);

            txtMobileNo.AddTextChangedListener(new InputFilterFormField(txtMobileNo, txtInputLayoutMobileNo));
            txtEmail.AddTextChangedListener(new InputFilterFormField(txtEmail, txtInputLayoutEmail));

            if (string.IsNullOrEmpty(txtMobileNo.Text))
            {
                txtMobileNo.Append("+60");
            }
            txtMobileNo.SetFilters(new Android.Text.IInputFilter[] { new InputFilterPhoneNumber() });

            txtMobileNo.TextChanged += TextChange;
            txtEmail.TextChanged += TextChange;
            txtReason.TextChanged += TextChange;

            txtEmail.ClearFocus();
        }

        public override View OnCreateView(string name, Context context, IAttributeSet attrs)
        {
            return base.OnCreateView(name, context, attrs);
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

        private void TextChange(object sender, TextChangedEventArgs e)
        {
            try
            {
                string mobile_no = txtMobileNo.Text.ToString().Trim();
                string email = txtEmail.Text.ToString().Trim();
                string reason = txtReason.Text.ToString().Trim();
                this.mPresenter.CheckRequiredFields(mobile_no, email, isOtherReasonSelected, reason);
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
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

        public void ClearErrors()
        {
            try
            {
                ClearEmailError();
                ClearInvalidMobileError();
                ClearReasonError();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void ShowEmptyEmailError()
        {
            try
            {
                this.txtInputLayoutEmail.Error = GetString(Resource.String.login_validation_email_empty_error);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void ShowInvalidEmailError()
        {
            try
            {
                this.txtInputLayoutEmail.Error = GetString(Resource.String.login_validation_email_invalid_error);
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
                txtEmail.Error = null;
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void DisableSubmitButton()
        {
            try
            {
                btnDisconnectionSubmit.Enabled = false;
                btnDisconnectionSubmit.Background = ContextCompat.GetDrawable(this, Resource.Drawable.silver_chalice_button_background);
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
                btnDisconnectionSubmit.Enabled = true;
                btnDisconnectionSubmit.Background = ContextCompat.GetDrawable(this, Resource.Drawable.green_button_background);
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
                txtInputLayoutMobileNo.Error = GetString(Resource.String.registration_form_errors_invalid_mobile_no);
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
                txtInputLayoutMobileNo.Error = null;
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void ShowEmptyReasonError()
        {
            try
            {
                txtInputLayoutTxtReason.Error = "Reason can't be empty";
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void UpdateMobileNumber(string mobile_no)
        {
            txtMobileNo.Text = mobile_no;
        }

        public void ClearReasonError()
        {
            try
            {
                txtInputLayoutTxtReason.Error = null;
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
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

    }
}
