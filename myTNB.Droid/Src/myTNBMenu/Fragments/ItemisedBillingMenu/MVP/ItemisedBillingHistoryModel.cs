using System;
using System.Collections.Generic;

namespace myTNB_Android.Src.myTNBMenu.Fragments.ItemisedBillingMenu.MVP
{
    public class ItemisedBillingHistoryModel
    {
        public string MonthYear { get; set; }
        public List<BillingHistoryData> BillingHistoryDataList { get; set;}

        public class BillingHistoryData
        {
            public string HistoryType { get; set; }
            public string DateAndHistoryType { get; set; }
            public string Amount { get; set; }
            public string DetailedInfoNumber { get; set; }
            public string PaidVia { get; set; }
        }
    }
}
