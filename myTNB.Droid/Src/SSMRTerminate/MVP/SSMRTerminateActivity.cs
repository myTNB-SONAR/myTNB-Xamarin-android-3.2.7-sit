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
using myTNB_Android.Src.SSMR.SMRApplication.MVP;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.Content.PM;
using myTNB_Android.Src.SSMRTerminate.Api;

namespace myTNB_Android.Src.SSMRTerminate.MVP
{
    [Activity(Label = "Discontinue Self Reading"
        , ScreenOrientation = ScreenOrientation.Portrait
        , Theme = "@style/Theme.SMRApplication")]
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

        bool checkForEditingInfo = false;

        bool isOtherReasonSelected = false;

        private TerminationReasonModel selectedReason;

        private List<TerminationReasonModel> terminationList = new List<TerminationReasonModel>();

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

            TextViewUtils.SetMuseoSans300Typeface(txtInputLayoutReason, txtInputLayoutEmail, txtInputLayoutMobileNo, txtInputLayoutTxtReason);
            TextViewUtils.SetMuseoSans500Typeface(btnDisconnectionSubmit, disconnectionTtile, disconnectionAccountTtile, contactDetailTtile, terminationReasonTitle);
            TextViewUtils.SetMuseoSans300Typeface(disconnectionAccountAddress, contactDetailConsent, txtTermsConditions, txtEmail, txtMobileNo, txtSelectReason, txtReason);

            contactDetailConsent.Visibility = ViewStates.Gone;
            txtInputLayoutTxtReason.Visibility = ViewStates.Gone;

            txtMobileNo.TextChanged += TextChange;
            txtEmail.TextChanged += TextChange;
            txtReason.TextChanged += Reason_TextChange;

            txtMobileNo.AddTextChangedListener(new InputFilterFormField(txtMobileNo, txtInputLayoutMobileNo));
            txtEmail.AddTextChangedListener(new InputFilterFormField(txtEmail, txtInputLayoutEmail));
            txtMobileNo.SetFilters(new Android.Text.IInputFilter[] { new InputFilterPhoneNumber() });

            // txtSelectReason.EnableClick();
            // txtSelectReason.SetOnTouchListener(this);

            if (string.IsNullOrEmpty(txtMobileNo.Text))
            {
                txtMobileNo.Text = "+60";
            }

            if (extras.ContainsKey(Constants.SELECTED_ACCOUNT))
            {
                selectedAccount = JsonConvert.DeserializeObject<AccountData>(extras.GetString(Constants.SELECTED_ACCOUNT));
                disconnectionAccountTtile.Text = selectedAccount.AccountNickName;
                disconnectionAccountAddress.Text = selectedAccount.AddStreet;
                this.mPresenter.InitiateCAInfo(selectedAccount);
            }

            this.mPresenter.InitiateTerminationReasonsList();
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

                if (!checkForEditingInfo)
                {
                    contactDetailConsent.Visibility = ViewStates.Visible;
                }
                checkForEditingInfo = true;
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }

        private void Reason_TextChange(object sender, TextChangedEventArgs e)
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
                this.txtInputLayoutEmail.Error = null;
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
            try
            {
                if (txtMobileNo.Text != mobile_no)
                {
                    txtMobileNo.Text = mobile_no;
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
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

        public void UpdateSMRData(string email, string mobile_no)
        {
            try
            {
                txtEmail.Text = email;
                txtMobileNo.Text = mobile_no;
                contactDetailConsent.Visibility = ViewStates.Gone;
                checkForEditingInfo = false;
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void SetTerminationReasonsList(List<TerminationReasonModel> list)
        {
            this.terminationList.Clear();
            this.terminationList.AddRange(list);
            if (this.terminationList.Count > 0)
            {
                selectedReason = this.terminationList[0];
                txtSelectReason.Text = selectedReason.ReasonName;
            }
            HideProgressDialog();
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

        public string GetDeviceId()
        {
            return this.DeviceId();
        }

        public bool IsActive()
        {
            return this.Window.DecorView.RootView.IsShown;
        }

    }
}
