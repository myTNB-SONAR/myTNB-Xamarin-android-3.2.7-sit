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

        public const string App_Launch_Master_Fail = "App_Launch_Master_Data_Fail";

        public struct DBR
        {
            public struct Screens
            {
                public struct Bills
                {
                    public const string EBill_Paper = "App_Visit_Bills_EBill&Paper";
                    public const string EBill_Email_Paper = "App_Visit_Bills_EBill&Paper&Email";
                    public const string EBill = "App_Visit_Bills_EBill";
                    public const string EBill_Email = "App_Visit_Bills_EBill&Email";
                }

                public struct BillDetails
                {
                    public const string EBill_Paper = "App_Visit_BillDetails_EBill&Paper";
                    public const string EBill_Email_Paper = "App_Visit_BillDetails_EBill&Paper&Email";
                    public const string EBill = "App_Visit_BillDetails_EBill";
                    public const string EBill_Email = "App_Visit_BillDetails_EBill&Emaill";
                }

                public struct ManageBillDelivery
                {
                    public const string EBill_Paper = "App_Visit_ManageBill_EBill&Paper";
                    public const string EBill_Email_Paper = "App_Visit_ManageBill_EBill&Paper&Email";
                    public const string EBill = "App_Visit_ManageBill_Ebill";
                    public const string EBill_Email = "App_Visit_ManageBill_EBill&Email";
                }

                public struct PaymentSuccess
                {
                    public const string Single = "App_Visit_PaymentSuccess_Single";
                    public const string Multiple = "App_Visit_PaymentSuccess_Multiple";
                }

                public struct Webview
                {
                    public const string Start_Paperless = "App_Visit_StartPaperless";
                    public const string Start_Paperless_Success = "App_Visit_StartPaperlessSuccess";
                    public const string Back_To_Paper = "App_Visit_OptBackPaperBill";
                    public const string Back_To_Paper_Success = "App_Visit_OptBackPaperSuccess";
                    public const string Start_Paperless_Share_Feedback = "App_Visit_DBRRating";
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
                    public const string EBill_Paper = "App_Action_Bills_EBill&Paper_Banner";
                    public const string EBill_Email_Paper = "App_Action_Bill_EBill&Paper&Email_Banner";
                    public const string EBill = "App_Action_Bill_EBill_Banner";
                    public const string EBill_Email = "App_Action_Bill_EBill&Email_Banner";
                }

                public struct BillDetails
                {
                    public const string EBill_Paper = "App_Action_BillDetails_EBill&Paper_Banner";
                    public const string EBill_Email_Paper = "App_Action_BillDetails_EBill&Paper&Email_Banner";
                    public const string EBill = "App_Action_BillDetails_EBill_Banner";
                    public const string EBill_Email = "App_Action_BillDetails_EBill&Email_Banner";
                }

                public struct ManageBillDelivery
                {
                    public const string EBill_Paper = "App_Action_ManageBill_EBill&Paper_JomPaperless";
                    public const string EBill_Email_Paper = "App_Action_ManageBill_EBill&Paper&Email_JomPaperless";
                    public const string EBill = "App_Action_ManageBill_EBill_UpdateMethod";
                    public const string EBill_Email = "App_Action_ManageBill_EBill&Email_UpdateMethod";
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

                public struct Webview
                {
                    public const string Start_Paperless_Close = "App_Action_StartPaperlessClose_Confirm";
                    public const string Back_To_Paper_Close = "App_Action_OptBackPaperClose_Confirm";
                    public const string Start_Paperless_Confirm = "App_Action_StartPaperless_Confirm";
                    public const string Back_To_Paper_Confirm = "App_Action_OptBackPaper_Confirm";
                    public const string Start_Paperless_Share_Feedback = "App_Action_StartPaperlessSuccess_ShareFeedback";
                    public const string Submit_Rating = "App_Action_DBRRating_Submit";
                    public const string Submit_Rating_Close = "App_Action_DBRRatingClose_Confirm";
                }
            }
        }

        public struct BR
        {
            public struct Screens
            {
                public struct Bill
                {
                    public const string View_Bill = "App_Visit_ViewBill";
                }

                public struct LoadingAccountStatement
                {
                    public const string Loading = "App_Visit_LoadAccountStatement";
                }

                public struct AccountStatement
                {
                    public const string View_Account_Statement = "App_Visit_ViewAccountStatement";
                }
            }

            public struct CTAs
            {
                public struct Home
                {
                    public const string Home_Banner = "App_Action_Home_BRBanner";
                }

                public struct BillRedesignComms
                {
                    public const string View_Bill = "App_Action_NewBill_ViewYourBills";
                }

                public struct Bill
                {
                    public const string View_Bill = "App_Action_ViewBill_Arrow";
                    public const string View_Account_Statement = "App_Action_Bill_AccountStatementIcon";
                    public const string No_Access_Got_It = "App_Action_PaymentHistory_GotIt";
                }

                public struct StatementPeriod
                {
                    public const string Past_3_Months = "App_Action_ViewAccountStatement_Past3Months";
                    public const string Past_6_Months = "App_Action_ViewAccountStatement_Past6Months";
                    public const string Confirm = "App_Action_RequestAccountStatement_Confirm";
                    public const string Back = "App_Action_RequestAccountStatement_BackArrow";
                }

                public struct BillFilter
                {
                    public const string All = "App_Action_BRViewFilter_All";
                    public const string Advice = "App_Action_BRViewFilter_Advice";
                    public const string Bills = "App_Action_BRViewFilter_Bills";
                    public const string Payments = "App_Action_BRViewFilter_Payments";
                }

                public struct Notifications
                {
                    public const string Update = "App_Action_InAppPreGTM_UpdateNow";
                    public const string Combined_Comms_Owner = "Push_Action_DiscoveryOwner_Banner";
                    public const string Combined_Comms_Non_Owner = "Push_Action_CombinedDiscoveryNonOwner_Banner";
                    public const string Combined_Comms_In_App_Non_Owner = "App_Action_InAppCombinedNonOwner_Here";
                }

                public struct Error
                {
                    public const string Timeout_Back_To_Bills = "App_Action_TimeoutAccountStatement_BackToBills";
                    public const string Refresh_Now = "App_Action_ErrorAccountStatement_RefreshNow";
                    public const string No_History_Back_To_Bills = "App_Action_ErrorAccountStatement_BackToBills";
                    public const string Refresh_Back_To_Bills = "App_Action_ErrorAccountStatement_BackArrow";
                }
            }
        }
    }
}