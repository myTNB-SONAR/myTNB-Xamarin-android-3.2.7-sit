using myTNB.Android.Src.MyTNBService.Request;
using System;
namespace myTNB.Android.Src.ServiceDistruptionRating.Request
{
    public class GetFeedbackCountRequest : BaseRequest
    {
        public string EventID;

        public GetFeedbackCountRequest(string sdEventID)
        {
            EventID = sdEventID;
        }
    }
}
