using System;
using System.Collections.Generic;
using myTNB_Android.Src.Base.MVP;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.NewAppTutorial.MVP;
using myTNB_Android.Src.SSMR.SMRApplication.MVP;

namespace myTNB_Android.Src.SSMRMeterHistory.MVP
{
    public class SSMRMeterHistoryContract
    {
        public interface IView : IExceptionView
        {
            void ShowProgressDialog();
            void HideProgressDialog();
            void ShowSMREligibleAccountList(List<SMRAccount> smrAccountList);
            void UpdateUIForSMR(SMRActivityInfoResponse activityInfoResponse);
            void ShowRefreshScreen(bool isShow);
            void ShowEnableDisableSMR(CAContactDetailsModel contactDetailsModel);
            void EnableButton();
            void ShowContactNotAvailableTooltip(string title, string content, string cta);
            string GetSMRActionKey();
            void OnShowSMRMeterReadingDialog();
            void OnCheckBCRMDowntime();
            void ShowSSMRTerminateActivity();
            void ShowSubmitMeterReadingActivity();
            void ShowSSMRStartActivity();
            void RestartSMRActivity();
            void ProceedToIU(List<SMRAccount> smrEligibleAccountList);
            string GetDeviceId();
        }

        public interface IPresenter
        {
            void CheckSMRAccountEligibility(List<SMRAccount> smrAccountList);
            void GetSSMRAccountStatus(string accountNumber);
            void GetCARegisteredContactInfoAsync(AccountData selectedAccount);
            List<NewAppModel> OnGeneraNewAppTutorialList(bool isSMR);
            void CheckIsBtnSubmitHide(SMRActivityInfoResponse SMRAccountActivityInfoResponse);
            void GetDownTime(string smrtypeBtn);
            void GetEligibleSMRAccountList();
        }

        public interface IApiNotification
        {
            //Task<CARegisteredContactInfoResponse> GetCARegisteredContactInfo(object requestObject);
        }
    }
}
