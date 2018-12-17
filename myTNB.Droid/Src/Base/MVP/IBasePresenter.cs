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

namespace myTNB_Android.Src.Base.MVP
{
    public interface IBasePresenter
    {
        /// <summary>
        /// Action to initiate the task has started
        /// </summary>
        void Start();
    }
}