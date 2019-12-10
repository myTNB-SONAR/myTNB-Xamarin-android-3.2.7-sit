using Android.Util;
using myTNB_Android.Src.AddAccount.Api;
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
                                    lang = LanguageUtil.GetAppLanguage().ToUpper(),
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

        public void GetAccountByIC(string apiKeyID, string currentLinkedAccounts, string userID, string identificationNo)
        {
            ServicePointManager.ServerCertificateValidationCallback += SSLFactoryHelper.CertificateValidationCallBack;
            GetCustomerAccountsByIC(apiKeyID, currentLinkedAccounts, userID, identificationNo);
        }

        private async void GetCustomerAccountsByIC(string apiKeyID, string currentAccountList, string email, string identificationNo)
        {
            try
            {
                this.mView.ShowGetAccountsProgressDialog();

                var result = await ServiceApiImpl.Instance.CustomerAccountsForICNum(new CustomerAccountsForICNumRequest(currentAccountList, identificationNo));

                if (result.Response != null && result.Response.ErrorCode != Constants.SERVICE_CODE_SUCCESS)
                {
                    this.mView.HideGetAccountsProgressDialog();

                    if (result.Response.Status.Equals("failed"))
                    {
                        this.mView.ShowBCRMDownException(result.Response.Message);
                    }
                    else
                    {
                        Exception e = new Exception();
                        this.mView.ShowRetryOptionsUnknownException(e);
                    }
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

        public void AddMultipleAccounts(string apiKeyId, string sspUserId, string email, List<Models.AddAccount> accounts)
        {
            try
            {
                ServicePointManager.ServerCertificateValidationCallback += SSLFactoryHelper.CertificateValidationCallBack;
                AddMultipleAccountsAsync(apiKeyId, sspUserId, email, accounts);
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
                    lang = LanguageUtil.GetAppLanguage().ToUpper(),
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
                    mView.ShowAddAccountSuccess(result.response);
                    MyTNBAccountManagement.GetInstance().RemoveCustomerBillingDetails();
                    HomeMenuUtils.ResetAll();
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
    }
}
