using Android.Util;
using myTNB.AndroidApp.Src.Utils;
using myTNB.AndroidApp.Src.ViewReceipt.Api;
using myTNB.AndroidApp.Src.ViewReceipt.Model;
using Refit;
using System;
using System.Net;

namespace myTNB.AndroidApp.Src.ViewReceipt.MVP
{
    public class ViewReceiptMultiAccountPresenter : ViewReceiptMultiAccountContract.IUserActionsListener
    {
        private string TAG = "View Receipt Presenter";
        private ViewReceiptMultiAccountContract.IView mView;

        public ViewReceiptMultiAccountPresenter(ViewReceiptMultiAccountContract.IView view)
        {
            this.mView = view;
            this.mView.SetPresenter(this);
        }

        public void NevigateToNextScreen()
        {

        }

        public void Start()
        {

        }

        public void GetReceiptDetails(string apiKeyID, string merchantTransId)
        {
            ServicePointManager.ServerCertificateValidationCallback += SSLFactoryHelper.CertificateValidationCallBack;
            GetReceiptDetailsAsync(apiKeyID, merchantTransId);
        }

        public async void GetReceiptDetailsAsync(string apiKeyId, string merchantTransId)
        {
            if (mView.IsActive())
            {
                this.mView.ShowGetReceiptDialog();
            }
            var api = RestService.For<GetMultiReceiptByTransId>(Constants.SERVER_URL.END_POINT);
            try
            {
                GetMultiReceiptByTransIdResponse result = await api.GetMultiReceiptByTransId(new GetReceiptRequest(apiKeyId, merchantTransId));
                if (mView.IsActive())
                {
                    this.mView.HideGetReceiptDialog();
                }
                this.mView.OnShowReceiptDetails(result);
            }
            catch (Exception e)
            {
                Log.Debug(TAG, e.StackTrace);
                if (mView.IsActive())
                {
                    this.mView.HideGetReceiptDialog();
                }
                Utility.LoggingNonFatalError(e);
                this.mView.ShowErrorMessage(Utility.GetLocalizedErrorLabel("defaultErrorMessage"));
            }

        }
    }
}