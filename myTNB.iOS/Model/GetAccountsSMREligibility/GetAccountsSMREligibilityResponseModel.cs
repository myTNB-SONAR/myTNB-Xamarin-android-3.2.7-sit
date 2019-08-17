using System.Collections.Generic;

namespace myTNB.Model
{
    public class AccountsSMREligibilityResponseModel
    {
        public AccountsSMREligibilityDataModel d { set; get; }
    }

    public class AccountsSMREligibilityDataModel : BaseModelV2
    {
        public AccountsSMREligibilitySerivceModel data { set; get; }
    }

    public class AccountsSMREligibilitySerivceModel
    {
        public List<AccountsSMREligibilityModel> accountEligibilities { set; get; }
    }

    public class AccountsSMREligibilityModel
    {
        public string ContractAccount { set; get; }
        public string SMREligibility { set; get; }
        public string IsSMRTagged { set; get; }
        public bool IsEligible
        {
            get
            {
                return !string.IsNullOrEmpty(SMREligibility) && SMREligibility.ToUpper() == "TRUE";
            }
        }
        public bool IsSSMR
        {
            get
            {
                return !string.IsNullOrEmpty(IsSMRTagged) && IsSMRTagged.ToUpper() == "TRUE";
            }
        }
    }
}