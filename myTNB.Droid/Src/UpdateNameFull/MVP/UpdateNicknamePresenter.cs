using Android.Text;
using myTNB.AndroidApp.Src.Base;
using myTNB.AndroidApp.Src.Database.Model;
using myTNB.AndroidApp.Src.myTNBMenu.Models;
using myTNB.AndroidApp.Src.MyTNBService.Request;
using myTNB.AndroidApp.Src.MyTNBService.ServiceImpl;
using myTNB.AndroidApp.Src.RegistrationForm.Models;
using myTNB.AndroidApp.Src.Utils;
using Newtonsoft.Json;
using Refit;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;

namespace myTNB.AndroidApp.Src.UpdateNameFull.MVP
{
    public class UpdateNicknamePresenter : UpdateNicknameContract.IUserActionsListener
    {
        private UpdateNicknameContract.IView mView;

        AccountData accountData;

        public UpdateNicknamePresenter(UpdateNicknameContract.IView mView)
        {
            this.mView = mView;
            this.mView.SetPresenter(this);
        }

        public async void OnUpdateName(string newName)
        {
            this.mView.ClearError();

            if (TextUtils.IsEmpty(newName))
            {
                this.mView.DisableSaveButton();
                this.mView.ShowEmptyNickNameError();
                return;
            }

            if (mView.IsActive())
            {
                this.mView.ShowProgressDialog();
            }

            try
            {
                UserEntity userEntity = UserEntity.GetActive();
                string mobile = userEntity.MobileNo;
                string usrId = userEntity.DeviceId;

                UpdateUserInfo userAuthRequest = new UpdateUserInfo(mobile, newName);
                var userNameResponse = await ServiceApiImpl.Instance.UserAuthenticateUpdateName(userAuthRequest);
                //var userNameResponse = await ServiceApiImpl.Instance.UserAuthenticateUpdateName(new UpdateUserInfo(usrId, mobile, newName));
                
                if (mView.IsActive())
                {
                    this.mView.HideProgressDialog();
                }

                if (userNameResponse.Response.Data.IsSuccess)
                {
                    if (UserEntity.IsCurrentlyActive())
                    {
                        UserEntity.UpdateFullname(newName);
                        this.mView.ShowSuccessUpdateName();
                        MyTNBAccountManagement.GetInstance().SetIsNameUpdated(true);
                    };
                }
                else
                {
                    this.mView.ShowResponseError(userNameResponse.Response.DisplayMessage);
                }
            }
            catch (System.OperationCanceledException e)
            {
                if (mView.IsActive())
                {
                    this.mView.HideProgressDialog();
                }
                this.mView.ShowRetryOptionsCancelledException(e);
                Utility.LoggingNonFatalError(e);
            }
            catch (ApiException apiException)
            {
                if (mView.IsActive())
                {
                    this.mView.HideProgressDialog();
                }
                // ADD HTTP CONNECTION EXCEPTION HERE
                this.mView.ShowRetryOptionsApiException(apiException);
                Utility.LoggingNonFatalError(apiException);
            }
            catch (Exception e)
            {
                if (mView.IsActive())
                {
                    this.mView.HideProgressDialog();
                }
                // ADD UNKNOWN EXCEPTION HERE
                this.mView.ShowRetryOptionsUnknownException(e);
                Utility.LoggingNonFatalError(e);
            }

        }

        public void OnVerifyName(string newName)
        {
            try
            {
                this.mView.ClearError();

                if (TextUtils.IsEmpty(newName))
                {
                    this.mView.DisableSaveButton();
                    return;
                }               
                else
                {
                    this.mView.EnableSaveButton();
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void Start()
        {
            try
            {
                this.mView.DisableSaveButton();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }
    }
}
