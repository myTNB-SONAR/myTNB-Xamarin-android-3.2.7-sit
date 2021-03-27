using myTNB_Android.Src.Database.Model;
using Newtonsoft.Json;

namespace myTNB_Android.Src.ManageUser.Models
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