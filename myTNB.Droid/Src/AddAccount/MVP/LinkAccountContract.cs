﻿using myTNB_Android.Src.AddAccount.Models;
using myTNB_Android.Src.Base.MVP;
using myTNB_Android.Src.Database.Model;
using Refit;
using System;
using System.Collections.Generic;

namespace myTNB_Android.Src.AddAccount.MVP
{
    public class LinkAccountContract
    {
        public interface IView : IBaseView<IUserActionsListener>
        {
            void ShowNoAccountAddedError(string message);

            //navigte user to add another account screen
            void ShowAddAnotherAccountScreen();

            //show progress dialog 
            void ShowGetAccountsProgressDialog();

            //hide progress dialog
            void HideGetAccountsProgressDialog();

            //show account list from BCRM
            void ShowBCRMAccountList(List<BCRMAccount> response);

            //show error for get customer accounts by NRIC
            void ShowErrorMessage();

            //Clear Adapter
            void ClearAdapter();

            //Show dashboard
            void ShowDashboard();

            /// <summary>
            /// Show add account success with service response
            /// </summary>
            void ShowAddAccountSuccess(AddMultipleAccountResponse.Response response);

            /// <summary>
            /// Show error message for add account response
            /// </summary>
            void ShowAddAccountFail(string errorMessage);

            /// <summary>
            /// Show progress dialog for add account service call
            /// </summary>
            void ShowAddingAccountProgressDialog();

            /// <summary>
            /// Hide progress dialog for add account service call
            /// </summary>
            void HideAddingAccountProgressDialog();

            /// <summary>
            /// 
            /// </summary>
            /// <param name="operationCanceledException"></param>
            void ShowRetryOptionsCancelledException(System.OperationCanceledException operationCanceledException);

            /// <summary>
            /// 
            /// </summary>
            /// <param name="apiException"></param>
            void ShowRetryOptionsApiException(ApiException apiException);

            /// <summary>
            /// 
            /// </summary>
            /// <param name="exception"></param>
            void ShowRetryOptionsUnknownException(Exception exception);

            /// <summary>
            /// 
            /// </summary>
            /// <param name="message"></param>
            void ShowBCRMDownException(String msg);

            string GetDeviceId();
        }

        public interface IUserActionsListener : IBasePresenter
        {
            /// <summary>
            /// Get Customer Accounts by NRIC/ID APi : GetCustomerAccountsForICNum
            /// </summary>
            /// <param name="exception"></param>
            void GetAccountByIC(string apiKeyId, string currentLinkedAccounts, string email, string identificationNo);

            /// <summary>
            /// Called when user click on Confirm button
            /// </summary>
            /// <param name="exception"></param>
            void OnConfirm(List<NewAccount> newList);

            /// <summary>
            /// Add multiple accounts to customers APi : AddMultipleSupplyAccountsToUserReg
            /// </summary>
            /// <param name="exception"></param>
            void AddMultipleAccounts(string apiKeyId, string sspUserId, string email, List<Models.AddAccount> accounts);

            void InsertingInSummarydashBoard(List<CustomerBillingAccount> customerBillingAccounts);

        }
    }
}