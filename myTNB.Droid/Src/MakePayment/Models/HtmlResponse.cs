using System;
using System.Net;

namespace myTNB_Android.Src.MakePayment.Models
{
    public class HtmlResponse
    {
        private static WebResponse response;

        public HtmlResponse(WebResponse res)
        {
            response = res;
        }

        public static WebResponse GetWebResponse(){
            return response;
        }
    }
}
