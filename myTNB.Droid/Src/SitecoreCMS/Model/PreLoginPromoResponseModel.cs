using System.Collections.Generic;

namespace myTNB.SitecoreCMS.Model
{
    public class PreLoginPromoResponseModel
    {
        public string Status { set; get; }
        public List<PreLoginPromoModel> Data { set; get; }
    }
}