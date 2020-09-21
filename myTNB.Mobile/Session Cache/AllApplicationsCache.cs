using System;
namespace myTNB.Mobile.SessionCache
{
    public sealed class AllApplicationsCache
    {
        private static readonly Lazy<AllApplicationsCache> lazy
            = new Lazy<AllApplicationsCache>(() => new AllApplicationsCache());

        public static AllApplicationsCache Instance
        {
            get
            {
                return lazy.Value;
            }
        }

        private AllApplicationsCache() { }

        //private int CurrentPage;

        public void SetData()
        {

        }

        public void GetData()
        {

        }

        public int Limit
        {
            set; get;
        }
    }
}
