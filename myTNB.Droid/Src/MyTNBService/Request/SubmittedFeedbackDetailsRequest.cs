using System;
namespace myTNB_Android.Src.MyTNBService.Request
{
    public class SubmittedFeedbackDetailsRequest : BaseRequest
    {
        public string serviceReqNo;

        public SubmittedFeedbackDetailsRequest(string serviceReqNo)
        {
            this.serviceReqNo = serviceReqNo;
        }
    }
}
