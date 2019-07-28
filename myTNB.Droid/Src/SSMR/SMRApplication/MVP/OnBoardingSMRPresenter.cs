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
        OnBoardingSMRContract.IApiNotification api;
        OnBoardingSMRContract.IView mView;
        public OnBoardingSMRPresenter(OnBoardingSMRContract.IView view)
        {
            this.mView = view;
            onBoardingDataModelList = new List<OnBoardingDataModel>();
            api = new CARegisteredApiImpl();
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

        public async Task GetCARegisteredContactInfo()
        {
            List<CustomerBillingAccount> customerBillingAccounts = CustomerBillingAccount.EligibleSMRAccountList();
            CustomerBillingAccount customerAccount = customerBillingAccounts[0];
            UserEntity user = UserEntity.GetActive();
            var newObject = new
            {
                contractAccount = customerAccount.AccNum,
                isOwnedAccount = true,
                usrInf = new
                {
                    eid = user.UserName,
                    sspuid = user.UserID,
                    lang = "EN",
                    sec_auth_k1 = Constants.APP_CONFIG.API_KEY_ID,
                    sec_auth_k2 = "test",
                    ses_param1 = "test",
                    ses_param2 = "test"
                }
            };
            CARegisteredContactInfoResponse response = await api.GetCARegisteredContactInfo(newObject);
            string email = response.Data.AccountDetailsData.Email;
            string mobileNumber = response.Data.AccountDetailsData.Mobile;
            this.mView.StartSMRApplication(email, mobileNumber);
        }
    }
}
