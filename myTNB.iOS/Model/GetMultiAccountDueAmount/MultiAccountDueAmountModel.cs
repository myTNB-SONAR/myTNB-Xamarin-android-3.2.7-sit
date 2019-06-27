using System.Collections.Generic;

namespace myTNB.Model.GetMultiAccountDueAmount
{
    public class MultiAccountDueAmountModel : BaseModel
    {
        public List<MultiAccountDueAmountDataModel> data { set; get; }
        public string MandatoryChargesLink { set; get; }
        public string MandatoryChargesTitle { set; get; }
        public string MandatoryChargesMessage { set; get; }
        public string MandatoryChargesPrimaryButtonText { set; get; }
        public string MandatoryChargesSecondaryButtonText { set; get; }
    }
}