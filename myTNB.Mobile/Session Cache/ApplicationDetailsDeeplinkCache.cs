using System;
using myTNB.Mobile.Extensions;

namespace myTNB.Mobile.SessionCache
{
    public sealed class ApplicationDetailsDeeplinkCache
    {
        private static readonly Lazy<ApplicationDetailsDeeplinkCache> lazy
             = new Lazy<ApplicationDetailsDeeplinkCache>(() => new ApplicationDetailsDeeplinkCache());

        public static ApplicationDetailsDeeplinkCache Instance
        {
            get
            {
                return lazy.Value;
            }
        }

        private ApplicationDetailsDeeplinkCache() { }

        public string ID { private set; get; } = string.Empty;
        public string Type { private set; get; } = string.Empty;
        public string System { private set; get; } = string.Empty;
        public string SaveID { private set; get; } = string.Empty;
        public bool IsListingDeepLink { private set; get; } = false;
        public bool IsDetailsDeepLink { private set; get; } = false;

        public void SetData(string deepLinkURL)
        {
            if (deepLinkURL.IsValid())
            {
                string[] detailsSegmentArray = deepLinkURL.Split(new string[] { "applicationDetails/" }, StringSplitOptions.None);
                if (detailsSegmentArray.Length > 1)
                {
                    string[] detailsArray = detailsSegmentArray[1].Split('/');
                    if (detailsArray.Length > 1)
                    {
                        ID = detailsArray[0];
                        Type = detailsArray[1];
                        if (detailsArray.Length > 2)
                        {
                            System = detailsArray[2];
                        }
                        else
                        {
                            System = "myTNB";
                        }
                        if (detailsArray.Length > 3)
                        {
                            SaveID = detailsArray[3];
                        }
                        else
                        {
                            SaveID = string.Empty;
                        }
                    }
                }
            }
        }

        public void SetIsListingDeepLink(bool isDeepLink)
        {
            IsListingDeepLink = isDeepLink;
        }

        public void SetIsDetailsDeepLink(bool isDetailsDeepLink)
        {
            IsDetailsDeepLink = isDetailsDeepLink;
        }
    }
}