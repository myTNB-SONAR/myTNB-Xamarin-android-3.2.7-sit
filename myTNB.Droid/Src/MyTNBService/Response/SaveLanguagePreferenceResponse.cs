using System;
using Newtonsoft.Json;

namespace myTNB.Android.Src.MyTNBService.Response
{
    public class SaveLanguagePreferenceResponse : BaseResponse<SaveLanguagePreferenceResponse.ResponseData>
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
