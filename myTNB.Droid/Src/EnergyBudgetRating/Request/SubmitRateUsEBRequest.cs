using myTNB.Android.Src.EnergyBudgetRating.Model;
using myTNB.Android.Src.MyTNBService.Request;
using Refit;
using System.Collections.Generic;

namespace myTNB.Android.Src.EnergyBudgetRating.Request
{
    public class SubmitRateUsEBRequest : BaseRequest
    {
        SubmitDataModel.InputAnswerT InputAnswer;

        public SubmitRateUsEBRequest(SubmitDataModel.InputAnswerT inputAnswer)
        {
            InputAnswer = inputAnswer;
        }
    }
}