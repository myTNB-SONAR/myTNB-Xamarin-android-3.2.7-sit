using Android.Util;
using myTNB_Android.Src.Utils;
using myTNB_Android.Src.ViewReceipt.Api;
using myTNB_Android.Src.ViewReceipt.Model;
using Refit;
using System;
using System.Net;

namespace myTNB_Android.Src.ViewReceipt.MVP
{
    public class ViewReceiptMultiAccountNewDesignPresenter : ViewReceiptMultiAccountNewDesignContract.IUserActionsListener
    {
        private string TAG = "ViewBillMultiAccountNewDesignPresenter";
        private ViewReceiptMultiAccountNewDesignContract.IView mView;

        public ViewReceiptMultiAccountNewDesignPresenter(ViewReceiptMultiAccountNewDesignContract.IView mView)
        {
            this.mView = mView;
            this.mView.SetPresenter(this);
        }

        public void GetReceiptDetails(string apiKeyId, string merchantTransId, string contractAccount, string email)
        {
            ServicePointManager.ServerCertificateValidationCallback += SSLFactoryHelper.CertificateValidationCallBack;
            GetReceiptDetailsAsync(apiKeyId, merchantTransId, contractAccount, email);
        }

        public void NevigateToNextScreen()
        {
            throw new NotImplementedException();
        }

        public void Start()
        {
            throw new NotImplementedException();
        }


        public async void GetReceiptDetailsAsync(string apiKeyId, string merchantTransId, string contractAccount, string email)
        {
            this.mView.ShowGetReceiptDialog();
            var api = RestService.For<GetMultiReceiptByTransId>(Constants.SERVER_URL.END_POINT);
            try
            {
                GetMultiReceiptByTransIdResponse result = await api.GetMultiReceiptByTransId(new GetReceiptRequest(apiKeyId, merchantTransId,
                    contractAccount, email));
                this.mView.HideGetReceiptDialog();
                this.mView.OnShowReceiptDetails(result);
            }
            catch (Exception e)
            {
                this.mView.HideGetReceiptDialog();
                Log.Debug(TAG, e.StackTrace);
                Utility.LoggingNonFatalError(e);
                this.mView.ShowErrorMessage("We are facing some issue with server, Please try again later.");
            }
        }

    }
}
