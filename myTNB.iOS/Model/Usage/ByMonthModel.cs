﻿using System.Collections.Generic;

namespace myTNB.Model.Usage
{
    public class ByMonthModel
    {
        public string Range { set; get; }
        public List<MonthItemModel> Months { set; get; }
    }

    public class MonthItemModel
    {
        public string Date { set; get; }
        public string Year { set; get; }
        public string Month { set; get; }
        public string Day { set; get; }
        public string AmountTotal { set; get; }
        public bool IsEstimatedReading { set; get; }
        public string UsageTotal { set; get; }
        public string Currency { set; get; }
        public string UsageUnit { set; get; }
        public List<TariffItemModel> tariffBlocks { set; get; }
        public bool IsCurrentlyUnavailable { set; get; }
    }

    public class TariffItemModel
    {
        public string BlockId { set; get; }
        public string Amount { set; get; }
        public string Usage { set; get; }
        public string BlockPrice { set; get; }
    }
}
