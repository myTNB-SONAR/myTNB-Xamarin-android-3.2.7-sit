using System;
using System.Collections.Generic;
using myTNB.Android.Src.SSMR.SMRApplication.Api;

namespace myTNB.Android.Src.SSMRMeterHistory.Api
{
    public class GetAccountListSMREligibilityRequest : BaseRequest
    {
        public List<string> contractAccounts;
        public GetAccountListSMREligibilityRequest(List<string> contractAccountList)
        {
            contractAccounts = contractAccountList;
        }
    }
}
