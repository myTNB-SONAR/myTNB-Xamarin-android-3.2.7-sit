using System;
using myTNB_Android.Src.myTNBMenu.Models;

namespace myTNB_Android.Src.Base
{
    public class SMRAccountActivityInfo
    {
        private string mAccountNumber;
        private SMRActivityInfoResponse mActivityInfoResponse;
        public SMRAccountActivityInfo(string accountNumber, SMRActivityInfoResponse activityInfoResponse)
        {
            mAccountNumber = accountNumber;
            mActivityInfoResponse = activityInfoResponse;
        }

        public string GetAccountNumber()
        {
            return mAccountNumber;
        }
        public SMRActivityInfoResponse GetActivityInfoResponse()
        {
            return mActivityInfoResponse;
        }

        public void SetActivityInfoResponse(SMRActivityInfoResponse activityInfoResponse)
        {
            this.mActivityInfoResponse = activityInfoResponse;
        }
    }
}
