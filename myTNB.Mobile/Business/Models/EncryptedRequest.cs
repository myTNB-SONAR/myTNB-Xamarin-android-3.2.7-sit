namespace myTNB.Mobile.Business
{
    public class EncryptedRequest
    {
        public EncryptedDataRequest dt { set; get; }
    }

    public class EncryptedDataRequest
    {
        public string ae { set; get; }
        public string ak { set; get; }
        public string av { set; get; }
    }
}