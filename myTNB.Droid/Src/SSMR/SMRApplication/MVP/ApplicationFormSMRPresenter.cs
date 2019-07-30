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
        SMRregistrationApiImpl api;
        public ApplicationFormSMRPresenter(ApplicationFormSMRContract.IView view)
        {
            mView = view;
            api = new SMRregistrationApiImpl();
        }

        public async void GetCARegisteredContactInfoAsync(SMRAccount smrAccount)
        {
            SMRregistrationContactInfoRequest request = new SMRregistrationContactInfoRequest(smrAccount.accountNumber,true);
            CARegisteredContactInfoResponse response = await api.GetRegisteredContactInfo(request);
            if (response.Data.ErrorCode == "7200")
            {
                smrAccount.email = response.Data.AccountDetailsData.Email;
                smrAccount.mobileNumber = response.Data.AccountDetailsData.Mobile;
            }

            mView.UpdateSMRInfo(smrAccount);
        }
    }
}
