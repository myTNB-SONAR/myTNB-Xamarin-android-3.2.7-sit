using myTNB.AndroidApp.Src.myTNBMenu.Fragments.RewardMenu.Api;

namespace myTNB.AndroidApp.Src.AppointmentScheduler.AppointmentSelect.MVP
{
    public class AppointmentSelectPresenter
    {
        public AppointmentSelectPresenter(AppointmentSelectContract.IView view)
        {
            mView = view;
            this.mApi = new RewardServiceImpl();
        }
        AppointmentSelectContract.IView mView;
        private AppointmentSelectActivity appointmentSelectActivity;
        private RewardServiceImpl mApi;
    }
}