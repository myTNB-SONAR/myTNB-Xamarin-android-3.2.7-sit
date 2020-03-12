using System;
using Newtonsoft.Json;

namespace myTNB_Android.Src.MyTNBService.Response
{
    public class GetLanguagePreferenceResponse : BaseResponse<GetLanguagePreferenceResponse.ResponseData>
    {
        public ResponseData GetData()
        {
            return Response.Data;
        }

        public class ResponseData
        {
            [JsonProperty(PropertyName = "lang")]
            public string lang { get; set; }
        }
    }
}
