using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Telephony;
using Android.Text;
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


        public FeedbackPreloginNewICPresenter(FeedbackPreloginNewICContract.IView mView)
        {
            this.mView = mView;
            this.mView.SetPresenter(this);
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
            try
            {
                //// TODO: REPLACE WITH THE FIRST
                //this.mView.DisableSubmitButton();
                //if (selectedCustomerBillingAccount != null)
                //{
                //    this.mView.ShowSelectedAccount(selectedCustomerBillingAccount);
                //}
                //else
                //{
                //    CustomerBillingAccount customerBillingAccount = CustomerBillingAccount.GetSelectedOrFirst();
                //    if (customerBillingAccount != null)
                //    {
                //        this.mView.ShowSelectedAccount(customerBillingAccount);
                //    }

                //}

                //UserEntity userEntity = UserEntity.GetActive();
                //if (TextUtils.IsEmpty(userEntity.MobileNo))
                //{
                //    this.mView.ShowMobileNo();
                //}
                //else
                //{
                //    this.mView.HideMobileNo();
                //}
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

        }

        public void OnSelectAccount()
        {
            try
            {
                if (CustomerBillingAccount.HasItems())
                {
                    if (selectedCustomerBillingAccount != null)
                    {
                        this.mView.ShowSelectAccount(AccountData.Copy(selectedCustomerBillingAccount, true));
                    }
                    else
                    {
                        CustomerBillingAccount customerBillingAccount = CustomerBillingAccount.GetFirst();
                        this.mView.ShowSelectAccount(AccountData.Copy(customerBillingAccount, true));
                    }

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

        public void CheckRequiredFields(string accno)
        {


            this.mView.RemoveNumberErrorMessage();
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


    }
}