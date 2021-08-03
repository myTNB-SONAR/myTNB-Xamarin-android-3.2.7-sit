using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using myTNB.Mobile.AWS.Models;
using Newtonsoft.Json.Linq;

namespace myTNB.Mobile
{
    public sealed class EligibilitySessionCache
    {
        private static readonly Lazy<EligibilitySessionCache> lazy =
           new Lazy<EligibilitySessionCache>(() => new EligibilitySessionCache());

        public static EligibilitySessionCache Instance
        {
            get
            {
                return lazy.Value;
            }
        }

        public enum Features
        {
            DBR,
            DS
            //Add Other Features
        }

        public enum FeatureProperty
        {
            Enabled,
            TargetGroup
            //Add Other Properties
        }

        public EligibilitySessionCache()
        {
        }

        private GetEligibilityResponse Data { set; get; }

        /// <summary>
        /// Sets the Session Data for eligibility
        /// </summary>
        /// <param name="response">GetEligibilityResponse</param>
        public void SetData(GetEligibilityResponse response)
        {
            if (response != null)
            {
                Data = response;
            }
        }

        /// <summary>
        /// Checks if feature is enabled
        /// Can check enabled and targetgroup property
        /// Feature and Propery are binded by enum to avoid wrong input
        /// </summary>
        /// <param name="feature">myTNB App Feature</param>
        /// <param name="featureProperty">property</param>
        /// <returns></returns>
        public bool IsFeatureEligible(Features feature
            , FeatureProperty featureProperty)
        {
            if (Data != null
                && Data.StatusDetail != null
                && Data.StatusDetail.IsSuccess
                && Data.Content != null
                && Data.Content.EligibileFeatures is EligibileFeaturesModel eligibleFeatures
                && eligibleFeatures != null
                && eligibleFeatures.EligibleFeatureDetails is List<EligibileFeatureDetailsModel> eligibleFeaturesList
                && eligibleFeaturesList != null
                && eligibleFeaturesList.Count > 0)
            {
                string featureString = feature.ToString();
                int index = eligibleFeaturesList.FindIndex(x => x.Feature == featureString);
                if (index > -1)
                {
                    if (featureProperty == FeatureProperty.Enabled)
                    {
                        return eligibleFeaturesList[index].Enabled;
                    }
                    else if (featureProperty == FeatureProperty.TargetGroup)
                    {
                        return eligibleFeaturesList[index].TargetGroup;
                    }
                    //Add Other Properties
                }
            }
            return false;
        }

        /// <summary>
        /// Returns the whole property value based on feature
        /// </summary>
        /// <typeparam name="T">Custom class of under Content Data</typeparam>
        /// <param name="feature">Name of Feature</param>
        /// <returns></returns>
        public T GetFeatureContent<T>(Features feature) where T : new()
        {
            T customClass = new T();
            try
            {
                if (Data != null
                    && Data.StatusDetail != null
                    && Data.StatusDetail.IsSuccess
                    && Data.Content != null)
                {
                    string featureString = feature.ToString();
                    Type content = Data.Content.GetType();
                    if (content != null
                        && content.GetProperty(featureString) is PropertyInfo props
                        && props != null)
                    {
                        object obj = props.GetValue(Data.Content, null);
                        customClass = (T)Convert.ChangeType(obj, typeof(T));
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("[DEBUG][Encrypt]GetFeatureContent Exception: " + e.Message);
            }
            return customClass;
        }

        public void Clear()
        {
            Data = null;
        }

        /// <summary>
        /// Returns the list of eligible CAs in the account
        /// </summary>
        /// <returns></returns>
        public List<string> GetDBRCAs()
        {
            List<string> caList = new List<string>();
            try
            {
                DBRModel dbrContent = GetFeatureContent<DBRModel>(Features.DBR);
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
        public bool IsCADBREligible(string ca)
        {
            try
            {
                DBRModel dbrContent = GetFeatureContent<DBRModel>(Features.DBR);
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

        public bool IsAccountDBREligible
        {
            get
            {
                return IsFeatureEligible(Features.DBR, FeatureProperty.Enabled)
                    && GetDBRCAs() is List<string> caList
                    && caList != null
                    && caList.Count > 0;
            }
        }

        /// <summary>
        /// Determines if DBR Card should be displayed or not.
        /// </summary>
        /// <param name="caList">list of CA for payment</param>
        /// <returns></returns>
        public bool ShouldShowDBRCard(List<string> caList = null)
        {
            if (LanguageManager.Instance.GetServiceConfig("ServiceConfiguration", "ForceHideDBRBanner") is JToken config
                && config != null
                && config.ToObject<bool>() is bool isForceHide
                && isForceHide)
            {
                return false;
            }
            bool ismyTNBAccountEligible = IsAccountDBREligible;
            if (GetFeatureContent<DBRModel>(Features.DBR) is DBRModel dbrList
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
        public bool ShouldShowDBRCard(string ca)
        {
            return ShouldShowDBRCard(new List<string> { ca });
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
                    return !IsFeatureEligible(Features.DBR, FeatureProperty.TargetGroup);
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