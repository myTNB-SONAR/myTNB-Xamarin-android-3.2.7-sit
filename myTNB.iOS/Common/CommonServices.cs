using System;
using System.Collections.Generic;
using Firebase.DynamicLinks;
using Foundation;

namespace myTNB
{
    public static class CommonServices
    {
        private static string GetDynamicLinkDomain(APIEnvironment environment)
        {
            string env = environment.ToString();
            return Constants.DynamicLinkDomain.ContainsKey(env) ? Constants.DynamicLinkDomain[env] : string.Empty;
        }

        public static DynamicLinkComponents GenerateLongURL(string linkStr)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>
            {
                { "Link", linkStr },
                { "BundleID", "com.mytnb.mytnb" },
                { "PackageName", "com.mytnb.mytnb" },
                { "MinimumAppVersioniOS", "2.1.0" },
                { "MinimumAppVersionAndroid", "171" },
                { "AppStoreId", "1297089591" }
            };

            APIEnvironment env = TNBGlobal.IsProduction ? APIEnvironment.PROD : APIEnvironment.SIT;
            string Dynamic_Link_Domain = GetDynamicLinkDomain(env);

            var link = NSUrl.FromString(dictionary["Link"]);
            var components = DynamicLinkComponents.Create(link, Dynamic_Link_Domain);
            var bundleId = dictionary["BundleID"];

            if (!string.IsNullOrWhiteSpace(bundleId))
            {
                var iOSParams = DynamicLinkiOSParameters.FromBundleId(bundleId);
                iOSParams.MinimumAppVersion = dictionary["MinimumAppVersioniOS"];
                iOSParams.AppStoreId = dictionary["AppStoreId"];
                components.IOSParameters = iOSParams;
            }

            var packageName = dictionary["PackageName"];

            if (!string.IsNullOrWhiteSpace(packageName))
            {
                var androidParams = DynamicLinkAndroidParameters.FromPackageName(packageName);
                androidParams.MinimumVersion = nint.Parse(dictionary["MinimumAppVersionAndroid"]);
                components.AndroidParameters = androidParams;
            }

            var options = DynamicLinkComponentsOptions.Create();
            options.PathLength = ShortDynamicLinkPathLength.Unguessable;
            components.Options = options;

            return components;
        }
    }
}
