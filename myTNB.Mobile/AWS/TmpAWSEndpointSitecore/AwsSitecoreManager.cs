using System;
using Newtonsoft.Json.Linq;

namespace myTNB.Mobile
{
    public sealed class AwsSitecoreManager
    {

        private static readonly Lazy<AwsSitecoreManager> lazy =
         new Lazy<AwsSitecoreManager>(() => new AwsSitecoreManager());

        public static AwsSitecoreManager Instance
        {
            get
            {
                return lazy.Value;
            }
        }

        public  AwsSitecoreManager()
        {

        }

        private struct AwsTempConstant
        {
            internal const string AWSServiceConfiguration = "AWSServiceConfiguration";
            internal const string TokenEndpoint = "TokenEndpoint";
            internal const string EligibilityEndpoint = "EligibilityEndpoint";
        }



        public string TokenEndpoint()
        {
            JToken config = LanguageManager.Instance.GetServiceConfig(AwsTempConstant.AWSServiceConfiguration, AwsTempConstant.TokenEndpoint);
            if (config != null )
            {
                return config.ToObject<string>();
            }
            return string.Empty;
        }


        public string EgibilityEndpoint()
        {
            JToken config = LanguageManager.Instance.GetServiceConfig(AwsTempConstant.AWSServiceConfiguration, AwsTempConstant.EligibilityEndpoint);
            if (config != null)
            {
                return config.ToObject<string>();
            }
            return string.Empty;
        }







    }
}
