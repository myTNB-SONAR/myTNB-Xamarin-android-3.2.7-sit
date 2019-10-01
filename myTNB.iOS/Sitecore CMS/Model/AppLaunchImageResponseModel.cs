using System;
using System.Collections.Generic;

namespace myTNB.SitecoreCMS.Model
{
    public class AppLaunchImageResponseModel
    {
        public string Status { set; get; }
        public List<AppLaunchImageModel> Data { set; get; }
    }

    public class AppLaunchImageModel
    {
        public string ID { set; get; }
        public string Title { set; get; }
        public string Description { set; get; }
        public string Image { set; get; }
        public string StartDateTime { set; get; }
        public string EndDateTime { set; get; }
        public string ShowForSeconds { set; get; }
    }

    public class AppLaunchImageTimestampResponseModel
    {
        public string Status { set; get; }
        public List<AppLaunchImageTimestamp> Data { set; get; }
    }

    public class AppLaunchImageTimestamp
    {
        public string Timestamp { set; get; }
        public string ID { set; get; }
    }
}
