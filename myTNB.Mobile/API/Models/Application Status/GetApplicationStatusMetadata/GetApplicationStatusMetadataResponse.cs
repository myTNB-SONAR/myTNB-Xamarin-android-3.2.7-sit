using System.Collections.Generic;
using Newtonsoft.Json;

namespace myTNB.Mobile
{
    public class GetApplicationStatusMetadataResponse : BaseResponse<GetApplicationStatusMetadataModel>
    {

    }

    public class GetApplicationStatusMetadataModel
    {
        [JsonProperty("statusColourLegend")]
        public List<ColorLegend> StatusColourLegend { set; get; }
        [JsonProperty("searchBy")]
        public List<TitleCodePair> SearchBy { set; get; }
        [JsonProperty("type")]
        public List<Search> Type { set; get; }
        [JsonProperty("appStatus")]
        public List<TitleCodePair> ApplicationStatus { set; get; }
        [JsonProperty("prjStatus")]
        public List<TitleCodePair> ProjectStatus { set; get; }
        [JsonProperty("MinFilterYear")]
        public string MinFilterYear { set; get; }
    }

    public class ColorLegend
    {
        public string Code { set; get; }
        public List<int> RGB { set; get; }
    }

    public class TitleCodePair
    {
        public string Title { set; get; }
        public string Code { set; get; }
    }

    public class Search : TitleCodePair
    {
        public List<string> SearchBy { set; get; }
    }
}