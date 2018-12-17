namespace myTNB.SitecoreCMS.Models
{
    public class FAQsModel
    {
        public string Image { set; get; }
        public string Question { set; get; }
        public string Answer { set; get; }
        public string ID { set; get; }
    }

    public class FAQsParentModel
    {
        public string Timestamp { set; get; }
        public string ID { set; get; }
    }
}