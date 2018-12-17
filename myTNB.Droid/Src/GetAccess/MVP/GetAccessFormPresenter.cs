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
using Android.Text;
using myTNB_Android.Src.Utils;

namespace myTNB_Android.Src.GetAccess.MVP
{
    public class GetAccessFormPresenter : GetAccessFormContract.IUserActionsListener
    {
        private GetAccessFormContract.IView mView;

        public GetAccessFormPresenter(GetAccessFormContract.IView mView)
        {
            this.mView = mView;
            this.mView.SetPresenter(this);
        }

        public void OnGetAccess(string icno, string maiden_name)
        {
            try
            {
                if (TextUtils.IsEmpty(icno))
                {
                    this.mView.ShowEmptyICNo();
                    return;
                }

                if (TextUtils.IsEmpty(maiden_name))
                {
                    this.mView.ShowEmptyMaidenName();
                    return;
                }
                this.mView.ShowSuccess();
            } catch(Exception e) {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void Start()
        {
            // NO IMPL
        }
    }
}