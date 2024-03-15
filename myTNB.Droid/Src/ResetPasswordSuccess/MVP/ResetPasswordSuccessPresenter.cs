namespace myTNB.AndroidApp.Src.ResetPasswordSuccess.MVP
{
    public class ResetPasswordSuccessPresenter : ResetPasswordSuccessContract.IUserActionsListener
    {

        private ResetPasswordSuccessContract.IView mView;

        public ResetPasswordSuccessPresenter(ResetPasswordSuccessContract.IView mView)
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