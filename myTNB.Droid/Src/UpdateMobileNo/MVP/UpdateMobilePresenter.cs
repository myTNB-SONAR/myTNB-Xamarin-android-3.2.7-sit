﻿using Android.Telephony;
using Android.Text;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.Login.Requests;
using myTNB_Android.Src.MyTNBService.ServiceImpl;
using myTNB_Android.Src.Utils;
using Refit;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;

namespace myTNB_Android.Src.UpdateMobileNo.MVP
{
    public class UpdateMobilePresenter : UpdateMobileContract.IUserActionsListener
    {

        private UpdateMobileContract.IView mView;

        CancellationTokenSource cts;

        public UpdateMobilePresenter(UpdateMobileContract.IView mVIew)
        {
            this.mView = mVIew;
            this.mView.SetPresenter(this);
        }

        public async void OnUpdatePhoneNo(string newPhoneNumber, UserAuthenticationRequest request)
        {
            this.mView.ClearErrors();

            if (TextUtils.IsEmpty(newPhoneNumber))
            {
                this.mView.ShowInvalidMobileNoError();
                return;
            }

            if (!PhoneNumberUtils.IsGlobalPhoneNumber(newPhoneNumber))
            {
                this.mView.ShowInvalidMobileNoError();
                return;
            }


            if (!Utility.IsValidMobileNumber(newPhoneNumber))
            {
                this.mView.ShowInvalidMobileNoError();
                return;
            }

            if (mView.IsActive())
            {
                this.mView.ShowProgress();
            }

            try
            {
                MyTNBService.Request.SendUpdatePhoneTokenSMSRequest sendUpdatePhoneTokenSMSRequest = new MyTNBService.Request.SendUpdatePhoneTokenSMSRequest(newPhoneNumber);
                if (request != null)
                {
                    sendUpdatePhoneTokenSMSRequest.SetUserName(request.UserName);
                }
                var updateMobileResponse = await ServiceApiImpl.Instance.SendUpdatePhoneTokenSMS(sendUpdatePhoneTokenSMSRequest);

                if (mView.IsActive())
                {
                    this.mView.HideProgress();
                }

                if (updateMobileResponse.IsSuccessResponse())
                {
                    this.mView.ShowSuccess(newPhoneNumber);
                }
                else
                {
                    this.mView.ShowErrorMessage(updateMobileResponse.Response.DisplayMessage);
                }
            }
            catch (System.OperationCanceledException e)
            {
                if (mView.IsActive())
                {
                    this.mView.HideProgress();
                }
                // ADD OPERATION CANCELLED HERE
                this.mView.ShowRetryOptionsCancelledException(e);
                Utility.LoggingNonFatalError(e);
            }
            catch (ApiException apiException)
            {
                if (mView.IsActive())
                {
                    this.mView.HideProgress();
                }
                // ADD HTTP CONNECTION EXCEPTION HERE
                this.mView.ShowRetryOptionsApiException(apiException);
                Utility.LoggingNonFatalError(apiException);
            }
            catch (Exception e)
            {
                if (mView.IsActive())
                {
                    this.mView.HideProgress();
                }
                // ADD UNKNOWN EXCEPTION HERE
                this.mView.ShowRetryOptionsUnknownException(e);
                Utility.LoggingNonFatalError(e);
            }


        }

        public string OnVerfiyCellularCode(string mobileNo)
        {
            try
            {
                if (TextUtils.IsEmpty(mobileNo) || mobileNo.Length < 3 || !mobileNo.Contains("+60"))
                {
                    mobileNo = "+60";
                    this.mView.ClearErrors();
                    //this.mView.DisableSubmitButton();
                }
                else if (mobileNo == "+60")
                {
                    this.mView.ClearErrors();
                    //this.mView.DisableSubmitButton();
                }
                else if (mobileNo.Contains("+60") && mobileNo.IndexOf("+60") > 0)
                {
                    mobileNo = mobileNo.Substring(mobileNo.IndexOf("+60"));
                    if (mobileNo == "+60")
                    {
                        this.mView.ClearErrors();
                        //this.mView.DisableSubmitButton();
                    }
                    else if (!Utility.IsValidMobileNumber(mobileNo))
                    {
                        this.mView.ShowInvalidMobileNoError();
                        //this.mView.DisableSubmitButton();
                    }
                    else
                    {
                        this.mView.ClearErrors();
                        //this.mView.EnableSubmitButton();
                    }
                }
                else
                {
                    if (!Utility.IsValidMobileNumber(mobileNo))
                    {
                        this.mView.ShowInvalidMobileNoError();
                        //this.mView.DisableSubmitButton();
                    }
                    else
                    {
                        this.mView.ClearErrors();
                        //this.mView.EnableSubmitButton();
                    }
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
            return mobileNo;
        }

        public void OnVerifyMobile(string mobileNo, bool isForceUpdate)
        {
            try
            {
                if (TextUtils.IsEmpty(mobileNo))
                {
                    return;
                }


                if (UserEntity.IsCurrentlyActive() && !isForceUpdate)
                {
                    UserEntity entity = UserEntity.GetActive();
                    if (entity.MobileNo.Equals(mobileNo))
                    {
                        this.mView.DisableSaveButton();
                        return;
                    }
                }

                this.mView.EnableSaveButton();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void Start()
        {
            //
            this.mView.DisableSaveButton();
            this.mView.ShowMobile("");
        }
    }
}
