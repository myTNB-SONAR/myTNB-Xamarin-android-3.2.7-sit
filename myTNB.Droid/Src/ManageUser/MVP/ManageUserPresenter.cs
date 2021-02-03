using Android.App;
using Android.Content;
using Android.Runtime;
using myTNB_Android.Src.Base;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.MyTNBService.Request;
using myTNB_Android.Src.MyTNBService.ServiceImpl;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;
using Refit;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading;

namespace myTNB_Android.Src.ManageUser.MVP
{
    public class ManageUserPresenter : ManageUserContract.IUserActionsListener
    {
        private ManageUserContract.IView mView;
        CancellationTokenSource cts;
        UserManageAccessAccount accountData;
        public ManageUserPresenter(ManageUserContract.IView mView, UserManageAccessAccount accountData)
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
                        //this.mView.ShowUpdateSuccessNickname(accountData);
                    }
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

       
        public async void UpdateAccountAccessRight(string userId, bool haveAccess, bool haveEBiling)
        {
            if (mView.IsActive())
            {
                this.mView.ShowRemoveProgress();
            }

            UserEntity user = UserEntity.GetActive();
            try
            {
                string action = "U";
                var updateUserAccessReponse = await ServiceApiImpl.Instance.UpdateAccountAccessRight(new UpdateUserAccessRequest(userId, haveAccess, haveEBiling, action, accountData.AccNum));

                if (mView.IsActive())
                {
                    this.mView.HideRemoveProgress();
                }

                if (updateUserAccessReponse.IsSuccessResponse())
                {
                    
                    UserManageAccessAccount.UpdateManageAccess(accountData.AccNum, userId, haveAccess, haveEBiling);

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

        public async void CancelInvitedUser(string userId)
        {
            if (mView.IsActive())
            {
                this.mView.ShowRemoveProgress();
            }

            try
            {
                var updateUserAccessReponse = await ServiceApiImpl.Instance.CancelInvitation_OT(new CancelInviteUserAccessRequest(userId));

                if (mView.IsActive())
                {
                    this.mView.HideRemoveProgress();
                }

                if (updateUserAccessReponse.IsSuccessResponse())
                {
                    UserManageAccessAccount.DeleteInvited(userId);
                    string message = updateUserAccessReponse.Response.DisplayMessage;
                    string cancelInvite = accountData.email + message;
                    this.mView.ShowSuccessCancelInvite(cancelInvite);
                }
                else
                {
                    this.mView.ShowErrorMessageResponse(updateUserAccessReponse.Response.Message);
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
                var updateUserResendAccessReponse = await ServiceApiImpl.Instance.SendReInviteEmail(new ResendInviteUserAccessRequest(email, AccNum, IsHaveAccess,IsApplyEBilling));

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