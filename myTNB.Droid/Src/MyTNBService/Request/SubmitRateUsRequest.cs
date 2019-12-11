using System;
using System.Collections.Generic;

namespace myTNB_Android.Src.MyTNBService.Request
{
    public class SubmitRateUsRequest : BaseRequest
    {
        public string ReferenceId, Email, DeviceId;
        public List<InputAnswerDetails> inputAnswerDetails;

        public SubmitRateUsRequest(string referenceId, string email, string deviceId)
        {
            ReferenceId = referenceId;
            Email = email;
            DeviceId = deviceId;
            inputAnswerDetails = new List<InputAnswerDetails>();
        }

        public void AddAnswerDetails(string wLTYQuestionId, string ratingInput, string multilineInput)
        {
            inputAnswerDetails.Add(new InputAnswerDetails(wLTYQuestionId, ratingInput, multilineInput));
        }

        public class InputAnswerDetails
        {
            public string WLTYQuestionId, RatingInput, MultilineInput;

            public InputAnswerDetails(string wLTYQuestionId, string ratingInput, string multilineInput)
            {
                WLTYQuestionId = wLTYQuestionId;
                RatingInput = ratingInput;
                MultilineInput = multilineInput;
            }
        }
    }
}
