using System.Collections.Generic;

namespace myTNB.Model
{
    public class PaymentHistoryModel : BaseModel
    {
        public List<PaymentHistoryDataModel> data { set; get; }
    }
}