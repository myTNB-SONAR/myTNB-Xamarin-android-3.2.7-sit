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
using Android.Content.PM;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.ResetPasswordSuccess.MVP;
using CheeseBind;
using myTNB_Android.Src.Utils;
using myTNB_Android.Src.Login.Activity;

namespace myTNB_Android.Src.ResetPasswordSuccess.Activity
{
    [Activity(NoHistory = true
              , Icon = "@drawable/ic_launcher"
          , ScreenOrientation = ScreenOrientation.Portrait
          , Theme = "@style/Theme.ResetPasswordSuccess")]
    public class ResetPasswordSuccessActivity : BaseAppCompatActivity , ResetPasswordSuccessContract.IView
    {

        private ResetPasswordSuccessContract.IUserActionsListener userActionsListener;
        private ResetPasswordSuccessPresenter mPresenter;

        [BindView(Resource.Id.txtTitleInfo)]
        TextView txtTitleInfo;

        [BindView(Resource.Id.txtContentInfo)]
        TextView txtContentInfo;

        [BindView(Resource.Id.btnLogin)]
        Button btnLogin;

        [BindView(Resource.Id.btnClose)]
        ImageView btnClose;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            mPresenter = new ResetPasswordSuccessPresenter(this);

            TextViewUtils.SetMuseoSans500Typeface(txtTitleInfo);
            TextViewUtils.SetMuseoSans300Typeface(txtContentInfo);
            TextViewUtils.SetMuseoSans500Typeface(btnLogin);
        }


        public bool IsActive()
        {
            return Window.DecorView.RootView.IsShown;
        }

        public override int ResourceId()
        {
            return Resource.Layout.ResetPasswordSuccessView;
        }

        public void SetPresenter(ResetPasswordSuccessContract.IUserActionsListener userActionListener)
        {
            this.userActionsListener = userActionListener;
        }

        public void ShowBackActivity()
        {
            Intent LoginIntent = new Intent(this, typeof(LoginActivity));
            LoginIntent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.ClearTask | ActivityFlags.NewTask);
            StartActivity(LoginIntent);
        }

        public void ShowLoginActivity()
        {
            Intent LoginIntent = new Intent(this, typeof(LoginActivity));
            LoginIntent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.ClearTask | ActivityFlags.NewTask);
            StartActivity(LoginIntent);
        }

        [OnClick(Resource.Id.btnLogin)]
        void OnLogin(object sender ,EventArgs eventArgs)
        {
            this.userActionsListener.OnLogin();
        }

        [OnClick(Resource.Id.btnClose)]
        void OnClose(object sender, EventArgs eventArgs)
        {
            this.userActionsListener.OnClose();
        }
    }
}