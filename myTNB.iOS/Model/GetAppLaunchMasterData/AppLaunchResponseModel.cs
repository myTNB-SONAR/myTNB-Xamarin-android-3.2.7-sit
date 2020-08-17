using myTNB.Model;

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
        public bool IsRewardsDisabled { set; get; }
        public bool IsFeedbackUpdateDetailDisabled { set; get; }
}
}