using myTNB.AndroidApp.Src.EnergyBudgetRating.Model;
using myTNB.AndroidApp.Src.MyTNBService.Request;
using Refit;
using System.Collections.Generic;

namespace myTNB.AndroidApp.Src.EnergyBudgetRating.Request
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