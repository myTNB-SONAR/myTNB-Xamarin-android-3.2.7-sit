﻿using System;
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

namespace myTNB_Android.Src.SSMRMeterHistory.MVP
{
    public class SSMRMeterHistoryPresenter : SSMRMeterHistoryContract.IPresenter
    {
        SSMRMeterHistoryContract.IView mView;
        SMRregistrationApi api;
        CancellationTokenSource cts;
        public SSMRMeterHistoryPresenter(SSMRMeterHistoryContract.IView view)
        {
            mView = view;
            api = new SMRregistrationApiImpl();
        }

        public async void CheckSMRAccountEligibility(List<SMRAccount> smrAccountList)
        {
            this.mView.ShowProgressDialog();
            List<SMRAccount> nonSMRTaggedAccountList = smrAccountList.FindAll(account =>
            {
                return !account.isTaggedSMR;
            });

            List<string> accountList = new List<string>();
            nonSMRTaggedAccountList.ForEach(account =>
            {
                accountList.Add(account.accountNumber);
            });
            try
            {
                ServicePointManager.ServerCertificateValidationCallback += SSLFactoryHelper.CertificateValidationCallBack;
                List<SMRAccount> eligibleList = new List<SMRAccount>();

                GetAccountsSMREligibilityResponse response = await this.api.GetAccountsSMREligibility(new GetAccountListSMREligibilityRequest(accountList));

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

                    //Mock Data - START
                    List<SMREligibiltyPopUpDetails> popupDetailList = new List<SMREligibiltyPopUpDetails>();
                    SMREligibiltyPopUpDetails detail = new SMREligibiltyPopUpDetails();
                    detail.Title = "Why are some of my accounts not here?";
                    detail.Description = "We took the liberty to only show you electricity accounts that are eligble for the Self Meter Reading service. <a style=\"text-decoration:none\" href =\"faqid={B8EBBADE-0918-43B7-8093-BB2B19614033}\">Click here</a> to learn more about its eligibility.";
                    detail.CTA = "Got It!";
                    detail.Type = "Not_SMR_CA";
                    popupDetailList.Add(detail);
                    //Mock Data - END
                    response.Response.Data.SMREligibiltyPopUpDetailList = popupDetailList;

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
                    lang = Constants.DEFAULT_LANG.ToUpper(),
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

                SMRActivityInfoResponse SMRAccountActivityInfoResponse = await ssmrAccountAPI.GetSMRAccountActivityInfo(new SMRAccountActivityInfoRequest()
                {
                    AccountNumber = accountNum,
                    IsOwnedAccount = "true",
                    userInterface = currentUsrInf
                }, cts.Token);


                if (SMRAccountActivityInfoResponse.Response.ErrorCode == "7200")
                {
                    this.mView.HideProgressDialog();
                    this.mView.UpdateUIForSMR(SMRAccountActivityInfoResponse);
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

        public List<SMRAccount> GetEligibleSMRAccountList()
        {
            List<CustomerBillingAccount> eligibleSMRAccountList = CustomerBillingAccount.GetEligibleAndSMRAccountList();
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
                smrEligibleAccountList.Add(smrEligibleAccount);
            });
            return smrEligibleAccountList;
        }
    }
}
