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
            public struct Screens
            {
                public struct Bills
                {
                    public const string Pre_EBill_Paper = "App_Visit_Bills_EBill&Paper";
                    public const string Pre_EBill_Email_Paper = "App_Visit_Bills_EBill&Paper&Email";
                    public const string Post_EBill = "App_Visit_Bills_EBill";
                    public const string Post_EBill_Email = "App_Visit_Bills_EBill&Email";
                }

                public struct BillDetails
                {
                    public const string Pre_EBill_Paper = "App_Visit_BillDetails_EBill&Paper";
                    public const string Pre_EBill_Email_Paper = "App_Visit_BillDetails_EBill&Paper&Email";
                    public const string Post_EBill = "App_Visit_BillDetails_EBill";
                    public const string Post_EBill_Email = "App_Visit_BillDetails_EBill&Emaill";
                }

                public struct ManageBillDelivery
                {
                    public const string Pre_EBill_Paper = "App_Visit_ManageBill_EBill&Paper";
                    public const string Pre_EBill_Email_Paper = "App_Visit_ManageBill_EBill&Paper&Email";
                    public const string Post_EBill = "App_Visit_ManageBill_Ebill";
                    public const string Post_EBill_Email = "App_Visit_ManageBill_EBill&Email";
                }

                public struct PaymentSuccess
                {
                    public const string Single = "App_Visit_PaymentSuccess_Single";
                    public const string Multiple = "App_Visit_PaymentSuccess_Multiple";
                }
            }

            public struct CTAs
            {
                public struct Home
                {
                    public const string Home_Banner = "App_Action_Home_Banner_JomPaperless";
                    public const string Reminder_Popup_Viewmore = "App_Action_FirstLogin_MarketingPopup_ViewMore";
                    public const string Reminder_Popup_GotIt = "App_Action_FirstLogin_MarketingPopup_GotIt";
                }

                public struct Bills
                {
                    public const string Pre_EBill_Paper = "App_Action_Bills_EBill&Paper_Banner";
                    public const string Pre_EBill_Email_Paper = "App_Action_Bill_EBill&Paper&Email_Banner";
                    public const string Post_EBill = "App_Action_Bill_EBill_Banner";
                    public const string Post_EBill_Email = "App_Action_Bill_EBill&Email_Banner";
                }

                public struct BillDetails
                {
                    public const string Pre_EBill_Paper = "App_Action_BillDetails_EBill&Paper_Banner";
                    public const string Pre_EBill_Email_Paper = "App_Action_BillDetails_EBill&Paper&Email_Banner";
                    public const string Post_EBill = "App_Action_BillDetail_EBill_Banner";
                    public const string Post_EBill_Email = "App_Action_BillDetail_EBill&Email_Banner";
                }

                public struct ManageBillDelivery
                {
                    public const string Pre_EBill_Paper = "App_Action_ManageBill_EBill&Paper_JomPaperless";
                    public const string Pre_EBill_Email_Paper = "App_Action_ManageBill_EBill&Paper&Email_JomPaperless";
                    public const string Post_EBill = "App_Action_ManageBill_EBill_UpdateMethod";
                    public const string Post_EBill_Email = "App_Action_ManageBill_EBill&Email_UpdateMethod";
                }

                public struct ManageElectricityAccount
                {
                    public const string Manage = "App_Action_Profile_ManageBillDelivery";
                }

                public struct PaymentSuccess
                {
                    public const string Single = "App_Action_PaySuccess_Single_JomPaperless";
                    public const string Multiple = "App_Action_PaySuccess_Multiple_JomPaperless";
                }
            }
        }
    }
}