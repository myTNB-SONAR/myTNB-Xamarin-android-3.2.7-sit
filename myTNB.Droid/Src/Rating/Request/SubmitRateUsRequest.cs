using Refit;
using System.Collections.Generic;

namespace myTNB.Android.Src.Rating.Request
{
    public class SubmitRateUsRequest
    {
        [AliasAs("ApiKeyID")]
        public string ApiKeyID { get; set; }

        [AliasAs("InputAnswer")]
        public InputAnswerT InputAnswer { get; set; }

        public class InputAnswerT
        {

            public InputAnswerT()
            {

            }

            [AliasAs("ReferenceId")]
            public string ReferenceId { get; set; }

            [AliasAs("Email")]
            public string Email { get; set; }

            [AliasAs("DeviceId")]
            public string DeviceId { get; set; }

            [AliasAs("DeviceId")]
            public List<InputAnswerDetails> InputAnswerDetails { get; set; }

        }

        public class InputAnswerDetails
        {

            public InputAnswerDetails()
            {

            }

            [AliasAs("WLTYQuestionId")]
            public string WLTYQuestionId { get; set; }

            [AliasAs("RatingInput")]
            public string RatingInput { get; set; }

            [AliasAs("MultilineInput")]
            public string MultilineInput { get; set; }

        }
    }
}