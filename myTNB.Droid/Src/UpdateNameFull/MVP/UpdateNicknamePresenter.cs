using Android.Text;
using myTNB_Android.Src.Base;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.MyTNBService.Request;
using myTNB_Android.Src.MyTNBService.ServiceImpl;
using myTNB_Android.Src.Utils;
using Refit;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;

namespace myTNB_Android.Src.UpdateNameFull.MVP
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

                //UpdateIdentificationNo userAuthRequest = new UpdateIdentificationNo(idtype, no_ic);
                //userAuthRequest.SetUserIdentification(userEntity.Email);
                var userNameResponse = await ServiceApiImpl.Instance.UserAuthenticateUpdateName(new UpdateUserInfo(usrId, mobile, newName));
                
                if (mView.IsActive())
                {
                    this.mView.HideProgressDialog();
                }

                if (!userNameResponse.IsSuccessResponse())
                {
                    this.mView.ShowSuccessUpdateName();
                    MyTNBAccountManagement.GetInstance().SetIsNameUpdated(true);
                }
                else
                {
                    this.mView.ShowSuccessUpdateName();
                    MyTNBAccountManagement.GetInstance().SetIsNameUpdated(true);
                    //this.mView.ShowResponseError(updateNickNameResponse.Response.DisplayMessage);
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
