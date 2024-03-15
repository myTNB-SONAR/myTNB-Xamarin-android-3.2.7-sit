using System;
using System.Collections.Generic;
using myTNB.AndroidApp.Src.Base.Request;

namespace myTNB.AndroidApp.Src.MyTNBService.Request
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
