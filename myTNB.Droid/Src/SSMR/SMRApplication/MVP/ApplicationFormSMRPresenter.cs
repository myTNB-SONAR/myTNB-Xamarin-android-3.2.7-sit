using System;
using System.Threading.Tasks;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.SSMR.SMRApplication.Api;
using myTNB_Android.Src.Utils;

namespace myTNB_Android.Src.SSMR.SMRApplication.MVP
{
    public class ApplicationFormSMRPresenter : ApplicationFormSMRContract.IPresenter
    {
        ApplicationFormSMRContract.IView mView;
        ApplicationFormSMRContract.IApiNotification api;
        public ApplicationFormSMRPresenter(ApplicationFormSMRContract.IView view)
        {
            mView = view;
            api = new CARegisteredApiImpl();
        }

        public async Task GetCARegisteredContactInfoAsync(SMRAccount smrAccount)
        {
            UserEntity user = UserEntity.GetActive();
            var newObject = new
            {
                contractAccount = smrAccount.accountNumber,
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
            if (response.Data.ErrorCode == "7200")
            {
                smrAccount.email = response.Data.AccountDetailsData.Email;
                smrAccount.mobileNumber = response.Data.AccountDetailsData.Mobile;
            }

            mView.UpdateSMRInfo(smrAccount);
        }
    }
}
