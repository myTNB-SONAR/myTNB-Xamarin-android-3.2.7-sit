using System;
namespace myTNB_Android.Src.MyTNBService.Request
{
    public class RemoveAccountRequest : BaseRequest
    {
        public string accNum;
        public bool isTaggedSmartMeter;
        public DeviceInfoRequest deviceInf;

        public RemoveAccountRequest(string accNum, bool tagSM)
        {
            deviceInf = new DeviceInfoRequest();
            this.accNum = accNum;
        }
    }
}
