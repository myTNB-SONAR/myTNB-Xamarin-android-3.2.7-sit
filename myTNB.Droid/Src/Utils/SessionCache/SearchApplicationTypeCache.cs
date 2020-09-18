using System;
using myTNB;

namespace myTNB_Android.Src.Utils.SessionCache
{
    internal sealed class SearchApplicationTypeCache
    {
        private static readonly Lazy<SearchApplicationTypeCache> lazy
            = new Lazy<SearchApplicationTypeCache>(() => new SearchApplicationTypeCache());
        internal static SearchApplicationTypeCache Instance
        {
            get
            {
                return lazy.Value;
            }
        }

        internal SearchApplicationTypeCache()
        {
        }

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
    }
}
