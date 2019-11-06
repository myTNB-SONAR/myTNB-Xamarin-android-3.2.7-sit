﻿namespace myTNB
{
    public static class Constants
    {
        //Common Images
        public static string IMG_Back = "Back-White";
        public static string IMG_Dropdown = "IC-Header-Dropdown";
        public static string IMG_InfoBlackIcon = "Info-Black-Icon";
        public static string IMG_SavingTipsBg = "Saving-Tips-Bg";
        public static string IMG_AcctREIcon = "RE-Leaf-Large";
        public static string IMG_RMKwHDropdownIcon = "RmKwh-Dropdown-Icon";
        public static string IMG_TariffEyeOpenIcon = "Tariff-Eye-Open-Icon";
        public static string IMG_TariffEyeCloseIcon = "Tariff-Eye-Close-Icon";
        public static string IMG_TariffEyeDisableIcon = "Tariff-Eye-Disable-Icon";
        public static string IMG_RefreshIcon = "Refresh-Icon";
        public static string IMG_BCRMDownIcon = "BCRM-Down-Icon";
        public static string IMG_CalendarIcon = "Calendar-Icon";
        public static string IMG_PredictIcon = "Predict-Icon";
        public static string IMG_TrendIcon = "Trend-Icon";
        public static string IMG_InfoBlue = "IC-Info-Blue";
        public static string IMG_TrendUpIcon = "Trend-Up-Icon";
        public static string IMG_TrendDownIcon = "Trend-Down-Icon";
        public static string IMG_ArrowLeftBlueIcon = "Arrow-Left-Blue";
        public static string IMG_ShowPassword = "IC-Action-Show-Password";
        public static string IMG_HidePassword = "IC-Action-Hide-Password";

        //Common I18N
        public static string Common_RefreshNow = "refreshNow";
        public static string Common_Yes = "yes";
        public static string Common_No = "no";
        public static string Common_SelectAll = "selectAll";
        public static string Common_UnselectAll = "unselectAll";
        public static string Common_TotalAmount = "totalAmount";
        public static string Common_TotalAmountRM = "totalAmountRM";
        public static string Common_AmountRM = "amountRM";
        public static string Common_Back = "back";
        public static string Common_Cards = "cards";
        public static string Common_Abort = "abort";
        public static string Common_Cancel = "cancel";
        public static string Common_Months = "months";
        public static string Common_Days = "days";
        public static string Common_DisconnectionMsg = "disconnectionMsg";
        public static string Common_RefreshMessage = "refreshDescription";
        public static string Common_RefreshBtnText = "refreshNow";
        public static string Common_BCRMMessage = "bcrmMessage";
        public static string Common_All = "all";
        public static string Common_Warning = "warning";
        public static string Common_Manage = "manage";
        public static string Common_Update = "update";
        public static string Common_Logout = "logout";
        public static string Common_Name = "name";
        public static string Common_IDNumber = "idNumber";
        public static string Common_Password = "password";
        public static string Common_MobileNo = "mobileNo";
        public static string Common_Email = "email";
        public static string Common_AddAnotherAccount = "addAnotherAcct";
        public static string Common_SetAppLanguage = "setAppLanguage";
        public static string Common_SetAppLanguageDescription = "setAppLanguageDescription";
        public static string Common_SaveChanges = "saveChanges";
        public static string Common_English = "english";
        public static string Common_Bahasa = "bahasa";
        public static string Common_GotIt = "gotIt";
        public static string Common_Submit = "submit";
        public static string Common_Login = "login";
        public static string Common_Next = "next";
        public static string Common_Resend = "resend";
        public static string Common_Retry = "retry";
        public static string Common_Ok = "ok";
        public static string Common_Save = "save";
        public static string Common_AccountNickname = "acctNickname";
        public static string Common_Fullname = "fullname";
        public static string Common_ConfirmPassword = "confirmPassword";
        public static string Common_AccountNo = "accountNo";
        public static string Common_AccountType = "accountType";
        public static string Common_AllNotifications = "allNotifications";
        public static string Common_SelectElectricityAccount = "selectElectricityAccount";
        public static string Common_Close = "close";
        public static string Common_CharacterLeft = "characterLeft";
        public static string Common_CharactersLeft = "charactersLeft";
        public static string Common_Comments = "comments";
        public static string Common_AccountHolder = "accountHolder";
        public static string Common_Skip = "skip";
        public static string Common_Details = "details";
        public static string Common_ChangeLanguageTitle = "changeLanguageTitle";
        public static string Common_ChangeLanguageMessage = "changeLanguageMessage";
        public static string Common_ChangeLanguageYes = "changeLanguageYes";
        public static string Common_ChangeLanguageNo = "changeLanguageNo";
        public static string Common_New = "new";

        //Error I18N
        public static string Error_RefreshMessage = "refreshMessage";
        public static string Error_DefaultServiceErrorMessage = "serviceErrorDefaultMessage";
        public static string Error_AmountTitle = "invalid_amountTitle";
        public static string Error_AmountMessage = "invalid_amountMessage";
        public static string Error_MinimumPayAmount = "minimumPayAmount";
        public static string Error_MinimumMandatoryAmount = "minimumMandatoryPayment";
        public static string Error_InvalidEmailAddress = "invalid_email";
        public static string Error_InvalidCode = "invalid_code";
        public static string Error_InvalidPassword = "invalid_password";
        public static string Error_MismatchedPassword = "invalid_mismatchedPassword";
        public static string Error_InvalidMobileNumber = "invalid_mobileNumber";
        public static string Error_InvalidNickname = "invalidNickname";
        public static string Error_DuplicateNickname = "duplicateNickname";
        public static string Error_InvalidFullname = "invalid_fullname";
        public static string Error_InvalidIDNumber = "invalid_idnumber";
        public static string Error_MismatchedEmail = "invalid_mismatchedEmail";
        public static string Error_DuplicateAccountTitle = "error_duplicateAccountTitle";
        public static string Error_DuplicateAccountMessage = "error_duplicateAccountMessage";
        public static string Error_InvalidICNumber = "invalid_icNumber";
        public static string Error_InvalidROCNumber = "invalid_rocNumber";
        public static string Error_AccountLength = "accountLength";
        public static string Error_InvalidAccountType = "invalid_accountType";
        public static string Error_NoDataConnectionTitle = "noDataConnectionTitle";
        public static string Error_NoDataConnectionMessage = "noDataConnectionMessage";
        public static string Error_DefaultErrorTitle = "defaultErrorTitle";
        public static string Error_DefaultErrorMessage = "defaultErrorMessage";
        public static string Error_NumberNotAvailable = "numberNotAvailable";
        public static string Error_RatingNotAvailable = "ratingNotAvailable";
        public static string Error_ShareNotAvailable = "shareNotAvailable";

        //Hint I18N
        public static string Hint_EmptyAcctSelector = "emptyAcctSelector";
        public static string Hint_MobileNumber = "mobileNumberSample";
        public static string Hint_Password = "password";
        public static string Hint_Nickname = "nickname";
        public static string Hint_BusinessNickname = "businessNickname";

        //Table View Cell
        public static string Cell_NoHistoryData = "noHistoryDataTableViewCell";

        //Global Constants
        public static string UnitEnergy
        {
            get
            {
                return LanguageUtility.GetCommonI18NValue("unitEnergy");
            }
        }
        public static string UnitEmission
        {
            get
            {
                return LanguageUtility.GetCommonI18NValue("unitEmission");
            }
        }
    }
}