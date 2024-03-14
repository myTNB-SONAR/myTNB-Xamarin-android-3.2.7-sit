using System;
using System.Collections.Generic;

namespace myTNB.Android.Src.MyTNBService.Response
{
    public class CANumberData
    {
        //[JsonProperty("overvoltageClaimEnabled")]
        public bool OvervoltageClaimEnabled { get; set; }

        public bool IsOvisUnderMaintenance { get; set; }
        //[JsonProperty("overvoltageClaimSupported")]
        public Dictionary<string, string> OvervoltageClaimSupported { get; set; }
    }
    public class TriggerOVISServicesResponseModel
    {
        public CANumberData d { get; set; }
    }
}
