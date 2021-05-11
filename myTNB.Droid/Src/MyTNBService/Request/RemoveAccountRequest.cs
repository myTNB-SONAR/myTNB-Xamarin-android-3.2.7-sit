using System;
namespace myTNB_Android.Src.MyTNBService.Request
{
    public class RemoveAccountRequest : BaseRequestV2
    {
        public string accNum;
        public DeviceInfoRequest deviceInf;

        public RemoveAccountRequest(string accNum)
        {
            deviceInf = new DeviceInfoRequest();
            this.accNum = accNum;
        }
    }
}
