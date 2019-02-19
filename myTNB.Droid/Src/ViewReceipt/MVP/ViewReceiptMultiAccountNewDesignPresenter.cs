using System;
using System.Net;
using Android.Util;
using myTNB_Android.Src.Utils;
using myTNB_Android.Src.ViewReceipt.Api;
using myTNB_Android.Src.ViewReceipt.Model;
using Refit;

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

        public void GetReceiptDetails(string apiKeyId, string merchantTransId)
        {
            ServicePointManager.ServerCertificateValidationCallback += SSLFactoryHelper.CertificateValidationCallBack;
            GetReceiptDetailsAsync(apiKeyId, merchantTransId);
        }

        public void NevigateToNextScreen()
        {
            throw new NotImplementedException();
        }

        public void Start()
        {
            throw new NotImplementedException();
        }


        public async void GetReceiptDetailsAsync(string apiKeyId, string merchantTransId)
        {
            if (mView.IsActive()) {
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

                this.mView.HideGetReceiptDialog();
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
