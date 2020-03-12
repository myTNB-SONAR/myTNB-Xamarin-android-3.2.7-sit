﻿using System;
using Newtonsoft.Json;

namespace myTNB_Android.Src.MyTNBService.Response
{
    public class SubmitFeedbackResponse : BaseResponse<SubmitFeedbackResponse.ResponseData>
    {
        public ResponseData GetData()
        {
            return Response.Data;
        }

        public class ResponseData
        {
            [JsonProperty(PropertyName = "ServiceReqNo")]
            public string ServiceReqNo { get; set; }

            [JsonProperty(PropertyName = "DateCreated")]
            public string DateCreated { get; set; }
        }
    }
}
