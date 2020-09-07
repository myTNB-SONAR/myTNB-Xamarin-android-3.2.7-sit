using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace myTNB
{
    public sealed class ServiceMappingManager
    {
        private static readonly Lazy<ServiceMappingManager> lazy =
            new Lazy<ServiceMappingManager>(() => new ServiceMappingManager());
        public static ServiceMappingManager Instance { get { return lazy.Value; } }

        private const string Services = "Services";
        private const string ServiceDetails = "ServiceDetails";

        /// <summary>
        /// Maps the service and its status code to language file.
        /// </summary>
        /// <param name="serviceName">Name of the service</param>
        /// <param name="statusCode">Status code returned from the Service Response</param>
        /// <returns>Mapped status code details</returns>
        public ServiceStatus GetStatusDetails(string serviceName, string statusCode)
        {
            List<ServiceStatus> statusList = GetStatusList(serviceName);
            if (statusList != null && statusList.Count > 0)
            {
                ServiceStatus serviceStatus = statusList.Find(x => x.Code == statusCode);
                if (serviceStatus != null)
                {
                    return serviceStatus;
                }
            }
            return new ServiceStatus();
        }

        private List<ServiceStatus> GetStatusList(string serviceName)
        {
            List<ServiceStatus> serviceStatusList = null;
            List<Dictionary<string, List<ServiceStatus>>> servicesList
                = GetValues<List<Dictionary<string, List<ServiceStatus>>>>(Services);
            if (servicesList != null && servicesList.Count > 0)
            {
                Dictionary<string, List<ServiceStatus>> serviceDictionary = servicesList.Find((x) =>
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

        private T GetValues<T>(string itemName) where T : new()
        {
            T valuesDictionary = new T();
            if (string.IsNullOrEmpty(itemName) || string.IsNullOrWhiteSpace(itemName))
            {
                return valuesDictionary;
            }
            try
            {
                JObject jsonObj = JObject.Parse(LanguageManager.Instance.JSONLang);
                if (jsonObj != null)
                {
                    string value = jsonObj[ServiceDetails][itemName]?.ToString() ?? string.Empty;
                    if (!string.IsNullOrEmpty(value) && !string.IsNullOrWhiteSpace(value))
                    {
                        valuesDictionary = JsonConvert.DeserializeObject<T>(value);
                    }
                }
                return valuesDictionary;
            }
            catch (Exception e)
            {
                Debug.WriteLine("DEBUG Error: ", e.Message);
            }
            return valuesDictionary;
        }
    }
}
