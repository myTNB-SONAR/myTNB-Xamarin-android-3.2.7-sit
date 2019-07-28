namespace myTNB.Model
{
    public class ContactDetailsResponseModel
    {
        public ContactDetailsDataModel d { set; get; }
    }

    public class ContactDetailsDataModel : BaseModelV2
    {
        public ContactDetailsModel data { set; get; }
    }

    public class ContactDetailsModel
    {
        public string Email { set; get; }
        public string Mobile { set; get; }
    }
}