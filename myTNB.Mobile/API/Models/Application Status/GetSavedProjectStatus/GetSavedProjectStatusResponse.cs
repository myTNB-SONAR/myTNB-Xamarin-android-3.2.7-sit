using System.Collections.Generic;
using Newtonsoft.Json;

namespace myTNB.Mobile
{
    public class GetSavedProjectStatusResponse : BaseResponse<GetSavedProjectStatusModel>
    {

    }

    public class GetSavedProjectStatusModel
    {
        [JsonProperty("pageSize")]
        public int PageSize { set; get; }
        [JsonProperty("pageNumber")]
        public int PageNumber { set; get; }
        [JsonProperty("savedProjects")]
        public List<Project> SavedProjects { set; get; }
    }

    public class Project : Details
    {
        [JsonProperty("isUpdated")]
        public bool IsUpdated { set; get; }
    }
}