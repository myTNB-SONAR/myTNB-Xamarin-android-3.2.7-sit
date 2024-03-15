using Android.App;
using Android.Content;
using Android.Runtime;
using Java.Util;
using myTNB.SQLite.SQLiteDataManager;
using myTNB.AndroidApp.Src.Base;
using myTNB.AndroidApp.Src.Database.Model;
using myTNB.AndroidApp.Src.LogUserAccess.Models;
using myTNB.AndroidApp.Src.LogUserAccess.Models;
using myTNB.AndroidApp.Src.ManageAccess.Models;
using myTNB.AndroidApp.Src.ManageCards.Models;
using myTNB.AndroidApp.Src.myTNBMenu.Models;
using myTNB.AndroidApp.Src.MyTNBService.Request;
using myTNB.AndroidApp.Src.MyTNBService.Response;
using myTNB.AndroidApp.Src.MyTNBService.ServiceImpl;
using myTNB.AndroidApp.Src.NewAppTutorial.MVP;
using myTNB.AndroidApp.Src.Utils;
using Newtonsoft.Json;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;

namespace myTNB.AndroidApp.Src.ManageAccess.MVP
{
    internal class ManageAccessPresenter : ManageAccessContract.IUserActionsListener
    {

        private ManageAccessContract.IView mView;
        List<AccountUserAccessData> selectedUserAccessList;
        AccountData accountData;
        LogUserAccessData logAccount;
        int position;
        private ISharedPreferences mSharedPref;

        public ManageAccessPresenter(ManageAccessContract.IView mView, AccountData accountData, ISharedPreferences mSharedPref)
        {
            this.mView = mView;
            this.mSharedPref = mSharedPref;
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

                        if (data != null && data.Extras.GetString("cancelInvited") != null)
                        {
                            string email = data.Extras.GetString("cancelInvited");
                            this.mView.ShowCancelAddSuccess(email);
                        }
                    }
                }
                else if (requestCode == Constants.ADD_USER)
                {
                    if (resultCode == Result.Ok)
                    {
                        if (data != null && data.Extras.GetString("Invited") != null)
                        {
                            string email = data.Extras.GetString("Invited");
                            this.mView.ShowAddTNBUserSuccess(email);
                        }
                        else if (data != null && data.Extras.GetString("Add") != null)
                        {
                            string email = data.Extras.GetString("Add");
                            this.mView.ShowAddNonTNBUserSuccess(email);
                        }

                        this.mView.AdapterClean();
                        Start();
                    }
                }              
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public async void OnRemoveAccount(List<UserManageAccessAccount> DeletedSelectedUser,List<DeleteAccessAccount> accountList, string AccountNum)
        {

            UserEntity user = UserEntity.GetActive();
            try
            {

                RemoveAccountRequestNew removeAccountRequest = new RemoveAccountRequestNew(accountList, AccountNum);
                removeAccountRequest.SetIsWhiteList(UserSessions.GetWhiteList(mSharedPref));

                var removeAccountResponse = await ServiceApiImpl.Instance.RemoveUserAcess_OT(removeAccountRequest);

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

        public async void OnRemoveAccountMultiple(List<UserManageAccessAccount> DeletedSelectedUser, bool MultipleDelete, List<DeleteAccessAccount> accountList, string AccountNum)
        {
            List<LogUserAccessNewData> newAccountList = new List<LogUserAccessNewData>();
            List<DeleteAccessAccount> AccountList = new List<DeleteAccessAccount>();
            DeleteAccessAccount deletedUser = new DeleteAccessAccount();


            foreach (UserManageAccessAccount accUser in DeletedSelectedUser)
            {
                if (accUser.UserAccountId != null)
                {
                    var newRecord = new DeleteAccessAccount()
                    {
                        UserAccountId = accUser.UserAccountId,
                        IsApplyEBilling = accUser.IsApplyEBilling,
                        IsHaveAccess = accUser.IsHaveAccess,
                        userId = accUser.userId,
                        email = accUser.email,
                        IsPreRegister = accUser.IsPreRegister,
                        tenantNickname = accUser.AccDesc
                    };
                    AccountList.Add(newRecord);
                    deletedUser = newRecord;
                }
            }

            if (AccountList.Count > 1)
            {
                MultipleDelete = true;
            }
            else
            {
                MultipleDelete = false;
            }

            if (mView.IsActive())
            {
                this.mView.ShowProgressDialog();
            }

            UserEntity user = UserEntity.GetActive();
            try
            {
                RemoveAccountRequestNew removeUserAccountRequest = new RemoveAccountRequestNew(AccountList, AccountNum);
                removeUserAccountRequest.SetIsWhiteList(UserSessions.GetWhiteList(mSharedPref));

                var removeAccountResponse = await ServiceApiImpl.Instance.RemoveUserAcess_OT(removeUserAccountRequest);

                if (mView.IsActive())
                {
                    this.mView.HideShowProgressDialog();
                }

                if (removeAccountResponse.IsSuccessResponse())
                {
                    UserManageAccessAccount.DeleteSelected(accountData.AccountNum);

                    
                    if (!MultipleDelete && DeletedSelectedUser.Count == 0)
                    {
                        if (deletedUser.IsPreRegister == true)
                        {
                            this.mView.UserAccessNonUserRemoveSuccess(deletedUser.email);
                            this.mView.ShowEmptyAccount();
                        }
                        else
                        {
                            this.mView.UserAccessRemoveSuccess();
                            this.mView.ShowEmptyAccount();
                        }
                        
                    }
                    else if (MultipleDelete && DeletedSelectedUser.Count >1)
                    {
                        this.mView.UserAccessRemoveSuccess();
                        this.mView.ShowEmptyAccount();
                    }
                    else
                    {
                        if(deletedUser.IsPreRegister == true)
                        {
                            this.mView.UserAccessRemoveNonUserSuccessSwipe(deletedUser.email);
                        }
                        else
                        {
                            this.mView.UserAccessRemoveSuccessSwipe();
                        }
                        
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
                LogUserAccessRequest logUserAccessRequest = new LogUserAccessRequest(accountData.AccountNum);
                logUserAccessRequest.SetIsWhiteList(UserSessions.GetWhiteList(mSharedPref));
                LogUserAccessResponse logUserAccessResponse = await ServiceApiImpl.Instance.GetAccountActivityLogList(logUserAccessRequest);

                if (mView.IsActive())
                {
                    this.mView.HideShowProgressDialog();

                }

                if (logUserAccessResponse.IsSuccessResponse())
                {
                    if (logUserAccessResponse.GetData().Count > 0)
                    //if (logUserAccessResponse.GetData() != null)
                    {
                        ProcessManageAccessAccountLog(logUserAccessResponse.GetData());
                    }
                    else
                    {
                        List<LogUserAccessNewData> newAccountList = new List<LogUserAccessNewData>();
                        this.mView.NavigateLogUserAccess(newAccountList);
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
                            IsPreRegister = acc.IsPreRegister,
                            CreateByName = acc.CreateByName
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


            List<UserManageAccessAccount> customerAccountList = UserManageAccessAccount.List(accountData?.AccountNum);
            if (customerAccountList != null && customerAccountList.Count > 0)
            {    
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
            }
            else
            {
                newList.Add(new NewAppModel()
                {
                    ContentShowPosition = ContentType.BottomRight,
                    ContentTitle = Utility.GetLocalizedLabel("UserAccess", "walkthroughStep1Title"),
                    ContentMessage = Utility.GetLocalizedLabel("UserAccess", "walkthroughStep1"),
                    ItemCount = 0,
                    NeedHelpHide = true,
                    IsButtonShow = false
                });

            }
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