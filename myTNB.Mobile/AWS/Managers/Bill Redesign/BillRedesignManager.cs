using System;

namespace myTNB.Mobile
{
    public class BillRedesignManager
    {

        private static readonly Lazy<BillRedesignManager> lazy =
           new Lazy<BillRedesignManager>(() => new BillRedesignManager());

        public static BillRedesignManager Instance
        {
            get
            {
                return lazy.Value;
            }
        }

        public BillRedesignManager() { }
    }
}