using System.Collections.Generic;

namespace myTNB.Model
{
    public class BillHistoryModel : BaseModel
    {
        public List<BillHistoryDataModel> data { set; get; }
    }
}