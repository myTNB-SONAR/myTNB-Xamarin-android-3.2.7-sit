using myTNB.SitecoreCMS.Model;
using System.Collections.Generic;

namespace myTNB.AndroidApp.Src.SitecoreCMS.Model
{
    public class FullRTEPagesResponseModel
    {
        public string Status { set; get; }
        public List<FullRTEPagesModel> Data { set; get; }
    }
}