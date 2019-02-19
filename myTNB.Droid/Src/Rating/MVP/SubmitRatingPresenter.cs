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
using System.Net;
using myTNB_Android.Src.Utils;
using System.Net.Http;
using Refit;
using myTNB_Android.Src.Rating.Api;
using Android.Util;
using myTNB_Android.Src.Rating.Request;
using myTNB_Android.Src.Database.Model;

namespace myTNB_Android.Src.Rating.MVP
{
    public class SubmitRatingPresenter : SubmitRatingContract.IUserActionsListener
    {

        private SubmitRatingContract.IView mView;

        CancellationTokenSource cts;

        private string TAG = typeof(SubmitRatingPresenter).Name;


        public SubmitRatingPresenter(SubmitRatingContract.IView view)
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
            cts = new CancellationTokenSource();
            this.mView.ShowProgressDialog();

            ServicePointManager.ServerCertificateValidationCallback += SSLFactoryHelper.CertificateValidationCallBack;

#if DEBUG
            var httpClient = new HttpClient(new HttpLoggingHandler(/*new NativeMessageHandler()*/)) { BaseAddress = new Uri(Constants.SERVER_URL.END_POINT) };
            var api = RestService.For<GetRateUsQuestionsApi>(httpClient);
#else
            var api = RestService.For<GetRateUsQuestionsApi>(Constants.SERVER_URL.END_POINT);
#endif
            try
            {
                var questionRespone = await api.GetQuestions(new Request.GetRateUsQuestionsRequest()
                {
                   ApiKeyID = Constants.APP_CONFIG.API_KEY_ID,
                   QuestionCategoryId = questionCategoryID
                }, cts.Token);

                this.mView.HideProgressDialog();

                if (questionRespone.feedbackQuestionStatus.IsError)
                {
                    this.mView.ShowError(questionRespone.feedbackQuestionStatus.Message);
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
                this.mView.HideProgressDialog();
            }
            catch (ApiException apiException)
            {
                // ADD HTTP CONNECTION EXCEPTION HERE
                Log.Debug(TAG, "Api Exception" + apiException.Message);
                this.mView.HideProgressDialog();
            }
            catch (System.Exception e)
            {
                // ADD UNKNOWN EXCEPTION HERE
                Log.Debug(TAG, "Stack " + e.StackTrace);
                this.mView.HideProgressDialog();
                this.mView.ShowRetryOptionsUnknownException(e);
            }
        }

        public void PrepareSubmitRateUsRequest(string referenceId, string deviceID, List<SubmitRateUsRequest.InputAnswerDetails> inputAnswerDetails)
        {
            try
            {
                if (UserEntity.IsCurrentlyActive())
                {
                    UserEntity entity = UserEntity.GetActive();

                    SubmitRateUsRequest submitRateUsRequest = new SubmitRateUsRequest()
                    {
                        ApiKeyID = Constants.APP_CONFIG.API_KEY_ID
                    };
                    submitRateUsRequest.InputAnswer = new SubmitRateUsRequest.InputAnswerT()
                    {
                        ReferenceId = referenceId,
                        Email = entity.Email,
                        DeviceId = deviceID,
                        InputAnswerDetails = inputAnswerDetails
                    };

                    SubmitRateUs(submitRateUsRequest);
                }
            }catch(Exception e)
            {
                Log.Debug(TAG, e.StackTrace);
            }
        }

        public void Start()
        {
            
        }

        public async void SubmitRateUs(SubmitRateUsRequest submitRateUsRequest)
        {
             cts = new CancellationTokenSource();
            this.mView.ShowProgressDialog();

            ServicePointManager.ServerCertificateValidationCallback += SSLFactoryHelper.CertificateValidationCallBack;

#if DEBUG
            var httpClient = new HttpClient(new HttpLoggingHandler(/*new NativeMessageHandler()*/)) { BaseAddress = new Uri(Constants.SERVER_URL.END_POINT) };
            var api = RestService.For<GetRateUsQuestionsApi>(httpClient);
#else
            var api = RestService.For<GetRateUsQuestionsApi>(Constants.SERVER_URL.END_POINT);
#endif
            try
            {
                var submitRateUsResponse = await api.SubmitRateUs(submitRateUsRequest, cts.Token);

                this.mView.HideProgressDialog();

                if (submitRateUsResponse.submitRateUsResult.IsError)
                {
                    this.mView.ShowError(submitRateUsResponse.submitRateUsResult.Message);
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
                this.mView.HideProgressDialog();
            }
            catch (ApiException apiException)
            {
                // ADD HTTP CONNECTION EXCEPTION HERE
                Log.Debug(TAG, "Api Exception" + apiException.Message);
                this.mView.HideProgressDialog();
            }
            catch (System.Exception e)
            {
                // ADD UNKNOWN EXCEPTION HERE
                Log.Debug(TAG, "Stack " + e.StackTrace);
                this.mView.HideProgressDialog();
                this.mView.ShowRetryOptionsUnknownException(e);
            }
        }
    }
}