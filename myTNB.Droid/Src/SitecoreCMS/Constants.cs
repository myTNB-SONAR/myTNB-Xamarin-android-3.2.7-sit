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
                public const string EppToolTip = "{8379FC18-1594-41FD-934A-EB45EDD17541}";
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

                public struct EnergyTips
                {
                    public const string Category = "Category";
                }

                public struct FullRTEPages
                {
                    public const string PublishedDate = "PublishedDate";
                }

                public struct Locations
                {
                    public const string PaymentTypes = "PaymentTypes";
                    public const string Longitude = "Longitude";
                    public const string Latitude = "Latitude";
                }

                public struct FAQs
                {
                    public const string Question = "Question";
                    public const string Answer = "Answer";
                }


                public struct Timestamp
                {
                    public const string TimestampField = "Timestamp";
                    public const string ShowNeedHelpField = "Show Need Help";
                }


                public struct AppLaunch
                {
                    public const string Title = "Title";
                    public const string Description = "Description";
                    public const string Image = "Image";
                    public const string StartDateTime = "StartDateTime";
                    public const string EndDateTime = "EndDateTime";
                    public const string ShowForSeconds = "ShowForSeconds";
                }

                public struct Help
                {
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

                public struct SSMRMeterReadingWalkthrough
                {
                    public const string Title = "Title";
                    public const string Description = "Description";
                    public const string Image = "Image";
                }

                public struct EnergySavingTips
                {
                    public const string Title = "Title";
                    public const string Description = "Description";
                    public const string Image = "Image";
                }

                public struct BillDetailsTooltip
                {
                    public const string Title = "Title";
                    public const string Description = "Description";
                    public const string Image = "Image";
                }

                public struct NewBillDesignDiscoverMore
                {
                    public const string Title = "Title";
                    public const string Description = "Description";
                    public const string Image1 = "Image1";
                    public const string Image2 = "Image2";
                    public const string IsZoomable = "IsZoomable";
                    public const string IsHeader = "IsHeader";
                    public const string IsFooter = "IsFooter";
                    public const string ShouldTrack = "ShouldTrack";
                    public const string DynatraceTag = "DynatraceTag";
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

                public struct Rewards
                {
                    public const string Title = "Title";
                    public const string TitleOnListing = "TitleOnListing";
                    public const string DisplayName = "DisplayName";
                    public const string Description = "Description";
                    public const string Image = "Image";
                    public const string PeriodLabel = "RewardPeriodText";
                    public const string LocationLabel = "LocationsText";
                    public const string TandCLabel = "TermsAndConditions";
                    public const string StartDate = "StartDateTime";
                    public const string EndDate = "EndDateTime";
                    public const string RewardUseWithinTime = "RewardUseWithinTime";
                    public const string RewardUseTitle = "RewardUseTitle";
                    public const string RewardUseDescription = "RewardUseDescription";
                    public const string RewardCategory = "RewardCategory";
                }

                public struct WhatsNew
                {
                    public const string Title = "Title";
                    public const string TitleOnListing = "TitleOnListing";
                    public const string DisplayName = "DisplayName";
                    public const string Description = "Description";
                    public const string Image = "Image";
                    public const string StartDate = "StartDate";
                    public const string PublishDate = "PublishDate";
                    public const string EndDate = "EndDate";
                    public const string CTA = "CTA";
                    public const string Image_DetailsView = "Image_DetailsView";
                    public const string Styles_DetailsView = "Styles_DetailsView";
                    public const string PortraitImage_PopUp = "PortraitImage_PopUp";
                    public const string ShowEveryCountDays_PopUp = "ShowEveryCountDays_PopUp";
                    public const string ShowForTotalCountDays_PopUp = "ShowForTotalCountDays_PopUp";
                    public const string ShowAtAppLaunchPopUp = "ShowAtAppLaunchPopUp";
                    public const string PopUp_Text_Only = "PopUp_Text_Only";
                    public const string PopUp_HeaderImage = "PopUp_HeaderImage";
                    public const string PopUp_Text_Content = "PopUp_Text_Content";
                    public const string Disable_DoNotShow_Checkbox = "Disable_DoNotShow_Checkbox";
                    public const string Donot_Show_In_WhatsNew = "Donot_Show_In_WhatsNew";
                    public const string WhatsNewCategory = "WhatsNewCategory";
                    public const string Infographic_FullView_URL = "Infographic_FullView_URL";
                }

                public struct Language
                {
                    public const string LanguageFile = "Language File";
                }

                public struct EppToolTip
                {
                    public const string Title = "Tool Tip Title";
                    public const string PopUpTitle = "Pop Up Title";
                    public const string PopUpBody = "Pop Up Body";
                    public const string Image = "Image";
                }

                public struct WhereIsMyAccToolTip
                {
                    public const string PopUpTitle = "Pop Up Title";
                    public const string PopUpBody = "Pop Up Body";
                    public const string Image = "Image";
                }

                public struct WhoIsRegisteredOwnerToolTip
                {
                    public const string PopUpTitle = "Pop Up Title";
                    public const string PopUpBody = "Pop Up Body";
                }

                public struct DoIneedOwnerConsentToolTip
                {
                    public const string PopUpTitle = "Pop Up Title";
                    public const string PopUpBody = "Pop Up Body";
                }

                public struct HowDoesCopyOfIdentification
                {
                    public const string PopUpTitle = "Pop Up Title";
                    public const string PopUpBody = "Pop Up Body";
                    public const string Image = "Image";

                }

                public struct HowDoesProofOfConsent
                {
                    public const string PopUpTitle = "Pop Up Title";
                    public const string PopUpBody = "Pop Up Body";
                    public const string Image = "Image";

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
                public const string FAQs = "{28587E38-0753-4B6B-8EA8-024022033846}";
                public const string EPPTips = "{9CC7DD33-7853-427F-BF0C-4CEB0E16F925}";
            }

            public struct ItemPath
            {
                public const string FAQs = "/sitecore/content/myTNBapp/Contents/FrequentlyAskedQuestions";

                public const string Timestamp = "/sitecore/content/myTNBapp/Timestamp";
                public const string WalkthroughScreens = "/sitecore/content/myTNBapp/Contents/Walkthrough Screens";
                public const string FullRTEPages = "/sitecore/content/myTNBapp/Contents/Full RTE Pages/Terms Condition";


                public const string AppLaunch = "/sitecore/content/myTNBapp/Contents/App Launch Image";

                public const string Help = "/sitecore/content/myTNBapp/Contents/Need Help";

                public const string ApplySSMRWalkthrough = "/sitecore/content/myTNBapp/Contents/Walkthrough SSMR Apply";

                public const string EnergySavingTips = "/sitecore/content/myTNBapp/Contents/Energy Saving Tips";

                public const string SSMRMeterReadingOnePhaseWalkthrough = "/sitecore/content/myTNBapp/Contents/Walkthrough SSMR MeterRead OnePhase";
                public const string SSMRMeterReadingThreePhaseWalkthrough = "/sitecore/content/myTNBapp/Contents/Walkthrough SSMR MeterRead ThreePhase";
                public const string BillDetailsTooltip = "/sitecore/content/myTNBapp/Contents/Itemised Billing Tooltip";
                public const string BillDetailsTooltipV2 = "/sitecore/content/myTNBapp/Contents/Itemised Billing Tooltip V2";

                public const string SSMRMeterReadingOnePhaseWalkthroughOCROff = "/sitecore/content/myTNBapp/Contents/Walkthrough SSMR MeterRead OnePhase OCR Off";
                public const string SSMRMeterReadingThreePhaseWalkthroughOCROff = "/sitecore/content/myTNBapp/Contents/Walkthrough SSMR MeterRead ThreePhase OCR Off";

                public const string Rewards = "/sitecore/content/myTNBapp/Contents/Rewards";
                public const string Country = "/sitecore/content/myTNBapp/Contents/Country List Json";
                public const string Language = "/sitecore/content/myTNBapp/Contents/App Language Json";
                public const string WhatsNew = "/sitecore/content/myTNBapp/Contents/WhatsNewV2";

                public const string EppToolTip = "/sitecore/content/myTNBapp/Contents/EppToolTip";
                public const string WhereIsMyAccToolTip = "/sitecore/content/myTNBapp/Contents/WhereIsMyAccToolTip";
                public const string WhoIsRegisteredOwnerToolTip = "/sitecore/content/myTNBapp/Contents/WhoIsRegisteredOwner";
                public const string DoIneedOwnerConsentToolTip = "/sitecore/content/myTNBapp/Contents/DoINeedOwnerConsentToolTip";
                public const string HowDoesCopyOfIdentification = "/sitecore/content/myTNBapp/Contents/HowDoesCopyOfIdentificationToolTip";
                public const string HowDoesProofOfConsent = "/sitecore/content/myTNBapp/Contents/ProofOwnerConsentToolTips";

                public const string NewBillDesignDiscoverMore = "/sitecore/content/myTNBapp/Contents/New Bill Design";
            }
        }
    }
}