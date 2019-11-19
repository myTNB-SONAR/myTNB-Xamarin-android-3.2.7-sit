using System;

namespace myTNB
{
    public static class DashboardHomeConstants
    {
        //Account List
        public static int InitialLoadMaxCount = 3;
        public static int MaxAccountPerLoad = 5;
        public static nfloat SearchViewHeight = ScaleUtility.GetScaledHeight(37f);
        public static nfloat PageControlHeight = ScaleUtility.GetScaledHeight(20f);
        public static nfloat EmptyAcctHeight = ScaleUtility.GetScaledHeight(101F);
        public static nfloat AccountCellHeight = ScaleUtility.GetScaledHeight(61F);
        public static nfloat ShimmerAcctHeight = ScaleUtility.GetScaledHeight(105F);

        //Cell
        public static string Cell_Accounts = "accountsTableViewCell";
        public static string Cell_Help = "helpTableViewCell";
        public static string Cell_Services = "servicesTableViewCell";
        public static string Cell_Promotion = "promotionTableViewCell";
        public static string Cell_AccountList = "accountListTableViewCell";
        public static string Cell_AccountListEmpty = "accountListEmptyTableViewCell";
        public static string Cell_RearrangeAccount = "rearrangeAccountTableViewCell";

        //I18N Keys
        public static string I18N_Afternoon = "greeting_afternoon";
        public static string I18N_Evening = "greeting_evening";
        public static string I18N_Morning = "greeting_morning";
        public static string I18N_NeedHelp = "needHelp";
        public static string I18N_MyAccounts = "myAccounts";
        public static string I18N_MyServices = "myServices";
        public static string I18N_Promotions = "promotions";
        public static string I18N_AddElectricityAcct = "addElectricityAcct";
        public static string I18N_MyAccts = "myAccounts";
        public static string I18N_SearchPlaceholder = "searchPlaceholder";
        public static string I18N_RefreshMsg = "refreshMessage";
        public static string I18N_RefreshBtnTxt = "refreshBtnText";
        public static string I18N_GetBy = "getBy";
        public static string I18N_PayBy = "payBy";
        public static string I18N_PaidExtra = "paidExtra";
        public static string I18N_AllCleared = "allCleared";
        public static string I18N_Add = "add";
        public static string I18N_Search = "search";
        public static string I18N_ShowLess = "showLess";
        public static string I18N_MoreAccts = "moreAccts";
        public static string I18N_RearrangeAccts = "rearrangeAccts";
        public static string I18N_ShowMore = "showMore";
        public static string I18N_ViewAdvice = "viewAdvice";
        public static string I18N_ViewAdvices = "viewAdvices";
        public static string I18N_ViewEBill = "viewEBill";
        public static string I18N_ViewEBills = "viewEBills";
        public static string I18N_PayBill = "payBill";
        public static string I18N_PayBills = "payBills";
        public static string I18N_SubmitFeedback = "submitFeedback";
        public static string I18N_SelfMeterReading = "selfMeterReading";
        public static string I18N_ViewEBillAndAdvice = "viewEBillAndAdvice";
        public static string I18N_TutorialAccountTitle = "tutorialAccountTitle";
        public static string I18N_TutorialQuickAccessTitle = "tutorialQuickAccessTitle";
        public static string I18N_TutorialQuickActionTitle = "tutorialQuickActionTitle";
        public static string I18N_TutorialNeedHelpTitle = "tutorialNeedHelpTitle";
        public static string I18N_TutorialNoAcctDesc = "tutorialNoAcctDesc";
        public static string I18N_TutorialSingleAcctDesc = "tutorialSingleAcctDesc";
        public static string I18N_TutorialThreeAcctsDesc = "tutorialThreeAcctsDesc";
        public static string I18N_TutorialMoreAcctsDesc = "tutorialMoreAcctsDesc";
        public static string I18N_TutorialQuickAccessDesc = "tutorialQuickAccessDesc";
        public static string I18N_TutorialQuickActionDesc = "tutorialQuickActionDesc";
        public static string I18N_TutorialNeedHelpDesc = "tutorialNeedHelpDesc";
        public static string I18N_GotIt = "gotIt";
        public static string I18N_RearrangeNavTitle = "title";
        public static string I18N_RearrangeBtnTitle = "btnTitle";

        //Image
        public static string Img_Notification = "Notification";
        public static string Img_AddIconGrey = "Add-Account-Icon-Grey";
        public static string Img_SearchIconBlue = "Search-Icon-Blue";
        public static string Img_SearchCancelIcon = "Search-Cancel-Icon";
        public static string Img_AddAcctIconWhite = "Add-Account-Icon-White";
        public static string Img_RefreshIcon = "Refresh-Icon";
        public static string Img_SearchIconWhite = "Search-Icon-White";
        public static string Img_SearchActiveIconWhite = "Search-Active-Icon-White";
        public static string Img_ArrowUpWhite = "Arrow-Up-White-Small";
        public static string Img_ArrowDownWhite = "Arrow-Down-White-Small";
        public static string Img_RearrangeIcon = "Rearrange-Icon";
        public static string Img_RELeaf = "RE-Leaf";
        public static string Img_ArrowDownBlue = "Arrow-Down-Blue-Small";
        public static string Img_ArrowUpBlue = "Arrow-Up-Blue-Small";

        //PageName
        public static string PageName = "DashboardHome";
        public static string PageNameRearrange = "RearrangeAccount";

        //Cell Index
        public static int CellIndex_Accounts = 0;
        public static int CellIndex_Services = 1;
        public static int CellIndex_Promotion = 2;
        public static int CellIndex_Help = 2;

        //SMR CTA
        public static string CTA_ShowReadingHistory = "v";
        public static string CTA_ShowSubmitReading = "s";

        //SiteCore
        public static string Sitecore_Timestamp = "SiteCorePromotionTimeStamp";
        public static string Sitecore_Success = "SUCCESS";

        //Format
        public static string Format_Date = "yyyyMMdd";

        //Preference Key
        public static string Pref_TutorialOverlay = "HomeTutorialOverlayV2.0.0";
    }
}