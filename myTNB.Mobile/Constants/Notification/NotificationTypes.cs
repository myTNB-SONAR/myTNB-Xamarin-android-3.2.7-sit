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
    }
}