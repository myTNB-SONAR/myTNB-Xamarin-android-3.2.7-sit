using myTNB.Android.Src.Database.Model;
using myTNB.Android.Src.ManageCards.Models;
using myTNB.Android.Src.MyTNBService.Request;
using myTNB.Android.Src.MyTNBService.ServiceImpl;
using myTNB.Android.Src.Utils;
using Refit;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;

namespace myTNB.Android.Src.ManageCards.MVP
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
            if (mView.IsActive())
            {
                this.mView.ShowProgressDialog();
            }

            try
            {

                var removeCardsResponse = await ServiceApiImpl.Instance.RemoveRegisteredCard(new RemoveRegisteredCardRequest(Data.Id));

                if (mView.IsActive())
                {
                    this.mView.HideProgressDialog();
                }

                if (removeCardsResponse.IsSuccessResponse())
                {
                    this.mView.ShowRemoveSuccess(Data, position);
                }
                else
                {
                    this.mView.ShowErrorMessage(removeCardsResponse.Response.DisplayMessage);
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

        public void OnRemoveStay(CreditCardData RemovedCard, int position)
        {
            this.mView.ShowSnackbarRemovedSuccess(RemovedCard, position);
        }

        public void Start()
        {
            //
            this.mView.ShowCards();
            try
            {
                ServicePointManager.ServerCertificateValidationCallback += SSLFactoryHelper.CertificateValidationCallBack;
                if (CustomerBillingAccount.HasItems())
                {
                    List<CustomerBillingAccount> customerAccountList = CustomerBillingAccount.List();
                    if (customerAccountList != null && customerAccountList.Count > 0)
                    {
                        this.mView.ShowAccountList(customerAccountList);
                    }
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

    }
}
