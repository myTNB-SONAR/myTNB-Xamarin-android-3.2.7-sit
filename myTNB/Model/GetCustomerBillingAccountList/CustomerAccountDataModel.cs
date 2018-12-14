using System.Collections.Generic;

namespace myTNB.Model
{
    public class CustomerAccountDataModel : BaseModel
    {
        public List<CustomerAccountRecordModel> data { set; get; }
    }
}