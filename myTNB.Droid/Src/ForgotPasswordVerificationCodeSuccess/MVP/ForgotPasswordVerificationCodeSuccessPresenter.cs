namespace myTNB.Android.Src.ForgotPasswordVerificationCodeSuccess.MVP
{
    public class ForgotPasswordVerificationCodeSuccessPresenter : ForgotPasswordVerificationCodeSuccessContract.IUserActionsListener
    {

        private ForgotPasswordVerificationCodeSuccessContract.IView mView;

        public ForgotPasswordVerificationCodeSuccessPresenter(ForgotPasswordVerificationCodeSuccessContract.IView mView)
        {
            this.mView = mView;
            this.mView.SetPresenter(this);
        }

        public void OnBackKeyPress()
        {
            // TODO LOGIC TO INTERCEPT BACK KEY PRESS
            this.mView.ShowBackActivity();
        }

        public void OnClose()
        {
            this.mView.ShowBackActivity();
        }

        public void OnLogin()
        {
            this.mView.ShowLoginActivity();
        }

        public void OnSupportNavigationKeyPress()
        {
            // TODO LOGIC TO ALLOW USER TO NAVIGATE BACK 
            this.mView.ShowBackActivity();
        }

        public void Start()
        {
            // NO IMPL
        }
    }
}