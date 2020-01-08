using System.Collections.Generic;

namespace myTNB.Model
{
    public class AmountDueStatusResponseModel
    {
        public AmountDueStatusDataModel d { set; get; }
    }

    public class AmountDueStatusDataModel : BaseModelV2
    {
        public List<DueAmountDataModel> data { set; get; }
    }
}
