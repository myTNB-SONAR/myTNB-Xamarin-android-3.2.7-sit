using System;
namespace myTNB_Android.Src.MyTNBService.Request
{
    public class RemoveAccountRequestNew : BaseRequestV2
    {
        public string accNum;
        public bool isHaveAccess, isApplyBilling;
        public DeviceInfoRequest deviceInf;

        public RemoveAccountRequestNew(string accNum, bool isHaveAccess, bool isApplyBilling)
        {
            deviceInf = new DeviceInfoRequest();
            this.accNum = accNum;
            this.isHaveAccess = isHaveAccess;
            this.isApplyBilling = isApplyBilling;
        }
    }
}
