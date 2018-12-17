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
using myTNB_Android.Src.Base.Models;

namespace myTNB_Android.Src.AppLaunch.Requests
{
    public class GetAccountTypesRequest : BaseRequest
    {
        public GetAccountTypesRequest(string apiKeyID) : base(apiKeyID)
        {
            base.apiKeyID = apiKeyID;
        }
    }
}