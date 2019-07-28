using System;
using System.Collections.Generic;

namespace myTNB_Android.Src.SSMR.SMRApplication.MVP
{
    public class OnBoardingSMRPresenter
    {
        List<OnBoardingDataModel> onBoardingDataModelList;
        public OnBoardingSMRPresenter()
        {
            onBoardingDataModelList = new List<OnBoardingDataModel>();
            for (int i = 0; i < 3; i++)
            {
                OnBoardingDataModel model = new OnBoardingDataModel();
                model.ImageURL = "onboarding_bg_"+(i+1);
                model.Title = "Title";
                model.Description = "Description";
                this.onBoardingDataModelList.Add(model);
            }
        }

        public List<OnBoardingDataModel> GetOnBoardingSMRData()
        {
            return this.onBoardingDataModelList;
        }
    }
}
