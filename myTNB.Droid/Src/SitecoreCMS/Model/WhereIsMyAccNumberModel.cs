using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace myTNB.SitecoreCMS.Model
{
   
    public class WhereIsMyAccNumberTimeStampResponseModel
    {
        public string Status { set; get; }
        public List<WhereIsMyAccNumberTimeStamp> Data { set; get; }
    }

    public class WhereIsMyAccNumberResponseModel
    {
        public string Status { set; get; }
        public List<WhereIsMyAccNumberModel> Data { set; get; }
    }

    public class WhereIsMyAccNumberModel
    {

        public string Title { set; get; }
        public string PopUpTitle { set; get; }
        public string PopUpBody { set; get; }
        public string ImageBase64 { set; get; }
        public string Image { set; get; }
        public string ID { get; set; }
    }

    public class WhereIsMyAccNumberTimeStamp
    {
        public string Timestamp { set; get; }
        public string ID { set; get; }
    }
}