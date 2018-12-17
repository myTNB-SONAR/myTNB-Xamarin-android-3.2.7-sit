using myTNB.SitecoreCM.Models;
using System.Collections.Generic;

namespace myTNB.SitecoreCMS.Model
{
    public class PromotionsParentV2ResponseModel
    {
        public string Status { set; get; }
        public List<PromotionParentModelV2> Data { set; get; }
    }
}