namespace myTNB.Model
{
    public class ContactDetailsResponseModel
    {
        public ContactDetailsDataModel d { set; get; }
    }

    public class ContactDetailsDataModel : BaseModelRefresh
    {
        public ContactDetailsModel data { set; get; }
        public bool IsBusinessFail
        {
            get
            {
                return ErrorCode == "7205";
            }
        }
    }

    public class ContactDetailsModel
    {
        public string Email { set; get; }
        public string Mobile { set; get; }
        public bool isAllowEdit { set; get; }
    }
}