﻿using System;
using myTNB.Android.Src.Base.Models;

namespace myTNB.Android.Src.MyTNBService.Response
{
    public class SubmittedFeedbackDetailsResponse : BaseResponse<SubmittedFeedbackDetails>
    {
        
        public SubmittedFeedbackDetails GetData()
        {
            return Response.Data;
        }
    }
}
