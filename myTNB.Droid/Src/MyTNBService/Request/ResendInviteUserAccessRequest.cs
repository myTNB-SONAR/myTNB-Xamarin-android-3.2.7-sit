using System;
namespace myTNB.Android.Src.MyTNBService.Request
{
    public class ResendInviteUserAccessRequest : BaseRequestV4
    {
        public string Email;
        public string AccountNo;
        public bool isHaveAccess;
        public bool isApplyBilling;
        public DeviceInfoRequest deviceInf;

        public ResendInviteUserAccessRequest(string Email, string AccountNo, bool isHaveAccess, bool isApplyBilling)
        {
            deviceInf = new DeviceInfoRequest();
            this.Email = Email;
            this.AccountNo = AccountNo;
            this.isHaveAccess = isHaveAccess;
            this.isApplyBilling = isApplyBilling;
        }
    }
}
