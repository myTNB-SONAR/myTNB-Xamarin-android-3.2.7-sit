using Newtonsoft.Json;

namespace myTNB.Mobile
{
    public class BaseResponse<T>
    {
        [JsonProperty("content")]
        public T Content { get; set; }

        [JsonProperty("statusDetail")]
        public StatusDetail StatusDetail { set; get; }
    }
}