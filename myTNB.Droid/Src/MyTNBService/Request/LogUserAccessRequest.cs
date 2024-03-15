using System;
namespace myTNB.AndroidApp.Src.MyTNBService.Request
{
    public class LogUserAccessRequest : BaseRequestV4
    {
        public string accountNo;
        public DeviceInfoRequest deviceInf;

        public LogUserAccessRequest(string accNum)
        {
            deviceInf = new DeviceInfoRequest();
            this.accountNo = accNum;
        }
    }
}
