using System;
namespace myTNB_Android.Src.MyTNBService.Request
{
    public class CancelInviteUserAccessRequest : BaseRequest
    {
        public string AccountId;
        public DeviceInfoRequest deviceInf;

        public CancelInviteUserAccessRequest(string userid)
        {
            deviceInf = new DeviceInfoRequest();
            this.AccountId = userid;
        }
    }
}
