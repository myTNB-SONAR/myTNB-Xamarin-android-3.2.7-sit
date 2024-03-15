using System;
namespace myTNB.AndroidApp.Src.MyTNBService.Request
{
    public class SubmittedFeedbackListRequest : BaseRequest
    {
        public DeviceInfoRequest deviceInf;
        public SubmittedFeedbackListRequest()
        {
            deviceInf = new DeviceInfoRequest();
        }
    }
}
