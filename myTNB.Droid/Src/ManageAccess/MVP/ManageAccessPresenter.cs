﻿using Android.App;
using Android.Content;
using Android.Runtime;
using myTNB.SQLite.SQLiteDataManager;
using myTNB_Android.Src.Base;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.ManageCards.Models;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.MyTNBService.Request;
using myTNB_Android.Src.MyTNBService.Response;
using myTNB_Android.Src.MyTNBService.ServiceImpl;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;

namespace myTNB_Android.Src.ManageAccess.MVP
{
    internal class ManageAccessPresenter : ManageAccessContract.IUserActionsListener
    {

        private ManageAccessContract.IView mView;
        AccountData accountData;
        int position;
        public ManageAccessPresenter(ManageAccessContract.IView mView, AccountData accountData)
        {
            this.mView = mView;
            this.mView.SetPresenter(this);
            this.accountData = accountData;
        }

        public void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            try
            {
                if (requestCode == Constants.MANAGE_SUPPLY_ACCOUNT_REQUEST)
                {
                    if (resultCode == Result.Ok)
                    {

                        this.mView.ClearAccountsAdapter();
                        List<UserManageAccessAccount> customerAccountList = UserManageAccessAccount.List(accountData.AccountNum);
                        if (customerAccountList != null && customerAccountList.Count > 0)
                        {
                            this.mView.ShowAccountList(customerAccountList);
                        }
                        else
                        {
                            this.mView.ShowEmptyAccount();
                        }
                        if (data != null && data.HasExtra(Constants.ACCOUNT_REMOVED_FLAG) && data.GetBooleanExtra(Constants.ACCOUNT_REMOVED_FLAG, false))
                        {
                            this.mView.ShowAccountRemovedSuccess();
                        }
                    }
                }
                else if (requestCode == Constants.ADD_USER)
                {
                    if (resultCode == Result.Ok)
                    {
                        this.mView.ShowAddNewUserEmailExistSuccess();
                    }
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }


        }

        public async void OnRemoveAccount(string AccountNum)
        {

            UserEntity user = UserEntity.GetActive();
            try
            {
                var removeAccountResponse = await ServiceApiImpl.Instance.RemoveAccount(new RemoveAccountRequest(AccountNum));

                if (removeAccountResponse.IsSuccessResponse())
                {
                    bool isSelectedAcc = false;
                    if (CustomerBillingAccount.HasSelected())
                    {
                        if (CustomerBillingAccount.GetSelected() != null &&
                            CustomerBillingAccount.GetSelected().AccNum.Equals(AccountNum))
                        {
                            isSelectedAcc = true;
                        }
                    }

                    CustomerBillingAccount.Remove(AccountNum);
                    if (isSelectedAcc && CustomerBillingAccount.HasItems())
                    {
                        /**Since Summary dashBoard logic is changed these codes where commented on 01-11-2018**/
                        //CustomerBillingAccount customerBillingAccount = CustomerBillingAccount.GetFirst();
                        //if (customerBillingAccount != null) {
                        //    CustomerBillingAccount.Update(customerBillingAccount.AccNum, true);
                        //}
                        /**Since Summary dashBoard logic is changed these codes where commented on 01-11-2018**/

                        CustomerBillingAccount.MakeFirstAsSelected();
                    }
                    //this.mView.ShowSuccessRemovedAccount();
                }
                else
                {
                    this.mView.ShowErrorMessageResponse(removeAccountResponse.Response.DisplayMessage);
                }
            }
            catch (System.OperationCanceledException e)
            {
                if (mView.IsActive())
                {
                    //this.mView.HideRemoveProgress();
                }
                // ADD OPERATION CANCELLED HERE
                this.mView.ShowRetryOptionsCancelledException(e);
                Utility.LoggingNonFatalError(e);
            }
            catch (ApiException apiException)
            {
                if (mView.IsActive())
                {
                    //this.mView.HideRemoveProgress();
                }
                // ADD HTTP CONNECTION EXCEPTION HERE
                this.mView.ShowRetryOptionsApiException(apiException);
                Utility.LoggingNonFatalError(apiException);
            }
            catch (Exception e)
            {
                if (mView.IsActive())
                {
                    //this.mView.HideRemoveProgress();
                }
                // ADD UNKNOWN EXCEPTION HERE
                this.mView.ShowRetryOptionsUnknownException(e);
                Utility.LoggingNonFatalError(e);
            }

        }

        public async void OnRemoveAccount(AccountData accountData)
        {
            if (mView.IsActive())
            {
                this.mView.ShowProgressDialog();
            }

            UserEntity user = UserEntity.GetActive();
            try
            {
                var removeAccountResponse = await ServiceApiImpl.Instance.RemoveAccount(new RemoveAccountRequest(accountData.AccountNum));

                if (mView.IsActive())
                {
                    this.mView.HideShowProgressDialog();
                }

                if (removeAccountResponse.IsSuccessResponse())
                {
                    bool isSelectedAcc = false;
                    if (CustomerBillingAccount.HasSelected())
                    {
                        if (CustomerBillingAccount.GetSelected() != null &&
                            CustomerBillingAccount.GetSelected().AccNum.Equals(accountData.AccountNum))
                        {
                            isSelectedAcc = true;
                        }
                    }

                    CustomerBillingAccount.Remove(accountData.AccountNum);
                    if (isSelectedAcc && CustomerBillingAccount.HasItems())
                    {
                        /**Since Summary dashBoard logic is changed these codes where commented on 01-11-2018**/
                        //CustomerBillingAccount customerBillingAccount = CustomerBillingAccount.GetFirst();
                        //if (customerBillingAccount != null) {
                        //    CustomerBillingAccount.Update(customerBillingAccount.AccNum, true);
                        //}
                        /**Since Summary dashBoard logic is changed these codes where commented on 01-11-2018**/

                        CustomerBillingAccount.MakeFirstAsSelected();
                    }
                    SMUsageHistoryEntity.RemoveAccountData(accountData.AccountNum);
                    UsageHistoryEntity.RemoveAccountData(accountData.AccountNum);
                    BillHistoryEntity.RemoveAccountData(accountData.AccountNum);
                    PaymentHistoryEntity.RemoveAccountData(accountData.AccountNum);
                    REPaymentHistoryEntity.RemoveAccountData(accountData.AccountNum);
                    AccountDataEntity.RemoveAccountData(accountData.AccountNum);
                    SummaryDashBoardAccountEntity.RemoveAll();
                    MyTNBAccountManagement.GetInstance().RemoveCustomerBillingDetails();
                    HomeMenuUtils.ResetAll();

                    this.mView.ClearAccountsAdapter();
                    List<UserManageAccessAccount> customerAccountList = new List<UserManageAccessAccount>();
                    if (customerAccountList != null && customerAccountList.Count > 0)
                    {
                        this.mView.ShowAccountList(customerAccountList);
                        this.mView.ShowAccountRemovedSuccess();
                    }
                    else
                    {
                        this.mView.ShowEmptyAccount();
                    }
                }
                else
                {
                    this.mView.ShowErrorMessageResponse(removeAccountResponse.Response.DisplayMessage);
                }
            }
            catch (System.OperationCanceledException e)
            {
                if (mView.IsActive())
                {
                    this.mView.HideShowProgressDialog();
                }
                // ADD OPERATION CANCELLED HERE
                this.mView.ShowRetryOptionsCancelledException(e);
                Utility.LoggingNonFatalError(e);
            }
            catch (ApiException apiException)
            {
                if (mView.IsActive())
                {
                    this.mView.HideShowProgressDialog();
                }
                // ADD HTTP CONNECTION EXCEPTION HERE
                this.mView.ShowRetryOptionsApiException(apiException);
                Utility.LoggingNonFatalError(apiException);
            }
            catch (Exception e)
            {
                if (mView.IsActive())
                {
                    this.mView.HideShowProgressDialog();
                }
                // ADD UNKNOWN EXCEPTION HERE
                this.mView.ShowRetryOptionsUnknownException(e);
                Utility.LoggingNonFatalError(e);
            }

        }

        public async void OnAddLogUserAccess(AccountData accountData)
        {
            if (mView.IsActive())
            {
                this.mView.ShowProgressDialog();
            }

            UserEntity user = UserEntity.GetActive();
            try
            {
                LogUserAccessResponse logUserAccessResponse = await ServiceApiImpl.Instance.GetAccountActivityLogList(new LogUserAccessRequest(accountData.AccountNum));

                if (mView.IsActive())
                {
                    this.mView.HideShowProgressDialog();
                }

                if (logUserAccessResponse.IsSuccessResponse())
                {

                }
                else
                {
                    this.mView.ShowErrorMessageResponse(logUserAccessResponse.Response.DisplayMessage);
                }
            }
            catch (System.OperationCanceledException e)
            {
                if (mView.IsActive())
                {
                    this.mView.HideShowProgressDialog();
                }
                // ADD OPERATION CANCELLED HERE
                this.mView.ShowRetryOptionsCancelledException(e);
                Utility.LoggingNonFatalError(e);
            }
            catch (ApiException apiException)
            {
                if (mView.IsActive())
                {
                    this.mView.HideShowProgressDialog();
                }
                // ADD HTTP CONNECTION EXCEPTION HERE
                this.mView.ShowRetryOptionsApiException(apiException);
                Utility.LoggingNonFatalError(apiException);
            }
            catch (Exception e)
            {
                if (mView.IsActive())
                {
                    this.mView.HideShowProgressDialog();
                }
                // ADD UNKNOWN EXCEPTION HERE
                this.mView.ShowRetryOptionsUnknownException(e);
                Utility.LoggingNonFatalError(e);
            }

        }

        public void OnAddAccount()
        {
            this.mView.ShowAddAccount();
        }

        public void Start()
        {
            try
            {
                ServicePointManager.ServerCertificateValidationCallback += SSLFactoryHelper.CertificateValidationCallBack;
                if (UserManageAccessAccount.HasItems())
                {
                    List<UserManageAccessAccount> customerAccountList = UserManageAccessAccount.List(accountData?.AccountNum);
                    List<UserManageAccessAccount> customerAccountDeleteList = UserManageAccessAccount.List(accountData?.AccountNum);
                    //UserManageAccessAccount customerBillingAccount = new UserManageAccessAccount();
                    //customerBillingAccount = UserManageAccessAccount.FindByAccNum(accountData.AccountNum);
                    if (customerAccountList != null && customerAccountList.Count > 0)
                    {
                        this.mView.ShowAccountList(customerAccountList);
                        this.mView.ShowAccountDeleteList(customerAccountDeleteList);
                    }
                    else
                    {
                        this.mView.ShowEmptyAccount();
                    }
                }
                else
                {
                    this.mView.ShowEmptyAccount();
                }

            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

        }
    }
}