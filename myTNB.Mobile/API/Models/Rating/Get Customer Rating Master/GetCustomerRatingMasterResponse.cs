using System.Collections.Generic;
using myTNB.Mobile.Extensions;
using Newtonsoft.Json;

namespace myTNB.Mobile.API.Models.Rating.GetCustomerRatingMaster
{
    public class GetCustomerRatingMasterResponse : BaseResponse<GetCustomerRatingMasterModel>
    {
    }

    public class GetCustomerRatingMasterModel
    {
        [JsonProperty("questionCategoryDescription")]
        public string QuestionCategoryDescription { set; get; }
        [JsonProperty("questionAnswerSets")]
        public List<QuestionAnswerSetsModel> QuestionAnswerSets { set; get; }
    }

    public class QuestionAnswerSetsModel
    {
        [JsonProperty("sequence")]
        public int Sequence { set; get; }
        [JsonProperty("questionDetail")]
        public QuestionDetailModel QuestionDetail { set; get; }
        [JsonProperty("answerDetail")]
        public AnswerDetailModel AnswerDetail { set; get; }

        [JsonIgnore]
        public RateType RateTpe
        {
            get
            {
                RateType rType = RateType.None;
                if (AnswerDetail != null && AnswerDetail.AnswerTypeValue.IsValid())
                {
                    switch (AnswerDetail.AnswerTypeValue)
                    {
                        case "Star":
                            {
                                return RateType.Star;
                            }
                        case "MultiSelect":
                            {
                                return RateType.MultiSelect;
                            }
                        case "FreeText":
                            {
                                return RateType.FreeText;
                            }
                    }
                }
                return rType;
            }
        }
    }

    public class QuestionDetailModel
    {
        [JsonProperty("questionId")]
        public int QuestionId { set; get; }
        [JsonProperty("previousQuestionId")]
        public int? PreviousQuestionId { set; get; }
        [JsonProperty("questionDescription")]
        public Dictionary<string, string> QuestionDescription { set; get; }
    }

    public class AnswerDetailModel
    {
        [JsonProperty("answerSetValue")]
        public Dictionary<string, string> AnswerSetValue { set; get; }
        [JsonProperty("answerTypeId")]
        public int AnswerTypeId { set; get; }
        [JsonProperty("answerTypeValue")]
        public string AnswerTypeValue { set; get; }
    }

    public enum RateType
    {
        None,
        Star,
        MultiSelect,
        FreeText
    }
}