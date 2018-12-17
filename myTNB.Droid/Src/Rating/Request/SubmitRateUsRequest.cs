using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Refit;
using Newtonsoft.Json;

namespace myTNB_Android.Src.Rating.Request
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