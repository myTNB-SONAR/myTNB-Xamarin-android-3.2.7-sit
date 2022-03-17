using System;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.MyTNBService.Request;
using myTNB_Android.Src.MyTNBService.ServiceImpl;
using myTNB_Android.Src.Utils;
using Refit;
using System.Threading.Tasks;
using myTNB_Android.Src.DeviceCache;
using myTNB_Android.Src.Base.Activity;
using myTNB.Mobile;
using myTNB.Mobile.AWS.Models.DS.Status;
using myTNB.Mobile.AWS.Managers.DS;

namespace myTNB_Android.Src.MyProfileDetail.MVP
{
    public class ProfileDetailPresenter : ProfileDetailContract.IUserActionsListener
    {
        private ProfileDetailContract.IView mView;
        private readonly string TAG = typeof(ProfileDetailPresenter).Name;
        private BaseAppCompatActivity mActivity;

        public ProfileDetailPresenter(ProfileDetailContract.IView mView, BaseAppCompatActivity activity)
        {
            this.mView = mView;
            this.mActivity = activity;
            this.mView.SetPresenter(this);
        }

        public void Start()
        {
            throw new NotImplementedException();
        }

        public async void ResendEmailVerify(string apiKeyId, string email)
        {
            if (mView.IsActive())
            {
                this.mView.ShowGetCodeProgressDialog();
            }

            try
            {

                var emailVerificationResponse = await ServiceApiImpl.Instance.SendEmailVerify(new SendEmailVerificationRequest(email));



                if (mView.IsActive())
                {
                    this.mView.HideGetCodeProgressDialog();
                }

                if (emailVerificationResponse.IsSuccessResponse())
                {
                    string message = emailVerificationResponse.Response.Message;
                    this.mView.ShowEmailUpdateSuccess(message);
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

        public void GetEKYCStatusOnCall()
        {
            Task.Run(() =>
            {
                _ = GetEKYCStatus();
            });
        }

        private async Task GetEKYCStatus()
        {
            string userId = UserEntity.GetActive().UserID ?? string.Empty;

            if (!AccessTokenCache.Instance.HasTokenSaved(this.mActivity))
            {
                string accessToken = await AccessTokenManager.Instance.GenerateAccessToken(userId);
                AccessTokenCache.Instance.SaveAccessToken(this.mActivity, accessToken);
            }

            GetEKYCStatusResponse eKYCStatusResponse = await DSManager.Instance.GetEKYCStatus(userId, AccessTokenCache.Instance.GetAccessToken(this.mActivity));
            if (eKYCStatusResponse != null &&
                eKYCStatusResponse.StatusDetail != null &&
                eKYCStatusResponse.StatusDetail.IsSuccess &&
                eKYCStatusResponse.Content != null)
            {
                this.mView.ShowAccountVerified(eKYCStatusResponse.Content.IsVerified);
            }
            else
            {
                this.mView.ShowAccountVerified(false);
            }
        }
    }
}
