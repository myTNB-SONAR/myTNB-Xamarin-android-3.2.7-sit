﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using myTNB.AndroidApp.Src.Base.MVP;

namespace myTNB.AndroidApp.Src.SMRnewTncView.MVP
{
    public class SMRnewTncViewContract
    {

        public interface IView : IBaseView<IUserActionsListener>
        {

        }

        public interface IUserActionsListener : IBasePresenter
        {

        }
    }
}