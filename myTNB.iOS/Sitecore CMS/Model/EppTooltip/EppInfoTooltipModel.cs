using System.Collections.Generic;
using Foundation;
using Newtonsoft.Json;

//Created by Syahmi ICS 05052020

namespace myTNB.SitecoreCMS.Model
{
    public class EppInfoTooltipTimeStampResponseModel
    {
        public string Status { set; get; }
        public List<EppTooltipTimeStamp> Data { set; get; }
    }

    public class EppInfoTooltipResponseModel
    {
        public string Status { set; get; }
        public List<EppTooltipModelEntity> Data { set; get; }
    }

    public class EppTooltipModelEntity
    {
        public string Title { set; get; }
        public string PopUpTitle { set; get; }
        public string PopUpBody { set; get; }
        public string Image { set; get; }
        public string ID { set; get; }
        public byte[] ImageByteArray { set; get; }
    }

    public class EppTooltipModel
    {
        public string Title { set; get; }
        public string PopUpTitle { set; get; }
        public string PopUpBody { set; get; }
        public string Image { set; get; }
        public string ID { set; get; }
        public NSData NSDataImage { set; get; }

        [JsonIgnore]
        public bool IsSitecoreData { set; get; }
    }

    public class EppTooltipTimeStamp
    {
        public string Timestamp { set; get; }
        public string ID { set; get; }
    }
}
