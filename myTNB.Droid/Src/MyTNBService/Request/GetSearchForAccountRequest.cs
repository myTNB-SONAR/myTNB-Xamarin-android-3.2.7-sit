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

namespace myTNB.Android.Src.MyTNBService.Request
{
   public class GetSearchForAccountRequest : BaseRequest
    {

        public List<string> contractAccounts = new List<string>();

        public GetSearchForAccountRequest(string accNum)
        {
             contractAccounts.Add(accNum);
        }

     

    }
}