using myTNB_Android.Src.MyTNBService.Request;
using System;
namespace myTNB_Android.Src.ServiceDistruptionRating.Request
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
