using Newtonsoft.Json;

namespace myTNB_Android.Src.NotificationDetails.Models
{
    public class NotificationDetails
    {
        [JsonProperty("Id")]
        public string Id { get; set; }

        [JsonProperty("Email")]
        public string Email { get; set; }

        [JsonProperty("DeviceId")]
        public string DeviceId { get; set; }

        [JsonProperty("AccountNum")]
        public string AccountNum { get; set; }

        [JsonProperty("Title")]
        public string Title { get; set; }

        [JsonProperty("Message")]
        public string Message { get; set; }

        [JsonProperty("IsRead")]
        public bool IsRead { get; set; }

        [JsonProperty("IsDeleted")]
        public bool IsDeleted { get; set; }

        [JsonProperty("NotificationTypeId")]
        public string NotificationTypeId { get; set; }

        [JsonProperty("BCRMNotificationTypeId")]
        public string BCRMNotificationTypeId { get; set; }

        [JsonProperty("CreatedDate")]
        public string CreatedDate { get; set; }

        [JsonProperty("NotificationType")]
        public string NotificationType { get; set; }

        [JsonProperty("Target")]
        public string Target { get; set; }

        [JsonProperty("IsSMRPeriodOpen")]
        public bool IsSMRPeriodOpen { get; set; }

        [JsonProperty("MerchantTransId")]
        public string MerchantTransId { get; set; }

        [JsonProperty("AccountDetails", Required = Newtonsoft.Json.Required.AllowNull)]
        public AccountDetailsData AccountDetails;

        [JsonProperty("AccountStatementDetail", Required = Newtonsoft.Json.Required.AllowNull)]
        public AccountStatementDetailData AccountStatementDetail;

        [JsonProperty("SDStatusDetails", Required = Newtonsoft.Json.Required.AllowNull)]
        public SDStatusDetailsData SDStatusDetails;

        [JsonProperty("HeaderTitle")]
        public string HeaderTitle { get; set; }

        [JsonProperty("MyHomeDetails")]
        public MyHomeDetailsData MyHomeDetails { get; set; }

        [JsonProperty("ApplicationStatusDetail", Required = Newtonsoft.Json.Required.AllowNull)]
        public ApplicationStatusDetailData ApplicationStatusDetail { get; set; }

        public class AccountDetailsData
        {
            [JsonProperty("BillDate")]
            public string BillDate { get; set; }
            [JsonProperty("AmountPayable")]
            public double AmountPayable { get; set; }
            [JsonProperty("PaymentDueDate")]
            public string PaymentDueDate { get; set; }
        }

        public class AccountStatementDetailData
        {
            [JsonProperty("StatementPeriod")]
            public string StatementPeriod { get; set; }
        }

        public class SDStatusDetailsData
        {
            [JsonProperty("NotificationTimestamp")]
            public string NotificationTimestamp { get; set; }

            [JsonProperty("ServiceDisruptionID")]
            public string ServiceDisruptionID { get; set; }
        }

        public class MyHomeDetailsData
        {
            [JsonProperty("SSODomain")]
            public string SSODomain { get; set; }

            [JsonProperty("OriginURL")]
            public string OriginURL { get; set; }

            [JsonProperty("RedirectURL")]
            public string RedirectURL { get; set; }
        }

        public class ApplicationStatusDetailData
        {
            [JsonProperty("SaveApplicationId")]
            public string SaveApplicationId { get; set; }

            [JsonProperty("ApplicationID")]
            public string ApplicationID { get; set; }

            [JsonProperty("ApplicationType")]
            public string ApplicationType { get; set; }

            [JsonProperty("System")]
            public string System { get; set; }

            [JsonProperty("MessageCategory")]
            public string MessageCategory { get; set; }

            [JsonProperty("MessageCode")]
            public string MessageCode { get; set; }

            [JsonProperty("Type")]
            public string Type { get; set; }
        }
    }
}