namespace myTNB.SSMR
{
    public static class SSMRConstants
    {
        //Page name
        public static string Pagename_SSMRApplication = "SSMRApplication";
        public static string Pagename_SSMRWalkthrough = "SSMROnboarding";
        public static string Pagename_SSMRCaptureMeter = "SSMRCaptureMeter";

        //Service name
        public static string Service_SubmitSSMRApplication = "SubmitSMRApplication";
        public static string Service_GetTerminationReasons = "GetSMRTerminationReasons";
        public static string Service_GetCARegisteredContact = "GetCAContactDetails";
        public static string Service_GetMeterReadingOCRValue = "GetMeterReadingOCRValue";
        public static string Service_Register = "R";
        public static string Service_Terminate = "T";
        public static string Service_OthersID = "1007";

        //Onboarding
        //i18n
        public static string I18N_Skip = "skip";
        public static string I18N_DontShow = "dontShow";
        public static string I18N_Title1 = "onboardingTitle1";
        public static string I18N_Title2 = "onboardingTitle2";
        public static string I18N_Title3 = "onboardingTitle3";
        public static string I18N_Description1 = "onboardingDescription1";
        public static string I18N_Description2 = "onboardingDescription2";
        public static string I18N_Description3 = "onboardingDescription3";
        public static string I18N_StartApplication = "startApplication";

        //Common
        public static string I18N_NavTitle = "navTitle";
        public static string I18N_Submit = "submit";
        public static string I18N_Account = "account";
        public static string I18N_MobileNumber = "mobileNumber";
        public static string I18N_Email = "emailAddress";
        public static string I18N_SelectAccounts = "selectAccounts";

        //Application
        public static string I18N_NavTitleApply = "navTitleApply";
        public static string I18N_NavTitleTerminate = "navTitleTerminate";
        public static string I18N_ApplyingFor = "applyingFor";
        public static string I18N_TerminateFor = "terminateFor";
        public static string I18N_ContactDetails = "contactDetails";
        public static string I18N_TerminateTitle = "terminateTitle";
        public static string I18N_TnC = "tnc";
        public static string I18N_EditInfo = "editContactInfo";
        public static string I18N_SelectReason = "selectReason";
        public static string I18N_StateReason = "stateReason";

        //Image
        public static string IMG_BGOnboarding1 = "SSMR_Background_1";
        public static string IMG_BGOnboarding2 = "SSMR_Background_2";
        public static string IMG_BGOnboarding3 = "SSMR_Background_3";
        public static string IMG_Mark = "Payment-Checkbox-Active";
        public static string IMG_Unmark = "Payment-Checkbox-Inactive";
        public static string IMG_Dropdow = "IC-Action-Dropdown";
        public static string IMG_MobileNumber = "Mobile";
        public static string IMG_Email = "Email";
        public static string IMG_SMRMediumIcon = "SMR-Medium-Icon";
        public static string IMG_SMROpenIcon = "SMR-Open-Icon";
        public static string IMG_BackIcon = "Back-White";
        public static string IMG_PrimaryIcon = "SSMRPrimaryIcon";
        public static string IMG_Info = "IC-Info";
        public static string IMG_OCRReading = "OCR-Reading";
        public static string IMG_CameraThumb = "Camera-Thumb";
        public static string IMG_Delete = "Notification-Delete";

        //Error
        public static string I18N_InvalidEmail = "invalid_email";
        public static string I18N_InvalidMobileNumber = "invalid_mobileNumber";

        //Hint
        public static string I18N_HintMobileNumber = "mobileNumberSample";

        //Cell
        public static string Cell_ReadingHistory = "readingHistoryTableViewCell";

        //Reading History
        public static string I18N_SectionTitle = "headerTitle";
        public static string STR_EstimatedReading = "estimated";

        //Capture Meter
        public static string I18N_OCRReading = "ocrReading";
        public static string I18N_NavTitleTakePhoto = "navTitleTakePhoto";
        public static string I18N_SingleTakePhotoDescription = "singleTakePhotoDescription";
        public static string I18N_EditDescription = "editDescription";
        public static string I18N_MultiTakePhotoDescription = "multiTakePhotoDescription";

        //Patterns
        public static string Pattern_ImageName = "MYTNBAPP_SSMR_OCR_{0}_{1}";
    }
}