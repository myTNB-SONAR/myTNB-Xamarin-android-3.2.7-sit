using Java.Util;
using System;
namespace myTNB_Android.Src.MyTNBService.Request
{
    public class RemoveUserAccountRequest : BaseRequestV2
    {
        public string AccountNo;
        public bool isHaveAccess, isApplyBilling;
        public DeviceInfoRequest deviceInf;
        public String[] accountIdList;
        //public ArrayList accountIdList = new ArrayList();

        public RemoveUserAccountRequest(String[] nameList, string accNo)
        {
            deviceInf = new DeviceInfoRequest();
            this.accountIdList = nameList;
            this.AccountNo = accNo;
            this.isHaveAccess = false;
            this.isApplyBilling = false;
        }
    }
}
