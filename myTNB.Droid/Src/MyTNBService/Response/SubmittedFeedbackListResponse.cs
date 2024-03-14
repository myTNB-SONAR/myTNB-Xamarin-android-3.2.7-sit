using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace myTNB.Android.Src.MyTNBService.Response
{
    public class SubmittedFeedbackListResponse : BaseResponse<List<SubmittedFeedbackListResponse.ResponseData>>
    {
        public List<ResponseData> GetData()
        {
            return Response.Data;
        }

        public class ResponseData
        {
            [JsonProperty(PropertyName = "ServiceReqNo")]
            public string ServiceReqNo { get; set; }

            [JsonProperty(PropertyName = "DateCreated")]
            public string DateCreated { get; set; }

            [JsonProperty(PropertyName = "FeedbackMessage")]
            public string FeedbackMessage { get; set; }

            [JsonProperty(PropertyName = "FeedbackCategoryName")]
            public string FeedbackCategoryName { get; set; }

            [JsonProperty(PropertyName = "FeedbackCategoryId")]
            public string FeedbackCategoryId { get; set; }

            [JsonProperty(PropertyName = "FeedbackNameInListView")]
            public string FeedbackNameInListView { get; set; }

            [JsonProperty(PropertyName = "StatusCode")]
            public string StatusCode { get; set; }

            [JsonProperty(PropertyName = "StatusDesc")]
            public string StatusDesc { get; set; }

            [JsonProperty(PropertyName = "IsRead")]
            public string IsRead { get; set; }
            
        }
    }
}
