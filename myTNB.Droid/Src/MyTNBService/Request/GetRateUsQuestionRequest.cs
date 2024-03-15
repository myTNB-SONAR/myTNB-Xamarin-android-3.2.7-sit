using System;
namespace myTNB.AndroidApp.Src.MyTNBService.Request
{
    public class GetRateUsQuestionRequest : BaseRequest
    {
        public string QuestionCategoryId;

        public GetRateUsQuestionRequest(string questionCategoryId)
        {
            QuestionCategoryId = questionCategoryId;
        }
    }
}
