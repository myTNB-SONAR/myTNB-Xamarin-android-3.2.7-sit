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
using myTNB_Android.Src.Base.Models;
using Refit;
using Newtonsoft.Json;

namespace myTNB_Android.Src.AppLaunch.Models
{
    public class MasterDataRequest
    {
        [JsonProperty("ApiKeyID")]
        public string ApiKeyID { get; set; }

        [JsonProperty("SSPUserId")]
        public string SSPUserId { get; set; }

        [JsonProperty("Email")]
        public string Email { get; set; }

        [JsonProperty("DeviceId")]
        public string DeviceId { get; set; }

        [JsonProperty("AppVersion")]
        public string AppVersion { get; set; }

        [JsonProperty("OsType")]
        public string OsType { get; set; }

        [JsonProperty("OsVersion")]
        public string OsVersion { get; set; }

        public MasterDataRequest()
        {

        }
    }
}