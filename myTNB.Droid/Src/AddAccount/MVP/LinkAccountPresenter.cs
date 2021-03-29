using Android.Text;
using Android.Util;
using myTNB_Android.Src.AddAccount.Models;
using myTNB_Android.Src.AddAccount.Requests;
using myTNB_Android.Src.Base;
using myTNB_Android.Src.Base.Api;
using myTNB_Android.Src.Base.Models;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.MyTNBService.Request;
using myTNB_Android.Src.MyTNBService.Response;
using myTNB_Android.Src.MyTNBService.ServiceImpl;
using myTNB_Android.Src.SummaryDashBoard.Models;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;
using Refit;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;

namespace myTNB_Android.Src.AddAccount.MVP
{
    public class LinkAccountPresenter : LinkAccountContract.IUserActionsListener
    {
        private static string TAG = "AddAccountPresenter";
        private LinkAccountContract.IView mView;

        public LinkAccountPresenter(LinkAccountContract.IView mView)
        {
            this.mView = mView;
            this.mView.SetPresenter(this);
        }


        public void Start()
        {
            Log.Debug(TAG, "Start Called");
            this.mView.ClearAdapter();
        }

        public void OnConfirm(List<NewAccount> newList)
        {
            foreach (NewAccount newAccount in newList)
            {
                CustomerBillingAccount.InsertOrReplace(newAccount, false);
            }
            if (CustomerBillingAccount.HasItems())
            {
                CustomerBillingAccount.RemoveSelected();
                CustomerBillingAccount.MakeFirstAsSelected();
            }
            this.mView.ShowDashboard();
        }

        public void CheckRequiredFields(string total, bool checkbox)
        {

            try
            {
                if (!TextUtils.IsEmpty(total) && (checkbox))
                {
                    if(total.Equals("0"))
                    {
                        this.mView.DisableConfirmButton();
                        return;
                    }
                    this.mView.EnableConfirmButton();
                }
                else
                {
                    this.mView.DisableConfirmButton();
                }
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void GetAccountByIC(string apiKeyID, string currentLinkedAccounts, string userID, string identificationNo)
        {
            this.mView.DisableConfirmButton();
            ServicePointManager.ServerCertificateValidationCallback += SSLFactoryHelper.CertificateValidationCallBack;
            GetCustomerAccountsByIC(apiKeyID, currentLinkedAccounts, userID, identificationNo);
        }

        private async void GetCustomerAccountsByIC(string apiKeyID, string currentAccountList, string email, string identificationNo)
        {
            try
            {
                this.mView.ShowGetAccountsProgressDialog();

                var result = await ServiceApiImpl.Instance.CustomerAccountsForICNum(new CustomerAccountsForICNumRequest(currentAccountList, identificationNo));

                if (!result.IsSuccessResponse())
                {
                    this.mView.HideGetAccountsProgressDialog();
                    this.mView.ShowServiceError(result.Response.DisplayTitle, result.Response.DisplayMessage);
                }
                else
                {
                    this.mView.HideGetAccountsProgressDialog();
                    List<BCRMAccount> bCRMAccountList = new List<BCRMAccount>();
                    foreach (CustomerAccountsForICNumResponse.ResponseData response in result.GetData())
                    {
                        BCRMAccount bCRMAccount = new BCRMAccount();
                        bCRMAccount.accNum = response.accNum;
                        bCRMAccount.accountTypeId = response.accountTypeId;
                        bCRMAccount.accountStAddress = response.accountStAddress;
                        bCRMAccount.icNum = response.icNum;
                        bCRMAccount.isOwned = response.isOwned;
                        bCRMAccount.accountCategoryId = response.accountCategoryId;
                        bCRMAccountList.Add(bCRMAccount);
                    }
                    this.mView.ShowBCRMAccountList(bCRMAccountList);

                }
            }
            catch (System.OperationCanceledException cancelledException)
            {
                mView.HideGetAccountsProgressDialog();
                this.mView.ShowErrorMessage();
                Utility.LoggingNonFatalError(cancelledException);
            }
            catch (ApiException apiException)
            {
                mView.HideGetAccountsProgressDialog();
                this.mView.ShowErrorMessage();
                Utility.LoggingNonFatalError(apiException);
            }
            catch (Exception unknownException)
            {
                mView.HideGetAccountsProgressDialog();
                this.mView.ShowErrorMessage();
                Utility.LoggingNonFatalError(unknownException);
            }


        }

        public void AddMultipleAccounts(string apiKeyId, string sspUserId, string email, string name, List<Models.AddAccount> accounts)
        {
            try
            {
                ServicePointManager.ServerCertificateValidationCallback += SSLFactoryHelper.CertificateValidationCallBack;
                AddMultipleAccountsAsync(apiKeyId, sspUserId, email, name, accounts);
            }
            catch (Exception unknownException)
            {
                if (mView.IsActive())
                {
                    mView.HideAddingAccountProgressDialog();
                }
                this.mView.ShowErrorMessage();
                Utility.LoggingNonFatalError(unknownException);
            }
        }

        private async void AddMultipleAccountsAsync(string apiKeyId, string sspUserID, string email, string name, List<Models.AddAccount> accounts)
        {
            try
            {
                if (mView.IsActive())
                {
                    mView.ShowAddingAccountProgressDialog();
                }
                //var tempReq = JsonConvert.SerializeObject(accounts);
                AddAccountsResponse result = await ServiceApiImpl.Instance.AddMultipleAccounts_OT(new AddAccountsRequest(accounts,name));

                if (result.IsSuccessResponse())
                {
                    /*if (mView.IsActive())
                    {
                        mView.HideAddingAccountProgressDialog();
                    }*/
                    mView.ShowAddAccountSuccess(result.GetData());
                    MyTNBAccountManagement.GetInstance().RemoveCustomerBillingDetails();
                    HomeMenuUtils.ResetAll();
                }
                else
                {
                    if (mView.IsActive())
                    {
                        mView.HideAddingAccountProgressDialog();
                    }
                    mView.ShowAddAccountFail(result.Response.DisplayMessage);
                }

            }
            catch (System.OperationCanceledException cancelledException)
            {
                if (mView.IsActive())
                {
                    mView.HideAddingAccountProgressDialog();
                }
                this.mView.ShowErrorMessage();
                Utility.LoggingNonFatalError(cancelledException);
            }
            catch (ApiException apiException)
            {
                if (mView.IsActive())
                {
                    mView.HideAddingAccountProgressDialog();
                }
                this.mView.ShowErrorMessage();
                Utility.LoggingNonFatalError(apiException);
            }
            catch (Exception unknownException)
            {
                if (mView.IsActive())
                {
                    mView.HideAddingAccountProgressDialog();
                }
                this.mView.ShowErrorMessage();
                Utility.LoggingNonFatalError(unknownException);
            }

        }

        public void InsertingInSummarydashBoard(List<CustomerBillingAccount> customerBillingAccounts)
        {
            try
            {
                SummaryDashBordRequest summaryDashBoardRequest = new SummaryDashBordRequest();
                List<string> account = new List<string>();

                foreach (var customer in customerBillingAccounts)
                {
                    account.Add(customer.AccNum);
                }


                UserEntity user = UserEntity.GetActive();
                FirebaseTokenEntity token = FirebaseTokenEntity.GetLatest();

                summaryDashBoardRequest.AccNum = account;
                UserInterface currentUsrInf = new UserInterface()
                {
                    eid = user.Email,
                    sspuid = user.UserID,
                    did = this.mView.GetDeviceId(),
                    ft = token.FBToken,
                    lang = LanguageUtil.GetAppLanguage().ToUpper(),
                    sec_auth_k1 = Constants.APP_CONFIG.API_KEY_ID,
                    sec_auth_k2 = "",
                    ses_param1 = "",
                    ses_param2 = ""
                };
                summaryDashBoardRequest.usrInf = currentUsrInf;

                CallSummaryAPI(summaryDashBoardRequest);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        private async void CallSummaryAPI(SummaryDashBordRequest summaryDashBoardRequest)
        {
            await SummaryDashBoardApiCall.GetSummaryDetails(summaryDashBoardRequest);
        }
        public void NavigateToTermsAndConditions()
        {
            this.mView.ShowTermsAndConditions();
        }
    }
}
