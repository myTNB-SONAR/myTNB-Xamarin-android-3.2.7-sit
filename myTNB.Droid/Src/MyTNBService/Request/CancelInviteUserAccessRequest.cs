using System;
namespace myTNB.Android.Src.MyTNBService.Request
{
    public class CancelInviteUserAccessRequest : BaseRequestV4
    {
        public string Email;
        public string AccountId;
        public string AccountNo;
        public DeviceInfoRequest deviceInf;

        public CancelInviteUserAccessRequest(string Email, string AccountNo, string userid)
        {
            deviceInf = new DeviceInfoRequest();
            this.Email = Email;
            this.AccountNo = AccountNo;
            this.AccountId = userid;
        }
    }
}
