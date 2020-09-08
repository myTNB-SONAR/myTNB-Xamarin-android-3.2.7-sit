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

    }
}