using System;
using System.Collections.Generic;
using System.Linq;

namespace myTNB.Mobile.SessionCache
{
    public sealed class SearchApplicationTypeCache
    {
        private static readonly Lazy<SearchApplicationTypeCache> lazy
            = new Lazy<SearchApplicationTypeCache>(() => new SearchApplicationTypeCache());
        public static SearchApplicationTypeCache Instance
        {
            get
            {
                return lazy.Value;
            }
        }

        public SearchApplicationTypeCache() { }

        private SearchApplicationTypeResponse searchApplicationTypeResponse;

        public void SetData(SearchApplicationTypeResponse searchApplicationTypeResponse)
        {
            if (searchApplicationTypeResponse != null)
            {
                this.searchApplicationTypeResponse = searchApplicationTypeResponse;
            }
        }

        public SearchApplicationTypeResponse GetData()
        {
            return searchApplicationTypeResponse;
        }

        public void Clear()
        {
            searchApplicationTypeResponse = null;
        }

        public bool IsEmpty
        {
            get
            {
                return searchApplicationTypeResponse == null;
            }
        }

        public List<SelectorModel> GetApplicationTypeList()
        {
            if (searchApplicationTypeResponse != null
                && searchApplicationTypeResponse.Content != null
                && searchApplicationTypeResponse.Content.Count > 0)
            {
                return searchApplicationTypeResponse.Content.Select(x => new SelectorModel
                {
                    Key = x.SearchApplicationTypeId,
                    Value = x.SearchApplicationTypeDescDisplay
                }).ToList();
            }
            return new List<SelectorModel>();
        }

        public string GetApplicationTypeDescription(string key)
        {
            if (searchApplicationTypeResponse != null
                && searchApplicationTypeResponse.Content != null
                && searchApplicationTypeResponse.Content.Count > 0)
            {
                int index = searchApplicationTypeResponse.Content.FindIndex(x => x.SearchApplicationTypeId == key);
                if (index > -1 && index < searchApplicationTypeResponse.Content.Count)
                {
                    return searchApplicationTypeResponse.Content[index].SearchApplicationTypeDescDisplay;
                }
            }

            return string.Empty;
        }

        public string GetSearchTypeDescription(string key)
        {
            if (searchApplicationTypeResponse != null
               && searchApplicationTypeResponse.Content != null
               && searchApplicationTypeResponse.Content.Count > 0)
            {
                for (int i = 0; i < searchApplicationTypeResponse.Content.Count; i++)
                {
                    if (searchApplicationTypeResponse.Content[i] is SearchApplicationTypeModel searchApplicationType
                        && searchApplicationType != null
                        && searchApplicationType.SearchTypes != null
                        && searchApplicationType.SearchTypes.Count > 0)
                    {
                        int index = searchApplicationType.SearchTypes.FindIndex(x => x.SearchTypeId == key);
                        if (index < 0)
                        {
                            continue;
                        }
                        return searchApplicationType.SearchTypes[index].SearchTypeDescDisplay;
                    }

                }
            }

            return string.Empty;
        }
    }
}