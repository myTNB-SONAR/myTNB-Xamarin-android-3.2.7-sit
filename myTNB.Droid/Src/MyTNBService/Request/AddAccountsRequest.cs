using System;
using System.Collections.Generic;

namespace myTNB_Android.Src.MyTNBService.Request
{
    public class AddAccountsRequest : BaseRequest
    {
        public List<AddAccount.Models.AddAccountV2> billAccounts;
        public DeviceInfoRequest deviceInf;
        public AddAccountsRequest(List<AddAccount.Models.AddAccountV2> accountList)
        {
            billAccounts = accountList;
            deviceInf = new DeviceInfoRequest();
        }
    }
}
