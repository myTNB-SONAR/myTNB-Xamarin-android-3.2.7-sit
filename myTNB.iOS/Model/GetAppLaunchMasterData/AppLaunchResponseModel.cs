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
        public MasterDataModel data { get; set; }
        public bool IsSMRApplyDisabled { get; set; }
        public bool IsEnergyTipsDisabled { get; set; }
        public bool IsOCRDown { get; set; }
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
