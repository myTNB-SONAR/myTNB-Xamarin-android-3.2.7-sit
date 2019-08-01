﻿using System;
using System.Net;
using System.Threading.Tasks;
using Android.Gms.Common.Apis;
using Android.Telephony;
using Android.Text;
using Android.Util;
using myTNB_Android.Src.Base.Models;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.SSMR.SMRApplication.Api;
using myTNB_Android.Src.SSMRTerminate.Api;
using myTNB_Android.Src.Utils;

namespace myTNB_Android.Src.SSMRTerminate.MVP
{
    public class SSMRTerminatePresenter : SSMRTerminateContract.IPresenter
    {
        SSMRTerminateContract.IView mView;
        SSMRTerminateContract.SSMRTerminateApiPresenter terminationApi;

        public SSMRTerminatePresenter(SSMRTerminateContract.IView view)
        {
            mView = view;
            terminationApi = new SSMRTerminateImpl();
        }

        public void InitiateCAInfo(AccountData selectedAccount)
        {
            GetCARegisteredContactInfoAsync(selectedAccount);
        }

        public void InitiateTerminationReasonsList()
        {
            GetTerminationReasonsList();
        }

        private async void GetTerminationReasonsList()
        {
            try
            {
                ServicePointManager.ServerCertificateValidationCallback += SSLFactoryHelper.CertificateValidationCallBack;
                UserInterface currentUsrInf = new UserInterface()
                {
                    eid = UserEntity.GetActive().Email,
                    sspuid = UserEntity.GetActive().UserID,
                    did = this.mView.GetDeviceId(),
                    ft = FirebaseTokenEntity.GetLatest().FBToken,
                    lang = Constants.DEFAULT_LANG.ToUpper(),
                    sec_auth_k1 = Constants.APP_CONFIG.API_KEY_ID,
                    sec_auth_k2 = "",
                    ses_param1 = "",
                    ses_param2 = ""
                };

                SMRTerminationReasonsResponse response = await this.terminationApi.GetSMRTerminationReasons(new GetSMRTerminationReasonsRequest()
                {
                    usrInf = currentUsrInf
                });

                if (response.Response.ErrorCode == "7200")
                {
                    this.mView.SetTerminationReasonsList(response.Response.TerminationReasons.ReasonList);
                }
                else
                {
                    this.mView.HideProgressDialog();
                }
            }
            catch (System.OperationCanceledException cancelledException)
            {
                this.mView.HideProgressDialog();
                Utility.LoggingNonFatalError(cancelledException);
            }
            catch (ApiException apiException)
            {
                this.mView.HideProgressDialog();
                Utility.LoggingNonFatalError(apiException);
            }
            catch (Exception unknownException)
            {
                this.mView.HideProgressDialog();
                Utility.LoggingNonFatalError(unknownException);
            }
        }

        private async void GetCARegisteredContactInfoAsync(AccountData selectedAccount)
        {
            try
            {
                ServicePointManager.ServerCertificateValidationCallback += SSLFactoryHelper.CertificateValidationCallBack;
                UserInterface currentUsrInf = new UserInterface()
                {
                    eid = UserEntity.GetActive().Email,
                    sspuid = UserEntity.GetActive().UserID,
                    did = this.mView.GetDeviceId(),
                    ft = FirebaseTokenEntity.GetLatest().FBToken,
                    lang = Constants.DEFAULT_LANG.ToUpper(),
                    sec_auth_k1 = Constants.APP_CONFIG.API_KEY_ID,
                    sec_auth_k2 = "",
                    ses_param1 = "",
                    ses_param2 = ""
                };

                CARegisteredContactInfoResponse response = await this.terminationApi.GetCARegisteredContactInfo(new GetRegisteredContactInfoRequest()
                {
                    AccountNumber = selectedAccount.AccountNum,
                    IsOwnedAccount = "true",
                    usrInf = currentUsrInf
                });

                if (response.Data.ErrorCode == "7200")
                {
                    this.mView.UpdateSMRData(response.Data.AccountDetailsData.Email, response.Data.AccountDetailsData.Mobile);
                }
            }
            catch (System.OperationCanceledException cancelledException)
            {
                Utility.LoggingNonFatalError(cancelledException);
            }
            catch (ApiException apiException)
            {
                Utility.LoggingNonFatalError(apiException);
            }
            catch (Exception unknownException)
            {
                Utility.LoggingNonFatalError(unknownException);
            }

        }

        public void CheckRequiredFields(string mobile_no, string email, bool isOtherReasonSelected, string otherReason)
        {

            try
            {
                if (!TextUtils.IsEmpty(mobile_no) && !TextUtils.IsEmpty(email))
                {
                    bool isError = false;
                    if (!Patterns.EmailAddress.Matcher(email).Matches())
                    {
                        this.mView.ShowInvalidEmailError();
                        this.mView.DisableSubmitButton();
                        isError = true;
                    }
                    else
                    {
                        this.mView.ClearEmailError();
                        isError = false;
                    }

                    if (mobile_no == "+60")
                    {
                        this.mView.UpdateMobileNumber(mobile_no);
                        this.mView.DisableSubmitButton();
                        isError = true;
                    }

                    if (mobile_no.Length == 3)
                    {
                        this.mView.ClearInvalidMobileError();
                        this.mView.DisableSubmitButton();
                        isError = true;
                    }
                    else if (!Utility.IsValidMobileNumber(mobile_no))
                    {
                        this.mView.ShowInvalidMobileNoError();
                        this.mView.DisableSubmitButton();
                        isError = true;
                    }
                    else
                    {
                        this.mView.ClearInvalidMobileError();
                        isError = false;
                    }

                    if(isError == true)
                    {
                        return;
                    }

                    this.mView.ClearErrors();
                    this.mView.EnableSubmitButton();
                }
                else
                {
                    this.mView.DisableSubmitButton();
                }
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }
    }
}
