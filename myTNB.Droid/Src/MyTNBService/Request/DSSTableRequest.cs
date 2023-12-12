using System;
namespace myTNB_Android.Src.MyTNBService.Request
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

