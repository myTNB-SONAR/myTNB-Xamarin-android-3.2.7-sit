using Newtonsoft.Json;
using Refit;

namespace myTNB.AndroidApp.Src.myTNBMenu.Models
{
    public class PaymentHistoryV5
    {

        //[JsonProperty("__type")]
        //[AliasAs("__type")]
        //public string Type { get; set; }

        [JsonProperty("DtEvent")]
        [AliasAs("DtEvent")]
        public string DtEvent { get; set; }

        [JsonProperty("DtInput")]
        [AliasAs("DtInput")]
        public string DtInput { get; set; }

        [JsonProperty("AmPaid")]
        [AliasAs("AmPaid")]
        public double AmtPaid { get; set; }

        [JsonProperty("CdPBranch")]
        [AliasAs("CdPBranch")]
        public string CdPBranch { get; set; }

        [JsonProperty("NmPBranch")]
        [AliasAs("NmPBranch")]
        public string NmPBranch { get; set; }

        [JsonProperty("DocumentNumber")]
        [AliasAs("DocumentNumber")]
        public string DocumentNumber { get; set; }

        [JsonProperty("MechantTransId")]
        [AliasAs("MechantTransId")]
        public string MechantTransId { get; set; }

        //[JsonProperty("isError")]
        //[AliasAs("isError")]
        //public bool IsError { get; set; }

        //[JsonProperty("message")]
        //[AliasAs("message")]
        //public string Message { get; set; }
    }
}