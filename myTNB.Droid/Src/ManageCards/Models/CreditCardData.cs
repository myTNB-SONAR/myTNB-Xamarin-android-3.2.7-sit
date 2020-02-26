using myTNB_Android.Src.MultipleAccountPayment.Models;
using Newtonsoft.Json;

namespace myTNB_Android.Src.ManageCards.Models
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

        public static CreditCardData Copy(CreditCard card)
        {


            return new CreditCardData()
            {
                Id = card.Id,
                Email = card.Email,
                LastDigits = card.LastDigits,
                CardType = card.CardType,
                CreatedDate = card.CreatedDate
            };
        }
    }
}