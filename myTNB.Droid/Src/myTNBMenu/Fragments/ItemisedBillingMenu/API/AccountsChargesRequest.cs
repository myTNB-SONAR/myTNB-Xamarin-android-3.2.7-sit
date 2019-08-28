using System;
using System.Collections.Generic;
using myTNB_Android.Src.Base.Request;

namespace myTNB_Android.Src.myTNBMenu.Fragments.ItemisedBillingMenu.API
{
    public class AccountsChargesRequest : APIBaseRequest
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
