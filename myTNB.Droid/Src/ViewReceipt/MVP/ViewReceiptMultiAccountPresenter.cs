using Android.Util;
using myTNB_Android.Src.Utils;
using myTNB_Android.Src.ViewReceipt.Api;
using myTNB_Android.Src.ViewReceipt.Model;
using Refit;
using System;
using System.Net;

namespace myTNB_Android.Src.ViewReceipt.MVP
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

        public void GetReceiptDetails(string apiKeyID, string merchantTransId, string contractAccount, string email)
        {
            ServicePointManager.ServerCertificateValidationCallback += SSLFactoryHelper.CertificateValidationCallBack;
            GetReceiptDetailsAsync(apiKeyID, merchantTransId, contractAccount, email);
        }

        public async void GetReceiptDetailsAsync(string apiKeyId, string merchantTransId, string contractAccount, string email)
        {
            if (mView.IsActive())
            {
                this.mView.ShowGetReceiptDialog();
            }
            var api = RestService.For<GetMultiReceiptByTransId>(Constants.SERVER_URL.END_POINT);
            try
            {
                GetMultiReceiptByTransIdResponse result = await api.GetMultiReceiptByTransId(new GetReceiptRequest(apiKeyId, merchantTransId,
                contractAccount, email));
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
                this.mView.ShowErrorMessage("We are facing some issue with server, Please try again latern");
            }

        }
    }
}
