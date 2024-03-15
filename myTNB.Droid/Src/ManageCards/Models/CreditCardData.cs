using myTNB.AndroidApp.Src.MultipleAccountPayment.Models;
using Newtonsoft.Json;
using Refit;

namespace myTNB.AndroidApp.Src.ManageCards.Models
{
    public class CreditCardData
    {
        [JsonProperty(PropertyName = "Id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "Email")]
        public string Email { get; set; }

        [JsonProperty(PropertyName = "LastDigits")]
        public string LastDigits { get; set; }

        [JsonProperty(PropertyName = "CardType")]
        public string CardType { get; set; }

        [JsonProperty(PropertyName = "CreatedDate")]
        public string CreatedDate { get; set; }

        [JsonProperty(PropertyName = "ExpiredDate")]
        public string ExpiredDate { get; set; }

        [JsonProperty(PropertyName = "IsExpired")]
        public bool IsExpired { get; set; }

        public static CreditCardData Copy(CreditCard card)
        {


            return new CreditCardData()
            {
                Id = card.Id,
                Email = card.Email,
                LastDigits = card.LastDigits,
                CardType = card.CardType,
                CreatedDate = card.CreatedDate,
                ExpiredDate = card.ExpiredDate,
                IsExpired = card.IsExpired
            };
        }
    }
}