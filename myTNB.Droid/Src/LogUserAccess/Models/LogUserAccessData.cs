using myTNB_Android.Src.Database.Model;
using Newtonsoft.Json;
using System;

namespace myTNB_Android.Src.LogUserAccess.Models
{
    public class LogUserAccessData
    {
        [JsonProperty("AccountNo")]
        public string AccountNo { get; set; }

        [JsonProperty("Action")]
        public string Action { get; set; }

        [JsonProperty("ActivityLogID")]
        public string ActivityLogID { get; set; }

        [JsonProperty("CreateBy")]
        public string CreateBy { get; set; }

        [JsonProperty("CreatedDate")]
        public DateTime CreatedDate { get; set; }

        [JsonProperty("IsApplyEBilling")]
        public bool IsApplyEBilling { get; set; }

        [JsonProperty("IsHaveAccess")]
        public bool IsHaveAccess { get; set; }

        [JsonProperty("UserID")]
        public string UserID { get; set; }

        [JsonProperty("UserName")]
        public string UserName { get; set; }


        public static LogUserAccessData Get(LogUserAccessEntity entity)
        {
            return new LogUserAccessData()
            {

            };
        }
    }
}