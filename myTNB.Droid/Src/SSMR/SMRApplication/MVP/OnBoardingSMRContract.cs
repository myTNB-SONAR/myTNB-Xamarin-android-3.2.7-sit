using System;
using System.Threading.Tasks;
using myTNB_Android.Src.SSMR.SMRApplication.Api;

namespace myTNB_Android.Src.SSMR.SMRApplication.MVP
{
    public class OnBoardingSMRContract
    {

        public interface IView
        {
            void StartSMRApplication(string email, string mobileNumber);
        }

        public interface IApiNotification
        {
            Task<CARegisteredContactInfoResponse> GetCARegisteredContactInfo(object requestObject);
        }
    }
}
