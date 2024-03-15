using Android.Util;
using myTNB.AndroidApp.Src.MyTNBService.Request;
using myTNB.AndroidApp.Src.MyTNBService.Response;
using myTNB.AndroidApp.Src.MyTNBService.ServiceImpl;
using myTNB.AndroidApp.Src.Utils;
using System;
using System.Net;

namespace myTNB.AndroidApp.Src.ViewReceipt.MVP
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

        public void GetReceiptDetails(string selectedAccountNumber, string detailedInfoNumber, bool isOwnedAccount, bool showAllReceipt)
        {
            ServicePointManager.ServerCertificateValidationCallback += SSLFactoryHelper.CertificateValidationCallBack;
            //GetReceiptDetailsAsync(apiKeyId, merchantTransId);
            GetPaymentReceipt(selectedAccountNumber, detailedInfoNumber, isOwnedAccount, showAllReceipt);
        }

        public void NavigateToNextScreen()
        {
            throw new NotImplementedException();
        }

        public void Start()
        {
            throw new NotImplementedException();
        }

        public async void GetPaymentReceipt(string selectedAccountNumber, string detailedInfoNumber, bool isOwnedAccount, bool showAllReceipt)
        {
            this.mView.ShowGetReceiptDialog();
            try
            {
                GetPaymentReceiptResponse result = await ServiceApiImpl.Instance.GetPaymentReceipt(new GetPaymentReceiptRequest(selectedAccountNumber, detailedInfoNumber, isOwnedAccount, showAllReceipt),
                    CancellationTokenSourceWrapper.GetTokenWithDelay(Constants.PAYMENT_RECEIPT_TIMEOUT));
                this.mView.HideGetReceiptDialog();
                if (result.IsSuccessResponse())
                {
                    this.mView.OnShowReceiptDetails(result);
                }
                else
                {
                    this.mView.ShowPaymentReceiptError();
                }
            }
            catch (Exception e)
            {
                this.mView.HideGetReceiptDialog();
                Log.Debug(TAG, e.StackTrace);
                Utility.LoggingNonFatalError(e);
                this.mView.ShowPaymentReceiptError();
            }
        }

    }
}
