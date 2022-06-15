using System.Web;
using Android.Net;
using myTNB.Mobile.SessionCache;
using System.Text.RegularExpressions;

using Constant = myTNB_Android.Src.Utils.Deeplink.Deeplink.Constants;
using Screen = myTNB_Android.Src.Utils.Deeplink.Deeplink.ScreenEnum;

namespace myTNB_Android.Src.Utils.Deeplink
{
    public sealed class DeeplinkUtil
    {
        static DeeplinkUtil? instance;

        public string ScreenKey = string.Empty;
        public Screen TargetScreen = Screen.None;

        public string eKYCIsContractorApplied = string.Empty;

        public static DeeplinkUtil Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new DeeplinkUtil();
                }
                return instance;
            }
        }

        private void SaveDeeplinkDetails(Screen deeplinkScreen, Uri deeplink)
        {
            switch (deeplinkScreen)
            {
                case Screen.Rewards:
                    if (deeplink.Query != null &&
                        deeplink.Query.IsValid())
                    {
                        var deeplinkQuery = HttpUtility.ParseQueryString(deeplink.Query);
                        ScreenKey = deeplinkQuery[Constant.RewardsIDKey];
                    }
                    else
                    {
                        ScreenKey = GetParamValueFromKey(Constant.RewardsIDKey, deeplink);
                    }
                    break;
                case Screen.WhatsNew:
                    if (deeplink.Query != null &&
                        deeplink.Query.IsValid())
                    {
                        var deeplinkQuery = HttpUtility.ParseQueryString(deeplink.Query);
                        ScreenKey = deeplinkQuery[Constant.WhatsNewIDKey];
                    }
                    else
                    {
                        ScreenKey = GetParamValueFromKey(Constant.WhatsNewIDKey, deeplink);
                    }
                    break;
                case Screen.ApplicationDetails:
                    ApplicationDetailsDeeplinkCache.Instance.SetData(deeplink.ToString());
                    break;
                case Screen.OvervoltageClaimDetails:
                    EnquiryDetailsDeeplinkCache.Instance.SetData(deeplink.ToString());
                    break;
                case Screen.IdentityVerification:
                    if (deeplink.Query != null &&
                        deeplink.Query.IsValid())
                    {
                        var deeplinkQuery = HttpUtility.ParseQueryString(deeplink.Query);
                        ScreenKey = deeplinkQuery[Constant.UserIDKey];
                        eKYCIsContractorApplied = deeplinkQuery[Constant.IsContractorAppliedKey];
                    }
                    else
                    {
                        ScreenKey = GetParamValueFromKey(Constant.UserIDKey, deeplink);
                        eKYCIsContractorApplied = GetParamValueFromKey(Constant.IsContractorAppliedKey, deeplink);
                    }
                    break;
                default:
                    break;
            }
        }

        public void InitiateDeepLink(Uri deeplink)
        {
            if (deeplink != null)
            {
                string deepLinkUrlString = deeplink.ToString().ToLower();
                if (deepLinkUrlString.IsValid())
                {
                    if (deepLinkUrlString.Contains(Screen.QR.ToString().ToLower()))
                    {
                        SaveDeeplinkDetailsForQR(deeplink);
                    }
                    else if (deepLinkUrlString.Contains(Screen.Rewards.ToString().ToLower()))
                    {
                        TargetScreen = Screen.Rewards;
                        SaveDeeplinkDetails(Screen.Rewards, deeplink);
                    }
                    else if (deepLinkUrlString.Contains(Screen.WhatsNew.ToString().ToLower()))
                    {
                        TargetScreen = Screen.WhatsNew;
                        SaveDeeplinkDetails(Screen.WhatsNew, deeplink);
                    }
                    else if (deepLinkUrlString.Contains(Screen.ApplicationListing.ToString().ToLower()))
                    {
                        TargetScreen = Screen.ApplicationListing;
                    }
                    else if (deepLinkUrlString.Contains(Screen.ApplicationDetails.ToString().ToLower()))
                    {
                        TargetScreen = Screen.ApplicationDetails;
                        SaveDeeplinkDetails(Screen.ApplicationDetails, deeplink);
                    }
                    else if (deepLinkUrlString.Contains(Screen.OvervoltageClaimDetails.ToString().ToLower()))
                    {
                        TargetScreen = Screen.OvervoltageClaimDetails;
                        SaveDeeplinkDetails(Screen.OvervoltageClaimDetails, deeplink);
                    }
                    else if (deepLinkUrlString.Contains(Screen.ManageBillDelivery.ToString().ToLower()))
                    {
                        TargetScreen = Screen.ManageBillDelivery;
                    }
                    else if (deepLinkUrlString.Contains(Screen.IdentityVerification.ToString().ToLower()))
                    {
                        TargetScreen = Screen.IdentityVerification;
                        SaveDeeplinkDetails(Screen.IdentityVerification, deeplink);
                    }
                }
            }
        }

        public void ClearDeeplinkData()
        {
            TargetScreen = Screen.None;
            ScreenKey = string.Empty;
            eKYCIsContractorApplied = string.Empty;
        }

        private void SaveDeeplinkDetailsForQR(Uri deeplink)
        {
            string deepLinkUrlString = deeplink.ToString().ToLower();
            {
                if (deepLinkUrlString.Contains(Screen.GetBill.ToString().ToLower()))
                {
                    TargetScreen = Screen.GetBill;
                    ScreenKey = GetParamValueFromKey(Constant.GetBillIDKey, deeplink, true);
                }
            }
        }

        private string GetParamValueFromKey(string key, Uri deeplink, bool isQRCode = false)
        {
            string value = string.Empty;
            string queryString = string.Empty;

            if (isQRCode)
            {
                if (deeplink != null && deeplink.Query != null)
                {
                    queryString = deeplink.Query.ToString();
                }
            }
            else
            {
                queryString = deeplink.Path.ToLower();
            }

            var parameters = queryString?.Split(Constant.Slash);
            if (parameters.Length > 0)
            {
                foreach (var item in parameters)
                {
                    var segment = item?.Split(Constant.AmperSand);
                    if (segment.Length > 0)
                    {
                        foreach (var pair in segment)
                        {
                            string pattern = string.Format(Constant.Pattern, key);
                            Regex regex = new Regex(pattern);
                            Match match = regex.Match(pair);
                            if (match.Success)
                            {
                                value = match.Value.Replace(string.Format(Constant.ReplaceKey, key), string.Empty);
                                break;
                            }
                        }
                    }
                }
            }
            return value;
        }
    }
}
