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

namespace myTNB.AndroidApp.Src.RegisterValidation.MVP
{
    public class EmelPopupPresenter : EmelPopupContract.IUserActionsListener
    {
        private EmelPopupContract.IView mView;
        private readonly string TAG = typeof(EmelPopupPresenter).Name;

        public EmelPopupPresenter(EmelPopupContract.IView mView)
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
