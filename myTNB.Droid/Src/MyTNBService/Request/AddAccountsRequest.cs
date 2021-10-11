using System;
using System.Collections.Generic;
using myTNB.Mobile;
using static myTNB.Mobile.FeatureInfoClass;

namespace myTNB_Android.Src.MyTNBService.Request
{
    public class AddAccountsRequest : BaseRequestV4
    {
        public bool isHaveAccess, isApplyBilling;
        public List<AddAccount.Models.AddAccount> billAccounts;
        public string AccountName;
        public DeviceInfoRequest deviceInf;
        public List<FeatureInfo> featureInfo;

        public AddAccountsRequest(List<AddAccount.Models.AddAccount> accountList, string accName)
        {
            deviceInf = new DeviceInfoRequest();
            billAccounts = accountList;
            this.isHaveAccess = false;
            this.isApplyBilling = false;
            this.AccountName = accName;
            this.featureInfo = FeatureInfoManager.Instance.GetFeatureInfo();
        }
    }
}
