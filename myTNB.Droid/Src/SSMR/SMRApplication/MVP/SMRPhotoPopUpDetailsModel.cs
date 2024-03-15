using System;
using Newtonsoft.Json;
using Refit;

namespace myTNB.AndroidApp.Src.SSMR.SMRApplication.MVP
{
    public class SMRPhotoPopUpDetailsModel
    {
        [JsonProperty(PropertyName = "Title")]
        [AliasAs("Title")]
        public string Title { get; set; }

        [JsonProperty(PropertyName = "Description")]
        [AliasAs("Description")]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "CTA")]
        [AliasAs("CTA")]
        public string CTA { get; set; }

        [JsonProperty(PropertyName = "Type")]
        [AliasAs("Type")]
        public string Type { get; set; }
    }
}
