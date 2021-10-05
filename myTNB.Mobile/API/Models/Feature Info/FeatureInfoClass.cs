using System;
using System.Collections.Generic;

namespace myTNB.Mobile
{
    public class FeatureInfoClass
    {
        public class FeaturesContractAccount
        {
            public string contractAccount { get; set; }
            public bool acted { get; set; }
            public string modifiedDate { get; set; }
        }

        public class FeatureInfo
        {
            public string FeatureName { get; set; }
            public List<FeaturesContractAccount> ContractAccount { get; set; }
        }
    }
}
