using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace myTNB_Android.Src.MyTNBService.Response
{
    public class AccountBillPayHistoryResponse
    {
		[JsonProperty(PropertyName = "d")]
		public APIResponseResult Data { get; set; }

		public class APIResponseResult
		{
			[JsonProperty(PropertyName = "__type")]
			public string Type { get; set; }

			[JsonProperty(PropertyName = "data")]
			public AccountBillPayHistoryData ResponseData { get; set; }

			[JsonProperty(PropertyName = "status")]
			public string Status { get; set; }

			[JsonProperty(PropertyName = "message")]
			public string Message { get; set; }

			[JsonProperty(PropertyName = "ErrorCode")]
			public string ErrorCode { get; set; }

			[JsonProperty(PropertyName = "ErrorMessage")]
			public string ErrorMessage { get; set; }

			[JsonProperty(PropertyName = "DisplayMessage")]
			public string DisplayMessage { get; set; }

			[JsonProperty(PropertyName = "DisplayType")]
			public string DisplayType { get; set; }

			[JsonProperty(PropertyName = "DisplayTitle")]
			public string DisplayTitle { get; set; }

            [JsonProperty(PropertyName = "RefreshMessage")]
            public string RefreshMessage { get; set; }

            [JsonProperty(PropertyName = "RefreshBtnText")]
            public string RefreshBtnText { get; set; }
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
		}
	}
}
