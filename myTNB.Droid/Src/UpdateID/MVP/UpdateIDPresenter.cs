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
using myTNB_Android.Src.Base;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace myTNB_Android.Src.UpdateID.MVP
{
    public class UpdateIDPresenter : UpdateIDContract.IUserActionsListener
    {
        private UpdateIDContract.IView mView;
        private readonly string MIN_6_ALPHANUMERIC_PATTERN = "[a-zA-Z0-9#?!@$%^&*-]{6,}";
        private Regex hasNumber = new Regex(@"[0-9]+");
        private Regex hasUpperChar = new Regex(@"[a-zA-Z]+");
        private Regex hasMinimum8Chars = new Regex(@".{8,}");

        CancellationTokenSource registerCts;
        private readonly string TAG = typeof(UpdateIDPresenter).Name;

        public UpdateIDPresenter(UpdateIDContract.IView mView)
        {
            this.mView = mView;
            this.mView.SetPresenter(this);
        }


        public async void OnUpdateIC(string idtype, string no_ic)
        {
            this.mView.ClearErrors();

            if (mView.IsActive())
            {
                this.mView.ShowProgress();
            }

            try
            {
                string oldIC = "";

                try
                {
                    UserEntity userEntity = UserEntity.GetActive();
                    no_ic = no_ic.Replace("-", string.Empty);
                    string email = userEntity.Email;
                    UpdateIdentificationNo userAuthRequest = new UpdateIdentificationNo(idtype, no_ic);
                    string s = JsonConvert.SerializeObject(userAuthRequest);
                    var userResponse = await ServiceApiImpl.Instance.UserAuthenticateUpdateID(userAuthRequest);
                    
                    if (mView.IsActive())
                    {
                        this.mView.HideProgress();
                    }

                    if (userResponse.Response.Data.IsSuccess)
                    {
                        if (UserEntity.IsCurrentlyActive())
                        {
                            UserEntity.UpdateICno(no_ic);
                            this.mView.ShowSuccessUpdateID();
                            //this.mView.HideProgress();
                        }
                    }
                    else
                    {
                        this.mView.ShowErrorMessage(userResponse.Response.DisplayMessage);
                    }
                }
                catch (Exception ex)
                {
                    if (mView.IsActive())
                    {
                        this.mView.HideProgress();
                    }
                    Utility.LoggingNonFatalError(ex);
                }


            }
            catch (System.OperationCanceledException e)
            {
                if (mView.IsActive())
                {
                    this.mView.HideProgress();
                }
                // ADD OPERATION CANCELLED HERE
                this.mView.ShowRetryOptionsCancelledException(e);
                Utility.LoggingNonFatalError(e);
            }
            catch (ApiException apiException)
            {
                if (mView.IsActive())
                {
                    this.mView.HideProgress();
                }
                // ADD HTTP CONNECTION EXCEPTION HERE
                this.mView.ShowRetryOptionsApiException(apiException);
                Utility.LoggingNonFatalError(apiException);
            }
            catch (Exception e)
            {
                if (mView.IsActive())
                {
                    this.mView.HideProgress();
                }
                // ADD UNKNOWN EXCEPTION HERE
                this.mView.ShowRetryOptionsUnknownException(e);
                Utility.LoggingNonFatalError(e);
            }


        }

        public bool validateField(string icno, string idtype)
        {
            try
            {
                bool isCorrect = true;

                this.mView.DisableRegisterButton();

                if (string.IsNullOrEmpty(icno))
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

        public void CheckRequiredFields(string icno, string idtype)
        {
            try
            {
                if (!TextUtils.IsEmpty(icno) && !TextUtils.IsEmpty(idtype))
                {
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
                    else 
                    {
                        this.mView.ShowFullPassportError();
                        this.mView.DisableRegisterButton();
                        return;
                    }
                    //else
                    //{
                    //    this.mView.ClearICMinimumCharactersError();
                    //    //this.mView.ShowIdentificationHint();
                    //}
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

        public async void OnCheckID(string icno, string idtype)
        {

            this.mView.ShowProgressDialog();

            try
            {
                UserCredentialsEntity userEntity = new UserCredentialsEntity();
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

        public void GoBack()
        {
            this.mView.ShowBackScreen();
        }

        public void Start()
        {
            //mView.DisableAddAccountButton();
            Log.Debug(TAG, "Start Called");
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
                isValid = icno.Length > 4 && icno.Length < 51;
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


    }
}
