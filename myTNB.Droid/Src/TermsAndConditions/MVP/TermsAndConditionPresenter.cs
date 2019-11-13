using Android.App;
using Android.Util;
using myTNB.SitecoreCMS.Services;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.SiteCore;
using myTNB_Android.Src.SitecoreCMS.Model;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

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
                    GetItemsService getItemsService = new GetItemsService(SiteCoreConfig.OS, density, SiteCoreConfig.SITECORE_URL, LanguageUtil.GetAppLanguage());
                    string json = getItemsService.GetFullRTEPagesItems();
                    //json = json.Replace("\\", string.Empty).Replace("\\n", string.Empty);
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
                    mView.HideProgressBar();
                    Utility.LoggingNonFatalError(e);
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