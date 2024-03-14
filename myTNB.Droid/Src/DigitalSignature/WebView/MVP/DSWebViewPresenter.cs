using System;
namespace myTNB.Android.Src.DigitalSignature.WebView.MVP
{
    public class DSWebViewPresenter : DSWebViewContract.IUserActionsListener
    {
        private readonly DSWebViewContract.IView view;

        public DSWebViewPresenter(DSWebViewContract.IView view)
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

        }

        public void Start() { }
    }
}
