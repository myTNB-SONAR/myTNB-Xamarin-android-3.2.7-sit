using Android.Content.PM;
using Android.Telephony;
using Android.Text;
using Android.Util;
using myTNB_Android.Src.MyTNBService.Request;
using myTNB_Android.Src.MyTNBService.ServiceImpl;
using myTNB_Android.Src.RegistrationForm.Models;
using myTNB_Android.Src.RegistrationForm.Requests;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;
using Refit;
using System;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;

namespace myTNB_Android.Src.RegistrationForm.MVP
{
    public class RegisterFormPresenter : RegisterFormContract.IUserActionsListener
    {
        private RegisterFormContract.IView mView;
        private readonly string MIN_6_ALPHANUMERIC_PATTERN = "[a-zA-Z0-9#?!@$%^&*-]{6,}";
        private Regex hasNumber = new Regex(@"[0-9]+");
        private Regex hasUpperChar = new Regex(@"[a-zA-Z]+");
        private Regex hasMinimum8Chars = new Regex(@".{8,}");

        CancellationTokenSource registerCts;
        private readonly string TAG = typeof(RegisterFormPresenter).Name;

        public RegisterFormPresenter(RegisterFormContract.IView mView)
        {
            this.mView = mView;
            this.mView.SetPresenter(this);
        }

        public void CheckRequiredFields(string fullname, string icno, string mobile_no, string email, string confirm_email, string password, string confirm_password)
        {

            try
            {
                if (!TextUtils.IsEmpty(fullname) && !TextUtils.IsEmpty(icno) && !TextUtils.IsEmpty(mobile_no) && !TextUtils.IsEmpty(email) && !TextUtils.IsEmpty(confirm_email) && !TextUtils.IsEmpty(password) && !TextUtils.IsEmpty(confirm_password))
                {

                    if (!Utility.isAlphaNumeric(fullname))
                    {
                        this.mView.ShowFullNameError();
                        this.mView.DisableRegisterButton();
                    }
                    else
                    {
                        this.mView.ClearFullNameError();
                    }


                    if (!Patterns.EmailAddress.Matcher(email).Matches())
                    {
                        this.mView.ShowInvalidEmailError();
                        this.mView.DisableRegisterButton();
                        return;
                    }
                    else
                    {
                        this.mView.ClearInvalidEmailError();
                    }

                    if (!email.Equals(confirm_email))
                    {
                        this.mView.ShowNotEqualConfirmEmailError();
                        this.mView.DisableRegisterButton();
                        return;
                    }
                    else
                    {
                        this.mView.ClearNotEqualConfirmEmailError();
                    }

                    if (!CheckPasswordIsValid(password))
                    {
                        this.mView.ShowPasswordMinimumOf6CharactersError();
                        this.mView.DisableRegisterButton();
                        return;
                    }
                    else
                    {
                        this.mView.ClearPasswordMinimumOf6CharactersError();
                    }

                    if (!password.Equals(confirm_password))
                    {
                        this.mView.ShowNotEqualConfirmPasswordError();
                        this.mView.DisableRegisterButton();
                        return;
                    }
                    else
                    {
                        this.mView.ClearNotEqualConfirmPasswordError();
                    }

                    this.mView.EnableRegisterButton();
                }
                else
                {
                    this.mView.DisableRegisterButton();
                }
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void GoBack()
        {
            this.mView.ShowBackScreen();
        }

        public void NavigateToTermsAndConditions()
        {
            this.mView.ShowTermsAndConditions();
        }

        public async void OnAcquireToken(string fullname, string icno, string mobile_no, string email, string confirm_email, string password, string confirm_password)
        {
            registerCts = new CancellationTokenSource();
            this.mView.ClearAllErrorFields();
            if (TextUtils.IsEmpty(fullname))
            {
                this.mView.ShowEmptyFullNameError();
                return;
            }

            if (!Utility.isAlphaNumeric(fullname))
            {
                this.mView.ShowFullNameError();
                return;
            }

            if (TextUtils.IsEmpty(icno))
            {
                this.mView.ShowEmptyICNoError();
                return;
            }

            if (TextUtils.IsEmpty(email))
            {
                this.mView.ShowEmptyEmailError();
                return;
            }


            if (!Patterns.EmailAddress.Matcher(email).Matches())
            {
                this.mView.ShowInvalidEmailError();
                return;
            }


            if (TextUtils.IsEmpty(confirm_email))
            {
                this.mView.ShowEmptyConfirmEmailError();
                return;
            }

            if (!email.Equals(confirm_email))
            {
                this.mView.ShowNotEqualConfirmEmailError();
                return;
            }

            if (TextUtils.IsEmpty(password))
            {
                this.mView.ShowEmptyPasswordError();
                return;
            }

            if (!CheckPasswordIsValid(password))
            {
                this.mView.ShowPasswordMinimumOf6CharactersError();
                return;
            }

            if (TextUtils.IsEmpty(confirm_password))
            {
                this.mView.ShowEmptyConfirmPasswordError();
                return;
            }

            if (!password.Equals(confirm_password))
            {
                this.mView.ShowNotEqualConfirmPasswordError();
                return;
            }

            this.mView.ShowProgressDialog();
            this.mView.ClearAllErrorFields();

            try
            {
                SendRegistrationTokenSMSRequest sendRegistrationTokenSMSRequest = new SendRegistrationTokenSMSRequest(mobile_no);
                sendRegistrationTokenSMSRequest.SetUserName(email);
                // string dt = JsonConvert.SerializeObject(sendRegistrationTokenSMSRequest);
                var verificationResponse = await ServiceApiImpl.Instance.SendRegistrationTokenSMS(sendRegistrationTokenSMSRequest);

                if (verificationResponse.IsSuccessResponse())
                {
                    var userCredentials = new UserCredentialsEntity()
                    {
                        Fullname = fullname,
                        ICNo = icno,
                        MobileNo = mobile_no,
                        Email = email,
                        ConfirmEmail = confirm_email,
                        Password = password,
                        ConfirmPassword = confirm_password,

                    };

                    // TODO : SHOW OTP SCREEN
                    this.mView.ShowRegisterValidation(userCredentials);
                }
                else
                {
                    this.mView.ShowInvalidAcquiringTokenThruSMS(verificationResponse.Response.DisplayMessage);
                }

            }
            catch (System.OperationCanceledException e)
            {
                Log.Debug(TAG, "Cancelled Exception");
                // ADD OPERATION CANCELLED HERE
                this.mView.ShowRetryOptionsCancelledException(e);
                Utility.LoggingNonFatalError(e);
            }
            catch (ApiException apiException)
            {
                // ADD HTTP CONNECTION EXCEPTION HERE
                this.mView.ShowRetryOptionsApiException(apiException);
                Utility.LoggingNonFatalError(apiException);
            }
            catch (System.Exception e)
            {
                // ADD UNKNOWN EXCEPTION HERE
                Log.Debug(TAG, "Stack " + e.StackTrace);
                this.mView.ShowRetryOptionsUnknownException(e);
                Utility.LoggingNonFatalError(e);
            }
            finally
            {
                this.mView.HideProgressDialog();
            }
        }

        public void Start()
        {
            try
            {
                this.mView.DisableRegisterButton();

                bool isGranted = this.mView.IsGrantedSMSReceivePermission();
                if (!isGranted)
                {
                    if (this.mView.ShouldShowSMSReceiveRationale())
                    {
                        //this.mView.ShowSMSPermissionRationale();
                    }
                    else
                    {
                        this.mView.RequestSMSPermission();
                    }


                }

            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public bool CheckPasswordIsValid(string password)
        {
            bool isValid = false;
            try
            {
                isValid = hasNumber.IsMatch(password) && hasUpperChar.IsMatch(password) && hasMinimum8Chars.IsMatch(password);
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
            return isValid;
        }

        public void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            // SILENTLY DIE , SMS RECEIVE IS ONLY OPTIONAL
            if (requestCode == Constants.RUNTIME_PERMISSION_SMS_REQUEST_CODE)
            {
                if (Utility.IsPermissionHasCount(grantResults))
                {
                    if (grantResults[0] == Permission.Denied)
                    {
                        //if (this.mView.ShouldShowSMSReceiveRationale())
                        //{
                        //    this.mView.ShowSMSPermissionRationale();
                        //}
                    }
                }
            }
        }

        public void OnRequestSMSPermission()
        {
            this.mView.RequestSMSPermission();
        }
    }
}
