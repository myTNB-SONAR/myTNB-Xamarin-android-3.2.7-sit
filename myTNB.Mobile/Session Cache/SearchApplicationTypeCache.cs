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
    }
}