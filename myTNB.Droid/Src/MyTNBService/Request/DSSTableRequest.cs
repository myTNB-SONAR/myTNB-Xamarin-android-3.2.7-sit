using System;
namespace myTNB.Android.Src.MyTNBService.Request
{
	public class DSSTableRequest : BaseRequest
    {
        public DeviceInfoRequest deviceInf;
        public DSSTableRequest()
        {
            deviceInf = new DeviceInfoRequest();
        }
    }
}

