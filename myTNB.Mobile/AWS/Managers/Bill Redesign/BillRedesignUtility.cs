using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using myTNB.Mobile.AWS.Models;
using Newtonsoft.Json.Linq;
using static myTNB.Mobile.EligibilitySessionCache;

namespace myTNB.Mobile
{
    public class BillRedesignUtility
    {
        private static readonly Lazy<BillRedesignUtility> lazy =
           new Lazy<BillRedesignUtility>(() => new BillRedesignUtility());

        public static BillRedesignUtility Instance
        {
            get
            {
                return lazy.Value;
            }
        }

        public BillRedesignUtility() { }

        /// <summary>
        /// Gets all CAs eligible for Bill Redesign
        /// </summary>
        /// <returns></returns>
        public List<string> GetCAs()
        {
            List<string> caList = new List<string>();
            try
            {
                DBRModel billRedesignContent = EligibilitySessionCache.Instance.GetFeatureContent<DBRModel>(Features.BR);
                if (billRedesignContent != null
                    && billRedesignContent.ContractAccounts != null
                    && billRedesignContent.ContractAccounts.Count > 0)
                {
                    caList = billRedesignContent.ContractAccounts.Select(x => x.ContractAccount).ToList();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("[DEBUG]GetBillRedesignCAs Exception: " + e.Message);
            }
            return caList;
        }


        /// <summary>
        /// Checks if myTNB Account is Bill Redesign Eligible
        /// </summary>
        public bool IsAccountEligible
        {
            get
            {
                if (EligibilitySessionCache.Instance.IsFeatureEligible(Features.BR, FeatureProperty.Enabled))
                {
                    if (EligibilitySessionCache.Instance.IsFeatureEligible(Features.BR, FeatureProperty.TargetGroup))
                    {
                        return GetCAs() is List<string> caList
                            && caList != null
                            && caList.Count > 0;
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

        public bool ShouldShowHomeCard
        {
            get
            {
                try
                {
                    if (LanguageManager.Instance.GetServiceConfig("ServiceConfiguration", "ForceHideBillRedesignBanner") is JToken config
                        && config != null
                        && config.ToObject<bool>() is bool isForceHide
                        && isForceHide)
                    {
                        return false;
                    }
                    else
                    {
                        return IsAccountEligible;
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine("[DEBUG] ShouldShowHomeCard Exception: " + e.Message);
                }
                return false;
            }
        }
    }
}