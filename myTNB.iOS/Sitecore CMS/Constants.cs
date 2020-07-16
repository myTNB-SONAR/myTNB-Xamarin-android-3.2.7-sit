namespace myTNB.SitecoreCMS
{
    public static class Constants
    {
        public struct Sitecore
        {
            public struct Templates
            {
                public const string MyTNBHome = "{D80CC87A-CA6F-4BBA-8A18-743F3B64F6FA}";
                public const string Timestamp = "{1F9D1E01-5C6E-43AD-9EAB-2CAD3328611A}";
                public const string WalkthroughScreens = "{9AE3E8C9-6C50-419D-AC3E-55A1D9D99E1B}";
                public const string EnergyTips = "{C1448C37-0E1F-4879-877C-92D0768DC87B}";
                public const string PreLoginPromo = "{3C1FD061-FBD7-4987-AF31-72519F3DD6A1}";
                public const string Locations = "{1F49F2D1-DF07-4AF0-8553-6204D23D552D}";
                public const string FullRTEPages = "{E57E72BF-12E7-44CF-8287-A0D931EBE237}";
                public const string Promotions = "{3216AF00-0C54-41E9-B574-0C73758285B4}";
            }

            public struct Fields
            {
                public struct Shared
                {
                    public const string Text = "Text";
                    public const string SubText = "SubText";
                    public const string GeneralText = "GeneralText";
                    public const string GeneralLink = "GeneralLink";
                    public const string Image = "Image";
                    public const string Title = "Title";
                }

                public struct FullRTEPages
                {
                    public const string PublishedDate = "PublishedDate";
                }

                public struct Promotions
                {
                    public const string GeneralLink = "GeneralLink";
                    public const string SubText = "SubText";
                    public const string Text = "Text";
                    public const string Title = "Title";
                    public const string HeaderContent = "HeaderContent";
                    public const string BodyContent = "BodyContent";
                    public const string FooterContent = "FooterContent";
                    public const string PortraitImage = "PortraitImage";
                    public const string LandscapeImage = "LandscapeImage";
                    public const string PromoStartDate = "PromoStartDate";
                    public const string PromoEndDate = "PromoEndDate";
                    public const string PublishedDate = "PublishedDate";
                    public const string isPromoExpired = "isPromoExpired";
                    public const string ShowAtAppLaunch = "ShowAtAppLaunch";
                }

                public struct FAQs
                {
                    public const string Question = "Question";
                    public const string Answer = "Answer";
                }

                public struct Timestamp
                {
                    public const string TimestampField = "Timestamp";
                }

                public struct Help
                {
                    public const string ShowNeedHelp = "Show Need Help";
                    public const string TopicBGImage = "TopicBGImage";
                    public const string BGStartColor = "BGStartColor";
                    public const string BGEndColor = "BGEndColor";
                    public const string BGGradientDirection = "BGGradientDirection";
                    public const string Title = "Title";
                    public const string Description = "Description";
                    public const string TopicBodyTitle = "TopicBodyTitle";
                    public const string TopicBodyContent = "TopicBodyContent";
                    public const string CTA = "CTA";
                    public const string Tags = "Tags";
                    public const string TargetItem = "Target-item";
                }

                public struct ApplySSMRWalkthrough
                {
                    public const string Title = "Title";
                    public const string Description = "Description";
                    public const string Image = "Image";
                }

                public struct MeterReadSSMRWalkthrough
                {
                    public const string Title = "Title";
                    public const string Description = "Description";
                    public const string Image = "Image";
                }

                public struct EnergyTips
                {
                    public const string Title = "Title";
                    public const string Description = "Description";
                    public const string Image = "Image";
                }

                //Created by Syahmi ICS 05052020
                public struct EppInfoTooltip
                {
                    public const string Title = "Tool Tip Title";
                    public const string PopUpTitle = "Pop Up Title";
                    public const string PopUpBody = "Pop Up Body";
                    public const string Image = "Image";
                }

                public struct BillDetailsTooltip
                {
                    public const string Title = "Title";
                    public const string Description = "Description";
                    public const string Image = "Image";
                }

                public struct AppLaunchImage
                {
                    public const string Description = "Description";
                    public const string StartDateTime = "StartDateTime";
                    public const string EndDateTime = "EndDateTime";
                    public const string ShowForSeconds = "ShowForSeconds";
                }

                public struct ImageName
                {
                    //iOS
                    public const string Image = "Image";
                    public const string Image_2X = "image_2x";
                    public const string Image_3X = "image_3x";
                    //Android
                    public const string Image_HDPI = "image_hdpi";
                    public const string Image_MDPI = "image_mdpi";
                    public const string Image_XHDPI = "image_xhdpi";
                    public const string Image_XXHDPI = "image_xxhdpi";
                    public const string Image_XXXHDPI = "image_xxxhdpi";
                }

                public struct Language
                {
                    public const string LanguageFile = "Language File";
                }

                public struct Country
                {
                    public const string CountryFile = "Language File";//"Country File";
                }

                public struct Rewards
                {
                    public const string Category = "RewardCategory";
                    public const string Title = "Title";
                    public const string TitleOnListing = "TitleOnListing";
                    public const string Description = "Description";
                    public const string Image = "Image";
                    public const string RewardPeriodText = "RewardPeriodText";
                    public const string LocationsText = "LocationsText";
                    public const string TermsAndConditions = "TermsAndConditions";
                    public const string StartDateTime = "StartDateTime";
                    public const string EndDateTime = "EndDateTime";
                    public const string RewardUseWithinTime = "RewardUseWithinTime";
                    public const string RewardUseTitle = "RewardUseTitle";
                    public const string RewardUseDescription = "RewardUseDescription";
                }

                public struct WhatsNew
                {
                    public const string Category = "WhatsNewCategory";
                    public const string Title = "Title";
                    public const string TitleOnListing = "TitleOnListing";
                    public const string Description = "Description";
                    public const string Image = "Image";
                    public const string StartDate = "StartDate";
                    public const string EndDate = "EndDate";
                    public const string PublishDate = "PublishDate";
                    public const string Image_DetailsView = "Image_DetailsView";
                    public const string Styles_DetailsView = "Styles_DetailsView";
                    public const string PortraitImage_PopUp = "PortraitImage_PopUp";
                    public const string ShowEveryCountDays_PopUp = "ShowEveryCountDays_PopUp";
                    public const string ShowForTotalCountDays_PopUp = "ShowForTotalCountDays_PopUp";
                    public const string ShowAtAppLaunchPopUp = "ShowAtAppLaunchPopUp";
                    public const string PopUp_Text_Only = "PopUp_Text_Only";
                    public const string PopUp_HeaderImage = "PopUp_HeaderImage";
                    public const string PopUp_Text_Content = "PopUp_Text_Content";
                    public const string Donot_Show_In_WhatsNew = "Donot_Show_In_WhatsNew";
                    public const string Disable_DoNotShow_Checkbox = "Disable_DoNotShow_Checkbox";
                }
            }

            public struct ItemID
            {
                public const string WalkthroughScreens = "{82916769-EB51-4C0B-AC65-5C9D94989D35}";
                public const string EnergyTips = "{7554B577-7220-472A-BA73-A4FC84EEFC2E}";
                public const string Timestamp = "{0CF0191C-8E83-47A7-B225-FA49F614BCDA}";
                public const string Locations = "{142AE94D-86D9-4FB4-88AB-F7F75B77323B}";
                public const string PreLoginPromo = "{8F33E504-01AC-45BC-97A2-D46795F9AEBC}";
                public const string FullRTEPages = "{41C06271-8712-4E29-BFC4-50FC9CA24132}";
                public const string Promotions = "{EF6BF00E-A039-4F69-8ADC-01A77C25FC2D}";
                public const string FAQs = "{28587E38-0753-4B6B-8EA8-024022033846}";
            }

            public struct ItemPath
            {
                public const string Promotions = "/sitecore/content/myTNBapp/Contents/PromotionsV2";
                public const string Help = "/sitecore/content/myTNBapp/Contents/Need Help";
                public const string FAQs = "/sitecore/content/myTNBapp/Contents/FrequentlyAskedQuestions";
                public const string Timestamp = "/sitecore/content/myTNBapp/Timestamp";
                public const string WalkthroughScreens = "/sitecore/content/myTNBapp/Contents/Walkthrough Screens";
                public const string FullRTEPages = "/sitecore/content/myTNBapp/Contents/Full RTE Pages/Terms Condition";
                public const string ApplySSMRWalkthrough = "/sitecore/content/myTNBapp/Contents/Walkthrough SSMR Apply";
                public const string MeterReadSSMRWalkthrough = "/sitecore/content/myTNBapp/Contents/Walkthrough SSMR MeterRead OnePhase";
                public const string MeterReadSSMRWalkthroughV2 = "/sitecore/content/myTNBapp/Contents/Walkthrough SSMR MeterRead ThreePhase";
                public const string MeterReadSSMRWalkthroughOCROff = "/sitecore/content/myTNBapp/Contents/Walkthrough SSMR MeterRead OnePhase OCR Off";
                public const string MeterReadSSMRWalkthroughV2OCROff = "/sitecore/content/myTNBapp/Contents/Walkthrough SSMR MeterRead ThreePhase OCR Off";
                public const string EnergyTips = "/sitecore/content/myTNBapp/Contents/Energy Saving Tips";
                public const string BillDetailsTooltip = "/sitecore/content/myTNBapp/Contents/Itemised Billing Tooltip";
                public const string AppLaunchImage = "/sitecore/content/myTNBapp/Contents/App Launch Image";
                public const string Rewards = "/sitecore/content/myTNBapp/Contents/Rewards";
                public const string WhatsNew = "/sitecore/content/myTNBapp/Contents/WhatsNewV2";
                public const string Language = "/sitecore/content/myTNBapp/Contents/App Language Json";
                public const string Country = "/sitecore/content/myTNBapp/Contents/Country List Json";

                //Created by Syahmi ICS 05052020
                public const string EppToolTip = "/sitecore/content/myTNBapp/Contents/EppToolTip";
            }
        }
    }
}