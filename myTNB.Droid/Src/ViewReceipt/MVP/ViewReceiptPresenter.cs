﻿using Android.Util;
using myTNB_Android.Src.Utils;
using myTNB_Android.Src.ViewReceipt.Api;
using myTNB_Android.Src.ViewReceipt.Model;
using Refit;
using System;
using System.Net;

namespace myTNB_Android.Src.ViewReceipt.MVP
{
    public class ViewReceiptPresenter : ViewReceiptContract.IUserActionsListener
    {
        private string TAG = "View Receipt Presenter";
        private ViewReceiptContract.IView mView;

        public ViewReceiptPresenter(ViewReceiptContract.IView view)
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
            var api = RestService.For<GetReceiptApi>(Constants.SERVER_URL.END_POINT);
            try
            {
                GetReceiptResponse result = await api.GetReceipt(new GetReceiptRequest(apiKeyId, merchantTransId));
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