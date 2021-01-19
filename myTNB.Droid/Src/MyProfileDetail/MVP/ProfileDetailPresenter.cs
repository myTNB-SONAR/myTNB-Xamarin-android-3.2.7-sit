using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Android.Util;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.MultipleAccountPayment.Models;
using myTNB_Android.Src.ManageCards.Models;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.MyTNBService.Request;
using myTNB_Android.Src.MyTNBService.ServiceImpl;
using myTNB_Android.Src.Utils;
using Refit;

namespace myTNB_Android.Src.MyProfileDetail.MVP
{
    public class ProfileDetailPresenter : ProfileDetailContract.IUserActionsListener
    {
        private ProfileDetailContract.IView mView;
        private readonly string TAG = typeof(ProfileDetailPresenter).Name;

        public ProfileDetailPresenter(ProfileDetailContract.IView mView)
        {
            this.mView = mView;
            this.mView.SetPresenter(this);
        }

        public void Start()
        {
            throw new NotImplementedException();
        }

        public async void ResendEmailVerify(string apiKeyId, string email)
        {
            if (mView.IsActive())
            {
                this.mView.ShowGetCodeProgressDialog();
            }

            try
            {
                
                var emailVerificationResponse = await ServiceApiImpl.Instance.SendEmailVerify(new SendEmailVerificationRequest(email));



                if (mView.IsActive())
                {
                    this.mView.HideGetCodeProgressDialog();
                }

                if (emailVerificationResponse.IsSuccessResponse())
                {
                    string message = emailVerificationResponse.Response.Message;
                    this.mView.ShowEmailUpdateSuccess(message);
                }
                else
                {
                    string errorMessage = emailVerificationResponse.Response.Message;
                    this.mView.ShowError(errorMessage);
                }
            }
            catch (OperationCanceledException e)
            {
                if (mView.IsActive())
                {
                    this.mView.HideGetCodeProgressDialog();
                }
                // CANCLLED
                this.mView.ShowRetryOptionsCodeCancelledException(e);
                Utility.LoggingNonFatalError(e);
            }
            catch (ApiException e)
            {
                if (mView.IsActive())
                {
                    this.mView.HideGetCodeProgressDialog();
                }
                // API EXCEPTION
                this.mView.ShowRetryOptionsCodeApiException(e);
                Utility.LoggingNonFatalError(e);
            }
            catch (Exception e)
            {
                if (mView.IsActive())
                {
                    this.mView.HideGetCodeProgressDialog();
                }
                // UNKNOWN EXCEPTION
                this.mView.ShowRetryOptionsCodeUnknownException(e);
                Utility.LoggingNonFatalError(e);
            }
        }
    }
}
