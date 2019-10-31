﻿using System;
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
using Android.Runtime;
using myTNB_Android.Src.TermsAndConditions.Activity;
using myTNB_Android.Src.SSMR.SMRApplication.Api;
using myTNB_Android.Src.SSMRMeterHistory.MVP;
using static Android.Views.View;

namespace myTNB_Android.Src.SSMRTerminate.MVP
{
    [Activity(Label = "Self Meter Reading"
        , ScreenOrientation = ScreenOrientation.Portrait
        , Theme = "@style/Theme.SSMRMeterHistoryStyle")]
    public class SSMRTerminateActivity : BaseToolbarAppCompatActivity, SSMRTerminateContract.IView, View.IOnTouchListener
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

        [BindView(Resource.Id.contactDetailContainer)]
        LinearLayout contactDetailContainer; 

        [BindView(Resource.Id.reasonDetailContainer)]
        LinearLayout reasonDetailContainer;

        SSMRTerminatePresenter mPresenter;

        bool isOtherReasonSelected = false;

        bool isFetchCAComplete = false;

        bool isFetchTerminationListComplete = false;

        bool checkForEditingInfo = false;

        private TerminationReasonModel selectedReason;

        private List<TerminationReasonModel> terminationList = new List<TerminationReasonModel>();

        private static string oldEmail = "";

        private static string oldPhoneNumber = "";

        private static string newEmail = "";

        private static string newPhoneNumber = "";

        private static int SELECT_TERMINATION_ACTIVITY_CODE = 4321;

        public readonly static int SSMR_METER_HISTORY_ACTIVITY_CODE = 8796;

        public string SMR_ACTION = "";

        public CAContactDetailsModel contactDetails;

        public override int ResourceId()
        {
            return Resource.Layout.SSMRDiscontinueApplicationLayout;
        }

        public override bool ShowCustomToolbarTitle()
        {
            return true;
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            if (requestCode == SELECT_TERMINATION_ACTIVITY_CODE)
            {
                if (resultCode == Result.Ok)
                {
                    Bundle extras = data.Extras;

                    if (extras.ContainsKey(Constants.SMR_TERMINATION_REASON_KEY))
                    {
                        this.terminationList = JsonConvert.DeserializeObject<List<TerminationReasonModel>>(extras.GetString(Constants.SMR_TERMINATION_REASON_KEY));
                        UpdateTerminationIsSelectList();
                    }
                }
            }
            else if (requestCode == SSMR_METER_HISTORY_ACTIVITY_CODE)
            {
                if (resultCode == Result.Ok)
                {
                    SetResult(Result.Ok);
                    Finish();
                }
            }
            base.OnActivityResult(requestCode, resultCode, data);
        }

        private void UpdateTerminationIsSelectList()
        {
            try
            {
                for(int i = 0; i < terminationList.Count; i++)
                {
                    if (terminationList[i].IsSelected)
                    {
                        selectedReason = this.terminationList[i];
                        txtSelectReason.Text = selectedReason.ReasonName;
                        if (selectedReason.ReasonId == "1007")
                        {
                            isOtherReasonSelected = true;
                            txtInputLayoutTxtReason.Visibility = ViewStates.Visible;
                            txtReason.Text = "";
                        }
                        else
                        {
                            isOtherReasonSelected = false;
                            txtInputLayoutTxtReason.Visibility = ViewStates.Gone;
                            txtReason.Text = "";
                        }
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            mPresenter = new SSMRTerminatePresenter(this);
            Bundle extras = Intent.Extras;

            TextViewUtils.SetMuseoSans300Typeface(txtInputLayoutReason, txtInputLayoutEmail, txtInputLayoutMobileNo, txtInputLayoutTxtReason);
            TextViewUtils.SetMuseoSans500Typeface(btnDisconnectionSubmit, disconnectionTtile, disconnectionAccountTtile, contactDetailTtile, terminationReasonTitle);
            TextViewUtils.SetMuseoSans300Typeface(disconnectionAccountAddress, contactDetailConsent, txtTermsConditions, txtEmail, txtMobileNo, txtSelectReason, txtReason);

            // SetStatusBarGradientBackground();
            // SetToolbarGradientBackground();

            if (Android.OS.Build.VERSION.SdkInt >= Android.OS.Build.VERSION_CODES.N)
            {
                txtTermsConditions.TextFormatted = Html.FromHtml(GetString(Resource.String.ssmr_terms_conditions), FromHtmlOptions.ModeLegacy);
            }
            else
            {
                txtTermsConditions.TextFormatted = Html.FromHtml(GetString(Resource.String.ssmr_terms_conditions));
            }

            contactDetailConsent.Visibility = ViewStates.Gone;
            txtInputLayoutTxtReason.Visibility = ViewStates.Gone;
            txtInputLayoutTxtReason.CounterMaxLength = 550;

            txtMobileNo.TextChanged += TextChange;
            txtEmail.TextChanged += TextChange;
            txtReason.TextChanged += Reason_TextChange;

            txtMobileNo.AddTextChangedListener(new InputFilterFormField(txtMobileNo, txtInputLayoutMobileNo));
            txtEmail.AddTextChangedListener(new InputFilterFormField(txtEmail, txtInputLayoutEmail));
            txtMobileNo.SetFilters(new Android.Text.IInputFilter[] { new InputFilterPhoneNumber() });

            txtSelectReason.EnableClick();
            txtSelectReason.SetOnTouchListener(this);

            oldEmail = "";

            oldPhoneNumber = "";

            newEmail = "";

            newPhoneNumber = "";

            if (string.IsNullOrEmpty(txtMobileNo.Text))
            {
                txtMobileNo.Text = "+60";
            }

            //ShowProgressDialog();

            if (extras != null)
            {
                if (extras.ContainsKey("SMR_ACTION"))
                {
                    SMR_ACTION = extras.GetString("SMR_ACTION");
                }

                if (extras.ContainsKey("SMR_CONTACT_DETAILS"))
                {
                    contactDetails = JsonConvert.DeserializeObject<CAContactDetailsModel>(extras.GetString("SMR_CONTACT_DETAILS"));
                }

                if (extras.ContainsKey(Constants.SELECTED_ACCOUNT))
                {
                    selectedAccount = JsonConvert.DeserializeObject<AccountData>(extras.GetString(Constants.SELECTED_ACCOUNT));
                    disconnectionAccountTtile.Text = selectedAccount.AccountNickName;
                    disconnectionAccountAddress.Text = selectedAccount.AddStreet;

                    //this.mPresenter.InitiateCAInfo(selectedAccount);
                }


            }
            ShowUIDetails();
        }

        private void ShowUIDetails()
        {
            if (SMR_ACTION == Constants.SMR_ENABLE_FLAG)
            {
                disconnectionTtile.Text = "I am starting for:";
                terminationReasonTitle.Visibility = ViewStates.Gone;
                reasonDetailContainer.Visibility = ViewStates.Gone;
                ShowContactDetails();
            }
            else
            {
                disconnectionTtile.Text = "I am stopping for:";
                terminationReasonTitle.Visibility = ViewStates.Visible;
                reasonDetailContainer.Visibility = ViewStates.Visible;

                contactDetailTtile.Visibility = ViewStates.Gone;
                contactDetailContainer.Visibility = ViewStates.Gone;
                this.mPresenter.InitiateTerminationReasonsList();
                EnableSubmitButton();
            }
        }

        private void ShowContactDetails()
        {
            if (contactDetails != null && contactDetails.isAllowEdit)
            {
                contactDetailTtile.Visibility = ViewStates.Visible;
                contactDetailContainer.Visibility = ViewStates.Visible;

                checkForEditingInfo = false;
                txtEmail.Text = contactDetails.email;
                checkForEditingInfo = false;
                txtMobileNo.Text = contactDetails.mobile;

                contactDetailConsent.Visibility = ViewStates.Gone;
            }
            else
            {
                contactDetailTtile.Visibility = ViewStates.Gone;
                contactDetailContainer.Visibility = ViewStates.Gone;

                EnableSubmitButton();
            }
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
                newEmail = email;
                newPhoneNumber = mobile_no;
                this.mPresenter.CheckRequiredFieldsForApply(mobile_no, email);

                if (checkForEditingInfo)
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
                this.mPresenter.CheckRequiredFieldsForTerminate(isOtherReasonSelected, reason);
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
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
                if (eTxtView.Id == Resource.Id.txtSelectReason)
                {

                    if (e.Action == MotionEventActions.Up)
                    {
                        if (!this.GetIsClicked())
                        {
                            this.SetIsClicked(true);
                            Intent intent = new Intent(this, typeof(SSMRTerminationReasonSelectionActivity));
                            intent.PutExtra(Constants.SMR_TERMINATION_REASON_KEY, JsonConvert.SerializeObject(terminationList));
                            StartActivityForResult(intent, SELECT_TERMINATION_ACTIVITY_CODE);
                        }
                        return true;
                    }
                }
            }
            return false;
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
                this.txtInputLayoutEmail.Error = "Invalid email address";
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
                this.txtInputLayoutEmail.Error = "Invalid email address";
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

        [OnClick(Resource.Id.txtTermsConditions)]
        void OnTermsConditions(object sender, EventArgs eventArgs)
        {
            if (!this.GetIsClicked())
            {
                this.SetIsClicked(true);
                this.mPresenter.NavigateToTermsAndConditions();
            }
        }

        [OnClick(Resource.Id.btnDisconnectionSubmit)]
        void OnSubmitRequest(object sender, EventArgs eventArgs)
        {
            string terminationReason = "";
            string smrMode;

            if (SMR_ACTION == Constants.SMR_ENABLE_FLAG)
            {
                smrMode = "R";
            }
            else
            {
                smrMode = "T";
                if (isOtherReasonSelected)
                {
                    terminationReason = txtReason.Text;
                }
                else
                {
                    terminationReason = selectedReason.ReasonName;
                }
            }
            
            this.mPresenter.OnSubmitApplication(selectedAccount.AccountNum,
                oldEmail, FormatMobileNumberForSubmit(oldPhoneNumber), newEmail, FormatMobileNumberForSubmit(newPhoneNumber), terminationReason, smrMode);
        }

        private string FormatMobileNumberForSubmit(string mobileNumber)
        {
            string output = mobileNumber;
            if (mobileNumber != "" && mobileNumber.Contains("+60"))
            {
                output = mobileNumber.Replace("+60","0");
            }
            return output;
        }

        public void ShowTermsAndConditions()
        {
            StartActivity(typeof(TermsAndConditionActivity));
        }

        public void OnRequestSuccessful(SMRregistrationSubmitResponse response)
        {
            Intent intent = new Intent(this, typeof(TerminateSMRAccountSuccessActivity));
            if (response != null)
            {
                intent.PutExtra("SUBMIT_RESULT", JsonConvert.SerializeObject(response));
            }

            intent.PutExtra("SMR_ACTION", SMR_ACTION);

            if (selectedAccount != null)
            {
                intent.PutExtra(Constants.SELECTED_ACCOUNT, JsonConvert.SerializeObject(selectedAccount));
            }
            StartActivityForResult(intent, SSMR_METER_HISTORY_ACTIVITY_CODE);
        }

        public void OnRequestFailed(SMRregistrationSubmitResponse response)
        {
            Intent intent = new Intent(this, typeof(TerminateSMRAccountFailedActivity));
            if (response != null)
            {
                intent.PutExtra("SUBMIT_RESULT", JsonConvert.SerializeObject(response));
            }

            intent.PutExtra("SMR_ACTION", SMR_ACTION);

            if (selectedAccount != null)
            {
                intent.PutExtra(Constants.SELECTED_ACCOUNT, JsonConvert.SerializeObject(selectedAccount));
            }
            StartActivityForResult(intent, SSMR_METER_HISTORY_ACTIVITY_CODE);
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
                txtInputLayoutMobileNo.Error = "Invalid mobile number";
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
                checkForEditingInfo = false;
                txtEmail.Text = email;
                checkForEditingInfo = false;
                if (!mobile_no.Contains("+60"))
                {
                    mobile_no = "+60" + mobile_no;
                }
                txtMobileNo.Text = mobile_no;
                oldEmail = email;
                oldPhoneNumber = mobile_no;
                newEmail = email;
                newPhoneNumber = mobile_no;
                contactDetailConsent.Visibility = ViewStates.Gone;
                isFetchCAComplete = true;
                if (isFetchCAComplete && isFetchTerminationListComplete)
                {
                    HideProgressDialog();
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
                isFetchCAComplete = true;
                if (isFetchCAComplete && isFetchTerminationListComplete)
                {
                    HideProgressDialog();
                }
            }
        }

        public void SetTerminationReasonsList(List<TerminationReasonModel> list)
        {
            if (list != null)
            {
                this.terminationList.Clear();
                this.terminationList.AddRange(list);
                if (this.terminationList.Count > 0)
                {
                    selectedReason = this.terminationList[0];
                    selectedReason.IsSelected = true;
                    this.terminationList[0] = selectedReason;
                    txtSelectReason.Text = selectedReason.ReasonName;
                }
            }
            isFetchTerminationListComplete = true;
            if (isFetchCAComplete && isFetchTerminationListComplete)
            {
                HideProgressDialog();
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

        public string GetDeviceId()
        {
            return this.DeviceId();
        }

        public bool IsActive()
        {
            return this.Window.DecorView.RootView.IsShown;
        }

        protected override void OnResume()
        {
            base.OnResume();
            try
            {
                if (SMR_ACTION == Constants.SMR_ENABLE_FLAG)
                {
                    FirebaseAnalyticsUtils.SetScreenName(this, "Apply SMR");
                }
                else if (SMR_ACTION == Constants.SMR_DISABLE_FLAG)
                {
                    FirebaseAnalyticsUtils.SetScreenName(this, "SMR Termination");
                }
                else
                {
                    FirebaseAnalyticsUtils.SetScreenName(this, "Apply SMR");
                }
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

    }
}
