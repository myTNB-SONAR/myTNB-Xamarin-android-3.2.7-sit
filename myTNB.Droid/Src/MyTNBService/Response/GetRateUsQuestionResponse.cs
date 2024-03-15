using System;
using System.Collections.Generic;
using myTNB.AndroidApp.Src.Rating.Model;

namespace myTNB.AndroidApp.Src.MyTNBService.Response
{
    public class GetRateUsQuestionResponse : BaseResponse<List<RateUsQuestion>>
    {
        public List<RateUsQuestion> GetData()
        {
            return Response.Data;
        }
    }
}
