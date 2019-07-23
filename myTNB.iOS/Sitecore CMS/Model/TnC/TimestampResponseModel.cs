using System.Collections.Generic;

namespace myTNB.SitecoreCMS.Model
{
    public class TimestampResponseModel
    {
        public string Status { set; get; }
        public List<TimestampModel> Data { set; get; }
    }
}