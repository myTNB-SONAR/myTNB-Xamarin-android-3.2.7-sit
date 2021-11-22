using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using myTNB.Mobile.AWS.Models;
using Newtonsoft.Json.Linq;
using static myTNB.Mobile.EligibilitySessionCache;

namespace myTNB.Mobile
{
    public class DBRUtility
    {
        private static readonly Lazy<DBRUtility> lazy =
           new Lazy<DBRUtility>(() => new DBRUtility());

        public static DBRUtility Instance
        {
            get
            {
                return lazy.Value;
            }
        }
        public DBRUtility()
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
                BaseCAListModel dbrContent = EligibilitySessionCache.Instance.GetFeatureContent<BaseCAListModel>(Features.DBR);
                if (dbrContent != null
                    && dbrContent.ContractAccounts != null
                    && dbrContent.ContractAccounts.Count > 0)
                {
                    caList = dbrContent.ContractAccounts.Select(x => x.ContractAccount).ToList();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("[DEBUG]GetDBRCAs Exception: " + e.Message);
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
                BaseCAListModel dbrContent = EligibilitySessionCache.Instance.GetFeatureContent<BaseCAListModel>(Features.DBR);
                if (dbrContent != null
                    && dbrContent.ContractAccounts != null
                    && dbrContent.ContractAccounts.Count > 0)
                {
                    int index = dbrContent.ContractAccounts.FindIndex(x => x.ContractAccount == ca);
                    return index > -1;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("[DEBUG]IsCADBREligible Exception: " + e.Message);
            }
            return false;
        }

        public bool IsAccountEligible
        {
            get
            {
                if (EligibilitySessionCache.Instance.IsFeatureEligible(Features.DBR, FeatureProperty.Enabled))
                {
                    if (EligibilitySessionCache.Instance.IsFeatureEligible(Features.DBR, FeatureProperty.TargetGroup))
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

        /// <summary>
        /// Determines if Home DBR Card should be Displayed
        /// </summary>
        public bool ShouldShowHomeCard
        {
            get
            {
                try
                {
                    if (LanguageManager.Instance.GetServiceConfig("ServiceConfiguration", "ForceHideDBRBanner") is JToken config
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
                    Debug.WriteLine("[DEBUG] ShouldShowHomeDBRCard Exception: " + e.Message);
                }
                return false;
            }
        }

        /// <summary>
        /// Determines if DBR Card should be displayed or not.
        /// </summary>
        /// <param name="caList">list of CA for payment</param>
        /// <returns></returns>
        public bool ShouldShowCard(List<string> caList = null)
        {
            if (LanguageManager.Instance.GetServiceConfig("ServiceConfiguration", "ForceHideDBRBanner") is JToken config
                && config != null
                && config.ToObject<bool>() is bool isForceHide
                && isForceHide)
            {
                return false;
            }
            bool ismyTNBAccountEligible = IsAccountEligible;
            if (EligibilitySessionCache.Instance.GetFeatureContent<BaseCAListModel>(Features.DBR) is BaseCAListModel dbrList
                && dbrList != null
                && dbrList.ContractAccounts != null
                && dbrList.ContractAccounts.Count > 0)
            {
                if (caList == null)
                {
                    int index = dbrList.ContractAccounts.FindIndex(x => !x.Acted);
                    return ismyTNBAccountEligible && index > -1;
                }
                else
                {
                    for (int i = 0; i < caList.Count; i++)
                    {
                        ContractAccountsModel item = dbrList.ContractAccounts.Find(x => x.ContractAccount == caList[i]);
                        if (item == null)
                        {
                            continue;
                        }
                        if (!item.Acted)
                        {
                            return ismyTNBAccountEligible;
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Determines if DBR Card should be displayed or not.
        /// </summary>
        /// <param name="ca">For single CA Use</param>
        /// <returns></returns>
        public bool ShouldShowCard(string ca)
        {
            return ShouldShowCard(new List<string> { ca });
        }

        /// <summary>
        /// Determines if the Owner/Tenant tag should be from Eligibility or from device cache.
        /// </summary>
        public bool IsDBROTTagFromCache
        {
            get
            {
                try
                {
                    return !EligibilitySessionCache.Instance.IsFeatureEligible(Features.DBR, FeatureProperty.TargetGroup);
                }
                catch (Exception e)
                {
                    Debug.WriteLine("[DEBUG]IsDBROTTagFromCache Exception: " + e.Message);
                }
                return false;
            }
        }
    }
}