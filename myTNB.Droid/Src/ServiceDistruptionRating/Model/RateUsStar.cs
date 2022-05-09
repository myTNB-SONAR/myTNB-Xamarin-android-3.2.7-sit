using Newtonsoft.Json;
using Refit;
using System.Collections.Generic;

namespace myTNB_Android.Src.ServiceDistruptionRating.Model
{
    public class RateUsStar
    {
        [JsonProperty(PropertyName = "WLTYQuestionId")]
        [AliasAs("WLTYQuestionId")]
        public string WLTYQuestionId { get; set; }

        [JsonProperty(PropertyName = "Question")]
        [AliasAs("Question")]
        public string Question { get; set; }

        [JsonProperty(PropertyName = "QuestionCategory")]
        [AliasAs("QuestionCategory")]
        public string QuestionCategory { get; set; }

        [JsonProperty(PropertyName = "QuestionType")]
        [AliasAs("QuestionType")]
        public string QuestionType { get; set; }

        [JsonProperty(PropertyName = "IsActive")]
        [AliasAs("IsActive")]
        public bool IsActive { get; set; }

        [JsonProperty(PropertyName = "IsMandatory")]
        [AliasAs("IsMandatory")]
        public bool IsMandatory { get; set; }

        [JsonProperty(PropertyName = "InputOptionValue")]
        [AliasAs("InputOptionValue")]
        public List<InputOptionValue> InputOptionValueList { get; set; }

        public bool IsQuestionAnswered = false;
        public string InputAnswer;
        public string InputRating;

        public class InputOptionValue
        {
            [JsonProperty(PropertyName = "InputOptionRate")]
            [AliasAs("InputOptionRate")]
            public string InputOptionRate { get; set; }

            [JsonProperty(PropertyName = "InputOptionValues")]
            [AliasAs("InputOptionValues")]
            public string InputOptionValues { get; set; }
        }
    }
}