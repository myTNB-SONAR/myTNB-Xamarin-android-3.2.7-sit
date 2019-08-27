using Newtonsoft.Json;
using UIKit;

namespace myTNB.Model.Usage
{
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
