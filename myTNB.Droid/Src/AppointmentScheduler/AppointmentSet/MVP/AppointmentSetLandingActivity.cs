
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;


using Android.Text;
using Android.Text.Method;
using Android.Text.Style;
using Android.Views;
using Android.Widget;
using CheeseBind;
using myTNB_Android.Src.AppointmentScheduler.AppointmentSetLanding.MVP;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.FAQ.Activity;
using myTNB_Android.Src.RewardDetail.MVP;
using myTNB_Android.Src.Utils;
using myTNB_Android.Src.WhatsNewDetail.MVP;

namespace myTNB_Android.Src.AppointmentScheduler.AAppointmentSetLanding.MVP
{
    [Activity(Label = "Application Set", Theme = "@style/Theme.AppointmentScheduler")]
    public class AppointmentSetLandingActivity : BaseAppCompatActivity, AppointmentSetLandingContract.IView
    {
        AppointmentSetLandingPresenter mPresenter;
        public override int ResourceId()
        {
            return Resource.Layout.AppointmentSetLandingLayout;
        }
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            mPresenter = new AppointmentSetLandingPresenter(this);
        }

        public void UpdateUI()
        {
            throw new NotImplementedException();
        }
    }
}