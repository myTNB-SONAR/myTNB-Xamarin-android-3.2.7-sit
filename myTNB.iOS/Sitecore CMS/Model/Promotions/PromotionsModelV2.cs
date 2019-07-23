
using myTNB.SQLite.SQLiteDataManager;
using SQLite;

namespace myTNB.SitecoreCMS.Model
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
        public bool IsPromoExpired { set; get; }
        public bool ShowAtAppLaunch { set; get; }

        public bool IsRead { set; get; }
        public string PromoShownDate { set; get; }

        public PromotionsEntity ToEntity()
        {
            var entity = new PromotionsEntity
            {
                ID = ID,
                GeneralLinkUrl = GeneralLinkUrl,
                SubText = SubText,
                Text = Text,
                Title = Title,
                HeaderContent = HeaderContent,
                BodyContent = BodyContent,
                FooterContent = FooterContent,
                PortraitImage = PortraitImage,
                LandscapeImage = LandscapeImage,
                PromoStartDate = PromoStartDate,
                PromoEndDate = PromoEndDate,
                PublishedDate = PublishedDate,
                IsPromoExpired = IsPromoExpired,
                ShowAtAppLaunch = ShowAtAppLaunch,
                IsRead = IsRead,
                PromoShownDate = PromoShownDate
            };

            return entity;
        }

    }

    public class PromotionParentModelV2
    {
        public string Timestamp { set; get; }
        public string ID { set; get; }
    }
}
