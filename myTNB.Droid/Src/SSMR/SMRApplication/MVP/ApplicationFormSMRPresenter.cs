using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Android.Text;
using Android.Util;
using myTNB.Mobile.Business;
using myTNB.AndroidApp.Src.Base.Models;
using myTNB.AndroidApp.Src.Database.Model;
using myTNB.AndroidApp.Src.SSMR.SMRApplication.Api;
using myTNB.AndroidApp.Src.SSMRMeterHistory.Api;
using myTNB.AndroidApp.Src.Utils;
using Newtonsoft.Json;
using Refit;
using static myTNB.AndroidApp.Src.SSMR.SMRApplication.Api.CARegisteredContactInfoResponse;
using static myTNB.AndroidApp.Src.SSMR.SMRApplication.Api.SMRregistrationSubmitResponse;

namespace myTNB.AndroidApp.Src.SSMR.SMRApplication.MVP
{
    public class ApplicationFormSMRPresenter : ApplicationFormSMRContract.IPresenter
    {
        ApplicationFormSMRContract.IView mView;
        SMRregistrationApiImpl api;
        public ApplicationFormSMRPresenter(ApplicationFormSMRContract.IView view)
        {
            mView = view;
            api = new SMRregistrationApiImpl();
        }

        public async void GetCARegisteredContactInfoAsync(SMRAccount smrAccount)
        {
            try
            {
                ServicePointManager.ServerCertificateValidationCallback += SSLFactoryHelper.CertificateValidationCallBack;
                SMRregistrationContactInfoRequest request = new SMRregistrationContactInfoRequest(smrAccount.accountNumber, true);
                var encryptedRequest = myTNB.Mobile.APISecurityManager.Instance.GetEncryptedRequest(request);
                CARegisteredContactInfoResponse response = await api.GetRegisteredContactInfo(encryptedRequest);
                if (response.Data.ErrorCode == "7200")
                {
                    smrAccount.email = response.Data.AccountDetailsData.Email;
                    smrAccount.mobileNumber = response.Data.AccountDetailsData.Mobile;
                }

                mView.UpdateSMRInfo(smrAccount);
            }
            catch (System.OperationCanceledException cancelledException)
            {
                mView.UpdateSMRInfo(smrAccount);
                Utility.LoggingNonFatalError(cancelledException);
            }
            catch (ApiException apiException)
            {
                mView.UpdateSMRInfo(smrAccount);
                Utility.LoggingNonFatalError(apiException);
            }
            catch (Exception unknownException)
            {
                mView.UpdateSMRInfo(smrAccount);
                Utility.LoggingNonFatalError(unknownException);
            }
        }

        public async void SubmitSMRRegistration(SMRAccount smrAccount,string newPhone, string newEmail, string reason)
        {
            try
            {
                ServicePointManager.ServerCertificateValidationCallback += SSLFactoryHelper.CertificateValidationCallBack;
                SMRregistrationSubmitRequest request = new SMRregistrationSubmitRequest(smrAccount.accountNumber, smrAccount.mobileNumber, newPhone,
                    smrAccount.email, newEmail, SUBMIT_MODE.REGISTER, reason);
                var encryptedRequest = myTNB.Mobile.APISecurityManager.Instance.GetEncryptedRequest(request);
                SMRregistrationSubmitResponse response = await api.SubmitSMRApplication(encryptedRequest);
                string jsonResponseString = JsonConvert.SerializeObject(response);
                if (response.Data.ErrorCode == "7200" && response.Data.AccountDetailsData != null)
                {

                    mView.ShowSubmitSuccessResult(jsonResponseString);
                }
                else
                {
                    mView.ShowSubmitFailedResult(jsonResponseString);
                }
            }
            catch (System.OperationCanceledException cancelledException)
            {
                this.mView.HideProgressDialog();
                this.mView.EnableButton();
                Utility.LoggingNonFatalError(cancelledException);
            }
            catch (ApiException apiException)
            {
                this.mView.HideProgressDialog();
                this.mView.EnableButton();
                Utility.LoggingNonFatalError(apiException);
            }
            catch (Exception unknownException)
            {
                this.mView.HideProgressDialog();
                this.mView.EnableButton();
                Utility.LoggingNonFatalError(unknownException);
            }
        }

        public async void CheckSMRAccountEligibility()
        {
            this.mView.ShowProgressDialog();
            List<SMRAccount> list = UserSessions.GetRealSMREligibilityAccountList();
            if (list == null)
            {
                list = UserSessions.GetSMREligibilityAccountList();
            }
            if (list != null && list.Count > 0)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    list[i].accountSelected = false;
                }
            }
            try
            {
                ServicePointManager.ServerCertificateValidationCallback += SSLFactoryHelper.CertificateValidationCallBack;
                if (list != null && list.Count > 0)
                {
                    List<string> accountList = new List<string>();
                    foreach(SMRAccount account in list)
                    {
                        accountList.Add(account.accountNumber);
                    }

                    List<SMRAccount> newList = new List<SMRAccount>();

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

                    var request = new GetAccountListSMREligibilityRequest(accountList);
                    var encryptedRequest = myTNB.Mobile.APISecurityManager.Instance.GetEncryptedRequest(request);
                    GetAccountsSMREligibilityResponse response = await this.api.GetAccountsSMREligibility(encryptedRequest);

                    if (response != null && response.Response != null && response.Response.ErrorCode == "7200" && response.Response.Data.SMREligibilityList.Count > 0)
                    {
                        foreach(var status in response.Response.Data.SMREligibilityList)
                        {
                            if (status.SMREligibility == "true")
                            {
                                SMRAccount selectedAccount = list.Find(x => x.accountNumber == status.ContractAccount);
                                selectedAccount.accountSelected = false;
                                newList.Add(selectedAccount);
                            }
                        }
                        if (newList.Count > 0)
                        {
                            this.mView.HideProgressDialog();
                            UserSessions.SetRealSMREligibilityAccountList(newList);
                        }
                        else
                        {
                            this.mView.HideProgressDialog();
                            UserSessions.SetRealSMREligibilityAccountList(new List<SMRAccount>());
                        }
                    }
                    else
                    {
                        this.mView.HideProgressDialog();
                        UserSessions.SetRealSMREligibilityAccountList(list);
                    }
                }
                else
                {
                    UserSessions.SetRealSMREligibilityAccountList(new List<SMRAccount>());
                    this.mView.HideProgressDialog();
                }
            }
            catch (System.OperationCanceledException cancelledException)
            {
                this.mView.HideProgressDialog();
                if (list != null && list.Count > 0)
                {
                    UserSessions.SetRealSMREligibilityAccountList(list);
                }
                else
                {
                    UserSessions.SetRealSMREligibilityAccountList(new List<SMRAccount>());
                }
                Utility.LoggingNonFatalError(cancelledException);
            }
            catch (ApiException apiException)
            {
                this.mView.HideProgressDialog();
                if (list != null && list.Count > 0)
                {
                    UserSessions.SetRealSMREligibilityAccountList(list);
                }
                else
                {
                    UserSessions.SetRealSMREligibilityAccountList(new List<SMRAccount>());
                }
                Utility.LoggingNonFatalError(apiException);
            }
            catch (Exception unknownException)
            {
                this.mView.HideProgressDialog();
                if (list != null && list.Count > 0)
                {
                    UserSessions.SetRealSMREligibilityAccountList(list);
                }
                else
                {
                    UserSessions.SetRealSMREligibilityAccountList(new List<SMRAccount>());
                }
                Utility.LoggingNonFatalError(unknownException);
            }
        }

        public void CheckRequiredFields(string mobile_no, string email)
        {

            try
            {
                List<SMRAccount> list = UserSessions.GetRealSMREligibilityAccountList();
                if (list == null)
                {
                    list = UserSessions.GetSMREligibilityAccountList();
                }
                SMRAccount sMRAccount = list.Find(smrAccount =>
                {
                    return smrAccount.accountSelected;
                });
                if (!TextUtils.IsEmpty(mobile_no) && !TextUtils.IsEmpty(email) && sMRAccount != null)
                {
                    bool isError = false;
                    if (!Patterns.EmailAddress.Matcher(email).Matches())
                    {
                        this.mView.ShowInvalidEmailError();
                        this.mView.DisableRegisterButton();
                        isError = true;
                    }
                    else
                    {
                        this.mView.ClearEmailError();
                    }

                    if (!Utility.IsValidMobileNumber(mobile_no))
                    {
                        this.mView.ShowInvalidMobileNoError();
                        this.mView.DisableRegisterButton();
                        isError = true;
                    }
                    else
                    {
                        this.mView.ClearInvalidMobileError();

                    }

                    if (isError)
                    {
                        return;
                    }

                    this.mView.ClearErrors();
                    this.mView.EnableRegisterButton();
                }
                else
                {
                    this.mView.DisableRegisterButton();
                }
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }
    }
}
