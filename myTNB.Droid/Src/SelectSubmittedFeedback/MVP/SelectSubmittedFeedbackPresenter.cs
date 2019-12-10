﻿using Android.Content;
using myTNB_Android.Src.Base.Api;
using myTNB_Android.Src.Base.Models;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.MyTNBService.Request;
using myTNB_Android.Src.MyTNBService.Response;
using myTNB_Android.Src.MyTNBService.ServiceImpl;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;
using Refit;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;

namespace myTNB_Android.Src.SelectSubmittedFeedback.MVP
{
    public class SelectSubmittedFeedbackPresenter : SelectSubmittedFeedbackContract.IUserActionsListener
    {

        private SelectSubmittedFeedbackContract.IView mView;
        private ISharedPreferences mSharedPref;
        CancellationTokenSource cts;

        public SelectSubmittedFeedbackPresenter(SelectSubmittedFeedbackContract.IView mView, ISharedPreferences mSharedPref)
        {
            this.mView = mView;
            this.mView.SetPresenter(this);
            this.mSharedPref = mSharedPref;
        }

        public async void OnSelect(SubmittedFeedback submittedFeedback)
        {
            cts = new CancellationTokenSource();
            if (mView.IsActive())
            {
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
                //var detailsResponse = await feedbackApi.GetSubmittedFeedbackDetails(new Base.Request.SubmittedFeedbackDetailsRequest()
                //{
                //    ApiKeyId = Constants.APP_CONFIG.API_KEY_ID,
                //    ServiceReqNo = submittedFeedback.FeedbackId
                //}, cts.Token);

                var detailsResponse = await ServiceApiImpl.Instance.SubmittedFeedbackDetails(new SubmittedFeedbackDetailsRequest(submittedFeedback.FeedbackId));

                if (mView.IsActive())
                {
                    this.mView.HideProgressDialog();
                }

                if (detailsResponse.IsSuccessResponse())
                {
                    if (submittedFeedback.FeedbackCategoryId.Equals("1"))
                    {
                        UserSessions.SaveSelectedFeedback(mSharedPref, JsonConvert.SerializeObject(detailsResponse.GetData()));
                        this.mView.ShowFeedbackDetailsBillRelated(detailsResponse.GetData(), submittedFeedback);
                    }
                    else if (submittedFeedback.FeedbackCategoryId.Equals("2"))
                    {
                        UserSessions.SaveSelectedFeedback(mSharedPref, JsonConvert.SerializeObject(detailsResponse.GetData()));
                        this.mView.ShowFeedbackDetailsFaultyLamps(detailsResponse.GetData());
                    }
                    else
                    {
                        UserSessions.SaveSelectedFeedback(mSharedPref, JsonConvert.SerializeObject(detailsResponse.GetData()));
                        this.mView.ShowFeedbackDetailsOthers(detailsResponse.GetData());
                    }
                }
                else
                {
                    if (detailsResponse.Response.Status.Equals("failed"))
                    {
                        this.mView.ShowBCRMDownException(detailsResponse.Response.Message);
                    }
                    else
                    {
                        this.mView.ShowRetryOptionsCancelledException(null);
                    }
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


        public async void OnStartShowLoading(string deviceId)
        {
            //if (mView.IsActive()) {
            this.mView.ShowProgressDialog();
            //}

            try
            {
                var submittedFeedbackResponse = await ServiceApiImpl.Instance.SubmittedFeedbackList(new SubmittedFeedbackListRequest());

                //if (mView.IsActive())
                //{
                this.mView.HideProgressDialog();
                //}

                if (submittedFeedbackResponse.IsSuccessResponse())
                {
                    List<SubmittedFeedback> submittedFeedbackList = new List<SubmittedFeedback>();
                    foreach (SubmittedFeedbackListResponse.ResponseData responseData in submittedFeedbackResponse.GetData())
                    {
                        SubmittedFeedback sf = new SubmittedFeedback();
                        sf.FeedbackId = responseData.ServiceReqNo;
                        sf.FeedbackCategoryId = responseData.FeedbackCategoryId;
                        sf.DateCreated = responseData.DateCreated;
                        sf.FeedbackMessage = responseData.FeedbackMessage;
                        sf.FeedbackCategoryName = responseData.FeedbackCategoryName;
                        sf.FeedbackNameInListView = responseData.FeedbackNameInListView;
                        submittedFeedbackList.Add(sf);
                        SubmittedFeedbackEntity.InsertOrReplace(sf);
                    }

                    this.mView.ClearList();

                    this.mView.ShowList(submittedFeedbackList);
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

            if (mView.IsActive())
            {
                this.mView.HideProgressDialog();
            }

        }

        public void Start()
        {
            this.mView.ShowStartLoading();
        }
    }
}
