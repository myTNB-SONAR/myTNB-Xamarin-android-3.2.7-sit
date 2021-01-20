using Android.App;
using Android.Content;
using Android.Runtime;
using Java.Util;
using myTNB.SQLite.SQLiteDataManager;
using myTNB_Android.Src.Base;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.LogUserAccess.Models;
using myTNB_Android.Src.LogUserAccess.Models;
using myTNB_Android.Src.ManageAccess.Models;
using myTNB_Android.Src.ManageCards.Models;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.MyTNBService.Request;
using myTNB_Android.Src.MyTNBService.Response;
using myTNB_Android.Src.MyTNBService.ServiceImpl;
using myTNB_Android.Src.NewAppTutorial.MVP;
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
        List<AccountUserAccessData> selectedUserAccessList;
        AccountData accountData;
        LogUserAccessData logAccount;
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
                        if (data.Extras.GetString("Invited") != null)
                        {
                            string email = data.Extras.GetString("Invited");
                            this.mView.ShowAddNonTNBUserSuccess(email);
                        }
                        else
                        {
                            string email = data.Extras.GetString("Add");
                            this.mView.ShowAddTNBUserSuccess(email);
                        }

                        this.mView.AdapterClean();
                        this.mView.AdapterDeleteClean();
                        Start();
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
                var removeAccountResponse = await ServiceApiImpl.Instance.RemoveUserAcess_OT(new RemoveAccountRequest(AccountNum));

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

        public async void OnRemoveAccountMultiple(List<UserManageAccessAccount> DeletedSelectedUser, bool MultipleDelete)
        {
            //List<LogUserAccessNewData> newAccountList = new List<LogUserAccessNewData>();
            ArrayList nameList2 = new ArrayList();
            int i = 0;
            String[] accountIdList = new String[DeletedSelectedUser.Count];

            foreach (UserManageAccessAccount accUser in DeletedSelectedUser)
            {
                if (accUser.UserAccountId != null)
                {                   
                    nameList2.Add(accUser.UserAccountId);
                    accountIdList[i] = accUser.UserAccountId;
                    ++i;
                }
            }

            if (mView.IsActive())
            {
                this.mView.ShowProgressDialog();
            }

            UserEntity user = UserEntity.GetActive();
            try
            {
                var removeAccountResponse = await ServiceApiImpl.Instance.RemoveUserAcess_OT(new RemoveUserAccountRequest(accountIdList));

                if (mView.IsActive())
                {
                    this.mView.HideShowProgressDialog();
                }

                if (removeAccountResponse.IsSuccessResponse())
                {
                    UserManageAccessAccount.DeleteSelected(accountData.AccountNum);
                    if (MultipleDelete)
                    {
                        this.mView.UserAccessRemoveSuccess();
                    }
                    else
                    {
                        this.mView.UserAccessRemoveSuccessSwipe();
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
                    if (logUserAccessResponse.GetData().Count > 0)
                    {
                        ProcessManageAccessAccountLog(logUserAccessResponse.GetData());
                    }
                }
                else
                {
                    List<LogUserAccessNewData> newAccountList = new List<LogUserAccessNewData>();
                    this.mView.NavigateLogUserAccess(newAccountList);
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

        private void ProcessManageAccessAccountLog(List<LogUserAccessResponse.LogUserAccessResponseData> list)
        {
            try
            {
                int ctr = 0;
                if (list.Count > 0)
                {
                    //List<UserManageAccessAccount> existingSortedList = AccountSortingEntity.List(UserEntity.GetActive().Email, Constants.APP_CONFIG.ENV);

                    List<LogUserAccessNewData> fetchList = new List<LogUserAccessNewData>();
                    List<LogUserAccessNewData> newAccountList = new List<LogUserAccessNewData>();

                    List<int> newExisitingListArray = new List<int>();

                    foreach (LogUserAccessResponse.LogUserAccessResponseData acc in list)
                    {
                        //int index = existingSortedList.FindIndex(x => x.AccNum == acc.AccountNumber);

                        var newRecord = new LogUserAccessNewData()
                        {
                            AccountNo = acc.AccountNo,
                            Action = acc.Action,
                            ActivityLogID = acc.ActivityLogID,
                            IsApplyEBilling = acc.IsApplyEBilling,
                            IsHaveAccess = acc.IsHaveAccess,
                            CreateBy = acc.CreateBy,
                            CreatedDate = acc.CreatedDate,
                            UserID = acc.UserID,
                            UserName = acc.UserName,
                        };
                        newAccountList.Add(newRecord);
                    }

                    this.mView.NavigateLogUserAccess(newAccountList);
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void OnAddAccount()
        {
            this.mView.ShowAddAccount();
        }

        public List<NewAppModel> OnGeneraNewAppTutorialList()
        {
            List<NewAppModel> newList = new List<NewAppModel>();

            newList.Add(new NewAppModel()
            {
                ContentShowPosition = ContentType.BottomRight,
                ContentTitle = Utility.GetLocalizedLabel("UserAccess", "walkthroughStep1Title"),
                ContentMessage = Utility.GetLocalizedLabel("UserAccess", "walkthroughStep1"),
                ItemCount = 0,
                NeedHelpHide = true,
                IsButtonShow = false
            });

            newList.Add(new NewAppModel()
            {
                ContentShowPosition = ContentType.BottomLeft,
                ContentTitle = Utility.GetLocalizedLabel("UserAccess", "walkthroughStep2Title"),
                ContentMessage = Utility.GetLocalizedLabel("UserAccess", "walkthroughStep2"),
                ItemCount = UserManageAccessAccount.List(accountData.AccountNum).Count,
                NeedHelpHide = true,
                IsButtonShow = false
            });

            return newList;
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