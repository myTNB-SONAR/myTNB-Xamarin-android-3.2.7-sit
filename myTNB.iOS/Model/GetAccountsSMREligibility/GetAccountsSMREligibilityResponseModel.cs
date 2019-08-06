using System.Collections.Generic;

namespace myTNB.Model
{
    public class AccountsSMREligibilityResponseModel
    {
        public AccountsSMREligibilityDataModel d { set; get; }
    }

    public class AccountsSMREligibilityDataModel : BaseModelV2
    {
        public List<AccountsSMREligibilityModel> data { set; get; }
    }

    public class AccountsSMREligibilityModel
    {
        public string ContractAccount { set; get; }
        public string SMREligibility { set; get; }
        public bool IsEligible
        {
            get
            {
                return !string.IsNullOrEmpty(SMREligibility) && SMREligibility.ToUpper() == "TRUE";
            }
        }
    }
}