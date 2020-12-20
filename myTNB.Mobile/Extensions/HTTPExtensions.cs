using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace myTNB.Mobile.Extensions
{
    public static class HTTPExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="rawResponse"></param>
        /// <returns></returns>
        internal static async Task<T> ParseAsync<T>(this HttpResponseMessage rawResponse) where T : new()
        {
            T customClass = new T();
            try
            {
                string response = await rawResponse.Content.ReadAsStringAsync();
                if (response != null)
                {
                    JObject jsonObj = JObject.Parse(response);
                    JToken jToken = jsonObj["d"];
                    if (jToken != null)
                    {
                        response = jToken.ToString();
                    }
                    return JsonConvert.DeserializeObject<T>(response);
                }
            }
            catch (JsonException jex)
            {
#if DEBUG
                Debug.WriteLine("Parse JsonException Exception: {0}", jex.Message);
#endif
            }
            catch (Exception e)
            {
#if DEBUG
                Debug.WriteLine("Parse General Exception: {0}", e.Message);
#endif
            }
            return customClass;
        }
    }
}