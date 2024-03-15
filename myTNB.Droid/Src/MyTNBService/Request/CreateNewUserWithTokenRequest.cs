using System;
namespace myTNB.AndroidApp.Src.MyTNBService.Request
{
    public class CreateNewUserWithTokenRequest : BaseRequest
    {
        public string displayName, token, password, icNo, mobileNo, ictype;
        public DeviceInfoRequest deviceInf;

        public CreateNewUserWithTokenRequest(string displayName, string token, string password, string icNo, string ictype, string mobileNo)
        {
            this.displayName = displayName;
            this.token = token;
            this.password = password;
            this.icNo = icNo;
            this.ictype = ictype;
            this.mobileNo = mobileNo;
            deviceInf = new DeviceInfoRequest();
        }
    }
}
