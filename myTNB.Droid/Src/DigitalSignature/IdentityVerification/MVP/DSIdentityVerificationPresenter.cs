using System;
namespace myTNB_Android.Src.DigitalSignature.IdentityVerification.MVP
{
    public class DSIdentityVerificationPresenter : DSIdentityVerificationContract.IUserActionsListener
    {
        private readonly DSIdentityVerificationContract.IView view;

        public DSIdentityVerificationPresenter(DSIdentityVerificationContract.IView view)
        {
            this.view = view;
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
    }
}
