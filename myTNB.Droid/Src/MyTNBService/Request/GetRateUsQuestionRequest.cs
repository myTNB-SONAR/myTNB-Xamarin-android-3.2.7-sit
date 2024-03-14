using System;
namespace myTNB.Android.Src.MyTNBService.Request
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
