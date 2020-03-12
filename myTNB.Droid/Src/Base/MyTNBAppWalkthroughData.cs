using System;
using System.Collections.Generic;
using myTNB.SitecoreCMS.Model;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.SSMR.SMRApplication.MVP;
using Newtonsoft.Json;

namespace myTNB_Android.Src.Base
{
    public class MyTNBAppWalkthroughData
    {
        private static MyTNBAppWalkthroughData Instance;
        private List<OnBoardingDataModel> onBoardingDataModelList;
        public MyTNBAppWalkthroughData(){}

        public static MyTNBAppWalkthroughData GetInstance()
        {
            if (Instance == null)
            {
                Instance = new MyTNBAppWalkthroughData();
            }
            return Instance;
        }

        private List<ApplySSMRModel> GetApplySSMROnboardingLocalList()
        {
            List<ApplySSMRModel> applySSMROnboardingLocalList = new List<ApplySSMRModel>();
            ApplySSMRModel applySSMRModel;

            applySSMRModel = new ApplySSMRModel();
            applySSMRModel.Image = "image_local_1";
            applySSMRModel.Title = "What is Self Meter Reading?";
            applySSMRModel.Description = "Self Meter Reading is a service you can use to easily read and submit your own meter readings. We will send you a reminder when it's time to submit.";
            applySSMROnboardingLocalList.Add(applySSMRModel);

            applySSMRModel = new ApplySSMRModel();
            applySSMRModel.Image = "image_local_2";
            applySSMRModel.Title = "Why should I start this service?";
            applySSMRModel.Description = "You get estimated bills when TNB meter readers can’t get access to your meter. With this service, you will always get bills with accurate readings.";
            applySSMROnboardingLocalList.Add(applySSMRModel);

            return applySSMROnboardingLocalList;
        }

        public List<ApplySSMRModel> GetApplySSMROnboardingList()
        {
            List<ApplySSMRModel> applySSMRModelLocalList = GetApplySSMROnboardingLocalList();
            List<ApplySSMRModel> applySSMRModelList = JsonConvert.DeserializeObject<List<ApplySSMRModel>>(SitecoreCmsEntity.GetItemById(SitecoreCmsEntity.SITE_CORE_ID.APPLY_SSMR_WALKTHROUGH));

            if (applySSMRModelList != null && applySSMRModelList.Count > 0)
            {
                for (int i=0; i < applySSMRModelList.Count; i++)
                {
                    applySSMRModelList[i].Image = !string.IsNullOrEmpty(applySSMRModelList[i].Image) ? applySSMRModelList[i].Image : applySSMRModelLocalList[i].Image;
                    applySSMRModelList[i].Title = !string.IsNullOrEmpty(applySSMRModelList[i].Title) ? applySSMRModelList[i].Title : applySSMRModelLocalList[i].Title;
                    applySSMRModelList[i].Description = !string.IsNullOrEmpty(applySSMRModelList[i].Description) ? applySSMRModelList[i].Description : applySSMRModelLocalList[i].Description;
                }
                return applySSMRModelList;
            }
            else
            {
                return applySSMRModelLocalList;
            }
        }
    }
}
