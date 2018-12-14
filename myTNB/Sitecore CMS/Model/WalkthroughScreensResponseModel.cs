using System.Collections.Generic;

namespace myTNB.SitecoreCMS.Model
{
    public class WalkthroughScreensResponseModel
    {
        public string Status { set; get; }
        public List<WalkthroughScreensModel> Data { set; get; }
    }
}