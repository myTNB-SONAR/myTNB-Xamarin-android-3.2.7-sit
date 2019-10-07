using Newtonsoft.Json;

namespace myTNB.Model
{
    public class BaseModelV2
    {
        public string ErrorCode { set; get; }
        public string ErrorMessage { set; get; }
        public string DisplayMessage { set; get; }
        public string DisplayType { set; get; }
        public string DisplayTitle { set; get; }
        public bool IsSuccess
        {
            get
            {
                return ErrorCode == "7200";
            }
        }
        [JsonIgnore]
        public bool didSucceed
        {
            get
            {
                if (!string.IsNullOrEmpty(isError))
                {
                    return IsSuccess || !bool.Parse(isError);
                }
                return IsSuccess;
            }
        }
        public string isError { set; get; }
    }
}