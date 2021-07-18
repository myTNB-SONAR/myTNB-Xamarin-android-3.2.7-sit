using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using myTNB.Mobile.AWS.Models;

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
    }
}
