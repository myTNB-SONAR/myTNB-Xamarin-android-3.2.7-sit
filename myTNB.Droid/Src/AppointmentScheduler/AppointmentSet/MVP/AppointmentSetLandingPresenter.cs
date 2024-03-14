using System;
using System.Globalization;
using System.Threading.Tasks;
using myTNB.Android.Src.AppointmentScheduler.AAppointmentSetLanding.MVP;
using myTNB.Android.Src.Base.Models;
using myTNB.Android.Src.Database.Model;
using myTNB.Android.Src.myTNBMenu.Fragments.RewardMenu.Api;
using myTNB.Android.Src.myTNBMenu.Fragments.RewardMenu.Model;
using myTNB.Android.Src.myTNBMenu.Fragments.RewardMenu.Request;
using myTNB.Android.Src.myTNBMenu.Fragments.RewardMenu.Response;
using myTNB.Android.Src.Utils;

namespace myTNB.Android.Src.AppointmentScheduler.AppointmentSetLanding.MVP
{
    public class AppointmentSetLandingPresenter
    {
        AppointmentSetLandingContract.IView mView;
        private AppointmentSetLandingActivity AppointmentSetLandingActivity;
        private RewardServiceImpl mApi;
        public AppointmentSetLandingPresenter(AppointmentSetLandingContract.IView view)
        {
            mView = view;
            this.mApi = new RewardServiceImpl();
        }
    }
}
