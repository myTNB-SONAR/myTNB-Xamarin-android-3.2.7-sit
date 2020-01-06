using System.Collections.Generic;

namespace myTNB.SitecoreCMS.Model
{
    public class PromotionsParentResponseModel
    {
        public string Status { set; get; }
        public List<PromotionParentModel> Data { set; get; }
    }
}