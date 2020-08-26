using System;
using System.Globalization;
using System.Threading.Tasks;
using myTNB_Android.Src.AppointmentScheduler.AAppointmentSetLanding.MVP;
using myTNB_Android.Src.Base.Models;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.myTNBMenu.Fragments.RewardMenu.Api;
using myTNB_Android.Src.myTNBMenu.Fragments.RewardMenu.Model;
using myTNB_Android.Src.myTNBMenu.Fragments.RewardMenu.Request;
using myTNB_Android.Src.myTNBMenu.Fragments.RewardMenu.Response;
using myTNB_Android.Src.Utils;

namespace myTNB_Android.Src.AppointmentScheduler.AppointmentSetLanding.MVP
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
