using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using myTNB_Android.Src.MakePayment.Models;
using myTNB_Android.Src.Utils;

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