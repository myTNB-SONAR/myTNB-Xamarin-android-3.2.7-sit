using Android.Text;
using Android.Util;
using myTNB_Android.Src.AddAccount.Models;
using myTNB_Android.Src.AddAccount.Requests;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.MyTNBService.Request;
using myTNB_Android.Src.MyTNBService.ServiceImpl;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;
using Refit;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using ValidateManualAccountRequest = myTNB_Android.Src.MyTNBService.Request.ValidateManualAccountRequest;

namespace myTNB_Android.Src.AddAccount.MVP
{
    public class AddAccountPresenter : AddAccountContract.IUserActionsListener
    {
        private static string TAG = "AddAccountPresenter";
        private AddAccountContract.IView mView;

        public AddAccountPresenter(AddAccountContract.IView mView)
        {
            this.mView = mView;
            this.mView.SetPresenter(this);
        }


        public void Start()
        {
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
            mView.ShowAddingAccountProgressDialog();

            try
            {
                UserEntity user = UserEntity.GetActive();
                var result = await ServiceApiImpl.Instance.AddAccountToCustomer(new AccountToCustomerRequest(tnbBillAcctNo, tnbAcctHolderIC, tnbAcctContractNo, des, isOwner.ToString(), suppliedMotherName, Convert.ToInt32(type)));

                if (result.Response != null && result.Response.ErrorCode != Constants.SERVICE_CODE_SUCCESS)
                {
                    mView.ShowAddAccountFail(result.Response.DisplayMessage);
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
                    mView.ShowAddAccountSuccess(result.Response.DisplayMessage);
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

        public bool ValidateEditText(string accountno, string accountNickName)
        {
            try
            {
                bool isCorrect = true;

                this.mView.DisableAddAccountButton();

                if (!string.IsNullOrEmpty(accountno))
                {
                   this.mView.RemoveNumberErrorMessage();
                }
                else
                {  
                    this.mView.ShowInvalidAccountNumberError();
                    isCorrect = false;
                }

                if (!string.IsNullOrEmpty(accountNickName))
                {
                    this.mView.ClearNameHint();
                }
                else
                {
                    //disable button if no text
                    //this.mView.ClearInvalidPasswordHint();
                    this.mView.ShowEmptyAccountNickNameError();
                    isCorrect = false;
                }

                //handle button to enable or disable
                if (isCorrect == true)
                {
                    this.mView.EnableAddAccountButton();
                    return true;
                }
                else
                {
                    this.mView.DisableAddAccountButton();
                    return false;
                }
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
                return false;
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
                    mView.ShowInvalidAccountNicknameError();
                    return;
                }

                if (!Utility.AddAccountNumberValidation(accountNum.Length))
                {
                    mView.ShowInvalidAccountNumberError();
                    return;
                }

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
            mView.ShowAddingAccountProgressDialog();

            try
            {
                ValidateManualAccountRequest addaccRequest = new ValidateManualAccountRequest(accountNum, accountType, userIdentificationNum, suppliedMotherName, isOwner.ToString());
              
                string dt = JsonConvert.SerializeObject(addaccRequest);
                var result = await ServiceApiImpl.Instance.ValidateManualAccount(new MyTNBService.Request.ValidateManualAccountRequest(accountNum, accountType, userIdentificationNum, suppliedMotherName, isOwner.ToString()));
                if (mView.IsActive())
                {
                    mView.HideAddingAccountProgressDialog();
                }
                if (result != null && result.Response != null && result.Response.ErrorCode != Constants.SERVICE_CODE_SUCCESS && result.Response.DisplayType.Equals("POPUP"))
                {
                    mView.GovermentDialog();
                }
                else if (result != null && result.Response != null && result.Response.ErrorCode != Constants.SERVICE_CODE_SUCCESS && result.Response.DisplayMessage.Contains(Utility.GetLocalizedLabel("AddAccount", "invalidROC")))
                {
                    mView.ShowAddAccountROCFail(result.Response.DisplayMessage);
                }
                else if (result != null && result.Response != null && result.Response.ErrorCode != Constants.SERVICE_CODE_SUCCESS && result.Response.DisplayMessage.Contains(Utility.GetLocalizedErrorLabel("invalid_accountType")))
                {
                    mView.ShowAddAccountInvalid(result.Response.DisplayMessage);
                }
                else if (result != null && result.Response != null && result.Response.ErrorCode != Constants.SERVICE_CODE_SUCCESS)
                {
                    mView.ShowAddAccountFail(result.Response.DisplayMessage);
                }
                else
                {
                    mView.ClearText();
                    NewAccount account = new NewAccount();
                    account.accountNumber = accountNum;
                    account.accountAddress = result.GetData().accountStAddress;
                    account.isOwner = isOwner;
                    account.type = accountType;
                    account.isRegistered = false;
                    account.accountLabel = accountLable;
                    account.icNum = userIdentificationNum;
                    account.accountCategoryId = result.GetData().accountCategoryId;
                    account.accountTypeId = accountType;
                    account.emailOwner = result.GetData().emailOwner;
                    account.mobileNoOwner = result.GetData().mobileNoOwner;
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
                //if (accountNickName.Length == 0)
                //{
                //    mView.RemoveNameErrorMessage();
                //}


                if (accountno.Length == 0)
                {
                    mView.RemoveNumberErrorMessage();
                }

                if (ownerIC.Length == 0)
                {
                    mView.ClearROCError();
                }


                if (!TextUtils.IsEmpty(accountno) && !TextUtils.IsEmpty(accountNickName))
                {

                    if (isOwner && !Utility.AddAccountNumberValidation(accountno.Length))
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
                                mView.RemoveNameErrorMessage(); // added
                                mView.RemoveNumberErrorMessage();// added
                            }
                        }
                        else
                        {
                            mView.EnableAddAccountButton();
                            mView.RemoveNameErrorMessage();  // added
                            mView.RemoveNumberErrorMessage(); // added
                        }
                    }
                    else if (TextUtils.IsEmpty(ownerIC))
                    {
                        mView.ClearROCError();
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
