using System;
namespace myTNB_Android.Src.MyTNBService.Request
{
    public class GetVerifyRequest : BaseRequest
    {
        public string usremail;
        //public DeviceInfoRequest deviceInf;

        public GetVerifyRequest(string usremail)
        {
            this.usremail = usremail;
            //deviceInf = new DeviceInfoRequest();

        }
    }
}
