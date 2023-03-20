using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using myTNB.Mobile.AWS.Models;
using Newtonsoft.Json.Linq;
using static myTNB.Mobile.EligibilitySessionCache;

namespace myTNB.Mobile
{
    public class MyHomeUtility
    {
        private static readonly Lazy<MyHomeUtility> lazy =
           new Lazy<MyHomeUtility>(() => new MyHomeUtility());

        public static MyHomeUtility Instance
        {
            get
            {
                return lazy.Value;
            }
        }

        public MyHomeUtility()
        {
        }

        /// <summary>
        /// Returns the list of eligible CAs in the account
        /// </summary>
        /// <returns></returns>
        public List<string> GetCAList()
        {
            List<string> caList = new List<string>();
            try
            {
                BaseCAListModel myHomeContent = EligibilitySessionCache.Instance.GetFeatureContent<BaseCAListModel>(Features.MyHome);
                if (myHomeContent != null
                    && myHomeContent.ContractAccounts != null
                    && myHomeContent.ContractAccounts.Count > 0)
                {
                    caList = myHomeContent.ContractAccounts.Select(x => x.ContractAccount).ToList();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("[DEBUG]GetMyHomeCAs Exception: " + e.Message);
            }
            return caList;
        }

        /// <summary>
        /// Detemines if CA is eligible or not
        /// </summary>
        /// <param name="ca"></param>
        /// <returns></returns>
        public bool IsCAEligible(string ca)
        {
            try
            {
                BaseCAListModel myHomeContent = EligibilitySessionCache.Instance.GetFeatureContent<BaseCAListModel>(Features.MyHome);
                if (myHomeContent != null
                    && myHomeContent.ContractAccounts != null
                    && myHomeContent.ContractAccounts.Count > 0)
                {
                    int index = myHomeContent.ContractAccounts.FindIndex(x => x.ContractAccount == ca);
                    return index > -1;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("[DEBUG]IsCAMyHomeEligible Exception: " + e.Message);
            }
            return false;
        }

        public bool IsAccountEligible
        {
            get
            {
                if (EligibilitySessionCache.Instance.IsFeatureEligible(Features.MyHome, FeatureProperty.Enabled))
                {
                    if (EligibilitySessionCache.Instance.IsFeatureEligible(Features.MyHome, FeatureProperty.TargetGroup))
                    {
                        if (GetCAList() is List<string> caList
                            && caList != null
                            && caList.Count > 0)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
        }

        public bool IsMarketingPopupEnabled
        {
            get
            {
                return IsAccountEligible
                    && LanguageManager.Instance.GetConfigToggleValue(LanguageManager.ConfigPropertyEnum.IsMyHomeMarketingPopupEnable);
            }
        }

        public bool IsBannerHidden
        {
            get
            {
                if (!IsAccountEligible)
                {
                    return true;
                }
                else
                {
                    return LanguageManager.Instance.GetConfigToggleValue(LanguageManager.ConfigPropertyEnum.ForceHidemyHomeBanner);
                }
            }
        }
    }
}