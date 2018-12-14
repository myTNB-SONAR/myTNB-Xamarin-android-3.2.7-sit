namespace myTNB.SitecoreCMS.Model
{
    public class PromotionsModel
    {
        public string Image { set; get; }
        public string Title { set; get; }
        public string Text { set; get; }
        public string SubText { set; get; }
        public string CampaignPeriod { set; get; }
        public string Prizes { set; get; }
        public string HowToWin { set; get; }
        public string FooterNote { set; get; }
        public string PublishedDate { set; get; }
        public string GeneralLinkUrl { set; get; }
        public string GeneralLinkText { set; get; }
        public string ID { set; get; }
        public bool IsRead { set; get; }
    }

    public class PromotionParentModel
    {
        public string Timestamp { set; get; }
        public string ID { set; get; }
    }
}