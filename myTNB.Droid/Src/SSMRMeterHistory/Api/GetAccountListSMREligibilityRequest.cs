using System;
using System.Collections.Generic;
using myTNB.AndroidApp.Src.SSMR.SMRApplication.Api;

namespace myTNB.AndroidApp.Src.SSMRMeterHistory.Api
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
