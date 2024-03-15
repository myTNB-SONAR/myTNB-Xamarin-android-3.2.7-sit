using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace myTNB.AndroidApp.Src.MyTNBService.Response
{
    public class AppNotificationChannelsResponse : BaseResponse<List<AppNotificationChannelsResponse.ResponseData>>
    {
        public List<ResponseData> GetData()
        {
            return Response.Data;
        }
        public class ResponseData
		{
            [JsonProperty(PropertyName = "Id")]
            public string Id { get; set; }

            [JsonProperty(PropertyName = "Title")]
            public string Title { get; set; }

            [JsonProperty(PropertyName = "Code")]
            public string Code { get; set; }

            [JsonProperty(PropertyName = "PreferenceMode")]
            public string PreferenceMode { get; set; }

            [JsonProperty(PropertyName = "Type")]
            public string Type { get; set; }

            [JsonProperty(PropertyName = "ShowInPreference")]
            public string ShowInPreference { get; set; }

            [JsonProperty(PropertyName = "ShowInFilterList")]
            public string ShowInFilterList { get; set; }

            [JsonProperty(PropertyName = "IsOpted")]
            public string IsOpted { get; set; }

            [JsonProperty(PropertyName = "MasterId")]
            public string MasterId { get; set; }

            [JsonProperty(PropertyName = "CreatedDate")]
            public string CreatedDate { get; set; }
        }
    }
}
