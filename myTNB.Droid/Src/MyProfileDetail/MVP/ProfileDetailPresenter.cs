using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Android.Util;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.MultipleAccountPayment.Models;
using myTNB_Android.Src.ManageCards.Models;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.MyTNBService.Request;
using myTNB_Android.Src.MyTNBService.ServiceImpl;
using myTNB_Android.Src.Utils;
using Refit;

namespace myTNB_Android.Src.MyProfileDetail.MVP
{
    public class ProfileDetailPresenter : ProfileDetailContract.IUserActionsListener
    {
        private ProfileDetailContract.IView mView;
        private readonly string TAG = typeof(ProfileDetailPresenter).Name;

        public ProfileDetailPresenter(ProfileDetailContract.IView mView)
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
