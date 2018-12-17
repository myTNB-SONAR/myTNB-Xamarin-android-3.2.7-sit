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
using myTNB_Android.Src.MakePayment.Models;
using myTNB_Android.Src.ManageCards.Models;
using Refit;
using myTNB_Android.Src.ManageCards.Api;
using System.Net;
using myTNB_Android.Src.Utils;
using System.Net.Http;
using System.Threading;

namespace myTNB_Android.Src.ManageCards.MVP
{
    public class ManageCardsPresenter : ManageCardsContract.IUserActionsListener
    {

        private ManageCardsContract.IView mView;

        CancellationTokenSource cts;

        public ManageCardsPresenter(ManageCardsContract.IView mView)
        {
            this.mView = mView;
            this.mView.SetPresenter(this);
        }

        public async void OnRemove(CreditCardData Data, int position)
        {
            cts = new CancellationTokenSource();
            if (mView.IsActive())
            {
                this.mView.ShowProgressDialog();
            }

            ServicePointManager.ServerCertificateValidationCallback += SSLFactoryHelper.CertificateValidationCallBack;
#if DEBUG || STUB
            var httpClient = new HttpClient(new HttpLoggingHandler(/*new NativeMessageHandler()*/)) { BaseAddress = new Uri(Constants.SERVER_URL.END_POINT) };
            var manageCardsApi = RestService.For<IManageCardsApi>(httpClient);
#else
            var manageCardsApi = RestService.For<IManageCardsApi>(Constants.SERVER_URL.END_POINT);
#endif

            try
            {
                var removeCardsResponse = await manageCardsApi.RemoveCard(new Request.RemoveRegisteredCardRequest()
                {
                    ApiKeyId = Constants.APP_CONFIG.API_KEY_ID,
                    RegisteredCardId = Data.Id
                } , cts.Token);

                if (mView.IsActive())
                {
                    this.mView.HideProgressDialog();
                }

                if (!removeCardsResponse.Data.IsError)
                {
                    this.mView.ShowRemoveSuccess(Data , position);
                }
                else
                {
                    this.mView.ShowErrorMessage(removeCardsResponse.Data.Message);
                }
            }
            catch (System.OperationCanceledException e)
            {
                if (mView.IsActive())
                {
                    this.mView.HideProgressDialog();
                }
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

        public void OnRemoveStay(CreditCardData RemovedCard , int position)
        {
            this.mView.ShowSnackbarRemovedSuccess(RemovedCard, position);
        }

        public void Start()
        {
            //
            this.mView.ShowCards();
        }
    }
}