using Android.Util;
using myTNB.AndroidApp.Src.AddAccount.Models;
using myTNB.AndroidApp.Src.Database.Model;
using myTNB.AndroidApp.Src.myTNBMenu.Api;
using myTNB.AndroidApp.Src.myTNBMenu.Models;
using myTNB.AndroidApp.Src.Utils;
using Newtonsoft.Json;
using Refit;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;

namespace myTNB.AndroidApp.Src.SelectSupplyAccount.MVP
{
    public class SelectSupplyAccountPresenter : SelectSupplyAccountContract.IUserActionsListener
    {
        CancellationTokenSource cts;
        private SelectSupplyAccountContract.IView mView;

        public SelectSupplyAccountPresenter(SelectSupplyAccountContract.IView mView)
        {
            this.mView = mView;
            this.mView.SetPresenter(this);
        }


        private void CloseSelectAccountsWithError()
        {
            this.mView.ShowDashboardChartWithError();
        }

        public void Start()
        {
            List<CustomerBillingAccount> custBAList = CustomerBillingAccount.List();

            this.mView.ShowList(custBAList);
        }
    }
}
