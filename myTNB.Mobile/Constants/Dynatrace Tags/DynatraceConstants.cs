namespace myTNB.Mobile
{
    public struct DynatraceConstants
    {
        //Existing Tags PRE ACME
        public const string WEBVIEW_PAYMENT = "WebViewPayment";
        public const string WEBVIEW_PAYMENT_FPX = "WebViewPaymentFPX";
        public const string WEBVIEW_PAYMENT_CC = "WebViewPaymentCC";
        public const string WEBVIEW_PAYMENT_TNG = "WebViewPaymentTNG";
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
                    public const string Fail = "App_Visit_Fail";
                }
            }

            public struct CTAs
            {
                public struct Home
                {
                    public const string Home_Banner = "App_Action_Home_Banner_JomPaperless";
                    public const string Reminder_Popup_Viewmore = "App_Action_MarketingPopup_HomePage_ViewMore";
                    public const string Reminder_Popup_GotIt = "App_Action_MarketingPopup_HomePage_GotIt";
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

                public struct Usage
                {
                    public const string Reminder_Popup_Viewmore = "App_Action_MarketingPopup_GraphUsage_ViewMore";
                    public const string Reminder_Popup_GotIt = "App_Action_MarketingPopup_GraphUsage_GotIt";
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

        public struct Enquiry
        {
            public struct Screens { }
            public struct CTAs
            {
                public struct Submit
                {
                    public const string Bill_Calculation = "App_Action_AboutMyBillEnquiry_SubmitBillCalculation";
                    public const string Delivery_Method = "App_Action_AboutMyBillEnquiry_SubmitBillDeliveryMethod";
                    public const string Payment_History = "App_Action_AboutMyBillEnquiry_SubmitPaymentHistory";
                    public const string Others = "App_Action_AboutMyBillEnquiry_SubmitOthers";
                }
            }
        }

        public struct AppUpdate
        {
            public struct Recommend
            {
                public const string RecommendAppUpdate_DisplayPopUp = "App_Action_Display_RecommendAppUpdatePopup";
                public const string RecommendAppUpdate_ClickYes = "App_Action_Click_RecommendAppUpdateYes";
                public const string RecommendAppUpdate_ClickNo = "App_Action_Click_RecommendAppUpdateNo";
                public const string RecommendAppUpdate_VersionBeforeUpdate = "App_Action_Get_VersionBeforeUpdate";
            }

        }

        public struct MyHome
        {
            public struct Screens
            {
                public struct OnBoarding
                {
                    public const string Enhance = "myHome_App_Visit_OnboardingComms_myHomeGTM1_Enhance";
                    public const string Connect = "myHome_App_Visit_OnboardingComms_myHomeGTM1_Connect";
                    public const string Manage = "myHome_App_Visit_OnboardingComms_myHomeGTM1_Manage";
                }
                public struct Tutorial
                {
                    public const string Dashboard_QuickLinks_MyHome = "myHome_App_Visit_OverlayTutorial_myHomeGTM1_IntroducingMyHome";
                    public const string Drawer_Start_Your_Application = "myHome_App_Visit_OverlayTutorial_myHomeGTM1_StartYourApplication";
                }
                public struct Home
                {
                    public const string Dashboard = "myHome_App_Visit_AppHomepage";
                    public const string Drawer = "myHome_App_Visit_Drawer_myHome";
                    public const string Resume_Reminder = "myHome_App_Visit_ApplicationReminder";
                }
            }
            public struct CTAs
            {
                public struct OnBoarding
                {
                    public const string Enhance_Skip = "myHome_App_Action_myHomeGTM1_Enhance_Skip";
                    public const string Connect_Skip = "myHome_App_Action_myHomeGTM1_Connect_Skip";
                    public const string Manage_Skip = "myHome_App_Action_myHomeGTM1_Manage_Skip";
                }
                public struct Tutorial
                {
                    public const string Dashboard_QuickLinks_MyHome_Skip = "myHome_App_Action_myHomeGTM1_IntroducingMyHome_Skip";
                    public const string Drawer_Start_Your_Application_Skip = "myHome_App_Action_myHomeGTM1_StartYourApplication_Skip";
                }
                public struct Home
                {
                    public const string Drawer_Open = "myHome_App_Action_Home_myHome_Drawer_Open";
                    public const string Drawer_Connect_My_Premise = "myHome_App_Action_Drawer_myHome_ConnectMyPremise";
                    public const string Drawer_Cancel = "myHome_App_Action_Drawer_myHome_Cancel";
                    public const string Drawer_Dismiss = "myHome_App_Action_Drawer_myHome_Dismiss";
                    public const string Drawer_Checklist = "myHome_App_Action_Drawer_myHome_myHomeChecklist";
                    public const string Resume_Reminder_Continue = "myHome_App_Action_ApplicationReminder_Continue";
                    public const string Resume_Reminder_IllDoItLater = "myHome_App_Action_ApplicationReminder_IllDoItLater";
                }
            }
        }

        public struct PushNotification
        {
            public struct Screens
            {
                public struct Landing
                {
                    public const string Visit = "App_Visit_AllNotification";
                }
                public struct Details
                {
                    public const string Visit = "App_Visit_Notification";
                    public const string NC_Contractor_Completed = "myHome_App_Visit_ContractorCompletedNotificationScreen";
                    public const string NC_Non_Contractor_Completed = "myHome_App_Visit_NonContractorCompletedNotificationScreen";
                    public const string EB_Reaching = "EB_view_notification_duration_reaching";
                    public const string EB_Reached = "EB_view_notification_duration_reached";
                    public const string NC_OTP_Verify = "myHome_App_Visit_VerifyAgainNotification";

                    public const string NC_Reappoint_Contractor = "myHome_App_Visit_ReappointContractorNotificationScreen";
                    public const string COT_OTP_Verify = "myHome_App_Visit_VerifyAgainNotification_COT";
                    public const string COT_Submitted = "myHome_App_Visit_ApplicationSubmittedAppNotification_ExistingOwner_COT";
                    public const string COT_Request = "myHome_App_Visit_ExistingOwner_COTRequest_WithTNB_AppNotification";
                }
            }

            public struct CTAs
            {
                public struct Landing
                {
                    public const string Back = "App_Action_AllNotification_Back";
                    public const string View_Notification_Detail = "App_Action_AllNotification_Detail";
                }
                public struct Details
                {
                    public const string Back = "App_Action_Notification_Back";
                    public const string NC_Contractor_Completed_Back = "myHome_App_Action_ContractorCompletedNotificationScreen_Back";
                    public const string NC_Non_Contractor_Completed_Back = "myHome_App_Action_NonContractorCompletedNotificationScreen_Back";
                    public const string NC_Contractor_Completed_View_Application_Details = "myHome_App_Action_ContractorCompletedNotificationScreen_ViewApplicationDetails";
                    public const string NC_Non_Contractor_Completed_View_Application_Details = "myHome_App_Action_NonContractorCompletedNotificationScreen_ViewApplicationDetails";
                    public const string NC_Submit_Now = "myHome_App_Action_Notification_SubmitNow";
                    public const string NC_Reappoint_Contractor_Reapply_Now = "myHome_App_Action_ReappointContractorNotificationScreen_ReapplyNow";
                    public const string NC_Reappoint_Contractor_Back = "myHome_App_Action_ReappointContractorNotificationScreen_Back";
                    public const string NC_OTP_Verify_Now = "myHome_App_Action_VerifyAgainNotification_VerifyNow";
                    public const string COT_OTP_Verify_Now = "myHome_App_Action_VerifyAgainNotification_COT_VerifyNow";
                    public const string COT_OTP_Verify_Back = "myHome_App_Action_VerifyAgainNotification_COT_Back";
                    public const string COT_Submitted_View_Application_Details = "myHome_App_Action_ApplicationSubmittedAppNotification_ExistingOwner_COT_ViewApplicationDetails";
                    public const string COT_Request_Submit_Now = "myHome_App_Action_ExistingOwner_COTRequest_WithTNB_AppNotification_SubmitNow";
                    public const string COT_Request_Back = "myHome_App_Action_ExistingOwner_COTRequest_WithTNB_AppNotification_Back";
                }
            }

            public struct Banner
            {
                public const string myHome_Reminder = "App_Visit_PushNotificationReminder";
            }
        }

        public struct ApplicationStatus
        {
            public struct Screens
            {
                public struct Details
                {
                    public const string NC_Start_Electricity = "myHome_App_Visit_StartYourElectricity_ApplicationDetails";
                    public const string NC_Delete_Draft_PopUp = "myHome_App_Visit_ApplicationDetails_Cancel";
                    public const string NC_Reappoint_Contractor = "myHome_App_Visit_ReappointContractor_ReapplyApplication";
                    public const string NC_Rate_Our_Service = "myHome_App_Visit_Application_Details_RateOurService";
                    public const string NC_Contractor_Rating = "myHome_App_Visit_Application_DetailsCompletedWC_RateOurService";
                    public const string NC_Customer_Rating = "myHome_App_Visit_Application_DetailsCompletedWthC_RateOurService";
                }
            }
            public struct CTAs
            {
                public struct Landing
                {
                    public const string Back = "App_Action_ApplicationStatus_Back";
                    public const string Search_Applications = "App_Action_ApplicationStatus_SearchApplicationStatus";
                    public const string Filter_Applications = "App_Action_ApplicationStatus_Filter";
                }

                public struct Details
                {
                    public const string Back = "App_Action_ApplicationDetails_Back";
                    public const string NC_Start_Application = "myHome_App_Action_ApplicationDetails_StartApplication";
                    public const string Activity_Log = "App_Action_ApplicationDetails_ViewActivityLog";
                    public const string NC_Delete = "myHome_App_Action_ApplicationDetails_Delete";
                    public const string NC_Resume = "myHome_App_Action_ApplicationDetails_Resume";
                    public const string NC_Delete_Draft_Im_Sure = "myHome_App_Action_ApplicationDetails_Cancel_YesImSure";
                    public const string NC_Delete_Draft_Cancel = "myHome_App_Action_ApplicationDetails_Cancel_Cancel";
                    public const string NC_Reappoint_Contractor_Back = "myHome_App_Action_ReappointContractor_Application_Details_Back";
                    public const string NC_Reappoint_Contractor_Reapply_Now = "myHome_App_Action_ReappointContractor_Application_Details_ReapplyNow";
                    public const string NC_Rate_Our_Servie_Back = "myHome_App_Action_Application_Details_RateOurService_Back";
                    public const string NC_Rate_Our_Service = "myHome_App_Action_Application_Details_RateOurService_RateOurService";
                    public const string NC_Contractor_Rating_Back = "myHome_App_Action_Application_DetailsCompletedWC_RateOurService_Back";
                    public const string NC_Customer_Rating_Back = "myHome_App_Action_Application_DetailsCompletedWthC_RateOurService_Back";
                    public const string NC_Contractor_Rating = "myHome_App_Action_Application_DetailsCompletedWC_RateOurService_RateOurService";
                    public const string NC_Customer_Rating = "myHome_App_Action_Application_DetailsCompletedWthC_RateOurService_RateOurService";
                }
            }
        }

        public struct FloatingIcon
        {
            public struct FloatingModule
            {
                public const string WEB = "Floating_Icon_WEB";
                public const string DBR = "Floating_Icon_DBR";
                public const string BR = "Floating_Icon_BR";
                public const string EB = "Floating_Icon_EB";
                public const string SD = "Floating_Icon_SD";
                public const string TNG = "Floating_Icon_TNG";
            }
        }
    }
}