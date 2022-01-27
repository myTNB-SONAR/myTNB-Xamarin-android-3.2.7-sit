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
        public List<string> GetCAList()
        {
            List<string> caList = new List<string>();
            try
            {
                BaseCAListModel billRedesignContent = EligibilitySessionCache.Instance.GetFeatureContent<BaseCAListModel>(Features.BR);
                if (billRedesignContent != null
                    && billRedesignContent.ContractAccounts != null
                    && billRedesignContent.ContractAccounts.Count > 0)
                {
                    caList = billRedesignContent.ContractAccounts.Select(x => x.ContractAccount).ToList();
                }
            }
            catch (Exception e)
            {
#if DEBUG
                Debug.WriteLine("[DEBUG]GetBillRedesignCAs Exception: " + e.Message);
#endif
            }
            return caList;
        }

        public bool IsAccountEligible
        {
            get
            {
                if (EligibilitySessionCache.Instance.IsFeatureEligible(Features.BR, FeatureProperty.Enabled))
                {
                    if (EligibilitySessionCache.Instance.IsFeatureEligible(Features.BR, FeatureProperty.TargetGroup))
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

        public bool IsCAEligible(string ca)
        {
            try
            {
                BaseCAListModel brContent = EligibilitySessionCache.Instance.GetFeatureContent<BaseCAListModel>(Features.BR);
                if (brContent != null
                    && brContent.ContractAccounts != null
                    && brContent.ContractAccounts.Count > 0)
                {
                    int index = brContent.ContractAccounts.FindIndex(x => x.ContractAccount == ca);
                    return index > -1;
                }
            }
            catch (Exception e)
            {
#if DEBUG
                Debug.WriteLine("[DEBUG]IsCAEligible Exception: " + e.Message);
#endif
            }
            return false;
        }

        public bool IsAccountStatementEligible(string ca
            , bool isOwner)
        {
            try
            {
                bool isCAEligible = IsCAEligible(ca);
                if (isOwner)
                {
                    return isCAEligible;
                }
                else
                {
                    bool isEligibleForNonOwner = LanguageManager.Instance.GetConfigToggleValue(LanguageManager.TogglePropertyEnum.ShouldShowAccountStatementToNonOwner);
                    if (isEligibleForNonOwner)
                    {
                        return isCAEligible;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception e)
            {
#if DEBUG
                Debug.WriteLine("[DEBUG]IsAccountStatementEligible Exception: " + e.Message);
#endif
            }
            return false;
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
#if DEBUG
                    Debug.WriteLine("[DEBUG] ShouldShowHomeCard Exception: " + e.Message);
#endif
                }
                return false;
            }
        }
    }
}