using System;
using Newtonsoft.Json;

namespace myTNB_Android.Src.AppLaunch.Models
{
    public class UpdateAppUserDeviceResponse
    {
        [JsonProperty("d")]
        public bool d { get; set; }
    }
}
