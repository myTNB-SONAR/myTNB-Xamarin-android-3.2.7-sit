﻿using System;
using System.Threading.Tasks;
using myTNB_Android.Src.SSMR.SMRApplication.Api;

namespace myTNB_Android.Src.SSMR.SMRApplication.MVP
{
    public class ApplicationFormSMRContract
    {
        public interface IView
        {
            void ShowSelectAccount();
            void UpdateSMRInfo(SMRAccount account);
        }

        public interface IPresenter
        {
        }

        public interface IApiNotification
        {
            Task<CARegisteredContactInfoResponse> GetCARegisteredContactInfo(object requestObject);
        }
    }
}
