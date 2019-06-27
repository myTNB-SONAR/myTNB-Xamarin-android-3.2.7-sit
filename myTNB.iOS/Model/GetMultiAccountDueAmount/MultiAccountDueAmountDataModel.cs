using System.Collections.Generic;

namespace myTNB.Model.GetMultiAccountDueAmount
{
    public class MultiAccountDueAmountDataModel
    {
        public double amountDue { set; get; }
        public string billDueDate { set; get; }
        public string accNum { set; get; }
        public string IncrementREDueDateByDays { set; get; }
        public List<ItemisedBilling> ItemizedBillings { set; get; }
        public double OpenChargesTotal { set; get; }
        public bool IsItemisedBilling
        {
            get
            {
                return OpenChargesTotal > 0;
            }
        }
    }
}