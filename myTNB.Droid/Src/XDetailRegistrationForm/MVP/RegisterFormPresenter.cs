using Android.Content.PM;
using Android.Telephony;
using Android.Text;
using Android.Util;
using myTNB.Android.Src.MyTNBService.Request;
using myTNB.Android.Src.MyTNBService.ServiceImpl;
using myTNB.Android.Src.XDetailRegistrationForm.Models;
using myTNB.Android.Src.XDetailRegistrationForm.Requests;
using myTNB.Android.Src.Database.Model;
using myTNB.Android.Src.Utils;
using Refit;
using System;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using Firebase.Iid;
using Newtonsoft.Json;
using myTNB.Android.Src.Base;
using System.Threading.Tasks;

namespace myTNB.Android.Src.XDetailRegistrationForm.MVP
{
    public class RegisterFormPresenter : RegisterFormContract.IUserActionsListener
    {
        private RegisterFormContract.IView mView;
        private readonly string MIN_6_ALPHANUMERIC_PATTERN = "[a-zA-Z0-9#?!@$%^&*-]{6,}";
        private Regex hasNumber = new Regex(@"[0-9]+");
        private Regex hasUpperChar = new Regex(@"[a-zA-Z]+");
        private Regex hasSpecialChar = new Regex(@"[_#<>/\|[{}:;'+=?!@$%^&*-]+");
        private Regex hasMinimum12Chars = new Regex(@".{12,12}$");
        private Regex hasMinimum5until50Chars = new Regex(@".{5,50}");
        private Regex hasHyphens = new Regex(@"/(?([0-9]{3}))?([ .-]?)([0-9]{3})\2([0-9]{4})/");



        CancellationTokenSource registerCts;
        private readonly string TAG = typeof(RegisterFormPresenter).Name;

        public RegisterFormPresenter(RegisterFormContract.IView mView)
        {
            this.mView = mView;
            this.mView.SetPresenter(this);
        }

        public bool validateField(string fullname, string icno, string mobile_no, string idtype, bool checkbox)
        {
            try
            {
                bool isCorrect = true;

                this.mView.DisableRegisterButton();

                if (string.IsNullOrEmpty(fullname))
                {
                    isCorrect = false;
                }

                if (string.IsNullOrEmpty(icno))
                {
                    isCorrect = false;
                }

                if (string.IsNullOrEmpty(mobile_no))
                {
                    isCorrect = false;
                }

                if (!checkbox)
                {
                    isCorrect = false;
                }

                string ic_no = icno.Replace("-", string.Empty);
                if (ic_no.Length < 12 && idtype == "1")
                {
                    isCorrect = false;
                }
                else if (icno.Length < 5 && icno.Length < 50 && idtype == "2")
                {
                    isCorrect = false;
                }
                else if (icno.Length < 5 && icno.Length < 50 && idtype == "3")
                {
                    isCorrect = false;
                }

                //handle button to enable or disable
                if (isCorrect == true)
                {
                    this.mView.EnableRegisterButton();
                    return true;
                }
                else
                {
                    this.mView.DisableRegisterButton();
                    return false;
                }
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
                return false;
            }
        }


        public void CheckRequiredFields(string fullname, string icno, string mobile_no, string idtype, bool checkbox)
        {

            try
            {
                if (!TextUtils.IsEmpty(fullname) && !TextUtils.IsEmpty(icno) && !TextUtils.IsEmpty(mobile_no) && !TextUtils.IsEmpty(idtype) && (checkbox))
                {

                    //if (!Utility.isAlphaNumeric(fullname) && !Utility.isSpecialcharacter(fullname))
                    //{
                    //    this.mView.ShowFullNameError();
                    //    this.mView.DisableRegisterButton();
                    //    return;
                    //}
                    //else
                    //{
                    //    this.mView.ClearFullNameError();
                    //}
                    string ic_no = icno.Replace("-", string.Empty);
                    if (!CheckIdentificationIsValid(ic_no) && idtype.Equals("1"))
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
                        //this.mView.ShowIdentificationHint();
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
                var task = Task.Run(async () => {  

                UserCredentialsEntity userEntity =  new UserCredentialsEntity();
                icno = icno.Replace("-", string.Empty);
                GetRegisteredUser getICVerify = new GetRegisteredUser(idtype, icno);
                getICVerify.SetUserName(userEntity.Email);
                var userResponse = await ServiceApiImpl.Instance.UserAuthenticateIDOnlyNew(getICVerify);

                //if (userResponse.GetDataAll().isActive)
                if (userResponse.Response.Data.isActive)
                {
                     MyTNBAccountManagement.GetInstance().SetIsIDUpdated(false);
                }
                else
                {
                    MyTNBAccountManagement.GetInstance().SetIsIDUpdated(true);
                     //this.mView.ShowInvalidAcquiringTokenThruSMS(userResponse.Response.DisplayMessage);
                }
                });
                Task.WaitAll(task);

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

            //if (!Utility.isAlphaNumeric(fullname) && !Utility.isSpecialcharacter(fullname))
            //{
            //    this.mView.ShowFullNameError();
            //    return;
            //}

            if (TextUtils.IsEmpty(icno))
            {
                this.mView.ShowEmptyICNoError();
                return;
            }

            if (idtype.Equals("1"))
            {
                icno = icno.Replace("-", string.Empty);
                if (!CheckIdentificationIsValid(icno))
                {
                    this.mView.ShowFullICError();
                    this.mView.DisableRegisterButton();
                    return;

                }
                else
                {
                    this.mView.ShowIdentificationHint();
                }
            }
            else if (idtype.Equals("2"))
            {
                if (!CheckArmyIdIsValid(icno))
                {
                    this.mView.ShowFullArmyIdError();
                    this.mView.DisableRegisterButton();
                    return;

                }
                else
                {
                    this.mView.ShowIdentificationHint();
                }
            }
            else
            {
                if (!CheckPassportIsValid(icno))
                {
                    this.mView.ShowFullPassportError();
                    this.mView.DisableRegisterButton();
                    return;
                }
                else
                {
                    this.mView.ShowIdentificationHint();
                }
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
                    var tempReq = JsonConvert.SerializeObject(sendRegistrationTokenSMSRequest);
                    string s = JsonConvert.SerializeObject(sendRegistrationTokenSMSRequest);
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


        public bool CheckIdentificationIsValid(string icno)
        {
            bool isValid = false;
            try
            {
                isValid = hasNumber.IsMatch(icno) && icno.Length == 12;
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
                isValid = (icno.Length > 4 && icno.Length < 51);
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
                isValid = (icno.Length > 4 && icno.Length < 51);
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
