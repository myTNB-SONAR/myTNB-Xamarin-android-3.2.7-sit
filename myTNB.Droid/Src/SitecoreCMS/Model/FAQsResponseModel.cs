using myTNB.SitecoreCM.Models;
using myTNB.SitecoreCMS.Models;
using System.Collections.Generic;

namespace myTNB.SitecoreCMS.Model
{
    public class FAQsResponseModel
    {
        public string Status { set; get; }
        public List<FAQsModel> Data { set; get; }
    }
}