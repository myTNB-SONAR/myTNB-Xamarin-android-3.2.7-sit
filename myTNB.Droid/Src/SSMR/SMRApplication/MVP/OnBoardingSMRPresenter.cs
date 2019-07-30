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

        public void OnBoardingList()
        {
            OnboardingSSMREntity entity = new OnboardingSSMREntity();
            List<OnboardingSSMREntity> items = entity.GetAllItems();
            if (items.Count > 0)
            {
                onBoardingDataModelList = new List<OnBoardingDataModel>();
                foreach (OnboardingSSMRModel model in items)
                {
                    OnBoardingDataModel dataModel = new OnBoardingDataModel();
                    dataModel.ImageURL = model.Image;
                    dataModel.Title = model.Title;
                    dataModel.Description = model.Description;
                    this.onBoardingDataModelList.Add(dataModel);
                }
            }
        }

        public void GetCARegisteredContactInfo()
        {
            List<CustomerBillingAccount> customerBillingAccounts = CustomerBillingAccount.EligibleSMRAccountList();
            List<SMRAccount> smrAccountList = new List<SMRAccount>();
            if (customerBillingAccounts.Count > 0)
            {
                foreach (CustomerBillingAccount billingAccount in customerBillingAccounts)
                {
                    SMRAccount smrAccount = new SMRAccount();
                    smrAccount.accountNumber = billingAccount.AccNum;
                    smrAccount.accountName = billingAccount.AccDesc;
                    smrAccount.accountAddress = billingAccount.AccountStAddress;
                    smrAccount.accountSelected = false;
                    smrAccountList.Add(smrAccount);
                }
                smrAccountList[0].accountSelected = true; //Default Selection
            }

            UserSessions.SetSMRAccountList(smrAccountList);
            this.mView.StartSMRApplication();
        }
    }
}
