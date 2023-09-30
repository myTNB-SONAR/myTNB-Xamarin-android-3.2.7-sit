using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using myTNB.Mobile.AWS.Models;
using myTNB.Mobile.Extensions;
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

        /*public bool IsAccountEligible
        {
            get
            {
                return EligibilitySessionCache.Instance.IsFeatureEligible(Features.DS, FeatureProperty.Enabled);
            }
        }*/

        public bool IsNotificationEligible
        {
            get
            {
                if (EligibilitySessionCache.Instance.IsFeatureEligible(Features.DBR, FeatureProperty.Enabled))
                {
                    if (EligibilitySessionCache.Instance.IsFeatureEligible(Features.DBR, FeatureProperty.TargetGroup))
                    {
                        return LanguageManager.Instance.GetConfigProperty<bool>(LanguageManager.ConfigPropertyEnum.IsDSNotificationEnable);
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

        internal bool IsCAEligible(string ca)
        {
            try
            {
                BaseCAListModel dsContent = EligibilitySessionCache.Instance.GetFeatureContent<BaseCAListModel>(Features.DS);
                if (dsContent != null
                    && dsContent.ContractAccounts != null
                    && dsContent.ContractAccounts.Count > 0)
                {
                    int index = dsContent.ContractAccounts.FindIndex(x => x.ContractAccount == ca);
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

        public string GetIdentificationTypeDescription(int? identificationType)
        {
            try
            {
                Dictionary<string, List<SelectorModel>> dsLandingDictionary = LanguageManager.Instance.GetSelectorsByPage("DSLanding");
                if (dsLandingDictionary != null
                    && dsLandingDictionary.Count > 0
                    && dsLandingDictionary.ContainsKey("idType")
                    && dsLandingDictionary["idType"] is List<SelectorModel> idTypeList
                    && idTypeList != null
                    && idTypeList.Count > 0
                    && identificationType != null
                    && identificationType.Value.ToString() is string idTypeString
                    && idTypeString.IsValid()
                    && idTypeList.FindIndex(x => x.Key == idTypeString) is int idTypeIndex
                    && idTypeIndex > -1)
                {
                    return idTypeList[idTypeIndex].Description;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("[DEBUG] GetIdentificationTypeDescription Error: " + e.Message);
            }
            return string.Empty;
        }
    }
}