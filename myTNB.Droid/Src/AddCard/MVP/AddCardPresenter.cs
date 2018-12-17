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

namespace myTNB_Android.Src.AddCard.MVP
{
    public class AddCardPresenter : AddCardContract.IUserActionsListener
    {
        private AddCardContract.IView mView;

        public AddCardPresenter(AddCardContract.IView mView)
        {
            this.mView = mView;
            this.mView.SetPresenter(this);
        }

        public void Start()
        {

        }
    }
}