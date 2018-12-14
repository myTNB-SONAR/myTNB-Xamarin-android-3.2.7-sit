using System.Collections.Generic;

namespace myTNB.SitecoreCMS.Model
{
    public class PromotionsTimestampResponseModel
    {
        public string Status { set; get; }
        public List<PromotionParentModel> Data { set; get; }
    }
}