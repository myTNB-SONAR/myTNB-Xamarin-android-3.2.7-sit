using Android.App;
using Android.Content;
using Android.Runtime;
using myTNB.SQLite.SQLiteDataManager;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.LogoutRate.Api;
using myTNB_Android.Src.MakePayment.Api;
using myTNB_Android.Src.MakePayment.Models;
using myTNB_Android.Src.ManageCards.Models;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;

namespace myTNB_Android.Src.MyAccount.MVP
{
    internal class MyAccountPresenter : MyAccountContract.IUserActionsListener
    {

        private MyAccountContract.IView mView;

        public MyAccountPresenter(MyAccountContract.IView mView)
        {
            this.mView = mView;
            this.mView.SetPresenter(this);
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
                        List<CustomerBillingAccount> customerAccountList = CustomerBillingAccount.List();
                        if (customerAccountList != null && customerAccountList.Count > 0)
                        {
                            this.mView.ShowAccountList(customerAccountList);
                        }
                        else
                        {
                            this.mView.ShowEmptyAccount();
                        }
                        if (data != null && data.HasExtra(Constants.ACCOUNT_REMOVED_FLAG) && data.GetBooleanExtra(Constants.ACCOUNT_REMOVED_FLAG, false))
                        {
                            this.mView.ShowAccountRemovedSuccess();
                        }
                    }
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

        public void Start()
        {
            try
            {
                ServicePointManager.ServerCertificateValidationCallback += SSLFactoryHelper.CertificateValidationCallBack;
                if (CustomerBillingAccount.HasItems())
                {
                    List<CustomerBillingAccount> customerAccountList = CustomerBillingAccount.List();
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