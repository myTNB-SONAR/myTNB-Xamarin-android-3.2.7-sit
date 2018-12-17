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
using Refit;

namespace myTNB_Android.Src.Base.Models
{
    public class BaseRequest
    {

        [AliasAs("apiKeyID")]
        public string apiKeyID { get; set; }

        public BaseRequest(string apiKeyID)
        {
            this.apiKeyID = apiKeyID;
        }
    }
}