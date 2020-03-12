using System;
namespace myTNB_Android.Src.MyTNBService.Request
{
    public class CreateNewUserWithTokenRequest : BaseRequest
    {
        public string displayName, token, password, icNo, mobileNo;
        public DeviceInfoRequest deviceInf;

        public CreateNewUserWithTokenRequest(string displayName, string token, string password, string icNo, string mobileNo)
        {
            this.displayName = displayName;
            this.token = token;
            this.password = password;
            this.icNo = icNo;
            this.mobileNo = mobileNo;
            deviceInf = new DeviceInfoRequest();
        }
    }
}
