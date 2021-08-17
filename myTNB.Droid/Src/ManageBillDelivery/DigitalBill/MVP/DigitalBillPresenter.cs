using Android.App;
using Android.Util;
using myTNB.SitecoreCMS.Model;
using myTNB.SitecoreCMS.Services;
using myTNB.SQLite.SQLiteDataManager;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.SiteCore;
using myTNB_Android.Src.SitecoreCMS.Model;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace myTNB_Android.Src.DigitalBill.MVP
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
        }
    }
}