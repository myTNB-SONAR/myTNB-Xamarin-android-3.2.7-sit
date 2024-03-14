using myTNB.Android.Src.myTNBMenu.Fragments.RewardMenu.Api;

namespace myTNB.Android.Src.AppointmentScheduler.AppointmentSelect.MVP
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