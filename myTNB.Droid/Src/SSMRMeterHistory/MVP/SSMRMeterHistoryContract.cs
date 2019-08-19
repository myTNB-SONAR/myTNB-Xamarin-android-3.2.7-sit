using System;
namespace myTNB_Android.Src.SSMRMeterHistory.MVP
{
    public class SSMRMeterHistoryContract
    {
        public interface IView
        {
            void ShowProgressDialog();
            void HideProgressDialog();
            void ShowSMREligibleAccountList();
        }

        public interface IPresenter
        {
            void CheckSMRAccountEligibility();
        }

        public interface IApiNotification
        {
            //Task<CARegisteredContactInfoResponse> GetCARegisteredContactInfo(object requestObject);
        }
    }
}
