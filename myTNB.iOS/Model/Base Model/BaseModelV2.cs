using Newtonsoft.Json;

namespace myTNB.Model
{
    public struct StatusCodes
    {
        public const string Success = "7200";
        public const string EmptyData = "7201";
        public const string PlannedMDMSDown = "8304";
        public const string UnplannedMDMSDown = "7204";
    }

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
                return ErrorCode == StatusCodes.Success;
            }
        }
        [JsonIgnore]
        public bool didSucceed
        {
            get
            {
                return IsSuccess;
            }
        }
        public string isError { set; get; }
    }
}