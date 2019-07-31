using System.Threading.Tasks;

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
        }

        public interface IPresenter
        {
            void CheckRequiredFields(string mobile_no, string email, bool isOtherReasonSelected, string otherReason);

            void Start();
        }

        /* public interface IApiNotification
        {
            Task<CARegisteredContactInfoResponse> GetCARegisteredContactInfo(object requestObject);
        }*/
    }
}