using System.Collections.Generic;
using Android.Graphics;


namespace myTNB.SitecoreCMS.Model
{
 
        public class WhoIsRegisteredOwnerTimeStampResponseModel
    {
            public string Status { set; get; }
            public List<WhoIsRegisteredOwnerTimeStamp> Data { set; get; }
        }

        public class WhoIsRegisteredOwnerResponseModel
    {
            public string Status { set; get; }
            public List<WhoIsRegisteredOwnerModel> Data { set; get; }
        }

        public class WhoIsRegisteredOwnerModel
        {

            public string PopUpTitle { set; get; }
            public string PopUpBody { set; get; }
            public string ID { get; set; }
        }

        public class WhoIsRegisteredOwnerTimeStamp
    {
            public string Timestamp { set; get; }
            public string ID { set; get; }
        }
    
}