using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Text;
using Android.Util;
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

namespace myTNB_Android.Src.AddNewUser.MVP
{
    public class AddNewUserPresenter : AddNewUserContract.IUserActionsListener
    {
        private AddNewUserContract.IView mView;
        CancellationTokenSource cts;
        AccountData accountData;
        public AddNewUserPresenter(AddNewUserContract.IView mView, AccountData accountData)
        {
            this.mView = mView;
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

        public async void OnAddAccount(string emailNewUser, bool ishaveAccess, bool ishaveEBilling)
        {
            if (mView.IsActive())
            {
                this.mView.ShowProgress();
            }

            UserEntity user = UserEntity.GetActive();
            try
            {
                //var removeAccountResponse = await ServiceApiImpl.Instance.RemoveAccount(new RemoveAccountRequest(accountData.AccountNum));

                if (mView.IsActive())
                {
                    this.mView.HideProgress();
                    this.mView.ShowSuccessAddNewUser();
                }

                /*if (removeAccountResponse.IsSuccessResponse())
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
                }
                else
                {
                    this.mView.ShowErrorMessageResponse(removeAccountResponse.Response.DisplayMessage);
                }*/
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