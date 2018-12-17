using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using Refit;

namespace myTNB_Android.Src.myTNBMenu.Models
{
    public class BillHistoryV5
    {

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

        //[JsonProperty("isError")]
        //[AliasAs("isError")]
        //public bool IsError { get; set; }

        //[JsonProperty("message")]
        //[AliasAs("message")]
        //public string Message { get; set; }
    }
}