using myTNB.Android.Src.Database.Model;
using Newtonsoft.Json;

namespace myTNB.Android.Src.ManageSupplyAccount.Models
{
    public class CustomerAccount
    {
        [JsonProperty("accNum")]
        public string AccNum { get; set; }

        [JsonProperty("accDesc")]
        public string AccDesc { get; set; }

        [JsonProperty("userAccountID")]
        public string UserAccountId { get; set; }

        [JsonProperty("icNum")]
        public string ICNum { get; set; }

        [JsonProperty("amCurrentChg")]
        public string AmtCurrentChg { get; set; }

        [JsonProperty("isRegistered")]
        public bool IsRegistered { get; set; }

        [JsonProperty("isPaid")]
        public bool IsPaid { get; set; }

        [JsonProperty("isOwned")]
        public bool IsOwned { get; set; }

        [JsonProperty("isSelected")]
        public bool IsSelected { get; set; }

        public static CustomerAccount Copy(CustomerBillingAccount customerBillingAccount)
        {
            return new CustomerAccount()
            {
                AccNum = customerBillingAccount.AccNum,
                AccDesc = customerBillingAccount.AccDesc,
                UserAccountId = customerBillingAccount.UserAccountId,
                ICNum = customerBillingAccount.ICNum,
                AmtCurrentChg = customerBillingAccount.AmtCurrentChg,
                IsRegistered = customerBillingAccount.IsRegistered,
                IsPaid = customerBillingAccount.IsPaid,
                IsOwned = customerBillingAccount.isOwned,
                IsSelected = customerBillingAccount.IsSelected
            };
        }
    }
}