using Newtonsoft.Json;
using Refit;
using System;
using System.Linq;

namespace myTNB.Android.Src.MultipleAccountPayment.Model
{
    public class CardDetails
    {
        public int cardId;

        public string cardNoMasked;

        public string cardNoPrintable;

        public bool saveCard;

        [JsonProperty(PropertyName = "CardNo")]
        [AliasAs("CardNo")]
        public string cardNo { get; set; }

        [JsonProperty(PropertyName = "NameOnCard")]
        [AliasAs("NameOnCard")]
        public string NameOnCard { get; set; }

        [JsonProperty(PropertyName = "CardEXp")]
        [AliasAs("CardExp")]
        public string cardExp { get; set; }

        [JsonProperty(PropertyName = "CardExpMonth")]
        [AliasAs("CardExpMonth")]
        public string CardExpMonth { get; set; }

        [JsonProperty(PropertyName = "CardExpYear")]
        [AliasAs("CardExpYear")]
        public string CardExpYear { get; set; }

        [JsonProperty(PropertyName = "cardCVV")]
        [AliasAs("cardCVV")]
        public string cardCVV { get; set; }

        [JsonProperty(PropertyName = "email")]
        [AliasAs("email")]
        public string email { get; set; }

        public CardDetails(string cardno, string nameoncard, string cardexp, string cardcvv, bool saveCard)
        {
            this.cardNo = cardno;
            this.cardNoMasked = GetMaskedCardNumber();
            this.NameOnCard = nameoncard;
            this.cardExp = cardexp;
            string[] temp = cardexp.Split(new string[] { "/" }, StringSplitOptions.None);
            this.CardExpMonth = temp[0];
            this.CardExpYear = temp[1];
            this.cardCVV = cardcvv;
            this.saveCard = saveCard;
        }

        public string GetCreditCardTypeName()
        {
            if (cardNo.Substring(0, 1).Equals("4"))
            {
                return "VISA";
            }
            else if (cardNo.Substring(0, 1).Equals("5"))
            {
                return "MASTERCARD";
            }
            else if (cardNo.Length > 2 && (cardNo.Substring(0, 2).Equals("37") || cardNo.Substring(0, 2).Equals("34")))
            {
                return "AMEX";
            }
            else if (cardNo.Length > 2 && (cardNo.Substring(0, 2).Equals("35")))
            {
                return "JCB";
            }
            return null;
        }

        public string GetCreditCardType()
        {
            if (cardNo.Substring(0, 1).Equals("4"))
            {
                return "V";
            }
            else if (cardNo.Substring(0, 1).Equals("5"))
            {
                return "M";
            }
            else if (cardNo.Length > 2 && (cardNo.Substring(0, 2).Equals("37") || cardNo.Substring(0, 2).Equals("34")))
            {
                return "A";
            }
            else if (cardNo.Length > 2 && (cardNo.Substring(0, 2).Equals("35")))
            {
                return "J";
            }
            return null;
        }

        public string GetMaskedCardNumber()
        {
            //cardNoPrintable = cardNo.Substring(cardNo.Length - 4).PadLeft(cardNo.Length, '*');
            string cardNoPrintable = null;
            if (cardNo.Length == 16)
            {
                string[] subStrings = Enumerable.Range(0, 4).Select(n => cardNo.Substring(n * 4, 4)).ToArray();
                cardNoPrintable = String.Format("XXXX XXXX XXXX {0}", subStrings[3]);
            }
            else if (cardNo.Length == 15)
            {
                string subStrings = cardNo.Substring(12);
                cardNoPrintable = String.Format("XXXX XXXX XXXX {0}", subStrings);
            }
            return cardNoPrintable;
        }
    }
}