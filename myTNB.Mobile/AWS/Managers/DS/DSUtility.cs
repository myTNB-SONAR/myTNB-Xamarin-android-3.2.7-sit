using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using myTNB.Mobile.AWS.Models;
using static myTNB.Mobile.EligibilitySessionCache;

namespace myTNB.Mobile.AWS.Managers.DS
{
    public sealed class DSUtility
    {
        private static readonly Lazy<DSUtility> lazy =
           new Lazy<DSUtility>(() => new DSUtility());

        public static DSUtility Instance
        {
            get
            {
                return lazy.Value;
            }
        }

        public DSUtility()
        {
        }

        public bool IsAccountEligible
        {
            get
            {
                return true;
                if (EligibilitySessionCache.Instance.IsFeatureEligible(Features.DS, FeatureProperty.Enabled))
                {
                    if (EligibilitySessionCache.Instance.IsFeatureEligible(Features.DS, FeatureProperty.TargetGroup))
                    {
                        return GetCAList() is List<string> caList
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

        private List<string> GetCAList()
        {
            List<string> caList = new List<string>();
            try
            {
                BaseCAListModel dsContent = EligibilitySessionCache.Instance.GetFeatureContent<BaseCAListModel>(Features.DS);
                if (dsContent != null
                    && dsContent.ContractAccounts != null
                    && dsContent.ContractAccounts.Count > 0)
                {
                    caList = dsContent.ContractAccounts.Select(x => x.ContractAccount).ToList();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("[DEBUG]GetDSCAs Exception: " + e.Message);
            }
            return caList;
        }

    }
}