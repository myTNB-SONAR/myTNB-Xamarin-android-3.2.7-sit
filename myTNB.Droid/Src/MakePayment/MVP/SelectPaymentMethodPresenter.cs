using Android.Util;
using myTNB_Android.Src.AddAccount.Requests;
using myTNB_Android.Src.MakePayment.Api;
using myTNB_Android.Src.MakePayment.Models;
using myTNB_Android.Src.MakePayment.Requests;
using myTNB_Android.Src.MyTNBService.Request;
using myTNB_Android.Src.MyTNBService.ServiceImpl;
using myTNB_Android.Src.Utils;
using Refit;
using System;
using System.Net;
using System.Net.Http;

namespace myTNB_Android.Src.MakePayment.MVP
{
    public class SelectPaymentMethodPresenter : SelectPaymentMethodContract.IUserActionsListener
    {
        private SelectPaymentMethodContract.IView mView;

        private string TAG = "SelectPaymentMethodPresenter";

        public SelectPaymentMethodPresenter(SelectPaymentMethodContract.IView mView)
        {
            this.mView = mView;
            this.mView.SetPresenter(this);
        }

        public void RequestPayment(string apiKeyID, string custName, string accNum, string payAm, string custEmail, string custPhone, string sspUserID, string platform, string paymentMode, string registeredCardId)
        {
            ServicePointManager.ServerCertificateValidationCallback += SSLFactoryHelper.CertificateValidationCallBack;
            InitiatePaymentRequestAsync(apiKeyID, custName, accNum, payAm, custEmail, custPhone, sspUserID, platform, paymentMode, registeredCardId);
        }

        public void Start()
        {
            // NO IMPL
        }

        public async void InitiatePaymentRequestAsync(string apiKeyID, string custName, string accNum, string payAm, string custEmail, string custPhone, string sspUserID, string platform, string paymentMode, string registeredCardId)
        {
            if (mView.IsActive())
            {
                this.mView.ShowPaymentRequestDialog();
            }

            var api = RestService.For<RequestPaymentApi>(Constants.SERVER_URL.END_POINT);

            //var httpClient = new HttpClient(new HttpLoggingHandler(/*new NativeMessageHandler()*/)) { BaseAddress = new Uri(Constants.SERVER_URL.END_POINT) };
            ///httpClient.MaxResponseContentBufferSize = 256000;
            //var api = RestService.For<RequestPaymentApi>(httpClient);

            try
            {
                InitiatePaymentResponse result = await api.InitiatePayment(new InitiatePaymentRequestV3(apiKeyID, custName, accNum, payAm, custEmail, custPhone, sspUserID, platform, paymentMode, registeredCardId));
                if (mView.IsActive())
                {
                    this.mView.HidePaymentRequestDialog();
                }
                this.mView.SaveInitiatePaymentResponse(result);
            }
            catch (System.OperationCanceledException e)
            {
                if (mView.IsActive())
                {
                    this.mView.HidePaymentRequestDialog();
                }
                this.mView.ShowErrorMessage(Utility.GetLocalizedErrorLabel("defaultErrorMessage"));
                Utility.LoggingNonFatalError(e);
            }
            catch (ApiException apiException)
            {
                if (mView.IsActive())
                {
                    this.mView.HidePaymentRequestDialog();
                }
                this.mView.ShowErrorMessage(Utility.GetLocalizedErrorLabel("defaultErrorMessage"));
                Utility.LoggingNonFatalError(apiException);
            }
            catch (Exception e)
            {
                if (mView.IsActive())
                {
                    this.mView.HidePaymentRequestDialog();
                }
                Utility.LoggingNonFatalError(e);
                this.mView.ShowErrorMessage(Utility.GetLocalizedErrorLabel("defaultErrorMessage"));
            }

        }

        public void SubmitPayment(string apiKeyID, string merchantId, string accNum, string payAm, string custName, string custEmail, string custPhone, string mParam1, string des, string cardNo, string cardName, string expM, string expY, string cvv)
        {
            ServicePointManager.ServerCertificateValidationCallback += SSLFactoryHelper.CertificateValidationCallBack;
            InitiateSubmitPaymentAsync(apiKeyID, merchantId, accNum, payAm, custName, custEmail, custPhone, mParam1, des, cardNo, cardName, expM, expY, cvv);
        }

        public async void InitiateSubmitPaymentAsync(string apiKeyID, string merchantId, string accNum, string payAm, string custName, string custEmail, string custPhone, string mParam1, string des, string cardNo, string cardName, string expM, string expY, string cvv)
        {
            try
            {
                if (mView.IsActive())
                {
                    this.mView.ShowPaymentRequestDialog();
                }
                var api = RestService.For<SubmitPaymentApi>(Constants.SERVER_URL.END_POINT);
                // TODO : UPDATE TO V5
                var result = await api.SubmitPayment(new SubmitPaymentRequestPG(apiKeyID, merchantId, accNum, payAm, custName, custEmail, custPhone, mParam1, des, cardNo, cardName, expM, expY, cvv));
                Log.Debug(TAG, "Submit Payment : " + result.ToString());
                if (mView.IsActive())
                {
                    this.mView.HidePaymentRequestDialog();
                }
                this.mView.InitiateWebView(result.ToString());
            }
            catch (Exception e)
            {
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
            if (mView.IsActive())
            {
                this.mView.ShowGetRegisteredCardDialog();
            }
            try
            {
                var result = await ServiceApiImpl.Instance.GetRegisteredCards(new RegisteredCardsRequest(true));
                if (mView.IsActive())
                {
                    this.mView.HideGetRegisteredCardDialog();
                }
                this.mView.GetRegisterCardsResult(result);

            }
            catch (Exception e)
            {
                Log.Debug(TAG, e.StackTrace);
                if (mView.IsActive())
                {
                    this.mView.HideGetRegisteredCardDialog();
                }
                this.mView.ShowErrorMessage(Utility.GetLocalizedErrorLabel("defaultErrorMessage"));
                Utility.LoggingNonFatalError(e);
            }

        }
    }
}