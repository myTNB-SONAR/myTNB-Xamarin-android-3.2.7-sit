using myTNB.Model;
using Newtonsoft.Json;

namespace myTNB
{
    public class AppLaunchResponseModel
    {
        public AppLaunchMasterDataModel d { set; get; }
    }

    public class AppLaunchMasterDataModel : BaseModelV2
    {
        public MasterDataModel data { set; get; }
        public bool IsSMRApplyDisabled { set; get; }
        public bool IsEnergyTipsDisabled { set; get; }
        public bool IsSMRFeatureDisabled { set; get; }
        public bool IsOCRDown { set; get; }
        public bool IsPayEnabled { set; get; }
        [JsonIgnore]
        public bool IsMaintenance
        {
            get
            {
                return ErrorCode == "7000";
            }
        }
    }
}