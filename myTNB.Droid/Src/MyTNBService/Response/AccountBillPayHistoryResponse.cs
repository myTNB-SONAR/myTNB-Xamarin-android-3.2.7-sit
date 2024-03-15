using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace myTNB.AndroidApp.Src.MyTNBService.Response
{
    public class AccountBillPayHistoryResponse : BaseResponse<AccountBillPayHistoryResponse.AccountBillPayHistoryData>
    {
		public AccountBillPayHistoryData GetData()
        {
            return Response.Data;
        }

        public class AccountBillPayHistoryData
		{
			[JsonProperty(PropertyName = "BillPayHistories")]
			public List<BillPayHistory> BillPayHistories { get; set; }

            [JsonProperty(PropertyName = "BillPayFilterData")]
            public List<BillPayFilterData> BillPayFilterData { get; set; }
        }

		public class BillPayHistory
		{
			[JsonProperty(PropertyName = "MonthYear")]
			public string MonthYear { get; set; }

			[JsonProperty(PropertyName = "BillPayHistoryData")]
			public List<BillPayHistoryData> BillPayHistoryData { get; set; }
		}

        public class BillPayFilterData
        {
            [JsonProperty(PropertyName = "Text")]
            public string Text { get; set; }

            [JsonProperty(PropertyName = "Type")]
            public string Type { get; set; }
        }

        public class BillPayHistoryData
		{
			[JsonProperty(PropertyName = "BillOrPaymentDate")]
			public string BillOrPaymentDate { get; set; }

			[JsonProperty(PropertyName = "HistoryType")]
			public string HistoryType { get; set; }

			[JsonProperty(PropertyName = "DateAndHistoryType")]
			public string DateAndHistoryType { get; set; }

			[JsonProperty(PropertyName = "Amount")]
			public string Amount { get; set; }

			[JsonProperty(PropertyName = "DetailedInfoNumber")]
			public string DetailedInfoNumber { get; set; }

			[JsonProperty(PropertyName = "PaidVia")]
			public string PaidVia { get; set; }

			[JsonProperty(PropertyName = "HistoryTypeText")]
			public string HistoryTypeText { get; set; }

            [JsonProperty(PropertyName = "IsPaymentPending")]
            public bool IsPaymentPending { get; set; }
        }
	}
}
