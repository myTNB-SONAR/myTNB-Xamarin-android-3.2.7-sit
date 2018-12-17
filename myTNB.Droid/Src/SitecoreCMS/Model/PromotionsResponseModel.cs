using myTNB.SitecoreCM.Models;
using System.Collections.Generic;

namespace myTNB.SitecoreCMS.Model
{
    public class PromotionsResponseModel
    {
        public string Status { set; get; }
        public List<PromotionsModel> Data { set; get; }
    }
}