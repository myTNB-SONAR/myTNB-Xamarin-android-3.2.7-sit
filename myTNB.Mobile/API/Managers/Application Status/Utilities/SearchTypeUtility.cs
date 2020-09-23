using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using myTNB.Mobile.API.Models;
using myTNB.Mobile.Extensions;

namespace myTNB.Mobile.API.Managers.ApplicationStatus
{
    internal static class SearchTypeUtility
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="response"></param>
        /// <param name="roleID"></param>
        internal static void SearchApplicationTypeParser(this SearchApplicationTypeResponse response
              , string roleID)
        {
            try
            {
                if (response != null && response.Content != null && response.Content.Count > 0 && roleID.IsValid())
                {
                    List<SearchApplicationTypeModel> content = response.Content.FindAll(x => x.UserRole.Contains(roleID));

                    List<KeyValueModel> excludedList = LanguageManager.Instance
                        .GetValues<List<KeyValueModel>>(Constants.LanguageFile_Mapping
                        , Constants.LanguageFile_ExcludedApplicationTypes);
                    if (excludedList != null && excludedList.Count > 0)
                    {
                        List<string> excludedKeys = excludedList.Select(x => x.key).ToList();
                        content = content.FindAll(x => !excludedKeys.Contains(x.SearchApplicationTypeId));
                    }
                    response.Content = content;
                    //Mark: Codes were disabled as multilingual in list of all values were confirmed.
                    //response.MapContent();
                }
            }
            catch (Exception e)
            {
#if DEBUG
                Debug.WriteLine("[DEBUG] SearchApplicationTypeParser Error: " + e.Message);
#endif
            }
        }

        /*private static void MapContent(this SearchApplicationTypeResponse response)
        {
            try
            {
                List<KeyValueModel> applicationTypeList
                     = LanguageManager.Instance.GetValues<List<KeyValueModel>>(Constants.LanguageFile_Mapping, Constants.LanguageFile_ApplicationType);
                if (applicationTypeList != null && applicationTypeList.Count > 0)
                {
                    for (int i = 0; i < response.Content.Count; i++)
                    {
                        string typeID = response.Content[i].SearchApplicationTypeId;
                        response.Content[i].SearchApplicationTypeDesc = applicationTypeList.Find(x => x.key == typeID)?.value
                            ?? response.Content[i].SearchApplicationTypeDesc;
                        response.Content[i].SearchTypes.MapContent();
                    }
                }
            }
            catch (Exception e)
            {
#if DEBUG
                Debug.WriteLine("[DEBUG] MapContent Error: " + e.Message);
#endif
            }
        }

        private static void MapContent(this List<SearchType> searchTypes)
        {
            try
            {
                if (searchTypes != null && searchTypes.Count > 0)
                {
                    List<KeyValueModel> searchTypeList
                         = LanguageManager.Instance.GetValues<List<KeyValueModel>>(Constants.LanguageFile_Mapping, Constants.LanguageFile_SearchType);
                    for (int i = 0; i < searchTypes.Count; i++)
                    {
                        string typeID = searchTypes[i].SearchTypeId;
                        searchTypes[i].SearchTypeDesc = searchTypeList.Find(x => x.key == typeID)?.value
                            ?? searchTypes[i].SearchTypeDesc;
                    }
                }
            }
            catch (Exception e)
            {
#if DEBUG
                Debug.WriteLine("[DEBUG] MapContent Error: " + e.Message);
#endif
            }
        }*/
    }
}
