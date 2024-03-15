using System;
using System.Threading.Tasks;
using Android.App;
using Android.Util;
using myTNB.SitecoreCMS.Model;
using myTNB.SitecoreCMS.Services;
using myTNB.SQLite.SQLiteDataManager;
using myTNB.AndroidApp.Src.Database.Model;
using myTNB.AndroidApp.Src.SiteCore;
using myTNB.AndroidApp.Src.Utils;

namespace myTNB.AndroidApp.Src.ServiceDistruption.MVP
{
    public class ServiceDistruptionPresenter : ServiceDistruptionContract.IUserActionsListener
    {
        private ServiceDistruptionContract.IView mView;

        public ServiceDistruptionPresenter(ServiceDistruptionContract.IView mView)
        {
            this.mView = mView;
            this.mView.SetPresenter(this);
        }

        public void Start()
        {
            // NO IMPL
        }

    }
}