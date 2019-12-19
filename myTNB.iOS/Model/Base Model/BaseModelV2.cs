using Newtonsoft.Json;

namespace myTNB.Model
{
    public struct StatusCodes
    {
        public const string Success = "7200";
        public const string EmptyData = "7201";
        public const string PlannedMDMSDown = "8304";
        public const string UnplannedMDMSDown = "7204";
        public const string PlannedDowntime = "xxxx";
        public const string AppMaintenance = "yyyy";
    }

    public class BaseModelV2
    {
        public string ErrorCode { set; get; }
        public string ErrorMessage { set; get; }
        public string DisplayMessage { set; get; }
        public string DisplayType { set; get; }
        public string DisplayTitle { set; get; }

        //Refresh, Added as Virtual so that existing code will not be affected

        public virtual string RefreshTitle { set; get; }
        public virtual string RefreshMessage { set; get; }
        public virtual string RefreshBtnText { set; get; }

        public bool IsSuccess
        {
            get
            {
                return ErrorCode == StatusCodes.Success;
            }
        }
        public bool IsPlannedDownTime
        {
            get
            {
                return ErrorCode == StatusCodes.PlannedDowntime;
            }
        }
        public bool IsMaintenance
        {
            get
            {
                return ErrorCode == StatusCodes.AppMaintenance;
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