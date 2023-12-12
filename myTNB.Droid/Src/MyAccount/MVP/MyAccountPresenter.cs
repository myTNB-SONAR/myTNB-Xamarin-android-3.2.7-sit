using Android.App;
using Android.Content;
using Android.Runtime;
using myTNB.SQLite.SQLiteDataManager;
using myTNB_Android.Src.Base;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.ManageCards.Models;
using myTNB_Android.Src.myTNBMenu.Async;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.MyTNBService.Request;
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

namespace myTNB_Android.Src.MyAccount.MVP
{
    internal class MyAccountPresenter : MyAccountContract.IUserActionsListener
    {

        private MyAccountContract.IView mView;
        private ISharedPreferences mSharedPref;

        public MyAccountPresenter(MyAccountContract.IView mView, ISharedPreferences mSharedPref)
        {
            this.mView = mView;
            this.mSharedPref = mSharedPref;
            this.mView.SetPresenter(this);
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
                        List<CustomerBillingAccount> customerAccountList = CustomerBillingAccount.List();
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
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }


        }

        public async void OnRemoveAccount(string AccountNum, bool isOwner, bool IsInManageAccessList)
        {
           
            bool isTaggedSmartMeter = false;
            for (int i = 0; i < UserSessions.GetEnergyBudgetList().Count; i++)
            {
                if (UserSessions.GetEnergyBudgetList()[i].accountNumber == AccountNum)
                {
                    isTaggedSmartMeter = true;
                }
            }

            UserEntity user = UserEntity.GetActive();
            try
            {
                bool isHaveAccess = false;
                bool isApplyBilling = false;
                RemoveAccountRequest removeAccountRequest = new RemoveAccountRequest(AccountNum, isTaggedSmartMeter, isOwner, IsInManageAccessList);
                removeAccountRequest.SetIsWhiteList(UserSessions.GetWhiteList(mSharedPref));

                var removeAccountResponse = await ServiceApiImpl.Instance.RemoveAccount(removeAccountRequest);

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
                    SMUsageHistoryEntity.RemoveAccountData(AccountNum);
                    UsageHistoryEntity.RemoveAccountData(AccountNum);
                    BillHistoryEntity.RemoveAccountData(AccountNum);
                    PaymentHistoryEntity.RemoveAccountData(AccountNum);
                    REPaymentHistoryEntity.RemoveAccountData(AccountNum);
                    AccountDataEntity.RemoveAccountData(AccountNum);
                    SummaryDashBoardAccountEntity.RemoveAll();
                    MyTNBAccountManagement.GetInstance().RemoveCustomerBillingDetails();
                    HomeMenuUtils.ResetAll();
                    this.mView.showsuccessDelete();

                    _ = await CustomEligibility.Instance.EvaluateEligibility((Context)this.mView, true);
                    await CustomEligibility.EvaluateEligibilityTenantDBR((Context)this.mView);
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

        public void OnAddAccount()
        {
            this.mView.ShowAddAccount();
        }

        public void Start()
        {
            try
            {
                ServicePointManager.ServerCertificateValidationCallback += SSLFactoryHelper.CertificateValidationCallBack;
                if (CustomerBillingAccount.HasItems())
                {
                    List<CustomerBillingAccount> customerAccountList = CustomerBillingAccount.List();
                    if (customerAccountList != null && customerAccountList.Count > 0)
                    {
                        this.mView.ShowAccountList(customerAccountList);
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