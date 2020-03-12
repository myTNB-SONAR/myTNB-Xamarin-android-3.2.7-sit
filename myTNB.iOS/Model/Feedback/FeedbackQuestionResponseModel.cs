using System.Collections.Generic;
using myTNB.Enums;
using Newtonsoft.Json;

namespace myTNB.Model
{
    public class FeedbackQuestionResponseModel
    {
        public FeedbackQuestionDataModel d { set; get; }
    }

    public class FeedbackQuestionDataModel : BaseModelV2
    {
        public List<FeedbackQuestionModel> data { set; get; }
    }

    public class FeedbackQuestionModel
    {
        string _isActive;
        string _isMandatory;

        public string WLTYQuestionId { get; set; }
        public string Question { get; set; }
        public string QuestionCategory { get; set; }
        public string QuestionType { get; set; }
        public List<QuestionInputOption> InputOptionValue { get; set; }

        [JsonIgnore]
        public string Answer { get; set; }

        public string IsActive
        {
            set
            {
                if (bool.TryParse(value, out bool parsed))
                {
                    _isActive = value;
                }
                else
                {
                    _isActive = "false";
                }
            }
            get
            {
                return _isActive ?? "false";
            }
        }

        public string IsMandatory
        {
            set
            {
                if (bool.TryParse(value, out bool parsed))
                {
                    _isMandatory = value;
                }
                else
                {
                    _isMandatory = "false";
                }
            }
            get
            {
                return _isMandatory ?? "false";
            }
        }

        [JsonIgnore]
        public bool Active
        {
            get
            {
                if (bool.TryParse(_isActive, out bool parsed))
                {
                    return parsed;
                }

                return false;
            }
        }

        [JsonIgnore]
        public bool Mandatory
        {
            get
            {
                if (bool.TryParse(_isMandatory, out bool parsed))
                {
                    return parsed;
                }

                return false;
            }
        }

        [JsonIgnore]
        public QuestionTypeEnum Kind
        {
            get
            {
                QuestionTypeEnum kind = default(QuestionTypeEnum);

                switch (QuestionType)
                {
                    case "Rating":
                        kind = QuestionTypeEnum.Rating;
                        break;
                    case "Multiline Comment":
                    default:
                        kind = QuestionTypeEnum.MultilineComment;
                        break;
                }
                return kind;
            }
        }
    }
}