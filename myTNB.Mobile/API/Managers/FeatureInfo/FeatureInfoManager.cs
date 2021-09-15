using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using myTNB.Mobile.AWS.Models;
using Newtonsoft.Json;
using static myTNB.Mobile.EligibilitySessionCache;
using static myTNB.Mobile.FeatureInfoClass;

namespace myTNB.Mobile
{
    public class FeatureInfoManager
    {
        public FeatureInfoManager() { }

        private static readonly Lazy<FeatureInfoManager> lazy =
         new Lazy<FeatureInfoManager>(() => new FeatureInfoManager());

        public static FeatureInfoManager Instance
        {
            get
            {
                return lazy.Value;
            }
        }

        private List<FeatureInfo> Data { set; get; }

        public List<FeatureInfo> GetFeatureInfo()
        {

            if (Data != null)
            {
                return Data;
            }
            else
            {
                return new List<FeatureInfo>();
            }
        }

        public void SetData(GetEligibilityResponse response)
        {
            try
            {
                List<FeatureInfo> ListOfFeature = new List<FeatureInfo>();

                //Convert Enum to list
                List<string> TypeOfFeature = new List<string>() { Features.EB.ToString() };

                if (response != null
                    && response.StatusDetail != null
                    && response.StatusDetail.IsSuccess
                    && response.Content != null)
                {
                    Type content = response.Content.GetType();
                    TypeOfFeature.ToList().ForEach(features =>
                    {
                        //List of CA that are eligible
                        List<FeaturesContractAccount> eligibleCA = new List<FeaturesContractAccount>();
                        if (content != null && content.GetProperty(features.ToString()) is PropertyInfo props && props != null)
                        {
                            object obj = props.GetValue(response.Content, null);
                            if (obj != null)
                            {
                                EBModel tempData = JsonConvert.DeserializeObject<EBModel>(JsonConvert.SerializeObject(obj));
                                foreach (ContractAccountsModel i in tempData.ContractAccounts)
                                {
                                    eligibleCA.Add(new FeaturesContractAccount
                                    {
                                        contractAccount = i.ContractAccount,
                                        acted = i.Acted,
                                        modifiedDate = i.ModifiedDate.ToString()
                                    });
                                }
                                ListOfFeature.Add(
                                new FeatureInfo()
                                {
                                    FeatureName = features.ToString(),
                                    ContractAccount = eligibleCA
                                });
                            }
                        }
                    });
                }
                Data = ListOfFeature;
            }
            catch (Exception e)
            {
                Data = new List<FeatureInfo>();
            }
        }

        public void Clear()
        {
            Data = new List<FeatureInfo>();
        }
    }
}
