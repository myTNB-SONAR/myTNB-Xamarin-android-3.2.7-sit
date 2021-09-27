using System.Collections.Generic;
using myTNB_Android.Src.SitecoreCMS.Model;
using Newtonsoft.Json;

namespace myTNB.SitecoreCMS.Model
{
    public class BillDetailsTooltipTimeStampResponseModel
    {
        public string Status { set; get; }
        public List<BillsTooltipTimeStamp> Data { set; get; }
    }

    public class BillDetailsTooltipResponseModel
    {
        public List<BillsTooltipModelEntity> Data { set; get; }
    }

    public class BillsTooltipModelEntity
    {
        [JsonProperty("Title")]
        public string Title { set; get; }

        [JsonProperty("Description")]
        public string Description { set; get; }

        [JsonProperty("Image")]
        public string Image { set; get; }

        [JsonProperty("ID")]
        public string ID { set; get; }
    }

    public class BillsTooltipModel
    {
        public string Title { set; get; }
        public List<string> Description { set; get; }
        public string Image { set; get; }
        public string ID { set; get; }
        [JsonIgnore]
        public bool IsSitecoreData { set; get; }
    }

    public class BillsTooltipTimeStamp
    {
        public string Timestamp { set; get; }
        public string ID { set; get; }
    }

    public enum BillsTooltipVersionEnum
    {
        V1,
        V2,
        None
    }
}
