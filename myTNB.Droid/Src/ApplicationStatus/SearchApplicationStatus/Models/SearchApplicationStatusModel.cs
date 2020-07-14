using System.Collections.Generic;
using Newtonsoft.Json;

namespace myTNB_Android.Src.ApplicationStatus.SearchApplicationStatus.Models
{
    public class SearchByModel
    {
        [JsonProperty("Title")]
        public string Title { get; set; }

        [JsonProperty("Code")]
        public string Code { get; set; }

        public bool isChecked { get; set; }
    }

    public class TypeModel
    {
        [JsonProperty("Title")]
        public string Title { get; set; }

        [JsonProperty("Code")]
        public string Code { get; set; }

        [JsonProperty("SearchBy")]
        public List<string> SearchByList { get; set; }

        public bool isChecked { get; set; }
    }
}