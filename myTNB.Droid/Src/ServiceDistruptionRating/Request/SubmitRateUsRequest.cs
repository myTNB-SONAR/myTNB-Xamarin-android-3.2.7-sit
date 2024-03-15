using myTNB.AndroidApp.Src.MyTNBService.Request;
using myTNB.AndroidApp.Src.ServiceDistruptionRating.Model;
using System;
using System.Collections.Generic;

namespace myTNB.AndroidApp.Src.ServiceDistruptionRating.Request
{
    public class SubmitRateUsRequest : BaseRequest
    {
        public InputAnswerData InputAnswer;
        public string EventID;
        public string CA;
        public string QuestionCategoryId;

        public SubmitRateUsRequest(string referenceId, string email, string deviceId, List<SubmitDataModel.InputAnswerDetails> inputAnswerDetails,string questionID, string eventId, string caNumber)
        {
            InputAnswer = new InputAnswerData(referenceId, email, deviceId, inputAnswerDetails);
            EventID = eventId;
            CA = caNumber;
            QuestionCategoryId = questionID;
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
