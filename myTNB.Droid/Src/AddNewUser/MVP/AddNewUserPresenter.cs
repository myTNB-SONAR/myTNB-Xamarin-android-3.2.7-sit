using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Text;
using Android.Util;
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

namespace myTNB_Android.Src.AddNewUser.MVP
{
    public class AddNewUserPresenter : AddNewUserContract.IUserActionsListener
    {
        private AddNewUserContract.IView mView;
        CancellationTokenSource cts;
        AccountData accountData;
        private ISharedPreferences mSharedPref;

        public AddNewUserPresenter(AddNewUserContract.IView mView, AccountData accountData, ISharedPreferences mSharedPref)
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

            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void CheckRequiredFields(string email)
        {

            try
            {
                if (!Patterns.EmailAddress.Matcher(email).Matches())
                {
                    //this.mView.ShowInvalidEmailError();
                    this.mView.DisableAddUserButton();
                    return;
                }
                else
                    this.mView.EnableAddUserButton();
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public async void OnAddAccount(string emailNewUser, string accNo, bool ishaveAccess, bool ishaveEBilling)
        {
            if (mView.IsActive())
            {
                this.mView.ShowProgress();
            }

            UserEntity user = UserEntity.GetActive();
            try
            {
                AddUserAccessAccountRequest addUserAccessAccountRequest = new AddUserAccessAccountRequest(emailNewUser, accNo, ishaveAccess, ishaveEBilling);
                addUserAccessAccountRequest.SetIsWhiteList(UserSessions.GetWhiteList(mSharedPref));

                var AddNewUserAccountResponse = await ServiceApiImpl.Instance.AddUserAcess_OT(addUserAccessAccountRequest);

                if (mView.IsActive())
                {
                    this.mView.HideProgress();
                }

                if (AddNewUserAccountResponse.IsSuccessResponse())
                {
                    try
                    {
                        ManageAccessAccountListResponse manageAccessAccountListResponse = await ServiceApiImpl.Instance.GetAccountAccessRightList(new GetAccountAccessRight(accountData.AccountNum));

                        if (manageAccessAccountListResponse != null && manageAccessAccountListResponse.Response.ErrorCode == Constants.SERVICE_CODE_SUCCESS)
                        {
                            if (manageAccessAccountListResponse.GetData().Count > 0)
                            {
                                ProcessManageAccessAccount(manageAccessAccountListResponse.GetData(), emailNewUser);
                            }
                        }
                    }
                    catch (System.OperationCanceledException e)
                    {
                        if (mView.IsActive())
                        {
                            this.mView.HideProgress();
                        }
                        // ADD OPERATION CANCELLED HERE
                        this.mView.ShowRetryOptionsCancelledException(e);
                        Utility.LoggingNonFatalError(e);
                    }
                    catch (ApiException apiException)
                    {
                        if (mView.IsActive())
                        {
                            this.mView.HideProgress();
                        }
                        // ADD HTTP CONNECTION EXCEPTION HERE
                        this.mView.ShowRetryOptionsApiException(apiException);
                        Utility.LoggingNonFatalError(apiException);
                    }
                    catch (Exception e)
                    {
                        if (mView.IsActive())
                        {
                            this.mView.HideProgress();
                        }
                        // ADD UNKNOWN EXCEPTION HERE
                        this.mView.ShowRetryOptionsUnknownException(e);
                        Utility.LoggingNonFatalError(e);
                    }
                }
                else
                {
                    this.mView.ShowErrorMessageResponse(AddNewUserAccountResponse.Response.DisplayTitle,AddNewUserAccountResponse.Response.DisplayMessage);
                }
            }
            catch (System.OperationCanceledException e)
            {
                if (mView.IsActive())
                {
                    this.mView.HideProgress();
                }
                // ADD OPERATION CANCELLED HERE
                this.mView.ShowRetryOptionsCancelledException(e);
                Utility.LoggingNonFatalError(e);
            }
            catch (ApiException apiException)
            {
                if (mView.IsActive())
                {
                    this.mView.HideProgress();
                }
                // ADD HTTP CONNECTION EXCEPTION HERE
                this.mView.ShowRetryOptionsApiException(apiException);
                Utility.LoggingNonFatalError(apiException);
            }
            catch (Exception e)
            {
                if (mView.IsActive())
                {
                    this.mView.HideProgress();
                }
                // ADD UNKNOWN EXCEPTION HERE
                this.mView.ShowRetryOptionsUnknownException(e);
                Utility.LoggingNonFatalError(e);
            }

        }

        private void ProcessManageAccessAccount(List<ManageAccessAccountListResponse.CustomerAccountData> list, string email)
        {
            try
            {
                int ctr = 0;
                if (list.Count > 0)
                {
                    List<UserManageAccessAccount> newAccountList = new List<UserManageAccessAccount>();
                    UserManageAccessAccount newuser = new UserManageAccessAccount();
                    foreach (ManageAccessAccountListResponse.CustomerAccountData acc in list)
                    {
                        if (acc.Email.ToLower().Equals(email.ToLower()))
                        {
                            var newRecord = new UserManageAccessAccount()
                            {
                                AccNum = acc.AccountNumber,
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
                            newuser = newRecord;
                        }
                    }

                    if (newAccountList.Count > 0)
                    {
                        newAccountList.Sort((x, y) => string.Compare(x.AccDesc, y.AccDesc));
                        foreach (ManageAccessAccountListResponse.CustomerAccountData acc in list)
                        {
                            if (acc.Email.ToLower().Equals(email.ToLower()))
                            {
                                int rowChange = UserManageAccessAccount.InsertOrReplace(acc);
                                ctr++;
                            }
                        }

                        if (newuser.IsPreRegister)
                        {
                            this.mView.ShowSuccessAddNewUserPreRegister(newuser.email);
                        }
                        else
                        {
                            this.mView.ShowSuccessAddNewUser(newuser.email);
                        }
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
            throw new NotImplementedException();
        }

        public void Start()
        {
            this.mView.DisableAddUserButton();
        }
    }
}