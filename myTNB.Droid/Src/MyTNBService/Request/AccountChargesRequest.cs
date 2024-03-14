using System;
using System.Collections.Generic;
using myTNB.Android.Src.Base.Request;

namespace myTNB.Android.Src.MyTNBService.Request
{
    public class AccountsChargesRequest : BaseRequest
    {
        public List<string> accounts;
        public bool isOwnedAccount;
        public AccountsChargesRequest(List<string> accountsValue, bool isOwnedAccountValue)
        {
            accounts = accountsValue;
            isOwnedAccount = isOwnedAccountValue;
        }
    }
}
