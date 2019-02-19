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
            this.mView.ShowProgressDialog();
            

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