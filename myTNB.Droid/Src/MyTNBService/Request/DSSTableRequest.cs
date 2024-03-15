using System;
namespace myTNB.AndroidApp.Src.MyTNBService.Request
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

