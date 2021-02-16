using System;
namespace myTNB.Mobile.SessionCache
{
    public sealed class ApplicationStatusSearchDetailCache
    {
        private static readonly Lazy<ApplicationStatusSearchDetailCache> lazy
             = new Lazy<ApplicationStatusSearchDetailCache>(() => new ApplicationStatusSearchDetailCache());
        public static ApplicationStatusSearchDetailCache Instance
        {
            get
            {
                return lazy.Value;
            }
        }
        private static GetApplicationStatusDisplay DetailDisplay;

        public ApplicationStatusSearchDetailCache() { }

        public bool ShouldSave { private set; get; }

        public void SetData(GetApplicationStatusDisplay data)
        {
            DetailDisplay = data;
            ShouldSave = true;
        }

        public GetApplicationStatusDisplay GetData()
        {
            return DetailDisplay;
        }

        public void Clear()
        {
            DetailDisplay = null;
            ShouldSave = false;
        }
    }
}