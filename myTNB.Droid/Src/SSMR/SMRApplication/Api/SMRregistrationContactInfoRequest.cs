using System;
namespace myTNB.Android.Src.SSMR.SMRApplication.Api
{
    public class SMRregistrationContactInfoRequest : BaseRequest
    {
        public string contractAccount;
        public bool isOwnedAccount;
        public SMRregistrationContactInfoRequest(string contractAccountValue, bool isOwnedAccountValue)
        {
            contractAccount = contractAccountValue;
            isOwnedAccount = isOwnedAccountValue;
        }
    }
}
