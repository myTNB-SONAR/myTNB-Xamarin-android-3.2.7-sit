using Android.Util;
using myTNB_Android.Src.MultipleAccountPayment.Model;
using myTNB_Android.Src.MultipleAccountPayment.Requests;
using myTNB_Android.Src.MyTNBService.Billing;
using myTNB_Android.Src.MyTNBService.Request;
using myTNB_Android.Src.MyTNBService.Response;
using myTNB_Android.Src.MyTNBService.ServiceImpl;
using myTNB_Android.Src.Utils;
using Refit;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
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

        public void Start()
        {
            // NO IMPL
        }

        //public async void InitiatePaymentRequestAsync(string apiKeyID, string custName, string custEmail, string custPhone, string sspUserID, string platform, string registeredCardId, string paymentMode, string totalAmount, List<PaymentItems> paymentItems)
        //{

        //    try
        //    {
        //        this.mView.ShowPaymentRequestDialog();

        //        var api = RestService.For<MPRequestPaymentApi>(Constants.SERVER_URL.END_POINT);

        //        MPInitiatePaymentResponse result = await api.InitiatePayment(new MPInitiatePaymentRequestV3(apiKeyID, custName, custEmail, custPhone, sspUserID, platform, registeredCardId, paymentMode, totalAmount, paymentItems));
        //        this.mView.SaveInitiatePaymentResponse(result);
        //        this.mView.HidePaymentRequestDialog();
        //    }
        //    catch (System.OperationCanceledException e)
        //    {
        //        Log.Debug(TAG, "Cancelled Exception");
        //        // ADD OPERATION CANCELLED HERE
        //        this.mView.HidePaymentRequestDialog();
        //        Utility.LoggingNonFatalError(e);
        //        this.mView.ShowErrorMessage("We are facing some issue with server, Please try again later");
        //    }
        //    catch (ApiException apiException)
        //    {
        //        // ADD HTTP CONNECTION EXCEPTION HERE
        //        Log.Debug(TAG, "Stack " + apiException.StackTrace);
        //        this.mView.HidePaymentRequestDialog();
        //        Utility.LoggingNonFatalError(apiException);
        //        this.mView.ShowErrorMessage("We are facing some issue with server, Please try again later");
        //    }
        //    catch (Exception e)
        //    {
        //        // ADD UNKNOWN EXCEPTION HERE
        //        Log.Debug(TAG, "Stack " + e.StackTrace);
        //        this.mView.HidePaymentRequestDialog();
        //        Utility.LoggingNonFatalError(e);
        //    }

        //}

        public async void GetPaymentTransactionId(string custName, string custPhone, string platform, string registeredCardId, string paymentMode, string totalAmount, List<PaymentItem> paymentItems)
        {

            try
            {
                this.mView.ShowPaymentRequestDialog();
                var newApi = new BillingApiImpl();
                PaymentTransactionIdRequest paymentTransactionIdRequest = new PaymentTransactionIdRequest(custName, custPhone, platform, registeredCardId,paymentMode, totalAmount, paymentItems);
                //var api = RestService.For<MPRequestPaymentApi>(Constants.SERVER_URL.END_POINT);

                //MPInitiatePaymentResponse result = await api.InitiatePayment(new MPInitiatePaymentRequestV3(apiKeyID, custName, custEmail, custPhone, sspUserID, platform, registeredCardId, paymentMode, totalAmount, paymentItems));
                //this.mView.SaveInitiatePaymentResponse(result);
                //this.mView.HidePaymentRequestDialog();
                PaymentTransactionIdResponse response = await newApi.GetPaymentTransactionId<PaymentTransactionIdResponse>(paymentTransactionIdRequest);
                this.mView.SetInitiatePaymentResponse(response);
                this.mView.HidePaymentRequestDialog();
            }
            catch (System.OperationCanceledException e)
            {
                Log.Debug(TAG, "Cancelled Exception");
                // ADD OPERATION CANCELLED HERE
                this.mView.HidePaymentRequestDialog();
                Utility.LoggingNonFatalError(e);
                this.mView.ShowErrorMessage("We are facing some issue with server, Please try again later");
            }
            catch (ApiException apiException)
            {
                // ADD HTTP CONNECTION EXCEPTION HERE
                Log.Debug(TAG, "Stack " + apiException.StackTrace);
                this.mView.HidePaymentRequestDialog();
                Utility.LoggingNonFatalError(apiException);
                this.mView.ShowErrorMessage("We are facing some issue with server, Please try again later");
            }
            catch (Exception e)
            {
                // ADD UNKNOWN EXCEPTION HERE
                Log.Debug(TAG, "Stack " + e.StackTrace);
                this.mView.HidePaymentRequestDialog();
                Utility.LoggingNonFatalError(e);
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
                this.mView.ShowErrorMessage("Unable to fetch card information");
            }

        }
    }
}