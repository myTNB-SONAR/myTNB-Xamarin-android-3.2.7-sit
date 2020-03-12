using SQLite;

namespace myTNB.SitecoreCM.Models
{
    public class PromotionsModelV2
    {
        [PrimaryKey]
        public string ID { set; get; }

        public string GeneralLinkUrl { set; get; }
        public string SubText { set; get; }
        public string Text { set; get; }
        public string Title { set; get; }
        public string HeaderContent { set; get; }
        public string BodyContent { set; get; }
        public string FooterContent { set; get; }
        public string PortraitImage { set; get; }
        public string LandscapeImage { set; get; }
        public string PromoStartDate { set; get; }
        public string PromoEndDate { set; get; }
        public string PublishedDate { set; get; }
        public string IsPromoExpired { set; get; }
        public string ShowAtAppLaunch { set; get; }

        public bool Read { set; get; }
        public string PromoShownDate { set; get; }
        public bool PromoShown { set; get; }
    }

    public class PromotionParentModelV2
    {
        public string Timestamp { set; get; }
        public string ID { set; get; }
    }
}