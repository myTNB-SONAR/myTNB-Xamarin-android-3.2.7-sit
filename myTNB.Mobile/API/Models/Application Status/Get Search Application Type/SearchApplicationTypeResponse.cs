using System;
using System.Collections.Generic;
using System.Text;
using myTNB.Mobile.API.Base;
using myTNB.Mobile.Extensions;
using Newtonsoft.Json;

namespace myTNB
{
    public class SearchApplicationTypeResponse : BaseListResponse<SearchApplicationTypeModel>
    {

    }

    public class SearchApplicationTypeModel
    {
        [JsonProperty("searchApplicationTypeId")]
        public string SearchApplicationTypeId { set; get; }

        [JsonProperty("searchApplicationTypeDesc")]
        public LanguageDisplayModel SearchApplicationTypeDesc { set; get; }

        [JsonProperty("searchApplicationNoInputMask")]
        public string SearchApplicationNoInputMask { set; get; }

        [JsonProperty("searchTypes")]
        public List<SearchType> SearchTypes { set; get; }

        [JsonProperty("userRole")]
        public List<string> UserRole { set; get; }

        //Mark: Display Specific Properties
        /// <summary>
        /// Used to display the hint if Application Number is selected
        /// </summary>
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

        /// <summary>
        /// Search By Description Display based on language
        /// </summary>
        [JsonIgnore]
        public string SearchApplicationTypeDescDisplay
        {
            get
            {
                return AppInfoManager.Instance.Language == LanguageManager.Language.EN
                    ? SearchApplicationTypeDesc.EN
                    : SearchApplicationTypeDesc.MS;
            }
        }
    }

    public class SearchType
    {
        [JsonProperty("searchTypeId")]
        public string SearchTypeId { set; get; }

        [JsonProperty("searchTypeDesc")]
        public LanguageDisplayModel SearchTypeDesc { set; get; }

        //Mark: Display Specific Properties
        /// <summary>
        /// Application Type Description Display based on language
        /// </summary>
        [JsonIgnore]
        public string SearchTypeDescDisplay
        {
            get
            {
                return AppInfoManager.Instance.Language == LanguageManager.Language.EN
                    ? SearchTypeDesc.EN
                    : SearchTypeDesc.MS;
            }
        }

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

    public class LanguageDisplayModel
    {
        public string EN { set; get; } = string.Empty;
        public string MS { set; get; } = string.Empty;
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