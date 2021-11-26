using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using myTNB.Mobile.AWS.Models;
using myTNB.Mobile.Extensions;

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
            EB,
            BR
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

        //This will hold Android's and iOS' CA List
        internal List<CACriteriaModel> CAList { set; get; }

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

        public void SetCAList(List<CACriteriaModel> caList)
        {
            this.CAList = caList;
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
                Debug.WriteLine("[DEBUG] GetFeatureContent Exception: " + e.Message);
            }
            return customClass;
        }

        public EligibilityCriteriaModel GetFeatureCriteria(Features feature)
        {
            EligibilityCriteriaModel criteria = null;
            try
            {
                string featureString = feature.ToString();
                if (Data != null
                    && Data.StatusDetail != null
                    && Data.StatusDetail.IsSuccess
                    && Data.Content != null
                    && Data.Content.EligibileFeatures is EligibileFeaturesModel eligibleFeatures
                    && eligibleFeatures != null
                    && eligibleFeatures.EligibleFeatureDetails is List<EligibileFeatureDetailsModel> eligibleFeaturesList
                    && eligibleFeaturesList != null
                    && eligibleFeaturesList.Count > 0
                    && eligibleFeaturesList.FindIndex(x => x.Feature == featureString) is int index
                    && index > -1)
                {
                    return eligibleFeaturesList[index].Criteria;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("[DEBUG] GetFeatureCriteria Exception: " + e.Message);
            }
            return criteria;
        }

        public void Clear()
        {
            Data = null;
            CAList = null;
        }

        public bool IsEligibleByCriteria(Features feature
            , string ca = "")
        {
            try
            {
                EligibilityCriteriaModel criteria = GetFeatureCriteria(feature);
                if (criteria == null)
                {
                    return true;
                }
                if (ca.IsValid()
                    && CAList != null
                    && CAList.Count > 0
                    && CAList.FindIndex(x => x.CA == ca) is int index)
                {
                    if (index > -1)
                    {
                        CACriteriaModel caObj = CAList[index];
                        return IsCriteriaEligible(criteria, caObj);
                    }
                }
                else if (CAList != null
                    && CAList.Count > 0)
                {
                    for (int i = 0; i < CAList.Count; i++)
                    {
                        CACriteriaModel caObj = CAList[i];
                        if (IsCriteriaEligible(criteria, caObj))
                        {
                            return true;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("[DEBUG] IsCriteriaEligible Error: " + e.Message);
            }
            return false;
        }

        private bool IsCriteriaEligible(EligibilityCriteriaModel eligibilityCriteria
            , CACriteriaModel caCriteria)
        {
            bool isOwnerTypeEligible = IsEligibleOwnerType(eligibilityCriteria, caCriteria);
            bool isCATypeEligible = IsEligibleCAType(eligibilityCriteria, caCriteria);
            bool isTarrifTypeEligible = IsEligibleTariffType(eligibilityCriteria, caCriteria);

            return isOwnerTypeEligible
                && isCATypeEligible
                && isTarrifTypeEligible;
        }

        private bool IsEligibleOwnerType(EligibilityCriteriaModel eligibilityCriteria
            , CACriteriaModel caCriteria)
        {
            if (eligibilityCriteria.OwnerType == null
                || eligibilityCriteria.OwnerType.Count == 0)
            {
                return true;
            }
            else
            {
                return caCriteria.IsOwner == eligibilityCriteria.IsOwner
                    || caCriteria.IsOwner == eligibilityCriteria.IsNonOwner;
            }
        }

        private bool IsEligibleCAType(EligibilityCriteriaModel eligibilityCriteria
            , CACriteriaModel caCriteria)
        {
            if (eligibilityCriteria.CaType == null
                || eligibilityCriteria.CaType.Count == 0)
            {
                return true;
            }
            else
            {
                return caCriteria.IsSmartMeter == eligibilityCriteria.IsSmartMeterCA
                    || caCriteria.IsNormalMeter == eligibilityCriteria.IsNormalCA
                    || caCriteria.IsRenewableEnergy == eligibilityCriteria.IsRenewableEnergyCA
                    || caCriteria.IsSMR == eligibilityCriteria.IsSelfMeterReadingCA;
            }
        }

        private bool IsEligibleTariffType(EligibilityCriteriaModel eligibilityCriteria
            , CACriteriaModel caCriteria)
        {
            if (eligibilityCriteria.TariffType == null
                || eligibilityCriteria.TariffType.Count == 0)
            {
                return true;
            }
            else
            {
                return eligibilityCriteria.TariffType.Contains(caCriteria.RateCategory);
            }
        }
    }
}