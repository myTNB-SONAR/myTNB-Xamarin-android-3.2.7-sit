using System.Collections.Generic;

namespace myTNB.Model
{
    public class PaymentRecordModel : CustomerAccountRecordModel
    {
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