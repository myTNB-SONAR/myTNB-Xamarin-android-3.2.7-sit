using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace myTNB.AndroidApp.Src.SMRnewTncView.MVP
{
   public  class SMRnewTncViewPresenter : SMRnewTncViewContract.IUserActionsListener
    {

        SMRnewTncViewContract.IView mView;

        public void Start()
        {
            // TODO : IMPL
        }

        public SMRnewTncViewPresenter(SMRnewTncViewContract.IView mView)
        {
            this.mView = mView;
            this.mView.SetPresenter(this);
        }







    }
}