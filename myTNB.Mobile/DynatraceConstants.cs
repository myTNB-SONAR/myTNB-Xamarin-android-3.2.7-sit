namespace myTNB.Mobile
{
    public struct DynatraceConstants
    {
        //Existing Tags PRE ACME
        public const string WEBVIEW_PAYMENT = "WebViewPayment";
        public const string WEBVIEW_PAYMENT_FPX = "WebViewPaymentFPX";
        public const string WEBVIEW_PAYMENT_CC = "WebViewPaymentCC";
        public const string WEBVIEW_PAYMENT_SUCCESS = "WebViewPayment_Success";
        public const string WEBVIEW_PAYMENT_FAIL = "WebViewPayment_Fail";
        public const string WEBVIEW_PAYMENT_FINISH_DASHBOARD = "WebViewPaymentFINISH_DASHBOARD";
        public const string SITECORE_REFER_ONLINE = "sitecore_refer_online";
        public const string SITECORE_REFER_LOCAL = "sitecore_refer_local";
        public const string HOMEPOPUP_WHATSNEWCLICKED = "WhatsNewClicked";

        public struct DBR
        {
            public struct Screen
            {
                public struct Bills
                {
                    public const string EBill = "App_Visit_Bills_EBill";
                    public const string Email = "App_Visit_Bills_EBill&Paper&Email";
                    public const string Paper = "App_Visit_Bills_EBill&Paper";
                }

                public struct BillDetails
                {
                    public const string EBill = "App_Visit_BillDetails_EBill";
                    public const string Email = "App_Visit_BillDetails_EBill&Paper&Email";
                    public const string Paper = "App_Visit_BillDetails_EBill&Paper";
                }

                public struct ManageBillDelivery
                {
                    public const string EBill = "App_Visit_ManageBill_Ebill";
                    public const string EmailAndPaper = "App_Visit_ManageBill_EBill&Paper&Email";
                    public const string Paper = "App_Visit_ManageBill_EBill&Paper";
                }
            }

            public struct CTA
            {
                public struct Home
                {
                    public const string Home_Banner = "App_Action_Home_Banner_JomPaperless";
                    public const string Reminder_Popup_Viewmore = "App_Action_FirstLogin_MarketingPopup_ViewMore";
                    public const string Reminder_Popup_GotIt = "App_Action_FirstLogin_MarketingPopup_GotIt";
                }

                public struct Bills
                {
                    public const string Banner_EBill = "App_Action_Bill_EBill_Banner";
                    public const string Banner_EMail = "App_Action_Bill_EBill&Paper&Email_Banner";
                    public const string Banner_Paper = "App_Action_Bills_EBill&Paper_Banner";
                }

                public struct BillDetails
                {
                    public const string Banner_EBill = "App_Action_BillDetail_EBill_Banner";
                    public const string Banner_EMail = "App_Action_BillDetails_EBill&Paper&Email_Banner";
                    public const string Banner_Paper = "App_Action_BillDetails_EBill&Paper_Banner";
                }

                public struct ManageBillDelivery
                {
                    public const string EBill = "App_Action_ManageBill_EBill_UpdateMethod";
                    public const string EmailAndPaper = "App_Action_ManageBill_EBill&Paper&Email_JomPaperless";
                    public const string Paper = "App_Action_ManageBill_EBill&Paper_JomPaperless";
                }

                public struct ManageElectricityAccount
                {
                    public const string Manage = "App_Action_Profile_ManageBillDelivery";
                }
            }
        }
    }
}