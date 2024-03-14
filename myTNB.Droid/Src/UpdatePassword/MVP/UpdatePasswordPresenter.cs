using Android.Text;
using myTNB.Android.Src.Database.Model;
using myTNB.Android.Src.MyTNBService.Request;
using myTNB.Android.Src.MyTNBService.ServiceImpl;
using myTNB.Android.Src.Utils;
using Refit;
using System;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;

namespace myTNB.Android.Src.UpdatePassword.MVP
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
            if (mView.IsActive())
            {
                this.mView.ShowProgress();
            }

            try
            {
                var updatePasswordResponse = await ServiceApiImpl.Instance.ChangeNewPassword(new ChangeNewPasswordRequest(currentPassword, newPassword, confirmNewPassword));

                if (mView.IsActive())
                {
                    this.mView.HideProgress();
                }

                if (updatePasswordResponse.IsSuccessResponse())
                {
                    this.mView.ShowSuccess();
                }
                else
                {
                    this.mView.ShowErrorMessage(updatePasswordResponse.Response.DisplayMessage);
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