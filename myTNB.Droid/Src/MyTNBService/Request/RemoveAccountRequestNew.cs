﻿using System;
using System.Collections.Generic;

namespace myTNB.AndroidApp.Src.MyTNBService.Request
{
    public class RemoveAccountRequestNew : BaseRequestV4
    {
        public string AccountNo;
        //public bool IsPreRegister;
        public DeviceInfoRequest deviceInf;
        public List<ManageAccess.Models.DeleteAccessAccount> removeAccounts;

        public RemoveAccountRequestNew(List<ManageAccess.Models.DeleteAccessAccount> accountList, string accNum)
        {
            deviceInf = new DeviceInfoRequest();
            this.AccountNo = accNum;
            //this.IsPreRegister = isPreRegister;
            removeAccounts = accountList;

        }

    }
}

