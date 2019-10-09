using System;
using System.Collections.Generic;
using myTNB.SitecoreCMS.Model;
using myTNB.SQLite.SQLiteDataManager;
using myTNB_Android.Src.Database.Model;
using Newtonsoft.Json;
using myTNB_Android.Src.SitecoreCMS.Model;
using myTNB_Android.Src.SSMR.SMRApplication.Api;
using myTNB_Android.Src.Utils;
using System.Threading.Tasks;

namespace myTNB_Android.Src.SSMR.SMRApplication.MVP
{
    public class OnBoardingSMRPresenter
    {
        List<OnBoardingDataModel> onBoardingDataModelList;
        OnBoardingSMRContract.IView mView;
        public OnBoardingSMRPresenter(OnBoardingSMRContract.IView view)
        {
            this.mView = view;
            onBoardingDataModelList = new List<OnBoardingDataModel>();
            for (int i = 0; i < 3; i++)
            {
                OnBoardingDataModel model = new OnBoardingDataModel();
                model.ImageURL = "onboarding_bg_" + (i + 1);
                model.Title = "Title";
                model.Description = "Description";
                this.onBoardingDataModelList.Add(model);
            }
        }
        
        public List<OnBoardingDataModel> GetOnBoardingSMRData()
        {
            return this.onBoardingDataModelList;
        }

        public void GetCARegisteredContactInfo()
        {
            this.mView.StartSMRApplication();
        }
    }
}
