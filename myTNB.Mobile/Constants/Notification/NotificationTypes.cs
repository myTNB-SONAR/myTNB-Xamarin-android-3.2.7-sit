namespace myTNB.Mobile.Constants
{
    public struct NotificationTypes
    {
        public const string APPLICATIONSTATUS = "APPLICATIONSTATUS";
        public const string APP_UPDATE = "APPUPDATE";

        public struct DBR
        {
            public const string DBR_Owner = "DBROWNER";
            public const string DBR_NonOwner = "DBRNONOWNER";
            public const string ACCOUNT_STATEMENT = "ACCOUNTSTATEMENT";
            public const string NEW_BILL_DESIGN = "NEWBILLDESIGN";
        }

        public struct MyHome
        {
            public const string MYHOME_NC_ADDRESS_SEARCH_COMPLETED = "NCADDDRESSSEARCHCOMPLETED";
            public const string MYHOME_NC_RESUME_APPLICATION = "NCRESUMEAPPLICATION";
            public const string MYHOME_NC_APPLICATION_COMPLETED = "NCAPPLICATIONCOMPLETED";
            public const string MYHOME_NC_APPLICATION_CONTRACTOR_COMPLETED = "NCAPPLICATIONCONTRACTORCOMPLETED";
            public const string MYHOME_NC_OTP_VERIFY = "NCOTPVERIFY";
        }

        public struct DS
        {
            public const string DIGITAL_SIGNATURE = "EKYCVERIFICATION";
            public const string EKYCIDNOTMATCHING = "EKYCIDNOTMATCHING";
            public const string EKYCFAILED = "EKYCFAILED";
            public const string EKYCTHREETIMESFAILURE = "EKYCTHREETIMESFAILURE";
            public const string EKYCSUCCESSFUL = "EKYCVERIFICATIONSUCCESS";
            public const string EKYCFIRSTNOTIFICATION = "EKYCFIRSTNOTIFICATION";
            public const string EKYCSECONDNOTIFICATION = "EKYCSECONDNOTIFICATION";
            public const string EKYCTHIRDPARTYFAILED = "EKYCTHIRDPARTYFAILED";
            public const string EKYCTHIRDPARTYSUCCESSFUL = "EKYCTHIRDPARTYVERIFICATIONSUCCESS";
            public const string EKYCTHIRDPARTYTHREETIMESFAILURE = "EKYCTHIRDPARTYMTHREETIMESFAILURE";
            public const string EKYCTHIRDPARTYIDNOTMATCHING = "EKYCTHIRDPARTYIDNOTMATCHING";
        }
    }
}