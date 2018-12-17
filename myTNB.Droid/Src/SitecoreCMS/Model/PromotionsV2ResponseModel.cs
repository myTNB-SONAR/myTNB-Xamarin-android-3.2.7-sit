using myTNB.SitecoreCM.Models;
using System.Collections.Generic;

namespace myTNB.SitecoreCMS.Model
{
    public class PromotionsV2ResponseModel
    {
        public string Status { set; get; }
        public List<PromotionsModelV2> Data { set; get; }
    }
}