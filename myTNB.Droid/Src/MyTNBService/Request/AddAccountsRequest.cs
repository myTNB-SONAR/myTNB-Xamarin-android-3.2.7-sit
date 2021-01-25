using System;
using System.Collections.Generic;

namespace myTNB_Android.Src.MyTNBService.Request
{
    public class AddAccountsRequest : BaseRequest
    {
        public bool isHaveAccess, isApplyBilling;
        public List<AddAccount.Models.AddAccount> billAccounts;
        public AddAccountsRequest(List<AddAccount.Models.AddAccount> accountList)
        {
            billAccounts = accountList;
            this.isHaveAccess = false;
            this.isApplyBilling = false;
        }
    }
}
