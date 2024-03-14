using System;
using myTNB.Android.Src.Base.Request;

namespace myTNB.Android.Src.MyTNBService.Request
{
    public class AccountBillPayHistoryRequest : BaseRequest
    {
        public string contractAccount;
        public bool isOwnedAccount;
        public string accountType;
        public AccountBillPayHistoryRequest(string contractAccountValue, bool isOwnedAccountValue, string accountTypeValue)
        {
            contractAccount = contractAccountValue;
            isOwnedAccount = isOwnedAccountValue;
            accountType = accountTypeValue;
        }
    }
}
