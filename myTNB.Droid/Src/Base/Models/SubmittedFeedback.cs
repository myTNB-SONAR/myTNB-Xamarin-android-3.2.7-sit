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
using Newtonsoft.Json;
using myTNB_Android.Src.Database.Model;

namespace myTNB_Android.Src.Base.Models
{
    public class SubmittedFeedback
    {
        [JsonProperty("ServiceReqNo")]
        public string FeedbackId { get; set; }

        [JsonProperty("DateCreated")]
        public string DateCreated { get; set; }

        [JsonProperty("FeedbackMessage")]
        public string FeedbackMessage { get; set; }

        [JsonProperty("FeedbackCategoryName")]
        public string FeedbackCategoryName { get; set; }

        [JsonProperty("FeedbackCategoryId")]
        public string FeedbackCategoryId { get; set; }

        public static SubmittedFeedback Copy(SubmittedFeedbackEntity entity)
        {
            return new SubmittedFeedback()
            {
                FeedbackId = entity.Id,
                DateCreated = entity.DateCreated,
                FeedbackMessage = entity.FeedbackMessage,
                FeedbackCategoryId = entity.FeedbackCategoryId,
                FeedbackCategoryName = entity.FeedbackCategoryName
            };
        }
    }
}