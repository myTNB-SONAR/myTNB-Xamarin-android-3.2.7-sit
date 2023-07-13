using System.Collections.Generic;
using Newtonsoft.Json;

namespace myTNB.Mobile.AWS
{
    public class PostEligibilityRequest
    {
        [JsonProperty("userID")]
        public string UserID { set; get; }
        [JsonProperty("email")]
        public string Email { set; get; }
        [JsonProperty("contractAccounts")]
        public List<ContractAccountModel> ContractAccounts { set; get; }
        [JsonProperty("premiseCriteria")]
        public List<PremiseCriteriaModel> PremiseCriteria { set; get; }
    }

    public class ContractAccountModel
    {
        public string accNum { set; get; }
        public string userAccountID { set; get; }
        public string accDesc { set; get; }
        public string icNum { set; get; }
        public double amCurrentChg { set; get; }
        public string isRegistered { set; get; }
        public string isOwned { set; get; }
        public string isPaid { set; get; }
        public string isError { set; get; }
        public string message { set; get; }
        public string accountTypeId { set; get; }
        public string accountStAddress { set; get; }
        public string ownerName { set; get; }
        public string accountCategoryId { set; get; }
        public string SmartMeterCode { set; get; }
        public string isTaggedSMR { set; get; }
        public int BudgetAmount { set; get; }
        public string InstallationType { set; get; }
        public bool IsApplyEBilling { set; get; }
        public bool IsHaveAccess { set; get; }
        public string BusinessArea { set; get; }
        public string RateCategory { set; get; }
    }
}