using System;
using System.Collections.Generic;

namespace myTNB.AndroidApp.Src.MyTNBService.Request
{
    public class SubmitRateUsRequest : BaseRequest
    {
        public InputAnswerData InputAnswer;

        public SubmitRateUsRequest(string referenceId, string email, string deviceId)
        {
            InputAnswer = new InputAnswerData(referenceId, email, deviceId);
        }

        public void AddAnswerDetails(string wLTYQuestionId, string ratingInput, string multilineInput)
        {
            InputAnswer.InputAnswerDetails.Add(new InputAnswerDetailList(wLTYQuestionId, ratingInput, multilineInput));
        }

        public class InputAnswerData
        {
            public string ReferenceId, Email, DeviceId;
            public List<InputAnswerDetailList> InputAnswerDetails;

            public InputAnswerData(string referenceId, string email, string deviceId)
            {
                ReferenceId = referenceId;
                Email = email;
                DeviceId = deviceId;
                InputAnswerDetails = new List<InputAnswerDetailList>();
            }
        }

        public class InputAnswerDetailList
        {
            public string WLTYQuestionId, RatingInput, MultilineInput;

            public InputAnswerDetailList(string wLTYQuestionId, string ratingInput, string multilineInput)
            {
                WLTYQuestionId = wLTYQuestionId;
                RatingInput = ratingInput;
                MultilineInput = multilineInput;
            }
        }
    }
}
