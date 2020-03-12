using System.Collections.Generic;

namespace myTNB.Model
{
    public class RegisteredCardsResponseModel
    {
        public RegisteredCardsModel d { set; get; }
    }

    public class RegisteredCardsModel : BaseModelV2
    {
        public List<RegisteredCardsDataModel> data { set; get; }
    }

    public class RegisteredCardsDataModel
    {
        public string Id { set; get; }
        public string Email { set; get; }
        public string CardHashCode { set; get; }
        public string CardType { set; get; }
        public string CreatedDate { set; get; }
        public string LastDigits { set; get; }
        public string ExposedDigits { set; get; }
    }
}