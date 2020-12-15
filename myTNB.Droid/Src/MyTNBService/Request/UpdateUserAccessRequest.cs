using System;
namespace myTNB_Android.Src.MyTNBService.Request
{
    public class UpdateUserAccessRequest : BaseRequest
    {
        public string accountId;
        public bool isHaveAccess, isApplyBilling;
        public DeviceInfoRequest deviceInf;

        public UpdateUserAccessRequest(string userid, bool haveaccess, bool isEbilling)
        {
            //deviceInf = new DeviceInfoRequest();
            this.accountId = userid;
            this.isHaveAccess = haveaccess;
            this.isApplyBilling = isEbilling;
        }
    }
}
