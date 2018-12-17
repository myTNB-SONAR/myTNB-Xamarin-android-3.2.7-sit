using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Net;
using myTNB_Android.Src.Utils;
using Refit;
using myTNB_Android.Src.AddAccount.Api;
using myTNB_Android.Src.AddAccount.Requests;
using Android.Util;
using myTNB_Android.Src.AddAccount.Models;
using myTNB_Android.Src.Database.Model;
using System.Net.Http;
using myTNB_Android.Src.SummaryDashBoard.Models;
using myTNB_Android.Src.Base.Api;

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
                // TODO : UPDATE TO V5
                var result = await api.GetCustomerAccountV5(new GetCustomerAccountsRequest(apiID, user.UserID));
                //Log.Debug(TAG, " : "+result.response);
                if (mView.IsActive())
                {
                    this.mView.HideGetAccountsProgressDialog();
                }
                this.mView.ShowAccountList(result.D.AccountListData);
            } catch(Exception e) {
                Utility.LoggingNonFatalError(e);
            }
            
        }

        public void OnConfirm(List<NewAccount> newList)
        {
            int ctr = 0;
            foreach (NewAccount newAccount in newList)
            {
                bool isSelected = ctr == 0 ? true : false;
                CustomerBillingAccount.InsertOrReplace(newAccount , isSelected );
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
            try {
                if (mView.IsActive())
                {
                    this.mView.ShowGetAccountsProgressDialog();
                }
            var api = RestService.For<GetCustomerAccountsForICNumApi>(Constants.SERVER_URL.END_POINT);
            // TODO : UPDATE TO V5
            var result = await api.GetCustomerAccountByIc(new GetBCRMAccountRequest(apiKeyID, currentAccountList, email, identificationNo));
            //Log.Debug(TAG, " : "+result.response);

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
            } catch (System.OperationCanceledException cancelledException)
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
            try {
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

           

                var result = await api.AddMultipleAccounts(new AddMultipleAccountRequest(apiKeyId, sspUserID, email, accounts));

                if (result.response.IsError)
                {
                    if (mView.IsActive())
                    {
                        mView.HideAddingAccountProgressDialog();
                    }
                    mView.ShowAddAccountFail(result.response.Message);                    
                }
                else
                {
                    if (mView.IsActive())
                    {
                        mView.HideAddingAccountProgressDialog();
                    }
                    mView.ShowAddAccountSuccess(result.response);
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

                summaryDashBoardRequest.AccNum = account;
                summaryDashBoardRequest.SspUserId = user.UserID;
                summaryDashBoardRequest.ApiKeyId = Constants.APP_CONFIG.API_KEY_ID;

                CallSummaryAPI(summaryDashBoardRequest);
            } catch(Exception e) {
                Utility.LoggingNonFatalError(e);
            }
        }

        private async void CallSummaryAPI(SummaryDashBordRequest summaryDashBoardRequest) {
            await SummaryDashBoardApiCall.GetSummaryDetails(summaryDashBoardRequest);    
        }
    }
}