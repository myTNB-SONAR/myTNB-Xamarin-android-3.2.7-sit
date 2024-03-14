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

namespace myTNB.Android.Src.UpdatePersonalDetailTnC.MVP
{
   public  class UpdatePersonalDetailTnCPresenter : UpdatePersonalDetailTnCContract.IUserActionsListener
    {

        UpdatePersonalDetailTnCContract.IView mView;

        public void Start()
        {
            // TODO : IMPL
        }

        public UpdatePersonalDetailTnCPresenter(UpdatePersonalDetailTnCContract.IView mView)
        {
            this.mView = mView;
            this.mView.SetPresenter(this);
        }







    }
}