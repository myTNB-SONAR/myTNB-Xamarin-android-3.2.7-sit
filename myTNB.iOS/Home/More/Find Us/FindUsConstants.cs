namespace myTNB.FindUs
{
    public static class FindUsConstants
    {
        public static string Pagename_FindUs = "FindUs";
        public static string Pagename_LocationDetails = "LocationDetails";

        public static string I18N_NavTitle = "title";
        public static string I18N_SearchPlaceholder = "searchPlaceholder";
        public static string I18N_Address = "address";
        public static string I18N_Phone = "phone";
        public static string I18N_OpeningHours = "openingHours";
        public static string I18N_Services = "services";
        public static string I18N_ZeroLocations = "zeroLocations";
        public static string I18N_NoKTFound = "noKTFound";
        public static string I18N_No711Found = "no711Found";
        public static string I18N_Show = "show";
        public static string I18N_MapSelection = "mapSelection";
        public static string I18N_SelectApplication = "selectApplication";
        public static string I18N_OpenIn = "openIn";
        public static string I18N_NoSupportedApp = "noSupportedApp";
        public static string I18N_ServiceDescription = "serviceDescription";
        public static string I18N_OperationHours = "operationHours";

        //Schema
        public static string Schema_Apple = "maps://?saddr=&daddr={0},{1}&directionsmode=driving";
        public static string Schema_Google = "comgooglemaps://?saddr=&daddr={0},{1}&directionsmode=driving";
        public static string Schema_Waze = "waze://?ll={0},{1}&navigate=yes";

        //URL
        public static string URL_Apple = "maps://";
        public static string URL_Google = "comgooglemaps://";
        public static string URL_Waze = "waze://";

        //Services
        public static string Service_GetLocationsByKeyword = "GetLocationsByKeyword";
        public static string Service_GetLocations = "GetLocations";
    }
}