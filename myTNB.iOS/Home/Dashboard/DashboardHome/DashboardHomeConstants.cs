using System;

namespace myTNB
{
    public static class DashboardHomeConstants
    {
        //Card
        public static int MaxAccountPerCard = 5;
        public static nfloat SearchViewHeight = 24f;
        public static nfloat PageControlHeight = 20f;
        public static nfloat GreetingViewHeight = 80f;

        //Cell
        public static string Cell_Accounts = "accountsTableViewCell";
        public static string Cell_Help = "helpTableViewCell";
        public static string Cell_Services = "servicesTableViewCell";

        //I18N Keys
        public static string I18N_Afternoon = "greeting_afternoon";
        public static string I18N_Evening = "greeting_evening";
        public static string I18N_Morning = "greeting_morning";
        public static string I18N_NeedHelp = "needHelp";
        public static string I18N_MyAccounts = "myAccounts";
        public static string I18N_MyServices = "myServices";

        //Image
        public static string Img_Notification = "Notification";

        //PageName
        public static string PageName = "DashboardHome";

        //Cell Index
        public static int CellIndex_Accounts = 0;
        public static int CellIndex_Services = 1;
        public static int CellIndex_Help = 2;

        //SMR CTA
        public static string CTA_ShowReadingHistory = "v";
        public static string CTA_ShowSubmitReading = "s";
    }
}