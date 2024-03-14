using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace myTNB.Android.Src.SSMR.SMRApplication.Api
{
    public class GetAccountsSMREligibilityResponse
    {
        [JsonProperty(PropertyName = "d")]
        public AccountsSMREligibilityResponse Response { get; set; }

        public class AccountsSMREligibilityResponse
        {
            [JsonProperty(PropertyName = "__type")]
            public string Type { get; set; }

            [JsonProperty(PropertyName = "data")]
            public SMREligibilityListModel Data { get; set; }

            [JsonProperty(PropertyName = "status")]
            public string Status { get; set; }

            [JsonProperty(PropertyName = "message")]
            public string Message { get; set; }

            [JsonProperty(PropertyName = "ErrorCode")]
            public string ErrorCode { get; set; }

            [JsonProperty(PropertyName = "ErrorMessage")]
            public string ErrorMessage { get; set; }

            [JsonProperty(PropertyName = "DisplayMessage")]
            public string DisplayMessage { get; set; }

            [JsonProperty(PropertyName = "DisplayType")]
            public string DisplayType { get; set; }

            [JsonProperty(PropertyName = "DisplayTitle")]
            public string DisplayTitle { get; set; }
        }

        public class SMREligibilityModel
        {
            [JsonProperty(PropertyName = "ContractAccount")]
            public string ContractAccount { get; set; }

            [JsonProperty(PropertyName = "SMREligibility")]
            public string SMREligibility { get; set; }

            [JsonProperty(PropertyName = "IsSMRTagged")]
            public string IsSMRTagged { get; set; }

        }

        public class SMREligibiltyPopUpDetails
        {
            [JsonProperty(PropertyName = "Title")]
            public string Title { get; set; }

            [JsonProperty(PropertyName = "Description")]
            public string Description { get; set; }

            [JsonProperty(PropertyName = "CTA")]
            public string CTA { get; set; }

            [JsonProperty(PropertyName = "Type")]
            public string Type { get; set; }
        }

        public class SMREligibilityListModel
        {
            [JsonProperty(PropertyName = "accountEligibilities")]
            public List<SMREligibilityModel> SMREligibilityList { get; set; }

            [JsonProperty(PropertyName = "SMREligibiltyPopUpDetails")]
            public List<SMREligibiltyPopUpDetails> SMREligibiltyPopUpDetailList { get; set; }

        }

        
    }
}
