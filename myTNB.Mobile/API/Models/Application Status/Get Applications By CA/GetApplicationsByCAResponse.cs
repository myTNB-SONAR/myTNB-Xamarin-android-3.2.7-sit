using myTNB.Mobile.API.Base;
using myTNB.Mobile.Extensions;
using myTNB.Mobile.SessionCache;
using Newtonsoft.Json;

namespace myTNB.Mobile.API.Models.ApplicationStatus.GetApplicationsByCA
{
    public class GetApplicationsByCAResponse : BaseListResponse<GetApplicationsByCAModel>
    {

    }
    public class GetApplicationsByCAModel
    {
        [JsonProperty("applicationModuleId")]
        public string ApplicationModuleId { set; get; }
        /// <summary>
        /// Pass this as application type in GetApplicationStatus
        /// </summary>
        [JsonProperty("applicationType")]
        public string ApplicationType { set; get; }
        /// <summary>
        /// Pass this as search type in GetApplicationStatus
        /// </summary>
        [JsonProperty("searchType")]
        public string SearchType { set; get; }
        /// <summary>
        /// Pass this as search term in GetApplicationStatus
        /// </summary>
        [JsonProperty("backendReferenceNo")]
        public string BackendReferenceNo { set; get; }
        [JsonProperty("backendApplicationType")]
        public string BackendApplicationType { set; get; }
        [JsonProperty("backendModule")]
        public string BackendModule { set; get; }
        [JsonProperty("statusId")]
        public string StatusId { set; get; }
        [JsonProperty("statusCode")]
        public string StatusCode { set; get; }
        [JsonProperty("statusDescription")]
        public string StatusDescription { set; get; }
        [JsonProperty("createdDate")]
        public string CreatedDate { set; get; }
        [JsonProperty("statusDescriptionColor")]
        public string StatusDescriptionColor { set; get; }

        /// <summary>
        /// Pass this as application type title in GetApplicationStatus
        /// </summary>
        [JsonIgnore]
        public string ApplicationTypeDisplay
        {
            get
            {
                return SearchApplicationTypeCache.Instance.GetApplicationTypeDescription(ApplicationType);
            }
        }

        /// <summary>
        /// Pass this as search type title in GetApplicationStatus
        /// </summary>
        [JsonIgnore]
        public string SearchTypeDisplay
        {
            get
            {
                return SearchApplicationTypeCache.Instance.GetSearchTypeDescription(SearchType);
            }
        }

        /// <summary>
        /// Used for displaying Reference ID
        /// </summary>
        [JsonIgnore]
        public string ReferenceIDDisplay
        {
            get
            {
                return string.Format("{0}: {1}"
                    , BackendApplicationType ?? string.Empty
                    , BackendReferenceNo ?? string.Empty);
            }
        }

        /// <summary>
        /// Used for displaying Status
        /// </summary>
        [JsonIgnore]
        public string StatusDisplay
        {
            get
            {
                return StatusDescription ?? string.Empty;
            }
        }

        /// <summary>
        /// RGB of the Status
        /// </summary>
        [JsonIgnore]
        public int[] StatusColor
        {
            get
            {
                switch (StatusColorDisplay)
                {
                    case Color.Green:
                        {
                            return new int[] { 32, 189, 76 };
                        }
                    case Color.Orange:
                        {
                            return new int[] { 255, 158, 67 };
                        }
                    case Color.Grey:
                    default:
                        {
                            return new int[] { 73, 73, 74 };
                        }
                }
            }
        }

        [JsonIgnore]
        private Color StatusColorDisplay
        {
            get
            {
                Color color = Color.Grey;
                if (StatusDescriptionColor.IsValid())
                {

                    switch (StatusDescriptionColor.ToUpper())
                    {
                        case "COMPLETED":
                            {
                                color = Color.Green;
                                break;
                            }
                        case "ACTION":
                            {
                                color = Color.Orange;
                                break;
                            }
                        case "CANCELLED":
                        default:
                            {
                                color = Color.Grey;
                                break;
                            }
                    }
                }
                return color;
            }
        }
    }
}