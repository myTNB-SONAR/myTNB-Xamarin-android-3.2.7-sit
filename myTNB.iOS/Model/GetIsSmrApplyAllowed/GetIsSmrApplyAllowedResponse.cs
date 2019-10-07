using System;
using System.Collections.Generic;

namespace myTNB.Model
{
    public class GetIsSmrApplyAllowedResponseModel
    {
        public GetIsSmrApplyAllowedDataModel d { set; get; }
    }

    public class GetIsSmrApplyAllowedDataModel : BaseModelV2
    {
        public List<IsSSMRAllowedItemModel> data { set; get; }
    }

    public class IsSSMRAllowedItemModel
    {
        public string ContractAccount { set; get; } = string.Empty;
        public bool AllowApply { set; get; }
    }
}