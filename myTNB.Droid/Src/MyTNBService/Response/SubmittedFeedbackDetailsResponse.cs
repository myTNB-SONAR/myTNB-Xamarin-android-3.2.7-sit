using System;
using myTNB.AndroidApp.Src.Base.Models;

namespace myTNB.AndroidApp.Src.MyTNBService.Response
{
    public class SubmittedFeedbackDetailsResponse : BaseResponse<SubmittedFeedbackDetails>
    {
        
        public SubmittedFeedbackDetails GetData()
        {
            return Response.Data;
        }
    }
}
