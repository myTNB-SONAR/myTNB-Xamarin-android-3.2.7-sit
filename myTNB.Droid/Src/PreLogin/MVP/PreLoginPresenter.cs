using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Threading.Tasks;
using myTNB.SitecoreCMS.Services;
using myTNB_Android.Src.SiteCore;
using myTNB.SitecoreCMS.Model;
using Newtonsoft.Json;
using Android.Util;
using myTNB_Android.Src.Database.Model;
using myTNB.SQLite.SQLiteDataManager;
using myTNB_Android.Src.Utils;

namespace myTNB_Android.Src.PreLogin.MVP
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
                    GetItemsService getItemsService = new GetItemsService(SiteCoreConfig.OS, density, SiteCoreConfig.SITECORE_URL, SiteCoreConfig.DEFAULT_LANGUAGE);
                    string json = getItemsService.GetPreLoginPromoItem();
                    PreLoginPromoResponseModel responseModel = JsonConvert.DeserializeObject<PreLoginPromoResponseModel>(json);
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