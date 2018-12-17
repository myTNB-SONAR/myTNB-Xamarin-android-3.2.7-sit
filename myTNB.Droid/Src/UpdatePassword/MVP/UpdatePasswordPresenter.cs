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
using Refit;
using System.Net.Http;
using myTNB_Android.Src.Utils;
using myTNB_Android.Src.UpdatePassword.Api;
using System.Net;
using myTNB_Android.Src.Database.Model;
using System.Threading;
using System.Text.RegularExpressions;

namespace myTNB_Android.Src.UpdatePassword.MVP
{
    public class UpdatePasswordPresenter : UpdatePasswordContract.IUserActionsListener
    {

        private UpdatePasswordContract.IView mView;
        //private readonly string MIN_6_ALPHANUMERIC_PATTERN = "[a-zA-Z0-9#?!@$%^&*-]{6,}";
        private Regex hasNumber = new Regex(@"[0-9]+");
        private Regex hasUpperChar = new Regex(@"[a-zA-Z]+");
        private Regex hasMinimum8Chars = new Regex(@".{8,}");

        CancellationTokenSource cts;
        public UpdatePasswordPresenter(UpdatePasswordContract.IView mView)
        {
            this.mView = mView;
            this.mView.SetPresenter(this);
        }

        public async void OnSave(string currentPassword, string newPassword, string confirmNewPassword)
        {
            cts = new CancellationTokenSource();
            this.mView.ClearErrors();
            if (TextUtils.IsEmpty(currentPassword))
            {
                this.mView.ShowEmptyCurrentPassword();
                return;
            }

            if (TextUtils.IsEmpty(newPassword))
            {
                this.mView.ShowEmptyNewPassword();
                return;
            }

            if (!CheckPasswordIsValid(newPassword))
            {
                this.mView.ShowInvalidNewPassword();
                return;
            }

            if (TextUtils.IsEmpty(confirmNewPassword))
            {
                this.mView.ShowEmptyConfirmPassword();
                return;
            }

            if (!newPassword.Equals(confirmNewPassword))
            {
                this.mView.ShowNewPasswordNotEqualToConfirmPassword();
                return;
            }
            ServicePointManager.ServerCertificateValidationCallback += SSLFactoryHelper.CertificateValidationCallBack;
            UserEntity userEntity = UserEntity.GetActive();
            if (mView.IsActive()) {
            this.mView.ShowProgress();
            }
#if DEBUG || STUB
            var httpClient = new HttpClient(new HttpLoggingHandler(/*new NativeMessageHandler()*/)) { BaseAddress = new Uri(Constants.SERVER_URL.END_POINT) };
            var updatePasswordApi = RestService.For<IUpdatePasswordApi>(httpClient);
#else
            var updatePasswordApi = RestService.For<IUpdatePasswordApi>(Constants.SERVER_URL.END_POINT);
#endif

            try
            {
                var updatePasswordResponse = await updatePasswordApi.ChangeNewPassword(new Request.UpdatePasswordRequest()
                {
                    Username = userEntity.UserName,
                    CurrentPassword = currentPassword,
                    NewPassword = newPassword,
                    ConfirmNewPassword = confirmNewPassword,
                    ApiKeyId = Constants.APP_CONFIG.API_KEY_ID,
                    IpAddress = Constants.APP_CONFIG.API_KEY_ID,
                    ClientType = Constants.APP_CONFIG.API_KEY_ID,
                    ActiveUserName = Constants.APP_CONFIG.API_KEY_ID,
                    DevicePlatform = Constants.APP_CONFIG.API_KEY_ID,
                    DeviceVersion = Constants.APP_CONFIG.API_KEY_ID,
                    DeviceCordova = Constants.APP_CONFIG.API_KEY_ID
                } , cts.Token);

                if (mView.IsActive())
                {
                    this.mView.HideProgress();
                }

                if (!updatePasswordResponse.Data.IsError)
                {
                    this.mView.ShowSuccess();
                }
                else
                {
                    this.mView.ShowErrorMessage(updatePasswordResponse.Data.Message);
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

        public void Start()
        {
            //
        }

        public bool CheckPasswordIsValid(string password)
        {
            bool isValid = false;
            isValid = hasNumber.IsMatch(password) && hasUpperChar.IsMatch(password) && hasMinimum8Chars.IsMatch(password);
            return isValid;
        }
    }
}