using Android.Util;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.EnergyBudgetRating.Model;
using myTNB_Android.Src.MyTNBService.Request;
using myTNB_Android.Src.MyTNBService.ServiceImpl;
using myTNB_Android.Src.Utils;
using Refit;
using System;
using System.Collections.Generic;
using System.Threading;

namespace myTNB_Android.Src.EnergyBudgetRating.MVP
{
    public class EnergyBudgetRatingPresenter : EnergyBudgetRatingContract.IUserActionsListener
    {

        private EnergyBudgetRatingContract.IView mView;

        CancellationTokenSource cts;

        private string TAG = typeof(EnergyBudgetRatingPresenter).Name;


        public EnergyBudgetRatingPresenter(EnergyBudgetRatingContract.IView view)
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

        public void PrepareSubmitRateUsRequest(string referenceId, string deviceID, List<RateUsStar.InputOptionValue> inputAnswerDetails)
        {
            try
            {
                if (UserEntity.IsCurrentlyActive())
                {
                    UserEntity entity = UserEntity.GetActive();

                    Request.SubmitRateUsEBRequest submitRateUsEBRequest = new Request.SubmitRateUsEBRequest()
                    {
                        ApiKeyID = Constants.APP_CONFIG.API_KEY_ID
                    };
                    submitRateUsEBRequest.InputAnswer = new Request.SubmitRateUsEBRequest.InputAnswerT()
                    {
                        ReferenceId = referenceId,
                        Email = entity.Email,
                        DeviceId = deviceID,
                        //InputAnswerDetails = inputAnswerDetails
                    };

                    SubmitRateUs(submitRateUsEBRequest);
                }
            }
            catch (Exception e)
            {
                Log.Debug(TAG, e.StackTrace);
                Utility.LoggingNonFatalError(e);
            }
        }

        public void Start()
        {

        }

        public async void SubmitRateUs(Request.SubmitRateUsEBRequest submitRateUsEBRequest)
        {
            if (mView.IsActive())
            {
                this.mView.ShowProgressDialog();
            }

            try
            {
                MyTNBService.Request.SubmitRateUsRequest rateUsRequest = new MyTNBService.Request.SubmitRateUsRequest(submitRateUsEBRequest.InputAnswer.ReferenceId,
                    submitRateUsEBRequest.InputAnswer.Email, submitRateUsEBRequest.InputAnswer.DeviceId);
                submitRateUsEBRequest.InputAnswer.InputAnswerDetails.ForEach(answer =>
                {
                    rateUsRequest.AddAnswerDetails(answer.WLTYQuestionId,
                        answer.RatingInput, answer.MultilineInput);
                });
                var submitRateUsResponse = await ServiceApiImpl.Instance.SubmitRateUs(rateUsRequest);

                if (mView.IsActive())
                {
                    this.mView.HideProgressDialog();
                }

                if (!submitRateUsResponse.IsSuccessResponse())
                {
                    this.mView.ShowError(submitRateUsResponse.Response.DisplayMessage);
                }
                else
                {
                    this.mView.ShowSumitRateUsSuccess();
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
