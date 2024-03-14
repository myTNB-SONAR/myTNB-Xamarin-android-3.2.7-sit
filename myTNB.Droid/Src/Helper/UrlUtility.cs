using System;
namespace myTNB.Android.Src.Helper
{
    public class UrlUtility
    {
        public string url { get; set; }
        public string QueyParams { get; set; }
        public void AddQueryParams(string name, string value)
        {            
            QueyParams += QueyParams == null ? "?" : "&";
            QueyParams += $"{name}={value}";
        }
        public string EncodeURL(string domain)
        {
            url = domain + QueyParams;
            url = Uri.EscapeUriString(url);
            return url;
        }
    }
}
