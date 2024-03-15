using System;
using myTNB.AndroidApp.Src.myTNBMenu.Models;

namespace myTNB.AndroidApp.Src.Base
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
