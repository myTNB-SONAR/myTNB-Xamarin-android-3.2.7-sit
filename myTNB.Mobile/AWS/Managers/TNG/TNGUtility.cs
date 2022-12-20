using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using myTNB.Mobile.AWS.Models;
using static myTNB.Mobile.EligibilitySessionCache;

namespace myTNB.Mobile
{
    public class TNGUtility
    {
        private static readonly Lazy<TNGUtility> lazy =
           new Lazy<TNGUtility>(() => new TNGUtility());

        public static TNGUtility Instance
        {
            get
            {
                return lazy.Value;
            }
        }

        public TNGUtility() { }


        public List<string> GetCAList()
        {
            List<string> caList = new List<string>();
            try
            {
                BaseCAListModel tngContent = EligibilitySessionCache.Instance.GetFeatureContent<BaseCAListModel>(Features.TNG);
                if (tngContent != null
                    && tngContent.ContractAccounts != null
                    && tngContent.ContractAccounts.Count > 0)
                {
                    caList = tngContent.ContractAccounts.Select(x => x.ContractAccount).ToList();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("[DEBUG]GetTNGCAs Exception: " + e.Message);
            }
            return caList;
        }

        public bool IsAccountEligible
        {
            get
            {
                if (EligibilitySessionCache.Instance.IsFeatureEligible(Features.TNG, FeatureProperty.Enabled))
                {
                    if (EligibilitySessionCache.Instance.IsFeatureEligible(Features.TNG, FeatureProperty.TargetGroup))
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