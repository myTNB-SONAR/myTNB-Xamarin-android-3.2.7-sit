﻿using Android.App;
using Android.Content;
using Android.Runtime;
using myTNB.AndroidApp.Src.Base;
using myTNB.AndroidApp.Src.Database.Model;
using myTNB.AndroidApp.Src.myTNBMenu.Models;
using myTNB.AndroidApp.Src.MyTNBService.Request;
using myTNB.AndroidApp.Src.MyTNBService.Response;
using myTNB.AndroidApp.Src.MyTNBService.ServiceImpl;
using myTNB.AndroidApp.Src.Utils;
using Newtonsoft.Json;
using Refit;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading;

namespace myTNB.AndroidApp.Src.ManageUser.MVP
{
    public class ManageUserPresenter : ManageUserContract.IUserActionsListener
    {
        private ManageUserContract.IView mView;
        CancellationTokenSource cts;
        UserManageAccessAccount accountData;
        private ISharedPreferences mSharedPref;

        public ManageUserPresenter(ManageUserContract.IView mView, UserManageAccessAccount accountData, ISharedPreferences mSharedPref)
        {
            this.mView = mView;
            this.mView.SetPresenter(this);
            this.mSharedPref = mSharedPref;
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
                        //this.mView.ShowUpdateSuccessNickname(accountData);
                    }
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

       
        public async void UpdateAccountAccessRight(string accountId, string userId, bool haveAccess, bool haveEBiling, string email)
        {
            if (mView.IsActive())
            {
                this.mView.ShowRemoveProgress();
            }

            UserEntity user = UserEntity.GetActive();
            try
            {
                string action = "U";
                UpdateUserAccessRequest updateUserAccessRequest = new UpdateUserAccessRequest(accountId, userId, haveAccess, haveEBiling, action, accountData.AccNum, user.DisplayName, email);
                updateUserAccessRequest.SetIsWhiteList(UserSessions.GetWhiteList(mSharedPref));

                var updateUserAccessReponse = await ServiceApiImpl.Instance.UpdateAccountAccessRight(updateUserAccessRequest);

                if (mView.IsActive())
                {
                    this.mView.HideRemoveProgress();
                }

                if (updateUserAccessReponse.IsSuccessResponse())
                {
                    
                    UserManageAccessAccount.UpdateManageAccess(accountData.AccNum, accountId, haveAccess, haveEBiling);

                    var updateacc = new UserManageAccessAccount()
                    {
                        IsApplyEBilling = haveEBiling,
                        IsHaveAccess = haveAccess,
                    };

                    this.mView.PopulateDataCheckBox(updateacc);
                    this.mView.DisableSaveButton();
                    MyTNBAccountManagement.GetInstance().AddNewUserAdded(false);
                    this.mView.ShowSaveSuccess();

                }
                else
                {
                    MyTNBAccountManagement.GetInstance().AddNewUserAdded(true);
                    this.mView.ShowErrorMessageResponse(updateUserAccessReponse.Response.DisplayMessage);
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

        public async void CancelInvitedUser(string email, string AccNum, string userId)
        {
            if (mView.IsActive())
            {
                this.mView.ShowRemoveProgress();
            }

            try
            {
                CancelInviteUserAccessRequest cancelInviteUserAccessRequest = new CancelInviteUserAccessRequest(email, AccNum, userId);
                cancelInviteUserAccessRequest.SetIsWhiteList(UserSessions.GetWhiteList(mSharedPref));
                var updateUserAccessReponse = await ServiceApiImpl.Instance.CancelInvitation_OT(cancelInviteUserAccessRequest);

                if (mView.IsActive())
                {
                    this.mView.HideRemoveProgress();
                }

                if (updateUserAccessReponse.IsSuccessResponse())
                {
                    UserManageAccessAccount.DeleteInvited(userId);
                    //string message = updateUserAccessReponse.Response.DisplayMessage;
                    //string cancelInvite = accountData.email + message;
                    this.mView.ShowSuccessCancelInvite(accountData.email);
                }
                else
                {
                    this.mView.ShowErrorMessageResponse(updateUserAccessReponse.Response.DisplayMessage);
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


        public async void ResendInvitedUser(string email, string AccNum, bool IsHaveAccess, bool IsApplyEBilling)
        {
            if (mView.IsActive())
            {
                this.mView.ShowRemoveProgress();
            }

            try
            {
                ResendInviteUserAccessRequest addUserAccessAccountRequest = new ResendInviteUserAccessRequest(email, AccNum, IsHaveAccess, IsApplyEBilling);
                addUserAccessAccountRequest.SetIsWhiteList(UserSessions.GetWhiteList(mSharedPref));
                var updateUserResendAccessReponse = await ServiceApiImpl.Instance.SendReInviteEmail(addUserAccessAccountRequest);

                if (mView.IsActive())
                {
                    this.mView.HideRemoveProgress();
                }

                if (updateUserResendAccessReponse.IsSuccessResponse())
                {
                    string message = updateUserResendAccessReponse.Response.Message;
                    string resendInvite = accountData.email;
                    this.mView.ShowInviteSuccess(resendInvite);
                    this.mView.DisableResendButton();
                }
                else
                {
                    this.mView.ShowErrorMessageResponse(updateUserResendAccessReponse.Response.DisplayMessage);
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

        public void Start()
        {
            //
        }
    }
}