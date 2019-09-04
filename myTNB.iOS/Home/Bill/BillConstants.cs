using System;
namespace myTNB.Home.Bill
{
    public static class BillConstants
    {
        //Pagename
        public static string Pagename_Bills = "Bills";
        public static string Pagename_BillDetails = "BillDetails";
        public static string Pagename_BillFilter = "BillFilter";

        //Service
        public static string Service_GetAccountsCharges = "GetAccountsCharges";
        public static string Service_GetAccountBillPayHistory = "GetAccountBillPayHistory";
        public static string Param_RE = "RE";
        public static string Param_UTIL = "UTIL";

        //Format
        public static string Format_DateParse = "yyyyMMdd";
        public static string Format_Date = "dd MMM yyyy";
        public static string Format_Default = "{0} {1}";

        //ViewCell
        public static string Cell_BillHistory = "billHistoryTableViewCell";
        public static string Cell_BillSection = "billSectionTableViewCell";
        public static string Cell_NoHistoryData = "noHistoryDataTableViewCell";
        public static string Cell_BillHistoryShimmer = "billHistoryShimmerViewCell";

        //I18N
        //Bill
        public static string I18N_NavTitle = "navTitle";
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

        //Bill Filter
        public static string I18N_FilterDescription = "description";
        public static string I18N_FilterBy = "filterBy";
        public static string I18N_ApplyFilter = "applyFilter";
        public static string I18N_SelectFilter = "selectFilter";

        //Image
        public static string IMG_NeedToPay = "Bills-NeedToPay-Banner";
        public static string IMG_Cleared = "Bills-Cleared-Banner";
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

        //Event
        public static string Event_MandatoryChargesPopup = "MandatoryChargesPopup";
        public static string Event_MandatoryDetails = "ExpandCollapseMandatoryList";
    }
}