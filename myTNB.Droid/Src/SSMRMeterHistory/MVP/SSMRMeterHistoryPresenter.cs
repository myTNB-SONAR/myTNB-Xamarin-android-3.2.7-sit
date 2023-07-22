using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using myTNB_Android.Src.Base;
using myTNB_Android.Src.Base.Models;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.myTNBMenu.Api;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.SSMR.SMRApplication.Api;
using myTNB_Android.Src.SSMR.SMRApplication.MVP;
using myTNB_Android.Src.SSMRMeterHistory.Api;
using myTNB_Android.Src.Utils;
using myTNB_Android.Src.myTNBMenu.Requests;
using Refit;
using static myTNB_Android.Src.SSMR.SMRApplication.Api.GetAccountsSMREligibilityResponse;
using myTNB_Android.Src.SSMRTerminate.Api;
using myTNB_Android.Src.NewAppTutorial.MVP;

namespace myTNB_Android.Src.SSMRMeterHistory.MVP
{
    public class SSMRMeterHistoryPresenter : SSMRMeterHistoryContract.IPresenter
    {
        SSMRMeterHistoryContract.IView mView;
        SMRregistrationApi api;
        SSMRTerminateImpl terminationApi;
        CancellationTokenSource cts;

        private bool isSubmitButtonHide = false;

        public SSMRMeterHistoryPresenter(SSMRMeterHistoryContract.IView view)
        {
            mView = view;
            api = new SMRregistrationApiImpl();
            terminationApi = new SSMRTerminateImpl();
        }

        public async void CheckSMRAccountEligibility(List<SMRAccount> smrAccountList)
        {
            List<SMRAccount> nonSMRTaggedAccountList = smrAccountList.FindAll(account =>
            {
                return !account.isTaggedSMR;
            });

            List<string> accountList = new List<string>();
            nonSMRTaggedAccountList.ForEach(account =>
            {
                accountList.Add(account.accountNumber);
            });
            if (accountList.Count == 0)
            {
                this.mView.ShowSMREligibleAccountList(smrAccountList);
            }
            else
            {
                try
                {
                    this.mView.ShowProgressDialog();
                    ServicePointManager.ServerCertificateValidationCallback += SSLFactoryHelper.CertificateValidationCallBack;
                    List<SMRAccount> eligibleList = new List<SMRAccount>();

                    var request = new GetAccountListSMREligibilityRequest(accountList);
                    var encryptedRequest = myTNB.Mobile.APISecurityManager.Instance.GetEncryptedRequest(request);
                    GetAccountsSMREligibilityResponse response = await this.api.GetAccountsSMREligibility(encryptedRequest);

                    if (response != null && response.Response != null && response.Response.ErrorCode == "7200" && response.Response.Data.SMREligibilityList.Count > 0)
                    {
                        response.Response.Data.SMREligibilityList.ForEach(account => {

                            if (account.SMREligibility == "true")
                            {
                                SMRAccount accountEligible = smrAccountList.Find(accountFromList => {
                                    return accountFromList.accountNumber == account.ContractAccount;
                                });
                                eligibleList.Add(accountEligible);
                            }
                        });
                        List<SMRAccount> isTaggedSMRList = smrAccountList.FindAll(account => {
                            return account.isTaggedSMR;
                        });
                        isTaggedSMRList.AddRange(eligibleList);

                        ////Mock Data - START
                        //List<SMREligibiltyPopUpDetails> popupDetailList =  new List<SMREligibiltyPopUpDetails>();
                        //SMREligibiltyPopUpDetails detail = new SMREligibiltyPopUpDetails();
                        //detail.Title = "Why are some of my accounts not here?";
                        //detail.Description = "We took the liberty to only show you electricity accounts that are eligble for the Self Meter Reading service. <a style=\"text-decoration:none\" href =\"faqid={B8EBBADE-0918-43B7-8093-BB2B19614033}\">Click here</a> to learn more about its eligibility.";
                        //detail.CTA = "Got It!";
                        //detail.Type = "Not_SMR_CA";
                        //popupDetailList.Add(detail);
                        ////Mock Data - END
                        //response.Response.Data.SMREligibiltyPopUpDetailList = popupDetailList;
                        MyTNBAppToolTipData.GetInstance().SetSMREligibiltyPopUpDetailList(response.Response.Data.SMREligibiltyPopUpDetailList);

                        this.mView.ShowSMREligibleAccountList(isTaggedSMRList);
                        this.mView.HideProgressDialog();
                    }
                    else
                    {
                        this.mView.HideProgressDialog();
                        this.mView.ShowGenericSnackbarException();
                    }
                }
                catch (System.OperationCanceledException cancelledException)
                {
                    this.mView.HideProgressDialog();
                    this.mView.ShowGenericSnackbarException();
                    Utility.LoggingNonFatalError(cancelledException);
                }
                catch (ApiException apiException)
                {
                    this.mView.HideProgressDialog();
                    this.mView.ShowGenericSnackbarException();
                    Utility.LoggingNonFatalError(apiException);
                }
                catch (Exception unknownException)
                {
                    this.mView.HideProgressDialog();
                    this.mView.ShowGenericSnackbarException();
                    Utility.LoggingNonFatalError(unknownException);
                }
            }
        }


        public async void GetSSMRAccountStatus(string accountNum)
        {
            this.mView.ShowProgressDialog();
            try
            {
                cts = new CancellationTokenSource();

                ServicePointManager.ServerCertificateValidationCallback += SSLFactoryHelper.CertificateValidationCallBack;
                UserEntity user = UserEntity.GetActive();
                UserInterface currentUsrInf = new UserInterface()
                {
                    eid = UserEntity.GetActive().Email,
                    sspuid = UserEntity.GetActive().UserID,
                    did = "",
                    ft = FirebaseTokenEntity.GetLatest().FBToken,
                    lang = LanguageUtil.GetAppLanguage().ToUpper(),
                    sec_auth_k1 = Constants.APP_CONFIG.API_KEY_ID,
                    sec_auth_k2 = "",
                    ses_param1 = "",
                    ses_param2 = ""
                };

#if DEBUG
                var httpClient = new HttpClient(new HttpLoggingHandler(/*new NativeMessageHandler()*/)) { BaseAddress = new Uri(Constants.SERVER_URL.END_POINT) };
                var ssmrAccountAPI = RestService.For<ISMRAccountActivityInfoApi>(httpClient);

#else
            var ssmrAccountAPI = RestService.For<ISMRAccountActivityInfoApi>(Constants.SERVER_URL.END_POINT);
#endif

                var request = new SMRAccountActivityInfoRequest()
                {
                    AccountNumber = accountNum,
                    IsOwnedAccount = "true",
                    userInterface = currentUsrInf
                };

                var encryptedRequest = myTNB.Mobile.APISecurityManager.Instance.GetEncryptedRequest(request);
                SMRActivityInfoResponse SMRAccountActivityInfoResponse = await ssmrAccountAPI.GetSMRAccountActivityInfo(encryptedRequest, cts.Token);

                if (SMRAccountActivityInfoResponse.Response.ErrorCode == "7200")
                {
                    this.mView.HideProgressDialog();
                    SMRPopUpUtils.OnSetSMRActivityInfoResponse(SMRAccountActivityInfoResponse);
                    MyTNBAppToolTipData.SetSMRActivityInfo(SMRAccountActivityInfoResponse.Response);
                    this.mView.UpdateUIForSMR(SMRAccountActivityInfoResponse);
                    CheckIsBtnSubmitHide(SMRAccountActivityInfoResponse);
                    this.mView.OnShowSMRMeterReadingDialog();
                }
                else
                {
                    this.mView.HideProgressDialog();
                    this.mView.ShowRefreshScreen(true);
                }
            }
            catch (System.OperationCanceledException e)
            {
                this.mView.HideProgressDialog();
                this.mView.ShowGenericSnackbarException();
                Utility.LoggingNonFatalError(e);
            }
            catch (ApiException apiException)
            {
                this.mView.HideProgressDialog();
                this.mView.ShowGenericSnackbarException();
                Utility.LoggingNonFatalError(apiException);
            }
            catch (Exception e)
            {
                this.mView.HideProgressDialog();
                this.mView.ShowGenericSnackbarException();
                Utility.LoggingNonFatalError(e);
            }
        }

        public async void GetCARegisteredContactInfoAsync(AccountData selectedAccount)
        {
            this.mView.ShowProgressDialog();
            try
            {
                CustomerBillingAccount account = CustomerBillingAccount.FindByAccNum(selectedAccount.AccountNum);
                ServicePointManager.ServerCertificateValidationCallback += SSLFactoryHelper.CertificateValidationCallBack;
                UserInterface currentUsrInf = new UserInterface()
                {
                    eid = UserEntity.GetActive().Email,
                    sspuid = UserEntity.GetActive().UserID,
                    did = "",
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
                    ICNumber = UserEntity.GetActive().IdentificationNo,
                    usrInf = currentUsrInf
                });

                if (response != null && response.Data != null && response.Data.ErrorCode == "7200")
                {
                    CAContactDetailsModel contactDetailsModel = new CAContactDetailsModel();
                    contactDetailsModel.email = response.Data.AccountDetailsData.Email;
                    contactDetailsModel.mobile = response.Data.AccountDetailsData.Mobile;
                    contactDetailsModel.isAllowEdit = response.Data.AccountDetailsData.isAllowEdit;
                    this.mView.ShowEnableDisableSMR(contactDetailsModel);
                }
                else if (response != null && response.Data != null && response.Data.ErrorCode == "7205" && this.mView.GetSMRActionKey() == Constants.SMR_ENABLE_FLAG)
                {
                    this.mView.EnableButton();
                    this.mView.ShowContactNotAvailableTooltip(response.Data.DisplayTitle, response.Data.DisplayMessage, response.Data.RefreshBtnText);
                }
                else
                {
                    this.mView.EnableButton();
                    if (this.mView.GetSMRActionKey() == Constants.SMR_ENABLE_FLAG)
                    {
                        this.mView.ShowContactNotAvailableTooltip(null, null, null);
                    }
                }
                this.mView.HideProgressDialog();
            }
            catch (System.OperationCanceledException cancelledException)
            {
                this.mView.HideProgressDialog();
                this.mView.EnableButton();
                if (this.mView.GetSMRActionKey() == Constants.SMR_ENABLE_FLAG)
                {
                    this.mView.ShowContactNotAvailableTooltip(null, null, null);
                }
                Utility.LoggingNonFatalError(cancelledException);
            }
            catch (ApiException apiException)
            {
                this.mView.HideProgressDialog();
                this.mView.EnableButton();
                if (this.mView.GetSMRActionKey() == Constants.SMR_ENABLE_FLAG)
                {
                    this.mView.ShowContactNotAvailableTooltip(null, null, null);
                }
                Utility.LoggingNonFatalError(apiException);
            }
            catch (Exception unknownException)
            {
                this.mView.HideProgressDialog();
                this.mView.EnableButton();
                if (this.mView.GetSMRActionKey() == Constants.SMR_ENABLE_FLAG)
                {
                    this.mView.ShowContactNotAvailableTooltip(null, null, null);
                }
                Utility.LoggingNonFatalError(unknownException);
            }

        }

        public List<SMRAccount> GetEligibleSMRAccountList()
        {
            List<CustomerBillingAccount> eligibleSMRAccountList = CustomerBillingAccount.GetEligibleAndSMRAccountList();

            if (MyTNBAccountManagement.GetInstance().IsSMROpenToTenant())
            {
                List<CustomerBillingAccount> eligibleSMRAccountListWithTenant = CustomerBillingAccount.GetEligibleAndSMRAccountListWithTenant();
                List<CustomerBillingAccount> NewEligibleSMRAccountListWithTenant = new List<CustomerBillingAccount>();
                if (eligibleSMRAccountListWithTenant != null && eligibleSMRAccountListWithTenant.Count > 0)
                {
                    foreach (var ca in eligibleSMRAccountListWithTenant)
                    {
                        if (ca.IsTaggedSMR)
                        {
                            NewEligibleSMRAccountListWithTenant.Add(ca);
                        }
                    }
                }
                eligibleSMRAccountList.AddRange(NewEligibleSMRAccountListWithTenant);
            }

            List<SMRAccount> smrEligibleAccountList = new List<SMRAccount>();
            SMRAccount smrEligibleAccount;
            eligibleSMRAccountList.ForEach(account =>
            {
                smrEligibleAccount = new SMRAccount();
                smrEligibleAccount.accountNumber = account.AccNum;
                smrEligibleAccount.accountName = account.AccDesc;
                smrEligibleAccount.accountSelected = account.IsSelected;
                smrEligibleAccount.isTaggedSMR = account.IsTaggedSMR;
                smrEligibleAccount.accountAddress = account.AccountStAddress;
                smrEligibleAccount.accountOwnerName = account.OwnerName;
                smrEligibleAccount.IsTenant = account.isOwned ? false:true;
                smrEligibleAccountList.Add(smrEligibleAccount);
            });
            return smrEligibleAccountList;
        }

        public void CheckIsBtnSubmitHide(SMRActivityInfoResponse SMRAccountActivityInfoResponse)
        {
            if (SMRAccountActivityInfoResponse.Response.Data.DashboardCTAType == Constants.SMR_SUBMIT_METER_KEY && SMRAccountActivityInfoResponse.Response.Data.isCurrentPeriodSubmitted == "false"
                && SMRAccountActivityInfoResponse.Response.Data.isDashboardCTADisabled == "false")
            {
                isSubmitButtonHide = false;
            }
            else
            {
                isSubmitButtonHide = true;
            }
        }

        public List<NewAppModel> OnGeneraNewAppTutorialList(bool isSMR)
        {
            List<NewAppModel> newList = new List<NewAppModel>();

            newList.Add(new NewAppModel()
            {
                ContentShowPosition = ContentType.TopLeft,
                ContentTitle = Utility.GetLocalizedLabel("SSMRReadingHistory", "tutorialHeaderTitle"),
                ContentMessage = Utility.GetLocalizedLabel("SSMRReadingHistory", "tutorialHeaderDesc"),
                ItemCount = isSubmitButtonHide ? 1 : 0,
                DisplayMode = isSMR? "SMR" : "NONSMR",
                IsButtonShow = false
            });

            return newList;
        }
    }
}
