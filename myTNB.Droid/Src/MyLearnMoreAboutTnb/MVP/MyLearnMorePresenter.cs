using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Android.Util;
using myTNB.AndroidApp.Src.Database.Model;
using myTNB.AndroidApp.Src.MultipleAccountPayment.Models;
using myTNB.AndroidApp.Src.ManageCards.Models;
using myTNB.AndroidApp.Src.myTNBMenu.Models;
using myTNB.AndroidApp.Src.MyTNBService.Request;
using myTNB.AndroidApp.Src.MyTNBService.ServiceImpl;
using myTNB.AndroidApp.Src.Utils;
using Refit;

namespace myTNB.AndroidApp.Src.MyLearnMoreAboutTnb.MVP
{
    public class MyLearnMorePresenter : MyLearnMoreContract.IUserActionsListener
    {
        private MyLearnMoreContract.IView mView;
        private readonly string TAG = typeof(MyLearnMorePresenter).Name;

        public MyLearnMorePresenter(MyLearnMoreContract.IView mView)
        {
            this.mView = mView;
            this.mView.SetPresenter(this);
        }

        public void Start()
        {
            throw new NotImplementedException();
        }

    }
}
