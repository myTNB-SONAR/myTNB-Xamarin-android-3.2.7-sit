using Newtonsoft.Json;

namespace myTNB.Mobile
{
    public class BaseResponse<T> : BaseStatus
    {
        [JsonProperty("content")]
        public T Content { get; set; }
    }

    public class BaseStatus
    {
        [JsonProperty("statusDetail")]
        public StatusDetail StatusDetail { set; get; }
    }
}