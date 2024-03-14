using Android.Text;
using myTNB.Android.Src.Base;
using myTNB.Android.Src.Database.Model;
using myTNB.Android.Src.myTNBMenu.Models;
using myTNB.Android.Src.MyTNBService.Request;
using myTNB.Android.Src.MyTNBService.ServiceImpl;
using myTNB.Android.Src.Utils;
using Refit;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;

namespace myTNB.Android.Src.UpdateNickname.MVP
{
    public class UpdateNicknamePresenter : UpdateNicknameContract.IUserActionsListener
    {
        private UpdateNicknameContract.IView mView;

        AccountData accountData;

        public UpdateNicknamePresenter(UpdateNicknameContract.IView mView, AccountData accountData)
        {
            this.mView = mView;
            this.accountData = accountData;
            this.mView.SetPresenter(this);
        }

        public async void OnUpdateNickName(string accountNo, string oldAccountNickName, string newAccountNickName)
        {
            this.mView.ClearError();

            if (TextUtils.IsEmpty(newAccountNickName))
            {
                this.mView.DisableSaveButton();
                this.mView.ShowEmptyNickNameError();
                return;
            }

            if (!TextUtils.IsEmpty(newAccountNickName))
            {
                bool isAccountNameExist = false;
                List<CustomerBillingAccount> accounts = CustomerBillingAccount.List();
                if (accounts != null && accounts.Count > 0 && !TextUtils.IsEmpty(newAccountNickName))
                {
                    foreach (CustomerBillingAccount item in accounts)
                    {
                        if (!string.IsNullOrEmpty(item.AccDesc) && item.AccDesc.ToLower().Trim().Equals(newAccountNickName.ToString().ToLower().Trim()))
                        {
                            isAccountNameExist = true;
                            break;
                        }
                    }
                }

                if (isAccountNameExist)
                {
                    this.mView.ShowSameNickNameError();
                    return;
                }
            }

            if (mView.IsActive())
            {
                this.mView.ShowProgressDialog();
            }

            try
            {
                var updateNickNameResponse = await ServiceApiImpl.Instance.UpdateLinkedAccountNickName(new UpdateLinkedAccountNameRequest(accountNo, oldAccountNickName, newAccountNickName));
                if (mView.IsActive())
                {
                    this.mView.HideProgressDialog();
                }

                if (updateNickNameResponse.IsSuccessResponse())
                {
                    CustomerBillingAccount.UpdateAccountName(newAccountNickName, accountNo);
                    AccountDataEntity.UpdateNickName(newAccountNickName, accountNo);

                    /**Since Summary dashBoard logic is changed these codes where commented on 01-11-2018**/
                    //SummaryDashBoardAccountEntity.UpdateNickName(newAccountNickName, accountNo);
                    /**Since Summary dashBoard logic is changed these codes where commented on 01-11-2018**/

                    SummaryDashBoardAccountEntity.RemoveAll();

                    MyTNBAccountManagement.GetInstance().RemoveCustomerBillingDetails();
                    HomeMenuUtils.ResetAll();

                    this.mView.ShowSuccessUpdateNickname(newAccountNickName);
                }
                else
                {
                    this.mView.ShowResponseError(updateNickNameResponse.Response.DisplayMessage);
                }
            }
            catch (System.OperationCanceledException e)
            {
                if (mView.IsActive())
                {
                    this.mView.HideProgressDialog();
                }
                this.mView.ShowRetryOptionsCancelledException(e);
                Utility.LoggingNonFatalError(e);
            }
            catch (ApiException apiException)
            {
                if (mView.IsActive())
                {
                    this.mView.HideProgressDialog();
                }
                // ADD HTTP CONNECTION EXCEPTION HERE
                this.mView.ShowRetryOptionsApiException(apiException);
                Utility.LoggingNonFatalError(apiException);
            }
            catch (Exception e)
            {
                if (mView.IsActive())
                {
                    this.mView.HideProgressDialog();
                }
                // ADD UNKNOWN EXCEPTION HERE
                this.mView.ShowRetryOptionsUnknownException(e);
                Utility.LoggingNonFatalError(e);
            }

        }

        public void OnVerifyNickName(string accountNo, string newAccountNickname)
        {
            try
            {
                this.mView.ClearError();

                if (TextUtils.IsEmpty(newAccountNickname))
                {
                    this.mView.DisableSaveButton();
                    return;
                }

                //if (!Utility.isAlphaNumeric(newAccountNickname))
                //{
                //    this.mView.ShowEnterValidAccountName();
                //    this.mView.DisableSaveButton();
                //    return;
                //}

                CustomerBillingAccount customerBillingAccount = CustomerBillingAccount.FindByAccNum(accountData.AccountNum);
                if (customerBillingAccount != null && customerBillingAccount.AccDesc.Equals(newAccountNickname))
                {
                    this.mView.DisableSaveButton();
                }
                else
                {
                    this.mView.EnableSaveButton();
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void Start()
        {
            try
            {
                this.mView.DisableSaveButton();
                CustomerBillingAccount customerBillingAccount = CustomerBillingAccount.FindByAccNum(accountData.AccountNum);
                if (customerBillingAccount != null && !TextUtils.IsEmpty(customerBillingAccount.AccDesc))
                {
                    this.mView.ShowNickname(customerBillingAccount.AccDesc);
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }
    }
}
