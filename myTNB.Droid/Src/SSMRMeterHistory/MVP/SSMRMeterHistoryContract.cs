﻿using System;
using System.Collections.Generic;
using myTNB_Android.Src.Base.MVP;
using myTNB_Android.Src.myTNBMenu.Models;
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
        }

        public interface IPresenter
        {
            List<SMRAccount> GetEligibleSMRAccountList();
            void CheckSMRAccountEligibility(List<SMRAccount> smrAccountList);
            void GetSSMRAccountStatus(string accountNumber);
        }

        public interface IApiNotification
        {
            //Task<CARegisteredContactInfoResponse> GetCARegisteredContactInfo(object requestObject);
        }
    }
}
