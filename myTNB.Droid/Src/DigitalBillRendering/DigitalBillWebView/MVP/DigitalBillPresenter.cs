using Android.App;
using Android.Util;
using myTNB.SitecoreCMS.Model;
using myTNB.SitecoreCMS.Services;
using myTNB.SQLite.SQLiteDataManager;
using myTNB.Android.Src.Database.Model;
using myTNB.Android.Src.SiteCore;
using myTNB.Android.Src.SitecoreCMS.Model;
using myTNB.Android.Src.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace myTNB.Android.Src.DigitalBill.MVP
{
    public class DigitalBillPresenter : DigitalBillContract.IUserActionsListener
    {
        private DigitalBillContract.IView mView;
        private CancellationTokenSource cts;

        public DigitalBillPresenter(DigitalBillContract.IView view)
        {
            this.mView = view;
            this.mView.SetPresenter(this);
        }



        public void Start()
        {
            //No Impl
            //GetDigitalBillData();
            this.mView.ShowDigitalBill(true);
        }
    }
}