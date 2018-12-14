using System.Collections.Generic;

namespace myTNB.Model.GetMultiAccountDueAmount
{
    public class MultiAccountDueAmountModel : BaseModel
    {
        public List<MultiAccountDueAmountDataModel> data { set; get; }
    }
}