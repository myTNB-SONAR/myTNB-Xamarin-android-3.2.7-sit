using System.Collections.Generic;
using Newtonsoft.Json;

namespace myTNB.Mobile
{
    public class GetSavedApplicationStatusResponse : BaseResponse<GetSavedApplicationStatusModel>
    {

    }

    public class GetSavedApplicationStatusModel
    {
        [JsonProperty("pageSize")]
        public int PageSize { set; get; }
        [JsonProperty("pageNumber")]
        public int PageNumber { set; get; }
        [JsonProperty("savedApplications")]
        public List<Application> SavedApplications { set; get; }
    }

    public class Application : Details
    {
        [JsonProperty("isUpdated")]
        public bool IsUpdated { set; get; }
    }
}