using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Text;
using Android.Util;
using System.Net.Http;
using myTNB_Android.Src.Utils;
using Refit;
using myTNB_Android.Src.RegistrationForm.Api;
using System.Net;
using myTNB_Android.Src.RegistrationForm.Requests;
using System.Threading;
using myTNB_Android.Src.Login.Api;
using myTNB_Android.Src.Login.Requests;
using myTNB_Android.Src.Database.Model;
using Java.Lang;
using myTNB_Android.Src.RegistrationForm.Models;
using System.Text.RegularExpressions;
using Android.Telephony;
using Android.Content.PM;

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

            try {
            if (!TextUtils.IsEmpty(fullname) && !TextUtils.IsEmpty(icno) && !TextUtils.IsEmpty(mobile_no) && !TextUtils.IsEmpty(email) && !TextUtils.IsEmpty(confirm_email) && !TextUtils.IsEmpty(password) && !TextUtils.IsEmpty(confirm_password) )
            {

                if (!Utility.isAlphaNumeric(fullname))
                {
                    this.mView.ShowFullNameError();
                    this.mView.DisableRegisterButton();
                } else {
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


                if (!Utility.IsValidMobileNumber(mobile_no))
                {
                    this.mView.ShowInvalidMobileNoError();
                    this.mView.DisableRegisterButton();
                    return;
                }  else {
                    this.mView.ClearInvalidMobileError();
                        
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

            if (!Utility.isAlphaNumeric(fullname)) {
                this.mView.ShowFullNameError();
                return;
            }


            if (TextUtils.IsEmpty(icno))
            {
                this.mView.ShowEmptyICNoError();
                return;
            }

            if (TextUtils.IsEmpty(mobile_no))
            {
                this.mView.ShowEmptyMobileNoError();
                return;
            }

            if (!PhoneNumberUtils.IsGlobalPhoneNumber(mobile_no))
            {
                this.mView.ShowInvalidMobileNoError();
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

            this.mView.ShowRegistrationProgressDialog();
            this.mView.ClearAllErrorFields();

             ServicePointManager.ServerCertificateValidationCallback += SSLFactoryHelper.CertificateValidationCallBack;

#if DEBUG
            var httpClient = new HttpClient(new HttpLoggingHandler(/*new NativeMessageHandler()*/)) { BaseAddress = new Uri(Constants.SERVER_URL.END_POINT) };
            var api = RestService.For<IGetVerificationCode>(httpClient);
#else
            var api = RestService.For<IGetVerificationCode>(Constants.SERVER_URL.END_POINT);
#endif


            try
            {

                var verificationResponse = await api.GetVerificationCodeThruSMSV2(new VerificationCodeRequest(Constants.APP_CONFIG.API_KEY_ID)
                {
                    userEmail = email,
                    username = email,
                    mobileNo = mobile_no,
                    ipAddress = Constants.APP_CONFIG.API_KEY_ID,
                    clientType = Constants.APP_CONFIG.API_KEY_ID , 
                    activeUserName = Constants.APP_CONFIG.API_KEY_ID,
                    devicePlatform = Constants.APP_CONFIG.API_KEY_ID,
                    deviceVersion = Constants.APP_CONFIG.API_KEY_ID , 
                    deviceCordova = Constants.APP_CONFIG.API_KEY_ID

                });

                if (!verificationResponse.verificationCode.isError)
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
                    this.mView.ShowInvalidAcquiringTokenThruSMS(verificationResponse.verificationCode.message);
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


       

            this.mView.HideRegistrationProgressDialog();
        }

        public void Start()
        {
            try {
            this.mView.DisableRegisterButton();
            this.mView.ClearFields();

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
            try {
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
                if (grantResults[0] == Permission.Denied)
                {
                    //if (this.mView.ShouldShowSMSReceiveRationale())
                    //{
                    //    this.mView.ShowSMSPermissionRationale();
                    //}
                }

            }
        }

        public void OnRequestSMSPermission()
        {
            this.mView.RequestSMSPermission();
        }
    }
}