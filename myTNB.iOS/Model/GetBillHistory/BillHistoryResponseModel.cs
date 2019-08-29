using System.Collections.Generic;

namespace myTNB.Model
{
    public class BillHistoryResponseModel
    {
        public BillHistoryModel d { set; get; }
    }
    public class BillHistoryModel : BaseModelV2
    {
        public List<BillHistoryDataModel> data { set; get; }
    }

    public class BillHistoryDataModel
    {
        public string BillingNo { set; get; } = string.Empty;
        public string NrBill { set; get; } = string.Empty;
        public string DtBill { set; get; } = string.Empty;
        public string AmPayable { set; get; } = string.Empty;
        public string QtUnits { set; get; } = string.Empty;
    }
}