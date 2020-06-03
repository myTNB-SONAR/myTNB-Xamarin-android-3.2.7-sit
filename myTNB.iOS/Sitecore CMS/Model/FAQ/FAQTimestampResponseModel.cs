using System.Collections.Generic;

namespace myTNB.SitecoreCMS.Model
{
    public class FAQTimestampResponseModel
    {
        public string Status { set; get; }
        public List<FAQsParentModel> Data { set; get; }
    }
}