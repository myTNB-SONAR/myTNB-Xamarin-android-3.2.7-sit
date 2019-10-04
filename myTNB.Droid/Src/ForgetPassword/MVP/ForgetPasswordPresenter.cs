using Android.Content;
using Android.Text;
using Android.Util;
using myTNB_Android.Src.ForgetPassword.Api;
using myTNB_Android.Src.ForgetPassword.MVP;
using myTNB_Android.Src.ForgetPassword.Requests;
using myTNB_Android.Src.Utils;
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
            cts = new CancellationTokenSource();
            mView.ClearErrorMessages();
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

            if (mView.IsActive())
            {
                this.mView.ShowGetCodeProgressDialog();
            }

            ServicePointManager.ServerCertificateValidationCallback += SSLFactoryHelper.CertificateValidationCallBack;

#if DEBUG
            var httpClient = new HttpClient(new HttpLoggingHandler(/*new NativeMessageHandler()*/)) { BaseAddress = new Uri(Constants.SERVER_URL.END_POINT) };
            var api = RestService.For<IForgetPassword>(httpClient);

#else
             var api = RestService.For<IForgetPassword>(Constants.SERVER_URL.END_POINT);
            
#endif

            try
            {
                var forgetPasswordResponse = await api.SendResetPasswordCode(new ForgetPasswordRequest(apiKeyId,
                   email,
                   email,
                   apiKeyId,
                   apiKeyId,
                   apiKeyId,
                   apiKeyId,
                   apiKeyId,
                   apiKeyId), cts.Token);

                if (mView.IsActive())
                {
                    this.mView.HideGetCodeProgressDialog();
                }

                if (forgetPasswordResponse.response.IsError)
                {
                    string errorMessage = forgetPasswordResponse.response.Message;
                    this.mView.ShowError(errorMessage);
                }
                else
                {

                    string message = forgetPasswordResponse.response.Message;
                    this.mView.ShowSuccess(message);
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

            this.mView.ClearErrorMessages();
            cts = new CancellationTokenSource();
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

            if (TextUtils.IsEmpty(code))
            {
                this.mView.ShowEmptyCodeError();
                return;
            }

            this.mView.DisableSubmitButton();

            if (mView.IsActive())
            {
                this.mView.ShowProgressDialog();
            }
            ServicePointManager.ServerCertificateValidationCallback += SSLFactoryHelper.CertificateValidationCallBack;

#if DEBUG
            var httpClient = new HttpClient(new HttpLoggingHandler(/*new NativeMessageHandler()*/)) { BaseAddress = new Uri(Constants.SERVER_URL.END_POINT) };
            var api = RestService.For<IForgetPassword>(httpClient);

#else
             var api = RestService.For<IForgetPassword>(Constants.SERVER_URL.END_POINT);
            
#endif

            try
            {
                var forgetPasswordResponse = await api.ResetPasswordWithToken(new ForgetPasswordVerificationCodeRequest(apiKeyId,
                   code,
                   email,
                   apiKeyId,
                   apiKeyId,
                   apiKeyId,
                   apiKeyId,
                   apiKeyId,
                   apiKeyId), cts.Token);

                if (mView.IsActive())
                {
                    this.mView.HideProgressDialog();
                }

                if (forgetPasswordResponse.response.IsError)
                {
                    string errorMessage = forgetPasswordResponse.response.Message;
                    this.mView.ShowError(errorMessage);
                }
                else
                {
                    this.mView.ClearErrorMessages();
                    this.mView.ClearTextFields();
                    string message = forgetPasswordResponse.response.Message;
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