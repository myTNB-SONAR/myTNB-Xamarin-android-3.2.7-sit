using Android.Text;
using Android.Util;
using myTNB_Android.Src.AddAccount.Api;
using myTNB_Android.Src.AddAccount.Models;
using myTNB_Android.Src.AddAccount.Requests;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.Utils;
using Refit;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;

namespace myTNB_Android.Src.AddAccount.MVP
{
    public class AddAccountPresenter : AddAccountContract.IUserActionsListener
    {
        private static string TAG = "AddAccountPresenter";
        private AddAccountContract.IView mView;
        private CancellationTokenSource addAccountCts;

        public AddAccountPresenter(AddAccountContract.IView mView)
        {
            this.mView = mView;
            this.mView.SetPresenter(this);
        }


        public void Start()
        {
            //ServicePointManager.ServerCertificateValidationCallback += SSLFactoryHelper.CertificateValidationCallBack;
            //GetCustomerAccounts();
            mView.DisableAddAccountButton();
            Log.Debug(TAG, "Start Called");
        }

        public void AddAccount(string apiKeyId, string userID, string email, string tnbBillAcctNo, string tnbAcctHolderIC, string tnbAcctContractNo, string type, string des, bool isOwner, string suppliedMotherName)
        {
            try
            {
                if (TextUtils.IsEmpty(tnbBillAcctNo))
                {
                    mView.ShowEmptyAccountNumberError();
                    return;
                }

                if (TextUtils.IsEmpty(des))
                {
                    mView.ShowEmptyAccountNickNameError();
                    return;
                }

                if (isOwner)
                {
                    if (TextUtils.IsEmpty(tnbAcctHolderIC))
                    {
                        mView.ShowEmptyOwnerIcNumberError();
                        return;
                    }

                    /*if (TextUtils.IsEmpty(tnbBillAcctNo))
                    {
                        mView.ShowEmptyMothersMaidenNameError();
                        return;
                    }*/
                }

                if (tnbBillAcctNo.Length != 12 && tnbBillAcctNo.Length != 14)
                {
                    mView.ShowInvalidAccountNumberError();
                    //mView.ShowEmptyAccountNumberError();
                    return;
                }

                List<CustomerBillingAccount> accounts = CustomerBillingAccount.List();
                if (accounts != null && accounts.Count > 0 && !TextUtils.IsEmpty(des))
                {
                    foreach (CustomerBillingAccount item in accounts)
                    {
                        if (item.AccDesc.ToLower().Equals(des.ToString().ToLower()))
                        {
                            mView.ShowSameAccountNameError();
                            return;
                        }
                    }
                }

                ServicePointManager.ServerCertificateValidationCallback += SSLFactoryHelper.CertificateValidationCallBack;
                AddAccountAsync(apiKeyId, userID, email, tnbBillAcctNo, tnbAcctHolderIC, tnbAcctContractNo, type, des, isOwner, suppliedMotherName);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        private async void AddAccountAsync(string apiKeyId, string userID, string email, string tnbBillAcctNo, string tnbAcctHolderIC, string tnbAcctContractNo, string type, string des, bool isOwner, string suppliedMotherName)
        {
            addAccountCts = new CancellationTokenSource();
            mView.ShowAddingAccountProgressDialog();

#if DEBUG
            var httpClient = new HttpClient(new HttpLoggingHandler(/*new NativeMessageHandler()*/)) { BaseAddress = new Uri(Constants.SERVER_URL.END_POINT) };
            var api = RestService.For<AddAccountToCustomer>(httpClient);

#else
             var api = RestService.For<AddAccountToCustomer>(Constants.SERVER_URL.END_POINT);
            
#endif

            try
            {

                UserEntity user = UserEntity.GetActive();
                //UserRegister userReg = UserRegister.GetActive();
                var result = await api.AddAccountToCustomer(new AddAccountToCustomerRequest(apiKeyId, user.UserID, user.Email, tnbBillAcctNo, tnbAcctHolderIC, tnbAcctContractNo, type, des, isOwner, suppliedMotherName), addAccountCts.Token);

                if (result.response[0].isError)
                {
                    mView.ShowAddAccountFail(result.response[0].message);
                    if (mView.IsActive())
                    {
                        mView.HideAddingAccountProgressDialog();
                    }
                }
                else
                {
                    mView.ClearText();
                    if (mView.IsActive())
                    {
                        mView.HideAddingAccountProgressDialog();
                    }
                    mView.ShowAddAccountSuccess(result.response[0].message);
                }

            }
            catch (System.OperationCanceledException cancelledException)
            {
                if (mView.IsActive())
                {
                    mView.HideAddingAccountProgressDialog();
                }
                this.mView.ShowRetryOptionsCancelledException(cancelledException);
                Utility.LoggingNonFatalError(cancelledException);
            }
            catch (ApiException apiException)
            {
                if (mView.IsActive())
                {
                    mView.HideAddingAccountProgressDialog();
                }
                this.mView.ShowRetryOptionsApiException(apiException);
                Utility.LoggingNonFatalError(apiException);
            }
            catch (Exception unknownException)
            {
                if (mView.IsActive())
                {
                    mView.HideAddingAccountProgressDialog();
                }
                this.mView.ShowRetryOptionsUnknownException(unknownException);
                Utility.LoggingNonFatalError(unknownException);
            }


        }

        public void ValidateAccount(string apiKeyId, string accountNum, string accountType, string userIdentificationNum, string suppliedMotherName, bool isOwner, string accountLabel)
        {
            try
            {
                if (TextUtils.IsEmpty(accountNum))
                {
                    mView.ShowEmptyAccountNumberError();
                    return;
                }

                //Removed mother's name validation check
                if (isOwner)
                {
                    if (TextUtils.IsEmpty(userIdentificationNum) && accountType.Equals("1"))
                    {
                        mView.ShowEmptyOwnerIcNumberError();
                        return;
                    }
                }

                if (TextUtils.IsEmpty(accountLabel))
                {
                    mView.ShowEmptyAccountNickNameError();
                    return;
                }

                if (!Utility.AddAccountNumberValidation(accountNum.Length))
                {
                    mView.ShowInvalidAccountNumberError();
                    return;
                }


                //if (!Utility.isAlphaNumeric(accountLabel))
                //{
                //    mView.ShowEnterValidAccountName();
                //    return;
                //}



                ServicePointManager.ServerCertificateValidationCallback += SSLFactoryHelper.CertificateValidationCallBack;
                ValidateAccountAsync(apiKeyId, accountNum, accountType, userIdentificationNum, suppliedMotherName, isOwner, accountLabel);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }




        private async void ValidateAccountAsync(string apiKeyId, string accountNum, string accountType, string userIdentificationNum, string suppliedMotherName, bool isOwner, string accountLable)
        {
            addAccountCts = new CancellationTokenSource();
            mView.ShowAddingAccountProgressDialog();

#if DEBUG
            var httpClient = new HttpClient(new HttpLoggingHandler(/*new NativeMessageHandler()*/)) { BaseAddress = new Uri(Constants.SERVER_URL.END_POINT) };
            var api = RestService.For<ValidateManualAccountLinkingApi>(httpClient);

#else
            var api = RestService.For<ValidateManualAccountLinkingApi>(Constants.SERVER_URL.END_POINT);

#endif

            try
            {

                var result = await api.ValidateManualAccount(new ValidateManualAccountRequest(apiKeyId, accountNum, accountType, userIdentificationNum, suppliedMotherName, isOwner));
                if (mView.IsActive())
                {
                    mView.HideAddingAccountProgressDialog();
                }
                if (result.validation.IsError)
                {
                    mView.ShowAddAccountFail(result.validation.Message);
                }
                else
                {
                    mView.ClearText();
                    NewAccount account = new NewAccount();
                    account.accountNumber = accountNum;
                    account.accountAddress = result.validation.Data.accountStAddress;
                    account.isOwner = isOwner;
                    account.type = accountType;
                    account.isRegistered = false;
                    account.accountLabel = accountLable;
                    account.icNum = userIdentificationNum;
                    account.accountCategoryId = result.validation.Data.accountCategoryId;
                    mView.ShowValidateAccountSucess(account);
                }

            }
            catch (System.OperationCanceledException cancelledException)
            {
                if (mView.IsActive())
                {
                    mView.HideAddingAccountProgressDialog();
                }
                this.mView.ShowRetryOptionsCancelledException(cancelledException);
                Utility.LoggingNonFatalError(cancelledException);
            }
            catch (ApiException apiException)
            {
                if (mView.IsActive())
                {
                    mView.HideAddingAccountProgressDialog();
                }
                this.mView.ShowRetryOptionsApiException(apiException);
                Utility.LoggingNonFatalError(apiException);
            }
            catch (Exception unknownException)
            {
                if (mView.IsActive())
                {
                    mView.HideAddingAccountProgressDialog();
                }
                this.mView.ShowRetryOptionsUnknownException(unknownException);
                Utility.LoggingNonFatalError(unknownException);
            }

        }

        public void CheckRequiredFields(string accountno, string accountNickName, bool isOwner, string ownerIC)
        {
            try
            {
                mView.RemoveNameErrorMessage();
                mView.RemoveNumberErrorMessage();
                if (!TextUtils.IsEmpty(accountno) && !TextUtils.IsEmpty(accountNickName))
                {

                    if (isOwner && TextUtils.IsEmpty(ownerIC) || !Utility.AddAccountNumberValidation(accountno.Length))
                    {
                        if (!Utility.AddAccountNumberValidation(accountno.Length))
                        {
                            mView.ShowInvalidAccountNumberError();
                        }
                        mView.DisableAddAccountButton();
                    }
                    else if (!TextUtils.IsEmpty(accountNickName))
                    {
                        List<CustomerBillingAccount> accounts = CustomerBillingAccount.List();
                        bool accountNameExsist = false;
                        if (accounts != null && accounts.Count > 0)
                        {
                            foreach (CustomerBillingAccount item in accounts)
                            {
                                if (!string.IsNullOrEmpty(item.AccDesc) && item.AccDesc.ToLower().Trim().Equals(accountNickName.ToString().ToLower().Trim()))
                                {
                                    accountNameExsist = true;
                                    break;
                                }
                            }
                            if (accountNameExsist)
                            {
                                mView.DisableAddAccountButton();
                                mView.ShowSameAccountNameError();
                            }
                            else
                            {
                                mView.EnableAddAccountButton();
                            }
                        }
                        else
                        {
                            mView.EnableAddAccountButton();
                        }
                    }
                    else
                    {
                        mView.DisableAddAccountButton();
                    }
                }
                else
                {
                    mView.DisableAddAccountButton();
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }



    }
}