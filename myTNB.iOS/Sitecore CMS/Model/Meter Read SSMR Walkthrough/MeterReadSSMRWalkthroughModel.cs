using System;
using System.Collections.Generic;

namespace myTNB.SitecoreCMS.Model
{
    public class MeterReadSSMRTimeStampResponseModel
    {
        public string Status { set; get; }
        public List<MeterReadSSMRTimeStamp> Data { set; get; }
    }

    public class MeterReadSSMRResponseModel
    {
        public string Status { set; get; }
        public List<MeterReadSSMRModel> Data { set; get; }
    }

    public class MeterReadSSMRModel
    {
        public string Title { set; get; }
        public string Description { set; get; }
        public string Image { set; get; }
        public byte[] ImageByteArray { set; get; }

        public string ID { set; get; }
    }

    public class MeterReadSSMRTimeStamp
    {
        public string Timestamp { set; get; }
        public string ID { set; get; }
    }
}
