using System;
using System.Collections.Generic;
using System.Net;
using myTNB_Android.Src.SSMR.SMRApplication.Api;
using myTNB_Android.Src.SSMR.SMRApplication.MVP;
using myTNB_Android.Src.SSMRMeterHistory.Api;
using myTNB_Android.Src.Utils;
using Refit;

namespace myTNB_Android.Src.SSMRMeterHistory.MVP
{
    public class SSMRMeterHistoryPresenter : SSMRMeterHistoryContract.IPresenter
    {
        SSMRMeterHistoryContract.IView mView;
        SMRregistrationApi api;
        public SSMRMeterHistoryPresenter(SSMRMeterHistoryContract.IView view)
        {
            mView = view;
            api = new SMRregistrationApiImpl();
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
                    foreach (SMRAccount account in list)
                    {
                        accountList.Add(account.accountNumber);
                    }

                    List<SMRAccount> newList = new List<SMRAccount>();

                    GetAccountsSMREligibilityResponse response = await this.api.GetAccountsSMREligibility(new GetAccountListSMREligibilityRequest(accountList));

                    if (response != null && response.Response != null && response.Response.ErrorCode == "7200" && response.Response.Data.SMREligibilityList.Count > 0)
                    {
                        foreach (var status in response.Response.Data.SMREligibilityList)
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
                        this.mView.ShowSMREligibleAccountList();
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
    }
}
