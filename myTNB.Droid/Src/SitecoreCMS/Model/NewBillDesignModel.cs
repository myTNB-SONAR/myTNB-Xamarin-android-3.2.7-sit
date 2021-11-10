using System.Collections.Generic;
using Newtonsoft.Json;

namespace myTNB_Android.Src.SitecoreCMS.Model
{
    public class NewBillDesignTimeStampResponseModel
    {
        public string Status { set; get; }
        public List<NewBillDesignTimeStamp> Data { set; get; }
    }

    public class NewBillDesignTimeStamp
    {
        public string Timestamp { set; get; }
        public string ID { set; get; }
    }

    public class NewBillDesignResponseModel
    {
        public List<NewBillDesignModelEntity> Data { set; get; }
    }

    public class NewBillDesignModelEntity
    {
        [JsonProperty("Title")]
        public string Title { set; get; }

        [JsonProperty("Description")]
        public string Description { set; get; }

        [JsonProperty("Image1")]
        public string Image1 { set; get; }

        [JsonProperty("Image2")]
        public string Image2 { set; get; }

        [JsonProperty("IsZoomable")]
        public bool IsZoomable { set; get; }

        [JsonProperty("IsHeader")]
        public bool IsHeader { set; get; }

        [JsonProperty("IsFooter")]
        public bool IsFooter { set; get; }

        [JsonProperty("ID")]
        public string ID { set; get; }

        [JsonProperty("ShouldTrack")]
        public bool ShouldTrack { set; get; }

        [JsonProperty("DynatraceTag")]
        public string DynatraceTag { set; get; }
    }
}