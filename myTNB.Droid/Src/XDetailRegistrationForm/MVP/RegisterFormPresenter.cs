using Android.Content.PM;
using Android.Telephony;
using Android.Text;
using Android.Util;
using myTNB_Android.Src.MyTNBService.Request;
using myTNB_Android.Src.MyTNBService.ServiceImpl;
using myTNB_Android.Src.XDetailRegistrationForm.Models;
using myTNB_Android.Src.XDetailRegistrationForm.Requests;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.Utils;
using Refit;
using System;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using Firebase.Iid;


namespace myTNB_Android.Src.XDetailRegistrationForm.MVP
{
    public class RegisterFormPresenter : RegisterFormContract.IUserActionsListener
    {
        private RegisterFormContract.IView mView;
        private readonly string MIN_6_ALPHANUMERIC_PATTERN = "[a-zA-Z0-9#?!@$%^&*-]{6,}";
        private Regex hasNumber = new Regex(@"[0-9]+");
        private Regex hasUpperChar = new Regex(@"[a-zA-Z]+");
        private Regex hasMinimum8Chars = new Regex(@".{8,}");
        private Regex hasMinimum12Chars = new Regex(@".{14,}");
        private Regex hasMinimum5until50Chars = new Regex(@".{5,50}");
        private Regex hasHyphens = new Regex(@"/(?([0-9]{3}))?([ .-]?)([0-9]{3})\2([0-9]{4})/");



        CancellationTokenSource registerCts;
        private readonly string TAG = typeof(RegisterFormPresenter).Name;

        public RegisterFormPresenter(RegisterFormContract.IView mView)
        {
            this.mView = mView;
            this.mView.SetPresenter(this);
        }

        public void CheckRequiredFields(string fullname, string icno, string mobile_no, string idtype, bool checkbox)
        {

            try
            {
                if (!TextUtils.IsEmpty(fullname) && !TextUtils.IsEmpty(icno) && !TextUtils.IsEmpty(mobile_no) && !TextUtils.IsEmpty(idtype) && (checkbox))
                {

                    if (!Utility.isAlphaNumeric(fullname))
                    {
                        this.mView.ShowFullNameError();
                        this.mView.DisableRegisterButton();
                        return;
                    }
                    else
                    {
                        this.mView.ClearFullNameError();
                    }
                    if (!CheckIdentificationIsValid(icno) && idtype.Equals("1"))
                    {
                        this.mView.ShowFullICError();
                        this.mView.DisableRegisterButton();
                        return;
                    }
                    else if (!CheckArmyIdIsValid(icno) && idtype.Equals("2"))
                    {
                        this.mView.ShowFullArmyIdError();
                        this.mView.DisableRegisterButton();
                        return;
                    }
                    else if (!CheckPassportIsValid(icno) && idtype.Equals("3"))
                    {
                        this.mView.ShowFullPassportError();
                        this.mView.DisableRegisterButton();
                        return;
                    }
                    else
                    {
                        this.mView.ClearICMinimumCharactersError();
                        this.mView.ShowIdentificationHint();
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

        public async void OnCheckID(string icno, string idtype)
        {           

            this.mView.ShowProgressDialog();
            //this.mView.ClearAllErrorFields();

            try
            {
                icno = icno.Replace("-", string.Empty);
                var userResponse = await ServiceApiImpl.Instance.UserAuthenticateIDOnly(new GetRegisteredUser(idtype, icno));
                if (!userResponse.IsSuccessResponse())
                {
                   if (userResponse.GetDataAll().IsRegistered)
                    {
                        this.mView.ShowInvalidIdentificationError();
                        this.mView.DisableRegisterButton();
                    }
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

        public async void OnAcquireToken(string fullname, string icno, string mobile_no, string email, string password, string idtype)
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

            icno = icno.Replace("-", string.Empty);
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

            this.mView.ShowProgressDialog();
            this.mView.ClearAllErrorFields();

            try
            {
                string fcmToken = String.Empty;

                if (FirebaseTokenEntity.HasLatest())
                {
                    fcmToken = FirebaseTokenEntity.GetLatest().FBToken;
                }
                else
                {
                    fcmToken = FirebaseInstanceId.Instance.Token;
                    FirebaseTokenEntity.InsertOrReplace(fcmToken, true);
                }

                    SendRegistrationTokenSMSRequest sendRegistrationTokenSMSRequest = new SendRegistrationTokenSMSRequest(mobile_no);
                    sendRegistrationTokenSMSRequest.SetUserName(email);
                    var verificationResponse = await ServiceApiImpl.Instance.SendRegistrationTokenSMS(sendRegistrationTokenSMSRequest);

                    if (verificationResponse.IsSuccessResponse())
                    {
                        var userCredentials = new UserCredentialsEntity()
                        {
                            Fullname = fullname,
                            ICNo = icno,
                            MobileNo = mobile_no,
                            Email = email,
                            Password = password,
                            IdType = idtype,
                        };

                        // TODO : SHOW OTP SCREEN
                        this.mView.ShowRegisterValidation(
                            userCredentials);
                    }
                    else
                    {
                        this.mView.ShowInvalidAcquiringTokenThruSMS(verificationResponse.Response.DisplayMessage);
                        //this.mView.ShowInvalidIdentificationError();

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

        public bool CheckIdentificationIsValid(string icno)
        {
            bool isValid = false;
            try
            {
                isValid = hasNumber.IsMatch(icno) && hasMinimum12Chars.IsMatch(icno);
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
            return isValid;
        }

        public bool CheckArmyIdIsValid(string icno)
        {
            bool isValid = false;
            try
            {
                isValid = hasNumber.IsMatch(icno) && hasMinimum5until50Chars.IsMatch(icno);
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
            return isValid;
        }
        public bool CheckPassportIsValid(string icno)
        {
            bool isValid = false;
            try
            {
                isValid = hasNumber.IsMatch(icno) && hasUpperChar.IsMatch(icno) && hasMinimum8Chars.IsMatch(icno);
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
