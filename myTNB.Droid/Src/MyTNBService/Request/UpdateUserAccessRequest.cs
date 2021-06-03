using System;
namespace myTNB_Android.Src.MyTNBService.Request
{
    public class UpdateUserAccessRequest : BaseRequestV2
    {
        public string accountId, userId, action, AccountNo, AccountName, tenantEmail;
        public bool isHaveAccess, isApplyBilling;
        public DeviceInfoRequest deviceInf;

        public UpdateUserAccessRequest(string accountId, string userId, bool haveaccess, bool isEbilling, string actionLog, string accNo, string accName, string email)
        {
            deviceInf = new DeviceInfoRequest();
            this.accountId = accountId;
            this.userId = userId;
            this.isHaveAccess = haveaccess;
            this.isApplyBilling = isEbilling;
            this.action = actionLog;
            this.AccountNo = accNo;
            this.AccountName = accName;
            this.tenantEmail = email;
        }
    }
}
