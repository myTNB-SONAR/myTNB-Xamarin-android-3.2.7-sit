using System;
using myTNB.Mobile.Extensions;

namespace myTNB.Mobile.SessionCache
{
    public sealed class EnquiryDetailsDeeplinkCache
    {
        private static readonly Lazy<EnquiryDetailsDeeplinkCache> lazy
             = new Lazy<EnquiryDetailsDeeplinkCache>(() => new EnquiryDetailsDeeplinkCache());

        public static EnquiryDetailsDeeplinkCache Instance
        {
            get
            {
                return lazy.Value;
            }
        }

        public EnquiryDetailsDeeplinkCache()
        {
        }
        public bool IsListingDeepLink { private set; get; } = false;
        public bool IsDetailsDeepLink { private set; get; } = false;
        public string ClaimID { private set; get; } = string.Empty;
        public string SRNumber { private set; get; } = string.Empty;
        public string Type { private set; get; } = string.Empty;
        public string System { private set; get; } = string.Empty;

        public void SetData(string deepLinkURL)
        {
            if (deepLinkURL.IsValid())
            {
                string[] claimIdArray = deepLinkURL.Split(new string[] { "overvoltageClaimDetails/" }, StringSplitOptions.None);
                if (claimIdArray.Length > 1)
                {
                    string[] detailsArray = claimIdArray[1].Split('/');
                    if (detailsArray.Length > 1)
                    {
                        SRNumber = detailsArray[0];
                        ClaimID = detailsArray[2];
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
