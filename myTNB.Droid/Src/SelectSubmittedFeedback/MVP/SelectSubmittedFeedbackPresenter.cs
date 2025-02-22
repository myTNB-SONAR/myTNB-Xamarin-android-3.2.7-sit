﻿using Android.Content;
using myTNB.AndroidApp.Src.Base.Api;
using myTNB.AndroidApp.Src.Base.Models;
using myTNB.AndroidApp.Src.Database.Model;
using myTNB.AndroidApp.Src.MyTNBService.Request;
using myTNB.AndroidApp.Src.MyTNBService.Response;
using myTNB.AndroidApp.Src.MyTNBService.ServiceImpl;
using myTNB.AndroidApp.Src.Utils;
using Newtonsoft.Json;
using Refit;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;

namespace myTNB.AndroidApp.Src.SelectSubmittedFeedback.MVP
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

            try
            {
                var detailsResponse = await ServiceApiImpl.Instance.SubmittedFeedbackWithContactDetails(new SubmittedFeedbackDetailsRequest(submittedFeedback.FeedbackId));

                if (mView.IsActive())
                {
                    this.mView.HideProgressDialog();
                }

                if (detailsResponse.IsSuccessResponse())
                {
                    if (submittedFeedback.FeedbackCategoryId.Equals("11"))
                    {
                        var claimDetailResponce = await ServiceApiImpl.Instance.OvervoltageClaimDetail(new SubmittedFeedbeckClaimIdDetailRequestModel(submittedFeedback.FeedbackId));

                        var ClaimId = claimDetailResponce.d.data != null ? claimDetailResponce.d.data.ClaimId : null;
                        UserSessions.SaveSelectedFeedback(mSharedPref, JsonConvert.SerializeObject(detailsResponse.GetData()));
                        this.mView.ShowFeedbackDetailsOverVoltage(detailsResponse.GetData(), submittedFeedback, ClaimId);
                    }
                    else
                    {
                        UserSessions.SaveSelectedFeedback(mSharedPref, JsonConvert.SerializeObject(detailsResponse.GetData()));
                        if (submittedFeedback.FeedbackCategoryId.Equals("1"))
                        {
                            this.mView.ShowFeedbackDetailsBillRelated(detailsResponse.GetData(), submittedFeedback, false);
                        }
                        else if (submittedFeedback.FeedbackCategoryId.Equals("2"))
                        {
                            this.mView.ShowFeedbackDetailsFaultyLamps(detailsResponse.GetData());
                        }
                        else if (submittedFeedback.FeedbackCategoryId.Equals("4"))
                        {
                            this.mView.ShowFeedbackDetailsBillRelated(detailsResponse.GetData(), submittedFeedback, false);
                        }
                        else if (submittedFeedback.FeedbackCategoryId.Equals("8"))
                        {
                            this.mView.ShowFeedbackDetailsBillRelated(detailsResponse.GetData(), submittedFeedback, true);
                        }
                        else if (submittedFeedback.FeedbackCategoryId.Equals("9"))
                        {
                            this.mView.ShowFeedbackDetailsGSL();
                        }
                        else
                        {
                            this.mView.ShowFeedbackDetailsOthers(detailsResponse.GetData());
                        }
                    }
                }
                else
                {
                    if (detailsResponse.Response.Status.Equals("failed"))
                    {
                        this.mView.ShowBCRMDownException(detailsResponse.Response.DisplayMessage);
                    }
                    else
                    {
                        string message = "";

                        if (detailsResponse != null && detailsResponse.Response != null && !string.IsNullOrEmpty(detailsResponse.Response.DisplayMessage))
                        {
                            message = detailsResponse.Response.DisplayMessage;
                        }

                        this.mView.ShowRetryOptionsCancelledException(null, message);
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
                this.mView.ShowRetryOptionsCancelledException(e, "");
                Utility.LoggingNonFatalError(e);
            }
            catch (ApiException apiException)
            {
                if (mView.IsActive())
                {
                    this.mView.HideProgressDialog();
                }
                // ADD HTTP CONNECTION EXCEPTION HERE
                this.mView.ShowRetryOptionsApiException(apiException, "");
                Utility.LoggingNonFatalError(apiException);
            }
            catch (Exception e)
            {
                if (mView.IsActive())
                {
                    this.mView.HideProgressDialog();
                }
                // ADD UNKNOWN EXCEPTION HERE
                this.mView.ShowRetryOptionsUnknownException(e, "");
                Utility.LoggingNonFatalError(e);
            }


        }


        public async void OnStartShowLoading(string deviceId)
        {

            try
            {
                var submittedFeedbackResponse = await ServiceApiImpl.Instance.SubmittedFeedbackList(new SubmittedFeedbackListRequest());



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
                        sf.StatusCode = responseData.StatusCode;
                        sf.StatusDesc = responseData.StatusDesc;
                        sf.IsRead = responseData.IsRead;
                        submittedFeedbackList.Add(sf);
                        SubmittedFeedbackEntity.InsertOrReplace(sf);
                    }

                    if (mView.IsActive())
                    {
                        this.mView.HideProgressDialog();
                    }

                    this.mView.ClearList();

                    this.mView.ShowList(submittedFeedbackList);
                }
                else
                {
                    if (mView.IsActive())
                    {
                        this.mView.HideProgressDialog();
                    }
                    this.mView.ShowRetryOptionsCancelledException(null, "");
                }
            }
            catch (System.OperationCanceledException e)
            {
                if (mView.IsActive())
                {
                    this.mView.HideProgressDialog();
                }
                // ADD OPERATION CANCELLED HERE
                this.mView.ShowRetryOptionsCancelledException(e, "");
                Utility.LoggingNonFatalError(e);
            }
            catch (ApiException apiException)
            {
                if (mView.IsActive())
                {
                    this.mView.HideProgressDialog();
                }
                // ADD HTTP CONNECTION EXCEPTION HERE
                this.mView.ShowRetryOptionsApiException(apiException, "");
                Utility.LoggingNonFatalError(apiException);
            }
            catch (Exception e)
            {
                if (mView.IsActive())
                {
                    this.mView.HideProgressDialog();
                }
                // ADD UNKNOWN EXCEPTION HERE
                this.mView.ShowRetryOptionsUnknownException(e, "");
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
