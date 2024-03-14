using System;
namespace myTNB.Android.Src.MyTNBService.Request
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
