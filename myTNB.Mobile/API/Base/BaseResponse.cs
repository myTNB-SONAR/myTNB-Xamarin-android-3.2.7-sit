using Newtonsoft.Json;

namespace myTNB.Mobile
{
    public class BaseResponse<T>
    {
        [JsonProperty("d")]
        public BaseAPIModel<T> D { get; set; }
    }

    public class BaseAPIModel<T>
    {
       /* [JsonProperty("data")]
        public T Data { set; get; }
        public bool IsSuccess { set; get; }
        public BaseContent Success { set; get; }
        public ErrorContent Error { set; get; }
       */
    }

    public class ErrorContent : BaseContent
    {
        public bool CanRefresh { set; get; }
        public RefreshContent Refresh { set; get; }
    }

    public class RefreshContent
    {
        public string Title { set; get; }
        public string Message { set; get; }
        public string CTATitle { set; get; }
    }

    public class BaseContent
    {
        public string Code { set; get; }
        public string Title { set; get; }
        public string Message { set; get; }
    }
}