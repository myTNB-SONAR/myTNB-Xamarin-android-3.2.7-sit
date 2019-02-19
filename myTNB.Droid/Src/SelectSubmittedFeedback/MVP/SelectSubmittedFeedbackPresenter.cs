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
using System.Threading;
using System.Net.Http;
using myTNB_Android.Src.Utils;
using myTNB_Android.Src.Base.Api;
using Refit;
using myTNB_Android.Src.Database.Model;
using System.Net;
using myTNB_Android.Src.Base.Models;
using Newtonsoft.Json;

namespace myTNB_Android.Src.SelectSubmittedFeedback.MVP
{
    public class SelectSubmittedFeedbackPresenter : SelectSubmittedFeedbackContract.IUserActionsListener
    {

        private SelectSubmittedFeedbackContract.IView mView;
        private ISharedPreferences mSharedPref;
        CancellationTokenSource cts;

        public SelectSubmittedFeedbackPresenter(SelectSubmittedFeedbackContract.IView mView , ISharedPreferences mSharedPref)
        {
            this.mView = mView;
            this.mView.SetPresenter(this);
            this.mSharedPref = mSharedPref;
        }

        public async void OnSelect(SubmittedFeedback submittedFeedback)
        {
            cts = new CancellationTokenSource();
            this.mView.ShowProgressDialog();
#if DEBUG
            var httpClient = new HttpClient(new HttpLoggingHandler(/*new NativeMessageHandler()*/)) { BaseAddress = new Uri(Constants.SERVER_URL.END_POINT) };
            var feedbackApi = RestService.For<IFeedbackApi>(httpClient);
#else

            var feedbackApi = RestService.For<IFeedbackApi>(Constants.SERVER_URL.END_POINT);
#endif
            ServicePointManager.ServerCertificateValidationCallback += SSLFactoryHelper.CertificateValidationCallBack;
            try
            {
                var detailsResponse = await feedbackApi.GetSubmittedFeedbackDetails(new Base.Request.SubmittedFeedbackDetailsRequest()
                {
                    ApiKeyId = Constants.APP_CONFIG.API_KEY_ID,
                    ServiceReqNo = submittedFeedback.FeedbackId
                }, cts.Token);

                if (!detailsResponse.Data.IsError)
                {
                    if (submittedFeedback.FeedbackCategoryId.Equals("1"))
                    {
                        UserSessions.SaveSelectedFeedback(mSharedPref, JsonConvert.SerializeObject(detailsResponse.Data.Data));
                        this.mView.ShowFeedbackDetailsBillRelated(detailsResponse.Data.Data);
                    }
                    else if (submittedFeedback.FeedbackCategoryId.Equals("2"))
                    {
                        UserSessions.SaveSelectedFeedback(mSharedPref, JsonConvert.SerializeObject(detailsResponse.Data.Data));
                        this.mView.ShowFeedbackDetailsFaultyLamps(detailsResponse.Data.Data);
                    }
                    else
                    {
                        UserSessions.SaveSelectedFeedback(mSharedPref, JsonConvert.SerializeObject(detailsResponse.Data.Data));
                        this.mView.ShowFeedbackDetailsOthers(detailsResponse.Data.Data);
                    }
                }
                else
                {
                    if (detailsResponse.Data.Status.Equals("failed"))
                    {
                        this.mView.ShowBCRMDownException(detailsResponse.Data.Message);
                    }
                    else
                    {
                        this.mView.ShowRetryOptionsCancelledException(null);
                    }
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

            this.mView.HideProgressDialog();

        }


        public async void OnStartShowLoading(string deviceId)
        {

            cts = new CancellationTokenSource();
            this.mView.ShowProgressDialog();
#if DEBUG
            var httpClient = new HttpClient(new HttpLoggingHandler(/*new NativeMessageHandler()*/)) { BaseAddress = new Uri(Constants.SERVER_URL.END_POINT) };
            var feedbackApi = RestService.For<IFeedbackApi>(httpClient);
#else

            var feedbackApi = RestService.For<IFeedbackApi>(Constants.SERVER_URL.END_POINT);
#endif
            ServicePointManager.ServerCertificateValidationCallback += SSLFactoryHelper.CertificateValidationCallBack;
            try
            {
                if (UserEntity.IsCurrentlyActive())
                {
                    UserEntity loggedUser = UserEntity.GetActive();

                    var submittedFeedbackResponse = await feedbackApi.GetSubmittedFeedbackList(new Base.Request.SubmittedFeedbackRequest()
                    {
                        ApiKeyId = Constants.APP_CONFIG.API_KEY_ID,
                        Email = loggedUser.Email,
                        DeviceId = deviceId

                    }, cts.Token);
                    if (!submittedFeedbackResponse.Data.IsError)
                    {
                        foreach (SubmittedFeedback sf in submittedFeedbackResponse.Data.Data)
                        {
                            SubmittedFeedbackEntity.InsertOrReplace(sf);

                        }

                        this.mView.ClearList();

                        this.mView.ShowList(submittedFeedbackResponse.Data.Data);
                    }
                    else
                    {
                        this.mView.ShowRetryOptionsCancelledException(null);
                    }
                }
                else
                {
                    var submittedFeedbackResponse = await feedbackApi.GetSubmittedFeedbackList(new Base.Request.SubmittedFeedbackRequest()
                    {
                        ApiKeyId = Constants.APP_CONFIG.API_KEY_ID,
                        Email = "",
                        DeviceId = deviceId

                    }, cts.Token);
                    if (!submittedFeedbackResponse.Data.IsError)
                    {
                        foreach (SubmittedFeedback sf in submittedFeedbackResponse.Data.Data)
                        {
                            SubmittedFeedbackEntity.InsertOrReplace(sf);

                        }

                        this.mView.ClearList();

                        this.mView.ShowList(submittedFeedbackResponse.Data.Data);
                    }
                    else
                    {
                        this.mView.ShowRetryOptionsCancelledException(null);
                    }
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

            this.mView.HideProgressDialog();


        }

        public void Start()
        {
            this.mView.ShowStartLoading();
        }
    }
}