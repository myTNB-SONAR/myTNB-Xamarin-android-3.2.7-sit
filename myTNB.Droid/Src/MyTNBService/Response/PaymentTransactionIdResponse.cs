using System;
using Newtonsoft.Json;
using Refit;

namespace myTNB_Android.Src.MyTNBService.Response
{
    public class PaymentTransactionIdResponse
    {
		[JsonProperty(PropertyName = "d")]
		[AliasAs("d")]
		public RequestPayBill requestPayBill { get; set; }

		public class RequestPayBill
		{
			[JsonProperty(PropertyName = "__type")]
			public string Type { get; set; }

			[JsonProperty(PropertyName = "status")]
			[AliasAs("status")]
			public string Status { get; set; }

			[JsonProperty(PropertyName = "isError")]
			[AliasAs("isError")]
			public bool IsError { get; set; }

			[JsonProperty(PropertyName = "message")]
			[AliasAs("message")]
			public string Message { get; set; }

			[JsonProperty(PropertyName = "data")]
			[AliasAs("data")]
			public InitiatePaymentResult initiatePaymentResult { get; set; }
		}

        public class InitiatePaymentResult
        {
            [JsonProperty(PropertyName = "action")]
            [AliasAs("action")]
            public string action { get; set; }

            [JsonProperty(PropertyName = "payMerchantID")]
            [AliasAs("payMerchantID")]
            public string payMerchantID { get; set; }

            [JsonProperty(PropertyName = "payMerchant_transID")]
            [AliasAs("payMerchant_transID")]
            public string payMerchant_transID { get; set; }

            [JsonProperty(PropertyName = "payCurrencyCode")]
            [AliasAs("payCurrencyCode")]
            public string payCurrencyCode { get; set; }

            [JsonProperty(PropertyName = "payAmount")]
            [AliasAs("payAmount")]
            public string payAmount { get; set; }

            [JsonProperty(PropertyName = "payCustName")]
            [AliasAs("payCustName")]
            public string payCustName { get; set; }

            [JsonProperty(PropertyName = "payCustEmail")]
            [AliasAs("payCustEmail")]
            public string payCustEmail { get; set; }

            [JsonProperty(PropertyName = "payCustPhoneNum")]
            [AliasAs("payCustPhoneNum")]
            public string payCustPhoneNum { get; set; }

            [JsonProperty(PropertyName = "payProdDesc")]
            [AliasAs("payProdDesc")]
            public string payProdDesc { get; set; }

            [JsonProperty(PropertyName = "payReturnUrl")]
            [AliasAs("payReturnUrl")]
            public string payReturnUrl { get; set; }

            [JsonProperty(PropertyName = "paySign")]
            [AliasAs("paySign")]
            public string paySign { get; set; }

            [JsonProperty(PropertyName = "payMParam")]
            [AliasAs("payMParam")]
            public string payMParam { get; set; }

            [JsonProperty(PropertyName = "payMethod")]
            [AliasAs("payMethod")]
            public string payMethod { get; set; }

            [JsonProperty(PropertyName = "platform")]
            [AliasAs("platform")]
            public string platform { get; set; }

            [JsonProperty(PropertyName = "transactionType")]
            [AliasAs("transactionType")]
            public string transactionType { get; set; }

            [JsonProperty(PropertyName = "tokenizedHashCodeCC")]
            [AliasAs("tokenizedHashCodeCC")]
            public string tokenizedHashCodeCC { get; set; }

            [JsonProperty(PropertyName = "isError")]
            [AliasAs("isError")]
            public string isError { get; set; }

            [JsonProperty(PropertyName = "message")]
            [AliasAs("message")]
            public string message { get; set; }
        }
    }
}
