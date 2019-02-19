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
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;
using System.Threading;
using System.Net.Http;
using Refit;
using myTNB_Android.Src.ManageSupplyAccount.Api;
using myTNB_Android.Src.Database.Model;

namespace myTNB_Android.Src.ManageSupplyAccount.MVP
{
    public class ManageSupplyAccountPresenter : ManageSupplyAccountContract.IUserActionsListener
    {
        private ManageSupplyAccountContract.IView mView;
        CancellationTokenSource cts;
        AccountData accountData;
        public ManageSupplyAccountPresenter(ManageSupplyAccountContract.IView mView , AccountData accountData)
        {
            this.mView = mView;
            this.mView.SetPresenter(this);
            this.accountData = accountData;
        }

        public void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
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

        public async void OnRemoveAccount(AccountData accountData)
        {
            cts = new CancellationTokenSource();

            this.mView.ShowRemoveProgress();
            
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
                } , cts.Token);

                if (!removeSupplyAccountApi.Data.IsError)
                {
                    bool isSelectedAcc = false;
                    if(CustomerBillingAccount.GetSelected() != null && 
                       CustomerBillingAccount.GetSelected().AccNum.Equals(accountData.AccountNum)){
                        isSelectedAcc = true;
                    }
                    CustomerBillingAccount.Remove(accountData.AccountNum);
                    if (isSelectedAcc && CustomerBillingAccount.Enumerate().ToList().Count() > 0) {
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
                // ADD OPERATION CANCELLED HERE
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


            this.mView.HideRemoveProgress();
            
        }

        public void OnUpdateNickname()
        {
            this.mView.ShowUpdateNickname();
        }

        public void Start()
        {
            //
            CustomerBillingAccount customerBillingAccount = CustomerBillingAccount.FindByAccNum(accountData.AccountNum);
            this.mView.ShowNickname(customerBillingAccount.AccDesc);
        }
    }
}