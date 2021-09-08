using myTNB_Android.Src.MyTNBService.Request;
using System;
namespace myTNB_Android.Src.EnergyBudgetRating.Request
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
