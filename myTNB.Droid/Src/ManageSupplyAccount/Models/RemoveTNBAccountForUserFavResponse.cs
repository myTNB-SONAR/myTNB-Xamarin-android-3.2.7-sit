using Newtonsoft.Json;

namespace myTNB.Android.Src.ManageSupplyAccount.Models
{
    public class RemoveTNBAccountForUserFavResponse
    {
        [JsonProperty(PropertyName = "d")]
        public RemoveTNBAccountForUserFav Data { get; set; }

        public class RemoveTNBAccountForUserFav
        {
            [JsonProperty(PropertyName = "__type")]
            public string Type { get; set; }

            [JsonProperty(PropertyName = "status")]
            public string Status { get; set; }

            [JsonProperty(PropertyName = "isError")]
            public bool IsError { get; set; }

            [JsonProperty(PropertyName = "message")]
            public string Message { get; set; }
        }
    }
}