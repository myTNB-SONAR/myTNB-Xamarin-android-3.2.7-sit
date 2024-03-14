using myTNB.Android.Src.MyTNBService.Request;
using System;
namespace myTNB.Android.Src.EnergyBudgetRating.Request
{
    public class GetFeedbackTwoQuestionRequest : BaseRequest
    {
        public string QuestionCategoryId;
        public int[] QuestionCategoryIdGrpList;

        public GetFeedbackTwoQuestionRequest(int[] idNumber)
        {
            QuestionCategoryIdGrpList = idNumber;
        }
    }
}
