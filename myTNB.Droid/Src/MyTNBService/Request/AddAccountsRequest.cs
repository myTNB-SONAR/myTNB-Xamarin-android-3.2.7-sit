using System;
using System.Collections.Generic;

namespace myTNB_Android.Src.MyTNBService.Request
{
    public class AddAccountsRequest : BaseRequest
    {
        public List<AddAccount.Models.AddAccount> billAccounts;
        public AddAccountsRequest(List<AddAccount.Models.AddAccount> accountList)
        {
            billAccounts = accountList;
        }
    }
}
