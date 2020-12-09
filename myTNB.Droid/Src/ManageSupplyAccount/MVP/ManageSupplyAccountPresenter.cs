using Android.App;
using Android.Content;
using Android.Runtime;
using myTNB_Android.Src.Base;
using myTNB_Android.Src.Database.Model;
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
            if (mView.IsActive())
            {
                this.mView.ShowRemoveProgress();
            }

            UserEntity user = UserEntity.GetActive();
            try
            {
                var removeAccountResponse = await ServiceApiImpl.Instance.RemoveAccount(new RemoveAccountRequest(accountData.AccountNum));

                if (mView.IsActive())
                {
                    this.mView.HideRemoveProgress();
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
                    this.mView.ShowSuccessRemovedAccount();
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

        public async void ManageAccessUser(AccountData accountData)
        {
            if (mView.IsActive())
            {
                this.mView.ShowRemoveProgress();
            }
            try
            {
                //var GetAccessAccountResponse = await ServiceApiImpl.Instance.RemoveAccount(new RemoveAccountRequest(accountData.AccountNum));
                //GetAccountAccessRight getAccountAccessRight = new GetAccountAccessRight(accountData.AccountNum, "string");
                ManageAccessAccountListResponse manageAccessAccountListResponse = await ServiceApiImpl.Instance.GetAccountAccessRightList( new GetAccountAccessRight(accountData.AccountNum));

                if (mView.IsActive())
                {
                    this.mView.HideRemoveProgress();
                }

                if (manageAccessAccountListResponse != null && manageAccessAccountListResponse.GetData() != null && manageAccessAccountListResponse.Response.ErrorCode == Constants.SERVICE_CODE_SUCCESS)
                {
                    if (manageAccessAccountListResponse.GetData().Count > 0)
                    {
                        ProcessManageAccessAccount(manageAccessAccountListResponse.GetData());
                    }
                    else
                    {
                        AccountSortingEntity.RemoveSpecificAccountSorting(UserEntity.GetActive().Email, Constants.APP_CONFIG.ENV);
                    }
                    this.mView.ManageUserActivity();

                }
                else
                {
                    this.mView.ShowErrorMessageResponse(manageAccessAccountListResponse.Response.DisplayMessage);
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

        private void ProcessManageAccessAccount(List<ManageAccessAccountListResponse.CustomerAccountData> list)
        {
            try
            {
                int ctr = 0;
                if (list.Count > 0)
                {
                    //List<UserManageAccessAccount> existingSortedList = AccountSortingEntity.List(UserEntity.GetActive().Email, Constants.APP_CONFIG.ENV);

                    List<UserManageAccessAccount> fetchList = new List<UserManageAccessAccount>();

                    List<UserManageAccessAccount> newExistingList = new List<UserManageAccessAccount>();
                    List<int> newExisitingListArray = new List<int>();
                    List<UserManageAccessAccount> newAccountList = new List<UserManageAccessAccount>();

                    foreach (ManageAccessAccountListResponse.CustomerAccountData acc in list)
                    {
                        //int index = existingSortedList.FindIndex(x => x.AccNum == acc.AccountNumber);

                        var newRecord = new UserManageAccessAccount()
                        {
                            AccNum = acc.AccountNumber,
                            //AccDesc = string.IsNullOrEmpty(acc.AccDesc) == true ? "--" : acc.AccDesc,
                            AccDesc = acc.AccountDescription,
                            UserAccountId = acc.AccountId,
                            IsApplyEBilling = acc.IsApplyEBilling,
                            IsHaveAccess = acc.IsHaveAccess,
                            IsOwnedAccount = acc.IsOwnedAccount,
                            IsPreRegister = acc.IsPreRegister,
                            email = acc.Email,
                            name = acc.Name,
                            userId = acc.UserId,
                        };
                        newAccountList.Add(newRecord);
                    }

                    if (newAccountList.Count > 0)
                    {
                        /*newAccountList.Sort((x, y) => string.Compare(x.AccDesc, y.AccDesc));
                        foreach (UserManageAccessAccount acc in newAccountList)
                        {
                            int rowChange = UserManageAccessAccount.InsertOrReplace(acc);
                            int rowChange = UserManageAccessAccount.InsertOrReplace(acc);
                            ctr++;
                        }*/
                        //UserManageAccessAccount.MakeFirstAsSelected();

                        //newAccountList.Sort((x, y) => string.Compare(x.AccDesc, y.AccDesc));
                        foreach (ManageAccessAccountListResponse.CustomerAccountData acc in list)
                        {
                            int rowChange = UserManageAccessAccount.InsertOrReplace(acc);
                            ctr++;

                        }
                    }
                }
                else
                {
                    foreach (ManageAccessAccountListResponse.CustomerAccountData acc in list)
                    {
                        int rowChange = UserManageAccessAccount.InsertOrReplace(acc);
                    }
                }
            }
            catch (Exception e)
            {
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