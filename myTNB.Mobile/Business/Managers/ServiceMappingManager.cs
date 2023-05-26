using System;
using System.Collections.Generic;
using System.Linq;
using myTNB.Mobile;

namespace myTNB
{
    internal sealed class ServiceMappingManager
    {
        private static readonly Lazy<ServiceMappingManager> lazy =
            new Lazy<ServiceMappingManager>(() => new ServiceMappingManager());
        internal static ServiceMappingManager Instance { get { return lazy.Value; } }

        /// <summary>
        /// Maps the service and its status code to language file.
        /// </summary>
        /// <param name="serviceName">Name of the service</param>
        /// <param name="statusCode">Status code returned from the Service Response</param>
        /// <returns>Mapped status code details</returns>
        internal StatusDetail GetStatusDetails(string serviceName
            , string statusCode)
        {
            List<StatusDetail> statusList = GetStatusList(serviceName);
            if (statusList != null && statusList.Count > 0)
            {
                StatusDetail serviceStatus = statusList.Find(x => x.Code == statusCode);
                if (serviceStatus != null)
                {
                    return serviceStatus;
                }
            }
            return new StatusDetail();
        }

        private List<StatusDetail> GetStatusList(string serviceName)
        {
            List<StatusDetail> serviceStatusList = null;
            List<Dictionary<string, List<StatusDetail>>> servicesList
                = LanguageManager.Instance.GetValues<List<Dictionary<string, List<StatusDetail>>>>(MobileConstants.LanguageFile_ServiceDetails
                    , MobileConstants.LanguageFile_Services);
            if (servicesList != null && servicesList.Count > 0)
            {
                Dictionary<string, List<StatusDetail>> serviceDictionary = servicesList.Find((x) =>
                {
                    if (x.Keys != null && x.Keys.Count > 0)
                    {
                        return x.Keys.Contains(serviceName);
                    }
                    return false;
                });
                if (serviceDictionary != null && serviceDictionary.Count > 0 && serviceDictionary.ContainsKey(serviceName))
                {
                    return serviceDictionary[serviceName];
                }
            }
            return serviceStatusList;
        }
    }
}