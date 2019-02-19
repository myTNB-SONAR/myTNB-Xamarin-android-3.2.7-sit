using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using myTNB.SitecoreCMS.Services;
using myTNB_Android.Src.SiteCore;
using Newtonsoft.Json;
using myTNB_Android.Src.SitecoreCMS.Model;
using myTNB_Android.Src.Database.Model;
using Android.Util;
using myTNB_Android.Src.Utils;

namespace myTNB_Android.Src.TermsAndConditions.MVP
{
    public class TermsAndConditionPresenter : TermsAndConditionContract.IUserActionsListener
    {
        private TermsAndConditionContract.IView mView;

        public TermsAndConditionPresenter(TermsAndConditionContract.IView view)
        {
            this.mView = view;
            this.mView.SetPresenter(this);
        }

        public Task GetTermsAndConditionData()
        {
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    string density = DPUtils.GetDeviceDensity(Application.Context);
                    GetItemsService getItemsService = new GetItemsService(SiteCoreConfig.OS, density, SiteCoreConfig.SITECORE_URL, SiteCoreConfig.DEFAULT_LANGUAGE);
                    string json = getItemsService.GetFullRTEPagesItems();
                    FullRTEPagesResponseModel responseModel = JsonConvert.DeserializeObject<FullRTEPagesResponseModel>(json);
                    if (responseModel.Status.Equals("Success"))
                    {
                        FullRTEPagesEntity wtManager = new FullRTEPagesEntity();
                        wtManager.DeleteTable();
                        wtManager.CreateTable();
                        wtManager.InsertListOfItems(responseModel.Data);
                        mView.ShowTermsAndCondition(true);
                        Log.Debug("FullRTEResponse", responseModel.Data.ToString());
                    }
                    else
                    {
                        mView.ShowTermsAndCondition(false);
                    }
                }
                catch (Exception e)
                {
                    Log.Error("API Exception", e.StackTrace);
                }
            });
        }

        public void Start()
        {
            //No Impl
            //GetTermsAndConditionData();
        }
    }
}