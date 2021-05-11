using System;
namespace myTNB_Android.Src.MyTNBService.Request
{
    public class UpdateUserAccessRequest : BaseRequestV2
    {
        public string accountId, action, AccountNo, AccountName;
        public bool isHaveAccess, isApplyBilling;
        public DeviceInfoRequest deviceInf;

        public UpdateUserAccessRequest(string userid, bool haveaccess, bool isEbilling, string actionLog, string accNo, string accName)
        {
            deviceInf = new DeviceInfoRequest();
            this.accountId = userid;
            this.isHaveAccess = haveaccess;
            this.isApplyBilling = isEbilling;
            this.action = actionLog;
            this.AccountNo = accNo;
            this.AccountName = accName;
        }
    }
}
