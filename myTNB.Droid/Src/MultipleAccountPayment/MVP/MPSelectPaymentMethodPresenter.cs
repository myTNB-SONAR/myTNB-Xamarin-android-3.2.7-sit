using Android.Util;
using myTNB_Android.Src.MultipleAccountPayment.Api;
using myTNB_Android.Src.MultipleAccountPayment.Model;
using myTNB_Android.Src.MultipleAccountPayment.Requests;
using myTNB_Android.Src.Utils;
using Refit;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;

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
            InitiatePaymentRequestAsync(apiKeyID, custName, custEmail, custPhone, sspUserID, platform, registeredCardId, paymentMode, totalAmount, paymentItems);
        }

        public void Start()
        {
            // NO IMPL
        }

        public async void InitiatePaymentRequestAsync(string apiKeyID, string custName, string custEmail, string custPhone, string sspUserID, string platform, string registeredCardId, string paymentMode, string totalAmount, List<PaymentItems> paymentItems)
        {

            try
            {
                //if (mView.IsActive())
                //{
                this.mView.ShowPaymentRequestDialog();
                //}

                var api = RestService.For<MPRequestPaymentApi>(Constants.SERVER_URL.END_POINT);

                //var httpClient = new HttpClient(new HttpLoggingHandler(/*new NativeMessageHandler()*/)) { BaseAddress = new Uri(Constants.SERVER_URL.END_POINT) };
                ///httpClient.MaxResponseContentBufferSize = 256000;
                //var api = RestService.For<RequestPaymentApi>(httpClient);


                MPInitiatePaymentResponse result = await api.InitiatePayment(new MPInitiatePaymentRequestV3(apiKeyID, custName, custEmail, custPhone, sspUserID, platform, registeredCardId, paymentMode, totalAmount, paymentItems));
                this.mView.SaveInitiatePaymentResponse(result);
                //if (mView.IsActive())
                //{
                this.mView.HidePaymentRequestDialog();
                //}

            }
            catch (System.OperationCanceledException e)
            {
                Log.Debug(TAG, "Cancelled Exception");
                // ADD OPERATION CANCELLED HERE
                //this.mView.ShowRetryOptionsCancelledException(e);
                //if (mView.IsActive())
                //{
                this.mView.HidePaymentRequestDialog();
                //}
                Utility.LoggingNonFatalError(e);
                this.mView.ShowErrorMessage("We are facing some issue with server, Please try again later");
            }
            catch (ApiException apiException)
            {
                // ADD HTTP CONNECTION EXCEPTION HERE
                //this.mView.ShowRetryOptionsApiException(apiException);
                Log.Debug(TAG, "Stack " + apiException.StackTrace);
                //if (mView.IsActive())
                //{
                this.mView.HidePaymentRequestDialog();
                //}
                Utility.LoggingNonFatalError(apiException);
                this.mView.ShowErrorMessage("We are facing some issue with server, Please try again later");
            }
            catch (Exception e)
            {
                // ADD UNKNOWN EXCEPTION HERE
                Log.Debug(TAG, "Stack " + e.StackTrace);
                //this.mView.ShowRetryOptionsUnknownException(e);
                //if (mView.IsActive())
                //{
                this.mView.HidePaymentRequestDialog();
                //}
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
                //if (mView.IsActive())
                //{
                this.mView.ShowGetRegisteredCardDialog();
                //}

#if DEBUG || STUB
                var httpClient = new HttpClient(new HttpLoggingHandler(/*new NativeMessageHandler()*/)) { BaseAddress = new Uri(Constants.SERVER_URL.END_POINT) };
                var api = RestService.For<MPGetRegisteredCardsApi>(httpClient);
#else
            var api = RestService.For<MPGetRegisteredCardsApi>(Constants.SERVER_URL.END_POINT);
#endif
                //var api = RestService.For<GetRegisteredCardsApi>(Constants.SERVER_URL.END_POINT);

                MPGetRegisteredCardsResponse result = await api.GetRegisteredCards(new MPGetRegisteredCardsRequest(apiKeyId, email));
                this.mView.GetRegisterCardsResult(result);
                //if (mView.IsActive())
                //{
                this.mView.HideGetRegisteredCardDialog();
                //}

            }
            catch (Exception e)
            {
                Log.Debug(TAG, e.StackTrace);
                //if (mView.IsActive())
                //{
                this.mView.HideGetRegisteredCardDialog();
                //}
                Utility.LoggingNonFatalError(e);
                this.mView.ShowErrorMessage("Unable to fetch card information");
            }

        }
    }
}