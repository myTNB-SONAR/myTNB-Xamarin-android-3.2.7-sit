using System;
namespace myTNB_Android.Src.MyTNBService.Request
{
    public class AddUserAccessAccountRequest : BaseRequestV4
    {
        public string AccountNo, Email;
        public bool isHaveAccess, isApplyBilling;
        public DeviceInfoRequest deviceInf;

        public AddUserAccessAccountRequest(string email, string accNum, bool ishaveAccess, bool isapplyBilling)
        {
            deviceInf = new DeviceInfoRequest();
            this.Email = email;
            this.AccountNo = accNum;
            this.isHaveAccess = ishaveAccess;
            this.isApplyBilling = isapplyBilling;
            //this.address = accAddress;
            //this.AccountName = accName;
        }
    }
}
