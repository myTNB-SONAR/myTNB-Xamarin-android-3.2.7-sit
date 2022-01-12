using Android.Util;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.ServiceDistruptionRating.Model;
using myTNB_Android.Src.MyTNBService.Request;
using myTNB_Android.Src.MyTNBService.ServiceImpl;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;
using Refit;
using System;
using System.Collections.Generic;
using System.Threading;

namespace myTNB_Android.Src.ServiceDistruptionRating.MVP
{
    public class ServiceDistruptionRatingPresenter : ServiceDistruptionRatingContract.IUserActionsListener
    {

        private ServiceDistruptionRatingContract.IView mView;

        CancellationTokenSource cts;

        private string TAG = typeof(ServiceDistruptionRatingPresenter).Name;


        public ServiceDistruptionRatingPresenter(ServiceDistruptionRatingContract.IView view)
        {
            this.mView = view;
            this.mView.SetPresenter(this);
        }

        public void GetQuestions(string questionCatId)
        {
            GetRateUsQuestions(questionCatId);
        }

        public async void GetRateUsQuestions(string questionCategoryID)
        {
            if (mView.IsActive())
            {
                this.mView.ShowProgressDialog();
            }
            try
            {
                var questionRespone = await ServiceApiImpl.Instance.GetRateUsQuestions(new GetRateUsQuestionRequest(questionCategoryID));

                if (mView.IsActive())
                {
                    this.mView.HideProgressDialog();
                }

                if (!questionRespone.IsSuccessResponse())
                {
                    this.mView.ShowError(questionRespone.Response.DisplayMessage);
                }
                else
                {
                    this.mView.ShowGetQuestionSuccess(questionRespone);
                }
            }
            catch (System.OperationCanceledException e)
            {
                Log.Debug(TAG, "Cancelled Exception");
                // ADD OPERATION CANCELLED HERE
                if (mView.IsActive())
                {
                    this.mView.HideProgressDialog();
                }
                Utility.LoggingNonFatalError(e);
            }
            catch (ApiException apiException)
            {
                // ADD HTTP CONNECTION EXCEPTION HERE
                Log.Debug(TAG, "Api Exception" + apiException.Message);
                if (mView.IsActive())
                {
                    this.mView.HideProgressDialog();
                }
                Utility.LoggingNonFatalError(apiException);
            }
            catch (System.Exception e)
            {
                // ADD UNKNOWN EXCEPTION HERE
                Log.Debug(TAG, "Stack " + e.StackTrace);
                if (mView.IsActive())
                {
                    this.mView.HideProgressDialog();
                }
                this.mView.ShowRetryOptionsUnknownException(e);
                Utility.LoggingNonFatalError(e);
            }
        }

        public void Start()
        {

        }

        public async void SubmitRateUs(List<SubmitDataModel.InputAnswerDetails> inputAnswerDetails, string questionCategoryId)
        {
            if (mView.IsActive())
            {
                this.mView.ShowProgressDialog();
            }

            try
            {
                if (UserEntity.IsCurrentlyActive())
                {
                    UserEntity entity = UserEntity.GetActive();

                    //EnergyBudgetRating.Request.SubmitRateUsRequest rateUsRequest = new EnergyBudgetRating.Request.SubmitRateUsRequest(string.Empty,
                    //entity.Email, entity.DeviceId, inputAnswerDetails, questionCategoryId);
                    //string dt = JsonConvert.SerializeObject(rateUsRequest);
                    //var submitRateUsResponse = await ServiceApiImpl.Instance.SubmitRateUsV2(rateUsRequest);
                    //if (mView.IsActive())
                    //{
                    //    this.mView.HideProgressDialog();
                    //}

                    //if (!submitRateUsResponse.IsSuccessResponse())
                    //{
                    //    this.mView.ShowError(submitRateUsResponse.Response.DisplayMessage);
                    //}
                    //else
                    //{
                    //    this.mView.ShowSumitRateUsSuccess();
                    //}
                }
            }
            catch (System.OperationCanceledException e)
            {
                Log.Debug(TAG, "Cancelled Exception");
                // ADD OPERATION CANCELLED HERE
                if (mView.IsActive())
                {
                    this.mView.HideProgressDialog();
                }
                Utility.LoggingNonFatalError(e);
            }
            catch (ApiException apiException)
            {
                // ADD HTTP CONNECTION EXCEPTION HERE
                Log.Debug(TAG, "Api Exception" + apiException.Message);
                if (mView.IsActive())
                {
                    this.mView.HideProgressDialog();
                }
                Utility.LoggingNonFatalError(apiException);
            }
            catch (System.Exception e)
            {
                // ADD UNKNOWN EXCEPTION HERE
                Log.Debug(TAG, "Stack " + e.StackTrace);
                if (mView.IsActive())
                {
                    this.mView.HideProgressDialog();
                }
                this.mView.ShowRetryOptionsUnknownException(e);
                Utility.LoggingNonFatalError(e);
            }
        }
    }
}
