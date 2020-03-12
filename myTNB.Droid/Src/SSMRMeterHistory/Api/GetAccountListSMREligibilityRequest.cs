using System;
using System.Collections.Generic;
using myTNB_Android.Src.SSMR.SMRApplication.Api;

namespace myTNB_Android.Src.SSMRMeterHistory.Api
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
