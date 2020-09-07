using System.Collections.Generic;
using Newtonsoft.Json;

namespace myTNB.Mobile
{
    public class GetSavedProjectStatusRequest: BaseRequest
    {
        [JsonProperty("pageNumber")]
        public int PageNumber { set; get; }
        [JsonProperty("accountNumbers")]
        public List<string> AccountNumbers { set; get; }
    }
}
