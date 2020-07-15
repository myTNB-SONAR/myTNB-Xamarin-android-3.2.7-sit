using Foundation;

namespace myTNB.SSMR
{
    public static class SSMRConstants
    {
        //Page name
        public static string Pagename_SSMRApplication = "SSMRApplication";
        public static string Pagename_SSMRWalkthrough = "SSMROnboarding";
        public static string Pagename_SSMRCaptureMeter = "SSMRCaptureMeter";
        public static string Pagename_SSMRMeterRead = "SSMRSubmitMeterReading";
        public static string Pagename_SSMRReadingHistory = "SSMRReadingHistory";
        public static string Popup_SMRPhotoPopUpDetails = "SMRPhotoPopUpDetails";
        public static string Popup_SMREligibiltyPopUpDetails = "SMREligibiltyPopUpDetails";

        //Service name
        public static string Service_SubmitSSMRApplication = "SubmitSMRApplication";
        public static string Service_GetTerminationReasons = "GetSMRTerminationReasons";
        public static string Service_GetCARegisteredContact = "GetCAContactDetails";
        public static string Service_GetMeterReadingOCRValue = "GetMeterReadingOCRValue";
        public static string Service_GetAccountsSMREligibility = "GetAccountsSMREligibility";
        public static string Service_GetSMRAccountActivityInfo = "GetSMRAccountActivityInfo";
        public static string Service_Register = "R";
        public static string Service_Terminate = "T";
        public static string Service_OthersID = "1007";

        //Tooltips
        public static string Tooltips_MultiPhaseGallery = "Multi_UploadPhoto";
        public static string Tooltips_MultiPhaseTakePhoto = "Multi_TakePhoto";
        public static string Tooltips_SinglePhaseGallery = "Single_UploadPhoto";
        public static string Tooltips_SinglePhaseTakePhoto = "Single_TakePhoto";
        public static string Tooltips_MultiPhaseOneMissingGallery = "Multi_UploadPhoto_One_Missing";
        public static string Tooltips_MultiPhaseOneMissingTakePhoto = "Multi_TakePhoto_One_Missing";

        //Onboarding
        //i18n
        public static string I18N_Skip = "skip";
        public static string I18N_Title1 = "onboardingTitle1";
        public static string I18N_Title2 = "onboardingTitle2";
        public static string I18N_Description1 = "onboardingDescription1";
        public static string I18N_Description2 = "onboardingDescription2";
        public static string I18N_StartApplication = "startApplication";
        public static string I18N_NoHistoryData = "noHistoryData";

        //Common
        public static string I18N_NavTitle = "title";
        public static string I18N_Submit = "submit";
        public static string I18N_Account = "account";
        public static string I18N_MobileNumber = "mobileNumber";
        public static string I18N_Email = "emailAddress";
        public static string I18N_SelectAccounts = "selectAccounts";
        public static string I18N_RefreshNow = "refreshNow";
        public static string I18N_RefreshDescription = "refreshDescription";

        //Application
        public static string I18N_ApplyingFor = "applyingFor";
        public static string I18N_TerminateFor = "terminateFor";
        public static string I18N_ContactDetails = "contactDetails";
        public static string I18N_TerminateTitle = "terminateTitle";
        public static string I18N_TnCSubscribe = "tncSubscribe";
        public static string I18N_TnCUnsubscribe = "tncUnsubscribe";
        public static string I18N_EditInfo = "editContactInfo";
        public static string I18N_SelectReason = "selectReason";
        public static string I18N_StateReason = "stateReason";
        public static string I18N_SelectAnAccount = "selectAnAccount";
        public static string I18N_NoEligibleAccount = "noEligibleAccount";

        //Image
        public static string IMG_Mark = "Payment-Checkbox-Active";
        public static string IMG_Unmark = "Payment-Checkbox-Inactive";
        public static string IMG_Dropdow = "IC-Action-Dropdown";
        public static string IMG_MobileNumber = "Mobile";
        public static string IMG_Email = "Email";
        public static string IMG_SMRMediumIcon = "SMR-Medium-Icon";
        public static string IMG_BackIcon = "Back-White";
        public static string IMG_Info = "IC-Info";
        public static string IMG_CameraThumb = "Camera-Thumb";
        public static string IMG_MultiPhase = "SSMR-MultiPhase";
        public static string IMG_SinglePhase = "SSMR-SinglePhase";
        public static string IMG_BGToolTip1 = "SMR-ToolTip-BG1";
        public static string IMG_BGToolTip2 = "SMR-ToolTip-BG2";
        public static string IMG_BGToolTip3 = "SMR-ToolTip-BG3";
        public static string IMG_NoData = "SSMR-NoData";
        public static string IMG_ReadingHistoryBanner = "SSMR-Reading-History-Banner";
        public static string IMG_Refresh = "SSMR-Refresh";
        public static string IMG_NoHistory = "SSMR-No-History";
        public static string IMG_CameraIcon = "Camera-Icon-Green";
        public static string IMG_Meter = "Sample-Meter";

        //Error
        public static string I18N_InvalidEmail = "invalid_email";
        public static string I18N_InvalidMobileNumber = "invalid_mobileNumber";

        //Hint
        public static string I18N_HintMobileNumber = "mobileNumberSample";

        //Cell
        public static string Cell_ReadingHistory = "readingHistoryTableViewCell";

        //Reading History
        public static string I18N_SectionTitle = "headerTitle";
        public static string I18N_SubTitle = "subTitle";
        public static string I18N_DisableSSMRCTA = "disableSSMRCTA";
        public static string I18N_EnableSSMRCTA = "enableSSMRCTA";
        public static string I18N_EnableSSMRDescription = "enableSSMRDescription";
        public static string I18N_SelectAccountNavTitle = "selectAccountNavTitle";
        public static string I18N_TutorialHeaderTitle = "tutorialHeaderTitle";
        public static string I18N_TutorialHeaderDesc = "tutorialHeaderDesc";
        public static string I18N_TutorialGotIt = "tutorialGotIt";
        public static string I18N_PrevReadingEmptyTitle = "prevReadingEmptyTitle";
        public static string I18N_PrevReadingEmptyMsg = "prevReadingEmptyMsg";
        public static string I18N_WhereIsMyMeter = "whereIsMyMeter";
        public static string I18N_WhereIsMyMeterTitle = "whereIsMyMeterTitle";
        public static string I18N_WhereIsMyMeterMessage = "whereIsMyMeterMessage";
        public static string I18N_DoYouHaveMeterAccess = "doYouHaveMeterAccess";

        //Capture Meter
        public static string I18N_OCRReading = "ocrReading";
        public static string I18N_NavTitleTakePhoto = "navTitleTakePhoto";
        public static string I18N_SingleTakePhotoDescription = "singleTakePhotoDescription";
        public static string I18N_EditDescription = "editDescription";
        public static string I18N_MultiTakePhotoDescription = "multiTakePhotoDescription";
        public static string I18N_MultiTakeNextPhotoDescription = "multiTakeNextPhotoDescription";
        public static string I18N_SingularDone = "singularDone";
        public static string I18N_PluralDone = "pluralDone";
        public static string I18N_SingularOnto = "singularOnto";
        public static string I18N_PluralOnto = "pluralOnto";
        public static string I18N_DeletePhoto = "deletePhoto";

        //Meter Read
        public static string I18N_HeaderDesc = "headerDescription";
        public static string I18N_SingleHeaderDesc = "singleHeaderDescription";
        public static string I18N_DontShowAgain = "dontShow";
        public static string I18N_ToolTip1 = "tooltipTitle1";
        public static string I18N_ToolTip2 = "tooltipTitle2";
        public static string I18N_ToolTip3 = "tooltipTitle3";
        public static string I18N_ToolTipDesc1 = "tooltipDescription1";
        public static string I18N_ToolTipDesc2 = "tooltipDescription2";
        public static string I18N_ToolTipDesc3 = "tooltipDescription3";
        public static string I18N_ToolTipDesc4 = "tooltipDescription4";
        public static string I18N_ToolTipDesc5 = "tooltipDescription5";
        public static string I18N_ImReady = "imReady";
        public static string I18N_Note = "note";
        public static string I18N_PreviousReading = "previousMeterReading";
        public static string I18N_SubmitReading = "submitReading";
        public static string I18N_TakeOrUploadPhoto = "takeOrUploadPhoto";
        public static string I18N_ManualInputTitle = "manualInputTitle";
        public static string I18N_OCRDownMessage = "ocrDownMessage";
        public static string I18N_TutorialReadMeterTitle = "tutorialReadMeterTitle";
        public static string I18N_TutorialReadMeterDesc = "tutorialReadMeterDesc";
        public static string I18N_TutorialReadMeterGotIt = "tutorialReadMeterGotIt";

        //Patterns
        public static string Pattern_ImageName = "MYTNBAPP_SSMR_OCR_{0}_{1}";

        //More IDs
        public static string More_Unsubscribe = "1004";

        //Notification IDs
        public static NSString Notification_SelectSSMRAccount = (NSString)"SelectSSMRAccount";

        //Event Names
        public static string EVENT_SubmitToOCR = "SubmitImagesToOCR";
        public static string EVENT_DeleteImage = "DeleteImage";
        public static string EVENT_DisableSelfMeterReading = "DisableSelfMeterReading";
        public static string EVENT_Refresh = "Refresh";

        public static int Max_ReasonCharacterCount = 550;

        //Preference Key
        public static string Pref_SSMRHistoryTutorialOverlay = "SSMRHistoryTutorialOverlayV2.0.0";
        public static string Pref_SSMRReadTutorialOverlay = "SSMRReadTutorialOverlayV2.0.0";
    }
}