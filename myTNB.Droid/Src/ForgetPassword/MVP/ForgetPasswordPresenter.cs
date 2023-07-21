using Android.Content;
using Android.Text;
using Android.Util;
using myTNB_Android.Src.ForgetPassword.MVP;
using myTNB_Android.Src.ForgetPassword.Requests;
using myTNB_Android.Src.MyTNBService.Request;
using myTNB_Android.Src.MyTNBService.ServiceImpl;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;
using Refit;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;

namespace myTNB_Android.Src.ForgetPassword.Activity
{
    public class ForgetPasswordPresenter : ForgetPasswordContract.IUserActionsListener
    {

        private ForgetPasswordContract.IView mView;
        CancellationTokenSource cts;
        private ISharedPreferences mSharedPref;

        public ForgetPasswordPresenter(ForgetPasswordContract.IView mView, ISharedPreferences sharedPref)
        {
            this.mView = mView;
            this.mSharedPref = sharedPref;
            this.mView.SetPresenter(this);
        }

        public async void GetCode(string apiKeyId, string email)
        {
            if (mView.IsActive())
            {
                this.mView.ShowGetCodeProgressDialog();
            }

            try
            {
                SendResetPasswordCodeRequest resetPasswordCodeRequest = new SendResetPasswordCodeRequest();
                resetPasswordCodeRequest.SetUserName(email);
                var forgetPasswordResponse = await ServiceApiImpl.Instance.ChangeNewPasswordNew(resetPasswordCodeRequest);

                

                if (mView.IsActive())
                {
                    this.mView.HideGetCodeProgressDialog();
                }

                if (!forgetPasswordResponse.IsSuccessResponse())
                {
                    string errorMessage = forgetPasswordResponse.Response.DisplayMessage;
                    this.mView.ShowError(errorMessage);
                }
                else
                {
                    if (forgetPasswordResponse.Response.Data.IsVerified)
                    {
                        string message = forgetPasswordResponse.Response.DisplayMessage;
                        this.mView.ShowSuccess(message);
                    }
                    else
                    {
                        this.mView.ShowEmailResendSuccess();
                    }
                }
            }
            catch (OperationCanceledException e)
            {
                if (mView.IsActive())
                {
                    this.mView.HideGetCodeProgressDialog();
                }
                // CANCLLED
                this.mView.ShowRetryOptionsCodeCancelledException(e);
                Utility.LoggingNonFatalError(e);
            }
            catch (ApiException e)
            {
                if (mView.IsActive())
                {
                    this.mView.HideGetCodeProgressDialog();
                }
                // API EXCEPTION
                this.mView.ShowRetryOptionsCodeApiException(e);
                Utility.LoggingNonFatalError(e);
            }
            catch (Exception e)
            {
                if (mView.IsActive())
                {
                    this.mView.HideGetCodeProgressDialog();
                }
                // UNKNOWN EXCEPTION
                this.mView.ShowRetryOptionsCodeUnknownException(e);
                Utility.LoggingNonFatalError(e);
            }
        }

        public async void ResendEmailVerify(string apiKeyId, string email)
        {
            if (mView.IsActive())
            {
                this.mView.ShowGetCodeProgressDialog();
            }

            try
            {
                SendEmailVerificationRequest resetPasswordCodeRequest = new SendEmailVerificationRequest(email);
                var emailVerificationResponse = await ServiceApiImpl.Instance.SendEmailVerify(new SendEmailVerificationRequest(email));

                if (mView.IsActive())
                {
                    this.mView.HideGetCodeProgressDialog();
                }

                if (emailVerificationResponse.IsSuccessResponse())
                {
                    string message = emailVerificationResponse.Response.Message;
                    this.mView.ShowEmailUpdateSuccess(message, email);
                }
                else
                {
                    string errorMessage = emailVerificationResponse.Response.Message;
                    this.mView.ShowError(errorMessage);
                }
            }
            catch (OperationCanceledException e)
            {
                if (mView.IsActive())
                {
                    this.mView.HideGetCodeProgressDialog();
                }
                // CANCLLED
                this.mView.ShowRetryOptionsCodeCancelledException(e);
                Utility.LoggingNonFatalError(e);
            }
            catch (ApiException e)
            {
                if (mView.IsActive())
                {
                    this.mView.HideGetCodeProgressDialog();
                }
                // API EXCEPTION
                this.mView.ShowRetryOptionsCodeApiException(e);
                Utility.LoggingNonFatalError(e);
            }
            catch (Exception e)
            {
                if (mView.IsActive())
                {
                    this.mView.HideGetCodeProgressDialog();
                }
                // UNKNOWN EXCEPTION
                this.mView.ShowRetryOptionsCodeUnknownException(e);
                Utility.LoggingNonFatalError(e);
            }
        }

        public void ResendCode(string email)
        {
            this.mView.StartProgress();
            GetCode(null,email);
        }

        public void OnComplete()
        {
            this.mView.EnableResendButton();
        }

        public void Start()
        {
            this.mView.DisableResendButton();
            this.mView.StartProgress();
        }



        public async void Submit(string apiKeyId, string email, string username, string code)
        {
            if (mView.IsActive())
            {
                this.mView.ShowProgressDialog();
            }

            try
            {
                ResetPasswordWithTokenRequest resetRequest = new ResetPasswordWithTokenRequest(code);
                resetRequest.SetUserName(email);
                var forgetPasswordResponse = await ServiceApiImpl.Instance.ResetPasswordWithToken(resetRequest);

                if (mView.IsActive())
                {
                    this.mView.HideProgressDialog();
                }

                if (!forgetPasswordResponse.IsSuccessResponse())
                {
                    string errorMessage = forgetPasswordResponse.Response.DisplayMessage;
                    this.mView.ShowError(errorMessage);
                    this.mView.ShowEmptyErrorPin();
                }
                else
                {
                    this.mView.ClearErrorMessages();
                    this.mView.ClearTextFields();
                    string message = forgetPasswordResponse.Response.DisplayMessage;
                    //this.mView.ShowSuccess(message);
                    this.mView.ShowCodeVerifiedSuccess();
                    this.mView.DisableResendButton();
                    // TODO : ADD FLAG THAT AFTER LOGIN USER WILL REDIRECT TO RESET PASSWORD
                    UserSessions.DoFlagResetPassword(mSharedPref);
                }
            }
            catch (OperationCanceledException e)
            {
                if (mView.IsActive())
                {
                    this.mView.HideProgressDialog();
                }
                // CANCLLED
                this.mView.ShowRetryOptionsCancelledException(e);
                Utility.LoggingNonFatalError(e);
            }
            catch (ApiException e)
            {
                if (mView.IsActive())
                {
                    this.mView.HideProgressDialog();
                }
                // API EXCEPTION
                this.mView.ShowRetryOptionsApiException(e);
                Utility.LoggingNonFatalError(e);
            }
            catch (Exception e)
            {
                if (mView.IsActive())
                {
                    this.mView.HideProgressDialog();
                }
                // UNKNOWN EXCEPTION
                this.mView.ShowRetryOptionsUnknownException(e);
                Utility.LoggingNonFatalError(e);
            }


            this.mView.EnableSubmitButton();

        }
    }
}
