using System;
namespace myTNB_Android.Src.MyTNBService.Request
{
    public class UpdateUserInfo : BaseRequest
    {
        public DeviceInfoRequest deviceInf;
        public string name, mobileNo, usrId;

        public UpdateUserInfo(string usrid, string clientType, string fname)
        {
            this.usrId = usrid;
            this.mobileNo = clientType;
            this.name = fname;
        }
    }
}
