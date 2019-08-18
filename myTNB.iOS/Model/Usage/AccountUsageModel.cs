using System.Collections.Generic;
using Newtonsoft.Json;
using UIKit;

namespace myTNB.Model.Usage
{
    public class AccountUsageResponseModel
    {
        public AccountUsageResponseDataModel d { set; get; }
    }

    public class AccountUsageResponseDataModel : BaseModelRefresh
    {
        public AccountUsageDataModel data { set; get; }
    }

    public class AccountUsageDataModel
    {
        public ByMonthModel ByMonth { set; get; }
        public List<LegendItemModel> TariffBlocksLegend { set; get; }
    }

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
        public string IsEstimatedReading { set; get; }
        public string UsageTotal { set; get; }
        public string Currency { set; get; }
        public string UsageUnit { set; get; }
        public List<TariffItemModel> tariffBlocks { set; get; }
    }

    public class TariffItemModel
    {
        public string BlockId { set; get; }
        public string Amount { set; get; }
        public string Usage { set; get; }
        public string BlockPrice { set; get; }
    }

    public class LegendItemModel
    {
        public string BlockId { set; get; }
        public string BlockRange { set; get; }
        public string BlockPrice { set; get; }
        public RGBItemModel RGB { set; get; }
        [JsonIgnore]
        public UIColor Colour
        {
            get
            {
                UIColor colour = UIColor.White;
                if (!string.IsNullOrEmpty(RGB.R.ToString()) &&
                    !string.IsNullOrEmpty(RGB.G.ToString()) &&
                    !string.IsNullOrEmpty(RGB.B.ToString()))
                {
                    colour = UIColor.FromRGB(RGB.R, RGB.G, RGB.B);
                }
                return colour;
            }
        }
    }

    public class RGBItemModel
    {
        public int R { set; get; }
        public int G { set; get; }
        public int B { set; get; }
    }
}
