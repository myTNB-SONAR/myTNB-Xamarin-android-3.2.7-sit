using Android.App;
using Android.Content;
using Android.Runtime;
using myTNB.SQLite.SQLiteDataManager;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.ManageCards.Models;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.SSMR.SMRApplication.MVP;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;

namespace myTNB_Android.Src.EnergyBudget.MVP
{
    internal class EnergyBudgetPresenter : EnergyBudgetContract.IUserActionsListener
    {

        private EnergyBudgetContract.IView mView;

        public EnergyBudgetPresenter(EnergyBudgetContract.IView mView)
        {
            this.mView = mView;
            this.mView.SetPresenter(this);
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

        public void ResfreshPageList(List<SMRAccount> list)
        {
            try
            {
                if (list != null)
                {
                    this.mView.ShowAccountList(list);
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
                //ServicePointManager.ServerCertificateValidationCallback += SSLFactoryHelper.CertificateValidationCallBack;
                //this.mView.GetAccountList();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

        }
    }
}