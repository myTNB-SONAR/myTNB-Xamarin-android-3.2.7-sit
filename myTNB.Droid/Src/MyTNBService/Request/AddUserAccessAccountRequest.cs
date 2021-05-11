using System;
namespace myTNB_Android.Src.MyTNBService.Request
{
    public class AddUserAccessAccountRequest : BaseRequest
    {
        public string AccountNo, Email, address, AccountName;
        public bool isHaveAccess, isApplyBilling;
        public DeviceInfoRequest deviceInf;

        public AddUserAccessAccountRequest(string email, string accNum, bool ishaveAccess, bool isapplyBilling, string accAddress, string accName)
        {
            deviceInf = new DeviceInfoRequest();
            this.Email = email;
            this.AccountNo = accNum;
            this.isHaveAccess = ishaveAccess;
            this.isApplyBilling = isapplyBilling;
            this.address = accAddress;
            this.AccountName = accName;
        }
    }
}
