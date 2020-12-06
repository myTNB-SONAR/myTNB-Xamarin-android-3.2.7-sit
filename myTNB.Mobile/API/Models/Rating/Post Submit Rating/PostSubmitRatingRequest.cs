using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace myTNB.Mobile.API.Models.Rating.PostSubmitRating
{
    public class PostSubmitRatingRequest
    {
        [JsonProperty("submitRating")]
        public SubmitRating SubmitRating { set; get; }
    }

    public class SubmitRating
    {
        public string CustomerName { set; get; }
        public string PhoneNumber { set; get; }
        public string SrNo { set; get; }
        public string ApplicationId { set; get; }
        public string BackendAppId { set; get; }
        public string ModuleName { set; get; }
        public string QuestionCategoryValue { set; get; }
        public List<RatingAnswers> RatingResult { set; get; }
    }

    public class RatingAnswers
    {
        public int QuestionId { set; get; }
        public string QuestionDescription { set; get; }
        public int AnswerTypeId { set; get; }
        public string AnswerDescription { set; get; }
        public string AnswerValue { set; get; }
    }
}