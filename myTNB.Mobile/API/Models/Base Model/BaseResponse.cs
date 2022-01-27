using Newtonsoft.Json;

namespace myTNB.Mobile.API.Models.BaseModel
{
    public struct StatusCodes
    {
        public const string Success = "7200";
        public const string EmptyData = "7201";
        public const string PlannedMDMSDown = "8304";
        public const string UnplannedMDMSDown = "7204";
        public const string PlannedDowntime = "8400";
        public const string AppMaintenance = "7000";
        public const string UnplannedDowntime = "8100";
    }

    public class BaseASMXResponse
    {
        public BaseASMXModel d { set; get; }
    }

    public class BaseASMXModel
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

        public bool IsUnplannedDownTime
        {
            get
            {
                return ErrorCode == StatusCodes.UnplannedDowntime;
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