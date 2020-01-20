using System;

namespace myTNB
{
    public static class WhatsNewConstants
    {
        //Pagename
        public static string Pagename_WhatsNew = "WhatsNew";

        //Tags
        public static int Tag_SelectedCategory = 100;
        public static int Tag_CategoryLabel = 101;
        public static int Tag_ViewContainer = 1000;
        public static int Tag_TableView = 2000;

        //Format
        public static string Format_Date = "dd MMM yyyy";

        //nfloat
        public static nfloat WhatsNewCellHeight = ScaleUtility.GetScaledHeight(177F);

        //Cell
        public static string Cell_WhatsNew = "whatsNewTableViewCell";

        //Event Names
        public static string Event_Refresh = "Refresh";

        //I18N Keys
        public static string I18N_Title = "title";
        public static string I18N_EmptyDesc = "emptyDesc";

        //Img
        public static string Img_WhatsNewDefaultBanner = "WhatsNew_Default-Banner";
        public static string Img_ShareIcon = "IC-Header-Share";
        public static string Img_EmptyIcon = "WhatsNew_Empty";
        public static string Img_Refresh = "SSMR-Refresh";
    }
}