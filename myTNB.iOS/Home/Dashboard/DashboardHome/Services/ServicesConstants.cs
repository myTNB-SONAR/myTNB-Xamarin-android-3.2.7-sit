using System;
using System.Collections.Generic;

namespace myTNB.Home.Dashboard.DashboardHome.Services
{
    public static class ServicesConstants
    {
        //ID-Image Value
        public static Dictionary<string, string> ImageDictionary = new Dictionary<string, string>()
        {
            { "1001","Services-ApplySSMR"}
            , { "1002","Services-CheckStatus"}
            , { "1003","Services-Feedback"}
            , { "1004","Services-Pay-Bills"}
            , { "1005","Services-View-Bills"}
        };

        //ID
        public static string ID_Feedback = "1001";
    }
}