using Android.Util;
using myTNB.Mobile.API.Managers.Payment;
using myTNB.Mobile.API.Models.ApplicationStatus;
using myTNB_Android.Src.MultipleAccountPayment.Model;
using myTNB_Android.Src.MyTNBService.Request;
using myTNB_Android.Src.MyTNBService.Response;
using myTNB_Android.Src.MyTNBService.ServiceImpl;
using myTNB_Android.Src.Utils;
using Refit;
using System;
using System.Collections.Generic;
using System.Net;
using static myTNB_Android.Src.MyTNBService.Request.PaymentTransactionIdRequest;

namespace myTNB_Android.Src.MultipleAccountPayment.MVP
{
    public class MPSelectPaymentMethodPresenter : MPSelectPaymentMethodContract.IUserActionsListener
    {
        private MPSelectPaymentMethodContract.IView mView;

        private string TAG = "SelectPaymentMethodPresenter";

        public MPSelectPaymentMethodPresenter(MPSelectPaymentMethodContract.IView mView)
        {
            this.mView = mView;
            this.mView.SetPresenter(this);
        }

        public void RequestPayment(string apiKeyID, string custName, string custEmail, string custPhone, string sspUserID, string platform, string registeredCardId, string paymentMode, string totalAmount, List<PaymentItems> paymentItems)
        {
            ServicePointManager.ServerCertificateValidationCallback += SSLFactoryHelper.CertificateValidationCallBack;
            //InitiatePaymentRequestAsync(apiKeyID, custName, custEmail, custPhone, sspUserID, platform, registeredCardId, paymentMode, totalAmount, paymentItems);
        }

        public void InitializePaymentTransaction(string custName, string custPhone, string platform, string registeredCardId, string paymentMode, string totalAmount, List<PaymentItem> paymentItems)
        {
            GetPaymentTransactionId(custName, custPhone, platform, registeredCardId, paymentMode, totalAmount, paymentItems);
        }
        
        public void InitializeApplicationPaymentTransaction(object userInfo
            , string customerName
            , string phoneNo
            , string osType
            , string registeredCardId
            , string paymentMode
            , string totalAmount
            , ApplicationPaymentDetail applicationPaymentDetail)
        {
            GetApplicationPayment(userInfo
                , customerName
                , phoneNo
                , osType
                , registeredCardId
                , paymentMode
                , totalAmount
                , applicationPaymentDetail);
        }

        public async void GetApplicationPayment(object userInfo
            , string customerName
            , string phoneNo
            , string osType
            , string registeredCardId
            , string paymentMode
            , string totalAmount
            , ApplicationPaymentDetail applicationPaymentDetail)
        {
            try
            {
                this.mView.ShowPaymentRequestDialog();
                PaymentTransactionIdResponse response = await PaymentManager.Instance.ApplicationPayment<PaymentTransactionIdResponse>(userInfo
                    , customerName
                    , phoneNo
                    , osType
                    , registeredCardId
                    , paymentMode
                    , totalAmount
                    , applicationPaymentDetail);
                this.mView.SetInitiatePaymentResponse(response);
                this.mView.HidePaymentRequestDialog();
            }
            catch (OperationCanceledException e)
            {
                this.mView.HidePaymentRequestDialog();
                Utility.LoggingNonFatalError(e);
                this.mView.ShowErrorMessage(Utility.GetLocalizedErrorLabel("defaultErrorMessage"));
            }
            catch (ApiException apiException)
            {
                this.mView.HidePaymentRequestDialog();
                Utility.LoggingNonFatalError(apiException);
                this.mView.ShowErrorMessage(Utility.GetLocalizedErrorLabel("defaultErrorMessage"));
            }
            catch (Exception e)
            {
                this.mView.HidePaymentRequestDialog();
                Utility.LoggingNonFatalError(e);
                this.mView.ShowErrorMessage(Utility.GetLocalizedErrorLabel("defaultErrorMessage"));
            }
        }

        public void Start()
        {
            // NO IMPL
        }

        public async void GetPaymentTransactionId(string custName, string custPhone, string platform, string registeredCardId, string paymentMode, string totalAmount, List<PaymentItem> paymentItems)
        {
            try
            {
                this.mView.ShowPaymentRequestDialog();
                PaymentTransactionIdRequest paymentTransactionIdRequest = new PaymentTransactionIdRequest(custName, custPhone, platform, registeredCardId, paymentMode, totalAmount, paymentItems);
                PaymentTransactionIdResponse response = await ServiceApiImpl.Instance.GetPaymentTransactionId(paymentTransactionIdRequest);
                this.mView.SetInitiatePaymentResponse(response);
                this.mView.HidePaymentRequestDialog();
            }
            catch (System.OperationCanceledException e)
            {
                this.mView.HidePaymentRequestDialog();
                Utility.LoggingNonFatalError(e);
                this.mView.ShowErrorMessage(Utility.GetLocalizedErrorLabel("defaultErrorMessage"));
            }
            catch (ApiException apiException)
            {
                this.mView.HidePaymentRequestDialog();
                Utility.LoggingNonFatalError(apiException);
                this.mView.ShowErrorMessage(Utility.GetLocalizedErrorLabel("defaultErrorMessage"));
            }
            catch (Exception e)
            {
                this.mView.HidePaymentRequestDialog();
                Utility.LoggingNonFatalError(e);
                this.mView.ShowErrorMessage(Utility.GetLocalizedErrorLabel("defaultErrorMessage"));
            }

        }

        public void GetRegisterdCards(string apiKeyID, string email)
        {
            ServicePointManager.ServerCertificateValidationCallback += SSLFactoryHelper.CertificateValidationCallBack;
            GetRegisteredCardsAsync(apiKeyID, email);
        }

        public async void GetRegisteredCardsAsync(string apiKeyId, string email)
        {
            try
            {
                this.mView.ShowGetRegisteredCardDialog();
                var result = await ServiceApiImpl.Instance.GetRegisteredCards(new RegisteredCardsRequest(true));
                this.mView.GetRegisterCardsResult(result);
                this.mView.HideGetRegisteredCardDialog();
            }
            catch (Exception e)
            {
                Log.Debug(TAG, e.StackTrace);
                this.mView.HideGetRegisteredCardDialog();
                Utility.LoggingNonFatalError(e);
                this.mView.ShowErrorMessageWithOK(Utility.GetLocalizedErrorLabel("paymentCCErrorMsg"));
            }

        }
    }
}