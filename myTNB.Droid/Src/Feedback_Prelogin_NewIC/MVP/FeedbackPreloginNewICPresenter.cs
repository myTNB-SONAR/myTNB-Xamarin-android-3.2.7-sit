using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Telephony;
using Android.Text;
using Castle.Core.Internal;
using Java.Text;
using myTNB_Android.Src.Base.Api;
using myTNB_Android.Src.Base.Models;
using myTNB_Android.Src.Base.Request;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.MyTNBService.Request;
using myTNB_Android.Src.MyTNBService.ServiceImpl;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;
using Refit;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;

namespace myTNB_Android.Src.Feedback_Prelogin_NewIC.MVP
{
   public  class FeedbackPreloginNewICPresenter  : FeedbackPreloginNewICContract.IUserActionsListener
    {

        FeedbackPreloginNewICContract.IView mView;

        CustomerBillingAccount selectedCustomerBillingAccount;

        private ISharedPreferences mSharedPref;


        public FeedbackPreloginNewICPresenter(FeedbackPreloginNewICContract.IView mView, ISharedPreferences mSharedPref)
        {
            this.mView = mView;
            this.mView.SetPresenter(this);
            this.mSharedPref = mSharedPref;
        }

        public void OnGeneralEnquiry()
        {
            this.mView.ShowGeneralEnquiry();
        }

       public void onUpdatePersonalDetail()
        {
            this.mView.showUpdatePersonalDetail();
        }

        public void Start()
        {
        }

        public void OnSelectAccount()
        {
            try
            {
                if (CustomerBillingAccount.HasItems())
                {
                    this.mView.ShowSelectAccount();
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

        }

        public void onShowWhereIsMyAcc()
        {
            this.mView.ShowWhereIsMyAcc();
        }

        public void showScan()
        {
            this.mView.onScan();
        }

        public void CheckRequiredFields(string accno)
        {


           
            try
            {
                if (!TextUtils.IsEmpty(accno) )
                {

                    if (!Utility.AddAccountNumberValidation(accno.Length))
                    {
                        this.mView.ShowInvalidAccountNumberError();
                        this.mView.toggleDisableClick();

                    }
                    else
                    {
                        this.mView.RemoveNumberErrorMessage();
                        this.mView.toggleEnableClick();
                    }


          
                }
                else
                {   

                    //if empty
                    this.mView.ShowEnterOrSelectAccNumber();
                    this.mView.toggleDisableClick();

                }
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public async void ValidateAccountAsync(string contractAccounts , bool isUpdateUserInfo)
        {
           

            try
            {

                if (mView.IsActive())
                {
                    this.mView.ShowProgressDialog();
                }

                //GetSearchForAccountRequest caReq = new GetSearchForAccountRequest();
                //caReq.SetAcc(contractAccounts);

                GetSearchForAccountRequest con = new GetSearchForAccountRequest(contractAccounts);

               // Console.WriteLine(SerializeObject(con));
          
                var result = await ServiceApiImpl.Instance.ValidateAccIsExist(con);

                //if (result != null && !result.GetSearchForAccount[0].FullName.IsNullOrEmpty()  && !result.GetSearchForAccount[0].IC.IsNullOrEmpty())
                if (result != null)
                {
                    this.mView.HideProgressDialog();
                    var data = result.GetSearchForAccount[0];
                    UserSessions.SaveGetAccountIsExist(mSharedPref, JsonConvert.SerializeObject(data));

                    if (isUpdateUserInfo)
                    {
                        onUpdatePersonalDetail();
                    }
                    else
                    {
                        OnGeneralEnquiry();
                    }


                   // mView.ShowAddAccountFail(result.Response.DisplayMessage);
                }
                else
                {   // no data
                    this.mView.HideProgressDialog();
                    this.mView.ShowInvalidAccountNumberError();
                    this.mView.makeSetClick(false);

                }

            }
            catch (System.OperationCanceledException e)
            {
                if (mView.IsActive())
                {
                    this.mView.HideProgressDialog();
                }
                this.mView.makeSetClick(false);
                //this.mView.ShowFail();
                this.mView.OnSubmitError();
                Utility.LoggingNonFatalError(e);
            }
            catch (ApiException apiException)
            {
                if (mView.IsActive())
                {
                    this.mView.HideProgressDialog();
                }
                //this.mView.ShowFail();
                this.mView.makeSetClick(false);
                this.mView.OnSubmitError();
                Utility.LoggingNonFatalError(apiException);
            }
            catch (Exception e)
            {
                if (mView.IsActive())
                {
                    this.mView.HideProgressDialog();
                }
                //this.mView.ShowFail();
                this.mView.makeSetClick(false);
                this.mView.OnSubmitError();
                Utility.LoggingNonFatalError(e);
            }

        }


    }
}