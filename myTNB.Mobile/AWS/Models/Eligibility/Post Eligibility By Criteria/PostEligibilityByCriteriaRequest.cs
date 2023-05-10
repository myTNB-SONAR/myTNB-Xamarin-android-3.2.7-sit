using System.Collections.Generic;

namespace myTNB.Mobile.AWS
{
    public class PostEligibilityByCriteriaRequest
    {
        public string UserID { set; get; }
        public List<PremiseCriteriaModel> PremiseCriteria { set; get; }
        public List<ContractAccountModel> ContractAccounts { set; get; }
    }

    public class PremiseCriteriaModel
    {
        public bool IsOwner { set; get; }
        public string RateCategory { set; get; }
        public string SmartMeterCode { set; get; }
        public string BusinessArea { set; get; }
    }
}