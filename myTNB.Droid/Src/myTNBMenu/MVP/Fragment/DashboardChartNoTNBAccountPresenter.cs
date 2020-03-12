namespace myTNB_Android.Src.myTNBMenu.MVP.Fragment
{
    public class DashboardChartNoTNBAccountPresenter : DashboardChartNoTNBAccountContract.IUserActionsListener
    {
        private DashboardChartNoTNBAccountContract.IView mView;

        public DashboardChartNoTNBAccountPresenter(DashboardChartNoTNBAccountContract.IView mView)
        {
            this.mView = mView;
            this.mView.SetPresenter(this);
        }

        public void OnAddAccount()
        {
            this.mView.ShowAddAccount();
        }

        public void OnNotification()
        {
            this.mView.ShowNotification();
        }

        public void Start()
        {

        }
    }
}