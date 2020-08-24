using System;
using myTNB_Android.Src.Base.Models;

namespace myTNB_Android.Src.MyTNBService.Response
{
    public class SubmittedFeedbackDetailsResponse : BaseResponse<SubmittedFeedbackDetails>
    {
        
        public SubmittedFeedbackDetails GetData()
        {
            return Response.Data;
        }
    }
}
