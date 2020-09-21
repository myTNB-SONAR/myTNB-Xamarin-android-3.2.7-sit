using System;
using System.Collections.Generic;
using System.Text;
using myTNB.Mobile.API.Base;
using myTNB.Mobile.Extensions;
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

        [JsonIgnore]
        public string ApplicationNoHint
        {
            get
            {
                string hint = string.Empty;
                if (SearchApplicationNoInputMask.IsValid())
                {
                    string eg = LanguageManager.Instance.GetPageValueByKey("Hint", "eg");
                    hint += eg;
                    int counter = 0;
                    StringBuilder stringBuilder = new StringBuilder();
                    for (int i = 0; i < SearchApplicationNoInputMask.Length; i++)
                    {
                        string appendValue = SearchApplicationNoInputMask[i].ToString();
                        if (appendValue == "#")
                        {
                            appendValue = counter.ToString();
                            counter++;
                        }
                        stringBuilder.Append(appendValue);
                    }
                    hint += stringBuilder.ToString();
                }
                return hint;
            }
        }

    }

    public class SearchType
    {
        [JsonProperty("searchTypeId")]
        public string SearchTypeId { set; get; }

        [JsonProperty("searchTypeDesc")]
        public string SearchTypeDesc { set; get; }

        //Mark: Display Specific Properties
        [JsonIgnore]
        public ApplicationStatusSearchType Type
        {
            get
            {
                ApplicationStatusSearchType searchType;
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