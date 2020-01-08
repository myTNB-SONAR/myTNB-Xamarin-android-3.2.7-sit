using System;
namespace myTNB_Android.Src.MyTNBService.Request
{
    public class RemoveAccountRequest : BaseRequest
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
