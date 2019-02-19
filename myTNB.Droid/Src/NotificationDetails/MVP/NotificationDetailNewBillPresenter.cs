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
using Refit;
using myTNB_Android.Src.myTNBMenu.Api;
using myTNB_Android.Src.Utils;
using System.Net.Http;
using System.Net;
using myTNB_Android.Src.myTNBMenu.Models;
using System.Threading;

namespace myTNB_Android.Src.NotificationDetails.MVP
{
    public class NotificationDetailNewBillPresenter : NotificationDetailNewBillContract.IUserActionsListener
    {
        private NotificationDetailNewBillContract.IView mView;
        CancellationTokenSource cts;

        public NotificationDetailNewBillPresenter(NotificationDetailNewBillContract.IView mVIew)
        {
            this.mView = mVIew;
            this.mView.SetPresenter(this);
        }



        public async void OnPayment(Models.NotificationDetails notificationDetails)
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
                    this.mView.ShowPayment(AccountData.Copy(customerBillingDetails.Data.AccountData, true));
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
            }
            catch (ApiException apiException)
            {
                // ADD HTTP CONNECTION EXCEPTION HERE
                this.mView.ShowRetryOptionsApiException(apiException);
            }
            catch (Exception e)
            {
                // ADD UNKNOWN EXCEPTION HERE
                this.mView.ShowRetryOptionsUnknownException(e);
            }
            this.mView.HideRetrievalProgress();

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
                    this.mView.ShowDetails(AccountData.Copy(customerBillingDetails.Data.AccountData, true));
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
            }
            catch (ApiException apiException)
            {
                // ADD HTTP CONNECTION EXCEPTION HERE
                this.mView.ShowRetryOptionsApiException(apiException);
            }
            catch (Exception e)
            {
                // ADD UNKNOWN EXCEPTION HERE
                this.mView.ShowRetryOptionsUnknownException(e);
            }
            this.mView.HideRetrievalProgress();
        }

        public void Start()
        {
            // NO IMPL
            // THE account number is already called by parent Presenter which is NotificationDetailPresenter
            // TODO: Proceed with calling 
            // TODO: 1. `Month` wildcard 
            // TODO: 2. `Bill Dated` wildcard
            // TODO: 3. `Total Outstanding Amt` wildcard
            // TODO: 4. `Payment Due Wildcard`
            this.mView.ShowMonthWildCard();
            this.mView.ShowBillDatedWildcard();
            this.mView.ShowTotalOutstandingAmtWildcard();
            this.mView.ShowPaymentDueWildcard();

        }
    }
}