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

namespace myTNB_Android.Src.Base.Models
{
    public class SubmittedFeedbackDetails
    {
        [JsonProperty("ServiceReqNo")]
        public string ServiceReqNo { get; set; }

        [JsonProperty("AccountNum")]
        public string AccountNum { get; set; }

        [JsonProperty("StateId")]
        public string StateId { get; set; }

        [JsonProperty("StateName")]
        public string StateName { get; set; }

        [JsonProperty("Location")]
        public string Location { get; set; }

        [JsonProperty("PoleNum")]
        public string PoleNum { get; set; }

        [JsonProperty("FeedbackTypeId")]
        public string FeedbackTypeId { get; set; }

        [JsonProperty("FeedbackTypeName")]
        public string FeedbackTypeName { get; set; }

        [JsonProperty("DateCreated")]
        public string DateCreated { get; set; }

        [JsonProperty("FeedbackMessage")]
        public string FeedbackMessage { get; set; }

        [JsonProperty("FeedbackImage")]
        public List<ImageResponse> ImageList { get; set; }

        [JsonProperty("StatusCode")]
        public string StatusCode { get; set; }

        [JsonProperty("StatusDesc")]
        public string StatusDesc { get; set; }

        public class ImageResponse
        {
            [JsonProperty("imageHex")]
            public string ImageHex { get; set; }

            [JsonProperty("fileName")]
            public string FileName { get; set; }

            [JsonProperty("fileSize")]
            public int FileSize { get; set; }
        }
    }
}