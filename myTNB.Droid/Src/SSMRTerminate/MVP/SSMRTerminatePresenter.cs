using System;
using System.Net;
using System.Threading.Tasks;
using Android.Gms.Common.Apis;
using Android.Telephony;
using Android.Text;
using Android.Util;
using myTNB.Android.Src.Base.Models;
using myTNB.Android.Src.Database.Model;
using myTNB.Android.Src.myTNBMenu.Models;
using myTNB.Android.Src.SSMR.SMRApplication.Api;
using myTNB.Android.Src.SSMR.SMRApplication.MVP;
using myTNB.Android.Src.SSMRTerminate.Api;
using myTNB.Android.Src.Utils;

namespace myTNB.Android.Src.SSMRTerminate.MVP
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
                    lang = LanguageUtil.GetAppLanguage().ToUpper(),
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

        public void NavigateToTermsAndConditionsView()
        {
            this.mView.ShowTermsAndConditionsView();
        }

        public void NavigateToFAQ()
        {
            this.mView.ShowFAQ();
        }

        private async void GetCARegisteredContactInfoAsync(AccountData selectedAccount)
        {
            SMRAccount selectedSMRAccount = new SMRAccount();
            selectedSMRAccount.email = "";
            selectedSMRAccount.mobileNumber = "";
            if (UserSessions.GetSMRAccountList().Count > 0)
            {
                selectedSMRAccount = UserSessions.GetSMRAccountList().Find(x => x.accountNumber == selectedAccount.AccountNum);
            }
            try
            {
                ServicePointManager.ServerCertificateValidationCallback += SSLFactoryHelper.CertificateValidationCallBack;
                UserInterface currentUsrInf = new UserInterface()
                {
                    eid = UserEntity.GetActive().Email,
                    sspuid = UserEntity.GetActive().UserID,
                    did = this.mView.GetDeviceId(),
                    ft = FirebaseTokenEntity.GetLatest().FBToken,
                    lang = LanguageUtil.GetAppLanguage().ToUpper(),
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
                    this.mView.UpdateSMRData(selectedSMRAccount.email, selectedSMRAccount.mobileNumber);
                }
            }
            catch (System.OperationCanceledException cancelledException)
            {
                this.mView.UpdateSMRData(selectedSMRAccount.email, selectedSMRAccount.mobileNumber);
                Utility.LoggingNonFatalError(cancelledException);
            }
            catch (ApiException apiException)
            {
                this.mView.UpdateSMRData(selectedSMRAccount.email, selectedSMRAccount.mobileNumber);
                Utility.LoggingNonFatalError(apiException);
            }
            catch (Exception unknownException)
            {
                this.mView.UpdateSMRData(selectedSMRAccount.email, selectedSMRAccount.mobileNumber);
                Utility.LoggingNonFatalError(unknownException);
            }

        }

        public async void OnSubmitApplication(string accountNum, string oldEmail, string oldPhoneNum, string newEmail, string newPhoneNum, string terminationReason, string mode)
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
                    lang = LanguageUtil.GetAppLanguage().ToUpper(),
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
                    SMRMode = mode,
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

        public void CheckRequiredFieldsForTerminate(bool isOtherReasonSelected, string otherReason)
        {
            try
            {
                if (isOtherReasonSelected)
                {
                    if (TextUtils.IsEmpty(otherReason))
                    {
                        this.mView.DisableSubmitButton();
                        this.mView.DisableSubmitButton();
                    }
                    else
                    {
                        this.mView.ClearReasonError();
                        this.mView.EnableSubmitButton();
                    }
                }
                else
                {
                    this.mView.ClearReasonError();
                    this.mView.EnableSubmitButton();
                }
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void CheckRequiredFieldsForApply(string mobile_no, string email,bool checkbox)
        {
            try
            {
                bool isError = false;
                if (!TextUtils.IsEmpty(email))
                {
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
                }
                else
                {
                    this.mView.DisableSubmitButton();
                    isError = true;
                }

                if (TextUtils.IsEmpty(mobile_no) || mobile_no.Length < 3 || !mobile_no.Contains("+60"))
                {
                    this.mView.UpdateMobileNumber("+60");
                    this.mView.ClearInvalidMobileError();
                    this.mView.DisableSubmitButton();
                    isError = true;
                }
                else if (mobile_no == "+60")
                {
                    this.mView.UpdateMobileNumber("+60");
                    this.mView.ClearInvalidMobileError();
                    this.mView.DisableSubmitButton();
                    isError = true;
                }
                else if (mobile_no.Contains("+60") && mobile_no.IndexOf("+60") > 0)
                {
                    mobile_no = mobile_no.Substring(mobile_no.IndexOf("+60"));
                    if (mobile_no == "+60")
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
                }
                else
                {
                    if (!Utility.IsValidMobileNumber(mobile_no))
                    {
                        this.mView.ShowInvalidMobileNoError();
                        this.mView.DisableSubmitButton();
                        isError = true;
                    }
                    else
                    {
                        this.mView.ClearInvalidMobileError();
                    }
                }

                if (isError == true)
                {
                    return;
                }

                this.mView.ClearErrors();
                if (checkbox)
                    {
                        this.mView.EnableSubmitButton();
                    }
                    else
                    {
                        this.mView.DisableSubmitButton();
                    }
                //this.mView.EnableSubmitButton();

            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }
    }
}
