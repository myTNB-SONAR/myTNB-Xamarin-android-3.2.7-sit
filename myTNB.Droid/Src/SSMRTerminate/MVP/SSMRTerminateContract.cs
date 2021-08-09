using System.Collections.Generic;
using System.Threading.Tasks;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.SSMR.SMRApplication.Api;
using myTNB_Android.Src.SSMRTerminate.Api;

namespace myTNB_Android.Src.SSMRTerminate.MVP
{
    public class SSMRTerminateContract
    {
        public interface IView
        {
            void ShowEmptyEmailError();
            void ShowInvalidEmailError();
            void ShowInvalidMobileNoError();
            void DisableSubmitButton();
            void ClearInvalidMobileError();
            void ClearEmailError();
            void EnableSubmitButton();
            void ClearErrors();
            void ShowEmptyReasonError();
            void ClearReasonError();
            void UpdateMobileNumber(string mobile_no);
            string GetDeviceId();
            bool IsActive();
            void ShowProgressDialog();
            void HideProgressDialog();
            void UpdateSMRData(string email, string mobile_no);
            void SetTerminationReasonsList(List<TerminationReasonModel> list);
            void ShowTermsAndConditions();
            void OnRequestSuccessful(SMRregistrationSubmitResponse response);
            void OnRequestFailed(SMRregistrationSubmitResponse response);
            void ShowFAQ();
        }

        public interface IPresenter
        {
            //void CheckRequiredFields(string mobile_no, string email, bool isOtherReasonSelected, string otherReason);

            void InitiateCAInfo(AccountData selectedAccount);

            void InitiateTerminationReasonsList();

            void NavigateToTermsAndConditions();

            void NavigateToFAQ();

            void OnSubmitApplication(string accountNum, string oldEmail, string oldPhoneNum, string newEmail, string newPhoneNum, string terminationReason, string mode);         
        }

        public interface SSMRTerminateApiPresenter
        {
            Task<CARegisteredContactInfoResponse> GetCARegisteredContactInfo(GetRegisteredContactInfoRequest request);

            Task<SMRTerminationReasonsResponse> GetSMRTerminationReasons(GetSMRTerminationReasonsRequest request);

            Task<SMRregistrationSubmitResponse> SubmitSMRApplication(SubmitSMRApplicationRequest request);
        }
    }
}