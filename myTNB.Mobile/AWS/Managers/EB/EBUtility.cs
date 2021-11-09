using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using myTNB.Mobile.AWS.Models;
using Newtonsoft.Json.Linq;
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
        public EBUtility()
        {
        }

        
        public bool IsPublicRelease
        {
            get
            {
                if (EligibilitySessionCache.Instance.IsFeatureEligible(Features.EB, FeatureProperty.Enabled))
                {
                    // if target group is false then it will lead to public release
                    if (!EligibilitySessionCache.Instance.IsFeatureEligible(Features.EB, FeatureProperty.TargetGroup))
                    {
  
                        return true;
                    }

                    return false;
                }
                else
                {
                    return false;
                }
            }
        }

    }
}