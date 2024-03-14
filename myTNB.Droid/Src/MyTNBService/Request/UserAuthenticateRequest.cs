using System;
using System.Collections.Generic;

namespace myTNB.Android.Src.MyTNBService.Request
{
    public class UserAuthenticateRequest : BaseRequest
    {
        public DeviceInfoRequest deviceInf;
        public string clientType, password;
        

        public UserAuthenticateRequest(string clientType, string password)
        {
            this.clientType = clientType;
            this.password = password;
            deviceInf = new DeviceInfoRequest();
            
        }
    }
}
