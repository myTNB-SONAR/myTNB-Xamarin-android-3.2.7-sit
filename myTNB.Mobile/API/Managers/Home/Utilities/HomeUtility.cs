using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using myTNB.Mobile.API.Models.Home.PostServices;
using myTNB.Mobile.Extensions;

namespace myTNB.Mobile.API.Managers.Home.Utilities
{
    internal static class HomeUtility
    {
        internal static void UpdateResponse(ref PostServicesResponse response
            , string appVersion)
        {
            GetCleanAppVersion(ref appVersion);
            if (response != null
                && response.Data != null
                && response.Data.Content != null
                && response.Data.Content.Services != null
                && response.Data.Content.Services.Count > 0)
            {
                List<ServicesModel> services = response.Data.Content.Services;
                int servicesCount = services.Count;
                for (int i = servicesCount - 1; i > -1; i--)
                {
                    ServicesModel service = services[i];
                    if (service.ServiceType == MobileEnums.ServiceEnum.MYHOME
                        && !MyHomeUtility.Instance.IsAccountEligible)
                    {
                        services.RemoveAt(i);
                    }
                    else if (!IsServiceIncluded(appVersion, service.AppVersion))
                    {
                        services.RemoveAt(i);
                    }
                    else if (service.Children != null
                        && service.Children.Count > 0)
                    {
                        UpdateChildrenItems(ref service
                            , appVersion);
                    }
                }
                response.Data.Content.Services = services;
            }
        }

        private static void GetCleanAppVersion(ref string appVersion)
        {
            appVersion = appVersion.Replace("v", string.Empty);
            appVersion = Regex.Replace(appVersion, @"\(.*?\)", string.Empty);
            appVersion = Regex.Replace(appVersion, @"[^0-9.,]+", "");
        }

        private static void UpdateChildrenItems(ref ServicesModel serviceItem
            , string appVersion)
        {
            int childrenCount = serviceItem.Children.Count;
            for (int i = childrenCount - 1; i > -1; i--)
            {
                ServicesBaseModel child = serviceItem.Children[i];
                if (!IsServiceIncluded(appVersion, child.AppVersion))
                {
                    serviceItem.Children.RemoveAt(i);
                }
            }
        }

        private static bool IsServiceIncluded(string appVersion
            , string serviceVersion)
        {
            if (!serviceVersion.IsValid())
            {
                return true;
            }
            return IsValidVersion(appVersion, serviceVersion);
        }

        private static bool IsValidVersion(string appVersionString
            , string serviceVersionString)
        {
            Version appVersion = new Version(appVersionString);
            Version serviceVersion = new Version(serviceVersionString);

            int comparison = appVersion.CompareTo(serviceVersion);
            if (comparison > 0)
            {
                return true;
            }
            else if (comparison < 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}