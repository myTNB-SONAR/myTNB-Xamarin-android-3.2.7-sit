using System.Threading.Tasks;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.DeviceCache;
using myTNB.Mobile;
using myTNB.Mobile.AWS.Models.DS.Identification;
using myTNB.Mobile.AWS.Managers.DS;
using myTNB_Android.Src.Utils;

namespace myTNB_Android.Src.DigitalSignature.IdentityVerification.MVP
{
    public class DSIdentityVerificationPresenter : DSIdentityVerificationContract.IUserActionsListener
    {
        private readonly DSIdentityVerificationContract.IView view;
        private BaseAppCompatActivity mActivity;
        GetEKYCIdentificationModel _identificationModel;

        public DSIdentityVerificationPresenter(DSIdentityVerificationContract.IView view, BaseAppCompatActivity activity)
        {
            this.view = view;
            this.mActivity = activity;
            this.view?.SetPresenter(this);
        }

        public void OnInitialize()
        {
            this.view?.SetUpViews();
            OnStart();
        }

        public void OnStart()
        {
            this.view.RenderContent();
        }

        public void Start() { }

        public void GetEKYCIdentificationOnCall()
        {
            Task.Run(() =>
            {
                _ = GetEKYCIdentification();
            });
        }

        private async Task GetEKYCIdentification()
        {
            string userId = UserEntity.GetActive().UserID ?? string.Empty;

            if (!AccessTokenCache.Instance.HasTokenSaved(this.mActivity))
            {
                string accessToken = await AccessTokenManager.Instance.GenerateAccessToken(userId);
                AccessTokenCache.Instance.SaveAccessToken(this.mActivity, accessToken);
            }

            GetEKYCIdentificationResponse eKYCIdentificationResponse = await DSManager.Instance.GetEKYCIdentification(userId, AccessTokenCache.Instance.GetAccessToken(this.mActivity));
            if (eKYCIdentificationResponse != null &&
                eKYCIdentificationResponse.StatusDetail != null &&
                eKYCIdentificationResponse.StatusDetail.IsSuccess &&
                eKYCIdentificationResponse.Content != null)
            {
                _identificationModel = eKYCIdentificationResponse.Content;
                OnDisplayPopUp(eKYCIdentificationResponse.Content);
            }
            else
            {
                this.mActivity.RunOnUiThread(() =>
                {
                    this.view?.HideProgressDialog();
                });
            }
        }

        private void OnDisplayPopUp(GetEKYCIdentificationModel eKYCIdentificationModel)
        {
            if (eKYCIdentificationModel.IsCompletedOnOtherDevice)
            {
                this.view?.ShowCompletedOnOtherDevicePopUp();
            }
            else if (eKYCIdentificationModel.IdentificationType == null ||
                !eKYCIdentificationModel.IdentificationNo.IsValid())
            {
                this.view?.ShowIdNotRegisteredPopUp();
            }
            else if (eKYCIdentificationModel.IsVerified)
            {
                this.view?.ShowIdentityHasBeenVerified();
            }
            else
            {
                this.view?.ShowPrepareDocumentPopUp(eKYCIdentificationModel.IdentificationType);
            }
        }

        public GetEKYCIdentificationModel GetIdentificationModel()
        {
            return _identificationModel;
        }
    }
}
