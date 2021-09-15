using System;
using System.Collections.Generic;

namespace myTNB_Android.Src.MyTNBService.Request
{
    public class RemoveAccountRequest : BaseRequestV2
    {
        public string AccountNo;
        //public bool IsPreRegister;
        public DeviceInfoRequest deviceInf;
        public List<ManageAccess.Models.DeleteAccessAccount> removeAccounts;

        public RemoveAccountRequest(List<ManageAccess.Models.DeleteAccessAccount> accountList, string accNum)
        {
            deviceInf = new DeviceInfoRequest();
            this.AccountNo = accNum;
            //this.IsPreRegister = isPreRegister;
            removeAccounts = accountList;
            
        }
        
    }
}
