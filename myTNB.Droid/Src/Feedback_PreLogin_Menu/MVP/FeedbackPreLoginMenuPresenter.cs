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
using myTNB_Android.Src.Database.Model;
using System.Threading;
using Refit;
using System.Net.Http;
using myTNB_Android.Src.Utils;
using myTNB_Android.Src.Base.Api;
using System.Net;
using myTNB_Android.Src.Base.Models;

namespace myTNB_Android.Src.Feedback_PreLogin_Menu.MVP
{
    public class FeedbackPreLoginMenuPresenter : FeedbackPreLoginMenuContract.IUserActionsListener
    {

        FeedbackPreLoginMenuContract.IView mView;
        CancellationTokenSource cts;
        string deviceId;

        public FeedbackPreLoginMenuPresenter(FeedbackPreLoginMenuContract.IView mView , string deviceId)
        {
            this.mView = mView;
            this.mView.SetPresenter(this);
            this.deviceId = deviceId;
        }

        public void OnBillingPayment()
        {
            this.mView.ShowBillingPayment();
        }

        public void OnFaultyStreetLamps()
        {
            this.mView.ShowFaultyStreetLamps();
        }

        public void OnOthers()
        {
            this.mView.ShowOthers();
        }

        public void OnResume()
        {
            int count = SubmittedFeedbackEntity.Count();
            this.mView.ShowSubmittedFeedbackCount(count);
        }

        public async void OnRetry()
        {

            cts = new CancellationTokenSource();

            if (mView.IsActive()) {
            this.mView.ShowProgressDialog();
            }
#if DEBUG
            var httpClient = new HttpClient(new HttpLoggingHandler(/*new NativeMessageHandler()*/)) { BaseAddress = new Uri(Constants.SERVER_URL.END_POINT) };
            var feedbackApi = RestService.For<IFeedbackApi>(httpClient);
#else

            var feedbackApi = RestService.For<IFeedbackApi>(Constants.SERVER_URL.END_POINT);
#endif
            ServicePointManager.ServerCertificateValidationCallback += SSLFactoryHelper.CertificateValidationCallBack;
            try
            {
                var submittedFeedbackResponse = await feedbackApi.GetSubmittedFeedbackList(new Base.Request.SubmittedFeedbackRequest()
                {
                    ApiKeyId = Constants.APP_CONFIG.API_KEY_ID,
                    Email = "",
                    DeviceId = deviceId

                }, cts.Token);

                if (mView.IsActive())
                {
                    this.mView.HideProgressDialog();
                }

                if (!submittedFeedbackResponse.Data.IsError)
                {
                    SubmittedFeedbackEntity.Remove();
                    foreach (SubmittedFeedback sf in submittedFeedbackResponse.Data.Data)
                    {
                        SubmittedFeedbackEntity.InsertOrReplace(sf);

                    }

                    int count = SubmittedFeedbackEntity.Count();
                    this.mView.ShowSubmittedFeedbackCount(count);

                }
                else
                {
                    this.mView.ShowRetryOptionsCancelledException(null);
                }

            }
            catch (System.OperationCanceledException e)
            {
                if (mView.IsActive())
                {
                    this.mView.HideProgressDialog();
                }
                // ADD OPERATION CANCELLED HERE
                this.mView.ShowRetryOptionsCancelledException(e);
                Utility.LoggingNonFatalError(e);
            }
            catch (ApiException apiException)
            {
                if (mView.IsActive())
                {
                    this.mView.HideProgressDialog();
                }
                // ADD HTTP CONNECTION EXCEPTION HERE
                this.mView.ShowRetryOptionsApiException(apiException);
                Utility.LoggingNonFatalError(apiException);
            }
            catch (Exception e)
            {
                if (mView.IsActive())
                {
                    this.mView.HideProgressDialog();
                }
                // ADD UNKNOWN EXCEPTION HERE
                this.mView.ShowRetryOptionsUnknownException(e);
                Utility.LoggingNonFatalError(e);
            }

        }

        public void OnSubmittedFeedback()
        {
            this.mView.ShowSubmittedFeedback();
        }

        public void Start()
        {
            try {
            if (FeedbackCategoryEntity.HasRecords())
            {
                List<FeedbackCategoryEntity> feedbackCategoryList = FeedbackCategoryEntity.GetActiveList();
                this.mView.ShowFeedbackMenu(feedbackCategoryList);
            }
        } catch (Exception e) {
                Utility.LoggingNonFatalError(e);
            }

            //            cts = new CancellationTokenSource();
            //            this.mView.ShowProgressDialog();
            //#if DEBUG
            //            var httpClient = new HttpClient(new HttpLoggingHandler(/*new NativeMessageHandler()*/)) { BaseAddress = new Uri(Constants.SERVER_URL.END_POINT) };
            //            var feedbackApi = RestService.For<IFeedbackApi>(httpClient);
            //#else

            //            var feedbackApi = RestService.For<IFeedbackApi>(Constants.SERVER_URL.END_POINT);
            //#endif
            //            ServicePointManager.ServerCertificateValidationCallback += SSLFactoryHelper.CertificateValidationCallBack;
            //            try
            //            {
            //                var submittedFeedbackResponse = await feedbackApi.GetSubmittedFeedbackList(new Base.Request.SubmittedFeedbackRequest()
            //                {
            //                    ApiKeyId = Constants.APP_CONFIG.API_KEY_ID,
            //                    Email = "",
            //                    DeviceId = deviceId

            //                }, cts.Token);
            //                if (!submittedFeedbackResponse.Data.IsError)
            //                {
            //                    SubmittedFeedbackEntity.Remove();
            //                    foreach (SubmittedFeedback sf in submittedFeedbackResponse.Data.Data)
            //                    {
            //                        SubmittedFeedbackEntity.InsertOrReplace(sf);

            //                    }

            //                    int count = SubmittedFeedbackEntity.Count();
            //                    this.mView.ShowSubmittedFeedbackCount(count);
            //                }
            //                else
            //                {
            //                    this.mView.ShowRetryOptionsCancelledException(null);
            //                }

            //            }
            //            catch (System.OperationCanceledException e)
            //            {
            //                // ADD OPERATION CANCELLED HERE
            //                this.mView.ShowRetryOptionsCancelledException(e);
            //            }
            //            catch (ApiException apiException)
            //            {
            //                // ADD HTTP CONNECTION EXCEPTION HERE
            //                this.mView.ShowRetryOptionsApiException(apiException);
            //            }
            //            catch (Exception e)
            //            {
            //                // ADD UNKNOWN EXCEPTION HERE
            //                this.mView.ShowRetryOptionsUnknownException(e);
            //            }

            //            this.mView.HideProgressDialog();
        }
    }
}