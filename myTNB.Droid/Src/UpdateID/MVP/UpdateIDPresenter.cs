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

namespace myTNB_Android.Src.UpdateID.MVP
{
    public class UpdateIDPresenter : UpdateIDContract.IUserActionsListener
    {
        private UpdateIDContract.IView mView;
        private readonly string MIN_6_ALPHANUMERIC_PATTERN = "[a-zA-Z0-9#?!@$%^&*-]{6,}";
        private Regex hasNumber = new Regex(@"[0-9]+");
        private Regex hasUpperChar = new Regex(@"[a-zA-Z]+");
        private Regex hasMinimum8Chars = new Regex(@".{8,}");
        private Regex hasMinimum12Chars = new Regex(@".{14,}");
        private Regex hasMinimum5until50Chars = new Regex(@".{5,50}");
        private Regex hasHyphens = new Regex(@"/(?([0-9]{3}))?([ .-]?)([0-9]{3})\2([0-9]{4})/");



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
                            this.mView.HideProgress();
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

        public void CheckRequiredFields(string icno, string idtype)
        {
            try
            {
                if (!TextUtils.IsEmpty(icno) && !TextUtils.IsEmpty(idtype))
                {
                    if (!CheckIdentificationIsValid(icno) && idtype.Equals("1"))
                    {
                        MyTNBAccountManagement.GetInstance().SetIsIDUpdated(false);
                        return;
                    }
                    else if (!CheckArmyIdIsValid(icno) && idtype.Equals("2"))
                    {
                        MyTNBAccountManagement.GetInstance().SetIsIDUpdated(false);
                        return;
                    }
                    else if (!CheckPassportIsValid(icno) && idtype.Equals("3"))
                    {
                        MyTNBAccountManagement.GetInstance().SetIsIDUpdated(false);
                        return;
                    }
                    else
                    {
                        //this.mView.ShowIdentificationHint();
                    }
                    MyTNBAccountManagement.GetInstance().SetIsIDUpdated(true);
                }
                else
                {
                    MyTNBAccountManagement.GetInstance().SetIsIDUpdated(false);
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


    }
}
