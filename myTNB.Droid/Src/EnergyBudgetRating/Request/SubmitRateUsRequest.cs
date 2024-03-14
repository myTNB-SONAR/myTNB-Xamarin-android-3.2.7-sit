using myTNB.Android.Src.EnergyBudgetRating.Model;
using myTNB.Android.Src.MyTNBService.Request;
using System;
using System.Collections.Generic;

namespace myTNB.Android.Src.EnergyBudgetRating.Request
{
    public class SubmitRateUsRequest : BaseRequest
    {
        public InputAnswerData InputAnswer;
        public string QuestionCategoryId;

        public SubmitRateUsRequest(string referenceId, string email, string deviceId, List<SubmitDataModel.InputAnswerDetails> inputAnswerDetails, string questionCategoryId)
        {
            InputAnswer = new InputAnswerData(referenceId, email, deviceId, inputAnswerDetails);
            QuestionCategoryId = questionCategoryId;
        }

        public class InputAnswerData
        {
            public string ReferenceId, Email, DeviceId;
            public List<SubmitDataModel.InputAnswerDetails> InputAnswerDetails;

            public InputAnswerData(string referenceId, string email, string deviceId, List<SubmitDataModel.InputAnswerDetails> inputAnswerDetails)
            {
                ReferenceId = referenceId;
                Email = email;
                DeviceId = deviceId;
                InputAnswerDetails = inputAnswerDetails;
            }
        }
    }
}
