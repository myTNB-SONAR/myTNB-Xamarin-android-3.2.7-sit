using System.Collections.Generic;
using myTNB.Model;

namespace myTNB
{
    public class GetAccountBillPayHistoryResponseModel
    {
        public GetAccountBillPayHistoryModel d { set; get; }
    }

    public class GetAccountBillPayHistoryModel : BaseModelV2
    {
        public BillPayHistoriesDataModel data { set; get; }
        public bool IsPayEnabled { set; get; }
    }

    public class BillPayHistoriesDataModel
    {
        public List<BillPayHistoryModel> BillPayHistories { set; get; } = new List<BillPayHistoryModel>();
        public List<FilterModel> BillPayFilterData { set; get; } = new List<FilterModel>();
    }

    public class BillPayHistoryModel
    {
        public string MonthYear { set; get; } = string.Empty;
        public List<BillPayHistoryDataModel> BillPayHistoryData = new List<BillPayHistoryDataModel>();
    }

    public class FilterModel
    {
        public string Text { set; get; } = string.Empty;
        public string Type { set; get; } = string.Empty;
    }

    public class BillPayHistoryDataModel
    {
        public string BillOrPaymentDate { set; get; } = string.Empty;
        public string HistoryType { set; get; } = string.Empty;
        public string DateAndHistoryType { set; get; } = string.Empty;
        public string Amount { set; get; } = string.Empty;
        public string DetailedInfoNumber { set; get; } = string.Empty;
        public string PaidVia { set; get; } = string.Empty;
        public bool IsPaymentPending { set; get; }
        public bool IsPayment
        {
            get
            {
                const string Payment = "PAYMENT";
                if (!string.IsNullOrEmpty(HistoryType) && !string.IsNullOrWhiteSpace(HistoryType))
                {
                    return HistoryType.ToUpper() == Payment;
                }
                return false;
            }
        }
        public bool IsDocumentAvailable
        {
            get
            {
                return !string.IsNullOrEmpty(DetailedInfoNumber) && !string.IsNullOrWhiteSpace(DetailedInfoNumber);
            }
        }
        public string HistoryTypeText { set; get; } = string.Empty;
    }
}