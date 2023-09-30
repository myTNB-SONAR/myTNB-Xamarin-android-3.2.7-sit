using System;
using System.Web;
using myTNB.Mobile.SessionCache;
using System.Text.RegularExpressions;

using Constant = myTNB_Android.Src.Utils.Deeplink.Deeplink.Constants;
using Screen = myTNB_Android.Src.Utils.Deeplink.Deeplink.ScreenEnum;
using myTNB_Android.Src.DigitalSignature.IdentityVerification.MVP;

namespace myTNB_Android.Src.Utils.Deeplink
{
    public sealed class DeeplinkUtil
    {
        static DeeplinkUtil? instance;

        public string ScreenKey = string.Empty;
        public Screen TargetScreen = Screen.None;

        public DSDynamicLinkParamsModel EKYCDynamicLinkModel = new DSDynamicLinkParamsModel();

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

        private void SaveDeeplinkDetails(Screen deeplinkScreen, Android.Net.Uri deeplink)
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
                        EKYCDynamicLinkModel.UserID = deeplinkQuery[Constant.UserIDKey];
                        string isContractorAppliedStr = deeplinkQuery[Constant.eKYCIsContractorAppliedKey];
                        if (isContractorAppliedStr.IsValid())
                        {
                            EKYCDynamicLinkModel.IsContractorApplied = bool.Parse(isContractorAppliedStr);
                        }

                        EKYCDynamicLinkModel.AppRef = deeplinkQuery[Constant.eKYCAppRefKey];
                        EKYCDynamicLinkModel.IdentificationNo = deeplinkQuery[Constant.identificationNo];

                        string identificationTypeStr = deeplinkQuery[Constant.identificationType];
                        if (identificationTypeStr.IsValid())
                        {
                            EKYCDynamicLinkModel.IdentificationType = int.Parse(identificationTypeStr);
                        }

                        EKYCDynamicLinkModel.ApplicationModuleID = deeplinkQuery[Constant.applicationModuleID];
                        EKYCDynamicLinkModel.Email = deeplinkQuery[Constant.email];
                    }
                    else
                    {
                        EKYCDynamicLinkModel.UserID = GetParamValueFromKey(Constant.UserIDKey, deeplink);
                        string isContractorAppliedStr = GetParamValueFromKey(Constant.eKYCIsContractorAppliedKey, deeplink);
                        if (isContractorAppliedStr.IsValid())
                        {
                            EKYCDynamicLinkModel.IsContractorApplied = bool.Parse(isContractorAppliedStr);
                        }
                        EKYCDynamicLinkModel.AppRef = GetParamValueFromKey(Constant.eKYCAppRefKey, deeplink);
                        EKYCDynamicLinkModel.IdentificationNo = GetParamValueFromKey(Constant.identificationNo, deeplink);

                        string identificationTypeStr = GetParamValueFromKey(Constant.identificationType, deeplink);
                        if (identificationTypeStr.IsValid())
                        {
                            EKYCDynamicLinkModel.IdentificationType = int.Parse(identificationTypeStr);
                        }

                        EKYCDynamicLinkModel.ApplicationModuleID = GetParamValueFromKey(Constant.applicationModuleID, deeplink);
                        EKYCDynamicLinkModel.Email = GetParamValueFromKey(Constant.email, deeplink);
                    }
                    break;
                default:
                    break;
            }
        }

        public void InitiateDeepLink(Android.Net.Uri deeplink)
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
            EKYCDynamicLinkModel = new DSDynamicLinkParamsModel();
        }

        private void SaveDeeplinkDetailsForQR(Android.Net.Uri deeplink)
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

        private string GetParamValueFromKey(string key, Android.Net.Uri deeplink, bool isQRCode = false)
        {
            string value = string.Empty;
            string queryString = string.Empty;

            try
            {
                if (isQRCode)
                {
                    if (deeplink != null && deeplink.Query != null)
                    {
                        queryString = deeplink.Query.ToString();
                    }
                }
                else
                {
                    queryString = deeplink.Path ?? string.Empty;
                }

                var parameters = queryString?.Split(Constant.Slash);
                if (parameters != null)
                {
                    if (parameters.Length > 0)
                    {
                        foreach (var item in parameters)
                        {
                            var segment = item?.Split(Constant.AmperSand);
                            if (segment != null)
                            {
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
                    }
                }
                
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
            
            return value;
        }
    }
}
