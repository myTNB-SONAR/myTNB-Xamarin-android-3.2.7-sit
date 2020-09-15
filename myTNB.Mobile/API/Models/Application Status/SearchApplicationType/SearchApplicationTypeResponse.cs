using System;
using System.Collections.Generic;
using myTNB.Mobile.API.Base;
using Newtonsoft.Json;

namespace myTNB
{
    public class SearchApplicationTypeResponse : BaseListResponse<SearhApplicationTypeModel>
    {

    }

    public class SearhApplicationTypeModel
    {
        [JsonProperty("searchApplicationTypeId")]
        public string SearchApplicationTypeId { set; get; }

        [JsonProperty("searchApplicationTypeDesc")]
        public string SearchApplicationTypeDesc { set; get; }

        [JsonProperty("searchApplicationNoInputMask")]
        public string SearchApplicationNoInputMask { set; get; }

        [JsonProperty("searchTypes")]
        public List<SearchType> SearchTypes { set; get; }

        [JsonProperty("userRole")]
        public List<string> UserRole { set; get; }

    }

    public class SearchType
    {
        [JsonProperty("searchTypeId")]
        public string SearchTypeId { set; get; }

        [JsonProperty("searchTypeDesc")]
        public string SearchTypeDesc { set; get; }

        [JsonIgnore]
        public ApplicationStatusSearchType Type
        {
            get
            {
                ApplicationStatusSearchType searchType = ApplicationStatusSearchType.None;
                switch (SearchTypeId)
                {
                    case "CA":
                        {
                            searchType = ApplicationStatusSearchType.CA;
                            break;
                        }
                    case "ApplicationNo":
                        {
                            searchType = ApplicationStatusSearchType.ApplicationNo;
                            break;
                        }
                    case "ServiceRequestNo":
                        {
                            searchType = ApplicationStatusSearchType.ServiceRequestNo;
                            break;
                        }
                    case "ServiceNotificationNo":
                        {
                            searchType = ApplicationStatusSearchType.ServiceNotificationNo;
                            break;
                        }
                    case "ProjectReferenceNo":
                        {
                            searchType = ApplicationStatusSearchType.ProjectReferenceNo;
                            break;
                        }
                    default:
                        {
                            searchType = ApplicationStatusSearchType.None;
                            break;
                        }
                }
                return searchType;
            }
        }
    }

    public enum ApplicationStatusSearchType
    {
        None,
        CA,
        ApplicationNo,
        ServiceRequestNo,
        ServiceNotificationNo,
        ProjectReferenceNo
    }
}