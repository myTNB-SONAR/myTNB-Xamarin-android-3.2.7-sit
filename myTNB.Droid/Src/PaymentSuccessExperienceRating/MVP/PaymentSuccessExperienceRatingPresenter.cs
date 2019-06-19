using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.PaymentSuccessExperienceRating.Api;
using myTNB_Android.Src.Utils;
using Refit;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;

namespace myTNB_Android.Src.PaymentSuccessExperienceRating.MVP
{
    public class PaymentSuccessExperienceRatingPresenter : PaymentSuccessExperienceRatingContract.IUserActionsListener
    {
        CancellationTokenSource cts;

        private PaymentSuccessExperienceRatingContract.IView mView;

        public PaymentSuccessExperienceRatingPresenter(PaymentSuccessExperienceRatingContract.IView mView)
        {
            this.mView = mView;
            this.mView.SetPresenter(this);
        }

        public void OnRating(int value)
        {
            // TODO: ADD IMPL TO SEND THE VALUE TO SERVER
            this.mView.OnShowRating(value);
        }

        public void Start()
        {
            //
        }

        public void SubmitRating(string rating, string message, string ratingFor)
        {
            ServicePointManager.ServerCertificateValidationCallback += SSLFactoryHelper.CertificateValidationCallBack;
            SubmitRatingAsync(rating, message, ratingFor);
        }

        public async void SubmitRatingAsync(string rating, string message, string ratingFor)
        {
            cts = new CancellationTokenSource();
            if (mView.IsActive())
            {
                this.mView.ShowProgressDialog();
            }

#if DEBUG || STUB
            var httpClient = new HttpClient(new HttpLoggingHandler(/*new NativeMessageHandler()*/)) { BaseAddress = new Uri(Constants.SERVER_URL.END_POINT) };
            var manageCardsApi = RestService.For<SubmitExperienceRatingApi>(httpClient);
#else
            var manageCardsApi = RestService.For<SubmitExperienceRatingApi>(Constants.SERVER_URL.END_POINT);
#endif

            try
            {
                var response = await manageCardsApi.SubmitRating(new Request.SubmitExperienceRatingRequest
                {
                    ApiKeyId = Constants.APP_CONFIG.API_KEY_ID,
                    email = UserEntity.GetActive().Email,
                    rating = rating,
                    message = message,
                    ratingFor = ratingFor
                }, cts.Token);

                if (mView.IsActive())
                {
                    this.mView.HideProgressDialog();
                }
                if (!response.Data.IsError)
                {
                    this.mView.ShowSubmitRatingSuccess(response);
                }
                else
                {
                    this.mView.ShowSubmitRatingError(response.Data.Message);
                }
            }
            catch (System.OperationCanceledException e)
            {
                if (mView.IsActive())
                {
                    this.mView.HideProgressDialog();
                }
                this.mView.ShowErrorMessage("Something went wrong! Please try again later");
                Utility.LoggingNonFatalError(e);
            }
            catch (ApiException apiException)
            {
                // ADD HTTP CONNECTION EXCEPTION HERE
                if (mView.IsActive())
                {
                    this.mView.HideProgressDialog();
                }
                this.mView.ShowErrorMessage("Please check your internet connection.");
                Utility.LoggingNonFatalError(apiException);
            }
            catch (Exception e)
            {
                // ADD UNKNOWN EXCEPTION HERE
                if (mView.IsActive())
                {
                    this.mView.HideProgressDialog();
                }
                this.mView.ShowErrorMessage("Please check your internet connection.");
                Utility.LoggingNonFatalError(e);
            }
        }
    }
}