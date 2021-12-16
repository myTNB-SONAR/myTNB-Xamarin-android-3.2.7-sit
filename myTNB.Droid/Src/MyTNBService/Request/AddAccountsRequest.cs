using myTNB.Mobile;
using System;
using System.Collections.Generic;
using static myTNB.Mobile.FeatureInfoClass;

namespace myTNB_Android.Src.MyTNBService.Request
{
    public class AddAccountsRequest : BaseRequest
    {
        public List<AddAccount.Models.AddAccountV2> billAccounts;
        public DeviceInfoRequest deviceInf;
        //public List<FeatureInfo> featureInfo;

        public AddAccountsRequest(List<AddAccount.Models.AddAccountV2> accountList)
        {
            billAccounts = accountList;
            deviceInf = new DeviceInfoRequest();
            //this.featureInfo = FeatureInfoManager.Instance.GetFeatureInfo();
        }
    }
}
