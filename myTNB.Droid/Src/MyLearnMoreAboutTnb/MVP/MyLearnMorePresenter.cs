using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Android.Util;
using myTNB.Android.Src.Database.Model;
using myTNB.Android.Src.MultipleAccountPayment.Models;
using myTNB.Android.Src.ManageCards.Models;
using myTNB.Android.Src.myTNBMenu.Models;
using myTNB.Android.Src.MyTNBService.Request;
using myTNB.Android.Src.MyTNBService.ServiceImpl;
using myTNB.Android.Src.Utils;
using Refit;

namespace myTNB.Android.Src.MyLearnMoreAboutTnb.MVP
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
