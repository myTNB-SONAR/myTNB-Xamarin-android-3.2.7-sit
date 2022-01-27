using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using myTNB.Mobile.AWS.Models;
using Newtonsoft.Json.Linq;
using static myTNB.Mobile.EligibilitySessionCache;

namespace myTNB.Mobile
{
    public class SDUtility
    {
        private static readonly Lazy<SDUtility> lazy = new Lazy<SDUtility>(() => new SDUtility());

        public static SDUtility Instance
        {
            get
            {
                return lazy.Value;
            }
        }

        public SDUtility()
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
                BaseCAListModel sdContent = EligibilitySessionCache.Instance.GetFeatureContent<BaseCAListModel>(Features.SD);
                if (sdContent != null
                    && sdContent.ContractAccounts != null
                    && sdContent.ContractAccounts.Count > 0)
                {
                    caList = sdContent.ContractAccounts.Select(x => x.ContractAccount).ToList();
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
                BaseCAListModel sdContent = EligibilitySessionCache.Instance.GetFeatureContent<BaseCAListModel>(Features.SD);
                if (sdContent != null
                    && sdContent.ContractAccounts != null
                    && sdContent.ContractAccounts.Count > 0)
                {
                    int index = sdContent.ContractAccounts.FindIndex(x => x.ContractAccount == ca);
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
                if (EligibilitySessionCache.Instance.IsFeatureEligible(Features.SD, FeatureProperty.Enabled))
                {
                    if (EligibilitySessionCache.Instance.IsFeatureEligible(Features.SD, FeatureProperty.TargetGroup))
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





    }
}
