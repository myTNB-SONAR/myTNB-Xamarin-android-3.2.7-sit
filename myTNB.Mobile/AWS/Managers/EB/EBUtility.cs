using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using myTNB.Mobile.AWS.Models;
using static myTNB.Mobile.EligibilitySessionCache;

namespace myTNB.Mobile
{
    public class EBUtility
    {
        private static readonly Lazy<EBUtility> lazy =
           new Lazy<EBUtility>(() => new EBUtility());

        public static EBUtility Instance
        {
            get
            {
                return lazy.Value;
            }
        }

        public EBUtility() { }

        public List<string> GetCAList()
        {
            List<string> caList = new List<string>();
            try
            {
                BaseCAListModel ebContent = EligibilitySessionCache.Instance.GetFeatureContent<BaseCAListModel>(Features.EB);
                if (ebContent != null
                    && ebContent.ContractAccounts != null
                    && ebContent.ContractAccounts.Count > 0)
                {
                    caList = ebContent.ContractAccounts.Select(x => x.ContractAccount).ToList();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("[DEBUG]GetEBCAs Exception: " + e.Message);
            }
            return caList;
        }

        public bool IsAccountEligible
        {
            get
            {
                if (EligibilitySessionCache.Instance.IsFeatureEligible(Features.EB, FeatureProperty.Enabled))
                {
                    if (EligibilitySessionCache.Instance.IsFeatureEligible(Features.EB, FeatureProperty.TargetGroup))
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