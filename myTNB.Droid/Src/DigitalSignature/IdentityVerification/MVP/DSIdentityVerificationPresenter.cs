using System.Threading.Tasks;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.DeviceCache;
using myTNB.Mobile;
using myTNB.Mobile.AWS.Managers.DS;
using myTNB.Mobile.AWS.Models.DS.Status;
using myTNB_Android.Src.Utils;

namespace myTNB_Android.Src.DigitalSignature.IdentityVerification.MVP
{
    public class DSIdentityVerificationPresenter : DSIdentityVerificationContract.IUserActionsListener
    {
        private readonly DSIdentityVerificationContract.IView view;
        private BaseAppCompatActivity mActivity;
        private GetEKYCStatusModel _eKYCstatusModel;
        DSDynamicLinkParamsModel _dynamicLinkParamsModel;

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
            this.view.VerifyMatchingID();
        }

        public void Start() { }

        public void GetEKYCStatusOnCall(DSDynamicLinkParamsModel dynamicLinkParamsModel)
        {
            _dynamicLinkParamsModel = new DSDynamicLinkParamsModel();
            _dynamicLinkParamsModel = dynamicLinkParamsModel;

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
                _eKYCstatusModel = eKYCStatusResponse.Content;
                _dynamicLinkParamsModel.Status = _eKYCstatusModel.Status;

                this.mActivity.RunOnUiThread(() =>
                {
                    this.view?.UpdateLoadingShimmer(false);
                    this.view?.UpdateBottomContainer(true);
                    OnDisplayEKYCStatus(_eKYCstatusModel);
                });
            }
            else
            {
                this.mActivity.RunOnUiThread(() =>
                {
                    this.view?.UpdateLoadingShimmer(false);
                    this.view?.UpdateBottomContainer(false);
                    this.view?.UpdateButtonState(false);
                    if (eKYCStatusResponse != null && eKYCStatusResponse.StatusDetail != null)
                    {
                        this.view?.ShowErrorMessage(eKYCStatusResponse.StatusDetail);
                    }
                });
            }
        }

        private void OnDisplayEKYCStatus(GetEKYCStatusModel eKYCStatusResponse)
        {
            if (eKYCStatusResponse.Status == DigitalSignatureConstants.EKYC_STATUS_PENDING)
            {
                this.view?.ShowCompletedOnOtherDevicePopUp();
            }
            else if (_dynamicLinkParamsModel.IdentificationType == null ||
                !_dynamicLinkParamsModel.IdentificationNo.IsValid())
            {
                this.view?.ShowIdNotRegisteredPopUp();
            }
            else if (eKYCStatusResponse.IsVerified)
            {
                this.view?.ShowIdentityHasBeenVerified();
            }
            else
            {
                this.view?.ShowPrepareDocumentPopUp(_dynamicLinkParamsModel.IdentificationType);
            }
        }

        public DSDynamicLinkParamsModel GetDSDynamicLinkParamsModel()
        {
            return _dynamicLinkParamsModel;
        }
    }
}
