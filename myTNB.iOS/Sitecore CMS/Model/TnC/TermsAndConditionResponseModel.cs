using System.Collections.Generic;

namespace myTNB.SitecoreCMS.Model
{
    public class TermsAndConditionResponseModel
    {
        public string Status { set; get; }
        public List<FullRTEPagesModel> Data { set; get; }
    }
}