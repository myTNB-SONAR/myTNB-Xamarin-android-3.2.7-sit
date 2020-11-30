namespace myTNB.Mobile
{
    internal static class Constants
    {
        //Headers
        internal const string Header_RoleID = "RoleId";
        internal const string Header_UserID = "UserId";
        internal const string Header_UserName = "UserName";
        internal const string Header_SecureKey = "SecureKey";
        internal const string Header_UserInfo = "UserInfo";

        //String Constants
        internal const string EMPTY = "empty";
        internal const string DEFAULT = "default";

        //Todo: Set to 60000
        internal const int APITimeOut = 60000;
        internal const int SitecoreTimeOut = 5000;
        internal const string SitecoreDomain = "sitecore";
        internal const string SitecoreUsername = "api_user";
        internal const string SitecorePassword = "mytnbapiuser!3$@2";
        internal const string ApiUrlPath = "v6/mytnbappws.asmx";

#if DEBUG
        private const string DEV1 = "http://10.215.128.191:88";
        private const string DEV2 = "http://10.215.128.191:89";
        private const string SIT = "https://mobiletestingws.tnb.com.my";
        private const string PROD = "https://mytnbapp.tnb.com.my";
        private const string DEVUNIFIED = "http://dev.mytnb.com.my:8322";

        internal static string ApiDomain = SIT;
        internal const string PaymentURL = "http://10.215.128.191:89/v5/PayRedirect.aspx";
        internal const string ApiKeyId = "9515F2FA-C267-42C9-8087-FABA77CB84DF";
        internal const string SitecoreURL = "https://sitecore.tnb.com.my/";// "http://10.215.70.246/";    //"http://tnbcsdevapp.tnb.my/";
#elif MASTER || SIT
        internal static string ApiDomain = "https://mobiletestingws.tnb.com.my";
        internal const string PaymentURL = "https://mobiletestingws.tnb.com.my/v5/PayRedirect.aspx";
        internal const string ApiKeyId = "9515F2FA-C267-42C9-8087-FABA77CB84DF";
        internal const string SitecoreURL = "http://tnbcsstgapp.tnb.my/"; //"http://10.215.70.248/
#else
        internal static string ApiDomain = "https://mytnbapp.tnb.com.my";
        internal const string PaymentURL = "https://mytnbapp.tnb.com.my/v5/PayRedirect.aspx";
        internal const string ApiKeyId = "E6148656-205B-494C-BC95-CC241423E72F";
        internal const string SitecoreURL = "https://sitecore.tnb.com.my/";
#endif

        //Service Name
        internal const string Service_SearchApplicationType = "SearchApplicationType";
        internal const string Service_GetApplicationStatus = "ApplicationStatus";
        internal const string Service_SaveApplication = "SaveApplication";
        internal const string Service_GetAllApplications = "AllApplications";
        internal const string Service_GetApplicationDetail = "ApplicationDetail";
        internal const string Service_RemoveApplication = "RemoveApplication";
        internal const string Service_SearchApplicationByCA = "SearchApplicationByCA";
        internal const string Service_GetTaxInvoice = "GetTaxInvoiceForApplicationPayment";
        internal const string Service_TaxInvoice = "GetTaxInvoiceApplicationPayment";
        internal const string Service_GetCustomerRatingMaster = "CustomerRatingMaster";

        //Language File Constants
        internal const string LanguageFile_ServiceDetails = "ServiceDetails";
        internal const string LanguageFile_Services = "Services";

        //Mapping
        internal const string LanguageFile_Mapping = "Mapping";
        internal const string LanguageFile_ExcludedApplicationTypes = "ExcludedApplicationTypes";

        //HardCoded Values
        internal const string Constants_Currency = "RM";

        //API Key
        internal const string APIKey = "eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJDaGFubmVsIjoibXlUTkJfQVBJX01vYmlsZSIsIkNoYW5uZWxLZXkiOiJGNUFEQjU0QzM1MkM0NzYwQjUzMkNEOUU1ODdBRTRGNiIsIm5iZiI6MTU5OTE5OTc0OSwiZXhwIjoxNTk5MjAzMzQ5LCJpYXQiOjE1OTkxOTk3NDksImlzcyI6Im15VE5CIEFQSSIsImF1ZCI6Im15VE5CIEFQSSBBdWRpZW5jZSJ9.Sy_xahwMgt2izUgztYq_BQeGECGsahP9oSNHeB1kwB0Ij8Grpg3kQZPCa_b_bbiyngzpjKy38_DFU12wToQAiA";
    }
}