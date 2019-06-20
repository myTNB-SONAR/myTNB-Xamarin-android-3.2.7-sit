using myTNB_Android.Src.Base.Models;
using Newtonsoft.Json;

namespace myTNB_Android.Src.AppLaunch.Models
{
    public class MasterDataResponse : BaseResponse<MasterData>
    {
        [JsonProperty("d")]
        public MasterDataObj Data { get; set; }

        public class MasterDataObj
        {
            [JsonProperty("__type")]
            public string Type { get; set; }

            [JsonProperty("status")]
            public string Status { get; set; }

            [JsonProperty("isError")]
            public bool IsError { get; set; }

            [JsonProperty("message")]
            public string Message { get; set; }

            [JsonProperty("data")]
            public MasterData MasterData { get; set; }
        }
    }
}