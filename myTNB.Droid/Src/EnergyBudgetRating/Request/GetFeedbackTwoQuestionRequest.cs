using myTNB.AndroidApp.Src.MyTNBService.Request;
using System;
namespace myTNB.AndroidApp.Src.EnergyBudgetRating.Request
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
