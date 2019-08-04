using System;
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
                    this.mView.SetTerminationReasonsList(null);
                }
            }
            catch (System.OperationCanceledException cancelledException)
            {
                this.mView.SetTerminationReasonsList(null);
                Utility.LoggingNonFatalError(cancelledException);
            }
            catch (ApiException apiException)
            {
                this.mView.SetTerminationReasonsList(null);
                Utility.LoggingNonFatalError(apiException);
            }
            catch (Exception unknownException)
            {
                this.mView.SetTerminationReasonsList(null);
                Utility.LoggingNonFatalError(unknownException);
            }
        }

        public void NavigateToTermsAndConditions()
        {
            this.mView.ShowTermsAndConditions();
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
                else
                {
                    this.mView.UpdateSMRData("", "");
                }
            }
            catch (System.OperationCanceledException cancelledException)
            {
                this.mView.UpdateSMRData("", "");
                Utility.LoggingNonFatalError(cancelledException);
            }
            catch (ApiException apiException)
            {
                this.mView.UpdateSMRData("", "");
                Utility.LoggingNonFatalError(apiException);
            }
            catch (Exception unknownException)
            {
                this.mView.UpdateSMRData("", "");
                Utility.LoggingNonFatalError(unknownException);
            }

        }

        public async void OnSubmitApplication(string accountNum, string oldEmail, string oldPhoneNum, string newEmail, string newPhoneNum, string terminationReason)
        {
            try
            {
                this.mView.ShowProgressDialog();
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


                SMRregistrationSubmitResponse response = await this.terminationApi.SubmitSMRApplication(new SubmitSMRApplicationRequest()
                {
                    AccountNumber = accountNum,
                    OldPhone = oldPhoneNum,
                    NewPhone = newPhoneNum,
                    OldEmail = oldEmail,
                    NewEmail = newEmail,
                    SMRMode = "T",
                    TerminationReason = terminationReason,
                    usrInf = currentUsrInf
                });

                this.mView.HideProgressDialog();
                if (response.Data.ErrorCode == "7200" /*&& response.Data.AccountDetailsData.Status == "S"*/)
                {
                    this.mView.OnRequestSuccessful(response);
                }
                else
                {
                    this.mView.OnRequestFailed(response);
                }
            }
            catch (System.OperationCanceledException cancelledException)
            {
                this.mView.HideProgressDialog();
                this.mView.OnRequestFailed(null);
                Utility.LoggingNonFatalError(cancelledException);
            }
            catch (ApiException apiException)
            {
                this.mView.HideProgressDialog();
                this.mView.OnRequestFailed(null);
                Utility.LoggingNonFatalError(apiException);
            }
            catch (Exception unknownException)
            {
                this.mView.HideProgressDialog();
                this.mView.OnRequestFailed(null);
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
                    else if (mobile_no.Length < 3)
                    {
                        this.mView.UpdateMobileNumber("+60");
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
                    }

                    if (isOtherReasonSelected)
                    {
                        if (TextUtils.IsEmpty(otherReason))
                        {
                            this.mView.ShowEmptyReasonError();
                            this.mView.DisableSubmitButton();
                            isError = true;
                        }
                        else
                        {
                            this.mView.ClearReasonError();
                        }
                    }
                    else
                    {
                        this.mView.ClearReasonError();
                    }

                    if (isError == true)
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
