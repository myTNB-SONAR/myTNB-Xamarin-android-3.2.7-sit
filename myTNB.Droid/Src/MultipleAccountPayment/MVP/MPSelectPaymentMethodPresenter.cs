﻿using Android.Util;
using myTNB.Mobile.API.Managers.Payment;
using myTNB.Mobile.API.Models.ApplicationStatus;
using myTNB.AndroidApp.Src.Base.Activity;
using myTNB.AndroidApp.Src.Base.Models;
using myTNB.AndroidApp.Src.MultipleAccountPayment.Model;
using myTNB.AndroidApp.Src.MyTNBService.Request;
using myTNB.AndroidApp.Src.MyTNBService.Response;
using myTNB.AndroidApp.Src.MyTNBService.ServiceImpl;
using myTNB.AndroidApp.Src.Utils;
using Newtonsoft.Json;
using Refit;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using static myTNB.AndroidApp.Src.MyTNBService.Request.PaymentTransactionIdRequest;

namespace myTNB.AndroidApp.Src.MultipleAccountPayment.MVP
{
    public class MPSelectPaymentMethodPresenter : MPSelectPaymentMethodContract.IUserActionsListener
    {
        private MPSelectPaymentMethodContract.IView mView;
        private BaseToolbarAppCompatActivity mActivity;

        private string TAG = "SelectPaymentMethodPresenter";

        public MPSelectPaymentMethodPresenter(MPSelectPaymentMethodContract.IView mView, BaseToolbarAppCompatActivity activity)
        {
            this.mView = mView;
            this.mActivity = activity;
            this.mView.SetPresenter(this);
        }

        public void RequestPayment(string apiKeyID, string custName, string custEmail, string custPhone, string sspUserID, string platform, string registeredCardId, string paymentMode, string totalAmount, List<PaymentItems> paymentItems)
        {
            ServicePointManager.ServerCertificateValidationCallback += SSLFactoryHelper.CertificateValidationCallBack;
            //InitiatePaymentRequestAsync(apiKeyID, custName, custEmail, custPhone, sspUserID, platform, registeredCardId, paymentMode, totalAmount, paymentItems);
        }

        public void InitializePaymentTransaction(DeviceInterface deviceInf, string custName, string custPhone, string platform, string registeredCardId, string paymentMode, string totalAmount, List<PaymentItem> paymentItems, string applicationType, string applicationRefNo)
        {
            GetPaymentTransactionId(deviceInf, custName, custPhone, platform, registeredCardId, paymentMode, totalAmount, paymentItems, applicationType, applicationRefNo);
        }

        public void InitializeApplicationPaymentTransaction(object userInfo
            , string customerName
            , string phoneNo
            , string osType
            , string registeredCardId
            , string paymentMode
            , string totalAmount
            , string applicationType
            , string searchTerm
            , string system
            , string statusId
            , string statusCode
            , ApplicationPaymentDetail applicationPaymentDetail)
        {
            GetApplicationPayment(userInfo
                , customerName
                , phoneNo
                , osType
                , registeredCardId
                , paymentMode
                , totalAmount
                , applicationType
                , searchTerm
                , system
                , statusId
                , statusCode
                , applicationPaymentDetail);
        }

        public async void GetApplicationPayment(object userInfo
            , string customerName
            , string phoneNo
            , string osType
            , string registeredCardId
            , string paymentMode
            , string totalAmount
            , string applicationType
            , string searchTerm
            , string system
            , string statusId
            , string statusCode
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
                    , applicationType
                    , searchTerm
                    , system
                    , statusId
                    , statusCode
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

        public async void GetPaymentTransactionId(DeviceInterface deviceInf, string custName, string custPhone, string platform, string registeredCardId, string paymentMode, string totalAmount, List<PaymentItem> paymentItems, string applicationType, string applicationRefNo)
        {
            try
            {
                this.mView.ShowPaymentRequestDialog();
                PaymentTransactionIdRequest paymentTransactionIdRequest = new PaymentTransactionIdRequest(deviceInf, custName, custPhone, platform, registeredCardId, paymentMode, totalAmount, paymentItems, applicationType, applicationRefNo);
                Debug.WriteLine("[DEBUG] [GetPaymentTransactionId REQUEST]: " + JsonConvert.SerializeObject(paymentTransactionIdRequest));
                PaymentTransactionIdResponse response = await ServiceApiImpl.Instance.GetPaymentTransactionId(paymentTransactionIdRequest);
                Debug.WriteLine("[DEBUG] [GetPaymentTransactionId RESPONSE]: " + JsonConvert.SerializeObject(response));
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
            this.mView.ShowGetRegisteredCardDialog();
            Task.Run(() =>
            {
                ServicePointManager.ServerCertificateValidationCallback += SSLFactoryHelper.CertificateValidationCallBack;
                _ = GetRegisteredCardsAsync(apiKeyID, email);
            });
        }

        public async Task GetRegisteredCardsAsync(string apiKeyId, string email)
        {
            try
            {
                var result = await ServiceApiImpl.Instance.GetRegisteredCards(new RegisteredCardsRequest(true));
                this.mActivity?.RunOnUiThread(() =>
                {
                    this.mView.GetRegisterCardsResult(result);
                    this.mView.HideGetRegisteredCardDialog();
                });
            }
            catch (Exception e)
            {
                Log.Debug(TAG, e.StackTrace);
                this.mActivity?.RunOnUiThread(() =>
                {
                    this.mView.HideGetRegisteredCardDialog();
                    Utility.LoggingNonFatalError(e);
                    this.mView.ShowErrorMessageWithOK(Utility.GetLocalizedErrorLabel("paymentCCErrorMsg"));
                });
            }
        }
    }
}