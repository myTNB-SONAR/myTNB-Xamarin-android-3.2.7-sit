using System;
namespace myTNB_Android.Src.MyTNBService.Request
{
    public class RemoveAccountRequestNew : BaseRequestV2
    {
        public string accNum;
        public DeviceInfoRequest deviceInf;

        public RemoveAccountRequestNew(string accNum)
        {
            deviceInf = new DeviceInfoRequest();
            this.accNum = accNum;
            
        }
    }
}
