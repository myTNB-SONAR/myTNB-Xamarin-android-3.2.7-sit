using Newtonsoft.Json;
using Refit;
using System.Collections.Generic;

namespace myTNB.AndroidApp.Src.MultipleAccountPayment.Model
{
    public class MPGetRegisteredCardsResponse
    {

        [JsonProperty(PropertyName = "d")]
        [AliasAs("d")]
        public CreditCardData Data { get; set; }

        public class CreditCardData
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
            public List<MPCreditCard> creditCard { get; set; }
        }

    }
}
