using System;
using System.Threading.Tasks;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.SSMR.SMRApplication.Api;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;
using static myTNB_Android.Src.SSMR.SMRApplication.Api.CARegisteredContactInfoResponse;
using static myTNB_Android.Src.SSMR.SMRApplication.Api.SMRregistrationSubmitResponse;

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

        public async void SubmitSMRRegistration(SMRAccount smrAccount,string newPhone, string newEmail, string reason)
        {
            SMRregistrationSubmitRequest request = new SMRregistrationSubmitRequest(smrAccount.accountNumber, smrAccount.mobileNumber, newPhone,
                smrAccount.email, newEmail, SUBMIT_MODE.REGISTER, reason);
            //CARegisteredContactInfoResponse response = await api.SubmitSMRApplication(request);
            SMRregistrationSubmitResponse response = new SMRregistrationSubmitResponse();
            SMRSubmitResponseData accountResponseData = new SMRSubmitResponseData();// response.Data;
            accountResponseData.DisplayTitle = "ERROR";
            string jsonResponseString = JsonConvert.SerializeObject(accountResponseData);
            if (true)//response.Data.ErrorCode == "7200")
            {
                mView.ShowSubmitResult(jsonResponseString);
            }
        }
    }
}
