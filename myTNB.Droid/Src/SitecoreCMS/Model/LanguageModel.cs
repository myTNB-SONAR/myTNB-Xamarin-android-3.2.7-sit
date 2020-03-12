using System.Collections.Generic;

namespace myTNB.SitecoreCMS.Model
{
    public class LanguageTimeStampResponseModel
    {
        public string Status { set; get; }
        public List<LanguageTimeStamp> Data { set; get; }
    }

    public class LanguageTimeStamp
    {
        public string Timestamp { set; get; }
        public string ID { set; get; }
    }

    public class LanguageResponseModel
    {
        public string Status { set; get; }
        public List<LanguageModel> Data { set; get; }
    }

    public class LanguageModel
    {
        public string LanguageFile { set; get; }
        public string ID { set; get; }
    }
}