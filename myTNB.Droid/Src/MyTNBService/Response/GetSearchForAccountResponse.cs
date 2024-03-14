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

namespace myTNB.Android.Src.MyTNBService.Response
{
    public class GetSearchForAccountResponse 
    {


        [JsonProperty(PropertyName = "d")]
        [AliasAs("d")]
        public List<GetSearchForAccountModel> GetSearchForAccount { set; get; }


        public class GetSearchForAccountModel
        {
            [JsonProperty(PropertyName = "ContractAccount")]
            public string ContractAccount { get; set; }

            [JsonProperty(PropertyName = "IC")]
            public string IC { get; set; }

            [JsonProperty(PropertyName = "FullName")]
            public string FullName { get; set; }

            [JsonProperty(PropertyName = "__type")]
            public string Type { get; set; }

        }

       
    



    }
}