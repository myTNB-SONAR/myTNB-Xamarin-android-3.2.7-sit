using System;
using System.Collections.Generic;
using myTNB.Mobile.API.Base;
using myTNB.SitecoreCMS.Model;
using myTNB_Android.Src.AppLaunch.Models;
using myTNB_Android.Src.FindUs.Response;
using myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.MVP;
using Newtonsoft.Json;
using Refit;

namespace myTNB_Android.Src.MyTNBService.Response
{
    public class DSSTableResponse
    {

        [JsonProperty(PropertyName = "d")]
        public DSSTableModel Response { get; set; }

        

        public class DSSTableModel : BaseResponseV2<DSSTableResponse.DSSTableModel>
        {
            public List<DownTime> data { set; get; }

            [JsonProperty(PropertyName = "status")]
            public string Status { get; set; }

            [JsonProperty(PropertyName = "message")]
            public string Message { get; set; }

            [JsonProperty(PropertyName = "ErrorCode")]
            public string ErrorCode { get; set; }

            [JsonProperty(PropertyName = "ErrorMessage")]
            public string ErrorMessage { get; set; }

            [JsonProperty(PropertyName = "DisplayMessage")]
            public string DisplayMessage { get; set; }

            [JsonProperty(PropertyName = "DisplayType")]
            public string DisplayType { get; set; }

            [JsonProperty(PropertyName = "DisplayTitle")]
            public string DisplayTitle { get; set; }

            [JsonProperty(PropertyName = "RefreshTitle")]
            public string RefreshTitle { get; set; }

            [JsonProperty(PropertyName = "RefreshMessage")]
            public string RefreshMessage { get; set; }

            [JsonProperty(PropertyName = "RefreshBtnText")]
            public string RefreshBtnText { get; set; }

            [JsonProperty(PropertyName = "IsPayEnabled")]
            public bool IsPayEnabled { get; set; }
        }

        //[JsonProperty(PropertyName = "d")]
        //public DSSTableModel Response { get; set; }

        //[JsonProperty(PropertyName = "data")]
        //[AliasAs("data")]
        //public DSSTableModel Data { get; set; }

        //[JsonProperty(PropertyName = "status")]
        //public string Status { get; set; }

        //[JsonProperty(PropertyName = "message")]
        //public string Message { get; set; }

        //[JsonProperty(PropertyName = "ErrorCode")]
        //public string ErrorCode { get; set; }

        //[JsonProperty(PropertyName = "ErrorMessage")]
        //public string ErrorMessage { get; set; }

        //[JsonProperty(PropertyName = "DisplayMessage")]
        //public string DisplayMessage { get; set; }

        //[JsonProperty(PropertyName = "DisplayType")]
        //public string DisplayType { get; set; }

        //[JsonProperty(PropertyName = "DisplayTitle")]
        //public string DisplayTitle { get; set; }

        //[JsonProperty(PropertyName = "RefreshTitle")]
        //public string RefreshTitle { get; set; }

        //[JsonProperty(PropertyName = "RefreshMessage")]
        //public string RefreshMessage { get; set; }

        //[JsonProperty(PropertyName = "RefreshBtnText")]
        //public string RefreshBtnText { get; set; }

        //[JsonProperty(PropertyName = "IsPayEnabled")]
        //public bool IsPayEnabled { get; set; }


        //public class DSSTableModel : BaseResponseV2<DSSTableResponse.DSSTableModel>
        //{
        //    [JsonProperty(PropertyName = "Downtime")]
        //    [AliasAs("Downtime")]
        //    public List<DownTime> Downtimes { get; set; }

        //}
    }
}

