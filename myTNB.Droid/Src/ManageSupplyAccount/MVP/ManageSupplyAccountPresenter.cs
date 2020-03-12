﻿using Android.App;
using Android.Content;
using Android.Runtime;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.ManageSupplyAccount.Api;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;
using Refit;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading;

namespace myTNB_Android.Src.ManageSupplyAccount.MVP
{
    public class ManageSupplyAccountPresenter : ManageSupplyAccountContract.IUserActionsListener
    {
        private ManageSupplyAccountContract.IView mView;
        CancellationTokenSource cts;
        AccountData accountData;
        public ManageSupplyAccountPresenter(ManageSupplyAccountContract.IView mView, AccountData accountData)
        {
            this.mView = mView;
            this.mView.SetPresenter(this);
            this.accountData = accountData;
        }

        public void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            try
            {
                if (requestCode == Constants.UPDATE_NICKNAME_REQUEST)
                {
                    if (resultCode == Result.Ok)
                    {
                        AccountData accountData = JsonConvert.DeserializeObject<AccountData>(data.Extras.GetString(Constants.SELECTED_ACCOUNT));
                        this.mView.ShowUpdateSuccessNickname(accountData);
                    }
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public async void OnRemoveAccount(AccountData accountData)
        {
            cts = new CancellationTokenSource();

            if (mView.IsActive())
            {
                this.mView.ShowRemoveProgress();
            }

#if DEBUG || STUB
            var httpClient = new HttpClient(new HttpLoggingHandler(/*new NativeMessageHandler()*/)) { BaseAddress = new Uri(Constants.SERVER_URL.END_POINT) };
            var api = RestService.For<IManageSupplyAccountApi>(httpClient);
#else
            var api = RestService.For<IManageSupplyAccountApi>(Constants.SERVER_URL.END_POINT);
#endif

            UserEntity user = UserEntity.GetActive();
            try
            {
                var removeSupplyAccountApi = await api.RemoveTNBAccountForUserFav(new Request.RemoveTNBAccountForUserFavRequest()
                {
                    UserID = user.UserID,
                    AccountNumber = accountData.AccountNum,
                    ApiKeyId = Constants.APP_CONFIG.API_KEY_ID,
                    Email = UserEntity.GetActive().Email,
                    IpAddress = Constants.APP_CONFIG.API_KEY_ID,
                    ClientType = Constants.APP_CONFIG.API_KEY_ID,
                    ActiveUserName = Constants.APP_CONFIG.API_KEY_ID,
                    DevicePlatform = Constants.APP_CONFIG.API_KEY_ID,
                    DeviceVersion = Constants.APP_CONFIG.API_KEY_ID,
                    DeviceCordova = Constants.APP_CONFIG.API_KEY_ID
                }, cts.Token);

                if (mView.IsActive())
                {
                    this.mView.HideRemoveProgress();
                }

                if (!removeSupplyAccountApi.Data.IsError)
                {
                    bool isSelectedAcc = false;
                    if (CustomerBillingAccount.GetSelected() != null &&
                       CustomerBillingAccount.GetSelected().AccNum.Equals(accountData.AccountNum))
                    {
                        isSelectedAcc = true;
                    }
                    CustomerBillingAccount.Remove(accountData.AccountNum);
                    if (isSelectedAcc && CustomerBillingAccount.Enumerate().ToList().Count() > 0)
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
                    this.mView.ShowSuccessRemovedAccount();
                }
                else
                {
                    this.mView.ShowErrorMessageResponse(removeSupplyAccountApi.Data.Message);
                }
            }
            catch (System.OperationCanceledException e)
            {
                if (mView.IsActive())
                {
                    this.mView.HideRemoveProgress();
                }
                // ADD OPERATION CANCELLED HERE
                this.mView.ShowRetryOptionsCancelledException(e);
                Utility.LoggingNonFatalError(e);
            }
            catch (ApiException apiException)
            {
                if (mView.IsActive())
                {
                    this.mView.HideRemoveProgress();
                }
                // ADD HTTP CONNECTION EXCEPTION HERE
                this.mView.ShowRetryOptionsApiException(apiException);
                Utility.LoggingNonFatalError(apiException);
            }
            catch (Exception e)
            {
                if (mView.IsActive())
                {
                    this.mView.HideRemoveProgress();
                }
                // ADD UNKNOWN EXCEPTION HERE
                this.mView.ShowRetryOptionsUnknownException(e);
                Utility.LoggingNonFatalError(e);
            }

        }

        public void OnUpdateNickname()
        {
            this.mView.ShowUpdateNickname();
        }

        public void Start()
        {
            //
            if (accountData != null && !string.IsNullOrEmpty(accountData?.AccountNum))
            {
                CustomerBillingAccount customerBillingAccount = new CustomerBillingAccount();
                customerBillingAccount = CustomerBillingAccount.FindByAccNum(accountData?.AccountNum);
                if (customerBillingAccount != null)
                {
                    this.mView.ShowNickname(customerBillingAccount?.AccDesc);
                }
            }

        }
    }
}