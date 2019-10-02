using Android.Util;
using myTNB_Android.Src.AddAccount.Api;
using myTNB_Android.Src.AddAccount.Models;
using myTNB_Android.Src.AddAccount.Requests;
using myTNB_Android.Src.Base;
using myTNB_Android.Src.Base.Api;
using myTNB_Android.Src.Base.Models;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.SummaryDashBoard.Models;
using myTNB_Android.Src.Utils;
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

        public void GetAccounts(string apiID, string userID)
        {
            ServicePointManager.ServerCertificateValidationCallback += SSLFactoryHelper.CertificateValidationCallBack;
            GetCustomerAccounts(apiID, userID);
        }

        private async void GetCustomerAccounts(string apiID, string userId)
        {
            if (mView.IsActive())
            {
                this.mView.ShowGetAccountsProgressDialog();
            }
            var api = RestService.For<GetCustomerAccounts>(Constants.SERVER_URL.END_POINT);
            try
            {
                UserEntity user = UserEntity.GetActive();
                var newObject = new
                            {
                                usrInf = new
                                {
                                    eid = user.UserName,
                                    sspuid = user.UserID,
                                    lang = "EN",
                                    sec_auth_k1 = Constants.APP_CONFIG.API_KEY_ID,
                                    sec_auth_k2 = "test",
                                    ses_param1 = "test",
                                    ses_param2 = "test"
                                }
                            };
                var result = await api.GetCustomerAccountV6(newObject); //GetCustomerAccountV5(new GetCustomerAccountsRequest(apiID, user.UserID));
                if (mView.IsActive())
                {
                    this.mView.HideGetAccountsProgressDialog();
                }
                this.mView.ShowAccountList(result.D.AccountListData);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

        }

        public void OnConfirm(List<NewAccount> newList)
        {
            int ctr = 0;
            foreach (NewAccount newAccount in newList)
            {
                bool isSelected = ctr == 0 ? true : false;
                CustomerBillingAccount.InsertOrReplace(newAccount, isSelected);
                ctr++;
            }

            this.mView.ShowDashboard();
        }

        public void GetAccountByIC(string apiKeyID, string currentLinkedAccounts, string userID, string identificationNo)
        {
            ServicePointManager.ServerCertificateValidationCallback += SSLFactoryHelper.CertificateValidationCallBack;
            GetCustomerAccountsByIC(apiKeyID, currentLinkedAccounts, userID, identificationNo);
        }

        private async void GetCustomerAccountsByIC(string apiKeyID, string currentAccountList, string email, string identificationNo)
        {
            try
            {
                if (mView.IsActive())
                {
                    this.mView.ShowGetAccountsProgressDialog();
                }
                var api = RestService.For<GetCustomerAccountsForICNumApi>(Constants.SERVER_URL.END_POINT);
                // TODO : UPDATE TO V5
                var result = await api.GetCustomerAccountByIc(new GetBCRMAccountRequest(apiKeyID, currentAccountList, email, identificationNo));

                if (result.Data.IsError)
                {
                    if (mView.IsActive())
                    {
                        this.mView.HideGetAccountsProgressDialog();
                    }
                    if (result.Data.Status.Equals("failed"))
                    {
                        this.mView.ShowBCRMDownException(result.Data.Message);
                    }
                    else
                    {
                        Exception e = new Exception();
                        this.mView.ShowRetryOptionsUnknownException(e);
                    }
                }
                else
                {
                    if (mView.IsActive())
                    {
                        this.mView.HideGetAccountsProgressDialog();
                    }
                    this.mView.ShowBCRMAccountList(result.Data.BCRMAccountList);

                }
            }
            catch (System.OperationCanceledException cancelledException)
            {
                if (mView.IsActive())
                {
                    mView.HideGetAccountsProgressDialog();
                }
                this.mView.ShowErrorMessage();
                Utility.LoggingNonFatalError(cancelledException);
            }
            catch (ApiException apiException)
            {
                if (mView.IsActive())
                {
                    mView.HideGetAccountsProgressDialog();
                }
                this.mView.ShowErrorMessage();
                Utility.LoggingNonFatalError(apiException);
            }
            catch (Exception unknownException)
            {
                if (mView.IsActive())
                {
                    mView.HideGetAccountsProgressDialog();
                }
                this.mView.ShowErrorMessage();
                Utility.LoggingNonFatalError(unknownException);
            }


        }

        public void AddMultipleAccounts(string apiKeyId, string sspUserId, string email, List<Models.AddAccount> accounts)
        {
            ServicePointManager.ServerCertificateValidationCallback += SSLFactoryHelper.CertificateValidationCallBack;
            AddMultipleAccountsAsync(apiKeyId, sspUserId, email, accounts);
        }

        private async void AddMultipleAccountsAsync(string apiKeyId, string sspUserID, string email, List<Models.AddAccount> accounts)
        {
            try
            {
                if (mView.IsActive())
                {
                    mView.ShowAddingAccountProgressDialog();
                }

#if DEBUG
                var httpClient = new HttpClient(new HttpLoggingHandler(/*new NativeMessageHandler()*/)) { BaseAddress = new Uri(Constants.SERVER_URL.END_POINT) };
                var api = RestService.For<AddMultipleAccountsToUserApi>(httpClient);

#else
                var api = RestService.For<AddMultipleAccountsToUserApi>(Constants.SERVER_URL.END_POINT);

#endif


            var reqObject = new
            {
                billAccounts = accounts,
                usrInf = new
                {
                    eid = email,
                    sspuid = sspUserID,
                    lang = "EN",
                    sec_auth_k1 = apiKeyId,
                    sec_auth_k2 = "test",
                    ses_param1 = "test",
                    ses_param2 = "test"
                }
            };
                var result = await api.AddMultipleAccounts(reqObject);

                if (result != null && result.response != null && result.response.ErrorCode == "7200")
                {
                    if (mView.IsActive())
                    {
                        mView.HideAddingAccountProgressDialog();
                    }
                    mView.ShowAddAccountFail(result.response.Message);
                    mView.ShowAddAccountSuccess(result.response);
                    MyTNBAccountManagement.GetInstance().RemoveCustomerBillingDetails();
                }
                else
                {
                    if (mView.IsActive())
                    {
                        mView.HideAddingAccountProgressDialog();
                    }
                    mView.ShowAddAccountFail(result.response.DisplayMessage);
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
                    lang = Constants.DEFAULT_LANG.ToUpper(),
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
    }
}
