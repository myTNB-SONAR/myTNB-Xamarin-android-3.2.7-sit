﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Threading;
using myTNB_Android.Src.Utils;
using System.Net.Http;
using Refit;
using myTNB_Android.Src.UpdateNickname.Api;
using Android.Text;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.SummaryDashBoard.Models;
using Newtonsoft.Json;

namespace myTNB_Android.Src.UpdateNickname.MVP
{
    public class UpdateNicknamePresenter : UpdateNicknameContract.IUserActionsListener
    {
        private UpdateNicknameContract.IView mView;
        CancellationTokenSource cts;

        AccountData accountData;

        public UpdateNicknamePresenter(UpdateNicknameContract.IView mView , AccountData accountData)
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

            if (!Utility.isAlphaNumeric(newAccountNickName)) {
                this.mView.DisableSaveButton();
                this.mView.ShowEnterValidAccountName();
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

            cts = new CancellationTokenSource();
            this.mView.ShowProgressDialog();

#if DEBUG || STUB
            var httpClient = new HttpClient(new HttpLoggingHandler(/*new NativeMessageHandler()*/)) { BaseAddress = new Uri(Constants.SERVER_URL.END_POINT) };
            var updateNickNameApi = RestService.For<IUpdateAccountApi>(httpClient);
#else
            var updateNickNameApi = RestService.For<IUpdateAccountApi>(Constants.SERVER_URL.END_POINT);
#endif
            UserEntity userEntity = UserEntity.GetActive();
            try
            {
                var updateNickNameResponse = await updateNickNameApi.UpdateLinkedAccountNickName(new Request.UpdateLinkedAccountNickNameRequest()
                {
                    ApiKeyId = Constants.APP_CONFIG.API_KEY_ID,
                    Email = userEntity.Email,
                    UserId = userEntity.UserID,
                    AccountNo = accountNo,
                    OldAccountNickName = oldAccountNickName,
                    NewAccountNickName = newAccountNickName
                } , cts.Token);

                if (!updateNickNameResponse.Data.IsError)
                {
                    CustomerBillingAccount.UpdateAccountName(newAccountNickName , accountNo);
                    AccountDataEntity.UpdateNickName(newAccountNickName, accountNo);

                    /**Since Summary dashBoard logic is changed these codes where commented on 01-11-2018**/
                    //SummaryDashBoardAccountEntity.UpdateNickName(newAccountNickName, accountNo);
                    /**Since Summary dashBoard logic is changed these codes where commented on 01-11-2018**/

                    SummaryDashBoardAccountEntity.RemoveAll();

                    this.mView.ShowSuccessUpdateNickname(newAccountNickName);
                }
                else
                {
                    this.mView.ShowResponseError(updateNickNameResponse.Data.Message);
                }
            }
            catch (System.OperationCanceledException e)
            {
                this.mView.ShowRetryOptionsCancelledException(e);
            }
            catch (ApiException apiException)
            {
                // ADD HTTP CONNECTION EXCEPTION HERE
                this.mView.ShowRetryOptionsApiException(apiException);
            }
            catch (Exception e)
            {
                // ADD UNKNOWN EXCEPTION HERE
                this.mView.ShowRetryOptionsUnknownException(e);
            }
            this.mView.HideProgressDialog();
        }

        public void OnVerifyNickName(string accountNo, string newAccountNickname)
        {
            this.mView.ClearError();

            if (TextUtils.IsEmpty(newAccountNickname))
            {
                this.mView.DisableSaveButton();
                this.mView.ShowEmptyNickNameError();
                return;
            }

            if (!Utility.isAlphaNumeric(newAccountNickname))
            {
                this.mView.ShowEnterValidAccountName();
                this.mView.DisableSaveButton();
                return;
            }

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

        public void Start()
        {
            this.mView.DisableSaveButton();
            CustomerBillingAccount customerBillingAccount = CustomerBillingAccount.FindByAccNum(accountData.AccountNum);
            if (customerBillingAccount != null && !TextUtils.IsEmpty(customerBillingAccount.AccDesc))
            {
                this.mView.ShowNickname(customerBillingAccount.AccDesc);
            }
            
        }
    }
}