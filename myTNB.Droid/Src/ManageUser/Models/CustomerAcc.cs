using myTNB.AndroidApp.Src.Database.Model;
using Newtonsoft.Json;

namespace myTNB.AndroidApp.Src.ManageUser.Models
{
    public class CustomerAcc
    {
        public class CustomerAccountData
        {
            [JsonProperty(PropertyName = "accountDescription")]
            public string accountDescription { get; set; }

        }
    }
}