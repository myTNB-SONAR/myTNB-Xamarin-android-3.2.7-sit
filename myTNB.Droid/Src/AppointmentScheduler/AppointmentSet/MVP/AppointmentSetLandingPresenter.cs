using System;
using System.Globalization;
using System.Threading.Tasks;
using myTNB.AndroidApp.Src.AppointmentScheduler.AAppointmentSetLanding.MVP;
using myTNB.AndroidApp.Src.Base.Models;
using myTNB.AndroidApp.Src.Database.Model;
using myTNB.AndroidApp.Src.myTNBMenu.Fragments.RewardMenu.Api;
using myTNB.AndroidApp.Src.myTNBMenu.Fragments.RewardMenu.Model;
using myTNB.AndroidApp.Src.myTNBMenu.Fragments.RewardMenu.Request;
using myTNB.AndroidApp.Src.myTNBMenu.Fragments.RewardMenu.Response;
using myTNB.AndroidApp.Src.Utils;

namespace myTNB.AndroidApp.Src.AppointmentScheduler.AppointmentSetLanding.MVP
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
