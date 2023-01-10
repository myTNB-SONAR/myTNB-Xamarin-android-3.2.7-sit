using Newtonsoft.Json;

namespace myTNB.Mobile.API.Base
{
    public class BaseResponseV2<T>
    {
        [JsonProperty("d")]
        public BaseResponse<T> Data { set; get; }
    }
}