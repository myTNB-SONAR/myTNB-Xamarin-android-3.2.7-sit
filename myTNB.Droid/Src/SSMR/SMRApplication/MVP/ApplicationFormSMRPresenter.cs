using System;
using System.Threading.Tasks;
using Android.Text;
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
            SMRregistrationSubmitResponse response = await api.SubmitSMRApplication(request);
            string jsonResponseString = JsonConvert.SerializeObject(response);
            if (response.Data.ErrorCode == "7200" && response.Data.AccountDetailsData != null)
            {
                
                mView.ShowSubmitSuccessResult(jsonResponseString);
            }
            else
            {
                mView.ShowSubmitFailedResult(jsonResponseString);
            }
        }

        public void CheckRequiredFields(string mobile_no, string email)
        {

            try
            {
                if (!TextUtils.IsEmpty(mobile_no) && !TextUtils.IsEmpty(email))
                {
                    if (!Utility.IsValidMobileNumber(mobile_no))
                    {
                        this.mView.ShowInvalidMobileNoError();
                        this.mView.DisableRegisterButton();
                        return;
                    }
                    else
                    {
                        this.mView.ClearInvalidMobileError();

                    }

                    this.mView.EnableRegisterButton();
                }
                else
                {
                    this.mView.DisableRegisterButton();
                }
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }
    }
}
