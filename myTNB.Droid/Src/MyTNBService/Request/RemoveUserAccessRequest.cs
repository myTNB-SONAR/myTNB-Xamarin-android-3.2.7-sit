using Java.Util;
using System;
namespace myTNB.Android.Src.MyTNBService.Request
{
    public class RemoveUserAccountRequest : BaseRequestV4
    {
        public string AccountNo;
        public bool isHaveAccess, isApplyBilling;
        public DeviceInfoRequest deviceInf;
        public String[] accountIdList;
        public String[] tenantEmail;
        //public ArrayList accountIdList = new ArrayList();

        public RemoveUserAccountRequest(String[] nameList, String[] emailList, string accNo)
        {
            deviceInf = new DeviceInfoRequest();
            this.accountIdList = nameList;
            this.tenantEmail = emailList;
            this.AccountNo = accNo;
            this.isHaveAccess = false;
            this.isApplyBilling = false;
        }
    }
}
