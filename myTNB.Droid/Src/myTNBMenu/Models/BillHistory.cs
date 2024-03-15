using Newtonsoft.Json;
using Refit;

namespace myTNB.AndroidApp.Src.myTNBMenu.Models
{
    public class BillHistory
    {
        //"__type": "Billing.BillHistory",
        //  "NrBill": "001526208796",
        //  "DtBill": "04/08/2017",
        //  "AmPayable": "      746.12-",
        //  "QtUnits": null,
        //  "isError": "false",
        //  "message": "Successful"
        [JsonProperty("__type")]
        [AliasAs("__type")]
        public string Type { get; set; }

        [JsonProperty("NrBill")]
        [AliasAs("NrBill")]
        public string NrBill { get; set; }

        [JsonProperty("DtBill")]
        [AliasAs("DtBill")]
        public string DtBill { get; set; }

        [JsonProperty("AmPayable")]
        [AliasAs("AmPayable")]
        public double AmPayable { get; set; }

        [JsonProperty("QtUnits")]
        [AliasAs("QtUnits")]
        public string QtUnits { get; set; }

        [JsonProperty("isError")]
        [AliasAs("isError")]
        public bool IsError { get; set; }

        [JsonProperty("message")]
        [AliasAs("message")]
        public string Message { get; set; }
    }
}