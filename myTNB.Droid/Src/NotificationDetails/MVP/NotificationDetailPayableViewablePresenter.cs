using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using myTNB_Android.Src.NotificationDetails.Models;
using System.Threading;
using System.Net;
using myTNB_Android.Src.Utils;
using System.Net.Http;
using Refit;
using myTNB_Android.Src.myTNBMenu.Api;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.Database.Model;

namespace myTNB_Android.Src.NotificationDetails.MVP
{
    public class NotificationDetailPayableViewablePresenter : NotificationDetailPayableViewableContract.IUserActionsListener
    {

        NotificationDetailPayableViewableContract.IView mView;
        CancellationTokenSource cts;

        public NotificationDetailPayableViewablePresenter(NotificationDetailPayableViewableContract.IView mView)
        {
            this.mView = mView;
            this.mView.SetPresenter(this);
        }

        public async void OnPayment(Models.NotificationDetails notificationDetails)
        {
            cts = new CancellationTokenSource();
            if (mView.IsActive())
            {
                this.mView.ShowRetrievalProgress();
            }
            ServicePointManager.ServerCertificateValidationCallback += SSLFactoryHelper.CertificateValidationCallBack;
#if DEBUG
            var httpClient = new HttpClient(new HttpLoggingHandler(/*new NativeMessageHandler()*/)) { BaseAddress = new Uri(Constants.SERVER_URL.END_POINT) };
            var detailedAccountApi = RestService.For<IDetailedCustomerAccount>(httpClient);
#else
            var detailedAccountApi =  RestService.For<IDetailedCustomerAccount>(Constants.SERVER_URL.END_POINT);
#endif
            try
            {
                var customerBillingDetails = await detailedAccountApi.GetDetailedAccount(new AddAccount.Requests.AccountDetailsRequest()
                {
                    apiKeyID = Constants.APP_CONFIG.API_KEY_ID,
                    CANum = notificationDetails.AccountNum
                }, cts.Token);

                if (mView.IsActive())
                {
                    this.mView.HideRetrievalProgress();
                }

                if (!customerBillingDetails.Data.IsError)
                {
                    CustomerBillingAccount account = CustomerBillingAccount.FindByAccNum(customerBillingDetails.Data.AccountData.AccountNum);
                    if (account != null)
                    {
                        this.mView.ShowPayment(AccountData.Copy(customerBillingDetails.Data.AccountData, account, true));
                    }
                    else
                    {
                        this.mView.ShowPayment(AccountData.Copy(customerBillingDetails.Data.AccountData, true));
                    }
                }
                else
                {
                    this.mView.ShowRetryOptionsApiException(null);
                }
            }
            catch (System.OperationCanceledException e)
            {
                if (mView.IsActive())
                {
                    this.mView.HideRetrievalProgress();
                }
                // ADD OPERATION CANCELLED HERE
                this.mView.ShowRetryOptionsCancelledException(e);
                Utility.LoggingNonFatalError(e);
            }
            catch (ApiException apiException)
            {
                if (mView.IsActive())
                {
                    this.mView.HideRetrievalProgress();
                }
                // ADD HTTP CONNECTION EXCEPTION HERE
                this.mView.ShowRetryOptionsApiException(apiException);
                Utility.LoggingNonFatalError(apiException);
            }
            catch (Exception e)
            {
                if (mView.IsActive())
                {
                    this.mView.HideRetrievalProgress();
                }
                // ADD UNKNOWN EXCEPTION HERE
                this.mView.ShowRetryOptionsUnknownException(e);
                Utility.LoggingNonFatalError(e);
            }
            if (mView.IsActive())
            {
                this.mView.HideRetrievalProgress();
            }
        }

        public async void OnViewDetails(Models.NotificationDetails notificationDetails)
        {
            cts = new CancellationTokenSource();
            this.mView.ShowRetrievalProgress();
            ServicePointManager.ServerCertificateValidationCallback += SSLFactoryHelper.CertificateValidationCallBack;
#if DEBUG
            var httpClient = new HttpClient(new HttpLoggingHandler(/*new NativeMessageHandler()*/)) { BaseAddress = new Uri(Constants.SERVER_URL.END_POINT) };
            var detailedAccountApi = RestService.For<IDetailedCustomerAccount>(httpClient);
#else
            var detailedAccountApi =  RestService.For<IDetailedCustomerAccount>(Constants.SERVER_URL.END_POINT);
#endif
            try
            {
                var customerBillingDetails = await detailedAccountApi.GetDetailedAccount(new AddAccount.Requests.AccountDetailsRequest()
                {
                    apiKeyID = Constants.APP_CONFIG.API_KEY_ID,
                    CANum = notificationDetails.AccountNum
                }, cts.Token);


                if (!customerBillingDetails.Data.IsError)
                {
                    CustomerBillingAccount account = CustomerBillingAccount.FindByAccNum(customerBillingDetails.Data.AccountData.AccountNum);
                    if (account != null)
                    {
                        this.mView.ShowDetails(AccountData.Copy(customerBillingDetails.Data.AccountData, account, true));
                    }
                    else
                    {
                        this.mView.ShowDetails(AccountData.Copy(customerBillingDetails.Data.AccountData, true));
                    }
                    
                }
                else
                {
                    this.mView.ShowRetryOptionsApiException(null);
                }
            }
            catch (System.OperationCanceledException e)
            {
                // ADD OPERATION CANCELLED HERE
                this.mView.ShowRetryOptionsCancelledException(e);
                Utility.LoggingNonFatalError(e);
            }
            catch (ApiException apiException)
            {
                // ADD HTTP CONNECTION EXCEPTION HERE
                this.mView.ShowRetryOptionsApiException(apiException);
                Utility.LoggingNonFatalError(apiException);
            }
            catch (Exception e)
            {
                // ADD UNKNOWN EXCEPTION HERE
                this.mView.ShowRetryOptionsUnknownException(e);
                Utility.LoggingNonFatalError(e);
            }
            this.mView.HideRetrievalProgress();
        }

        public void OnViewPromotion(Models.NotificationDetails notificationDetails)
        {
            this.mView.ShowPromotion(notificationDetails);
        }

        public void Start()
        {
            // NO IMPL
        }
    }
}