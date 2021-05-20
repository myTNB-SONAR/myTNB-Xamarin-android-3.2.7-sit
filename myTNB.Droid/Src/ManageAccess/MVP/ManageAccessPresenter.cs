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
                            string message = data.Extras.GetString("cancelInvited");
                            this.mView.ShowCancelAddSuccess(message);
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

                RemoveAccountRequest removeAccountRequest = new RemoveAccountRequest(accountList, AccountNum);
                removeAccountRequest.SetIsWhiteList(UserSessions.GetWhiteList(mSharedPref));
                string dt = JsonConvert.SerializeObject(removeAccountRequest);

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

            //ArrayList nameList2 = new ArrayList();
            //int i = 0;


            //ArrayList nameList3 = new ArrayList();
            //int m = 0;
            //String[] accountemailList = new String[DeletedSelectedUser.Count];

            //foreach (var accUser in accountList)
            //{
            //    AccountList.Add(new UserManageAccessAccount
            //    {
            //        UserAccountId = accUser.UserAccountId,
            //        IsApplyEBilling = accUser.IsApplyEBilling,
            //        IsHaveAccess = accUser.IsHaveAccess,
            //        userId = accUser.userId,
            //        email = accUser.email
            //    });
            //}


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
                    };
                    AccountList.Add(newRecord);
                }
            }


            //}
            //String[] accountIdList = new String[DeletedSelectedUser.Count];

            //foreach (UserManageAccessAccount accUser in DeletedSelectedUser)
            //{
            //    if (accUser.UserAccountId != null)
            //    {
            //        nameList2.Add(accUser.UserAccountId);
            //        accountIdList[i] = accUser.UserAccountId;

            //        ++i;
            //    }
            //}


            if (mView.IsActive())
            {
                this.mView.ShowProgressDialog();
            }

            UserEntity user = UserEntity.GetActive();
            try
            {
                RemoveAccountRequest removeUserAccountRequest = new RemoveAccountRequest(AccountList, AccountNum);
                removeUserAccountRequest.SetIsWhiteList(UserSessions.GetWhiteList(mSharedPref));
                string dt = JsonConvert.SerializeObject(removeUserAccountRequest);

                var removeAccountResponse = await ServiceApiImpl.Instance.RemoveUserAcess_OT(removeUserAccountRequest);

                if (mView.IsActive())
                {
                    this.mView.HideShowProgressDialog();
                }

                if (removeAccountResponse.IsSuccessResponse())
                {
                    UserManageAccessAccount.DeleteSelected(accountData.AccountNum);
                    if (this.mView.checkListUserEmpty() == 0)
                    {
                        
                        this.mView.UserAccessRemoveSuccess();
                        this.mView.ShowEmptyAccount();

                    }
                    else if (MultipleDelete && this.mView.checkListUserEmpty() > 0)
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