using System;
using System.Threading.Tasks;
using Android.App;
using Android.Util;
using myTNB.SitecoreCMS.Model;
using myTNB.SitecoreCMS.Services;
using myTNB.SQLite.SQLiteDataManager;
using myTNB.Android.Src.Database.Model;
using myTNB.Android.Src.SiteCore;
using myTNB.Android.Src.Utils;

namespace myTNB.Android.Src.PreLogin.MVP
{
    public class PreLoginPresenter : PreLoginContract.IUserActionsListener
    {
        private PreLoginContract.IView mView;

        public PreLoginPresenter(PreLoginContract.IView mView)
        {
            this.mView = mView;
            this.mView.SetPresenter(this);
        }

        public void NavigateToFeedback()
        {
            this.mView.ShowFeedback();
        }

        public void NavigateToLogin()
        {
            this.mView.ShowLogin();
        }

        public void NavigateToRegister()
        {
            this.mView.ShowRegister();
        }

        public void NavigateToFindUs()
        {
            this.mView.ShowFindUS();
        }

        public void NavigateToCheckStatus()
        {
            this.mView.ShowCheckStatus();
        }

        public void NavigateToCallUs()
        {
            if (WeblinkEntity.HasRecord("TNBCL"))
            {
                this.mView.ShowCallUs(WeblinkEntity.GetByCode("TNBCL"));
            }
        }

        public void Start()
        {
            // NO IMPL
        }

        public Task OnGetPreLoginPromo()
        {
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    string density = DPUtils.GetDeviceDensity(Application.Context);
                    GetItemsService getItemsService = new GetItemsService(SiteCoreConfig.OS, density, SiteCoreConfig.SITECORE_URL, LanguageUtil.GetAppLanguage());
                    PreLoginPromoResponseModel responseModel = getItemsService.GetPreLoginPromoItem();
                    if (responseModel.Status.Equals("Success"))
                    {
                        PreLoginPromoEntity wtManager = new PreLoginPromoEntity();
                        wtManager.DeleteTable();
                        wtManager.CreateTable();
                        wtManager.InsertListOfItems(responseModel.Data);
                        mView.ShowPreLoginPromotion(true);
                        Log.Debug("WalkThroughResponse", responseModel.Data.ToString());
                    }
                    else
                    {
                        mView.ShowPreLoginPromotion(false);
                    }
                }
                catch (Exception e)
                {
                    Log.Error("API Exception", e.StackTrace);
                    Utility.LoggingNonFatalError(e);
                }
            });
        }
    }
}