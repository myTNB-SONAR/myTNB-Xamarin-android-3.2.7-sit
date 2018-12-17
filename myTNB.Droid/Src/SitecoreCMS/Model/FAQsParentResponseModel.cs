using myTNB.SitecoreCM.Models;
using myTNB.SitecoreCMS.Models;
using System.Collections.Generic;

namespace myTNB.SitecoreCMS.Model
{
    public class FAQsParentResponseModel
    {
        public string Status { set; get; }
        public List<FAQsParentModel> Data { set; get; }
    }
}