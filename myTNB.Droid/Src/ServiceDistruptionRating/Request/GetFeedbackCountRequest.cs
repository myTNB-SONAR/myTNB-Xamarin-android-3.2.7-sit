using myTNB.AndroidApp.Src.MyTNBService.Request;
using System;
namespace myTNB.AndroidApp.Src.ServiceDistruptionRating.Request
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
