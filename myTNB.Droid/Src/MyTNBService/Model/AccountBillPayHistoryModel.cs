using System;
using System.Collections.Generic;

namespace myTNB.Android.Src.MyTNBService.Model
{
    public class AccountBillPayHistoryModel
    {
		public string MonthYear { get; set; }
		public List<BillingHistoryData> BillingHistoryDataList { get; set; }

		public class BillingHistoryData
		{
			public string HistoryType { get; set; }
			public string DateAndHistoryType { get; set; }
			public string Amount { get; set; }
			public string DetailedInfoNumber { get; set; }
			public string PaidVia { get; set; }
			public string HistoryTypeText { get; set; }
            public bool IsPaymentPending { get; set; }
        }
	}
}
