namespace myTNB.Home.Bill
{
    public static class BillConstants
    {
        //Pagename
        public static string Pagename_Bills = "Bills";
        public static string Pagename_BillDetails = "BillDetails";
        public static string Pagename_BillFilter = "BillFilter";
        public static string Popup_MandatoryChargesPopUpDetails = "MandatoryChargesPopUpDetails";
        public static string Popup_MandatoryChargesKey = "MandatoryCharges";
        public static string Popup_MandatoryPaymentKey = "MandatoryPayment";

        //Service
        public static string Service_GetAccountsCharges = "GetAccountsCharges";
        public static string Service_GetAccountBillPayHistory = "GetAccountBillPayHistory";
        public static string Service_GetBillHistory = "GetBillHistory";
        public static string Param_RE = "RE";
        public static string Param_UTIL = "UTIL";

        //Format
        public static string Format_DateParse = "yyyyMMdd";
        public static string Format_Date = "dd MMM yyyy";
        public static string Format_Default = "{0} {1}";
        public static string Format_DateCache = @"dd/MM/yyyy";

        //ViewCell
        public static string Cell_BillHistory = "billHistoryTableViewCell";
        public static string Cell_BillSection = "billSectionTableViewCell";
        public static string Cell_BillHistoryShimmer = "billHistoryShimmerViewCell";
        public static string Cell_Refresh = "refreshViewCell";

        //I18N
        //Bill
        public static string I18N_NavTitle = "title";
        public static string I18N_ClearedBills = "clearedBills";
        public static string I18N_ViewMore = "viewMore";
        public static string I18N_PaidExtra = "paidExtra";
        public static string I18N_NeedToPay = "needToPay";
        public static string I18N_By = "by";
        public static string I18N_Pay = "pay";
        public static string I18N_GetBy = "getBy";
        public static string I18N_MyEarnings = "myEarnings";
        public static string I18N_BeenPaidExtra = "beenPaidExtra";
        public static string I18N_NoHistoryData = "noHistoryData";
        public static string I18N_MyHistory = "myHistory";
        public static string I18N_BcrmDownMessage = "bcrmDownMessage";
        public static string I18N_ViewBill = "viewBill";
        public static string I18N_All = "all";
        public static string I18N_TutorialBillTitle = "tutorialBillTitle";
        public static string I18N_TutorialAdviceTitle = "tutorialAdviceTitle";
        public static string I18N_TutorialPayTitle = "tutorialPayTitle";
        public static string I18N_TutorialViewDetailsTitle = "tutorialViewDetailsTitle";
        public static string I18N_TutorialHistoryNormalTitle = "tutorialHistoryNormalTitle";
        public static string I18N_TutorialHistoryTitle = "tutorialHistoryTitle";
        public static string I18N_TutorialBillREAcctDesc = "tutorialBillREAcctDesc";
        public static string I18N_TutorialBillNormalAcctDesc = "tutorialBillNormalAcctDesc";
        public static string I18N_TutorialPayDesc = "tutorialPayDesc";
        public static string I18N_TutorialViewDetailsDesc = "tutorialViewDetailsDesc";
        public static string I18N_TutorialHistoryNormalAcctDesc = "tutorialHistoryNormalAcctDesc";
        public static string I18N_TutorialHistoryREAcctDesc = "tutorialHistoryREAcctDesc";
        public static string I18N_GotIt = "gotIt";

        //Bill Details
        public static string I18N_BillDetails = "billDetails";
        public static string I18N_OutstandingCharges = "outstandingCharges";
        public static string I18N_BillThisMonth = "billThisMonth";
        public static string I18N_MinimumChargeDescription = "minimumChargeDescription";
        public static string I18N_ApplicationCharges = "applicationCharges";
        public static string I18N_ToolTipTitle1 = "tooltiptitle1";
        public static string I18N_ToolTipDesc1 = "tooltipdesc1";
        public static string I18N_ToolTipTitle2 = "tooltiptitle2";
        public static string I18N_ToolTipDesc2 = "tooltipdesc2";
        public static string I18N_TutorialTitle = "tutorialTitle";
        public static string I18N_TutorialDesc = "tutorialDesc";

        //Bill Filter
        public static string I18N_FilterDescription = "description";
        public static string I18N_FilterDescriptionRE = "descriptionRE";
        public static string I18N_FilterBy = "filterBy";
        public static string I18N_ApplyFilter = "applyFilter";
        public static string I18N_SelectFilter = "selectFilter";

        //EmptyDataPreffix
        public static string I18N_EmptyHistory = "emptyHistory";
        public static string I18N_EmptyPaymentHistory = "emptyPaymentHistory";
        public static string I18N_EmptyBillHistory = "emptyBillHistory";
        public static string I18N_EmptyHistoryRE = "emptyHistoryRE";
        public static string I18N_EmptyPaymentHistoryRE = "emptyPaymentHistoryRE";
        public static string I18N_EmptyBillHistoryRE = "emptyBillHistoryRE";

        //Image
        public static string IMG_NeedToPay = "Bills-NeedToPay-Banner";
        public static string IMG_Cleared = "Bills-Cleared-Banner";
        public static string IMG_RE = "Bills-RE-Banner";
        public static string IMG_ArrowDown = "Arrow-Expand-Down";
        public static string IMG_ArrowUp = "Arrow-Collapse-Up";
        public static string IMG_InfoBlue = "IC-Info-Blue";
        public static string IMG_BackIcon = "Back-White";
        public static string IMG_Info = "IC-Info";
        public static string IMG_NoHistoryData = "Bills-No-History-Data";
        public static string IMG_BGToolTip1 = "Itemized-Tooltip1";
        public static string IMG_BGToolTip2 = "Itemized-Tooltip2";
        public static string IMG_ArrowExpand = "Arrow-Expand";
        public static string IMG_BCRMDown = "BCRM-Down-Background";
        public static string IMG_LoadingBanner = "Bill-Loading-Banner";
        public static string IMG_Refresh = "SSMR-Refresh";
        public static string IMG_DropdownIcon = "IC-Action-Dropdown";
        public static string IMG_NavFiltered = "IC-Action-Nav-Filtered";
        public static string IMG_NavUnfiltered = "IC-Action-Nav-Unfiltered";
        public static string IMG_PendingPayment = "Bills-Pending-Payment";

        //Event
        public static string Event_MandatoryChargesPopup = "MandatoryChargesPopup";
        public static string Event_MandatoryDetails = "ExpandCollapseMandatoryList";

        //Preference Key
        public static string Pref_BillTutorialNormalOverlay = "BillTutorialNormalOverlayV2.0.0";
        public static string Pref_BillTutorialRElOverlay = "BillTutorialRElOverlayV2.0.0";
        public static string Pref_BillDetailsTutorialOverlay = "BillDetailsTutorialOverlayV2.0.0";

        //Regex Constants
        public static string REGEX_Dropdown = "#dropdown#";
    }
}