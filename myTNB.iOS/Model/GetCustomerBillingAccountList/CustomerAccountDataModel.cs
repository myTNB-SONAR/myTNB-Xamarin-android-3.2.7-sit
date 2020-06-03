using System.Collections.Generic;

namespace myTNB.Model
{
    public class CustomerAccountDataModel : BaseModelV2
    {
        public List<CustomerAccountRecordModel> data { set; get; }
    }
}