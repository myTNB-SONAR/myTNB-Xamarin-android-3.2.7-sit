//using myTNB.Mobile;
//using System;
//using System.Collections.Generic;
//using static myTNB.Mobile.FeatureInfoClass;

//namespace myTNB_Android.Src.MyTNBService.Request
//{
//    public class AddAccountsRequest : BaseRequestV2
//    {
//        public bool isHaveAccess, isApplyBilling;
//        public List<AddAccount.Models.AddAccount> billAccounts;
//        public string AccountName;
//        public DeviceInfoRequest deviceInf;
//        public List<Login.Models.FeatureInfo> featureInfo;


//        // public List<AddAccount.Models.AddAccountV2> billAccounts;
//        // public DeviceInfoRequest deviceInf;
//        // public List<FeatureInfo> featureInfo;

//        //public AddAccountsRequest(List<AddAccount.Models.AddAccountV2> accountList)
//        public AddAccountsRequest(List<AddAccount.Models.AddAccount> accountList , string accName)
//        {
//            deviceInf = new DeviceInfoRequest();
//            billAccounts = accountList;
//            this.isHaveAccess = false;
//            this.isApplyBilling = false;
//            this.AccountName = accName;
//        //     featureInfo = new List<Login.Models.FeatureInfo>();
//        //     deviceInf = new DeviceInfoRequest();
//            this.featureInfo = FeatureInfoManager.Instance.GetFeatureInfo();
//        }
//    }
//}


using System;
using System.Collections.Generic;

namespace myTNB_Android.Src.MyTNBService.Request
{
    public class AddAccountsRequest : BaseRequestV2
    {
        public bool isHaveAccess, isApplyBilling;
        public List<AddAccount.Models.AddAccount> billAccounts;
        public string AccountName;
        public DeviceInfoRequest deviceInf;
        public List<Login.Models.FeatureInfo> featureInfo;

        public AddAccountsRequest(List<AddAccount.Models.AddAccount> accountList, string accName)
        {
            deviceInf = new DeviceInfoRequest();
            billAccounts = accountList;
            this.isHaveAccess = false;
            this.isApplyBilling = false;
            this.AccountName = accName;
            featureInfo = new List<Login.Models.FeatureInfo>();
        }
    }
}
