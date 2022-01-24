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
using myTNB_Android.Src.ServiceDistruptionRating.Request;
using myTNB_Android.Src.MyTNBService.Response;
using myTNB_Android.Src.Base;

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

        public async void OnSetFeedback(string SdEventId, string Email)
        {
            try
            {
                var SDInfo = new ServiceDisruptionInfo
                {
                    sdEventId = SdEventId,
                    email = Email,
                };
                UserServiceDistruptionSetSubRequest request = new UserServiceDistruptionSetSubRequest("sdfeedbackstatus", SDInfo);
                string ts = JsonConvert.SerializeObject(request);
                UserServiceDistruptionSetSubResponse response = await ServiceApiImpl.Instance.ServiceDisruptionInfo(request);
                if (response.Response.ErrorCode == Constants.SERVICE_CODE_SUCCESS)
                {
                    MyTNBAccountManagement.GetInstance().SetIsFinishFeedback(true);
                    this.mView.OnBackPressed();
                }
                else
                {
                    this.mView.ShowRetryOptionsApiException(null);
                    //this.mView.OnBackPressed();
                }
            }
            catch (System.OperationCanceledException e)
            {
                //this.mView.HideLoadingScreen();
                // ADD OPERATION CANCELLED HERE
                //this.mView.ShowRetryOptionsCancelledException(e);
                Utility.LoggingNonFatalError(e);
            }
            catch (ApiException apiException)
            {
                //this.mView.HideLoadingScreen();
                // ADD HTTP CONNECTION EXCEPTION HERE
                this.mView.ShowRetryOptionsApiException(apiException);
                Utility.LoggingNonFatalError(apiException);
            }
            catch (Exception e)
            {
                //this.mView.HideLoadingScreen();
                // ADD UNKNOWN EXCEPTION HERE
                this.mView.ShowRetryOptionsUnknownException(e);
                Utility.LoggingNonFatalError(e);
            }
        }

        public void Start()
        {
            throw new NotImplementedException();
        }

        public async void SubmitRateUs(List<SubmitDataModel.InputAnswerDetails> inputAnswerDetails, string questionCategoryId, string eventId, string caNumList)
        {
            try
            {
                if (UserEntity.IsCurrentlyActive())
                {
                    UserEntity entity = UserEntity.GetActive();

                    Request.SubmitRateUsRequest rateUsRequest = new Request.SubmitRateUsRequest(string.Empty,
                    entity.Email, entity.DeviceId, inputAnswerDetails, questionCategoryId, eventId, caNumList);
                    string dt = JsonConvert.SerializeObject(rateUsRequest);
                    var submitRateUsResponse = await ServiceApiImpl.Instance.SDSubmitRateUs(rateUsRequest);
                    if (!submitRateUsResponse.IsSuccessResponse())
                    {
                        this.mView.ShowError(submitRateUsResponse.Response.DisplayMessage);
                    }
                    else
                    {
                        this.mView.OnCallService();
                    }
                }
            }
            catch (System.OperationCanceledException e)
            {
                Log.Debug(TAG, "Cancelled Exception");
                Utility.LoggingNonFatalError(e);
            }
            catch (ApiException apiException)
            {
                // ADD HTTP CONNECTION EXCEPTION HERE
                Log.Debug(TAG, "Api Exception" + apiException.Message);
                Utility.LoggingNonFatalError(apiException);
            }
            catch (System.Exception e)
            {
                // ADD UNKNOWN EXCEPTION HERE
                Log.Debug(TAG, "Stack " + e.StackTrace);
                this.mView.ShowRetryOptionsUnknownException(e);
                Utility.LoggingNonFatalError(e);
            }
        }
    }
}
