using System.Collections.Generic;
using Foundation;
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
        public string Status { set; get; }
        public List<BillsTooltipModelEntity> Data { set; get; }
    }

    public class BillsTooltipModelEntity
    {
        public string Title { set; get; }
        public string Description { set; get; }
        public string Image { set; get; }
        public string ID { set; get; }
        public byte[] ImageByteArray { set; get; }
    }

    public class BillsTooltipModel
    {
        public string Title { set; get; }
        public List<string> Description { set; get; }
        public string Image { set; get; }
        public string ID { set; get; }
        public NSData NSDataImage { set; get; }

        [JsonIgnore]
        public bool IsSitecoreData { set; get; }
    }

    public class BillsTooltipTimeStamp
    {
        public string Timestamp { set; get; }
        public string ID { set; get; }
    }
}