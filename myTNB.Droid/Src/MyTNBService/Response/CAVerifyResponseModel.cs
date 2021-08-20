using System;
using System.Collections.Generic;

namespace myTNB_Android.Src.MyTNBService.Response
{
    public class CANumberData
    {
        //[JsonProperty("overvoltageClaimEnabled")]
        public bool OvervoltageClaimEnabled { get; set; }

        //[JsonProperty("overvoltageClaimSupported")]
        public Dictionary<string, string> OvervoltageClaimSupported { get; set; }
    }

    public class CAVerifyResponseModel
    {
        public CANumberData d { get; set; }
        //public bool OvervoltageClaimEnabled { get; set; }
    }
}
