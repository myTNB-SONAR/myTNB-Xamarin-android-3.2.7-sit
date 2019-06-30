using System.Collections.Generic;

namespace myTNB.Model
{
    public class MultiAccountDueAmountModel : BaseModel
    {
        public List<MultiAccountDueAmountDataModel> data { set; get; }
        public string MandatoryChargesTitle { set; get; }
        public string MandatoryChargesMessage { set; get; }
        public string MandatoryChargesPriButtonText { set; get; }
        public string MandatoryChargesSecButtonText { set; get; }
    }
}